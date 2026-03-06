using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/Interactors/Sphere Interaction Caster", 22)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.Casters.SphereInteractionCaster.html")]
	public class SphereInteractionCaster : InteractionCasterBase
	{
		public LayerMask physicsLayerMask
		{
			get
			{
				return this.m_PhysicsLayerMask;
			}
			set
			{
				this.m_PhysicsLayerMask = value;
			}
		}

		public QueryTriggerInteraction physicsTriggerInteraction
		{
			get
			{
				return this.m_PhysicsTriggerInteraction;
			}
			set
			{
				this.m_PhysicsTriggerInteraction = value;
			}
		}

		public float castRadius
		{
			get
			{
				return this.m_CastRadius;
			}
			set
			{
				this.m_CastRadius = value;
			}
		}

		protected virtual void OnEnable()
		{
			this.m_FirstFrame = true;
			this.m_LastSphereCastOrigin = Vector3.zero;
		}

		protected virtual void OnDisable()
		{
		}

		public override bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
		{
			if (!base.TryGetColliderTargets(interactionManager, targets))
			{
				return false;
			}
			Vector3 position = base.effectiveCastOrigin.position;
			Vector3 lastSphereCastOrigin = this.m_LastSphereCastOrigin;
			Vector3 position2 = position;
			float radius = this.m_CastRadius * base.transform.lossyScale.x;
			Vector3 direction;
			float num;
			float maxDistance;
			BurstPhysicsUtils.GetSphereOverlapParameters(lastSphereCastOrigin, position2, out direction, out num, out maxDistance);
			bool result;
			if (this.m_FirstFrame || num < 0.001f)
			{
				int num2 = this.m_LocalPhysicsScene.OverlapSphere(position2, radius, this.m_OverlapSphereColliderHits, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int i = 0; i < num2; i++)
				{
					targets.Add(this.m_OverlapSphereColliderHits[i]);
				}
				result = (num2 > 0);
			}
			else
			{
				int num3 = this.m_LocalPhysicsScene.SphereCast(lastSphereCastOrigin, radius, direction, this.m_OverlapSphereHits, maxDistance, this.m_PhysicsLayerMask, this.m_PhysicsTriggerInteraction);
				for (int j = 0; j < num3; j++)
				{
					targets.Add(this.m_OverlapSphereHits[j].collider);
				}
				result = (num3 > 0);
			}
			this.m_LastSphereCastOrigin = position;
			this.m_FirstFrame = false;
			return result;
		}

		protected override bool InitializeCaster()
		{
			if (!base.isInitialized)
			{
				this.m_LocalPhysicsScene = base.gameObject.scene.GetPhysicsScene();
				base.isInitialized = true;
			}
			return base.isInitialized;
		}

		private const int k_MaxRaycastHits = 10;

		private readonly RaycastHit[] m_OverlapSphereHits = new RaycastHit[10];

		private readonly Collider[] m_OverlapSphereColliderHits = new Collider[10];

		[Header("Filtering Settings")]
		[SerializeField]
		[Tooltip("Layer mask used for limiting sphere cast and sphere overlap targets.")]
		private LayerMask m_PhysicsLayerMask = -1;

		[SerializeField]
		[Tooltip("Determines whether the cast sphere overlap will hit triggers. Use Global refers to the Queries Hit Triggers setting in Physics Project Settings.")]
		private QueryTriggerInteraction m_PhysicsTriggerInteraction = QueryTriggerInteraction.Ignore;

		[Header("Sphere Casting Settings")]
		[SerializeField]
		[Tooltip("Radius of the sphere cast.")]
		private float m_CastRadius = 0.1f;

		private bool m_FirstFrame = true;

		private Vector3 m_LastSphereCastOrigin = Vector3.zero;

		private PhysicsScene m_LocalPhysicsScene;
	}
}
