using System;

namespace Fusion.LagCompensation
{
	[Serializable]
	public struct PositionRotationQueryParams
	{
		public PositionRotationQueryParams(QueryParams queryParams, Hitbox hitbox)
		{
			this.QueryParams = queryParams;
			this.Hitbox = hitbox;
		}

		public QueryParams QueryParams;

		public Hitbox Hitbox;
	}
}
