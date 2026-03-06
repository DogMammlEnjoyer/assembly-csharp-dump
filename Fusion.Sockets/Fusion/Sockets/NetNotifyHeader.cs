using System;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct NetNotifyHeader
	{
		public unsafe override string ToString()
		{
			ulong ackMask = this.AckMask;
			return string.Format("[Type: {0} Frag:{1} Seq:{2}, AckSeq:{3}, AckMask:{4}]", new object[]
			{
				this.PacketType,
				this.Fragment,
				this.Sequence,
				this.AckSequence,
				Maths.PrintBits((byte*)(&ackMask), 8)
			});
		}

		public static NetNotifyHeader CreateData(ushort sequence, ushort ackSequence, ulong ackMask)
		{
			NetNotifyHeader result;
			result.PacketType = NetPacketType.NotifyData;
			result.Sequence = sequence;
			result.AckSequence = ackSequence;
			result.AckMask = ackMask;
			result.Fragment = 0;
			return result;
		}

		public static NetNotifyHeader CreateAcks(ushort ackSequence, ulong ackMask)
		{
			NetNotifyHeader result;
			result.PacketType = NetPacketType.NotifyAcks;
			result.Sequence = 0;
			result.AckSequence = ackSequence;
			result.AckMask = ackMask;
			result.Fragment = 0;
			return result;
		}

		public const int SIZE_IN_BYTES = 14;

		public const int SIZE_IN_BITS = 112;

		[FieldOffset(0)]
		public NetPacketType PacketType;

		[FieldOffset(1)]
		public byte Fragment;

		[FieldOffset(2)]
		public ushort Sequence;

		[FieldOffset(4)]
		public ushort AckSequence;

		[FieldOffset(6)]
		public ulong AckMask;
	}
}
