using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshLoopClosure
	{
		public MeshLoopClosure(DMesh3 mesh, EdgeLoop border_loop)
		{
			this.Mesh = mesh;
			this.InitialBorderLoop = border_loop;
		}

		public virtual ValidationStatus Validate()
		{
			ValidationStatus validationStatus = MeshValidation.IsBoundaryLoop(this.Mesh, this.InitialBorderLoop);
			if (validationStatus != ValidationStatus.Ok)
			{
				return validationStatus;
			}
			ValidationStatus validationStatus2 = MeshValidation.HasDuplicateTriangles(this.Mesh);
			if (validationStatus2 != ValidationStatus.Ok)
			{
				return validationStatus2;
			}
			return ValidationStatus.Ok;
		}

		public virtual bool Close()
		{
			this.Close_Flat();
			return true;
		}

		public void Close_Flat()
		{
			double num;
			double num2;
			double num3;
			MeshQueries.EdgeLengthStats(this.Mesh, out num, out num2, out num3, 1000);
			double num4 = (this.TargetEdgeLen <= 0.0) ? num3 : this.TargetEdgeLen;
			List<int> list;
			MeshLoopClosure.cleanup_boundary(this.Mesh, this.InitialBorderLoop, num3, out list, 3);
			MeshBoundaryLoops meshBoundaryLoops = new MeshBoundaryLoops(this.Mesh, true);
			int num5 = meshBoundaryLoops.FindLoopContainingEdge(list[0]);
			if (num5 == -1)
			{
				num5 = meshBoundaryLoops.MaxVerticesLoopIndex;
			}
			EdgeLoop loop = meshBoundaryLoops.Loops[num5];
			int num6 = (this.ExtrudeGroup == -1) ? this.Mesh.AllocateTriangleGroup() : this.ExtrudeGroup;
			int group_id = (this.FillGroup == -1) ? this.Mesh.AllocateTriangleGroup() : this.FillGroup;
			MeshExtrudeLoop meshExtrudeLoop = new MeshExtrudeLoop(this.Mesh, loop);
			meshExtrudeLoop.PositionF = ((Vector3d v, Vector3f n, int i) => this.FlatClosePlane.ProjectToPlane((Vector3f)v, 1));
			meshExtrudeLoop.Extrude(num6);
			MeshValidation.IsBoundaryLoop(this.Mesh, meshExtrudeLoop.NewLoop);
			new MeshLoopSmooth(this.Mesh, meshExtrudeLoop.NewLoop)
			{
				ProjectF = ((Vector3d v, int i) => this.FlatClosePlane.ProjectToPlane((Vector3f)v, 1)),
				Alpha = 0.5,
				Rounds = 100
			}.Smooth();
			SimpleHoleFiller simpleHoleFiller = new SimpleHoleFiller(this.Mesh, meshExtrudeLoop.NewLoop);
			simpleHoleFiller.Fill(group_id);
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(this.Mesh);
			meshFaceSelection.Select(meshExtrudeLoop.NewTriangles);
			meshFaceSelection.Select(simpleHoleFiller.NewTriangles);
			meshFaceSelection.ExpandToOneRingNeighbours(null);
			meshFaceSelection.ExpandToOneRingNeighbours(null);
			meshFaceSelection.LocalOptimize(true, true, true, true, false);
			int[] regionTris = meshFaceSelection.ToArray();
			FaceGroupUtil.SetGroupToGroup(this.Mesh, num6, 0);
			RegionRemesher regionRemesher = new RegionRemesher(this.Mesh, regionTris);
			DCurveProjectionTarget target = new DCurveProjectionTarget(MeshUtil.ExtractLoopV(this.Mesh, meshExtrudeLoop.NewLoop.Vertices));
			int[] array = (int[])meshExtrudeLoop.NewLoop.Vertices.Clone();
			regionRemesher.Region.MapVerticesToSubmesh(array);
			MeshConstraintUtil.ConstrainVtxLoopTo(regionRemesher.Constraints, regionRemesher.Mesh, array, target, -1);
			DMeshAABBTree3 dmeshAABBTree = new DMeshAABBTree3(this.Mesh, false);
			dmeshAABBTree.Build(DMeshAABBTree3.BuildStrategy.TopDownMidpoint, DMeshAABBTree3.ClusterPolicy.Default);
			MeshProjectionTarget projectionTarget = new MeshProjectionTarget(this.Mesh, dmeshAABBTree);
			regionRemesher.SetProjectionTarget(projectionTarget);
			if (true)
			{
				regionRemesher.Precompute();
				regionRemesher.EnableFlips = (regionRemesher.EnableSplits = (regionRemesher.EnableCollapses = true));
				regionRemesher.MinEdgeLength = num4;
				regionRemesher.MaxEdgeLength = 2.0 * num4;
				regionRemesher.EnableSmoothing = true;
				regionRemesher.SmoothSpeedT = 1.0;
				for (int k = 0; k < 40; k++)
				{
					regionRemesher.BasicRemeshPass();
				}
				regionRemesher.SetProjectionTarget(null);
				regionRemesher.SmoothSpeedT = 0.25;
				for (int j = 0; j < 10; j++)
				{
					regionRemesher.BasicRemeshPass();
				}
				regionRemesher.BackPropropagate(true);
			}
			MeshLoopClosure.smooth_region(this.Mesh, regionRemesher.Region.BaseBorderV, 3);
		}

		public static void smooth_region(DMesh3 mesh, IEnumerable<int> vertices, int nRings)
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(mesh);
			meshFaceSelection.SelectVertexOneRings(vertices);
			for (int i = 0; i < nRings; i++)
			{
				meshFaceSelection.ExpandToOneRingNeighbours(null);
			}
			meshFaceSelection.LocalOptimize(true, true, true, true, false);
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(mesh);
			meshVertexSelection.SelectTriangleVertices(meshFaceSelection.ToArray());
			new MeshIterativeSmooth(mesh, meshVertexSelection.ToArray(), true)
			{
				Alpha = 0.20000000298023224,
				Rounds = 10
			}.Smooth();
		}

		public static void smooth_loop(DMesh3 mesh, EdgeLoop loop, int nRings)
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(mesh);
			meshFaceSelection.SelectVertexOneRings(loop.Vertices);
			for (int i = 0; i < nRings; i++)
			{
				meshFaceSelection.ExpandToOneRingNeighbours(null);
			}
			meshFaceSelection.LocalOptimize(true, true, true, true, false);
			MeshVertexSelection meshVertexSelection = new MeshVertexSelection(mesh);
			meshVertexSelection.SelectTriangleVertices(meshFaceSelection.ToArray());
			meshVertexSelection.Deselect(loop.Vertices);
			MeshLoopSmooth meshLoopSmooth = new MeshLoopSmooth(mesh, loop);
			meshLoopSmooth.Rounds = 1;
			MeshIterativeSmooth meshIterativeSmooth = new MeshIterativeSmooth(mesh, meshVertexSelection.ToArray(), true);
			meshIterativeSmooth.Rounds = 1;
			for (int j = 0; j < 10; j++)
			{
				meshLoopSmooth.Smooth();
				meshIterativeSmooth.Smooth();
			}
		}

		public static void cleanup_boundary(DMesh3 mesh, EdgeLoop loop, double target_edge_len, out List<int> result_edges, int nRings = 3)
		{
			MeshFaceSelection meshFaceSelection = new MeshFaceSelection(mesh);
			meshFaceSelection.SelectVertexOneRings(loop.Vertices);
			for (int i = 0; i < nRings; i++)
			{
				meshFaceSelection.ExpandToOneRingNeighbours(null);
			}
			meshFaceSelection.LocalOptimize(true, true, true, true, false);
			RegionRemesher regionRemesher = new RegionRemesher(mesh, meshFaceSelection.ToArray());
			int[] array = new int[loop.EdgeCount];
			Array.Copy(loop.Edges, array, loop.EdgeCount);
			regionRemesher.Region.MapEdgesToSubmesh(array);
			MeshConstraintUtil.AddTrackedEdges(regionRemesher.Constraints, array, 100);
			regionRemesher.Precompute();
			regionRemesher.EnableFlips = (regionRemesher.EnableSplits = (regionRemesher.EnableCollapses = true));
			regionRemesher.MinEdgeLength = target_edge_len;
			regionRemesher.MaxEdgeLength = 2.0 * target_edge_len;
			regionRemesher.EnableSmoothing = true;
			regionRemesher.SmoothSpeedT = 0.10000000149011612;
			for (int j = 0; j < nRings * 3; j++)
			{
				regionRemesher.BasicRemeshPass();
			}
			List<int> edges = regionRemesher.Constraints.FindConstrainedEdgesBySetID(100);
			regionRemesher.BackPropropagate(true);
			result_edges = MeshIndexUtil.MapEdgesViaVertexMap(regionRemesher.ReinsertSubToBaseMapV, regionRemesher.Region.SubMesh, regionRemesher.BaseMesh, edges);
		}

		public DMesh3 Mesh;

		public EdgeLoop InitialBorderLoop;

		public Frame3f FlatClosePlane;

		public double TargetEdgeLen;

		public int ExtrudeGroup = -1;

		public int FillGroup = -1;
	}
}
