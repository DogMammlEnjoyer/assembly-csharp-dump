using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Technie.PhysicsCreator
{
	public class CuttableMesh
	{
		public CuttableMesh(Mesh inputMesh)
		{
			this.Init(inputMesh, inputMesh.name);
		}

		public CuttableMesh(MeshRenderer input)
		{
			this.inputMeshRenderer = input;
			Mesh sharedMesh = input.GetComponent<MeshFilter>().sharedMesh;
			this.Init(sharedMesh, input.name);
		}

		private void Init(Mesh inputMesh, string debugName)
		{
			this.subMeshes = new List<CuttableSubMesh>();
			if (inputMesh.isReadable)
			{
				Vector3[] vertices = inputMesh.vertices;
				Vector3[] normals = inputMesh.normals;
				Vector2[] uv = inputMesh.uv;
				Vector2[] uv2 = inputMesh.uv2;
				Color32[] colors = inputMesh.colors32;
				this.hasUvs = (uv != null && uv.Length != 0);
				this.hasUv1s = (uv2 != null && uv2.Length != 0);
				this.hasColours = (colors != null && colors.Length != 0);
				for (int i = 0; i < inputMesh.subMeshCount; i++)
				{
					CuttableSubMesh item = new CuttableSubMesh(inputMesh.GetIndices(i), vertices, normals, colors, uv, uv2);
					this.subMeshes.Add(item);
				}
				return;
			}
			Debug.LogError("CuttableMesh's input mesh is not readable: " + debugName, inputMesh);
		}

		public CuttableMesh(CuttableMesh inputMesh, List<CuttableSubMesh> newSubMeshes)
		{
			this.inputMeshRenderer = inputMesh.inputMeshRenderer;
			this.hasUvs = inputMesh.hasUvs;
			this.hasUv1s = inputMesh.hasUv1s;
			this.hasColours = inputMesh.hasColours;
			this.subMeshes = new List<CuttableSubMesh>();
			this.subMeshes.AddRange(newSubMeshes);
		}

		public void Add(CuttableMesh other)
		{
			if (this.subMeshes.Count != other.subMeshes.Count)
			{
				throw new Exception("Mismatched submesh count");
			}
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				this.subMeshes[i].Add(other.subMeshes[i]);
			}
		}

		public int NumSubMeshes()
		{
			return this.subMeshes.Count;
		}

		public bool HasUvs()
		{
			return this.hasUvs;
		}

		public bool HasColours()
		{
			return this.hasColours;
		}

		public List<CuttableSubMesh> GetSubMeshes()
		{
			return this.subMeshes;
		}

		public CuttableSubMesh GetSubMesh(int index)
		{
			return this.subMeshes[index];
		}

		public Transform GetTransform()
		{
			if (this.inputMeshRenderer != null)
			{
				return this.inputMeshRenderer.transform;
			}
			return null;
		}

		public MeshRenderer ConvertToRenderer(string newObjectName)
		{
			Mesh mesh = this.CreateMesh();
			if (mesh.vertexCount == 0)
			{
				return null;
			}
			GameObject gameObject = new GameObject(newObjectName);
			gameObject.transform.SetParent(this.inputMeshRenderer.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.AddComponent<MeshFilter>().mesh = mesh;
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.shadowCastingMode = this.inputMeshRenderer.shadowCastingMode;
			meshRenderer.reflectionProbeUsage = this.inputMeshRenderer.reflectionProbeUsage;
			meshRenderer.lightProbeUsage = this.inputMeshRenderer.lightProbeUsage;
			meshRenderer.sharedMaterials = this.inputMeshRenderer.sharedMaterials;
			return meshRenderer;
		}

		public Mesh CreateMesh()
		{
			Mesh mesh = new Mesh();
			int num = 0;
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				num += this.subMeshes[i].NumIndices();
			}
			mesh.indexFormat = ((num > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			List<Color32> list3 = this.hasColours ? new List<Color32>() : null;
			List<Vector2> list4 = this.hasUvs ? new List<Vector2>() : null;
			List<Vector2> list5 = this.hasUv1s ? new List<Vector2>() : null;
			List<int> list6 = new List<int>();
			foreach (CuttableSubMesh cuttableSubMesh in this.subMeshes)
			{
				list6.Add(list.Count);
				cuttableSubMesh.AddTo(list, list2, list3, list4, list5);
			}
			mesh.vertices = list.ToArray();
			mesh.normals = list2.ToArray();
			mesh.colors32 = (this.hasColours ? list3.ToArray() : null);
			mesh.uv = (this.hasUvs ? list4.ToArray() : null);
			mesh.uv2 = (this.hasUv1s ? list5.ToArray() : null);
			mesh.subMeshCount = this.subMeshes.Count;
			for (int j = 0; j < this.subMeshes.Count; j++)
			{
				CuttableSubMesh cuttableSubMesh2 = this.subMeshes[j];
				int baseVertex = list6[j];
				int[] triangles = cuttableSubMesh2.GenIndices();
				mesh.SetTriangles(triangles, j, true, baseVertex);
			}
			return mesh;
		}

		private MeshRenderer inputMeshRenderer;

		private bool hasUvs;

		private bool hasUv1s;

		private bool hasColours;

		private List<CuttableSubMesh> subMeshes;
	}
}
