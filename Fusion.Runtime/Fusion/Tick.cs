using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct Tick : IComparable<Tick>, IEquatable<Tick>
	{
		public Tick Next(int increment)
		{
			Tick result;
			result.Raw = this.Raw + increment;
			return result;
		}

		public bool Equals(Tick other)
		{
			return this.Raw == other.Raw;
		}

		public int CompareTo(Tick other)
		{
			return this.Raw.CompareTo(other.Raw);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Tick)
			{
				Tick other = (Tick)obj;
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
			return this.Raw;
		}

		public override string ToString()
		{
			return string.Format("[Tick:{0}]", this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >(Tick a, Tick b)
		{
			return a.Raw > b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator >=(Tick a, Tick b)
		{
			return a.Raw >= b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <(Tick a, Tick b)
		{
			return a.Raw < b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator <=(Tick a, Tick b)
		{
			return a.Raw <= b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(Tick a, Tick b)
		{
			return a.Raw == b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(Tick a, Tick b)
		{
			return a.Raw != b.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Tick(int value)
		{
			Tick result;
			result.Raw = ((value < 0) ? 0 : value);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator int(Tick value)
		{
			return value.Raw;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator bool(Tick value)
		{
			return value.Raw > 0;
		}

		public const int SIZE = 4;

		public const int ALIGNMENT = 4;

		[FieldOffset(0)]
		public int Raw;

		public sealed class RelationalComparer : IComparer<Tick>
		{
			public int Compare(Tick x, Tick y)
			{
				return x.Raw.CompareTo(y.Raw);
			}
		}

		public sealed class EqualityComparer : IEqualityComparer<Tick>
		{
			public bool Equals(Tick x, Tick y)
			{
				return x.Raw == y.Raw;
			}

			public int GetHashCode(Tick obj)
			{
				return obj.Raw;
			}
		}
	}
}
