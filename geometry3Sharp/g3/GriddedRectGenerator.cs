using System;

namespace g3
{
	public class GriddedRectGenerator : TrivialRectGenerator
	{
		public override MeshGenerator Generate()
		{
			if (!MathUtil.InRange(this.IndicesMap.a, 1, 3) || !MathUtil.InRange(this.IndicesMap.b, 1, 3))
			{
				throw new Exception("GriddedRectGenerator: Invalid IndicesMap!");
			}
			int num = (this.EdgeVertices > 1) ? this.EdgeVertices : 2;
			int num2 = num - 1;
			int nCount = num * num;
			this.vertices = new VectorArray3d(nCount, false);
			this.uv = new VectorArray2f(this.vertices.Count);
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(2 * num2 * num2);
			this.groups = new int[this.triangles.Count];
			Vector3d vector3d = this.make_vertex(-this.Width / 2f, -this.Height / 2f);
			Vector3d vector3d2 = this.make_vertex(this.Width / 2f, -this.Height / 2f);
			Vector3d vector3d3 = this.make_vertex(this.Width / 2f, this.Height / 2f);
			Vector3d vector3d4 = this.make_vertex(-this.Width / 2f, this.Height / 2f);
			float x = 0f;
			float x2 = 1f;
			float y = 0f;
			float y2 = 1f;
			if (this.UVMode != TrivialRectGenerator.UVModes.FullUVSquare)
			{
				if (this.Width > this.Height)
				{
					float num3 = this.Height / this.Width;
					if (this.UVMode == TrivialRectGenerator.UVModes.CenteredUVRectangle)
					{
						y = 0.5f - num3 / 2f;
						y2 = 0.5f + num3 / 2f;
					}
					else
					{
						y2 = num3;
					}
				}
				else if (this.Height > this.Width)
				{
					float num4 = this.Width / this.Height;
					if (this.UVMode == TrivialRectGenerator.UVModes.CenteredUVRectangle)
					{
						x = 0.5f - num4 / 2f;
						x2 = 0.5f + num4 / 2f;
					}
					else
					{
						x2 = num4;
					}
				}
			}
			Vector2f vector2f = new Vector2f(x, y);
			Vector2f vector2f2 = new Vector2f(x2, y);
			Vector2f vector2f3 = new Vector2f(x2, y2);
			Vector2f vector2f4 = new Vector2f(x, y2);
			int num5 = 0;
			int num6 = 0;
			int num7 = num5;
			for (int i = 0; i < num; i++)
			{
				double num8 = (double)i / (double)(num - 1);
				for (int j = 0; j < num; j++)
				{
					double num9 = (double)j / (double)(num - 1);
					this.normals[num5] = this.Normal;
					this.uv[num5] = base.bilerp(ref vector2f, ref vector2f2, ref vector2f3, ref vector2f4, (float)num9, (float)num8);
					this.vertices[num5++] = base.bilerp(ref vector3d, ref vector3d2, ref vector3d3, ref vector3d4, num9, num8);
				}
			}
			for (int k = 0; k < num2; k++)
			{
				for (int l = 0; l < num2; l++)
				{
					int num10 = num7 + k * num + l;
					int num11 = num7 + (k + 1) * num + l;
					int c = num10 + 1;
					int num12 = num11 + 1;
					this.groups[num6] = 0;
					this.triangles.Set(num6++, num10, num12, c, this.Clockwise);
					this.groups[num6] = 0;
					this.triangles.Set(num6++, num10, num11, num12, this.Clockwise);
				}
			}
			return this;
		}

		public int EdgeVertices = 8;
	}
}
