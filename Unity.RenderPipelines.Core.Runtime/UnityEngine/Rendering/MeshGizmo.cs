using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UnityEngine.Rendering
{
	internal class MeshGizmo : IDisposable
	{
		public MeshGizmo(int capacity = 0)
		{
			this.vertices = new List<Vector3>(capacity);
			this.indices = new List<int>(capacity);
			this.colors = new List<Color>(capacity);
			this.mesh = new Mesh
			{
				indexFormat = IndexFormat.UInt32,
				hideFlags = HideFlags.HideAndDontSave
			};
		}

		public void Clear()
		{
			this.vertices.Clear();
			this.indices.Clear();
			this.colors.Clear();
		}

		public void AddWireCube(Vector3 center, Vector3 size, Color color)
		{
			MeshGizmo.<>c__DisplayClass10_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.color = color;
			Vector3 vector = size / 2f;
			Vector3 b = new Vector3(vector.x, vector.y, vector.z);
			Vector3 b2 = new Vector3(-vector.x, vector.y, vector.z);
			Vector3 b3 = new Vector3(-vector.x, -vector.y, vector.z);
			Vector3 b4 = new Vector3(vector.x, -vector.y, vector.z);
			Vector3 b5 = new Vector3(vector.x, vector.y, -vector.z);
			Vector3 b6 = new Vector3(-vector.x, vector.y, -vector.z);
			Vector3 b7 = new Vector3(-vector.x, -vector.y, -vector.z);
			Vector3 b8 = new Vector3(vector.x, -vector.y, -vector.z);
			this.<AddWireCube>g__AddEdge|10_0(center + b, center + b2, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b2, center + b3, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b3, center + b4, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b4, center + b, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b5, center + b6, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b6, center + b7, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b7, center + b8, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b8, center + b5, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b, center + b5, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b2, center + b6, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b3, center + b7, ref CS$<>8__locals1);
			this.<AddWireCube>g__AddEdge|10_0(center + b4, center + b8, ref CS$<>8__locals1);
		}

		private void DrawMesh(Matrix4x4 trs, Material mat, MeshTopology topology, CompareFunction depthTest, string gizmoName)
		{
			this.mesh.Clear();
			this.mesh.SetVertices(this.vertices);
			this.mesh.SetColors(this.colors);
			this.mesh.SetIndices(this.indices, topology, 0, true, 0);
			mat.SetFloat("_HandleZTest", (float)depthTest);
			CommandBuffer commandBuffer = CommandBufferPool.Get(gizmoName ?? "Mesh Gizmo Rendering");
			commandBuffer.DrawMesh(this.mesh, trs, mat, 0, 0);
			Graphics.ExecuteCommandBuffer(commandBuffer);
		}

		public void RenderWireframe(Matrix4x4 trs, CompareFunction depthTest = CompareFunction.LessEqual, string gizmoName = null)
		{
			this.DrawMesh(trs, this.wireMaterial, MeshTopology.Lines, depthTest, gizmoName);
		}

		public void Dispose()
		{
			CoreUtils.Destroy(this.mesh);
		}

		[CompilerGenerated]
		private void <AddWireCube>g__AddEdge|10_0(Vector3 p1, Vector3 p2, ref MeshGizmo.<>c__DisplayClass10_0 A_3)
		{
			this.vertices.Add(p1);
			this.vertices.Add(p2);
			this.indices.Add(this.indices.Count);
			this.indices.Add(this.indices.Count);
			this.colors.Add(A_3.color);
			this.colors.Add(A_3.color);
		}

		public static readonly int vertexCountPerCube = 24;

		public Mesh mesh;

		private List<Vector3> vertices;

		private List<int> indices;

		private List<Color> colors;

		private Material wireMaterial;

		private Material dottedWireMaterial;

		private Material solidMaterial;
	}
}
