using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct TriangleMeshGeometry : IGeometry
	{
		public Vector3 Scale
		{
			get
			{
				return this.m_Scale;
			}
			set
			{
				this.m_Scale = value;
			}
		}

		public Quaternion ScaleAxisRotation
		{
			get
			{
				return this.m_Rotation;
			}
			set
			{
				this.m_Rotation = value;
			}
		}

		public GeometryType GeometryType
		{
			get
			{
				return GeometryType.TriangleMesh;
			}
		}

		private Vector3 m_Scale;

		private Quaternion m_Rotation;

		private byte m_MeshFlags;

		private byte pad1;

		private short pad2;

		private IntPtr m_TriangleMesh;

		private uint pad3;
	}
}
