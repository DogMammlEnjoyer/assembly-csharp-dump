using System;

namespace g3
{
	public class TrivialDiscGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			this.vertices = new VectorArray3d(this.Slices + 1, false);
			this.uv = new VectorArray2f(this.Slices + 1);
			this.normals = new VectorArray3f(this.Slices + 1);
			this.triangles = new IndexArray3i(this.Slices);
			int num = 0;
			this.vertices[num] = Vector3d.Zero;
			this.uv[num] = new Vector2f(0.5f, 0.5f);
			this.normals[num] = Vector3f.AxisY;
			num++;
			bool flag = this.EndAngleDeg - this.StartAngleDeg > 359.99f;
			float num2 = (this.EndAngleDeg - this.StartAngleDeg) * 0.017453292f;
			float num3 = this.StartAngleDeg * 0.017453292f;
			float num4 = flag ? (num2 / (float)this.Slices) : (num2 / (float)(this.Slices - 1));
			for (int i = 0; i < this.Slices; i++)
			{
				float num5 = num3 + (float)i * num4;
				double num6 = Math.Cos((double)num5);
				double num7 = Math.Sin((double)num5);
				this.vertices[num] = new Vector3d((double)this.Radius * num6, 0.0, (double)this.Radius * num7);
				this.uv[num] = new Vector2f(0.5 * (1.0 + num6), 0.5 * (1.0 + num7));
				this.normals[num] = Vector3f.AxisY;
				num++;
			}
			int num8 = 0;
			for (int j = 1; j < this.Slices; j++)
			{
				this.triangles.Set(num8++, j, 0, j + 1, this.Clockwise);
			}
			if (flag)
			{
				this.triangles.Set(num8++, this.Slices, 0, 1, this.Clockwise);
			}
			return this;
		}

		public float Radius = 1f;

		public float StartAngleDeg;

		public float EndAngleDeg = 360f;

		public int Slices = 32;
	}
}
