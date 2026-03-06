using System;

namespace UnityEngine.ProBuilder
{
	internal struct IntVec2 : IEquatable<IntVec2>
	{
		public float x
		{
			get
			{
				return this.value.x;
			}
		}

		public float y
		{
			get
			{
				return this.value.y;
			}
		}

		public IntVec2(Vector2 vector)
		{
			this.value = vector;
		}

		public override string ToString()
		{
			return string.Format("({0:F2}, {1:F2})", this.x, this.y);
		}

		public static bool operator ==(IntVec2 a, IntVec2 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(IntVec2 a, IntVec2 b)
		{
			return !(a == b);
		}

		public bool Equals(IntVec2 p)
		{
			return IntVec2.round(this.x) == IntVec2.round(p.x) && IntVec2.round(this.y) == IntVec2.round(p.y);
		}

		public bool Equals(Vector2 p)
		{
			return IntVec2.round(this.x) == IntVec2.round(p.x) && IntVec2.round(this.y) == IntVec2.round(p.y);
		}

		public override bool Equals(object b)
		{
			return (b is IntVec2 && this.Equals((IntVec2)b)) || (b is Vector2 && this.Equals((Vector2)b));
		}

		public override int GetHashCode()
		{
			return VectorHash.GetHashCode(this.value);
		}

		private static int round(float v)
		{
			return Convert.ToInt32(v * 1000f);
		}

		public static implicit operator Vector2(IntVec2 p)
		{
			return p.value;
		}

		public static implicit operator IntVec2(Vector2 p)
		{
			return new IntVec2(p);
		}

		public Vector2 value;
	}
}
