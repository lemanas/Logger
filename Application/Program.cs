using System;
using System.Threading;
using LogTest;

namespace LogUsers
{
    class Program
    {
        static void Main()
        {
            ILog  logger = new AsyncLog();

            for (int i = 0; i < 15; i++)
            {
                logger.Write("Number with Flush: " + i + "\n");
                Thread.Sleep(50);
            }

            //logger.StopWithFlush();

            ILog logger2 = new AsyncLog();

            for (int i = 50; i > 0; i--)
            {
                logger2.Write("Number with No flush: " + i + "\n");
                Thread.Sleep(20);
            }

            //logger2.StopWithoutFlush();

            Console.ReadLine();
        }
    }
}
