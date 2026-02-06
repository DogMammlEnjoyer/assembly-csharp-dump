using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public class RaycastAllQuery : RaycastQuery
	{
		public RaycastAllQuery(ref RaycastQueryParams raycastQueryParams) : base(ref raycastQueryParams)
		{
			this._physXRaycastHits = new RaycastHit[raycastQueryParams.StaticHitsCapacity];
			this._box2DRaycastHits = new RaycastHit2D[raycastQueryParams.StaticHitsCapacity];
		}

		public RaycastAllQuery(ref RaycastQueryParams raycastQueryParams, RaycastHit[] physXRaycastHitsCache, RaycastHit2D[] box2DRaycastHitCache) : base(ref raycastQueryParams)
		{
			this._physXRaycastHits = physXRaycastHitsCache;
			this._box2DRaycastHits = box2DRaycastHitCache;
		}

		internal override bool NarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
		{
			int count = hits.Count;
			foreach (int index in candidates)
			{
				ref HitboxCollider collider = ref container.GetCollider(index);
				Vector3 point;
				Vector3 normal;
				float distance;
				bool flag = base.NarrowPhaseRay(ref collider, this.Origin, this.Direction, this.Length, out point, out normal, out distance);
				if (flag)
				{
					hits.Add(base.CreateHitboxHit(ref collider, point, distance, normal));
				}
			}
			return hits.Count > count;
		}

		internal override void PerformStaticQuery(NetworkRunner runner, List<LagCompensatedHit> hits, HitOptions options)
		{
			bool flag = (options & HitOptions.IncludePhysX) > HitOptions.None;
			if (flag)
			{
				int num = runner.GetPhysicsScene().Raycast(this.Origin, this.Direction, this._physXRaycastHits, this.Length, this.LayerMask, this.TriggerInteraction);
				for (int i = 0; i < num; i++)
				{
					hits.Add((LagCompensatedHit)this._physXRaycastHits[i]);
				}
			}
			else
			{
				bool flag2 = (options & HitOptions.IncludeBox2D) > HitOptions.None;
				if (flag2)
				{
					int num2 = runner.GetPhysicsScene2D().Raycast(this.Origin, this.Direction, this.Length, this._box2DRaycastHits, this.LayerMask);
					for (int j = 0; j < num2; j++)
					{
						hits.Add((LagCompensatedHit)this._box2DRaycastHits[j]);
					}
				}
			}
		}

		private RaycastHit[] _physXRaycastHits;

		private RaycastHit2D[] _box2DRaycastHits;
	}
}
