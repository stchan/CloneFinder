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

        const String reportMessageComputingHashes = "Computing hashes for potential duplicates...";
        const String reportMessageHashCode = "Files with Hash {0}:";
        const String reportMessageNoDuplicates = "No duplicates found.";

        const String reportMessageCSVOutput = "\"{0}\", \"{1}\"";

        public void FindDuplicates(Options commandLineOptions)
        {
            CloneFinderCore.DirectoryWalker duplicateWalker = new CloneFinderCore.DirectoryWalker(commandLineOptions.Items[0]);
            if (commandLineOptions.progressIndicator)
            {
                duplicateWalker.FileProcessed += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(duplicateWalker_FileProcessed);
                duplicateWalker.DirectoryWalkComplete += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(duplicateWalker_DirectoryWalkComplete);
            }
            Collection<CloneFinderCore.ProcessedFileInfo> duplicateFiles = duplicateWalker.WalkDirectory();
            WriteResults(duplicateFiles, commandLineOptions.csvOutput);
        }

        void duplicateWalker_DirectoryWalkComplete(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            Console.WriteLine(reportMessageComputingHashes);
        }

        private void duplicateWalker_FileProcessed(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            Console.WriteLine("Processed: " + e.Name);
        }

        private void WriteResults(Collection<CloneFinderCore.ProcessedFileInfo> duplicateFiles, bool writeAsCSV)
        {
            if (duplicateFiles.Count > 0)
            {
                String lastHash = String.Empty;
                for (int loop = 0; loop < duplicateFiles.Count; loop++)
                {
                    if (!writeAsCSV)
                    {
                        // Standard output format
                        if (lastHash != duplicateFiles[loop].FileHash)
                        {
                            lastHash = duplicateFiles[loop].FileHash;
                            Console.WriteLine();
                            Console.WriteLine(String.Format(reportMessageHashCode, lastHash));
                        }
                        Console.WriteLine(Path.Combine(duplicateFiles[loop].FilePath, duplicateFiles[loop].Name));
                    }
                    else
                    {
                        // Comma separated output requested
                        Console.WriteLine(String.Format(reportMessageCSVOutput,
                                                        Path.Combine(duplicateFiles[loop].FilePath, duplicateFiles[loop].Name),
                                                        duplicateFiles[loop].FileHash));
                    }
                }
            }
            else
            {
                Console.WriteLine(reportMessageNoDuplicates);
            }
        }

    }
}
