using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(4)]
	[StructLayout(LayoutKind.Explicit)]
	public struct QuaternionCompressed : INetworkStruct, IEquatable<QuaternionCompressed>
	{
		public float X
		{
			get
			{
				return FloatUtils.Decompress(this.xEncoded, 1024f);
			}
			set
			{
				this.xEncoded = FloatUtils.Compress(value, 1024);
			}
		}

		public float Y
		{
			get
			{
				return FloatUtils.Decompress(this.yEncoded, 1024f);
			}
			set
			{
				this.yEncoded = FloatUtils.Compress(value, 1024);
			}
		}

		public float Z
		{
			get
			{
				return FloatUtils.Decompress(this.zEncoded, 1024f);
			}
			set
			{
				this.zEncoded = FloatUtils.Compress(value, 1024);
			}
		}

		public float W
		{
			get
			{
				return FloatUtils.Decompress(this.wEncoded, 1024f);
			}
			set
			{
				this.wEncoded = FloatUtils.Compress(value, 1024);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator QuaternionCompressed(Quaternion v)
		{
			QuaternionCompressed result;
			result.xEncoded = FloatUtils.Compress(v.x, 1024);
			result.yEncoded = FloatUtils.Compress(v.y, 1024);
			result.zEncoded = FloatUtils.Compress(v.z, 1024);
			result.wEncoded = FloatUtils.Compress(v.w, 1024);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Quaternion(QuaternionCompressed q)
		{
			Quaternion result;
			result.x = FloatUtils.Decompress(q.xEncoded, 1024f);
			result.y = FloatUtils.Decompress(q.yEncoded, 1024f);
			result.z = FloatUtils.Decompress(q.zEncoded, 1024f);
			result.w = FloatUtils.Decompress(q.wEncoded, 1024f);
			return result;
		}

		public bool Equals(QuaternionCompressed other)
		{
			return this.xEncoded == other.xEncoded && this.yEncoded == other.yEncoded && this.zEncoded == other.zEncoded && this.wEncoded == other.wEncoded;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is QuaternionCompressed)
			{
				QuaternionCompressed other = (QuaternionCompressed)obj;
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
			int num = this.xEncoded;
			num = (num * 397 ^ this.yEncoded);
			num = (num * 397 ^ this.zEncoded);
			return num * 397 ^ this.wEncoded;
		}

		public static bool operator ==(QuaternionCompressed left, QuaternionCompressed right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(QuaternionCompressed left, QuaternionCompressed right)
		{
			return !left.Equals(right);
		}

		[FieldOffset(0)]
		public int xEncoded;

		[FieldOffset(4)]
		public int yEncoded;

		[FieldOffset(8)]
		public int zEncoded;

		[FieldOffset(12)]
		public int wEncoded;
	}
}
