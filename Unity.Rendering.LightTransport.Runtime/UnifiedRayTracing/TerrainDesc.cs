using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct TerrainDesc
	{
		public TerrainDesc(Terrain terrain)
		{
			this.terrain = terrain;
			this.localToWorldMatrix = Matrix4x4.identity;
			this.mask = uint.MaxValue;
			this.renderingLayerMask = uint.MaxValue;
			this.materialID = 0U;
			this.enableTriangleCulling = true;
			this.frontTriangleCounterClockwise = false;
		}

		public Terrain terrain;

		public Matrix4x4 localToWorldMatrix;

		public uint mask;

		public uint renderingLayerMask;

		public uint materialID;

		public bool enableTriangleCulling;

		public bool frontTriangleCounterClockwise;
	}
}
