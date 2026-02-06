using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public class RaycastQuery : Query
	{
		public RaycastQuery(ref RaycastQueryParams raycastQueryParams) : base(ref raycastQueryParams.QueryParams)
		{
			this.Direction = raycastQueryParams.Direction;
			this.Origin = raycastQueryParams.Origin;
			this.Length = raycastQueryParams.Length;
		}

		protected override bool Check(ref AABB bounds)
		{
			Vector3 min = bounds.Min;
			Vector3 max = bounds.Max;
			float num = this.Length * this.Length;
			bool flag = true;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			Vector3 vector3 = default(Vector3);
			bool flag5 = this.Origin.x < min.x;
			if (flag5)
			{
				vector2.x = min.x;
				flag = false;
			}
			else
			{
				bool flag6 = this.Origin.x > max.x;
				if (flag6)
				{
					vector2.x = max.x;
					flag = false;
				}
				else
				{
					flag2 = true;
				}
			}
			bool flag7 = this.Origin.y < min.y;
			if (flag7)
			{
				vector2.y = min.y;
				flag = false;
			}
			else
			{
				bool flag8 = this.Origin.y > max.y;
				if (flag8)
				{
					vector2.y = max.y;
					flag = false;
				}
				else
				{
					flag3 = true;
				}
			}
			bool flag9 = this.Origin.z < min.z;
			if (flag9)
			{
				vector2.z = min.z;
				flag = false;
			}
			else
			{
				bool flag10 = this.Origin.z > max.z;
				if (flag10)
				{
					vector2.z = max.z;
					flag = false;
				}
				else
				{
					flag4 = true;
				}
			}
			bool flag11 = flag;
			bool result;
			if (flag11)
			{
				vector3 = this.Origin;
				result = true;
			}
			else
			{
				bool flag12 = this.Direction.x != 0f && !flag2;
				if (flag12)
				{
					vector.x = (vector2.x - this.Origin.x) / this.Direction.x;
				}
				else
				{
					vector.x = -1f;
				}
				bool flag13 = this.Direction.y != 0f && !flag3;
				if (flag13)
				{
					vector.y = (vector2.y - this.Origin.y) / this.Direction.y;
				}
				else
				{
					vector.y = -1f;
				}
				bool flag14 = this.Direction.z != 0f && !flag4;
				if (flag14)
				{
					vector.z = (vector2.z - this.Origin.z) / this.Direction.z;
				}
				else
				{
					vector.z = -1f;
				}
				int num2 = 0;
				float num3 = vector.x;
				bool flag15 = num3 < vector.y;
				if (flag15)
				{
					num2 = 1;
					num3 = vector.y;
				}
				bool flag16 = num3 < vector.z;
				if (flag16)
				{
					num2 = 2;
					num3 = vector.z;
				}
				bool flag17 = num3 < 0f;
				if (flag17)
				{
					result = false;
				}
				else
				{
					bool flag18 = num2 != 0;
					if (flag18)
					{
						vector3.x = this.Origin.x + num3 * this.Direction.x;
						bool flag19 = vector3.x < min.x || vector3.x > max.x;
						if (flag19)
						{
							return false;
						}
					}
					else
					{
						vector3.x = vector2.x;
					}
					bool flag20 = num2 != 1;
					if (flag20)
					{
						vector3.y = this.Origin.y + num3 * this.Direction.y;
						bool flag21 = vector3.y < min.y || vector3.y > max.y;
						if (flag21)
						{
							return false;
						}
					}
					else
					{
						vector3.y = vector2.y;
					}
					bool flag22 = num2 != 2;
					if (flag22)
					{
						vector3.z = this.Origin.z + num3 * this.Direction.z;
						bool flag23 = vector3.z < min.z || vector3.z > max.z;
						if (flag23)
						{
							return false;
						}
					}
					else
					{
						vector3.z = vector2.z;
					}
					Vector3 origin = this.Origin;
					origin.x -= vector3.x;
					origin.y -= vector3.y;
					origin.z -= vector3.z;
					bool flag24 = origin.sqrMagnitude <= num;
					result = flag24;
				}
			}
			return result;
		}

		internal override bool NarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
		{
			int count = hits.Count;
			float num = float.MaxValue;
			foreach (int index in candidates)
			{
				ref HitboxCollider collider = ref container.GetCollider(index);
				Vector3 point;
				Vector3 normal;
				float num2;
				bool flag = this.NarrowPhaseRay(ref collider, this.Origin, this.Direction, this.Length, out point, out normal, out num2);
				if (flag)
				{
					bool flag2 = num2 >= num;
					if (!flag2)
					{
						num = num2;
						hits.Insert(hits.Count, base.CreateHitboxHit(ref collider, point, num2, normal));
					}
				}
			}
			return hits.Count > count;
		}

		internal override void PerformStaticQuery(NetworkRunner runner, List<LagCompensatedHit> hits, HitOptions options)
		{
			bool flag = (options & HitOptions.IncludePhysX) > HitOptions.None;
			if (flag)
			{
				bool flag2 = runner.GetPhysicsScene().Raycast(this.Origin, this.Direction, out this._raycastHit, this.Length, this.LayerMask, this.TriggerInteraction);
				bool flag3 = flag2;
				if (flag3)
				{
					hits.Add((LagCompensatedHit)this._raycastHit);
				}
			}
			else
			{
				bool flag4 = (options & HitOptions.IncludeBox2D) > HitOptions.None;
				if (flag4)
				{
					this._raycastHit2D = runner.GetPhysicsScene2D().Raycast(this.Origin, this.Direction, this.Length, this.LayerMask);
					bool flag5 = this._raycastHit2D.collider != null;
					if (flag5)
					{
						hits.Add((LagCompensatedHit)this._raycastHit2D);
					}
				}
			}
		}

		internal bool NarrowPhaseRay(ref HitboxCollider c, Vector3 origin, Vector3 direction, float length, out Vector3 point, out Vector3 normal, out float distance)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
			{
				Matrix4x4 inverse = c.LocalToWorld.inverse;
				Vector3 vector = inverse.MultiplyPoint(origin) - c.Offset;
				Vector3 vector2 = inverse.MultiplyVector(direction);
				Vector3 vector3 = -c.BoxExtents;
				Vector3 boxExtents = c.BoxExtents;
				bool flag = LagCompensationUtils.RayAABB(ref vector3, ref boxExtents, ref vector, ref vector2, length * length, out point, out normal, out distance);
				if (flag)
				{
					point = c.LocalToWorld.MultiplyPoint(point + c.Offset);
					normal = c.LocalToWorld.MultiplyVector(normal);
					return true;
				}
				break;
			}
			case HitboxTypes.Sphere:
			{
				Vector3 center = c.LocalToWorld.MultiplyPoint(c.Offset);
				bool flag2 = LagCompensationUtils.RaySphereIntersection(origin, direction, length, center, c.Radius, out point, out normal, out distance);
				if (flag2)
				{
					return true;
				}
				break;
			}
			case HitboxTypes.Capsule:
			{
				Matrix4x4 inverse2 = c.LocalToWorld.inverse;
				Vector3 rayLocalOrigin = inverse2.MultiplyPoint(origin);
				Vector3 vector4 = inverse2.MultiplyVector(direction);
				bool flag3 = LagCompensationUtils.LocalRayCapsuleIntersection(c.CapsuleLocalTopCenter, c.CapsuleLocalBottomCenter, c.Radius, rayLocalOrigin, vector4.normalized, length, out point, out normal, out distance);
				if (flag3)
				{
					point = c.LocalToWorld.MultiplyPoint(point);
					normal = c.LocalToWorld.MultiplyVector(normal);
					return true;
				}
				break;
			}
			}
			point = default(Vector3);
			normal = default(Vector3);
			distance = 0f;
			return false;
		}

		public Vector3 Direction;

		public Vector3 Origin;

		public float Length;

		private RaycastHit _raycastHit;

		private RaycastHit2D _raycastHit2D;
	}
}
