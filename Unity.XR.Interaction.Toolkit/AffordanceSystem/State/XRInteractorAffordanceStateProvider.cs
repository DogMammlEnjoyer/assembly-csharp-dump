using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Datums;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State
{
	[AddComponentMenu("Affordance System/XR Interactor Affordance State Provider", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State.XRInteractorAffordanceStateProvider.html")]
	[DisallowMultipleComponent]
	[Obsolete("The Affordance System namespace and all associated classes have been deprecated. The existing affordance system will be moved, replaced and updated with a new interaction feedback system in a future version of XRI.")]
	public class XRInteractorAffordanceStateProvider : BaseAffordanceStateProvider
	{
		public Object interactorSource
		{
			get
			{
				return this.m_InteractorSource;
			}
			set
			{
				this.m_InteractorSource = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.SetBoundInteractionReceiver(value as IXRInteractor);
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

		public bool ignoreUGUIHover
		{
			get
			{
				return this.m_IgnoreUGUIHover;
			}
			set
			{
				this.m_IgnoreUGUIHover = value;
			}
		}

		public bool ignoreUGUISelect
		{
			get
			{
				return this.m_IgnoreUGUISelect;
			}
			set
			{
				this.m_IgnoreUGUISelect = value;
			}
		}

		public bool ignoreXRInteractionEvents
		{
			get
			{
				return this.m_IgnoreXRInteractionEvents;
			}
			set
			{
				this.m_IgnoreXRInteractionEvents = value;
			}
		}

		protected virtual bool hasXRHover
		{
			get
			{
				return !this.m_IgnoreXRInteractionEvents && this.m_HasHoverInteractor && this.m_HoverInteractor.hasHover;
			}
		}

		protected virtual bool hasUIHover
		{
			get
			{
				return !this.m_IgnoreUGUIHover && this.m_UIHovering;
			}
		}

		protected virtual bool hasXRSelection
		{
			get
			{
				return !this.m_IgnoreXRInteractionEvents && this.m_HasSelectInteractor && this.m_SelectInteractor.hasSelection;
			}
		}

		protected virtual bool hasUISelection
		{
			get
			{
				return !this.m_IgnoreUGUISelect && this.m_UISelecting;
			}
		}

		protected virtual bool isActivated
		{
			get
			{
				return !this.m_IgnoreXRInteractionEvents && this.m_IsActivated;
			}
		}

		protected virtual bool isRegistered
		{
			get
			{
				return this.m_IsRegistered;
			}
		}

		protected virtual bool isBlockedByGroup
		{
			get
			{
				return this.m_IsIXRInteractor && !this.m_Interactor.IsBlockedByInteractionWithinGroup();
			}
		}

		public XRInteractorAffordanceStateProvider.SelectClickAnimationMode selectClickAnimationMode
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

		public XRInteractorAffordanceStateProvider.ActivateClickAnimationMode activateClickAnimationMode
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

		protected void Awake()
		{
			IXRInteractor ixrinteractor2;
			if (this.m_InteractorSource != null)
			{
				IXRInteractor ixrinteractor = this.m_InteractorSource as IXRInteractor;
				if (ixrinteractor != null)
				{
					ixrinteractor2 = ixrinteractor;
					goto IL_26;
				}
			}
			ixrinteractor2 = base.GetComponentInParent<IXRInteractor>();
			IL_26:
			IXRInteractor boundInteractionReceiver = ixrinteractor2;
			if (!this.SetBoundInteractionReceiver(boundInteractionReceiver))
			{
				XRLoggingUtils.LogWarning(string.Format("Could not find required interactor component on {0}", base.gameObject) + " for which to provide affordance states.", this);
				base.enabled = false;
			}
		}

		public bool SetBoundInteractionReceiver(IXRInteractor interactor)
		{
			this.ClearBindings();
			Object @object = interactor as Object;
			bool flag = @object != null && @object != null;
			if (flag)
			{
				this.m_Interactor = interactor;
				IXRHoverInteractor ixrhoverInteractor = this.m_Interactor as IXRHoverInteractor;
				if (ixrhoverInteractor != null)
				{
					this.m_HoverInteractor = ixrhoverInteractor;
				}
				IXRSelectInteractor ixrselectInteractor = this.m_Interactor as IXRSelectInteractor;
				if (ixrselectInteractor != null)
				{
					this.m_SelectInteractor = ixrselectInteractor;
				}
				IXRInteractionStrengthInteractor ixrinteractionStrengthInteractor = this.m_Interactor as IXRInteractionStrengthInteractor;
				if (ixrinteractionStrengthInteractor != null)
				{
					this.m_InteractionStrengthInteractor = ixrinteractionStrengthInteractor;
				}
				XRRayInteractor xrrayInteractor = this.m_Interactor as XRRayInteractor;
				if (xrrayInteractor != null)
				{
					this.m_RayInteractor = xrrayInteractor;
				}
				ICurveInteractionDataProvider curveInteractionDataProvider = this.m_Interactor as ICurveInteractionDataProvider;
				if (curveInteractionDataProvider != null)
				{
					this.m_CurveInteractionDataProvider = curveInteractionDataProvider;
				}
			}
			else
			{
				this.m_Interactor = null;
				this.m_HoverInteractor = null;
				this.m_SelectInteractor = null;
				this.m_InteractionStrengthInteractor = null;
				this.m_RayInteractor = null;
				this.m_CurveInteractionDataProvider = null;
			}
			this.m_IsIXRInteractor = (this.m_Interactor != null);
			this.m_HasHoverInteractor = (this.m_HoverInteractor != null);
			this.m_HasSelectInteractor = (this.m_SelectInteractor != null);
			this.m_HasInteractionStrengthInteractor = (this.m_InteractionStrengthInteractor != null);
			this.m_HasRayInteractor = (this.m_RayInteractor != null);
			this.m_HasCurveInteractionDataProvider = (this.m_CurveInteractionDataProvider != null);
			this.BindToProviders();
			return flag;
		}

		protected override void BindToProviders()
		{
			base.BindToProviders();
			Object @object = this.m_Interactor as Object;
			this.m_IsBoundToInteractionEvents = (@object != null && @object != null);
			if (this.m_IsBoundToInteractionEvents)
			{
				this.m_Interactor.registered += this.OnRegistered;
				this.m_Interactor.unregistered += this.OnUnregistered;
				if (this.m_HasHoverInteractor)
				{
					this.m_HoverInteractor.hoverEntered.AddListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
					this.m_HoverInteractor.hoverExited.AddListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
				}
				if (this.m_HasSelectInteractor)
				{
					this.m_SelectInteractor.selectEntered.AddListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
					this.m_SelectInteractor.selectExited.AddListener(new UnityAction<SelectExitEventArgs>(this.OnSelectExited));
				}
				if (this.m_HasInteractionStrengthInteractor)
				{
					base.AddBinding(this.m_InteractionStrengthInteractor.largestInteractionStrength.Subscribe(new Action<float>(this.OnLargestInteractionStrengthChanged)));
				}
				this.m_IsActivated = false;
				XRBaseInteractor xrbaseInteractor = this.m_Interactor as XRBaseInteractor;
				if (xrbaseInteractor != null)
				{
					this.m_IsRegistered = (xrbaseInteractor.interactionManager != null && xrbaseInteractor.interactionManager.IsRegistered(this.m_Interactor));
				}
				else
				{
					Behaviour behaviour = this.m_Interactor as Behaviour;
					if (behaviour != null)
					{
						this.m_IsRegistered = behaviour.isActiveAndEnabled;
					}
					else
					{
						this.m_IsRegistered = true;
					}
				}
				if (this.m_UGUIUpdateCoroutine != null)
				{
					base.StopCoroutine(this.m_UGUIUpdateCoroutine);
				}
				this.m_UGUIUpdateCoroutine = base.StartCoroutine(this.UIUpdateCheckCoroutine());
			}
			this.RefreshState();
		}

		public void RefreshState()
		{
			base.UpdateAffordanceState(this.GenerateNewAffordanceState());
		}

		protected override void ClearBindings()
		{
			base.ClearBindings();
			if (this.m_IsBoundToInteractionEvents)
			{
				this.m_Interactor.registered -= this.OnRegistered;
				this.m_Interactor.unregistered -= this.OnUnregistered;
				if (this.m_HasHoverInteractor)
				{
					this.m_HoverInteractor.hoverEntered.RemoveListener(new UnityAction<HoverEnterEventArgs>(this.OnHoverEntered));
					this.m_HoverInteractor.hoverExited.RemoveListener(new UnityAction<HoverExitEventArgs>(this.OnHoverExited));
				}
				if (this.m_HasSelectInteractor)
				{
					this.m_SelectInteractor.selectEntered.RemoveListener(new UnityAction<SelectEnterEventArgs>(this.OnSelectEntered));
					this.m_SelectInteractor.selectExited.RemoveListener(new UnityAction<SelectExitEventArgs>(this.OnSelectExited));
				}
			}
			foreach (IXRActivateInteractable ixractivateInteractable in this.m_BoundActivateInteractable)
			{
				if (ixractivateInteractable != null)
				{
					ixractivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
				}
			}
			this.m_BoundActivateInteractable.Clear();
			this.m_IsBoundToInteractionEvents = false;
			if (this.m_UGUIUpdateCoroutine != null)
			{
				base.StopCoroutine(this.m_UGUIUpdateCoroutine);
				this.m_UGUIUpdateCoroutine = null;
			}
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
			if ((this.hasXRSelection || this.hasUISelection) && !this.m_IgnoreSelectEvents)
			{
				float transitionAmount = this.m_HasInteractionStrengthInteractor ? this.m_InteractionStrengthInteractor.largestInteractionStrength.Value : 1f;
				return new AffordanceStateData(4, transitionAmount);
			}
			if ((this.hasXRHover || this.hasUIHover) && !this.m_IgnoreHoverEvents)
			{
				float transitionAmount2 = this.m_HasInteractionStrengthInteractor ? this.m_InteractionStrengthInteractor.largestInteractionStrength.Value : 0f;
				return new AffordanceStateData(2, transitionAmount2);
			}
			if (!this.isRegistered || this.isBlockedByGroup)
			{
				return AffordanceStateShortcuts.disabledState;
			}
			return AffordanceStateShortcuts.idleState;
		}

		protected virtual void OnRegistered(InteractorRegisteredEventArgs args)
		{
			this.m_IsRegistered = true;
			this.RefreshState();
		}

		protected virtual void OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			this.m_IsRegistered = false;
			this.RefreshState();
		}

		protected virtual void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (!this.m_IgnoreActivateEvents)
			{
				IXRActivateInteractable ixractivateInteractable = args.interactableObject as IXRActivateInteractable;
				if (ixractivateInteractable != null && !this.m_BoundActivateInteractable.Contains(ixractivateInteractable))
				{
					this.m_BoundActivateInteractable.Add(ixractivateInteractable);
					ixractivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
					ixractivateInteractable.activated.AddListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.AddListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
				}
			}
			this.RefreshState();
		}

		protected virtual void OnHoverExited(HoverExitEventArgs args)
		{
			IXRActivateInteractable ixractivateInteractable = args.interactableObject as IXRActivateInteractable;
			if (ixractivateInteractable != null && this.m_BoundActivateInteractable.Contains(ixractivateInteractable))
			{
				this.m_BoundActivateInteractable.Remove(ixractivateInteractable);
				ixractivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
				ixractivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
			}
			this.RefreshState();
		}

		protected virtual void OnSelectEntered(SelectEnterEventArgs args)
		{
			if (!this.m_IgnoreActivateEvents)
			{
				IXRActivateInteractable ixractivateInteractable = args.interactableObject as IXRActivateInteractable;
				if (ixractivateInteractable != null && !this.m_BoundActivateInteractable.Contains(ixractivateInteractable))
				{
					this.m_BoundActivateInteractable.Add(ixractivateInteractable);
					ixractivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
					ixractivateInteractable.activated.AddListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.AddListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
				}
			}
			if (this.m_IgnoreSelectEvents || this.m_IgnoreXRInteractionEvents || this.m_SelectClickAnimationMode != XRInteractorAffordanceStateProvider.SelectClickAnimationMode.SelectEntered || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.SelectedClickBehavior();
		}

		protected virtual void OnSelectExited(SelectExitEventArgs args)
		{
			if (!this.hasXRHover)
			{
				IXRActivateInteractable ixractivateInteractable = args.interactableObject as IXRActivateInteractable;
				if (ixractivateInteractable != null && this.m_BoundActivateInteractable.Contains(ixractivateInteractable))
				{
					this.m_BoundActivateInteractable.Remove(ixractivateInteractable);
					ixractivateInteractable.activated.RemoveListener(new UnityAction<ActivateEventArgs>(this.OnActivated));
					ixractivateInteractable.deactivated.RemoveListener(new UnityAction<DeactivateEventArgs>(this.OnDeactivated));
				}
			}
			if (this.m_IgnoreSelectEvents || this.m_IgnoreXRInteractionEvents || this.m_SelectClickAnimationMode != XRInteractorAffordanceStateProvider.SelectClickAnimationMode.SelectExited || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.SelectedClickBehavior();
		}

		protected virtual void OnLargestInteractionStrengthChanged(float value)
		{
			if (this.m_SelectedClickAnimation != null || this.m_ActivatedClickAnimation != null)
			{
				return;
			}
			this.RefreshState();
		}

		private void OnActivated(ActivateEventArgs args)
		{
			this.m_IsActivated = true;
			if (this.m_IgnoreActivateEvents || this.m_IgnoreXRInteractionEvents || this.m_ActivateClickAnimationMode != XRInteractorAffordanceStateProvider.ActivateClickAnimationMode.Activated || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.ActivatedClickBehavior();
		}

		private void OnDeactivated(DeactivateEventArgs args)
		{
			this.m_IsActivated = false;
			if (this.m_IgnoreActivateEvents || this.m_IgnoreXRInteractionEvents || this.m_ActivateClickAnimationMode != XRInteractorAffordanceStateProvider.ActivateClickAnimationMode.Deactivated || this.m_ClickAnimationDuration < Mathf.Epsilon)
			{
				this.RefreshState();
				return;
			}
			this.ActivatedClickBehavior();
		}

		protected virtual void SelectedClickBehavior()
		{
			if (this.m_SelectedClickAnimation != null)
			{
				base.StopCoroutine(this.m_SelectedClickAnimation);
			}
			this.m_SelectedClickAnimation = base.StartCoroutine(this.ClickAnimation(4, this.m_ClickAnimationDuration, delegate
			{
				this.m_SelectedClickAnimation = null;
			}));
		}

		protected virtual void ActivatedClickBehavior()
		{
			if (this.m_ActivatedClickAnimation != null)
			{
				base.StopCoroutine(this.m_ActivatedClickAnimation);
			}
			this.m_ActivatedClickAnimation = base.StartCoroutine(this.ClickAnimation(5, this.m_ClickAnimationDuration, delegate
			{
				this.m_ActivatedClickAnimation = null;
			}));
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

		private IEnumerator UIUpdateCheckCoroutine()
		{
			for (;;)
			{
				yield return null;
				if (this.m_HasCurveInteractionDataProvider || this.m_HasRayInteractor)
				{
					bool flag = false;
					bool flag2 = false;
					Vector3 vector;
					RaycastResult raycastResult;
					int num;
					if ((!this.m_IgnoreHoverEvents || !this.m_IgnoreSelectEvents) && (this.m_HasCurveInteractionDataProvider ? (this.m_CurveInteractionDataProvider.TryGetCurveEndPoint(out vector, false, false) == EndPointType.UI) : (this.m_RayInteractor.TryGetCurrentUIRaycastResult(out raycastResult, out num) && num != 0)))
					{
						if (!this.m_IgnoreSelectEvents && !this.m_IgnoreUGUISelect)
						{
							TrackedDeviceModel trackedDeviceModel;
							flag2 = (this.m_HasCurveInteractionDataProvider ? this.m_CurveInteractionDataProvider.hasValidSelect : (this.m_RayInteractor.TryGetUIModel(out trackedDeviceModel) && trackedDeviceModel.select));
						}
						if (!this.m_IgnoreHoverEvents && !this.m_IgnoreUGUIHover)
						{
							flag = true;
						}
					}
					bool flag3 = flag != this.m_UIHovering || flag2 != this.m_UISelecting;
					this.m_UIHovering = flag;
					this.m_UISelecting = flag2;
					if (flag3)
					{
						this.RefreshState();
					}
				}
			}
			yield break;
		}

		[SerializeField]
		[RequireInterface(typeof(IXRInteractor))]
		[Tooltip("The interactor component that drives the affordance states. If null, Unity will try and find an interactor component attached.")]
		private Object m_InteractorSource;

		[Header("Event Constraints")]
		[SerializeField]
		[Tooltip("When hover events are registered and this is true, the state will fallback to idle or disabled.")]
		private bool m_IgnoreHoverEvents;

		[SerializeField]
		[Tooltip("When select events are registered and this is true, the state will fallback to idle or disabled. \nNote: Click animations must be disabled separately.")]
		private bool m_IgnoreSelectEvents;

		[SerializeField]
		[Tooltip("When activate events are registered and this is true, the state will fallback to idle or disabled.\nNote: Click animations must be disabled separately.")]
		private bool m_IgnoreActivateEvents = true;

		[SerializeField]
		[Tooltip("With the XR Ray Interactor it is possible to trigger select events from the ray interactor overlapping with a canvas.")]
		private bool m_IgnoreUGUIHover;

		[SerializeField]
		[Tooltip("With the XR Ray Interactor it is possible to trigger select events from the ray interactor overlapping with a canvas and triggering the select input.")]
		private bool m_IgnoreUGUISelect;

		[SerializeField]
		[Tooltip("This option will prevent Hover, Select, and Activate events from being triggered when they come from the XR Interaction Manager. UGUI hover and select events will still come through.")]
		private bool m_IgnoreXRInteractionEvents;

		[Header("Click Animation Config")]
		[SerializeField]
		[Tooltip("Condition to trigger click animation for Selected interaction events.")]
		private XRInteractorAffordanceStateProvider.SelectClickAnimationMode m_SelectClickAnimationMode = XRInteractorAffordanceStateProvider.SelectClickAnimationMode.SelectEntered;

		[SerializeField]
		[Tooltip("Condition to trigger click animation for activated interaction events.")]
		private XRInteractorAffordanceStateProvider.ActivateClickAnimationMode m_ActivateClickAnimationMode = XRInteractorAffordanceStateProvider.ActivateClickAnimationMode.Activated;

		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("Duration of click animations for selected and activated events.")]
		private float m_ClickAnimationDuration = 0.25f;

		[SerializeField]
		[Tooltip("Animation curve reference for click animation events. Select the More menu (⋮) to choose between a direct reference and a reusable asset.")]
		private AnimationCurveDatumProperty m_ClickAnimationCurve = new AnimationCurveDatumProperty(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

		private IXRInteractor m_Interactor;

		private IXRHoverInteractor m_HoverInteractor;

		private IXRSelectInteractor m_SelectInteractor;

		private IXRInteractionStrengthInteractor m_InteractionStrengthInteractor;

		private XRRayInteractor m_RayInteractor;

		private ICurveInteractionDataProvider m_CurveInteractionDataProvider;

		private bool m_IsBoundToInteractionEvents;

		private bool m_HasRayInteractor;

		private bool m_HasCurveInteractionDataProvider;

		private bool m_HasHoverInteractor;

		private bool m_HasSelectInteractor;

		private bool m_HasInteractionStrengthInteractor;

		private bool m_IsIXRInteractor;

		private Coroutine m_SelectedClickAnimation;

		private Coroutine m_ActivatedClickAnimation;

		private bool m_IsActivated;

		private bool m_IsRegistered;

		private readonly HashSet<IXRActivateInteractable> m_BoundActivateInteractable = new HashSet<IXRActivateInteractable>();

		private bool m_UIHovering;

		private bool m_UISelecting;

		private Coroutine m_UGUIUpdateCoroutine;

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
