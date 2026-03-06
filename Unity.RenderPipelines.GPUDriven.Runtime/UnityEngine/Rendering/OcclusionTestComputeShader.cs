using System;

namespace UnityEngine.Rendering
{
	internal struct OcclusionTestComputeShader
	{
		public void Init(ComputeShader cs)
		{
			this.cs = cs;
			this.occlusionDebugKeyword = new LocalKeyword(cs, "OCCLUSION_DEBUG");
		}

		public ComputeShader cs;

		public LocalKeyword occlusionDebugKeyword;
	}
}
