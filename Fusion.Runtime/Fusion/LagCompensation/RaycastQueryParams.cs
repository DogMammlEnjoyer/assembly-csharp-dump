using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public struct RaycastQueryParams
	{
		public RaycastQueryParams(QueryParams queryParams, Vector3 origin, Vector3 direction, float length, int staticHitsCapacity = 64)
		{
			this.QueryParams = queryParams;
			this.Origin = origin;
			this.Direction = direction;
			this.Length = length;
			this.StaticHitsCapacity = staticHitsCapacity;
		}

		public QueryParams QueryParams;

		public Vector3 Origin;

		public Vector3 Direction;

		public float Length;

		public int StaticHitsCapacity;
	}
}
