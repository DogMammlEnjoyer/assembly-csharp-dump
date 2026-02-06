using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public class BoxOverlapQuery : Query
	{
		public BoxOverlapQuery(ref BoxOverlapQueryParams boxOverlapParams) : base(ref boxOverlapParams.QueryParams)
		{
			this.Rotation = boxOverlapParams.Rotation;
			this.Extents = boxOverlapParams.Extents;
			this.Center = boxOverlapParams.Center;
			this._physXOverlapHits = new Collider[boxOverlapParams.StaticHitsCapacity];
			this._box2DOverlapHits = new Collider2D[boxOverlapParams.StaticHitsCapacity];
			this._queryNarrowData = new LagCompensationUtils.BoxNarrowData(this.Center, this.Rotation, this.Extents);
		}

		public BoxOverlapQuery(ref BoxOverlapQueryParams boxOverlapParams, Collider[] physXOverlapHitsCache, Collider2D[] box2DOverlapHitsCache) : base(ref boxOverlapParams.QueryParams)
		{
			this.Rotation = boxOverlapParams.Rotation;
			this.Extents = boxOverlapParams.Extents;
			this.Center = boxOverlapParams.Center;
			this._physXOverlapHits = physXOverlapHitsCache;
			this._box2DOverlapHits = box2DOverlapHitsCache;
			this._queryNarrowData = new LagCompensationUtils.BoxNarrowData(this.Center, this.Rotation, this.Extents);
		}

		protected override bool Check(ref AABB bounds)
		{
			Vector3 b = this.Rotation * this.Extents;
			Vector3 pointB = this.Center + b;
			Vector3 pointA = this.Center - b;
			AABB aabb = new AABB(this.Center, pointA, pointB);
			return aabb.Min.x <= bounds.Max.x && aabb.Max.x >= bounds.Min.x && aabb.Min.y <= bounds.Max.y && aabb.Max.y >= bounds.Min.y && aabb.Min.z <= bounds.Max.z && aabb.Max.z >= bounds.Min.z;
		}

		internal override bool NarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
		{
			this._queryNarrowData = this.PreComputeNarrowData();
			bool result = false;
			foreach (int index in candidates)
			{
				ref HitboxCollider collider = ref container.GetCollider(index);
				Vector3 point;
				Vector3 normal;
				bool flag = this.NarrowPhaseBox(ref this._queryNarrowData, ref collider, true, out point, out normal);
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
				int num = runner.GetPhysicsScene().OverlapBox(this.Center, this.Extents, this._physXOverlapHits, this.Rotation, this.LayerMask, this.TriggerInteraction);
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
					float angle;
					Vector3 vector;
					this.Rotation.ToAngleAxis(out angle, out vector);
					int num2 = runner.GetPhysicsScene2D().OverlapBox(this.Center, this.Extents, angle, this._box2DOverlapHits, this.LayerMask);
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

		internal bool NarrowPhaseBox(ref LagCompensationUtils.BoxNarrowData boxQueryNarrowData, ref HitboxCollider c, bool computeDetailedInfo, out Vector3 hitPoint, out Vector3 hitNormal)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
				Assert.Check(c.IsBoxNarrowDataInitialized);
				return LagCompensationUtils.NarrowBoxBox(ref boxQueryNarrowData, ref c.BoxNarrowData, computeDetailedInfo, out hitPoint, out hitNormal);
			case HitboxTypes.Sphere:
			{
				Vector3 sphereCenter = boxQueryNarrowData.WorldToLocalPoint(c.Position + c.Rotation * c.Offset);
				LagCompensationUtils.ContactData contactData;
				bool flag = LagCompensationUtils.LocalAABBSphereContact(boxQueryNarrowData.Extents, sphereCenter, c.Radius, out contactData);
				if (flag)
				{
					hitPoint = boxQueryNarrowData.LocalToWorldPoint(contactData.Point);
					hitNormal = boxQueryNarrowData.LocalToWorldVector(-contactData.Normal);
					return true;
				}
				break;
			}
			case HitboxTypes.Capsule:
			{
				Vector3 localCapsulePointA = boxQueryNarrowData.WorldToLocalPoint(c.LocalToWorld.MultiplyPoint3x4(c.CapsuleLocalTopCenter));
				Vector3 localCapsulePointB = boxQueryNarrowData.WorldToLocalPoint(c.LocalToWorld.MultiplyPoint3x4(c.CapsuleLocalBottomCenter));
				Vector3 localCapsuleCenter = boxQueryNarrowData.WorldToLocalPoint(c.Position + c.Offset);
				LagCompensationUtils.ContactData contactData;
				bool flag2 = LagCompensationUtils.LocalAABBCapsuleIntersection(localCapsuleCenter, localCapsulePointA, localCapsulePointB, c.Radius, boxQueryNarrowData.Extents, out contactData);
				if (flag2)
				{
					hitPoint = boxQueryNarrowData.LocalToWorldPoint(contactData.Point);
					hitNormal = boxQueryNarrowData.LocalToWorldVector(contactData.Normal);
					return true;
				}
				break;
			}
			}
			hitPoint = default(Vector3);
			hitNormal = default(Vector3);
			return false;
		}

		internal LagCompensationUtils.BoxNarrowData PreComputeNarrowData()
		{
			bool flag = this.Rotation == Quaternion.identity;
			bool flag2;
			LagCompensationUtils.BoxNarrowData result;
			if (flag)
			{
				flag2 = false;
				result = default(LagCompensationUtils.BoxNarrowData);
			}
			else
			{
				flag2 = true;
				result = new LagCompensationUtils.BoxNarrowData(this.Center, this.Rotation, this.Extents);
				Vector3 start = result.BoxEdgesRotated.E00.Start;
				Vector3 start2 = result.BoxEdgesRotated.E01.Start;
				Vector3 start3 = result.BoxEdgesRotated.E02.Start;
				Vector3 start4 = result.BoxEdgesRotated.E03.Start;
				start.x = Mathf.Abs(start.x);
				start.y = Mathf.Abs(start.y);
				start.z = Mathf.Abs(start.z);
				start2.x = Mathf.Abs(start2.x);
				start2.y = Mathf.Abs(start2.y);
				start2.z = Mathf.Abs(start2.z);
				start3.x = Mathf.Abs(start3.x);
				start3.y = Mathf.Abs(start3.y);
				start3.z = Mathf.Abs(start3.z);
				start4.x = Mathf.Abs(start4.x);
				start4.y = Mathf.Abs(start4.y);
				start4.z = Mathf.Abs(start4.z);
				Vector3 vector;
				vector.x = Mathf.Max(start.x, start2.x);
				vector.y = Mathf.Max(start.y, start2.y);
				vector.z = Mathf.Max(start.z, start2.z);
				vector.x = Mathf.Max(vector.x, start3.x);
				vector.y = Mathf.Max(vector.y, start3.y);
				vector.z = Mathf.Max(vector.z, start3.z);
				vector.x = Mathf.Max(vector.x, start4.x);
				vector.y = Mathf.Max(vector.y, start4.y);
				vector.z = Mathf.Max(vector.z, start4.z);
			}
			bool flag3 = !flag2;
			if (flag3)
			{
				result = new LagCompensationUtils.BoxNarrowData(this.Center, this.Rotation, this.Extents);
			}
			return result;
		}

		public Vector3 Center;

		public Vector3 Extents;

		public Quaternion Rotation;

		private Collider[] _physXOverlapHits;

		private Collider2D[] _box2DOverlapHits;

		internal LagCompensationUtils.BoxNarrowData _queryNarrowData;
	}
}
