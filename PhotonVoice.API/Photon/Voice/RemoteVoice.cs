using System;
using System.Collections.Generic;
using System.Threading;

namespace Photon.Voice
{
	internal class RemoteVoice : IDisposable
	{
		internal VoiceInfo Info { get; private set; }

		internal int DelayFrames { get; set; }

		internal RemoteVoice(VoiceClient client, RemoteVoiceOptions options, int channelId, int playerId, byte voiceId, VoiceInfo info, byte lastEventNumber)
		{
			this.options = options;
			this.LogPrefix = options.logPrefix;
			this.voiceClient = client;
			this.channelId = channelId;
			this.playerId = playerId;
			this.voiceId = voiceId;
			this.Info = info;
			this.lastEvNumber = lastEventNumber;
			if (this.options.Decoder == null)
			{
				string fmt = this.LogPrefix + ": decoder is null (set it with options Decoder property or SetOutput method in OnRemoteVoiceInfoAction)";
				this.voiceClient.logger.LogError(fmt, Array.Empty<object>());
				this.disposed = true;
				return;
			}
			Thread thread = new Thread(delegate()
			{
				this.decodeThread();
			});
			Util.SetThreadName(thread, "[PV] Dec" + this.shortName);
			thread.Start();
		}

		private string shortName
		{
			get
			{
				return string.Concat(new string[]
				{
					"v#",
					this.voiceId.ToString(),
					"ch#",
					this.voiceClient.channelStr(this.channelId),
					"p#",
					this.playerId.ToString()
				});
			}
		}

		public string LogPrefix { get; private set; }

		public void ReceiveSpacingProfileStart()
		{
			this.receiveSpacingProfile.Start();
		}

		public string ReceiveSpacingProfileDump
		{
			get
			{
				return this.receiveSpacingProfile.Dump;
			}
		}

		public int ReceiveSpacingProfileMax
		{
			get
			{
				return this.receiveSpacingProfile.Max;
			}
		}

		private static byte byteDiff(byte latest, byte last)
		{
			return latest - (last + 1);
		}

		internal void receiveBytes(ref FrameBuffer receivedBytes, byte evNumber)
		{
			if (evNumber != this.lastEvNumber)
			{
				int num = (int)RemoteVoice.byteDiff(evNumber, this.lastEvNumber);
				if (num == 0)
				{
					this.lastEvNumber = evNumber;
				}
				else if (num < 127)
				{
					this.voiceClient.logger.LogWarning(string.Concat(new string[]
					{
						this.LogPrefix,
						" evNumer: ",
						evNumber.ToString(),
						" playerVoice.lastEvNumber: ",
						this.lastEvNumber.ToString(),
						" missing: ",
						num.ToString(),
						" r/b ",
						receivedBytes.Length.ToString()
					}), Array.Empty<object>());
					this.voiceClient.FramesLost += num;
					this.lastEvNumber = evNumber;
					this.receiveNullFrames(num);
				}
				else
				{
					this.voiceClient.logger.LogWarning(string.Concat(new string[]
					{
						this.LogPrefix,
						" evNumer: ",
						evNumber.ToString(),
						" playerVoice.lastEvNumber: ",
						this.lastEvNumber.ToString(),
						" late: ",
						(255 - num).ToString(),
						" r/b ",
						receivedBytes.Length.ToString()
					}), Array.Empty<object>());
				}
			}
			this.receiveFrame(ref receivedBytes);
		}

		private void receiveFrame(ref FrameBuffer frame)
		{
			object obj = this.disposeLock;
			lock (obj)
			{
				if (!this.disposed)
				{
					this.receiveSpacingProfile.Update(false, (frame.Flags & FrameFlags.EndOfStream) > (FrameFlags)0);
					Queue<FrameBuffer> obj2 = this.frameQueue;
					lock (obj2)
					{
						this.frameQueue.Enqueue(frame);
						frame.Retain();
						if ((frame.Flags & FrameFlags.EndOfStream) != (FrameFlags)0)
						{
							this.flushingFramePosInQueue = this.frameQueue.Count - 1;
						}
					}
					this.frameQueueReady.Set();
				}
			}
		}

		private void receiveNullFrames(int count)
		{
			object obj = this.disposeLock;
			lock (obj)
			{
				if (!this.disposed)
				{
					for (int i = 0; i < count; i++)
					{
						this.receiveSpacingProfile.Update(true, false);
						Queue<FrameBuffer> obj2 = this.frameQueue;
						lock (obj2)
						{
							this.frameQueue.Enqueue(this.nullFrame);
						}
					}
					this.frameQueueReady.Set();
				}
			}
		}

		private void decodeThread()
		{
			this.voiceClient.logger.LogInfo(this.LogPrefix + ": Starting decode thread", Array.Empty<object>());
			IDecoder decoder = this.options.Decoder;
			try
			{
				decoder.Open(this.Info);
				while (!this.disposed)
				{
					this.frameQueueReady.WaitOne();
					while (!this.disposed)
					{
						bool flag = false;
						Queue<FrameBuffer> obj = this.frameQueue;
						FrameBuffer frameBuffer;
						lock (obj)
						{
							int num = 0;
							if (this.flushingFramePosInQueue < 0 && this.DelayFrames > 0 && this.DelayFrames < 300)
							{
								num = this.DelayFrames;
							}
							if (this.frameQueue.Count <= num)
							{
								break;
							}
							frameBuffer = this.frameQueue.Dequeue();
							this.flushingFramePosInQueue--;
							if (this.flushingFramePosInQueue == -2147483648)
							{
								this.flushingFramePosInQueue = -1;
							}
							flag = true;
						}
						if (flag)
						{
							decoder.Input(ref frameBuffer);
							frameBuffer.Release();
						}
					}
				}
			}
			catch (Exception ex)
			{
				ILogger logger = this.voiceClient.logger;
				string logPrefix = this.LogPrefix;
				string str = ": Exception in decode thread: ";
				Exception ex2 = ex;
				logger.LogError(logPrefix + str + ((ex2 != null) ? ex2.ToString() : null), Array.Empty<object>());
				throw ex;
			}
			finally
			{
				object obj2 = this.disposeLock;
				lock (obj2)
				{
					this.disposed = true;
				}
				this.frameQueueReady.Close();
				Queue<FrameBuffer> obj = this.frameQueue;
				lock (obj)
				{
					while (this.frameQueue.Count > 0)
					{
						this.frameQueue.Dequeue().Release();
					}
				}
				decoder.Dispose();
				this.voiceClient.logger.LogInfo(this.LogPrefix + ": Exiting decode thread", Array.Empty<object>());
			}
		}

		internal void removeAndDispose()
		{
			if (this.options.OnRemoteVoiceRemoveAction != null)
			{
				this.options.OnRemoteVoiceRemoveAction();
			}
			this.Dispose();
		}

		public void Dispose()
		{
			object obj = this.disposeLock;
			lock (obj)
			{
				if (!this.disposed)
				{
					this.disposed = true;
					this.frameQueueReady.Set();
				}
			}
		}

		internal RemoteVoiceOptions options;

		internal int channelId;

		private int playerId;

		private byte voiceId;

		private volatile bool disposed;

		private object disposeLock = new object();

		private SpacingProfile receiveSpacingProfile = new SpacingProfile(1000);

		internal byte lastEvNumber;

		private VoiceClient voiceClient;

		private Queue<FrameBuffer> frameQueue = new Queue<FrameBuffer>();

		private AutoResetEvent frameQueueReady = new AutoResetEvent(false);

		private int flushingFramePosInQueue = -1;

		private FrameBuffer nullFrame;
	}
}
