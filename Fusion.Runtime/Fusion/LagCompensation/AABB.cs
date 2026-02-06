using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	public readonly struct AABB
	{
		public AABB(Bounds bounds)
		{
			this.Center = bounds.center;
			this.Extents = bounds.extents;
			this.Min = bounds.min;
			this.Max = bounds.max;
		}

		public AABB(Vector3 center, Vector3 extents)
		{
			this.Center = center;
			this.Extents = extents;
			this.Min = center - extents;
			this.Max = center + extents;
		}

		public AABB(Vector3 center, Vector3 pointA, Vector3 pointB)
		{
			this.Max = default(Vector3);
			this.Center = center;
			this.Extents = this.Max - this.Center;
			this.Min = Vector3.Min(pointA, pointB);
			this.Max = Vector3.Max(pointA, pointB);
		}

		public readonly Vector3 Center;

		public readonly Vector3 Extents;

		public readonly Vector3 Min;

		public readonly Vector3 Max;
	}
}
