using System;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
	[SelectionBase]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("XR/Climb Interactable", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbInteractable.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class ClimbInteractable : XRBaseInteractable
	{
		public ClimbProvider climbProvider
		{
			get
			{
				return this.m_ClimbProvider;
			}
			set
			{
				this.m_ClimbProvider = value;
			}
		}

		public Transform climbTransform
		{
			get
			{
				if (this.m_ClimbTransform == null)
				{
					this.m_ClimbTransform = base.transform;
				}
				return this.m_ClimbTransform;
			}
			set
			{
				this.m_ClimbTransform = value;
			}
		}

		public bool filterInteractionByDistance
		{
			get
			{
				return this.m_FilterInteractionByDistance;
			}
			set
			{
				this.m_FilterInteractionByDistance = value;
			}
		}

		public float maxInteractionDistance
		{
			get
			{
				return this.m_MaxInteractionDistance;
			}
			set
			{
				this.m_MaxInteractionDistance = value;
			}
		}

		public TeleportationMultiAnchorVolume climbAssistanceTeleportVolume
		{
			get
			{
				return this.m_ClimbAssistanceTeleportVolume;
			}
			set
			{
				this.m_ClimbAssistanceTeleportVolume = value;
			}
		}

		public ClimbSettingsDatumProperty climbSettingsOverride
		{
			get
			{
				return this.m_ClimbSettingsOverride;
			}
			set
			{
				this.m_ClimbSettingsOverride = value;
			}
		}

		protected virtual void OnValidate()
		{
			if (this.m_ClimbTransform == null)
			{
				this.m_ClimbTransform = base.transform;
			}
		}

		protected override void Reset()
		{
			base.selectMode = InteractableSelectMode.Multiple;
			this.m_ClimbTransform = base.transform;
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.m_ClimbProvider == null)
			{
				ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out this.m_ClimbProvider, true);
			}
		}

		public override bool IsHoverableBy(IXRHoverInteractor interactor)
		{
			return base.IsHoverableBy(interactor) && (!this.m_FilterInteractionByDistance || this.GetDistanceSqrToInteractor(interactor) <= this.m_MaxInteractionDistance * this.m_MaxInteractionDistance);
		}

		public override bool IsSelectableBy(IXRSelectInteractor interactor)
		{
			return base.IsSelectableBy(interactor) && (base.IsSelected(interactor) || !this.m_FilterInteractionByDistance || this.GetDistanceSqrToInteractor(interactor) <= this.m_MaxInteractionDistance * this.m_MaxInteractionDistance);
		}

		protected override void OnSelectEntered(SelectEnterEventArgs args)
		{
			base.OnSelectEntered(args);
			if (this.m_ClimbProvider != null || ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out this.m_ClimbProvider))
			{
				this.m_ClimbProvider.StartClimbGrab(this, args.interactorObject);
			}
		}

		protected override void OnSelectExited(SelectExitEventArgs args)
		{
			base.OnSelectExited(args);
			if (this.m_ClimbProvider != null)
			{
				this.m_ClimbProvider.FinishClimbGrab(args.interactorObject);
			}
		}

		private const float k_DefaultMaxInteractionDistance = 0.1f;

		[SerializeField]
		[Tooltip("The climb provider that performs locomotion while this interactable is selected. If no climb provider is configured, will attempt to find one.")]
		private ClimbProvider m_ClimbProvider;

		[SerializeField]
		[Tooltip("Transform that defines the coordinate space for climb locomotion. Will use this GameObject's Transform by default.")]
		private Transform m_ClimbTransform;

		[SerializeField]
		[Tooltip("Controls whether to apply a distance check when validating hover and select interaction.")]
		private bool m_FilterInteractionByDistance = true;

		[SerializeField]
		[Tooltip("The maximum distance that an interactor can be from this interactable to begin hover or select.")]
		private float m_MaxInteractionDistance = 0.1f;

		[SerializeField]
		[Tooltip("The teleport volume used to assist with movement to a specific destination after ending a climb (optional, may be None). Only used if there is a Climb Teleport Interactor in the scene.")]
		private TeleportationMultiAnchorVolume m_ClimbAssistanceTeleportVolume;

		[SerializeField]
		[Tooltip("Optional override of locomotion settings specified in the climb provider. Only applies as an override if set to Use Value or if the asset reference is set.")]
		private ClimbSettingsDatumProperty m_ClimbSettingsOverride;
	}
}
