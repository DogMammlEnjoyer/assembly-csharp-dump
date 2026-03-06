using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal class ReferenceCounter
	{
		public void Inc()
		{
			this.value += 1UL;
		}

		public void Dec()
		{
			this.value -= 1UL;
		}

		public ulong value;
	}
}
