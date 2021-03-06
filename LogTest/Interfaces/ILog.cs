﻿using System.Threading.Tasks;

namespace LogComponent.Interfaces
{
    public interface ILog
    {
        /// <summary>
        /// Stops logging, but first writes all outstanding logs
        /// </summary>
        void StopWithFlush();

        /// <summary>
        /// Stops logging immediately
        /// </summary>
        void StopWithoutFlush();

        /// <summary>
        /// Write a message to the Log.
        /// </summary>
        /// <param name="text">The text to written to the log</param>
        Task AddLogToQueue(string text);
    }
}
