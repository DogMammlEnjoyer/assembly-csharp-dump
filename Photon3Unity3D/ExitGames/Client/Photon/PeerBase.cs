using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ExitGames.Client.Photon.Encryption;
using Photon.SocketServer.Security;

namespace ExitGames.Client.Photon
{
	public abstract class PeerBase
	{
		public string ServerAddress { get; internal set; }

		public string ProxyServerAddress { get; internal set; }

		internal IPhotonPeerListener Listener
		{
			get
			{
				return this.photonPeer.Listener;
			}
		}

		public DebugLevel debugOut
		{
			get
			{
				return this.photonPeer.DebugOut;
			}
		}

		internal int DisconnectTimeout
		{
			get
			{
				return this.photonPeer.DisconnectTimeout;
			}
		}

		internal int timePingInterval
		{
			get
			{
				return this.photonPeer.TimePingInterval;
			}
		}

		internal byte ChannelCount
		{
			get
			{
				return this.photonPeer.ChannelCount;
			}
		}

		internal long BytesOut
		{
			get
			{
				return this.bytesOut;
			}
		}

		internal long BytesIn
		{
			get
			{
				return this.bytesIn;
			}
		}

		internal abstract int QueuedIncomingCommandsCount { get; }

		internal abstract int QueuedOutgoingCommandsCount { get; }

		internal virtual int SentReliableCommandsCount
		{
			get
			{
				return 0;
			}
		}

		public virtual string PeerID
		{
			get
			{
				return ((ushort)this.peerID).ToString();
			}
		}

		internal int timeInt
		{
			get
			{
				return (int)this.watch.ElapsedMilliseconds;
			}
		}

		internal static int outgoingStreamBufferSize
		{
			get
			{
				return PhotonPeer.OutgoingStreamBufferSize;
			}
		}

		internal int mtu
		{
			get
			{
				return this.photonPeer.MaximumTransferUnit;
			}
		}

		protected internal bool IsIpv6
		{
			get
			{
				return this.PhotonSocket != null && this.PhotonSocket.AddressResolvedAsIpv6;
			}
		}

		protected PeerBase()
		{
			this.networkSimulationSettings.peerBase = this;
			PeerBase.peerCount += 1;
		}

		public static StreamBuffer MessageBufferPoolGet()
		{
			Queue<StreamBuffer> messageBufferPool = PeerBase.MessageBufferPool;
			StreamBuffer result;
			lock (messageBufferPool)
			{
				bool flag2 = PeerBase.MessageBufferPool.Count > 0;
				if (flag2)
				{
					result = PeerBase.MessageBufferPool.Dequeue();
				}
				else
				{
					result = new StreamBuffer(75);
				}
			}
			return result;
		}

		public static void MessageBufferPoolPut(StreamBuffer buff)
		{
			buff.Position = 0;
			buff.SetLength(0L);
			Queue<StreamBuffer> messageBufferPool = PeerBase.MessageBufferPool;
			lock (messageBufferPool)
			{
				PeerBase.MessageBufferPool.Enqueue(buff);
			}
		}

		internal virtual void Reset()
		{
			this.SerializationProtocol = SerializationProtocolFactory.Create(this.photonPeer.SerializationProtocolType);
			this.photonPeer.InitializeTrafficStats();
			this.ByteCountLastOperation = 0;
			this.ByteCountCurrentDispatch = 0;
			this.bytesIn = 0L;
			this.bytesOut = 0L;
			this.packetLossByCrc = 0;
			this.packetLossByChallenge = 0;
			this.networkSimulationSettings.LostPackagesIn = 0;
			this.networkSimulationSettings.LostPackagesOut = 0;
			LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
			lock (netSimListOutgoing)
			{
				this.NetSimListOutgoing.Clear();
			}
			LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
			lock (netSimListIncoming)
			{
				this.NetSimListIncoming.Clear();
			}
			Queue<PeerBase.MyAction> actionQueue = this.ActionQueue;
			lock (actionQueue)
			{
				this.ActionQueue.Clear();
			}
			this.peerConnectionState = ConnectionStateValue.Disconnected;
			this.watch.Reset();
			this.watch.Start();
			this.isEncryptionAvailable = false;
			this.ApplicationIsInitialized = false;
			this.CryptoProvider = null;
			this.roundTripTime = 200;
			this.roundTripTimeVariance = 5;
			this.serverTimeOffsetIsAvailable = false;
			this.serverTimeOffset = 0;
		}

		internal abstract bool Connect(string serverAddress, string proxyServerAddress, string appID, object photonToken);

		private string GetHttpKeyValueString(Dictionary<string, string> dic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in dic)
			{
				stringBuilder.Append(keyValuePair.Key).Append("=").Append(keyValuePair.Value).Append("&");
			}
			return stringBuilder.ToString();
		}

		internal byte[] WriteInitRequest()
		{
			bool flag = this.PhotonSocket == null || !this.PhotonSocket.Connected;
			if (flag)
			{
				this.EnqueueDebugReturn(DebugLevel.WARNING, "The peer attempts to prepare an Init-Request but the socket is not connected!?");
			}
			bool useInitV = this.photonPeer.UseInitV3;
			byte[] result;
			if (useInitV)
			{
				result = this.WriteInitV3();
			}
			else
			{
				bool flag2 = this.PhotonToken == null;
				if (flag2)
				{
					byte[] array = new byte[41];
					byte[] clientVersion = Version.clientVersion;
					array[0] = 243;
					array[1] = 0;
					array[2] = this.SerializationProtocol.VersionBytes[0];
					array[3] = this.SerializationProtocol.VersionBytes[1];
					array[4] = this.photonPeer.ClientSdkIdShifted;
					array[5] = ((byte)(clientVersion[0] << 4) | clientVersion[1]);
					array[6] = clientVersion[2];
					array[7] = clientVersion[3];
					array[8] = 0;
					bool flag3 = string.IsNullOrEmpty(this.AppId);
					if (flag3)
					{
						this.AppId = "LoadBalancing";
					}
					for (int i = 0; i < 32; i++)
					{
						array[i + 9] = ((i < this.AppId.Length) ? ((byte)this.AppId[i]) : 0);
					}
					bool isIpv = this.IsIpv6;
					if (isIpv)
					{
						byte[] array2 = array;
						int num = 5;
						array2[num] |= 128;
					}
					else
					{
						byte[] array3 = array;
						int num2 = 5;
						array3[num2] &= 127;
					}
					result = array;
				}
				else
				{
					bool flag4 = this.PhotonToken != null;
					if (flag4)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary["init"] = null;
						dictionary["app"] = this.AppId;
						dictionary["clientversion"] = this.photonPeer.ClientVersion;
						dictionary["protocol"] = this.SerializationProtocol.ProtocolType;
						dictionary["sid"] = this.photonPeer.ClientSdkIdShifted.ToString();
						byte[] array4 = null;
						int num3 = 0;
						bool flag5 = this.PhotonToken != null;
						if (flag5)
						{
							array4 = this.SerializationProtocol.Serialize(this.PhotonToken);
							num3 += array4.Length;
						}
						string text = this.GetHttpKeyValueString(dictionary);
						bool isIpv2 = this.IsIpv6;
						if (isIpv2)
						{
							text += "&IPv6";
						}
						string text2 = string.Format("POST /?{0} HTTP/1.1\r\nHost: {1}\r\nContent-Length: {2}\r\n\r\n", text, this.ServerAddress, num3);
						byte[] array5 = new byte[text2.Length + num3];
						bool flag6 = array4 != null;
						if (flag6)
						{
							Buffer.BlockCopy(array4, 0, array5, text2.Length, array4.Length);
						}
						Buffer.BlockCopy(Encoding.UTF8.GetBytes(text2), 0, array5, 0, text2.Length);
						result = array5;
					}
					else
					{
						result = null;
					}
				}
			}
			return result;
		}

		private byte[] WriteInitV3()
		{
			StreamBuffer streamBuffer = new StreamBuffer(0);
			streamBuffer.WriteByte(245);
			InitV3Flags initV3Flags = InitV3Flags.NoFlags;
			bool isIpv = this.IsIpv6;
			if (isIpv)
			{
				initV3Flags |= InitV3Flags.IPv6Flag;
			}
			IPhotonEncryptor encryptor = this.photonPeer.Encryptor;
			bool flag = encryptor != null;
			if (flag)
			{
				initV3Flags |= InitV3Flags.EncryptionFlag;
			}
			streamBuffer.WriteBytes((byte)(initV3Flags >> 8), (byte)initV3Flags);
			byte b = this.SerializationProtocol.VersionBytes[1];
			byte b2 = b;
			if (b2 != 6)
			{
				if (b2 != 8)
				{
					throw new Exception("Unknown protocol version: " + this.SerializationProtocol.VersionBytes[1].ToString());
				}
				streamBuffer.WriteByte(18);
			}
			else
			{
				streamBuffer.WriteByte(16);
			}
			streamBuffer.Write(Version.clientVersion, 0, 4);
			streamBuffer.WriteByte(this.photonPeer.ClientSdkIdShifted);
			streamBuffer.WriteByte(0);
			bool flag2 = string.IsNullOrEmpty(this.AppId);
			if (flag2)
			{
				this.AppId = "Master";
			}
			byte[] bytes = Encoding.UTF8.GetBytes(this.AppId);
			int num = bytes.Length;
			bool flag3 = num > 255;
			if (flag3)
			{
				throw new Exception("AppId is too long. Limited by 255 symbols.");
			}
			streamBuffer.WriteByte((byte)num);
			streamBuffer.Write(bytes, 0, bytes.Length);
			byte[] array = this.PhotonToken as byte[];
			bool flag4 = array != null;
			if (flag4)
			{
				num = array.Length;
				streamBuffer.WriteBytes((byte)(num >> 8), (byte)num);
				streamBuffer.Write(array, 0, num);
			}
			else
			{
				streamBuffer.WriteBytes(0, 0);
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>();
			bool flag5 = this.CustomInitData != null;
			if (flag5)
			{
				dictionary.Add(0, this.CustomInitData);
			}
			bool flag6 = encryptor != null;
			if (flag6)
			{
				throw new NotImplementedException("InitV3 with encryption is not implemented yet.");
			}
			this.SerializationProtocol.Serialize(streamBuffer, dictionary, true);
			return streamBuffer.ToArray();
		}

		internal string PrepareWebSocketUrl(string serverAddress, string appId, object photonToken)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.Append(serverAddress);
			stringBuilder.AppendFormat("/?libversion={0}&sid={1}", this.photonPeer.ClientVersion, this.photonPeer.ClientSdkIdShifted);
			bool flag = !this.photonPeer.RemoveAppIdFromWebSocketPath;
			if (flag)
			{
				stringBuilder.AppendFormat("&app={0}", appId);
			}
			bool isIpv = this.IsIpv6;
			if (isIpv)
			{
				stringBuilder.Append("&IPv6");
			}
			bool flag2 = photonToken != null;
			if (flag2)
			{
				stringBuilder.Append("&xInit=");
			}
			return stringBuilder.ToString();
		}

		public abstract void OnConnect();

		internal void InitCallback()
		{
			bool flag = this.peerConnectionState == ConnectionStateValue.Connecting;
			if (flag)
			{
				this.peerConnectionState = ConnectionStateValue.Connected;
			}
			this.ApplicationIsInitialized = true;
			this.FetchServerTimestamp();
			this.Listener.OnStatusChanged(StatusCode.Connect);
		}

		internal abstract void Disconnect();

		internal abstract void StopConnection();

		internal abstract void FetchServerTimestamp();

		internal abstract bool IsTransportEncrypted();

		internal abstract bool EnqueuePhotonMessage(StreamBuffer opBytes, SendOptions sendParams);

		internal StreamBuffer SerializeOperationToMessage(byte opCode, Dictionary<byte, object> parameters, EgMessageType messageType, bool encrypt)
		{
			bool flag = encrypt && !this.IsTransportEncrypted();
			StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
			streamBuffer.SetLength(0L);
			bool flag2 = !flag;
			if (flag2)
			{
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
			}
			this.SerializationProtocol.SerializeOperationRequest(streamBuffer, opCode, parameters, false);
			bool flag3 = flag;
			if (flag3)
			{
				byte[] array = this.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
				streamBuffer.SetLength(0L);
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
				streamBuffer.Write(array, 0, array.Length);
			}
			byte[] buffer = streamBuffer.GetBuffer();
			bool flag4 = messageType != EgMessageType.Operation;
			if (flag4)
			{
				buffer[this.messageHeader.Length - 1] = (byte)messageType;
			}
			bool flag5 = flag || (encrypt && this.photonPeer.EnableEncryptedFlag);
			if (flag5)
			{
				buffer[this.messageHeader.Length - 1] = (buffer[this.messageHeader.Length - 1] | 128);
			}
			return streamBuffer;
		}

		internal StreamBuffer SerializeOperationToMessage(byte opCode, ParameterDictionary parameters, EgMessageType messageType, bool encrypt)
		{
			bool flag = encrypt && !this.IsTransportEncrypted();
			StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
			streamBuffer.SetLength(0L);
			bool flag2 = !flag;
			if (flag2)
			{
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
			}
			this.SerializationProtocol.SerializeOperationRequest(streamBuffer, opCode, parameters, false);
			bool flag3 = flag;
			if (flag3)
			{
				byte[] array = this.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
				streamBuffer.SetLength(0L);
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
				streamBuffer.Write(array, 0, array.Length);
			}
			byte[] buffer = streamBuffer.GetBuffer();
			bool flag4 = messageType != EgMessageType.Operation;
			if (flag4)
			{
				buffer[this.messageHeader.Length - 1] = (byte)messageType;
			}
			bool flag5 = flag || (encrypt && this.photonPeer.EnableEncryptedFlag);
			if (flag5)
			{
				buffer[this.messageHeader.Length - 1] = (buffer[this.messageHeader.Length - 1] | 128);
			}
			return streamBuffer;
		}

		internal StreamBuffer SerializeMessageToMessage(object message, bool encrypt)
		{
			bool flag = encrypt && !this.IsTransportEncrypted();
			StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
			streamBuffer.SetLength(0L);
			bool flag2 = !flag;
			if (flag2)
			{
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
			}
			bool flag3 = message is byte[];
			bool flag4 = flag3;
			if (flag4)
			{
				byte[] array = message as byte[];
				streamBuffer.Write(array, 0, array.Length);
			}
			else
			{
				this.SerializationProtocol.SerializeMessage(streamBuffer, message);
			}
			bool flag5 = flag;
			if (flag5)
			{
				byte[] array2 = this.CryptoProvider.Encrypt(streamBuffer.GetBuffer(), 0, streamBuffer.Length);
				streamBuffer.SetLength(0L);
				streamBuffer.Write(this.messageHeader, 0, this.messageHeader.Length);
				streamBuffer.Write(array2, 0, array2.Length);
			}
			byte[] buffer = streamBuffer.GetBuffer();
			buffer[this.messageHeader.Length - 1] = (flag3 ? 9 : 8);
			bool flag6 = flag || (encrypt && this.photonPeer.EnableEncryptedFlag);
			if (flag6)
			{
				buffer[this.messageHeader.Length - 1] = (buffer[this.messageHeader.Length - 1] | 128);
			}
			return streamBuffer;
		}

		internal abstract bool SendOutgoingCommands();

		internal virtual bool SendAcksOnly()
		{
			return false;
		}

		internal abstract void ReceiveIncomingCommands(byte[] inBuff, int dataLength);

		internal abstract bool DispatchIncomingCommands();

		internal virtual bool DeserializeMessageAndCallback(StreamBuffer stream)
		{
			bool flag = stream.Length < 2;
			bool result;
			if (flag)
			{
				bool flag2 = this.debugOut >= DebugLevel.ERROR;
				if (flag2)
				{
					this.Listener.DebugReturn(DebugLevel.ERROR, "Incoming UDP data too short! " + stream.Length.ToString());
				}
				result = false;
			}
			else
			{
				byte b = stream.ReadByte();
				bool flag3 = b != 243 && b != 253;
				if (flag3)
				{
					bool flag4 = this.debugOut >= DebugLevel.ERROR;
					if (flag4)
					{
						this.Listener.DebugReturn(DebugLevel.ALL, "No regular operation UDP message: " + b.ToString());
					}
					result = false;
				}
				else
				{
					byte b2 = stream.ReadByte();
					byte b3 = b2 & 127;
					bool flag5 = (b2 & 128) > 0;
					bool flag6 = b3 != 1;
					if (flag6)
					{
						try
						{
							bool flag7 = flag5;
							if (flag7)
							{
								byte[] buf = this.CryptoProvider.Decrypt(stream.GetBuffer(), 2, stream.Length - 2);
								stream = new StreamBuffer(buf);
							}
							else
							{
								stream.Seek(2L, SeekOrigin.Begin);
							}
						}
						catch (Exception ex)
						{
							bool flag8 = this.debugOut >= DebugLevel.ERROR;
							if (flag8)
							{
								this.Listener.DebugReturn(DebugLevel.ERROR, "msgType: " + b3.ToString() + " exception: " + ex.ToString());
							}
							SupportClass.WriteStackTrace(ex);
							return false;
						}
					}
					int num = 0;
					IProtocol.DeserializationFlags flags = (this.photonPeer.UseByteArraySlicePoolForEvents ? IProtocol.DeserializationFlags.AllowPooledByteArray : IProtocol.DeserializationFlags.None) | (this.photonPeer.WrapIncomingStructs ? IProtocol.DeserializationFlags.WrapIncomingStructs : IProtocol.DeserializationFlags.None);
					switch (b3)
					{
					case 1:
						this.InitCallback();
						goto IL_5AE;
					case 3:
					{
						OperationResponse operationResponse = null;
						try
						{
							operationResponse = this.SerializationProtocol.DeserializeOperationResponse(stream, flags);
						}
						catch (Exception ex2)
						{
							DebugLevel level = DebugLevel.ERROR;
							string str = "Deserialization failed for Operation Response. ";
							Exception ex3 = ex2;
							this.EnqueueDebugReturn(level, str + ((ex3 != null) ? ex3.ToString() : null));
							return false;
						}
						bool trafficStatsEnabled = this.TrafficStatsEnabled;
						if (trafficStatsEnabled)
						{
							this.TrafficStatsGameLevel.CountResult(this.ByteCountCurrentDispatch);
							num = this.timeInt;
						}
						this.Listener.OnOperationResponse(operationResponse);
						bool trafficStatsEnabled2 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled2)
						{
							this.TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, this.timeInt - num);
						}
						goto IL_5AE;
					}
					case 4:
					{
						EventData eventData = null;
						try
						{
							eventData = this.SerializationProtocol.DeserializeEventData(stream, this.reusableEventData, flags);
						}
						catch (Exception ex4)
						{
							DebugLevel level2 = DebugLevel.ERROR;
							string str2 = "Deserialization failed for Event. ";
							Exception ex5 = ex4;
							this.EnqueueDebugReturn(level2, str2 + ((ex5 != null) ? ex5.ToString() : null));
							return false;
						}
						bool trafficStatsEnabled3 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled3)
						{
							this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
							num = this.timeInt;
						}
						this.Listener.OnEvent(eventData);
						bool trafficStatsEnabled4 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled4)
						{
							this.TrafficStatsGameLevel.TimeForEventCallback(eventData.Code, this.timeInt - num);
						}
						bool reuseEventInstance = this.photonPeer.ReuseEventInstance;
						if (reuseEventInstance)
						{
							this.reusableEventData = eventData;
						}
						goto IL_5AE;
					}
					case 5:
						try
						{
							DisconnectMessage dm = this.SerializationProtocol.DeserializeDisconnectMessage(stream);
							this.photonPeer.OnDisconnectMessageCall(dm);
						}
						catch (Exception ex6)
						{
							DebugLevel level3 = DebugLevel.ERROR;
							string str3 = "Deserialization failed for disconnect message. ";
							Exception ex7 = ex6;
							this.EnqueueDebugReturn(level3, str3 + ((ex7 != null) ? ex7.ToString() : null));
							return false;
						}
						goto IL_5AE;
					case 7:
					{
						OperationResponse operationResponse;
						try
						{
							operationResponse = this.SerializationProtocol.DeserializeOperationResponse(stream, IProtocol.DeserializationFlags.None);
						}
						catch (Exception ex8)
						{
							DebugLevel level4 = DebugLevel.ERROR;
							string str4 = "Deserialization failed for internal Operation Response. ";
							Exception ex9 = ex8;
							this.EnqueueDebugReturn(level4, str4 + ((ex9 != null) ? ex9.ToString() : null));
							return false;
						}
						bool trafficStatsEnabled5 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled5)
						{
							this.TrafficStatsGameLevel.CountResult(this.ByteCountCurrentDispatch);
							num = this.timeInt;
						}
						bool flag9 = operationResponse.OperationCode == PhotonCodes.InitEncryption;
						if (flag9)
						{
							this.DeriveSharedKey(operationResponse);
						}
						else
						{
							bool flag10 = operationResponse.OperationCode == PhotonCodes.Ping;
							if (flag10)
							{
								bool flag11 = this.peerConnectionState == ConnectionStateValue.Connecting && (this.usedTransportProtocol == ConnectionProtocol.WebSocket || this.usedTransportProtocol == ConnectionProtocol.WebSocketSecure);
								if (flag11)
								{
									this.photonPeer.PingUsedAsInit = true;
									this.InitCallback();
								}
								TPeer tpeer = this as TPeer;
								bool flag12 = tpeer != null;
								if (flag12)
								{
									tpeer.ReadPingResult(operationResponse);
								}
							}
							else
							{
								this.EnqueueDebugReturn(DebugLevel.ERROR, "Received unknown internal operation. " + operationResponse.ToStringFull());
							}
						}
						bool trafficStatsEnabled6 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled6)
						{
							this.TrafficStatsGameLevel.TimeForResponseCallback(operationResponse.OperationCode, this.timeInt - num);
						}
						goto IL_5AE;
					}
					case 8:
					{
						object obj = this.SerializationProtocol.DeserializeMessage(stream);
						bool trafficStatsEnabled7 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled7)
						{
							this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
							num = this.timeInt;
						}
						bool trafficStatsEnabled8 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled8)
						{
							this.TrafficStatsGameLevel.TimeForMessageCallback(this.timeInt - num);
						}
						goto IL_5AE;
					}
					case 9:
					{
						bool trafficStatsEnabled9 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled9)
						{
							this.TrafficStatsGameLevel.CountEvent(this.ByteCountCurrentDispatch);
							num = this.timeInt;
						}
						byte[] array = stream.ToArrayFromPos();
						bool trafficStatsEnabled10 = this.TrafficStatsEnabled;
						if (trafficStatsEnabled10)
						{
							this.TrafficStatsGameLevel.TimeForRawMessageCallback(this.timeInt - num);
						}
						goto IL_5AE;
					}
					}
					this.EnqueueDebugReturn(DebugLevel.ERROR, "unexpected msgType " + b3.ToString());
					IL_5AE:
					result = true;
				}
			}
			return result;
		}

		internal void UpdateRoundTripTimeAndVariance(int lastRoundtripTime)
		{
			bool flag = lastRoundtripTime < 0;
			if (!flag)
			{
				this.roundTripTimeVariance -= this.roundTripTimeVariance / 4;
				bool flag2 = lastRoundtripTime >= this.roundTripTime;
				if (flag2)
				{
					this.roundTripTime += (lastRoundtripTime - this.roundTripTime) / 8;
					this.roundTripTimeVariance += (lastRoundtripTime - this.roundTripTime) / 4;
				}
				else
				{
					this.roundTripTime += (lastRoundtripTime - this.roundTripTime) / 8;
					this.roundTripTimeVariance -= (lastRoundtripTime - this.roundTripTime) / 4;
				}
				bool flag3 = this.roundTripTime < this.lowestRoundTripTime;
				if (flag3)
				{
					this.lowestRoundTripTime = this.roundTripTime;
				}
				bool flag4 = this.roundTripTimeVariance > this.highestRoundTripTimeVariance;
				if (flag4)
				{
					this.highestRoundTripTimeVariance = this.roundTripTimeVariance;
				}
			}
		}

		internal bool ExchangeKeysForEncryption(object lockObject)
		{
			this.isEncryptionAvailable = false;
			bool flag = this.CryptoProvider != null;
			if (flag)
			{
				this.CryptoProvider.Dispose();
				this.CryptoProvider = null;
			}
			bool flag2 = this.photonPeer.PayloadEncryptorType != null;
			if (flag2)
			{
				try
				{
					this.CryptoProvider = (ICryptoProvider)Activator.CreateInstance(this.photonPeer.PayloadEncryptorType);
					bool flag3 = this.CryptoProvider == null;
					if (flag3)
					{
						IPhotonPeerListener listener = this.Listener;
						DebugLevel level = DebugLevel.WARNING;
						string str = "Payload encryptor creation by type failed, Activator.CreateInstance() returned null for: ";
						Type payloadEncryptorType = this.photonPeer.PayloadEncryptorType;
						listener.DebugReturn(level, str + ((payloadEncryptorType != null) ? payloadEncryptorType.ToString() : null));
					}
				}
				catch (Exception ex)
				{
					IPhotonPeerListener listener2 = this.Listener;
					DebugLevel level2 = DebugLevel.WARNING;
					string str2 = "Payload encryptor creation by type failed: ";
					Exception ex2 = ex;
					listener2.DebugReturn(level2, str2 + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
			bool flag4 = this.CryptoProvider == null;
			if (flag4)
			{
				this.CryptoProvider = new DiffieHellmanCryptoProvider();
			}
			Dictionary<byte, object> dictionary = new Dictionary<byte, object>(1);
			dictionary[PhotonCodes.ClientKey] = this.CryptoProvider.PublicKey;
			bool flag5 = lockObject != null;
			if (flag5)
			{
				lock (lockObject)
				{
					SendOptions sendOptions = new SendOptions
					{
						Channel = 0,
						Encrypt = false,
						DeliveryMode = DeliveryMode.Reliable
					};
					StreamBuffer opBytes = this.SerializeOperationToMessage(PhotonCodes.InitEncryption, dictionary, EgMessageType.InternalOperationRequest, sendOptions.Encrypt);
					return this.EnqueuePhotonMessage(opBytes, sendOptions);
				}
			}
			SendOptions sendOptions2 = new SendOptions
			{
				Channel = 0,
				Encrypt = false,
				DeliveryMode = DeliveryMode.Reliable
			};
			StreamBuffer opBytes2 = this.SerializeOperationToMessage(PhotonCodes.InitEncryption, dictionary, EgMessageType.InternalOperationRequest, sendOptions2.Encrypt);
			return this.EnqueuePhotonMessage(opBytes2, sendOptions2);
		}

		internal void DeriveSharedKey(OperationResponse operationResponse)
		{
			bool flag = operationResponse.ReturnCode != 0;
			if (flag)
			{
				this.EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. " + operationResponse.ToStringFull());
				this.EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
			}
			else
			{
				byte[] array = (byte[])operationResponse.Parameters[PhotonCodes.ServerKey];
				bool flag2 = array == null || array.Length == 0;
				if (flag2)
				{
					this.EnqueueDebugReturn(DebugLevel.ERROR, "Establishing encryption keys failed. Server's public key is null or empty. " + operationResponse.ToStringFull());
					this.EnqueueStatusCallback(StatusCode.EncryptionFailedToEstablish);
				}
				else
				{
					this.CryptoProvider.DeriveSharedKey(array);
					this.isEncryptionAvailable = true;
					this.EnqueueStatusCallback(StatusCode.EncryptionEstablished);
				}
			}
		}

		internal virtual void InitEncryption(byte[] secret)
		{
			bool flag = this.photonPeer.PayloadEncryptorType != null;
			if (flag)
			{
				try
				{
					this.CryptoProvider = (ICryptoProvider)Activator.CreateInstance(this.photonPeer.PayloadEncryptorType, new object[]
					{
						secret
					});
					bool flag2 = this.CryptoProvider == null;
					if (flag2)
					{
						IPhotonPeerListener listener = this.Listener;
						DebugLevel level = DebugLevel.WARNING;
						string str = "Payload encryptor creation by type failed, Activator.CreateInstance() returned null for: ";
						Type payloadEncryptorType = this.photonPeer.PayloadEncryptorType;
						listener.DebugReturn(level, str + ((payloadEncryptorType != null) ? payloadEncryptorType.ToString() : null));
					}
					else
					{
						this.isEncryptionAvailable = true;
					}
				}
				catch (Exception ex)
				{
					IPhotonPeerListener listener2 = this.Listener;
					DebugLevel level2 = DebugLevel.WARNING;
					string str2 = "Payload encryptor creation by type failed: ";
					Exception ex2 = ex;
					listener2.DebugReturn(level2, str2 + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
			bool flag3 = this.CryptoProvider == null;
			if (flag3)
			{
				this.CryptoProvider = new DiffieHellmanCryptoProvider(secret);
				this.isEncryptionAvailable = true;
			}
		}

		internal void EnqueueActionForDispatch(PeerBase.MyAction action)
		{
			Queue<PeerBase.MyAction> actionQueue = this.ActionQueue;
			lock (actionQueue)
			{
				this.ActionQueue.Enqueue(action);
			}
		}

		internal void EnqueueDebugReturn(DebugLevel level, string debugReturn)
		{
			Queue<PeerBase.MyAction> actionQueue = this.ActionQueue;
			lock (actionQueue)
			{
				this.ActionQueue.Enqueue(delegate
				{
					this.Listener.DebugReturn(level, debugReturn);
				});
			}
		}

		internal void EnqueueStatusCallback(StatusCode statusValue)
		{
			Queue<PeerBase.MyAction> actionQueue = this.ActionQueue;
			lock (actionQueue)
			{
				this.ActionQueue.Enqueue(delegate
				{
					this.Listener.OnStatusChanged(statusValue);
				});
			}
		}

		public NetworkSimulationSet NetworkSimulationSettings
		{
			get
			{
				return this.networkSimulationSettings;
			}
		}

		internal void SendNetworkSimulated(byte[] dataToSend)
		{
			bool flag = !this.NetworkSimulationSettings.IsSimulationEnabled;
			if (flag)
			{
				throw new NotImplementedException("SendNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			bool flag2 = this.usedTransportProtocol == ConnectionProtocol.Udp && this.NetworkSimulationSettings.OutgoingLossPercentage > 0 && this.lagRandomizer.Next(101) < this.NetworkSimulationSettings.OutgoingLossPercentage;
			if (flag2)
			{
				NetworkSimulationSet networkSimulationSet = this.networkSimulationSettings;
				int lostPackagesOut = networkSimulationSet.LostPackagesOut;
				networkSimulationSet.LostPackagesOut = lostPackagesOut + 1;
			}
			else
			{
				int num = (this.networkSimulationSettings.OutgoingJitter <= 0) ? 0 : (this.lagRandomizer.Next(this.networkSimulationSettings.OutgoingJitter * 2) - this.networkSimulationSettings.OutgoingJitter);
				int num2 = this.networkSimulationSettings.OutgoingLag + num;
				int num3 = this.timeInt + num2;
				SimulationItem value = new SimulationItem
				{
					DelayedData = dataToSend,
					TimeToExecute = num3,
					Delay = num2
				};
				LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
				lock (netSimListOutgoing)
				{
					bool flag4 = this.NetSimListOutgoing.Count == 0 || this.usedTransportProtocol == ConnectionProtocol.Tcp;
					if (flag4)
					{
						this.NetSimListOutgoing.AddLast(value);
					}
					else
					{
						LinkedListNode<SimulationItem> linkedListNode = this.NetSimListOutgoing.First;
						while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
						{
							linkedListNode = linkedListNode.Next;
						}
						bool flag5 = linkedListNode == null;
						if (flag5)
						{
							this.NetSimListOutgoing.AddLast(value);
						}
						else
						{
							this.NetSimListOutgoing.AddBefore(linkedListNode, value);
						}
					}
				}
			}
		}

		internal void ReceiveNetworkSimulated(byte[] dataReceived)
		{
			bool flag = !this.networkSimulationSettings.IsSimulationEnabled;
			if (flag)
			{
				throw new NotImplementedException("ReceiveNetworkSimulated was called, despite NetworkSimulationSettings.IsSimulationEnabled == false.");
			}
			bool flag2 = this.usedTransportProtocol == ConnectionProtocol.Udp && this.networkSimulationSettings.IncomingLossPercentage > 0 && this.lagRandomizer.Next(101) < this.networkSimulationSettings.IncomingLossPercentage;
			if (flag2)
			{
				NetworkSimulationSet networkSimulationSet = this.networkSimulationSettings;
				int lostPackagesIn = networkSimulationSet.LostPackagesIn;
				networkSimulationSet.LostPackagesIn = lostPackagesIn + 1;
			}
			else
			{
				int num = (this.networkSimulationSettings.IncomingJitter <= 0) ? 0 : (this.lagRandomizer.Next(this.networkSimulationSettings.IncomingJitter * 2) - this.networkSimulationSettings.IncomingJitter);
				int num2 = this.networkSimulationSettings.IncomingLag + num;
				int num3 = this.timeInt + num2;
				SimulationItem value = new SimulationItem
				{
					DelayedData = dataReceived,
					TimeToExecute = num3,
					Delay = num2
				};
				LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
				lock (netSimListIncoming)
				{
					bool flag4 = this.NetSimListIncoming.Count == 0 || this.usedTransportProtocol == ConnectionProtocol.Tcp;
					if (flag4)
					{
						this.NetSimListIncoming.AddLast(value);
					}
					else
					{
						LinkedListNode<SimulationItem> linkedListNode = this.NetSimListIncoming.First;
						while (linkedListNode != null && linkedListNode.Value.TimeToExecute < num3)
						{
							linkedListNode = linkedListNode.Next;
						}
						bool flag5 = linkedListNode == null;
						if (flag5)
						{
							this.NetSimListIncoming.AddLast(value);
						}
						else
						{
							this.NetSimListIncoming.AddBefore(linkedListNode, value);
						}
					}
				}
			}
		}

		protected internal void NetworkSimRun()
		{
			for (;;)
			{
				bool flag = false;
				ManualResetEvent netSimManualResetEvent = this.networkSimulationSettings.NetSimManualResetEvent;
				lock (netSimManualResetEvent)
				{
					flag = this.networkSimulationSettings.IsSimulationEnabled;
				}
				bool flag3 = !flag;
				if (flag3)
				{
					this.networkSimulationSettings.NetSimManualResetEvent.WaitOne();
				}
				else
				{
					LinkedList<SimulationItem> netSimListIncoming = this.NetSimListIncoming;
					lock (netSimListIncoming)
					{
						while (this.NetSimListIncoming.First != null)
						{
							SimulationItem value = this.NetSimListIncoming.First.Value;
							bool flag5 = value.stopw.ElapsedMilliseconds < (long)value.Delay;
							if (flag5)
							{
								break;
							}
							this.ReceiveIncomingCommands(value.DelayedData, value.DelayedData.Length);
							this.NetSimListIncoming.RemoveFirst();
						}
					}
					LinkedList<SimulationItem> netSimListOutgoing = this.NetSimListOutgoing;
					lock (netSimListOutgoing)
					{
						while (this.NetSimListOutgoing.First != null)
						{
							SimulationItem value2 = this.NetSimListOutgoing.First.Value;
							bool flag7 = value2.stopw.ElapsedMilliseconds < (long)value2.Delay;
							if (flag7)
							{
								break;
							}
							bool flag8 = this.PhotonSocket != null && this.PhotonSocket.Connected;
							if (flag8)
							{
								this.PhotonSocket.Send(value2.DelayedData, value2.DelayedData.Length);
							}
							this.NetSimListOutgoing.RemoveFirst();
						}
					}
					Thread.Sleep(0);
				}
			}
		}

		internal bool TrafficStatsEnabled
		{
			get
			{
				return this.photonPeer.TrafficStatsEnabled;
			}
		}

		internal TrafficStats TrafficStatsIncoming
		{
			get
			{
				return this.photonPeer.TrafficStatsIncoming;
			}
		}

		internal TrafficStats TrafficStatsOutgoing
		{
			get
			{
				return this.photonPeer.TrafficStatsOutgoing;
			}
		}

		internal TrafficStatsGameLevel TrafficStatsGameLevel
		{
			get
			{
				return this.photonPeer.TrafficStatsGameLevel;
			}
		}

		internal PhotonPeer photonPeer;

		public IProtocol SerializationProtocol;

		internal ConnectionProtocol usedTransportProtocol;

		internal IPhotonSocket PhotonSocket;

		internal ConnectionStateValue peerConnectionState;

		internal int ByteCountLastOperation;

		internal int ByteCountCurrentDispatch;

		internal NCommand CommandInCurrentDispatch;

		internal int packetLossByCrc;

		internal int packetLossByChallenge;

		internal readonly Queue<PeerBase.MyAction> ActionQueue = new Queue<PeerBase.MyAction>();

		internal short peerID = -1;

		internal int serverTimeOffset;

		internal bool serverTimeOffsetIsAvailable;

		internal int roundTripTime;

		internal int roundTripTimeVariance;

		internal int lastRoundTripTime;

		internal int lowestRoundTripTime;

		internal int highestRoundTripTimeVariance;

		internal int timestampOfLastReceive;

		internal static short peerCount;

		internal long bytesOut;

		internal long bytesIn;

		internal object PhotonToken;

		internal object CustomInitData;

		public string AppId;

		internal EventData reusableEventData;

		private Stopwatch watch = Stopwatch.StartNew();

		internal int timeoutInt;

		internal int timeLastAckReceive;

		internal int longestSentCall;

		internal int timeLastSendAck;

		internal int timeLastSendOutgoing;

		internal bool ApplicationIsInitialized;

		internal bool isEncryptionAvailable;

		internal int outgoingCommandsInStream = 0;

		protected internal static Queue<StreamBuffer> MessageBufferPool = new Queue<StreamBuffer>(32);

		internal byte[] messageHeader;

		internal ICryptoProvider CryptoProvider;

		private readonly Random lagRandomizer = new Random();

		internal readonly LinkedList<SimulationItem> NetSimListOutgoing = new LinkedList<SimulationItem>();

		internal readonly LinkedList<SimulationItem> NetSimListIncoming = new LinkedList<SimulationItem>();

		private readonly NetworkSimulationSet networkSimulationSettings = new NetworkSimulationSet();

		internal int TrafficPackageHeaderSize;

		internal delegate void MyAction();

		private static class GpBinaryV3Parameters
		{
			public const byte CustomObject = 0;

			public const byte ExtraPlatformParams = 1;
		}
	}
}
