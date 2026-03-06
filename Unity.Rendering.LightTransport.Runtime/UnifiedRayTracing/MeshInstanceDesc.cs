using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct MeshInstanceDesc
	{
		public MeshInstanceDesc(Mesh mesh, int subMeshIndex = 0)
		{
			this.mesh = mesh;
			this.subMeshIndex = subMeshIndex;
			this.localToWorldMatrix = Matrix4x4.identity;
			this.mask = uint.MaxValue;
			this.instanceID = uint.MaxValue;
			this.enableTriangleCulling = true;
			this.frontTriangleCounterClockwise = false;
		}

		public Mesh mesh;

		public int subMeshIndex;

		public Matrix4x4 localToWorldMatrix;

		public uint mask;

		public uint instanceID;

		public bool enableTriangleCulling;

		public bool frontTriangleCounterClockwise;
	}
}
