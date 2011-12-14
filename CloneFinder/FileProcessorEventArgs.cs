using System;
using System.Collections.Generic;
using System.Text;

namespace CloneFinder
{
    public enum FileProcessorOperationResult
    {
        NotApplicable = 0,
        Success,
        Failure,
        Error
    }

    public class FileProcessorEventArgs : EventArgs
    {
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


        private FileProcessorOperationResult operationResultCode;
        public FileProcessorOperationResult Result
        {
            get { return this.operationResultCode; }
        }

        private String eventMessage;
        public String Message
        {
            get { return this.eventMessage; }
        }

        private int? totalFiles = null;
        public int? FileCount
        {
            get { return this.totalFiles; }
        }

        #endregion

    }
}