using System;
using LogComponent;
using LogComponent.Interfaces;

namespace LogUsers
{
    class Program
    {
        static void Main()
        {
            string location = @"C:\LogTest\";

            ILog logger = new AsyncLog(nameof(logger), location);

            for (int i = 0; i <= 14; i++)
            {
                logger.AddLogToQueue("Number with Flush: " + i);
                //if (i == 10) logger.StopWithFlush();
            }

            ILog logger2 = new AsyncLog(nameof(logger2), location);

            for (int i = 50; i > 0; i--)
            {
                logger2.AddLogToQueue("Number with No flush: " + i);
                //if (i == 30) logger2.StopWithoutFlush();
            }

            Console.WriteLine("Main program done");
            Console.ReadLine();
        }
    }
}
