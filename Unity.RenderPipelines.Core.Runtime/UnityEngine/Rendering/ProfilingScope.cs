using System;

namespace UnityEngine.Rendering
{
	public struct ProfilingScope : IDisposable
	{
		public ProfilingScope(ProfilingSampler sampler)
		{
		}

		public ProfilingScope(CommandBuffer cmd, ProfilingSampler sampler)
		{
		}

		public ProfilingScope(BaseCommandBuffer cmd, ProfilingSampler sampler)
		{
		}

		public void Dispose()
		{
		}
	}
}
