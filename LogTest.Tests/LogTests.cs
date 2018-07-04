using System;
using System.IO;
using System.Threading;
using LogTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogTest.Tests
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void TestOnException()
        {
            var date = DateTime.Today.ToString("yyyyMMdd");
            ILog testLogger = new AsyncLog(nameof(testLogger));
            string file = @"C:\LogTest\" + nameof(testLogger) + date + ".txt";

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

            ILog testLogger2 = new AsyncLog(nameof(testLogger2));
            string file2 = @"C:\LogTest\" + nameof(testLogger2) + date + ".txt";

            for (int i = 0; i < 10; i++)
            {
                testLogger2.AddLogToQueue("Test log after exception no: " + i);
            }

            testLogger2.StopWithFlush();

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
            ILog logDateTester = new AsyncLog(nameof(logDateTester));
            string path = @"C:\LogTest\" + nameof(logDateTester) + date + ".txt";

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
            var date = DateTime.Today.ToString("yyyyMMdd");

            // First logger
            ILog firstFlushLog = new AsyncLog(nameof(firstFlushLog));
            string pathFirst = @"C:\LogTest\" + nameof(firstFlushLog) + date + ".txt";

            for (int i = 0; i < 15; i++)
            {
                firstFlushLog.AddLogToQueue("Number with Flush: " + i);
                if (i == 10) firstFlushLog.StopWithFlush();
            }

            Thread.Sleep(1);
            var firstLineCount = File.ReadAllLines(pathFirst).Length;

            // Second logger
            ILog secondFlushLog = new AsyncLog(nameof(secondFlushLog));
            string pathSecond = @"C:\LogTest\" + nameof(secondFlushLog) + date + ".txt";

            for (int i = 0; i < 15; i++)
            {
                if (i == 10) secondFlushLog.StopWithFlush();
                secondFlushLog.AddLogToQueue("Number with Flush: " + i);
            }

            Thread.Sleep(1);
            var secondLineCount = File.ReadAllLines(pathSecond).Length;

            File.Delete(pathFirst);
            File.Delete(pathSecond);
            Assert.IsTrue(firstLineCount > secondLineCount);
        }

        [TestMethod]
        public void TestWithoutFlush()
        {
            var date = DateTime.Today.ToString("yyyyMMdd");
            ILog noFlushLog = new AsyncLog(nameof(noFlushLog));
            string path = @"C:\LogTest\" + nameof(noFlushLog) + date + ".txt";

            for (int i = 0; i < 15; i++)
            {
                noFlushLog.AddLogToQueue("Number with Flush: " + i);
            }

            noFlushLog.StopWithoutFlush();
            noFlushLog.StopWithFlush();

            Thread.Sleep(1000);

            Assert.IsTrue(File.Exists(path));
            Assert.IsTrue(File.OpenRead(path).Length == 0);

            File.Delete(path);
        }

    }
}
