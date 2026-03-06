using System;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[Obsolete("XRBaseControllerInteractor has been deprecated in version 3.0.0. It has been renamed to XRBaseInputInteractor. (UnityUpgradable) -> UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor")]
	public abstract class XRBaseControllerInteractor : XRBaseInteractor, IXRActivateInteractor, IXRInteractor
	{
		[Obsolete("playAudioClipOnSelectEnter has been deprecated. Use playAudioClipOnSelectEntered instead. (UnityUpgradable) -> playAudioClipOnSelectEntered", true)]
		public bool playAudioClipOnSelectEnter
		{
			get
			{
				return false;
			}
		}

		[Obsolete("audioClipForOnSelectEnter has been deprecated. Use audioClipForOnSelectEntered instead. (UnityUpgradable) -> audioClipForOnSelectEntered", true)]
		public AudioClip audioClipForOnSelectEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("AudioClipForOnSelectEnter has been deprecated. Use audioClipForOnSelectEntered instead. (UnityUpgradable) -> audioClipForOnSelectEntered", true)]
		public AudioClip AudioClipForOnSelectEnter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("playAudioClipOnSelectExit has been deprecated. Use playAudioClipOnSelectExited instead. (UnityUpgradable) -> playAudioClipOnSelectExited", true)]
		public bool playAudioClipOnSelectExit
		{
			get
			{
				return false;
			}
		}

		[Obsolete("audioClipForOnSelectExit has been deprecated. Use audioClipForOnSelectExited instead. (UnityUpgradable) -> audioClipForOnSelectExited", true)]
		public AudioClip audioClipForOnSelectExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("AudioClipForOnSelectExit has been deprecated. Use audioClipForOnSelectExited instead. (UnityUpgradable) -> audioClipForOnSelectExited", true)]
		public AudioClip AudioClipForOnSelectExit
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("playAudioClipOnHoverEnter has been deprecated. Use playAudioClipOnHoverEntered instead. (UnityUpgradable) -> playAudioClipOnHoverEntered", true)]
		public bool playAudioClipOnHoverEnter
		{
			get
			{
				return false;
			}
		}

		[Obsolete("audioClipForOnHoverEnter has been deprecated. Use audioClipForOnHoverEntered instead. (UnityUpgradable) -> audioClipForOnHoverEntered", true)]
		public AudioClip audioClipForOnHoverEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("AudioClipForOnHoverEnter has been deprecated. Use audioClipForOnHoverEntered instead. (UnityUpgradable) -> audioClipForOnHoverEntered", true)]
		public AudioClip AudioClipForOnHoverEnter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("playAudioClipOnHoverExit has been deprecated. Use playAudioClipOnHoverExited instead. (UnityUpgradable) -> playAudioClipOnHoverExited", true)]
		public bool playAudioClipOnHoverExit
		{
			get
			{
				return false;
			}
		}

		[Obsolete("audioClipForOnHoverExit has been deprecated. Use audioClipForOnHoverExited instead. (UnityUpgradable) -> audioClipForOnHoverExited", true)]
		public AudioClip audioClipForOnHoverExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("AudioClipForOnHoverExit has been deprecated. Use audioClipForOnHoverExited instead. (UnityUpgradable) -> audioClipForOnHoverExited", true)]
		public AudioClip AudioClipForOnHoverExit
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("playHapticsOnSelectEnter has been deprecated. Use playHapticsOnSelectEntered instead. (UnityUpgradable) -> playHapticsOnSelectEntered", true)]
		public bool playHapticsOnSelectEnter
		{
			get
			{
				return false;
			}
		}

		[Obsolete("playHapticsOnSelectExit has been deprecated. Use playHapticsOnSelectExited instead. (UnityUpgradable) -> playHapticsOnSelectExited", true)]
		public bool playHapticsOnSelectExit
		{
			get
			{
				return false;
			}
		}

		[Obsolete("playHapticsOnHoverEnter has been deprecated. Use playHapticsOnHoverEntered instead. (UnityUpgradable) -> playHapticsOnHoverEntered", true)]
		public bool playHapticsOnHoverEnter
		{
			get
			{
				return false;
			}
		}

		[Obsolete("validTargets has been deprecated. Use a property of type List<IXRInteractable> instead.", true)]
		protected virtual List<XRBaseInteractable> validTargets { get; }

		public XRBaseControllerInteractor.InputTriggerType selectActionTrigger
		{
			get
			{
				return this.m_SelectActionTrigger;
			}
			set
			{
				this.m_SelectActionTrigger = value;
			}
		}

		public bool hideControllerOnSelect
		{
			get
			{
				return this.m_HideControllerOnSelect;
			}
			set
			{
				this.m_HideControllerOnSelect = value;
				if (!this.m_HideControllerOnSelect && this.m_Controller != null)
				{
					this.m_Controller.hideControllerModel = false;
				}
			}
		}

		public bool allowHoveredActivate
		{
			get
			{
				return this.m_AllowHoveredActivate;
			}
			set
			{
				this.m_AllowHoveredActivate = value;
			}
		}

		public override TargetPriorityMode targetPriorityMode
		{
			get
			{
				return this.m_TargetPriorityMode;
			}
			set
			{
				this.m_TargetPriorityMode = value;
			}
		}

		public bool playAudioClipOnSelectEntered
		{
			get
			{
				return this.m_PlayAudioClipOnSelectEntered;
			}
			set
			{
				this.m_PlayAudioClipOnSelectEntered = value;
			}
		}

		public AudioClip audioClipForOnSelectEntered
		{
			get
			{
				return this.m_AudioClipForOnSelectEntered;
			}
			set
			{
				this.m_AudioClipForOnSelectEntered = value;
			}
		}

		public bool playAudioClipOnSelectExited
		{
			get
			{
				return this.m_PlayAudioClipOnSelectExited;
			}
			set
			{
				this.m_PlayAudioClipOnSelectExited = value;
			}
		}

		public AudioClip audioClipForOnSelectExited
		{
			get
			{
				return this.m_AudioClipForOnSelectExited;
			}
			set
			{
				this.m_AudioClipForOnSelectExited = value;
			}
		}

		public bool playAudioClipOnSelectCanceled
		{
			get
			{
				return this.m_PlayAudioClipOnSelectCanceled;
			}
			set
			{
				this.m_PlayAudioClipOnSelectCanceled = value;
			}
		}

		public AudioClip audioClipForOnSelectCanceled
		{
			get
			{
				return this.m_AudioClipForOnSelectCanceled;
			}
			set
			{
				this.m_AudioClipForOnSelectCanceled = value;
			}
		}

		public bool playAudioClipOnHoverEntered
		{
			get
			{
				return this.m_PlayAudioClipOnHoverEntered;
			}
			set
			{
				this.m_PlayAudioClipOnHoverEntered = value;
			}
		}

		public AudioClip audioClipForOnHoverEntered
		{
			get
			{
				return this.m_AudioClipForOnHoverEntered;
			}
			set
			{
				this.m_AudioClipForOnHoverEntered = value;
			}
		}

		public bool playAudioClipOnHoverExited
		{
			get
			{
				return this.m_PlayAudioClipOnHoverExited;
			}
			set
			{
				this.m_PlayAudioClipOnHoverExited = value;
			}
		}

		public AudioClip audioClipForOnHoverExited
		{
			get
			{
				return this.m_AudioClipForOnHoverExited;
			}
			set
			{
				this.m_AudioClipForOnHoverExited = value;
			}
		}

		public bool playAudioClipOnHoverCanceled
		{
			get
			{
				return this.m_PlayAudioClipOnHoverCanceled;
			}
			set
			{
				this.m_PlayAudioClipOnHoverCanceled = value;
			}
		}

		public AudioClip audioClipForOnHoverCanceled
		{
			get
			{
				return this.m_AudioClipForOnHoverCanceled;
			}
			set
			{
				this.m_AudioClipForOnHoverCanceled = value;
			}
		}

		public bool allowHoverAudioWhileSelecting
		{
			get
			{
				return this.m_AllowHoverAudioWhileSelecting;
			}
			set
			{
				this.m_AllowHoverAudioWhileSelecting = value;
			}
		}

		public bool playHapticsOnSelectEntered
		{
			get
			{
				return this.m_PlayHapticsOnSelectEntered;
			}
			set
			{
				this.m_PlayHapticsOnSelectEntered = value;
			}
		}

		public float hapticSelectEnterIntensity
		{
			get
			{
				return this.m_HapticSelectEnterIntensity;
			}
			set
			{
				this.m_HapticSelectEnterIntensity = value;
			}
		}

		public float hapticSelectEnterDuration
		{
			get
			{
				return this.m_HapticSelectEnterDuration;
			}
			set
			{
				this.m_HapticSelectEnterDuration = value;
			}
		}

		public bool playHapticsOnSelectExited
		{
			get
			{
				return this.m_PlayHapticsOnSelectExited;
			}
			set
			{
				this.m_PlayHapticsOnSelectExited = value;
			}
		}

		public float hapticSelectExitIntensity
		{
			get
			{
				return this.m_HapticSelectExitIntensity;
			}
			set
			{
				this.m_HapticSelectExitIntensity = value;
			}
		}

		public float hapticSelectExitDuration
		{
			get
			{
				return this.m_HapticSelectExitDuration;
			}
			set
			{
				this.m_HapticSelectExitDuration = value;
			}
		}

		public bool playHapticsOnSelectCanceled
		{
			get
			{
				return this.m_PlayHapticsOnSelectCanceled;
			}
			set
			{
				this.m_PlayHapticsOnSelectCanceled = value;
			}
		}

		public float hapticSelectCancelIntensity
		{
			get
			{
				return this.m_HapticSelectCancelIntensity;
			}
			set
			{
				this.m_HapticSelectCancelIntensity = value;
			}
		}

		public float hapticSelectCancelDuration
		{
			get
			{
				return this.m_HapticSelectCancelDuration;
			}
			set
			{
				this.m_HapticSelectCancelDuration = value;
			}
		}

		public bool playHapticsOnHoverEntered
		{
			get
			{
				return this.m_PlayHapticsOnHoverEntered;
			}
			set
			{
				this.m_PlayHapticsOnHoverEntered = value;
			}
		}

		public float hapticHoverEnterIntensity
		{
			get
			{
				return this.m_HapticHoverEnterIntensity;
			}
			set
			{
				this.m_HapticHoverEnterIntensity = value;
			}
		}

		public float hapticHoverEnterDuration
		{
			get
			{
				return this.m_HapticHoverEnterDuration;
			}
			set
			{
				this.m_HapticHoverEnterDuration = value;
			}
		}

		public bool playHapticsOnHoverExited
		{
			get
			{
				return this.m_PlayHapticsOnHoverExited;
			}
			set
			{
				this.m_PlayHapticsOnHoverExited = value;
			}
		}

		public float hapticHoverExitIntensity
		{
			get
			{
				return this.m_HapticHoverExitIntensity;
			}
			set
			{
				this.m_HapticHoverExitIntensity = value;
			}
		}

		public float hapticHoverExitDuration
		{
			get
			{
				return this.m_HapticHoverExitDuration;
			}
			set
			{
				this.m_HapticHoverExitDuration = value;
			}
		}

		public bool playHapticsOnHoverCanceled
		{
			get
			{
				return this.m_PlayHapticsOnHoverCanceled;
			}
			set
			{
				this.m_PlayHapticsOnHoverCanceled = value;
			}
		}

		public float hapticHoverCancelIntensity
		{
			get
			{
				return this.m_HapticHoverCancelIntensity;
			}
			set
			{
				this.m_HapticHoverCancelIntensity = value;
			}
		}

		public float hapticHoverCancelDuration
		{
			get
			{
				return this.m_HapticHoverCancelDuration;
			}
			set
			{
				this.m_HapticHoverCancelDuration = value;
			}
		}

		public bool allowHoverHapticsWhileSelecting
		{
			get
			{
				return this.m_AllowHoverHapticsWhileSelecting;
			}
			set
			{
				this.m_AllowHoverHapticsWhileSelecting = value;
			}
		}

		public bool allowActivate
		{
			get
			{
				return this.m_AllowActivate;
			}
			set
			{
				this.m_AllowActivate = value;
			}
		}

		public XRBaseController xrController
		{
			get
			{
				return this.m_Controller;
			}
			set
			{
				if (this.m_Controller != value)
				{
					this.m_Controller = value;
					this.OnXRControllerChanged();
				}
			}
		}

		private static ActivateEventArgs CreateActivateEventArgs()
		{
			return new ActivateEventArgs();
		}

		private static DeactivateEventArgs CreateDeactivateEventArgs()
		{
			return new DeactivateEventArgs();
		}

		protected override void Awake()
		{
			this.targetsForSelection = new List<IXRSelectInteractable>();
			base.Awake();
			this.xrController = base.gameObject.GetComponentInParent<XRBaseController>(true);
			if (this.xrController == null)
			{
				Debug.LogWarning(string.Format("Could not find {0} component on {1} or any of its parents.", "XRBaseController", base.gameObject), this);
			}
			if (this.m_SelectActionTrigger == XRBaseControllerInteractor.InputTriggerType.Toggle && base.startingSelectedInteractable != null)
			{
				this.m_ToggleSelectActive = true;
			}
			if (this.m_PlayAudioClipOnSelectEntered || this.m_PlayAudioClipOnSelectExited || this.m_PlayAudioClipOnSelectCanceled || this.m_PlayAudioClipOnHoverEntered || this.m_PlayAudioClipOnHoverExited || this.m_PlayAudioClipOnHoverCanceled)
			{
				this.CreateEffectsAudioSource();
			}
		}

		private protected virtual void OnXRControllerChanged()
		{
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				this.m_ToggleSelectDeactivatedThisFrame = false;
				if (this.m_SelectActionTrigger == XRBaseControllerInteractor.InputTriggerType.Toggle || this.m_SelectActionTrigger == XRBaseControllerInteractor.InputTriggerType.Sticky)
				{
					if (this.m_Controller == null)
					{
						return;
					}
					if (this.m_ToggleSelectActive && this.m_Controller.selectInteractionState.activatedThisFrame)
					{
						this.m_ToggleSelectActive = false;
						this.m_ToggleSelectDeactivatedThisFrame = true;
						this.m_WaitingForSelectDeactivate = true;
					}
					if (this.m_Controller.selectInteractionState.deactivatedThisFrame)
					{
						this.m_WaitingForSelectDeactivate = false;
					}
				}
			}
		}

		public override void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.ProcessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic && this.m_AllowActivate)
			{
				bool shouldActivate = this.shouldActivate;
				bool shouldDeactivate = this.shouldDeactivate;
				if (shouldActivate || shouldDeactivate)
				{
					this.GetActivateTargets(XRBaseControllerInteractor.s_ActivateTargets);
					if (shouldActivate)
					{
						this.SendActivateEvent(XRBaseControllerInteractor.s_ActivateTargets);
					}
					if (shouldDeactivate)
					{
						this.SendDeactivateEvent(XRBaseControllerInteractor.s_ActivateTargets);
					}
				}
			}
		}

		private void SendActivateEvent(List<IXRActivateInteractable> targets)
		{
			foreach (IXRActivateInteractable ixractivateInteractable in targets)
			{
				if (ixractivateInteractable != null && !(ixractivateInteractable as Object == null))
				{
					ActivateEventArgs activateEventArgs;
					using (this.m_ActivateEventArgs.Get(out activateEventArgs))
					{
						activateEventArgs.interactorObject = this;
						activateEventArgs.interactableObject = ixractivateInteractable;
						ixractivateInteractable.OnActivated(activateEventArgs);
					}
				}
			}
		}

		private void SendDeactivateEvent(List<IXRActivateInteractable> targets)
		{
			foreach (IXRActivateInteractable ixractivateInteractable in targets)
			{
				if (ixractivateInteractable != null && !(ixractivateInteractable as Object == null))
				{
					DeactivateEventArgs deactivateEventArgs;
					using (this.m_DeactivateEventArgs.Get(out deactivateEventArgs))
					{
						deactivateEventArgs.interactorObject = this;
						deactivateEventArgs.interactableObject = ixractivateInteractable;
						ixractivateInteractable.OnDeactivated(deactivateEventArgs);
					}
				}
			}
		}

		public override bool isSelectActive
		{
			get
			{
				if (!base.isSelectActive)
				{
					return false;
				}
				if (base.isPerformingManualInteraction)
				{
					return true;
				}
				switch (this.m_SelectActionTrigger)
				{
				case XRBaseControllerInteractor.InputTriggerType.State:
					return this.m_Controller != null && this.m_Controller.selectInteractionState.active;
				case XRBaseControllerInteractor.InputTriggerType.StateChange:
					return (this.m_Controller != null && this.m_Controller.selectInteractionState.activatedThisFrame) || (base.hasSelection && this.m_Controller != null && !this.m_Controller.selectInteractionState.deactivatedThisFrame);
				case XRBaseControllerInteractor.InputTriggerType.Toggle:
					return this.m_ToggleSelectActive || (this.m_Controller != null && this.m_Controller.selectInteractionState.activatedThisFrame && !this.m_ToggleSelectDeactivatedThisFrame);
				case XRBaseControllerInteractor.InputTriggerType.Sticky:
					return this.m_ToggleSelectActive || this.m_WaitingForSelectDeactivate || (this.m_Controller != null && this.m_Controller.selectInteractionState.activatedThisFrame);
				default:
					return false;
				}
			}
		}

		protected virtual bool isUISelectActive
		{
			get
			{
				return this.m_Controller != null && this.m_Controller.uiPressInteractionState.active;
			}
		}

		protected Vector2 uiScrollValue
		{
			get
			{
				if (!(this.m_Controller != null))
				{
					return Vector2.zero;
				}
				return this.m_Controller.uiScrollValue;
			}
		}

		public virtual bool shouldActivate
		{
			get
			{
				return this.m_AllowActivate && (base.hasSelection || (this.m_AllowHoveredActivate && base.hasHover)) && this.m_Controller != null && this.m_Controller.activateInteractionState.activatedThisFrame;
			}
		}

		public virtual bool shouldDeactivate
		{
			get
			{
				return this.m_AllowActivate && (base.hasSelection || (this.m_AllowHoveredActivate && base.hasHover)) && this.m_Controller != null && this.m_Controller.activateInteractionState.deactivatedThisFrame;
			}
		}

		public virtual void GetActivateTargets(List<IXRActivateInteractable> targets)
		{
			targets.Clear();
			if (base.hasSelection)
			{
				using (List<IXRSelectInteractable>.Enumerator enumerator = base.interactablesSelected.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IXRSelectInteractable ixrselectInteractable = enumerator.Current;
						IXRActivateInteractable ixractivateInteractable = ixrselectInteractable as IXRActivateInteractable;
						if (ixractivateInteractable != null)
						{
							targets.Add(ixractivateInteractable);
						}
					}
					return;
				}
			}
			if (this.m_AllowHoveredActivate && base.hasHover)
			{
				foreach (IXRHoverInteractable ixrhoverInteractable in base.interactablesHovered)
				{
					IXRActivateInteractable ixractivateInteractable2 = ixrhoverInteractable as IXRActivateInteractable;
					if (ixractivateInteractable2 != null)
					{
						targets.Add(ixractivateInteractable2);
					}
				}
			}
		}

		protected override void OnSelectEntering(SelectEnterEventArgs args)
		{
			base.OnSelectEntering(args);
			this.HandleSelecting();
			if (this.m_PlayHapticsOnSelectEntered)
			{
				this.SendHapticImpulse(this.m_HapticSelectEnterIntensity, this.m_HapticSelectEnterDuration);
			}
			if (this.m_PlayAudioClipOnSelectEntered)
			{
				this.PlayAudio(this.m_AudioClipForOnSelectEntered);
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			this.HandleDeselecting();
			if (args.isCanceled)
			{
				if (this.m_PlayHapticsOnSelectCanceled)
				{
					this.SendHapticImpulse(this.m_HapticSelectCancelIntensity, this.m_HapticSelectCancelDuration);
				}
				if (this.m_PlayAudioClipOnSelectCanceled)
				{
					this.PlayAudio(this.m_AudioClipForOnSelectCanceled);
					return;
				}
			}
			else
			{
				if (this.m_PlayHapticsOnSelectExited)
				{
					this.SendHapticImpulse(this.m_HapticSelectExitIntensity, this.m_HapticSelectExitDuration);
				}
				if (this.m_PlayAudioClipOnSelectExited)
				{
					this.PlayAudio(this.m_AudioClipForOnSelectExited);
				}
			}
		}

		protected override void OnHoverEntering(HoverEnterEventArgs args)
		{
			base.OnHoverEntering(args);
			IXRHoverInteractable interactableObject = args.interactableObject;
			if (this.m_PlayHapticsOnHoverEntered && this.CanPlayHoverHaptics(interactableObject))
			{
				this.SendHapticImpulse(this.m_HapticHoverEnterIntensity, this.m_HapticHoverEnterDuration);
			}
			if (this.m_PlayAudioClipOnHoverEntered && this.CanPlayHoverAudio(interactableObject))
			{
				this.PlayAudio(this.m_AudioClipForOnHoverEntered);
			}
		}

		protected override void OnHoverExiting(HoverExitEventArgs args)
		{
			base.OnHoverExiting(args);
			IXRHoverInteractable interactableObject = args.interactableObject;
			if (args.isCanceled)
			{
				if (this.m_PlayHapticsOnHoverCanceled && this.CanPlayHoverHaptics(interactableObject))
				{
					this.SendHapticImpulse(this.m_HapticHoverCancelIntensity, this.m_HapticHoverCancelDuration);
				}
				if (this.m_PlayAudioClipOnHoverCanceled && this.CanPlayHoverAudio(interactableObject))
				{
					this.PlayAudio(this.m_AudioClipForOnHoverCanceled);
					return;
				}
			}
			else
			{
				if (this.m_PlayHapticsOnHoverExited && this.CanPlayHoverHaptics(interactableObject))
				{
					this.SendHapticImpulse(this.m_HapticHoverExitIntensity, this.m_HapticHoverExitDuration);
				}
				if (this.m_PlayAudioClipOnHoverExited && this.CanPlayHoverAudio(interactableObject))
				{
					this.PlayAudio(this.m_AudioClipForOnHoverExited);
				}
			}
		}

		private bool CanPlayHoverAudio(IXRHoverInteractable hoveredInteractable)
		{
			return this.m_AllowHoverAudioWhileSelecting || !base.IsSelecting(hoveredInteractable);
		}

		private bool CanPlayHoverHaptics(IXRHoverInteractable hoveredInteractable)
		{
			return this.m_AllowHoverHapticsWhileSelecting || !base.IsSelecting(hoveredInteractable);
		}

		public bool SendHapticImpulse(float amplitude, float duration)
		{
			return this.m_Controller != null && this.m_Controller.SendHapticImpulse(amplitude, duration);
		}

		protected virtual void PlayAudio(AudioClip audioClip)
		{
			if (audioClip == null)
			{
				return;
			}
			if (this.m_EffectsAudioSource == null)
			{
				this.CreateEffectsAudioSource();
			}
			this.m_EffectsAudioSource.PlayOneShot(audioClip);
		}

		private void CreateEffectsAudioSource()
		{
			this.m_EffectsAudioSource = base.gameObject.AddComponent<AudioSource>();
			this.m_EffectsAudioSource.loop = false;
			this.m_EffectsAudioSource.playOnAwake = false;
		}

		private void HandleSelecting()
		{
			this.m_ToggleSelectActive = true;
			this.m_WaitingForSelectDeactivate = false;
			if (this.m_HideControllerOnSelect && this.m_Controller != null)
			{
				this.m_Controller.hideControllerModel = true;
			}
		}

		private void HandleDeselecting()
		{
			if (base.hasSelection)
			{
				return;
			}
			this.m_ToggleSelectActive = false;
			this.m_WaitingForSelectDeactivate = false;
			if (this.m_Controller != null)
			{
				this.m_Controller.hideControllerModel = false;
			}
		}

		Transform IXRInteractor.get_transform()
		{
			return base.transform;
		}

		[SerializeField]
		private XRBaseControllerInteractor.InputTriggerType m_SelectActionTrigger = XRBaseControllerInteractor.InputTriggerType.StateChange;

		[SerializeField]
		private bool m_HideControllerOnSelect;

		[SerializeField]
		private bool m_AllowHoveredActivate;

		[SerializeField]
		private TargetPriorityMode m_TargetPriorityMode;

		[SerializeField]
		[FormerlySerializedAs("m_PlayAudioClipOnSelectEnter")]
		private bool m_PlayAudioClipOnSelectEntered;

		[SerializeField]
		[FormerlySerializedAs("m_AudioClipForOnSelectEnter")]
		private AudioClip m_AudioClipForOnSelectEntered;

		[SerializeField]
		[FormerlySerializedAs("m_PlayAudioClipOnSelectExit")]
		private bool m_PlayAudioClipOnSelectExited;

		[SerializeField]
		[FormerlySerializedAs("m_AudioClipForOnSelectExit")]
		private AudioClip m_AudioClipForOnSelectExited;

		[SerializeField]
		private bool m_PlayAudioClipOnSelectCanceled;

		[SerializeField]
		private AudioClip m_AudioClipForOnSelectCanceled;

		[SerializeField]
		[FormerlySerializedAs("m_PlayAudioClipOnHoverEnter")]
		private bool m_PlayAudioClipOnHoverEntered;

		[SerializeField]
		[FormerlySerializedAs("m_AudioClipForOnHoverEnter")]
		private AudioClip m_AudioClipForOnHoverEntered;

		[SerializeField]
		[FormerlySerializedAs("m_PlayAudioClipOnHoverExit")]
		private bool m_PlayAudioClipOnHoverExited;

		[SerializeField]
		[FormerlySerializedAs("m_AudioClipForOnHoverExit")]
		private AudioClip m_AudioClipForOnHoverExited;

		[SerializeField]
		private bool m_PlayAudioClipOnHoverCanceled;

		[SerializeField]
		private AudioClip m_AudioClipForOnHoverCanceled;

		[SerializeField]
		private bool m_AllowHoverAudioWhileSelecting = true;

		[SerializeField]
		[FormerlySerializedAs("m_PlayHapticsOnSelectEnter")]
		private bool m_PlayHapticsOnSelectEntered;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticSelectEnterIntensity;

		[SerializeField]
		private float m_HapticSelectEnterDuration;

		[SerializeField]
		[FormerlySerializedAs("m_PlayHapticsOnSelectExit")]
		private bool m_PlayHapticsOnSelectExited;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticSelectExitIntensity;

		[SerializeField]
		private float m_HapticSelectExitDuration;

		[SerializeField]
		private bool m_PlayHapticsOnSelectCanceled;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticSelectCancelIntensity;

		[SerializeField]
		private float m_HapticSelectCancelDuration;

		[SerializeField]
		[FormerlySerializedAs("m_PlayHapticsOnHoverEnter")]
		private bool m_PlayHapticsOnHoverEntered;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticHoverEnterIntensity;

		[SerializeField]
		private float m_HapticHoverEnterDuration;

		[SerializeField]
		[FormerlySerializedAs("m_PlayHapticsOnHoverExit")]
		private bool m_PlayHapticsOnHoverExited;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticHoverExitIntensity;

		[SerializeField]
		private float m_HapticHoverExitDuration;

		[SerializeField]
		private bool m_PlayHapticsOnHoverCanceled;

		[SerializeField]
		[Range(0f, 1f)]
		private float m_HapticHoverCancelIntensity;

		[SerializeField]
		private float m_HapticHoverCancelDuration;

		[SerializeField]
		private bool m_AllowHoverHapticsWhileSelecting = true;

		private bool m_AllowActivate = true;

		private XRBaseController m_Controller;

		private readonly LinkedPool<ActivateEventArgs> m_ActivateEventArgs = new LinkedPool<ActivateEventArgs>(new Func<ActivateEventArgs>(XRBaseControllerInteractor.CreateActivateEventArgs), null, null, null, false, 10000);

		private readonly LinkedPool<DeactivateEventArgs> m_DeactivateEventArgs = new LinkedPool<DeactivateEventArgs>(new Func<DeactivateEventArgs>(XRBaseControllerInteractor.CreateDeactivateEventArgs), null, null, null, false, 10000);

		private static readonly List<IXRActivateInteractable> s_ActivateTargets = new List<IXRActivateInteractable>();

		private bool m_ToggleSelectActive;

		private bool m_ToggleSelectDeactivatedThisFrame;

		private bool m_WaitingForSelectDeactivate;

		private AudioSource m_EffectsAudioSource;

		[Obsolete("XRBaseControllerInteractor.InputTriggerType has been deprecated in version 3.0.0. It has been moved to XRBaseInputInteractor.InputTriggerType.")]
		public enum InputTriggerType
		{
			State,
			StateChange,
			Toggle,
			Sticky
		}
	}
}
