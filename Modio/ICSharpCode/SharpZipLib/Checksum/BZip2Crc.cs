using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.SharpZipLib.Checksum
{
	public sealed class BZip2Crc : IChecksum
	{
		public BZip2Crc()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.checkValue = uint.MaxValue;
		}

		public long Value
		{
			get
			{
				return (long)((ulong)(~(ulong)this.checkValue));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(int bval)
		{
			this.checkValue = (BZip2Crc.crcTable[(int)((byte)((ulong)(this.checkValue >> 24 & 255U) ^ (ulong)((long)bval)))] ^ this.checkValue << 8);
		}

		public void Update(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			this.Update(buffer, 0, buffer.Length);
		}

		public void Update(ArraySegment<byte> segment)
		{
			this.Update(segment.Array, segment.Offset, segment.Count);
		}

		private void Update(byte[] data, int offset, int count)
		{
			int num = count % 16;
			int num2 = offset + count - num;
			while (offset != num2)
			{
				this.checkValue = CrcUtilities.UpdateDataForNormalPoly(data, offset, BZip2Crc.crcTable, this.checkValue);
				offset += 16;
			}
			if (num != 0)
			{
				this.SlowUpdateLoop(data, offset, num2 + num);
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void SlowUpdateLoop(byte[] data, int offset, int end)
		{
			while (offset != end)
			{
				this.Update((int)data[offset++]);
			}
		}

		private const uint crcInit = 4294967295U;

		private static readonly uint[] crcTable = CrcUtilities.GenerateSlicingLookupTable(79764919U, false);

		private uint checkValue;
	}
}
