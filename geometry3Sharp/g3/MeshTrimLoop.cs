using System;

namespace g3
{
	public class MeshTrimLoop
	{
		public MeshTrimLoop(DMesh3 mesh, DCurve3 trimline, int tSeedTID, DMeshAABBTree3 spatial = null)
		{
			if (spatial != null && spatial.Mesh == mesh)
			{
				throw new ArgumentException("MeshTrimLoop: input spatial DS must have its own copy of mesh");
			}
			this.Mesh = mesh;
			this.TrimLine = new DCurve3(trimline);
			if (spatial != null)
			{
				this.Spatial = spatial;
			}
			this.seed_tri = tSeedTID;
		}

		public MeshTrimLoop(DMesh3 mesh, DCurve3 trimline, Vector3d vSeedPt, DMeshAABBTree3 spatial = null)
		{
			if (spatial != null && spatial.Mesh == mesh)
			{
				throw new ArgumentException("MeshTrimLoop: input spatial DS must have its own copy of mesh");
			}
			this.Mesh = mesh;
			this.TrimLine = new DCurve3(trimline);
			if (spatial != null)
			{
				this.Spatial = spatial;
			}
			this.seed_pt = vSeedPt;
		}

		public virtual ValidationStatus Validate()
		{
			return ValidationStatus.Ok;
		}

		public virtual bool Trim()
		{
			if (this.Spatial == null)
			{
				this.Spatial = new DMeshAABBTree3(new DMesh3(this.Mesh, false, MeshComponents.None), false);
				this.Spatial.Build(DMeshAABBTree3.BuildStrategy.TopDownMidpoint, DMeshAABBTree3.ClusterPolicy.Default);
			}
			if (this.seed_tri == -1)
			{
				this.seed_tri = this.Spatial.FindNearestTriangle(this.seed_pt, double.MaxValue);
			}
			MeshFaceSelection meshFaceSelection = new MeshFacesFromLoop(this.Mesh, this.TrimLine, this.Spatial, this.seed_tri).ToSelection();
			meshFaceSelection.LocalOptimize(true, true, true, true, false);
			MeshEditor meshEditor = new MeshEditor(this.Mesh);
			meshEditor.RemoveTriangles(meshFaceSelection, true);
			MeshConnectedComponents meshConnectedComponents = new MeshConnectedComponents(this.Mesh);
			meshConnectedComponents.FindConnectedT();
			if (meshConnectedComponents.Count > 1)
			{
				int largestByCount = meshConnectedComponents.LargestByCount;
				for (int i = 0; i < meshConnectedComponents.Count; i++)
				{
					if (i != largestByCount)
					{
						meshEditor.RemoveTriangles(meshConnectedComponents[i].Indices, true);
					}
				}
			}
			meshEditor.RemoveAllBowtieVertices(true);
			MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, true);
			bool flag = false;
			try
			{
				flag = meshBoundaryLoops.Compute();
			}
			catch (Exception)
			{
				return false;
			}
			if (!flag)
			{
				return false;
			}
			if (meshBoundaryLoops.Count > 1)
			{
				return false;
			}
			int[] vertices = meshBoundaryLoops[0].Vertices;
			MeshFaceSelection meshFaceSelection2 = new MeshFaceSelection(this.Mesh);
			meshFaceSelection2.SelectVertexOneRings(vertices);
			meshFaceSelection2.ExpandToOneRingNeighbours(this.RemeshBorderRings, null);
			RegionRemesher regionRemesher = new RegionRemesher(this.Mesh, meshFaceSelection2.ToArray());
			regionRemesher.Region.MapVerticesToSubmesh(vertices);
			double num = this.TargetEdgeLength;
			if (num <= 0.0)
			{
				double num2;
				double num3;
				double num4;
				MeshQueries.EdgeLengthStatsFromEdges(this.Mesh, meshBoundaryLoops[0].Edges, out num2, out num3, out num4, 0);
				num = num4;
			}
			MeshProjectionTarget meshProjectionTarget = new MeshProjectionTarget(this.Spatial.Mesh, this.Spatial);
			regionRemesher.SetProjectionTarget(meshProjectionTarget);
			regionRemesher.SetTargetEdgeLength(num);
			regionRemesher.SmoothSpeedT = this.SmoothingAlpha;
			DCurveProjectionTarget dcurveProjectionTarget = new DCurveProjectionTarget(this.TrimLine);
			SequentialProjectionTarget target = new SequentialProjectionTarget(new IProjectionTarget[]
			{
				dcurveProjectionTarget,
				meshProjectionTarget
			});
			int setID = 3;
			MeshConstraintUtil.ConstrainVtxLoopTo(regionRemesher, vertices, target, setID);
			for (int j = 0; j < this.RemeshRounds; j++)
			{
				regionRemesher.BasicRemeshPass();
			}
			regionRemesher.BackPropropagate(true);
			return true;
		}

		public DMesh3 Mesh;

		public DMeshAABBTree3 Spatial;

		public DCurve3 TrimLine;

		public int RemeshBorderRings = 2;

		public double SmoothingAlpha = 1.0;

		public double TargetEdgeLength;

		public int RemeshRounds = 20;

		private int seed_tri = -1;

		private Vector3d seed_pt = Vector3d.MaxValue;
	}
}
