using System;

namespace Pathfinding
{
	public struct Int2 : IEquatable<Int2>
	{
		public Int2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public long sqrMagnitudeLong
		{
			get
			{
				return (long)this.x * (long)this.x + (long)this.y * (long)this.y;
			}
		}

		public static Int2 operator -(Int2 lhs)
		{
			lhs.x = -lhs.x;
			lhs.y = -lhs.y;
			return lhs;
		}

		public static Int2 operator +(Int2 a, Int2 b)
		{
			return new Int2(a.x + b.x, a.y + b.y);
		}

		public static Int2 operator -(Int2 a, Int2 b)
		{
			return new Int2(a.x - b.x, a.y - b.y);
		}

		public static bool operator ==(Int2 a, Int2 b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(Int2 a, Int2 b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static long DotLong(Int2 a, Int2 b)
		{
			return (long)a.x * (long)b.x + (long)a.y * (long)b.y;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			Int2 @int = (Int2)o;
			return this.x == @int.x && this.y == @int.y;
		}

		public bool Equals(Int2 other)
		{
			return this.x == other.x && this.y == other.y;
		}

		public override int GetHashCode()
		{
			return this.x * 49157 + this.y * 98317;
		}

		public static Int2 Min(Int2 a, Int2 b)
		{
			return new Int2(Math.Min(a.x, b.x), Math.Min(a.y, b.y));
		}

		public static Int2 Max(Int2 a, Int2 b)
		{
			return new Int2(Math.Max(a.x, b.x), Math.Max(a.y, b.y));
		}

		public static Int2 FromInt3XZ(Int3 o)
		{
			return new Int2(o.x, o.z);
		}

		public static Int3 ToInt3XZ(Int2 o)
		{
			return new Int3(o.x, 0, o.y);
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"(",
				this.x.ToString(),
				", ",
				this.y.ToString(),
				")"
			});
		}

		public int x;

		public int y;
	}
}
