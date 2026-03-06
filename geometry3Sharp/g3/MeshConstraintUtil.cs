using System;
using System.Collections.Generic;

namespace g3
{
	public static class MeshConstraintUtil
	{
		public static void FixEdges(MeshConstraints cons, DMesh3 mesh, IEnumerable<int> edges)
		{
			foreach (int num in edges)
			{
				if (mesh.IsEdge(num))
				{
					cons.SetOrUpdateEdgeConstraint(num, EdgeConstraint.FullyConstrained);
					Index2i edgeV = mesh.GetEdgeV(num);
					cons.SetOrUpdateVertexConstraint(edgeV.a, VertexConstraint.Pinned);
					cons.SetOrUpdateVertexConstraint(edgeV.b, VertexConstraint.Pinned);
				}
			}
		}

		public static void FixAllBoundaryEdges(MeshConstraints cons, DMesh3 mesh)
		{
			int maxEdgeID = mesh.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (mesh.IsEdge(i) && mesh.IsBoundaryEdge(i))
				{
					cons.SetOrUpdateEdgeConstraint(i, EdgeConstraint.FullyConstrained);
					Index2i edgeV = mesh.GetEdgeV(i);
					cons.SetOrUpdateVertexConstraint(edgeV.a, VertexConstraint.Pinned);
					cons.SetOrUpdateVertexConstraint(edgeV.b, VertexConstraint.Pinned);
				}
			}
		}

		public static void FixAllBoundaryEdges(Remesher r)
		{
			if (r.Constraints == null)
			{
				r.SetExternalConstraints(new MeshConstraints());
			}
			MeshConstraintUtil.FixAllBoundaryEdges(r.Constraints, r.Mesh);
		}

		public static void FixAllBoundaryEdges_AllowCollapse(MeshConstraints cons, DMesh3 mesh, int setID)
		{
			EdgeConstraint ec = new EdgeConstraint(EdgeRefineFlags.NoFlip | EdgeRefineFlags.NoSplit);
			VertexConstraint vc = new VertexConstraint(true, setID);
			int maxEdgeID = mesh.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (mesh.IsEdge(i) && mesh.IsBoundaryEdge(i))
				{
					cons.SetOrUpdateEdgeConstraint(i, ec);
					Index2i edgeV = mesh.GetEdgeV(i);
					cons.SetOrUpdateVertexConstraint(edgeV.a, vc);
					cons.SetOrUpdateVertexConstraint(edgeV.b, vc);
				}
			}
		}

		public static void FixAllBoundaryEdges_AllowSplit(MeshConstraints cons, DMesh3 mesh, int setID)
		{
			EdgeConstraint ec = new EdgeConstraint(EdgeRefineFlags.NoFlip | EdgeRefineFlags.NoCollapse);
			VertexConstraint vc = new VertexConstraint(true, setID);
			int maxEdgeID = mesh.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (mesh.IsEdge(i) && mesh.IsBoundaryEdge(i))
				{
					cons.SetOrUpdateEdgeConstraint(i, ec);
					Index2i edgeV = mesh.GetEdgeV(i);
					cons.SetOrUpdateVertexConstraint(edgeV.a, vc);
					cons.SetOrUpdateVertexConstraint(edgeV.b, vc);
				}
			}
		}

		public static void FixSubmeshBoundaryEdges(MeshConstraints cons, DSubmesh3 sub)
		{
			foreach (int eID in sub.BaseBorderE)
			{
				Index2i edgeV = sub.BaseMesh.GetEdgeV(eID);
				Index2i index2i = sub.MapVerticesToSubmesh(edgeV);
				int eid = sub.SubMesh.FindEdge(index2i.a, index2i.b);
				cons.SetOrUpdateEdgeConstraint(eid, EdgeConstraint.FullyConstrained);
				cons.SetOrUpdateVertexConstraint(index2i.a, VertexConstraint.Pinned);
				cons.SetOrUpdateVertexConstraint(index2i.b, VertexConstraint.Pinned);
			}
		}

		public static void FixAllGroupBoundaryEdges(MeshConstraints cons, DMesh3 mesh, bool bPinVertices)
		{
			int maxEdgeID = mesh.MaxEdgeID;
			for (int i = 0; i < maxEdgeID; i++)
			{
				if (mesh.IsEdge(i) && mesh.IsGroupBoundaryEdge(i))
				{
					cons.SetOrUpdateEdgeConstraint(i, EdgeConstraint.FullyConstrained);
					if (bPinVertices)
					{
						Index2i edgeV = mesh.GetEdgeV(i);
						cons.SetOrUpdateVertexConstraint(edgeV.a, VertexConstraint.Pinned);
						cons.SetOrUpdateVertexConstraint(edgeV.b, VertexConstraint.Pinned);
					}
				}
			}
		}

		public static void FixAllGroupBoundaryEdges(Remesher r, bool bPinVertices)
		{
			if (r.Constraints == null)
			{
				r.SetExternalConstraints(new MeshConstraints());
			}
			MeshConstraintUtil.FixAllGroupBoundaryEdges(r.Constraints, r.Mesh, bPinVertices);
		}

		public static void ConstrainVtxLoopTo(MeshConstraints cons, DMesh3 mesh, IList<int> loopV, IProjectionTarget target, int setID = -1)
		{
			VertexConstraint vc = new VertexConstraint(target);
			int count = loopV.Count;
			for (int i = 0; i < count; i++)
			{
				cons.SetOrUpdateVertexConstraint(loopV[i], vc);
			}
			EdgeConstraint ec = new EdgeConstraint(EdgeRefineFlags.NoFlip, target);
			ec.TrackingSetID = setID;
			for (int j = 0; j < count; j++)
			{
				int vA = loopV[j];
				int vB = loopV[(j + 1) % count];
				int num = mesh.FindEdge(vA, vB);
				if (num != -1)
				{
					cons.SetOrUpdateEdgeConstraint(num, ec);
				}
			}
		}

		public static void ConstrainVtxLoopTo(Remesher r, int[] loopV, IProjectionTarget target, int setID = -1)
		{
			if (r.Constraints == null)
			{
				r.SetExternalConstraints(new MeshConstraints());
			}
			MeshConstraintUtil.ConstrainVtxLoopTo(r.Constraints, r.Mesh, loopV, target, -1);
		}

		public static void ConstrainVtxSpanTo(MeshConstraints cons, DMesh3 mesh, IList<int> spanV, IProjectionTarget target, int setID = -1)
		{
			VertexConstraint vc = new VertexConstraint(target);
			int count = spanV.Count;
			for (int i = 1; i < count - 1; i++)
			{
				cons.SetOrUpdateVertexConstraint(spanV[i], vc);
			}
			cons.SetOrUpdateVertexConstraint(spanV[0], VertexConstraint.Pinned);
			cons.SetOrUpdateVertexConstraint(spanV[count - 1], VertexConstraint.Pinned);
			EdgeConstraint ec = new EdgeConstraint(EdgeRefineFlags.NoFlip, target);
			ec.TrackingSetID = setID;
			for (int j = 0; j < count - 1; j++)
			{
				int vA = spanV[j];
				int vB = spanV[j + 1];
				int num = mesh.FindEdge(vA, vB);
				if (num != -1)
				{
					cons.SetOrUpdateEdgeConstraint(num, ec);
				}
			}
		}

		public static void ConstrainVtxSpanTo(Remesher r, int[] spanV, IProjectionTarget target, int setID = -1)
		{
			if (r.Constraints == null)
			{
				r.SetExternalConstraints(new MeshConstraints());
			}
			MeshConstraintUtil.ConstrainVtxSpanTo(r.Constraints, r.Mesh, spanV, target, -1);
		}

		public static void PreserveBoundaryLoops(MeshConstraints cons, DMesh3 mesh)
		{
			foreach (EdgeLoop edgeLoop in new MeshBoundaryLoops(mesh, true))
			{
				DCurveProjectionTarget target = new DCurveProjectionTarget(MeshUtil.ExtractLoopV(mesh, edgeLoop.Vertices));
				MeshConstraintUtil.ConstrainVtxLoopTo(cons, mesh, edgeLoop.Vertices, target, -1);
			}
		}

		public static void PreserveBoundaryLoops(Remesher r)
		{
			if (r.Constraints == null)
			{
				r.SetExternalConstraints(new MeshConstraints());
			}
			MeshConstraintUtil.PreserveBoundaryLoops(r.Constraints, r.Mesh);
		}

		public static void AddTrackedEdges(MeshConstraints cons, int[] edges, int setID)
		{
			EdgeConstraint unconstrained = EdgeConstraint.Unconstrained;
			unconstrained.TrackingSetID = setID;
			for (int i = 0; i < edges.Length; i++)
			{
				cons.SetOrUpdateEdgeConstraint(edges[i], unconstrained);
			}
		}
	}
}
