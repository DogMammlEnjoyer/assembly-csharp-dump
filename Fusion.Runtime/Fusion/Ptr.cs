using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct Ptr : IEquatable<Ptr>, INetworkStruct
	{
		public static Ptr Null
		{
			get
			{
				return default(Ptr);
			}
		}

		public bool Equals(Ptr other)
		{
			return this.Address == other.Address;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Ptr)
			{
				Ptr other = (Ptr)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return this.Address;
		}

		public override string ToString()
		{
			return string.Format("0x{0:X}", this.Address);
		}

		public static implicit operator bool(Ptr a)
		{
			return a.Address != 0;
		}

		public static bool operator ==(Ptr a, Ptr b)
		{
			return a.Address == b.Address;
		}

		public static bool operator !=(Ptr a, Ptr b)
		{
			return a.Address != b.Address;
		}

		public static Ptr operator +(Ptr p, int v)
		{
			p.Address += v;
			return p;
		}

		public static Ptr operator -(Ptr p, int v)
		{
			p.Address -= v;
			return p;
		}

		public const int SIZE = 4;

		[FieldOffset(0)]
		public int Address;

		public sealed class EqualityComparer : IEqualityComparer<Ptr>
		{
			public bool Equals(Ptr x, Ptr y)
			{
				return x.Address == y.Address;
			}

			public int GetHashCode(Ptr obj)
			{
				return obj.Address;
			}
		}
	}
}
