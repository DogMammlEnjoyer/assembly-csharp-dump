using System;

namespace UnityEngine.Rendering.UnifiedRayTracing
{
	internal struct GeometryPoolHandle : IEquatable<GeometryPoolHandle>
	{
		public readonly bool valid
		{
			get
			{
				return this.index != -1;
			}
		}

		public bool Equals(GeometryPoolHandle other)
		{
			return this.index == other.index;
		}

		public int index;

		public static readonly GeometryPoolHandle Invalid = new GeometryPoolHandle
		{
			index = -1
		};
	}
}
