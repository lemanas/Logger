using System;
using LogTest;

namespace LogUsers
{
    class Program
    {
        static void Main()
        {
            ILog  logger = new AsyncLog(LoggerType.WithFlush);

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i);
                if (i == 10) logger.Stop();
            }

            ILog logger2 = new AsyncLog(LoggerType.WithoutFlush);

            for (int i = 50; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i);
                if (i == 40) logger2.Stop();
            }

            Console.WriteLine("Main program done");
            Console.ReadLine();
        }
    }
}
