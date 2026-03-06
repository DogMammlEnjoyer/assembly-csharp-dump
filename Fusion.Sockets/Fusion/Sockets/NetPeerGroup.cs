using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Fusion.Sockets
{
	public struct NetPeerGroup
	{
		public double Time
		{
			get
			{
				return this._clock.ElapsedInSeconds;
			}
		}

		public int Group
		{
			get
			{
				return (int)this._group;
			}
		}

		public unsafe int ConnectionCount
		{
			get
			{
				return this._connectionsMap->Count;
			}
		}

		internal unsafe static void Dispose(NetPeerGroup* g, INetPeerGroupCallbacks callbacks)
		{
			bool flag = g == null;
			if (!flag)
			{
				NetConnectionMap.Dispose(ref g->_connectionsMap, callbacks);
				NetBitBufferBlock.Dispose(ref g->_sendBlock);
				NetBitBufferStack.Dispose(ref g->_recvStack);
			}
		}

		public unsafe static NetConnection* GetConnection(NetPeerGroup* g, NetConnectionId id)
		{
			return g->_connectionsMap->Find(id);
		}

		public unsafe static NetConnection* GetConnectionByIndex(NetPeerGroup* g, int index)
		{
			return g->_connectionsMap->FindByIndex(index);
		}

		public unsafe static bool TryGetConnectionByIndex(NetPeerGroup* g, int index, out NetConnection* connection)
		{
			return g->_connectionsMap->TryFindByIndex(index, out connection);
		}

		public unsafe static NetConnectionMap.Iterator ConnectionIterator(NetPeerGroup* g)
		{
			return new NetConnectionMap.Iterator(g->_connectionsMap);
		}

		public unsafe static void Connect(NetPeerGroup* g, NetAddress address, byte[] token, byte[] uniqueId = null)
		{
			NetConnection* ptr = NetPeerGroup.AllocateConnection(g, address, token, uniqueId);
			bool flag = ptr == null;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log("No free connection slots");
				}
			}
			else
			{
				NetPeerGroup.ChangeConnectionStatus(g, null, ptr, NetConnectionStatus.Connecting);
				NetPeerGroup.SendCommandConnect(g, null, ptr);
			}
		}

		public unsafe static void Connect(NetPeerGroup* g, string ip, ushort port, byte[] token, byte[] uniqueId = null)
		{
			NetPeerGroup.Connect(g, NetAddress.CreateFromIpPort(ip, port), token, uniqueId);
		}

		public unsafe static void Disconnect(NetPeerGroup* g, NetConnection* c, byte[] token)
		{
			bool flag = g == null;
			if (!flag)
			{
				bool flag2 = c->Status == NetConnectionStatus.Connected || c->Status == NetConnectionStatus.Connecting;
				if (flag2)
				{
					NetPeerGroup.SendCommand<NetCommandDisconnect>(g, c, NetCommandDisconnect.Create(NetDisconnectReason.Requested, token));
					NetPeerGroup.DisconnectInternal(g, c, NetDisconnectReason.Requested, null);
				}
			}
		}

		internal unsafe static void DisconnectInternal(NetPeerGroup* g, NetConnection* c, NetDisconnectReason reason = NetDisconnectReason.ByRemote, byte[] token = null)
		{
			bool flag = g == null;
			if (!flag)
			{
				bool flag2 = c->Status == NetConnectionStatus.Connected || c->Status == NetConnectionStatus.Connecting;
				if (flag2)
				{
					c->StateDisconnected = default(NetConnection.StateDisconnectedData);
					c->StateDisconnected.Reason = reason;
					bool flag3 = token != null;
					if (flag3)
					{
						int num = Math.Min(128, token.Length);
						c->DisconnectTokenLength = num;
						c->DisconnectToken = Native.MallocAndClearArray<byte>(num);
						for (int i = 0; i < num; i++)
						{
							c->DisconnectToken[i] = token[i];
						}
					}
					else
					{
						c->DisconnectToken = null;
						c->DisconnectTokenLength = 0;
					}
					NetPeerGroup.ChangeConnectionStatus(g, null, c, NetConnectionStatus.Disconnected);
				}
			}
		}

		public unsafe static void Update(NetPeerGroup* g, INetPeerGroupCallbacks cb)
		{
			bool flag = g == null;
			if (!flag)
			{
				NetPeerGroup.Receive(g, cb);
				Assert.Check(g->_recvStack.Count == 0);
				NetPeerGroup.UpdateConnections(g, cb);
				IntPtr intPtr;
				do
				{
					intPtr = Volatile.Read(ref g->_recvHead);
				}
				while (Interlocked.CompareExchange(ref g->_recvHead, IntPtr.Zero, intPtr) != intPtr);
				bool flag2 = intPtr != IntPtr.Zero;
				if (flag2)
				{
					g->_recvStack.PushFromHead((NetBitBuffer*)((void*)intPtr));
					NetPeerGroup.Receive(g, cb);
				}
			}
		}

		internal unsafe static void Initialize(short groupIndex, NetPeerGroup* g, NetPeer* p, NetConfig config)
		{
			*g = default(NetPeerGroup);
			g->_config = config;
			g->_peer = p;
			g->_group = groupIndex;
			g->_clock = Timer.StartNew();
			g->_sendBlock = NetBitBufferBlock.Create(config.PacketSize);
			g->_recvStack = NetBitBufferStack.Create(1024);
			int connectionsPerGroup = g->_config.ConnectionsPerGroup;
			NetConfig* ptr = &g->_config;
			g->_connectionsMap = NetConnectionMap.Allocate(connectionsPerGroup, groupIndex, ptr);
			g->ReliableSendInterval = 0.05;
		}

		internal unsafe static IntPtr PopSendHead(NetPeerGroup* g)
		{
			IntPtr intPtr;
			do
			{
				intPtr = Volatile.Read(ref g->_sendHead);
			}
			while (Interlocked.CompareExchange(ref g->_sendHead, IntPtr.Zero, intPtr) != intPtr);
			return intPtr;
		}

		internal unsafe static void PushOnRecvHead(NetPeerGroup* g, NetBitBuffer* b)
		{
			IntPtr intPtr;
			do
			{
				intPtr = Volatile.Read(ref g->_recvHead);
				b->Next = (NetBitBuffer*)((void*)intPtr);
			}
			while (Interlocked.CompareExchange(ref g->_recvHead, (IntPtr)((void*)b), intPtr) != intPtr);
		}

		private unsafe static void UpdateConnections(NetPeerGroup* g, INetPeerGroupCallbacks cb)
		{
			int countUsed = g->_connectionsMap->CountUsed;
			NetConnection* connectionsBuffer = g->_connectionsMap->ConnectionsBuffer;
			for (int i = 0; i < countUsed; i++)
			{
				NetConnection* ptr = connectionsBuffer + i;
				bool flag = ptr->MapState != NetConnectionMap.EntryState.Used;
				if (!flag)
				{
					switch (ptr->Status)
					{
					case NetConnectionStatus.Connecting:
						NetPeerGroup.UpdateConnecting(g, cb, ptr);
						break;
					case NetConnectionStatus.Connected:
						NetPeerGroup.UpdateConnected(g, cb, ptr);
						break;
					case NetConnectionStatus.Disconnected:
						NetPeerGroup.UpdateDisconnected(g, cb, ptr);
						break;
					case NetConnectionStatus.Shutdown:
						NetPeerGroup.UpdateShutdown(g, cb, ptr);
						break;
					}
				}
			}
		}

		public unsafe static void SendReliable(NetPeerGroup* g, NetConnection* c, ReliableId rid, byte* data, int dataLength)
		{
			Assert.Check(sizeof(ReliableId) == 48);
			Assert.Check(sizeof(ReliableHeader) == 64);
			Assert.Check(c->Status == NetConnectionStatus.Connected);
			Assert.Check((void*)data);
			Assert.Check(dataLength >= 0);
			int num = dataLength;
			while (dataLength > 0)
			{
				int num2 = Math.Min(dataLength, 1088);
				ReliableHeader* ptr = (ReliableHeader*)Native.MallocAndClear(64 + num2);
				rid.Sequence = c->ReliableBuffer.NextSendSequence();
				rid.SliceLength = num2;
				rid.TotalLength = ((rid.TotalLength < num) ? num : rid.TotalLength);
				ptr->Id = rid;
				Native.MemCpy((void*)(ptr + 64 / sizeof(ReliableHeader)), (void*)data, num2);
				data += num2;
				dataLength -= num2;
				c->ReliableSendList.AddLast(ptr);
			}
		}

		public unsafe static void ChangeConnectionAddressDuringConnecting(NetPeerGroup* g, NetConnection* c, NetAddress newAddress)
		{
			Assert.Check(c->Status == NetConnectionStatus.Connecting);
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("Changing address for connection ({0}:{1}) from {2} to {3} during connecting phase", new object[]
				{
					c->LocalId,
					(IntPtr)((void*)c),
					c->Address,
					newAddress
				}));
			}
			NetAddress address = c->Address;
			g->_connectionsMap->Remap(address, newAddress);
			Assert.Check(c->Address.Equals(newAddress));
			NetPeer.RemapAddress(g->_peer, address, newAddress);
		}

		private unsafe static void SendCommandConnect(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			Assert.Check(c->Status == NetConnectionStatus.Connecting);
			bool flag = c->StateConnecting.Attempts == g->_config.ConnectAttempts;
			if (flag)
			{
				Assert.Check(cb != null);
				NetAddress address = c->Address;
				NetPeerGroup.ChangeConnectionStatus(g, cb, c, NetConnectionStatus.Shutdown);
				cb.OnConnectionFailed(address, NetConnectFailedReason.Timeout);
			}
			else
			{
				if (cb != null)
				{
					cb.OnConnectionAttempt(c, c->StateConnecting.Attempts, g->_config.ConnectAttempts);
				}
				c->StateConnecting.Attempts = c->StateConnecting.Attempts + 1;
				c->StateConnecting.AttemptTimeout = g->_clock.ElapsedInSeconds + g->_config.ConnectInterval;
				NetPeerGroup.SendCommand<NetCommandConnect>(g, c, NetCommandConnect.Create(c->LocalId, c->ConnectionToken, c->ConnectionTokenLength, c->UniqueId));
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(string.Format("Connection Attempt: {0} [{1}/{2}]", *c, c->StateConnecting.Attempts, g->_config.ConnectAttempts));
				}
			}
		}

		private unsafe static void UpdateConnecting(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			bool flag = c->StateConnecting.AttemptTimeout < g->_clock.ElapsedInSeconds;
			if (flag)
			{
				NetPeerGroup.SendCommandConnect(g, cb, c);
			}
		}

		private unsafe static void UpdateConnected(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			bool flag = c->RecvTime + g->_config.ConnectionTimeout < g->_clock.ElapsedInSeconds;
			if (flag)
			{
				NetPeerGroup.DisconnectInternal(g, c, NetDisconnectReason.Timeout, null);
			}
			else
			{
				bool flag2 = c->ReliableSendTimer.IsRunning && c->ReliableSendTimer.Peek() >= g->ReliableSendInterval && c->ReliableSendList.Count > 0;
				if (flag2)
				{
					NetBitBuffer* ptr;
					bool notifyDataBuffer = NetPeerGroup.GetNotifyDataBuffer(g, c, out ptr);
					if (notifyDataBuffer)
					{
						c->ReliableSendTimer.Consume();
						ReliableHeader* ptr2 = c->ReliableSendList.RemoveHead();
						byte* data = ReliableHeader.GetData(ptr2);
						ptr->WriteBytesAligned((void*)(&ptr2->Id), 48);
						Native.MemCpy((void*)ptr->GetDataPointer(), (void*)data, ptr2->Id.SliceLength);
						ptr->OffsetBits += ptr2->Id.SliceLength * 8;
						ptr->PacketType = NetPacketType.NotifyReliableData;
						bool flag3 = !NetPeerGroup.SendNotifyDataBuffer(g, c, ptr, (void*)ptr2);
						if (flag3)
						{
							Native.Free<ReliableHeader>(ref ptr2);
							c->ReliableSendList.Dispose();
						}
					}
				}
				bool flag4 = (c->NotifyRecvUnackedCount > 0 && c->NotifyRecvUnackedCount >= g->_config.Notify.AckForceCount) || c->NotifySendTime + g->_config.Notify.AckForceTimeout < g->_clock.ElapsedInSeconds;
				if (flag4)
				{
					NetBitBuffer* ptr3;
					bool connectionSendBuffer = NetPeerGroup.GetConnectionSendBuffer(g, c, out ptr3);
					if (connectionSendBuffer)
					{
						c->NotifyRecvUnackedCount = 0;
						c->NotifySendTime = g->_clock.ElapsedInSeconds;
						*(NetNotifyHeader*)ptr3->Data = NetNotifyHeader.CreateAcks(c->NotifyRecvSequence, c->NotifyRecvMask);
						ptr3->OffsetBits = 112;
						NetPeerGroup.Send(g, c, ptr3);
					}
				}
			}
		}

		private unsafe static void UpdateDisconnected(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			bool flag = c->StateDisconnected.SentDisconnectCommand == 0;
			if (flag)
			{
				c->StateDisconnected.SentDisconnectCommand = (NetPeerGroup.SendCommand<NetCommandDisconnect>(g, c, NetCommandDisconnect.Create(c->StateDisconnected.Reason, c->DisconnectToken, c->DisconnectTokenLength)) ? 1 : 0);
			}
			bool flag2 = c->StateDisconnected.CallbackInvoked == 0;
			if (flag2)
			{
				c->StateDisconnected.CallbackInvoked = 1;
				cb.OnDisconnected(c, c->StateDisconnected.Reason);
			}
			bool flag3 = c->StateDisconnected.SentDisconnectCommand == 1 && c->StateDisconnected.CallbackInvoked == 1;
			if (flag3)
			{
				NetPeerGroup.ChangeConnectionStatus(g, cb, c, NetConnectionStatus.Shutdown);
			}
		}

		private unsafe static void UpdateShutdown(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			bool flag = c->StateShutdown.Unmapped == 1;
			if (flag)
			{
				bool flag2 = c->StateShutdown.Timeout < g->_clock.ElapsedInSeconds;
				if (flag2)
				{
					NetPeerGroup.ReleaseConnection(g, cb, c);
				}
			}
			else
			{
				bool flag3 = c->StateShutdown.Unmapped == 0;
				if (flag3)
				{
					NetPeerGroup.QueueAddressUnmap(g, c);
				}
			}
		}

		private unsafe static void SendUnconnected(NetPeerGroup* g, NetBitBuffer* b)
		{
			IntPtr sendHead;
			do
			{
				sendHead = g->_sendHead;
				b->Next = (NetBitBuffer*)((void*)sendHead);
			}
			while (Interlocked.CompareExchange(ref g->_sendHead, (IntPtr)((void*)b), sendHead) != sendHead);
		}

		private unsafe static void Send(NetPeerGroup* g, NetConnection* c, NetBitBuffer* b)
		{
			Assert.Check(!c->Address.Equals(default(NetAddress)));
			bool flag = b->PacketType == NetPacketType.NotifyData;
			if (flag)
			{
				c->NotifyRecvUnackedCount = 0;
			}
			c->SendTime = g->_clock.ElapsedInSeconds;
			IntPtr sendHead;
			do
			{
				sendHead = g->_sendHead;
				b->Next = (NetBitBuffer*)((void*)sendHead);
			}
			while (Interlocked.CompareExchange(ref g->_sendHead, (IntPtr)((void*)b), sendHead) != sendHead);
		}

		private unsafe static bool GetConnectionSendBuffer(NetPeerGroup* g, NetConnection* c, out NetBitBuffer* b)
		{
			bool flag = g->_sendBlock->TryAcquire(out b);
			bool result;
			if (flag)
			{
				b.Group = g->_group;
				b.Address = c->Address;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public unsafe static bool SendUnconnectedData(NetPeerGroup* g, NetAddress address, void* data, int dataLength)
		{
			NetBitBuffer* ptr;
			bool flag = g->_sendBlock->TryAcquire(out ptr);
			bool result;
			if (flag)
			{
				*(byte*)ptr->Data = 5;
				ptr->Group = 0;
				ptr->OffsetBits = 8;
				ptr->Address = address;
				ptr->WriteBytesAligned(data, dataLength);
				NetPeerGroup.SendUnconnected(g, ptr);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public unsafe static bool GetUnreliableDataBuffer(NetPeerGroup* g, NetConnection* c, out NetBitBuffer* b)
		{
			bool flag = c->Status == NetConnectionStatus.Connected && NetPeerGroup.GetConnectionSendBuffer(g, c, out b);
			bool result;
			if (flag)
			{
				*(NetUnreliableHeader*)b.Data = NetUnreliableHeader.Create();
				b.OffsetBits = 8;
				result = true;
			}
			else
			{
				b = (IntPtr)((UIntPtr)0);
				result = false;
			}
			return result;
		}

		public unsafe static bool SendUnreliableDataBuffer(NetPeerGroup* g, NetConnection* c, NetBitBuffer* b)
		{
			Assert.Check(b->PacketType == NetPacketType.UnreliableData);
			bool flag = c->Status != NetConnectionStatus.Connected;
			bool result;
			if (flag)
			{
				NetBitBuffer.Release(b);
				result = false;
			}
			else
			{
				NetPeerGroup.Send(g, c, b);
				result = true;
			}
			return result;
		}

		public unsafe static bool GetNotifyDataBuffer(NetPeerGroup* g, NetConnection* c, out NetBitBuffer* b)
		{
			bool flag = c->Status == NetConnectionStatus.Connected && !c->NotifySendWindow.IsFull && NetPeerGroup.GetConnectionSendBuffer(g, c, out b);
			bool result;
			if (flag)
			{
				NetNotifyHeader netNotifyHeader = default(NetNotifyHeader);
				netNotifyHeader.PacketType = NetPacketType.NotifyData;
				*(NetNotifyHeader*)b.Data = netNotifyHeader;
				b.OffsetBits = 112;
				result = true;
			}
			else
			{
				b = (IntPtr)((UIntPtr)0);
				result = false;
			}
			return result;
		}

		public unsafe static bool SendNotifyDataBuffer(NetPeerGroup* g, NetConnection* c, NetBitBuffer* b, void* userData)
		{
			Assert.Check(b->PacketType == NetPacketType.NotifyData || b->PacketType == NetPacketType.NotifyReliableData);
			bool flag = c->Status != NetConnectionStatus.Connected;
			bool result;
			if (flag)
			{
				NetBitBuffer.Release(b);
				result = false;
			}
			else
			{
				bool isFull = c->NotifySendWindow.IsFull;
				if (isFull)
				{
					NetPeerGroup.DisconnectInternal(g, c, NetDisconnectReason.SendWindowFull, null);
					NetBitBuffer.Release(b);
					result = false;
				}
				else
				{
					TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork != null)
					{
						logTraceNetwork.Log(string.Format("{0} Send:{1}", LogUtils.GetDump<NetBitBuffer>(b), Maths.BytesRequiredForBits(b->OffsetBits)));
					}
					NetNotifyHeader netNotifyHeader = NetNotifyHeader.CreateData(NetConnection.NextNotifySendSequence(c), c->NotifyRecvSequence, c->NotifyRecvMask);
					bool flag2 = b->PacketType == NetPacketType.NotifyReliableData;
					if (flag2)
					{
						netNotifyHeader.PacketType = NetPacketType.NotifyReliableData;
					}
					Native.MemCpy((void*)b->Data, (void*)(&netNotifyHeader), 14);
					NetSendEnvelope netSendEnvelope;
					netSendEnvelope.Sequence = netNotifyHeader.Sequence;
					netSendEnvelope.UserData = userData;
					netSendEnvelope.SendTime = g->_clock.ElapsedInSeconds;
					netSendEnvelope.PacketType = b->PacketType;
					c->NotifySendWindow.Push(netSendEnvelope);
					c->NotifySendTime = netSendEnvelope.SendTime;
					NetPeerGroup.Send(g, c, b);
					result = true;
				}
			}
			return result;
		}

		private unsafe static void Receive(NetPeerGroup* g, INetPeerGroupCallbacks cb)
		{
			NetBitBuffer* ptr = null;
			while (g->_recvStack.TryPop(&ptr))
			{
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Log(string.Format("Receive:{0}", ptr->LengthBytes));
				}
				try
				{
					NetConnection* ptr2 = g->_connectionsMap->Find(ptr->Address);
					bool flag = ptr2 == null;
					if (flag)
					{
						NetPeerGroup.HandlePacketUnconnected(g, cb, ptr);
					}
					else
					{
						NetPeerGroup.HandlePacket(g, cb, ptr2, ptr);
					}
				}
				finally
				{
					NetBitBuffer.Release(ptr);
				}
			}
		}

		private unsafe static void HandlePacketUnconnected(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetBitBuffer* b)
		{
			bool flag = b->PacketType == NetPacketType.Command;
			if (flag)
			{
				DebugLogStream logDebug = InternalLogStreams.LogDebug;
				if (logDebug != null)
				{
					logDebug.Log(string.Format("Handle Packet Unconnected from {0}", b->Address));
				}
				NetCommandHeader* data = (NetCommandHeader*)b->Data;
				bool flag2 = data->Command == NetCommands.Connect;
				if (flag2)
				{
					NetCommandConnect command = *(NetCommandConnect*)b->Data;
					byte[] tokenDataAsArray = NetCommandConnect.GetTokenDataAsArray(command);
					byte[] uniqueIdAsArray = NetCommandConnect.GetUniqueIdAsArray(command);
					switch (cb.OnConnectionRequest(b->Address, tokenDataAsArray, uniqueIdAsArray))
					{
					case OnConnectionRequestReply.Ok:
					{
						NetConnection* ptr = NetPeerGroup.AllocateConnection(g, b->Address, tokenDataAsArray, uniqueIdAsArray);
						bool flag3 = ptr == null;
						if (!flag3)
						{
							NetPeerGroup.HandlePacketCommand(g, cb, ptr, b);
						}
						break;
					}
					case OnConnectionRequestReply.Refuse:
					{
						bool flag4 = !NetPeerGroup.SendCommandUnconnected<NetCommandRefused>(g, b->Address, NetCommandRefused.Create(NetConnectFailedReason.ServerRefused));
						if (flag4)
						{
							DebugLogStream logDebug2 = InternalLogStreams.LogDebug;
							if (logDebug2 != null)
							{
								logDebug2.Error("Sending Refused Connection Failed");
							}
						}
						break;
					}
					}
				}
			}
			else
			{
				bool flag5 = b->PacketType == NetPacketType.Unconnected;
				if (flag5)
				{
					cb.OnUnconnectedData(b);
				}
			}
		}

		public unsafe static double GetConnectionIdleTime(NetPeerGroup* g, NetConnection* c)
		{
			return g->Time - c->RecvTime;
		}

		private unsafe static void HandlePacket(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			c->RecvTime = g->_clock.ElapsedInSeconds;
			switch (b->PacketType)
			{
			case NetPacketType.Command:
			{
				bool flag = c->Status == NetConnectionStatus.Connecting || c->Status == NetConnectionStatus.Connected;
				if (flag)
				{
					NetPeerGroup.HandlePacketCommand(g, cb, c, b);
				}
				return;
			}
			case NetPacketType.UnreliableData:
			{
				bool flag2 = c->Status == NetConnectionStatus.Connected;
				if (flag2)
				{
					NetPeerGroup.HandlePacketUnreliableData(g, cb, c, b);
				}
				return;
			}
			case NetPacketType.NotifyData:
			case NetPacketType.NotifyReliableData:
			{
				bool flag3 = c->Status == NetConnectionStatus.Connected;
				if (flag3)
				{
					NetPeerGroup.HandlePacketNotifyData(g, cb, c, b);
				}
				return;
			}
			case NetPacketType.NotifyAcks:
			{
				bool flag4 = c->Status == NetConnectionStatus.Connected;
				if (flag4)
				{
					NetPeerGroup.HandlePacketNotifyAcks(g, cb, c, b);
				}
				return;
			}
			case NetPacketType.Unconnected:
				return;
			}
			LogStream logError = InternalLogStreams.LogError;
			if (logError != null)
			{
				logError.Log(string.Format("Invalid Packet Type {0}", b->PacketType));
			}
		}

		private unsafe static void HandlePacketNotifyAcks(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			bool flag = b->LengthBytes < 14;
			if (!flag)
			{
				c->NotifyRecvTime = g->_clock.ElapsedInSeconds;
				NetPeerGroup.HandlePacketAcks(g, cb, c, *(NetNotifyHeader*)b->Data);
			}
		}

		private unsafe static void HandlePacketNotifyData(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			bool flag = b->LengthBytes <= 14;
			if (!flag)
			{
				NetNotifyHeader netNotifyHeader = default(NetNotifyHeader);
				Native.MemCpy((void*)(&netNotifyHeader), (void*)b->Data, 14);
				int num = c->NotifySendSequencer.Distance((ulong)netNotifyHeader.Sequence, (ulong)c->NotifyRecvSequence);
				bool flag2 = num > g->_config.Notify.SequenceBounds || num < -g->_config.Notify.SequenceBounds;
				if (flag2)
				{
					NetPeerGroup.DisconnectInternal(g, c, NetDisconnectReason.SequenceOutOfBounds, null);
				}
				else
				{
					bool flag3 = num < 0;
					if (!flag3)
					{
						bool flag4 = netNotifyHeader.Fragment > 0;
						if (flag4)
						{
							bool flag5 = num == 0;
							if (!flag5)
							{
								Assert.Check(b->PacketType == NetPacketType.NotifyData);
								int num2 = (int)netNotifyHeader.Fragment & -129;
								bool flag6 = num2 == 1;
								if (flag6)
								{
									TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
									if (logTraceNetwork != null)
									{
										logTraceNetwork.Log("frag-recv-start");
									}
									TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
									if (logTraceNetwork2 != null)
									{
										logTraceNetwork2.Log(string.Format("frag-recv:{0} seq:{1} size:{2}", num2, netNotifyHeader.Sequence, b->LengthBytes));
									}
									c->NotifyRecvFragment = num2;
									c->NotifyRecvFragmentBufferLength = 0;
									c->NotifyRecvFragmentSequenceDistance = num;
									Native.MemCpy((void*)c->NotifyRecvFragmentBuffer, (void*)b->Data, b->LengthBytes);
									c->NotifyRecvFragmentBufferLength = b->LengthBytes;
								}
								else
								{
									bool flag7 = num2 > 1 && num == c->NotifyRecvFragmentSequenceDistance && c->NotifyRecvFragment + 1 == num2;
									if (flag7)
									{
										TraceLogStream logTraceNetwork3 = InternalLogStreams.LogTraceNetwork;
										if (logTraceNetwork3 != null)
										{
											logTraceNetwork3.Log(string.Format("frag-recv:{0} seq:{1} size:{2}", num2, netNotifyHeader.Sequence, b->LengthBytes));
										}
										c->NotifyRecvFragment = num2;
										int num3 = b->LengthBytes - 14;
										bool flag8 = c->NotifyRecvFragmentBufferLength + num3 > 51200;
										if (flag8)
										{
											LogStream logError = InternalLogStreams.LogError;
											if (logError != null)
											{
												logError.Log("Fragment buffer overflow");
											}
											c->NotifyRecvFragment = 0;
											c->NotifyRecvFragmentBufferLength = 0;
											c->NotifyRecvFragmentSequenceDistance = 0;
										}
										else
										{
											Native.MemCpy((void*)(c->NotifyRecvFragmentBuffer + c->NotifyRecvFragmentBufferLength), (void*)((byte*)b->Data + 14), num3);
											c->NotifyRecvFragmentBufferLength = c->NotifyRecvFragmentBufferLength + num3;
											bool flag9 = (netNotifyHeader.Fragment & 128) == 128;
											if (flag9)
											{
												NetPeerGroup.HandlePacketNotifyData_Part2(netNotifyHeader, num, g, cb, c, b);
												TraceLogStream logTraceNetwork4 = InternalLogStreams.LogTraceNetwork;
												if (logTraceNetwork4 != null)
												{
													logTraceNetwork4.Log("frag-reassembled");
												}
												NetBitBuffer netBitBuffer = default(NetBitBuffer);
												netBitBuffer.LengthBytes = c->NotifyRecvFragmentBufferLength;
												netBitBuffer.OffsetBits = 112;
												netBitBuffer.Data = (ulong*)c->NotifyRecvFragmentBuffer;
												netBitBuffer.Address = b->Address;
												netBitBuffer.Group = (short)g->Group;
												cb.OnNotifyData(c, &netBitBuffer);
												c->NotifyRecvFragment = 0;
												c->NotifyRecvFragmentBufferLength = 0;
												c->NotifyRecvFragmentSequenceDistance = 0;
											}
										}
									}
									else
									{
										c->NotifyRecvFragment = 0;
										c->NotifyRecvFragmentBufferLength = 0;
										c->NotifyRecvFragmentSequenceDistance = 0;
									}
								}
							}
						}
						else
						{
							bool flag10 = num <= 0;
							if (!flag10)
							{
								c->NotifyRecvFragment = 0;
								c->NotifyRecvFragmentBufferLength = 0;
								c->NotifyRecvFragmentSequenceDistance = 0;
								NetPeerGroup.HandlePacketNotifyData_Part2(netNotifyHeader, num, g, cb, c, b);
								bool flag11 = b->PacketType == NetPacketType.NotifyReliableData;
								if (flag11)
								{
									ReliableId id;
									bool flag12 = c->ReliableBuffer.Receive(b, out id);
									if (flag12)
									{
										byte* dataPointer = b->GetDataPointer();
										cb.OnReliableData(c, id, dataPointer);
										for (;;)
										{
											void* ptr;
											bool flag13 = c->ReliableBuffer.LateReceive(out ptr, out id, out dataPointer);
											if (!flag13)
											{
												break;
											}
											cb.OnReliableData(c, id, dataPointer);
											c->ReliableBuffer.LateFree(ref ptr);
										}
									}
								}
								else
								{
									cb.OnNotifyData(c, b);
								}
							}
						}
					}
				}
			}
		}

		private unsafe static void HandlePacketNotifyData_Part2(NetNotifyHeader header, int sequenceDistance, NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			c->NotifyRecvSequence = header.Sequence;
			bool flag = sequenceDistance >= g->_config.Notify.AckMaskBits;
			if (flag)
			{
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Warn("Huge loss. Clear Ack Mask.");
				}
				c->NotifyRecvMask = 1UL;
			}
			else
			{
				c->NotifyRecvMask = (c->NotifyRecvMask << sequenceDistance | 1UL);
			}
			c->NotifyRecvTime = g->_clock.ElapsedInSeconds;
			c->NotifyRecvUnackedCount = c->NotifyRecvUnackedCount + 1;
			NetPeerGroup.HandlePacketAcks(g, cb, c, header);
			b->OffsetBits = 112;
		}

		private unsafe static void HandlePacketAcks(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetNotifyHeader h)
		{
			int num = 0;
			while (c->NotifySendWindow.Count > 0)
			{
				NetSendEnvelope netSendEnvelope = c->NotifySendWindow.Peek();
				int num2 = c->NotifySendSequencer.Distance((ulong)netSendEnvelope.Sequence, (ulong)h.AckSequence);
				bool flag = num2 > 0;
				if (flag)
				{
					break;
				}
				bool flag2 = num2 == 0;
				if (flag2)
				{
					c->Rtt = Math.Max(0.0, g->_clock.ElapsedInSeconds - netSendEnvelope.SendTime);
				}
				num++;
				c->NotifyRecvAckTime = g->_clock.ElapsedInSeconds;
				c->NotifySendWindow.Pop();
				bool flag3 = num2 <= -g->_config.Notify.AckMaskBits || (h.AckMask & 1UL << -num2) == 0UL;
				if (flag3)
				{
					bool flag4 = netSendEnvelope.PacketType == NetPacketType.NotifyReliableData;
					if (flag4)
					{
						ReliableHeader* item = netSendEnvelope.TakeUserData<ReliableHeader>();
						c->ReliableSendList.AddFirst(item);
					}
					else
					{
						cb.OnNotifyLost(c, ref netSendEnvelope);
					}
				}
				else
				{
					bool flag5 = netSendEnvelope.PacketType == NetPacketType.NotifyReliableData;
					if (flag5)
					{
						ReliableHeader* ptr = netSendEnvelope.TakeUserData<ReliableHeader>();
						Native.Free<ReliableHeader>(ref ptr);
					}
					else
					{
						cb.OnNotifyDelivered(c, ref netSendEnvelope);
					}
				}
				Assert.Always<IntPtr>(netSendEnvelope.UserData == null, (IntPtr)netSendEnvelope.UserData);
			}
			bool flag6 = num == 0;
			if (flag6)
			{
				c->NotifyRecvAckOutdatedCount = c->NotifyRecvAckOutdatedCount + 1;
			}
		}

		private unsafe static void HandlePacketUnreliableData(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			b->OffsetBits = 8;
			cb.OnUnreliableData(c, b);
		}

		private unsafe static void HandlePacketCommand(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetBitBuffer* b)
		{
			NetCommandHeader* data = (NetCommandHeader*)b->Data;
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("command {0} <= {1}", c->Address, data->Command));
			}
			switch (data->Command)
			{
			case NetCommands.Connect:
				NetPeerGroup.HandleCommandConnect(g, cb, c, *(NetCommandConnect*)b->Data);
				break;
			case NetCommands.Accepted:
				NetPeerGroup.HandleCommandAccepted(g, cb, c, *(NetCommandAccepted*)b->Data);
				break;
			case NetCommands.Refused:
				NetPeerGroup.HandleCommandRefused(g, cb, c, *(NetCommandRefused*)b->Data);
				break;
			case NetCommands.Disconnect:
				NetPeerGroup.HandleCommandDisconnect(g, cb, c, *(NetCommandDisconnect*)b->Data);
				break;
			default:
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(string.Format("Invalid Command Type {0}", data->Command));
				}
				break;
			}
			}
		}

		private unsafe static void HandleCommandRefused(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetCommandRefused cmd)
		{
			Assert.Check(c->Status == NetConnectionStatus.Connecting);
			try
			{
				cb.OnConnectionFailed(c->Address, cmd.Reason);
			}
			catch (Exception error)
			{
				LogStream logException = InternalLogStreams.LogException;
				if (logException != null)
				{
					logException.Log(error);
				}
			}
			NetPeerGroup.ChangeConnectionStatus(g, cb, c, NetConnectionStatus.Shutdown);
		}

		private unsafe static void HandleCommandDisconnect(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetCommandDisconnect cmd)
		{
			bool flag = c->Status != NetConnectionStatus.Connected;
			if (flag)
			{
				LogStream logError = InternalLogStreams.LogError;
				if (logError != null)
				{
					logError.Log(string.Format("received {0} with connection status {1}", "NetCommandDisconnect", c->Status));
				}
			}
			else
			{
				NetPeerGroup.DisconnectInternal(g, c, cmd.Reason, null);
			}
		}

		private unsafe static void HandleCommandConnect(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetCommandConnect cmd)
		{
			NetConnectionStatus status = c->Status;
			NetConnectionStatus netConnectionStatus = status;
			if (netConnectionStatus != NetConnectionStatus.Created)
			{
				if (netConnectionStatus != NetConnectionStatus.Connected)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log(string.Format("received {0} with connection status {1}", "NetCommandConnect", c->Status));
					}
				}
				else
				{
					NetPeerGroup.SendCommand<NetCommandAccepted>(g, c, NetCommandAccepted.Create(c->LocalId, c->RemoteId, c->Counter));
				}
			}
			else
			{
				c->RemoteId = cmd.ConnectionId;
				uint counter = g->_counter + 1U;
				g->_counter = counter;
				c->Counter = counter;
				NetPeerGroup.ChangeConnectionStatus(g, cb, c, NetConnectionStatus.Connected);
				NetPeerGroup.SendCommand<NetCommandAccepted>(g, c, NetCommandAccepted.Create(c->LocalId, c->RemoteId, c->Counter));
				cb.OnConnected(c);
			}
		}

		private unsafe static void HandleCommandAccepted(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetCommandAccepted cmd)
		{
			NetConnectionStatus status = c->Status;
			NetConnectionStatus netConnectionStatus = status;
			if (netConnectionStatus != NetConnectionStatus.Connecting)
			{
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Error(string.Format("received {0} with connection status {1}", "NetCommandAccepted", c->Status));
				}
			}
			else
			{
				Assert.Check(c->LocalId.Equals(cmd.AcceptedRemoteId));
				c->RemoteId = cmd.AcceptedLocalId;
				c->Counter = cmd.Counter;
				NetPeerGroup.ChangeConnectionStatus(g, cb, c, NetConnectionStatus.Connected);
				cb.OnConnected(c);
			}
		}

		private unsafe static bool SendCommand<[IsUnmanaged] T>(NetPeerGroup* g, NetConnection* c, T cmd) where T : struct, ValueType
		{
			NetBitBuffer* ptr;
			bool connectionSendBuffer = NetPeerGroup.GetConnectionSendBuffer(g, c, out ptr);
			bool result;
			if (connectionSendBuffer)
			{
				*(T*)ptr->Data = cmd;
				ptr->OffsetBits = Maths.SizeOfBits<T>();
				NetPeerGroup.Send(g, c, ptr);
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Log(string.Format("command {0} => {1}", c->Address, ((NetCommandHeader*)ptr->Data)->Command));
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private unsafe static bool SendCommandUnconnected<[IsUnmanaged] T>(NetPeerGroup* g, NetAddress address, T cmd) where T : struct, ValueType
		{
			NetBitBuffer* ptr;
			bool flag = g->_sendBlock->TryAcquire(out ptr);
			bool result;
			if (flag)
			{
				*(T*)ptr->Data = cmd;
				ptr->Group = g->_group;
				ptr->Address = address;
				ptr->OffsetBits = Maths.SizeOfBits<T>();
				NetPeerGroup.SendUnconnected(g, ptr);
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Log(string.Format("command {0} => {1}", address, ((NetCommandHeader*)ptr->Data)->Command));
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private unsafe static void QueueAddressUnmap(NetPeerGroup* g, NetConnection* c)
		{
			Assert.Check(c->Status == NetConnectionStatus.Shutdown);
			Assert.Check(c->StateShutdown.Unmapped == 0);
			NetBitBuffer* ptr;
			bool flag = c->StateShutdown.Unmapped == 0 && NetPeerGroup.GetConnectionSendBuffer(g, c, out ptr);
			if (flag)
			{
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Log(string.Format("Sending Unmap For: {0}", c->Address));
				}
				ptr->Group = -1;
				ptr->OffsetBits = 0;
				NetPeerGroup.Send(g, c, ptr);
				c->StateShutdown.Unmapped = 1;
			}
		}

		private unsafe static void ChangeConnectionStatus(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c, NetConnectionStatus status)
		{
			bool flag = c->Status != status;
			if (flag)
			{
				TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
				if (logTraceNetwork != null)
				{
					logTraceNetwork.Log(string.Format("{0} status changed from {1} to {2}", c->Address, c->Status, status));
				}
				c->Status = status;
				if (status == NetConnectionStatus.Shutdown)
				{
					c->StateShutdown.Unmapped = 0;
					c->StateShutdown.Timeout = g->_clock.ElapsedInSeconds + g->_config.ConnectionShutdownTime;
					while (c->NotifySendWindow.Count > 0)
					{
						NetSendEnvelope netSendEnvelope = c->NotifySendWindow.Peek();
						c->NotifySendWindow.Pop();
						cb.OnNotifyDispose(ref netSendEnvelope);
						Assert.Always<IntPtr>(netSendEnvelope.UserData == null, (IntPtr)netSendEnvelope.UserData);
					}
					NetPeerGroup.QueueAddressUnmap(g, c);
				}
			}
		}

		private unsafe static void ReleaseConnection(NetPeerGroup* g, INetPeerGroupCallbacks cb, NetConnection* c)
		{
			Assert.Check(g->_connectionsMap->Find(c->Address) == c);
			g->_connectionsMap->Remove(c->Address);
		}

		private unsafe static NetConnection* AllocateConnection(NetPeerGroup* g, NetAddress address, byte[] token, byte[] uniqueId)
		{
			Assert.Check(uniqueId != null, "UniqueId is required");
			NetConnection* ptr = g->_connectionsMap->Insert(address, uniqueId);
			bool flag = ptr == null;
			NetConnection* result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Assert.Check(ptr->RecvTime == 0.0, "c->RecvTime == 0");
				Assert.Check(ptr->SendTime == 0.0, "c->SendTime == 0");
				Assert.Check(ptr->Rtt == 0.0, "c->Rtt == 0");
				Assert.Check(ptr->NotifySendSequencer.Sequence == 0UL, "c->NotifySendSequencer.Sequence == 0");
				Assert.Check(ptr->NotifySendWindow.Head == 0, "c->NotifySendWindow.Head == 0");
				Assert.Check(ptr->NotifySendWindow.Tail == 0, "c->NotifySendWindow.Tail == 0");
				Assert.Check(ptr->NotifySendWindow.Count == 0, "c->NotifySendWindow.Count == 0");
				Assert.Check(ptr->NotifyRecvTime == 0.0, "c->NotifyRecvTime == 0");
				Assert.Check(ptr->NotifyRecvMask == 0UL, "c->NotifyRecvMask == 0");
				Assert.Check(ptr->NotifyRecvSequence == 0, "c->NotifyRecvSequence == 0");
				Assert.Check(ptr->NotifyRecvUnackedCount == 0, "c->NotifyRecvUnackedCount == 0");
				ptr->RecvTime = g->_clock.ElapsedInSeconds;
				ptr->SendTime = ptr->RecvTime;
				ptr->Rtt = g->_config.ConnectionDefaultRtt;
				ptr->Status = NetConnectionStatus.Created;
				bool flag2 = token != null;
				if (flag2)
				{
					ptr->ConnectionTokenLength = NetCommandConnect.ClampTokenLength(token.Length);
					ptr->ConnectionToken = (byte*)Native.MallocAndClear(token.Length);
					fixed (byte[] array = token)
					{
						byte* source;
						if (token == null || array.Length == 0)
						{
							source = null;
						}
						else
						{
							source = &array[0];
						}
						Native.MemCpy((void*)ptr->ConnectionToken, (void*)source, ptr->ConnectionTokenLength);
					}
				}
				result = ptr;
			}
			return result;
		}

		private const double RELIABLE_SEND_INTERVAL = 0.05;

		private unsafe NetPeer* _peer;

		private short _group;

		private Timer _clock;

		private NetConfig _config;

		private uint _counter;

		private IntPtr _sendHead;

		private IntPtr _recvHead;

		private NetBitBufferStack _recvStack;

		private unsafe NetBitBufferBlock* _sendBlock;

		private unsafe NetConnectionMap* _connectionsMap;

		internal double ReliableSendInterval;
	}
}
