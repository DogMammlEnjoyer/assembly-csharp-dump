using System;

namespace UnityEngine.LowLevelPhysics
{
	public struct ConvexMeshGeometry : IGeometry
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
				return GeometryType.ConvexMesh;
			}
		}

		private Vector3 m_Scale;

		private Quaternion m_Rotation;

		private IntPtr m_ConvexMesh;

		private byte m_MeshFlags;

		private byte pad1;

		private short pad2;

		private uint pad3;
	}
}
