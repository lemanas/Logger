using System.IO;
using System.Threading;

namespace LogComponent.Interfaces
{
    interface IWriter
    {
        void Write(FileStream sourceStream, string text, CancellationToken token);
    }
}
