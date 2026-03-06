using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Gaze;
using UnityEngine.XR.Interaction.Toolkit.Interactables.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactables
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[SelectionBase]
	[DefaultExecutionOrder(-98)]
	public abstract class XRBaseInteractable : MonoBehaviour, IXRActivateInteractable, IXRInteractable, IXRHoverInteractable, IXRSelectInteractable, IXRFocusInteractable, IXRInteractionStrengthInteractable, IXROverridesGazeAutoSelect
	{
		public event Action<InteractableRegisteredEventArgs> registered;

		public event Action<InteractableUnregisteredEventArgs> unregistered;

		public Func<IXRInteractable, Vector3, DistanceInfo> getDistanceOverride { get; set; }

		public XRInteractionManager interactionManager
		{
			get
			{
				return this.m_InteractionManager;
			}
			set
			{
				this.m_InteractionManager = value;
				if (Application.isPlaying && base.isActiveAndEnabled)
				{
					this.RegisterWithInteractionManager();
				}
			}
		}

		public List<Collider> colliders
		{
			get
			{
				return this.m_Colliders;
			}
		}

		public InteractionLayerMask interactionLayers
		{
			get
			{
				return this.m_InteractionLayers;
			}
			set
			{
				this.m_InteractionLayers = value;
			}
		}

		public XRBaseInteractable.DistanceCalculationMode distanceCalculationMode
		{
			get
			{
				return this.m_DistanceCalculationMode;
			}
			set
			{
				this.m_DistanceCalculationMode = value;
			}
		}

		public InteractableSelectMode selectMode
		{
			get
			{
				return this.m_SelectMode;
			}
			set
			{
				this.m_SelectMode = value;
			}
		}

		public InteractableFocusMode focusMode
		{
			get
			{
				return this.m_FocusMode;
			}
			set
			{
				this.m_FocusMode = value;
			}
		}

		public GameObject customReticle
		{
			get
			{
				return this.m_CustomReticle;
			}
			set
			{
				this.m_CustomReticle = value;
			}
		}

		public bool allowGazeInteraction
		{
			get
			{
				return this.m_AllowGazeInteraction;
			}
			set
			{
				this.m_AllowGazeInteraction = value;
			}
		}

		public bool allowGazeSelect
		{
			get
			{
				return this.m_AllowGazeSelect;
			}
			set
			{
				this.m_AllowGazeSelect = value;
			}
		}

		public bool overrideGazeTimeToSelect
		{
			get
			{
				return this.m_OverrideGazeTimeToSelect;
			}
			set
			{
				this.m_OverrideGazeTimeToSelect = value;
			}
		}

		public float gazeTimeToSelect
		{
			get
			{
				return this.m_GazeTimeToSelect;
			}
			set
			{
				this.m_GazeTimeToSelect = value;
			}
		}

		public bool overrideTimeToAutoDeselectGaze
		{
			get
			{
				return this.m_OverrideTimeToAutoDeselectGaze;
			}
			set
			{
				this.m_OverrideTimeToAutoDeselectGaze = value;
			}
		}

		public float timeToAutoDeselectGaze
		{
			get
			{
				return this.m_TimeToAutoDeselectGaze;
			}
			set
			{
				this.m_TimeToAutoDeselectGaze = value;
			}
		}

		public bool allowGazeAssistance
		{
			get
			{
				return this.m_AllowGazeAssistance;
			}
			set
			{
				this.m_AllowGazeAssistance = value;
			}
		}

		public HoverEnterEvent firstHoverEntered
		{
			get
			{
				return this.m_FirstHoverEntered;
			}
			set
			{
				this.m_FirstHoverEntered = value;
			}
		}

		public HoverExitEvent lastHoverExited
		{
			get
			{
				return this.m_LastHoverExited;
			}
			set
			{
				this.m_LastHoverExited = value;
			}
		}

		public HoverEnterEvent hoverEntered
		{
			get
			{
				return this.m_HoverEntered;
			}
			set
			{
				this.m_HoverEntered = value;
			}
		}

		public HoverExitEvent hoverExited
		{
			get
			{
				return this.m_HoverExited;
			}
			set
			{
				this.m_HoverExited = value;
			}
		}

		public SelectEnterEvent firstSelectEntered
		{
			get
			{
				return this.m_FirstSelectEntered;
			}
			set
			{
				this.m_FirstSelectEntered = value;
			}
		}

		public SelectExitEvent lastSelectExited
		{
			get
			{
				return this.m_LastSelectExited;
			}
			set
			{
				this.m_LastSelectExited = value;
			}
		}

		public SelectEnterEvent selectEntered
		{
			get
			{
				return this.m_SelectEntered;
			}
			set
			{
				this.m_SelectEntered = value;
			}
		}

		public SelectExitEvent selectExited
		{
			get
			{
				return this.m_SelectExited;
			}
			set
			{
				this.m_SelectExited = value;
			}
		}

		public FocusEnterEvent firstFocusEntered
		{
			get
			{
				return this.m_FirstFocusEntered;
			}
			set
			{
				this.m_FirstFocusEntered = value;
			}
		}

		public FocusExitEvent lastFocusExited
		{
			get
			{
				return this.m_LastFocusExited;
			}
			set
			{
				this.m_LastFocusExited = value;
			}
		}

		public FocusEnterEvent focusEntered
		{
			get
			{
				return this.m_FocusEntered;
			}
			set
			{
				this.m_FocusEntered = value;
			}
		}

		public FocusExitEvent focusExited
		{
			get
			{
				return this.m_FocusExited;
			}
			set
			{
				this.m_FocusExited = value;
			}
		}

		public ActivateEvent activated
		{
			get
			{
				return this.m_Activated;
			}
			set
			{
				this.m_Activated = value;
			}
		}

		public DeactivateEvent deactivated
		{
			get
			{
				return this.m_Deactivated;
			}
			set
			{
				this.m_Deactivated = value;
			}
		}

		public List<IXRHoverInteractor> interactorsHovering
		{
			get
			{
				return (List<IXRHoverInteractor>)this.m_InteractorsHovering.AsList();
			}
		}

		public bool isHovered { get; private set; }

		public List<IXRSelectInteractor> interactorsSelecting
		{
			get
			{
				return (List<IXRSelectInteractor>)this.m_InteractorsSelecting.AsList();
			}
		}

		public IXRSelectInteractor firstInteractorSelecting { get; private set; }

		public bool isSelected { get; private set; }

		public List<IXRInteractionGroup> interactionGroupsFocusing
		{
			get
			{
				return (List<IXRInteractionGroup>)this.m_InteractionGroupsFocusing.AsList();
			}
		}

		public IXRInteractionGroup firstInteractionGroupFocusing { get; private set; }

		public bool isFocused { get; private set; }

		public bool canFocus
		{
			get
			{
				return this.m_FocusMode > InteractableFocusMode.None;
			}
		}

		public List<Object> startingHoverFilters
		{
			get
			{
				return this.m_StartingHoverFilters;
			}
			set
			{
				this.m_StartingHoverFilters = value;
			}
		}

		public IXRFilterList<IXRHoverFilter> hoverFilters
		{
			get
			{
				return this.m_HoverFilters;
			}
		}

		public List<Object> startingSelectFilters
		{
			get
			{
				return this.m_StartingSelectFilters;
			}
			set
			{
				this.m_StartingSelectFilters = value;
			}
		}

		public IXRFilterList<IXRSelectFilter> selectFilters
		{
			get
			{
				return this.m_SelectFilters;
			}
		}

		public List<Object> startingInteractionStrengthFilters
		{
			get
			{
				return this.m_StartingInteractionStrengthFilters;
			}
			set
			{
				this.m_StartingInteractionStrengthFilters = value;
			}
		}

		public IXRFilterList<IXRInteractionStrengthFilter> interactionStrengthFilters
		{
			get
			{
				return this.m_InteractionStrengthFilters;
			}
		}

		public IReadOnlyBindableVariable<float> largestInteractionStrength
		{
			get
			{
				return this.m_LargestInteractionStrength;
			}
		}

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
		}

		protected virtual void Awake()
		{
			if (this.m_Colliders.Count == 0)
			{
				base.GetComponentsInChildren<Collider>(this.m_Colliders);
				this.m_Colliders.RemoveAll((Collider col) => col.isTrigger);
			}
			this.m_HoverFilters.RegisterReferences<Object>(this.m_StartingHoverFilters, this);
			this.m_SelectFilters.RegisterReferences<Object>(this.m_StartingSelectFilters, this);
			this.m_InteractionStrengthFilters.RegisterReferences<Object>(this.m_StartingInteractionStrengthFilters, this);
			this.FindCreateInteractionManager();
		}

		protected virtual void OnEnable()
		{
			this.FindCreateInteractionManager();
			this.RegisterWithInteractionManager();
		}

		protected virtual void OnDisable()
		{
			this.UnregisterWithInteractionManager();
		}

		protected virtual void OnDestroy()
		{
		}

		private void FindCreateInteractionManager()
		{
			if (this.m_InteractionManager != null)
			{
				return;
			}
			this.m_InteractionManager = ComponentLocatorUtility<XRInteractionManager>.FindOrCreateComponent();
		}

		private void RegisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager == this.m_InteractionManager)
			{
				return;
			}
			this.UnregisterWithInteractionManager();
			if (this.m_InteractionManager != null)
			{
				this.m_InteractionManager.RegisterInteractable(this);
				this.m_RegisteredInteractionManager = this.m_InteractionManager;
			}
		}

		private void UnregisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager != null)
			{
				this.m_RegisteredInteractionManager.UnregisterInteractable(this);
				this.m_RegisteredInteractionManager = null;
			}
		}

		public virtual Transform GetAttachTransform(IXRInteractor interactor)
		{
			return base.transform;
		}

		public Pose GetAttachPoseOnSelect(IXRSelectInteractor interactor)
		{
			Pose result;
			if (!this.m_AttachPoseOnSelect.TryGetValue(interactor, out result))
			{
				return Pose.identity;
			}
			return result;
		}

		public Pose GetLocalAttachPoseOnSelect(IXRSelectInteractor interactor)
		{
			Pose result;
			if (!this.m_LocalAttachPoseOnSelect.TryGetValue(interactor, out result))
			{
				return Pose.identity;
			}
			return result;
		}

		public virtual float GetDistanceSqrToInteractor(IXRInteractor interactor)
		{
			Transform transform = (interactor != null) ? interactor.GetAttachTransform(this) : null;
			if (transform == null)
			{
				return float.MaxValue;
			}
			Vector3 position = transform.position;
			return this.GetDistance(position).distanceSqr;
		}

		public virtual DistanceInfo GetDistance(Vector3 position)
		{
			if (this.getDistanceOverride != null)
			{
				return this.getDistanceOverride(this, position);
			}
			switch (this.m_DistanceCalculationMode)
			{
			default:
			{
				Vector3 position2 = base.transform.position;
				Vector3 vector = position2 - position;
				DistanceInfo result = new DistanceInfo
				{
					point = position2,
					distanceSqr = vector.sqrMagnitude
				};
				return result;
			}
			case XRBaseInteractable.DistanceCalculationMode.ColliderPosition:
			{
				DistanceInfo result;
				XRInteractableUtility.TryGetClosestCollider(this, position, out result);
				return result;
			}
			case XRBaseInteractable.DistanceCalculationMode.ColliderVolume:
			{
				DistanceInfo result;
				XRInteractableUtility.TryGetClosestPointOnCollider(this, position, out result);
				return result;
			}
			}
		}

		public float GetInteractionStrength(IXRInteractor interactor)
		{
			float result;
			if (this.m_InteractionStrengths.TryGetValue(interactor, out result))
			{
				return result;
			}
			return 0f;
		}

		public virtual bool IsHoverableBy(IXRHoverInteractor interactor)
		{
			return this.m_AllowGazeInteraction || !(interactor is XRGazeInteractor);
		}

		public virtual bool IsSelectableBy(IXRSelectInteractor interactor)
		{
			return (this.m_AllowGazeInteraction && this.m_AllowGazeSelect) || !(interactor is XRGazeInteractor);
		}

		public bool IsHovered(IXRHoverInteractor interactor)
		{
			return this.isHovered && this.m_InteractorsHovering.Contains(interactor);
		}

		public bool IsSelected(IXRSelectInteractor interactor)
		{
			return this.isSelected && this.m_InteractorsSelecting.Contains(interactor);
		}

		protected bool IsHovered(IXRInteractor interactor)
		{
			IXRHoverInteractor ixrhoverInteractor = interactor as IXRHoverInteractor;
			return ixrhoverInteractor != null && this.IsHovered(ixrhoverInteractor);
		}

		protected bool IsSelected(IXRInteractor interactor)
		{
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			return ixrselectInteractor != null && this.IsSelected(ixrselectInteractor);
		}

		public virtual GameObject GetCustomReticle(IXRInteractor interactor)
		{
			GameObject result;
			if (this.m_ReticleCache.TryGetValue(interactor, out result))
			{
				return result;
			}
			return null;
		}

		public virtual void AttachCustomReticle(IXRInteractor interactor)
		{
			Transform transform = (interactor != null) ? interactor.transform : null;
			if (transform == null)
			{
				return;
			}
			IXRCustomReticleProvider component = transform.GetComponent<IXRCustomReticleProvider>();
			if (component != null)
			{
				GameObject obj;
				if (this.m_ReticleCache.TryGetValue(interactor, out obj))
				{
					Object.Destroy(obj);
					this.m_ReticleCache.Remove(interactor);
				}
				if (this.m_CustomReticle != null)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.m_CustomReticle);
					this.m_ReticleCache.Add(interactor, gameObject);
					component.AttachCustomReticle(gameObject);
					IXRInteractableCustomReticle component2 = gameObject.GetComponent<IXRInteractableCustomReticle>();
					if (component2 != null)
					{
						component2.OnReticleAttached(this, component);
					}
				}
			}
		}

		public virtual void RemoveCustomReticle(IXRInteractor interactor)
		{
			Transform transform = (interactor != null) ? interactor.transform : null;
			if (transform == null)
			{
				return;
			}
			IXRCustomReticleProvider component = transform.GetComponent<IXRCustomReticleProvider>();
			GameObject gameObject;
			if (component != null && this.m_ReticleCache.TryGetValue(interactor, out gameObject))
			{
				IXRInteractableCustomReticle component2 = gameObject.GetComponent<IXRInteractableCustomReticle>();
				if (component2 != null)
				{
					component2.OnReticleDetaching();
				}
				Object.Destroy(gameObject);
				this.m_ReticleCache.Remove(interactor);
				component.RemoveCustomReticle();
			}
		}

		protected void CaptureAttachPose(IXRSelectInteractor interactor)
		{
			Transform attachTransform = this.GetAttachTransform(interactor);
			if (attachTransform != null)
			{
				this.m_AttachPoseOnSelect[interactor] = attachTransform.GetWorldPose();
				this.m_LocalAttachPoseOnSelect[interactor] = attachTransform.GetLocalPose();
				return;
			}
			this.m_AttachPoseOnSelect.Remove(interactor);
			this.m_LocalAttachPoseOnSelect.Remove(interactor);
		}

		public virtual void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
		}

		void IXRInteractionStrengthInteractable.ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			this.ProcessInteractionStrength(updatePhase);
		}

		void IXRInteractable.OnRegistered(InteractableRegisteredEventArgs args)
		{
			this.OnRegistered(args);
		}

		void IXRInteractable.OnUnregistered(InteractableUnregisteredEventArgs args)
		{
			this.OnUnregistered(args);
		}

		void IXRActivateInteractable.OnActivated(ActivateEventArgs args)
		{
			this.OnActivated(args);
		}

		void IXRActivateInteractable.OnDeactivated(DeactivateEventArgs args)
		{
			this.OnDeactivated(args);
		}

		bool IXRHoverInteractable.IsHoverableBy(IXRHoverInteractor interactor)
		{
			return this.IsHoverableBy(interactor) && this.ProcessHoverFilters(interactor);
		}

		void IXRHoverInteractable.OnHoverEntering(HoverEnterEventArgs args)
		{
			this.OnHoverEntering(args);
		}

		void IXRHoverInteractable.OnHoverEntered(HoverEnterEventArgs args)
		{
			this.OnHoverEntered(args);
		}

		void IXRHoverInteractable.OnHoverExiting(HoverExitEventArgs args)
		{
			this.OnHoverExiting(args);
		}

		void IXRHoverInteractable.OnHoverExited(HoverExitEventArgs args)
		{
			this.OnHoverExited(args);
		}

		bool IXRSelectInteractable.IsSelectableBy(IXRSelectInteractor interactor)
		{
			return this.IsSelectableBy(interactor) && this.ProcessSelectFilters(interactor);
		}

		void IXRSelectInteractable.OnSelectEntering(SelectEnterEventArgs args)
		{
			this.OnSelectEntering(args);
		}

		void IXRSelectInteractable.OnSelectEntered(SelectEnterEventArgs args)
		{
			this.OnSelectEntered(args);
		}

		void IXRSelectInteractable.OnSelectExiting(SelectExitEventArgs args)
		{
			this.OnSelectExiting(args);
		}

		void IXRSelectInteractable.OnSelectExited(SelectExitEventArgs args)
		{
			this.OnSelectExited(args);
		}

		void IXRFocusInteractable.OnFocusEntering(FocusEnterEventArgs args)
		{
			this.OnFocusEntering(args);
		}

		void IXRFocusInteractable.OnFocusEntered(FocusEnterEventArgs args)
		{
			this.OnFocusEntered(args);
		}

		void IXRFocusInteractable.OnFocusExiting(FocusExitEventArgs args)
		{
			this.OnFocusExiting(args);
		}

		void IXRFocusInteractable.OnFocusExited(FocusExitEventArgs args)
		{
			this.OnFocusExited(args);
		}

		protected virtual void OnRegistered(InteractableRegisteredEventArgs args)
		{
			if (args.manager != this.m_InteractionManager)
			{
				Debug.LogWarning("An Interactable was registered with an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was registered with \"{2}\".", this, this.m_InteractionManager, args.manager), this);
			}
			Action<InteractableRegisteredEventArgs> action = this.registered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void OnUnregistered(InteractableUnregisteredEventArgs args)
		{
			if (args.manager != this.m_RegisteredInteractionManager)
			{
				Debug.LogWarning("An Interactable was unregistered from an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was unregistered from \"{2}\".", this, this.m_RegisteredInteractionManager, args.manager), this);
			}
			Action<InteractableUnregisteredEventArgs> action = this.unregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void OnHoverEntering(HoverEnterEventArgs args)
		{
			if (this.m_CustomReticle != null)
			{
				this.AttachCustomReticle(args.interactorObject);
			}
			this.m_InteractorsHovering.Add(args.interactorObject);
			this.isHovered = true;
			XRBaseInputInteractor xrbaseInputInteractor = args.interactorObject as XRBaseInputInteractor;
			if (xrbaseInputInteractor != null)
			{
				this.m_VariableSelectInteractors.Add(xrbaseInputInteractor);
			}
		}

		protected virtual void OnHoverEntered(HoverEnterEventArgs args)
		{
			if (this.m_InteractorsHovering.Count == 1)
			{
				HoverEnterEvent firstHoverEntered = this.m_FirstHoverEntered;
				if (firstHoverEntered != null)
				{
					firstHoverEntered.Invoke(args);
				}
			}
			HoverEnterEvent hoverEntered = this.m_HoverEntered;
			if (hoverEntered == null)
			{
				return;
			}
			hoverEntered.Invoke(args);
		}

		protected virtual void OnHoverExiting(HoverExitEventArgs args)
		{
			if (this.m_CustomReticle != null)
			{
				this.RemoveCustomReticle(args.interactorObject);
			}
			this.m_InteractorsHovering.Remove(args.interactorObject);
			if (this.m_InteractorsHovering.Count == 0)
			{
				this.isHovered = false;
			}
			if (!this.IsSelected(args.interactorObject))
			{
				if (this.m_InteractionStrengths.Count > 0)
				{
					this.m_InteractionStrengths.Remove(args.interactorObject);
				}
				XRBaseInputInteractor xrbaseInputInteractor = args.interactorObject as XRBaseInputInteractor;
				if (xrbaseInputInteractor != null)
				{
					this.m_VariableSelectInteractors.Remove(xrbaseInputInteractor);
				}
			}
		}

		protected virtual void OnHoverExited(HoverExitEventArgs args)
		{
			if (!this.isHovered)
			{
				HoverExitEvent lastHoverExited = this.m_LastHoverExited;
				if (lastHoverExited != null)
				{
					lastHoverExited.Invoke(args);
				}
			}
			HoverExitEvent hoverExited = this.m_HoverExited;
			if (hoverExited == null)
			{
				return;
			}
			hoverExited.Invoke(args);
		}

		protected virtual void OnSelectEntering(SelectEnterEventArgs args)
		{
			this.m_InteractorsSelecting.Add(args.interactorObject);
			this.isSelected = true;
			XRBaseInputInteractor xrbaseInputInteractor = args.interactorObject as XRBaseInputInteractor;
			if (xrbaseInputInteractor != null)
			{
				this.m_VariableSelectInteractors.Add(xrbaseInputInteractor);
			}
			if (this.m_InteractorsSelecting.Count == 1)
			{
				this.firstInteractorSelecting = args.interactorObject;
			}
			this.CaptureAttachPose(args.interactorObject);
		}

		protected virtual void OnSelectEntered(SelectEnterEventArgs args)
		{
			if (this.m_InteractorsSelecting.Count == 1)
			{
				SelectEnterEvent firstSelectEntered = this.m_FirstSelectEntered;
				if (firstSelectEntered != null)
				{
					firstSelectEntered.Invoke(args);
				}
			}
			SelectEnterEvent selectEntered = this.m_SelectEntered;
			if (selectEntered == null)
			{
				return;
			}
			selectEntered.Invoke(args);
		}

		protected virtual void OnSelectExiting(SelectExitEventArgs args)
		{
			this.m_InteractorsSelecting.Remove(args.interactorObject);
			if (this.m_InteractorsSelecting.Count == 0)
			{
				this.isSelected = false;
			}
			if (!this.IsHovered(args.interactorObject))
			{
				if (this.m_InteractionStrengths.Count > 0)
				{
					this.m_InteractionStrengths.Remove(args.interactorObject);
				}
				XRBaseInputInteractor xrbaseInputInteractor = args.interactorObject as XRBaseInputInteractor;
				if (xrbaseInputInteractor != null)
				{
					this.m_VariableSelectInteractors.Remove(xrbaseInputInteractor);
				}
			}
		}

		protected virtual void OnSelectExited(SelectExitEventArgs args)
		{
			if (!this.isSelected)
			{
				SelectExitEvent lastSelectExited = this.m_LastSelectExited;
				if (lastSelectExited != null)
				{
					lastSelectExited.Invoke(args);
				}
			}
			SelectExitEvent selectExited = this.m_SelectExited;
			if (selectExited != null)
			{
				selectExited.Invoke(args);
			}
			if (!this.isSelected)
			{
				this.firstInteractorSelecting = null;
				this.m_AttachPoseOnSelect.Clear();
				this.m_LocalAttachPoseOnSelect.Clear();
			}
		}

		protected virtual void OnFocusEntering(FocusEnterEventArgs args)
		{
			this.m_InteractionGroupsFocusing.Add(args.interactionGroup);
			this.isFocused = true;
			if (this.m_InteractionGroupsFocusing.Count == 1)
			{
				this.firstInteractionGroupFocusing = args.interactionGroup;
			}
		}

		protected virtual void OnFocusEntered(FocusEnterEventArgs args)
		{
			if (this.m_InteractionGroupsFocusing.Count == 1)
			{
				FocusEnterEvent firstFocusEntered = this.m_FirstFocusEntered;
				if (firstFocusEntered != null)
				{
					firstFocusEntered.Invoke(args);
				}
			}
			FocusEnterEvent focusEntered = this.m_FocusEntered;
			if (focusEntered == null)
			{
				return;
			}
			focusEntered.Invoke(args);
		}

		protected virtual void OnFocusExiting(FocusExitEventArgs args)
		{
			this.m_InteractionGroupsFocusing.Remove(args.interactionGroup);
			if (this.m_InteractionGroupsFocusing.Count == 0)
			{
				this.isFocused = false;
			}
		}

		protected virtual void OnFocusExited(FocusExitEventArgs args)
		{
			if (!this.isFocused)
			{
				FocusExitEvent lastFocusExited = this.m_LastFocusExited;
				if (lastFocusExited != null)
				{
					lastFocusExited.Invoke(args);
				}
			}
			FocusExitEvent focusExited = this.m_FocusExited;
			if (focusExited != null)
			{
				focusExited.Invoke(args);
			}
			if (!this.isFocused)
			{
				this.firstInteractionGroupFocusing = null;
			}
		}

		protected virtual void OnActivated(ActivateEventArgs args)
		{
			ActivateEvent activated = this.m_Activated;
			if (activated == null)
			{
				return;
			}
			activated.Invoke(args);
		}

		protected virtual void OnDeactivated(DeactivateEventArgs args)
		{
			DeactivateEvent deactivated = this.m_Deactivated;
			if (deactivated == null)
			{
				return;
			}
			deactivated.Invoke(args);
		}

		protected virtual void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			float num = 0f;
			using (XRBaseInteractable.s_ProcessInteractionStrengthMarker.Auto())
			{
				if (!this.isSelected && !this.isHovered)
				{
					if (this.m_ClearedLargestInteractionStrength)
					{
						return;
					}
					this.m_LargestInteractionStrength.Value = 0f;
					this.m_ClearedLargestInteractionStrength = true;
					return;
				}
				else
				{
					this.m_ClearedLargestInteractionStrength = false;
					bool flag = this.m_InteractionStrengthFilters.registeredSnapshot.Count > 0;
					if (this.isSelected)
					{
						int i = 0;
						int count = this.m_InteractorsSelecting.Count;
						while (i < count)
						{
							IXRSelectInteractor ixrselectInteractor = this.m_InteractorsSelecting[i];
							if (!(ixrselectInteractor is XRBaseInputInteractor))
							{
								float num2 = flag ? this.ProcessInteractionStrengthFilters(ixrselectInteractor, 1f) : 1f;
								this.m_InteractionStrengths[ixrselectInteractor] = num2;
								num = Mathf.Max(num, num2);
							}
							i++;
						}
					}
					if (this.isHovered)
					{
						int j = 0;
						int count2 = this.m_InteractorsHovering.Count;
						while (j < count2)
						{
							IXRHoverInteractor ixrhoverInteractor = this.m_InteractorsHovering[j];
							if (!(ixrhoverInteractor is XRBaseInputInteractor) && !this.IsSelected(ixrhoverInteractor))
							{
								float num3 = flag ? this.ProcessInteractionStrengthFilters(ixrhoverInteractor, 0f) : 0f;
								this.m_InteractionStrengths[ixrhoverInteractor] = num3;
								num = Mathf.Max(num, num3);
							}
							j++;
						}
					}
					int k = 0;
					int count3 = this.m_VariableSelectInteractors.Count;
					while (k < count3)
					{
						XRBaseInputInteractor xrbaseInputInteractor = this.m_VariableSelectInteractors[k];
						float num4 = flag ? this.ProcessInteractionStrengthFilters(xrbaseInputInteractor, this.ReadInteractionStrength(xrbaseInputInteractor)) : this.ReadInteractionStrength(xrbaseInputInteractor);
						this.m_InteractionStrengths[xrbaseInputInteractor] = num4;
						num = Mathf.Max(num, num4);
						k++;
					}
				}
			}
			using (XRBaseInteractable.s_ProcessInteractionStrengthEventMarker.Auto())
			{
				this.m_LargestInteractionStrength.Value = num;
			}
		}

		private float ReadInteractionStrength(XRBaseInputInteractor interactor)
		{
			if (!interactor.forceDeprecatedInput)
			{
				return interactor.selectInput.ReadValue();
			}
			if (interactor.xrController != null)
			{
				return interactor.xrController.selectInteractionState.value;
			}
			if (!this.IsSelected(interactor))
			{
				return 0f;
			}
			return 1f;
		}

		protected bool ProcessHoverFilters(IXRHoverInteractor interactor)
		{
			return XRFilterUtility.Process(this.m_HoverFilters, interactor, this);
		}

		protected bool ProcessSelectFilters(IXRSelectInteractor interactor)
		{
			return XRFilterUtility.Process(this.m_SelectFilters, interactor, this);
		}

		protected float ProcessInteractionStrengthFilters(IXRInteractor interactor, float interactionStrength)
		{
			return XRFilterUtility.Process(this.m_InteractionStrengthFilters, interactor, this, interactionStrength);
		}

		[Obsolete("interactionLayerMask has been deprecated. Use interactionLayers instead.", true)]
		public LayerMask interactionLayerMask
		{
			get
			{
				Debug.LogError("interactionLayerMask has been deprecated. Use interactionLayers instead.", this);
				throw new NotSupportedException("interactionLayerMask has been deprecated. Use interactionLayers instead.");
			}
			set
			{
				Debug.LogError("interactionLayerMask has been deprecated. Use interactionLayers instead.", this);
				throw new NotSupportedException("interactionLayerMask has been deprecated. Use interactionLayers instead.");
			}
		}

		[Obsolete("onFirstHoverEntered has been deprecated. Use firstHoverEntered with updated signature instead.", true)]
		public XRInteractableEvent onFirstHoverEntered
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onLastHoverExited has been deprecated. Use lastHoverExited with updated signature instead.", true)]
		public XRInteractableEvent onLastHoverExited
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.", true)]
		public XRInteractableEvent onHoverEntered
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onHoverExited has been deprecated. Use hoverExited with updated signature instead.", true)]
		public XRInteractableEvent onHoverExited
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onSelectEntered has been deprecated. Use selectEntered with updated signature instead.", true)]
		public XRInteractableEvent onSelectEntered
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature and check for !args.isCanceled instead.", true)]
		public XRInteractableEvent onSelectExited
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onSelectCanceled has been deprecated. Use selectExited with updated signature and check for args.isCanceled instead.", true)]
		public XRInteractableEvent onSelectCanceled
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onActivate has been deprecated. Use activated with updated signature instead.", true)]
		public XRInteractableEvent onActivate
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onDeactivate has been deprecated. Use deactivated with updated signature instead.", true)]
		public XRInteractableEvent onDeactivate
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onFirstHoverEnter has been deprecated. Use onFirstHoverEntered instead. (UnityUpgradable) -> onFirstHoverEntered", true)]
		public XRInteractableEvent onFirstHoverEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered", true)]
		public XRInteractableEvent onHoverEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited", true)]
		public XRInteractableEvent onHoverExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onLastHoverExit has been deprecated. Use onLastHoverExited instead. (UnityUpgradable) -> onLastHoverExited", true)]
		public XRInteractableEvent onLastHoverExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered", true)]
		public XRInteractableEvent onSelectEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited", true)]
		public XRInteractableEvent onSelectExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onSelectCancel has been deprecated. Use onSelectCanceled instead. (UnityUpgradable) -> onSelectCanceled", true)]
		public XRInteractableEvent onSelectCancel
		{
			get
			{
				return null;
			}
		}

		[Obsolete("OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.", true)]
		protected virtual void OnHoverEntering(XRBaseInteractor interactor)
		{
			Debug.LogError("OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.");
		}

		[Obsolete("OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.", true)]
		protected virtual void OnHoverEntered(XRBaseInteractor interactor)
		{
			Debug.LogError("OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.");
		}

		[Obsolete("OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.", true)]
		protected virtual void OnHoverExiting(XRBaseInteractor interactor)
		{
			Debug.LogError("OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.");
		}

		[Obsolete("OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.", true)]
		protected virtual void OnHoverExited(XRBaseInteractor interactor)
		{
			Debug.LogError("OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.");
		}

		[Obsolete("OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.", true)]
		protected virtual void OnSelectEntering(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.");
		}

		[Obsolete("OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.", true)]
		protected virtual void OnSelectEntered(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.");
		}

		[Obsolete("OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.", true)]
		protected virtual void OnSelectExiting(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.", this);
			throw new NotSupportedException("OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.");
		}

		[Obsolete("OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.", true)]
		protected virtual void OnSelectExited(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.", this);
			throw new NotSupportedException("OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.");
		}

		[Obsolete("OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.", true)]
		protected virtual void OnSelectCanceling(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.", this);
			throw new NotSupportedException("OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.");
		}

		[Obsolete("OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.", true)]
		protected virtual void OnSelectCanceled(XRBaseInteractor interactor)
		{
			Debug.LogError("OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.", this);
			throw new NotSupportedException("OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.");
		}

		[Obsolete("OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.", true)]
		protected virtual void OnActivate(XRBaseInteractor interactor)
		{
			Debug.LogError("OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.", this);
			throw new NotSupportedException("OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.");
		}

		[Obsolete("OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.", true)]
		protected virtual void OnDeactivate(XRBaseInteractor interactor)
		{
			Debug.LogError("OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.", this);
			throw new NotSupportedException("OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.");
		}

		[Obsolete("GetDistanceSqrToInteractor(XRBaseInteractor) has been deprecated. Use GetDistanceSqrToInteractor(IXRInteractor) instead.", true)]
		public virtual float GetDistanceSqrToInteractor(XRBaseInteractor interactor)
		{
			Debug.LogError("GetDistanceSqrToInteractor(XRBaseInteractor) has been deprecated. Use GetDistanceSqrToInteractor(IXRInteractor) instead.", this);
			throw new NotSupportedException("GetDistanceSqrToInteractor(XRBaseInteractor) has been deprecated. Use GetDistanceSqrToInteractor(IXRInteractor) instead.");
		}

		[Obsolete("AttachCustomReticle(XRBaseInteractor) has been deprecated. Use AttachCustomReticle(IXRInteractor) instead.", true)]
		public virtual void AttachCustomReticle(XRBaseInteractor interactor)
		{
			Debug.LogError("AttachCustomReticle(XRBaseInteractor) has been deprecated. Use AttachCustomReticle(IXRInteractor) instead.", this);
			throw new NotSupportedException("AttachCustomReticle(XRBaseInteractor) has been deprecated. Use AttachCustomReticle(IXRInteractor) instead.");
		}

		[Obsolete("RemoveCustomReticle(XRBaseInteractor) has been deprecated. Use RemoveCustomReticle(IXRInteractor) instead.", true)]
		public virtual void RemoveCustomReticle(XRBaseInteractor interactor)
		{
			Debug.LogError("RemoveCustomReticle(XRBaseInteractor) has been deprecated. Use RemoveCustomReticle(IXRInteractor) instead.", this);
			throw new NotSupportedException("RemoveCustomReticle(XRBaseInteractor) has been deprecated. Use RemoveCustomReticle(IXRInteractor) instead.");
		}

		[Obsolete("hoveringInteractors has been deprecated. Use interactorsHovering instead.", true)]
		public List<XRBaseInteractor> hoveringInteractors
		{
			get
			{
				Debug.LogError("hoveringInteractors has been deprecated. Use interactorsHovering instead.", this);
				throw new NotSupportedException("hoveringInteractors has been deprecated. Use interactorsHovering instead.");
			}
		}

		[Obsolete("selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.", true)]
		public XRBaseInteractor selectingInteractor
		{
			get
			{
				Debug.LogError("selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.", this);
				throw new NotSupportedException("selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.");
			}
			protected set
			{
				Debug.LogError("selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.", this);
				throw new NotSupportedException("selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.");
			}
		}

		[Obsolete("IsHoverableBy(XRBaseInteractor) has been deprecated. Use IsHoverableBy(IXRHoverInteractor) instead.", true)]
		public virtual bool IsHoverableBy(XRBaseInteractor interactor)
		{
			Debug.LogError("IsHoverableBy(XRBaseInteractor) has been deprecated. Use IsHoverableBy(IXRHoverInteractor) instead.", this);
			throw new NotSupportedException("IsHoverableBy(XRBaseInteractor) has been deprecated. Use IsHoverableBy(IXRHoverInteractor) instead.");
		}

		[Obsolete("IsSelectableBy(XRBaseInteractor) has been deprecated. Use IsSelectableBy(IXRSelectInteractor) instead.", true)]
		public virtual bool IsSelectableBy(XRBaseInteractor interactor)
		{
			Debug.LogError("IsSelectableBy(XRBaseInteractor) has been deprecated. Use IsSelectableBy(IXRSelectInteractor) instead.", this);
			throw new NotSupportedException("IsSelectableBy(XRBaseInteractor) has been deprecated. Use IsSelectableBy(IXRSelectInteractor) instead.");
		}

		Transform IXRInteractable.get_transform()
		{
			return base.transform;
		}

		private const float k_InteractionStrengthHover = 0f;

		private const float k_InteractionStrengthSelect = 1f;

		[SerializeField]
		private XRInteractionManager m_InteractionManager;

		[SerializeField]
		private List<Collider> m_Colliders = new List<Collider>();

		[SerializeField]
		private InteractionLayerMask m_InteractionLayers = 1;

		[SerializeField]
		private XRBaseInteractable.DistanceCalculationMode m_DistanceCalculationMode = XRBaseInteractable.DistanceCalculationMode.ColliderPosition;

		[SerializeField]
		private InteractableSelectMode m_SelectMode;

		[SerializeField]
		private InteractableFocusMode m_FocusMode = InteractableFocusMode.Single;

		[SerializeField]
		private GameObject m_CustomReticle;

		[SerializeField]
		private bool m_AllowGazeInteraction;

		[SerializeField]
		private bool m_AllowGazeSelect;

		[SerializeField]
		private bool m_OverrideGazeTimeToSelect;

		[SerializeField]
		private float m_GazeTimeToSelect = 0.5f;

		[SerializeField]
		private bool m_OverrideTimeToAutoDeselectGaze;

		[SerializeField]
		private float m_TimeToAutoDeselectGaze = 3f;

		[SerializeField]
		private bool m_AllowGazeAssistance;

		[SerializeField]
		private HoverEnterEvent m_FirstHoverEntered = new HoverEnterEvent();

		[SerializeField]
		private HoverExitEvent m_LastHoverExited = new HoverExitEvent();

		[SerializeField]
		private HoverEnterEvent m_HoverEntered = new HoverEnterEvent();

		[SerializeField]
		private HoverExitEvent m_HoverExited = new HoverExitEvent();

		[SerializeField]
		private SelectEnterEvent m_FirstSelectEntered = new SelectEnterEvent();

		[SerializeField]
		private SelectExitEvent m_LastSelectExited = new SelectExitEvent();

		[SerializeField]
		private SelectEnterEvent m_SelectEntered = new SelectEnterEvent();

		[SerializeField]
		private SelectExitEvent m_SelectExited = new SelectExitEvent();

		[SerializeField]
		private FocusEnterEvent m_FirstFocusEntered = new FocusEnterEvent();

		[SerializeField]
		private FocusExitEvent m_LastFocusExited = new FocusExitEvent();

		[SerializeField]
		private FocusEnterEvent m_FocusEntered = new FocusEnterEvent();

		[SerializeField]
		private FocusExitEvent m_FocusExited = new FocusExitEvent();

		[SerializeField]
		private ActivateEvent m_Activated = new ActivateEvent();

		[SerializeField]
		private DeactivateEvent m_Deactivated = new DeactivateEvent();

		private readonly HashSetList<IXRHoverInteractor> m_InteractorsHovering = new HashSetList<IXRHoverInteractor>(0);

		private readonly HashSetList<IXRSelectInteractor> m_InteractorsSelecting = new HashSetList<IXRSelectInteractor>(0);

		private readonly HashSetList<IXRInteractionGroup> m_InteractionGroupsFocusing = new HashSetList<IXRInteractionGroup>(0);

		[SerializeField]
		[RequireInterface(typeof(IXRHoverFilter))]
		private List<Object> m_StartingHoverFilters = new List<Object>();

		private readonly ExposedRegistrationList<IXRHoverFilter> m_HoverFilters = new ExposedRegistrationList<IXRHoverFilter>
		{
			bufferChanges = false
		};

		[SerializeField]
		[RequireInterface(typeof(IXRSelectFilter))]
		private List<Object> m_StartingSelectFilters = new List<Object>();

		private readonly ExposedRegistrationList<IXRSelectFilter> m_SelectFilters = new ExposedRegistrationList<IXRSelectFilter>
		{
			bufferChanges = false
		};

		[SerializeField]
		[RequireInterface(typeof(IXRInteractionStrengthFilter))]
		private List<Object> m_StartingInteractionStrengthFilters = new List<Object>();

		private readonly ExposedRegistrationList<IXRInteractionStrengthFilter> m_InteractionStrengthFilters = new ExposedRegistrationList<IXRInteractionStrengthFilter>
		{
			bufferChanges = false
		};

		private readonly BindableVariable<float> m_LargestInteractionStrength = new BindableVariable<float>(0f, true, null, false);

		private bool m_ClearedLargestInteractionStrength;

		private readonly Dictionary<IXRSelectInteractor, Pose> m_AttachPoseOnSelect = new Dictionary<IXRSelectInteractor, Pose>();

		private readonly Dictionary<IXRSelectInteractor, Pose> m_LocalAttachPoseOnSelect = new Dictionary<IXRSelectInteractor, Pose>();

		private readonly Dictionary<IXRInteractor, GameObject> m_ReticleCache = new Dictionary<IXRInteractor, GameObject>();

		private readonly HashSetList<XRBaseInputInteractor> m_VariableSelectInteractors = new HashSetList<XRBaseInputInteractor>(0);

		private readonly Dictionary<IXRInteractor, float> m_InteractionStrengths = new Dictionary<IXRInteractor, float>();

		private XRInteractionManager m_RegisteredInteractionManager;

		private static readonly ProfilerMarker s_ProcessInteractionStrengthMarker = new ProfilerMarker("XRI.ProcessInteractionStrength.Interactables");

		private static readonly ProfilerMarker s_ProcessInteractionStrengthEventMarker = new ProfilerMarker("XRI.ProcessInteractionStrength.InteractablesEvent");

		private const string k_InteractionLayerMaskDeprecated = "interactionLayerMask has been deprecated. Use interactionLayers instead.";

		private const string k_OnHoverEnteringDeprecated = "OnHoverEntering(XRBaseInteractor) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.";

		private const string k_OnHoverEnteredDeprecated = "OnHoverEntered(XRBaseInteractor) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.";

		private const string k_OnHoverExitingDeprecated = "OnHoverExiting(XRBaseInteractor) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.";

		private const string k_OnHoverExitedDeprecated = "OnHoverExited(XRBaseInteractor) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.";

		private const string k_OnSelectEnteringDeprecated = "OnSelectEntering(XRBaseInteractor) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.";

		private const string k_OnSelectEnteredDeprecated = "OnSelectEntered(XRBaseInteractor) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.";

		private const string k_OnSelectExitingDeprecated = "OnSelectExiting(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for !args.isCanceled instead.";

		private const string k_OnSelectExitedDeprecated = "OnSelectExited(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for !args.isCanceled instead.";

		private const string k_OnSelectCancelingDeprecated = "OnSelectCanceling(XRBaseInteractor) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) and check for args.isCanceled instead.";

		private const string k_OnSelectCanceledDeprecated = "OnSelectCanceled(XRBaseInteractor) has been deprecated. Use OnSelectExited(SelectExitEventArgs) and check for args.isCanceled instead.";

		private const string k_OnActivateDeprecated = "OnActivate(XRBaseInteractor) has been deprecated. Use OnActivated(ActivateEventArgs) instead.";

		private const string k_OnDeactivateDeprecated = "OnDeactivate(XRBaseInteractor) has been deprecated. Use OnDeactivated(DeactivateEventArgs) instead.";

		private const string k_GetDistanceSqrToInteractorDeprecated = "GetDistanceSqrToInteractor(XRBaseInteractor) has been deprecated. Use GetDistanceSqrToInteractor(IXRInteractor) instead.";

		private const string k_AttachCustomReticleDeprecated = "AttachCustomReticle(XRBaseInteractor) has been deprecated. Use AttachCustomReticle(IXRInteractor) instead.";

		private const string k_RemoveCustomReticleDeprecated = "RemoveCustomReticle(XRBaseInteractor) has been deprecated. Use RemoveCustomReticle(IXRInteractor) instead.";

		private const string k_HoveringInteractorsDeprecated = "hoveringInteractors has been deprecated. Use interactorsHovering instead.";

		private const string k_SelectingInteractorDeprecated = "selectingInteractor has been deprecated. Use interactorsSelecting, GetOldestInteractorSelecting, or isSelected for similar functionality.";

		private const string k_IsHoverableByDeprecated = "IsHoverableBy(XRBaseInteractor) has been deprecated. Use IsHoverableBy(IXRHoverInteractor) instead.";

		private const string k_IsSelectableByDeprecated = "IsSelectableBy(XRBaseInteractor) has been deprecated. Use IsSelectableBy(IXRSelectInteractor) instead.";

		public enum MovementType
		{
			VelocityTracking,
			Kinematic,
			Instantaneous
		}

		public enum DistanceCalculationMode
		{
			TransformPosition,
			ColliderPosition,
			ColliderVolume
		}
	}
}
