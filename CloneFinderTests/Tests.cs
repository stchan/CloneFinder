using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using NUnit.Framework;

namespace CloneFinderCoreTests
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
                CloneFinderCore.DirectoryWalker nullParameter = new CloneFinderCore.DirectoryWalker(null);
            }
            catch (ArgumentNullException)
            {
                nullParameterTest = true;
            }
            Assert.IsTrue(nullParameterTest, "ArgumentNullException caught when constructor is passed null.");

            bool emptyStringParameterTest = false;
            try
            {
                CloneFinderCore.DirectoryWalker emptyStringParameter = new CloneFinderCore.DirectoryWalker(String.Empty);
            }
            catch (ArgumentNullException)
            {
                emptyStringParameterTest = true;
            }
            Assert.IsTrue(emptyStringParameterTest);

            bool whiteSpaceParameterTest = false;
            try
            {
                CloneFinderCore.DirectoryWalker whiteSpaceParameter = new CloneFinderCore.DirectoryWalker("    ");
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
            CloneFinderCore.FileProcessorLengthFirst testLengthFirst = new CloneFinderCore.FileProcessorLengthFirst();
            testLengthFirst.Initialize();
            Assert.IsTrue(File.Exists(testLengthFirst.Test_DatabaseFile));
            testLengthFirst.Dispose();
        }

        [Test]
        public void InitialDirectoryWalk()
        {
            CloneFinderCore.DirectoryWalker testDirectoryWalk = new CloneFinderCore.DirectoryWalker(@"C:\Users\sherman");
            testDirectoryWalk.DirectoryWalkStarted += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkStarted);
            testDirectoryWalk.DirectoryWalkComplete += new EventHandler<CloneFinderCore.DirectoryWalkEventArgs>(DirectoryWalk_DirectoryWalkComplete);
            testDirectoryWalk.WalkDirectory();
        }

        int directoryWalkCompletedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkComplete(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            this.directoryWalkCompletedEventCount++;
        }

        int directoryWalkStartedEventCount = 0;
        private void DirectoryWalk_DirectoryWalkStarted(object sender, CloneFinderCore.DirectoryWalkEventArgs e)
        {
            this.directoryWalkStartedEventCount++;
        }


        #endregion
#endif
    }
}
