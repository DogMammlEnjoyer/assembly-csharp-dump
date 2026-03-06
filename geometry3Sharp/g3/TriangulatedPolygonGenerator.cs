using System;

namespace g3
{
	public class TriangulatedPolygonGenerator : MeshGenerator
	{
		public override MeshGenerator Generate()
		{
			MeshInsertPolygon meshInsertPolygon;
			DMesh3 dmesh = new DMesh3(this.ComputeResult(out meshInsertPolygon), true, true, true, true);
			int vertexCount = dmesh.VertexCount;
			this.vertices = new VectorArray3d(vertexCount, false);
			this.uv = new VectorArray2f(vertexCount);
			this.normals = new VectorArray3f(vertexCount);
			for (int i = 0; i < vertexCount; i++)
			{
				this.vertices[i] = dmesh.GetVertex(i);
				this.uv[i] = dmesh.GetVertexUV(i);
				this.normals[i] = this.FixedNormal;
			}
			int triangleCount = dmesh.TriangleCount;
			this.triangles = new IndexArray3i(triangleCount);
			for (int j = 0; j < triangleCount; j++)
			{
				this.triangles[j] = dmesh.GetTriangle(j);
			}
			return this;
		}

		public DMesh3 ComputeResult(out MeshInsertPolygon insertion)
		{
			AxisAlignedBox2d bounds = this.Polygon.Bounds;
			double fRadius = 0.1 * bounds.DiagonalLength;
			bounds.Expand(fRadius);
			GriddedRectGenerator griddedRectGenerator;
			if (this.Subdivisions != 1)
			{
				(griddedRectGenerator = new GriddedRectGenerator()).EdgeVertices = this.Subdivisions;
			}
			else
			{
				griddedRectGenerator = new TrivialRectGenerator();
			}
			GriddedRectGenerator griddedRectGenerator2 = griddedRectGenerator;
			griddedRectGenerator2.Width = (float)bounds.Width;
			griddedRectGenerator2.Height = (float)bounds.Height;
			griddedRectGenerator2.IndicesMap = new Index2i(1, 2);
			griddedRectGenerator2.UVMode = this.UVMode;
			griddedRectGenerator2.Clockwise = true;
			griddedRectGenerator2.Generate();
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			griddedRectGenerator2.MakeMesh(dmesh);
			GeneralPolygon2d generalPolygon2d = new GeneralPolygon2d(this.Polygon);
			Vector2d center = bounds.Center;
			generalPolygon2d.Translate(-center);
			MeshInsertPolygon meshInsertPolygon = new MeshInsertPolygon
			{
				Mesh = dmesh,
				Polygon = generalPolygon2d
			};
			if (!meshInsertPolygon.Insert())
			{
				throw new Exception("TriangulatedPolygonGenerator: failed to Insert()");
			}
			MeshFaceSelection selected = meshInsertPolygon.InteriorTriangles;
			new MeshEditor(dmesh).RemoveTriangles((int tid) => !selected.IsSelected(tid), true);
			Vector3d v = new Vector3d(center.x, center.y, 0.0);
			MeshTransforms.Translate(dmesh, v);
			insertion = meshInsertPolygon;
			return dmesh;
		}

		public GeneralPolygon2d Polygon;

		public Vector3f FixedNormal = Vector3f.AxisZ;

		public TrivialRectGenerator.UVModes UVMode;

		public int Subdivisions = 1;
	}
}
