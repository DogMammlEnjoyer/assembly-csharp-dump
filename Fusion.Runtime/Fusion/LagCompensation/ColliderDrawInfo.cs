using System;
using UnityEngine;

namespace Fusion.LagCompensation
{
	public class ColliderDrawInfo
	{
		public HitboxTypes Type
		{
			get
			{
				return this.Container.GetCollider(this.Index).Type;
			}
		}

		public Vector3 BoxExtents
		{
			get
			{
				return this.Container.GetCollider(this.Index).BoxExtents;
			}
		}

		public Vector3 Offset
		{
			get
			{
				return this.Container.GetCollider(this.Index).Offset;
			}
		}

		public float Radius
		{
			get
			{
				return this.Container.GetCollider(this.Index).Radius;
			}
		}

		public float CapsuleExtents
		{
			get
			{
				return this.Container.GetCollider(this.Index).CapsuleExtents;
			}
		}

		public Vector3 CapsuleTopCenter
		{
			get
			{
				return this.Container.GetCollider(this.Index).CapsuleLocalTopCenter;
			}
		}

		public Vector3 CapsuleBottomCenter
		{
			get
			{
				return this.Container.GetCollider(this.Index).CapsuleLocalBottomCenter;
			}
		}

		public Matrix4x4 LocalToWorldMatrix
		{
			get
			{
				return this.Container.GetCollider(this.Index).LocalToWorld;
			}
		}

		internal ColliderDrawInfo FromHitboxCollider(int colliderIndex)
		{
			this.Index = colliderIndex;
			return this;
		}

		internal void SetContainer(IHitboxColliderContainer container)
		{
			this.Container = container;
		}

		internal int Index;

		internal IHitboxColliderContainer Container;
	}
}
