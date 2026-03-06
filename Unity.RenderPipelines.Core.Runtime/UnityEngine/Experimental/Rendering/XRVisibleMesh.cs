using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	internal class XRVisibleMesh
	{
		internal XRVisibleMesh(XRPass xrPass)
		{
			this.m_Pass = xrPass;
		}

		internal void Dispose()
		{
			if (this.m_CombinedMesh)
			{
				CoreUtils.Destroy(this.m_CombinedMesh);
				this.m_CombinedMesh = null;
			}
		}

		internal bool hasValidVisibleMesh
		{
			get
			{
				if (!this.IsVisibleMeshSupported())
				{
					return false;
				}
				if (this.m_Pass.singlePassEnabled)
				{
					return this.m_CombinedMesh != null;
				}
				return this.m_Pass.GetVisibleMesh(0) != null;
			}
		}

		internal void RenderVisibleMeshCustomMaterial(CommandBuffer cmd, float occlusionMeshScale, Material material, MaterialPropertyBlock materialBlock, int shaderPass, bool yFlip = false)
		{
			if (this.IsVisibleMeshSupported())
			{
				using (new ProfilingScope(cmd, XRVisibleMesh.k_VisibleMeshProfilingSampler))
				{
					Vector3 vector = new Vector3(occlusionMeshScale, yFlip ? occlusionMeshScale : (-occlusionMeshScale), 1f);
					Mesh mesh = this.m_Pass.singlePassEnabled ? this.m_CombinedMesh : this.m_Pass.GetVisibleMesh(0);
					cmd.DrawMesh(mesh, Matrix4x4.Scale(vector), material, 0, shaderPass, materialBlock);
				}
			}
		}

		internal void UpdateCombinedMesh()
		{
			int num;
			if (this.IsVisibleMeshSupported() && this.m_Pass.singlePassEnabled && this.TryGetVisibleMeshCombinedHashCode(out num))
			{
				if (this.m_CombinedMesh == null || num != this.m_CombinedMeshHashCode)
				{
					this.CreateVisibleMeshCombined();
					this.m_CombinedMeshHashCode = num;
					return;
				}
			}
			else
			{
				this.m_CombinedMesh = null;
				this.m_CombinedMeshHashCode = 0;
			}
		}

		private bool IsVisibleMeshSupported()
		{
			return this.m_Pass.enabled && this.m_Pass.occlusionMeshScale > 0f;
		}

		private bool TryGetVisibleMeshCombinedHashCode(out int hashCode)
		{
			hashCode = 17;
			for (int i = 0; i < this.m_Pass.viewCount; i++)
			{
				Mesh visibleMesh = this.m_Pass.GetVisibleMesh(i);
				if (!(visibleMesh != null))
				{
					hashCode = 0;
					return false;
				}
				hashCode = hashCode * 23 + visibleMesh.GetHashCode();
			}
			return true;
		}

		private void CreateVisibleMeshCombined()
		{
			CoreUtils.Destroy(this.m_CombinedMesh);
			this.m_CombinedMesh = new Mesh();
			this.m_CombinedMesh.indexFormat = IndexFormat.UInt16;
			int num = 0;
			uint num2 = 0U;
			for (int i = 0; i < this.m_Pass.viewCount; i++)
			{
				Mesh visibleMesh = this.m_Pass.GetVisibleMesh(i);
				num += visibleMesh.vertexCount;
				num2 += visibleMesh.GetIndexCount(0);
			}
			Vector3[] array = new Vector3[num];
			ushort[] array2 = new ushort[num2];
			int num3 = 0;
			int num4 = 0;
			for (int j = 0; j < this.m_Pass.viewCount; j++)
			{
				Mesh visibleMesh2 = this.m_Pass.GetVisibleMesh(j);
				int[] indices = visibleMesh2.GetIndices(0);
				visibleMesh2.vertices.CopyTo(array, num3);
				for (int k = 0; k < visibleMesh2.vertices.Length; k++)
				{
					array[num3 + k].z = (float)j;
				}
				for (int l = 0; l < indices.Length; l++)
				{
					int num5 = num3 + indices[l];
					array2[num4 + l] = (ushort)num5;
				}
				num3 += visibleMesh2.vertexCount;
				num4 += indices.Length;
			}
			this.m_CombinedMesh.vertices = array;
			this.m_CombinedMesh.SetIndices(array2, MeshTopology.Triangles, 0, true, 0);
		}

		private XRPass m_Pass;

		private Mesh m_CombinedMesh;

		private int m_CombinedMeshHashCode;

		private static readonly ProfilingSampler k_VisibleMeshProfilingSampler = new ProfilingSampler("XR Visible Mesh");
	}
}
