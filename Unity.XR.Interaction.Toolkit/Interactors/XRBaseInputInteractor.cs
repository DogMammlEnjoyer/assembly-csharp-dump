using System;
using System.Collections.Generic;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit.Feedback;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	public abstract class XRBaseInputInteractor : XRBaseInteractor, IXRActivateInteractor, IXRInteractor
	{
		public XRInputButtonReader selectInput
		{
			get
			{
				return this.m_SelectInput;
			}
			set
			{
				this.SetInputProperty(ref this.m_SelectInput, value);
			}
		}

		public XRInputButtonReader activateInput
		{
			get
			{
				return this.m_ActivateInput;
			}
			set
			{
				this.SetInputProperty(ref this.m_ActivateInput, value);
			}
		}

		public XRBaseInputInteractor.InputTriggerType selectActionTrigger
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
				this.m_LogicalSelectState.mode = this.m_SelectActionTrigger;
				return this.m_LogicalSelectState.active;
			}
		}

		public virtual bool shouldActivate
		{
			get
			{
				return this.m_AllowActivate && (base.hasSelection || (this.m_AllowHoveredActivate && base.hasHover)) && this.m_LogicalActivateState.wasPerformedThisFrame;
			}
		}

		public virtual bool shouldDeactivate
		{
			get
			{
				return this.m_AllowActivate && (base.hasSelection || (this.m_AllowHoveredActivate && base.hasHover)) && this.m_LogicalActivateState.wasCompletedThisFrame;
			}
		}

		public XRBaseInputInteractor.LogicalInputState logicalSelectState
		{
			get
			{
				return this.m_LogicalSelectState;
			}
		}

		public XRBaseInputInteractor.LogicalInputState logicalActivateState
		{
			get
			{
				return this.m_LogicalActivateState;
			}
		}

		protected List<XRInputButtonReader> buttonReaders { get; } = new List<XRInputButtonReader>();

		protected List<XRInputValueReader> valueReaders { get; } = new List<XRInputValueReader>();

		protected override void Awake()
		{
			this.targetsForSelection = new List<IXRSelectInteractable>();
			base.Awake();
			this.buttonReaders.Add(this.m_SelectInput);
			this.buttonReaders.Add(this.m_ActivateInput);
			this.xrController = base.gameObject.GetComponentInParent<XRBaseController>(true);
			if (this.m_HideControllerOnSelect && this.m_Controller == null)
			{
				Debug.LogWarning("Hide Controller On Select is deprecated and being used by this interactor. It is only functional if a deprecated XR Controller component is added to this GameObject or a parent GameObject. Use the Select Entered and Select Exited events to hide the controller instead.", this);
			}
			if ((this.m_PlayAudioClipOnSelectEntered && this.m_AudioClipForOnSelectEntered != null) || (this.m_PlayAudioClipOnSelectExited && this.m_AudioClipForOnSelectExited != null) || (this.m_PlayAudioClipOnSelectCanceled && this.m_AudioClipForOnSelectCanceled != null) || (this.m_PlayAudioClipOnHoverEntered && this.m_AudioClipForOnHoverEntered != null) || (this.m_PlayAudioClipOnHoverExited && this.m_AudioClipForOnHoverExited != null) || (this.m_PlayAudioClipOnHoverCanceled && this.m_AudioClipForOnHoverCanceled != null))
			{
				Debug.LogWarning("Audio Events are deprecated and being used by this interactor. Use the SimpleAudioFeedback component instead.", this);
				this.GetOrCreateAndMigrateAudioFeedback();
			}
			if (this.m_PlayHapticsOnSelectEntered || this.m_PlayHapticsOnSelectExited || this.m_PlayHapticsOnSelectCanceled || this.m_PlayHapticsOnHoverEntered || this.m_PlayHapticsOnHoverExited || this.m_PlayHapticsOnHoverCanceled)
			{
				Debug.LogWarning("Haptic Events are deprecated and being used by this interactor. Use the SimpleHapticFeedback component instead.", this);
				this.GetOrCreateAndMigrateHapticFeedback();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.buttonReaders.ForEach(delegate(XRInputButtonReader reader)
			{
				if (reader != null)
				{
					reader.EnableDirectActionIfModeUsed();
				}
			});
			this.valueReaders.ForEach(delegate(XRInputValueReader reader)
			{
				if (reader != null)
				{
					reader.EnableDirectActionIfModeUsed();
				}
			});
			this.WarnMixedInputConfiguration();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.buttonReaders.ForEach(delegate(XRInputButtonReader reader)
			{
				if (reader != null)
				{
					reader.DisableDirectActionIfModeUsed();
				}
			});
			this.valueReaders.ForEach(delegate(XRInputValueReader reader)
			{
				if (reader != null)
				{
					reader.DisableDirectActionIfModeUsed();
				}
			});
		}

		public override void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			base.PreprocessInteractor(updatePhase);
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
			{
				if (this.forceDeprecatedInput)
				{
					if (this.m_Controller != null)
					{
						InteractionState selectInteractionState = this.m_Controller.selectInteractionState;
						this.m_LogicalSelectState.UpdateInput(selectInteractionState.active, selectInteractionState.activatedThisFrame, selectInteractionState.deactivatedThisFrame, base.hasSelection);
						InteractionState activateInteractionState = this.m_Controller.activateInteractionState;
						this.m_LogicalActivateState.UpdateInput(activateInteractionState.active, activateInteractionState.activatedThisFrame, activateInteractionState.deactivatedThisFrame, base.hasSelection);
						return;
					}
				}
				else
				{
					this.m_LogicalSelectState.UpdateInput(this.m_SelectInput.ReadIsPerformed(), this.m_SelectInput.ReadWasPerformedThisFrame(), this.m_SelectInput.ReadWasCompletedThisFrame(), base.hasSelection);
					this.m_LogicalActivateState.UpdateInput(this.m_ActivateInput.ReadIsPerformed(), this.m_ActivateInput.ReadWasPerformedThisFrame(), this.m_ActivateInput.ReadWasCompletedThisFrame(), base.hasSelection);
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
					this.GetActivateTargets(XRBaseInputInteractor.s_ActivateTargets);
					if (shouldActivate)
					{
						this.SendActivateEvent(XRBaseInputInteractor.s_ActivateTargets);
					}
					if (shouldDeactivate)
					{
						this.SendDeactivateEvent(XRBaseInputInteractor.s_ActivateTargets);
					}
				}
			}
		}

		protected void SetInputProperty(ref XRInputButtonReader property, XRInputButtonReader value)
		{
			XRInputReaderUtility.SetInputProperty(ref property, value, this, this.buttonReaders);
		}

		protected void SetInputProperty<TValue>(ref XRInputValueReader<TValue> property, XRInputValueReader<TValue> value) where TValue : struct
		{
			XRInputReaderUtility.SetInputProperty<TValue>(ref property, value, this, this.valueReaders);
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
			this.m_LogicalSelectState.UpdateHasSelection(true);
			if (this.m_HideControllerOnSelect && this.m_Controller != null)
			{
				this.m_Controller.hideControllerModel = true;
			}
		}

		protected override void OnSelectExiting(SelectExitEventArgs args)
		{
			base.OnSelectExiting(args);
			if (base.hasSelection)
			{
				return;
			}
			this.m_LogicalSelectState.UpdateHasSelection(false);
			if (this.m_HideControllerOnSelect && this.m_Controller != null)
			{
				this.m_Controller.hideControllerModel = false;
			}
		}

		public bool SendHapticImpulse(float amplitude, float duration)
		{
			if (this.m_HapticImpulsePlayer == null)
			{
				this.GetOrCreateHapticImpulsePlayer();
			}
			return this.m_HapticImpulsePlayer.SendHapticImpulse(amplitude, duration);
		}

		protected virtual void PlayAudio(AudioClip audioClip)
		{
			if (audioClip == null)
			{
				return;
			}
			if (this.m_AudioSource == null)
			{
				this.GetOrCreateAudioSource();
			}
			this.m_AudioSource.PlayOneShot(audioClip);
		}

		private void GetOrCreateAudioSource()
		{
			if (!base.TryGetComponent<AudioSource>(out this.m_AudioSource))
			{
				this.m_AudioSource = base.gameObject.AddComponent<AudioSource>();
			}
			this.m_AudioSource.loop = false;
			this.m_AudioSource.playOnAwake = false;
		}

		private void GetOrCreateHapticImpulsePlayer()
		{
			this.m_HapticImpulsePlayer = HapticImpulsePlayer.GetOrCreateInHierarchy(base.gameObject);
		}

		private void GetOrCreateAndMigrateAudioFeedback()
		{
			if (this.m_AudioFeedback != null)
			{
				return;
			}
			if (!base.TryGetComponent<SimpleAudioFeedback>(out this.m_AudioFeedback))
			{
				this.m_AudioFeedback = base.gameObject.AddComponent<SimpleAudioFeedback>();
				this.m_AudioFeedback.playSelectEntered = this.m_PlayAudioClipOnSelectEntered;
				this.m_AudioFeedback.selectEnteredClip = this.m_AudioClipForOnSelectEntered;
				this.m_AudioFeedback.playSelectExited = this.m_PlayAudioClipOnSelectExited;
				this.m_AudioFeedback.selectExitedClip = this.m_AudioClipForOnSelectExited;
				this.m_AudioFeedback.playSelectCanceled = this.m_PlayAudioClipOnSelectCanceled;
				this.m_AudioFeedback.selectCanceledClip = this.m_AudioClipForOnSelectCanceled;
				this.m_AudioFeedback.playHoverEntered = this.m_PlayAudioClipOnHoverEntered;
				this.m_AudioFeedback.hoverEnteredClip = this.m_AudioClipForOnHoverEntered;
				this.m_AudioFeedback.playHoverExited = this.m_PlayAudioClipOnHoverExited;
				this.m_AudioFeedback.hoverExitedClip = this.m_AudioClipForOnHoverExited;
				this.m_AudioFeedback.playHoverCanceled = this.m_PlayAudioClipOnHoverCanceled;
				this.m_AudioFeedback.hoverCanceledClip = this.m_AudioClipForOnHoverCanceled;
				this.m_AudioFeedback.allowHoverAudioWhileSelecting = this.m_AllowHoverAudioWhileSelecting;
				this.m_AudioFeedback.SetInteractorSource(this);
			}
		}

		private void GetOrCreateAndMigrateHapticFeedback()
		{
			if (this.m_HapticFeedback != null)
			{
				return;
			}
			if (!base.TryGetComponent<SimpleHapticFeedback>(out this.m_HapticFeedback))
			{
				this.m_HapticFeedback = base.gameObject.AddComponent<SimpleHapticFeedback>();
				this.m_HapticFeedback.playSelectEntered = this.m_PlayHapticsOnSelectEntered;
				SimpleHapticFeedback hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.selectEnteredData == null)
				{
					hapticFeedback.selectEnteredData = new HapticImpulseData();
				}
				this.m_HapticFeedback.selectEnteredData.amplitude = this.m_HapticSelectEnterIntensity;
				this.m_HapticFeedback.selectEnteredData.duration = this.m_HapticSelectEnterDuration;
				this.m_HapticFeedback.playSelectExited = this.m_PlayHapticsOnSelectExited;
				hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.selectExitedData == null)
				{
					hapticFeedback.selectExitedData = new HapticImpulseData();
				}
				this.m_HapticFeedback.selectExitedData.amplitude = this.m_HapticSelectExitIntensity;
				this.m_HapticFeedback.selectExitedData.duration = this.m_HapticSelectExitDuration;
				this.m_HapticFeedback.playSelectCanceled = this.m_PlayHapticsOnSelectCanceled;
				hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.selectCanceledData == null)
				{
					hapticFeedback.selectCanceledData = new HapticImpulseData();
				}
				this.m_HapticFeedback.selectCanceledData.amplitude = this.m_HapticSelectCancelIntensity;
				this.m_HapticFeedback.selectCanceledData.duration = this.m_HapticSelectCancelDuration;
				this.m_HapticFeedback.playHoverEntered = this.m_PlayHapticsOnHoverEntered;
				hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.hoverEnteredData == null)
				{
					hapticFeedback.hoverEnteredData = new HapticImpulseData();
				}
				this.m_HapticFeedback.hoverEnteredData.amplitude = this.m_HapticHoverEnterIntensity;
				this.m_HapticFeedback.hoverEnteredData.duration = this.m_HapticHoverEnterDuration;
				this.m_HapticFeedback.playHoverExited = this.m_PlayHapticsOnHoverExited;
				hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.hoverExitedData == null)
				{
					hapticFeedback.hoverExitedData = new HapticImpulseData();
				}
				this.m_HapticFeedback.hoverExitedData.amplitude = this.m_HapticHoverExitIntensity;
				this.m_HapticFeedback.hoverExitedData.duration = this.m_HapticHoverExitDuration;
				this.m_HapticFeedback.playHoverCanceled = this.m_PlayHapticsOnHoverCanceled;
				hapticFeedback = this.m_HapticFeedback;
				if (hapticFeedback.hoverCanceledData == null)
				{
					hapticFeedback.hoverCanceledData = new HapticImpulseData();
				}
				this.m_HapticFeedback.hoverCanceledData.amplitude = this.m_HapticHoverCancelIntensity;
				this.m_HapticFeedback.hoverCanceledData.duration = this.m_HapticHoverCancelDuration;
				this.m_HapticFeedback.allowHoverHapticsWhileSelecting = this.m_AllowHoverHapticsWhileSelecting;
				this.m_HapticFeedback.SetInteractorSource(this);
			}
		}

		[Obsolete("hideControllerOnSelect has been deprecated in version 3.0.0.")]
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

		[Obsolete("inputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
		public XRBaseInputInteractor.InputCompatibilityMode inputCompatibilityMode
		{
			get
			{
				return this.m_InputCompatibilityMode;
			}
			set
			{
				this.m_InputCompatibilityMode = value;
			}
		}

		[Obsolete("forceDeprecatedInput introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
		public bool forceDeprecatedInput
		{
			get
			{
				return (this.m_HasXRController && this.m_InputCompatibilityMode == XRBaseInputInteractor.InputCompatibilityMode.Automatic) || this.m_InputCompatibilityMode == XRBaseInputInteractor.InputCompatibilityMode.ForceDeprecatedInput;
			}
			set
			{
				this.m_InputCompatibilityMode = (value ? XRBaseInputInteractor.InputCompatibilityMode.ForceDeprecatedInput : XRBaseInputInteractor.InputCompatibilityMode.ForceInputReaders);
			}
		}

		[Obsolete("xrController has been deprecated in version 3.0.0.")]
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

		[Obsolete("isUISelectActive has been deprecated in version 3.0.0. Use a serialized XRInputButtonReader to read button input instead. Some derived interactors have a uiPressInput property that can be used instead.")]
		protected virtual bool isUISelectActive
		{
			get
			{
				return this.m_Controller != null && this.m_Controller.uiPressInteractionState.active;
			}
		}

		[Obsolete("uiScrollValue has been deprecated in version 3.0.0. Use a serialized XRInputValueReader<Vector2> to read scroll input instead. Some derived interactors have a uiScrollInput property that can be used instead.")]
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

		[Obsolete("playAudioClipOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectEntered instead.")]
		public bool playAudioClipOnSelectEntered
		{
			get
			{
				return this.m_PlayAudioClipOnSelectEntered;
			}
			set
			{
				this.m_PlayAudioClipOnSelectEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playSelectEntered = value;
				}
			}
		}

		[Obsolete("audioClipForOnSelectEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectEnteredClip instead.")]
		public AudioClip audioClipForOnSelectEntered
		{
			get
			{
				return this.m_AudioClipForOnSelectEntered;
			}
			set
			{
				this.m_AudioClipForOnSelectEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.selectEnteredClip = value;
				}
			}
		}

		[Obsolete("playAudioClipOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectExited instead.")]
		public bool playAudioClipOnSelectExited
		{
			get
			{
				return this.m_PlayAudioClipOnSelectExited;
			}
			set
			{
				this.m_PlayAudioClipOnSelectExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playSelectExited = value;
				}
			}
		}

		[Obsolete("audioClipForOnSelectExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectExitedClip instead.")]
		public AudioClip audioClipForOnSelectExited
		{
			get
			{
				return this.m_AudioClipForOnSelectExited;
			}
			set
			{
				this.m_AudioClipForOnSelectExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.selectExitedClip = value;
				}
			}
		}

		[Obsolete("playAudioClipOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playSelectCanceled instead.")]
		public bool playAudioClipOnSelectCanceled
		{
			get
			{
				return this.m_PlayAudioClipOnSelectCanceled;
			}
			set
			{
				this.m_PlayAudioClipOnSelectCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playSelectCanceled = value;
				}
			}
		}

		[Obsolete("audioClipForOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.selectCanceledClip instead.")]
		public AudioClip audioClipForOnSelectCanceled
		{
			get
			{
				return this.m_AudioClipForOnSelectCanceled;
			}
			set
			{
				this.m_AudioClipForOnSelectCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.selectCanceledClip = value;
				}
			}
		}

		[Obsolete("playAudioClipOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverEntered instead.")]
		public bool playAudioClipOnHoverEntered
		{
			get
			{
				return this.m_PlayAudioClipOnHoverEntered;
			}
			set
			{
				this.m_PlayAudioClipOnHoverEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playHoverEntered = value;
				}
			}
		}

		[Obsolete("audioClipForOnHoverEntered has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverEnteredClip instead.")]
		public AudioClip audioClipForOnHoverEntered
		{
			get
			{
				return this.m_AudioClipForOnHoverEntered;
			}
			set
			{
				this.m_AudioClipForOnHoverEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.hoverEnteredClip = value;
				}
			}
		}

		[Obsolete("playAudioClipOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverExited instead.")]
		public bool playAudioClipOnHoverExited
		{
			get
			{
				return this.m_PlayAudioClipOnHoverExited;
			}
			set
			{
				this.m_PlayAudioClipOnHoverExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playHoverExited = value;
				}
			}
		}

		[Obsolete("audioClipForOnHoverExited has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverExitedClip instead.")]
		public AudioClip audioClipForOnHoverExited
		{
			get
			{
				return this.m_AudioClipForOnHoverExited;
			}
			set
			{
				this.m_AudioClipForOnHoverExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.hoverExitedClip = value;
				}
			}
		}

		[Obsolete("playAudioClipOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.playHoverCanceled instead.")]
		public bool playAudioClipOnHoverCanceled
		{
			get
			{
				return this.m_PlayAudioClipOnHoverCanceled;
			}
			set
			{
				this.m_PlayAudioClipOnHoverCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.playHoverCanceled = value;
				}
			}
		}

		[Obsolete("audioClipForOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleAudioFeedback.hoverCanceledClip instead.")]
		public AudioClip audioClipForOnHoverCanceled
		{
			get
			{
				return this.m_AudioClipForOnHoverCanceled;
			}
			set
			{
				this.m_AudioClipForOnHoverCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.hoverCanceledClip = value;
				}
			}
		}

		[Obsolete("allowHoverAudioWhileSelecting has been deprecated in version 3.0.0. Use SimpleAudioFeedback.allowHoverAudioWhileSelecting instead.")]
		public bool allowHoverAudioWhileSelecting
		{
			get
			{
				return this.m_AllowHoverAudioWhileSelecting;
			}
			set
			{
				this.m_AllowHoverAudioWhileSelecting = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateAudioFeedback();
					this.m_AudioFeedback.allowHoverAudioWhileSelecting = value;
				}
			}
		}

		[Obsolete("playHapticsOnSelectEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectEntered instead.")]
		public bool playHapticsOnSelectEntered
		{
			get
			{
				return this.m_PlayHapticsOnSelectEntered;
			}
			set
			{
				this.m_PlayHapticsOnSelectEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playSelectEntered = value;
				}
			}
		}

		[Obsolete("hapticSelectEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.amplitude instead.")]
		public float hapticSelectEnterIntensity
		{
			get
			{
				return this.m_HapticSelectEnterIntensity;
			}
			set
			{
				this.m_HapticSelectEnterIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectEnteredData != null)
					{
						this.m_HapticFeedback.selectEnteredData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.selectEnteredData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticSelectEnterDuration
					};
				}
			}
		}

		[Obsolete("hapticSelectEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectEnteredData.duration instead.")]
		public float hapticSelectEnterDuration
		{
			get
			{
				return this.m_HapticSelectEnterDuration;
			}
			set
			{
				this.m_HapticSelectEnterDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectEnteredData != null)
					{
						this.m_HapticFeedback.selectEnteredData.duration = value;
						return;
					}
					this.m_HapticFeedback.selectEnteredData = new HapticImpulseData
					{
						amplitude = this.m_HapticSelectEnterIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("playHapticsOnSelectExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectExited instead.")]
		public bool playHapticsOnSelectExited
		{
			get
			{
				return this.m_PlayHapticsOnSelectExited;
			}
			set
			{
				this.m_PlayHapticsOnSelectExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playSelectExited = value;
				}
			}
		}

		[Obsolete("hapticSelectExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.amplitude instead.")]
		public float hapticSelectExitIntensity
		{
			get
			{
				return this.m_HapticSelectExitIntensity;
			}
			set
			{
				this.m_HapticSelectExitIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectExitedData != null)
					{
						this.m_HapticFeedback.selectExitedData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.selectExitedData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticSelectExitDuration
					};
				}
			}
		}

		[Obsolete("hapticSelectExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectExitedData.duration instead.")]
		public float hapticSelectExitDuration
		{
			get
			{
				return this.m_HapticSelectExitDuration;
			}
			set
			{
				this.m_HapticSelectExitDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectExitedData != null)
					{
						this.m_HapticFeedback.selectExitedData.duration = value;
						return;
					}
					this.m_HapticFeedback.selectExitedData = new HapticImpulseData
					{
						amplitude = this.m_HapticSelectExitIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("playHapticsOnSelectCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playSelectCanceled instead.")]
		public bool playHapticsOnSelectCanceled
		{
			get
			{
				return this.m_PlayHapticsOnSelectCanceled;
			}
			set
			{
				this.m_PlayHapticsOnSelectCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playSelectCanceled = value;
				}
			}
		}

		[Obsolete("hapticSelectCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.amplitude instead.")]
		public float hapticSelectCancelIntensity
		{
			get
			{
				return this.m_HapticSelectCancelIntensity;
			}
			set
			{
				this.m_HapticSelectCancelIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectCanceledData != null)
					{
						this.m_HapticFeedback.selectCanceledData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.selectCanceledData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticSelectCancelDuration
					};
				}
			}
		}

		[Obsolete("hapticSelectCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.selectCanceledData.duration instead.")]
		public float hapticSelectCancelDuration
		{
			get
			{
				return this.m_HapticSelectCancelDuration;
			}
			set
			{
				this.m_HapticSelectCancelDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.selectCanceledData != null)
					{
						this.m_HapticFeedback.selectCanceledData.duration = value;
						return;
					}
					this.m_HapticFeedback.selectCanceledData = new HapticImpulseData
					{
						amplitude = this.m_HapticSelectCancelIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("playHapticsOnHoverEntered has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverEntered instead.")]
		public bool playHapticsOnHoverEntered
		{
			get
			{
				return this.m_PlayHapticsOnHoverEntered;
			}
			set
			{
				this.m_PlayHapticsOnHoverEntered = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playHoverEntered = value;
				}
			}
		}

		[Obsolete("hapticHoverEnterIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.amplitude instead.")]
		public float hapticHoverEnterIntensity
		{
			get
			{
				return this.m_HapticHoverEnterIntensity;
			}
			set
			{
				this.m_HapticHoverEnterIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverEnteredData != null)
					{
						this.m_HapticFeedback.hoverEnteredData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.hoverEnteredData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticHoverEnterDuration
					};
				}
			}
		}

		[Obsolete("hapticHoverEnterDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverEnteredData.duration instead.")]
		public float hapticHoverEnterDuration
		{
			get
			{
				return this.m_HapticHoverEnterDuration;
			}
			set
			{
				this.m_HapticHoverEnterDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverEnteredData != null)
					{
						this.m_HapticFeedback.hoverEnteredData.duration = value;
						return;
					}
					this.m_HapticFeedback.hoverEnteredData = new HapticImpulseData
					{
						amplitude = this.m_HapticHoverEnterIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("playHapticsOnHoverExited has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverExited instead.")]
		public bool playHapticsOnHoverExited
		{
			get
			{
				return this.m_PlayHapticsOnHoverExited;
			}
			set
			{
				this.m_PlayHapticsOnHoverExited = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playHoverExited = value;
				}
			}
		}

		[Obsolete("hapticHoverExitIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.amplitude instead.")]
		public float hapticHoverExitIntensity
		{
			get
			{
				return this.m_HapticHoverExitIntensity;
			}
			set
			{
				this.m_HapticHoverExitIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverExitedData != null)
					{
						this.m_HapticFeedback.hoverExitedData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.hoverExitedData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticHoverExitDuration
					};
				}
			}
		}

		[Obsolete("hapticHoverExitDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverExitedData.duration instead.")]
		public float hapticHoverExitDuration
		{
			get
			{
				return this.m_HapticHoverExitDuration;
			}
			set
			{
				this.m_HapticHoverExitDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverExitedData != null)
					{
						this.m_HapticFeedback.hoverExitedData.duration = value;
						return;
					}
					this.m_HapticFeedback.hoverExitedData = new HapticImpulseData
					{
						amplitude = this.m_HapticHoverExitIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("playHapticsOnHoverCanceled has been deprecated in version 3.0.0. Use SimpleHapticFeedback.playHoverCanceled instead.")]
		public bool playHapticsOnHoverCanceled
		{
			get
			{
				return this.m_PlayHapticsOnHoverCanceled;
			}
			set
			{
				this.m_PlayHapticsOnHoverCanceled = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.playHoverCanceled = value;
				}
			}
		}

		[Obsolete("hapticHoverCancelIntensity has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.amplitude instead.")]
		public float hapticHoverCancelIntensity
		{
			get
			{
				return this.m_HapticHoverCancelIntensity;
			}
			set
			{
				this.m_HapticHoverCancelIntensity = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverCanceledData != null)
					{
						this.m_HapticFeedback.hoverCanceledData.amplitude = value;
						return;
					}
					this.m_HapticFeedback.hoverCanceledData = new HapticImpulseData
					{
						amplitude = value,
						duration = this.m_HapticHoverCancelDuration
					};
				}
			}
		}

		[Obsolete("hapticHoverCancelDuration has been deprecated in version 3.0.0. Use SimpleHapticFeedback.hoverCanceledData.duration instead.")]
		public float hapticHoverCancelDuration
		{
			get
			{
				return this.m_HapticHoverCancelDuration;
			}
			set
			{
				this.m_HapticHoverCancelDuration = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					if (this.m_HapticFeedback.hoverCanceledData != null)
					{
						this.m_HapticFeedback.hoverCanceledData.duration = value;
						return;
					}
					this.m_HapticFeedback.hoverCanceledData = new HapticImpulseData
					{
						amplitude = this.m_HapticHoverCancelIntensity,
						duration = value
					};
				}
			}
		}

		[Obsolete("allowHoverHapticsWhileSelecting has been deprecated in version 3.0.0. Use SimpleHapticFeedback.allowHoverHapticsWhileSelecting instead.")]
		public bool allowHoverHapticsWhileSelecting
		{
			get
			{
				return this.m_AllowHoverHapticsWhileSelecting;
			}
			set
			{
				this.m_AllowHoverHapticsWhileSelecting = value;
				if (Application.isPlaying)
				{
					this.GetOrCreateAndMigrateHapticFeedback();
					this.m_HapticFeedback.allowHoverHapticsWhileSelecting = value;
				}
			}
		}

		[Obsolete("OnXRControllerChanged has been deprecated in version 3.0.0.")]
		private protected virtual void OnXRControllerChanged()
		{
			this.m_HasXRController = (this.m_Controller != null);
		}

		private void WarnMixedInputConfiguration()
		{
			if (this.forceDeprecatedInput)
			{
				foreach (XRInputButtonReader xrinputButtonReader in this.buttonReaders)
				{
					if ((xrinputButtonReader.inputSourceMode == XRInputButtonReader.InputSourceMode.InputActionReference && (xrinputButtonReader.inputActionReferencePerformed != null || xrinputButtonReader.inputActionReferenceValue != null)) || (xrinputButtonReader.inputSourceMode != XRInputButtonReader.InputSourceMode.InputActionReference && xrinputButtonReader.inputSourceMode != XRInputButtonReader.InputSourceMode.Unused))
					{
						Debug.LogWarning("The interactor has input properties configured to be used but the interactor is set to read input through the deprecated XR Controller component instead. If you want to force the input readers to be used even when an XR Controller component is present, set Input Compatibility Mode to Force Input Readers.", this);
						return;
					}
				}
				foreach (XRInputValueReader xrinputValueReader in this.valueReaders)
				{
					if ((xrinputValueReader.inputSourceMode == XRInputValueReader.InputSourceMode.InputActionReference && xrinputValueReader.inputActionReference != null) || (xrinputValueReader.inputSourceMode != XRInputValueReader.InputSourceMode.InputActionReference && xrinputValueReader.inputSourceMode != XRInputValueReader.InputSourceMode.Unused))
					{
						Debug.LogWarning("The interactor has input properties configured to be used but the interactor is set to read input through the deprecated XR Controller component instead. If you want to force the input readers to be used even when an XR Controller component is present, set Input Compatibility Mode to Force Input Readers.", this);
						break;
					}
				}
			}
		}

		[Obsolete("CreateEffectsAudioSource has been deprecated in version 3.0.0.")]
		private void CreateEffectsAudioSource()
		{
		}

		[Obsolete("CanPlayHoverAudio has been deprecated in version 3.0.0.")]
		private bool CanPlayHoverAudio(IXRHoverInteractable hoveredInteractable)
		{
			return this.m_AllowHoverAudioWhileSelecting || !base.IsSelecting(hoveredInteractable);
		}

		[Obsolete("CanPlayHoverHaptics has been deprecated in version 3.0.0.")]
		private bool CanPlayHoverHaptics(IXRHoverInteractable hoveredInteractable)
		{
			return this.m_AllowHoverHapticsWhileSelecting || !base.IsSelecting(hoveredInteractable);
		}

		[Obsolete("HandleSelecting has been deprecated in version 3.0.0.")]
		private void HandleSelecting()
		{
		}

		[Obsolete("HandleDeselecting has been deprecated in version 3.0.0.")]
		private void HandleDeselecting()
		{
		}

		protected override void OnHoverEntering(HoverEnterEventArgs args)
		{
			base.OnHoverEntering(args);
		}

		protected override void OnHoverExiting(HoverExitEventArgs args)
		{
			base.OnHoverExiting(args);
		}

		private static ActivateEventArgs CreateActivateEventArgs()
		{
			return new ActivateEventArgs();
		}

		private static DeactivateEventArgs CreateDeactivateEventArgs()
		{
			return new DeactivateEventArgs();
		}

		Transform IXRInteractor.get_transform()
		{
			return base.transform;
		}

		[SerializeField]
		private XRInputButtonReader m_SelectInput = new XRInputButtonReader("Select", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRInputButtonReader m_ActivateInput = new XRInputButtonReader("Activate", null, false, XRInputButtonReader.InputSourceMode.InputActionReference);

		[SerializeField]
		private XRBaseInputInteractor.InputTriggerType m_SelectActionTrigger = XRBaseInputInteractor.InputTriggerType.StateChange;

		[SerializeField]
		private bool m_AllowHoveredActivate;

		[SerializeField]
		private TargetPriorityMode m_TargetPriorityMode;

		private bool m_AllowActivate = true;

		private readonly LinkedPool<ActivateEventArgs> m_ActivateEventArgs = new LinkedPool<ActivateEventArgs>(() => new ActivateEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<DeactivateEventArgs> m_DeactivateEventArgs = new LinkedPool<DeactivateEventArgs>(() => new DeactivateEventArgs(), null, null, null, false, 10000);

		private static readonly List<IXRActivateInteractable> s_ActivateTargets = new List<IXRActivateInteractable>();

		private readonly XRBaseInputInteractor.LogicalInputState m_LogicalSelectState = new XRBaseInputInteractor.LogicalInputState();

		private readonly XRBaseInputInteractor.LogicalInputState m_LogicalActivateState = new XRBaseInputInteractor.LogicalInputState();

		private SimpleAudioFeedback m_AudioFeedback;

		private SimpleHapticFeedback m_HapticFeedback;

		private AudioSource m_AudioSource;

		private HapticImpulsePlayer m_HapticImpulsePlayer;

		[SerializeField]
		private bool m_HideControllerOnSelect;

		[SerializeField]
		[Obsolete("m_InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
		private XRBaseInputInteractor.InputCompatibilityMode m_InputCompatibilityMode;

		[Obsolete("m_Controller has been deprecated in version 3.0.0.")]
		private XRBaseController m_Controller;

		private bool m_HasXRController;

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

		public enum InputTriggerType
		{
			State,
			StateChange,
			Toggle,
			Sticky
		}

		public class LogicalInputState
		{
			public bool active { get; private set; }

			public XRBaseInputInteractor.InputTriggerType mode
			{
				get
				{
					return this.m_Mode;
				}
				set
				{
					if (this.m_Mode != value)
					{
						this.m_Mode = value;
						this.Refresh();
					}
				}
			}

			public bool isPerformed { get; private set; }

			public bool wasPerformedThisFrame { get; private set; }

			public bool wasCompletedThisFrame { get; private set; }

			internal void UpdateInput(bool performed, bool performedThisFrame, bool completedThisFrame, bool hasSelection)
			{
				this.UpdateInput(performed, performedThisFrame, completedThisFrame, hasSelection, Time.realtimeSinceStartup);
			}

			private void UpdateInput(bool performed, bool performedThisFrame, bool completedThisFrame, bool hasSelection, float realtime)
			{
				this.isPerformed = performed;
				this.wasPerformedThisFrame = performedThisFrame;
				this.wasCompletedThisFrame = completedThisFrame;
				this.m_HasSelection = hasSelection;
				if (this.wasPerformedThisFrame)
				{
					this.m_TimeAtPerformed = realtime;
				}
				if (this.wasCompletedThisFrame)
				{
					this.m_TimeAtCompleted = realtime;
				}
				this.m_ToggleDeactivatedThisFrame = false;
				if (this.mode == XRBaseInputInteractor.InputTriggerType.Toggle || this.mode == XRBaseInputInteractor.InputTriggerType.Sticky)
				{
					if (this.m_ToggleActive && performedThisFrame)
					{
						this.m_ToggleActive = false;
						this.m_ToggleDeactivatedThisFrame = true;
						this.m_WaitingForDeactivate = true;
					}
					if (this.wasCompletedThisFrame)
					{
						this.m_WaitingForDeactivate = false;
					}
				}
				this.Refresh();
			}

			internal void UpdateHasSelection(bool hasSelection)
			{
				if (this.m_HasSelection == hasSelection)
				{
					return;
				}
				this.m_HasSelection = hasSelection;
				this.m_ToggleActive = hasSelection;
				this.m_WaitingForDeactivate = false;
				this.Refresh();
			}

			private void Refresh()
			{
				switch (this.mode)
				{
				case XRBaseInputInteractor.InputTriggerType.State:
					this.active = this.isPerformed;
					return;
				case XRBaseInputInteractor.InputTriggerType.StateChange:
					this.active = (this.wasPerformedThisFrame || (this.m_HasSelection && !this.wasCompletedThisFrame));
					return;
				case XRBaseInputInteractor.InputTriggerType.Toggle:
					this.active = (this.m_ToggleActive || (this.wasPerformedThisFrame && !this.m_ToggleDeactivatedThisFrame));
					return;
				case XRBaseInputInteractor.InputTriggerType.Sticky:
					this.active = (this.m_ToggleActive || this.m_WaitingForDeactivate || this.wasPerformedThisFrame);
					return;
				default:
					return;
				}
			}

			[Obsolete("wasUnperformedThisFrame has been deprecated in version 3.0.0-pre.2. It has been renamed to wasCompletedThisFrame. (UnityUpgradable) -> wasCompletedThisFrame")]
			public bool wasUnperformedThisFrame
			{
				get
				{
					return this.wasCompletedThisFrame;
				}
			}

			private XRBaseInputInteractor.InputTriggerType m_Mode;

			private bool m_HasSelection;

			private float m_TimeAtPerformed;

			private float m_TimeAtCompleted;

			private bool m_ToggleActive;

			private bool m_ToggleDeactivatedThisFrame;

			private bool m_WaitingForDeactivate;
		}

		[Obsolete("InputCompatibilityMode introduced in version 3.0.0 is marked for removal. This is only used for backwards compatibility and will be eventually removed in a future version.")]
		public enum InputCompatibilityMode
		{
			Automatic,
			ForceDeprecatedInput,
			ForceInputReaders
		}
	}
}
