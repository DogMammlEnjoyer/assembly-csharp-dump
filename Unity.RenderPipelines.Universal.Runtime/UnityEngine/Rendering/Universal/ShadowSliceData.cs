using System;

namespace UnityEngine.Rendering.Universal
{
	public struct ShadowSliceData
	{
		public void Clear()
		{
			this.viewMatrix = Matrix4x4.identity;
			this.projectionMatrix = Matrix4x4.identity;
			this.shadowTransform = Matrix4x4.identity;
			this.offsetX = (this.offsetY = 0);
			this.resolution = 1024;
		}

		public Matrix4x4 viewMatrix;

		public Matrix4x4 projectionMatrix;

		public Matrix4x4 shadowTransform;

		public int offsetX;

		public int offsetY;

		public int resolution;

		public ShadowSplitData splitData;
	}
}
