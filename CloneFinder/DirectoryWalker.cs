using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace CloneFinder
{
    /// <summary>
    /// This class contains methods to walk a directory,
    /// process the files within, and
    /// return any duplicates found
    /// </summary>
    public class DirectoryWalker
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        public DirectoryWalker()
        {

        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="rootDirectory">Path of the directory to start the walk from</param>
        public DirectoryWalker(string rootDirectory) : this()
        {
            if (!String.IsNullOrEmpty(rootDirectory) &&
                !String.IsNullOrWhiteSpace(rootDirectory))
            {
                this.directoryRoot = rootDirectory;
            }
            else
            {
                throw new ArgumentNullException("rootDirectory", exceptionMessageInvalidRootDirectory);
            }
        }

        #endregion

        #region Class constants

        private const String exceptionMessageInvalidRootDirectory = "Root directory is null or invalid.";
        private const String exceptionMessageDirectoryChangedDuringWalk = "Root directory cannot be changed while a walk is in progress.";

        #endregion

        #region Public events

        /*
        /// <summary>
        /// This event is raised when a directory is accessed
        /// </summary>
        public event EventHandler<DirectoryWalkEventArgs> DirectoryAccessed;
        protected virtual void OnDirectoryAccessed(DirectoryWalkEventArgs e)
        {
            if (DirectoryAccessed != null)
                DirectoryAccessed(this, e);
        }
        */

        /// <summary>
        /// This event is raised when a file/directory is accessed
        /// </summary>
        public event EventHandler<DirectoryWalkEventArgs> FileAccessed;
        protected virtual void OnFileAccessed(DirectoryWalkEventArgs e)
        {
            if (FileAccessed != null)
                FileAccessed(this, e);
        }

        /// <summary>
        /// This event raised when a file/directory is processed
        /// </summary>
        public event EventHandler<DirectoryWalkEventArgs> FileProcessed;
        protected virtual void OnFileProcessed(DirectoryWalkEventArgs e)
        {
            if (FileProcessed != null)
                FileProcessed(this, e);
        }

        /// <summary>
        /// This event raised when a directory walk is started
        /// </summary>
        public event EventHandler<DirectoryWalkEventArgs> DirectoryWalkStarted;
        protected virtual void OnDirectoryWalkStarted(DirectoryWalkEventArgs e)
        {
            if (DirectoryWalkStarted != null)
                DirectoryWalkStarted(this, e);
        }

        /// <summary>
        /// This event raised when all files/subdirectories have been processed
        /// </summary>
        public event EventHandler<DirectoryWalkEventArgs> DirectoryWalkComplete;
        protected virtual void OnDirectoryWalkCompleted(DirectoryWalkEventArgs e)
        {
            if (DirectoryWalkComplete != null)
                DirectoryWalkComplete(this, e);
        }

        #endregion

        #region Public Properties

        String directoryRoot = null;
        /// <summary>
        /// Root directory to start the walk from
        /// (Cannot be changed once a directory walk is started -
        /// an <see cref="InvalidOperationException"/> will be
        /// thrown if changed after a walk is started)
        /// </summary>
        public String RootDirectory
        {
            get
            {
                return this.directoryRoot;
            }
            set
            {
                if (this.walkInProgress == false)
                {
                    this.directoryRoot = value;
                }
                else
                {
                    throw new InvalidOperationException(exceptionMessageDirectoryChangedDuringWalk);
                }
            }
        }

        bool walkInProgress = false;
        /// <summary>
        /// Is a directory walk currently in progress (true/false)
        /// </summary>
        public bool WalkInProgress
        {
            get { return this.walkInProgress; }
        }

        #endregion


        /// <summary>
        /// Performs a directory traversal
        /// </summary>
        /// <returns>A collection of <see cref="ProcessedFileInfo"/> objects
        ///          containing any duplicate files found</returns>
        public Collection<ProcessedFileInfo> WalkDirectory()
        {
            Collection<ProcessedFileInfo> duplicates = null;
            if (!String.IsNullOrEmpty(this.directoryRoot) &&
                !String.IsNullOrWhiteSpace(this.directoryRoot) &&
                Directory.Exists(this.directoryRoot))
            {
                this.walkInProgress = true;
                duplicates = BreadthFirstTraversal(this.directoryRoot);
            }
            else
            {
                this.walkInProgress = false;
                throw new InvalidOperationException(exceptionMessageInvalidRootDirectory);
            }
            return duplicates;
        }


        /// <summary>
        /// Does the actual work of iterating the folder tree.
        /// This is a breadth-first traversal.
        /// Code was adapted from the MSDN documentation for
        /// walking an NTFS directory.
        /// </summary>
        /// <param name="rootFolder">Folder to start the traversal from</param>
        private Collection<ProcessedFileInfo> BreadthFirstTraversal(String rootFolder)
        {
            
            // Data structure to hold names of subfolders to be
            // examined for files.
            Stack<string> dirs = new Stack<string>(20);

            if (System.IO.Directory.Exists(rootFolder))
            {
                // 
                OnDirectoryWalkStarted(new DirectoryWalkEventArgs(rootFolder,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.NotApplicable,
                                                                  "Directory walk started."));

                dirs.Push(rootFolder);
                FileProcessorLengthFirst lengthFirstFileProc = new FileProcessorLengthFirst();
                IFileProcessor fileProcessorInterface = lengthFirstFileProc as IFileProcessor;
                fileProcessorInterface.Initialize();
                while (dirs.Count > 0)
                {
                    string currentDir = dirs.Pop();
                    string[] subDirs;
                    try
                    {
                        subDirs = System.IO.Directory.GetDirectories(currentDir);
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                          String.Empty,
                                                                          DirectoryWalkObjectType.Directory,
                                                                          DirectoryWalkOperationResult.Success,
                                                                          "Directory listing (subdirectories) taken successfully."));
                    }
                    // An UnauthorizedAccessException exception will be thrown if we do not have
                    // discovery permission on a folder or file. It may or may not be acceptable 
                    // to ignore the exception and continue enumerating the remaining files and 
                    // folders. It is also possible (but unlikely) that a DirectoryNotFound exception 
                    // will be raised. This will happen if currentDir has been deleted by
                    // another application or thread after our call to Directory.Exists. The 
                    // choice of which exceptions to catch depends entirely on the specific task 
                    // you are intending to perform and also on how much you know with certainty 
                    // about the systems on which this code will run.
                    catch (UnauthorizedAccessException)
                    {
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.Failure,
                                                                  "Directory listing (subdirectories) could not be taken - insufficient permissions."));
                        continue;
                    }
                    catch (System.IO.DirectoryNotFoundException)
                    {
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.Error,
                                                                  "Directory listing (subdirectories) could not be taken - directory was deleted or moved."));
                        continue;
                    }

                    string[] files = null;
                    try
                    {
                        files = System.IO.Directory.GetFiles(currentDir);
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                          String.Empty,
                                                                          DirectoryWalkObjectType.Directory,
                                                                          DirectoryWalkOperationResult.Success,
                                                                          "Directory listing (files) taken successfully."));
                    }

                    catch (UnauthorizedAccessException)
                    {
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.Failure,
                                                                  "Directory listing (files) could not be taken - insufficient permissions."));
                        continue;
                    }

                    catch (System.IO.DirectoryNotFoundException)
                    {
                        OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.Error,
                                                                  "Directory listing (files) could not be taken - directory was deleted or moved."));
                        continue;
                    }
                    // Perform the required action on each file here.
                    // Modify this block to perform your required task.
                    foreach (string file in files)
                    {
                        try
                        {
                            OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                       Path.GetFileName(file),
                                                                       DirectoryWalkObjectType.File,
                                                                       DirectoryWalkOperationResult.NotApplicable,
                                                                       "Accessing file."));
                            System.IO.FileInfo fi = new System.IO.FileInfo(file);
                            fileProcessorInterface.ProcessFile(fi);
                            OnFileProcessed(new DirectoryWalkEventArgs(currentDir,
                                                                       Path.GetFileName(file),
                                                                       DirectoryWalkObjectType.File,
                                                                       DirectoryWalkOperationResult.Success,
                                                                       "File processed."));
                        }
                        catch (System.IO.FileNotFoundException)
                        {
                            // If file was deleted by a separate application
                            //  or thread since the call to TraverseTree()
                            // then just continue.
                            OnFileAccessed(new DirectoryWalkEventArgs(currentDir,
                                                                      String.Empty,
                                                                      DirectoryWalkObjectType.Directory,
                                                                      DirectoryWalkOperationResult.Error,
                                                                      "File could not be processed - it was deleted or moved."));
                            continue;
                        }
                    }

                    // Push the subdirectories onto the stack for traversal.
                    // This could also be done before handing the files.
                    foreach (string str in subDirs)
                        dirs.Push(str);
                }
                OnDirectoryWalkCompleted(new DirectoryWalkEventArgs(rootFolder,
                                                                  String.Empty,
                                                                  DirectoryWalkObjectType.Directory,
                                                                  DirectoryWalkOperationResult.NotApplicable,
                                                                  "Directory walk completed - computing hashes for length duplicates..."));
                Collection<ProcessedFileInfo> duplicateFiles = lengthFirstFileProc.DuplicateFiles;
                fileProcessorInterface.Close();
                return duplicateFiles;
            }
            else
            {
                // Root folder does not exist
                throw new ArgumentException(String.Empty,"rootFolder");
            }
        } //BreadthFirstTraversal


        
    }
}
