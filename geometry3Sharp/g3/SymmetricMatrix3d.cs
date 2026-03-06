using System;

namespace g3
{
	internal class SymmetricMatrix3d
	{
		public void SetATA(ref Matrix3d A)
		{
			Vector3d vector3d = A.Column(0);
			Vector3d v = A.Column(1);
			Vector3d v2 = A.Column(2);
			this.entries[0] = vector3d.LengthSquared;
			this.entries[1] = v.LengthSquared;
			this.entries[2] = v2.LengthSquared;
			this.entries[3] = vector3d.Dot(v);
			this.entries[4] = vector3d.Dot(v2);
			this.entries[5] = v.Dot(v2);
		}

		public void quatConjugate01(double c, double s)
		{
			double num = c * c - s * s;
			double num2 = 2.0 * s * c;
			double num3 = num2 * num;
			double num4 = num * num;
			double num5 = num2 * num2;
			double num6 = num4 * this.entries[0] + 2.0 * num3 * this.entries[3] + num5 * this.entries[1];
			double num7 = num5 * this.entries[0] - 2.0 * num3 * this.entries[3] + num4 * this.entries[1];
			double num8 = this.entries[3] * (num4 - num5) + num3 * (this.entries[1] - this.entries[0]);
			double num9 = num * this.entries[4] + num2 * this.entries[5];
			double num10 = num * this.entries[5] - num2 * this.entries[4];
			this.entries[0] = num6;
			this.entries[1] = num7;
			this.entries[3] = num8;
			this.entries[4] = num9;
			this.entries[5] = num10;
		}

		public void quatConjugate02(double c, double s)
		{
			double num = c * c - s * s;
			double num2 = 2.0 * s * c;
			double num3 = num2 * num;
			double num4 = num * num;
			double num5 = num2 * num2;
			double num6 = num4 * this.entries[0] - 2.0 * num3 * this.entries[4] + num5 * this.entries[2];
			double num7 = num5 * this.entries[0] + 2.0 * num3 * this.entries[4] + num4 * this.entries[2];
			double num8 = num * this.entries[3] - num2 * this.entries[5];
			double num9 = num3 * (this.entries[0] - this.entries[2]) + (num4 - num5) * this.entries[4];
			double num10 = num2 * this.entries[3] + num * this.entries[5];
			this.entries[0] = num6;
			this.entries[2] = num7;
			this.entries[3] = num8;
			this.entries[4] = num9;
			this.entries[5] = num10;
		}

		public void quatConjugate12(double c, double s)
		{
			double num = c * c - s * s;
			double num2 = 2.0 * s * c;
			double num3 = num2 * num;
			double num4 = num * num;
			double num5 = num2 * num2;
			double num6 = num4 * this.entries[1] + 2.0 * num3 * this.entries[5] + num5 * this.entries[2];
			double num7 = num5 * this.entries[1] - 2.0 * num3 * this.entries[5] + num4 * this.entries[2];
			double num8 = num * this.entries[3] + num2 * this.entries[4];
			double num9 = -num2 * this.entries[3] + num * this.entries[4];
			double num10 = (num4 - num5) * this.entries[5] + num3 * (this.entries[2] - this.entries[1]);
			this.entries[1] = num6;
			this.entries[2] = num7;
			this.entries[3] = num8;
			this.entries[4] = num9;
			this.entries[5] = num10;
		}

		public double[] entries = new double[6];
	}
}
