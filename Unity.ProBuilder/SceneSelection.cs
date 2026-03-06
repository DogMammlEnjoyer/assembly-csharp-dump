using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UnityEngine.ProBuilder
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class SceneSelection : IEquatable<SceneSelection>
	{
		public List<int> vertexes
		{
			get
			{
				return this.m_Vertices;
			}
			set
			{
				this.m_Vertices = value;
			}
		}

		public List<Edge> edges
		{
			get
			{
				return this.m_Edges;
			}
			set
			{
				this.m_Edges = value;
			}
		}

		public List<Face> faces
		{
			get
			{
				return this.m_Faces;
			}
			set
			{
				this.m_Faces = value;
			}
		}

		public SceneSelection(GameObject gameObject = null)
		{
			this.gameObject = gameObject;
			this.m_Vertices = new List<int>();
			this.m_Edges = new List<Edge>();
			this.m_Faces = new List<Face>();
		}

		public SceneSelection(ProBuilderMesh mesh, int vertex) : this(mesh, new List<int>
		{
			vertex
		})
		{
		}

		public SceneSelection(ProBuilderMesh mesh, Edge edge) : this(mesh, new List<Edge>
		{
			edge
		})
		{
		}

		public SceneSelection(ProBuilderMesh mesh, Face face) : this(mesh, new List<Face>
		{
			face
		})
		{
		}

		internal SceneSelection(ProBuilderMesh mesh, List<int> vertexes) : this((mesh != null) ? mesh.gameObject : null)
		{
			this.mesh = mesh;
			this.m_Vertices = vertexes;
			this.m_Edges = new List<Edge>();
			this.m_Faces = new List<Face>();
		}

		internal SceneSelection(ProBuilderMesh mesh, List<Edge> edges) : this((mesh != null) ? mesh.gameObject : null)
		{
			this.mesh = mesh;
			this.vertexes = new List<int>();
			this.edges = edges;
			this.faces = new List<Face>();
		}

		internal SceneSelection(ProBuilderMesh mesh, List<Face> faces) : this((mesh != null) ? mesh.gameObject : null)
		{
			this.mesh = mesh;
			this.vertexes = new List<int>();
			this.edges = new List<Edge>();
			this.faces = faces;
		}

		public void SetSingleFace(Face face)
		{
			this.faces.Clear();
			this.faces.Add(face);
		}

		public void SetSingleVertex(int vertex)
		{
			this.vertexes.Clear();
			this.vertexes.Add(vertex);
		}

		public void SetSingleEdge(Edge edge)
		{
			this.edges.Clear();
			this.edges.Add(edge);
		}

		public void Clear()
		{
			this.gameObject = null;
			this.mesh = null;
			this.faces.Clear();
			this.edges.Clear();
			this.vertexes.Clear();
		}

		public void CopyTo(SceneSelection dst)
		{
			dst.gameObject = this.gameObject;
			dst.mesh = this.mesh;
			dst.faces.Clear();
			dst.edges.Clear();
			dst.vertexes.Clear();
			dst.faces.AddRange(this.faces);
			dst.edges.AddRange(this.edges);
			dst.vertexes.AddRange(this.vertexes);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("GameObject: " + ((this.gameObject != null) ? this.gameObject.name : null));
			stringBuilder.AppendLine("ProBuilderMesh: " + ((this.mesh != null) ? this.mesh.name : null));
			stringBuilder.AppendLine("Face: " + ((this.faces != null) ? this.faces.ToString() : null));
			stringBuilder.AppendLine("Edge: " + this.edges.ToString());
			string str = "Vertex: ";
			List<int> vertexes = this.vertexes;
			stringBuilder.AppendLine(str + ((vertexes != null) ? vertexes.ToString() : null));
			return stringBuilder.ToString();
		}

		public bool Equals(SceneSelection other)
		{
			return other != null && (this == other || (object.Equals(this.gameObject, other.gameObject) && object.Equals(this.mesh, other.mesh) && this.vertexes.SequenceEqual(other.vertexes) && this.edges.SequenceEqual(other.edges) && this.faces.SequenceEqual(other.faces)));
		}

		public override bool Equals(object obj)
		{
			return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((SceneSelection)obj)));
		}

		public override int GetHashCode()
		{
			return (((((this.gameObject != null) ? this.gameObject.GetHashCode() : 0) * 397 ^ ((this.mesh != null) ? this.mesh.GetHashCode() : 0)) * 397 ^ ((this.vertexes != null) ? this.vertexes.GetHashCode() : 0)) * 397 ^ ((this.edges != null) ? this.edges.GetHashCode() : 0)) * 397 ^ ((this.faces != null) ? this.faces.GetHashCode() : 0);
		}

		public static bool operator ==(SceneSelection left, SceneSelection right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(SceneSelection left, SceneSelection right)
		{
			return !object.Equals(left, right);
		}

		public GameObject gameObject;

		public ProBuilderMesh mesh;

		private List<int> m_Vertices;

		private List<Edge> m_Edges;

		private List<Face> m_Faces;

		[Obsolete("Use SetSingleVertex")]
		public int vertex;

		[Obsolete("Use SetSingleEdge")]
		public Edge edge;

		[Obsolete("Use SetSingleFace")]
		public Face face;
	}
}
