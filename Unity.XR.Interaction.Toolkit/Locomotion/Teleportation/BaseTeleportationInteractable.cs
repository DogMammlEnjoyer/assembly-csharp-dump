using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.XR.CoreUtils;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public abstract class BaseTeleportationInteractable : XRBaseInteractable, IXRReticleDirectionProvider
	{
		public TeleportationProvider teleportationProvider
		{
			get
			{
				return this.m_TeleportationProvider;
			}
			set
			{
				this.m_TeleportationProvider = value;
			}
		}

		public MatchOrientation matchOrientation
		{
			get
			{
				return this.m_MatchOrientation;
			}
			set
			{
				this.m_MatchOrientation = value;
			}
		}

		public bool matchDirectionalInput
		{
			get
			{
				return this.m_MatchDirectionalInput;
			}
			set
			{
				this.m_MatchDirectionalInput = value;
			}
		}

		public BaseTeleportationInteractable.TeleportTrigger teleportTrigger
		{
			get
			{
				return this.m_TeleportTrigger;
			}
			set
			{
				this.m_TeleportTrigger = value;
			}
		}

		public bool filterSelectionByHitNormal
		{
			get
			{
				return this.m_FilterSelectionByHitNormal;
			}
			set
			{
				this.m_FilterSelectionByHitNormal = value;
			}
		}

		public float upNormalToleranceDegrees
		{
			get
			{
				return this.m_UpNormalToleranceDegrees;
			}
			set
			{
				this.m_UpNormalToleranceDegrees = value;
			}
		}

		public TeleportingEvent teleporting
		{
			get
			{
				return this.m_Teleporting;
			}
			set
			{
				this.m_Teleporting = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			if (this.m_TeleportationProvider == null)
			{
				ComponentLocatorUtility<TeleportationProvider>.TryFindComponent(out this.m_TeleportationProvider, true);
			}
		}

		protected override void Reset()
		{
			base.selectMode = InteractableSelectMode.Multiple;
		}

		protected virtual bool GenerateTeleportRequest(IXRInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
		{
			return false;
		}

		protected bool SendTeleportRequest(IXRInteractor interactor)
		{
			RaycastHit raycastHit = default(RaycastHit);
			XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
			IXRInteractable ixrinteractable;
			if (xrrayInteractor != null && xrrayInteractor != null && (!xrrayInteractor.TryGetCurrent3DRaycastHit(out raycastHit) || !base.interactionManager.TryGetInteractableForCollider(raycastHit.collider, out ixrinteractable) || ixrinteractable != this || (this.m_FilterSelectionByHitNormal && Vector3.Angle(base.transform.up, raycastHit.normal) > this.m_UpNormalToleranceDegrees)))
			{
				return false;
			}
			if (this.m_TeleportationProvider == null && !ComponentLocatorUtility<TeleportationProvider>.TryFindComponent(out this.m_TeleportationProvider))
			{
				Debug.LogWarning("Teleportation Provider was null and one could not be found in the scene: Teleport request failed.", this);
				return false;
			}
			TeleportRequest teleportRequest = new TeleportRequest
			{
				matchOrientation = this.m_MatchOrientation,
				requestTime = Time.time
			};
			bool flag = this.GenerateTeleportRequest(interactor, raycastHit, ref teleportRequest);
			if (flag)
			{
				this.UpdateTeleportRequestRotation(interactor, ref teleportRequest);
				flag = this.m_TeleportationProvider.QueueTeleportRequest(teleportRequest);
				if (flag && this.m_Teleporting != null)
				{
					TeleportingEventArgs teleportingEventArgs;
					using (this.m_TeleportingEventArgs.Get(out teleportingEventArgs))
					{
						teleportingEventArgs.interactorObject = interactor;
						teleportingEventArgs.interactableObject = this;
						teleportingEventArgs.teleportRequest = teleportRequest;
						this.m_Teleporting.Invoke(teleportingEventArgs);
					}
				}
			}
			return flag;
		}

		private void UpdateTeleportRequestRotation(IXRInteractor interactor, ref TeleportRequest teleportRequest)
		{
			Vector3 forward;
			if (!this.m_MatchDirectionalInput || interactor == null || !this.m_TeleportForwardPerInteractor.TryGetValue(interactor, out forward))
			{
				return;
			}
			MatchOrientation matchOrientation = teleportRequest.matchOrientation;
			if (matchOrientation == MatchOrientation.WorldSpaceUp)
			{
				teleportRequest.destinationRotation = Quaternion.LookRotation(forward, Vector3.up);
				teleportRequest.matchOrientation = MatchOrientation.TargetUpAndForward;
				return;
			}
			if (matchOrientation != MatchOrientation.TargetUp)
			{
				return;
			}
			teleportRequest.destinationRotation = Quaternion.LookRotation(forward, base.transform.up);
			teleportRequest.matchOrientation = MatchOrientation.TargetUpAndForward;
		}

		public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractable(updatePhase);
			if (updatePhase != XRInteractionUpdateOrder.UpdatePhase.Dynamic || !this.m_MatchDirectionalInput)
			{
				return;
			}
			int i = 0;
			int count = base.interactorsHovering.Count;
			while (i < count)
			{
				IXRHoverInteractor interactor = base.interactorsHovering[i];
				this.<ProcessInteractable>g__CalculateTeleportForward|37_0(interactor);
				i++;
			}
			int j = 0;
			int count2 = base.interactorsSelecting.Count;
			while (j < count2)
			{
				IXRSelectInteractor interactor2 = base.interactorsSelecting[j];
				if (!base.IsHovered(interactor2))
				{
					this.<ProcessInteractable>g__CalculateTeleportForward|37_0(interactor2);
				}
				j++;
			}
		}

		protected override void OnSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_TeleportTrigger == BaseTeleportationInteractable.TeleportTrigger.OnSelectEntered)
			{
				this.SendTeleportRequest(args.interactorObject);
			}
			base.OnSelectEntered(args);
		}

		protected override void OnSelectExited(SelectExitEventArgs args)
		{
			if (this.m_TeleportTrigger == BaseTeleportationInteractable.TeleportTrigger.OnSelectExited && !args.isCanceled)
			{
				this.SendTeleportRequest(args.interactorObject);
			}
			base.OnSelectExited(args);
		}

		protected override void OnActivated(ActivateEventArgs args)
		{
			if (this.m_TeleportTrigger == BaseTeleportationInteractable.TeleportTrigger.OnActivated)
			{
				this.SendTeleportRequest(args.interactorObject);
			}
			base.OnActivated(args);
		}

		protected override void OnDeactivated(DeactivateEventArgs args)
		{
			if (this.m_TeleportTrigger == BaseTeleportationInteractable.TeleportTrigger.OnDeactivated)
			{
				this.SendTeleportRequest(args.interactorObject);
			}
			base.OnDeactivated(args);
		}

		public override bool IsSelectableBy(IXRSelectInteractor interactor)
		{
			bool flag = base.IsSelectableBy(interactor);
			if (flag && this.m_FilterSelectionByHitNormal)
			{
				XRRayInteractor xrrayInteractor = interactor as XRRayInteractor;
				RaycastHit raycastHit;
				IXRInteractable ixrinteractable;
				if (xrrayInteractor != null && xrrayInteractor != null && xrrayInteractor.TryGetCurrent3DRaycastHit(out raycastHit) && base.interactionManager.TryGetInteractableForCollider(raycastHit.collider, out ixrinteractable) && ixrinteractable == this)
				{
					flag &= (Vector3.Angle(base.transform.up, raycastHit.normal) <= this.m_UpNormalToleranceDegrees);
				}
			}
			return flag;
		}

		public void GetReticleDirection(IXRInteractor interactor, Vector3 hitNormal, out Vector3 reticleUp, out Vector3? optionalReticleForward)
		{
			optionalReticleForward = null;
			reticleUp = hitNormal;
			XROrigin xrOrigin = this.teleportationProvider.mediator.xrOrigin;
			switch (this.matchOrientation)
			{
			case MatchOrientation.WorldSpaceUp:
			{
				reticleUp = Vector3.up;
				Vector3 value;
				if (this.m_MatchDirectionalInput && interactor != null && this.m_TeleportForwardPerInteractor.TryGetValue(interactor, out value))
				{
					optionalReticleForward = new Vector3?(value);
					return;
				}
				if (xrOrigin != null)
				{
					optionalReticleForward = new Vector3?(xrOrigin.Camera.transform.forward);
					return;
				}
				break;
			}
			case MatchOrientation.TargetUp:
			{
				reticleUp = base.transform.up;
				Vector3 value;
				if (this.m_MatchDirectionalInput && interactor != null && this.m_TeleportForwardPerInteractor.TryGetValue(interactor, out value))
				{
					optionalReticleForward = new Vector3?(value);
					return;
				}
				if (xrOrigin != null)
				{
					optionalReticleForward = new Vector3?(xrOrigin.Camera.transform.forward);
					return;
				}
				break;
			}
			case MatchOrientation.TargetUpAndForward:
				reticleUp = base.transform.up;
				optionalReticleForward = new Vector3?(base.transform.forward);
				return;
			case MatchOrientation.None:
				if (xrOrigin != null)
				{
					reticleUp = xrOrigin.Origin.transform.up;
					optionalReticleForward = new Vector3?(xrOrigin.Camera.transform.forward);
				}
				break;
			default:
				return;
			}
		}

		[Obsolete("GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest) has been deprecated. Use GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest) instead.", true)]
		protected virtual bool GenerateTeleportRequest(XRBaseInteractor interactor, RaycastHit raycastHit, ref TeleportRequest teleportRequest)
		{
			Debug.LogError("GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest) has been deprecated. Use GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest) instead.", this);
			throw new NotSupportedException("GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest) has been deprecated. Use GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest) instead.");
		}

		[CompilerGenerated]
		private void <ProcessInteractable>g__CalculateTeleportForward|37_0(IXRInteractor interactor)
		{
			Transform attachTransform = interactor.GetAttachTransform(this);
			MatchOrientation matchOrientation = this.matchOrientation;
			if (matchOrientation == MatchOrientation.WorldSpaceUp)
			{
				this.m_TeleportForwardPerInteractor[interactor] = Vector3.ProjectOnPlane(attachTransform.forward, Vector3.up).normalized;
				return;
			}
			if (matchOrientation != MatchOrientation.TargetUp)
			{
				return;
			}
			this.m_TeleportForwardPerInteractor[interactor] = Vector3.ProjectOnPlane(attachTransform.forward, base.transform.up).normalized;
		}

		private const float k_DefaultNormalToleranceDegrees = 30f;

		[SerializeField]
		[Tooltip("The teleportation provider that this teleportation interactable will communicate teleport requests to. If no teleportation provider is configured, will attempt to find a teleportation provider.")]
		private TeleportationProvider m_TeleportationProvider;

		[SerializeField]
		[Tooltip("How to orient the rig after teleportation.\nSet to:\n\nWorld Space Up to stay oriented according to the world space up vector.\n\nSet to Target Up to orient according to the target BaseTeleportationInteractable Transform's up vector.\n\nSet to Target Up And Forward to orient according to the target BaseTeleportationInteractable Transform's rotation.\n\nSet to None to maintain the same orientation before and after teleporting.")]
		private MatchOrientation m_MatchOrientation;

		[SerializeField]
		[Tooltip("Whether or not to rotate the rig to match the forward direction of the attach transform of the selecting interactor.")]
		private bool m_MatchDirectionalInput;

		[SerializeField]
		[Tooltip("Specify when the teleportation will be triggered. Options map to when the trigger is pressed or when it is released.")]
		private BaseTeleportationInteractable.TeleportTrigger m_TeleportTrigger;

		[SerializeField]
		[Tooltip("When enabled, this teleportation interactable will only be selectable by a ray interactor if its current hit normal is aligned with this object's up vector.")]
		private bool m_FilterSelectionByHitNormal;

		[SerializeField]
		[Tooltip("Sets the tolerance in degrees from this object's up vector for a hit normal to be considered aligned with the up vector.")]
		private float m_UpNormalToleranceDegrees = 30f;

		[SerializeField]
		private TeleportingEvent m_Teleporting = new TeleportingEvent();

		private readonly LinkedPool<TeleportingEventArgs> m_TeleportingEventArgs = new LinkedPool<TeleportingEventArgs>(() => new TeleportingEventArgs(), null, null, null, false, 10000);

		private readonly Dictionary<IXRInteractor, Vector3> m_TeleportForwardPerInteractor = new Dictionary<IXRInteractor, Vector3>();

		private const string k_GenerateTeleportRequestDeprecated = "GenerateTeleportRequest(XRBaseInteractor, RaycastHit, ref TeleportRequest) has been deprecated. Use GenerateTeleportRequest(IXRInteractor, RaycastHit, ref TeleportRequest) instead.";

		public enum TeleportTrigger
		{
			OnSelectExited,
			OnSelectEntered,
			OnActivated,
			OnDeactivated,
			[Obsolete("OnSelectExit has been deprecated. Use OnSelectExited instead. (UnityUpgradable) -> OnSelectExited", true)]
			OnSelectExit = 0,
			[Obsolete("OnSelectEnter has been deprecated. Use OnSelectEntered instead. (UnityUpgradable) -> OnSelectEntered", true)]
			OnSelectEnter,
			[Obsolete("OnActivate has been deprecated. Use OnActivated instead. (UnityUpgradable) -> OnActivated", true)]
			OnActivate,
			[Obsolete("OnDeactivate has been deprecated. Use OnDeactivated instead. (UnityUpgradable) -> OnDeactivated", true)]
			OnDeactivate
		}
	}
}
