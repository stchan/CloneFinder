using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace CloneFinder
{
    /// <summary>
    /// This file processor class finds duplicates by
    /// noting all groups of files with the same length
    /// then computing hashes for those files.
    /// </summary>
    public class FileProcessorLengthFirst : FileProcessorBase, IDisposable
    {
        #region Ctor

        public FileProcessorLengthFirst()
        { }

        #endregion

        #region IDisposable implementation

        private bool disposed = false; // Has IDisposable.Dispose() already been called

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool calledByUserCode)
        {
            if (!this.disposed)
            {
                if (calledByUserCode)
                {
                    // Clean up managed objects here
                    if (this.databaseConnection != null)
                    {
                        try
                        {
                            if (this.databaseConnection.State != ConnectionState.Closed) this.databaseConnection.Close();
                        }
                        catch (SQLiteException)
                        { }
                        if (File.Exists(this.databaseFile)) File.Delete(this.databaseFile);
                        this.databaseConnection.Dispose();
                    }
                }

                // Clean up unmanaged objects here (we don't directly have any, so no need to do anything)

                this.disposed = true;
            }

        }

        /* Destructor is not implemented because
         * we don't have any direct references to local unfinalizable unmanaged resources
         * (Yes I know SQLite has unmanaged code - but
         * let's be optimistic and hope the caller
         * calls dispose on this instance)
        /// <summary>
        /// Destructor for finalizer
        /// </summary>
        ~FileProcessorLengthFirst()
        {
            Dispose(false);
        }
        */

        #endregion

        #region Instance variables

        private String databaseFile = null; // Fully qualified pathname of the SQLite database file
        private SQLiteConnection databaseConnection = null;

        #endregion

        #region SQL statement constants
        private const String sqlFileInformationFullRowInsert = "INSERT INTO FILEINFORMATION (FILEPATH, FILENAME, FILESIZE, LASTMODIFYDATE, FILEHASH) " +
                                                               "VALUES (@FILEPATH, @FILENAME, @FILESIZE, @LASTMODIFYDATE, @FILEHASH);";
        private const String sqlHashDupeSelect = "select FILEPATH, FILENAME, FILESIZE, FILEHASH, LASTMODIFYDATE from FILEINFORMATION where FILEHASH in " +
                                                          "(select FILEHASH from FILEINFORMATION GROUP BY FILEHASH HAVING COUNT(*) > 1);";
        private const String sqlLengthDupeSelect = "select FILEPATH, FILENAME, FILESIZE, ROWID from FILEINFORMATION where FILESIZE in " +
                                                          "(select FILESIZE from FILEINFORMATION WHERE FILESIZE > 0 GROUP BY FILESIZE HAVING COUNT(*) > 1);";
        private const String sqlFilePathParam = "@FILEPATH";
        private const String sqlFileNameParam = "@FILENAME";
        private const String sqlFileSizeParam = "@FILESIZE";
        private const String sqlLastModifyDateParam = "@LASTMODIFYDATE";
        private const String sqlFileHashParam = "@FILEHASH";
        private const String sqlRowIdParamsqlRowIdParam = "@ROWID";

        private const String sqlFileInformationFileHashUpdate = "update FILEINFORMATION set FILEHASH = @FILEHASH where ROWID = @ROWID";

        private const String sqlColumnFilePath = "FILEPATH";
        private const String sqlColumnFileName = "FILENAME";
        private const String sqlColumnFileSize = "FILESIZE";
        private const String sqlColumnLastModifyDate = "LASTMODIFYDATE";
        private const String sqlColumnFileHash = "FILEHASH";

        #endregion

        #region IFileProcessor implementation

        public override void Close()
        {
            //this.Dispose();
            if (this.databaseConnection != null &&
                this.databaseConnection.State != ConnectionState.Closed)
            {
                try
                {
                    this.databaseConnection.Close();
                }
                catch (SQLiteException)
                { }
            }
        }

        /// <summary>
        /// Initializes the SQLite database in a temp
        /// location determined by Windows
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public override bool Initialize()
        {
            return Initialize(null);
        }

        /// <summary>
        /// Initializes the SQLite database
        /// </summary>
        /// <param name="databaseFilePath">The fully qualifed filename to use for the database file</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool Initialize(String databaseFilePath)
        {
            bool initializedOk = false;
            try
            {
                if (String.IsNullOrEmpty(databaseFilePath) ||
                    String.IsNullOrWhiteSpace(databaseFilePath))
                {
                    databaseFilePath = Path.GetTempFileName();
                    this.databaseFile = databaseFilePath;
                }
                File.Delete(databaseFilePath);
                String connectString = String.Format("Data Source={0}; Version=3", databaseFilePath);
                this.databaseConnection = new SQLiteConnection(connectString);
                this.databaseConnection.Open();
                PerformanceTweaks(this.databaseConnection);
                CreateDatabaseObjects(this.databaseConnection);
                initializedOk = true;
            }
            catch (SQLiteException)
            { }
            catch (IOException)
            { }
            catch (UnauthorizedAccessException)
            { }
            return initializedOk; 
        }

        private void PerformanceTweaks(SQLiteConnection databaseConnection)
        {
            // Turn off journaling and allow async writes -
            // databases are temporary so we don't care if we
            // lose data during a crash
            using (SQLiteCommand pragmaCommand = databaseConnection.CreateCommand())
            {
                pragmaCommand.CommandText = "PRAGMA journal_mode = OFF;";
                pragmaCommand.ExecuteNonQuery();
                pragmaCommand.CommandText = "PRAGMA synchronous = OFF;";
                pragmaCommand.ExecuteNonQuery();
            }        
        }

        private void CreateDatabaseObjects(SQLiteConnection databaseConnection)
        {
            using (SQLiteCommand ddlCommand = databaseConnection.CreateCommand())
            {
                ddlCommand.CommandText = "CREATE TABLE FILEINFORMATION (FILEPATH TEXT, FILENAME TEXT, FILESIZE INTEGER, LASTMODIFYDATE TEXT, FILEHASH TEXT);";
                ddlCommand.ExecuteNonQuery();
                ddlCommand.CommandText = "CREATE INDEX IDXFILESIZES ON FILEINFORMATION (FILESIZE);";
                ddlCommand.ExecuteNonQuery();
                ddlCommand.CommandText = "CREATE INDEX IDXFILENAMES ON FILEINFORMATION (FILENAME);";
                ddlCommand.ExecuteNonQuery();
                ddlCommand.CommandText = "CREATE INDEX IDXFILEPATHS ON FILEINFORMATION (FILEPATH);";
                ddlCommand.ExecuteNonQuery();
                ddlCommand.CommandText = "CREATE INDEX IDXFILEHASHES ON FILEINFORMATION (FILEHASH);";
                ddlCommand.ExecuteNonQuery();
            }
        }

        public override void ProcessFile(FileInfo fileInformation)
        {
            using (SQLiteCommand insertCommand = this.databaseConnection.CreateCommand())
            {
                insertCommand.CommandText = sqlFileInformationFullRowInsert;
                insertCommand.Parameters.AddWithValue(sqlFilePathParam, fileInformation.DirectoryName);
                insertCommand.Parameters.AddWithValue(sqlFileNameParam, fileInformation.Name);
                insertCommand.Parameters.AddWithValue(sqlFileSizeParam, fileInformation.Length);
                insertCommand.Parameters.AddWithValue(sqlLastModifyDateParam, fileInformation.LastAccessTime);
                insertCommand.Parameters.AddWithValue(sqlFileHashParam, null);
                insertCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Returns a collection of <see cref="ProcessedFileInfo"/> objects
        /// containing any duplicates found
        /// </summary>
        public override Collection<ProcessedFileInfo> DuplicateFiles
        {
            get { return RetrieveDuplicates(); }
        }


        private Collection<ProcessedFileInfo> RetrieveDuplicates()
        {
            Collection<ProcessedFileInfo> duplicateFiles = new Collection<ProcessedFileInfo>();
            ComputeHashesForLengthDuplicates();
            using (SQLiteCommand selectCommand = this.databaseConnection.CreateCommand())
            {
                selectCommand.CommandText = sqlHashDupeSelect;
                using (SQLiteDataReader hashDupeReader = selectCommand.ExecuteReader())
                {
                    while (hashDupeReader.Read())
                    {
                        ProcessedFileInfo hashDuplicate = new ProcessedFileInfo(hashDupeReader[sqlColumnFilePath] as String,
                                                                                hashDupeReader[sqlColumnFileName] as String,
                                                                                hashDupeReader[sqlColumnFileSize] as long?,
                                                                                Convert.ToDateTime(hashDupeReader[sqlColumnLastModifyDate]) as DateTime?,
                                                                                hashDupeReader[sqlColumnFileHash] as String);
                        duplicateFiles.Add(hashDuplicate);
                    }
                }
            }
            return duplicateFiles;
        }


        private void ComputeHashesForLengthDuplicates()
        {
            using (SQLiteCommand selectCommand = this.databaseConnection.CreateCommand())
            {
                selectCommand.CommandText = sqlLengthDupeSelect;
                using (SQLiteDataReader lengthDupeReader = selectCommand.ExecuteReader())
                {
                    using (SQLiteCommand updateCommand = this.databaseConnection.CreateCommand())
                    {
                        updateCommand.CommandText = sqlFileInformationFileHashUpdate;
                        StringBuilder filePath = new StringBuilder();
                        while (lengthDupeReader.Read())
                        {
                            filePath.Clear();
                            filePath.Append(lengthDupeReader[sqlColumnFilePath] as String);
                            if (!filePath.ToString().EndsWith("\\")) filePath.Append("\\");
                            filePath.Append(lengthDupeReader[sqlColumnFileName] as String);
                            if (File.Exists(filePath.ToString()))
                            {
                                try
                                {
                                    String fileHash = ComputeFileHash(filePath.ToString());
                                    updateCommand.Parameters.Clear();
                                    updateCommand.Parameters.AddWithValue(sqlFileHashParam, fileHash);
                                    updateCommand.Parameters.AddWithValue(sqlRowIdParamsqlRowIdParam, lengthDupeReader["ROWID"]);
                                    updateCommand.ExecuteNonQuery();
                                }
                                catch (IOException)
                                { }
                                catch (UnauthorizedAccessException)
                                { }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Debugging/NUnit support

#if DEBUG

        public String Test_DatabaseFile
        {
            get { return this.databaseFile; }
        }

        
#endif

        #endregion
    }
}
