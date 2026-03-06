using System;
using JetBrains.Annotations;

namespace MathGeoLib
{
	[PublicAPI]
	public struct Matrix3X4
	{
		public Matrix3X4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23)
		{
			this.M00 = m00;
			this.M01 = m01;
			this.M02 = m02;
			this.M03 = m03;
			this.M10 = m10;
			this.M11 = m11;
			this.M12 = m12;
			this.M13 = m13;
			this.M20 = m20;
			this.M21 = m21;
			this.M22 = m22;
			this.M23 = m23;
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				string.Format("{0}: {1}, ", "M00", this.M00),
				string.Format("{0}: {1}, ", "M01", this.M01),
				string.Format("{0}: {1}, ", "M02", this.M02),
				string.Format("{0}: {1}, ", "M03", this.M03),
				string.Format("{0}: {1}, ", "M10", this.M10),
				string.Format("{0}: {1}, ", "M11", this.M11),
				string.Format("{0}: {1}, ", "M12", this.M12),
				string.Format("{0}: {1}, ", "M13", this.M13),
				string.Format("{0}: {1}, ", "M20", this.M20),
				string.Format("{0}: {1}, ", "M21", this.M21),
				string.Format("{0}: {1}, ", "M22", this.M22),
				string.Format("{0}: {1}", "M23", this.M23)
			});
		}

		public readonly float M00;

		public readonly float M01;

		public readonly float M02;

		public readonly float M03;

		public readonly float M10;

		public readonly float M11;

		public readonly float M12;

		public readonly float M13;

		public readonly float M20;

		public readonly float M21;

		public readonly float M22;

		public readonly float M23;
	}
}
