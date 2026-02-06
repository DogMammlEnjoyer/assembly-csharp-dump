using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public struct BoxOverlapQueryParams
	{
		public BoxOverlapQueryParams(QueryParams queryParams, Vector3 center, Vector3 extents, Quaternion rotation, int staticHitsCapacity)
		{
			this.QueryParams = queryParams;
			this.Center = center;
			this.Extents = extents;
			this.Rotation = rotation;
			this.StaticHitsCapacity = staticHitsCapacity;
		}

		public QueryParams QueryParams;

		public Vector3 Center;

		public Vector3 Extents;

		public Quaternion Rotation;

		public int StaticHitsCapacity;
	}
}
