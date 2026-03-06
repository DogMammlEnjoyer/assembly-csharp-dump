using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Audio.Decoding;
using Meta.Voice.Logging;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace Meta.WitAi.Requests
{
	[Preserve]
	[LogCategory(LogCategory.Audio, LogCategory.Output)]
	internal class AudioStreamHandler : DownloadHandlerScript, IVRequestDownloadDecoder, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Output, null);

		public bool IsStarted { get; private set; }

		public event VRequestResponseDelegate OnFirstResponse;

		public event VRequestResponseDelegate OnResponse;

		public float Progress { get; private set; }

		public event VRequestProgressDelegate OnProgress;

		public bool IsComplete { get; private set; }

		public TaskCompletionSource<bool> Completion { get; } = new TaskCompletionSource<bool>();

		public bool IsError { get; private set; }

		public IAudioDecoder AudioDecoder { get; }

		public bool WillDecodeInBackground
		{
			get
			{
				return this.AudioDecoder.WillDecodeInBackground;
			}
		}

		public AudioSampleDecodeDelegate OnSamplesDecoded { get; }

		private bool _decodeComplete
		{
			get
			{
				return this._decodedBytes == this.Max(this._receivedBytes, this._expectedBytes);
			}
		}

		private ulong Max(ulong var1, ulong var2)
		{
			if (var1 <= var2)
			{
				return var2;
			}
			return var1;
		}

		public AudioStreamHandler(IAudioDecoder audioDecoder, AudioSampleDecodeDelegate onSamplesDecoded)
		{
			this.AudioDecoder = audioDecoder;
			this.OnSamplesDecoded = onSamplesDecoded;
			if (this.WillDecodeInBackground)
			{
				this._buffers = new Queue<byte[]>();
			}
		}

		~AudioStreamHandler()
		{
			this.UnloadBuffers();
		}

		[Preserve]
		protected override void ReceiveContentLengthHeader(ulong contentLength)
		{
			if (contentLength == 0UL || this.IsComplete)
			{
				return;
			}
			this._expectedBytes = contentLength;
			this.IsError = (this._expectedBytes < 2400UL);
		}

		[Preserve]
		protected override bool ReceiveData(byte[] bufferData, int length)
		{
			if (!base.ReceiveData(bufferData, length) || this.IsComplete)
			{
				return false;
			}
			if (!this.IsStarted)
			{
				this.IsStarted = true;
				VRequestResponseDelegate onFirstResponse = this.OnFirstResponse;
				if (onFirstResponse != null)
				{
					onFirstResponse();
				}
			}
			VRequestResponseDelegate onResponse = this.OnResponse;
			if (onResponse != null)
			{
				onResponse();
			}
			if (this.WillDecodeInBackground)
			{
				this.EnqueueAndDecodeChunkAsync(bufferData, 0, length);
			}
			else
			{
				this._receivedBytes += (ulong)length;
				this.DecodeChunk(bufferData, 0, length);
				if (this._decodeComplete)
				{
					this.TryToFinalize();
				}
			}
			return true;
		}

		private void EnqueueAndDecodeChunkAsync(byte[] chunk, int offset, int length)
		{
			while (length > 0)
			{
				if (this._inBuffer == null)
				{
					this._inBuffer = AudioStreamHandler._bufferPool.Get();
					Queue<byte[]> buffers = this._buffers;
					lock (buffers)
					{
						this._buffers.Enqueue(this._inBuffer);
					}
				}
				int num = Mathf.Min(length, this._inBuffer.Length - this._inBufferOffset);
				Array.Copy(chunk, offset, this._inBuffer, this._inBufferOffset, num);
				offset += num;
				length -= num;
				this._inBufferOffset += num;
				if (this._inBufferOffset >= this._inBuffer.Length)
				{
					this._inBufferOffset = 0;
					this._inBuffer = null;
				}
				this._receivedBytes += (ulong)((long)num);
			}
			if (this._decoder == null)
			{
				this._decoder = ThreadUtility.Background(this.Logger, new Action(this.DecodeAsync));
			}
		}

		private void DecodeAsync()
		{
			if (this.IsError)
			{
				this._decodedBytes = this._receivedBytes;
				ThreadUtility.CallOnMainThread(new Action(this.TryToFinalize)).WrapErrors();
				return;
			}
			while (this._decodedBytes < this._receivedBytes)
			{
				if (this._decodeBuffer == null)
				{
					Queue<byte[]> buffers = this._buffers;
					lock (buffers)
					{
						if (!this._buffers.TryDequeue(out this._decodeBuffer))
						{
							break;
						}
					}
				}
				int num = Mathf.Min((int)(this._receivedBytes - this._decodedBytes), this._decodeBuffer.Length - this._decodeBufferOffset);
				this.DecodeChunk(this._decodeBuffer, this._decodeBufferOffset, num);
				this._decodeBufferOffset += num;
				if (this._decodeBufferOffset >= this._decodeBuffer.Length)
				{
					this._decodeBufferOffset = 0;
					AudioStreamHandler._bufferPool.Return(this._decodeBuffer);
					this._decodeBuffer = null;
				}
				this.RefreshProgress();
			}
			this._decoder = null;
			if (this._decodeComplete)
			{
				ThreadUtility.CallOnMainThread(new Action(this.TryToFinalize)).WrapErrors();
			}
		}

		private void DecodeChunk(byte[] chunk, int offset, int length)
		{
			try
			{
				this.AudioDecoder.Decode(chunk, offset, length, this.OnSamplesDecoded);
			}
			catch (Exception ex)
			{
				this.Logger.Error("AudioStreamHandler Decode Failed\nException: {0}", new object[]
				{
					ex
				});
			}
			finally
			{
				this._decodedBytes += (ulong)((long)length);
			}
		}

		[Preserve]
		protected override string GetText()
		{
			if (this.IsError && this._inBuffer != null)
			{
				return Encoding.UTF8.GetString(this._inBuffer, 0, this._inBufferOffset);
			}
			return null;
		}

		private void RefreshProgress()
		{
			if (this._expectedBytes <= 0UL)
			{
				return;
			}
			float progress = this.GetProgress();
			if (!this.Progress.Equals(progress))
			{
				this.Progress = progress;
				VRequestProgressDelegate onProgress = this.OnProgress;
				if (onProgress == null)
				{
					return;
				}
				onProgress(progress);
			}
		}

		[Preserve]
		protected override float GetProgress()
		{
			if (this._expectedBytes > 0UL)
			{
				return Mathf.Clamp01(this._decodedBytes / this._expectedBytes);
			}
			return 0f;
		}

		[Preserve]
		protected override void CompleteContent()
		{
			if (this._requestComplete)
			{
				return;
			}
			this._requestComplete = true;
			this.TryToFinalize();
		}

		private void TryToFinalize()
		{
			if (this.IsComplete || !this._requestComplete || !this._decodeComplete)
			{
				return;
			}
			this.IsComplete = true;
			this.Dispose();
		}

		public override void Dispose()
		{
			base.Dispose();
			this.UnloadBuffers();
			this.IsComplete = true;
			this.Completion.TrySetResult(true);
		}

		private void UnloadBuffers()
		{
			if (this._unloaded)
			{
				return;
			}
			this._inBuffer = null;
			if (this.WillDecodeInBackground)
			{
				Queue<byte[]> buffers = this._buffers;
				lock (buffers)
				{
					byte[] item;
					while (this._buffers.TryDequeue(out item))
					{
						AudioStreamHandler._bufferPool.Return(item);
					}
				}
			}
			if (this._decodeBuffer != null)
			{
				AudioStreamHandler._bufferPool.Return(this._decodeBuffer);
				this._decodeBuffer = null;
			}
			this._unloaded = true;
		}

		private const int BUFFER_LENGTH = 24000;

		private static readonly ArrayPool<byte> _bufferPool = new ArrayPool<byte>(24000, 0);

		private readonly Queue<byte[]> _buffers;

		private byte[] _inBuffer;

		private int _inBufferOffset;

		private byte[] _decodeBuffer;

		private int _decodeBufferOffset;

		private ulong _expectedBytes;

		private ulong _receivedBytes;

		private ulong _decodedBytes;

		private bool _requestComplete;

		private Task _decoder;

		private bool _unloaded;
	}
}
