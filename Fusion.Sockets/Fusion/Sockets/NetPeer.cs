using System;
using System.Runtime.CompilerServices;

namespace Fusion.Sockets
{
	public struct NetPeer
	{
		public NetAddress Address
		{
			get
			{
				return this._address;
			}
		}

		public NetConfig Config
		{
			get
			{
				return this._config;
			}
		}

		public int GroupCount
		{
			get
			{
				return this._config.ConnectionGroups;
			}
		}

		public bool IsShutdown
		{
			get
			{
				return this._state == 2;
			}
		}

		public unsafe static NetConfig* GetConfigPointer(NetPeer* p)
		{
			bool flag = p->_state == 2;
			NetConfig* result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = &p->_config;
			}
			return result;
		}

		public unsafe static NetPeerGroup* GetGroup(NetPeer* p, int index)
		{
			bool flag = p->_state == 2;
			NetPeerGroup* result;
			if (flag)
			{
				result = null;
			}
			else
			{
				Assert.Check(index < p->_config.ConnectionGroups);
				result = p->_groups + index;
			}
			return result;
		}

		public unsafe static void Update(NetPeer* p, INetSocket socket, Random rng)
		{
			bool flag = false;
			NetPeer.Update(p, socket, &flag, rng);
		}

		public unsafe static void Update(NetPeer* p, INetSocket socket, bool* work, Random rng)
		{
			bool flag = p->_state == 2;
			if (!flag)
			{
				bool flag2 = p->_state != 0;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Can't call Update on NetPeer which is running or has been running on a thread");
					}
				}
				else
				{
					NetPeer.RecvInternal(p, socket, work, rng);
					NetPeer.SendInternal(p, socket, work);
				}
			}
		}

		public unsafe static void Recv(NetPeer* p, INetSocket socket, Random rng)
		{
			bool flag = false;
			NetPeer.Recv(p, socket, &flag, rng);
		}

		public unsafe static void Recv(NetPeer* p, INetSocket socket, bool* work, Random rng)
		{
			bool flag = p->_state == 2;
			if (!flag)
			{
				bool flag2 = p->_state != 0;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Can't call Update on NetPeer which is running or has been running on a thread");
					}
				}
				else
				{
					NetPeer.RecvInternal(p, socket, work, rng);
				}
			}
		}

		public unsafe static void RemapAddress(NetPeer* p, NetAddress oldAddress, NetAddress newAddress)
		{
			int num = p->_groupsMap->Remove(oldAddress);
			Assert.Check(num >= 0);
			p->_groupsMap->Insert(newAddress, 0);
		}

		public unsafe static void Send(NetPeer* p, INetSocket socket)
		{
			bool flag = p->_state == 2;
			if (!flag)
			{
				bool flag2 = false;
				NetPeer.Send(p, socket, &flag2);
			}
		}

		public unsafe static void Send(NetPeer* p, INetSocket socket, bool* work)
		{
			bool flag = p->_state == 2;
			if (!flag)
			{
				bool flag2 = p->_state != 0;
				if (flag2)
				{
					LogStream logError = InternalLogStreams.LogError;
					if (logError != null)
					{
						logError.Log("Can't call Update on NetPeer which is running or has been running on a thread");
					}
				}
				else
				{
					NetPeer.SendInternal(p, socket, work);
				}
			}
		}

		public unsafe static NetPeer* Initialize(NetConfig config, INetSocket socket)
		{
			NetPeer* ptr = Native.MallocAndClear<NetPeer>();
			NetPeer.Initialize(ptr, config, socket);
			return ptr;
		}

		public unsafe static void Initialize(NetPeer* p, NetConfig config, INetSocket socket)
		{
			config.MaxConnections = Maths.Clamp(config.MaxConnections, 1, 2048);
			socket.Initialize(config);
			p->_config = config;
			p->_state = 0;
			p->_recvTimer = default(Timer);
			p->_fragmentBuffer = (byte*)Native.MallocAndClear(1280);
			p->_refusedCommand = Native.MallocAndClear<NetCommandRefused>();
			p->_delayedClock = Timer.StartNew();
			p->_delayedPackets = default(NetDelayedPacketList);
			p->_sendStack = NetBitBufferStack.Create(2048);
			p->_recvBlock = NetBitBufferBlock.Create(config.PacketSize);
			p->_socket = socket.Create(config);
			p->_groupsMap = NetPeerGroupMap.Allocate(config.MaxConnections);
			p->_groups = Native.MallocAndClearArray<NetPeerGroup>(config.ConnectionGroups);
			p->_groupsAssigned = Native.MallocAndClearArray<int>(config.ConnectionGroups);
			short num = 0;
			while ((int)num < config.ConnectionGroups)
			{
				NetPeerGroup.Initialize(num, p->_groups + num, p, config);
				num += 1;
			}
			p->_address = socket.Bind(p->_socket, p->_config);
			TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
			if (logTraceNetwork != null)
			{
				logTraceNetwork.Log(string.Format("socket bound to {0}", p->_address));
			}
		}

		public unsafe static void Destroy(NetPeer* p, INetSocket socket, INetPeerGroupCallbacks callbacks)
		{
			bool flag = p->_state == 0;
			if (flag)
			{
				p->_state = 2;
				NetPeer.DestroySocket(p, socket, callbacks);
			}
		}

		private unsafe static void DestroySocket(NetPeer* p, INetSocket socket, INetPeerGroupCallbacks callbacks)
		{
			bool flag = p != null && p->_socket.IsCreated;
			if (flag)
			{
				NetBitBufferStack.Dispose(ref p->_sendStack);
				p->_delayedPackets.Dispose();
				for (int i = 0; i < p->GroupCount; i++)
				{
					NetPeerGroup.Dispose(p->_groups + i, callbacks);
				}
				NetBitBuffer.ReleaseRef(ref p->_recv);
				NetBitBufferBlock.Dispose(ref p->_recvBlock);
				NetPeerGroupMap.Dispose(ref p->_groupsMap);
				Native.Free<int>(ref p->_groupsAssigned);
				Native.Free<NetCommandRefused>(ref p->_refusedCommand);
				Native.Free<byte>(ref p->_fragmentBuffer);
				Native.Free<NetPeerGroup>(ref p->_groups);
				socket.Destroy(p->_socket);
				p->_socket = default(NetSocket);
				Native.Free<NetPeer>(ref p);
			}
		}

		private unsafe static short FindGroupWithLeastAssignedAddresses(NetPeer* p)
		{
			short result = -1;
			int num = p->_config.ConnectionsPerGroup;
			short num2 = 0;
			while ((int)num2 < p->_config.ConnectionGroups)
			{
				bool flag = p->_groupsAssigned[num2] < num;
				if (flag)
				{
					result = num2;
					num = p->_groupsAssigned[num2];
				}
				num2 += 1;
			}
			return result;
		}

		private unsafe static void RecvInternal(NetPeer* p, INetSocket socket, bool* work, Random rng)
		{
			p->_recvTimer.Restart();
			NetPeer.RecvDelayed(p, socket, work, rng);
			bool flag = NetPeer.RecvExpired(p);
			if (!flag)
			{
				int num;
				while (NetPeer.RecvBufferAvailable(p) && (num = socket.Receive(p->_socket, &p->_recv->Address, (byte*)p->_recv->Data, p->_config.PacketSize)) > 0)
				{
					TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork != null)
					{
						logTraceNetwork.Log(string.Format("recv {0} <= {1} bytes", p->_recv->Address, num));
					}
					*work = true;
					p->_recv->LengthBytes = num;
					bool flag2 = p->_config.Simulation.LossNotifySequencesLength > 0 && p->_recv->PacketType == NetPacketType.NotifyData;
					if (flag2)
					{
						Assert.Check(p->_config.Simulation.LossNotifySequences != null);
						ushort sequence = ((NetNotifyHeader*)p->_recv->Data)->Sequence;
						bool flag3 = false;
						for (int i = 0; i < p->_config.Simulation.LossNotifySequencesLength; i++)
						{
							bool flag4 = p->_config.Simulation.LossNotifySequences[i] == (short)sequence;
							if (flag4)
							{
								flag3 = true;
								break;
							}
						}
						bool flag5 = flag3;
						if (flag5)
						{
							continue;
						}
					}
					NetConfigSimulationOscillator lossOscillator = p->_config.Simulation.LossOscillator;
					bool flag6 = lossOscillator.Min > 0.0 && lossOscillator.Min <= lossOscillator.Max;
					if (flag6)
					{
						double curveValue = lossOscillator.GetCurveValue(rng, p->_delayedClock.ElapsedInSeconds);
						bool flag7 = rng.NextDouble() <= curveValue;
						if (flag7)
						{
							continue;
						}
					}
					NetConfigSimulationOscillator delayOscillator = p->_config.Simulation.DelayOscillator;
					bool flag8 = delayOscillator.Min > 0.0 && delayOscillator.Min <= delayOscillator.Max;
					if (flag8)
					{
						NetDelayedPacket* ptr = NetDelayedPacket.Create(p->_recv->LengthBytes);
						Native.MemCpy((void*)ptr->Data, (void*)p->_recv->Data, p->_recv->LengthBytes);
						ptr->Address = p->_recv->Address;
						double curveValue2 = delayOscillator.GetCurveValue(rng, p->_delayedClock.ElapsedInSeconds);
						ptr->DeliveryTime = p->_delayedClock.ElapsedInSeconds + curveValue2;
						bool flag9 = curveValue2 > 0.0;
						if (flag9)
						{
							p->_delayedPackets.AddLast(ptr);
							continue;
						}
					}
					NetPeer.RecvBufferPushToGroup(p, socket, rng);
					bool flag10 = NetPeer.RecvExpired(p);
					if (flag10)
					{
						break;
					}
				}
			}
		}

		private unsafe static void RecvBufferPushToGroup(NetPeer* p, INetSocket socket, Random rng)
		{
			Assert.Check(p->_recv != null);
			Assert.Check(!p->_recv->Address.Equals(default(NetAddress)));
			short num = p->_groupsMap->Find(p->_recv->Address);
			bool flag = num == -1;
			if (flag)
			{
				NetCommandHeader netCommandHeader = *(NetCommandHeader*)p->_recv->Data;
				bool flag2 = netCommandHeader.PacketType == NetPacketType.Command && netCommandHeader.Command == NetCommands.Connect;
				if (!flag2)
				{
					return;
				}
				num = NetPeer.FindGroupWithLeastAssignedAddresses(p);
				bool flag3 = num == -1;
				if (flag3)
				{
					*p->_refusedCommand = NetCommandRefused.Create(NetConnectFailedReason.ServerFull);
					socket.Send(p->_socket, &p->_recv->Address, (byte*)p->_refusedCommand, 3, false);
					return;
				}
				Assert.Check(p->_groupsAssigned[num] >= 0 && p->_groupsAssigned[num] < p->_config.ConnectionsPerGroup);
				bool flag4 = p->_groupsMap->Insert(p->_recv->Address, num);
				if (!flag4)
				{
					return;
				}
				p->_groupsAssigned[num]++;
			}
			Assert.Check(num >= 0 && (int)num <= p->_config.ConnectionGroups);
			bool flag5 = p->_config.Simulation.DuplicateChance > 0.0 && rng.NextDouble() <= p->_config.Simulation.DuplicateChance;
			if (flag5)
			{
				NetBitBuffer* ptr;
				bool flag6 = p->_recvBlock->TryAcquire(out ptr);
				if (flag6)
				{
					ptr->Address = p->_recv->Address;
					ptr->LengthBytes = p->_recv->LengthBytes;
					Native.MemCpy((void*)ptr->Data, (void*)p->_recv->Data, p->_recv->LengthBytes);
					NetPeerGroup.PushOnRecvHead(p->_groups + num, ptr);
				}
			}
			NetPeerGroup.PushOnRecvHead(p->_groups + num, p->_recv);
			p->_recv = null;
		}

		private unsafe static void RecvDelayed(NetPeer* p, INetSocket socket, bool* work, Random rng)
		{
			while (p->_delayedPackets.Count > 0 && p->_delayedPackets.Head->DeliveryTime < p->_delayedClock.ElapsedInSeconds && NetPeer.RecvBufferAvailable(p))
			{
				bool flag = NetPeer.RecvExpired(p);
				if (flag)
				{
					break;
				}
				*work = true;
				NetDelayedPacket* ptr = p->_delayedPackets.RemoveHead();
				Native.MemCpy((void*)p->_recv->Data, (void*)ptr->Data, ptr->DataLength);
				p->_recv->Address = ptr->Address;
				p->_recv->LengthBytes = ptr->DataLength;
				NetPeer.RecvBufferPushToGroup(p, socket, rng);
				Native.Free<NetDelayedPacket>(ref ptr);
			}
		}

		private unsafe static void SendInternal(NetPeer* p, INetSocket socket, bool* work)
		{
			NetPeer.SendFromStack(p, socket, work);
			Assert.Check(p->_sendStack.Count == 0);
			for (int i = 0; i < p->_config.ConnectionGroups; i++)
			{
				IntPtr intPtr = NetPeerGroup.PopSendHead(p->_groups + i);
				bool flag = intPtr == IntPtr.Zero;
				if (!flag)
				{
					*work = true;
					p->_sendStack.PushFromHead((NetBitBuffer*)((void*)intPtr));
				}
			}
			NetPeer.SendFromStack(p, socket, work);
		}

		private unsafe static void SendFromStack(NetPeer* p, INetSocket socket, bool* work)
		{
			NetBitBuffer* ptr = null;
			while (p->_sendStack.TryPop(&ptr))
			{
				*work = true;
				Assert.Check(!ptr->Address.Equals(default(NetAddress)));
				bool flag = ptr->PacketType == NetPacketType.Command;
				if (flag)
				{
					NetCommandHeader* data = (NetCommandHeader*)ptr->Data;
					bool flag2 = data->Command == NetCommands.Connect;
					if (flag2)
					{
						short num = p->_groupsMap->Find(ptr->Address);
						bool flag3 = num == -1;
						if (flag3)
						{
							bool flag4 = p->_groupsMap->Insert(ptr->Address, ptr->Group);
							if (!flag4)
							{
								NetBitBuffer.Release(ptr);
								continue;
							}
							p->_groupsAssigned[ptr->Group]++;
						}
					}
				}
				bool flag5 = ptr->PacketType != NetPacketType.Unconnected;
				if (flag5)
				{
					Assert.Check((int)p->_groupsMap->Find(ptr->Address) < p->_config.ConnectionGroups);
				}
				bool flag6 = ptr->Group == -1;
				if (flag6)
				{
					Assert.Check(ptr->OffsetBits == 0);
					int num2 = p->_groupsMap->Remove(ptr->Address);
					socket.DeleteEncryptionKey(ptr->Address);
					TraceLogStream logTraceNetwork = InternalLogStreams.LogTraceNetwork;
					if (logTraceNetwork != null)
					{
						logTraceNetwork.Log(string.Format("{0} unmapped from {1}", ptr->Address, num2));
					}
					Assert.Check(num2 < p->_config.ConnectionGroups);
					p->_groupsAssigned[num2]--;
					Assert.Check(p->_groupsAssigned[num2] >= 0);
					NetBitBuffer.Release(ptr);
				}
				else
				{
					int num3 = Maths.BytesRequiredForBits(ptr->OffsetBits);
					bool flag7 = ptr->PacketType == NetPacketType.NotifyData && num3 > 1280;
					if (flag7)
					{
						TraceLogStream logTraceNetwork2 = InternalLogStreams.LogTraceNetwork;
						if (logTraceNetwork2 != null)
						{
							logTraceNetwork2.Log(string.Format("send {0} => {1} bytes [FRAGMENTED, MTU:{2}]", ptr->Address, num3, 1280));
						}
						NetNotifyHeader netNotifyHeader = default(NetNotifyHeader);
						Native.MemCpy((void*)(&netNotifyHeader), (void*)ptr->Data, 14);
						byte* ptr2 = (byte*)ptr->Data + 14;
						int i = num3 - 14;
						byte b = 1;
						TraceLogStream logTraceNetwork3 = InternalLogStreams.LogTraceNetwork;
						if (logTraceNetwork3 != null)
						{
							logTraceNetwork3.Log("frag-send-start");
						}
						while (i > 0)
						{
							Assert.Check<int, int>(b >= 1 && b <= 40, "Max amount of fragments reached {0}, remaining data: {1}", 40, i);
							int num4 = Math.Min(1122, i);
							i -= num4;
							Assert.Check(i >= 0);
							netNotifyHeader.Fragment = b;
							bool flag8 = i == 0;
							if (flag8)
							{
								netNotifyHeader.Fragment |= 128;
							}
							TraceLogStream logTraceNetwork4 = InternalLogStreams.LogTraceNetwork;
							if (logTraceNetwork4 != null)
							{
								logTraceNetwork4.Log(string.Format("frag-send:{0} seq:{1} size:{2} last:{3}", new object[]
								{
									b,
									netNotifyHeader.Sequence,
									num4,
									(netNotifyHeader.Fragment & 128) == 128
								}));
							}
							Native.MemCpy((void*)p->_fragmentBuffer, (void*)(&netNotifyHeader), 14);
							Native.MemCpy((void*)(p->_fragmentBuffer + 14), (void*)ptr2, num4);
							ptr2 += num4;
							b += 1;
							socket.Send(p->_socket, &ptr->Address, p->_fragmentBuffer, num4 + 14, true);
						}
						TraceLogStream logTraceNetwork5 = InternalLogStreams.LogTraceNetwork;
						if (logTraceNetwork5 != null)
						{
							logTraceNetwork5.Log("frag-send-end");
						}
					}
					else
					{
						socket.Send(p->_socket, &ptr->Address, (byte*)ptr->Data, num3, false);
					}
					bool flag9 = ptr->PacketType == NetPacketType.Command;
					if (flag9)
					{
						NetCommandHeader* data2 = (NetCommandHeader*)ptr->Data;
						bool flag10 = data2->Command == NetCommands.Refused;
						if (flag10)
						{
							bool flag11 = p->_groupsMap->Find(ptr->Address) != -1;
							if (flag11)
							{
								int num5 = p->_groupsMap->Remove(ptr->Address);
								TraceLogStream logTraceNetwork6 = InternalLogStreams.LogTraceNetwork;
								if (logTraceNetwork6 != null)
								{
									logTraceNetwork6.Log(string.Format("{0} unmapped from {1} because it was refused.", ptr->Address, num5));
								}
								Assert.Check(num5 < p->_config.ConnectionGroups);
								p->_groupsAssigned[num5]--;
								Assert.Check(p->_groupsAssigned[num5] >= 0);
							}
						}
					}
					NetBitBuffer.Release(ptr);
				}
			}
			Assert.Check(p->_sendStack.Count == 0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool RecvBufferAvailable(NetPeer* p)
		{
			bool flag = p->_recv == null;
			if (flag)
			{
				p->_recv = p->_recvBlock->TryAcquire();
			}
			bool flag2 = p->_recv != null;
			if (flag2)
			{
				p->_recv->Address = default(NetAddress);
			}
			return p->_recv != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static bool RecvExpired(NetPeer* p)
		{
			return p->_recvTimer.ElapsedInMilliseconds > p->_config.OperationExpireTime;
		}

		public const int DEFAULT_HEADERS = 144;

		public const int MAX_MTU_BYTES_TOTAL = 1280;

		public const int MAX_MTU_BYTES_PAYLOAD = 1136;

		public const int MAX_MTU_BITS_PAYLOAD = 9088;

		public const int MAX_PACKET_BYTES_PAYLOAD = 44880;

		public const int MAX_PACKET_BYTES_TOTAL = 51200;

		internal const int FRAG_MAX_COUNT = 40;

		internal const byte FRAG_END_BIT = 128;

		private const int STATE_RUNNING = 0;

		private const int STATE_SHUTDOWN = 2;

		private volatile int _state;

		private NetConfig _config;

		private Timer _recvTimer;

		private unsafe byte* _fragmentBuffer;

		internal NetSocket _socket;

		private NetAddress _address;

		private NetBitBufferStack _sendStack;

		private unsafe NetPeerGroup* _groups;

		private unsafe NetPeerGroupMap* _groupsMap;

		private unsafe int* _groupsAssigned;

		private unsafe NetCommandRefused* _refusedCommand;

		private unsafe NetBitBuffer* _recv;

		private unsafe NetBitBufferBlock* _recvBlock;

		private Timer _delayedClock;

		private NetDelayedPacketList _delayedPackets;
	}
}
