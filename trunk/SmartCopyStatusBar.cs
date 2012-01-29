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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SmartCopyTool
{
    /// <summary>
    /// Parcel off the status bar into its own class
    /// And set up a background thread to keep it up to date.
    /// </summary>
    class SmartCopyStatusBar
    {
        private TreeView theTree = null;
        private StatusStrip theStrip = null;
        private ToolStripLabel theLabel = null;
        private ToolStripLabel theOtherLabel = null;

        private Options parentOptions = null;
        private Options statusBarOptions = null;        // The options that were set when status bar was last updated

        private long m_numCheckedFolders = -1;
        private long m_checkedFoldersSize = -1;

        private string backgroundOperationName;
        private string backgroundOperationStatus;
        private int backgroundOperationPercent;

        public volatile bool ScanRequest = false;

        public FolderData CurrentFolder = null;
        public long CurrentFolderSize = -1;

        private Thread updateThread = null;

        public delegate List<FileData> DelegateGetFiles( FolderData dir, Options options );
        private DelegateGetFiles delGetFiles = GetSelectedFiles;

        public SmartCopyStatusBar( StatusStrip strip, TreeView tree, Options options )
        {
            theTree = tree;
            theStrip = strip;
            theLabel = (ToolStripLabel)strip.Items[ "label1" ];
            theOtherLabel = (ToolStripLabel)strip.Items[ "label2" ];
            parentOptions = options;

            // Start a thread to keep status bar updated in the background
            updateThread = new Thread( UpdateThread );
            updateThread.Name = "Status Bar Update Thread";
            updateThread.IsBackground = true;
            updateThread.Priority = ThreadPriority.BelowNormal;
            updateThread.Start();
        }

        public void Terminate()
        {
            // Cancel scan task if there's one running
            updateThread.Abort();
        }

        public void SetCurrentOperation( string name, string status = null )
        {
            backgroundOperationName = name;
            backgroundOperationStatus = status;
            backgroundOperationPercent = -1;
        }

        public void SetCurrentOperationStatus( string status = null, int percentComplete = -1 )
        {
            backgroundOperationStatus = status;
            backgroundOperationPercent = percentComplete;
        }

        public string GetStatusString( Options options )
        {
            string statusString = "";

            if ( CurrentFolder != null )
            {
                FolderData dir = CurrentFolder;
                if ( dir != null && Directory.Exists( dir.FullName ) )
                {
                    statusString = String.Format( "{0} [ {1} folders, {2} files, {3}]", dir.FullName, dir.NumDirectories, dir.NumFiles, HumanReadable.Size( CurrentFolderSize ) );
                }
            }
            else
            {
                statusString = options.sourcePath;
            }

            if ( ScanRequest )
            {
                statusString += "<...scanning...>";
            }
            else
            {
                if ( m_numCheckedFolders > 0 )
                {
                    statusString += String.Format( "   Selected Folders: {0} ({1}) ", m_numCheckedFolders, HumanReadable.Size( m_checkedFoldersSize ) );
                }
            }

            statusString += String.Format( "     Filters: [{0}]", options.Filters );

            return statusString;
        }

        private string GetStatusSubstring( Options options )
        {
            string substring = "";
            if ( backgroundOperationName != null )
            {
                if ( backgroundOperationStatus != null )
                {
                    substring += String.Format( " <...{0}...{1}%...{2}...> ", backgroundOperationName, backgroundOperationPercent, backgroundOperationStatus );
                }
                else if ( backgroundOperationPercent >= 0 )
                {
                    substring += String.Format( " <...{0}...{1}%...> ", backgroundOperationName, backgroundOperationPercent );
                }
                else
                {
                    substring += String.Format( " <...{0}...> ", backgroundOperationName );
                }
            }
            else
            {
                if ( options.includeHidden == true )
                {
                    substring += " (Include Hidden)";
                }

                if ( options.ignoreSize == true )
                {
                    substring += " (Ignore Size)";
                }

                if ( options.allowOverwrite == true )
                {
                    substring += " (Allow Overwrite)";
                }

                if ( options.showFilteredFiles == true )
                {
                    substring += " (Show Filtered Files)";
                }

            }

            return substring;
        }

        /// <summary>
        /// Invoke the GUI thread to set the label text
        /// </summary>
        private void UpdateDisplay()
        {
            if ( statusBarOptions != null && theTree != null && theTree.IsHandleCreated )
            {
                if ( theLabel != null && theOtherLabel != null )
                {
                    string status = GetStatusString( statusBarOptions );
                    string substring = GetStatusSubstring( statusBarOptions );
                    theTree.Invoke( new Action( () =>
                    {
                        theLabel.Text = status;
                        theOtherLabel.Text = substring;
                    }
                    ) );
                }
            }
        }

        private void UpdateThread()
        {
            try
            {
                while ( true )
                {
                    // If any significant options have changed, recalculate stats
                    if ( statusBarOptions == null ||
                         parentOptions.includeHidden != statusBarOptions.includeHidden ||
                         parentOptions.Filters != statusBarOptions.Filters ||
                         parentOptions.sourcePath != statusBarOptions.sourcePath ||
                         parentOptions.ignoreSize != statusBarOptions.ignoreSize ||
                         parentOptions.allowOverwrite != statusBarOptions.allowOverwrite )
                    {
                        statusBarOptions = new Options( parentOptions );
                    }

                    if ( ScanRequest )
                    {
                        m_numCheckedFolders = 0;
                        m_checkedFoldersSize = 0;
                        UpdateDisplay();

                        // Reset before we start the scan, because can be restarted at any time
                        ScanRequest = false;

                        if ( theTree.Nodes.Count > 0 )
                        {
                            DirectoryScannerTask( statusBarOptions, theTree.Nodes[ 0 ] );
                        }
                    }

                    UpdateDisplay();
                    Thread.Sleep( 100 );
                }
            }
            catch (ThreadAbortException) 
            { 
            }
        }

        private void DirectoryScannerTask( Options options, TreeNode node )
        {
            // Allow the scan to be restarted at any time
            if ( ScanRequest )
                return;

            if ( node != null )
            {
                FolderData dir = (FolderData)node.Tag;                

                if ( node.Checked && node.Tag != null )
                {
                    m_numCheckedFolders++;
                    FolderData folder = (FolderData)node.Tag;
                    List<FileData> files = (List<FileData>)theTree.Invoke( delGetFiles, folder, options );

                    foreach ( FileData file in files )
                        m_checkedFoldersSize += file.Length;
                }

                foreach ( TreeNode child in node.Nodes )
                {
                    DirectoryScannerTask( options, child );
                }                
            }
        }

        private static List<FileData> GetSelectedFiles( FolderData folder, Options options )
        {
            return folder.GetSelectedFiles( options );
        }

    }
}
