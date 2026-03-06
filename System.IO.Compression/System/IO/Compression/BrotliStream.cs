using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO.Compression
{
	public sealed class BrotliStream : Stream
	{
		public BrotliStream(Stream stream, CompressionMode mode) : this(stream, mode, false)
		{
		}

		public BrotliStream(Stream stream, CompressionMode mode, bool leaveOpen)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (mode != CompressionMode.Decompress)
			{
				if (mode != CompressionMode.Compress)
				{
					throw new ArgumentException("Enum value was out of legal range.", "mode");
				}
				if (!stream.CanWrite)
				{
					throw new ArgumentException("Stream does not support writing.", "stream");
				}
			}
			else if (!stream.CanRead)
			{
				throw new ArgumentException("Stream does not support reading.", "stream");
			}
			this._mode = mode;
			this._stream = stream;
			this._leaveOpen = leaveOpen;
			this._buffer = new byte[65520];
		}

		private void EnsureNotDisposed()
		{
			if (this._stream == null)
			{
				throw new ObjectDisposedException("stream", "Can not access a closed Stream.");
			}
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this._stream != null)
				{
					if (this._mode == CompressionMode.Compress)
					{
						this.WriteCore(ReadOnlySpan<byte>.Empty, true);
					}
					if (!this._leaveOpen)
					{
						this._stream.Dispose();
					}
				}
			}
			finally
			{
				this._stream = null;
				this._encoder.Dispose();
				this._decoder.Dispose();
				base.Dispose(disposing);
			}
		}

		private static void ValidateParameters(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", "Positive number required.");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "Positive number required.");
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException("Offset plus count is larger than the length of target array.");
			}
		}

		public Stream BaseStream
		{
			get
			{
				return this._stream;
			}
		}

		public override bool CanRead
		{
			get
			{
				return this._mode == CompressionMode.Decompress && this._stream != null && this._stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._mode == CompressionMode.Compress && this._stream != null && this._stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		private bool AsyncOperationIsActive
		{
			get
			{
				return this._activeAsyncOperation != 0;
			}
		}

		private void EnsureNoActiveAsyncOperation()
		{
			if (this.AsyncOperationIsActive)
			{
				BrotliStream.ThrowInvalidBeginCall();
			}
		}

		private void AsyncOperationStarting()
		{
			if (Interlocked.CompareExchange(ref this._activeAsyncOperation, 1, 0) != 0)
			{
				BrotliStream.ThrowInvalidBeginCall();
			}
		}

		private void AsyncOperationCompleting()
		{
			Interlocked.CompareExchange(ref this._activeAsyncOperation, 0, 1);
		}

		private static void ThrowInvalidBeginCall()
		{
			throw new InvalidOperationException("Only one asynchronous reader or writer is allowed time at one time.");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			BrotliStream.ValidateParameters(buffer, offset, count);
			return this.Read(new Span<byte>(buffer, offset, count));
		}

		public override int Read(Span<byte> buffer)
		{
			if (this._mode != CompressionMode.Decompress)
			{
				throw new InvalidOperationException("Can not perform Read operations on a BrotliStream constructed with CompressionMode.Compress.");
			}
			this.EnsureNotDisposed();
			int num = 0;
			OperationStatus operationStatus = OperationStatus.DestinationTooSmall;
			while (buffer.Length > 0 && operationStatus != OperationStatus.Done)
			{
				if (operationStatus == OperationStatus.NeedMoreData)
				{
					if (this._bufferCount > 0 && this._bufferOffset != 0)
					{
						this._buffer.AsSpan(this._bufferOffset, this._bufferCount).CopyTo(this._buffer);
					}
					this._bufferOffset = 0;
					int num2;
					while (this._bufferCount < this._buffer.Length && (num2 = this._stream.Read(this._buffer, this._bufferCount, this._buffer.Length - this._bufferCount)) > 0)
					{
						this._bufferCount += num2;
						if (this._bufferCount > this._buffer.Length)
						{
							throw new InvalidDataException("BrotliStream.BaseStream returned more bytes than requested in Read.");
						}
					}
					if (this._bufferCount <= 0)
					{
						break;
					}
				}
				int num3;
				int num4;
				operationStatus = this._decoder.Decompress(this._buffer.AsSpan(this._bufferOffset, this._bufferCount), buffer, out num3, out num4);
				if (operationStatus == OperationStatus.InvalidData)
				{
					throw new InvalidOperationException("Decoder ran into invalid data.");
				}
				if (num3 > 0)
				{
					this._bufferOffset += num3;
					this._bufferCount -= num3;
				}
				if (num4 > 0)
				{
					num += num4;
					buffer = buffer.Slice(num4);
				}
			}
			return num;
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.ReadAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			return TaskToApm.End<int>(asyncResult);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			BrotliStream.ValidateParameters(buffer, offset, count);
			return this.ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken).AsTask();
		}

		public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this._mode != CompressionMode.Decompress)
			{
				throw new InvalidOperationException("Can not perform Read operations on a BrotliStream constructed with CompressionMode.Compress.");
			}
			this.EnsureNoActiveAsyncOperation();
			this.EnsureNotDisposed();
			if (cancellationToken.IsCancellationRequested)
			{
				return new ValueTask<int>(Task.FromCanceled<int>(cancellationToken));
			}
			return this.FinishReadAsyncMemory(buffer, cancellationToken);
		}

		private ValueTask<int> FinishReadAsyncMemory(Memory<byte> buffer, CancellationToken cancellationToken)
		{
			BrotliStream.<FinishReadAsyncMemory>d__41 <FinishReadAsyncMemory>d__;
			<FinishReadAsyncMemory>d__.<>4__this = this;
			<FinishReadAsyncMemory>d__.buffer = buffer;
			<FinishReadAsyncMemory>d__.cancellationToken = cancellationToken;
			<FinishReadAsyncMemory>d__.<>t__builder = AsyncValueTaskMethodBuilder<int>.Create();
			<FinishReadAsyncMemory>d__.<>1__state = -1;
			<FinishReadAsyncMemory>d__.<>t__builder.Start<BrotliStream.<FinishReadAsyncMemory>d__41>(ref <FinishReadAsyncMemory>d__);
			return <FinishReadAsyncMemory>d__.<>t__builder.Task;
		}

		public BrotliStream(Stream stream, CompressionLevel compressionLevel) : this(stream, compressionLevel, false)
		{
		}

		public BrotliStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen) : this(stream, CompressionMode.Compress, leaveOpen)
		{
			this._encoder.SetQuality(BrotliUtils.GetQualityFromCompressionLevel(compressionLevel));
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			BrotliStream.ValidateParameters(buffer, offset, count);
			this.WriteCore(new ReadOnlySpan<byte>(buffer, offset, count), false);
		}

		public override void Write(ReadOnlySpan<byte> buffer)
		{
			this.WriteCore(buffer, false);
		}

		internal void WriteCore(ReadOnlySpan<byte> buffer, bool isFinalBlock = false)
		{
			if (this._mode != CompressionMode.Compress)
			{
				throw new InvalidOperationException("Can not perform Write operations on a BrotliStream constructed with CompressionMode.Decompress.");
			}
			this.EnsureNotDisposed();
			OperationStatus operationStatus = OperationStatus.DestinationTooSmall;
			Span<byte> destination = new Span<byte>(this._buffer);
			while (operationStatus == OperationStatus.DestinationTooSmall)
			{
				int num = 0;
				int num2 = 0;
				operationStatus = this._encoder.Compress(buffer, destination, out num, out num2, isFinalBlock);
				if (operationStatus == OperationStatus.InvalidData)
				{
					throw new InvalidOperationException("Encoder ran into invalid data.");
				}
				if (num2 > 0)
				{
					this._stream.Write(destination.Slice(0, num2));
				}
				if (num > 0)
				{
					buffer = buffer.Slice(num);
				}
			}
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback asyncCallback, object asyncState)
		{
			return TaskToApm.Begin(this.WriteAsync(buffer, offset, count, CancellationToken.None), asyncCallback, asyncState);
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			TaskToApm.End(asyncResult);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			BrotliStream.ValidateParameters(buffer, offset, count);
			return this.WriteAsync(new ReadOnlyMemory<byte>(buffer, offset, count), cancellationToken).AsTask();
		}

		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (this._mode != CompressionMode.Compress)
			{
				throw new InvalidOperationException("Can not perform Write operations on a BrotliStream constructed with CompressionMode.Decompress.");
			}
			this.EnsureNoActiveAsyncOperation();
			this.EnsureNotDisposed();
			return new ValueTask(cancellationToken.IsCancellationRequested ? Task.FromCanceled<int>(cancellationToken) : this.WriteAsyncMemoryCore(buffer, cancellationToken));
		}

		private Task WriteAsyncMemoryCore(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
		{
			BrotliStream.<WriteAsyncMemoryCore>d__52 <WriteAsyncMemoryCore>d__;
			<WriteAsyncMemoryCore>d__.<>4__this = this;
			<WriteAsyncMemoryCore>d__.buffer = buffer;
			<WriteAsyncMemoryCore>d__.cancellationToken = cancellationToken;
			<WriteAsyncMemoryCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WriteAsyncMemoryCore>d__.<>1__state = -1;
			<WriteAsyncMemoryCore>d__.<>t__builder.Start<BrotliStream.<WriteAsyncMemoryCore>d__52>(ref <WriteAsyncMemoryCore>d__);
			return <WriteAsyncMemoryCore>d__.<>t__builder.Task;
		}

		public override void Flush()
		{
			this.EnsureNotDisposed();
			if (this._mode == CompressionMode.Compress)
			{
				if (this._encoder._state == null || this._encoder._state.IsClosed)
				{
					return;
				}
				OperationStatus operationStatus = OperationStatus.DestinationTooSmall;
				Span<byte> destination = new Span<byte>(this._buffer);
				while (operationStatus == OperationStatus.DestinationTooSmall)
				{
					int num = 0;
					operationStatus = this._encoder.Flush(destination, out num);
					if (operationStatus == OperationStatus.InvalidData)
					{
						throw new InvalidDataException("Encoder ran into invalid data.");
					}
					if (num > 0)
					{
						this._stream.Write(destination.Slice(0, num));
					}
				}
			}
		}

		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			this.EnsureNoActiveAsyncOperation();
			this.EnsureNotDisposed();
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled(cancellationToken);
			}
			if (this._mode == CompressionMode.Compress)
			{
				return this.FlushAsyncCore(cancellationToken);
			}
			return Task.CompletedTask;
		}

		private Task FlushAsyncCore(CancellationToken cancellationToken)
		{
			BrotliStream.<FlushAsyncCore>d__55 <FlushAsyncCore>d__;
			<FlushAsyncCore>d__.<>4__this = this;
			<FlushAsyncCore>d__.cancellationToken = cancellationToken;
			<FlushAsyncCore>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<FlushAsyncCore>d__.<>1__state = -1;
			<FlushAsyncCore>d__.<>t__builder.Start<BrotliStream.<FlushAsyncCore>d__55>(ref <FlushAsyncCore>d__);
			return <FlushAsyncCore>d__.<>t__builder.Task;
		}

		private const int DefaultInternalBufferSize = 65520;

		private Stream _stream;

		private readonly byte[] _buffer;

		private readonly bool _leaveOpen;

		private readonly CompressionMode _mode;

		private int _activeAsyncOperation;

		private BrotliDecoder _decoder;

		private int _bufferOffset;

		private int _bufferCount;

		private BrotliEncoder _encoder;
	}
}
