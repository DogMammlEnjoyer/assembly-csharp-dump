using System;
using System.Collections.Generic;

namespace Photon.Voice
{
	public class LocalVoice : IDisposable
	{
		[Obsolete("Use InterestGroup.")]
		public byte Group
		{
			get
			{
				return this.InterestGroup;
			}
			set
			{
				this.InterestGroup = value;
			}
		}

		public byte InterestGroup { get; set; }

		public VoiceInfo Info
		{
			get
			{
				return this.info;
			}
		}

		public bool TransmitEnabled
		{
			get
			{
				return this.transmitEnabled;
			}
			set
			{
				if (this.transmitEnabled != value)
				{
					if (this.transmitEnabled && this.encoder != null && this.voiceClient.transport.IsChannelJoined(this.channelId))
					{
						this.encoder.EndOfStream();
					}
					this.transmitEnabled = value;
				}
			}
		}

		public bool IsCurrentlyTransmitting
		{
			get
			{
				return Environment.TickCount - this.lastTransmitTime < 100;
			}
		}

		public int FramesSent { get; private set; }

		public int FramesSentBytes { get; private set; }

		public bool Reliable { get; set; }

		public bool Encrypt { get; set; }

		public IServiceable LocalUserServiceable { get; set; }

		public bool DebugEchoMode
		{
			get
			{
				return this.debugEchoMode;
			}
			set
			{
				if (this.debugEchoMode != value)
				{
					this.debugEchoMode = value;
					if (this.voiceClient != null && this.voiceClient.transport != null && this.voiceClient.transport.IsChannelJoined(this.channelId))
					{
						if (this.debugEchoMode)
						{
							this.voiceClient.sendVoicesInfoAndConfigFrame(new List<LocalVoice>
							{
								this
							}, this.channelId, -1);
							return;
						}
						this.voiceClient.transport.SendVoiceRemove(this, this.channelId, -1);
					}
				}
			}
		}

		public void SendSpacingProfileStart()
		{
			this.sendSpacingProfile.Start();
		}

		public string SendSpacingProfileDump
		{
			get
			{
				return this.sendSpacingProfile.Dump;
			}
		}

		public int SendSpacingProfileMax
		{
			get
			{
				return this.sendSpacingProfile.Max;
			}
		}

		public byte ID
		{
			get
			{
				return this.id;
			}
		}

		public byte EvNumber
		{
			get
			{
				return this.evNumber;
			}
		}

		internal LocalVoice()
		{
		}

		internal LocalVoice(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId)
		{
			this.info = voiceInfo;
			this.channelId = channelId;
			this.voiceClient = voiceClient;
			this.id = id;
			if (encoder == null)
			{
				string fmt = this.LogPrefix + ": encoder is null";
				voiceClient.logger.LogError(fmt, Array.Empty<object>());
				throw new ArgumentNullException("encoder");
			}
			this.encoder = encoder;
			this.encoder.Output = new Action<ArraySegment<byte>, FrameFlags>(this.sendFrame);
		}

		protected string shortName
		{
			get
			{
				return "v#" + this.id.ToString() + "ch#" + this.voiceClient.channelStr(this.channelId);
			}
		}

		public string Name
		{
			get
			{
				return string.Concat(new string[]
				{
					"Local ",
					this.info.Codec.ToString(),
					" v#",
					this.id.ToString(),
					" ch#",
					this.voiceClient.channelStr(this.channelId)
				});
			}
		}

		public string LogPrefix
		{
			get
			{
				return "[PV] " + this.Name;
			}
		}

		internal virtual void service()
		{
			for (;;)
			{
				FrameFlags flags;
				ArraySegment<byte> compressed = this.encoder.DequeueOutput(out flags);
				if (compressed.Count == 0)
				{
					break;
				}
				this.sendFrame(compressed, flags);
			}
			if (this.LocalUserServiceable != null)
			{
				this.LocalUserServiceable.Service(this);
			}
		}

		internal void sendConfigFrame(int targetPlayerId)
		{
			if (this.configFrame.Count != 0)
			{
				this.voiceClient.logger.LogInfo(this.LogPrefix + " Sending config frame to pl " + targetPlayerId.ToString(), Array.Empty<object>());
				this.sendFrame0(this.configFrame, FrameFlags.Config, targetPlayerId, true);
			}
		}

		internal void sendFrame(ArraySegment<byte> compressed, FrameFlags flags)
		{
			if ((flags & FrameFlags.Config) != (FrameFlags)0)
			{
				byte[] array = (this.configFrame.Array != null && this.configFrame.Array.Length >= compressed.Count) ? this.configFrame.Array : new byte[compressed.Count];
				Buffer.BlockCopy(compressed.Array, compressed.Offset, array, 0, compressed.Count);
				this.configFrame = new ArraySegment<byte>(array, 0, compressed.Count);
				this.voiceClient.logger.LogInfo(this.LogPrefix + " Got config frame " + this.configFrame.Count.ToString() + " bytes", Array.Empty<object>());
			}
			if (this.voiceClient.transport.IsChannelJoined(this.channelId) && this.TransmitEnabled)
			{
				this.sendFrame0(compressed, flags, 0, this.Reliable);
			}
		}

		internal void sendFrame0(ArraySegment<byte> compressed, FrameFlags flags, int targetPlayerId, bool reliable)
		{
			if ((flags & FrameFlags.Config) != (FrameFlags)0)
			{
				reliable = true;
			}
			if ((flags & FrameFlags.KeyFrame) != (FrameFlags)0)
			{
				reliable = true;
			}
			FrameFlags frameFlags = flags & FrameFlags.EndOfStream;
			int framesSent = this.FramesSent;
			this.FramesSent = framesSent + 1;
			this.FramesSentBytes += compressed.Count;
			this.voiceClient.transport.SendFrame(compressed, flags, this.evNumber, this.id, this.channelId, targetPlayerId, reliable, this);
			this.sendSpacingProfile.Update(false, false);
			if (this.DebugEchoMode)
			{
				this.eventTimestamps[this.evNumber] = Environment.TickCount;
			}
			this.evNumber += 1;
			if (compressed.Count > 0 && (flags & FrameFlags.Config) == (FrameFlags)0)
			{
				this.lastTransmitTime = Environment.TickCount;
			}
		}

		public void RemoveSelf()
		{
			if (this.voiceClient != null)
			{
				this.voiceClient.RemoveLocalVoice(this);
			}
		}

		public virtual void Dispose()
		{
			if (!this.disposed)
			{
				if (this.encoder != null)
				{
					this.encoder.Dispose();
				}
				this.disposed = true;
			}
		}

		public const int DATA_POOL_CAPACITY = 50;

		private bool transmitEnabled = true;

		private bool debugEchoMode;

		protected VoiceInfo info;

		protected IEncoder encoder;

		internal byte id;

		internal int channelId;

		internal byte evNumber;

		protected VoiceClient voiceClient;

		protected ArraySegment<byte> configFrame;

		protected volatile bool disposed;

		protected object disposeLock = new object();

		private const int NO_TRANSMIT_TIMEOUT_MS = 100;

		private int lastTransmitTime = Environment.TickCount - 100;

		internal Dictionary<byte, int> eventTimestamps = new Dictionary<byte, int>();

		private SpacingProfile sendSpacingProfile = new SpacingProfile(1000);
	}
}
