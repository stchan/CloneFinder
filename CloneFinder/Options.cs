using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;

namespace CloneFinder
{
    public class Options
    {
        #region Ctor

        public Options()
        { }

        #endregion

        [Option("c", "csv", Required = false, HelpText = "Output results in CSV format.")]
        public bool csvOutput = false;

        [Option("p", "progress", Required = false, HelpText = "Show progress indicator.")]
        public bool progressIndicator = false;


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

        [ValueList(typeof(List<string>))]
        public IList<string> Items = null;
    }
}
