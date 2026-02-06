using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion
{
	[NetworkStructWeaved(1)]
	[StructLayout(LayoutKind.Explicit)]
	public struct FloatCompressed : INetworkStruct, IEquatable<FloatCompressed>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator FloatCompressed(float v)
		{
			FloatCompressed result;
			result.valueEncoded = FloatUtils.Compress(v, 1024);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator float(FloatCompressed q)
		{
			return FloatUtils.Decompress(q.valueEncoded, 1024f);
		}

		public bool Equals(FloatCompressed other)
		{
			return this.valueEncoded == other.valueEncoded;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is FloatCompressed)
			{
				FloatCompressed other = (FloatCompressed)obj;
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
			return this.valueEncoded;
		}

		public static bool operator ==(FloatCompressed left, FloatCompressed right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(FloatCompressed left, FloatCompressed right)
		{
			return !left.Equals(right);
		}

		[FieldOffset(0)]
		public int valueEncoded;
	}
}
