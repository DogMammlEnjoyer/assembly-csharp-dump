using System;
using System.Collections.Generic;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public class CuttableSubMesh
	{
		public CuttableSubMesh(bool hasNormals, bool hasColours, bool hasUvs, bool hasUv1)
		{
			this.vertices = new List<Vector3>();
			if (hasNormals)
			{
				this.normals = new List<Vector3>();
			}
			if (hasColours)
			{
				this.colours = new List<Color32>();
			}
			if (hasUvs)
			{
				this.uvs = new List<Vector2>();
			}
			if (hasUv1)
			{
				this.uv1s = new List<Vector2>();
			}
		}

		public CuttableSubMesh(int[] indices, Vector3[] inputVertices, Vector3[] inputNormals, Color32[] inputColours, Vector2[] inputUvs, Vector2[] inputUv1)
		{
			this.vertices = new List<Vector3>();
			if (inputNormals != null && inputNormals.Length != 0)
			{
				this.normals = new List<Vector3>();
			}
			if (inputColours != null && inputColours.Length != 0)
			{
				this.colours = new List<Color32>();
			}
			if (inputUvs != null && inputUvs.Length != 0)
			{
				this.uvs = new List<Vector2>();
			}
			if (inputUv1 != null && inputUv1.Length != 0)
			{
				this.uv1s = new List<Vector2>();
			}
			foreach (int num in indices)
			{
				this.vertices.Add(inputVertices[num]);
				if (this.normals != null)
				{
					this.normals.Add(inputNormals[num]);
				}
				if (this.colours != null)
				{
					this.colours.Add(inputColours[num]);
				}
				if (this.uvs != null)
				{
					this.uvs.Add(inputUvs[num]);
				}
				if (this.uv1s != null)
				{
					this.uv1s.Add(inputUv1[num]);
				}
			}
		}

		public void Add(CuttableSubMesh other)
		{
			for (int i = 0; i < other.vertices.Count; i++)
			{
				this.CopyVertex(i, other);
			}
		}

		public int NumVertices()
		{
			return this.vertices.Count;
		}

		public Vector3 GetVertex(int index)
		{
			return this.vertices[index];
		}

		public bool HasNormals()
		{
			return this.normals != null;
		}

		public bool HasColours()
		{
			return this.colours != null;
		}

		public bool HasUvs()
		{
			return this.uvs != null;
		}

		public bool HasUv1()
		{
			return this.uv1s != null;
		}

		public void CopyVertex(int srcIndex, CuttableSubMesh srcMesh)
		{
			this.vertices.Add(srcMesh.vertices[srcIndex]);
			if (this.normals != null)
			{
				this.normals.Add(srcMesh.normals[srcIndex]);
			}
			if (this.colours != null)
			{
				this.colours.Add(srcMesh.colours[srcIndex]);
			}
			if (this.uvs != null)
			{
				this.uvs.Add(srcMesh.uvs[srcIndex]);
			}
			if (this.uv1s != null)
			{
				this.uv1s.Add(srcMesh.uv1s[srcIndex]);
			}
		}

		public void AddInterpolatedVertex(int i0, int i1, float weight, CuttableSubMesh srcMesh)
		{
			Vector3 vertex = srcMesh.GetVertex(i0);
			Vector3 vertex2 = srcMesh.GetVertex(i1);
			this.vertices.Add(Vector3.Lerp(vertex, vertex2, weight));
			if (this.normals != null)
			{
				this.normals.Add(Vector3.Lerp(srcMesh.normals[i0], srcMesh.normals[i1], weight).normalized);
			}
			if (this.colours != null)
			{
				this.colours.Add(Color32.Lerp(srcMesh.colours[i0], srcMesh.colours[i1], weight));
			}
			if (this.uvs != null)
			{
				this.uvs.Add(Vector2.Lerp(srcMesh.uvs[i0], srcMesh.uvs[i1], weight));
			}
			if (this.uv1s != null)
			{
				this.uv1s.Add(Vector2.Lerp(srcMesh.uv1s[i0], srcMesh.uv1s[i1], weight));
			}
		}

		public void AddTo(List<Vector3> destVertices, List<Vector3> destNormals, List<Color32> destColours, List<Vector2> destUvs, List<Vector2> destUv1s)
		{
			destVertices.AddRange(this.vertices);
			if (this.normals != null)
			{
				destNormals.AddRange(this.normals);
			}
			if (this.colours != null)
			{
				destColours.AddRange(this.colours);
			}
			if (this.uvs != null)
			{
				destUvs.AddRange(this.uvs);
			}
			if (this.uv1s != null)
			{
				destUv1s.AddRange(this.uv1s);
			}
		}

		public int NumIndices()
		{
			return this.vertices.Count;
		}

		public int[] GenIndices()
		{
			int[] array = new int[this.vertices.Count];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = i;
			}
			return array;
		}

		private List<Vector3> vertices;

		private List<Vector3> normals;

		private List<Color32> colours;

		private List<Vector2> uvs;

		private List<Vector2> uv1s;
	}
}
