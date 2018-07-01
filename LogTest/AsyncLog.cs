using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogTest.Interfaces;

namespace LogTest
{
    public class AsyncLog : ILog
    {
        private readonly string _path;
        private readonly CancellationTokenSource _ctx;
        private readonly CancellationToken _token;
        private readonly BlockingCollection<string> _logCollection;

        public AsyncLog(string name)
        {
            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            _path = @"C:\LogTest\" + name;
            _ctx = new CancellationTokenSource();
            _token = _ctx.Token;
            _logCollection = new BlockingCollection<string>();
            Task.Factory.StartNew(StartBackgroundWrite, TaskCreationOptions.LongRunning, _token);
        }


        public void StopWithFlush()
        {
            _logCollection.CompleteAdding();
        }

        public void StopWithoutFlush()
        {
            _ctx.Cancel();
        }

        public void Write(string text)
        {
            var t = new Task(() =>
            {
                byte[] encodedText = Encoding.Unicode.GetBytes(text);
                byte[] newLine = Encoding.Unicode.GetBytes(Environment.NewLine);

                using (var sourceStream = new FileStream(_path + DateTime.Today.ToString("yyyyMMdd") + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
                {
                    sourceStream.WriteAsync(encodedText, 0, encodedText.Length, _token);
                    sourceStream.WriteAsync(newLine, 0, newLine.Length, _token);
                }
            });
            t.Start();
            try
            {
                t.Wait(_token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Cancelled...");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        public void AddLogToQueue(string text)
        {
            if (!_ctx.IsCancellationRequested)
            {
                try
                {
                    _logCollection.Add(text, _token);
                }
                catch (Exception)
                {
                    Console.WriteLine("Cancelled...");
                }
            }
        }

        private void StartBackgroundWrite(object o)
        {
            string file = _path + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            string errorsFile = _path + "errorLog.txt";

            byte[] newLine = Encoding.Unicode.GetBytes(Environment.NewLine);
            try
            {
                using (var sourceStream = new FileStream(file,
                    FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
                {
                    foreach (var log in _logCollection.GetConsumingEnumerable())
                    {
                        byte[] encodedText = Encoding.Unicode.GetBytes(log);

                        sourceStream.WriteAsync(encodedText, 0, encodedText.Length, _token);
                        sourceStream.WriteAsync(newLine, 0, newLine.Length, _token);
                    }
                }
            }
            catch (Exception exception)
            {
                File.AppendAllText(errorsFile, DateTime.Now + " || " + exception.Message);
                File.AppendAllText(errorsFile, newLine.ToString());
            }
        }
    }
}