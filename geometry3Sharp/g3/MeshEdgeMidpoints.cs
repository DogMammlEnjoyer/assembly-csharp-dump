using System;
using System.Collections.Generic;

namespace g3
{
	public class MeshEdgeMidpoints : IPointSet
	{
		public MeshEdgeMidpoints(DMesh3 mesh)
		{
			this.Mesh = mesh;
		}

		public int VertexCount
		{
			get
			{
				return this.Mesh.EdgeCount;
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
			return this.Mesh.IsEdge(vID);
		}

		public IEnumerable<int> VertexIndices()
		{
			return this.Mesh.EdgeIndices();
		}

		public int Timestamp
		{
			get
			{
				return this.Mesh.Timestamp;
			}
		}

		public DMesh3 Mesh;
	}
}
