using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine.Networking;

namespace Modio.Unity
{
	internal class StreamingDownloadHandler : DownloadHandlerScript
	{
		internal StreamingDownloadHandler(int bufferSize = 1048576, CancellationToken token = default(CancellationToken)) : this(new byte[bufferSize], token)
		{
		}

		private StreamingDownloadHandler(byte[] buffer, CancellationToken token = default(CancellationToken)) : base(buffer)
		{
			this._streamBuffer = new StreamingDownloadHandler.ChunkedStreamBuffer(token);
			this._cancellationToken = token;
		}

		public void SetCallingRequest(UnityWebRequest request)
		{
			this._callingRequest = request;
		}

		internal Stream GetStream()
		{
			return this._streamBuffer;
		}

		protected override bool ReceiveData(byte[] dataReceived, int dataLength)
		{
			if (this._cancellationToken.IsCancellationRequested)
			{
				this._callingRequest.Abort();
				this._streamBuffer.Flush();
				this._hasReceivedHeaders.TrySetCanceled();
				return true;
			}
			this._streamBuffer.Write(dataReceived, 0, dataLength);
			this._hasReceivedHeaders.TrySetResult(true);
			return true;
		}

		public Task ResponseReceived(CancellationToken token)
		{
			StreamingDownloadHandler.<ResponseReceived>d__9 <ResponseReceived>d__;
			<ResponseReceived>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ResponseReceived>d__.<>4__this = this;
			<ResponseReceived>d__.<>1__state = -1;
			<ResponseReceived>d__.<>t__builder.Start<StreamingDownloadHandler.<ResponseReceived>d__9>(ref <ResponseReceived>d__);
			return <ResponseReceived>d__.<>t__builder.Task;
		}

		protected override void CompleteContent()
		{
			base.CompleteContent();
			this._streamBuffer.Complete();
		}

		private readonly StreamingDownloadHandler.ChunkedStreamBuffer _streamBuffer;

		private readonly CancellationToken _cancellationToken;

		private readonly TaskCompletionSource<bool> _hasReceivedHeaders = new TaskCompletionSource<bool>();

		private UnityWebRequest _callingRequest;

		private class ChunkedStreamBuffer : Stream
		{
			internal ChunkedStreamBuffer(CancellationToken shutdownToken)
			{
				this._shutdownToken = shutdownToken;
			}

			public override void Flush()
			{
				StreamingDownloadHandler.ChunkedStreamBuffer.BufferChunk bufferChunk;
				while (this._dataQueue.TryDequeue(out bufferChunk))
				{
					bufferChunk.Dispose();
				}
				this._dataQueue.Clear();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				return this.ReadAsync(buffer, offset, count, CancellationToken.None).Result;
			}

			public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				StreamingDownloadHandler.ChunkedStreamBuffer.<ReadAsync>d__6 <ReadAsync>d__;
				<ReadAsync>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
				<ReadAsync>d__.<>4__this = this;
				<ReadAsync>d__.buffer = buffer;
				<ReadAsync>d__.offset = offset;
				<ReadAsync>d__.count = count;
				<ReadAsync>d__.cancellationToken = cancellationToken;
				<ReadAsync>d__.<>1__state = -1;
				<ReadAsync>d__.<>t__builder.Start<StreamingDownloadHandler.ChunkedStreamBuffer.<ReadAsync>d__6>(ref <ReadAsync>d__);
				return <ReadAsync>d__.<>t__builder.Task;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotSupportedException();
			}

			public override void SetLength(long value)
			{
				throw new NotSupportedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				if (this._shutdownToken.IsCancellationRequested)
				{
					return;
				}
				int num = Math.Min(buffer.Length, count);
				NativeArray<byte> nativeArray = new NativeArray<byte>(num, Allocator.Persistent, NativeArrayOptions.ClearMemory);
				NativeArray<byte>.Copy(buffer, offset, nativeArray, 0, num - offset);
				this._dataQueue.Enqueue(new StreamingDownloadHandler.ChunkedStreamBuffer.BufferChunk(nativeArray, 0));
				this._signal.Set();
			}

			public override bool CanRead
			{
				get
				{
					return true;
				}
			}

			public override bool CanSeek
			{
				get
				{
					return false;
				}
			}

			public override bool CanWrite
			{
				get
				{
					return true;
				}
			}

			public override long Length
			{
				get
				{
					return -1L;
				}
			}

			public override long Position { get; set; } = -1L;

			private bool IsDone { get; set; }

			public void Complete()
			{
				this.IsDone = true;
				this._signal.Set();
			}

			private readonly ConcurrentQueue<StreamingDownloadHandler.ChunkedStreamBuffer.BufferChunk> _dataQueue = new ConcurrentQueue<StreamingDownloadHandler.ChunkedStreamBuffer.BufferChunk>();

			private readonly CancellationToken _shutdownToken;

			private readonly StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent _signal = new StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent();

			private class BufferChunk : IDisposable
			{
				internal NativeArray<byte> Data { get; }

				internal int Offset { get; set; }

				internal int Length
				{
					get
					{
						return this.Data.Length;
					}
				}

				internal bool HasData
				{
					get
					{
						return this.Offset < this.Data.Length;
					}
				}

				internal int RemainingLength
				{
					get
					{
						return this.Data.Length - this.Offset;
					}
				}

				internal BufferChunk(NativeArray<byte> data, int offset)
				{
					this.Data = data;
					this.Offset = offset;
				}

				public void Dispose()
				{
					this.Data.Dispose();
				}
			}

			private class AsyncAutoResetEvent
			{
				public Task WaitAsync(CancellationToken cancellationToken = default(CancellationToken))
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return Task.FromCanceled(cancellationToken);
					}
					Queue<TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty>> signalWaiters = this._signalWaiters;
					Task result;
					lock (signalWaiters)
					{
						if (this._signaled)
						{
							this._signaled = false;
							result = Task.CompletedTask;
						}
						else
						{
							TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty> taskCompletionSource = new TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty>(TaskCreationOptions.RunContinuationsAsynchronously);
							if (cancellationToken.IsCancellationRequested)
							{
								taskCompletionSource.TrySetCanceled(cancellationToken);
								result = taskCompletionSource.Task;
							}
							else
							{
								this._signalWaiters.Enqueue(taskCompletionSource);
								result = taskCompletionSource.Task;
							}
						}
					}
					return result;
				}

				public void Set()
				{
					TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty> taskCompletionSource = null;
					Queue<TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty>> signalWaiters = this._signalWaiters;
					lock (signalWaiters)
					{
						if (this._signalWaiters.Count > 0)
						{
							taskCompletionSource = this._signalWaiters.Dequeue();
						}
						else
						{
							this._signaled = true;
						}
					}
					if (taskCompletionSource != null)
					{
						taskCompletionSource.TrySetResult(default(StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty));
					}
				}

				private readonly Queue<TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty>> _signalWaiters = new Queue<TaskCompletionSource<StreamingDownloadHandler.ChunkedStreamBuffer.AsyncAutoResetEvent.Empty>>();

				private bool _signaled;

				private struct Empty
				{
				}
			}
		}
	}
}
