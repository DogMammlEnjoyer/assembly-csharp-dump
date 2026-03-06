using System;

namespace UnityEngine.Rendering
{
	public struct OcclusionCullingSettings
	{
		public OcclusionCullingSettings(int viewInstanceID, OcclusionTest occlusionTest)
		{
			this.viewInstanceID = viewInstanceID;
			this.occlusionTest = occlusionTest;
			this.instanceMultiplier = 1;
		}

		public int viewInstanceID;

		public OcclusionTest occlusionTest;

		public int instanceMultiplier;
	}
}
