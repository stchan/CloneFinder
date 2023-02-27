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
                ParserResult<Options> parseResult = Parser.Default.ParseArguments<Options>(args); 
                if (parseResult.Tag == ParserResultType.Parsed)
                {
                    IOptions parsedOptions = ((Parsed<Options>)(parseResult)).Value;
                    if (parsedOptions.Validates())
                    {
                        RequestProcessor duplicateProcessor = new RequestProcessor();
                        duplicateProcessor.FindDuplicates(parsedOptions);
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
                // Something totally unexpected happened -
                // catch the exception so the whole process
                // doesn't come crashing down if this is
                // being run in a batch file
                Console.Error.WriteLine(messageUnexpectedInternalError);
                Environment.ExitCode = -2;
            }
        }



    }
}
