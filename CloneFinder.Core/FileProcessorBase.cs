﻿using System;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;

namespace CloneFinder.Core
{
    public abstract class FileProcessorBase : IFileProcessor
    {


        #region Public Events

        #endregion

        #region IFileProcessor implementation (all should be virtual)

        public virtual void Close()
        { }
            
        public virtual bool Initialize()
        { return false; }

        public virtual void ProcessFile(FileInfo fileInformation)
        {
            fileInformation = null;
        }

        public virtual Collection<ProcessedFileInfo> DuplicateFiles
        {
            get { return null; }
        }

        #endregion

        protected virtual String ComputeFileHash(String filePath)
        {
            String fileHashAsString = null;
            using (StreamReader fileStream = new StreamReader(filePath))
            {
                using (SHA512 hashProvider = SHA512.Create())
                {
                    fileHashAsString = BitConverter.ToString(hashProvider.ComputeHash(fileStream.BaseStream));
                }
            }
            return fileHashAsString;
        }
    
    }
}
