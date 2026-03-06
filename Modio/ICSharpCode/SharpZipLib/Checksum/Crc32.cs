using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.SharpZipLib.Checksum
{
	public sealed class Crc32 : IChecksum
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static uint ComputeCrc32(uint oldCrc, byte bval)
		{
			return Crc32.crcTable[(int)((oldCrc ^ (uint)bval) & 255U)] ^ oldCrc >> 8;
		}

		public Crc32()
		{
			this.Reset();
		}

		public void Reset()
		{
			this.checkValue = Crc32.crcInit;
		}

		public long Value
		{
			get
			{
				return (long)((ulong)(this.checkValue ^ Crc32.crcXor));
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Update(int bval)
		{
			this.checkValue = (Crc32.crcTable[(int)(checked((IntPtr)(unchecked((ulong)this.checkValue ^ (ulong)((long)bval)) & 255UL)))] ^ this.checkValue >> 8);
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
				this.checkValue = CrcUtilities.UpdateDataForReversedPoly(data, offset, Crc32.crcTable, this.checkValue);
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

		private static readonly uint crcInit = uint.MaxValue;

		private static readonly uint crcXor = uint.MaxValue;

		private static readonly uint[] crcTable = CrcUtilities.GenerateSlicingLookupTable(3988292384U, true);

		private uint checkValue;
	}
}
