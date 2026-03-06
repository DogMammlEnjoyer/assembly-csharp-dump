using System;

namespace ICSharpCode.SharpZipLib.Zip.Compression
{
	public class PendingBuffer
	{
		public PendingBuffer() : this(4096)
		{
		}

		public PendingBuffer(int bufferSize)
		{
			this.buffer = new byte[bufferSize];
		}

		public void Reset()
		{
			this.start = (this.end = (this.bitCount = 0));
		}

		public void WriteByte(int value)
		{
			byte[] array = this.buffer;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
		}

		public void WriteShort(int value)
		{
			byte[] array = this.buffer;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
			byte[] array2 = this.buffer;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)(value >> 8);
		}

		public void WriteInt(int value)
		{
			byte[] array = this.buffer;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)value;
			byte[] array2 = this.buffer;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)(value >> 8);
			byte[] array3 = this.buffer;
			num = this.end;
			this.end = num + 1;
			array3[num] = (byte)(value >> 16);
			byte[] array4 = this.buffer;
			num = this.end;
			this.end = num + 1;
			array4[num] = (byte)(value >> 24);
		}

		public void WriteBlock(byte[] block, int offset, int length)
		{
			Array.Copy(block, offset, this.buffer, this.end, length);
			this.end += length;
		}

		public int BitCount
		{
			get
			{
				return this.bitCount;
			}
		}

		public void AlignToByte()
		{
			if (this.bitCount > 0)
			{
				byte[] array = this.buffer;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				if (this.bitCount > 8)
				{
					byte[] array2 = this.buffer;
					num = this.end;
					this.end = num + 1;
					array2[num] = (byte)(this.bits >> 8);
				}
			}
			this.bits = 0U;
			this.bitCount = 0;
		}

		public void WriteBits(int b, int count)
		{
			this.bits |= (uint)((uint)b << this.bitCount);
			this.bitCount += count;
			if (this.bitCount >= 16)
			{
				byte[] array = this.buffer;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				byte[] array2 = this.buffer;
				num = this.end;
				this.end = num + 1;
				array2[num] = (byte)(this.bits >> 8);
				this.bits >>= 16;
				this.bitCount -= 16;
			}
		}

		public void WriteShortMSB(int s)
		{
			byte[] array = this.buffer;
			int num = this.end;
			this.end = num + 1;
			array[num] = (byte)(s >> 8);
			byte[] array2 = this.buffer;
			num = this.end;
			this.end = num + 1;
			array2[num] = (byte)s;
		}

		public bool IsFlushed
		{
			get
			{
				return this.end == 0;
			}
		}

		public int Flush(byte[] output, int offset, int length)
		{
			if (this.bitCount >= 8)
			{
				byte[] array = this.buffer;
				int num = this.end;
				this.end = num + 1;
				array[num] = (byte)this.bits;
				this.bits >>= 8;
				this.bitCount -= 8;
			}
			if (length > this.end - this.start)
			{
				length = this.end - this.start;
				Array.Copy(this.buffer, this.start, output, offset, length);
				this.start = 0;
				this.end = 0;
			}
			else
			{
				Array.Copy(this.buffer, this.start, output, offset, length);
				this.start += length;
			}
			return length;
		}

		public byte[] ToByteArray()
		{
			this.AlignToByte();
			byte[] array = new byte[this.end - this.start];
			Array.Copy(this.buffer, this.start, array, 0, array.Length);
			this.start = 0;
			this.end = 0;
			return array;
		}

		private readonly byte[] buffer;

		private int start;

		private int end;

		private uint bits;

		private int bitCount;
	}
}
