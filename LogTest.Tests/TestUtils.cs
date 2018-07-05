using System;

namespace LogTest.Tests
{
    public class TestUtils
    {
        public static string BuildFilePath(string location, string name)
        {
            string date = DateTime.Today.ToString("yyyyMMdd");
            return location + name + date + ".txt";
        }
    }
}
