using System;
using System.Runtime.InteropServices;

namespace Fusion.Sockets
{
	[StructLayout(LayoutKind.Explicit)]
	public struct ReliableId
	{
		public long SourceCombined
		{
			get
			{
				return (long)this.Source << 32 | (long)this.SourceSend;
			}
		}

		public const int SIZE = 48;

		[FieldOffset(0)]
		public ulong Sequence;

		[FieldOffset(8)]
		public int SliceLength;

		[FieldOffset(12)]
		public int TotalLength;

		[FieldOffset(16)]
		public int Source;

		[FieldOffset(20)]
		public int SourceSend;

		[FieldOffset(24)]
		public int Target;

		[FieldOffset(28)]
		public ReliableKey Key;

		[FieldOffset(44)]
		private int _padding;
	}
}
