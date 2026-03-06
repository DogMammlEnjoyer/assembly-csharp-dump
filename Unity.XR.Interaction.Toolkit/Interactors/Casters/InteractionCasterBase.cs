using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors.Casters
{
	public abstract class InteractionCasterBase : MonoBehaviour, IInteractionCaster
	{
		public bool isInitialized { get; protected set; }

		public Transform castOrigin
		{
			get
			{
				return this.m_CastOrigin;
			}
			set
			{
				this.m_CastOrigin = value;
			}
		}

		public Transform effectiveCastOrigin
		{
			get
			{
				if (this.m_EnableStabilization && this.m_InitializedStabilizationOrigin)
				{
					return this.m_StabilizationAnchor;
				}
				return this.castOrigin;
			}
		}

		public bool enableStabilization
		{
			get
			{
				return this.m_EnableStabilization;
			}
			set
			{
				this.m_EnableStabilization = value;
			}
		}

		public float positionStabilization
		{
			get
			{
				return this.m_PositionStabilization;
			}
			set
			{
				this.m_PositionStabilization = value;
			}
		}

		public float angleStabilization
		{
			get
			{
				return this.m_AngleStabilization;
			}
			set
			{
				this.m_AngleStabilization = value;
			}
		}

		public IXRRayProvider aimTarget
		{
			get
			{
				return this.m_AimTargetObjectRef.Get(this.m_AimTargetObject);
			}
			set
			{
				this.m_AimTargetObjectRef.Set(ref this.m_AimTargetObject, value);
			}
		}

		protected virtual void OnValidate()
		{
			if (this.m_CastOrigin == null)
			{
				this.m_CastOrigin = base.transform;
			}
		}

		protected virtual void Awake()
		{
			if (this.m_CastOrigin == null)
			{
				this.m_CastOrigin = base.transform;
			}
			this.InitializeCaster();
			this.InitializeStabilization();
		}

		protected virtual void OnDestroy()
		{
			this.isInitialized = false;
		}

		public virtual bool TryGetColliderTargets(XRInteractionManager interactionManager, List<Collider> targets)
		{
			targets.Clear();
			if (!this.InitializeCaster() && !this.InitializeStabilization())
			{
				return false;
			}
			this.UpdateInternalData();
			return true;
		}

		protected abstract bool InitializeCaster();

		protected virtual void UpdateInternalData()
		{
			if (!this.m_EnableStabilization)
			{
				return;
			}
			float deltaTime = Time.unscaledTime - this.m_LastStabilizationUpdateTime;
			this.m_LastStabilizationUpdateTime = Time.unscaledTime;
			IXRRayProvider aimTarget = this.aimTarget;
			XRTransformStabilizer.ApplyStabilization(ref this.m_StabilizationAnchor, this.m_CastOrigin, aimTarget, this.m_PositionStabilization, this.m_AngleStabilization, deltaTime, false);
		}

		protected virtual bool InitializeStabilization()
		{
			if (!this.m_EnableStabilization || this.m_InitializedStabilizationOrigin)
			{
				return true;
			}
			if (this.m_StabilizationAnchor == null)
			{
				XROrigin xrorigin;
				if (!ComponentLocatorUtility<XROrigin>.TryFindComponent(out xrorigin))
				{
					Debug.LogError("Failed to find XROrigin component in scene. Cannot stabilize cast origin for " + base.GetType().Name + ".", this);
					this.m_EnableStabilization = false;
					return false;
				}
				string text = "";
				IXRInteractor ixrinteractor;
				if (base.TryGetComponent<IXRInteractor>(out ixrinteractor))
				{
					text = ixrinteractor.handedness.ToString();
				}
				this.m_StabilizationAnchor = new GameObject(string.Concat(new string[]
				{
					"[",
					text,
					" ",
					base.GetType().Name,
					"] Stabilization Cast Origin"
				})).transform;
				this.m_StabilizationAnchor.SetParent(xrorigin.Origin.transform, false);
				this.m_StabilizationAnchor.SetLocalPose(Pose.identity);
				this.m_InitializedStabilizationOrigin = true;
			}
			return this.m_InitializedStabilizationOrigin;
		}

		[SerializeField]
		[Tooltip("Source of origin and direction used when updating sample points.")]
		private Transform m_CastOrigin;

		[Header("Stabilization Parameters")]
		[SerializeField]
		[Tooltip("Determines whether to stabilize the cast origin.")]
		private bool m_EnableStabilization;

		[SerializeField]
		[Tooltip("Factor for stabilizing position. Larger values increase the range of stabilization, making the effect more pronounced over a greater distance.")]
		private float m_PositionStabilization = 0.25f;

		[SerializeField]
		[Tooltip("Factor for stabilizing angle. Larger values increase the range of stabilization, making the effect more pronounced over a greater angle.")]
		private float m_AngleStabilization = 20f;

		[SerializeField]
		[RequireInterface(typeof(IXRRayProvider))]
		[Tooltip("Optional ray provider for calculating stable rotation.")]
		private Object m_AimTargetObject;

		private readonly UnityObjectReferenceCache<IXRRayProvider, Object> m_AimTargetObjectRef = new UnityObjectReferenceCache<IXRRayProvider, Object>();

		private bool m_InitializedStabilizationOrigin;

		private Transform m_StabilizationAnchor;

		private float m_LastStabilizationUpdateTime;
	}
}
