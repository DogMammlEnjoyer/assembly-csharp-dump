using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class SmoothedHoleFill
	{
		public SmoothedHoleFill(DMesh3 mesh, EdgeLoop fillLoop = null)
		{
			this.Mesh = mesh;
			this.FillLoop = fillLoop;
		}

		public bool Apply()
		{
			EdgeLoop edgeLoop = null;
			if (this.FillLoop == null)
			{
				MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, true);
				if (meshBoundaryLoops.Count == 0)
				{
					return false;
				}
				if (this.BorderHintTris != null)
				{
					edgeLoop = this.select_loop_tris_hint(meshBoundaryLoops);
				}
				if (edgeLoop == null && meshBoundaryLoops.MaxVerticesLoopIndex >= 0)
				{
					edgeLoop = meshBoundaryLoops[meshBoundaryLoops.MaxVerticesLoopIndex];
				}
			}
			else
			{
				edgeLoop = this.FillLoop;
			}
			if (edgeLoop == null)
			{
				return false;
			}
			SimpleHoleFiller simpleHoleFiller = new SimpleHoleFiller(this.Mesh, edgeLoop);
			if (!simpleHoleFiller.Fill(-1))
			{
				return false;
			}
			if (edgeLoop.Vertices.Length <= 3)
			{
				this.FillTriangles = simpleHoleFiller.NewTriangles;
				this.FillVertices = new int[0];
				return true;
			}
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
			meshFaceSelection.Select(simpleHoleFiller.NewTriangles);
			if (this.OffsetDistance > 0.0)
			{
				MeshExtrudeFaces meshExtrudeFaces = new MeshExtrudeFaces(this.Mesh, meshFaceSelection);
				meshExtrudeFaces.ExtrudedPositionF = ((Vector3d v, Vector3f n, int vid) => v + this.OffsetDistance * this.OffsetDirection);
				if (!meshExtrudeFaces.Extrude())
				{
					return false;
				}
				meshFaceSelection.Select(meshExtrudeFaces.JoinTriangles);
			}
			if (!this.ConstrainToHoleInterior)
			{
				meshFaceSelection.ExpandToOneRingNeighbours(2, null);
				meshFaceSelection.LocalOptimize(true, true, true, true, false);
			}
			if (this.RemeshBeforeSmooth)
			{
				RegionRemesher regionRemesher = new RegionRemesher(this.Mesh, meshFaceSelection);
				regionRemesher.SetTargetEdgeLength(this.TargetEdgeLength);
				regionRemesher.EnableSmoothing = (this.SmoothAlpha > 0.0);
				regionRemesher.SmoothSpeedT = this.SmoothAlpha;
				if (this.ConfigureRemesherF != null)
				{
					this.ConfigureRemesherF(regionRemesher, true);
				}
				for (int i = 0; i < this.InitialRemeshPasses; i++)
				{
					regionRemesher.BasicRemeshPass();
				}
				regionRemesher.BackPropropagate(true);
				meshFaceSelection = new MeshFaceSelection(this.Mesh);
				meshFaceSelection.Select(regionRemesher.CurrentBaseTriangles);
				if (!this.ConstrainToHoleInterior)
				{
					meshFaceSelection.LocalOptimize(true, true, true, true, false);
				}
			}
			if (this.ConstrainToHoleInterior)
			{
				for (int j = 0; j < this.SmoothSolveIterations; j++)
				{
					this.smooth_and_remesh_preserve(meshFaceSelection, j == this.SmoothSolveIterations - 1);
					meshFaceSelection = new MeshFaceSelection(this.Mesh);
					meshFaceSelection.Select(this.FillTriangles);
				}
			}
			else
			{
				this.smooth_and_remesh(meshFaceSelection);
				meshFaceSelection = new MeshFaceSelection(this.Mesh);
				meshFaceSelection.Select(this.FillTriangles);
			}
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(this.Mesh);
			meshVertexSelection.SelectInteriorVertices(meshFaceSelection);
			this.FillVertices = meshVertexSelection.ToArray();
			return true;
		}

		private void smooth_and_remesh_preserve(MeshFaceSelection tris, bool bFinal)
		{
			if (this.EnableLaplacianSmooth)
			{
				LaplacianMeshSmoother.RegionSmooth(this.Mesh, tris, 2, 2, true, 10.0, 0.0);
			}
			if (this.RemeshAfterSmooth)
			{
				MeshProjectionTarget projectionTarget = bFinal ? MeshProjectionTarget.Auto(this.Mesh, tris, 5) : null;
				RegionRemesher regionRemesher = new RegionRemesher(this.Mesh, tris);
				regionRemesher.SetTargetEdgeLength(this.TargetEdgeLength);
				regionRemesher.SmoothSpeedT = 1.0;
				regionRemesher.SetProjectionTarget(projectionTarget);
				if (this.ConfigureRemesherF != null)
				{
					this.ConfigureRemesherF(regionRemesher, false);
				}
				for (int i = 0; i < 10; i++)
				{
					regionRemesher.BasicRemeshPass();
				}
				regionRemesher.BackPropropagate(true);
				this.FillTriangles = regionRemesher.CurrentBaseTriangles;
				return;
			}
			this.FillTriangles = tris.ToArray();
		}

		private void smooth_and_remesh(MeshFaceSelection tris)
		{
			if (this.EnableLaplacianSmooth)
			{
				LaplacianMeshSmoother.RegionSmooth(this.Mesh, tris, 2, 2, false, 10.0, 0.0);
			}
			if (this.RemeshAfterSmooth)
			{
				tris.ExpandToOneRingNeighbours(2, null);
				tris.LocalOptimize(true, true, true, true, false);
				MeshProjectionTarget projectionTarget = MeshProjectionTarget.Auto(this.Mesh, tris, 5);
				RegionRemesher regionRemesher = new RegionRemesher(this.Mesh, tris);
				regionRemesher.SetTargetEdgeLength(this.TargetEdgeLength);
				regionRemesher.SmoothSpeedT = 1.0;
				regionRemesher.SetProjectionTarget(projectionTarget);
				if (this.ConfigureRemesherF != null)
				{
					this.ConfigureRemesherF(regionRemesher, false);
				}
				for (int i = 0; i < 10; i++)
				{
					regionRemesher.BasicRemeshPass();
				}
				regionRemesher.BackPropropagate(true);
				this.FillTriangles = regionRemesher.CurrentBaseTriangles;
				return;
			}
			this.FillTriangles = tris.ToArray();
		}

		private EdgeLoop select_loop_tris_hint(MeshBoundaryLoops loops)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (int tID in this.BorderHintTris)
			{
				if (this.Mesh.IsTriangle(tID))
				{
					Index3i triEdges = this.Mesh.GetTriEdges(tID);
					for (int i = 0; i < 3; i++)
					{
						if (this.Mesh.IsBoundaryEdge(triEdges[i]))
						{
							hashSet.Add(triEdges[i]);
						}
					}
				}
			}
			int count = loops.Count;
			int num = -1;
			int num2 = 0;
			for (int j = 0; j < count; j++)
			{
				int num3 = 0;
				foreach (int item in loops[j].Edges)
				{
					if (hashSet.Contains(item))
					{
						num3++;
					}
				}
				if (num3 > num2)
				{
					num = j;
					num2 = num3;
				}
			}
			if (num == -1)
			{
				return null;
			}
			return loops[num];
		}

		public DMesh3 Mesh;

		public Vector3d OffsetDirection = Vector3d.Zero;

		public double OffsetDistance;

		public double TargetEdgeLength = 2.5;

		public double SmoothAlpha = 1.0;

		public int InitialRemeshPasses = 20;

		public bool RemeshBeforeSmooth = true;

		public bool RemeshAfterSmooth = true;

		public Action<Remesher, bool> ConfigureRemesherF;

		public bool EnableLaplacianSmooth = true;

		public int SmoothSolveIterations = 1;

		public bool ConstrainToHoleInterior;

		public EdgeLoop FillLoop;

		public List<int> BorderHintTris;

		public int[] FillTriangles;

		public int[] FillVertices;
	}
}
