using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class MeshInsertProjectedPolygon
	{
		public MeshInsertProjectedPolygon(DMesh3 mesh, Polygon2d poly, Frame3f frame, int seedTri)
		{
			this.Mesh = mesh;
			this.Polygon = new Polygon2d(poly);
			this.ProjectFrame = frame;
			this.SeedTriangle = seedTri;
		}

		public MeshInsertProjectedPolygon(DMesh3 mesh, DCurve3 polygon3, Frame3f frame, int seedTri)
		{
			if (!polygon3.Closed)
			{
				throw new Exception("MeshInsertPolyCurve(): only closed polygon3 supported for now");
			}
			this.Mesh = mesh;
			this.ProjectFrame = frame;
			this.SeedTriangle = seedTri;
			this.Polygon = new Polygon2d();
			foreach (Vector3d v in polygon3.Vertices)
			{
				Vector2f v2 = frame.ToPlaneUV((Vector3f)v, 2);
				this.Polygon.AppendVertex(v2);
			}
		}

		public virtual ValidationStatus Validate()
		{
			if (!this.Mesh.IsTriangle(this.SeedTriangle))
			{
				return ValidationStatus.NotATriangle;
			}
			return ValidationStatus.Ok;
		}

		public bool Insert()
		{
			Func<int, bool> func = delegate(int vid)
			{
				Vector3d vertex2 = this.Mesh.GetVertex(vid);
				Vector2f v = this.ProjectFrame.ToPlaneUV((Vector3f)vertex2, 2);
				return this.Polygon.Contains(v);
			};
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(this.Mesh);
			Index3i triangle = this.Mesh.GetTriangle(this.SeedTriangle);
			List<int> list = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				if (func(triangle[i]))
				{
					list.Add(triangle[i]);
				}
			}
			if (list.Count == 0)
			{
				list.Add(triangle.a);
				list.Add(triangle.b);
				list.Add(triangle.c);
			}
			meshVertexSelection.FloodFill(list.ToArray(), func);
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh, meshVertexSelection, 1);
			meshFaceSelection.ExpandToOneRingNeighbours(null);
			meshFaceSelection.FillEars(true);
			RegionOperator regionOperator = new RegionOperator(this.Mesh, meshFaceSelection, null);
			DMesh3 subMesh = regionOperator.Region.SubMesh;
			Vector3d[] initialPositions = new Vector3d[subMesh.MaxVertexID];
			MeshTransforms.PerVertexTransform(subMesh, subMesh.VertexIndices(), delegate(Vector3d v, int vid)
			{
				Vector2f vector2f = this.ProjectFrame.ToPlaneUV((Vector3f)v, 2);
				initialPositions[vid] = v;
				return new Vector3d((double)vector2f.x, (double)vector2f.y, 0.0);
			});
			DMesh3 dmesh = new DMesh3(subMesh, false, true, true, true);
			DMeshAABBTree3 dmeshAABBTree = new DMeshAABBTree3(dmesh, true);
			MeshInsertUVPolyCurve meshInsertUVPolyCurve = new MeshInsertUVPolyCurve(subMesh, this.Polygon);
			if (!meshInsertUVPolyCurve.Apply())
			{
				throw new Exception("insertUV.Apply() failed");
			}
			if (this.SimplifyInsertion)
			{
				meshInsertUVPolyCurve.Simplify();
			}
			int[] curveVertices = meshInsertUVPolyCurve.CurveVertices;
			EdgeLoop insertedLoop = null;
			if (meshInsertUVPolyCurve.Loops.Count == 1)
			{
				insertedLoop = meshInsertUVPolyCurve.Loops[0];
			}
			List<int> list2 = new List<int>();
			foreach (int num in subMesh.TriangleIndices())
			{
				Vector3d triCentroid = subMesh.GetTriCentroid(num);
				if (this.Polygon.Contains(triCentroid.xy))
				{
					list2.Add(num);
				}
			}
			if (this.RemovePolygonInterior)
			{
				new MeshEditor(subMesh).RemoveTriangles(list2, true);
				this.InteriorTriangles = null;
			}
			else
			{
				this.InteriorTriangles = list2.ToArray();
			}
			Vector3d zero = Vector3d.Zero;
			Vector3d zero2 = Vector3d.Zero;
			Vector3d zero3 = Vector3d.Zero;
			foreach (int vID in subMesh.VertexIndices())
			{
				Vector3d vertex = subMesh.GetVertex(vID);
				int tID = dmeshAABBTree.FindNearestTriangle(vertex, double.MaxValue);
				Index3i triangle2 = dmesh.GetTriangle(tID);
				dmesh.GetTriVertices(tID, ref zero, ref zero2, ref zero3);
				Vector3d vector3d = MathUtil.BarycentricCoords(ref vertex, ref zero, ref zero2, ref zero3);
				Vector3d vNewPos = vector3d.x * initialPositions[triangle2.a] + vector3d.y * initialPositions[triangle2.b] + vector3d.z * initialPositions[triangle2.c];
				subMesh.SetVertex(vID, vNewPos);
			}
			return this.BackPropagate(regionOperator, curveVertices, insertedLoop);
		}

		protected virtual bool BackPropagate(RegionOperator regionOp, int[] insertedPolyVerts, EdgeLoop insertedLoop)
		{
			bool flag = regionOp.BackPropropagate(true);
			if (flag)
			{
				this.ModifiedRegion = regionOp;
				IndexUtil.Apply(insertedPolyVerts, regionOp.ReinsertSubToBaseMapV);
				this.InsertedPolygonVerts = insertedPolyVerts;
				if (insertedLoop != null)
				{
					this.InsertedLoop = MeshIndexUtil.MapLoopViaVertexMap(regionOp.ReinsertSubToBaseMapV, regionOp.Region.SubMesh, regionOp.Region.BaseMesh, insertedLoop);
					if (this.RemovePolygonInterior)
					{
						this.InsertedLoop.CorrectOrientation();
					}
				}
			}
			return flag;
		}

		public DMesh3 Mesh;

		public int SeedTriangle = -1;

		public Frame3f ProjectFrame;

		public bool SimplifyInsertion = true;

		public bool RemovePolygonInterior = true;

		public RegionOperator ModifiedRegion;

		public int[] InsertedPolygonVerts;

		public EdgeLoop InsertedLoop;

		public int[] InteriorTriangles;

		public Polygon2d Polygon;
	}
}
