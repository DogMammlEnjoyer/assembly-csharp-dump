using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshBoundaryEdgeMidpoints : IPointSet
	{
		public MeshBoundaryEdgeMidpoints(DMesh3 mesh)
		{
			this.Mesh = mesh;
			this.num_boundary_edges = 0;
			foreach (int num in mesh.BoundaryEdgeIndices())
			{
				this.num_boundary_edges++;
			}
		}

		public int VertexCount
		{
			get
			{
				return this.num_boundary_edges;
			}
		}

		public int MaxVertexID
		{
			get
			{
				return this.Mesh.MaxEdgeID;
			}
		}

		public bool HasVertexNormals
		{
			get
			{
				return false;
			}
		}

		public bool HasVertexColors
		{
			get
			{
				return false;
			}
		}

		public Vector3d GetVertex(int i)
		{
			return this.Mesh.GetEdgePoint(i, 0.5);
		}

		public Vector3f GetVertexNormal(int i)
		{
			return Vector3f.AxisY;
		}

		public Vector3f GetVertexColor(int i)
		{
			return Vector3f.One;
		}

		public bool IsVertex(int vID)
		{
			return this.Mesh.IsEdge(vID) && this.Mesh.IsBoundaryEdge(vID);
		}

		public IEnumerable<int> VertexIndices()
		{
			return this.Mesh.BoundaryEdgeIndices();
		}

		public int Timestamp
		{
			get
			{
				return this.Mesh.Timestamp;
			}
		}

		private int num_boundary_edges;

		public DMesh3 Mesh;
	}
}
