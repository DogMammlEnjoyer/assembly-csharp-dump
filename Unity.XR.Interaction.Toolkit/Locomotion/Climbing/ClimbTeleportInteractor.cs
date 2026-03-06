using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
	[AddComponentMenu("XR/Locomotion/Climb Teleport Interactor", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing.ClimbTeleportInteractor.html")]
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public class ClimbTeleportInteractor : XRBaseInteractor, IXRActivateInteractor, IXRInteractor
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

		public TeleportVolumeDestinationSettingsDatumProperty destinationEvaluationSettings
		{
			get
			{
				return this.m_DestinationEvaluationSettings;
			}
			set
			{
				this.m_DestinationEvaluationSettings = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_ClimbProvider == null && !ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out this.m_ClimbProvider))
			{
				return;
			}
			this.m_ClimbProvider.locomotionStateChanged += this.OnLocomotionStateChanged;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.ReleaseTargetTeleportVolume();
			if (this.m_ClimbProvider == null)
			{
				return;
			}
			this.m_ClimbProvider.locomotionStateChanged -= this.OnLocomotionStateChanged;
			this.m_ClimbProvider.climbAnchorUpdated -= this.OnClimbAnchorUpdated;
		}

		private void OnLocomotionStateChanged(LocomotionProvider provider, LocomotionState state)
		{
			if (state == LocomotionState.Moving)
			{
				this.OnClimbBegin();
				return;
			}
			if (state == LocomotionState.Ended)
			{
				this.OnClimbEnd();
			}
		}

		private void OnClimbBegin()
		{
			this.SetTargetTeleportVolume(this.m_ClimbProvider.climbAnchorInteractable);
			this.m_ClimbProvider.climbAnchorUpdated += this.OnClimbAnchorUpdated;
		}

		private void OnClimbEnd()
		{
			this.m_ClimbProvider.climbAnchorUpdated -= this.OnClimbAnchorUpdated;
			if (this.m_TargetTeleportVolume == null)
			{
				return;
			}
			BaseTeleportationInteractable.TeleportTrigger teleportTrigger = this.m_TargetTeleportVolume.teleportTrigger;
			if (teleportTrigger > BaseTeleportationInteractable.TeleportTrigger.OnSelectEntered)
			{
				if (teleportTrigger - BaseTeleportationInteractable.TeleportTrigger.OnActivated <= 1)
				{
					ActivateEventArgs activateEventArgs;
					using (this.m_ActivateEventArgs.Get(out activateEventArgs))
					{
						activateEventArgs.interactorObject = this;
						activateEventArgs.interactableObject = this.m_TargetTeleportVolume;
						((IXRActivateInteractable)this.m_TargetTeleportVolume).OnActivated(activateEventArgs);
					}
					DeactivateEventArgs deactivateEventArgs;
					using (this.m_DeactivateEventArgs.Get(out deactivateEventArgs))
					{
						deactivateEventArgs.interactorObject = this;
						deactivateEventArgs.interactableObject = this.m_TargetTeleportVolume;
						((IXRActivateInteractable)this.m_TargetTeleportVolume).OnDeactivated(deactivateEventArgs);
					}
				}
			}
			else
			{
				this.StartManualInteraction(this.m_TargetTeleportVolume);
				this.EndManualInteraction();
			}
			this.ReleaseTargetTeleportVolume();
		}

		private void OnClimbAnchorUpdated(ClimbProvider provider)
		{
			this.SetTargetTeleportVolume(provider.climbAnchorInteractable);
		}

		private void SetTargetTeleportVolume(ClimbInteractable activeClimbInteractable)
		{
			TeleportationMultiAnchorVolume climbAssistanceTeleportVolume = activeClimbInteractable.climbAssistanceTeleportVolume;
			if (this.m_TargetTeleportVolume == climbAssistanceTeleportVolume)
			{
				return;
			}
			this.ReleaseTargetTeleportVolume();
			this.m_TargetTeleportVolume = climbAssistanceTeleportVolume;
			if (this.m_TargetTeleportVolume == null)
			{
				return;
			}
			this.m_PreservedTeleportVolumeSettings = this.m_TargetTeleportVolume.destinationEvaluationSettings;
			if (this.destinationEvaluationSettings.Value != null)
			{
				this.m_TargetTeleportVolume.destinationEvaluationSettings = this.destinationEvaluationSettings;
			}
		}

		private void ReleaseTargetTeleportVolume()
		{
			if (this.m_TargetTeleportVolume != null)
			{
				this.m_TargetTeleportVolume.destinationEvaluationSettings = this.m_PreservedTeleportVolumeSettings;
			}
			this.m_PreservedTeleportVolumeSettings = null;
			this.m_TargetTeleportVolume = null;
		}

		public override void GetValidTargets(List<IXRInteractable> targets)
		{
			targets.Clear();
			if (this.m_TargetTeleportVolume != null)
			{
				targets.Add(this.m_TargetTeleportVolume);
			}
		}

		public override bool isSelectActive
		{
			get
			{
				return base.isSelectActive && base.isPerformingManualInteraction;
			}
		}

		public bool shouldActivate
		{
			get
			{
				return false;
			}
		}

		public bool shouldDeactivate
		{
			get
			{
				return false;
			}
		}

		public void GetActivateTargets(List<IXRActivateInteractable> targets)
		{
			targets.Clear();
			if (this.m_TargetTeleportVolume != null)
			{
				targets.Add(this.m_TargetTeleportVolume);
			}
		}

		Transform IXRInteractor.get_transform()
		{
			return base.transform;
		}

		[SerializeField]
		[Tooltip("The climb locomotion provider to query for active locomotion and climbed interactable.")]
		private ClimbProvider m_ClimbProvider;

		[SerializeField]
		[Tooltip("Optional settings for how the hovered teleport volume evaluates a destination anchor. Applies as an override to the teleport volume's settings if set to Use Value or if the asset reference is set.")]
		private TeleportVolumeDestinationSettingsDatumProperty m_DestinationEvaluationSettings = new TeleportVolumeDestinationSettingsDatumProperty(new TeleportVolumeDestinationSettings
		{
			enableDestinationEvaluationDelay = false,
			pollForDestinationChange = true
		});

		private readonly LinkedPool<ActivateEventArgs> m_ActivateEventArgs = new LinkedPool<ActivateEventArgs>(() => new ActivateEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<DeactivateEventArgs> m_DeactivateEventArgs = new LinkedPool<DeactivateEventArgs>(() => new DeactivateEventArgs(), null, null, null, false, 10000);

		private TeleportationMultiAnchorVolume m_TargetTeleportVolume;

		private TeleportVolumeDestinationSettingsDatumProperty m_PreservedTeleportVolumeSettings;
	}
}
