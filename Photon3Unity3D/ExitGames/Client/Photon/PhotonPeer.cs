using System;
using System.Collections.Generic;
using System.Diagnostics;
using ExitGames.Client.Photon.Encryption;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
	public class PhotonPeer
	{
		[Obsolete("See remarks.")]
		public int CommandBufferSize { get; set; }

		[Obsolete("See remarks.")]
		public int LimitOfUnreliableCommands { get; set; }

		[Obsolete("Returns SupportClass.GetTickCount(). Should be replaced by a StopWatch or the per peer PhotonPeer.ClientTime.")]
		public int LocalTimeInMilliSeconds
		{
			get
			{
				return SupportClass.GetTickCount();
			}
		}

		[Obsolete("Use the ITrafficRecorder to capture all traffic instead.")]
		public string CommandLogToString()
		{
			return string.Empty;
		}

		protected internal byte ClientSdkIdShifted
		{
			get
			{
				return (byte)((int)this.ClientSdkId << 1 | 0);
			}
		}

		[Obsolete("The static string Version should be preferred.")]
		public string ClientVersion
		{
			get
			{
				bool flag = string.IsNullOrEmpty(PhotonPeer.clientVersion);
				if (flag)
				{
					PhotonPeer.clientVersion = string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						ExitGames.Client.Photon.Version.clientVersion[0],
						ExitGames.Client.Photon.Version.clientVersion[1],
						ExitGames.Client.Photon.Version.clientVersion[2],
						ExitGames.Client.Photon.Version.clientVersion[3]
					});
				}
				return PhotonPeer.clientVersion;
			}
		}

		public static string Version
		{
			get
			{
				bool flag = string.IsNullOrEmpty(PhotonPeer.clientVersion);
				if (flag)
				{
					PhotonPeer.clientVersion = string.Format("{0}.{1}.{2}.{3}", new object[]
					{
						ExitGames.Client.Photon.Version.clientVersion[0],
						ExitGames.Client.Photon.Version.clientVersion[1],
						ExitGames.Client.Photon.Version.clientVersion[2],
						ExitGames.Client.Photon.Version.clientVersion[3]
					});
				}
				return PhotonPeer.clientVersion;
			}
		}

		public SerializationProtocol SerializationProtocolType { get; set; }

		public Type SocketImplementation { get; internal set; }

		public int SocketErrorCode
		{
			get
			{
				return (this.peerBase != null && this.peerBase.PhotonSocket != null) ? this.peerBase.PhotonSocket.SocketErrorCode : 0;
			}
		}

		public IPhotonPeerListener Listener { get; protected set; }

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<DisconnectMessage> OnDisconnectMessage;

		public bool ReuseEventInstance
		{
			get
			{
				return this.reuseEventInstance;
			}
			set
			{
				object dispatchLockObject = this.DispatchLockObject;
				lock (dispatchLockObject)
				{
					this.reuseEventInstance = value;
					bool flag2 = !value;
					if (flag2)
					{
						this.peerBase.reusableEventData = null;
					}
				}
			}
		}

		public bool UseByteArraySlicePoolForEvents
		{
			get
			{
				return this.useByteArraySlicePoolForEvents;
			}
			set
			{
				this.useByteArraySlicePoolForEvents = value;
			}
		}

		public bool WrapIncomingStructs
		{
			get
			{
				return this.wrapIncomingStructs;
			}
			set
			{
				this.wrapIncomingStructs = value;
			}
		}

		public ByteArraySlicePool ByteArraySlicePool
		{
			get
			{
				return this.peerBase.SerializationProtocol.ByteArraySlicePool;
			}
		}

		[Obsolete("Use SendWindowSize instead.")]
		public int SequenceDeltaLimitSends
		{
			get
			{
				return this.SendWindowSize;
			}
			set
			{
				this.SendWindowSize = value;
			}
		}

		public long BytesIn
		{
			get
			{
				return this.peerBase.BytesIn;
			}
		}

		public long BytesOut
		{
			get
			{
				return this.peerBase.BytesOut;
			}
		}

		public int ByteCountCurrentDispatch
		{
			get
			{
				return this.peerBase.ByteCountCurrentDispatch;
			}
		}

		public string CommandInfoCurrentDispatch
		{
			get
			{
				return (this.peerBase.CommandInCurrentDispatch != null) ? this.peerBase.CommandInCurrentDispatch.ToString() : string.Empty;
			}
		}

		public int ByteCountLastOperation
		{
			get
			{
				return this.peerBase.ByteCountLastOperation;
			}
		}

		public bool EnableServerTracing { get; set; }

		public byte QuickResendAttempts
		{
			get
			{
				return this.quickResendAttempts;
			}
			set
			{
				this.quickResendAttempts = value;
				bool flag = this.quickResendAttempts > 4;
				if (flag)
				{
					this.quickResendAttempts = 4;
				}
			}
		}

		public PeerStateValue PeerState
		{
			get
			{
				bool flag = this.peerBase.peerConnectionState == ConnectionStateValue.Connected && !this.peerBase.ApplicationIsInitialized;
				PeerStateValue result;
				if (flag)
				{
					result = PeerStateValue.InitializingApplication;
				}
				else
				{
					result = (PeerStateValue)this.peerBase.peerConnectionState;
				}
				return result;
			}
		}

		public string PeerID
		{
			get
			{
				return this.peerBase.PeerID;
			}
		}

		public int QueuedIncomingCommands
		{
			get
			{
				return this.peerBase.QueuedIncomingCommandsCount;
			}
		}

		public int QueuedOutgoingCommands
		{
			get
			{
				return this.peerBase.QueuedOutgoingCommandsCount;
			}
		}

		public static void MessageBufferPoolTrim(int countOfBuffers)
		{
			Queue<StreamBuffer> messageBufferPool = PeerBase.MessageBufferPool;
			lock (messageBufferPool)
			{
				bool flag2 = countOfBuffers <= 0;
				if (flag2)
				{
					PeerBase.MessageBufferPool.Clear();
				}
				else
				{
					bool flag3 = countOfBuffers >= PeerBase.MessageBufferPool.Count;
					if (!flag3)
					{
						while (PeerBase.MessageBufferPool.Count > countOfBuffers)
						{
							PeerBase.MessageBufferPool.Dequeue();
						}
						PeerBase.MessageBufferPool.TrimExcess();
					}
				}
			}
		}

		public static int MessageBufferPoolSize()
		{
			return PeerBase.MessageBufferPool.Count;
		}

		public bool CrcEnabled
		{
			get
			{
				return this.crcEnabled;
			}
			set
			{
				bool flag = this.crcEnabled == value;
				if (!flag)
				{
					bool flag2 = this.peerBase.peerConnectionState > ConnectionStateValue.Disconnected;
					if (flag2)
					{
						throw new Exception("CrcEnabled can only be set while disconnected.");
					}
					this.crcEnabled = value;
				}
			}
		}

		public int PacketLossByCrc
		{
			get
			{
				return this.peerBase.packetLossByCrc;
			}
		}

		public int PacketLossByChallenge
		{
			get
			{
				return this.peerBase.packetLossByChallenge;
			}
		}

		public int SentReliableCommandsCount
		{
			get
			{
				return this.peerBase.SentReliableCommandsCount;
			}
		}

		public int ResentReliableCommands
		{
			get
			{
				return (this.UsedProtocol != ConnectionProtocol.Udp) ? 0 : ((EnetPeer)this.peerBase).reliableCommandsRepeated;
			}
		}

		public int DisconnectTimeout
		{
			get
			{
				return this.disconnectTimeout;
			}
			set
			{
				bool flag = value < 0;
				if (flag)
				{
					this.disconnectTimeout = 10000;
				}
				this.disconnectTimeout = value;
			}
		}

		public int ServerTimeInMilliSeconds
		{
			get
			{
				return this.peerBase.serverTimeOffsetIsAvailable ? (this.peerBase.serverTimeOffset + this.ConnectionTime) : 0;
			}
		}

		[Obsolete("The PhotonPeer will no longer use this delegate. It uses a Stopwatch in all cases. You can access PhotonPeer.ConnectionTime.")]
		public SupportClass.IntegerMillisecondsDelegate LocalMsTimestampDelegate
		{
			set
			{
				bool flag = this.PeerState > PeerStateValue.Disconnected;
				if (flag)
				{
					throw new Exception("LocalMsTimestampDelegate only settable while disconnected. State: " + this.PeerState.ToString());
				}
				SupportClass.IntegerMilliseconds = value;
			}
		}

		public int ConnectionTime
		{
			get
			{
				return this.peerBase.timeInt;
			}
		}

		public int LastSendAckTime
		{
			get
			{
				return this.peerBase.timeLastSendAck;
			}
		}

		public int LastSendOutgoingTime
		{
			get
			{
				return this.peerBase.timeLastSendOutgoing;
			}
		}

		public int LongestSentCall
		{
			get
			{
				return this.peerBase.longestSentCall;
			}
			set
			{
				this.peerBase.longestSentCall = value;
			}
		}

		public int RoundTripTime
		{
			get
			{
				return this.peerBase.roundTripTime;
			}
		}

		public int RoundTripTimeVariance
		{
			get
			{
				return this.peerBase.roundTripTimeVariance;
			}
		}

		public int LastRoundTripTime
		{
			get
			{
				return this.peerBase.lastRoundTripTime;
			}
		}

		public int TimestampOfLastSocketReceive
		{
			get
			{
				return this.peerBase.timestampOfLastReceive;
			}
		}

		public string ServerAddress
		{
			get
			{
				return this.peerBase.ServerAddress;
			}
		}

		public string ServerIpAddress
		{
			get
			{
				return IPhotonSocket.ServerIpAddress;
			}
		}

		public ConnectionProtocol UsedProtocol
		{
			get
			{
				return this.peerBase.usedTransportProtocol;
			}
		}

		public ConnectionProtocol TransportProtocol { get; set; }

		public virtual bool IsSimulationEnabled
		{
			get
			{
				return this.NetworkSimulationSettings.IsSimulationEnabled;
			}
			set
			{
				bool flag = value == this.NetworkSimulationSettings.IsSimulationEnabled;
				if (!flag)
				{
					object sendOutgoingLockObject = this.SendOutgoingLockObject;
					lock (sendOutgoingLockObject)
					{
						this.NetworkSimulationSettings.IsSimulationEnabled = value;
					}
				}
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return this.peerBase.NetworkSimulationSettings;
			}
		}

		public int MaximumTransferUnit
		{
			get
			{
				return this.mtu;
			}
			set
			{
				bool flag = this.PeerState > PeerStateValue.Disconnected;
				if (flag)
				{
					throw new Exception("MaximumTransferUnit is only settable while disconnected. State: " + this.PeerState.ToString());
				}
				bool flag2 = value < 576;
				if (flag2)
				{
					value = 576;
				}
				this.mtu = value;
			}
		}

		public bool IsEncryptionAvailable
		{
			get
			{
				return this.peerBase.isEncryptionAvailable;
			}
		}

		[Obsolete("Internally not used anymore. Call SendAcksOnly() instead.")]
		public bool IsSendingOnlyAcks { get; set; }

		public TrafficStats TrafficStatsIncoming { get; internal set; }

		public TrafficStats TrafficStatsOutgoing { get; internal set; }

		public TrafficStatsGameLevel TrafficStatsGameLevel { get; internal set; }

		public long TrafficStatsElapsedMs
		{
			get
			{
				return (this.trafficStatsStopwatch != null) ? this.trafficStatsStopwatch.ElapsedMilliseconds : 0L;
			}
		}

		public bool TrafficStatsEnabled
		{
			get
			{
				return this.trafficStatsEnabled;
			}
			set
			{
				bool flag = this.trafficStatsEnabled == value;
				if (!flag)
				{
					this.trafficStatsEnabled = value;
					bool flag2 = this.trafficStatsEnabled;
					if (flag2)
					{
						bool flag3 = this.trafficStatsStopwatch == null;
						if (flag3)
						{
							this.InitializeTrafficStats();
						}
						this.trafficStatsStopwatch.Start();
					}
					else
					{
						bool flag4 = this.trafficStatsStopwatch != null;
						if (flag4)
						{
							this.trafficStatsStopwatch.Stop();
						}
					}
				}
			}
		}

		public void TrafficStatsReset()
		{
			this.TrafficStatsEnabled = false;
			this.InitializeTrafficStats();
			this.TrafficStatsEnabled = true;
		}

		internal void InitializeTrafficStats()
		{
			bool flag = this.trafficStatsStopwatch == null;
			if (flag)
			{
				this.trafficStatsStopwatch = new Stopwatch();
			}
			else
			{
				this.trafficStatsStopwatch.Reset();
			}
			this.TrafficStatsIncoming = new TrafficStats(this.peerBase.TrafficPackageHeaderSize);
			this.TrafficStatsOutgoing = new TrafficStats(this.peerBase.TrafficPackageHeaderSize);
			this.TrafficStatsGameLevel = new TrafficStatsGameLevel(this.trafficStatsStopwatch);
			bool flag2 = this.trafficStatsEnabled;
			if (flag2)
			{
				this.trafficStatsStopwatch.Start();
			}
		}

		public string VitalStatsToString(bool all)
		{
			string text = "";
			bool flag = this.TrafficStatsGameLevel != null;
			if (flag)
			{
				text = this.TrafficStatsGameLevel.ToStringVitalStats();
			}
			bool flag2 = !all;
			string result;
			if (flag2)
			{
				result = string.Format("Rtt(variance): {0}({1}). Since receive: {2}ms. Longest send: {5}ms. Stats elapsed: {4}sec.\n{3}", new object[]
				{
					this.RoundTripTime,
					this.RoundTripTimeVariance,
					this.ConnectionTime - this.TimestampOfLastSocketReceive,
					text,
					this.TrafficStatsElapsedMs / 1000L,
					this.LongestSentCall
				});
			}
			else
			{
				result = string.Format("Rtt(variance): {0}({1}). Since receive: {2}ms. Longest send: {7}ms. Stats elapsed: {6}sec.\n{3}\n{4}\n{5}", new object[]
				{
					this.RoundTripTime,
					this.RoundTripTimeVariance,
					this.ConnectionTime - this.TimestampOfLastSocketReceive,
					text,
					this.TrafficStatsIncoming,
					this.TrafficStatsOutgoing,
					this.TrafficStatsElapsedMs / 1000L,
					this.LongestSentCall
				});
			}
			return result;
		}

		public Type PayloadEncryptorType
		{
			get
			{
				return this.payloadEncryptorType;
			}
			set
			{
				bool flag = value == null || typeof(ICryptoProvider).IsAssignableFrom(value);
				bool flag2 = flag;
				if (flag2)
				{
					this.payloadEncryptorType = value;
				}
				else
				{
					this.Listener.DebugReturn(DebugLevel.WARNING, "Failed to set the EncryptorType. Type must implement IPhotonEncryptor.");
				}
			}
		}

		public Type EncryptorType
		{
			get
			{
				return this.encryptorType;
			}
			set
			{
				bool flag = value == null || typeof(IPhotonEncryptor).IsAssignableFrom(value);
				bool flag2 = flag;
				if (flag2)
				{
					this.encryptorType = value;
				}
				else
				{
					this.Listener.DebugReturn(DebugLevel.WARNING, "Failed to set the EncryptorType. Type must implement IPhotonEncryptor.");
				}
			}
		}

		public int CountDiscarded { get; set; }

		public int DeltaUnreliableNumber { get; set; }

		public PhotonPeer(ConnectionProtocol protocolType)
		{
			this.TransportProtocol = protocolType;
			this.SocketImplementationConfig = new Dictionary<ConnectionProtocol, Type>(5);
			this.SocketImplementationConfig[ConnectionProtocol.Udp] = typeof(SocketUdp);
			this.SocketImplementationConfig[ConnectionProtocol.Tcp] = typeof(SocketTcp);
			this.SocketImplementationConfig[ConnectionProtocol.WebSocket] = typeof(PhotonClientWebSocket);
			this.SocketImplementationConfig[ConnectionProtocol.WebSocketSecure] = typeof(PhotonClientWebSocket);
			this.CreatePeerBase();
		}

		public PhotonPeer(IPhotonPeerListener listener, ConnectionProtocol protocolType) : this(protocolType)
		{
			this.Listener = listener;
		}

		public virtual bool Connect(string serverAddress, string appId, object photonToken = null, object customInitData = null)
		{
			return this.Connect(serverAddress, null, appId, photonToken, customInitData);
		}

		public virtual bool Connect(string serverAddress, string proxyServerAddress, string appId, object photonToken, object customInitData = null)
		{
			object dispatchLockObject = this.DispatchLockObject;
			bool result;
			lock (dispatchLockObject)
			{
				object sendOutgoingLockObject = this.SendOutgoingLockObject;
				lock (sendOutgoingLockObject)
				{
					bool flag3 = this.peerBase != null && this.peerBase.peerConnectionState > ConnectionStateValue.Disconnected;
					if (flag3)
					{
						this.Listener.DebugReturn(DebugLevel.WARNING, "Connect() can't be called if peer is not Disconnected. Not connecting.");
						result = false;
					}
					else
					{
						bool flag4 = photonToken == null;
						if (flag4)
						{
							this.Encryptor = null;
							this.RandomizedSequenceNumbers = null;
							this.RandomizeSequenceNumbers = false;
							this.GcmDatagramEncryption = false;
						}
						this.CreatePeerBase();
						this.peerBase.Reset();
						this.PingUsedAsInit = false;
						this.peerBase.ServerAddress = serverAddress;
						this.peerBase.ProxyServerAddress = proxyServerAddress;
						this.peerBase.AppId = appId;
						this.peerBase.PhotonToken = photonToken;
						this.peerBase.CustomInitData = customInitData;
						Type socketImplementation = null;
						bool flag5 = !this.SocketImplementationConfig.TryGetValue(this.TransportProtocol, out socketImplementation);
						if (flag5)
						{
							this.peerBase.EnqueueDebugReturn(DebugLevel.ERROR, "Connect failed. SocketImplementationConfig is not set for protocol " + this.TransportProtocol.ToString() + ": " + SupportClass.DictionaryToString(this.SocketImplementationConfig, false));
							result = false;
						}
						else
						{
							this.SocketImplementation = socketImplementation;
							try
							{
								this.peerBase.PhotonSocket = (IPhotonSocket)Activator.CreateInstance(this.SocketImplementation, new object[]
								{
									this.peerBase
								});
							}
							catch (Exception ex)
							{
								IPhotonPeerListener listener = this.Listener;
								DebugLevel level = DebugLevel.ERROR;
								string[] array = new string[6];
								array[0] = "Connect() failed to create a IPhotonSocket instance for ";
								array[1] = this.TransportProtocol.ToString();
								array[2] = ". SocketImplementationConfig: ";
								array[3] = SupportClass.DictionaryToString(this.SocketImplementationConfig, false);
								array[4] = " Exception: ";
								int num = 5;
								Exception ex2 = ex;
								array[num] = ((ex2 != null) ? ex2.ToString() : null);
								listener.DebugReturn(level, string.Concat(array));
								return false;
							}
							result = this.peerBase.Connect(serverAddress, proxyServerAddress, appId, photonToken);
						}
					}
				}
			}
			return result;
		}

		private void CreatePeerBase()
		{
			ConnectionProtocol transportProtocol = this.TransportProtocol;
			ConnectionProtocol connectionProtocol = transportProtocol;
			if (connectionProtocol != ConnectionProtocol.Tcp && connectionProtocol - ConnectionProtocol.WebSocket > 1)
			{
				bool flag = !(this.peerBase is EnetPeer);
				if (flag)
				{
					this.peerBase = new EnetPeer();
				}
			}
			else
			{
				TPeer tpeer = this.peerBase as TPeer;
				bool flag2 = tpeer == null;
				if (flag2)
				{
					tpeer = new TPeer();
					this.peerBase = tpeer;
				}
				tpeer.DoFraming = (this.TransportProtocol == ConnectionProtocol.Tcp);
			}
			this.peerBase.photonPeer = this;
			this.peerBase.usedTransportProtocol = this.TransportProtocol;
		}

		public virtual void Disconnect()
		{
			object dispatchLockObject = this.DispatchLockObject;
			lock (dispatchLockObject)
			{
				object sendOutgoingLockObject = this.SendOutgoingLockObject;
				lock (sendOutgoingLockObject)
				{
					this.peerBase.Disconnect();
				}
			}
		}

		internal void OnDisconnectMessageCall(DisconnectMessage dm)
		{
			bool flag = this.OnDisconnectMessage != null;
			if (flag)
			{
				this.OnDisconnectMessage(dm);
			}
		}

		public virtual void StopThread()
		{
			object dispatchLockObject = this.DispatchLockObject;
			lock (dispatchLockObject)
			{
				object sendOutgoingLockObject = this.SendOutgoingLockObject;
				lock (sendOutgoingLockObject)
				{
					this.peerBase.StopConnection();
				}
			}
		}

		public virtual void FetchServerTimestamp()
		{
			this.peerBase.FetchServerTimestamp();
		}

		public bool EstablishEncryption()
		{
			bool asyncKeyExchange = PhotonPeer.AsyncKeyExchange;
			bool result;
			if (asyncKeyExchange)
			{
				SupportClass.StartBackgroundCalls(delegate
				{
					this.peerBase.ExchangeKeysForEncryption(this.SendOutgoingLockObject);
					return false;
				}, 100, "");
				result = true;
			}
			else
			{
				result = this.peerBase.ExchangeKeysForEncryption(this.SendOutgoingLockObject);
			}
			return result;
		}

		public bool InitDatagramEncryption(byte[] encryptionSecret, byte[] hmacSecret, bool randomizedSequenceNumbers = false, bool chainingModeGCM = false)
		{
			bool flag = this.EncryptorType != null;
			if (flag)
			{
				try
				{
					this.Encryptor = (IPhotonEncryptor)Activator.CreateInstance(this.EncryptorType);
					bool flag2 = this.Encryptor == null;
					if (flag2)
					{
						this.Listener.DebugReturn(DebugLevel.WARNING, "Datagram encryptor creation by type failed, Activator.CreateInstance() returned null");
					}
				}
				catch (Exception ex)
				{
					IPhotonPeerListener listener = this.Listener;
					DebugLevel level = DebugLevel.WARNING;
					string str = "Datagram encryptor creation by type failed: ";
					Exception ex2 = ex;
					listener.DebugReturn(level, str + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
			bool flag3 = this.Encryptor == null;
			if (flag3)
			{
				this.Encryptor = new EncryptorNet();
			}
			bool flag4 = this.Encryptor == null;
			if (flag4)
			{
				throw new NullReferenceException("Can not init datagram encryption. No suitable encryptor found or provided.");
			}
			IPhotonPeerListener listener2 = this.Listener;
			DebugLevel level2 = DebugLevel.INFO;
			string str2 = "Datagram encryptor of type ";
			Type type = this.Encryptor.GetType();
			listener2.DebugReturn(level2, str2 + ((type != null) ? type.ToString() : null) + " created. Api version: " + 2.ToString());
			this.Listener.DebugReturn(DebugLevel.INFO, "Datagram encryptor initialization: GCM = " + chainingModeGCM.ToString() + ", random seq num = " + randomizedSequenceNumbers.ToString());
			this.Encryptor.Init(encryptionSecret, hmacSecret, null, chainingModeGCM, this.mtu);
			bool flag5 = randomizedSequenceNumbers;
			if (flag5)
			{
				this.RandomizedSequenceNumbers = encryptionSecret;
				this.RandomizeSequenceNumbers = true;
				this.GcmDatagramEncryption = chainingModeGCM;
			}
			return true;
		}

		public void InitPayloadEncryption(byte[] secret)
		{
			this.PayloadEncryptionSecret = secret;
		}

		public virtual void Service()
		{
			while (this.DispatchIncomingCommands())
			{
			}
			while (this.SendOutgoingCommands())
			{
			}
		}

		public virtual bool SendOutgoingCommands()
		{
			bool flag = this.TrafficStatsEnabled;
			if (flag)
			{
				this.TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			object sendOutgoingLockObject = this.SendOutgoingLockObject;
			bool result;
			lock (sendOutgoingLockObject)
			{
				result = this.peerBase.SendOutgoingCommands();
			}
			return result;
		}

		public virtual bool SendAcksOnly()
		{
			bool flag = this.TrafficStatsEnabled;
			if (flag)
			{
				this.TrafficStatsGameLevel.SendOutgoingCommandsCalled();
			}
			object sendOutgoingLockObject = this.SendOutgoingLockObject;
			bool result;
			lock (sendOutgoingLockObject)
			{
				result = this.peerBase.SendAcksOnly();
			}
			return result;
		}

		public virtual bool DispatchIncomingCommands()
		{
			bool flag = this.TrafficStatsEnabled;
			if (flag)
			{
				this.TrafficStatsGameLevel.DispatchIncomingCommandsCalled();
			}
			object dispatchLockObject = this.DispatchLockObject;
			bool result;
			lock (dispatchLockObject)
			{
				this.peerBase.ByteCountCurrentDispatch = 0;
				result = this.peerBase.DispatchIncomingCommands();
			}
			return result;
		}

		public virtual bool SendOperation(byte operationCode, Dictionary<byte, object> operationParameters, SendOptions sendOptions)
		{
			bool flag = sendOptions.Encrypt && !this.IsEncryptionAvailable && this.peerBase.usedTransportProtocol != ConnectionProtocol.WebSocketSecure;
			if (flag)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			bool flag2 = this.peerBase.peerConnectionState != ConnectionStateValue.Connected;
			bool result;
			if (flag2)
			{
				bool flag3 = this.DebugOut >= DebugLevel.ERROR;
				if (flag3)
				{
					this.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + operationCode.ToString() + " Not connected. PeerState: " + this.peerBase.peerConnectionState.ToString());
				}
				this.Listener.OnStatusChanged(StatusCode.SendError);
				result = false;
			}
			else
			{
				bool flag4 = sendOptions.Channel >= this.ChannelCount;
				if (flag4)
				{
					bool flag5 = this.DebugOut >= DebugLevel.ERROR;
					if (flag5)
					{
						this.Listener.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
						{
							"Cannot send op: Selected channel (",
							sendOptions.Channel.ToString(),
							")>= channelCount (",
							this.ChannelCount.ToString(),
							")."
						}));
					}
					this.Listener.OnStatusChanged(StatusCode.SendError);
					result = false;
				}
				else
				{
					object enqueueLock = this.EnqueueLock;
					lock (enqueueLock)
					{
						StreamBuffer opBytes = this.peerBase.SerializeOperationToMessage(operationCode, operationParameters, EgMessageType.Operation, sendOptions.Encrypt);
						result = this.peerBase.EnqueuePhotonMessage(opBytes, sendOptions);
					}
				}
			}
			return result;
		}

		public virtual bool SendOperation(byte operationCode, ParameterDictionary operationParameters, SendOptions sendOptions)
		{
			bool flag = sendOptions.Encrypt && !this.IsEncryptionAvailable && this.peerBase.usedTransportProtocol != ConnectionProtocol.WebSocketSecure;
			if (flag)
			{
				throw new ArgumentException("Can't use encryption yet. Exchange keys first.");
			}
			bool flag2 = this.peerBase.peerConnectionState != ConnectionStateValue.Connected;
			bool result;
			if (flag2)
			{
				bool flag3 = this.DebugOut >= DebugLevel.ERROR;
				if (flag3)
				{
					this.Listener.DebugReturn(DebugLevel.ERROR, "Cannot send op: " + operationCode.ToString() + " Not connected. PeerState: " + this.peerBase.peerConnectionState.ToString());
				}
				this.Listener.OnStatusChanged(StatusCode.SendError);
				result = false;
			}
			else
			{
				bool flag4 = sendOptions.Channel >= this.ChannelCount;
				if (flag4)
				{
					bool flag5 = this.DebugOut >= DebugLevel.ERROR;
					if (flag5)
					{
						this.Listener.DebugReturn(DebugLevel.ERROR, string.Concat(new string[]
						{
							"Cannot send op: Selected channel (",
							sendOptions.Channel.ToString(),
							")>= channelCount (",
							this.ChannelCount.ToString(),
							")."
						}));
					}
					this.Listener.OnStatusChanged(StatusCode.SendError);
					result = false;
				}
				else
				{
					object enqueueLock = this.EnqueueLock;
					lock (enqueueLock)
					{
						StreamBuffer opBytes = this.peerBase.SerializeOperationToMessage(operationCode, operationParameters, EgMessageType.Operation, sendOptions.Encrypt);
						result = this.peerBase.EnqueuePhotonMessage(opBytes, sendOptions);
					}
				}
			}
			return result;
		}

		public static bool RegisterType(Type customType, byte code, SerializeMethod serializeMethod, DeserializeMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}

		public static bool RegisterType(Type customType, byte code, SerializeStreamMethod serializeMethod, DeserializeStreamMethod constructor)
		{
			return Protocol.TryRegisterType(customType, code, serializeMethod, constructor);
		}

		[Obsolete("Check QueuedOutgoingCommands and QueuedIncomingCommands on demand instead.")]
		public int WarningSize;

		[Obsolete("Where dynamic linking is available, this library will attempt to load it and fallback to a managed implementation. This value is always true.")]
		public const bool NativeDatagramEncrypt = true;

		[Obsolete("Use the ITrafficRecorder to capture all traffic instead.")]
		public int CommandLogSize;

		public const bool NoSocket = false;

		public const bool DebugBuild = true;

		public const int NativeEncryptorApiVersion = 2;

		public TargetFrameworks TargetFramework = TargetFrameworks.NetStandard20;

		public static bool NoNativeCallbacks;

		public bool RemoveAppIdFromWebSocketPath;

		public byte ClientSdkId = 15;

		private static string clientVersion;

		[Obsolete("A Native Socket implementation is no longer part of this DLL but delivered in a separate add-on. This value always returns false.")]
		public static readonly bool NativeSocketLibAvailable = false;

		[Obsolete("Native Payload Encryption is no longer part of this DLL but delivered in a separate add-on. This value always returns false.")]
		public static readonly bool NativePayloadEncryptionLibAvailable = false;

		[Obsolete("Native Datagram Encryption is no longer part of this DLL but delivered in a separate add-on. This value always returns false.")]
		public static readonly bool NativeDatagramEncryptionLibAvailable = false;

		internal bool UseInitV3;

		public Dictionary<ConnectionProtocol, Type> SocketImplementationConfig;

		public DebugLevel DebugOut = DebugLevel.ERROR;

		private bool reuseEventInstance;

		private bool useByteArraySlicePoolForEvents = false;

		private bool wrapIncomingStructs = false;

		public bool SendInCreationOrder = true;

		public int SendWindowSize = 50;

		public ITrafficRecorder TrafficRecorder;

		private byte quickResendAttempts;

		public byte ChannelCount = 2;

		public bool EnableEncryptedFlag = false;

		private bool crcEnabled;

		public int SentCountAllowance = 7;

		public int InitialResendTimeMax = 400;

		public int TimePingInterval = 1000;

		public bool PingUsedAsInit = false;

		private int disconnectTimeout = 10000;

		public static int OutgoingStreamBufferSize = 1200;

		private int mtu = 1200;

		public static bool AsyncKeyExchange = false;

		internal bool RandomizeSequenceNumbers;

		internal byte[] RandomizedSequenceNumbers;

		internal bool GcmDatagramEncryption;

		private Stopwatch trafficStatsStopwatch;

		private bool trafficStatsEnabled = false;

		internal PeerBase peerBase;

		private readonly object SendOutgoingLockObject = new object();

		private readonly object DispatchLockObject = new object();

		private readonly object EnqueueLock = new object();

		private Type payloadEncryptorType;

		protected internal byte[] PayloadEncryptionSecret;

		private Type encryptorType;

		protected internal IPhotonEncryptor Encryptor;
	}
}
