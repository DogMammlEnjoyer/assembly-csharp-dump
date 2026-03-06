using System;

namespace UnityEngine.Rendering
{
	internal struct RenderersBatchersContextDesc
	{
		public static RenderersBatchersContextDesc NewDefault()
		{
			return new RenderersBatchersContextDesc
			{
				instanceNumInfo = new InstanceNumInfo(1024, 32)
			};
		}

		public InstanceNumInfo instanceNumInfo;

		public bool supportDitheringCrossFade;

		public bool enableBoundingSpheresInstanceData;

		public float smallMeshScreenPercentage;

		public bool enableCullerDebugStats;
	}
}
