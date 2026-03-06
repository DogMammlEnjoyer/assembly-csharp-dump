using System;
using System.Collections.Generic;

namespace g3
{
	public class GraphTubeMesher
	{
		protected virtual bool Cancelled()
		{
			return this.Progress != null && this.Progress.Cancelled();
		}

		public GraphTubeMesher(DGraph3 graph)
		{
			this.Graph = graph;
		}

		public GraphTubeMesher(GraphSupportGenerator support_gen)
		{
			this.Graph = support_gen.Graph;
			this.TipVertices = support_gen.TipVertices;
			this.GroundVertices = support_gen.GroundVertices;
			this.SamplerCellSizeHint = support_gen.CellSize;
		}

		public virtual void Generate()
		{
			AxisAlignedBox3d cachedBounds = this.Graph.CachedBounds;
			cachedBounds.Expand(2.0 * this.PostRadius);
			double num = (this.SamplerCellSizeHint == 0.0) ? (this.PostRadius / 5.0) : this.SamplerCellSizeHint;
			ImplicitFieldSampler3d implicitFieldSampler3d = new ImplicitFieldSampler3d(cachedBounds, num);
			this.ActualCellSize = num;
			ImplicitLine3d implicitLine3d = new ImplicitLine3d
			{
				Radius = this.PostRadius
			};
			foreach (int eID in this.Graph.EdgeIndices())
			{
				Index2i edgeV = this.Graph.GetEdgeV(eID);
				Vector3d vertex = this.Graph.GetVertex(edgeV.a);
				Vector3d vertex2 = this.Graph.GetVertex(edgeV.b);
				double radius = this.PostRadius;
				int item = (vertex.y > vertex2.y) ? edgeV.a : edgeV.b;
				if (this.TipVertices.Contains(item))
				{
					radius = this.TipRadius;
				}
				implicitLine3d.Segment = new Segment3d(vertex, vertex2);
				implicitLine3d.Radius = radius;
				implicitFieldSampler3d.Sample(implicitLine3d, implicitLine3d.Radius / 2.0);
			}
			foreach (int vID in this.GroundVertices)
			{
				Vector3d vertex3 = this.Graph.GetVertex(vID);
				implicitFieldSampler3d.Sample(new ImplicitSphere3d
				{
					Origin = vertex3 - this.PostRadius / 2.0 * Vector3d.AxisY,
					Radius = this.GroundRadius
				}, 0.0);
			}
			ImplicitHalfSpace3d b = new ImplicitHalfSpace3d
			{
				Origin = Vector3d.Zero,
				Normal = Vector3d.AxisY
			};
			ImplicitDifference3d @implicit = new ImplicitDifference3d
			{
				A = implicitFieldSampler3d.ToImplicit(),
				B = b
			};
			MarchingCubes marchingCubes = new MarchingCubes
			{
				Implicit = @implicit,
				Bounds = cachedBounds,
				CubeSize = this.PostRadius / 3.0
			};
			marchingCubes.Bounds.Min.y = -2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes2 = marchingCubes;
			marchingCubes2.Bounds.Min.x = marchingCubes2.Bounds.Min.x - 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes3 = marchingCubes;
			marchingCubes3.Bounds.Min.z = marchingCubes3.Bounds.Min.z - 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes4 = marchingCubes;
			marchingCubes4.Bounds.Max.x = marchingCubes4.Bounds.Max.x + 2.0 * marchingCubes.CubeSize;
			MarchingCubes marchingCubes5 = marchingCubes;
			marchingCubes5.Bounds.Max.z = marchingCubes5.Bounds.Max.z + 2.0 * marchingCubes.CubeSize;
			marchingCubes.CancelF = new Func<bool>(this.Cancelled);
			marchingCubes.Generate();
			this.ResultMesh = marchingCubes.Mesh;
		}

		public DGraph3 Graph;

		public HashSet<int> TipVertices;

		public HashSet<int> GroundVertices;

		public double PostRadius = 1.25;

		public double TipRadius = 0.5;

		public double GroundRadius = 3.25;

		public double SamplerCellSizeHint;

		public double ActualCellSize;

		public ProgressCancel Progress;

		public DMesh3 ResultMesh;
	}
}
