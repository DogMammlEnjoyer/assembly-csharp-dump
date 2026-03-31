using System;
using System.Runtime.InteropServices;

namespace K4os.Compression.LZ4
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	internal readonly struct PickleHeader
	{
		public ushort DataOffset { get; }

		public ushort Flags { get; }

		public int ResultLength { get; }

		public bool IsCompressed
		{
			get
			{
				return (this.Flags & 1) > 0;
			}
		}

		public PickleHeader(ushort dataOffset, int resultLength, bool compressed)
		{
			this.DataOffset = dataOffset;
			this.ResultLength = resultLength;
			this.Flags = (ushort)((compressed ? 1 : 0) | 0);
		}
	}
}
