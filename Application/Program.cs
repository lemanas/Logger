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
                if (i == 10) logger.Stop();
                logger.Write("Number with Flush: " + i);
            }

            ILog logger2 = new AsyncLog(LoggerType.WithoutFlush);

            for (int i = 50; i > 0; i--)
            {
                if (i == 40) logger2.Stop();
                logger2.Write("Number with No flush: " + i);
            }

            Console.WriteLine("Main program done");
            Console.ReadLine();
        }
    }
}
