using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloneFinderCore
{
    public class ProcessedFileInfo
    {

        #region Ctor
        public ProcessedFileInfo()
        {

        }

        public ProcessedFileInfo(String path,
                                 String name,
                                 long? length,
                                 DateTime? modificationTime,
                                 String hash)
        {
            this.fileFullPath = path;
            this.fileName = name;
            this.fileLength = length;
            this.fileModificationTime = modificationTime;
            this.fileHash = hash;
        }
        #endregion

        #region Public Properties

        private string fileFullPath;
        /// <summary>
        /// The fully qualified path, without the filename
        /// </summary>
        public String FilePath
        {
            get { return this.fileFullPath; }
            set { this.fileFullPath = value; }
        }

        private string fileName;
        /// <summary>
        /// The filename, including extension
        /// </summary>
        public String Name
        {
            get { return this.fileName; }
            set { this.fileName = value; }
        }

        private long? fileLength;
        /// <summary>
        /// File length, in bytes
        /// </summary>
        public long? Length
        {
            get { return this.fileLength; }
            set { this.fileLength = value; }
        }

        private DateTime? fileModificationTime;
        /// <summary>
        /// File's last modified date/time
        /// </summary>
        public DateTime? LastModified
        {
            get { return this.fileModificationTime; }
            set { this.fileModificationTime = value; }
        }

        private String fileHash;
        /// <summary>
        /// File hash value, as a hex string
        /// </summary>
        public String FileHash
        {
            get { return this.fileHash; }
            set { this.fileHash = value; }
        }
        #endregion

    }

}
