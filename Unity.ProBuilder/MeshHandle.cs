using System;

namespace UnityEngine.ProBuilder
{
	internal sealed class MeshHandle
	{
		public Mesh mesh
		{
			get
			{
				return this.m_Mesh;
			}
		}

		public MeshHandle(Transform transform, Mesh mesh)
		{
			this.m_Transform = transform;
			this.m_Mesh = mesh;
		}

		public void DrawMeshNow(int submeshIndex)
		{
			if (this.m_Transform == null || this.m_Mesh == null)
			{
				return;
			}
			Graphics.DrawMeshNow(this.m_Mesh, this.m_Transform.localToWorldMatrix, submeshIndex);
		}

		private Transform m_Transform;

		private Mesh m_Mesh;
	}
}
