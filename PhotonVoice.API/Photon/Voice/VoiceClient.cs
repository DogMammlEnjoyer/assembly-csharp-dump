using System;
using System.Collections.Generic;
using System.Linq;

namespace Photon.Voice
{
	public class VoiceClient : IDisposable
	{
		public int FramesLost { get; internal set; }

		public int FramesReceived { get; private set; }

		public int FramesSent
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
				{
					num += keyValuePair.Value.FramesSent;
				}
				return num;
			}
		}

		public int FramesSentBytes
		{
			get
			{
				int num = 0;
				foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
				{
					num += keyValuePair.Value.FramesSentBytes;
				}
				return num;
			}
		}

		public int RoundTripTime { get; private set; }

		public int RoundTripTimeVariance { get; private set; }

		public bool SuppressInfoDuplicateWarning { get; set; }

		public VoiceClient.RemoteVoiceInfoDelegate OnRemoteVoiceInfoAction { get; set; }

		public int DebugLostPercent { get; set; }

		public IEnumerable<LocalVoice> LocalVoices
		{
			get
			{
				LocalVoice[] array = new LocalVoice[this.localVoices.Count];
				this.localVoices.Values.CopyTo(array, 0);
				return array;
			}
		}

		public IEnumerable<LocalVoice> LocalVoicesInChannel(int channelId)
		{
			List<LocalVoice> list;
			if (this.localVoicesPerChannel.TryGetValue(channelId, out list))
			{
				LocalVoice[] array = new LocalVoice[list.Count];
				list.CopyTo(array, 0);
				return array;
			}
			return new LocalVoice[0];
		}

		public IEnumerable<RemoteVoiceInfo> RemoteVoiceInfos
		{
			get
			{
				foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> playerVoices in this.remoteVoices)
				{
					foreach (KeyValuePair<byte, RemoteVoice> keyValuePair in playerVoices.Value)
					{
						yield return new RemoteVoiceInfo(keyValuePair.Value.channelId, playerVoices.Key, keyValuePair.Key, keyValuePair.Value.Info);
					}
					Dictionary<byte, RemoteVoice>.Enumerator enumerator2 = default(Dictionary<byte, RemoteVoice>.Enumerator);
					playerVoices = default(KeyValuePair<int, Dictionary<byte, RemoteVoice>>);
				}
				Dictionary<int, Dictionary<byte, RemoteVoice>>.Enumerator enumerator = default(Dictionary<int, Dictionary<byte, RemoteVoice>>.Enumerator);
				yield break;
				yield break;
			}
		}

		public void LogSpacingProfiles()
		{
			foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
			{
				keyValuePair.Value.SendSpacingProfileStart();
				this.logger.LogInfo(keyValuePair.Value.LogPrefix + " ev. prof.: " + keyValuePair.Value.SendSpacingProfileDump, Array.Empty<object>());
			}
			foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> keyValuePair2 in this.remoteVoices)
			{
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair3 in keyValuePair2.Value)
				{
					keyValuePair3.Value.ReceiveSpacingProfileStart();
					this.logger.LogInfo(keyValuePair3.Value.LogPrefix + " ev. prof.: " + keyValuePair3.Value.ReceiveSpacingProfileDump, Array.Empty<object>());
				}
			}
		}

		public void LogStats()
		{
			int statDisposerCreated = FrameBuffer.statDisposerCreated;
			int statDisposerDisposed = FrameBuffer.statDisposerDisposed;
			int statPinned = FrameBuffer.statPinned;
			int statUnpinned = FrameBuffer.statUnpinned;
			this.logger.LogInfo(string.Concat(new string[]
			{
				"[PV] FrameBuffer stats Disposer: ",
				statDisposerCreated.ToString(),
				" - ",
				statDisposerDisposed.ToString(),
				" = ",
				(statDisposerCreated - statDisposerDisposed).ToString()
			}), Array.Empty<object>());
			this.logger.LogInfo(string.Concat(new string[]
			{
				"[PV] FrameBuffer stats Pinned: ",
				statPinned.ToString(),
				" - ",
				statUnpinned.ToString(),
				" = ",
				(statPinned - statUnpinned).ToString()
			}), Array.Empty<object>());
		}

		public void SetRemoteVoiceDelayFrames(Codec codec, int delayFrames)
		{
			this.remoteVoiceDelayFrames[codec] = delayFrames;
			foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> keyValuePair in this.remoteVoices)
			{
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair2 in keyValuePair.Value)
				{
					if (codec == keyValuePair2.Value.Info.Codec)
					{
						keyValuePair2.Value.DelayFrames = delayFrames;
					}
				}
			}
		}

		public VoiceClient(IVoiceTransport transport, ILogger logger, VoiceClient.CreateOptions opt = default(VoiceClient.CreateOptions))
		{
			this.transport = transport;
			this.logger = logger;
			if (opt.Equals(default(VoiceClient.CreateOptions)))
			{
				opt = VoiceClient.CreateOptions.Default;
			}
			this.voiceIDMin = opt.VoiceIDMin;
			this.voiceIDMax = opt.VoiceIDMax;
			this.voiceIdLast = this.voiceIDMax;
		}

		public void Service()
		{
			foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
			{
				keyValuePair.Value.service();
			}
		}

		private LocalVoice createLocalVoice(int channelId, Func<byte, int, LocalVoice> voiceFactory)
		{
			byte newVoiceId = this.getNewVoiceId();
			if (newVoiceId != 0)
			{
				LocalVoice localVoice = voiceFactory(newVoiceId, channelId);
				if (localVoice != null)
				{
					this.addVoice(newVoiceId, channelId, localVoice);
					this.logger.LogInfo(localVoice.LogPrefix + " added enc: " + localVoice.Info.ToString(), Array.Empty<object>());
					return localVoice;
				}
			}
			return null;
		}

		public LocalVoice CreateLocalVoice(VoiceInfo voiceInfo, int channelId = 0, IEncoder encoder = null)
		{
			return this.createLocalVoice(channelId, (byte vId, int chId) => new LocalVoice(this, encoder, vId, voiceInfo, chId));
		}

		public LocalVoiceFramed<T> CreateLocalVoiceFramed<T>(VoiceInfo voiceInfo, int frameSize, int channelId = 0, IEncoder encoder = null)
		{
			return (LocalVoiceFramed<T>)this.createLocalVoice(channelId, (byte vId, int chId) => new LocalVoiceFramed<T>(this, encoder, vId, voiceInfo, chId, frameSize));
		}

		public LocalVoiceAudio<T> CreateLocalVoiceAudio<T>(VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, IEncoder encoder, int channelId)
		{
			return (LocalVoiceAudio<T>)this.createLocalVoice(channelId, (byte vId, int chId) => LocalVoiceAudio<T>.Create(this, vId, encoder, voiceInfo, audioSourceDesc, chId));
		}

		public LocalVoice CreateLocalVoiceAudioFromSource(VoiceInfo voiceInfo, IAudioDesc source, AudioSampleType sampleType, IEncoder encoder = null, int channelId = 0)
		{
			if (sampleType == AudioSampleType.Source)
			{
				if (source is IAudioPusher<float> || source is IAudioReader<float>)
				{
					sampleType = AudioSampleType.Float;
				}
				else if (source is IAudioPusher<short> || source is IAudioReader<short>)
				{
					sampleType = AudioSampleType.Short;
				}
			}
			if (encoder == null)
			{
				if (sampleType != AudioSampleType.Short)
				{
					if (sampleType == AudioSampleType.Float)
					{
						encoder = Platform.CreateDefaultAudioEncoder<float>(this.logger, voiceInfo);
					}
				}
				else
				{
					encoder = Platform.CreateDefaultAudioEncoder<short>(this.logger, voiceInfo);
				}
			}
			if (source is IAudioPusher<float>)
			{
				LocalVoiceAudio<float> localVoice;
				if (sampleType == AudioSampleType.Short)
				{
					this.logger.LogInfo("[PV] Creating local voice with source samples type conversion from IAudioPusher float to short.", Array.Empty<object>());
					LocalVoiceAudio<short> localVoice = this.CreateLocalVoiceAudio<short>(voiceInfo, source, encoder, channelId);
					FactoryReusableArray<float> bufferFactory = new FactoryReusableArray<float>(0);
					((IAudioPusher<float>)source).SetCallback(delegate(float[] buf)
					{
						short[] array = localVoice.BufferFactory.New(buf.Length);
						AudioUtil.Convert(buf, array, buf.Length);
						localVoice.PushDataAsync(array);
					}, bufferFactory);
					return localVoice;
				}
				localVoice = this.CreateLocalVoiceAudio<float>(voiceInfo, source, encoder, channelId);
				((IAudioPusher<float>)source).SetCallback(delegate(float[] buf)
				{
					localVoice.PushDataAsync(buf);
				}, localVoice.BufferFactory);
				return localVoice;
			}
			else if (source is IAudioPusher<short>)
			{
				LocalVoiceAudio<short> localVoice;
				if (sampleType == AudioSampleType.Float)
				{
					this.logger.LogInfo("[PV] Creating local voice with source samples type conversion from IAudioPusher short to float.", Array.Empty<object>());
					LocalVoiceAudio<float> localVoice = this.CreateLocalVoiceAudio<float>(voiceInfo, source, encoder, channelId);
					FactoryReusableArray<short> bufferFactory2 = new FactoryReusableArray<short>(0);
					((IAudioPusher<short>)source).SetCallback(delegate(short[] buf)
					{
						float[] array = localVoice.BufferFactory.New(buf.Length);
						AudioUtil.Convert(buf, array, buf.Length);
						localVoice.PushDataAsync(array);
					}, bufferFactory2);
					return localVoice;
				}
				localVoice = this.CreateLocalVoiceAudio<short>(voiceInfo, source, encoder, channelId);
				((IAudioPusher<short>)source).SetCallback(delegate(short[] buf)
				{
					localVoice.PushDataAsync(buf);
				}, localVoice.BufferFactory);
				return localVoice;
			}
			else if (source is IAudioReader<float>)
			{
				if (sampleType == AudioSampleType.Short)
				{
					this.logger.LogInfo("[PV] Creating local voice with source samples type conversion from IAudioReader float to short.", Array.Empty<object>());
					LocalVoiceAudio<short> localVoiceAudio = this.CreateLocalVoiceAudio<short>(voiceInfo, source, encoder, channelId);
					localVoiceAudio.LocalUserServiceable = new BufferReaderPushAdapterAsyncPoolFloatToShort(localVoiceAudio, source as IAudioReader<float>);
					return localVoiceAudio;
				}
				LocalVoiceAudio<float> localVoiceAudio2 = this.CreateLocalVoiceAudio<float>(voiceInfo, source, encoder, channelId);
				localVoiceAudio2.LocalUserServiceable = new BufferReaderPushAdapterAsyncPool<float>(localVoiceAudio2, source as IAudioReader<float>);
				return localVoiceAudio2;
			}
			else
			{
				if (!(source is IAudioReader<short>))
				{
					this.logger.LogError("[PV] CreateLocalVoiceAudioFromSource does not support Voice.IAudioDesc of type {0}", new object[]
					{
						source.GetType()
					});
					return LocalVoiceAudioDummy.Dummy;
				}
				if (sampleType == AudioSampleType.Float)
				{
					this.logger.LogInfo("[PV] Creating local voice with source samples type conversion from IAudioReader short to float.", Array.Empty<object>());
					LocalVoiceAudio<float> localVoiceAudio3 = this.CreateLocalVoiceAudio<float>(voiceInfo, source, encoder, channelId);
					localVoiceAudio3.LocalUserServiceable = new BufferReaderPushAdapterAsyncPoolShortToFloat(localVoiceAudio3, source as IAudioReader<short>);
					return localVoiceAudio3;
				}
				LocalVoiceAudio<short> localVoiceAudio4 = this.CreateLocalVoiceAudio<short>(voiceInfo, source, encoder, channelId);
				localVoiceAudio4.LocalUserServiceable = new BufferReaderPushAdapterAsyncPool<short>(localVoiceAudio4, source as IAudioReader<short>);
				return localVoiceAudio4;
			}
		}

		private byte idInc(byte id)
		{
			if (id != this.voiceIDMax)
			{
				return id + 1;
			}
			return this.voiceIDMin;
		}

		private byte getNewVoiceId()
		{
			bool[] array = new bool[256];
			foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
			{
				array[(int)keyValuePair.Value.id] = true;
			}
			for (byte b = this.idInc(this.voiceIdLast); b != this.voiceIdLast; b = this.idInc(b))
			{
				if (!array[(int)b])
				{
					this.voiceIdLast = b;
					return b;
				}
			}
			return 0;
		}

		private void addVoice(byte newId, int channelId, LocalVoice v)
		{
			this.localVoices[newId] = v;
			List<LocalVoice> list;
			if (!this.localVoicesPerChannel.TryGetValue(channelId, out list))
			{
				list = new List<LocalVoice>();
				this.localVoicesPerChannel[channelId] = list;
			}
			list.Add(v);
			if (this.transport.IsChannelJoined(channelId))
			{
				this.sendVoicesInfoAndConfigFrame(new List<LocalVoice>
				{
					v
				}, channelId, 0);
			}
			v.InterestGroup = this.GlobalInterestGroup;
		}

		public void RemoveLocalVoice(LocalVoice voice)
		{
			this.localVoices.Remove(voice.id);
			this.localVoicesPerChannel[voice.channelId].Remove(voice);
			if (this.transport.IsChannelJoined(voice.channelId))
			{
				this.transport.SendVoiceRemove(voice, voice.channelId, 0);
			}
			voice.Dispose();
			this.logger.LogInfo(voice.LogPrefix + " removed", Array.Empty<object>());
		}

		private void sendChannelVoicesInfo(int channelId, int targetPlayerId)
		{
			List<LocalVoice> voiceList;
			if (this.transport.IsChannelJoined(channelId) && this.localVoicesPerChannel.TryGetValue(channelId, out voiceList))
			{
				this.sendVoicesInfoAndConfigFrame(voiceList, channelId, targetPlayerId);
			}
		}

		internal void sendVoicesInfoAndConfigFrame(IEnumerable<LocalVoice> voiceList, int channelId, int targetPlayerId)
		{
			this.transport.SendVoicesInfo(voiceList, channelId, targetPlayerId);
			foreach (LocalVoice localVoice in voiceList)
			{
				localVoice.sendConfigFrame(targetPlayerId);
			}
			if (targetPlayerId == 0)
			{
				IEnumerable<LocalVoice> enumerable = from x in this.localVoices.Values
				where x.DebugEchoMode
				select x;
				if (enumerable.Count<LocalVoice>() > 0)
				{
					this.transport.SendVoicesInfo(enumerable, channelId, -1);
				}
			}
		}

		internal byte GlobalInterestGroup
		{
			get
			{
				return this.globalInterestGroup;
			}
			set
			{
				this.globalInterestGroup = value;
				foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
				{
					keyValuePair.Value.InterestGroup = this.globalInterestGroup;
				}
			}
		}

		private void clearRemoteVoices()
		{
			foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> keyValuePair in this.remoteVoices)
			{
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair2 in keyValuePair.Value)
				{
					keyValuePair2.Value.removeAndDispose();
				}
			}
			this.remoteVoices.Clear();
			this.logger.LogInfo("[PV] Remote voices cleared", Array.Empty<object>());
		}

		private void clearRemoteVoicesInChannel(int channelId)
		{
			foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> keyValuePair in this.remoteVoices)
			{
				List<byte> list = new List<byte>();
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair2 in keyValuePair.Value)
				{
					if (keyValuePair2.Value.channelId == channelId)
					{
						keyValuePair2.Value.removeAndDispose();
						list.Add(keyValuePair2.Key);
					}
				}
				foreach (byte key in list)
				{
					keyValuePair.Value.Remove(key);
				}
			}
			this.logger.LogInfo("[PV] Remote voices for channel " + this.channelStr(channelId) + " cleared", Array.Empty<object>());
		}

		private void clearRemoteVoicesInChannelForPlayer(int channelId, int playerId)
		{
			Dictionary<byte, RemoteVoice> dictionary = null;
			if (this.remoteVoices.TryGetValue(playerId, out dictionary))
			{
				List<byte> list = new List<byte>();
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair in dictionary)
				{
					if (keyValuePair.Value.channelId == channelId)
					{
						keyValuePair.Value.removeAndDispose();
						list.Add(keyValuePair.Key);
					}
				}
				foreach (byte key in list)
				{
					dictionary.Remove(key);
				}
			}
		}

		public void onJoinChannel(int channel)
		{
			this.sendChannelVoicesInfo(channel, 0);
		}

		public void onLeaveChannel(int channel)
		{
			this.clearRemoteVoicesInChannel(channel);
		}

		public void onLeaveAllChannels()
		{
			this.clearRemoteVoices();
		}

		public void onPlayerJoin(int channelId, int playerId)
		{
			this.sendChannelVoicesInfo(channelId, playerId);
		}

		public void onPlayerLeave(int channelId, int playerId)
		{
			this.clearRemoteVoicesInChannelForPlayer(channelId, playerId);
		}

		public void onVoiceInfo(int channelId, int playerId, byte voiceId, byte eventNumber, VoiceInfo info)
		{
			Dictionary<byte, RemoteVoice> dictionary = null;
			if (!this.remoteVoices.TryGetValue(playerId, out dictionary))
			{
				dictionary = new Dictionary<byte, RemoteVoice>();
				this.remoteVoices[playerId] = dictionary;
			}
			if (!dictionary.ContainsKey(voiceId))
			{
				string text = string.Concat(new string[]
				{
					" p#",
					this.playerStr(playerId),
					" v#",
					voiceId.ToString(),
					" ch#",
					this.channelStr(channelId)
				});
				this.logger.LogInfo(string.Concat(new string[]
				{
					"[PV] ",
					text,
					" Info received: ",
					info.ToString(),
					" ev=",
					eventNumber.ToString()
				}), Array.Empty<object>());
				string logPrefix = "[PV] Remote " + info.Codec.ToString() + text;
				RemoteVoiceOptions options = new RemoteVoiceOptions(this.logger, logPrefix, info);
				if (this.OnRemoteVoiceInfoAction != null)
				{
					this.OnRemoteVoiceInfoAction(channelId, playerId, voiceId, info, ref options);
				}
				if (options.Decoder == null)
				{
					return;
				}
				RemoteVoice remoteVoice = new RemoteVoice(this, options, channelId, playerId, voiceId, info, eventNumber);
				dictionary[voiceId] = remoteVoice;
				int delayFrames;
				if (this.remoteVoiceDelayFrames.TryGetValue(info.Codec, out delayFrames))
				{
					remoteVoice.DelayFrames = delayFrames;
					return;
				}
			}
			else if (!this.SuppressInfoDuplicateWarning)
			{
				this.logger.LogWarning(string.Concat(new string[]
				{
					"[PV] Info duplicate for voice #",
					voiceId.ToString(),
					" of player ",
					this.playerStr(playerId),
					" at channel ",
					this.channelStr(channelId)
				}), Array.Empty<object>());
			}
		}

		public void onVoiceRemove(int channelId, int playerId, byte[] voiceIds)
		{
			Dictionary<byte, RemoteVoice> dictionary = null;
			if (this.remoteVoices.TryGetValue(playerId, out dictionary))
			{
				foreach (byte key in voiceIds)
				{
					RemoteVoice remoteVoice;
					if (dictionary.TryGetValue(key, out remoteVoice))
					{
						dictionary.Remove(key);
						this.logger.LogInfo(string.Concat(new string[]
						{
							"[PV] Remote voice #",
							key.ToString(),
							" of player ",
							this.playerStr(playerId),
							" at channel ",
							this.channelStr(channelId),
							" removed"
						}), Array.Empty<object>());
						remoteVoice.removeAndDispose();
					}
					else
					{
						this.logger.LogWarning(string.Concat(new string[]
						{
							"[PV] Remote voice #",
							key.ToString(),
							" of player ",
							this.playerStr(playerId),
							" at channel ",
							this.channelStr(channelId),
							" not found when trying to remove"
						}), Array.Empty<object>());
					}
				}
				return;
			}
			this.logger.LogWarning(string.Concat(new string[]
			{
				"[PV] Remote voice list of player ",
				this.playerStr(playerId),
				" at channel ",
				this.channelStr(channelId),
				" not found when trying to remove voice(s)"
			}), Array.Empty<object>());
		}

		public void onFrame(int channelId, int playerId, byte voiceId, byte evNumber, ref FrameBuffer receivedBytes, bool isLocalPlayer)
		{
			LocalVoice localVoice;
			int num;
			if (isLocalPlayer && this.localVoices.TryGetValue(voiceId, out localVoice) && localVoice.eventTimestamps.TryGetValue(evNumber, out num))
			{
				int num2 = Environment.TickCount - num;
				int num3 = num2 - this.prevRtt;
				this.prevRtt = num2;
				if (num3 < 0)
				{
					num3 = -num3;
				}
				this.RoundTripTimeVariance = (num3 + this.RoundTripTimeVariance * 19) / 20;
				this.RoundTripTime = (num2 + this.RoundTripTime * 19) / 20;
			}
			if (this.DebugLostPercent > 0 && this.rnd.Next(100) < this.DebugLostPercent)
			{
				this.logger.LogWarning("[PV] Debug Lost Sim: 1 packet dropped", Array.Empty<object>());
				return;
			}
			int framesReceived = this.FramesReceived;
			this.FramesReceived = framesReceived + 1;
			Dictionary<byte, RemoteVoice> dictionary = null;
			if (!this.remoteVoices.TryGetValue(playerId, out dictionary))
			{
				this.logger.LogWarning(string.Concat(new string[]
				{
					"[PV] Frame event for voice #",
					voiceId.ToString(),
					" of not inited player ",
					this.playerStr(playerId),
					" at channel ",
					this.channelStr(channelId)
				}), Array.Empty<object>());
				return;
			}
			RemoteVoice remoteVoice = null;
			if (dictionary.TryGetValue(voiceId, out remoteVoice))
			{
				remoteVoice.receiveBytes(ref receivedBytes, evNumber);
				return;
			}
			this.logger.LogWarning(string.Concat(new string[]
			{
				"[PV] Frame event for not inited voice #",
				voiceId.ToString(),
				" of player ",
				this.playerStr(playerId),
				" at channel ",
				this.channelStr(channelId)
			}), Array.Empty<object>());
		}

		internal string channelStr(int channelId)
		{
			string text = this.transport.ChannelIdStr(channelId);
			if (text != null)
			{
				return channelId.ToString() + "(" + text + ")";
			}
			return channelId.ToString();
		}

		internal string playerStr(int playerId)
		{
			string text = this.transport.PlayerIdStr(playerId);
			if (text != null)
			{
				return playerId.ToString() + "(" + text + ")";
			}
			return playerId.ToString();
		}

		public void Dispose()
		{
			foreach (KeyValuePair<byte, LocalVoice> keyValuePair in this.localVoices)
			{
				keyValuePair.Value.Dispose();
			}
			foreach (KeyValuePair<int, Dictionary<byte, RemoteVoice>> keyValuePair2 in this.remoteVoices)
			{
				foreach (KeyValuePair<byte, RemoteVoice> keyValuePair3 in keyValuePair2.Value)
				{
					keyValuePair3.Value.Dispose();
				}
			}
		}

		internal IVoiceTransport transport;

		internal ILogger logger;

		private int prevRtt;

		private Dictionary<Codec, int> remoteVoiceDelayFrames = new Dictionary<Codec, int>();

		private byte voiceIDMin;

		private byte voiceIDMax;

		private byte voiceIdLast;

		private byte globalInterestGroup;

		private Dictionary<byte, LocalVoice> localVoices = new Dictionary<byte, LocalVoice>();

		private Dictionary<int, List<LocalVoice>> localVoicesPerChannel = new Dictionary<int, List<LocalVoice>>();

		private Dictionary<int, Dictionary<byte, RemoteVoice>> remoteVoices = new Dictionary<int, Dictionary<byte, RemoteVoice>>();

		private Random rnd = new Random();

		public delegate void RemoteVoiceInfoDelegate(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options);

		public struct CreateOptions
		{
			public byte VoiceIDMin;

			public byte VoiceIDMax;

			public static VoiceClient.CreateOptions Default = new VoiceClient.CreateOptions
			{
				VoiceIDMin = 1,
				VoiceIDMax = 15
			};
		}
	}
}
