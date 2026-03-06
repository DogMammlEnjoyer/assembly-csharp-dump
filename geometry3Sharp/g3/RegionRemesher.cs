using System;
using System.Collections.Generic;
using System.Linq;

namespace g3
{
	public class RegionRemesher : Remesher
	{
		public RegionRemesher(DMesh3 mesh, int[] regionTris)
		{
			this.BaseMesh = mesh;
			this.Region = new DSubmesh3(mesh, regionTris);
			this.Region.ComputeBoundaryInfo(regionTris);
			this.mesh = this.Region.SubMesh;
			this.cur_base_tris = (int[])regionTris.Clone();
			this.bdry_constraints = new MeshConstraints();
			MeshConstraintUtil.FixSubmeshBoundaryEdges(this.bdry_constraints, this.Region);
			base.SetExternalConstraints(this.bdry_constraints);
		}

		public RegionRemesher(DMesh3 mesh, IEnumerable<int> regionTris)
		{
			this.BaseMesh = mesh;
			this.Region = new DSubmesh3(mesh, regionTris, 0);
			int tri_count_est = regionTris.Count<int>();
			this.Region.ComputeBoundaryInfo(regionTris, tri_count_est);
			this.mesh = this.Region.SubMesh;
			this.cur_base_tris = regionTris.ToArray<int>();
			this.bdry_constraints = new MeshConstraints();
			MeshConstraintUtil.FixSubmeshBoundaryEdges(this.bdry_constraints, this.Region);
			base.SetExternalConstraints(this.bdry_constraints);
		}

		public int[] CurrentBaseTriangles
		{
			get
			{
				return this.cur_base_tris;
			}
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
			this.cur_base_tris = array;
			return result;
		}

		public static RegionRemesher QuickRemesh(DMesh3 mesh, int[] tris, double minEdgeLen, double maxEdgeLen, double smoothSpeed, int rounds, IProjectionTarget target, RegionRemesher.QuickRemeshFlags flags = RegionRemesher.QuickRemeshFlags.PreventNormalFlips)
		{
			RegionRemesher regionRemesher = new RegionRemesher(mesh, tris);
			if (target != null)
			{
				regionRemesher.SetProjectionTarget(target);
			}
			regionRemesher.MinEdgeLength = minEdgeLen;
			regionRemesher.MaxEdgeLength = maxEdgeLen;
			regionRemesher.SmoothSpeedT = smoothSpeed;
			if ((flags & RegionRemesher.QuickRemeshFlags.PreventNormalFlips) != RegionRemesher.QuickRemeshFlags.NoFlags)
			{
				regionRemesher.PreventNormalFlips = true;
			}
			for (int i = 0; i < rounds; i++)
			{
				regionRemesher.BasicRemeshPass();
			}
			regionRemesher.BackPropropagate(true);
			return regionRemesher;
		}

		public static RegionRemesher QuickRemesh(DMesh3 mesh, int[] tris, double targetEdgeLen, double smoothSpeed, int rounds, IProjectionTarget target, RegionRemesher.QuickRemeshFlags flags = RegionRemesher.QuickRemeshFlags.PreventNormalFlips)
		{
			RegionRemesher regionRemesher = new RegionRemesher(mesh, tris);
			if (target != null)
			{
				regionRemesher.SetProjectionTarget(target);
			}
			regionRemesher.SetTargetEdgeLength(targetEdgeLen);
			regionRemesher.SmoothSpeedT = smoothSpeed;
			if ((flags & RegionRemesher.QuickRemeshFlags.PreventNormalFlips) != RegionRemesher.QuickRemeshFlags.NoFlags)
			{
				regionRemesher.PreventNormalFlips = true;
			}
			for (int i = 0; i < rounds; i++)
			{
				regionRemesher.BasicRemeshPass();
			}
			regionRemesher.BackPropropagate(true);
			return regionRemesher;
		}

		public DMesh3 BaseMesh;

		public DSubmesh3 Region;

		public IndexMap ReinsertSubToBaseMapV;

		public MeshEditor.DuplicateTriBehavior ReinsertDuplicateTriBehavior;

		private MeshConstraints bdry_constraints;

		private int[] cur_base_tris;

		[Flags]
		public enum QuickRemeshFlags
		{
			NoFlags = 0,
			PreventNormalFlips = 1
		}
	}
}
