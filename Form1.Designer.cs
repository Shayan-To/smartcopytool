namespace SmartCopyTool
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMenu = new System.Windows.Forms.ToolStripDropDownButton();
            this.statusMenuAbort = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMenuPause = new System.Windows.Forms.ToolStripMenuItem();
            this.label2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMove = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFlatten = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.menuChangeSourceFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRescan = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRemoveMirrored = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveUnmirrored = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFilterByDate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDeleteFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFindOrphans = new System.Windows.Forms.ToolStripMenuItem();
            this.menuUpdateDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSynchroniseDirectory = new System.Windows.Forms.ToolStripMenuItem();
            this.menuMergeDirectories = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuSaveSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRestoreSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemoveSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.menuExpandSelectedFolders = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSelectAllInSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.menuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.filtersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSetFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.menuClearFilters = new System.Windows.Forms.ToolStripMenuItem();
            this.menuShowFilteredFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.menuIncludeHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIgnoreSize = new System.Windows.Forms.ToolStripMenuItem();
            this.menuIgnoreExtension = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAllowOverwrite = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDeleteReadOnly = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAutoselectFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutThisSoftwareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directoryTree = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.fileListView = new System.Windows.Forms.ListView();
            this.columnFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnNotes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmCheckSelectedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUncheckSelectedFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmSelectAllFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.fileContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.label1,
            this.statusMenu,
            this.label2,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 540);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label1
            // 
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 17);
            this.label1.Text = "Smart Copy Tool";
            // 
            // statusMenu
            // 
            this.statusMenu.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusMenuAbort,
            this.statusMenuPause});
            this.statusMenu.Image = ((System.Drawing.Image)(resources.GetObject("statusMenu.Image")));
            this.statusMenu.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.statusMenu.Name = "statusMenu";
            this.statusMenu.Size = new System.Drawing.Size(44, 20);
            this.statusMenu.Text = "Stop";
            // 
            // statusMenuAbort
            // 
            this.statusMenuAbort.Name = "statusMenuAbort";
            this.statusMenuAbort.Size = new System.Drawing.Size(105, 22);
            this.statusMenuAbort.Text = "Abort";
            this.statusMenuAbort.Click += new System.EventHandler(this.statusMenuAbort_Click);
            // 
            // statusMenuPause
            // 
            this.statusMenuPause.Name = "statusMenuPause";
            this.statusMenuPause.Size = new System.Drawing.Size(105, 22);
            this.statusMenuPause.Text = "Pause";
            this.statusMenuPause.Click += new System.EventHandler(this.statusMenuPause_Click);
            // 
            // label2
            // 
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.Text = "Hey There!";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.filtersToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopy,
            this.menuMove,
            this.menuFlatten,
            this.toolStripSeparator8,
            this.menuChangeSourceFolder,
            this.menuRescan,
            this.toolStripSeparator4,
            this.menuRemoveMirrored,
            this.menuRemoveUnmirrored,
            this.menuFilterByDate,
            this.toolStripSeparator2,
            this.menuDeleteFiles,
            this.toolStripSeparator3,
            this.menuFindOrphans,
            this.menuUpdateDirectory,
            this.menuSynchroniseDirectory,
            this.menuMergeDirectories,
            this.toolStripSeparator1,
            this.menuSaveSelection,
            this.menuRestoreSelection,
            this.menuRemoveSelection,
            this.menuExpandSelectedFolders,
            this.menuSelectAllInSelected,
            this.toolStripSeparator6,
            this.menuExit});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "File";
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.menuCopy.Size = new System.Drawing.Size(293, 22);
            this.menuCopy.Text = "&Copy Selected Files";
            this.menuCopy.ToolTipText = "Copy selected folders to a target directory";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuMove
            // 
            this.menuMove.Name = "menuMove";
            this.menuMove.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.menuMove.Size = new System.Drawing.Size(293, 22);
            this.menuMove.Text = "&Move Selected Files";
            this.menuMove.ToolTipText = "Move selected files and folders to a target directory";
            this.menuMove.Click += new System.EventHandler(this.menuMove_Click);
            // 
            // menuFlatten
            // 
            this.menuFlatten.Name = "menuFlatten";
            this.menuFlatten.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.menuFlatten.Size = new System.Drawing.Size(293, 22);
            this.menuFlatten.Text = "&Flatten Selected Files";
            this.menuFlatten.Click += new System.EventHandler(this.menuFlatten_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(290, 6);
            // 
            // menuChangeSourceFolder
            // 
            this.menuChangeSourceFolder.Name = "menuChangeSourceFolder";
            this.menuChangeSourceFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.menuChangeSourceFolder.Size = new System.Drawing.Size(293, 22);
            this.menuChangeSourceFolder.Text = "Change Source &Folder";
            this.menuChangeSourceFolder.ToolTipText = "Select a new source folder";
            this.menuChangeSourceFolder.Click += new System.EventHandler(this.menuChangeSourceFolder_Click);
            // 
            // menuRescan
            // 
            this.menuRescan.Name = "menuRescan";
            this.menuRescan.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.D)));
            this.menuRescan.Size = new System.Drawing.Size(293, 22);
            this.menuRescan.Text = "Re&scan";
            this.menuRescan.ToolTipText = "Rescan the current directory structure";
            this.menuRescan.Click += new System.EventHandler(this.menuRescan_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(290, 6);
            // 
            // menuRemoveMirrored
            // 
            this.menuRemoveMirrored.Name = "menuRemoveMirrored";
            this.menuRemoveMirrored.Size = new System.Drawing.Size(293, 22);
            this.menuRemoveMirrored.Text = "Filter Mirrored Paths";
            this.menuRemoveMirrored.ToolTipText = "Remove files and folders from the list if they exist in a target directory";
            this.menuRemoveMirrored.Click += new System.EventHandler(this.menuFilterMirrored_Click);
            // 
            // menuRemoveUnmirrored
            // 
            this.menuRemoveUnmirrored.Name = "menuRemoveUnmirrored";
            this.menuRemoveUnmirrored.Size = new System.Drawing.Size(293, 22);
            this.menuRemoveUnmirrored.Text = "Filter Unmirrored Paths";
            this.menuRemoveUnmirrored.ToolTipText = "Remove files and folders from the list if they do not exist in a target directory" +
    "";
            this.menuRemoveUnmirrored.Click += new System.EventHandler(this.menuFilterUnmirrored_Click);
            // 
            // menuFilterByDate
            // 
            this.menuFilterByDate.Name = "menuFilterByDate";
            this.menuFilterByDate.Size = new System.Drawing.Size(293, 22);
            this.menuFilterByDate.Text = "Filter By Date";
            this.menuFilterByDate.Click += new System.EventHandler(this.menuFilterByDate_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(290, 6);
            // 
            // menuDeleteFiles
            // 
            this.menuDeleteFiles.Name = "menuDeleteFiles";
            this.menuDeleteFiles.Size = new System.Drawing.Size(293, 22);
            this.menuDeleteFiles.Text = "Delete Selected Files";
            this.menuDeleteFiles.ToolTipText = "Delete selected files from source directory";
            this.menuDeleteFiles.Click += new System.EventHandler(this.menuDeleteFiles_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(290, 6);
            // 
            // menuFindOrphans
            // 
            this.menuFindOrphans.Name = "menuFindOrphans";
            this.menuFindOrphans.Size = new System.Drawing.Size(293, 22);
            this.menuFindOrphans.Text = "Find Orphans";
            this.menuFindOrphans.ToolTipText = "Find files in target directory which do not exist in current directory.  Changes " +
    "current directory.";
            this.menuFindOrphans.Click += new System.EventHandler(this.findOrphansToolStripMenuItem_Click);
            // 
            // menuUpdateDirectory
            // 
            this.menuUpdateDirectory.Name = "menuUpdateDirectory";
            this.menuUpdateDirectory.Size = new System.Drawing.Size(293, 22);
            this.menuUpdateDirectory.Text = "Update Directory";
            this.menuUpdateDirectory.ToolTipText = "Copy any files and folders in the source to the target if they are not already pr" +
    "esent.";
            this.menuUpdateDirectory.Click += new System.EventHandler(this.menuUpdateDirectory_Click);
            // 
            // menuSynchroniseDirectory
            // 
            this.menuSynchroniseDirectory.Name = "menuSynchroniseDirectory";
            this.menuSynchroniseDirectory.Size = new System.Drawing.Size(293, 22);
            this.menuSynchroniseDirectory.Text = "Synchronise Directory";
            this.menuSynchroniseDirectory.ToolTipText = "Synchronise target directory with current directory tree.  Any files in target no" +
    "t in source will be deleted!";
            this.menuSynchroniseDirectory.Click += new System.EventHandler(this.menuSynchroniseDirectory_Click);
            // 
            // menuMergeDirectories
            // 
            this.menuMergeDirectories.Name = "menuMergeDirectories";
            this.menuMergeDirectories.Size = new System.Drawing.Size(293, 22);
            this.menuMergeDirectories.Text = "Merge Directories";
            this.menuMergeDirectories.ToolTipText = "Merge this directory with another.  Any files not in either directory will be cop" +
    "ied to the other.";
            this.menuMergeDirectories.Click += new System.EventHandler(this.menuMergeDirectories_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(290, 6);
            // 
            // menuSaveSelection
            // 
            this.menuSaveSelection.Name = "menuSaveSelection";
            this.menuSaveSelection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.menuSaveSelection.Size = new System.Drawing.Size(293, 22);
            this.menuSaveSelection.Text = "Save Selection To Text File";
            this.menuSaveSelection.ToolTipText = "Save selected files and folders to a text file or playlist";
            this.menuSaveSelection.Click += new System.EventHandler(this.menuSaveSelection_Click);
            // 
            // menuRestoreSelection
            // 
            this.menuRestoreSelection.Name = "menuRestoreSelection";
            this.menuRestoreSelection.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.menuRestoreSelection.Size = new System.Drawing.Size(293, 22);
            this.menuRestoreSelection.Text = "Restore Selection From File";
            this.menuRestoreSelection.ToolTipText = "Select files and folders from a saved selection file";
            this.menuRestoreSelection.Click += new System.EventHandler(this.menuRestoreSelection_Click);
            // 
            // menuRemoveSelection
            // 
            this.menuRemoveSelection.Name = "menuRemoveSelection";
            this.menuRemoveSelection.ShortcutKeyDisplayString = "Ctrl+Shift+R";
            this.menuRemoveSelection.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.menuRemoveSelection.Size = new System.Drawing.Size(293, 22);
            this.menuRemoveSelection.Text = "Remove Selection From File";
            this.menuRemoveSelection.ToolTipText = "Deselect files and folders from a saved selection file";
            this.menuRemoveSelection.Click += new System.EventHandler(this.menuRemoveSelection_Click);
            // 
            // menuExpandSelectedFolders
            // 
            this.menuExpandSelectedFolders.Name = "menuExpandSelectedFolders";
            this.menuExpandSelectedFolders.Size = new System.Drawing.Size(293, 22);
            this.menuExpandSelectedFolders.Text = "Expand Selected Folders";
            this.menuExpandSelectedFolders.Click += new System.EventHandler(this.menuExpandSelectedFolders_Click);
            // 
            // menuSelectAllInSelected
            // 
            this.menuSelectAllInSelected.Name = "menuSelectAllInSelected";
            this.menuSelectAllInSelected.Size = new System.Drawing.Size(293, 22);
            this.menuSelectAllInSelected.Text = "Select All Files In Selected Folders";
            this.menuSelectAllInSelected.Click += new System.EventHandler(this.menuSelectAllInSelected_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(290, 6);
            // 
            // menuExit
            // 
            this.menuExit.Name = "menuExit";
            this.menuExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.menuExit.Size = new System.Drawing.Size(293, 22);
            this.menuExit.Text = "E&xit";
            this.menuExit.Click += new System.EventHandler(this.menuExit_Click);
            // 
            // filtersToolStripMenuItem
            // 
            this.filtersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuSetFilters,
            this.menuClearFilters,
            this.menuShowFilteredFiles,
            this.toolStripSeparator5,
            this.menuIncludeHidden,
            this.menuIgnoreSize,
            this.menuIgnoreExtension,
            this.menuAllowOverwrite,
            this.menuDeleteReadOnly,
            this.toolStripSeparator9,
            this.menuAutoselectFiles});
            this.filtersToolStripMenuItem.Name = "filtersToolStripMenuItem";
            this.filtersToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.filtersToolStripMenuItem.Text = "Options";
            // 
            // menuSetFilters
            // 
            this.menuSetFilters.Name = "menuSetFilters";
            this.menuSetFilters.Size = new System.Drawing.Size(268, 22);
            this.menuSetFilters.Text = "Set Filename Filters";
            this.menuSetFilters.Click += new System.EventHandler(this.menuSetFilters_Click);
            // 
            // menuClearFilters
            // 
            this.menuClearFilters.Name = "menuClearFilters";
            this.menuClearFilters.Size = new System.Drawing.Size(268, 22);
            this.menuClearFilters.Text = "Clear Filename Filters";
            this.menuClearFilters.Click += new System.EventHandler(this.menuClearFilters_Click);
            // 
            // menuShowFilteredFiles
            // 
            this.menuShowFilteredFiles.CheckOnClick = true;
            this.menuShowFilteredFiles.Name = "menuShowFilteredFiles";
            this.menuShowFilteredFiles.Size = new System.Drawing.Size(268, 22);
            this.menuShowFilteredFiles.Text = "Show Filtered Files";
            this.menuShowFilteredFiles.CheckedChanged += new System.EventHandler(this.menuShowFilteredFiles_CheckedChanged);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(265, 6);
            // 
            // menuIncludeHidden
            // 
            this.menuIncludeHidden.CheckOnClick = true;
            this.menuIncludeHidden.Name = "menuIncludeHidden";
            this.menuIncludeHidden.Size = new System.Drawing.Size(268, 22);
            this.menuIncludeHidden.Text = "Include Hidden Files";
            this.menuIncludeHidden.ToolTipText = "Include hidden files in copy/move/compare operations";
            this.menuIncludeHidden.CheckedChanged += new System.EventHandler(this.menuIncludeHidden_CheckedChanged);
            // 
            // menuIgnoreSize
            // 
            this.menuIgnoreSize.CheckOnClick = true;
            this.menuIgnoreSize.Name = "menuIgnoreSize";
            this.menuIgnoreSize.Size = new System.Drawing.Size(268, 22);
            this.menuIgnoreSize.Text = "Ignore Size";
            this.menuIgnoreSize.ToolTipText = "Ignore file sizes when checking for mirrored or unmirrored files and folders";
            this.menuIgnoreSize.CheckedChanged += new System.EventHandler(this.menuIgnoreSize_CheckedChanged);
            // 
            // menuIgnoreExtension
            // 
            this.menuIgnoreExtension.CheckOnClick = true;
            this.menuIgnoreExtension.Name = "menuIgnoreExtension";
            this.menuIgnoreExtension.Size = new System.Drawing.Size(268, 22);
            this.menuIgnoreExtension.Text = "Ignore Extension";
            this.menuIgnoreExtension.ToolTipText = "Ignore file extensions when checked";
            this.menuIgnoreExtension.CheckedChanged += new System.EventHandler(this.menuIgnoreExtension_CheckedChanged);
            // 
            // menuAllowOverwrite
            // 
            this.menuAllowOverwrite.CheckOnClick = true;
            this.menuAllowOverwrite.Name = "menuAllowOverwrite";
            this.menuAllowOverwrite.Size = new System.Drawing.Size(268, 22);
            this.menuAllowOverwrite.Text = "Allow Overwrite";
            this.menuAllowOverwrite.ToolTipText = "Overwrite files in destination when  copying or moving";
            this.menuAllowOverwrite.CheckedChanged += new System.EventHandler(this.menuAllowOverwrite_CheckedChanged);
            // 
            // menuDeleteReadOnly
            // 
            this.menuDeleteReadOnly.CheckOnClick = true;
            this.menuDeleteReadOnly.Name = "menuDeleteReadOnly";
            this.menuDeleteReadOnly.Size = new System.Drawing.Size(268, 22);
            this.menuDeleteReadOnly.Text = "Allow Read-Only File Deletion";
            this.menuDeleteReadOnly.ToolTipText = "Allow read-only files to be deleted or overwritten";
            this.menuDeleteReadOnly.CheckedChanged += new System.EventHandler(this.menuDeleteReadOnly_CheckedChanged);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(265, 6);
            // 
            // menuAutoselectFiles
            // 
            this.menuAutoselectFiles.CheckOnClick = true;
            this.menuAutoselectFiles.Name = "menuAutoselectFiles";
            this.menuAutoselectFiles.Size = new System.Drawing.Size(268, 22);
            this.menuAutoselectFiles.Text = "Autoselect Files On Selection Restore";
            this.menuAutoselectFiles.ToolTipText = "When restoring selections from file, autoselect files in selected folders";
            this.menuAutoselectFiles.CheckedChanged += new System.EventHandler(this.menuAutoselectFiles_CheckedChanged);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutThisSoftwareToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutThisSoftwareToolStripMenuItem
            // 
            this.aboutThisSoftwareToolStripMenuItem.Name = "aboutThisSoftwareToolStripMenuItem";
            this.aboutThisSoftwareToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.aboutThisSoftwareToolStripMenuItem.Text = "About This Software";
            this.aboutThisSoftwareToolStripMenuItem.Click += new System.EventHandler(this.aboutThisSoftwareToolStripMenuItem_Click);
            // 
            // directoryTree
            // 
            this.directoryTree.CheckBoxes = true;
            this.directoryTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.directoryTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.directoryTree.Location = new System.Drawing.Point(0, 0);
            this.directoryTree.Name = "directoryTree";
            this.directoryTree.Size = new System.Drawing.Size(261, 516);
            this.directoryTree.TabIndex = 0;
            this.directoryTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.directoryTree_AfterCheck);
            this.directoryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treenode_Selected);
            this.directoryTree.DoubleClick += new System.EventHandler(this.directoryTree_DoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.directoryTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(784, 516);
            this.splitContainer1.SplitterDistance = 261;
            this.splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.fileListView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.textBox1);
            this.splitContainer2.Size = new System.Drawing.Size(519, 516);
            this.splitContainer2.SplitterDistance = 350;
            this.splitContainer2.TabIndex = 0;
            // 
            // fileListView
            // 
            this.fileListView.CheckBoxes = true;
            this.fileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnFilename,
            this.columnSize,
            this.columnDate,
            this.columnNotes});
            this.fileListView.ContextMenuStrip = this.fileContextMenu;
            this.fileListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileListView.FullRowSelect = true;
            this.fileListView.Location = new System.Drawing.Point(0, 0);
            this.fileListView.Name = "fileListView";
            this.fileListView.Size = new System.Drawing.Size(519, 350);
            this.fileListView.TabIndex = 0;
            this.fileListView.UseCompatibleStateImageBehavior = false;
            this.fileListView.View = System.Windows.Forms.View.Details;
            this.fileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.fileListView_ColumnClick);
            this.fileListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.fileListView_ItemChecked);
            // 
            // columnFilename
            // 
            this.columnFilename.Text = "Filename";
            this.columnFilename.Width = 261;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size";
            this.columnSize.Width = 89;
            // 
            // columnDate
            // 
            this.columnDate.Text = "Date";
            this.columnDate.Width = 87;
            // 
            // columnNotes
            // 
            this.columnNotes.Text = "Notes";
            this.columnNotes.Width = 77;
            // 
            // fileContextMenu
            // 
            this.fileContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmCheckSelectedFiles,
            this.tsmUncheckSelectedFiles,
            this.toolStripSeparator7,
            this.tsmSelectAllFiles});
            this.fileContextMenu.Name = "fileContextMenu";
            this.fileContextMenu.ShowItemToolTips = false;
            this.fileContextMenu.Size = new System.Drawing.Size(194, 76);
            // 
            // tsmCheckSelectedFiles
            // 
            this.tsmCheckSelectedFiles.Name = "tsmCheckSelectedFiles";
            this.tsmCheckSelectedFiles.Size = new System.Drawing.Size(193, 22);
            this.tsmCheckSelectedFiles.Text = "Check Selected Files";
            this.tsmCheckSelectedFiles.Click += new System.EventHandler(this.tsmCheckSelectedFiles_Click);
            // 
            // tsmUncheckSelectedFiles
            // 
            this.tsmUncheckSelectedFiles.Name = "tsmUncheckSelectedFiles";
            this.tsmUncheckSelectedFiles.Size = new System.Drawing.Size(193, 22);
            this.tsmUncheckSelectedFiles.Text = "Uncheck Selected Files";
            this.tsmUncheckSelectedFiles.Click += new System.EventHandler(this.tsmUncheckSelectedFiles_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(190, 6);
            // 
            // tsmSelectAllFiles
            // 
            this.tsmSelectAllFiles.Name = "tsmSelectAllFiles";
            this.tsmSelectAllFiles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.tsmSelectAllFiles.Size = new System.Drawing.Size(193, 22);
            this.tsmSelectAllFiles.Text = "Select All Files";
            this.tsmSelectAllFiles.Click += new System.EventHandler(this.tsmSelectAllFiles_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Window;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(519, 162);
            this.textBox1.TabIndex = 0;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select Source Folder";
            // 
            // folderBrowserDialog2
            // 
            this.folderBrowserDialog2.Description = "Select Target Folder";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // openFileDialog
            // 
            this.openFileDialog.CheckFileExists = false;
            this.openFileDialog.DefaultExt = "txt";
            this.openFileDialog.FileName = "selection";
            this.openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            this.openFileDialog.Title = "Choose Filename";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Form1";
            this.Text = "Smart Copy Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.fileContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.TreeView directoryTree;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuMove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuChangeSourceFolder;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuRescan;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveMirrored;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveUnmirrored;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteFiles;
        private System.Windows.Forms.ToolStripMenuItem menuExit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolStripMenuItem filtersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuSetFilters;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem menuIncludeHidden;
        private System.Windows.Forms.ToolStripStatusLabel label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.ToolStripMenuItem menuIgnoreSize;
        private System.Windows.Forms.ToolStripMenuItem menuFindOrphans;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem menuAllowOverwrite;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem menuClearFilters;
        private System.Windows.Forms.ListView fileListView;
        private System.Windows.Forms.ColumnHeader columnFilename;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.ColumnHeader columnDate;
        private System.Windows.Forms.ColumnHeader columnNotes;
        private System.Windows.Forms.ContextMenuStrip fileContextMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmSelectAllFiles;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem tsmCheckSelectedFiles;
        private System.Windows.Forms.ToolStripMenuItem tsmUncheckSelectedFiles;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutThisSoftwareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem menuShowFilteredFiles;
        private System.Windows.Forms.ToolStripStatusLabel label2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripDropDownButton statusMenu;
        private System.Windows.Forms.ToolStripMenuItem statusMenuAbort;
        private System.Windows.Forms.ToolStripMenuItem statusMenuPause;
        private System.Windows.Forms.ToolStripMenuItem menuSynchroniseDirectory;
        private System.Windows.Forms.ToolStripMenuItem menuMergeDirectories;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem menuSaveSelection;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem menuRestoreSelection;
        private System.Windows.Forms.ToolStripMenuItem menuExpandSelectedFolders;
        private System.Windows.Forms.ToolStripMenuItem menuUpdateDirectory;
        private System.Windows.Forms.ToolStripMenuItem menuFilterByDate;
        private System.Windows.Forms.ToolStripMenuItem menuIgnoreExtension;
        private System.Windows.Forms.ToolStripMenuItem menuRemoveSelection;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem menuAutoselectFiles;
        private System.Windows.Forms.ToolStripMenuItem menuSelectAllInSelected;
        private System.Windows.Forms.ToolStripMenuItem menuFlatten;
        private System.Windows.Forms.ToolStripMenuItem menuDeleteReadOnly;
    }
}

