using System;

namespace g3
{
	public class PuncturedDiscGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			this.vertices = new VectorArray3d(2 * this.Slices, false);
			this.uv = new VectorArray2f(2 * this.Slices);
			this.normals = new VectorArray3f(2 * this.Slices);
			this.triangles = new IndexArray3i(2 * this.Slices);
			bool flag = this.EndAngleDeg - this.StartAngleDeg > 359.99f;
			float num = (this.EndAngleDeg - this.StartAngleDeg) * 0.017453292f;
			float num2 = this.StartAngleDeg * 0.017453292f;
			float num3 = flag ? (num / (float)this.Slices) : (num / (float)(this.Slices - 1));
			float num4 = this.InnerRadius / this.OuterRadius;
			for (int i = 0; i < this.Slices; i++)
			{
				float num5 = num2 + (float)i * num3;
				double num6 = Math.Cos((double)num5);
				double num7 = Math.Sin((double)num5);
				this.vertices[i] = new Vector3d((double)this.InnerRadius * num6, 0.0, (double)this.InnerRadius * num7);
				this.vertices[this.Slices + i] = new Vector3d((double)this.OuterRadius * num6, 0.0, (double)this.OuterRadius * num7);
				this.uv[i] = new Vector2f(0.5 * (1.0 + (double)num4 * num6), 0.5 * (1.0 + (double)num4 * num7));
				this.uv[this.Slices + i] = new Vector2f(0.5 * (1.0 + num6), 0.5 * (1.0 + num7));
				this.normals[i] = (this.normals[this.Slices + i] = Vector3f.AxisY);
			}
			int num8 = 0;
			for (int j = 0; j < this.Slices - 1; j++)
			{
				this.triangles.Set(num8++, j, j + 1, this.Slices + j + 1, this.Clockwise);
				this.triangles.Set(num8++, j, this.Slices + j + 1, this.Slices + j, this.Clockwise);
			}
			if (flag)
			{
				this.triangles.Set(num8++, this.Slices - 1, 0, this.Slices, this.Clockwise);
				this.triangles.Set(num8++, this.Slices - 1, this.Slices, 2 * this.Slices - 1, this.Clockwise);
			}
			return this;
		}

		public float OuterRadius = 1f;

		public float InnerRadius = 0.5f;

		public float StartAngleDeg;

		public float EndAngleDeg = 360f;

		public int Slices = 32;
	}
}
