// Copyright 2011 Simon Booth
// 
// Released under GNU Public License.
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace SmartCopyTool
{
    /// <summary>
    /// Base class to perform potentially long-running operations
    /// asynchronously, with progress reporting and potentially preemptive termination.
    /// </summary>
    abstract class Worker
    {
        public enum State { IDLE, ACTIVE, COMPLETED, ABORTED, ERROR };

        // Copy off state to pass back on progress report event
        public class Report
        {
            public int percent;
            public State state;
            public string status = null;
            public TreeNode[] removedNodes = null;
            public TreeNode[] selectedNodes = null;
            public string[] warnings = null;              // Log warning here, parent thread will display them
            public TimeSpan timeTaken;

            public Report( Worker worker )
            {
                percent = worker.reportedPercent;
                state = worker.myState;
                timeTaken = worker.myTimer.Elapsed;
                if ( worker.showStatusMessage )
                {
                    lock ( worker.myStatus ) status = worker.myStatus;
                }
                if ( worker.warnings.Count > 0 )
                {
                    warnings = worker.warnings.ToArray();
                    worker.warnings.Clear();
                }
                if ( worker.nodesRemoved.Count > 0 )
                {
                    removedNodes = worker.nodesRemoved.ToArray();
                    worker.nodesRemoved.Clear();
                }
                if ( worker.nodesSelected.Count > 0 )
                {
                    selectedNodes = worker.nodesSelected.ToArray();
                    worker.nodesSelected.Clear();
                }
            }
        }

        public enum PauseState
        {
            RUNNING,
            PAUSE_REQUESTED,
            PAUSED,
            RESUME_REQUESTED,
        };

        public string operationName = null;
        public string operationText = null;
        public bool canCancel = true;
        public bool canPause = false;
        public bool showStatusMessage = false;

        protected State myState = State.IDLE;
        private string myStatus = "";                                      // Set this to describe what worker is currently doing
        private List<TreeNode> nodesRemoved = new List<TreeNode>();      // If we remove nodes, or want to, add them to this list and let main thread do it
        private List<TreeNode> nodesSelected = new List<TreeNode>();
        private List<string> warnings = new List<string>();              // Log warning here, parent thread will display them
        private int reportedPercent = -1;

        protected BackgroundWorker bgw = null;                              // Want to pass into constructor really, but can't really be sure which bgw it will be until DoWork is called

        public bool CancellationPending { get { return bgw.CancellationPending; } }

        private volatile PauseState pauseState = PauseState.RUNNING;

        protected bool PauseRequested { get { return pauseState == PauseState.PAUSE_REQUESTED; } }
        protected void DoPause()
        {
            // Urgh... should really use a WaitEvent or a Semaphore or something, but this works and resuming on cancel comes free
            myTimer.Stop();
            while ( pauseState != PauseState.RESUME_REQUESTED && !CancellationPending )
            {
                Thread.Sleep( 100 );
            }
            pauseState = PauseState.RUNNING;
            myTimer.Start();
        }

        private Stopwatch myTimer;
        private long timeTaken;

        /// <summary>
        /// Call Invoke to start the worker working (and perform some bookkeeping)
        /// </summary>
        /// <param name="bgw"></param>
        /// <returns></returns>
        public Report Invoke( BackgroundWorker bgw )
        {
            reportedPercent = -1;
            this.bgw = bgw;
            myTimer = new Stopwatch();
            myTimer.Start();
            myState = DoWork();
            return GetState();
        }

        // Subclasses should overload DoWork to do something
        protected abstract State DoWork();

        // Log a warning to the buffer
        protected void LogWarning( string warning, params object[] args )
        {
            warnings.Add( ( args != null && args.Count() > 0 ) ? String.Format( warning, args ) : warning );
        }

        // Set the operation's current status
        protected void SetStatus( string status, params object[] args )
        {
            // Always want to make a copy of the string, so call String.Format whether we have args or not
            if ( args.Length > 0 )
            {
                myStatus = String.Format( status, args );
            }
            else
            {
                // Gremlin lurking... what if our status string has curly braces?
                myStatus = String.Format( "{0}", status );
            }
        }

        // Get path relative to root
        protected string GetRelativePath( string path, string root )
        {
            string relative = path.Substring( root.Length );
            if ( relative.Length > 1 && relative[ 0 ] == Path.DirectorySeparatorChar )
            {
                relative = relative.Substring( 1 );
            }
            return relative;
        }

        // Re-root a path - i.e. get the path relative to dst instead of src
        protected string RerootPath( string path, string src, string dst )
        {
            string targetName = Path.Combine( dst, GetRelativePath( path, src ) );
            return targetName;
        }

        // Indicate that a tree node should be removed
        protected void FlagForRemoval( TreeNode node )
        {
            nodesRemoved.Add( node );
        }

        protected void SelectNode( TreeNode node )
        {
            nodesSelected.Add( node );
        }

        /// <summary>
        /// Get a report on the current state
        /// </summary>
        /// <returns></returns>
        private Report GetState()
        {
            return new Report( this );
        }

        /// <summary>
        /// Report progress to parent thread.  Always sends copy of current state.
        /// </summary>
        /// <param name="percent"></param>
        protected void ReportProgress( int percent )
        {
            if ( myTimer.ElapsedMilliseconds - timeTaken > 100 )
            {
                timeTaken = myTimer.ElapsedMilliseconds;
                reportedPercent = Math.Min( percent, 100 );
                bgw.ReportProgress( percent, new Report( this ) );                
            }
        }

        /// <summary>
        /// This is pretty ugly to be honest...
        /// </summary>
        /// <returns></returns>
        public bool RequestPause()
        {
            if ( canPause && pauseState == PauseState.RUNNING )
            {
                pauseState = PauseState.PAUSE_REQUESTED;
                return true;
            }
            return false;
        }

        public bool RequestResume()
        {
            if ( pauseState == PauseState.PAUSE_REQUESTED || pauseState == PauseState.PAUSED )
            {
                pauseState = PauseState.RESUME_REQUESTED;
                return true;
            }

            return false;
        }


        // Recursively count number of CHECKED folders and subfolders
        static internal int CountSelectedNodes( TreeNode node )
        {
            FolderData folder = (FolderData)node.Tag;
            int count = ( folder.ContainsCheckedFiles ) ? 1 : 0;
            foreach ( TreeNode child in node.Nodes )
                count += CountSelectedNodes( child );
            return count;
        }
    }

    /// <summary>
    /// Asynchronous worker to copy files from source to destination
    /// </summary>
    class FileCopier : Worker
    {
        Options options;
        List<FileData> filesToCopy;

        public FileCopier( List<FileData> filesToCopy, Options options )
            : base()
        {
            operationName = "Copying Files";
            operationText = String.Format( "Copying {0} files from {1} to {2}", filesToCopy.Count, options.sourcePath, options.targetPath );
            canCancel = true;
            canPause = true;
            showStatusMessage = true;
            this.options = options;
            this.filesToCopy = filesToCopy;
        }

        protected override State DoWork()
        {
            int total = filesToCopy.Count;

            if ( total > 0 )
            {
                int count = 0;

                DirectoryInfo src = new DirectoryInfo( options.sourcePath );
                DirectoryInfo dst = new DirectoryInfo( options.targetPath );

                foreach ( FileData file in filesToCopy )
                {
                    Debug.Assert( file.FullName.StartsWith( src.FullName ), "Walked into a folder that is not a subdirectory of root!" );

                    if ( PauseRequested )
                        DoPause();

                    if ( CancellationPending )
                        return State.ABORTED;

                    string targetName = RerootPath( file.FullName, src.FullName, dst.FullName );

                    SetStatus( GetRelativePath( file.FullName, src.FullName ) );
                    ReportProgress( ( ++count * 100 ) / total );

                    try
                    {
                        // Copy the file
                        if ( options.allowOverwrite && File.Exists( targetName ) )
                        {
                            LogWarning( "Overwriting {0}", targetName );
                            System.IO.File.Delete( targetName );
                            System.IO.File.Copy( file.FullName, targetName );
                        }
                        else if ( !File.Exists( targetName ) )
                        {
                            FileData target = new FileData( targetName, null );

                            // Create directory if it doesn't exist
                            if ( Directory.Exists( target.DirectoryName ) == false )
                            {
                                Directory.CreateDirectory( target.DirectoryName );
                            }

                            System.IO.File.Copy( file.FullName, targetName );
                        }
                        else
                        {
                            LogWarning( "Skipping {0}", targetName );
                        }

                    }
                    catch ( Exception ex )
                    {
                        file.Notes = ex.Message;
                        LogWarning( "Could not copy {0} - {1}", targetName, ex.Message );
                    }

                }
            }

            return State.COMPLETED;
        }

    }   // End of FileCopier


    /// <summary>
    /// Move selected files and folders to target directory.
    /// Massively more complicated than a copy operation because of the need to remove
    /// any empty folders left behind after all files moved.  Could just move files and
    /// then rescan to remove empty folders, but moving whole directory trees is much
    /// more efficient if the destination is on the same volume anyway.
    /// </summary>
    class FileMover : Worker
    {
        TreeView tree;
        Options options;

        private int numNodesToMove = 0;
        private int numMoved = 0;

        public FileMover( TreeView tree, Options options )
            : base()
        {
            operationName = "Moving Files";
            operationText = String.Format( "Moving selected files and folders from {0} to {1}", options.sourcePath, options.targetPath );
            canCancel = true;
            canPause = true;
            showStatusMessage = true;
            this.tree = tree;
            this.options = options;
        }

        protected override State DoWork()
        {
            // How many checked nodes are there?
            numNodesToMove = CountSelectedNodes( tree.Nodes[ 0 ] );
            numMoved = 0;

            if ( numNodesToMove == 0 )
                return State.COMPLETED;

            DirectoryInfo src = new DirectoryInfo( options.sourcePath );
            DirectoryInfo dst = new DirectoryInfo( options.targetPath );
            Debug.Assert( src != null && dst != null );

            TreeNode node = tree.Nodes[ 0 ];
            return RecursiveMoveFolder( node, src, dst );
        }

        private State RecursiveMoveFolder( TreeNode node, DirectoryInfo src, DirectoryInfo dst )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return State.ABORTED;

            FolderData folder = (FolderData)node.Tag;
            Debug.Assert( folder.FullName.StartsWith( src.FullName ), "Walked into a folder that is not a subdirectory of root!" );

            // Get relative path
            string targetName = RerootPath( folder.FullName, src.FullName, dst.FullName );
            DirectoryInfo target = new DirectoryInfo( targetName );

            // Try to move entire folder subtree
            if ( CanMoveEntireNode( node ) )
            {
                // Create directory if it doesn't exist
                if ( Directory.Exists( targetName ) == false )
                {
                    DirectoryInfo parent = Directory.GetParent( targetName );
                    if ( parent.Exists == false )
                        parent.Create();

                    // Directory.Move will fail if target is on another volume, so have to move file by file in that case.
                    // Could figure out if it's going to happen, but might as well just try it and handle the exception if it comes (might cover other issues too)
                    try
                    {
                        Directory.Move( folder.FullName, targetName );
                        numMoved += CountSelectedNodes( node );
                        FlagForRemoval( node );
                        SetStatus( "Moving {0}", folder.FullName );
                        ReportProgress( ( numMoved * 100 ) / numNodesToMove );
                        return State.COMPLETED;
                    }
                    catch ( Exception )
                    {
                        // Couldn't move the whole lot, do it file by file...                            
                    }
                }
            }

            // If we get here, the entire folder can't be moved, so move any checked children...
            TreeNode next = null;
            for ( TreeNode child = node.FirstNode; child != null; child = next )
            {
                next = child.NextNode;

                State result = RecursiveMoveFolder( child, src, dst );

                // Feed up any errors or aborts
                if ( result != State.COMPLETED )
                {
                    return result;
                }
            }

            // ... then our own files
//            if ( node.Checked )
            if ( folder.ContainsCheckedFiles )
            {
                // Create directory if it doesn't exist
                if ( Directory.Exists( targetName ) == false )   // && folder.filteredFiles.Count > 0
                {
                    Directory.CreateDirectory( targetName );
                }

                List<FileData> filesToMove = folder.GetSelectedFiles( options );
                long index = 0;
                foreach ( FileData file in filesToMove )
                {
                    Debug.Assert( file.FullName.StartsWith( src.FullName ), "Walked into a folder that is not a subdirectory of root!" );

                    SetStatus( file.FullName );
                    double progress = ( numMoved * 100 ) / numNodesToMove;
                    progress += ( ++index * 100 ) / ( filesToMove.Count * numNodesToMove );
                    ReportProgress( (int)progress );

                    if ( PauseRequested )
                        DoPause();

                    if ( CancellationPending )
                        return State.ABORTED;

                    string targetFilename = RerootPath( file.FullName, src.FullName, dst.FullName );
                   
                    try
                    {
                        if ( !File.Exists( targetFilename ) )
                        {
                            System.IO.File.Move( file.FullName, targetFilename );
                            file.Deleted = true;
                        }
                        else if ( options.allowOverwrite )
                        {
                            LogWarning( "Overwriting {0}", targetFilename );
                            System.IO.File.Delete( targetFilename );
                            System.IO.File.Move( file.FullName, targetFilename );
                            file.Deleted = true;
                        }
                        else
                        {
                            file.Notes = "Skipped";
                            LogWarning( "Skipping {0}", file.FullName );
                        }
                    }
                    catch ( Exception e )
                    {
                        file.Notes = e.Message.TrimEnd();
                        LogWarning( "Failed to move {0} - {1}", file.Name, file.Notes );
                    }

                }

                // Remove the node if no files remain in it -- creating a new set of folder data is a bit extreme, will reset any checks
                folder = new FolderData( folder.FullName );
                node.Tag = folder;

                if ( folder.GetSelectedFiles( options ) == null || folder.GetSelectedFiles( options ).Count == 0 )
                {
                    SetStatus( "Removing {0}", folder.FullName );
                    ReportProgress( ( ++numMoved * 100 ) / numNodesToMove );

                    try
                    {
                        Directory.Delete( folder.FullName );
                        FlagForRemoval( node );
                    }
                    catch ( Exception e )
                    {
                        LogWarning( "Could not remove {0} - {1}", folder.FullName, e.Message.TrimEnd( '\r', '\n' ) );
                    }
                }
            }

            // Finished moving files
            return State.COMPLETED;
        }

        /// <summary>
        /// Check whether entire folder can be moved:
        /// 1. Folder is checked
        /// 2. No files filtered out in folder
        /// 3. Can move all subfolders
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool CanMoveEntireNode( TreeNode node )
        {
            FolderData folder = (FolderData)node.Tag;

            if ( node.Checked == false || folder.HasSelectedFiles( options ) )
                return false;

            long moveChildren = 0;
            foreach ( TreeNode child in node.Nodes )
            {
                if ( CanMoveEntireNode( child ) )
                    moveChildren++;
            }

            return ( node.Nodes.Count == moveChildren );
        }

    }   // END OF FileMover

    /// <summary>
    /// Delete selected files and folders
    /// </summary>
    class FileDeleter : Worker
    {
        TreeView tree;
        Options options;

        private int numNodesToDelete = 0;
        private int numDeleted = 0;

        public FileDeleter( TreeView tree, Options options )
            : base()
        {
            operationName = "Deleting Files";
            operationText = String.Format( "Deleting selected files and folders from {0}", options.sourcePath );
            canCancel = true;
            canPause = true;
            this.tree = tree;
            this.options = options;
        }

        protected override State DoWork()
        {
            // How many checked nodes are there?
            numNodesToDelete = CountSelectedNodes( tree.Nodes[ 0 ] );
            numDeleted = 0;

            if ( numNodesToDelete == 0 )
                return State.COMPLETED;

            return DeleteSelectedFolders( tree.Nodes[ 0 ] );
        }

        private State DeleteSelectedFolders( TreeNode node )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return State.ABORTED;

            // First recursively delete any children (cache the list first)
            TreeNode[] children = new TreeNode[ node.Nodes.Count ];
            node.Nodes.CopyTo( children, 0 );

            int index = 0;
            foreach ( TreeNode child in children )
            {
                DeleteSelectedFolders( child );
            }

            FolderData folder = (FolderData)node.Tag;

            if ( folder.ContainsCheckedFiles )
            {
                // Remove any files
                int numSelected = folder.NumSelectedFiles( options );
                if ( numSelected > 0 )
                {
                    foreach ( FileData file in folder.GetSelectedFiles( options ) )
                    {
                        SetStatus( "Deleting {0}", file.FullName );
                        double progress = ( numDeleted * 100 ) / numNodesToDelete;
                        progress += ( ++index * 100 ) / ( numSelected * numNodesToDelete );
                        ReportProgress( (int)progress );

                        if ( PauseRequested )
                            DoPause();

                        if ( CancellationPending )
                            return State.ABORTED;

                        try
                        {
                            File.Delete( file.FullName );
                            file.Deleted = true;
                        }
                        catch ( Exception )
                        {
                            file.Notes = "Could not delete";
                            LogWarning( "Could not delete {0}", file.FullName );
                        }
                    }
                }
            }

            if ( node.Checked )
            {
                // And if the folder is now empty, remove it
                SetStatus( "Deleting {0}", folder.FullName );
                ReportProgress( ( ++numDeleted * 100 ) / numNodesToDelete );

                try
                {
                    Directory.Delete( folder.FullName );
                    FlagForRemoval( node );
                }
                catch ( Exception e )
                {
                    LogWarning( "Cannot delete {0} - {1}", folder.FullName, e.Message.TrimEnd( '\r', '\n' ) );
                }
                    
            }

            return State.COMPLETED;
        }
    }   // END OF FileDeleter

    /// <summary>
    /// Remove nodes from the tree if they are mirrored in target folder
    /// (or if they are unmirrored, depending on the remove type)
    /// </summary>
    class MirrorRemover : Worker
    {
        public enum RemoveType { MIRRORED, UNMIRRORED };

        TreeView tree;
        Options options;

        private int numNodes = 0;
        private int numProcessed = 0;
        private RemoveType operation = RemoveType.MIRRORED;

        public MirrorRemover( TreeView tree, Options options, RemoveType operation = RemoveType.MIRRORED )
            : base()
        {
            operationName = ( operation == RemoveType.MIRRORED ) ? "Removing Mirrored Paths" : "Removing Unmirrored Paths";
            operationText = String.Format( "Removing {0} files and folders from {1}", ( operation == RemoveType.MIRRORED ) ? "mirrored" : "unmirroed", options.sourcePath );
            canCancel = true;
            canPause = true;
            this.tree = tree;
            this.options = options;
            this.operation = operation;
        }

        protected override State DoWork()
        {
            numNodes = tree.GetNodeCount( true );
            numProcessed = 0;

            DirectoryInfo src = new DirectoryInfo( options.sourcePath );
            DirectoryInfo dst = new DirectoryInfo( options.targetPath );
            Debug.Assert( src != null && dst != null );

            if ( numNodes == 0 )
                return State.COMPLETED;

            if ( operation == RemoveType.MIRRORED )
            {
                RemoveIfMirrored( tree.Nodes[ 0 ], src, dst );
            }
            else
            {
                RemoveIfUnmirrored( tree.Nodes[ 0 ], src, dst );
            }

            return State.COMPLETED;
        }

        /// <summary>
        /// Recursively remove mirrored folders from the tree
        /// </summary>
        /// <param name="node"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        private bool RemoveIfMirrored( TreeNode node, DirectoryInfo src, DirectoryInfo dst )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return false;

            FolderData directory = (FolderData)node.Tag;
            Debug.Assert( directory.FullName.StartsWith( src.FullName ), "Walked into a folder that is not a subdirectory of root!" );

            string targetName = RerootPath( directory.FullName, src.FullName, dst.FullName );

            SetStatus( GetRelativePath( directory.FullName, src.FullName ) );
            ReportProgress( ( ++numProcessed * 100 ) / numNodes );

            FolderData targetDirectory = new FolderData( targetName );

            bool mirrored = true;

            // First try and remove all children
            // Cache the next node so we don't get an iteration fail when we remove a node
            if ( node.Nodes.Count > 0 )
            {
                TreeNode next = null;
                for ( TreeNode child = node.Nodes[ 0 ]; child != null; child = next )
                {
                    next = child.NextNode;
                    if ( false == RemoveIfMirrored( child, src, dst ) )
                    {
                        mirrored = false;
                    }
                }
            }

            if ( mirrored )
            {
                // update the file's flags
                directory.GetSelectedFiles( options );
            }

            // Remove any mirrored files and see if that leaves any unmirrored files in the list
            foreach ( FileData file in directory.GetFiles() )
            {
                if ( PauseRequested )
                    DoPause();

                if ( CancellationPending )
                    return false;

                if ( FolderData.FileIsMirroredAt( file, targetName, options ) )
                {
                    file.Removed = true;
                    file.Notes = String.Format( "Mirrored at {0}", targetName );
                }
                else if ( !file.Filtered && !file.Removed )
                    mirrored = false;
            }

            // If no unmirrored subdirectories or files left, remove it
            if ( mirrored )
            {
                FlagForRemoval( node );
            }

            return mirrored;
        }

        /// <summary>
        /// Recursively remove unmirrored folders from the tree
        /// </summary>
        /// <param name="node"></param>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns>true if the directory was removed</returns>
        private bool RemoveIfUnmirrored( TreeNode node, DirectoryInfo src, DirectoryInfo dst )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return false;

            FolderData directory = (FolderData)node.Tag;
            Debug.Assert( directory.FullName.StartsWith( src.FullName ), "Walked into a folder that is not a subdirectory of root!" );

            string targetName = RerootPath( directory.FullName, src.FullName, dst.FullName );

            SetStatus( GetRelativePath( directory.FullName, src.FullName ) );
            ReportProgress( ( ++numProcessed * 100 ) / numNodes );

            bool unmirrored = true;

            // Does the folder exist at all?
            if ( Directory.Exists( targetName ) == false )
            {
                FlagForRemoval( node );
            }
            else
            {
                FolderData targetDirectory = new FolderData( targetName );

                // First try and remove all children
                // Cache the next node so we don't get an iteration fail when we remove a node
                if ( node.Nodes.Count > 0 )
                {
                    TreeNode next = null;
                    for ( TreeNode child = node.Nodes[ 0 ]; child != null; child = next )
                    {
                        next = child.NextNode;
                        if ( false == RemoveIfUnmirrored( child, src, dst ) )
                        {
                            unmirrored = false;
                        }
                    }
                }

                if ( unmirrored )
                {
                    // update the file's flags
                    directory.GetSelectedFiles( options );
                }

                // remove all unmirrored files
                foreach ( FileData file in directory.GetFiles() )
                {
                    if ( PauseRequested )
                        DoPause();

                    if ( CancellationPending )
                        return false;

                    if ( false == FolderData.FileIsMirroredAt( file, targetName, options ) )
                    {
                        file.Removed = true;
                        file.Notes = String.Format( "Unmirrored at {0}", targetName );
                    }
                    else if ( !file.Filtered && !file.Removed )
                        unmirrored = false;
                }

                // If there are no files or subdirectories left, the folder is mirrored
                if ( unmirrored )
                {
                    FlagForRemoval( node );
                }
            }

            return unmirrored;
        }

    }   // End of MirrorRemover


    /// <summary>
    /// Build directory tree asynchronously
    /// </summary>
    class TreeBuilder : Worker
    {
        TreeView tree;
        Options options;

        public delegate TreeNodeCollection AddDelegate( TreeNodeCollection parent, DirectoryInfo path, Options options );
        private AddDelegate myDelegate = AddFolderToTree;

        public TreeBuilder( TreeView tree, Options options )
            : base()
        {
            operationName = "Scanning Directories";
            operationText = String.Format( "Scanning directory structure of {0}", options.sourcePath );
            canCancel = true;
            canPause = true;
            this.tree = tree;
            this.options = options;
        }

        protected override State DoWork()
        {
            return AddDirectory( options.sourcePath, tree.Nodes );
        }

        private State AddDirectory( string fullpath, TreeNodeCollection parent )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return State.ABORTED;

            DirectoryInfo path = new DirectoryInfo( fullpath );

            SetStatus( fullpath );
            ReportProgress( -1 );

            try
            {
                // Try this before we add the node, incase there's an exception on access
                DirectoryInfo[] paths = path.GetDirectories();
                Array.Sort( paths, new Comparison<DirectoryInfo>( (a,b) => a.Name.CompareTo( b.Name ) ) );

                TreeNodeCollection nodes = (TreeNodeCollection)tree.Invoke( myDelegate, parent, path, options );

                foreach ( DirectoryInfo dir in paths.OrderBy( dir => path.Name ) )
                {
                    if ( options.includeHidden || ( ( dir.Attributes & FileAttributes.Hidden ) == 0 ) )
                    {
                        State result = AddDirectory( dir.FullName, nodes );
                        if ( result != State.COMPLETED )
                            return result;
                    }
                }

            }
            catch ( Exception e )
            {
                LogWarning( "Skipped {0} - {1}", path.FullName, e.Message.TrimEnd( 'r', 'n' ) );
            }

            return State.COMPLETED;
        }

        /// <summary>
        /// Delegate method to add the node to the tree so that it can be invoked in the parent thread
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="path"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static TreeNodeCollection AddFolderToTree( TreeNodeCollection parent, DirectoryInfo path, Options options )
        {
            TreeNode node = parent.Add( path.Name );
            node.Tag = new FolderData( path );
            if ( node.Parent == null )
            {
                node.Expand();
            }
            return node.Nodes;
        }

    }   // End of TreeBuilder


    /// <summary>
    /// Write filenames of all selected files to a text file.
    /// Doing this recursively is a bit excessive, but could potentially take a while over a network I suppose.
    /// </summary>
    class SelectionWriter : Worker
    {
        TreeView tree;
        Options options;

        private int numNodesToWrite = 0;
        private int numWritten = 0;
        private string filename = null;
        private bool bExtended = false;

        public SelectionWriter( TreeView tree, string filename, bool bExtended, Options options )
            : base()
        {
            operationName = "Saving Selection";
            operationText = String.Format( "Saving Selection to {0}", filename );
            canCancel = true;
            canPause = true;
            showStatusMessage = true;
            this.options = options;
            this.tree = tree;
            this.filename = filename;
            this.bExtended = bExtended;
        }

        protected override State DoWork()
        {
            State result = State.COMPLETED;

            // How many checked nodes are there?
            numNodesToWrite = CountSelectedNodes( tree.Nodes[ 0 ] );
            numWritten = 0;

            if ( numNodesToWrite == 0 )
                return State.COMPLETED;

            try
            {
                using ( StreamWriter textfile = new StreamWriter( filename, false, Encoding.UTF8 ) )
                {
                    if ( bExtended )
                    {
                        textfile.WriteLine( "#SMARTCOPY" );
                    }

                    result = WriteSelection( tree.Nodes[ 0 ], textfile );
                }
            }
            catch ( Exception ex )
            {
                LogWarning( "Failed to write file - {0}", ex.Message );
                result = State.ERROR;
            }

            return result;
        }

        private State WriteSelection( TreeNode node, StreamWriter textfile )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return State.ABORTED;

            FolderData folder = (FolderData)node.Tag;

            string basepath = options.sourcePath;
            Debug.Assert( basepath != null );

            if ( basepath[basepath.Length - 1] != Path.DirectorySeparatorChar )
                basepath += Path.DirectorySeparatorChar;

            if ( node.Checked )
            {
                SetStatus( "{0}", folder.FullName );
                ReportProgress( ( ++numWritten * 100 ) / numNodesToWrite );

                if ( bExtended )
                {
                    textfile.WriteLine( "" );
                    string line = folder.FullName;
                    if ( line.StartsWith( basepath ) )
                        line = line.Substring( basepath.Length );

                    textfile.WriteLine( String.Format( "#NODE {0}", line ) );                    
                }
            }

            if ( folder.ContainsCheckedFiles )
            {
                foreach ( FileData file in folder.GetSelectedFiles( options ) )
                {
                    string line = file.FullName;
                    if ( bExtended && line.StartsWith( basepath ) )
                    {
                        // Store relative paths
                        line = line.Substring( basepath.Length );
                    }
                    textfile.WriteLine( line );
                }
            }

            foreach ( TreeNode child in node.Nodes )
            {
                WriteSelection( child, textfile );
            }

            return State.COMPLETED;
        }

    } // End of SelectionWriter

    /// <summary>
    /// Restore selected files & folders from a list of filenames in a text file.  Assumes we're starting off with a blank slate (i.e. no selected nodes)
    /// </summary>
    class SelectionReader : Worker
    {
        TreeView tree;
        Options options;

        private int numNodesToSelect = 0;
        private int numFilesToSelect = 0;
        private int numSelected = 0;
        private string filename = null;

        public SelectionReader( TreeView tree, string filename, Options options )
            : base()
        {
            operationName = "Restoring Selection";
            operationText = String.Format( "Restoring selection from {0}", filename );
            canCancel = true;
            canPause = true;
            showStatusMessage = true;
            this.options = options;
            this.tree = tree;
            this.filename = filename;
        }

        protected override State DoWork()
        {
            State result = State.COMPLETED;

            try
            {
                // Parse the file first to build up a list of nodes and files to select
                List<string> sFiles = new List<string>();
                List<string> sNodes = new List<string>();

                using ( StreamReader textfile = new StreamReader( filename, true ) )
                {
                    while ( false == textfile.EndOfStream )
	                {
                        string line = textfile.ReadLine();

                        if ( line.StartsWith( "#NODE " ) )
                        {
                            line = line.Substring( "#NODE ".Length );
                            if ( false == Path.IsPathRooted( line ) )
                            {
                                line = Path.Combine( options.sourcePath, line );
                            }
                            sNodes.Add( line );
                        }
                        else if ( line.Length == 0 || line.StartsWith( "#" ) )
                        {
                            continue;
                        }
                        else
                        {
                            if ( false == Path.IsPathRooted( line ) )
                            {
                                line = Path.Combine( options.sourcePath, line );
                            }
                            sFiles.Add( line );
                        }
	                }
                }

                string[] files = sFiles.ToArray();
                string[] nodes = sNodes.ToArray();

                // Sorting now makes searching easier later
                Array.Sort( files );
                Array.Sort( nodes );

                numNodesToSelect = nodes.Length;
                numFilesToSelect = files.Length;

                result = RestoreSelection( tree.Nodes[ 0 ], nodes, files );
            }
            catch ( Exception ex )
            {
                LogWarning( "Failed to restore selection - {0}", ex.Message );
                result = State.ERROR;
            }

            return result;
        }

        private State RestoreSelection( TreeNode node, string[] nodes, string[] files )
        {
            if ( PauseRequested )
                DoPause();

            if ( CancellationPending )
                return State.ABORTED;

            FolderData folder = (FolderData)node.Tag;

            SetStatus( "{0}", folder.FullName );

            folder.ContainsCheckedFiles = false;
            folder.ContainsUncheckedFiles = false;

            foreach ( FileData file in folder.GetFiles() )
            {
                if ( Array.BinarySearch( files, file.FullName ) >= 0 )
                {
                    file.Checked = true;
                    ReportProgress( ( ++numSelected * 100 ) / ( numNodesToSelect + numFilesToSelect ) );
                }
                else
                {
                    file.Checked = false;
                }
            }

            // If the file contained extended NODE tags, restore them
            if ( nodes != null && nodes.Length > 0 )
            {
                if ( Array.BinarySearch( nodes, folder.FullName ) >= 0 )
                {
                    SelectNode( node );
                    ReportProgress( ( ++numSelected * 100 ) / ( numNodesToSelect + numFilesToSelect ) );
                }
            }
            else
            {
                // Otherwise, select any node with checked files, and its parents
                if ( folder.ContainsCheckedFiles )
                {
                    TreeNode n = node;
                    while ( n != null )
                    {
                        SelectNode( n );
                        n = n.Parent;
                    }
                }
            }

            // Children next
            foreach ( TreeNode child in node.Nodes )
            {
                State result = RestoreSelection( child, nodes, files );

                if ( result != State.COMPLETED )
                    return result;
            }

            return State.COMPLETED;
        }

    } // End of SelectionWriter
}

