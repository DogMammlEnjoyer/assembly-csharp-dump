using System;
using System.Collections.Generic;

namespace ExitGames.Client.Photon
{
	internal class TPeer : PeerBase
	{
		internal override int QueuedIncomingCommandsCount
		{
			get
			{
				return this.incomingList.Count;
			}
		}

		internal override int QueuedOutgoingCommandsCount
		{
			get
			{
				return this.outgoingCommandsInStream;
			}
		}

		internal TPeer()
		{
			byte[] array = new byte[5];
			array[0] = 240;
			this.pingRequest = array;
			this.pingParamDict = new ParameterDictionary();
			this.DoFraming = true;
			this.waitForInitResponse = true;
			base..ctor();
			this.TrafficPackageHeaderSize = 0;
		}

		internal override bool IsTransportEncrypted()
		{
			return this.usedTransportProtocol == ConnectionProtocol.WebSocketSecure;
		}

		internal override void Reset()
		{
			base.Reset();
			bool flag = this.photonPeer.PayloadEncryptionSecret != null && this.usedTransportProtocol != ConnectionProtocol.WebSocketSecure;
			if (flag)
			{
				this.InitEncryption(this.photonPeer.PayloadEncryptionSecret);
			}
			this.incomingList = new Queue<StreamBuffer>(32);
			this.timestampOfLastReceive = base.timeInt;
			this.lastPingActivity = base.timeInt;
			this.waitForInitResponse = true;
		}

		internal override bool Connect(string serverAddress, string proxyServerAddress, string appID, object photonToken)
		{
			this.outgoingStream = new List<StreamBuffer>();
			this.messageHeader = (this.DoFraming ? TPeer.tcpFramedMessageHead : TPeer.tcpMsgHead);
			bool flag = this.usedTransportProtocol == ConnectionProtocol.WebSocket || this.usedTransportProtocol == ConnectionProtocol.WebSocketSecure;
			if (flag)
			{
				this.PhotonSocket.ConnectAddress = base.PrepareWebSocketUrl(serverAddress, appID, photonToken);
			}
			bool flag2 = this.PhotonSocket.Connect();
			bool result;
			if (flag2)
			{
				this.peerConnectionState = ConnectionStateValue.Connecting;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override void OnConnect()
		{
			this.lastPingActivity = base.timeInt;
			bool flag = this.DoFraming || this.PhotonToken != null;
			if (flag)
			{
				this.waitForInitResponse = true;
				byte[] data = base.WriteInitRequest();
				this.EnqueueInit(data);
			}
			else
			{
				this.waitForInitResponse = false;
			}
			this.SendOutgoingCommands();
		}

		internal override void Disconnect()
		{
			bool flag = this.peerConnectionState == ConnectionStateValue.Disconnected || this.peerConnectionState == ConnectionStateValue.Disconnecting;
			if (!flag)
			{
				bool flag2 = base.debugOut >= DebugLevel.ALL;
				if (flag2)
				{
					base.Listener.DebugReturn(DebugLevel.ALL, "TPeer.Disconnect()");
				}
				this.StopConnection();
			}
		}

		internal override void StopConnection()
		{
			this.peerConnectionState = ConnectionStateValue.Disconnecting;
			bool flag = this.PhotonSocket != null;
			if (flag)
			{
				this.PhotonSocket.Disconnect();
			}
			Queue<StreamBuffer> obj = this.incomingList;
			lock (obj)
			{
				this.incomingList.Clear();
			}
			this.peerConnectionState = ConnectionStateValue.Disconnected;
			base.EnqueueStatusCallback(StatusCode.Disconnect);
		}

		internal override void FetchServerTimestamp()
		{
			this.SendPing();
			this.serverTimeOffsetIsAvailable = false;
		}

		private void EnqueueInit(byte[] data)
		{
			StreamBuffer streamBuffer = new StreamBuffer(data.Length + 32);
			byte[] array = new byte[]
			{
				251,
				0,
				0,
				0,
				0,
				0,
				1
			};
			int num = 1;
			Protocol.Serialize(data.Length + array.Length, array, ref num);
			streamBuffer.Write(array, 0, array.Length);
			streamBuffer.Write(data, 0, data.Length);
			bool trafficStatsEnabled = base.TrafficStatsEnabled;
			if (trafficStatsEnabled)
			{
				base.TrafficStatsOutgoing.CountControlCommand(streamBuffer.Length);
			}
			this.EnqueueMessageAsPayload(DeliveryMode.Reliable, streamBuffer, 0);
		}

		internal override bool DispatchIncomingCommands()
		{
			bool flag = this.peerConnectionState == ConnectionStateValue.Connected && base.timeInt - this.timestampOfLastReceive > base.DisconnectTimeout;
			if (flag)
			{
				base.EnqueueStatusCallback(StatusCode.TimeoutDisconnect);
				base.EnqueueActionForDispatch(new PeerBase.MyAction(this.Disconnect));
			}
			for (;;)
			{
				Queue<PeerBase.MyAction> actionQueue = this.ActionQueue;
				PeerBase.MyAction myAction;
				lock (actionQueue)
				{
					bool flag3 = this.ActionQueue.Count <= 0;
					if (flag3)
					{
						break;
					}
					myAction = this.ActionQueue.Dequeue();
				}
				myAction();
			}
			Queue<StreamBuffer> obj = this.incomingList;
			StreamBuffer streamBuffer;
			lock (obj)
			{
				bool flag5 = this.incomingList.Count <= 0;
				if (flag5)
				{
					return false;
				}
				streamBuffer = this.incomingList.Dequeue();
			}
			this.ByteCountCurrentDispatch = streamBuffer.Length + 3;
			bool result = this.DeserializeMessageAndCallback(streamBuffer);
			PeerBase.MessageBufferPoolPut(streamBuffer);
			return result;
		}

		internal override bool SendOutgoingCommands()
		{
			bool flag = this.peerConnectionState == ConnectionStateValue.Disconnected;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.SendPing();
				bool flag2 = !this.PhotonSocket.Connected;
				if (flag2)
				{
					result = false;
				}
				else
				{
					this.timeLastSendOutgoing = base.timeInt;
					List<StreamBuffer> obj = this.outgoingStream;
					lock (obj)
					{
						int num = 0;
						int num2 = 0;
						PhotonSocketError photonSocketError = PhotonSocketError.Success;
						for (int i = 0; i < this.outgoingStream.Count; i++)
						{
							StreamBuffer streamBuffer = this.outgoingStream[i];
							photonSocketError = this.SendData(streamBuffer.GetBuffer(), streamBuffer.Length);
							bool flag4 = photonSocketError == PhotonSocketError.Busy;
							if (flag4)
							{
								break;
							}
							num2 += streamBuffer.Length;
							num++;
							bool flag5 = photonSocketError != PhotonSocketError.PendingSend;
							if (flag5)
							{
								PeerBase.MessageBufferPoolPut(streamBuffer);
							}
							bool flag6 = num2 >= base.mtu || photonSocketError == PhotonSocketError.PendingSend;
							if (flag6)
							{
								break;
							}
						}
						this.outgoingStream.RemoveRange(0, num);
						this.outgoingCommandsInStream -= num;
						bool flag7 = photonSocketError == PhotonSocketError.Busy || photonSocketError == PhotonSocketError.PendingSend;
						if (flag7)
						{
							result = false;
						}
						else
						{
							result = (this.outgoingStream.Count > 0);
						}
					}
				}
			}
			return result;
		}

		internal override bool SendAcksOnly()
		{
			bool flag = this.peerConnectionState == ConnectionStateValue.Disconnected;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				this.SendPing();
				result = false;
			}
			return result;
		}

		internal override bool EnqueuePhotonMessage(StreamBuffer opBytes, SendOptions sendParams)
		{
			return this.EnqueueMessageAsPayload(sendParams.DeliveryMode, opBytes, sendParams.Channel);
		}

		internal bool EnqueueMessageAsPayload(DeliveryMode deliveryMode, StreamBuffer opMessage, byte channelId)
		{
			bool flag = opMessage == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool doFraming = this.DoFraming;
				if (doFraming)
				{
					byte[] buffer = opMessage.GetBuffer();
					int num = 1;
					Protocol.Serialize(opMessage.Length, buffer, ref num);
					buffer[5] = channelId;
					switch (deliveryMode)
					{
					case DeliveryMode.Unreliable:
						buffer[6] = 0;
						break;
					case DeliveryMode.Reliable:
						buffer[6] = 1;
						break;
					case DeliveryMode.UnreliableUnsequenced:
						buffer[6] = 2;
						break;
					case DeliveryMode.ReliableUnsequenced:
						buffer[6] = 3;
						break;
					default:
						throw new ArgumentOutOfRangeException("DeliveryMode", deliveryMode, null);
					}
				}
				List<StreamBuffer> obj = this.outgoingStream;
				lock (obj)
				{
					this.outgoingStream.Add(opMessage);
					this.outgoingCommandsInStream++;
				}
				int length = opMessage.Length;
				this.ByteCountLastOperation = length;
				bool trafficStatsEnabled = base.TrafficStatsEnabled;
				if (trafficStatsEnabled)
				{
					switch (deliveryMode)
					{
					case DeliveryMode.Unreliable:
					case DeliveryMode.UnreliableUnsequenced:
						base.TrafficStatsOutgoing.CountUnreliableOpCommand(length);
						break;
					case DeliveryMode.Reliable:
					case DeliveryMode.ReliableUnsequenced:
						base.TrafficStatsOutgoing.CountReliableOpCommand(length);
						break;
					}
					base.TrafficStatsGameLevel.CountOperation(length);
				}
				result = true;
			}
			return result;
		}

		internal void SendPing()
		{
			bool flag = base.timeInt - this.lastPingActivity < base.timePingInterval;
			if (!flag)
			{
				bool flag2 = this.peerConnectionState == ConnectionStateValue.Connected;
				if (!flag2)
				{
					bool flag3 = this.peerConnectionState != ConnectionStateValue.Connecting || this.waitForInitResponse;
					if (flag3)
					{
						return;
					}
				}
				int timeInt = base.timeInt;
				this.lastPingActivity = timeInt;
				bool flag4 = !this.DoFraming;
				StreamBuffer streamBuffer;
				if (flag4)
				{
					ParameterDictionary obj = this.pingParamDict;
					lock (obj)
					{
						this.pingParamDict[1] = timeInt;
						streamBuffer = base.SerializeOperationToMessage(PhotonCodes.Ping, this.pingParamDict, EgMessageType.InternalOperationRequest, false);
					}
				}
				else
				{
					int num = 1;
					Protocol.Serialize(timeInt, this.pingRequest, ref num);
					streamBuffer = PeerBase.MessageBufferPoolGet();
					streamBuffer.Write(this.pingRequest, 0, this.pingRequest.Length);
				}
				bool trafficStatsEnabled = base.TrafficStatsEnabled;
				if (trafficStatsEnabled)
				{
					base.TrafficStatsOutgoing.CountControlCommand(streamBuffer.Length);
				}
				PhotonSocketError photonSocketError = this.SendData(streamBuffer.GetBuffer(), streamBuffer.Length);
				bool flag6 = photonSocketError == PhotonSocketError.Success;
				if (flag6)
				{
					PeerBase.MessageBufferPoolPut(streamBuffer);
				}
			}
		}

		internal PhotonSocketError SendData(byte[] data, int length)
		{
			PhotonSocketError result = PhotonSocketError.Success;
			try
			{
				this.bytesOut += (long)length;
				bool trafficStatsEnabled = base.TrafficStatsEnabled;
				if (trafficStatsEnabled)
				{
					TrafficStats trafficStatsOutgoing = base.TrafficStatsOutgoing;
					int totalPacketCount = trafficStatsOutgoing.TotalPacketCount;
					trafficStatsOutgoing.TotalPacketCount = totalPacketCount + 1;
					base.TrafficStatsOutgoing.TotalCommandsInPackets++;
				}
				bool isSimulationEnabled = base.NetworkSimulationSettings.IsSimulationEnabled;
				if (isSimulationEnabled)
				{
					byte[] array = new byte[length];
					Buffer.BlockCopy(data, 0, array, 0, length);
					base.SendNetworkSimulated(array);
				}
				else
				{
					int timeInt = base.timeInt;
					result = this.PhotonSocket.Send(data, length);
					int num = base.timeInt - timeInt;
					bool flag = num > this.longestSentCall;
					if (flag)
					{
						this.longestSentCall = num;
					}
				}
			}
			catch (Exception ex)
			{
				bool flag2 = base.debugOut >= DebugLevel.ERROR;
				if (flag2)
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, ex.ToString());
				}
				SupportClass.WriteStackTrace(ex);
			}
			return result;
		}

		internal override void ReceiveIncomingCommands(byte[] inbuff, int dataLength)
		{
			bool flag = inbuff == null;
			if (flag)
			{
				bool flag2 = base.debugOut >= DebugLevel.ERROR;
				if (flag2)
				{
					base.EnqueueDebugReturn(DebugLevel.ERROR, "checkAndQueueIncomingCommands() inBuff: null");
				}
			}
			else
			{
				this.timestampOfLastReceive = base.timeInt;
				this.bytesIn += (long)(dataLength + 7);
				bool trafficStatsEnabled = base.TrafficStatsEnabled;
				if (trafficStatsEnabled)
				{
					TrafficStats trafficStatsIncoming = base.TrafficStatsIncoming;
					int num = trafficStatsIncoming.TotalPacketCount;
					trafficStatsIncoming.TotalPacketCount = num + 1;
					TrafficStats trafficStatsIncoming2 = base.TrafficStatsIncoming;
					num = trafficStatsIncoming2.TotalCommandsInPackets;
					trafficStatsIncoming2.TotalCommandsInPackets = num + 1;
				}
				bool flag3 = inbuff[0] == 243;
				if (flag3)
				{
					byte b = inbuff[1] & 127;
					byte b2 = inbuff[2];
					bool flag4 = b == 7 && b2 == PhotonCodes.Ping;
					if (flag4)
					{
						this.DeserializeMessageAndCallback(new StreamBuffer(inbuff));
					}
					else
					{
						StreamBuffer streamBuffer = PeerBase.MessageBufferPoolGet();
						streamBuffer.Write(inbuff, 0, dataLength);
						streamBuffer.Position = 0;
						Queue<StreamBuffer> obj = this.incomingList;
						lock (obj)
						{
							this.incomingList.Enqueue(streamBuffer);
						}
					}
				}
				else
				{
					bool flag6 = inbuff[0] == 240;
					if (flag6)
					{
						base.TrafficStatsIncoming.CountControlCommand(dataLength);
						this.ReadPingResult(inbuff);
					}
					else
					{
						bool flag7 = base.debugOut >= DebugLevel.ERROR && dataLength > 0;
						if (flag7)
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, "receiveIncomingCommands() MagicNumber should be 0xF0 or 0xF3. Is: " + inbuff[0].ToString() + " dataLength: " + dataLength.ToString());
						}
					}
				}
			}
		}

		private void ReadPingResult(byte[] inbuff)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 1;
			Protocol.Deserialize(out num, inbuff, ref num3);
			Protocol.Deserialize(out num2, inbuff, ref num3);
			this.lastRoundTripTime = base.timeInt - num2;
			bool flag = !this.serverTimeOffsetIsAvailable;
			if (flag)
			{
				this.roundTripTime = this.lastRoundTripTime;
			}
			base.UpdateRoundTripTimeAndVariance(this.lastRoundTripTime);
			bool flag2 = !this.serverTimeOffsetIsAvailable;
			if (flag2)
			{
				this.serverTimeOffset = num + (this.lastRoundTripTime >> 1) - base.timeInt;
				this.serverTimeOffsetIsAvailable = true;
			}
		}

		protected internal void ReadPingResult(OperationResponse operationResponse)
		{
			int num = (int)operationResponse.Parameters[2];
			int num2 = (int)operationResponse.Parameters[1];
			this.lastRoundTripTime = base.timeInt - num2;
			bool flag = !this.serverTimeOffsetIsAvailable;
			if (flag)
			{
				this.roundTripTime = this.lastRoundTripTime;
			}
			base.UpdateRoundTripTimeAndVariance(this.lastRoundTripTime);
			bool flag2 = !this.serverTimeOffsetIsAvailable;
			if (flag2)
			{
				this.serverTimeOffset = num + (this.lastRoundTripTime >> 1) - base.timeInt;
				this.serverTimeOffsetIsAvailable = true;
			}
		}

		internal const int TCP_HEADER_BYTES = 7;

		internal const int MSG_HEADER_BYTES = 2;

		public const int ALL_HEADER_BYTES = 9;

		private Queue<StreamBuffer> incomingList = new Queue<StreamBuffer>(32);

		internal List<StreamBuffer> outgoingStream;

		private int lastPingActivity;

		private readonly byte[] pingRequest;

		private readonly ParameterDictionary pingParamDict;

		internal static readonly byte[] tcpFramedMessageHead = new byte[]
		{
			251,
			0,
			0,
			0,
			0,
			0,
			0,
			243,
			2
		};

		internal static readonly byte[] tcpMsgHead = new byte[]
		{
			243,
			2
		};

		protected internal bool DoFraming;

		private bool waitForInitResponse;
	}
}
