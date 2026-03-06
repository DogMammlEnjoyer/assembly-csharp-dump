using System;
using Pathfinding.Serialization;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding
{
	[Serializable]
	public class GraphCollision
	{
		public void Initialize(GraphTransform transform, float scale)
		{
			this.up = (transform.Transform(Vector3.up) - transform.Transform(Vector3.zero)).normalized;
			this.upheight = this.up * this.height;
			this.finalRadius = this.diameter * scale * 0.5f;
			this.finalRaycastRadius = this.thickRaycastDiameter * scale * 0.5f;
			this.contactFilter = new ContactFilter2D
			{
				layerMask = this.mask,
				useDepth = false,
				useLayerMask = true,
				useNormalAngle = false,
				useTriggers = false
			};
		}

		public bool Check(Vector3 position)
		{
			if (!this.collisionCheck)
			{
				return true;
			}
			if (this.use2D)
			{
				ColliderType colliderType = this.type;
				if (colliderType <= ColliderType.Capsule)
				{
					return Physics2D.OverlapCircle(position, this.finalRadius, this.contactFilter, GraphCollision.dummyArray) == 0;
				}
				return Physics2D.OverlapPoint(position, this.contactFilter, GraphCollision.dummyArray) == 0;
			}
			else
			{
				position += this.up * this.collisionOffset;
				ColliderType colliderType = this.type;
				if (colliderType == ColliderType.Sphere)
				{
					return !Physics.CheckSphere(position, this.finalRadius, this.mask, QueryTriggerInteraction.Ignore);
				}
				if (colliderType == ColliderType.Capsule)
				{
					return !Physics.CheckCapsule(position, position + this.upheight, this.finalRadius, this.mask, QueryTriggerInteraction.Ignore);
				}
				RayDirection rayDirection = this.rayDirection;
				if (rayDirection == RayDirection.Up)
				{
					return !Physics.Raycast(position, this.up, this.height, this.mask, QueryTriggerInteraction.Ignore);
				}
				if (rayDirection == RayDirection.Both)
				{
					return !Physics.Raycast(position, this.up, this.height, this.mask, QueryTriggerInteraction.Ignore) && !Physics.Raycast(position + this.upheight, -this.up, this.height, this.mask, QueryTriggerInteraction.Ignore);
				}
				return !Physics.Raycast(position + this.upheight, -this.up, this.height, this.mask, QueryTriggerInteraction.Ignore);
			}
		}

		public Vector3 CheckHeight(Vector3 position)
		{
			RaycastHit raycastHit;
			bool flag;
			return this.CheckHeight(position, out raycastHit, out flag);
		}

		public Vector3 CheckHeight(Vector3 position, out RaycastHit hit, out bool walkable)
		{
			walkable = true;
			if (!this.heightCheck || this.use2D)
			{
				hit = default(RaycastHit);
				return position;
			}
			if (this.thickRaycast)
			{
				Ray ray = new Ray(position + this.up * this.fromHeight, -this.up);
				if (Physics.SphereCast(ray, this.finalRaycastRadius, out hit, this.fromHeight + 0.005f, this.heightMask, QueryTriggerInteraction.Ignore))
				{
					return VectorMath.ClosestPointOnLine(ray.origin, ray.origin + ray.direction, hit.point);
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			else
			{
				if (Physics.Raycast(position + this.up * this.fromHeight, -this.up, out hit, this.fromHeight + 0.005f, this.heightMask, QueryTriggerInteraction.Ignore))
				{
					return hit.point;
				}
				walkable &= !this.unwalkableWhenNoGround;
			}
			return position;
		}

		public RaycastHit[] CheckHeightAll(Vector3 position, out int numHits)
		{
			if (!this.heightCheck || this.use2D)
			{
				this.hitBuffer[0] = new RaycastHit
				{
					point = position,
					distance = 0f
				};
				numHits = 1;
				return this.hitBuffer;
			}
			numHits = Physics.RaycastNonAlloc(position + this.up * this.fromHeight, -this.up, this.hitBuffer, this.fromHeight + 0.005f, this.heightMask, QueryTriggerInteraction.Ignore);
			if (numHits == this.hitBuffer.Length)
			{
				this.hitBuffer = new RaycastHit[this.hitBuffer.Length * 2];
				return this.CheckHeightAll(position, out numHits);
			}
			return this.hitBuffer;
		}

		public void DeserializeSettingsCompatibility(GraphSerializationContext ctx)
		{
			this.type = (ColliderType)ctx.reader.ReadInt32();
			this.diameter = ctx.reader.ReadSingle();
			this.height = ctx.reader.ReadSingle();
			this.collisionOffset = ctx.reader.ReadSingle();
			this.rayDirection = (RayDirection)ctx.reader.ReadInt32();
			this.mask = ctx.reader.ReadInt32();
			this.heightMask = ctx.reader.ReadInt32();
			this.fromHeight = ctx.reader.ReadSingle();
			this.thickRaycast = ctx.reader.ReadBoolean();
			this.thickRaycastDiameter = ctx.reader.ReadSingle();
			this.unwalkableWhenNoGround = ctx.reader.ReadBoolean();
			this.use2D = ctx.reader.ReadBoolean();
			this.collisionCheck = ctx.reader.ReadBoolean();
			this.heightCheck = ctx.reader.ReadBoolean();
		}

		public ColliderType type = ColliderType.Capsule;

		public float diameter = 1f;

		public float height = 2f;

		public float collisionOffset;

		public RayDirection rayDirection = RayDirection.Both;

		public LayerMask mask;

		public LayerMask heightMask = -1;

		public float fromHeight = 100f;

		public bool thickRaycast;

		public float thickRaycastDiameter = 1f;

		public bool unwalkableWhenNoGround = true;

		public bool use2D;

		public bool collisionCheck = true;

		public bool heightCheck = true;

		public Vector3 up;

		private Vector3 upheight;

		private ContactFilter2D contactFilter;

		private static Collider2D[] dummyArray = new Collider2D[1];

		private float finalRadius;

		private float finalRaycastRadius;

		public const float RaycastErrorMargin = 0.005f;

		private RaycastHit[] hitBuffer = new RaycastHit[8];
	}
}
