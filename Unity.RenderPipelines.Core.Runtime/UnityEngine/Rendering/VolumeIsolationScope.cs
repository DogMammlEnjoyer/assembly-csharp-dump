using System;

namespace UnityEngine.Rendering
{
	[Obsolete("VolumeIsolationScope is deprecated, it does not have any effect anymore.")]
	public struct VolumeIsolationScope : IDisposable
	{
		public VolumeIsolationScope(bool unused)
		{
		}

		void IDisposable.Dispose()
		{
		}
	}
}
