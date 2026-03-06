using System;

namespace UnityEngine.ProBuilder
{
	internal struct IntVec4 : IEquatable<IntVec4>
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

		public float z
		{
			get
			{
				return this.value.z;
			}
		}

		public float w
		{
			get
			{
				return this.value.w;
			}
		}

		public IntVec4(Vector4 vector)
		{
			this.value = vector;
		}

		public override string ToString()
		{
			return string.Format("({0:F2}, {1:F2}, {2:F2}, {3:F2})", new object[]
			{
				this.x,
				this.y,
				this.z,
				this.w
			});
		}

		public static bool operator ==(IntVec4 a, IntVec4 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(IntVec4 a, IntVec4 b)
		{
			return !(a == b);
		}

		public bool Equals(IntVec4 p)
		{
			return IntVec4.round(this.x) == IntVec4.round(p.x) && IntVec4.round(this.y) == IntVec4.round(p.y) && IntVec4.round(this.z) == IntVec4.round(p.z) && IntVec4.round(this.w) == IntVec4.round(p.w);
		}

		public bool Equals(Vector4 p)
		{
			return IntVec4.round(this.x) == IntVec4.round(p.x) && IntVec4.round(this.y) == IntVec4.round(p.y) && IntVec4.round(this.z) == IntVec4.round(p.z) && IntVec4.round(this.w) == IntVec4.round(p.w);
		}

		public override bool Equals(object b)
		{
			return (b is IntVec4 && this.Equals((IntVec4)b)) || (b is Vector4 && this.Equals((Vector4)b));
		}

		public override int GetHashCode()
		{
			return VectorHash.GetHashCode(this.value);
		}

		private static int round(float v)
		{
			return Convert.ToInt32(v * 1000f);
		}

		public static implicit operator Vector4(IntVec4 p)
		{
			return p.value;
		}

		public static implicit operator IntVec4(Vector4 p)
		{
			return new IntVec4(p);
		}

		public Vector4 value;
	}
}
