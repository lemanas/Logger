using System;
using System.IO;
using System.Threading;
using LogComponent;
using LogComponent.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogTest.Tests
{
    [TestClass]
    public class LogTests
    {
        private static readonly string Location = @"C:\LogTest\";   

        [TestMethod]
        public void TestOnException()
        {
            ILog testLogger = new AsyncLog(nameof(testLogger), Location);
            string file = TestUtils.BuildFilePath(Location, nameof(testLogger));

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    testLogger.AddLogToQueue("Test log no: " + i);
                    if (i == 5)
                        throw new Exception();
                }
            }
            catch
            {
                // ignored
            }

            testLogger.StopWithFlush();

            ILog testLogger2 = new AsyncLog(nameof(testLogger2), Location);
            string file2 = TestUtils.BuildFilePath(Location, nameof(testLogger2));

            for (int i = 0; i < 10; i++)
            {
                testLogger2.AddLogToQueue("Test log after exception no: " + i);
            }

            testLogger2.StopWithFlush();

            Thread.Sleep(100);

            Assert.IsTrue(File.Exists(file));
            Assert.IsTrue(File.Exists(file2));

            Thread.Sleep(1000);

            File.Delete(file);
            File.Delete(file2);
        }

        [TestMethod]
        public void TestFileDate()
        {
            var date = DateTime.Today.ToString("yyyyMMdd");

            ILog logDateTester = new AsyncLog(nameof(logDateTester), Location);
            string path = TestUtils.BuildFilePath(Location, nameof(logDateTester));

            logDateTester.AddLogToQueue("Some random test log"); // file created on first log
            logDateTester.StopWithFlush();

            Thread.Sleep(1);

            var file = File.OpenRead(path);
            file.Dispose();
            var fileDate = file.Name.Substring(24, 8);

            File.Delete(path);
            Assert.IsTrue(fileDate == date);
        }

        [TestMethod]
        public void TestStopWithFlush()
        {
            // First logger
            ILog firstFlushLog = new AsyncLog(nameof(firstFlushLog), Location);
            string pathFirst = TestUtils.BuildFilePath(Location, nameof(firstFlushLog));

            for (int i = 0; i < 15; i++)
            {
                firstFlushLog.AddLogToQueue("Number with Flush: " + i);
                if (i == 10) firstFlushLog.StopWithFlush();
            }

            Thread.Sleep(100);

            var firstFile = File.OpenRead(pathFirst);
            var firstFileLength = firstFile.Length;
            firstFile.Dispose();

            // Second logger
            ILog secondFlushLog = new AsyncLog(nameof(secondFlushLog), Location);
            string pathSecond = TestUtils.BuildFilePath(Location, nameof(secondFlushLog));

            for (int i = 0; i < 15; i++)
            {
                if (i == 10) secondFlushLog.StopWithFlush();
                secondFlushLog.AddLogToQueue("Number with Flush: " + i);
            }

            Thread.Sleep(100);

            var secondFile = File.OpenRead(pathSecond);
            var secondFileLength = secondFile.Length;
            secondFile.Dispose();

            File.Delete(pathFirst);
            File.Delete(pathSecond);
            Assert.IsTrue(firstFileLength > secondFileLength);
        }

        [TestMethod]
        public void TestWithoutFlush()
        {
            ILog noFlushLog = new AsyncLog(nameof(noFlushLog), Location);
            string path = TestUtils.BuildFilePath(Location, nameof(noFlushLog));

            for (int i = 0; i < 15; i++)
            {
                noFlushLog.AddLogToQueue("Number with Flush: " + i);
            }

            noFlushLog.StopWithoutFlush();

            Thread.Sleep(100);

            var file = File.OpenRead(path);

            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(file.Length == 0);

            file.Dispose();
            File.Delete(path);
        }

    }
}
