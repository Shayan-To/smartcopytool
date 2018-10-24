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
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

///<summary>
/// Tool for selectively copying or moving parts of large directory structures.
/// e.g. copying a selection of mp3s from main library to a portable device.
/// e.g. updating a backup copy or synchronising contents of two directory trees
/// e.g. splitting a large collection of files into different folders/volumes.
/// 
/// Main functions: 
/// Scan directory tree, select branches and copy/move/delete the files within.
/// Filter out files and folders which already exist (are mirrored) in a target directory.
/// Set filename filters using simple wildcards * and ? to include or exclude particular files
/// Search for files which are orphaned in target directory - i.e. are not present in source directory.
/// 
/// TODO:  Tri-state checkboxes for folders, implement logic that folder is checked iff has checked children
/// TODO:  Toolbar for the options, rather than menu
/// TODO:  Context menu for files AND folders, with rename and delete + 'make source directory' (for folders only)
/// TODO:  Open files in the shell
/// TODO:  Change filters & removing mirrored/unmirrored files to one-shot operations that just check or uncheck files & folders?
/// TODO:  Localization?
namespace SmartCopyTool
{
    public partial class Form1 : Form
    {
        private Log myLog = new Log();
        private FormProgressDialog myProgress = null;

        private Options myOptions = null;
        private string mySettingsfile = Path.Combine( Path.GetDirectoryName( Application.ExecutablePath ), "SmartCopy.ini" );

        private int marqueeProgress = 0;

        private SmartCopyStatusBar myStatusBar = null;

        private Worker myWorker = null;

        private bool bCheckNodeChildren = true;


        /// <summary>
        /// Application Initialisation.
        /// Set start directory and build folder tree
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            myLog.SetTextbox( textBox1 );
            myLog.Write( "Initialising..." );

            if (File.Exists(mySettingsfile))
            {
                // Try to load settings file
                myLog.Write("Loading settings from " + mySettingsfile);
                try
                {
                    myOptions = Options.Load(mySettingsfile);
                }
                catch (Exception e)
                {
                    myLog.Write("Could not load settings file ({0})... using default settings", e.Message.TrimEnd('r', 'n'));
                    myOptions = null;
                }
            }

            // Set default options if it failed
            if ( myOptions == null )
            {
                myOptions = new Options();
                myOptions.includeHidden = menuIncludeHidden.Checked;
                myOptions.ignoreSize = menuIgnoreSize.Checked;
                myOptions.ignoreExtension = menuIgnoreExtension.Checked;
                myOptions.allowOverwrite = menuAllowOverwrite.Checked;
                myOptions.showFilteredFiles = menuShowFilteredFiles.Checked;
                myOptions.autoselectFilesOnRestore = menuAutoselectFiles.Checked;
                myOptions.windowSize = this.Size;
                myOptions.windowLocation = this.Location;
                myOptions.columnSizes = new int[ fileListView.Columns.Count ];
                for ( int i = 0; i < fileListView.Columns.Count; i++ )
                {
                    myOptions.columnSizes[ i ] = fileListView.Columns[ i ].Width;
                }
            }

            // Create the status bar
            myStatusBar = new SmartCopyStatusBar( statusStrip1, directoryTree, myOptions );

            // Restore other options
            menuIncludeHidden.Checked = myOptions.includeHidden;
            menuIgnoreSize.Checked = myOptions.ignoreSize;
            menuIgnoreExtension.Checked = myOptions.ignoreExtension;
            menuAllowOverwrite.Checked = myOptions.allowOverwrite;
            menuShowFilteredFiles.Checked = myOptions.showFilteredFiles;
            menuAutoselectFiles.Checked = myOptions.autoselectFilesOnRestore;
            if ( myOptions.columnSizes != null && myOptions.columnSizes.Count() == fileListView.Columns.Count )
            {
                for ( int i = 0; i < fileListView.Columns.Count; i++ )
                {
                    fileListView.Columns[ i ].Width = myOptions.columnSizes[ i ];
                }                
            }

            SetFolderInfoSorting( myOptions.SortColumn, myOptions.SortOrder );

            Application.Idle += new EventHandler( Form1_Idle );
        }

        /// <summary>
        /// Event handler for when form is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load( object sender, EventArgs e )
        {
            // Restore window size and location
//            myLog.Write( "Stored: Location {0}, Size {1}", myOptions.windowLocation.ToString(), myOptions.windowSize.ToString() );
            this.Size = myOptions.windowSize;
            this.Location = myOptions.windowLocation;
        }

        /// <summary>
        /// When application is idle, check if there is a selected source directory
        /// and if not, force the user to choose one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Idle( object sender, System.EventArgs e )
        {
            if ( directoryTree != null && directoryTree.Nodes.Count == 0 && OperationInProgress() == false )
            {
                if ( SelectSourceFolder() == DialogResult.OK )
                {
                    myLog.Write( "Source Path: " + myOptions.sourcePath );
                    myLog.Write( "Number of folders: " + directoryTree.GetNodeCount( true ) );
                }
                else
                {
                    myLog.Write( "Exiting..." );
                    Close();
                }
            }
        }

        /// <summary>
        /// Catch a close event and save the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing( object sender, FormClosingEventArgs e )
        {
            // Kill the status bar thread
            try
            {
                if ( myStatusBar != null )
                {
                    myStatusBar.Terminate();
                }
            }
            catch ( Exception ex )
            {
                myLog.Write( "Terminated status bar... {0}", ex.Message );
            }

            myLog.Write( "Saving settings to " + mySettingsfile );

            // Remember window size & position
            myOptions.windowLocation = this.Location;
            myOptions.windowSize = this.Size;

            // Store column widths here
            myOptions.columnSizes = new int[ fileListView.Columns.Count ];
            for ( int i = 0; i < fileListView.Columns.Count; i++ )
            {
                myOptions.columnSizes[ i ] = fileListView.Columns[ i ].Width;
            }

            try
            {
                myOptions.Save( mySettingsfile );
            }
            catch ( Exception ex )
            {
                MessageBox.Show( String.Format( "Error saving settings file\r\n{0}", ex.Message ), "Error", MessageBoxButtons.OK );
            }

            // Cancel any ongoing operations
            if ( OperationInProgress() )
            {
                AddOperationEndHandler( bw_EventHandlerCloseForm );
                backgroundWorker1.CancelAsync();
                e.Cancel = true;
                myLog.Write( "Waiting for operation to cancel..." );
            }
            else
            {
                myLog.Write( "Exiting..." );
            }
        }

        private void aboutThisSoftwareToolStripMenuItem_Click( object sender, EventArgs e )
        {
            new AboutBox1().ShowDialog();
        }

        /// <summary>
        /// Show folder contents in top-right pane
        /// </summary>
        /// <param name="dir"></param>
        private void DisplayFolderContents( TreeNode node )
        {
            if ( node == null || node.Tag == null )
                return;

            fileListView.Items.Clear();
            fileListView.BeginUpdate();

            // Turn off sorting while we populate the list
            System.Collections.IComparer sorter = fileListView.ListViewItemSorter;
            fileListView.ListViewItemSorter = null;

            FolderData dir = node.Tag as FolderData;

            if ( dir != null && dir.GetFiles() != null )
            {
                long size = 0;

                // Need to update the filtered files list
                dir.GetSelectedFiles( myOptions );

                foreach ( FileData file in dir.GetFiles() )
                {
                    if ( file.Deleted || file.Removed )
                        continue;

                    if ( file.Filtered && myOptions.showFilteredFiles == false )
                        continue;

                    string[] columns = new string[] { file.Name, HumanReadable.Size( file.Length ), file.CreationTime.ToString(), file.Notes == null ? "" : file.Notes };
                    ListViewItem item = new ListViewItem( columns );
                    item.Tag = file;
                    item.Checked = file.Checked;
                    item.ForeColor = GetFileColour( file );
                    fileListView.Items.Add( item ); // Don't add until it's fully initialised to avoid rogue events

                    if ( file.Selected )
                    {
                        size += file.Length;                        
                    }
                }

                myStatusBar.CurrentFolder = dir;
                myStatusBar.CurrentFolderSize = size;
            }
            else
            {
                myStatusBar.CurrentFolder = null;
                myStatusBar.CurrentFolderSize = -1;
            }

            fileListView.ListViewItemSorter = sorter;
            fileListView.EndUpdate();
            fileListView.Update();          // Encourage a render
        }

        /// <summary>
        /// Get colour for a file
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Color GetFileColour( FileData file )
        {
            if ( file.Hidden )
            {
                return ( myOptions.includeHidden && file.Selected ) ? Color.Brown : Color.RosyBrown;
            }
            else if ( file.Filtered || file.Removed )
            {
                return Color.SlateBlue;
            }
            else if ( !file.Checked )
            {
                return Color.Gray;
            }
            else
            {
                return Color.Black;
            }
        }


        /// <summary>
        /// Activate a folder selection dialog.
        /// If a folder is selected, scan it and build a tree
        /// </summary>
        /// <returns></returns>
        private DialogResult SelectSourceFolder()
        {
            folderBrowserDialog1.SelectedPath = myOptions.sourcePath;
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if ( result == DialogResult.OK )
            {
                myOptions.sourcePath = folderBrowserDialog1.SelectedPath;
                BuildDirectoryTree( myOptions, true );
            }

            return result;
        }


        /// <summary>
        /// Populate the tree recursively
        /// </summary>
        /// <param name="folder"></param>
        public DialogResult BuildDirectoryTree( Options options, bool bRunInBackground = false )
        {
            DialogResult result = DialogResult.OK;

            DirectoryInfo root = new DirectoryInfo( options.sourcePath );

            if ( root != null )
            {
                directoryTree.Nodes.Clear();

                if ( bRunInBackground )
                {
                    PerformBackgroundOperation( new TreeBuilder( directoryTree, myOptions ) );
                    AddOperationEndHandler( bw_EventHandlerExpandRoot );
                }
                else
                {
                    result = PerformLongOperation( new TreeBuilder( directoryTree, myOptions ) );
                    if ( directoryTree.Nodes.Count > 0 )
                    {
                        TreeNode node = directoryTree.Nodes[ 0 ];
                        directoryTree.SelectedNode = node;
                        DisplayFolderContents( node );
                        node.Expand();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Recursively build list of all filtered files
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private List<FileData> GetSelectedFiles( TreeNode node )
        {
            FolderData folder = (FolderData)node.Tag;

            List<FileData> files;
            if ( folder.NumSelectedFiles( myOptions ) > 0 )
            {
                files = new List<FileData>( folder.GetSelectedFiles( myOptions ) );
            }
            else
            {
                files = new List<FileData>();
            }

            // Children might be checked even when parent is not
            foreach ( TreeNode child in node.Nodes )
            {
                files = files.Concat( GetSelectedFiles( child ) ).ToList();
            }

            return files;
        }

        /// <summary>
        /// Remove all files/folders which exist and are identical in the target directory from the tree
        /// Really should be an asynchronous function...
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private DialogResult RemoveMirroredPaths( bool bRunInBackground = false )
        {
            DialogResult result = DialogResult.OK;

            TreeNode root = directoryTree.Nodes[ 0 ];
            myLog.Write( "Remove folders from tree if mirrored in " + myOptions.targetPath );

            if ( bRunInBackground )
            {
                PerformBackgroundOperation( new MirrorRemover( directoryTree, myOptions, MirrorRemover.RemoveType.MIRRORED ) );
                AddOperationEndHandler( bw_EventHandlerExpandRoot );
            }
            else
            {
                result = PerformLongOperation( new MirrorRemover( directoryTree, myOptions, MirrorRemover.RemoveType.MIRRORED ) );

                if ( directoryTree.Nodes.Count > 0 )
                {
                    DisplayFolderContents( directoryTree.SelectedNode );
                    myStatusBar.ScanRequest = true;
                }
                else
                {
                    myLog.Write( "No unmirrored folders!" );
                    // Never really want to remove the root, do we?
                    // Reference counting to the rescue!
                    //directoryTree.Nodes.Add( root );
                    //directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                }
            }

            return result;
        }

        /// <summary>
        /// Remove all files/folders which do not exist or are not identical in the target directory from the tree
        /// Really should be an asynchronous function...
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private DialogResult RemoveUnmirroredPaths( bool bRunInBackground = false )
        {
            DialogResult result = DialogResult.OK;

            TreeNode root = directoryTree.Nodes[ 0 ];
            myLog.Write( "Remove folders from tree if unmirrored in " + myOptions.targetPath );

            if ( bRunInBackground )
            {
                PerformBackgroundOperation( new MirrorRemover( directoryTree, myOptions, MirrorRemover.RemoveType.UNMIRRORED ) );
                AddOperationEndHandler( bw_EventHandlerExpandRoot );
            }
            else
            {
                result = PerformLongOperation( new MirrorRemover( directoryTree, myOptions, MirrorRemover.RemoveType.UNMIRRORED ) );

                if ( directoryTree.Nodes.Count > 0 )
                {
                    DisplayFolderContents( directoryTree.SelectedNode );
                    myStatusBar.ScanRequest = true;
                }
                else
                {
                    myLog.Write( "No mirrored folders!" );
                    // Never really want to remove the root, do we?
                    // Reference counting to the rescue!
                    //directoryTree.Nodes.Add( root );
                    //directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                }

            }

            return result;
        }

        /// <summary>
        /// Copy selected files and folders to target directory
        /// </summary>
        private DialogResult CopySelectedFolders( bool bRunInBackground = false )
        {
            DialogResult result = DialogResult.OK;

            List<FileData> filesToCopy = GetSelectedFiles( directoryTree.Nodes[ 0 ] );

            if ( filesToCopy.Count > 0 )
            {
                myLog.Write( "Copying {0} files to {1}", filesToCopy.Count, myOptions.targetPath );
                if ( bRunInBackground )
                {
                    PerformBackgroundOperation( new FileCopier( filesToCopy, myOptions ) );
                }
                else
                {
                    result = PerformLongOperation( new FileCopier( filesToCopy, myOptions ) );
                }
            }
            else
            {
                MessageBox.Show( "No files selected, nothing to copy!", "Cannot comply", MessageBoxButtons.OK );
            }

            return result;
        }

        /// <summary>
        /// Move selected files and folders to target directory.
        /// Massively more complicated than a copy operation because of the need to remove
        /// any empty folders left behind after all files moved.  Could just move files and
        /// then rescan to remove empty folders, but moving whole directory trees is much
        /// more efficient if the destination is on the same volume anyway.
        /// </summary>
        private DialogResult MoveSelectedFolders( bool bRunInBackground = false )
        {
            DialogResult result = DialogResult.OK;

            DirectoryInfo src = new DirectoryInfo( myOptions.sourcePath );
            DirectoryInfo dst = new DirectoryInfo( myOptions.targetPath );
            Debug.Assert( src != null && dst != null );

            // Check whether destination is a subdirectory of source (or indeed, source itself)
            // There are occasions when it might be quite useful to permit it, e.g. moving a selection of subfolders
            // into a different subfolder, but would have to check whether destination was due to be moved... which is
            // probably not that difficult really (worst case would be to scan the whole tree)
            if ( dst.FullName == src.FullName )
            {

                // Should really check for moving a directory tree into itself or any of its subfolders
                MessageBox.Show( "Cannot move folder into its own path!", "Error!", MessageBoxButtons.OK );
                return result;
            }

            if ( dst.FullName.StartsWith(src.FullName + "\\") )
            {
                MessageBox.Show("Destination is in same path as source, some files and folders may not be moved.", "Warning!", MessageBoxButtons.OK);
            }
            
            if ( GetSelectedFiles( directoryTree.Nodes[0] ).Count == 0 )       // This could take a while, is it worth it just to show the warning?
            {
                MessageBox.Show( "No files selected, nothing to move!", "Cannot comply", MessageBoxButtons.OK );                
            }
            else
            {
                myLog.Write( "Moving folders from {0} to {1}", myOptions.sourcePath, myOptions.targetPath );

                // Run the asynchronous worker
                if ( bRunInBackground )
                {
                    PerformBackgroundOperation( new FileMover( directoryTree, myOptions ) );
                }
                else
                {
                    result = PerformLongOperation( new FileMover( directoryTree, myOptions ) );
                }

                // Update the view...
                DisplayFolderContents( directoryTree.SelectedNode );
                myStatusBar.ScanRequest = true;
            }

            return result;
        }

        /// <summary>
        /// Delete all selected folders and files
        /// Let's implement this one recursively
        /// </summary>
        public DialogResult DeleteSelectedFolders( TreeNode node )
        {
            if ( GetSelectedFiles( directoryTree.Nodes[ 0 ] ).Count == 0 )       // This could take a while, is it worth it just to show the warning?
            {
                MessageBox.Show( "No files selected, nothing to delete!", "Cannot comply", MessageBoxButtons.OK );
                return DialogResult.OK;
            }
            else
            {
                DialogResult result = PerformLongOperation( new FileDeleter( directoryTree, myOptions ) );

                // Update the view...
                DisplayFolderContents( directoryTree.SelectedNode );
                myStatusBar.ScanRequest = true;
                return result;
            }
        }

        /// <summary>
        /// Find files in the destination which do not exist in the source tree.
        /// Will change the current directory to the target directory.
        /// Should we ignore checkboxes in the source?  Obviously by definition paths
        /// that don't exist won't be checked, but their parents might be...
        /// </summary>
        public DialogResult FindOrphansInTarget()
        {
            // Swap source & target directories
            string temp = myOptions.sourcePath;
            myOptions.sourcePath = myOptions.targetPath;
            myOptions.targetPath = temp;

            // Lazy implementation - build folder tree for target then remove if mirrored
            DialogResult result = BuildDirectoryTree( myOptions, false );
            if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
            {
                result = RemoveMirroredPaths();                
            }

            return result;
        }

        /// <summary>
        /// Copy any selected, unmirrored files into the target directory, then
        /// delete any files in the target which are not mirrored in the source.
        /// Composite operation, but common enough to make a one-click operation.
        /// </summary>
        public void SynchroniseTargetDirectory()
        {
            DialogResult result = DialogResult.OK;

            // Copy unmirrored files into target directory
            result = RemoveMirroredPaths();

            if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
            {
                // Select the whole tree... selective synchronisation is complicated by the need
                // to delete files which weren't just not there in source, but were not selected...
                directoryTree.Nodes[ 0 ].Checked = true;

                result = CopySelectedFolders( false );

                if ( result == DialogResult.OK )
                {
                    // Swap source & target directories
                    string temp = myOptions.sourcePath;
                    myOptions.sourcePath = myOptions.targetPath;
                    myOptions.targetPath = temp;

                    result = BuildDirectoryTree( myOptions, false );
                    if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
                    {
                        result = RemoveMirroredPaths();

                        // If there are any orphaned files, delete them
                        if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
                        {
                            directoryTree.Nodes[ 0 ].Checked = true;
                            DeleteSelectedFolders( directoryTree.Nodes[ 0 ] );
                        }
                    }

                    // Flip back to the source directory and rebuild the tree
                    myOptions.targetPath = myOptions.sourcePath;
                    myOptions.sourcePath = temp;
                    result = BuildDirectoryTree( myOptions, false );
                }
            }
        }

        /// <summary>
        /// Update target directory... copy any files in the source that are not found in the target
        /// </summary>
        public void UpdateTargetDirectory()
        {
            DialogResult result = DialogResult.OK;

            // Copy unmirrored files into target directory
            result = RemoveMirroredPaths();

            if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
            {
                // Select the whole tree... selective synchronisation is complicated by the need
                // to delete files which weren't just not there in source, but were not selected...
                directoryTree.Nodes[ 0 ].Checked = true;

                result = CopySelectedFolders( false );
            }
        }

        /// <summary>
        /// Merge two directories... copy any files not in one into the other.
        /// </summary>
        public void MergeDirectories()
        {
            DialogResult result = DialogResult.OK;

            // Copy unmirrored files into target directory
            result = RemoveMirroredPaths();

            if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
            {
                // Select the whole tree... 
                directoryTree.Nodes[ 0 ].Checked = true;

                result = CopySelectedFolders( false );

                // If there are any orphaned files, copy them back across
                if ( result == DialogResult.OK )
                {
                    // Swap source & target directories
                    string temp = myOptions.sourcePath;
                    myOptions.sourcePath = myOptions.targetPath;
                    myOptions.targetPath = temp;

                    result = BuildDirectoryTree( myOptions, false );
                    if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
                    {
                        result = RemoveMirroredPaths();
                        if ( result == DialogResult.OK && directoryTree.Nodes.Count > 0 )
                        {
                            directoryTree.Nodes[ 0 ].Checked = true;
                            CopySelectedFolders();
                        }
                    }

                    // Flip back to the source directory and rebuild the tree
                    myOptions.targetPath = myOptions.sourcePath;
                    myOptions.sourcePath = temp;
                    result = BuildDirectoryTree( myOptions, false );
                }
            }
        }


        /// <summary>
        /// Item in the tree has been selected (clicked on)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treenode_Selected( object sender, TreeViewEventArgs e )
        {
            TreeNode node = e.Node;
            FolderData dir = (FolderData)node.Tag;
            if ( dir != null )
            {
                DisplayFolderContents( node );
            }
        }

        /// <summary>
        /// Toggle checked state on double-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directoryTree_DoubleClick( object sender, EventArgs e )
        {
            TreeNode node = directoryTree.SelectedNode;
            if ( node != null )
            {
                node.Expand();
                node.Checked = !node.Checked;
            }
        }

        /// <summary>
        /// Checked state is passed on to children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void directoryTree_AfterCheck( object sender, TreeViewEventArgs e )
        {
            if ( bCheckNodeChildren )
            {
                TreeNode node = e.Node;
                bool bChecked = node.Checked;

                foreach ( TreeNode child in node.Nodes )
                {
                    // Check for equality to prevent spurious events
                    if ( child.Checked != bChecked )
                    {
                        child.Checked = bChecked;
                    }
                }

                // OK, check/uncheck all files too...
                FolderData folder = (FolderData)node.Tag;
                var files = folder.GetFiles().ToArray();
                foreach ( FileData file in files )
                {
                    if ( file.Checked != bChecked )
                    {
                        // Don't change the state of files that have been filtered, removed or deleted
                        if ( file.Filtered || file.Removed || file.Deleted )
                            continue;

                        file.Checked = bChecked;
                    }
                }

                // Should become selected node if it's not?
                if ( node == directoryTree.SelectedNode )
                {
                    DisplayFolderContents( node );
                }

                myStatusBar.ScanRequest = true;                
            }
        }

        /// <summary>
        /// Handle Change Source Folder menu option
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuChangeSourceFolder_Click( object sender, EventArgs e )
        {
            if ( SelectSourceFolder() == DialogResult.OK )
            {
                myLog.Write( "Source Path: " + myOptions.sourcePath );
            }
        }

        /// <summary>
        /// Rescan is just SelectSourceFolder without the selection.
        /// Inherently resets any checked/selected folders.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRescan_Click( object sender, EventArgs e )
        {
            PerformBackgroundOperation(new TreeRescanner(directoryTree, myOptions));
        }

        /// <summary>
        /// Exit menu handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExit_Click( object sender, EventArgs e )
        {
            if ( MessageBox.Show( "Really exit program?", "Quit?", MessageBoxButtons.YesNo ) == DialogResult.Yes )
            {
                Close();
            }
        }

        /// <summary>
        /// State of include hidden files menu option changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuIncludeHidden_CheckedChanged( object sender, EventArgs e )
        {
            myOptions.includeHidden = menuIncludeHidden.Checked;

            // Rescan folders and update the display
            if ( directoryTree.Nodes.Count > 0 )
            {
                if ( directoryTree.SelectedNode == null )
                    directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                DisplayFolderContents( directoryTree.SelectedNode );
            }

            myStatusBar.ScanRequest = true;
        }

        /// <summary>
        /// State of Ignore Size menu option changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuIgnoreSize_CheckedChanged( object sender, EventArgs e )
        {
            myOptions.ignoreSize = menuIgnoreSize.Checked;
        }

        /// <summary>
        /// State of Ignore Extension menu option changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuIgnoreExtension_CheckedChanged(object sender, EventArgs e)
        {
            myOptions.ignoreExtension = menuIgnoreExtension.Checked;
        }

        /// <summary>
        /// State of 'Allow Overwrite' menu option changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAllowOverwrite_CheckedChanged( object sender, EventArgs e )
        {
            myOptions.allowOverwrite = menuAllowOverwrite.Checked;
        }

        /// <summary>
        /// State of "Show filtered files" menu option changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuShowFilteredFiles_CheckedChanged( object sender, EventArgs e )
        {
            myOptions.showFilteredFiles = menuShowFilteredFiles.Checked;
            if ( directoryTree != null && directoryTree.SelectedNode != null )
            {
                DisplayFolderContents( directoryTree.SelectedNode );
            }
        }

        /// <summary>
        /// State of "Autoselect files on restore" changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuAutoselectFiles_CheckedChanged(object sender, EventArgs e)
        {
            myOptions.autoselectFilesOnRestore = menuAutoselectFiles.Checked;
        }


        /// <summary>
        /// Handle Remove Mirrored items menu selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFilterMirrored_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;
                RemoveMirroredPaths( true );
            }
        }

        /// <summary>
        /// Handle Remove Unmirrored Items menu selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFilterUnmirrored_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;
                RemoveUnmirroredPaths( true );
            }
        }

        /// <summary>
        /// Set date filters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuFilterByDate_Click(object sender, EventArgs e)
        {
            using (FormDateFilters formDateFilters = new FormDateFilters(myOptions.FilterIfOlder, myOptions.FilterIfNewer))
            {
                if (formDateFilters.ShowDialog() == DialogResult.OK)
                {
                    myOptions.FilterIfOlder = formDateFilters.FilterIfOlder;
                    myOptions.FilterIfNewer = formDateFilters.FilterIfNewer;

                    if (myOptions.FilterIfOlder.HasValue == false && myOptions.FilterIfNewer.HasValue == false)
                    {
                        MessageBox.Show("No date filters selected, nothing to filter!", "Cannot comply", MessageBoxButtons.OK);
                        return;
                    }

                    bool bRunInBackground = false;
                    DateRemover myDateRemover = new DateRemover(directoryTree, myOptions, myOptions.FilterIfOlder, myOptions.FilterIfNewer);
                    if (bRunInBackground)
                    {
                        PerformBackgroundOperation(myDateRemover);
                        AddOperationEndHandler(bw_EventHandlerExpandRoot);
                    }
                    else
                    {
                        PerformLongOperation(myDateRemover);

                        if (directoryTree.Nodes.Count > 0)
                        {
                            DisplayFolderContents(directoryTree.SelectedNode);
                            myStatusBar.ScanRequest = true;
                        }
                        else
                        {
                            myLog.Write("No files or folders between the unfiltered date range!");
                            // Never really want to remove the root, do we?
                            // Reference counting to the rescue!
                            //directoryTree.Nodes.Add( root );
                            //directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                        }
                    }
                }
            }
        }




        /// <summary>
        /// Copy all selected folders/files if you can
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuCopy_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;
                CopySelectedFolders( false );
            }
        }

        /// <summary>
        /// Move all selected files/folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMove_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;
                MoveSelectedFolders( false );
            }
        }

        /// <summary>
        /// Delete all selected files/folders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuDeleteFiles_Click( object sender, EventArgs e )
        {
            DialogResult result = MessageBox.Show( "Are you sure you want to delete the selected folders?", "Really delete?", MessageBoxButtons.YesNo );
            if ( result == DialogResult.Yes )
            {
                DeleteSelectedFolders( directoryTree.Nodes[0] );
            }
        }

        /// <summary>
        /// Find files in the destination which do not exist in the source tree.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void findOrphansToolStripMenuItem_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;
                FindOrphansInTarget();
            }
        }

        /// <summary>
        /// One-shot operation to copy any missing files/folders from the Source to the Target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuUpdateDirectory_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;

                UpdateTargetDirectory();
            }

        }



        /// <summary>
        /// One-shot operation to synchronise a directory with the current directory.  Substeps are:
        /// 1.  Select all nodes in the current tree (partial tree sync gets complicated)
        /// 2.  Remove mirrored files
        /// 3.  Copy remaining files
        /// 4.  Find orphans in the target
        /// 5.  Delete the orphans
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSynchroniseDirectory_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;

                string message = String.Format( "Are you sure?  Any unfiltered files in {0} not in {1} will be deleted!", myOptions.targetPath, myOptions.sourcePath );

                if ( MessageBox.Show( message, "Really Synchronise?", MessageBoxButtons.OKCancel ) == DialogResult.OK )
                {
                    SynchroniseTargetDirectory();
                }
            }
        }

        /// <summary>
        /// Another one-shot combo operation.  
        /// Merges two directories so that you end up with 2 identical copies representing their union.
        /// 1. Select all nodes
        /// 2. Remove mirrored paths in source
        /// 3. Find orphans in target
        /// 4. Back-copy to source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuMergeDirectories_Click( object sender, EventArgs e )
        {
            folderBrowserDialog2.SelectedPath = myOptions.targetPath;

            if ( folderBrowserDialog2.ShowDialog() == DialogResult.OK )
            {
                myOptions.targetPath = folderBrowserDialog2.SelectedPath;

                string message = String.Format( "Are you sure?  Files will be copied in both directions." );

                if ( MessageBox.Show( message, "Really merge?", MessageBoxButtons.OKCancel ) == DialogResult.OK )
                {
                    MergeDirectories();
                }
            }

        }


        /// <summary>
        /// Write the paths + filenames of selected files to a text file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSaveSelection_Click( object sender, EventArgs e )
        {
            openFileDialog.InitialDirectory = myOptions.sourcePath;
            openFileDialog.Filter = "Extended Format (*.m3u,*.m3u8,*.txt)|*.m3u;*.m3u8;*.txt|Text Files (*.txt)|*.txt|All Files|*.*";

            if ( openFileDialog.ShowDialog() == DialogResult.OK )
            {
                bool bExtended = openFileDialog.FilterIndex == 1;
                PerformLongOperation( new SelectionWriter( directoryTree, openFileDialog.FileName, bExtended, myOptions ) );
            }
        }

        /// <summary>
        /// Read a list of filenames from a text file and select the corresponding tree nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRestoreSelection_Click( object sender, EventArgs e )
        {
            bool restore = bCheckNodeChildren;

            if ( directoryTree.Nodes.Count > 0 )
            {
                openFileDialog.InitialDirectory = myOptions.sourcePath;
                openFileDialog.Filter = "Text Files (*.txt,*.m3u,*.m3u8)|*.txt;*.m3u;*.m3u8|All Files|*.*";

                if ( openFileDialog.ShowDialog() == DialogResult.OK )
                {
                    directoryTree.Nodes[ 0 ].Checked = false;
                    bCheckNodeChildren = false;     // Ugh, really wanted to avoid having to do this, but it is the simplest fix right now
                    PerformLongOperation( new SelectionFileReader( directoryTree, openFileDialog.FileName, myOptions ) );
                }
            }
            else
            {
                MessageBox.Show( "Cannot restore selection to empty tree" );
            }
            bCheckNodeChildren = restore;
        }

        /// <summary>
        /// Read a list of filenames from a text file and deselect the corresponding tree nodes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRemoveSelection_Click(object sender, EventArgs e)
        {
            bool restore = bCheckNodeChildren;

            if (directoryTree.Nodes.Count > 0)
            {
                openFileDialog.InitialDirectory = myOptions.sourcePath;
                openFileDialog.Filter = "Text Files (*.txt,*.m3u,*.m3u8)|*.txt;*.m3u;*.m3u8|All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    directoryTree.Nodes[0].Checked = false;
                    bCheckNodeChildren = false;     // Ugh, really wanted to avoid having to do this, but it is the simplest fix right now
                    PerformLongOperation(new DeselectionFileReader(directoryTree, openFileDialog.FileName, myOptions));
                }
            }
            else
            {
                MessageBox.Show("Cannot remove selection from empty tree");
            }
            bCheckNodeChildren = restore;
        }


        /// <summary>
        /// Expand all nodes with selected files or subfolders
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuExpandSelectedFolders_Click( object sender, EventArgs e )
        {
            if ( directoryTree.Nodes.Count > 0 )
            {
                doRecurseExpandNodeIfSelected( directoryTree.Nodes[ 0 ] );
            }
        }

        private bool doRecurseExpandNodeIfSelected( TreeNode node )
        {
            bool bExpand = false;

            FolderData folder = (FolderData)node.Tag;
            if ( node.Checked || folder.ContainsCheckedFiles )
            {
                bExpand = true;
            }

            foreach ( TreeNode child in node.Nodes )
            {
                if ( doRecurseExpandNodeIfSelected( child ) == true )
                {
                    bExpand = true;
                }
            }

            if ( bExpand )
            {
                node.Expand();                
            }

            return bExpand;
        }

        private void menuSelectAllInSelected_Click(object sender, EventArgs e)
        {
            if (directoryTree.Nodes.Count > 0)
            {
                doRecurseSelectFilesIfSelected(directoryTree.Nodes[0]);

                DisplayFolderContents(directoryTree.SelectedNode);
            }
        }

        private void doRecurseSelectFilesIfSelected(TreeNode node)
        {
            FolderData folder = (FolderData)node.Tag;

            if (node.Checked)
            {
                foreach (FileData file in folder.GetFiles())
                {
                    if (!file.Checked && !file.Filtered)
                    {
                        file.Checked = true;
                    }
                }
            }

            foreach (TreeNode child in node.Nodes)
            {
                doRecurseSelectFilesIfSelected(child);
            }
        }



        /// <summary>
        /// Set filename filters.  
        /// Use simple wildcard matching scheme ala Directory.GetFiles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuSetFilters_Click( object sender, EventArgs e )
        {
            using (FormSetFilters dialog = new FormSetFilters())
            {
                dialog.filterString = myOptions.Filters;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    myOptions.Filters = dialog.filterString;
                    myLog.Write("Set filters to [{0}]", myOptions.Filters);

                    if (directoryTree.Nodes.Count > 0)
                    {
                        if (directoryTree.SelectedNode == null)
                            directoryTree.SelectedNode = directoryTree.Nodes[0];
                        DisplayFolderContents(directoryTree.SelectedNode);
                    }

                    myStatusBar.ScanRequest = true;
                }
            }
        }

        /// <summary>
        /// Reset filters to default
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuClearFilters_Click( object sender, EventArgs e )
        {
            myOptions.Filters = "*";
            myLog.Write( "Set filters to [{0}]", myOptions.Filters );

            // Update status bar to show filtes, rescan directories (will UNDO any filtered directories!)
            if ( directoryTree.Nodes.Count > 0 )
            {
                if ( directoryTree.SelectedNode == null )
                    directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                DisplayFolderContents( directoryTree.SelectedNode );
            }

            myStatusBar.ScanRequest = true;
        }



        /// <summary>
        /// Context menu handlers for file view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmCheckSelectedFiles_Click( object sender, EventArgs e )
        {
            foreach ( ListViewItem item in fileListView.SelectedItems )
            {
                item.Checked = true;
            }
        }

        private void tsmUncheckSelectedFiles_Click( object sender, EventArgs e )
        {
            foreach ( ListViewItem item in fileListView.SelectedItems )
            {
                item.Checked = false;
            }
        }



        // OK, let's go with checkboxes
        private void fileListView_ItemChecked( object sender, ItemCheckedEventArgs e )
        {
            ListViewItem item = e.Item;
            Debug.Assert( item != null && item.Tag != null );
            FileData file = (FileData)item.Tag;

            // Logic here is somewhat tortuous... we want to preserve the underlying user-checked state
            // of the file, but present it as unchecked and uncheckable if it's filtered out by the options
            if ( file.Filtered || file.Removed || file.Deleted || ( file.Hidden && !myOptions.includeHidden ) )
            {
                item.Checked = false;
            }
            else
            {
                file.Checked = item.Checked;
            }

            item.ForeColor = GetFileColour( file );
            myStatusBar.ScanRequest = true;
        }

        /// <summary>
        /// Sort file info by different columns... who'd have thought this'd be so complicated to add in!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void fileListView_ColumnClick( object sender, ColumnClickEventArgs e )
        {
            // Toggle ascending/descending if we click the already selected column, else default to ascending
            if ( myOptions.SortColumn == e.Column )
            {
                myOptions.SortOrder = ( myOptions.SortOrder == SortOrder.Descending ) ? SortOrder.Ascending : SortOrder.Descending;
            }
            else
            {
                myOptions.SortOrder = SortOrder.Ascending;
            }

            SetFolderInfoSorting( e.Column, myOptions.SortOrder );
        }

        /// <summary>
        /// Sort file info by different columns... who'd have thought this'd be so complicated to add in!
        /// </summary>
        /// <param name="iColumn"></param>
        /// <param name="order"></param>
        private void SetFolderInfoSorting( int iColumn, SortOrder order )
        {
            fileListView.Columns[ myOptions.SortColumn ].Text = fileListView.Columns[ myOptions.SortColumn ].Text.Trim( ')', '^', 'v', '(', ' ' );
            fileListView.Columns[ iColumn ].Text += ( order == SortOrder.Descending ) ? " (v)" : " (^)";

            ColumnHeader column = fileListView.Columns[ iColumn ];

            myOptions.SortColumn = iColumn;
            myOptions.SortOrder = order;

            if ( iColumn == 0 )
            {
                fileListView.ListViewItemSorter = null; // Built in sort is fine + faster
                fileListView.Sorting = order;
            }
            else
            {
                fileListView.Sorting = SortOrder.None;      // It's going to ignore me anyway

                // Hard-code the indices because the column names come through as NULL... stupid .NET!
                if ( column.Index == 1 )
                {
                    fileListView.ListViewItemSorter = new ListViewItemComparerSize( myOptions.SortOrder );
                }
                else if ( column.Index == 2 )
                {
                    fileListView.ListViewItemSorter = new ListViewItemComparerDate( myOptions.SortOrder );
                }
                else
                {
                    fileListView.ListViewItemSorter = new ListViewItemComparer( iColumn, myOptions.SortOrder );
                }
            }

//            string[] sortnames = { "None", "Ascending", "Descending" };
//            myLog.Write( "Sorting by column {0}: {1} - {2}", e.Column, fileListView.Columns[ e.Column ].Text, sortnames[ (int)myOptions.SortOrder ] );

            DisplayFolderContents( directoryTree.SelectedNode );
        }


        /// <summary>
        /// If there is a currently selected folder, select all the files in it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmSelectAllFiles_Click( object sender, EventArgs e )
        {
            foreach ( ListViewItem item in fileListView.Items )
                item.Selected = true;
        }


        //
        // OK, let's go multithreaded
        //

        /// <summary>
        /// Perform a long operation asynchronously, with a dialog
        /// box showing progress and (optionally?) a cancel button.
        /// </summary>
        /// <param name="worker"></param>
        private DialogResult PerformLongOperation( Worker worker )
        {
            // Should never have 2 long operations running!
            Debug.Assert( backgroundWorker1.IsBusy == false );

            myWorker = worker;

            backgroundWorker1.RunWorkerAsync( worker );

            using (myProgress = new FormProgressDialog(worker))
            {
                myProgress.Text = worker.operationName;
                myProgress.label1.Text = worker.operationText;
                myProgress.label2.Text = "";
                myProgress.progressBar1.Value = 0;
                myProgress.cancelButton.Enabled = (worker.canCancel);
                myProgress.pauseButton.Enabled = (worker.canPause);
                DialogResult result = myProgress.ShowDialog();

                if (result == DialogResult.Cancel)
                {
                    if (backgroundWorker1.IsBusy)
                    {
                        // Don't allow any other ops to be started until worker has really cancelled
                        myLog.Write("Cancelling {0}... please wait.", worker.operationName);
                        menuStrip1.Enabled = false;
                        AddOperationEndHandler(bw_EventHandlerEnableMenus);
                        backgroundWorker1.CancelAsync();
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Perform a long operation asynchronously, without progress dialog
        /// </summary>
        /// <param name="worker"></param>
        private void PerformBackgroundOperation( Worker worker )
        {
            // Should never have 2 long operations running!
            Debug.Assert( backgroundWorker1.IsBusy == false );

            myWorker = worker;

            backgroundWorker1.RunWorkerAsync( worker );

            menuStrip1.Enabled = false; // Don't allow any other ops to be started!
            AddOperationEndHandler( bw_EventHandlerEnableMenus );
            myStatusBar.SetCurrentOperation( worker.operationName );
            statusMenuAbort.Enabled = worker.canCancel;
            statusMenuPause.Enabled = worker.canPause;
            statusMenu.Visible = worker.canCancel || worker.canPause;
        }

        /// <summary>
        /// Check whether a background or long operation is in progress
        /// </summary>
        /// <returns></returns>
        private bool OperationInProgress()
        {
            return backgroundWorker1.IsBusy;
        }

        /// <summary>
        /// Add an event handler to run on the end of a background operation
        /// </summary>
        /// <param name="handler"></param>
        private void AddOperationEndHandler( RunWorkerCompletedEventHandler handler )
        {
            backgroundWorker1.RunWorkerCompleted += handler;
        }

        /// <summary>
        /// Start off the worker in its new thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork( object sender, DoWorkEventArgs e )
        {
            Worker theWorker = (Worker)e.Argument;

            Worker.Report result = theWorker.Invoke( backgroundWorker1 );

            e.Result = result;
        }

        /// <summary>
        /// Some subtask has been completed... update the progress dialog.
        /// Set worker.currentOperation to be a dynamic report of what the worker is doing.
        /// If the first character is an '!' then the message will be logged as well as shown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged( object sender, ProgressChangedEventArgs e )
        {
            if ( myProgress != null )
            {
                if ( e.ProgressPercentage >= 0 )
                {
                    int progress = ( myProgress.progressBar1.Maximum * e.ProgressPercentage ) / 100;
                    myProgress.progressBar1.Style = ProgressBarStyle.Blocks;
                    myProgress.progressBar1.Value = progress > myProgress.progressBar1.Maximum ? myProgress.progressBar1.Maximum : progress;
                }
                else
                {
                    myProgress.progressBar1.Style = ProgressBarStyle.Marquee;
                    myProgress.progressBar1.Value = ( ( ++marqueeProgress ) / 20 ) % myProgress.progressBar1.Maximum;
                }
            }

            Worker.Report report = e.UserState as Worker.Report;

            if ( report != null )
            {
                ProcessWorkerReport( report );
                if ( report.status != null )
                {
                    myStatusBar.SetCurrentOperationStatus( report.status, e.ProgressPercentage );                    
                }
                else if ( report.percent >= 0 )
                {
                    myStatusBar.SetCurrentOperationStatus( percentComplete: e.ProgressPercentage );
                }
            }
        }

        /// <summary>
        /// The task has completed, one way or another.  Hide the progress dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
        {
            DialogResult dialogresult = DialogResult.OK;
            Worker.Report report = (Worker.Report)e.Result;

            if ( report != null )
            {
                Worker.State result = ProcessWorkerReport( report );
                switch ( result )
                {
                    case Worker.State.ABORTED:
                        myLog.Write( String.Format( "Aborted {0}", myWorker.operationName ) );
                        dialogresult = DialogResult.Cancel;      // Abort/Cancel... same same but different
                        break;
                    case Worker.State.COMPLETED:
                        myLog.Write( String.Format( "Finished {0}", myWorker.operationName ) );
                        dialogresult = DialogResult.OK;
                        break;
                    case Worker.State.ERROR:
                        myLog.Write( String.Format( "Error in {0}!", myWorker.operationName ) );
                        dialogresult = DialogResult.Abort;
                        break;
                    default:
                        myLog.Write( "Worker returned unknown state: {0}!", result.ToString() );
                        break;
                }
            }

            if ( myProgress != null )
            {
                // Feed back the result to PerformLongOperation
                myProgress.DialogResult = dialogresult;
                myProgress.Close();
                myProgress = null;                
            }

            if ( e.Error != null )
            {
                MessageBox.Show( e.Error.Message );
            }

            myStatusBar.SetCurrentOperation( null, null );
            statusMenu.Visible = false;
            myWorker = null;
        }

        /// <summary>
        /// Handle the report of a Background Worker
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        private Worker.State ProcessWorkerReport( Worker.Report report )
        {
            bool restore = bCheckNodeChildren;

            bCheckNodeChildren = false;     // Ugh, don't select all the subfolders of a selected node when rescanning (ugly)

            myLog.BeginUpdate();

            if ( myProgress != null )
            {
                if ( report.status != null )
                {
                    myProgress.label2.Text = report.status;
                }

                // Go on then, let's estimate how long this will take
                if ( report.percent > 0 )
                {
                    long timeTotal = ( report.timeTaken.Ticks * 100 ) / report.percent;
                    TimeSpan timeremaining = new TimeSpan( timeTotal ) - report.timeTaken;
                    myProgress.labelTimeTaken.Text = String.Format( "Time taken: {0}", HumanReadable.TimeSpan( report.timeTaken ) );
                    myProgress.labelTimeRemaining.Text = String.Format( "Time remaining: {0}", HumanReadable.TimeSpan( timeremaining ) );
                    myProgress.labelTimeTaken.Visible = true;
                    myProgress.labelTimeRemaining.Visible = true;
                }
                else
                {
                    myProgress.labelTimeTaken.Visible = false;
                    myProgress.labelTimeRemaining.Visible = false;
                }

            }

            // Echo any logged warnings
            if ( report.warnings != null )
            {
                foreach ( string warning in report.warnings )
                    myLog.Write( warning );
                report.warnings = null;
            }

            // Worker thread can't remove nodes itself, so let it build a list of nodes that it wants removed and process it here
            if ( report.removedNodes != null )
            {
                directoryTree.BeginUpdate();
                foreach ( TreeNode node in report.removedNodes )
                    node.Remove();
                report.removedNodes = null;
                directoryTree.EndUpdate();
            }

            // Worker thread can't check nodes itself, so let it build a list of nodes that it wants checked and process it here
            // Note that this will automatically check all children, including files!
            if ( report.selectedNodes != null )
            {
                directoryTree.BeginUpdate();

                foreach ( TreeNode node in report.selectedNodes )
                {
                    node.Checked = true;
                }

                report.selectedNodes = null;
                directoryTree.EndUpdate();
            }

            if (report.deselectedNodes != null)
            {
                directoryTree.BeginUpdate();
                foreach (TreeNode node in report.deselectedNodes)
                {
                    node.Checked = false;
                }

                report.deselectedNodes = null;
                directoryTree.EndUpdate();
            }

            if (report.clearTree)
            {
                directoryTree.BeginUpdate();
                directoryTree.Nodes.Clear();
                directoryTree.EndUpdate();
            }

            bCheckNodeChildren = restore;

            myLog.EndUpdate();
            return report.state;
        }

        /// <summary>
        /// Extra event handlers to perform operations when background worker completes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void bw_EventHandlerCloseForm( object sender, RunWorkerCompletedEventArgs args )
        {
            Close();
        }

        private void bw_EventHandlerEnableMenus( object sender, RunWorkerCompletedEventArgs args )
        {
            menuStrip1.Enabled = true;
        }

        private void bw_EventHandlerExpandRoot( object sender, RunWorkerCompletedEventArgs args )
        {
            if ( directoryTree.Nodes.Count > 0 )
            {
                directoryTree.Nodes[ 0 ].Expand();
                if ( directoryTree.SelectedNode == null )
                {
                    directoryTree.SelectedNode = directoryTree.Nodes[ 0 ];
                }

                DisplayFolderContents( directoryTree.SelectedNode );
            }
            else
            {
                // Handler gets called multiple times for some reason, so message boxes are annoying
                myLog.Write( "No folders in this directory - please selected another" );
//                MessageBox.Show( "No folders in this directory - please selected another", "No folders!", MessageBoxButtons.OK );
            }
        }

        /// <summary>
        /// Handle use of Abort button on statusbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusMenuAbort_Click( object sender, EventArgs e )
        {
            if ( OperationInProgress() )
            {
                // Don't allow any other ops to be started until worker has really cancelled
                myLog.Write( "Cancelling... please wait." );
                menuStrip1.Enabled = false;
                statusMenu.Text = "Stop";
                statusMenuPause.Text = "Pause";
                statusMenu.Visible = false;
                AddOperationEndHandler( bw_EventHandlerEnableMenus );
                backgroundWorker1.CancelAsync();
            }
        }

        /// <summary>
        /// Handle use of Pause button on status bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void statusMenuPause_Click( object sender, EventArgs e )
        {
            if ( OperationInProgress() && myWorker != null )
            {
                if ( statusMenuPause.Text == "Pause" )
                {
                    myLog.Write( "Pausing..." );
                    menuStrip1.Enabled = false;
                    myWorker.RequestPause();
                    statusMenuPause.Text = "Resume";
                    statusMenu.Text = "Paused";
                }
                else
                {
                    myLog.Write( "Resuming..." );
                    menuStrip1.Enabled = false;
                    myWorker.RequestResume();
                    statusMenuPause.Text = "Pause";
                    statusMenu.Text = "Stop";
                }
            }
        }

    }

    /// <summary>
    /// Log window class... simplifies code a little bit
    /// </summary>
    public class Log
    {
        public string theLog = "";
        private TextBox theTextBox;
        private bool Updating = false;

        public void Write( string output )
        {
            theLog = theLog + output + Environment.NewLine;
            if ( theTextBox != null && !Updating )
            {
                // Update control text and scroll to end
                theTextBox.Text = theLog;
                theTextBox.SelectionStart = theTextBox.Text.Length;
                theTextBox.ScrollToCaret();
            }
        }

        public void Write( string output, params object[] args )
        {
            string text = ( args.Length > 0 ) ? String.Format( output, args ) : output;
            theLog = theLog + text + Environment.NewLine;
            if ( theTextBox != null && !Updating )
            {
                // Update control text and scroll to end
                theTextBox.Text = theLog;
                theTextBox.SelectionStart = theTextBox.Text.Length;
                theTextBox.ScrollToCaret();
            }
        }

        public void Clear()
        {
            theLog = "";
        }

        public void BeginUpdate()
        {
            Updating = true;
        }

        public void EndUpdate()
        {
            Updating = false;
            if ( theTextBox != null )
            {
                // Update control text and scroll to end
                theTextBox.Text = theLog;
                theTextBox.SelectionStart = theTextBox.Text.Length;
                theTextBox.ScrollToCaret();
            }
        }

        public string GetText( int maxLines = 0 )
        {
            return theLog;
        }

        public void SetTextbox( TextBox aTextBox )
        {
            theTextBox = aTextBox;
        }
    }

    /// <summary>
    /// Holder for application options, to make it easier
    /// to pass them around between different classes and to
    /// serialise/deserialise them.
    /// </summary>
    public class Options
	{
        public string sourcePath = "";
        public string targetPath = "";
        public bool includeHidden = false;
        public bool ignoreSize = false;
        public bool ignoreExtension = false;
        public bool allowOverwrite = false;
        public bool showFilteredFiles = false;
        public bool autoselectFilesOnRestore = false;
        private string filters = "*";
        public Regex filtersRegex = null;
        public Size windowSize;
        public Point windowLocation;
        public int[] columnSizes = { };
        public int SortColumn = 0;
        public SortOrder SortOrder = SortOrder.Ascending;
        public DateTime? FilterIfOlder = null;
        public DateTime? FilterIfNewer = null;


        public Options() { }

        // Why isn't there a default copy constructor?
        public Options( Options options )
        {
            sourcePath = options.sourcePath;
            targetPath = options.targetPath;
            includeHidden = options.includeHidden;
            ignoreSize = options.ignoreSize;
            allowOverwrite = options.allowOverwrite;
            showFilteredFiles = options.showFilteredFiles;
            autoselectFilesOnRestore = options.autoselectFilesOnRestore;
            filters = options.filters;                  // copy reference, bypass the property Filters
            windowSize = options.windowSize;
            windowLocation = options.windowLocation;
            columnSizes = options.columnSizes;
            filtersRegex = options.filtersRegex;        // copy reference
            SortColumn = options.SortColumn;
            SortOrder = options.SortOrder;
        }

        // Cache the regular expression for convenience... are we going to end up leaking these?
        public string Filters
        {
            get { return filters; }
            set
            {
                filters = value;
                if ( filters != null && filters != "" && filters != "*" )
                {
                    filtersRegex = Wildcard.WildcardToRegex( filters );
                }
                else
                    filtersRegex = null;
            }
        }


        public void Save( string filename )
        {
            autoselectFilesOnRestore = false;       // Too risky to leave on!

            XmlSerializer xml = new XmlSerializer(this.GetType());
            XmlTextWriter writer = new XmlTextWriter( filename, null );
            xml.Serialize( writer, this );
            writer.Close();
        }

        public static Options Load( string filename )
        {
            Options opt = new Options();
            XmlSerializer xml = new XmlSerializer( opt.GetType() );
            XmlTextReader reader = new XmlTextReader( filename );
            opt = (Options)xml.Deserialize( reader );
            reader.Close();
            return opt;
        }
	}

    /// <summary>
    /// Static class to hold some helpers for producing human-friendly strings from different data types
    /// </summary>
    public static class HumanReadable
    {
        static string[] sizes = { "B", "KB", "MB", "GB" };

        public static string Size( long size )
        {
            double len = (double)size;
            int order = 0;
            while ( len >= 1024 && order + 1 < sizes.Length )
            {
                len = len / 1024;
                order++;
            }
            return String.Format( "{0:0.##} {1}", len, sizes[ order ] );
        }

        public static string TimeSpan( TimeSpan ts )
        { 
            if ( ts.Hours > 0 )
            {
                return String.Format( "{0} hours, {1} minutes", ts.Hours, ts.Minutes );
            }
            else if ( ts.Minutes > 0 )
            {
                return String.Format( "{0} minutes, {1} seconds", ts.Minutes, ts.Seconds );
            }
            else
            {
                return String.Format( "{0} seconds", ts.Seconds );
            }
        }

    }

    /// <summary>
    /// Class to sort list item by column - why isn't this built-in?
    /// </summary>
    internal class ListViewItemComparer : System.Collections.IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer( int column, SortOrder sorting )
        {
            col = column;
            order = sorting;
        }
        public int Compare( object x, object y )
        {
            if ( order == SortOrder.Ascending )
            {
                return String.Compare( ( (ListViewItem)x ).SubItems[ col ].Text.ToLowerInvariant(), ( (ListViewItem)y ).SubItems[ col ].Text.ToLowerInvariant() );
            }
            else if ( order == SortOrder.Descending )
            {
                return -String.Compare( ( (ListViewItem)x ).SubItems[ col ].Text.ToLowerInvariant(), ( (ListViewItem)y ).SubItems[ col ].Text.ToLowerInvariant() );
            }
            return 0;
        }
    }

    /// <summary>
    /// Sort numeric values
    /// </summary>
    internal class ListViewItemComparerNumeric : System.Collections.IComparer
    {
        private int col = 0;
        private SortOrder order;

        public ListViewItemComparerNumeric( int column, SortOrder sorting )
        {
            col = column;
            order = sorting;
        }
        public int Compare( object x, object y )
        {
            Int64 xx = Convert.ToInt64( ( (ListViewItem)x ).SubItems[ col ].Text );
            Int64 yy = Convert.ToInt64( ( (ListViewItem)y ).SubItems[ col ].Text );
            if ( xx != yy )
            {
                if ( order == SortOrder.Ascending )
                {
                    return ( xx > yy ) ? 1 : -1;
                }
                else if ( order == SortOrder.Descending )
                {
                    return ( xx > yy ) ? -1 : 1;
                }
            }
            return 0;
        }
    }

    /// <summary>
    /// OK fine, we'll write specialised classes for sorting on size and date to save
    /// having to add the hidden columns for every file
    /// </summary>
    internal class ListViewItemComparerSize : System.Collections.IComparer
    {
        private SortOrder order;
        public ListViewItemComparerSize( SortOrder sorting )
        {
            order = sorting;
        }
        public int Compare( object x, object y )
        {
            FileData file1 = ( (ListViewItem)x ).Tag as FileData;
            FileData file2 = ( (ListViewItem)y ).Tag as FileData;

            if ( file1 == null || file2 == null || file1 == file2 )
                return 0;

            if ( order == SortOrder.Ascending )
            {
                return ( file1.Length > file2.Length ) ? 1 : -1;
            }
            else if ( order == SortOrder.Descending )
            {
                return ( file1.Length > file2.Length ) ? -1 : 1;
            }

            return 0;
        }

    }

    internal class ListViewItemComparerDate : System.Collections.IComparer
    {
        private SortOrder order;
        public ListViewItemComparerDate( SortOrder sorting )
        {
            order = sorting;
        }
        public int Compare( object x, object y )
        {
            FileData file1 = ( (ListViewItem)x ).Tag as FileData;
            FileData file2 = ( (ListViewItem)y ).Tag as FileData;

            if ( file1 == null || file2 == null || file1 == file2 )
                return 0;

            if ( order == SortOrder.Ascending )
            {
                return ( file1.CreationTime > file2.CreationTime ) ? 1 : -1;
            }
            else if ( order == SortOrder.Descending )
            {
                return ( file1.CreationTime > file2.CreationTime ) ? -1 : 1;
            }

            return 0;
        }

    }


    
}
