using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public struct SphereOverlapQueryParams
	{
		public SphereOverlapQueryParams(QueryParams queryParams, Vector3 center, float radius, int staticHitsCapacity)
		{
			this.QueryParams = queryParams;
			this.Center = center;
			this.Radius = radius;
			this.StaticHitsCapacity = staticHitsCapacity;
		}

		public QueryParams QueryParams;

		public Vector3 Center;

		public float Radius;

		public int StaticHitsCapacity;
	}
}
