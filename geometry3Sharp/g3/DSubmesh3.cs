using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class DSubmesh3
	{
		public DSubmesh3(DMesh3 mesh, int[] subTriangles)
		{
			this.BaseMesh = mesh;
			this.compute(subTriangles, subTriangles.Length);
		}

		public DSubmesh3(DMesh3 mesh, IEnumerable<int> subTriangles, int nTriEstimate = 0)
		{
			this.BaseMesh = mesh;
			this.compute(subTriangles, nTriEstimate);
		}

		public DSubmesh3(DMesh3 mesh)
		{
			this.BaseMesh = mesh;
		}

		public void Compute(int[] subTriangles)
		{
			this.compute(subTriangles, subTriangles.Length);
		}

		public void Compute(IEnumerable<int> subTriangles, int nTriEstimate = 0)
		{
			this.compute(subTriangles, nTriEstimate);
		}

		public int MapVertexToSubmesh(int base_vID)
		{
			return this.BaseToSubV[base_vID];
		}

		public int MapVertexToBaseMesh(int sub_vID)
		{
			if (sub_vID < this.SubToBaseV.Length)
			{
				return this.SubToBaseV[sub_vID];
			}
			return -1;
		}

		public Index2i MapVerticesToSubmesh(Index2i v)
		{
			return new Index2i(this.BaseToSubV[v.a], this.BaseToSubV[v.b]);
		}

		public Index2i MapVerticesToBaseMesh(Index2i v)
		{
			return new Index2i(this.MapVertexToBaseMesh(v.a), this.MapVertexToBaseMesh(v.b));
		}

		public void MapVerticesToSubmesh(int[] vertices)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i] = this.BaseToSubV[vertices[i]];
			}
		}

		public int MapEdgeToSubmesh(int base_eid)
		{
			Index2i edgeV = this.BaseMesh.GetEdgeV(base_eid);
			Index2i index2i = this.MapVerticesToSubmesh(edgeV);
			return this.SubMesh.FindEdge(index2i.a, index2i.b);
		}

		public void MapEdgesToSubmesh(int[] edges)
		{
			for (int i = 0; i < edges.Length; i++)
			{
				edges[i] = this.MapEdgeToSubmesh(edges[i]);
			}
		}

		public int MapEdgeToBaseMesh(int sub_eid)
		{
			Index2i edgeV = this.SubMesh.GetEdgeV(sub_eid);
			Index2i index2i = this.MapVerticesToBaseMesh(edgeV);
			return this.BaseMesh.FindEdge(index2i.a, index2i.b);
		}

		public int MapTriangleToSubmesh(int base_tID)
		{
			if (!this.ComputeTriMaps)
			{
				throw new InvalidOperationException("DSubmesh3.MapTriangleToSubmesh: must set ComputeTriMaps = true!");
			}
			return this.BaseToSubT[base_tID];
		}

		public int MapTriangleToBaseMesh(int sub_tID)
		{
			if (!this.ComputeTriMaps)
			{
				throw new InvalidOperationException("DSubmesh3.MapTriangleToBaseMesh: must set ComputeTriMaps = true!");
			}
			if (sub_tID < this.SubToBaseT.Length)
			{
				return this.SubToBaseT[sub_tID];
			}
			return -1;
		}

		public void MapTrianglesToSubmesh(int[] triangles)
		{
			if (!this.ComputeTriMaps)
			{
				throw new InvalidOperationException("DSubmesh3.MapTrianglesToSubmesh: must set ComputeTriMaps = true!");
			}
			for (int i = 0; i < triangles.Length; i++)
			{
				triangles[i] = this.BaseToSubT[triangles[i]];
			}
		}

		public void ComputeBoundaryInfo(int[] subTriangles)
		{
			this.ComputeBoundaryInfo(subTriangles, subTriangles.Length);
		}

		public void ComputeBoundaryInfo(IEnumerable<int> triangles, int tri_count_est)
		{
			IndexFlagSet indexFlagSet = new IndexFlagSet(this.BaseMesh.MaxTriangleID, tri_count_est);
			foreach (int key in triangles)
			{
				indexFlagSet[key] = true;
			}
			this.BaseBorderV = new IndexHashSet();
			this.BaseBorderE = new IndexHashSet();
			this.BaseBoundaryE = new IndexHashSet();
			foreach (int tID in triangles)
			{
				Index3i triEdges = this.BaseMesh.GetTriEdges(tID);
				for (int i = 0; i < 3; i++)
				{
					int num = triEdges[i];
					Index2i edgeT = this.BaseMesh.GetEdgeT(num);
					if (edgeT.b == -1)
					{
						this.BaseBoundaryE[num] = true;
					}
					else if (indexFlagSet[edgeT.a] != indexFlagSet[edgeT.b])
					{
						this.BaseBorderE[num] = true;
						Index2i edgeV = this.BaseMesh.GetEdgeV(num);
						this.BaseBorderV[edgeV.a] = true;
						this.BaseBorderV[edgeV.b] = true;
					}
				}
			}
		}

		private void compute(IEnumerable<int> triangles, int tri_count_est)
		{
			int subsetCountEst = tri_count_est / 2;
			this.SubMesh = new DMesh3(this.BaseMesh.Components & this.WantComponents);
			this.BaseSubmeshV = new IndexFlagSet(this.BaseMesh.MaxVertexID, subsetCountEst);
			this.BaseToSubV = new IndexMap(this.BaseMesh.MaxVertexID, subsetCountEst);
			this.SubToBaseV = new DVector<int>();
			if (this.ComputeTriMaps)
			{
				this.BaseToSubT = new IndexMap(this.BaseMesh.MaxTriangleID, tri_count_est);
				this.SubToBaseT = new DVector<int>();
			}
			foreach (int num in triangles)
			{
				if (!this.BaseMesh.IsTriangle(num))
				{
					throw new Exception("DSubmesh3.compute: triangle " + num.ToString() + " does not exist in BaseMesh!");
				}
				Index3i triangle = this.BaseMesh.GetTriangle(num);
				Index3i zero = Index3i.Zero;
				int gid = this.BaseMesh.GetTriangleGroup(num);
				for (int i = 0; i < 3; i++)
				{
					int num2 = triangle[i];
					int num3;
					if (!this.BaseSubmeshV[num2])
					{
						num3 = this.SubMesh.AppendVertex(this.BaseMesh, num2);
						this.BaseSubmeshV[num2] = true;
						this.BaseToSubV[num2] = num3;
						this.SubToBaseV.insert(num2, num3);
					}
					else
					{
						num3 = this.BaseToSubV[num2];
					}
					zero[i] = num3;
				}
				if (this.OverrideGroupID >= 0)
				{
					gid = this.OverrideGroupID;
				}
				int num4 = this.SubMesh.AppendTriangle(zero, gid);
				if (this.ComputeTriMaps)
				{
					this.BaseToSubT[num] = num4;
					this.SubToBaseT.insert(num, num4);
				}
			}
		}

		public static DMesh3 QuickSubmesh(DMesh3 mesh, int[] triangles)
		{
			return new DSubmesh3(mesh, triangles).SubMesh;
		}

		public static DMesh3 QuickSubmesh(DMesh3 mesh, IEnumerable<int> triangles)
		{
			return DSubmesh3.QuickSubmesh(mesh, triangles.ToArray<int>());
		}

		public DMesh3 BaseMesh;

		public DMesh3 SubMesh;

		public MeshComponents WantComponents = MeshComponents.All;

		public bool ComputeTriMaps;

		public int OverrideGroupID = -1;

		public IndexFlagSet BaseSubmeshV;

		public IndexMap BaseToSubV;

		public DVector<int> SubToBaseV;

		public IndexMap BaseToSubT;

		public DVector<int> SubToBaseT;

		public IndexHashSet BaseBorderE;

		public IndexHashSet BaseBoundaryE;

		public IndexHashSet BaseBorderV;
	}
}
