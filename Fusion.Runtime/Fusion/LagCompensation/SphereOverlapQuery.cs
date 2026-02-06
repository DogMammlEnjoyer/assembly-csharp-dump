using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public class SphereOverlapQuery : Query
	{
		public SphereOverlapQuery(ref SphereOverlapQueryParams sphereOverlapParams) : base(ref sphereOverlapParams.QueryParams)
		{
			this.Center = sphereOverlapParams.Center;
			this.Radius = sphereOverlapParams.Radius;
			this._physXOverlapHits = new Collider[sphereOverlapParams.StaticHitsCapacity];
			this._box2DOverlapHits = new Collider2D[sphereOverlapParams.StaticHitsCapacity];
		}

		public SphereOverlapQuery(ref SphereOverlapQueryParams sphereOverlapParams, Collider[] physXOverlapHitsCache, Collider2D[] box2DOverlapHitsCache) : base(ref sphereOverlapParams.QueryParams)
		{
			this.Center = sphereOverlapParams.Center;
			this.Radius = sphereOverlapParams.Radius;
			this._physXOverlapHits = physXOverlapHitsCache;
			this._box2DOverlapHits = box2DOverlapHitsCache;
		}

		protected override bool Check(ref AABB bounds)
		{
			return LagCompensationUtils.LocalAABBSphereIntersection(bounds.Extents, this.Center - bounds.Center, this.Radius);
		}

		internal override bool NarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
		{
			bool result = false;
			foreach (int index in candidates)
			{
				ref HitboxCollider collider = ref container.GetCollider(index);
				Vector3 point;
				Vector3 normal;
				bool flag = this.NarrowPhaseSphere(ref collider, this.Center, this.Radius, out point, out normal);
				if (flag)
				{
					result = true;
					hits.Add(base.CreateHitboxHit(ref collider, point, 0f, normal));
				}
			}
			return result;
		}

		internal override void PerformStaticQuery(NetworkRunner runner, List<LagCompensatedHit> hits, HitOptions options)
		{
			bool flag = (options & HitOptions.IncludePhysX) > HitOptions.None;
			if (flag)
			{
				int num = runner.GetPhysicsScene().OverlapSphere(this.Center, this.Radius, this._physXOverlapHits, this.LayerMask, this.TriggerInteraction);
				for (int i = 0; i < num; i++)
				{
					Collider collider = this._physXOverlapHits[i];
					LagCompensatedHit lagCompensatedHit = new LagCompensatedHit
					{
						Collider = collider,
						Normal = default(Vector3),
						Distance = 0f,
						GameObject = collider.gameObject,
						Type = HitType.PhysX
					};
					LagCompensatedHit item = lagCompensatedHit;
					hits.Add(item);
				}
			}
			else
			{
				bool flag2 = (options & HitOptions.IncludeBox2D) > HitOptions.None;
				if (flag2)
				{
					int num2 = runner.GetPhysicsScene2D().OverlapCircle(this.Center, this.Radius, this._box2DOverlapHits, this.LayerMask);
					for (int j = 0; j < num2; j++)
					{
						Collider2D collider2D = this._box2DOverlapHits[j];
						LagCompensatedHit lagCompensatedHit = new LagCompensatedHit
						{
							Collider2D = collider2D,
							Normal = default(Vector3),
							Distance = 0f,
							GameObject = collider2D.gameObject,
							Type = HitType.Box2D
						};
						LagCompensatedHit item2 = lagCompensatedHit;
						hits.Add(item2);
					}
				}
			}
		}

		internal bool NarrowPhaseSphere(ref HitboxCollider c, Vector3 origin, float radius, out Vector3 point, out Vector3 normal)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
			{
				Vector3 sphereCenter = c.LocalToWorld.inverse.MultiplyPoint(origin) - c.Offset;
				LagCompensationUtils.ContactData contactData;
				bool flag = LagCompensationUtils.LocalAABBSphereContact(c.BoxExtents, sphereCenter, radius, out contactData);
				if (flag)
				{
					point = c.LocalToWorld.MultiplyPoint(contactData.Point + c.Offset);
					normal = c.LocalToWorld.MultiplyVector(contactData.Normal);
					return true;
				}
				break;
			}
			case HitboxTypes.Sphere:
			{
				Vector3 centerB = c.LocalToWorld.MultiplyPoint(c.Offset);
				bool flag2 = LagCompensationUtils.SphereSphere(origin, radius, centerB, c.Radius, out point, out normal);
				if (flag2)
				{
					return true;
				}
				break;
			}
			case HitboxTypes.Capsule:
			{
				Vector3 sphereCenter2 = c.LocalToWorld.inverse.MultiplyPoint(origin);
				LagCompensationUtils.ContactData contactData2;
				bool flag3 = LagCompensationUtils.LocalSphereCapsuleIntersection(c.CapsuleLocalTopCenter, c.CapsuleLocalBottomCenter, c.Radius, sphereCenter2, radius, out contactData2);
				if (flag3)
				{
					point = c.LocalToWorld.MultiplyPoint(contactData2.Point);
					normal = c.LocalToWorld.MultiplyVector(contactData2.Normal);
					return true;
				}
				break;
			}
			}
			point = default(Vector3);
			normal = default(Vector3);
			return false;
		}

		public Vector3 Center;

		public float Radius;

		private Collider[] _physXOverlapHits;

		private Collider2D[] _box2DOverlapHits;
	}
}
