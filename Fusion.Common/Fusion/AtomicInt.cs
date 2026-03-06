using System;
using System.Threading;

namespace Fusion
{
	public struct AtomicInt
	{
		public AtomicInt(int value)
		{
			this._value = value;
		}

		public int Value
		{
			get
			{
				return Thread.VolatileRead(ref this._value);
			}
		}

		public int IncrementPost()
		{
			return Interlocked.Increment(ref this._value) - 1;
		}

		public int IncrementPre()
		{
			return Interlocked.Increment(ref this._value);
		}

		public int Decrement()
		{
			return Interlocked.Decrement(ref this._value);
		}

		public int Exchange(int value)
		{
			return Interlocked.Exchange(ref this._value, value);
		}

		public int CompareExchange(int value, int assumed)
		{
			return Interlocked.CompareExchange(ref this._value, value, assumed);
		}

		private volatile int _value;
	}
}
