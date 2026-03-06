using System;
using System.Collections.Generic;
using g3;

namespace gs
{
	public class MeshTopology
	{
		public double CreaseAngle
		{
			get
			{
				return this.crease_angle;
			}
			set
			{
				this.crease_angle = value;
				this.invalidate_topology();
			}
		}

		public MeshTopology(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public void Compute()
		{
			this.validate_topology();
		}

		public void AddRemeshConstraints(MeshConstraints constraints)
		{
			this.validate_topology();
			int num = 10;
			foreach (EdgeSpan edgeSpan in this.Spans)
			{
				DCurveProjectionTarget target = new DCurveProjectionTarget(edgeSpan.ToCurve(null));
				MeshConstraintUtil.ConstrainVtxSpanTo(constraints, this.Mesh, edgeSpan.Vertices, target, num++);
			}
			foreach (EdgeLoop edgeLoop in this.Loops)
			{
				DCurveProjectionTarget target2 = new DCurveProjectionTarget(edgeLoop.ToCurve(null));
				MeshConstraintUtil.ConstrainVtxLoopTo(constraints, this.Mesh, edgeLoop.Vertices, target2, num++);
			}
			VertexConstraint pinned = VertexConstraint.Pinned;
			pinned.FixedSetID = -1;
			foreach (int vid in this.JunctionVertices)
			{
				if (constraints.HasVertexConstraint(vid))
				{
					VertexConstraint vertexConstraint = constraints.GetVertexConstraint(vid);
					vertexConstraint.Target = null;
					vertexConstraint.Fixed = true;
					vertexConstraint.FixedSetID = -1;
					constraints.SetOrUpdateVertexConstraint(vid, vertexConstraint);
				}
				else
				{
					constraints.SetOrUpdateVertexConstraint(vid, pinned);
				}
			}
		}

		private void invalidate_topology()
		{
			this.topo_timestamp = -1;
		}

		private void validate_topology()
		{
			if (this.IgnoreTimestamp && this.AllEdges != null)
			{
				return;
			}
			if (this.Mesh.ShapeTimestamp != this.topo_timestamp)
			{
				this.find_crease_edges(this.CreaseAngle);
				this.extract_topology();
				this.topo_timestamp = this.Mesh.ShapeTimestamp;
			}
		}

		private void find_crease_edges(double angle_tol)
		{
			this.CreaseEdges = new HashSet<int>();
			this.BoundaryEdges = new HashSet<int>();
			double num = Math.Cos(angle_tol * 0.017453292519943295);
			foreach (int num2 in this.Mesh.EdgeIndices())
			{
				Index2i edgeT = this.Mesh.GetEdgeT(num2);
				if (edgeT.b == -1)
				{
					this.BoundaryEdges.Add(num2);
				}
				else
				{
					Vector3d triNormal = this.Mesh.GetTriNormal(edgeT.a);
					Vector3d triNormal2 = this.Mesh.GetTriNormal(edgeT.b);
					if (Math.Abs(triNormal.Dot(triNormal2)) < num)
					{
						this.CreaseEdges.Add(num2);
					}
				}
			}
			this.AllEdges = new HashSet<int>(this.CreaseEdges);
			foreach (int item in this.BoundaryEdges)
			{
				this.AllEdges.Add(item);
			}
			this.AllVertices = new HashSet<int>();
			IndexUtil.EdgesToVertices(this.Mesh, this.AllEdges, this.AllVertices);
		}

		private void extract_topology()
		{
			DGraph3 dgraph = new DGraph3();
			int[] array = new int[this.Mesh.MaxVertexID];
			int[] array2 = new int[this.AllVertices.Count];
			foreach (int num in this.AllVertices)
			{
				int num2 = dgraph.AppendVertex(this.Mesh.GetVertex(num));
				array[num] = num2;
				array2[num2] = num;
			}
			int[] array3 = new int[this.Mesh.MaxEdgeID];
			foreach (int num3 in this.AllEdges)
			{
				Index2i edgeV = this.Mesh.GetEdgeV(num3);
				int v = array[edgeV.a];
				int v2 = array[edgeV.b];
				int num4 = dgraph.AppendEdge(v, v2, num3);
				array3[num3] = num4;
			}
			DGraph3Util.Curves curves = DGraph3Util.ExtractCurves(dgraph, true, null);
			int count = curves.PathEdges.Count;
			this.Spans = new EdgeSpan[count];
			for (int i = 0; i < count; i++)
			{
				List<int> list = curves.PathEdges[i];
				for (int j = 0; j < list.Count; j++)
				{
					list[j] = dgraph.GetEdgeGroup(list[j]);
				}
				this.Spans[i] = EdgeSpan.FromEdges(this.Mesh, list);
			}
			int count2 = curves.LoopEdges.Count;
			this.Loops = new EdgeLoop[count2];
			for (int k = 0; k < count2; k++)
			{
				List<int> list2 = curves.LoopEdges[k];
				for (int l = 0; l < list2.Count; l++)
				{
					list2[l] = dgraph.GetEdgeGroup(list2[l]);
				}
				this.Loops[k] = EdgeLoop.FromEdges(this.Mesh, list2);
			}
			this.JunctionVertices = new HashSet<int>();
			foreach (int num5 in curves.JunctionV)
			{
				this.JunctionVertices.Add(array2[num5]);
			}
		}

		public DMesh3 MakeElementsMesh(Polygon2d spanProfile, Polygon2d loopProfile)
		{
			DMesh3 dmesh = new DMesh3(true, false, false, false);
			this.validate_topology();
			EdgeSpan[] spans = this.Spans;
			for (int i = 0; i < spans.Length; i++)
			{
				TubeGenerator tubeGenerator = new TubeGenerator(spans[i].ToCurve(this.Mesh), spanProfile);
				MeshEditor.Append(dmesh, tubeGenerator.Generate().MakeDMesh());
			}
			EdgeLoop[] loops = this.Loops;
			for (int i = 0; i < loops.Length; i++)
			{
				TubeGenerator tubeGenerator2 = new TubeGenerator(loops[i].ToCurve(this.Mesh), loopProfile);
				MeshEditor.Append(dmesh, tubeGenerator2.Generate().MakeDMesh());
			}
			return dmesh;
		}

		public DMesh3 Mesh;

		private double crease_angle = 30.0;

		public HashSet<int> BoundaryEdges;

		public HashSet<int> CreaseEdges;

		public HashSet<int> AllEdges;

		public HashSet<int> AllVertices;

		public HashSet<int> JunctionVertices;

		public EdgeLoop[] Loops;

		public EdgeSpan[] Spans;

		private int topo_timestamp = -1;

		public bool IgnoreTimestamp;
	}
}
