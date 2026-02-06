using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Fusion
{
	[NetworkStructWeaved(3)]
	[StructLayout(LayoutKind.Explicit)]
	public struct Vector3Compressed : INetworkStruct, IEquatable<Vector3Compressed>
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3Compressed(Vector3 v)
		{
			Vector3Compressed result;
			result.xEncoded = FloatUtils.Compress(v.x, 1024);
			result.yEncoded = FloatUtils.Compress(v.y, 1024);
			result.zEncoded = FloatUtils.Compress(v.z, 1024);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3(Vector3Compressed q)
		{
			Vector3 result;
			result.x = FloatUtils.Decompress(q.xEncoded, 1024f);
			result.y = FloatUtils.Decompress(q.yEncoded, 1024f);
			result.z = FloatUtils.Decompress(q.zEncoded, 1024f);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector3Compressed(Vector2 v)
		{
			Vector3Compressed result;
			result.xEncoded = FloatUtils.Compress(v.x, 1024);
			result.yEncoded = FloatUtils.Compress(v.y, 1024);
			result.zEncoded = 0;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator Vector2(Vector3Compressed q)
		{
			Vector2 result;
			result.x = FloatUtils.Decompress(q.xEncoded, 1024f);
			result.y = FloatUtils.Decompress(q.yEncoded, 1024f);
			return result;
		}

		public bool Equals(Vector3Compressed other)
		{
			return this.xEncoded == other.xEncoded && this.yEncoded == other.yEncoded && this.zEncoded == other.zEncoded;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is Vector3Compressed)
			{
				Vector3Compressed other = (Vector3Compressed)obj;
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
			return num * 397 ^ this.zEncoded;
		}

		public static bool operator ==(Vector3Compressed left, Vector3Compressed right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Vector3Compressed left, Vector3Compressed right)
		{
			return !left.Equals(right);
		}

		[FieldOffset(0)]
		public int xEncoded;

		[FieldOffset(4)]
		public int yEncoded;

		[FieldOffset(8)]
		public int zEncoded;
	}
}
