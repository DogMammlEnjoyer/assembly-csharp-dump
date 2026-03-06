using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct GeometryPoolEntryInfo
	{
		public static GeometryPoolEntryInfo NewDefault()
		{
			return new GeometryPoolEntryInfo
			{
				valid = false,
				refCount = 0U
			};
		}

		public bool valid;

		public uint refCount;
	}
}
