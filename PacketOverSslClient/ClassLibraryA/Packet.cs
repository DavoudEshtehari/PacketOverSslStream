using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibraryA
{
    public class Packet
    {
        private byte[] _data;
        private int _dataLength;

        public Packet(byte[] data)
        {
            _data = data;
            _dataLength = data.Length;
        }

        public async void WriteToStreamAsync(Stream stream) =>
            //uint status = 0;
            await stream.WriteAsync(_data, 0, _dataLength, CancellationToken.None).ContinueWith(t =>
            {
                Exception e = t.Exception?.InnerException;
                if (e != null)
                {
                    //status = 1;
                    //Release();
                    Console.WriteLine($"Release: {e}");
                }
                //callback(this, status);
                Console.WriteLine("callback.");
            },
            CancellationToken.None,
            TaskContinuationOptions.DenyChildAttach,
            TaskScheduler.Default);
    }
}
