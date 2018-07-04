using System;
using LogTest;
using LogTest.Interfaces;

namespace LogUsers
{
    class Program
    {
        static void Main()
        {
            ILog  logger = new AsyncLog(nameof(logger));

            for (int i = 0; i <= 14; i++)
            {
                logger.AddLogToQueue("Number with Flush: " + i);
            }

            ILog logger2 = new AsyncLog(nameof(logger2));

            for (int i = 50; i > 0; i--)
            {
                logger2.AddLogToQueue("Number with No flush: " + i);
            }

            Console.WriteLine("Main program done");
            Console.ReadLine();
        }
    }
}
