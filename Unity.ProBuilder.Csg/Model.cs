using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.ProBuilder.Csg
{
	internal sealed class Model
	{
		public List<Material> materials
		{
			get
			{
				return this.m_Materials;
			}
			set
			{
				this.m_Materials = value;
			}
		}

		public List<Vertex> vertices
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

		public List<List<int>> indices
		{
			get
			{
				return this.m_Indices;
			}
			set
			{
				this.m_Indices = value;
			}
		}

		public Mesh mesh
		{
			get
			{
				return (Mesh)this;
			}
		}

		public Model(GameObject gameObject)
		{
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			Mesh mesh = (component != null) ? component.sharedMesh : null;
			MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
			this..ctor(mesh, (component2 != null) ? component2.sharedMaterials : null, gameObject.GetComponent<Transform>());
		}

		public Model(Mesh mesh, Material[] materials, Transform transform)
		{
			if (mesh == null)
			{
				throw new ArgumentNullException("mesh");
			}
			if (transform == null)
			{
				throw new ArgumentNullException("transform");
			}
			this.m_Vertices = (from x in mesh.GetVertices()
			select transform.TransformVertex(x)).ToList<Vertex>();
			this.m_Materials = new List<Material>(materials);
			this.m_Indices = new List<List<int>>();
			int i = 0;
			int subMeshCount = mesh.subMeshCount;
			while (i < subMeshCount)
			{
				if (mesh.GetTopology(i) == MeshTopology.Triangles)
				{
					List<int> list = new List<int>();
					mesh.GetIndices(list, i);
					this.m_Indices.Add(list);
				}
				i++;
			}
		}

		internal Model(List<Polygon> polygons)
		{
			this.m_Vertices = new List<Vertex>();
			Dictionary<Material, List<int>> dictionary = new Dictionary<Material, List<int>>();
			int num = 0;
			for (int i = 0; i < polygons.Count; i++)
			{
				Polygon polygon = polygons[i];
				List<int> list;
				if (!dictionary.TryGetValue(polygon.material, out list))
				{
					dictionary.Add(polygon.material, list = new List<int>());
				}
				for (int j = 2; j < polygon.vertices.Count; j++)
				{
					this.m_Vertices.Add(polygon.vertices[0]);
					list.Add(num++);
					this.m_Vertices.Add(polygon.vertices[j - 1]);
					list.Add(num++);
					this.m_Vertices.Add(polygon.vertices[j]);
					list.Add(num++);
				}
			}
			this.m_Materials = dictionary.Keys.ToList<Material>();
			this.m_Indices = dictionary.Values.ToList<List<int>>();
		}

		internal List<Polygon> ToPolygons()
		{
			List<Polygon> list = new List<Polygon>();
			int i = 0;
			int count = this.m_Indices.Count;
			while (i < count)
			{
				List<int> list2 = this.m_Indices[i];
				int j = 0;
				int count2 = list2.Count;
				while (j < list2.Count)
				{
					List<Vertex> list3 = new List<Vertex>
					{
						this.m_Vertices[list2[j]],
						this.m_Vertices[list2[j + 1]],
						this.m_Vertices[list2[j + 2]]
					};
					list.Add(new Polygon(list3, this.m_Materials[i]));
					j += 3;
				}
				i++;
			}
			return list;
		}

		public static explicit operator Mesh(Model model)
		{
			Mesh mesh = new Mesh();
			VertexUtility.SetMesh(mesh, model.m_Vertices);
			mesh.subMeshCount = model.m_Indices.Count;
			int i = 0;
			int subMeshCount = mesh.subMeshCount;
			while (i < subMeshCount)
			{
				mesh.SetIndices(model.m_Indices[i], MeshTopology.Triangles, i, true, 0);
				i++;
			}
			return mesh;
		}

		private List<Vertex> m_Vertices;

		private List<Material> m_Materials;

		private List<List<int>> m_Indices;
	}
}
