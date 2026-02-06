using System;
using System.Runtime.CompilerServices;
using Fusion.LagCompensation;
using UnityEngine;

namespace Fusion
{
	[AddComponentMenu("Fusion/Lag Compensation/Hitbox")]
	public class Hitbox : Behaviour
	{
		internal float AbsSphereRadius
		{
			get
			{
				return Mathf.Abs(this.SphereRadius);
			}
		}

		internal float AbsCapsuleRadius
		{
			get
			{
				return Mathf.Abs(this.CapsuleRadius);
			}
		}

		internal Vector3 CapsuleTopCenter
		{
			get
			{
				return this.Offset + Vector3.up * (Mathf.Max(this.CapsuleExtents * 0.5f, this.AbsCapsuleRadius) - this.AbsCapsuleRadius);
			}
		}

		internal Vector3 CapsuleBottomCenter
		{
			get
			{
				return this.Offset + Vector3.down * (Mathf.Max(this.CapsuleExtents * 0.5f, this.AbsCapsuleRadius) - this.AbsCapsuleRadius);
			}
		}

		internal Vector3 AbsBoxExtents
		{
			get
			{
				Vector3 result;
				result.x = Mathf.Abs(this.BoxExtents.x);
				result.y = Mathf.Abs(this.BoxExtents.y);
				result.z = Mathf.Abs(this.BoxExtents.z);
				return result;
			}
		}

		public int HitboxIndex
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this._hitboxIndex;
			}
		}

		internal uint HitboxMask
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				Assert.Check(this._hitboxIndex < 31);
				return 1U << this._hitboxIndex + 1;
			}
		}

		public bool HitboxActive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return BehaviourUtils.IsAlive(this.Root) && this.Root.IsHitboxActive(this);
			}
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				bool flag = BehaviourUtils.IsAlive(this.Root);
				if (flag)
				{
					this.Root.SetHitboxActive(this, value);
				}
			}
		}

		public int ColliderIndex { get; internal set; }

		public Vector3 Position
		{
			get
			{
				return base.transform.position + base.transform.rotation * this.Offset;
			}
		}

		private void Awake()
		{
			this.CacheInfo();
		}

		internal void CacheInfo()
		{
			this._cachedLayerMask = 1 << base.gameObject.layer;
			this._cachedTransform = base.transform;
		}

		internal void SetColliderData(ref HitboxCollider c, int tick)
		{
			Assert.Check(BehaviourUtils.IsAlive(this.Root));
			c.Type = this.Type;
			c.Offset = this.Offset;
			this._cachedTransform.GetPositionAndRotation(out c.Position, out c.Rotation);
			c.ResetCachedMatrix();
			c.BoxExtents = this.AbsBoxExtents;
			c.Radius = ((this.Type == HitboxTypes.Sphere) ? this.AbsSphereRadius : this.AbsCapsuleRadius);
			c.CapsuleExtents = this.CapsuleExtents;
			c.Hitbox = this;
			c.DebugTick = tick;
			c.layerMask = this._cachedLayerMask;
			c.Active = this.Root.IsHitboxActiveFastUnchecked(this);
			c.IsBoxNarrowDataInitialized = false;
		}

		public void SetLayer(int layer)
		{
			base.gameObject.layer = layer;
			this._cachedLayerMask = 1 << layer;
		}

		public void OnDrawGizmos()
		{
			Color gizmosColor = this.GizmosColor;
			bool flag = BehaviourUtils.IsAlive(this.Root) && this.Root.StateBufferIsValid && (!this.Root.HitboxRootActive || !this.Root.IsHitboxActiveFastUnchecked(this));
			if (flag)
			{
				gizmosColor.a *= 0.1f;
			}
			Matrix4x4 matrix4x = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
			this.DrawGizmos(gizmosColor, ref matrix4x);
		}

		protected virtual void DrawGizmos(Color color, ref Matrix4x4 localToWorldMatrix)
		{
			Gizmos.matrix = localToWorldMatrix;
			Gizmos.color = color;
			switch (this.Type)
			{
			case HitboxTypes.Box:
				Gizmos.DrawWireCube(this.Offset, this.AbsBoxExtents * 2f);
				break;
			case HitboxTypes.Sphere:
				Gizmos.DrawWireSphere(this.Offset, this.AbsSphereRadius);
				break;
			case HitboxTypes.Capsule:
				LagCompensationDraw.GizmosDrawWireCapsule(this.CapsuleTopCenter, this.CapsuleBottomCenter, this.AbsCapsuleRadius);
				break;
			}
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
		}

		[InlineHelp]
		public HitboxTypes Type;

		[InlineHelp]
		[DrawIf("Type", 2L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		[Unit(Units.Units)]
		public float SphereRadius;

		[InlineHelp]
		[DrawIf("Type", 3L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		[Unit(Units.Units)]
		public float CapsuleRadius;

		[InlineHelp]
		[DrawIf("Type", 1L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		public Vector3 BoxExtents;

		[InlineHelp]
		[DrawIf("Type", 3L, CompareOperator.Equal, DrawIfMode.ReadOnly, Hide = true)]
		[Unit(Units.Units)]
		public float CapsuleExtents;

		[DrawIf("Type", Hide = true)]
		public Vector3 Offset;

		[HideInInspector]
		public HitboxRoot Root;

		internal int _hitboxIndex;

		[InlineHelp]
		public Color GizmosColor = Color.yellow;

		private int _cachedLayerMask;

		private Transform _cachedTransform;
	}
}
