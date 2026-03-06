using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class RegionOperator
	{
		public RegionOperator(DMesh3 mesh, int[] regionTris, Action<DSubmesh3> submeshConfigF = null)
		{
			this.BaseMesh = mesh;
			this.Region = new DSubmesh3(mesh);
			if (submeshConfigF != null)
			{
				submeshConfigF(this.Region);
			}
			this.Region.Compute(regionTris);
			this.Region.ComputeBoundaryInfo(regionTris);
			this.cur_base_tris = (int[])regionTris.Clone();
		}

		public RegionOperator(DMesh3 mesh, IEnumerable<int> regionTris, Action<DSubmesh3> submeshConfigF = null)
		{
			this.BaseMesh = mesh;
			this.Region = new DSubmesh3(mesh);
			if (submeshConfigF != null)
			{
				submeshConfigF(this.Region);
			}
			this.Region.Compute(regionTris, 0);
			int tri_count_est = regionTris.Count<int>();
			this.Region.ComputeBoundaryInfo(regionTris, tri_count_est);
			this.cur_base_tris = regionTris.ToArray<int>();
		}

		public int[] CurrentBaseTriangles
		{
			get
			{
				return this.cur_base_tris;
			}
		}

		public HashSet<int> CurrentBaseInteriorVertices()
		{
			HashSet<int> hashSet = new HashSet<int>();
			IndexHashSet baseBorderV = this.Region.BaseBorderV;
			foreach (int tID in this.cur_base_tris)
			{
				Index3i triangle = this.BaseMesh.GetTriangle(tID);
				if (!baseBorderV[triangle.a])
				{
					hashSet.Add(triangle.a);
				}
				if (!baseBorderV[triangle.b])
				{
					hashSet.Add(triangle.b);
				}
				if (!baseBorderV[triangle.c])
				{
					hashSet.Add(triangle.c);
				}
			}
			return hashSet;
		}

		public void RepairPossibleNonManifoldEdges()
		{
			int maxEdgeID = this.Region.SubMesh.MaxEdgeID;
			List<int> list = new List<int>();
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (this.Region.SubMesh.IsEdge(i) && !this.Region.SubMesh.IsBoundaryEdge(i))
				{
					Index2i edgeV = this.Region.SubMesh.GetEdgeV(i);
					if (this.Region.SubMesh.IsBoundaryVertex(edgeV.a) && this.Region.SubMesh.IsBoundaryVertex(edgeV.b))
					{
						int num = this.Region.MapVertexToBaseMesh(edgeV.a);
						int num2 = this.Region.MapVertexToBaseMesh(edgeV.b);
						if (num != -1 && num2 != -1 && this.Region.BaseMesh.FindEdge(num, num2) != -1)
						{
							list.Add(i);
						}
					}
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				DMesh3.EdgeSplitInfo edgeSplitInfo;
				this.Region.SubMesh.SplitEdge(list[j], out edgeSplitInfo, 0.5);
			}
		}

		public void SetSubmeshGroupID(int gid)
		{
			FaceGroupUtil.SetGroupID(this.Region.SubMesh, gid);
		}

		public bool BackPropropagate(bool bAllowSubmeshRepairs = true)
		{
			if (bAllowSubmeshRepairs)
			{
				this.RepairPossibleNonManifoldEdges();
			}
			MeshEditor meshEditor = new MeshEditor(this.BaseMesh);
			meshEditor.RemoveTriangles(this.cur_base_tris, true);
			int[] array = new int[this.Region.SubMesh.TriangleCount];
			this.ReinsertSubToBaseMapV = null;
			bool result = meshEditor.ReinsertSubmesh(this.Region, ref array, out this.ReinsertSubToBaseMapV, this.ReinsertDuplicateTriBehavior);
			int maxTriangleID = this.Region.SubMesh.MaxTriangleID;
			this.ReinsertSubToBaseMapT = new IndexMap(false, maxTriangleID);
			int num = 0;
			for (int i = 0; i < maxTriangleID; i++)
			{
				if (this.Region.SubMesh.IsTriangle(i))
				{
					this.ReinsertSubToBaseMapT[i] = array[num++];
				}
			}
			this.cur_base_tris = array;
			return result;
		}

		public bool BackPropropagateVertices(bool bRecomputeBoundaryNormals = false)
		{
			bool flag = this.Region.SubMesh.HasVertexNormals && this.Region.BaseMesh.HasVertexNormals;
			foreach (int num in this.Region.SubMesh.VertexIndices())
			{
				int vID = this.Region.SubToBaseV[num];
				Vector3d vertex = this.Region.SubMesh.GetVertex(num);
				this.Region.BaseMesh.SetVertex(vID, vertex);
				if (flag)
				{
					this.Region.BaseMesh.SetVertexNormal(vID, this.Region.SubMesh.GetVertexNormal(num));
				}
			}
			if (bRecomputeBoundaryNormals)
			{
				foreach (int num2 in this.Region.BaseBorderV)
				{
					Vector3d v = MeshNormals.QuickCompute(this.Region.BaseMesh, num2, MeshNormals.NormalsTypes.Vertex_OneRingFaceAverage_AreaWeighted);
					this.Region.BaseMesh.SetVertexNormal(num2, (Vector3f)v);
				}
			}
			return true;
		}

		public DMesh3 BaseMesh;

		public DSubmesh3 Region;

		public IndexMap ReinsertSubToBaseMapV;

		public IndexMap ReinsertSubToBaseMapT;

		public MeshEditor.DuplicateTriBehavior ReinsertDuplicateTriBehavior;

		private int[] cur_base_tris;
	}
}
