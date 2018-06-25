using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LogTest
{
    public class AsyncLog : ILog
    {
        private readonly string _path; 

        public AsyncLog()
        {
            if (!Directory.Exists(@"C:\LogTest")) 
                Directory.CreateDirectory(@"C:\LogTest");

            _path = @"C:\LogTest\Log" + DateTime.Today.ToString("yyyyMMdd") + ".log";
        }

        public void StopWithoutFlush()
        {
            throw new NotImplementedException();
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

                using (var sourceStream = new FileStream(_path, FileMode.Append, FileAccess.Write, FileShare.None, 4096, true))
                {
                    sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                    sourceStream.WriteAsync(newLine, 0, newLine.Length);
                }
            });
        }
    }
}