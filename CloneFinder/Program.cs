using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

using CommandLine;

namespace CloneFinder
{
    public class Program
    {
        #region Constants
        
        const String messagePathNotFound = "{0} not found, or not accessible.";
        const String messageNoPathSpecified = "No path specified.";
        const String messageUnexpectedInternalError = "Unexpected internal error - exiting.";

        #endregion

        public static void Main(string[] args)
        {
            try
            {
                Options commandLineOptions = new Options();
                ICommandLineParser commandParser = new CommandLineParser();
                if (commandParser.ParseArguments(args, commandLineOptions, Console.Error))
                {
                    if (ValidateOptions(commandLineOptions))
                    {
                        RequestProcessor duplicateProcessor = new RequestProcessor();
                        duplicateProcessor.FindDuplicates(commandLineOptions);
                    }
                }
                else
                {
                    // Command line params could not be parsed,
                    // or help was requested
                    Environment.ExitCode = -1;
                }
            }
            catch
            {
                Console.Error.WriteLine(messageUnexpectedInternalError);
                Environment.ExitCode = -2;
            }
        }

        private static bool ValidateOptions(Options commandLineOptions)
        {
            bool validatedOK = true;
            StringBuilder errorMessage = new StringBuilder();

            // Make sure we've been passed valid path(s),
            // and that we have read permissions
            if (commandLineOptions.Items.Count > 0)
            {
                // only going to run the loop once -
                // leaving this here for future enhancement which
                // will allow multiple paths to be specified
                for (int loop = 0; loop < 1; loop++)
                {
                    if (!Directory.Exists(commandLineOptions.Items[loop]))
                    {
                        validatedOK = false;
                        errorMessage.AppendFormat(messagePathNotFound, commandLineOptions.Items[loop]);
                        errorMessage.AppendLine();
                    }
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
                Console.Error.WriteLine(errorMessage.ToString());
                Environment.ExitCode = 1;
            }
            return validatedOK;
        }


    }
}
