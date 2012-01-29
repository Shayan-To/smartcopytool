SmartCopy
Copyright 2011-2012 Simon Booth

SmartCopy is a utility I really never expected to have to write myself.  It’s function is to manipulate files in a large, deep directory structure selectively and (hopefully) intelligently.  Operating systems’ native interfaces all seem to do a poor job of this, and the alternative tools I’ve tried all seem flawed one way or another.  I hope SmartCopy provides an improvement on existing utilities – it certainly fills my needs better than anything I’ve tried.

What are the functions that SmartCopy provides?  It’s basic job is to provide selective backups of a directory structure.  I initially wrote it to help populate a small mp3 player (20gb) from a very large mp3 collection (500gb) – a task which seems unnecessarily difficult using Windows Explorer (for example).  Perhaps I have an uncommonly large media collection, or perhaps I organise it rather more obsessively than others, but it does surprise me that this task does not have a wealth of utilities designed to make it as efficient and painless as possible (I count iTunes as an example of an extremely bad utility for the purpose).

The more general case I use the tool for is maintaining backups.  I backup parts of my collection on to a variety of devices – large stores for permanent backups, portable drives for mobile libraries, etc.  It also useful for general organisation and maintenance of any large collection of files.

An example usage would be maintaining an incremental backup.  The steps involved in this would be:

1.  On startup, select the directory you wish to backup (source).

2.  Use the ‘Remove Mirrored Paths’ option and select your target directory.  This will compare the directories, and filter out any files/folders already present in the target from the source view.

3.  Select all files and folders which remain by checking the root node of the source view (all subfolders and files will automatically be checked)

4.  Use copy selected files to transfer any new files (and any modified files, if ‘Allow Overwrite’ is checked and ‘Ignore Size’ is not)

5.  Use ‘Find Orphans’ to locate any files in the target which are not present in the source (this automatically switches the view to the target folder)

6.  Select the root node and use ‘Delete Selected Files’ to eliminate them.

These steps are common enough as a process that there is a single command in the menu which performs exactly those operations – ‘Synchronise Directory’.  With a little imagination, the individual operations combined with others and filters can be used to perform more interesting operations, e.g. to split a mixed media directory into separate trees for audio and video, or to backup, move or delete only those files and folders which are different in the source from some other directory.

Some advantages of using SmartCopy for these sorts of operations include:

- Comparisons and filtering are done up-front, so you know you are not going to be bothered 2 hours in to a long backup with a prompt asking if you want to overwrite something.
- Long operations can be paused and resumed – useful if you’re doing a backup over the network and need to temporarily free up bandwidth to make a Skype call, for instance.

The original version of the tool was written in Python back in 2007, when cross-platform compatibility seemed important to me.  However, I found that Python’s cross-platform capabilities did not out-weigh the inconvenience of not having Python installed on other machines I wished to copy parts of my media collection on to when I was travelling.  Since I wanted to learn some C# and .NET, I decided that converting SmartCopy to that platform was a useful exercise with a practical end product.  Hence, the latest version is written in C# and targetted for .NET runtime 4.0.