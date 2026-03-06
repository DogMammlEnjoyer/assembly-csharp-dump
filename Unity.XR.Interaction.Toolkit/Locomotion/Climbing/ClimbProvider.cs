using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Gravity;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
	[AddComponentMenu("XR/Locomotion/Climb Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbProvider.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class ClimbProvider : LocomotionProvider, IGravityController
	{
		public List<LocomotionProvider> providersToDisable
		{
			get
			{
				return this.m_ProvidersToDisable;
			}
			set
			{
				this.m_ProvidersToDisable = value;
			}
		}

		public bool enableGravityOnClimbEnd
		{
			get
			{
				return this.m_EnableGravityOnClimbEnd;
			}
			set
			{
				this.m_EnableGravityOnClimbEnd = value;
			}
		}

		public ClimbSettingsDatumProperty climbSettings
		{
			get
			{
				return this.m_ClimbSettings;
			}
			set
			{
				this.m_ClimbSettings = value;
			}
		}

		public ClimbInteractable climbAnchorInteractable
		{
			get
			{
				if (this.m_GrabbedClimbables.Count > 0)
				{
					return this.m_GrabbedClimbables[this.m_GrabbedClimbables.Count - 1];
				}
				return null;
			}
		}

		public IXRSelectInteractor climbAnchorInteractor
		{
			get
			{
				if (this.m_GrabbingInteractors.Count > 0)
				{
					return this.m_GrabbingInteractors[this.m_GrabbingInteractors.Count - 1];
				}
				return null;
			}
		}

		public XROriginMovement transformation { get; set; } = new XROriginMovement();

		public bool canProcess
		{
			get
			{
				return base.isActiveAndEnabled;
			}
		}

		public bool gravityPaused { get; protected set; }

		public event Action<ClimbProvider> climbAnchorUpdated;

		protected override void Awake()
		{
			base.Awake();
			if (this.m_ClimbSettings == null || this.m_ClimbSettings.Value == null)
			{
				this.m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());
			}
			ComponentLocatorUtility<GravityProvider>.TryFindComponent(out this.m_GravityProvider);
		}

		protected virtual void Update()
		{
			if (!base.isLocomotionActive)
			{
				return;
			}
			if (this.m_GrabbingInteractors.Count <= 0)
			{
				this.FinishLocomotion();
				return;
			}
			if (base.locomotionState == LocomotionState.Preparing)
			{
				base.TryStartLocomotionImmediately();
			}
			int index = this.m_GrabbingInteractors.Count - 1;
			IXRSelectInteractor ixrselectInteractor = this.m_GrabbingInteractors[index];
			ClimbInteractable climbInteractable = this.m_GrabbedClimbables[index];
			if (ixrselectInteractor == null || climbInteractable == null)
			{
				this.FinishLocomotion();
				return;
			}
			this.StepClimbMovement(climbInteractable, ixrselectInteractor);
		}

		public void StartClimbGrab(ClimbInteractable climbInteractable, IXRSelectInteractor interactor)
		{
			XROrigin xrOrigin = base.mediator.xrOrigin;
			if (((xrOrigin != null) ? xrOrigin.Origin : null) == null)
			{
				return;
			}
			bool flag = base.locomotionState == LocomotionState.Moving || base.locomotionState == LocomotionState.Preparing;
			this.m_GrabbingInteractors.Add(interactor);
			this.m_GrabbedClimbables.Add(climbInteractable);
			this.UpdateClimbAnchor(climbInteractable, interactor);
			base.TryPrepareLocomotion();
			if (!flag)
			{
				this.TryLockGravity(GravityOverride.ForcedOff);
			}
			foreach (LocomotionProvider locomotionProvider in this.m_ProvidersToDisable)
			{
				if (!(locomotionProvider == null) && locomotionProvider.enabled)
				{
					locomotionProvider.enabled = false;
					this.m_EnabledProvidersToDisable.Add(locomotionProvider);
				}
			}
		}

		public void FinishClimbGrab(IXRSelectInteractor interactor)
		{
			int num = this.m_GrabbingInteractors.IndexOf(interactor);
			if (num < 0)
			{
				return;
			}
			if (num > 0 && num == this.m_GrabbingInteractors.Count - 1)
			{
				int index = num - 1;
				this.UpdateClimbAnchor(this.m_GrabbedClimbables[index], this.m_GrabbingInteractors[index]);
			}
			this.m_GrabbingInteractors.RemoveAt(num);
			this.m_GrabbedClimbables.RemoveAt(num);
		}

		private void UpdateClimbAnchor(ClimbInteractable climbInteractable, IXRInteractor interactor)
		{
			Transform climbTransform = climbInteractable.climbTransform;
			this.m_InteractorAnchorWorldPosition = interactor.transform.position;
			this.m_InteractorAnchorClimbSpacePosition = climbTransform.InverseTransformPoint(this.m_InteractorAnchorWorldPosition);
			Action<ClimbProvider> action = this.climbAnchorUpdated;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		private void StepClimbMovement(ClimbInteractable currentClimbInteractable, IXRSelectInteractor currentInteractor)
		{
			ClimbSettings activeClimbSettings = this.GetActiveClimbSettings(currentClimbInteractable);
			bool allowFreeXMovement = activeClimbSettings.allowFreeXMovement;
			bool allowFreeYMovement = activeClimbSettings.allowFreeYMovement;
			bool allowFreeZMovement = activeClimbSettings.allowFreeZMovement;
			Vector3 position = currentInteractor.transform.position;
			Vector3 motion;
			if (allowFreeXMovement && allowFreeYMovement && allowFreeZMovement)
			{
				motion = this.m_InteractorAnchorWorldPosition - position;
			}
			else
			{
				Transform climbTransform = currentClimbInteractable.climbTransform;
				Vector3 b = climbTransform.InverseTransformPoint(position);
				Vector3 vector = this.m_InteractorAnchorClimbSpacePosition - b;
				if (!allowFreeXMovement)
				{
					vector.x = 0f;
				}
				if (!allowFreeYMovement)
				{
					vector.y = 0f;
				}
				if (!allowFreeZMovement)
				{
					vector.z = 0f;
				}
				motion = climbTransform.TransformVector(vector);
			}
			this.transformation.motion = motion;
			base.TryQueueTransformation(this.transformation);
		}

		private void FinishLocomotion()
		{
			base.TryEndLocomotion();
			this.m_GrabbingInteractors.Clear();
			this.m_GrabbedClimbables.Clear();
			this.RemoveGravityLock();
			this.gravityPaused = !this.m_EnableGravityOnClimbEnd;
			foreach (LocomotionProvider locomotionProvider in this.m_EnabledProvidersToDisable)
			{
				if (!(locomotionProvider == null))
				{
					locomotionProvider.enabled = true;
				}
			}
			this.m_EnabledProvidersToDisable.Clear();
		}

		private ClimbSettings GetActiveClimbSettings(ClimbInteractable climbInteractable)
		{
			if (climbInteractable.climbSettingsOverride.Value != null)
			{
				return climbInteractable.climbSettingsOverride;
			}
			return this.m_ClimbSettings;
		}

		public bool TryLockGravity(GravityOverride gravityOverride)
		{
			return this.m_GravityProvider != null && this.m_GravityProvider.TryLockGravity(this, gravityOverride);
		}

		public void RemoveGravityLock()
		{
			if (this.m_GravityProvider != null)
			{
				this.m_GravityProvider.UnlockGravity(this);
			}
		}

		void IGravityController.OnGroundedChanged(bool isGrounded)
		{
			this.OnGroundedChanged(isGrounded);
		}

		void IGravityController.OnGravityLockChanged(GravityOverride gravityOverride)
		{
			this.OnGravityLockChanged(gravityOverride);
		}

		protected virtual void OnGroundedChanged(bool isGrounded)
		{
			this.gravityPaused = false;
		}

		protected virtual void OnGravityLockChanged(GravityOverride gravityOverride)
		{
			if (gravityOverride == GravityOverride.ForcedOn)
			{
				this.gravityPaused = false;
			}
		}

		[SerializeField]
		[Tooltip("List of providers to disable while climb locomotion is active. If empty, no providers will be disabled by this component while climbing.")]
		private List<LocomotionProvider> m_ProvidersToDisable = new List<LocomotionProvider>();

		[SerializeField]
		[Tooltip("Whether to allow falling when climb locomotion ends. Disable to pause gravity when releasing, keeping the user from falling.")]
		private bool m_EnableGravityOnClimbEnd = true;

		[SerializeField]
		[Tooltip("Climb locomotion settings. Can be overridden by the Climb Interactable used for locomotion.")]
		private ClimbSettingsDatumProperty m_ClimbSettings = new ClimbSettingsDatumProperty(new ClimbSettings());

		private GravityProvider m_GravityProvider;

		private readonly List<IXRSelectInteractor> m_GrabbingInteractors = new List<IXRSelectInteractor>();

		private readonly List<ClimbInteractable> m_GrabbedClimbables = new List<ClimbInteractable>();

		private Vector3 m_InteractorAnchorWorldPosition;

		private Vector3 m_InteractorAnchorClimbSpacePosition;

		private List<LocomotionProvider> m_EnabledProvidersToDisable = new List<LocomotionProvider>();
	}
}
