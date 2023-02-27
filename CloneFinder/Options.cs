using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text;

namespace CloneFinder
{
    public interface IOptions
    {
        public bool CsvOutput { get; set; }
        public bool ProgressIndicator { get; set; }
        public IEnumerable<string> SearchPath { get; set; }
        public static IEnumerable<Example> Examples { get; }
        public bool Validates();
    }

    public class Options : IOptions
    {
        #region Constants

        const String messagePathNotFound = "{0} not found, or not accessible.";
        const String messageNoPathSpecified = "No path specified.";
        const String messageUnexpectedInternalError = "Unexpected internal error - exiting.";

        #endregion

        #region Ctor

        public Options()
        { }

        #endregion

        [Option('c', "csv", Required = false, HelpText = "Output results in CSV format.")]
        public bool CsvOutput { get; set; }

        [Option('p', "progress", Required = false, HelpText = "Show progress indicator.")]
        public bool ProgressIndicator { get; set; }

        [Value(0, Required = true, HelpText = "Path to search.")]
        public IEnumerable<string> SearchPath { get; set; }

        /*
        [HelpOption(HelpText = "Display this help text.")]
        public String ShowUsage()
        {
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.AppendLine("Usage:");
            helpMessage.AppendLine("\n   CloneFinder [-c] [-p] path");
            helpMessage.AppendLine("\nOptions:");
            helpMessage.AppendLine("   -c, --csv        Display results as comma separated values (CSV).");
            helpMessage.AppendLine("   -p, --progress   Show progress indicator.");
            helpMessage.AppendLine("\nExample:");
            helpMessage.AppendLine("\n   CloneFinder -c c:\\temp");
            helpMessage.AppendLine("\nSearches for duplicates in c:\\temp, and outputs results in CSV format.");
            return helpMessage.ToString();
        }
        */

        [Usage(ApplicationAlias = "clonefinder")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>()
                {
                    new Example(@"Search for duplicates in c:\temp", new Options { SearchPath = new string[] { @"c:\temp"}}),
                    new Example(@"Search for duplicates in c:\temp, and output results in CSV format.", new Options { SearchPath = new string[] { @"c:\temp"}, CsvOutput = true }),
                    new Example(@"Search for duplicates in c:\temp, output results in CSV format, and show progress indicator.", new Options { SearchPath = new string[] { @"c:\temp"}, CsvOutput = true, ProgressIndicator = true })
                };
            }
        }

        public bool Validates()
        {
            bool validatedOK = true;
            StringBuilder errorMessage = new StringBuilder();

            // Make sure we've been passed valid path(s),
            // and that we have read permissions
            if (SearchPath.Count() > 0)
            {
                // only going to check the first path
                // A possible future enhancement
                // will allow multiple paths to be specified
                string[] searchPaths = SearchPath.ToArray();
                if (!Directory.Exists(searchPaths[0]))
                {
                    validatedOK = false;
                    errorMessage.AppendFormat(messagePathNotFound, searchPaths[0]);
                    errorMessage.AppendLine();
                }
            }
            else
            {
                // No path to search was specified
                validatedOK = false;
                errorMessage.AppendLine();
                errorMessage.Append(messageNoPathSpecified);
            }
            if (!validatedOK)
            {
                // Tell the user there was a problem
                // and we're not going to find
                // their precious duplicates
                Console.Error.WriteLine(errorMessage.ToString());
                //Environment.ExitCode = 1;
            }
            return validatedOK;
        }

    }
}
