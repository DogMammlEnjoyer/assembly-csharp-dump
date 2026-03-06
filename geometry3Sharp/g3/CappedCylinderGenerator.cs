using System;

namespace g3
{
	public class CappedCylinderGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			bool flag = this.EndAngleDeg - this.StartAngleDeg > 359.99f;
			int num = (this.NoSharedVertices && flag) ? (this.Slices + 1) : this.Slices;
			int num2 = this.NoSharedVertices ? (this.Slices + 1) : 1;
			int num3 = (this.NoSharedVertices && !flag) ? 8 : 0;
			this.vertices = new VectorArray3d(2 * num + 2 * num2 + num3, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num4 = 2 * this.Slices;
			int num5 = 2 * this.Slices;
			int num6 = (!flag) ? 4 : 0;
			this.triangles = new IndexArray3i(num4 + num5 + num6);
			this.groups = new int[this.triangles.Count];
			float num7 = (this.EndAngleDeg - this.StartAngleDeg) * 0.017453292f;
			float num8 = this.StartAngleDeg * 0.017453292f;
			float num9 = flag ? (num7 / (float)this.Slices) : (num7 / (float)(this.Slices - 1));
			for (int i = 0; i < num; i++)
			{
				float num10 = num8 + (float)i * num9;
				double num11 = Math.Cos((double)num10);
				double num12 = Math.Sin((double)num10);
				this.vertices[i] = new Vector3d((double)this.BaseRadius * num11, 0.0, (double)this.BaseRadius * num12);
				this.vertices[num + i] = new Vector3d((double)this.TopRadius * num11, (double)this.Height, (double)this.TopRadius * num12);
				float x = (float)i / (float)this.Slices;
				this.uv[i] = new Vector2f(x, 0f);
				this.uv[num + i] = new Vector2f(x, 1f);
				Vector3f value = new Vector3f((float)num11, 0f, (float)num12);
				value.Normalize(1.1920929E-07f);
				this.normals[i] = (this.normals[num + i] = value);
			}
			int num13 = 0;
			for (int j = 0; j < num - 1; j++)
			{
				this.groups[num13] = 1;
				this.triangles.Set(num13++, j, j + 1, num + j + 1, this.Clockwise);
				this.groups[num13] = 1;
				this.triangles.Set(num13++, j, num + j + 1, num + j, this.Clockwise);
			}
			if (flag && !this.NoSharedVertices)
			{
				this.groups[num13] = 1;
				this.triangles.Set(num13++, num - 1, 0, num, this.Clockwise);
				this.groups[num13] = 1;
				this.triangles.Set(num13++, num - 1, num, 2 * num - 1, this.Clockwise);
			}
			int num14 = 2 * num;
			this.vertices[num14] = new Vector3d(0.0, 0.0, 0.0);
			this.uv[num14] = new Vector2f(0.5f, 0.5f);
			this.normals[num14] = new Vector3f(0f, -1f, 0f);
			int num15 = 2 * num + 1;
			this.vertices[num15] = new Vector3d(0.0, (double)this.Height, 0.0);
			this.uv[num15] = new Vector2f(0.5f, 0.5f);
			this.normals[num15] = new Vector3f(0f, 1f, 0f);
			if (this.NoSharedVertices)
			{
				int num16 = 2 * num + 2;
				for (int k = 0; k < this.Slices; k++)
				{
					float num17 = num8 + (float)k * num9;
					double num18 = Math.Cos((double)num17);
					double num19 = Math.Sin((double)num17);
					this.vertices[num16 + k] = new Vector3d((double)this.BaseRadius * num18, 0.0, (double)this.BaseRadius * num19);
					this.uv[num16 + k] = new Vector2f(0.5 * (1.0 + num18), 0.5 * (1.0 + num19));
					this.normals[num16 + k] = -Vector3f.AxisY;
				}
				base.append_disc(this.Slices, num14, num16, flag, this.Clockwise, ref num13, 2);
				int num20 = 2 * num + 2 + this.Slices;
				for (int l = 0; l < this.Slices; l++)
				{
					float num21 = num8 + (float)l * num9;
					double num22 = Math.Cos((double)num21);
					double num23 = Math.Sin((double)num21);
					this.vertices[num20 + l] = new Vector3d((double)this.TopRadius * num22, (double)this.Height, (double)this.TopRadius * num23);
					this.uv[num20 + l] = new Vector2f(0.5 * (1.0 + num22), 0.5 * (1.0 + num23));
					this.normals[num20 + l] = Vector3f.AxisY;
				}
				base.append_disc(this.Slices, num15, num20, flag, !this.Clockwise, ref num13, 3);
				if (!flag)
				{
					int num24 = 2 * num + 2 + 2 * this.Slices;
					this.vertices[num24] = (this.vertices[num24 + 5] = this.vertices[num14]);
					this.vertices[num24 + 1] = (this.vertices[num24 + 4] = this.vertices[num15]);
					this.vertices[num24 + 2] = this.vertices[num];
					this.vertices[num24 + 3] = this.vertices[0];
					this.vertices[num24 + 6] = this.vertices[num - 1];
					this.vertices[num24 + 7] = this.vertices[2 * num - 1];
					this.normals[num24] = (this.normals[num24 + 1] = (this.normals[num24 + 2] = (this.normals[num24 + 3] = base.estimate_normal(num24, num24 + 1, num24 + 2))));
					this.normals[num24 + 4] = (this.normals[num24 + 5] = (this.normals[num24 + 6] = (this.normals[num24 + 7] = base.estimate_normal(num24 + 4, num24 + 5, num24 + 6))));
					VectorArray2f uv = this.uv;
					int i2 = num24;
					VectorArray2f uv2 = this.uv;
					int i3 = num24 + 5;
					Vector2f value2 = new Vector2f(0f, 0f);
					uv2[i3] = value2;
					uv[i2] = value2;
					VectorArray2f uv3 = this.uv;
					int i4 = num24 + 1;
					VectorArray2f uv4 = this.uv;
					int i5 = num24 + 4;
					value2 = new Vector2f(0f, 1f);
					uv4[i5] = value2;
					uv3[i4] = value2;
					VectorArray2f uv5 = this.uv;
					int i6 = num24 + 2;
					VectorArray2f uv6 = this.uv;
					int i7 = num24 + 7;
					value2 = new Vector2f(1f, 1f);
					uv6[i7] = value2;
					uv5[i6] = value2;
					VectorArray2f uv7 = this.uv;
					int i8 = num24 + 3;
					VectorArray2f uv8 = this.uv;
					int i9 = num24 + 6;
					value2 = new Vector2f(1f, 0f);
					uv8[i9] = value2;
					uv7[i8] = value2;
					base.append_rectangle(num24, num24 + 1, num24 + 2, num24 + 3, !this.Clockwise, ref num13, 4);
					base.append_rectangle(num24 + 4, num24 + 5, num24 + 6, num24 + 7, !this.Clockwise, ref num13, 5);
				}
			}
			else
			{
				base.append_disc(this.Slices, num14, 0, flag, this.Clockwise, ref num13, 2);
				base.append_disc(this.Slices, num15, num, flag, !this.Clockwise, ref num13, 3);
				if (!flag)
				{
					base.append_rectangle(num14, 0, num, num15, this.Clockwise, ref num13, 4);
					base.append_rectangle(num - 1, num14, num15, 2 * num - 1, this.Clockwise, ref num13, 5);
				}
			}
			return this;
		}

		public float BaseRadius = 1f;

		public float TopRadius = 1f;

		public float Height = 1f;

		public float StartAngleDeg;

		public float EndAngleDeg = 360f;

		public int Slices = 16;

		public bool NoSharedVertices;
	}
}
