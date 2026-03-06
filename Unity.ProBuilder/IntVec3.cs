using System;

namespace UnityEngine.ProBuilder
{
	internal struct IntVec3 : IEquatable<IntVec3>
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

		public IntVec3(Vector3 vector)
		{
			this.value = vector;
		}

		public override string ToString()
		{
			return string.Format("({0:F2}, {1:F2}, {2:F2})", this.x, this.y, this.z);
		}

		public static bool operator ==(IntVec3 a, IntVec3 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(IntVec3 a, IntVec3 b)
		{
			return !(a == b);
		}

		public bool Equals(IntVec3 p)
		{
			return IntVec3.round(this.x) == IntVec3.round(p.x) && IntVec3.round(this.y) == IntVec3.round(p.y) && IntVec3.round(this.z) == IntVec3.round(p.z);
		}

		public bool Equals(Vector3 p)
		{
			return IntVec3.round(this.x) == IntVec3.round(p.x) && IntVec3.round(this.y) == IntVec3.round(p.y) && IntVec3.round(this.z) == IntVec3.round(p.z);
		}

		public override bool Equals(object b)
		{
			return (b is IntVec3 && this.Equals((IntVec3)b)) || (b is Vector3 && this.Equals((Vector3)b));
		}

		public override int GetHashCode()
		{
			return VectorHash.GetHashCode(this.value);
		}

		private static int round(float v)
		{
			return Convert.ToInt32(v * 1000f);
		}

		public static implicit operator Vector3(IntVec3 p)
		{
			return p.value;
		}

		public static implicit operator IntVec3(Vector3 p)
		{
			return new IntVec3(p);
		}

		public Vector3 value;
	}
}
