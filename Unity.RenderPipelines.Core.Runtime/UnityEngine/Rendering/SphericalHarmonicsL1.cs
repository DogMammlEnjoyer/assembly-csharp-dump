using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public struct SphericalHarmonicsL1
	{
		public static SphericalHarmonicsL1 operator +(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			return new SphericalHarmonicsL1
			{
				shAr = lhs.shAr + rhs.shAr,
				shAg = lhs.shAg + rhs.shAg,
				shAb = lhs.shAb + rhs.shAb
			};
		}

		public static SphericalHarmonicsL1 operator -(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			return new SphericalHarmonicsL1
			{
				shAr = lhs.shAr - rhs.shAr,
				shAg = lhs.shAg - rhs.shAg,
				shAb = lhs.shAb - rhs.shAb
			};
		}

		public static SphericalHarmonicsL1 operator *(SphericalHarmonicsL1 lhs, float rhs)
		{
			return new SphericalHarmonicsL1
			{
				shAr = lhs.shAr * rhs,
				shAg = lhs.shAg * rhs,
				shAb = lhs.shAb * rhs
			};
		}

		public static SphericalHarmonicsL1 operator /(SphericalHarmonicsL1 lhs, float rhs)
		{
			return new SphericalHarmonicsL1
			{
				shAr = lhs.shAr / rhs,
				shAg = lhs.shAg / rhs,
				shAb = lhs.shAb / rhs
			};
		}

		public static bool operator ==(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			return lhs.shAr == rhs.shAr && lhs.shAg == rhs.shAg && lhs.shAb == rhs.shAb;
		}

		public static bool operator !=(SphericalHarmonicsL1 lhs, SphericalHarmonicsL1 rhs)
		{
			return !(lhs == rhs);
		}

		public override bool Equals(object other)
		{
			return other is SphericalHarmonicsL1 && this == (SphericalHarmonicsL1)other;
		}

		public override int GetHashCode()
		{
			return ((391 + this.shAr.GetHashCode()) * 23 + this.shAg.GetHashCode()) * 23 + this.shAb.GetHashCode();
		}

		public Vector4 shAr;

		public Vector4 shAg;

		public Vector4 shAb;

		public static readonly SphericalHarmonicsL1 zero = new SphericalHarmonicsL1
		{
			shAr = Vector4.zero,
			shAg = Vector4.zero,
			shAb = Vector4.zero
		};
	}
}
