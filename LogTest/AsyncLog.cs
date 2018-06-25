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

        public AsyncLog()
        {
            if (!Directory.Exists(@"C:\LogTest")) 
                Directory.CreateDirectory(@"C:\LogTest");

            _path = @"C:\LogTest\Log" + DateTime.Today.ToString("yyyyMMdd") + ".log";
            _ctx = new CancellationTokenSource();
            _token = _ctx.Token;
        }

        public void StopWithoutFlush()
        {
            _ctx.Cancel();
        }

        public void StopWithFlush()
        {
            throw new NotImplementedException();
        }

        public async Task Write(string text)
        {
            await Task.Factory.StartNew(() =>
            {
                byte[] encodedText = Encoding.Unicode.GetBytes(text);
                byte[] newLine = Encoding.Unicode.GetBytes(Environment.NewLine);

                using (var sourceStream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.Write, 4096, true))
                {
                    sourceStream.WriteAsync(encodedText, 0, encodedText.Length, _token);
                    sourceStream.WriteAsync(newLine, 0, newLine.Length, _token);
                }
            }, _token);
        }
    }
}