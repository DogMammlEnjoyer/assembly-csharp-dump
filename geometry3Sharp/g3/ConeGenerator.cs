using System;

namespace g3
{
	public class ConeGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			bool flag = this.EndAngleDeg - this.StartAngleDeg > 359.99f;
			int num = (this.NoSharedVertices && flag) ? (this.Slices + 1) : this.Slices;
			int num2 = this.NoSharedVertices ? num : 1;
			int num3 = this.NoSharedVertices ? (this.Slices + 1) : 1;
			int num4 = (this.NoSharedVertices && !flag) ? 6 : 0;
			this.vertices = new VectorArray3d(num + num2 + num3 + num4, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			int num5 = this.NoSharedVertices ? (2 * this.Slices) : this.Slices;
			int slices = this.Slices;
			int num6 = (!flag) ? 2 : 0;
			this.triangles = new IndexArray3i(num5 + slices + num6);
			float num7 = (this.EndAngleDeg - this.StartAngleDeg) * 0.017453292f;
			float num8 = this.StartAngleDeg * 0.017453292f;
			float num9 = flag ? (num7 / (float)this.Slices) : (num7 / (float)(this.Slices - 1));
			for (int i = 0; i < num; i++)
			{
				float num10 = num8 + (float)i * num9;
				double num11 = Math.Cos((double)num10);
				double num12 = Math.Sin((double)num10);
				this.vertices[i] = new Vector3d((double)this.BaseRadius * num11, 0.0, (double)this.BaseRadius * num12);
				this.uv[i] = new Vector2f(0.5 * (1.0 + num11), 0.5 * (1.0 + num12));
				Vector3f value = new Vector3f(num11 * (double)this.Height, (double)(this.BaseRadius / this.Height), num12 * (double)this.Height);
				value.Normalize(1.1920929E-07f);
				this.normals[i] = value;
				if (this.NoSharedVertices)
				{
					this.vertices[num + i] = new Vector3d(0.0, (double)this.Height, 0.0);
					this.uv[num + i] = new Vector2f(0.5f, 0.5f);
					this.normals[num + i] = value;
				}
			}
			if (!this.NoSharedVertices)
			{
				this.vertices[num] = new Vector3d(0.0, (double)this.Height, 0.0);
				this.normals[num] = Vector3f.AxisY;
				this.uv[num] = new Vector2f(0.5f, 0.5f);
			}
			int num13 = 0;
			if (this.NoSharedVertices)
			{
				for (int j = 0; j < num - 1; j++)
				{
					this.triangles.Set(num13++, j, j + 1, num + j + 1, this.Clockwise);
					this.triangles.Set(num13++, j, num + j + 1, num + j, this.Clockwise);
				}
			}
			else
			{
				base.append_disc(this.Slices, num, 0, flag, !this.Clockwise, ref num13, -1);
			}
			int num14 = num + num2;
			this.vertices[num14] = new Vector3d(0.0, 0.0, 0.0);
			this.uv[num14] = new Vector2f(0.5f, 0.5f);
			this.normals[num14] = new Vector3f(0f, -1f, 0f);
			if (this.NoSharedVertices)
			{
				int num15 = num14 + 1;
				for (int k = 0; k < this.Slices; k++)
				{
					float num16 = num8 + (float)k * num9;
					double num17 = Math.Cos((double)num16);
					double num18 = Math.Sin((double)num16);
					this.vertices[num15 + k] = new Vector3d((double)this.BaseRadius * num17, 0.0, (double)this.BaseRadius * num18);
					this.uv[num15 + k] = new Vector2f(0.5 * (1.0 + num17), 0.5 * (1.0 + num18));
					this.normals[num15 + k] = -Vector3f.AxisY;
				}
				base.append_disc(this.Slices, num14, num15, flag, this.Clockwise, ref num13, -1);
				if (!flag)
				{
					int num19 = num15 + this.Slices;
					VectorArray3d vertices = this.vertices;
					int i2 = num19;
					Vector3d value2 = this.vertices[num19 + 4] = this.vertices[num14];
					vertices[i2] = value2;
					VectorArray3d vertices2 = this.vertices;
					int i3 = num19 + 1;
					VectorArray3d vertices3 = this.vertices;
					int i4 = num19 + 3;
					value2 = new Vector3d(0.0, (double)this.Height, 0.0);
					vertices3[i4] = value2;
					vertices2[i3] = value2;
					this.vertices[num19 + 2] = this.vertices[0];
					this.vertices[num19 + 5] = this.vertices[num - 1];
					this.normals[num19] = (this.normals[num19 + 1] = (this.normals[num19 + 2] = base.estimate_normal(num19, num19 + 1, num19 + 2)));
					this.normals[num19 + 3] = (this.normals[num19 + 4] = (this.normals[num19 + 5] = base.estimate_normal(num19 + 3, num19 + 4, num19 + 5)));
					VectorArray2f uv = this.uv;
					int i5 = num19;
					VectorArray2f uv2 = this.uv;
					int i6 = num19 + 4;
					Vector2f value3 = new Vector2f(0f, 0f);
					uv2[i6] = value3;
					uv[i5] = value3;
					VectorArray2f uv3 = this.uv;
					int i7 = num19 + 1;
					VectorArray2f uv4 = this.uv;
					int i8 = num19 + 3;
					value3 = new Vector2f(0f, 1f);
					uv4[i8] = value3;
					uv3[i7] = value3;
					VectorArray2f uv5 = this.uv;
					int i9 = num19 + 2;
					VectorArray2f uv6 = this.uv;
					int i10 = num19 + 5;
					value3 = new Vector2f(1f, 0f);
					uv6[i10] = value3;
					uv5[i9] = value3;
					this.triangles.Set(num13++, num19, num19 + 1, num19 + 2, !this.Clockwise);
					this.triangles.Set(num13++, num19 + 3, num19 + 4, num19 + 5, !this.Clockwise);
				}
			}
			else
			{
				base.append_disc(this.Slices, num14, 0, flag, this.Clockwise, ref num13, -1);
				if (!flag)
				{
					this.triangles.Set(num13++, num14, num, 0, !this.Clockwise);
					this.triangles.Set(num13++, num14, num, num - 1, this.Clockwise);
				}
			}
			return this;
		}

		public float BaseRadius = 1f;

		public float Height = 1f;

		public float StartAngleDeg;

		public float EndAngleDeg = 360f;

		public int Slices = 16;

		public bool NoSharedVertices;
	}
}
