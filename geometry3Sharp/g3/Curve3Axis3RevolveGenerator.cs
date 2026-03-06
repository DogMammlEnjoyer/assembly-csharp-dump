using System;

namespace g3
{
	public class Curve3Axis3RevolveGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			int num = this.Curve.Length;
			int num2 = this.NoSharedVertices ? (this.Slices + 1) : this.Slices;
			int num3 = this.NoSharedVertices ? (this.Slices + 1) : 1;
			if (!this.Capped)
			{
				num3 = 0;
			}
			this.vertices = new VectorArray3d(num2 * num + 2 * num3, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num4 = (num - 1) * (2 * this.Slices);
			int num5 = this.Capped ? (2 * this.Slices) : 0;
			this.triangles = new IndexArray3i(num4 + num5);
			float num6 = (float)(6.283185307179586 / (double)this.Slices);
			Frame3f axis = this.Axis;
			for (int i = 0; i < num; i++)
			{
				Vector3d v = this.Curve[i];
				Vector3f v2 = axis.ToFrameP((Vector3f)v);
				float x = (float)i / (float)(num - 1);
				int num7 = i * num2;
				for (int j = 0; j < num2; j++)
				{
					float angleRad = (float)j * num6;
					Vector3f v3 = Quaternionf.AxisAngleR(Vector3f.AxisY, angleRad) * v2;
					Vector3d vector3d = axis.FromFrameP(v3);
					int i2 = num7 + j;
					this.vertices[i2] = vector3d;
					float y = (float)j / (float)num2;
					this.uv[i2] = new Vector2f(x, y);
					Vector3f value = (Vector3f)(vector3d - axis.Origin).Normalized;
					this.normals[i2] = value;
				}
			}
			int num8 = 0;
			for (int k = 0; k < num - 1; k++)
			{
				int num9 = k * num2;
				int num10 = num9 + num2;
				for (int l = 0; l < num2 - 1; l++)
				{
					this.triangles.Set(num8++, num9 + l, num9 + l + 1, num10 + l + 1, this.Clockwise);
					this.triangles.Set(num8++, num9 + l, num10 + l + 1, num10 + l, this.Clockwise);
				}
				if (!this.NoSharedVertices)
				{
					this.triangles.Set(num8++, num10 - 1, num9, num10, this.Clockwise);
					this.triangles.Set(num8++, num10 - 1, num10, num10 + num2 - 1, this.Clockwise);
				}
			}
			if (this.Capped)
			{
				Vector3d vector3d2 = Vector3d.Zero;
				Vector3d vector3d3 = Vector3d.Zero;
				for (int m = 0; m < this.Slices; m++)
				{
					vector3d2 += this.vertices[m];
					vector3d3 += this.vertices[(num - 1) * num2 + m];
				}
				vector3d2 /= (double)this.Slices;
				vector3d3 /= (double)this.Slices;
				Frame3f frame3f = axis;
				frame3f.Origin = (Vector3f)vector3d2;
				Frame3f frame3f2 = axis;
				frame3f2.Origin = (Vector3f)vector3d3;
				int num11 = num * num2;
				this.vertices[num11] = frame3f.Origin;
				this.uv[num11] = new Vector2f(0.5f, 0.5f);
				this.normals[num11] = -frame3f.Z;
				this.startCapCenterIndex = num11;
				int num12 = num11 + 1;
				this.vertices[num12] = frame3f2.Origin;
				this.uv[num12] = new Vector2f(0.5f, 0.5f);
				this.normals[num12] = frame3f2.Z;
				this.endCapCenterIndex = num12;
				if (this.NoSharedVertices)
				{
					int num13 = 0;
					int num14 = num12 + 1;
					for (int n = 0; n < this.Slices; n++)
					{
						this.vertices[num14 + n] = this.vertices[num13 + n];
						float num15 = (float)n * num6;
						double num16 = Math.Cos((double)num15);
						double num17 = Math.Sin((double)num15);
						this.uv[num14 + n] = new Vector2f(0.5 * (1.0 + num16), 0.5 * (1.0 + num17));
						this.normals[num14 + n] = this.normals[num11];
					}
					base.append_disc(this.Slices, num11, num14, true, this.Clockwise, ref num8, -1);
					int num18 = num2 * (num - 1);
					int num19 = num14 + this.Slices;
					for (int num20 = 0; num20 < this.Slices; num20++)
					{
						this.vertices[num19 + num20] = this.vertices[num18 + num20];
						float num21 = (float)num20 * num6;
						double num22 = Math.Cos((double)num21);
						double num23 = Math.Sin((double)num21);
						this.uv[num19 + num20] = new Vector2f(0.5 * (1.0 + num22), 0.5 * (1.0 + num23));
						this.normals[num19 + num20] = this.normals[num12];
					}
					base.append_disc(this.Slices, num12, num19, true, !this.Clockwise, ref num8, -1);
				}
				else
				{
					base.append_disc(this.Slices, num11, 0, true, this.Clockwise, ref num8, -1);
					base.append_disc(this.Slices, num12, num2 * (num - 1), true, !this.Clockwise, ref num8, -1);
				}
			}
			return this;
		}

		public Vector3d[] Curve;

		public Frame3f Axis = Frame3f.Identity;

		public int RevolveAxis = 1;

		public bool Capped = true;

		public int Slices = 16;

		public bool NoSharedVertices = true;

		public int startCapCenterIndex = -1;

		public int endCapCenterIndex = -1;
	}
}
