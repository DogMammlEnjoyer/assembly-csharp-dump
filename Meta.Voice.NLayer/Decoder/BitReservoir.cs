using System;
using System.IO;

namespace Meta.Voice.NLayer.Decoder
{
	internal class BitReservoir
	{
		private static int GetSlots(IMpegFrame frame)
		{
			int num = frame.FrameLength - 4;
			if (frame.HasCrc)
			{
				num -= 2;
			}
			if (frame.Version == MpegVersion.Version1 && frame.ChannelMode != MpegChannelMode.Mono)
			{
				return num - 32;
			}
			if (frame.Version > MpegVersion.Version1 && frame.ChannelMode == MpegChannelMode.Mono)
			{
				return num - 9;
			}
			return num - 17;
		}

		public bool AddBits(IMpegFrame frame, int overlap)
		{
			int end = this._end;
			int num = BitReservoir.GetSlots(frame);
			while (--num >= 0)
			{
				int num2 = frame.ReadBits(8);
				if (num2 == -1)
				{
					throw new InvalidDataException("Frame did not have enough bytes!");
				}
				byte[] buf = this._buf;
				int num3 = this._end + 1;
				this._end = num3;
				buf[num3] = (byte)num2;
				if (this._end == this._buf.Length - 1)
				{
					this._end = -1;
				}
			}
			this._bitsLeft = 8;
			if (end == -1)
			{
				return overlap == 0;
			}
			if ((end + 1 - this._start + this._buf.Length) % this._buf.Length >= overlap)
			{
				this._start = (end + 1 - overlap + this._buf.Length) % this._buf.Length;
				return true;
			}
			this._start = end + overlap;
			return false;
		}

		public int GetBits(int count)
		{
			int num;
			int result = this.TryPeekBits(count, out num);
			if (num < count)
			{
				throw new InvalidDataException("Reservoir did not have enough bytes!");
			}
			this.SkipBits(count);
			return result;
		}

		public int Get1Bit()
		{
			if (this._bitsLeft == 0)
			{
				throw new InvalidDataException("Reservoir did not have enough bytes!");
			}
			this._bitsLeft--;
			this._bitsRead += 1L;
			int result = this._buf[this._start] >> this._bitsLeft & 1;
			if (this._bitsLeft == 0 && (this._start = (this._start + 1) % this._buf.Length) != this._end + 1)
			{
				this._bitsLeft = 8;
			}
			return result;
		}

		public int TryPeekBits(int count, out int readCount)
		{
			if (count < 0 || count > 32)
			{
				throw new ArgumentOutOfRangeException("count", "Must return between 0 and 32 bits!");
			}
			if (this._bitsLeft == 0 || count == 0)
			{
				readCount = 0;
				return 0;
			}
			int num = (int)this._buf[this._start];
			if (count < this._bitsLeft)
			{
				num >>= this._bitsLeft - count;
				num &= (1 << count) - 1;
				readCount = count;
				return num;
			}
			num &= (1 << this._bitsLeft) - 1;
			count -= this._bitsLeft;
			readCount = this._bitsLeft;
			int num2 = this._start;
			while (count > 0 && (num2 = (num2 + 1) % this._buf.Length) != this._end + 1)
			{
				int num3 = Math.Min(count, 8);
				num <<= num3;
				num |= this._buf[num2] >> (8 - num3) % 8;
				count -= num3;
				readCount += num3;
			}
			return num;
		}

		public int BitsAvailable
		{
			get
			{
				if (this._bitsLeft > 0)
				{
					return (this._end + this._buf.Length - this._start) % this._buf.Length * 8 + this._bitsLeft;
				}
				return 0;
			}
		}

		public long BitsRead
		{
			get
			{
				return this._bitsRead;
			}
		}

		public void SkipBits(int count)
		{
			if (count > 0)
			{
				if (count > this.BitsAvailable)
				{
					throw new ArgumentOutOfRangeException("count");
				}
				int num = 8 - this._bitsLeft + count;
				this._start = (num / 8 + this._start) % this._buf.Length;
				this._bitsLeft = 8 - num % 8;
				this._bitsRead += (long)count;
			}
		}

		public void RewindBits(int count)
		{
			this._bitsLeft += count;
			this._bitsRead -= (long)count;
			while (this._bitsLeft > 8)
			{
				this._start--;
				this._bitsLeft -= 8;
			}
			while (this._start < 0)
			{
				this._start += this._buf.Length;
			}
		}

		public void FlushBits()
		{
			if (this._bitsLeft < 8)
			{
				this.SkipBits(this._bitsLeft);
			}
		}

		public void Reset()
		{
			this._start = 0;
			this._end = -1;
			this._bitsLeft = 0;
		}

		private byte[] _buf = new byte[8192];

		private int _start;

		private int _end = -1;

		private int _bitsLeft;

		private long _bitsRead;
	}
}
