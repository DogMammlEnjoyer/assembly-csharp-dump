using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(2)]
	[StructLayout(LayoutKind.Explicit)]
	public struct Vector2Compressed : INetworkStruct, IEquatable<Vector2Compressed>
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2Compressed(Vector2 v)
		{
			Vector2Compressed result;
			result.xEncoded = FloatUtils.Compress(v.x, 1024);
			result.yEncoded = FloatUtils.Compress(v.y, 1024);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(Vector2Compressed q)
		{
			Vector2 result;
			result.x = FloatUtils.Decompress(q.xEncoded, 1024f);
			result.y = FloatUtils.Decompress(q.yEncoded, 1024f);
			return result;
		}

		public bool Equals(Vector2Compressed other)
		{
			return this.xEncoded == other.xEncoded && this.yEncoded == other.yEncoded;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Vector2Compressed)
			{
				Vector2Compressed other = (Vector2Compressed)obj;
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
			return this.xEncoded * 397 ^ this.yEncoded;
		}

		public static bool operator ==(Vector2Compressed left, Vector2Compressed right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector2Compressed left, Vector2Compressed right)
		{
			return !left.Equals(right);
		}

		[FieldOffset(0)]
		public int xEncoded;

		[FieldOffset(4)]
		public int yEncoded;
	}
}
