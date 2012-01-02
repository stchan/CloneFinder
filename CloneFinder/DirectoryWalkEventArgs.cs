using System;
using System.Collections.Generic;
using System.Text;

namespace CloneFinderCore
{
    public enum DirectoryWalkObjectType
    {
        Undefined = 0,
        File,
        Directory
    }

    public enum DirectoryWalkOperationResult
    {
        NotApplicable = 0,
        Success,
        Failure,
        Error
    }

    public class DirectoryWalkEventArgs : EventArgs
    {

        #region Ctor

        public DirectoryWalkEventArgs(String fileSystemObjectPath,
                                      String fileSystemObjectName,
                                      DirectoryWalkObjectType fileSystemObjectType,
                                      DirectoryWalkOperationResult resultCode,
                                      String message)
        {
            this.objectFullPath = fileSystemObjectPath;
            this.objectName = fileSystemObjectName;
            this.objectDirectoryWalkType = fileSystemObjectType;
            this.operationResultCode = resultCode;
            this.eventMessage = message;
        }

        #endregion

        #region Public properties

        private String objectFullPath;
        public String FullPath
        { 
            get { return this.objectFullPath; } 
        }

        private String objectName;
        public String Name
        {
            get { return this.objectName; }
        }

        private DirectoryWalkObjectType objectDirectoryWalkType;
        public DirectoryWalkObjectType ObjectType
        {
            get { return this.objectDirectoryWalkType; }
        }

        private DirectoryWalkOperationResult operationResultCode;
        public DirectoryWalkOperationResult Result
        {
            get { return this.operationResultCode; }
        }

        private String eventMessage;
        public String Message
        {
            get { return this.eventMessage; }
        }

        #endregion

    }
}
