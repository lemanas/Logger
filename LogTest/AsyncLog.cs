using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogTest
{
    public class AsyncLog : ILog
    {
        private readonly string _path;
        private readonly CancellationTokenSource _ctx;
        private readonly CancellationToken _token;
        private readonly LoggerType _type;

        public AsyncLog(LoggerType type)
        {
            if (!Directory.Exists(@"C:\LogTest"))
                Directory.CreateDirectory(@"C:\LogTest");

            _type = type;
            _path = @"C:\LogTest\Log" + _type;
            _ctx = new CancellationTokenSource();
            _token = _ctx.Token;
        }

        public void Stop()
        {
            if (_type == LoggerType.WithFlush)
            {
                _ctx.Cancel();
            }
            if (_type == LoggerType.WithoutFlush)
            {
                _ctx.Cancel();
            }
        }

        public void Write(string text)
        {
            var t = new Task(() =>
            {
                Thread.Sleep(1000);
                byte[] encodedText = Encoding.Unicode.GetBytes(text);
                byte[] newLine = Encoding.Unicode.GetBytes(Environment.NewLine);

                using (var sourceStream = new FileStream(_path + DateTime.Today.ToString("yyyyMMdd") + ".log", FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
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
    }
}