using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion.LagCompensation;
using UnityEngine;

namespace Fusion
{
	[NetworkBehaviourWeaved(1)]
	[DisallowMultipleComponent]
	[AddComponentMenu("Fusion/Lag Compensation/Hitbox Root")]
	public class HitboxRoot : NetworkBehaviour
	{
		public unsafe bool HitboxRootActive
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (*base.ReinterpretState<uint>(0) & 1U) == 0U;
			}
			set
			{
				if (value)
				{
					*base.ReinterpretState<uint>(0) &= 4294967294U;
				}
				else
				{
					*base.ReinterpretState<uint>(0) |= 1U;
				}
			}
		}

		internal bool Registered
		{
			get
			{
				return BehaviourUtils.IsAlive(this.Manager);
			}
		}

		public HitboxManager Manager { get; internal set; }

		public bool InInterest
		{
			get
			{
				return base.Runner.IsInterestedIn(base.Object, base.Runner.LocalPlayer).GetValueOrDefault(true);
			}
		}

		private void Awake()
		{
			this.CachedTransform = base.transform;
		}

		public void OnDrawGizmos()
		{
			Vector3 pos;
			Quaternion q;
			base.transform.GetPositionAndRotation(out pos, out q);
			Matrix4x4 matrix4x = Matrix4x4.TRS(pos, q, Vector3.one);
			this.DrawGizmos(this.GizmosColor, ref matrix4x);
		}

		protected virtual void DrawGizmos(Color color, ref Matrix4x4 localToWorldMatrix)
		{
			Gizmos.matrix = localToWorldMatrix;
			Gizmos.color = color;
			Gizmos.DrawWireSphere(this.Offset, this.BroadRadius);
			Gizmos.color = Color.white;
			Gizmos.matrix = Matrix4x4.identity;
		}

		[EditorButton("Find Hitboxes", EditorButtonVisibility.EditMode, 0, false)]
		public void InitHitboxes()
		{
			bool includeInactive = (this.Config & HitboxRoot.ConfigFlags.IncludeInactiveHitboxes) == HitboxRoot.ConfigFlags.IncludeInactiveHitboxes;
			this.Hitboxes = base.transform.GetNestedComponentsInChildren(null, includeInactive).ToArray();
			bool flag = this.Hitboxes.Length > 31;
			if (flag)
			{
				Debug.LogWarning(string.Format("Hitbox count above limit per root, clamped to max {0}", 31));
				Array.Resize<Hitbox>(ref this.Hitboxes, 31);
			}
			for (int i = 0; i < this.Hitboxes.Length; i++)
			{
				Hitbox hitbox = this.Hitboxes[i];
				hitbox._hitboxIndex = i;
				hitbox.Root = this;
			}
			bool flag2 = this.BroadRadius == 0f;
			if (flag2)
			{
				this.SetMinBoundingRadius();
			}
		}

		[EditorButton("Quick Set BroadRadius", EditorButtonVisibility.Always, 0, true)]
		public void SetMinBoundingRadius()
		{
			bool flag = this.Hitboxes.Length == 0;
			if (!flag)
			{
				Vector3 b = base.transform.position + base.transform.rotation * this.Offset;
				float num = 0f;
				foreach (Hitbox hitbox in this.Hitboxes)
				{
					bool flag2 = hitbox.Type == HitboxTypes.None;
					if (!flag2)
					{
						Vector3 a = hitbox.transform.position + hitbox.transform.rotation * hitbox.Offset;
						float num2 = (a - b).magnitude;
						switch (hitbox.Type)
						{
						case HitboxTypes.Box:
							num2 += hitbox.BoxExtents.magnitude;
							break;
						case HitboxTypes.Sphere:
							num2 += hitbox.AbsSphereRadius;
							break;
						case HitboxTypes.Capsule:
							num2 += Mathf.Max(hitbox.CapsuleExtents * 0.5f, hitbox.CapsuleRadius);
							break;
						}
						bool flag3 = num2 > num;
						if (flag3)
						{
							num = num2;
						}
					}
				}
				this.BroadRadius = num;
			}
		}

		public void SetHitboxActive(Hitbox hitbox, bool setActive)
		{
			bool flag = hitbox.HitboxIndex >= 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Hitbox index {0} is outside the valid range: [0, {1})", hitbox.HitboxIndex, 31));
			}
			bool flag2 = !BehaviourUtils.IsSame(this, hitbox.Root);
			if (flag2)
			{
				Assert.Fail(string.Format("Hitbox '{0}' is part of a different HitboxRoot '{1}' than this '{2}'. Are you missing a call to {3} to update the root reference?", new object[]
				{
					hitbox,
					hitbox.Root,
					this,
					"InitHitboxes"
				}));
			}
			else
			{
				bool flag3 = hitbox.HitboxIndex >= this.Hitboxes.Length || !BehaviourUtils.IsSame(this.Hitboxes[hitbox.HitboxIndex], hitbox);
				if (flag3)
				{
					Assert.Fail(string.Format("Hitbox '{0}' (index {1}) does not match Root's Hitbox of same index: '{2}'. Are you missing a call to {3} to update the root reference?", new object[]
					{
						hitbox,
						hitbox.HitboxIndex,
						this.Hitboxes[hitbox.HitboxIndex],
						"InitHitboxes"
					}));
				}
				else
				{
					this.SetHitboxActiveFastUnchecked(hitbox, setActive);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe void SetHitboxActiveFastUnchecked(Hitbox hitbox, bool setActive)
		{
			if (setActive)
			{
				*base.ReinterpretState<uint>(0) &= ~hitbox.HitboxMask;
			}
			else
			{
				*base.ReinterpretState<uint>(0) |= hitbox.HitboxMask;
			}
		}

		public bool IsHitboxActive(Hitbox hitbox)
		{
			bool flag = hitbox.HitboxIndex >= 31;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Hitbox index {0} is outside the valid range: [0, {1})", hitbox.HitboxIndex, 31));
			}
			bool flag2 = !BehaviourUtils.IsSame(this, hitbox.Root);
			bool result;
			if (flag2)
			{
				Assert.Fail(string.Format("Hitbox '{0}' is part of a different HitboxRoot '{1}' than this '{2}'. Are you missing a call to {3} to update the root reference?", new object[]
				{
					hitbox,
					hitbox.Root,
					this,
					"InitHitboxes"
				}));
				result = false;
			}
			else
			{
				bool flag3 = hitbox.HitboxIndex >= this.Hitboxes.Length || !BehaviourUtils.IsSame(this.Hitboxes[hitbox.HitboxIndex], hitbox);
				if (flag3)
				{
					Assert.Fail(string.Format("Hitbox '{0}' (index {1}) does not match Root's Hitbox of same index: '{2}'. Are you missing a call to {3} to update the root reference?", new object[]
					{
						hitbox,
						hitbox.HitboxIndex,
						this.Hitboxes[hitbox.HitboxIndex],
						"InitHitboxes"
					}));
					result = false;
				}
				else
				{
					result = this.IsHitboxActiveFastUnchecked(hitbox);
				}
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe bool IsHitboxActiveFastUnchecked(Hitbox hitbox)
		{
			return (*base.ReinterpretState<uint>(0) & hitbox.HitboxMask) == 0U;
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			bool flag = this.Registered && BehaviourUtils.IsAlive(this.Manager);
			if (flag)
			{
				HitboxManager manager = this.Manager;
				this.Manager = null;
				manager.Remove(this);
			}
		}

		internal void RegisterColliders(IHitboxColliderContainer container, int tick)
		{
			bool flag = (this.Config & HitboxRoot.ConfigFlags.ReinitializeHitboxesBeforeRegistration) == HitboxRoot.ConfigFlags.ReinitializeHitboxesBeforeRegistration;
			if (flag)
			{
				this.InitHitboxes();
			}
			bool flag2 = this.Hitboxes == null;
			if (!flag2)
			{
				foreach (Hitbox hitbox in this.Hitboxes)
				{
					int colliderIndex;
					ref HitboxCollider nextCollider = ref container.GetNextCollider(out colliderIndex);
					bool flag3 = !hitbox.gameObject.activeSelf;
					if (flag3)
					{
						hitbox.CacheInfo();
					}
					hitbox.SetColliderData(ref nextCollider, tick);
					hitbox.ColliderIndex = colliderIndex;
				}
			}
		}

		internal void DeregisterColliders(IHitboxColliderContainer container)
		{
			bool flag = this.Hitboxes == null;
			if (!flag)
			{
				foreach (Hitbox hitbox in this.Hitboxes)
				{
					bool flag2 = hitbox.ColliderIndex > 0;
					if (flag2)
					{
						container.ReleaseCollider(hitbox.ColliderIndex);
					}
				}
			}
		}

		internal Bounds GetBounds()
		{
			Vector3 vector = base.transform.TransformPoint(this.Offset);
			return new Bounds
			{
				min = new Vector3(vector.x - this.BroadRadius, vector.y - this.BroadRadius, vector.z - this.BroadRadius),
				max = new Vector3(vector.x + this.BroadRadius, vector.y + this.BroadRadius, vector.z + this.BroadRadius)
			};
		}

		private const int WORD_COUNT = 1;

		public const int MAX_HITBOXES = 31;

		[InlineHelp]
		public HitboxRoot.ConfigFlags Config = HitboxRoot.ConfigFlags.Default;

		[InlineHelp]
		[Unit(Units.Units)]
		public float BroadRadius;

		[InlineHelp]
		public Vector3 Offset;

		[InlineHelp]
		public Color GizmosColor = Color.gray;

		[InlineHelp]
		[Space(4f)]
		public Hitbox[] Hitboxes;

		internal Transform CachedTransform;

		[Flags]
		public enum ConfigFlags
		{
			ReinitializeHitboxesBeforeRegistration = 1,
			IncludeInactiveHitboxes = 2,
			Legacy = 1,
			Default = 3
		}

		internal class HitboxComparerX : IComparer<HitboxRoot>
		{
			public int Compare(HitboxRoot a, HitboxRoot b)
			{
				return a.transform.position.x.CompareTo(b.transform.position.x);
			}
		}

		internal class HitboxComparerY : IComparer<HitboxRoot>
		{
			public int Compare(HitboxRoot a, HitboxRoot b)
			{
				return a.transform.position.y.CompareTo(b.transform.position.y);
			}
		}

		internal class HitboxComparerZ : IComparer<HitboxRoot>
		{
			public int Compare(HitboxRoot a, HitboxRoot b)
			{
				return a.transform.position.z.CompareTo(b.transform.position.z);
			}
		}
	}
}
