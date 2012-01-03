using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;


namespace CloneFinder
{
    public class RequestProcessor
    {

        #region Ctor

        public RequestProcessor()
        { }

        #endregion

        const String reportMessageHashCode = "Files with Hash {0}:";
        const String reportMessageNoDuplicates = "No duplicates found.";

        public void FindDuplicates(Options commandLineOptions)
        {
            CloneFinderCore.DirectoryWalker duplicateWalker = new CloneFinderCore.DirectoryWalker(commandLineOptions.Items[0]);
            if (commandLineOptions.progressIndicator) duplicateWalker.FileProcessed += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(duplicateWalker_FileProcessed);
            Collection<CloneFinderCore.ProcessedFileInfo> duplicateFiles = duplicateWalker.WalkDirectory();
            WriteResults(duplicateFiles);
        }

        private void duplicateWalker_FileProcessed(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            Console.WriteLine("Processed: " + e.Name);
        }

        private void WriteResults(Collection<CloneFinderCore.ProcessedFileInfo> duplicateFiles)
        {
            if (duplicateFiles.Count > 0)
            {
                String lastHash = String.Empty;
                for (int loop = 0; loop < duplicateFiles.Count; loop++)
                {
                    if (lastHash != duplicateFiles[loop].FileHash)
                    {
                        lastHash = duplicateFiles[loop].FileHash;
                        Console.WriteLine();
                        Console.WriteLine(String.Format(reportMessageHashCode, lastHash));
                    }
                    Console.WriteLine(duplicateFiles[loop].FilePath + duplicateFiles[loop].Name);
                }
            }
            else
            {
                Console.WriteLine(reportMessageNoDuplicates);
            }
        }

    }
}
