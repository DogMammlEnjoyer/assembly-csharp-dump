using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion.LagCompensation
{
	[Serializable]
	public abstract class Query : IBoundsTraversalTest
	{
		protected Query(ref QueryParams qParams)
		{
			this.PreProcessingDelegate = qParams.PreProcessingDelegate;
			this.TriggerInteraction = qParams.TriggerInteraction;
			this.LayerMask = qParams.LayerMask;
			this.UserArgs = qParams.UserArgs;
			this.Options = qParams.Options;
			this.TickTo = qParams.TickTo;
			this.Player = qParams.Player;
			this.Alpha = qParams.Alpha;
			this.Tick = new int?(qParams.Tick);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool IBoundsTraversalTest.Check(ref AABB bounds)
		{
			return this.Check(ref bounds);
		}

		protected abstract bool Check(ref AABB bounds);

		internal abstract bool NarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits);

		internal abstract void PerformStaticQuery(NetworkRunner runner, List<LagCompensatedHit> hits, HitOptions options);

		internal HitboxHit CreateHitboxHit(ref HitboxCollider collider, Vector3 point, float distance, Vector3 normal)
		{
			return new HitboxHit
			{
				Point = point,
				Distance = distance,
				Normal = normal,
				Hitbox = collider.Hitbox,
				DebugTick = collider.DebugTick,
				DebugPosition = collider.Position,
				DebugRotation = collider.Rotation,
				Alpha = 0f
			};
		}

		public QueryTriggerInteraction TriggerInteraction;

		public HitOptions Options;

		public LayerMask LayerMask;

		[NonSerialized]
		public PlayerRef Player;

		[NonSerialized]
		public int? Tick;

		[NonSerialized]
		public unsafe void* UserArgs;

		[NonSerialized]
		public float? Alpha;

		[NonSerialized]
		public int? TickTo;

		[NonSerialized]
		public PreProcessingDelegate PreProcessingDelegate;
	}
}
