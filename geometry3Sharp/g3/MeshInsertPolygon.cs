using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshInsertPolygon
	{
		public bool Insert()
		{
			this.OuterInsert = new MeshInsertUVPolyCurve(this.Mesh, this.Polygon.Outer);
			if (!this.OuterInsert.Apply() || this.OuterInsert.Loops.Count == 0)
			{
				return false;
			}
			if (this.SimplifyInsertion)
			{
				this.OuterInsert.Simplify();
			}
			this.HoleInserts = new List<MeshInsertUVPolyCurve>(this.Polygon.Holes.Count);
			for (int i = 0; i < this.Polygon.Holes.Count; i++)
			{
				MeshInsertUVPolyCurve meshInsertUVPolyCurve = new MeshInsertUVPolyCurve(this.Mesh, this.Polygon.Holes[i]);
				meshInsertUVPolyCurve.Apply();
				if (this.SimplifyInsertion)
				{
					meshInsertUVPolyCurve.Simplify();
				}
				this.HoleInserts.Add(meshInsertUVPolyCurve);
			}
			int num = -1;
			EdgeLoop edgeLoop = this.OuterInsert.Loops[0];
			for (int j = 0; j < edgeLoop.EdgeCount; j++)
			{
				if (this.Mesh.IsEdge(edgeLoop.Edges[j]))
				{
					Index2i edgeT = this.Mesh.GetEdgeT(edgeLoop.Edges[j]);
					Vector3d triCentroid = this.Mesh.GetTriCentroid(edgeT.a);
					bool flag = this.Polygon.Outer.Contains(triCentroid.xy);
					Vector3d triCentroid2 = this.Mesh.GetTriCentroid(edgeT.b);
					bool flag2 = this.Polygon.Outer.Contains(triCentroid2.xy);
					if (flag && !flag2)
					{
						num = edgeT.a;
						break;
					}
					if (flag2 && !flag)
					{
						num = edgeT.b;
						break;
					}
				}
			}
			if (num == -1)
			{
				throw new Exception("MeshPolygonsInserter: could not find seed triangle!");
			}
			this.InsertedPolygonEdges = new HashSet<int>(edgeLoop.Edges);
			foreach (MeshInsertUVPolyCurve meshInsertUVPolyCurve2 in this.HoleInserts)
			{
				foreach (int item in meshInsertUVPolyCurve2.Loops[0].Edges)
				{
					this.InsertedPolygonEdges.Add(item);
				}
			}
			this.InteriorTriangles = new MeshFaceSelection(this.Mesh);
			this.InteriorTriangles.FloodFill(num, null, (int eid) => !this.InsertedPolygonEdges.Contains(eid));
			return true;
		}

		public DMesh3 Mesh;

		public GeneralPolygon2d Polygon;

		public bool SimplifyInsertion = true;

		public MeshInsertUVPolyCurve OuterInsert;

		public List<MeshInsertUVPolyCurve> HoleInserts;

		public HashSet<int> InsertedPolygonEdges;

		public MeshFaceSelection InteriorTriangles;
	}
}
