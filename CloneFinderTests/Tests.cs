using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace CloneFinderTests
{
    [TestFixture()]
    public class Tests
    {
        #region Ctor
        
        public Tests()
        { }
        
        #endregion

#if DEBUG
        #region DirectoryWalker tests

        [Test]
        public void DirectoryWalker_DoesConstructorRejectNullArgument()
        {
            // Constructor should throw exception
            // when a null, empty, whitespace is the parameter
            bool nullParameterTest = false;
            try
            {
                CloneFinder.DirectoryWalker nullParameter = new CloneFinder.DirectoryWalker(null);
            }
            catch (ArgumentNullException)
            {
                nullParameterTest = true;
            }
            Assert.IsTrue(nullParameterTest, "ArgumentNullException caught when constructor is passed null.");

            bool emptyStringParameterTest = false;
            try
            {
                CloneFinder.DirectoryWalker emptyStringParameter = new CloneFinder.DirectoryWalker(String.Empty);
            }
            catch (ArgumentNullException)
            {
                emptyStringParameterTest = true;
            }
            Assert.IsTrue(emptyStringParameterTest);

            bool whiteSpaceParameterTest = false;
            try
            {
                CloneFinder.DirectoryWalker whiteSpaceParameter = new CloneFinder.DirectoryWalker("    ");
            }
            catch (ArgumentNullException)
            {
                whiteSpaceParameterTest = true;
            }
            Assert.IsTrue(whiteSpaceParameterTest);

        }

        [Test]
        public void Test1()
        {
            CloneFinder.FileProcessorLengthFirst testLengthFirst = new CloneFinder.FileProcessorLengthFirst();
            testLengthFirst.Initialize();
            Assert.IsTrue(File.Exists(testLengthFirst.Test_DatabaseFile));
            testLengthFirst.Dispose();
        }

        [Test]
        public void InitialDirectoryWalk()
        {
            CloneFinder.DirectoryWalker testDirectoryWalk = new CloneFinder.DirectoryWalker(@"C:\Users\sherman");
            testDirectoryWalk.DirectoryWalkStarted += new EventHandler<CloneFinder.DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkStarted);
            testDirectoryWalk.DirectoryWalkComplete += new EventHandler<CloneFinder.DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkComplete);
            testDirectoryWalk.WalkDirectory();
        }

        int directoryWalkCompletedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkComplete(object sender, CloneFinder.DirectoryWalkEventArgs e)
        {
            this.directoryWalkCompletedEventCount++;
        }

        int directoryWalkStartedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkStarted(object sender, CloneFinder.DirectoryWalkEventArgs e)
        {
            this.directoryWalkStartedEventCount++;
        }


        #endregion
#endif
    }
}
