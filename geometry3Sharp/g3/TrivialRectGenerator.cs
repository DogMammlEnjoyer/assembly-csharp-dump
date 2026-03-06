using System;

namespace g3
{
	public class TrivialRectGenerator : MeshGenerator
	{
		protected virtual Vector3d make_vertex(float x, float y)
		{
			Vector3d zero = Vector3d.Zero;
			zero[Math.Abs(this.IndicesMap.a) - 1] = (double)((this.IndicesMap.a < 0) ? (-(double)x) : x);
			zero[Math.Abs(this.IndicesMap.b) - 1] = (double)((this.IndicesMap.b < 0) ? (-(double)y) : y);
			return zero;
		}

		public override MeshGenerator Generate()
		{
			if (!MathUtil.InRange(this.IndicesMap.a, 1, 3) || !MathUtil.InRange(this.IndicesMap.b, 1, 3))
			{
				throw new Exception("TrivialRectGenerator: Invalid IndicesMap!");
			}
			this.vertices = new VectorArray3d(4, false);
			this.uv = new VectorArray2f(4);
			this.normals = new VectorArray3f(4);
			this.triangles = new IndexArray3i(2);
			this.vertices[0] = this.make_vertex(-this.Width / 2f, -this.Height / 2f);
			this.vertices[1] = this.make_vertex(this.Width / 2f, -this.Height / 2f);
			this.vertices[2] = this.make_vertex(this.Width / 2f, this.Height / 2f);
			this.vertices[3] = this.make_vertex(-this.Width / 2f, this.Height / 2f);
			this.normals[0] = (this.normals[1] = (this.normals[2] = (this.normals[3] = this.Normal)));
			float x = 0f;
			float x2 = 1f;
			float y = 0f;
			float y2 = 1f;
			if (this.UVMode != TrivialRectGenerator.UVModes.FullUVSquare)
			{
				if (this.Width > this.Height)
				{
					float num = this.Height / this.Width;
					if (this.UVMode == TrivialRectGenerator.UVModes.CenteredUVRectangle)
					{
						y = 0.5f - num / 2f;
						y2 = 0.5f + num / 2f;
					}
					else
					{
						y2 = num;
					}
				}
				else if (this.Height > this.Width)
				{
					float num2 = this.Width / this.Height;
					if (this.UVMode == TrivialRectGenerator.UVModes.CenteredUVRectangle)
					{
						x = 0.5f - num2 / 2f;
						x2 = 0.5f + num2 / 2f;
					}
					else
					{
						x2 = num2;
					}
				}
			}
			this.uv[0] = new Vector2f(x, y);
			this.uv[1] = new Vector2f(x2, y);
			this.uv[2] = new Vector2f(x2, y2);
			this.uv[3] = new Vector2f(x, y2);
			if (this.Clockwise)
			{
				this.triangles.Set(0, 0, 1, 2, false);
				this.triangles.Set(1, 0, 2, 3, false);
			}
			else
			{
				this.triangles.Set(0, 0, 2, 1, false);
				this.triangles.Set(1, 0, 3, 2, false);
			}
			return this;
		}

		public float Width = 1f;

		public float Height = 1f;

		public Vector3f Normal = Vector3f.AxisY;

		public Index2i IndicesMap = new Index2i(1, 3);

		public TrivialRectGenerator.UVModes UVMode;

		public enum UVModes
		{
			FullUVSquare,
			CenteredUVRectangle,
			BottomCornerUVRectangle
		}
	}
}
