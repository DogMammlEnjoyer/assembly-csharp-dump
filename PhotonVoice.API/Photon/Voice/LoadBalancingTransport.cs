using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice
{
	public class LoadBalancingTransport : LoadBalancingClient, IVoiceTransport, ILogger, IDisposable
	{
		public VoiceClient VoiceClient
		{
			get
			{
				return this.voiceClient;
			}
		}

		public void LogError(string fmt, params object[] args)
		{
			this.DebugReturn(DebugLevel.ERROR, string.Format(fmt, args));
		}

		public void LogWarning(string fmt, params object[] args)
		{
			this.DebugReturn(DebugLevel.WARNING, string.Format(fmt, args));
		}

		public void LogInfo(string fmt, params object[] args)
		{
			this.DebugReturn(DebugLevel.INFO, string.Format(fmt, args));
		}

		public void LogDebug(string fmt, params object[] args)
		{
			this.DebugReturn(DebugLevel.ALL, string.Format(fmt, args));
		}

		internal byte photonChannelForCodec(Codec c)
		{
			return (byte)(1 + Array.IndexOf(Enum.GetValues(typeof(Codec)), c));
		}

		public bool IsChannelJoined(int channelId)
		{
			return base.State == ClientState.Joined;
		}

		public LoadBalancingTransport(ILogger logger = null, ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp) : base(connectionProtocol)
		{
			if (logger == null)
			{
				logger = this;
			}
			base.EventReceived += this.onEventActionVoiceClient;
			base.StateChanged += this.onStateChangeVoiceClient;
			this.voiceClient = new VoiceClient(this, logger, default(VoiceClient.CreateOptions));
			int num = Enum.GetValues(typeof(Codec)).Length + 1;
			if ((int)base.LoadBalancingPeer.ChannelCount < num)
			{
				base.LoadBalancingPeer.ChannelCount = (byte)num;
			}
			this.protocol = new PhotonTransportProtocol(this.voiceClient, logger);
		}

		public new void Service()
		{
			base.Service();
			this.voiceClient.Service();
		}

		[Obsolete("Use LoadBalancingPeer::OpChangeGroups().")]
		public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
		{
			return base.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
		}

		[Obsolete("Use GlobalInterestGroup.")]
		public byte GlobalAudioGroup
		{
			get
			{
				return this.GlobalInterestGroup;
			}
			set
			{
				this.GlobalInterestGroup = value;
			}
		}

		public byte GlobalInterestGroup
		{
			get
			{
				return this.voiceClient.GlobalInterestGroup;
			}
			set
			{
				this.voiceClient.GlobalInterestGroup = value;
				if (base.State == ClientState.Joined)
				{
					if (this.voiceClient.GlobalInterestGroup != 0)
					{
						base.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[]
						{
							this.voiceClient.GlobalInterestGroup
						});
						return;
					}
					base.LoadBalancingPeer.OpChangeGroups(new byte[0], null);
				}
			}
		}

		public void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId)
		{
			foreach (IGrouping<Codec, LocalVoice> grouping in from v in voices
			group v by v.Info.Codec)
			{
				object customEventContent = this.protocol.buildVoicesInfo(grouping, true);
				SendOptions sendOptions = new SendOptions
				{
					Reliability = true,
					Channel = this.photonChannelForCodec(grouping.Key)
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
				if (targetPlayerId == -1)
				{
					raiseEventOptions.TargetActors = new int[]
					{
						base.LocalPlayer.ActorNumber
					};
				}
				else if (targetPlayerId != 0)
				{
					raiseEventOptions.TargetActors = new int[]
					{
						targetPlayerId
					};
				}
				this.OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
			}
		}

		public void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId)
		{
			object customEventContent = this.protocol.buildVoiceRemoveMessage(voice);
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true,
				Channel = this.photonChannelForCodec(voice.Info.Codec)
			};
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			if (targetPlayerId == -1)
			{
				raiseEventOptions.TargetActors = new int[]
				{
					base.LocalPlayer.ActorNumber
				};
			}
			else if (targetPlayerId != 0)
			{
				raiseEventOptions.TargetActors = new int[]
				{
					targetPlayerId
				};
			}
			if (voice.DebugEchoMode)
			{
				raiseEventOptions.Receivers = ReceiverGroup.All;
			}
			this.OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
		}

		public virtual void SendFrame(ArraySegment<byte> data, FrameFlags flags, byte evNumber, byte voiceId, int channelId, int targetPlayerId, bool reliable, LocalVoice localVoice)
		{
			object[] customEventContent = this.protocol.buildFrameMessage(voiceId, evNumber, data, flags);
			SendOptions sendOptions = new SendOptions
			{
				Reliability = reliable,
				Channel = this.photonChannelForCodec(localVoice.Info.Codec),
				Encrypt = localVoice.Encrypt
			};
			RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
			if (targetPlayerId == -1)
			{
				raiseEventOptions.TargetActors = new int[]
				{
					base.LocalPlayer.ActorNumber
				};
			}
			else if (targetPlayerId != 0)
			{
				raiseEventOptions.TargetActors = new int[]
				{
					targetPlayerId
				};
			}
			if (localVoice.DebugEchoMode)
			{
				raiseEventOptions.Receivers = ReceiverGroup.All;
			}
			raiseEventOptions.InterestGroup = localVoice.InterestGroup;
			this.OpRaiseEvent(202, customEventContent, raiseEventOptions, sendOptions);
			while (base.LoadBalancingPeer.SendOutgoingCommands())
			{
			}
		}

		public string ChannelIdStr(int channelId)
		{
			return null;
		}

		public string PlayerIdStr(int playerId)
		{
			return null;
		}

		protected virtual void onEventActionVoiceClient(EventData ev)
		{
			if (ev.Code == 202)
			{
				this.protocol.onVoiceEvent(ev[245], 0, ev.Sender, ev.Sender == base.LocalPlayer.ActorNumber);
				return;
			}
			byte code = ev.Code;
			if (code != 254)
			{
				if (code == 255)
				{
					int sender = ev.Sender;
					if (sender != base.LocalPlayer.ActorNumber)
					{
						this.voiceClient.onPlayerJoin(0, sender);
						return;
					}
				}
			}
			else
			{
				int sender = ev.Sender;
				if (sender == base.LocalPlayer.ActorNumber)
				{
					this.voiceClient.onLeaveAllChannels();
					return;
				}
				this.voiceClient.onPlayerLeave(0, sender);
			}
		}

		private void onStateChangeVoiceClient(ClientState fromState, ClientState state)
		{
			if (fromState == ClientState.Joined)
			{
				this.voiceClient.onLeaveChannel(0);
			}
			if (state == ClientState.Joined)
			{
				this.voiceClient.onJoinChannel(0);
				if (this.voiceClient.GlobalInterestGroup != 0)
				{
					base.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[]
					{
						this.voiceClient.GlobalInterestGroup
					});
				}
			}
		}

		public void Dispose()
		{
			this.voiceClient.Dispose();
		}

		internal const int VOICE_CHANNEL = 0;

		protected VoiceClient voiceClient;

		private PhotonTransportProtocol protocol;
	}
}
