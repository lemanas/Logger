using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LogComponent.Interfaces;

namespace LogComponent
{
    public class AsyncLog : ILog
    {
        private readonly string _path;
        private readonly CancellationTokenSource _ctx;
        private readonly CancellationToken _token;
        private readonly BlockingCollection<string> _logCollection;

        public AsyncLog(string name, string location)
        {
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);

            _path = location + name;
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
            _logCollection.CompleteAdding();
            _ctx.Cancel();
        }

        public async Task AddLogToQueue(string text)
        {
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _logCollection.Add(text, _token);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Cancelled...");
                    }
                }, _token);
            }
            catch (Exception)
            {
                Console.WriteLine("Cancelled...");
            }
        }

        private void StartBackgroundWrite(object o)
        {
            string file = _path + DateTime.Today.ToString("yyyyMMdd") + ".txt";
            string errorsFile = _path + "errorLog.txt";
            IWriter writer = new AsyncWriter();

            try
            {
                using (var sourceStream = new FileStream(file,
                    FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
                {
                    foreach (var log in _logCollection.GetConsumingEnumerable())
                    {
                        if (!_ctx.IsCancellationRequested)
                            writer.Write(sourceStream, log, _token);
                    }
                }
            }
            catch (Exception exception)
            {
                using (var sourceStream = new FileStream(errorsFile,
                    FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
                {
                    writer.Write(sourceStream, exception.Message, _token);
                }
            }
        }
    }
}