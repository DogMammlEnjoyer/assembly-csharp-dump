using System;
using System.Collections.Generic;

namespace g3
{
	public class PlanarSpansFiller
	{
		public PlanarSpansFiller(DMesh3 mesh, IList<EdgeSpan> spans)
		{
			this.Mesh = mesh;
			this.FillSpans = new List<EdgeSpan>(spans);
			this.Bounds = AxisAlignedBox2d.Empty;
		}

		public void SetPlane(Vector3d origin, Vector3d normal)
		{
			this.PlaneOrigin = origin;
			this.PlaneNormal = normal;
			Vector3d.ComputeOrthogonalComplement(1, this.PlaneNormal, ref this.PlaneX, ref this.PlaneY);
		}

		public void SetPlane(Vector3d origin, Vector3d normal, Vector3d planeX, Vector3d planeY)
		{
			this.PlaneOrigin = origin;
			this.PlaneNormal = normal;
			this.PlaneX = planeX;
			this.PlaneY = planeY;
		}

		public bool Fill()
		{
			this.compute_polygon();
			Vector2d shiftOrigin = this.Bounds.Center;
			double scale = 1.0 / this.Bounds.MaxDim;
			this.SpansPoly.Translate(-shiftOrigin);
			this.SpansPoly.Scale(scale * Vector2d.One, Vector2d.Zero);
			new Dictionary<PlanarComplex.Element, int>();
			float num = 1.5f;
			int num2 = 0;
			if (this.FillTargetEdgeLen < 1.7976931348623157E+308 && this.FillTargetEdgeLen > 0.0)
			{
				int num3 = (int)((double)(num / (float)scale) / this.FillTargetEdgeLen) + 1;
				num2 = ((num3 <= 1) ? 0 : num3);
			}
			MeshGenerator meshGenerator;
			if (num2 == 0)
			{
				meshGenerator = new TrivialRectGenerator
				{
					IndicesMap = new Index2i(1, 2),
					Width = num,
					Height = num
				};
			}
			else
			{
				meshGenerator = new GriddedRectGenerator
				{
					IndicesMap = new Index2i(1, 2),
					Width = num,
					Height = num,
					EdgeVertices = num2
				};
			}
			DMesh3 dmesh = meshGenerator.Generate().MakeDMesh();
			dmesh.ReverseOrientation(true);
			int[] array = null;
			MeshInsertUVPolyCurve meshInsertUVPolyCurve = new MeshInsertUVPolyCurve(dmesh, this.SpansPoly);
			bool flag = meshInsertUVPolyCurve.Validate(9.999999974752427E-07 * scale) != ValidationStatus.Ok;
			bool flag2 = true;
			if (!flag && meshInsertUVPolyCurve.Apply())
			{
				meshInsertUVPolyCurve.Simplify();
				array = meshInsertUVPolyCurve.CurveVertices;
				flag2 = false;
			}
			if (flag2)
			{
				return false;
			}
			List<int> list = new List<int>();
			foreach (int num4 in dmesh.TriangleIndices())
			{
				Vector3d triCentroid = dmesh.GetTriCentroid(num4);
				if (!this.SpansPoly.Contains(triCentroid.xy))
				{
					list.Add(num4);
				}
			}
			foreach (int tID in list)
			{
				dmesh.RemoveTriangle(tID, true, false);
			}
			MeshTransforms.PerVertexTransform(dmesh, delegate(Vector3d v)
			{
				Vector2d vector2d = v.xy;
				vector2d /= scale;
				vector2d += shiftOrigin;
				return this.to3D(vector2d);
			});
			IndexMap mergeMapV = new IndexMap(true, -1);
			if (this.MergeFillBoundary && array != null)
			{
				throw new NotImplementedException("PlanarSpansFiller: merge fill boundary not implemented!");
			}
			int[] array2;
			new MeshEditor(this.Mesh).AppendMesh(dmesh, mergeMapV, out array2, this.Mesh.AllocateTriangleGroup());
			return true;
		}

		private void compute_polygon()
		{
			this.SpansPoly = new Polygon2d();
			for (int i = 0; i < this.FillSpans.Count; i++)
			{
				foreach (int vID in this.FillSpans[i].Vertices)
				{
					Vector2d v = this.to2D(this.Mesh.GetVertex(vID));
					this.SpansPoly.AppendVertex(v);
				}
			}
			this.Bounds = this.SpansPoly.Bounds;
		}

		private Vector2d to2D(Vector3d v)
		{
			Vector3d vector3d = v - this.PlaneOrigin;
			vector3d -= vector3d.Dot(this.PlaneNormal) * this.PlaneNormal;
			return new Vector2d(this.PlaneX.Dot(vector3d), this.PlaneY.Dot(vector3d));
		}

		private Vector3d to3D(Vector2d v)
		{
			return this.PlaneOrigin + this.PlaneX * v.x + this.PlaneY * v.y;
		}

		public DMesh3 Mesh;

		public Vector3d PlaneOrigin;

		public Vector3d PlaneNormal;

		public double FillTargetEdgeLen = double.MaxValue;

		public bool MergeFillBoundary;

		private Vector3d PlaneX;

		private Vector3d PlaneY;

		private List<EdgeSpan> FillSpans;

		private Polygon2d SpansPoly;

		private AxisAlignedBox2d Bounds;
	}
}
