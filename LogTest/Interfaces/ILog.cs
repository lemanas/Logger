namespace LogTest
{
    public interface ILog
    {
        /// <summary>
        /// Stops logging with or without flush based on ILog type
        /// </summary>
        void Stop();

        /// <summary>
        /// Write a message to the Log.
        /// </summary>
        /// <param name="text">The text to written to the log</param>
        void Write(string text);


    }
}
