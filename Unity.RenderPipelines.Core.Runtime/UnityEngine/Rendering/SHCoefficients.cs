using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public struct SHCoefficients : IEquatable<SHCoefficients>
	{
		public SHCoefficients(SphericalHarmonicsL2 sh)
		{
			this.SHAr = SHCoefficients.GetSHA(sh, 0);
			this.SHAg = SHCoefficients.GetSHA(sh, 1);
			this.SHAb = SHCoefficients.GetSHA(sh, 2);
			this.SHBr = SHCoefficients.GetSHB(sh, 0);
			this.SHBg = SHCoefficients.GetSHB(sh, 1);
			this.SHBb = SHCoefficients.GetSHB(sh, 2);
			this.SHC = SHCoefficients.GetSHC(sh);
			this.ProbesOcclusion = Vector4.one;
		}

		public SHCoefficients(SphericalHarmonicsL2 sh, Vector4 probesOcclusion)
		{
			this = new SHCoefficients(sh);
			this.ProbesOcclusion = probesOcclusion;
		}

		private static Vector4 GetSHA(SphericalHarmonicsL2 sh, int i)
		{
			return new Vector4(sh[i, 3], sh[i, 1], sh[i, 2], sh[i, 0] - sh[i, 6]);
		}

		private static Vector4 GetSHB(SphericalHarmonicsL2 sh, int i)
		{
			return new Vector4(sh[i, 4], sh[i, 5], sh[i, 6] * 3f, sh[i, 7]);
		}

		private static Vector4 GetSHC(SphericalHarmonicsL2 sh)
		{
			return new Vector4(sh[0, 8], sh[1, 8], sh[2, 8], 1f);
		}

		public bool Equals(SHCoefficients other)
		{
			return this.SHAr.Equals(other.SHAr) && this.SHAg.Equals(other.SHAg) && this.SHAb.Equals(other.SHAb) && this.SHBr.Equals(other.SHBr) && this.SHBg.Equals(other.SHBg) && this.SHBb.Equals(other.SHBb) && this.SHC.Equals(other.SHC) && this.ProbesOcclusion.Equals(other.ProbesOcclusion);
		}

		public override bool Equals(object obj)
		{
			if (obj is SHCoefficients)
			{
				SHCoefficients other = (SHCoefficients)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<Vector4, Vector4, Vector4, Vector4, Vector4, Vector4, Vector4, Vector4>(this.SHAr, this.SHAg, this.SHAb, this.SHBr, this.SHBg, this.SHBb, this.SHC, this.ProbesOcclusion);
		}

		public static bool operator ==(SHCoefficients left, SHCoefficients right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SHCoefficients left, SHCoefficients right)
		{
			return !left.Equals(right);
		}

		public Vector4 SHAr;

		public Vector4 SHAg;

		public Vector4 SHAb;

		public Vector4 SHBr;

		public Vector4 SHBg;

		public Vector4 SHBb;

		public Vector4 SHC;

		public Vector4 ProbesOcclusion;
	}
}
