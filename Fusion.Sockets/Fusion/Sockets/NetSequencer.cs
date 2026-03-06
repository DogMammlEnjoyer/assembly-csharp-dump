using System;

namespace Fusion.Sockets
{
	internal struct NetSequencer
	{
		public int Bits
		{
			get
			{
				return this._bytes * 8;
			}
		}

		public int Bytes
		{
			get
			{
				return this._bytes;
			}
		}

		public ulong Sequence
		{
			get
			{
				return this._sequence;
			}
			set
			{
				Assert.Check(value <= this._mask);
				this._sequence = (value & this._mask);
			}
		}

		public void Reset()
		{
			this._sequence = 0UL;
		}

		public NetSequencer(int bytes)
		{
			this._bytes = bytes;
			this._sequence = 0UL;
			this._mask = (1UL << bytes * 8) - 1UL;
			this._shift = (8 - bytes) * 8;
		}

		public ulong Next()
		{
			return this._sequence = this.NextAfter(this._sequence);
		}

		public ulong NextAfter(ulong sequence)
		{
			return sequence + 1UL & this._mask;
		}

		public int Distance(ulong from, ulong to)
		{
			to <<= this._shift;
			from <<= this._shift;
			long num = (long)(from - to >> this._shift);
			Assert.Check(num >= -2147483648L && num <= 2147483647L);
			return (int)num;
		}

		private int _shift;

		private int _bytes;

		private ulong _mask;

		private ulong _sequence;
	}
}
