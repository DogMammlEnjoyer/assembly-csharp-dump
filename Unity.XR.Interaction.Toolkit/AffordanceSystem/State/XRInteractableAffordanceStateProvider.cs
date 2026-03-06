using System;
using System.Collections;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State
{
	[AddComponentMenu("Affordance System/XR Interactable Affordance State Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State.XRInteractableAffordanceStateProvider.html")]
	[DisallowMultipleComponent]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class XRInteractableAffordanceStateProvider : BaseAffordanceStateProvider
	{
		public Object interactableSource
		{
			get
			{
				return this.m_InteractableSource;
			}
			set
			{
				this.m_InteractableSource = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.SetBoundInteractionReceiver(value as IXRInteractable);
				}
			}
		}

		public bool ignoreHoverEvents
		{
			get
			{
				return this.m_IgnoreHoverEvents;
			}
			set
			{
				this.m_IgnoreHoverEvents = value;
			}
		}

		public bool ignoreHoverPriorityEvents
		{
			get
			{
				return this.m_IgnoreHoverPriorityEvents;
			}
			set
			{
				if (Application.isPlaying && base.isActiveAndEnabled && !this.m_IgnoreHoverPriorityEvents && value)
				{
					this.StopHoveredPriorityRoutine();
					this.RefreshState();
				}
				this.m_IgnoreHoverPriorityEvents = value;
			}
		}

		public bool ignoreFocusEvents
		{
			get
			{
				return this.m_IgnoreFocusEvents;
			}
			set
			{
				this.m_IgnoreFocusEvents = value;
			}
		}

		public bool ignoreSelectEvents
		{
			get
			{
				return this.m_IgnoreSelectEvents;
			}
			set
			{
				this.m_IgnoreSelectEvents = value;
			}
		}

		public bool ignoreActivateEvents
		{
			get
			{
				return this.m_IgnoreActivateEvents;
			}
			set
			{
				this.m_IgnoreActivateEvents = value;
			}
		}

		public XRInteractableAffordanceStateProvider.SelectClickAnimationMode selectClickAnimationMode
		{
			get
			{
				return this.m_SelectClickAnimationMode;
			}
			set
			{
				this.m_SelectClickAnimationMode = value;
			}
		}

		public XRInteractableAffordanceStateProvider.ActivateClickAnimationMode activateClickAnimationMode
		{
			get
			{
				return this.m_ActivateClickAnimationMode;
			}
			set
			{
				this.m_ActivateClickAnimationMode = value;
			}
		}

		public float clickAnimationDuration
		{
			get
			{
				return this.m_ClickAnimationDuration;
			}
			set
			{
				this.m_ClickAnimationDuration = value;
			}
		}

		public AnimationCurveDatumProperty clickAnimationCurve
		{
			get
			{
				return this.m_ClickAnimationCurve;
			}
			set
			{
				this.m_ClickAnimationCurve = value;
			}
		}

		protected virtual bool isHovered
		{
			get
			{
				return this.m_HasHoverInteractable && this.m_HoverInteractable.isHovered;
			}
		}

		protected virtual bool isSelected
		{
			get
			{
				return this.m_HasSelectInteractable && this.m_SelectInteractable.isSelected;
			}
		}

		protected virtual bool isFocused
		{
			get
			{
				return this.m_FocusInteractable != null && this.m_FocusInteractable.isFocused;
			}
		}

		protected virtual bool isActivated
		{
			get
			{
				return this.m_IsActivated;
			}
		}

		protected virtual bool isRegistered
		{
			get
			{
				return this.m_IsRegistered;
			}
		}

		protected void Awake()
		{
			IXRInteractable ixrinteractable2;
			if (this.m_InteractableSource != null)
			{
				IXRInteractable ixrinteractable = this.m_InteractableSource as IXRInteractable;
				if (ixrinteractable != null)
				{
					ixrinteractable2 = ixrinteractable;
					goto IL_26;
				}
			}
			ixrinteractable2 = base.GetComponentInParent<IXRInteractable>();
			IL_26:
			IXRInteractable boundInteractionReceiver = ixrinteractable2;
			if (!this.SetBoundInteractionReceiver(boundInteractionReceiver))
			{
				XRLoggingUtils.LogWarning(string.Format("Could not find required interactable component on {0}", base.gameObject) + " for which to provide affordance states.", this);
				base.enabled = false;
			}
		}

		protected override void OnValidate()
		{
			base.OnValidate();
			if (Application.isPlaying && base.isActiveAndEnabled && this.m_IgnoreHoverPriorityEvents)
			{
				this.StopHoveredPriorityRoutine();
				this.RefreshState();
			}
		}

		public bool SetBoundInteractionReceiver(IXRInteractable receiver)
		{
			this.ClearBindings();
			Object @object = receiver as Object;
			bool flag = @object != null && @object != null;
			if (flag)
			{
				this.m_Interactable = receiver;
				IXRHoverInteractable ixrhoverInteractable = this.m_Interactable as IXRHoverInteractable;
				if (ixrhoverInteractable != null)
				{
					this.m_HoverInteractable = ixrhoverInteractable;
				}
				IXRSelectInteractable ixrselectInteractable = this.m_Interactable as IXRSelectInteractable;
				if (ixrselectInteractable != null)
				{
					this.m_SelectInteractable = ixrselectInteractable;
				}
				IXRFocusInteractable ixrfocusInteractable = this.m_Interactable as IXRFocusInteractable;
				if (ixrfocusInteractable != null)
				{
					this.m_FocusInteractable = ixrfocusInteractable;
				}
				IXRActivateInteractable ixractivateInteractable = this.m_Interactable as IXRActivateInteractable;
				if (ixractivateInteractable != null)
				{
					this.m_ActivateInteractable = ixractivateInteractable;
				}
				IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = this.m_Interactable as IXRInteractionStrengthInteractable;
				if (ixrinteractionStrengthInteractable != null)
				{
					this.m_InteractionStrengthInteractable = ixrinteractionStrengthInteractable;
				}
			}
			else
			{
				this.m_Interactable = null;
				this.m_HoverInteractable = null;
				this.m_SelectInteractable = null;
				this.m_FocusInteractable = null;
				this.m_ActivateInteractable = null;
				this.m_InteractionStrengthInteractable = null;
			}
			this.m_HasHoverInteractable = (this.m_HoverInteractable != null);
			this.m_HasSelectInteractable = (this.m_SelectInteractable != null);
			this.m_HasInteractionStrengthInteractable = (this.m_InteractionStrengthInteractable != null);
			this.BindToProviders();
			return flag;
		}

		protected virtual void OnRegistered(InteractableRegisteredEventArgs args)
		{
			this.m_IsRegistered = true;
			this.RefreshState();
		}

		protected virtual void OnUnregistered(InteractableUnregisteredEventArgs args)
		{
			this.m_IsRegistered = false;
			this.RefreshState();
		}

		protected virtual void OnFirstHoverEntered(HoverEnterEventArgs args)
		{
			this.RefreshState();
		}

		protected virtual void OnLastHoverExited(HoverExitEventArgs args)
		{
			this.RefreshState();
		}

		protected virtual void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_IgnoreHoverPriorityEvents)
			{
				return;
			}
			IXRTargetPriorityInteractor ixrtargetPriorityInteractor = args.interactorObject as IXRTargetPriorityInteractor;
			if (ixrtargetPriorityInteractor != null)
			{
				this.m_HoveringPriorityInteractorCount++;
				if (ixrtargetPriorityInteractor.targetPriorityMode != TargetPriorityMode.None)
				{
					this.m_HoveredPriorityRoutine = (this.m_HoveredPriorityRoutine ?? base.StartCoroutine(this.HoveredPriorityRoutine()));
				}
			}
		}

		protected virtual void OnHoverExited(HoverExitEventArgs args)
		{
			if (this.m_IgnoreHoverPriorityEvents)
			{
				return;
			}
			if (args.interactorObject is IXRTargetPriorityInteractor)
			{
				this.m_HoveringPriorityInteractorCount--;
				if (this.m_HoveringPriorityInteractorCount > 0)
				{
					return;
				}
				this.StopHoveredPriorityRoutine();
				this.RefreshState();
			}
		}

		private void StopHoveredPriorityRoutine()
		{
			this.m_HoveringPriorityInteractorCount = 0;
			this.m_IsHoveredPriority = false;
			if (this.m_HoveredPriorityRoutine != null)
			{
				base.StopCoroutine(this.m_HoveredPriorityRoutine);
				this.m_HoveredPriorityRoutine = null;
			}
		}

		protected virtual void OnFirstSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_IgnoreSelectEvents || this.m_SelectClickAnimationMode != XRInteractableAffordanceStateProvider.SelectClickAnimationMode.SelectEntered || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.SelectedClickBehavior();
		}

		protected virtual void OnLastSelectExited(SelectExitEventArgs args)
		{
			if (!this.m_IgnoreSelectEvents && this.m_SelectClickAnimationMode == XRInteractableAffordanceStateProvider.SelectClickAnimationMode.SelectExited && this.m_ClickAnimationDuration >= Mathf.Epsilon)
			{
				this.SelectedClickBehavior();
				return;
			}
			if (this.m_SelectedClickAnimation != null)
			{
				return;
			}
			this.RefreshState();
		}

		protected virtual void OnFirstFocusEntered(FocusEnterEventArgs args)
		{
			this.RefreshState();
		}

		protected virtual void OnLastFocusExited(FocusExitEventArgs args)
		{
			this.RefreshState();
		}

		protected virtual void OnActivatedEvent(ActivateEventArgs args)
		{
			this.m_IsActivated = true;
			if (this.m_IgnoreActivateEvents || this.m_ActivateClickAnimationMode != XRInteractableAffordanceStateProvider.ActivateClickAnimationMode.Activated || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.ActivatedClickBehavior();
		}

		protected virtual void OnDeactivatedEvent(DeactivateEventArgs args)
		{
			this.m_IsActivated = false;
			if (!this.m_IgnoreActivateEvents && this.m_ActivateClickAnimationMode == XRInteractableAffordanceStateProvider.ActivateClickAnimationMode.Deactivated && this.m_ClickAnimationDuration >= Mathf.Epsilon)
			{
				this.ActivatedClickBehavior();
				return;
			}
			if (this.m_ActivatedClickAnimation != null)
			{
				return;
			}
			this.RefreshState();
		}

		protected virtual void OnLargestInteractionStrengthChanged(float value)
		{
			if (this.m_SelectedClickAnimation != null || this.m_ActivatedClickAnimation != null)
			{
				return;
			}
			this.RefreshState();
		}

		protected virtual void SelectedClickBehavior()
		{
			this.StopAllClickAnimations();
			this.m_SelectedClickAnimation = base.StartCoroutine(this.ClickAnimation(4, this.m_ClickAnimationDuration, delegate
			{
				this.m_SelectedClickAnimation = null;
			}));
		}

		protected virtual void ActivatedClickBehavior()
		{
			this.StopAllClickAnimations();
			this.m_ActivatedClickAnimation = base.StartCoroutine(this.ClickAnimation(5, this.m_ClickAnimationDuration, delegate
			{
				this.m_ActivatedClickAnimation = null;
			}));
		}

		private void StopActivatedCoroutine()
		{
			if (this.m_ActivatedClickAnimation == null)
			{
				return;
			}
			base.StopCoroutine(this.m_ActivatedClickAnimation);
			this.m_ActivatedClickAnimation = null;
		}

		private void StopSelectedCoroutine()
		{
			if (this.m_SelectedClickAnimation == null)
			{
				return;
			}
			base.StopCoroutine(this.m_SelectedClickAnimation);
			this.m_SelectedClickAnimation = null;
		}

		private void StopAllClickAnimations()
		{
			this.StopActivatedCoroutine();
			this.StopSelectedCoroutine();
		}

		protected virtual IEnumerator ClickAnimation(byte targetStateIndex, float duration, Action onComplete = null)
		{
			for (float elapsedTime = 0f; elapsedTime < duration; elapsedTime += Time.deltaTime)
			{
				float time = Mathf.Clamp01(elapsedTime / duration);
				float transitionAmount = this.m_ClickAnimationCurve.Value.Evaluate(time);
				AffordanceStateData newAffordanceStateData = new AffordanceStateData(targetStateIndex, transitionAmount);
				base.UpdateAffordanceState(newAffordanceStateData);
				yield return null;
			}
			yield return null;
			this.RefreshState();
			if (onComplete != null)
			{
				onComplete();
			}
			yield break;
		}

		protected virtual AffordanceStateData GenerateNewAffordanceState()
		{
			if (!this.m_IsBoundToInteractionEvents)
			{
				return base.currentAffordanceStateData.Value;
			}
			if (this.isActivated && !this.m_IgnoreActivateEvents)
			{
				return AffordanceStateShortcuts.activatedState;
			}
			if (!this.isActivated && this.isSelected && !this.m_IgnoreSelectEvents)
			{
				float transitionAmount = this.m_HasInteractionStrengthInteractable ? this.m_InteractionStrengthInteractable.largestInteractionStrength.Value : 1f;
				return new AffordanceStateData(4, transitionAmount);
			}
			if (!this.isActivated && !this.isSelected && this.isHovered && !this.m_IgnoreHoverEvents)
			{
				byte stateIndex = this.m_IsHoveredPriority ? 3 : 2;
				float transitionAmount2 = this.m_HasInteractionStrengthInteractable ? this.m_InteractionStrengthInteractable.largestInteractionStrength.Value : 0f;
				return new AffordanceStateData(stateIndex, transitionAmount2);
			}
			if (!this.isActivated && !this.isSelected && !this.isHovered && this.isFocused && !this.m_IgnoreFocusEvents)
			{
				return AffordanceStateShortcuts.focusedState;
			}
			if (!this.isRegistered)
			{
				return AffordanceStateShortcuts.disabledState;
			}
			return AffordanceStateShortcuts.idleState;
		}

		private IEnumerator HoveredPriorityRoutine()
		{
			do
			{
				XRBaseInteractable xrbaseInteractable = this.m_HoverInteractable as XRBaseInteractable;
				if (xrbaseInteractable != null && xrbaseInteractable.interactionManager != null && xrbaseInteractable.interactionManager.IsHighestPriorityTarget(xrbaseInteractable, null) != this.m_IsHoveredPriority)
				{
					this.m_IsHoveredPriority = !this.m_IsHoveredPriority;
					this.RefreshState();
				}
				yield return null;
			}
			while (this.m_HoveringPriorityInteractorCount > 0);
			this.m_HoveredPriorityRoutine = null;
			yield break;
		}

		protected override void BindToProviders()
		{
			base.BindToProviders();
			Object @object = this.m_Interactable as Object;
			this.m_IsBoundToInteractionEvents = (@object != null && @object != null);
			if (this.m_IsBoundToInteractionEvents)
			{
				this.m_Interactable.registered += this.OnRegistered;
				this.m_Interactable.unregistered += this.OnUnregistered;
				if (this.m_HoverInteractable != null)
				{
					this.m_HoverInteractable.firstHoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnFirstHoverEntered));
					this.m_HoverInteractable.lastHoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnLastHoverExited));
					this.m_HoverInteractable.hoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
					this.m_HoverInteractable.hoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
				}
				if (this.m_SelectInteractable != null)
				{
					this.m_SelectInteractable.firstSelectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
					this.m_SelectInteractable.lastSelectExited.AddListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
				}
				if (this.m_FocusInteractable != null)
				{
					this.m_FocusInteractable.firstFocusEntered.AddListener(new UnityAction<FocusEnterEventArgs>(this.OnFirstFocusEntered));
					this.m_FocusInteractable.lastFocusExited.AddListener(new UnityAction<FocusExitEventArgs>(this.OnLastFocusExited));
				}
				if (this.m_ActivateInteractable != null)
				{
					this.m_ActivateInteractable.activated.AddListener(new UnityAction<ActivateEventArgs>(this.OnActivatedEvent));
					this.m_ActivateInteractable.deactivated.AddListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivatedEvent));
				}
				if (this.m_InteractionStrengthInteractable != null)
				{
					base.AddBinding(this.m_InteractionStrengthInteractable.largestInteractionStrength.Subscribe(new Action<float>(this.OnLargestInteractionStrengthChanged)));
				}
				this.m_IsActivated = false;
				XRBaseInteractable xrbaseInteractable = this.m_Interactable as XRBaseInteractable;
				if (xrbaseInteractable != null)
				{
					this.m_IsRegistered = (xrbaseInteractable.interactionManager != null && xrbaseInteractable.interactionManager.IsRegistered(this.m_Interactable));
				}
				else
				{
					Behaviour behaviour = this.m_Interactable as Behaviour;
					if (behaviour != null)
					{
						this.m_IsRegistered = behaviour.isActiveAndEnabled;
					}
					else
					{
						this.m_IsRegistered = true;
					}
				}
			}
			this.RefreshState();
		}

		public void RefreshState()
		{
			AffordanceStateData newAffordanceStateData = this.GenerateNewAffordanceState();
			if (newAffordanceStateData.stateIndex != 4)
			{
				this.StopSelectedCoroutine();
			}
			if (newAffordanceStateData.stateIndex != 5)
			{
				this.StopActivatedCoroutine();
			}
			base.UpdateAffordanceState(newAffordanceStateData);
		}

		protected override void ClearBindings()
		{
			base.ClearBindings();
			if (this.m_IsBoundToInteractionEvents)
			{
				this.m_Interactable.registered -= this.OnRegistered;
				this.m_Interactable.unregistered -= this.OnUnregistered;
				if (this.m_HoverInteractable != null)
				{
					this.m_HoverInteractable.firstHoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnFirstHoverEntered));
					this.m_HoverInteractable.lastHoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnLastHoverExited));
					this.m_HoverInteractable.hoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
					this.m_HoverInteractable.hoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
				}
				if (this.m_SelectInteractable != null)
				{
					this.m_SelectInteractable.firstSelectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnFirstSelectEntered));
					this.m_SelectInteractable.lastSelectExited.RemoveListener(new UnityAction<SelectExitEventArgs>(this.OnLastSelectExited));
				}
				if (this.m_FocusInteractable != null)
				{
					this.m_FocusInteractable.firstFocusEntered.RemoveListener(new UnityAction<FocusEnterEventArgs>(this.OnFirstFocusEntered));
					this.m_FocusInteractable.lastFocusExited.RemoveListener(new UnityAction<FocusExitEventArgs>(this.OnLastFocusExited));
				}
				if (this.m_ActivateInteractable != null)
				{
					this.m_ActivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivatedEvent));
					this.m_ActivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivatedEvent));
				}
			}
			this.m_IsBoundToInteractionEvents = false;
		}

		[SerializeField]
		[RequireInterface(typeof(IXRInteractable))]
		[Tooltip("The interactable component that drives the affordance states. If null, Unity will try and find an interactable component attached.")]
		private Object m_InteractableSource;

		[Header("Event Constraints")]
		[SerializeField]
		[Tooltip("When hover events are registered and this is true, the state will fallback to idle or disabled.")]
		private bool m_IgnoreHoverEvents;

		[SerializeField]
		[Tooltip("When this is true, the state will fallback to hover if the later is not ignored. When this is false, this provider will check if the Interactable Source has priority for selection when hovered, and update its state accordingly.")]
		private bool m_IgnoreHoverPriorityEvents = true;

		[SerializeField]
		[Tooltip("When focus events are registered and this is true, the state will fallback to idle or disabled.")]
		private bool m_IgnoreFocusEvents = true;

		[SerializeField]
		[Tooltip("When select events are registered and this is true, the state will fallback to idle or disabled. Note this will not affect click animations which can be disabled separately.")]
		private bool m_IgnoreSelectEvents;

		[SerializeField]
		[Tooltip("When activate events are registered and this is true, the state will fallback to idle or disabled.Note this will not affect click animations which can be disabled separately.")]
		private bool m_IgnoreActivateEvents;

		[Header("Click Animation Config")]
		[SerializeField]
		[Tooltip("Condition to trigger click animation for Selected interaction events.")]
		private XRInteractableAffordanceStateProvider.SelectClickAnimationMode m_SelectClickAnimationMode = XRInteractableAffordanceStateProvider.SelectClickAnimationMode.SelectEntered;

		[SerializeField]
		[Tooltip("Condition to trigger click animation for activated interaction events.")]
		private XRInteractableAffordanceStateProvider.ActivateClickAnimationMode m_ActivateClickAnimationMode;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Duration of click animations for selected and activated events.")]
		private float m_ClickAnimationDuration = 0.25f;

		[SerializeField]
		[Tooltip("Animation curve reference for click animation events. Select the More menu (⋮) to choose between a direct reference and a reusable scriptable object animation curve datum.")]
		private AnimationCurveDatumProperty m_ClickAnimationCurve = new AnimationCurveDatumProperty(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

		private IXRInteractable m_Interactable;

		private IXRHoverInteractable m_HoverInteractable;

		private IXRSelectInteractable m_SelectInteractable;

		private IXRFocusInteractable m_FocusInteractable;

		private IXRActivateInteractable m_ActivateInteractable;

		private IXRInteractionStrengthInteractable m_InteractionStrengthInteractable;

		private Coroutine m_SelectedClickAnimation;

		private Coroutine m_ActivatedClickAnimation;

		private Coroutine m_HoveredPriorityRoutine;

		private bool m_IsBoundToInteractionEvents;

		private bool m_IsActivated;

		private bool m_IsRegistered;

		private bool m_IsHoveredPriority;

		private bool m_HasHoverInteractable;

		private bool m_HasSelectInteractable;

		private bool m_HasInteractionStrengthInteractable;

		private int m_HoveringPriorityInteractorCount;

		public enum SelectClickAnimationMode
		{
			None,
			SelectEntered,
			SelectExited
		}

		public enum ActivateClickAnimationMode
		{
			None,
			Activated,
			Deactivated
		}
	}
}
