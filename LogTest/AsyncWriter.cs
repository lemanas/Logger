using System;
using System.IO;
using System.Text;
using System.Threading;
using LogComponent.Interfaces;

namespace LogComponent
{
    class AsyncWriter : IWriter
    {
        private static readonly byte[] NewLine = Encoding.Unicode.GetBytes(Environment.NewLine);

        public void Write(FileStream sourceStream, string text, CancellationToken token)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            sourceStream.WriteAsync(encodedText, 0, encodedText.Length, token);
            sourceStream.WriteAsync(NewLine, 0, NewLine.Length, token);
        }
    }
}
