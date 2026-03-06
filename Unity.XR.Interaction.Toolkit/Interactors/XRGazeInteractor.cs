using System;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[AddComponentMenu("XR/Interactors/XR Gaze Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRGazeInteractor.html")]
	public class XRGazeInteractor : XRRayInteractor
	{
		public XRGazeInteractor.GazeAssistanceCalculation gazeAssistanceCalculation
		{
			get
			{
				return this.m_GazeAssistanceCalculation;
			}
			set
			{
				this.m_GazeAssistanceCalculation = value;
			}
		}

		public float gazeAssistanceColliderFixedSize
		{
			get
			{
				return this.m_GazeAssistanceColliderFixedSize;
			}
			set
			{
				this.m_GazeAssistanceColliderFixedSize = value;
			}
		}

		public float gazeAssistanceColliderScale
		{
			get
			{
				return this.m_GazeAssistanceColliderScale;
			}
			set
			{
				this.m_GazeAssistanceColliderScale = value;
			}
		}

		public XRInteractableSnapVolume gazeAssistanceSnapVolume
		{
			get
			{
				return this.m_GazeAssistanceSnapVolume;
			}
			set
			{
				this.m_GazeAssistanceSnapVolume = value;
			}
		}

		public bool gazeAssistanceDistanceScaling
		{
			get
			{
				return this.m_GazeAssistanceDistanceScaling;
			}
			set
			{
				this.m_GazeAssistanceDistanceScaling = value;
			}
		}

		public bool clampGazeAssistanceDistanceScaling
		{
			get
			{
				return this.m_ClampGazeAssistanceDistanceScaling;
			}
			set
			{
				this.m_ClampGazeAssistanceDistanceScaling = value;
			}
		}

		public float gazeAssistanceDistanceScalingClampValue
		{
			get
			{
				return this.m_GazeAssistanceDistanceScalingClampValue;
			}
			set
			{
				this.m_GazeAssistanceDistanceScalingClampValue = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.CreateGazeAssistanceSnapVolume();
		}

		private void CreateGazeAssistanceSnapVolume()
		{
			if (this.m_GazeAssistanceSnapVolume == null)
			{
				GameObject gameObject = new GameObject("Gaze Snap Volume");
				gameObject.AddComponent<SphereCollider>().isTrigger = true;
				this.m_GazeAssistanceSnapVolume = gameObject.AddComponent<XRInteractableSnapVolume>();
				return;
			}
			if (this.m_GazeAssistanceSnapVolume.snapCollider != null && !(this.m_GazeAssistanceSnapVolume.snapCollider is SphereCollider) && !(this.m_GazeAssistanceSnapVolume.snapCollider is BoxCollider))
			{
				Debug.LogWarning("The Gaze Assistance Snap Volume is using a Snap Collider which does not support automatic dynamic scaling by the XR Gaze Interactor. It must be a Sphere Collider or Box Collider.", this);
			}
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				IXRInteractable interactable = this.CanInteract(base.currentNearestValidTarget) ? base.currentNearestValidTarget : null;
				this.UpdateSnapVolumeInteractable(interactable);
			}
		}

		protected virtual void UpdateSnapVolumeInteractable(IXRInteractable interactable)
		{
			if (this.m_GazeAssistanceSnapVolume == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			float num = this.m_GazeAssistanceColliderScale;
			float num2 = 0f;
			IXRInteractable interactable2 = null;
			Collider collider = null;
			XRBaseInteractable xrbaseInteractable = interactable as XRBaseInteractable;
			if (xrbaseInteractable != null && xrbaseInteractable != null && xrbaseInteractable.allowGazeAssistance)
			{
				interactable2 = interactable;
				vector = interactable.transform.position;
				Vector3 position;
				Vector3 vector2;
				int num3;
				bool flag;
				DistanceInfo distanceInfo;
				if (base.TryGetHitInfo(out position, out vector2, out num3, out flag) && XRInteractableUtility.TryGetClosestCollider(interactable, position, out distanceInfo))
				{
					collider = distanceInfo.collider;
					vector = distanceInfo.collider.bounds.center;
				}
				num2 = this.CalculateSnapColliderSize(collider);
			}
			if (this.m_GazeAssistanceDistanceScaling)
			{
				num *= Vector3.Distance(base.transform.position, vector);
				if (this.m_ClampGazeAssistanceDistanceScaling)
				{
					num = Mathf.Clamp(num, 0f, this.m_GazeAssistanceDistanceScalingClampValue);
				}
			}
			Transform transform = this.m_GazeAssistanceSnapVolume.transform;
			transform.position = vector;
			transform.localScale = new Vector3(num, num, num);
			Collider snapCollider = this.m_GazeAssistanceSnapVolume.snapCollider;
			SphereCollider sphereCollider = snapCollider as SphereCollider;
			if (sphereCollider == null)
			{
				BoxCollider boxCollider = snapCollider as BoxCollider;
				if (boxCollider != null)
				{
					boxCollider.size = new Vector3(num2, num2, num2);
				}
			}
			else
			{
				sphereCollider.radius = num2;
			}
			this.m_GazeAssistanceSnapVolume.interactable = interactable2;
			this.m_GazeAssistanceSnapVolume.snapToCollider = collider;
		}

		private float CalculateSnapColliderSize(Collider interactableCollider)
		{
			XRGazeInteractor.GazeAssistanceCalculation gazeAssistanceCalculation = this.m_GazeAssistanceCalculation;
			if (gazeAssistanceCalculation != XRGazeInteractor.GazeAssistanceCalculation.FixedSize)
			{
				if (gazeAssistanceCalculation == XRGazeInteractor.GazeAssistanceCalculation.ColliderSize)
				{
					if (interactableCollider != null)
					{
						return interactableCollider.bounds.size.MaxComponent();
					}
				}
				return 0f;
			}
			return this.m_GazeAssistanceColliderFixedSize;
		}

		private bool CanInteract(IXRInteractable interactable)
		{
			IXRHoverInteractable ixrhoverInteractable = interactable as IXRHoverInteractable;
			if (ixrhoverInteractable == null || !base.interactionManager.CanHover(this, ixrhoverInteractable))
			{
				IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
				return ixrselectInteractable != null && base.interactionManager.CanSelect(this, ixrselectInteractable);
			}
			return true;
		}

		protected override float GetHoverTimeToSelect(IXRInteractable interactable)
		{
			IXROverridesGazeAutoSelect ixroverridesGazeAutoSelect = interactable as IXROverridesGazeAutoSelect;
			if (ixroverridesGazeAutoSelect != null && ixroverridesGazeAutoSelect.overrideGazeTimeToSelect)
			{
				return ixroverridesGazeAutoSelect.gazeTimeToSelect;
			}
			return base.GetHoverTimeToSelect(interactable);
		}

		protected override float GetTimeToAutoDeselect(IXRInteractable interactable)
		{
			IXROverridesGazeAutoSelect ixroverridesGazeAutoSelect = interactable as IXROverridesGazeAutoSelect;
			if (ixroverridesGazeAutoSelect != null && ixroverridesGazeAutoSelect.overrideTimeToAutoDeselectGaze)
			{
				return ixroverridesGazeAutoSelect.timeToAutoDeselectGaze;
			}
			return base.GetTimeToAutoDeselect(interactable);
		}

		[SerializeField]
		private XRGazeInteractor.GazeAssistanceCalculation m_GazeAssistanceCalculation;

		[SerializeField]
		private float m_GazeAssistanceColliderFixedSize = 1f;

		[SerializeField]
		private float m_GazeAssistanceColliderScale = 1f;

		[SerializeField]
		private XRInteractableSnapVolume m_GazeAssistanceSnapVolume;

		[SerializeField]
		private bool m_GazeAssistanceDistanceScaling;

		[SerializeField]
		private bool m_ClampGazeAssistanceDistanceScaling;

		[SerializeField]
		private float m_GazeAssistanceDistanceScalingClampValue = 1f;

		public enum GazeAssistanceCalculation
		{
			FixedSize,
			ColliderSize
		}
	}
}
