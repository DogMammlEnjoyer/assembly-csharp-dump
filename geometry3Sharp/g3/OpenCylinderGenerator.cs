using System;

namespace g3
{
	public class OpenCylinderGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			bool flag = this.EndAngleDeg - this.StartAngleDeg > 359.99f;
			int num = (this.NoSharedVertices && flag) ? (this.Slices + 1) : this.Slices;
			this.vertices = new VectorArray3d(2 * num, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(2 * this.Slices);
			float num2 = (this.EndAngleDeg - this.StartAngleDeg) * 0.017453292f;
			float num3 = this.StartAngleDeg * 0.017453292f;
			float num4 = flag ? (num2 / (float)this.Slices) : (num2 / (float)(this.Slices - 1));
			for (int i = 0; i < num; i++)
			{
				float num5 = num3 + (float)i * num4;
				double num6 = Math.Cos((double)num5);
				double num7 = Math.Sin((double)num5);
				this.vertices[i] = new Vector3d((double)this.BaseRadius * num6, 0.0, (double)this.BaseRadius * num7);
				this.vertices[num + i] = new Vector3d((double)this.TopRadius * num6, (double)this.Height, (double)this.TopRadius * num7);
				float x = (float)i / (float)this.Slices;
				this.uv[i] = new Vector2f(x, 0f);
				this.uv[num + i] = new Vector2f(x, 1f);
				Vector3f value = new Vector3f((float)num6, 0f, (float)num7);
				value.Normalize(1.1920929E-07f);
				this.normals[i] = (this.normals[num + i] = value);
			}
			int num8 = 0;
			for (int j = 0; j < num - 1; j++)
			{
				this.triangles.Set(num8++, j, j + 1, num + j + 1, this.Clockwise);
				this.triangles.Set(num8++, j, num + j + 1, num + j, this.Clockwise);
			}
			if (flag && !this.NoSharedVertices)
			{
				this.triangles.Set(num8++, num - 1, 0, num, this.Clockwise);
				this.triangles.Set(num8++, num - 1, num, 2 * num - 1, this.Clockwise);
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
