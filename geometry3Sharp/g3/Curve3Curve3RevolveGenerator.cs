using System;

namespace g3
{
	public class Curve3Curve3RevolveGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			double num = CurveUtils.ArcLength(this.Curve, false);
			SampledArcLengthParam sampledArcLengthParam = new SampledArcLengthParam(this.Axis, this.Axis.Length);
			double num2 = sampledArcLengthParam.ArcLength / num;
			int num3 = this.Curve.Length;
			int num4 = this.NoSharedVertices ? (this.Slices + 1) : this.Slices;
			int num5 = this.NoSharedVertices ? (this.Slices + 1) : 1;
			if (!this.Capped)
			{
				num5 = 0;
			}
			this.vertices = new VectorArray3d(num4 * num3 + 2 * num5, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num6 = (num3 - 1) * (2 * this.Slices);
			int num7 = this.Capped ? (2 * this.Slices) : 0;
			this.triangles = new IndexArray3i(num6 + num7);
			float num8 = (float)(6.283185307179586 / (double)this.Slices);
			double num9 = 0.0;
			CurveSample curveSample = sampledArcLengthParam.Sample(num9);
			Frame3f frame3f = new Frame3f((Vector3f)curveSample.position, (Vector3f)curveSample.tangent, 1);
			Frame3f frame3f2 = frame3f;
			for (int i = 0; i < num3; i++)
			{
				if (i > 0)
				{
					num9 += (this.Curve[i] - this.Curve[i - 1]).Length;
					curveSample = sampledArcLengthParam.Sample(num9 * num2);
					frame3f2.Origin = (Vector3f)curveSample.position;
					frame3f2.AlignAxis(1, (Vector3f)curveSample.tangent);
				}
				Vector3d v = this.Curve[i];
				Vector3f v2 = frame3f2.ToFrameP((Vector3f)v);
				float x = (float)i / (float)(num3 - 1);
				int num10 = i * num4;
				for (int j = 0; j < num4; j++)
				{
					float angleRad = (float)j * num8;
					Vector3f v3 = Quaternionf.AxisAngleR(Vector3f.AxisY, angleRad) * v2;
					Vector3d vector3d = frame3f2.FromFrameP(v3);
					int i2 = num10 + j;
					this.vertices[i2] = vector3d;
					float y = (float)j / (float)num4;
					this.uv[i2] = new Vector2f(x, y);
					Vector3f value = (Vector3f)(vector3d - frame3f2.Origin).Normalized;
					this.normals[i2] = value;
				}
			}
			int num11 = 0;
			for (int k = 0; k < num3 - 1; k++)
			{
				int num12 = k * num4;
				int num13 = num12 + num4;
				for (int l = 0; l < num4 - 1; l++)
				{
					this.triangles.Set(num11++, num12 + l, num12 + l + 1, num13 + l + 1, this.Clockwise);
					this.triangles.Set(num11++, num12 + l, num13 + l + 1, num13 + l, this.Clockwise);
				}
				if (!this.NoSharedVertices)
				{
					this.triangles.Set(num11++, num13 - 1, num12, num13, this.Clockwise);
					this.triangles.Set(num11++, num13 - 1, num13, num13 + num4 - 1, this.Clockwise);
				}
			}
			if (this.Capped)
			{
				Vector3d vector3d2 = Vector3d.Zero;
				Vector3d vector3d3 = Vector3d.Zero;
				for (int m = 0; m < this.Slices; m++)
				{
					vector3d2 += this.vertices[m];
					vector3d3 += this.vertices[(num3 - 1) * num4 + m];
				}
				vector3d2 /= (double)this.Slices;
				vector3d3 /= (double)this.Slices;
				Frame3f frame3f3 = frame3f;
				frame3f3.Origin = (Vector3f)vector3d2;
				Frame3f frame3f4 = frame3f2;
				frame3f4.Origin = (Vector3f)vector3d3;
				int num14 = num3 * num4;
				this.vertices[num14] = frame3f3.Origin;
				this.uv[num14] = new Vector2f(0.5f, 0.5f);
				this.normals[num14] = -frame3f3.Z;
				this.startCapCenterIndex = num14;
				int num15 = num14 + 1;
				this.vertices[num15] = frame3f4.Origin;
				this.uv[num15] = new Vector2f(0.5f, 0.5f);
				this.normals[num15] = frame3f4.Z;
				this.endCapCenterIndex = num15;
				if (this.NoSharedVertices)
				{
					int num16 = 0;
					int num17 = num15 + 1;
					for (int n = 0; n < this.Slices; n++)
					{
						this.vertices[num17 + n] = this.vertices[num16 + n];
						float num18 = (float)n * num8;
						double num19 = Math.Cos((double)num18);
						double num20 = Math.Sin((double)num18);
						this.uv[num17 + n] = new Vector2f(0.5 * (1.0 + num19), 0.5 * (1.0 + num20));
						this.normals[num17 + n] = this.normals[num14];
					}
					base.append_disc(this.Slices, num14, num17, true, this.Clockwise, ref num11, -1);
					int num21 = num4 * (num3 - 1);
					int num22 = num17 + this.Slices;
					for (int num23 = 0; num23 < this.Slices; num23++)
					{
						this.vertices[num22 + num23] = this.vertices[num21 + num23];
						float num24 = (float)num23 * num8;
						double num25 = Math.Cos((double)num24);
						double num26 = Math.Sin((double)num24);
						this.uv[num22 + num23] = new Vector2f(0.5 * (1.0 + num25), 0.5 * (1.0 + num26));
						this.normals[num22 + num23] = this.normals[num15];
					}
					base.append_disc(this.Slices, num15, num22, true, !this.Clockwise, ref num11, -1);
				}
				else
				{
					base.append_disc(this.Slices, num14, 0, true, this.Clockwise, ref num11, -1);
					base.append_disc(this.Slices, num15, num4 * (num3 - 1), true, !this.Clockwise, ref num11, -1);
				}
			}
			return this;
		}

		public Vector3d[] Curve;

		public Vector3d[] Axis;

		public bool Capped = true;

		public int Slices = 16;

		public bool NoSharedVertices = true;

		public int startCapCenterIndex = -1;

		public int endCapCenterIndex = -1;
	}
}
