using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class PointSplatsGenerator : MeshGenerator
	{
		public PointSplatsGenerator()
		{
			this.WantUVs = false;
		}

		public override MeshGenerator Generate()
		{
			int num = (this.PointIndicesCount == -1) ? this.PointIndices.Count<int>() : this.PointIndicesCount;
			this.vertices = new VectorArray3d(num * 3, false);
			this.uv = null;
			this.normals = new VectorArray3f(this.vertices.Count);
			this.triangles = new IndexArray3i(num);
			Matrix2f m = new Matrix2f(2.0943952f);
			Vector2f v = new Vector2f(0.0, this.Radius);
			Vector2f v2 = m * v;
			Vector2f v3 = m * v2;
			int num2 = 0;
			int num3 = 0;
			foreach (int arg in this.PointIndices)
			{
				Vector3d origin = this.PointF(arg);
				Vector3d setZ = this.NormalF(arg);
				Frame3f frame3f = new Frame3f(origin, setZ);
				this.triangles.Set(num3++, num2, num2 + 1, num2 + 2, this.Clockwise);
				this.vertices[num2++] = frame3f.FromPlaneUV(v, 2);
				this.vertices[num2++] = frame3f.FromPlaneUV(v2, 2);
				this.vertices[num2++] = frame3f.FromPlaneUV(v3, 2);
			}
			return this;
		}

		public static DMesh3 Generate(IList<int> indices, Func<int, Vector3d> PointF, Func<int, Vector3d> NormalF, double radius)
		{
			return new PointSplatsGenerator
			{
				PointIndices = indices,
				PointIndicesCount = indices.Count,
				PointF = PointF,
				NormalF = NormalF,
				Radius = radius
			}.Generate().MakeDMesh();
		}

		public IEnumerable<int> PointIndices;

		public int PointIndicesCount = -1;

		public Func<int, Vector3d> PointF;

		public Func<int, Vector3d> NormalF;

		public double Radius = 1.0;
	}
}
