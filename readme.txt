SmartCopy
Copyright 2011-2016 Simon Booth

SmartCopy is a utility I really never expected to have to write myself.  It’s function is to manipulate files in a large, deep directory structure selectively and (hopefully) intelligently.  Operating systems’ native interfaces all seem to do a poor job of this, and the alternative tools I’ve tried all seem flawed one way or another.  I hope SmartCopy provides an improvement on existing utilities – it certainly fills my needs better than anything I’ve tried.

What are the functions that SmartCopy provides?  It’s basic job is to provide selective backups of a directory structure.  I initially wrote it to help populate a small mp3 player (20gb) from a very large mp3 collection (500gb) – a task which seems unnecessarily difficult using Windows Explorer (for example).  Perhaps I have an uncommonly large media collection, or perhaps I organise it rather more obsessively than others, but it does surprise me that this task does not have a wealth of utilities designed to make it as efficient and painless as possible (I count iTunes as an example of an extremely bad utility for the purpose).

The more general case I use the tool for is maintaining backups.  I backup parts of my collection on to a variety of devices – large stores for permanent backups, portable drives for mobile libraries, etc.  It also useful for general organisation and maintenance of any large collection of files.

An example usage would be maintaining an incremental backup.  The steps involved in this would be:

1.  On startup, select the directory you wish to backup (source).

2.  Use the ‘Filter Mirrored Paths’ option and select your target directory.  This will compare the directories, and filter out any files/folders already present in the target from the source view.

3.  Select all files and folders which remain by checking the root node of the source view (all subfolders and files will automatically be checked)

4.  Use copy selected files to transfer any new files (and any modified files, if ‘Allow Overwrite’ is checked and ‘Ignore Size’ is not)

5.  Use ‘Find Orphans’ to locate any files in the target which are not present in the source (this automatically switches the view to the target folder)

6.  Select the root node and use ‘Delete Selected Files’ to eliminate them.

These steps are common enough as a process that there is a single command in the menu which performs exactly those operations – ‘Synchronise Directory’.  With a little imagination, the individual operations combined with others and filters can be used to perform more interesting operations, e.g. to split a mixed media directory into separate trees for audio and video, or to backup, move or delete only those files and folders which are different in the source from some other directory.

Some advantages of using SmartCopy for these sorts of operations include:

- Comparisons and filtering are done up-front, so you know you are not going to be bothered 2 hours in to a long backup with a prompt asking if you want to overwrite something.
- Long operations can be paused and resumed – useful if you’re doing a backup over the network and need to temporarily free up bandwidth to make a Skype call, for instance.

The original version of the tool was written in Python back in 2007, when cross-platform compatibility seemed important to me.  However, I found that Python’s cross-platform capabilities did not out-weigh the inconvenience of not having Python installed on other machines I wished to copy parts of my media collection on to when I was travelling.  Since I wanted to learn some C# and .NET, I decided that converting SmartCopy to that platform was a useful exercise with a practical end product.  Hence, the latest version is written in C# and targetted for .NET runtime 4.0.


Options
=======

Set filename filters
--------------------
Specify a pattern or patterns for files that SmartCopy will consider during its operations, e.g. "*.mp3" if you want to copy/move/delete only mp3 files.  The default is '*', or all files.

Clear filename filters
----------------------
Sets the filename filters to the default of '*', or all files.

Show filtered files
-------------------
If selected, files which have been filtered by the above settings will be displayed in file lists, but in a pale colour.  Otherwise they will be hidden from view.

Include hidden files
--------------------
If selected, hidden files will be considered by smartcopy during copy/move/delete operations.

Ignore size
-----------
When determining if files are the same in source and target folders, the default assumption is that if they have the same name but different sizes, they are not the same file.  Ignore size if you do not want this to be considered (i.e. it is sufficient that the file exists).

Ignore extension
----------------
When determining if a file in the source also exists in the destination, you can choose to ignore the file extension.  This is useful for, e.g. finding which FLAC files in your lossless archive have not been converted to MP3 in your lossy library, or files that do not have a .zip backup, or... I am sure people will find other uses..  It is probably most useful when combined with the 'Ignore size' option.

Allow overwrite
---------------
Copy and move operations will not overwrite files in the target directory by default.  Check this option if you would like files from the source to be copied and moved even when this will overwrite files in the target.

Autoselect Files on Restore
---------------------------

When restoring a selection, all files in a selected folder will be selected automatically.  Useful if you've renamed a bunch of files, for instance... potentially disastrous if you only actually want to copy/move a selection of files.  I recommend that you don't enable this option unless you have a specific need for it, and it is never saved enabled between sessions.


Operations
==========

Change source folder
--------------------
When SmartCopy is launched you will be asked to choose a folder - this is the source folder.  The directory is scanned recursively to build a file and folder list, which forms the base for all operations.  You can change the source folder using 'Change source folder', which will cause the file list to be rebuilt.

Rescan
------
Rebuilds the file list from the source folder, in case of external changes or when settings have changed.  Any filtered files will be restored.  Currently selected files and folders are automatically saved and restored (if they still exist).

Copy selected files
-------------------
You will be prompted to choose a target folder.  All files (and their containing folders) that have been selected in the main window will be copied to the target, preserving directory structure.

Move selected files
-------------------
You will be prompted to choose a target folder.  All files (and their containing folders) that have been selected in the main window will be moved to the target, preserving directory structure.

If the target folder is in the same directory structure as the source folder (e.g. a subfolder of the source root) then you will receive a warning that some files and folders will not be moved.  Most likely this will just be the target folder itself, which obviously can't be moved whilst you are trying to move other files and folders into it.

Delete selected files
---------------------
All files that have been selected in the main window will be deleted from the source folder.  This is a permanent delete, so handle with care.

Filter mirrored paths
---------------------
You will be prompted to choose a target directory.  All files that are present in the target directory will be removed from the file list (this does not delete the files, just prevents SmartCopy from considering them).

The settings for 'ignore size' and 'ignore extension' are respected by the operation.

Filter unmirrored paths
-----------------------
You will be prompted to choose a target directory.  All files that are NOT present in the target directory will be removed from the file list (this does not delete the files, just prevents SmartCopy from considering them)

The settings for 'ignore size' and 'ignore extension' are respected by the operation.

Filter by date
--------------
You will be prompted to select a date range, and any files outside that range will be removed from the file list (this does not delete the files, just prevents SmartCopy from considering them)

Find orphans
------------
You will be prompted to choose a target directory.  The current file list will be replaced with any files in the target directory that are not in the source.  

The settings for 'ignore size' and 'ignore extension' are respected by the operation.

This is a composite operation of 'change source folder' followed by 'filter mirrored paths' (using the original source as the new target).

After this operation, the target directory becomes the new source directory, and the previous source directory is automatically set as the new target directory.

Update directory
----------------
You will be prompted to choose a target directory.  Any files and folders in the source directory that are not already in the target will be copied into it.

The settings for 'ignore size', 'ignore extension' and 'allow overwrite' are respected by the operation.  The current file selection is not considered, all missing files are copied.

This is a composite operation of 'filter mirrored paths' + 'select all' + 'copy selected files'.

Synchronise directory
---------------------
You will be prompted to choose a target directory.  Any files and folders in the source directory that are not already in the target will be copied into it, and any files that are in the target directory but not in the source will be deleted.

The settings for 'ignore size', 'ignore extension' and 'allow overwrite' are respected by the operation.  The current file selection is not considered, all missing files are copied.

This is a composite operation of 'filter mirrored paths' + 'select all' + 'copy selected files' + 'find orphans' + 'select all' + 'delete selected files'.

Merge directories
-----------------
You will be prompted to choose a target directory.  Any files and folders in the source directory that are not already in the target will be copied into it, and any files that are in the target directory but not in the source will be copied in the opposite direction.

The settings for 'ignore size', 'ignore extension' and 'allow overwrite' are respected by the operation.  The current file selection is not considered, all missing files are copied.

This is a composite operation of 'filter mirrored paths' + 'select all' + 'copy selected files' + 'find orphans' + 'select all' + 'copy selected files'.

Save selection to text file
---------------------------
Writes the current selection out to a text file.  The save dialog includes the option to save as a plain text file (just a list of filenames) or in extended format as an m3u or m3u8 file.  These extended files can be used as playlists in foobar2000, winamp, itunes etc.  The file paths are stored relative to the location where the file is saved.

Restore selection from text file
--------------------------------
Restores the file selection from a previously saved text file.  Any files that are no longer present in the file list will be ignored.  Since selections are saved as relative paths, the current source directory need not be the same directory from which the selection was saved, if it has any files and folders in common.

Remove selection from text file
-------------------------------
The inverse of "restore selection from text file" - deselects a previously saved selection.

Expand selected folders
-----------------------
Any folders which are currently selected or contain selected files will be expanded in the file list, making it easier to see what has been selected.




