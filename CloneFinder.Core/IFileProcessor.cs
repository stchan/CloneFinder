using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace CloneFinder.Core
{
    public interface IFileProcessor
    {
        bool Initialize();
        void Close();
        void ProcessFile(FileInfo fileInformation);
        Collection<ProcessedFileInfo> DuplicateFiles { get; }
    }
}
