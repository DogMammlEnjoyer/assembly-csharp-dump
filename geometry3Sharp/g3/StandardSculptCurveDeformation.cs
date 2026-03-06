using System;
using System.Collections;

namespace g3
{
	public class StandardSculptCurveDeformation : SculptCurveDeformation
	{
		public StandardSculptCurveDeformation()
		{
			this.NewV = new DVector<Vector3d>();
			this.NewV.resize(256);
			this.ModifiedV = new BitArray(256);
		}

		public override SculptCurveDeformation.DeformInfo Apply(Frame3f vNextPos)
		{
			Interval1d empty = Interval1d.Empty;
			int vertexCount = base.Curve.VertexCount;
			if (vertexCount > this.NewV.size)
			{
				this.NewV.resize(vertexCount);
			}
			if (vertexCount > this.ModifiedV.Length)
			{
				this.ModifiedV = new BitArray(2 * vertexCount);
			}
			this.ModifiedV.SetAll(false);
			bool flag = this.SmoothAlpha > 0.0 && this.SmoothIterations > 0;
			double num = base.Radius * base.Radius;
			if (this.DeformF != null)
			{
				for (int i = 0; i < vertexCount; i++)
				{
					double lengthSquared = (base.Curve[i] - this.vPreviousPos.Origin).LengthSquared;
					if (lengthSquared < num)
					{
						double arg = base.WeightFunc(Math.Sqrt(lengthSquared), base.Radius);
						Vector3d value = this.DeformF(i, arg);
						if (!flag)
						{
							if (i > 0)
							{
								empty.Contain(value.DistanceSquared(base.Curve[i - 1]));
							}
							if (i < vertexCount - 1)
							{
								empty.Contain(value.DistanceSquared(base.Curve[i + 1]));
							}
						}
						this.NewV[i] = value;
						this.ModifiedV[i] = true;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < this.SmoothIterations; j++)
				{
					int num2 = base.Curve.Closed ? 0 : 1;
					int num3 = base.Curve.Closed ? vertexCount : (vertexCount - 1);
					for (int k = num2; k < num3; k++)
					{
						Vector3d vector3d = this.ModifiedV[k] ? this.NewV[k] : base.Curve[k];
						double lengthSquared2 = (vector3d - this.vPreviousPos.Origin).LengthSquared;
						if (this.ModifiedV[k] || lengthSquared2 < num)
						{
							double num4 = this.SmoothAlpha * base.WeightFunc(Math.Sqrt(lengthSquared2), base.Radius);
							int num5 = (k == 0) ? (vertexCount - 1) : (k - 1);
							int num6 = (k + 1) % vertexCount;
							Vector3d v = this.ModifiedV[num5] ? this.NewV[num5] : base.Curve[num5];
							Vector3d v2 = this.ModifiedV[num6] ? this.NewV[num6] : base.Curve[num6];
							Vector3d v3 = (v + v2) * 0.5;
							this.NewV[k] = (1.0 - num4) * vector3d + num4 * v3;
							this.ModifiedV[k] = true;
							if (k > 0)
							{
								empty.Contain(this.NewV[k].DistanceSquared(base.Curve[k - 1]));
							}
							if (k < vertexCount - 1)
							{
								empty.Contain(this.NewV[k].DistanceSquared(base.Curve[k + 1]));
							}
						}
					}
				}
			}
			for (int l = 0; l < vertexCount; l++)
			{
				if (this.ModifiedV[l])
				{
					base.Curve[l] = this.NewV[l];
				}
			}
			return new SculptCurveDeformation.DeformInfo
			{
				bNoChange = false,
				minEdgeLenSqr = empty.a,
				maxEdgeLenSqr = empty.b
			};
		}

		public Func<int, double, Vector3d> DeformF;

		public double SmoothAlpha = 0.10000000149011612;

		public int SmoothIterations = 1;

		public DVector<Vector3d> NewV;

		public BitArray ModifiedV;
	}
}
