using System;

namespace Fusion.Sockets
{
	public struct NetBitBufferNull : INetBitWriteStream
	{
		public int OffsetBits
		{
			get
			{
				return this._offsetBits;
			}
			set
			{
				this._offsetBits = value;
			}
		}

		public void PadToByteBoundary()
		{
			bool flag = this._offsetBits % 8 != 0;
			if (flag)
			{
				this.WriteByte(0, 8 - this._offsetBits % 8);
			}
		}

		public void WriteByte(byte value, int bits = 8)
		{
			this._offsetBits += bits;
		}

		public void WriteInt32(int value, int bits = 32)
		{
			this._offsetBits += bits;
		}

		public void WriteInt32VarLength(int value)
		{
			this.WriteUInt32VarLength((uint)value);
		}

		public void WriteInt32VarLength(int value, int blockSize)
		{
			this.WriteUInt32VarLength((uint)value, blockSize);
		}

		public void WriteUInt32VarLength(uint value, int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = (Maths.BitScanReverse(value) + blockSize) / blockSize;
			this._offsetBits += num + num * blockSize;
		}

		public void WriteUInt64VarLength(ulong value, int blockSize)
		{
			blockSize = Maths.Clamp(blockSize, 2, 16);
			int num = (Maths.BitScanReverse(value) + blockSize) / blockSize;
			this._offsetBits += num + num * blockSize;
		}

		public void WriteUInt32VarLength(uint value)
		{
			int num = 0;
			for (;;)
			{
				value >>= 7;
				bool flag = value > 0U;
				if (!flag)
				{
					break;
				}
				num++;
			}
			this._offsetBits += (num + 1) * 8;
		}

		public bool WriteBoolean(bool b)
		{
			this._offsetBits++;
			return b;
		}

		public unsafe void WriteBytesAligned(void* buffer, int length)
		{
			this.PadToByteBoundary();
			this._offsetBits += length * 8;
		}

		public void WriteBytesAligned(Span<byte> buffer)
		{
			this.PadToByteBoundary();
			this._offsetBits += buffer.Length * 8;
		}

		private int _offsetBits;
	}
}
