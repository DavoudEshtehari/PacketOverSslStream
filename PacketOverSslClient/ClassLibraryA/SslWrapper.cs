using System;
using System.IO;
using System.Net.Security;
using System.Threading;
using System.Threading.Tasks;

namespace ClassLibraryA
{
    public class SslStreamWrapper : SslStream
    {
        public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
        {
            // unlimited loop happens here
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }
        public SslStreamWrapper(Stream innerStream) : base(innerStream)
        {
        }

        public SslStreamWrapper(Stream innerStream, bool leaveInnerStreamOpen) : base(innerStream, leaveInnerStreamOpen)
        {
        }

        public SslStreamWrapper(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
        {
        }

        public SslStreamWrapper(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback)
        {
        }

        public SslStreamWrapper(Stream innerStream, bool leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy) : base(innerStream, leaveInnerStreamOpen, userCertificateValidationCallback, userCertificateSelectionCallback, encryptionPolicy)
        {
        }
    }

    public class SslOverTdsStream : Stream
    {
        private const int HEADER_LEN = 8;
        private const int HEADER_LEN_FIELD_OFFSET = 2;
        private const int PACKET_SIZE_WITHOUT_HEADER = 4088;

        private readonly Stream _stream;


        public SslOverTdsStream()
        {
        }

        public SslOverTdsStream(Stream stream)
        {
            _stream = stream;
        }

        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => false;

        public override bool CanTimeout => base.CanTimeout;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override int ReadTimeout { get => base.ReadTimeout; set => base.ReadTimeout = value; }
        public override int WriteTimeout { get => base.WriteTimeout; set => base.WriteTimeout = value; }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return base.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            base.Close();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            base.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return base.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        // public override ValueTask DisposeAsync()
        // {
        //     return base.DisposeAsync();
        // }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return base.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            base.EndWrite(asyncResult);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return base.FlushAsync(cancellationToken);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return base.InitializeLifetimeService();
        }

        public override int Read(byte[] buffer, int offset, int count)
            => ReadInternal(buffer, offset, count, CancellationToken.None, async: false).GetAwaiter().GetResult();

        public override int Read(Span<byte> buffer)
        {
            return base.Read(buffer);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
            => ReadInternal(buffer, offset, count, token, async: true);

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return base.ReadAsync(buffer, cancellationToken);
        }

        public override int ReadByte()
        {
            return base.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override void Write(byte[] buffer, int offset, int count)
            => WriteInternal(buffer, offset, count, CancellationToken.None, async: false).Wait();

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            base.Write(buffer);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
            => WriteInternal(buffer, offset, count, token, async: true);

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return base.WriteAsync(buffer, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            base.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private async Task<int> ReadInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
        {
            if (async)
            {
                return await _stream.ReadAsync(buffer, offset, count, token).ConfigureAwait(false);
            }
            else
            {
                return _stream.Read(buffer, offset, count);
            }
        }

        private async Task WriteInternal(byte[] buffer, int offset, int count, CancellationToken token, bool async)
        {
            if (async)
            {
                await _stream.WriteAsync(buffer, offset, count, token).ConfigureAwait(false);
                await _stream.FlushAsync().ConfigureAwait(false);
            }
            else
            {
                _stream.Write(buffer, offset, count);
                _stream.Flush();
            }
        }
    }
}
