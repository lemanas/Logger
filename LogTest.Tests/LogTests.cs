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
                    testLogger.Write("Test log no: " + i);
                    if (i == 5)
                        throw new Exception();
                }
            }
            catch
            {
                // ignored
            }

            var firstLineCount = File.ReadAllLines(file).Length;

            for (int i = 0; i < 10; i++)
            {
                testLogger.Write("Test log after exception no: " + i);
            }

            var finalLineCount = File.ReadAllLines(file).Length;

            File.Delete(file);
            Assert.IsTrue(finalLineCount > firstLineCount); 
        }

        [TestMethod]
        public void TestFileDate()
        {
            var date = DateTime.Today.ToString("yyyyMMdd");
            ILog logDateTester = new AsyncLog(nameof(logDateTester));
            string path = @"C:\LogTest\" + nameof(logDateTester) + date + ".txt";

            logDateTester.Write("The one and only log in this file"); // file created on first log
            var fileName = File.OpenRead(path).Name;
            var fileDate = fileName.Substring(24, 8);


            File.Delete(path);
            Assert.IsNotNull(fileName);
            Assert.IsTrue(fileDate == date);
        }

        [TestMethod]
        public void TestStopWithFlush()
        {
            var date = DateTime.Today.ToString("yyyyMMdd");

            // First logger
            ILog firstFlushLog = new AsyncLog(nameof(firstFlushLog));
            string pathFirst = @"C:\LogTest\" + nameof(firstFlushLog) + date + ".txt";
            File.Delete(pathFirst);

            for (int i = 0; i < 15; i++)
            {
                firstFlushLog.AddLogToQueue("Number with Flush: " + i);
                if (i == 10) firstFlushLog.StopWithFlush();
            }

            Thread.Sleep(2000);
            var firstLineCount = File.ReadAllLines(pathFirst).Length;

            // Second logger
            ILog secondFlushLog = new AsyncLog(nameof(secondFlushLog));
            string pathSecond = @"C:\LogTest\" + nameof(secondFlushLog) + date + ".txt";

            for (int i = 0; i < 15; i++)
            {
                if (i == 10) secondFlushLog.StopWithFlush();
                secondFlushLog.AddLogToQueue("Number with Flush: " + i);
            }

            Thread.Sleep(2000);
            var secondLineCount = File.ReadAllLines(pathSecond).Length;


            Thread.Sleep(2000);
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
            File.Delete(path);
        }

    }
}
