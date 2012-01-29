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
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SmartCopyTool
{
    /// <summary>
    /// Class to manage data about a node in the directory tree.
    /// </summary>
    public class FolderData
    {
        [Flags]
        enum FolderFlags
        {
            ContainsCheckedFiles    = ( 1 << 0 ),       // There are checked files in this folder
            ContainsUncheckedFiles  = ( 1 << 1 ),       // There are unchecked files in this folder (not mutually exclusive)
            ContainsRemovedFiles    = ( 1 << 2 ),       // Contains file that have been removed by operation such as 'Remove Mirrored'
            ContainsDeletedFiles    = ( 1 << 3 )        // Contains files that have been deleted
        }

        private DirectoryInfo dir = null;
        private List<FileData> myFiles = null;
        private List<FileData> mySelectedFiles = null;
        private Options myOptions = null;
        private FolderFlags flags;

        public string Name { get { return dir.Name; } }
        public string FullName { get { return dir.FullName; } }

        private DirectoryInfo Directory { get { return dir; } }

        public int NumDirectories { get { return dir.EnumerateDirectories().Count(); } }
        public int NumFiles { get { return dir.EnumerateFiles().Count(); } }

        public override string ToString() { return dir.ToString(); }

        public FolderData( string folderPath )
        {
            dir = new DirectoryInfo( folderPath );
        }

        public FolderData( DirectoryInfo dir )
        {
            this.dir = dir;
        }

        // Properties to encapsulate the flags
        public bool ContainsCheckedFiles {
            get { return ( flags & FolderFlags.ContainsCheckedFiles ) != 0; }
            set { if ( value ) flags |= FolderFlags.ContainsCheckedFiles; else flags &= ~FolderFlags.ContainsCheckedFiles; 
                 InvalidateSelectedFiles(); }
        }

        public bool ContainsUncheckedFiles {
            get { return ( flags & FolderFlags.ContainsUncheckedFiles ) != 0; }
            set { if ( value ) flags |= FolderFlags.ContainsUncheckedFiles; else flags &= ~FolderFlags.ContainsUncheckedFiles; 
                 InvalidateSelectedFiles(); }
        }

        public bool ContainsRemovedFiles {
            get { return ( flags & FolderFlags.ContainsRemovedFiles ) != 0; }
            set { if ( value ) flags |= FolderFlags.ContainsRemovedFiles; else flags &= ~FolderFlags.ContainsRemovedFiles; 
                 InvalidateSelectedFiles(); }
        }

        public bool ContainsDeletedFiles {
            get { return ( flags & FolderFlags.ContainsDeletedFiles ) != 0; }
            set { if ( value ) flags |= FolderFlags.ContainsDeletedFiles; else flags &= ~FolderFlags.ContainsDeletedFiles; 
                 InvalidateSelectedFiles(); }
        }

        /// <summary>
        /// Access all the files in the directory
        /// </summary>
        /// <returns></returns>
        public List<FileData> GetFiles()
        {
            if ( myFiles == null )
            {
                myFiles = new List<FileData>();
                foreach ( FileInfo file in dir.GetFiles() )
                {
                    myFiles.Add( new FileData( file, this ) );
                }
                
                // Sort, out of politeness
                myFiles.Sort( (a,b) => a.Name.CompareTo( b.Name ) );
            }
            else if ( ContainsDeletedFiles )
            {
                // Deleting elements from the list is not very thread-friendly, so build a new list
                // and return that next time somebody asks (the old list will remain around as long as anybody's referencing it)
                List<FileData> newList = new List<FileData>();
                foreach ( FileData file in myFiles)
                {
                    if ( file.Deleted == false )
                    {
                        newList.Add( file );
                    }                    
                }
                myFiles = newList;
                ContainsDeletedFiles = false;
            }
            return myFiles;
        }

        public int NumSelectedFiles( Options options )
        {
            return GetSelectedFiles( options ) == null ? 0 : mySelectedFiles.Count;
        }


        /// <summary>
        /// Call if the filtered file list needs to be updated.
        /// </summary>
        internal void InvalidateSelectedFiles()
        {
            mySelectedFiles = null;
        }

        /// <summary>
        /// Return a list of files in a directory that have been selected (one way or another) by the user.
        /// Note that this is cached in the filteredFiles member until the options change or the list is invalidated.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public List<FileData> GetSelectedFiles( Options options )
        {
            if ( dir.Exists == false )
                return null;

            // Hmmm, want to compare options by VALUE, not reference
            // TODO: hmmm... adding a check to each file does make the cached list hard to maintain
            if ( mySelectedFiles != null )
            {
                if ( options.includeHidden == myOptions.includeHidden && options.Filters == myOptions.Filters )
                {
                    return mySelectedFiles;
                }                
            }

            try
            {
                List<FileData> files = new List<FileData>();        // List isn't an efficient container... must be a better way to do this anyway

                foreach ( FileData file in GetFiles() )
                {
                    file.Filtered = false;

                    // Check if hidden files should be included or not
                    if (( !options.includeHidden && ( file.Attributes & FileAttributes.Hidden ) != 0 ) ||
                        ( options.filtersRegex != null && options.filtersRegex.IsMatch( file.Name ) == false )
                        )
                    {
                        file.Filtered = true;
                        continue;
                    }

                    // Unchecked and removed files are not included in the list but should not be flagged as filtered
                    // or we won't be able to check them again if they become unfiltered
                    if ( file.Deleted || file.Removed || !file.Checked  )
                        continue;

                    files.Add( file );
                }

                mySelectedFiles = files;
                myOptions = new Options( options );
            }
            catch ( Exception )
            {
                // Could not access directory... probably system folder
                mySelectedFiles = null;
            }

            return mySelectedFiles;
        }

        /// <summary>
        /// Get the total size of all filtered files
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public long GetSelectedFilesTotalSize( Options options )
        {
            if ( GetSelectedFiles( options ) == null )
                return 0;

            // Must be a funky way to do this with delegates?
            long size = 0;
            foreach ( FileData f in mySelectedFiles )
            {
                size += f.Length;
            }
            return size;
        }

        /// <summary>
        /// Check whether some files in this folder have been filtered out
        /// </summary>
        /// <returns></returns>
        public bool HasSelectedFiles( Options options )
        {
            if ( GetSelectedFiles( options ) == null )
                return false;
            int filtered = mySelectedFiles.Count;
            int total = GetFiles().Count(); // should cache this if system doesn't
            return filtered != total;                
        }

        /// <summary>
        /// Check to see whether file is mirrored in target folder
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destination"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        static public bool FileIsMirroredAt( FileData file, string destination, Options options )
        {
            string target = Path.Combine( destination, file.Name );

            // Check if file exists (should check size too)
            if ( File.Exists( target ) == false )
            {
                return false;
            }
            else if ( options.ignoreSize == false )
            {
                FileData targetFile = new FileData( target, null );
                if ( targetFile.Length != file.Length )
                {
                    file.Notes = String.Format( "Size mismatch: {0} -> {1}", HumanReadable.Size( file.Length ), HumanReadable.Size( targetFile.Length ) );
                    return false;
                }
            }

            return true;
        }

    }    // END OF FolderData


    /// <summary>
    /// Class to store data about files - get a little bit more sophisticated than FileInfo allows
    /// </summary>
    public class FileData
    {
        [Flags]
        enum FileFlags
        {
            Checked     = ( 1 << 0 ),       // User has chosen to include or exclude this file
            Filtered    = ( 1 << 1 ),       // File has been filtered out by options
            Removed     = ( 1 << 2 ),       // File has been removed by operation such as 'Remove Mirrored'
            Deleted     = ( 1 << 3 )        // File has been moved or deleted
        }

        private FileInfo file;
        private FileFlags flags;
        private FolderData folder;

        public bool Checked {
            get { return ( flags & FileFlags.Checked ) != 0; }
            set {
                if ( value != Checked )
                {
                    if ( value == true )
                    {
                        flags |= FileFlags.Checked;
                        if ( folder != null )
                            folder.ContainsCheckedFiles = true;
                    }
                    else
                    {
                        flags &= ~FileFlags.Checked;
                        if ( folder != null )
                            folder.ContainsUncheckedFiles = true;
                    }
                }
            }
        }

        public bool Filtered {
            get { return ( flags & FileFlags.Filtered ) != 0; }
            set { if ( value ) flags |= FileFlags.Filtered; else flags &= ~FileFlags.Filtered; 
                 if ( folder != null ) 
                     folder.InvalidateSelectedFiles(); 
            }
        }

        public bool Removed
        {
            get { return ( flags & FileFlags.Removed ) != 0; }
            set { if ( value ) flags |= FileFlags.Removed; else flags &= ~FileFlags.Removed; 
                 if ( folder != null ) 
                     folder.ContainsRemovedFiles = true; 
            }
        }

        public bool Deleted
        {
            get { return ( flags & FileFlags.Deleted ) != 0; }
            set { if ( value ) flags |= FileFlags.Deleted; else flags &= ~FileFlags.Deleted; 
                 if ( folder != null ) 
                     folder.ContainsDeletedFiles = true; 
            }
        }

        public bool Selected
        {
            get { return ( Checked & !Filtered & !Removed & !Deleted ); }
        }

        public bool Hidden
        {
            get { return ( file.Attributes & FileAttributes.Hidden ) != 0; }
        }

        public string Notes;

        public string Name { get { return file.Name; } }
        public string FullName { get { return file.FullName; } }
        public long Length { get { return file.Length; } }
        public FileAttributes Attributes { get { return file.Attributes; } }
        public DateTime CreationTime { get { return file.CreationTime; } }
        public string DirectoryName { get { return file.DirectoryName; } }

        public override string ToString() { return file.ToString(); }

        public FileData( FileInfo info, FolderData folder )
        {
            this.file = info;
            this.folder = folder;
            Checked = false;
        }

        public FileData( string path, FolderData folder )
        {
            this.file = new FileInfo( path );
            this.folder = folder;
            Checked = false;
        }

        public static implicit operator FileInfo( FileData file )
        {
            return file.file;
        }

    } // End of FileData


    /// <summary>
    /// Represents a wildcard running on the
    /// <see cref="System.Text.RegularExpressions"/> engine.
    /// Based on code by reinux at CodeProject - http://www.codeproject.com/Members/reinux
    /// </summary>
    public class Wildcard
    {
        /// <summary>
        /// Converts a wildcard to a regex.
        /// </summary>
        /// <param name="pattern">The wildcard pattern to convert.</param>
        /// <returns>A regex equivalent of the given wildcard.</returns>
        public static Regex WildcardToRegex( string pattern, RegexOptions options = RegexOptions.IgnoreCase )
        {
            return new Regex( "^(" + Regex.Escape( pattern ).
             Replace( "\\*", ".*" ).
             Replace( "\\?", "." ).
             Replace( ";", "|" ) + 
             ")$", options );
        }
    }
}