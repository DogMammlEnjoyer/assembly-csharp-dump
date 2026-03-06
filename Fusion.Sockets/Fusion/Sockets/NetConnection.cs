using System;

namespace Fusion.Sockets
{
	public struct NetConnection
	{
		public bool Active
		{
			get
			{
				return this.MapState == NetConnectionMap.EntryState.Used;
			}
		}

		public double RoundTripTime
		{
			get
			{
				return this.Rtt;
			}
		}

		public NetAddress RemoteAddress
		{
			get
			{
				return this.Address;
			}
		}

		public NetConnectionStatus ConnectionStatus
		{
			get
			{
				return this.Status;
			}
		}

		public NetConnectionId LocalConnectionId
		{
			get
			{
				return this.LocalId;
			}
		}

		public NetConnectionId RemoteConnectionId
		{
			get
			{
				return this.RemoteId;
			}
		}

		internal unsafe static ushort NextNotifySendSequence(NetConnection* c)
		{
			ulong num = c->NotifySendSequencer.Next();
			Assert.Check(num <= 65535UL);
			return (ushort)num;
		}

		internal unsafe static void Initialize(NetConnection* c, short group, short index, NetConfig* config)
		{
			c->LocalId.Group = group;
			c->LocalId.GroupIndex = index;
			c->NotifySendWindow = NetSendEnvelopeRingBuffer.Create(config->Notify.WindowSize);
			c->NotifySendSequencer = new NetSequencer(config->Notify.SequenceBytes);
			c->NotifyRecvFragmentBuffer = (byte*)Native.MallocAndClear(51200);
			c->NotifyRecvFragmentBufferLength = 0;
			c->NotifyRecvFragment = 0;
			NetConnection.Reset(c);
		}

		internal unsafe static void SetRtt(NetConnection* c, double rtt = 0.0)
		{
			c->Rtt = rtt;
		}

		internal unsafe static void Reset(NetConnection* c)
		{
			Assert.Check(c->NotifySendWindow.Count == 0);
			c->LocalId.Generation = c->LocalId.Generation + 1U;
			bool flag = c->LocalId.Generation == 0U;
			if (flag)
			{
				c->LocalId.Generation = c->LocalId.Generation + 1U;
			}
			Native.Free<byte>(ref c->ConnectionToken);
			c->ConnectionTokenLength = 0;
			Native.Free<byte>(ref c->DisconnectToken);
			c->DisconnectTokenLength = 0;
			Native.Free<byte>(ref c->UniqueId);
			c->UniqueIdHash = 0L;
			c->ReliableSendList.Dispose();
			c->ReliableSendTimer = TimerDelta.StartNew();
			c->ReliableBuffer.Dispose();
			c->ReliableBuffer = ReliableBuffer.Create();
			c->Address = default(NetAddress);
			c->NotifySendWindow.Reset();
			c->NotifySendSequencer.Reset();
			c->Status = (NetConnectionStatus)0;
			c->StateShutdown = default(NetConnection.StateShutdownData);
			c->StateConnecting = default(NetConnection.StateConnectingData);
			c->StateDisconnected = default(NetConnection.StateDisconnectedData);
			c->RemoteId = default(NetConnectionId);
			c->MapState = NetConnectionMap.EntryState.None;
			c->UniqueId = null;
			c->UniqueIdHash = 0L;
			c->SendTime = 0.0;
			c->RecvTime = 0.0;
			c->Rtt = 0.0;
			c->NotifySendTime = 0.0;
			c->NotifyRecvTime = 0.0;
			c->NotifyRecvMask = 0UL;
			c->NotifyRecvAckTime = 0.0;
			c->NotifyRecvSequence = 0;
			c->NotifyRecvUnackedCount = 0;
			c->NotifyRecvAckOutdatedCount = 0;
			c->NotifyRecvFragment = 0;
			c->NotifyRecvFragmentBufferLength = 0;
			c->NotifyRecvFragmentSequenceDistance = 0;
		}

		public override string ToString()
		{
			return string.Format("[{0}: {1}={2}, {3}={4}]", new object[]
			{
				"NetConnection",
				"RemoteAddress",
				this.RemoteAddress,
				"UniqueId",
				this.UniqueIdHash
			});
		}

		internal const byte UNIQUE_ID_SIZE = 8;

		internal ulong MapHash;

		internal unsafe NetConnection* MapNext;

		internal NetConnectionMap.EntryState MapState;

		internal NetConnectionId LocalId;

		internal NetConnectionId RemoteId;

		internal NetAddress Address;

		internal NetConnectionStatus Status;

		internal double Rtt;

		internal double SendTime;

		internal double RecvTime;

		internal NetConnection.StateConnectingData StateConnecting;

		internal NetConnection.StateDisconnectedData StateDisconnected;

		internal NetConnection.StateShutdownData StateShutdown;

		internal NetSendEnvelopeRingBuffer NotifySendWindow;

		internal NetSequencer NotifySendSequencer;

		internal double NotifySendTime;

		internal double NotifyRecvAckTime;

		internal int NotifyRecvAckOutdatedCount;

		internal double NotifyRecvTime;

		internal ulong NotifyRecvMask;

		internal ushort NotifyRecvSequence;

		internal int NotifyRecvUnackedCount;

		internal int NotifyRecvFragment;

		internal unsafe byte* NotifyRecvFragmentBuffer;

		internal int NotifyRecvFragmentBufferLength;

		internal int NotifyRecvFragmentSequenceDistance;

		internal unsafe byte* ConnectionToken;

		internal int ConnectionTokenLength;

		internal unsafe byte* DisconnectToken;

		internal int DisconnectTokenLength;

		internal long UniqueIdHash;

		internal unsafe byte* UniqueId;

		internal uint Counter;

		internal ReliableBuffer ReliableBuffer;

		internal ReliableList ReliableSendList;

		internal TimerDelta ReliableSendTimer;

		internal struct StateConnectingData
		{
			public int Attempts;

			public double AttemptTimeout;
		}

		internal struct StateShutdownData
		{
			public double Timeout;

			public int Unmapped;
		}

		internal struct StateDisconnectedData
		{
			public NetDisconnectReason Reason;

			public int CallbackInvoked;

			public int SentDisconnectCommand;
		}
	}
}
