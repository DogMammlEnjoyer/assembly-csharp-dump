using System;

namespace UnityEngine.Rendering
{
	public struct OccluderSubviewUpdate
	{
		public OccluderSubviewUpdate(int subviewIndex)
		{
			this.subviewIndex = subviewIndex;
			this.depthSliceIndex = 0;
			this.depthOffset = Vector2Int.zero;
			this.viewMatrix = Matrix4x4.identity;
			this.invViewMatrix = Matrix4x4.identity;
			this.gpuProjMatrix = Matrix4x4.identity;
			this.viewOffsetWorldSpace = Vector3.zero;
		}

		public int subviewIndex;

		public int depthSliceIndex;

		public Vector2Int depthOffset;

		public Matrix4x4 viewMatrix;

		public Matrix4x4 invViewMatrix;

		public Matrix4x4 gpuProjMatrix;

		public Vector3 viewOffsetWorldSpace;
	}
}
