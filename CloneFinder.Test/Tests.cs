using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


using Xunit;

using CloneFinder.Core;
#nullable disable

namespace CloneFinder.CoreTests
{
    public class Tests
    {
        #region Ctor
        
        public Tests()
        { }
        
        #endregion

#if DEBUG
        #region DirectoryWalker tests

        [Fact]
        public void DirectoryWalker_DoesConstructorRejectNullArgument()
        {
            // Constructor should throw exception
            // when a null, empty, whitespace is the parameter
            bool nullParameterTest = false;
            try
            {
                DirectoryWalker nullParameter = new DirectoryWalker(null);
            }
            catch (ArgumentNullException)
            {
                nullParameterTest = true;
            }
            Assert.True(nullParameterTest, "ArgumentNullException caught when constructor is passed null.");

            bool emptyStringParameterTest = false;
            try
            {
                DirectoryWalker emptyStringParameter = new DirectoryWalker(String.Empty);
            }
            catch (ArgumentNullException)
            {
                emptyStringParameterTest = true;
            }
            Assert.True(emptyStringParameterTest);

            bool whiteSpaceParameterTest = false;
            try
            {
                DirectoryWalker whiteSpaceParameter = new DirectoryWalker("    ");
            }
            catch (ArgumentNullException)
            {
                whiteSpaceParameterTest = true;
            }
            Assert.True(whiteSpaceParameterTest);

        }

        [Fact]
        public void Test1()
        {
            FileProcessorLengthFirst testLengthFirst = new FileProcessorLengthFirst();
            testLengthFirst.Initialize();
            Assert.True(File.Exists(testLengthFirst.Test_DatabaseFile));
            testLengthFirst.Dispose();
        }

        [Fact]
        public void InitialDirectoryWalk()
        {
            String targetDir = Environment.GetEnvironmentVariable("UserProfile");
            targetDir = Path.Combine(targetDir, "Downloads");
            int fileCount = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories).Length;
            //int directoryCount = Directory.GetDirectories(userProfileDir, "*", SearchOption.AllDirectories).Length;
            DirectoryWalker testDirectoryWalk = new DirectoryWalker(targetDir);
            testDirectoryWalk.DirectoryWalkStarted += new EventHandler<DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkStarted);
            testDirectoryWalk.DirectoryWalkComplete += new EventHandler<DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkComplete);
            testDirectoryWalk.FileAccessed += new EventHandler<DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkFileAccessed);
            testDirectoryWalk.FileProcessed += new EventHandler<DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkFileProcessed);
            testDirectoryWalk.WalkDirectory();
            Assert.True(this.directoryWalkFileProcessedEventCount == (fileCount));
        }

        int directoryWalkFileProcessedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkFileProcessed(object sender, DirectoryWalkEventArgs e)
        {
            directoryWalkFileProcessedEventCount++;
        }

        int directoryWalkFileAccessedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkFileAccessed(object sender, DirectoryWalkEventArgs e)
        {
            this.directoryWalkFileAccessedEventCount++;
        }

        int directoryWalkCompletedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkComplete(object sender, DirectoryWalkEventArgs e)
        {
            this.directoryWalkCompletedEventCount++;
        }

        int directoryWalkStartedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkStarted(object sender, DirectoryWalkEventArgs e)
        {
            this.directoryWalkStartedEventCount++;
        }


        #endregion
#endif
    }
}
