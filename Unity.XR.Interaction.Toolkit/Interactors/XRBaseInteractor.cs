using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Bindings.Variables;
using Unity.XR.CoreUtils.Collections;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[SelectionBase]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-99)]
	public abstract class XRBaseInteractor : MonoBehaviour, IXRHoverInteractor, IXRInteractor, IXRSelectInteractor, IXRTargetPriorityInteractor, IXRGroupMember, IXRInteractionStrengthInteractor
	{
		public event Action<InteractorRegisteredEventArgs> registered;

		public event Action<InteractorUnregisteredEventArgs> unregistered;

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

		public IXRInteractionGroup containingGroup { get; private set; }

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

		public InteractorHandedness handedness
		{
			get
			{
				return this.m_Handedness;
			}
			set
			{
				this.m_Handedness = value;
			}
		}

		public Transform attachTransform
		{
			get
			{
				return this.m_AttachTransform;
			}
			set
			{
				this.m_AttachTransform = value;
			}
		}

		public bool keepSelectedTargetValid
		{
			get
			{
				return this.m_KeepSelectedTargetValid;
			}
			set
			{
				this.m_KeepSelectedTargetValid = value;
			}
		}

		public bool disableVisualsWhenBlockedInGroup
		{
			get
			{
				return this.m_DisableVisualsWhenBlockedInGroup;
			}
			set
			{
				this.m_DisableVisualsWhenBlockedInGroup = value;
			}
		}

		public XRBaseInteractable startingSelectedInteractable
		{
			get
			{
				return this.m_StartingSelectedInteractable;
			}
			set
			{
				this.m_StartingSelectedInteractable = value;
			}
		}

		public XRBaseTargetFilter startingTargetFilter
		{
			get
			{
				return this.m_StartingTargetFilter;
			}
			set
			{
				this.m_StartingTargetFilter = value;
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

		public IXRTargetFilter targetFilter
		{
			get
			{
				Object @object = this.m_TargetFilter as Object;
				if (@object != null && @object == null)
				{
					return null;
				}
				return this.m_TargetFilter;
			}
			set
			{
				if (!Application.isPlaying)
				{
					this.m_TargetFilter = value;
					return;
				}
				IXRTargetFilter targetFilter = this.targetFilter;
				if (targetFilter != null)
				{
					targetFilter.Unlink(this);
				}
				this.m_TargetFilter = value;
				IXRTargetFilter targetFilter2 = this.targetFilter;
				if (targetFilter2 == null)
				{
					return;
				}
				targetFilter2.Link(this);
			}
		}

		public bool allowHover
		{
			get
			{
				return this.m_AllowHover;
			}
			set
			{
				this.m_AllowHover = value;
			}
		}

		public bool allowSelect
		{
			get
			{
				return this.m_AllowSelect;
			}
			set
			{
				this.m_AllowSelect = value;
			}
		}

		public bool isPerformingManualInteraction
		{
			get
			{
				return this.m_IsPerformingManualInteraction;
			}
		}

		public List<IXRHoverInteractable> interactablesHovered
		{
			get
			{
				return (List<IXRHoverInteractable>)this.m_InteractablesHovered.AsList();
			}
		}

		public bool hasHover { get; private set; }

		public List<IXRSelectInteractable> interactablesSelected
		{
			get
			{
				return (List<IXRSelectInteractable>)this.m_InteractablesSelected.AsList();
			}
		}

		public IXRSelectInteractable firstInteractableSelected { get; private set; }

		public bool hasSelection { get; private set; }

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

		public IReadOnlyBindableVariable<float> largestInteractionStrength
		{
			get
			{
				return this.m_LargestInteractionStrength;
			}
		}

		private protected bool TryGetXROrigin(out Transform origin)
		{
			if (this.m_HasXROrigin)
			{
				origin = this.m_XROriginTransform;
				return true;
			}
			if (!this.m_FailedToFindXROrigin)
			{
				XROrigin componentInParent = base.GetComponentInParent<XROrigin>();
				if (componentInParent != null)
				{
					GameObject origin2 = componentInParent.Origin;
					if (origin2 != null)
					{
						this.m_XROriginTransform = origin2.transform;
						this.m_HasXROrigin = true;
						origin = this.m_XROriginTransform;
						return true;
					}
				}
				this.m_FailedToFindXROrigin = true;
			}
			origin = null;
			return false;
		}

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
		}

		protected virtual void Awake()
		{
			this.CreateAttachTransform();
			if (this.m_StartingTargetFilter != null)
			{
				this.targetFilter = this.m_StartingTargetFilter;
			}
			this.m_HoverFilters.RegisterReferences<Object>(this.m_StartingHoverFilters, this);
			this.m_SelectFilters.RegisterReferences<Object>(this.m_StartingSelectFilters, this);
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

		protected virtual void Start()
		{
			if (this.m_InteractionManager != null && this.m_StartingSelectedInteractable != null)
			{
				this.m_InteractionManager.SelectEnter(this, this.m_StartingSelectedInteractable);
			}
		}

		protected virtual void OnDestroy()
		{
			IXRTargetFilter targetFilter = this.targetFilter;
			if (targetFilter != null)
			{
				targetFilter.Unlink(this);
			}
			if (this.containingGroup != null)
			{
				Object @object = this.containingGroup as Object;
				if (@object == null || @object != null)
				{
					this.containingGroup.RemoveGroupMember(this);
				}
			}
		}

		public virtual Transform GetAttachTransform(IXRInteractable interactable)
		{
			if (!(this.m_AttachTransform != null))
			{
				return base.transform;
			}
			return this.m_AttachTransform;
		}

		public Pose GetAttachPoseOnSelect(IXRSelectInteractable interactable)
		{
			Pose result;
			if (!this.m_AttachPoseOnSelect.TryGetValue(interactable, out result))
			{
				return Pose.identity;
			}
			return result;
		}

		public Pose GetLocalAttachPoseOnSelect(IXRSelectInteractable interactable)
		{
			Pose result;
			if (!this.m_LocalAttachPoseOnSelect.TryGetValue(interactable, out result))
			{
				return Pose.identity;
			}
			return result;
		}

		public virtual void GetValidTargets(List<IXRInteractable> targets)
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
				this.m_InteractionManager.RegisterInteractor(this);
				this.m_RegisteredInteractionManager = this.m_InteractionManager;
			}
		}

		private void UnregisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager != null)
			{
				this.m_RegisteredInteractionManager.UnregisterInteractor(this);
				this.m_RegisteredInteractionManager = null;
			}
		}

		public virtual bool isHoverActive
		{
			get
			{
				return this.m_AllowHover;
			}
		}

		public virtual bool isSelectActive
		{
			get
			{
				return this.m_AllowSelect;
			}
		}

		public virtual TargetPriorityMode targetPriorityMode { get; set; }

		public virtual List<IXRSelectInteractable> targetsForSelection { get; set; }

		public virtual bool CanHover(IXRHoverInteractable interactable)
		{
			return true;
		}

		public virtual bool CanSelect(IXRSelectInteractable interactable)
		{
			return true;
		}

		public bool IsHovering(IXRHoverInteractable interactable)
		{
			return this.hasHover && this.m_InteractablesHovered.Contains(interactable);
		}

		public bool IsSelecting(IXRSelectInteractable interactable)
		{
			return this.hasSelection && this.m_InteractablesSelected.Contains(interactable);
		}

		protected bool IsHovering(IXRInteractable interactable)
		{
			IXRHoverInteractable ixrhoverInteractable = interactable as IXRHoverInteractable;
			return ixrhoverInteractable != null && this.IsHovering(ixrhoverInteractable);
		}

		protected bool IsSelecting(IXRInteractable interactable)
		{
			IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
			return ixrselectInteractable != null && this.IsSelecting(ixrselectInteractable);
		}

		public virtual XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
		{
			get
			{
				return null;
			}
		}

		protected void CaptureAttachPose(IXRSelectInteractable interactable)
		{
			Transform attachTransform = this.GetAttachTransform(interactable);
			if (attachTransform != null)
			{
				this.m_AttachPoseOnSelect[interactable] = attachTransform.GetWorldPose();
				this.m_LocalAttachPoseOnSelect[interactable] = attachTransform.GetLocalPose();
				return;
			}
			this.m_AttachPoseOnSelect.Remove(interactable);
			this.m_LocalAttachPoseOnSelect.Remove(interactable);
		}

		protected void CreateAttachTransform()
		{
			if (this.m_AttachTransform == null)
			{
				this.m_AttachTransform = new GameObject("[" + base.gameObject.name + "] Attach").transform;
				this.m_AttachTransform.SetParent(base.transform, false);
				this.m_AttachTransform.SetLocalPose(Pose.identity);
			}
		}

		public virtual void PreprocessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
		}

		public virtual void ProcessInteractor(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
		}

		public float GetInteractionStrength(IXRInteractable interactable)
		{
			float result;
			if (this.m_InteractionStrengths.TryGetValue(interactable, out result))
			{
				return result;
			}
			return 0f;
		}

		void IXRInteractionStrengthInteractor.ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			this.ProcessInteractionStrength(updatePhase);
		}

		void IXRInteractor.OnRegistered(InteractorRegisteredEventArgs args)
		{
			this.OnRegistered(args);
		}

		void IXRInteractor.OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			this.OnUnregistered(args);
		}

		bool IXRHoverInteractor.CanHover(IXRHoverInteractable interactable)
		{
			return this.CanHover(interactable) && this.ProcessHoverFilters(interactable);
		}

		void IXRHoverInteractor.OnHoverEntering(HoverEnterEventArgs args)
		{
			this.OnHoverEntering(args);
		}

		void IXRHoverInteractor.OnHoverEntered(HoverEnterEventArgs args)
		{
			this.OnHoverEntered(args);
		}

		void IXRHoverInteractor.OnHoverExiting(HoverExitEventArgs args)
		{
			this.OnHoverExiting(args);
		}

		void IXRHoverInteractor.OnHoverExited(HoverExitEventArgs args)
		{
			this.OnHoverExited(args);
		}

		bool IXRSelectInteractor.CanSelect(IXRSelectInteractable interactable)
		{
			return this.CanSelect(interactable) && this.ProcessSelectFilters(interactable);
		}

		void IXRSelectInteractor.OnSelectEntering(SelectEnterEventArgs args)
		{
			this.OnSelectEntering(args);
		}

		void IXRSelectInteractor.OnSelectEntered(SelectEnterEventArgs args)
		{
			this.OnSelectEntered(args);
		}

		void IXRSelectInteractor.OnSelectExiting(SelectExitEventArgs args)
		{
			this.OnSelectExiting(args);
		}

		void IXRSelectInteractor.OnSelectExited(SelectExitEventArgs args)
		{
			this.OnSelectExited(args);
		}

		protected virtual void OnRegistered(InteractorRegisteredEventArgs args)
		{
			if (args.manager != this.m_InteractionManager)
			{
				Debug.LogWarning("An Interactor was registered with an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was registered with \"{2}\".", this, this.m_InteractionManager, args.manager), this);
			}
			Action<InteractorRegisteredEventArgs> action = this.registered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			if (args.manager != this.m_RegisteredInteractionManager)
			{
				Debug.LogWarning("An Interactor was unregistered from an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was unregistered from \"{2}\".", this, this.m_RegisteredInteractionManager, args.manager), this);
			}
			Action<InteractorUnregisteredEventArgs> action = this.unregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void OnHoverEntering(HoverEnterEventArgs args)
		{
			this.m_InteractablesHovered.Add(args.interactableObject);
			this.hasHover = true;
			IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = args.interactableObject as IXRInteractionStrengthInteractable;
			if (ixrinteractionStrengthInteractable != null)
			{
				this.m_InteractionStrengthInteractables.Add(ixrinteractionStrengthInteractable);
			}
		}

		protected virtual void OnHoverEntered(HoverEnterEventArgs args)
		{
			HoverEnterEvent hoverEntered = this.m_HoverEntered;
			if (hoverEntered == null)
			{
				return;
			}
			hoverEntered.Invoke(args);
		}

		protected virtual void OnHoverExiting(HoverExitEventArgs args)
		{
			this.m_InteractablesHovered.Remove(args.interactableObject);
			if (this.m_InteractablesHovered.Count == 0)
			{
				this.hasHover = false;
			}
			if (!this.IsSelecting(args.interactableObject))
			{
				if (this.m_InteractionStrengths.Count > 0)
				{
					this.m_InteractionStrengths.Remove(args.interactableObject);
				}
				IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = args.interactableObject as IXRInteractionStrengthInteractable;
				if (ixrinteractionStrengthInteractable != null)
				{
					this.m_InteractionStrengthInteractables.Remove(ixrinteractionStrengthInteractable);
				}
			}
		}

		protected virtual void OnHoverExited(HoverExitEventArgs args)
		{
			HoverExitEvent hoverExited = this.m_HoverExited;
			if (hoverExited == null)
			{
				return;
			}
			hoverExited.Invoke(args);
		}

		protected virtual void OnSelectEntering(SelectEnterEventArgs args)
		{
			this.m_InteractablesSelected.Add(args.interactableObject);
			this.hasSelection = true;
			IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = args.interactableObject as IXRInteractionStrengthInteractable;
			if (ixrinteractionStrengthInteractable != null)
			{
				this.m_InteractionStrengthInteractables.Add(ixrinteractionStrengthInteractable);
			}
			if (this.m_InteractablesSelected.Count == 1)
			{
				this.firstInteractableSelected = args.interactableObject;
			}
			this.CaptureAttachPose(args.interactableObject);
		}

		protected virtual void OnSelectEntered(SelectEnterEventArgs args)
		{
			SelectEnterEvent selectEntered = this.m_SelectEntered;
			if (selectEntered == null)
			{
				return;
			}
			selectEntered.Invoke(args);
		}

		protected virtual void OnSelectExiting(SelectExitEventArgs args)
		{
			this.m_InteractablesSelected.Remove(args.interactableObject);
			if (this.m_InteractablesSelected.Count == 0)
			{
				this.hasSelection = false;
			}
			if (!this.IsHovering(args.interactableObject))
			{
				if (this.m_InteractionStrengths.Count > 0)
				{
					this.m_InteractionStrengths.Remove(args.interactableObject);
				}
				IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = args.interactableObject as IXRInteractionStrengthInteractable;
				if (ixrinteractionStrengthInteractable != null)
				{
					this.m_InteractionStrengthInteractables.Remove(ixrinteractionStrengthInteractable);
				}
			}
		}

		protected virtual void OnSelectExited(SelectExitEventArgs args)
		{
			SelectExitEvent selectExited = this.m_SelectExited;
			if (selectExited != null)
			{
				selectExited.Invoke(args);
			}
			if (!this.hasSelection)
			{
				this.firstInteractableSelected = null;
				this.m_AttachPoseOnSelect.Clear();
				this.m_LocalAttachPoseOnSelect.Clear();
			}
		}

		protected virtual void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			float num = 0f;
			using (XRBaseInteractor.s_ProcessInteractionStrengthMarker.Auto())
			{
				if (!this.hasSelection && !this.hasHover)
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
					if (this.hasSelection)
					{
						int i = 0;
						int count = this.m_InteractablesSelected.Count;
						while (i < count)
						{
							IXRSelectInteractable ixrselectInteractable = this.m_InteractablesSelected[i];
							if (!(ixrselectInteractable is IXRInteractionStrengthInteractable))
							{
								this.m_InteractionStrengths[ixrselectInteractable] = 1f;
								num = 1f;
							}
							i++;
						}
					}
					if (this.hasHover)
					{
						int j = 0;
						int count2 = this.m_InteractablesHovered.Count;
						while (j < count2)
						{
							IXRHoverInteractable ixrhoverInteractable = this.m_InteractablesHovered[j];
							if (!(ixrhoverInteractable is IXRInteractionStrengthInteractable) && !this.IsSelecting(ixrhoverInteractable))
							{
								this.m_InteractionStrengths[ixrhoverInteractable] = 0f;
							}
							j++;
						}
					}
					int k = 0;
					int count3 = this.m_InteractionStrengthInteractables.Count;
					while (k < count3)
					{
						IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = this.m_InteractionStrengthInteractables[k];
						float interactionStrength = ixrinteractionStrengthInteractable.GetInteractionStrength(this);
						this.m_InteractionStrengths[ixrinteractionStrengthInteractable] = interactionStrength;
						num = Mathf.Max(num, interactionStrength);
						k++;
					}
				}
			}
			using (XRBaseInteractor.s_ProcessInteractionStrengthEventMarker.Auto())
			{
				this.m_LargestInteractionStrength.Value = num;
			}
		}

		public virtual void StartManualInteraction(IXRSelectInteractable interactable)
		{
			if (this.interactionManager == null)
			{
				Debug.LogWarning("Cannot start manual interaction without an Interaction Manager set.", this);
				return;
			}
			this.interactionManager.SelectEnter(this, interactable);
			this.m_IsPerformingManualInteraction = true;
			this.m_ManualInteractionInteractable = interactable;
		}

		public virtual void EndManualInteraction()
		{
			if (this.interactionManager == null)
			{
				Debug.LogWarning("Cannot end manual interaction without an Interaction Manager set.", this);
				return;
			}
			if (!this.m_IsPerformingManualInteraction)
			{
				Debug.LogWarning("Tried to end manual interaction but was not performing manual interaction. Ignoring request.", this);
				return;
			}
			this.interactionManager.SelectExit(this, this.m_ManualInteractionInteractable);
			this.m_IsPerformingManualInteraction = false;
			this.m_ManualInteractionInteractable = null;
		}

		protected bool ProcessHoverFilters(IXRHoverInteractable interactable)
		{
			return XRFilterUtility.Process(this.m_HoverFilters, this, interactable);
		}

		protected bool ProcessSelectFilters(IXRSelectInteractable interactable)
		{
			return XRFilterUtility.Process(this.m_SelectFilters, this, interactable);
		}

		void IXRGroupMember.OnRegisteringAsGroupMember(IXRInteractionGroup group)
		{
			if (this.containingGroup != null)
			{
				Debug.LogError(base.name + " is already part of a Group. Remove the member from the Group first.", this);
				return;
			}
			if (!group.ContainsGroupMember(this))
			{
				Debug.LogError("OnRegisteringAsGroupMember was called but the Group does not contain " + base.name + ". Add the member to the Group rather than calling this method directly.", this);
				return;
			}
			this.containingGroup = group;
		}

		void IXRGroupMember.OnRegisteringAsNonGroupMember()
		{
			this.containingGroup = null;
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

		[Obsolete("enableInteractions has been deprecated. Use allowHover and allowSelect instead.", true)]
		public bool enableInteractions
		{
			get
			{
				Debug.LogError("enableInteractions has been deprecated. Use allowHover and allowSelect instead.", this);
				throw new NotSupportedException("enableInteractions has been deprecated. Use allowHover and allowSelect instead.");
			}
			set
			{
				Debug.LogError("enableInteractions has been deprecated. Use allowHover and allowSelect instead.", this);
				throw new NotSupportedException("enableInteractions has been deprecated. Use allowHover and allowSelect instead.");
			}
		}

		[Obsolete("onHoverEntered has been deprecated. Use hoverEntered with updated signature instead.", true)]
		public XRInteractorEvent onHoverEntered
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
		public XRInteractorEvent onHoverExited
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
		public XRInteractorEvent onSelectEntered
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onSelectExited has been deprecated. Use selectExited with updated signature instead.", true)]
		public XRInteractorEvent onSelectExited
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		[Obsolete("onHoverEnter has been deprecated. Use onHoverEntered instead. (UnityUpgradable) -> onHoverEntered", true)]
		public XRInteractorEvent onHoverEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onHoverExit has been deprecated. Use onHoverExited instead. (UnityUpgradable) -> onHoverExited", true)]
		public XRInteractorEvent onHoverExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onSelectEnter has been deprecated. Use onSelectEntered instead. (UnityUpgradable) -> onSelectEntered", true)]
		public XRInteractorEvent onSelectEnter
		{
			get
			{
				return null;
			}
		}

		[Obsolete("onSelectExit has been deprecated. Use onSelectExited instead. (UnityUpgradable) -> onSelectExited", true)]
		public XRInteractorEvent onSelectExit
		{
			get
			{
				return null;
			}
		}

		[Obsolete("OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.", true)]
		protected virtual void OnHoverEntering(XRBaseInteractable interactable)
		{
			Debug.LogError("OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.");
		}

		[Obsolete("OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.", true)]
		protected virtual void OnHoverEntered(XRBaseInteractable interactable)
		{
			Debug.LogError("OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.");
		}

		[Obsolete("OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.", true)]
		protected virtual void OnHoverExiting(XRBaseInteractable interactable)
		{
			Debug.LogError("OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.");
		}

		[Obsolete("OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.", true)]
		protected virtual void OnHoverExited(XRBaseInteractable interactable)
		{
			Debug.LogError("OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.", this);
			throw new NotSupportedException("OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.");
		}

		[Obsolete("OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.", true)]
		protected virtual void OnSelectEntering(XRBaseInteractable interactable)
		{
			Debug.LogError("OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.");
		}

		[Obsolete("OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.", true)]
		protected virtual void OnSelectEntered(XRBaseInteractable interactable)
		{
			Debug.LogError("OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.");
		}

		[Obsolete("OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.", true)]
		protected virtual void OnSelectExiting(XRBaseInteractable interactable)
		{
			Debug.LogError("OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.");
		}

		[Obsolete("OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.", true)]
		protected virtual void OnSelectExited(XRBaseInteractable interactable)
		{
			Debug.LogError("OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.", this);
			throw new NotSupportedException("OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.");
		}

		[Obsolete("selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.", true)]
		public XRBaseInteractable selectTarget
		{
			get
			{
				Debug.LogError("selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.", this);
				throw new NotSupportedException("selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.");
			}
			protected set
			{
				Debug.LogError("selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.", this);
				throw new NotSupportedException("selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.");
			}
		}

		[Obsolete("hoverTargets has been deprecated. Use interactablesHovered instead.", true)]
		protected List<XRBaseInteractable> hoverTargets
		{
			get
			{
				Debug.LogError("hoverTargets has been deprecated. Use interactablesHovered instead.", this);
				throw new NotSupportedException("hoverTargets has been deprecated. Use interactablesHovered instead.");
			}
		}

		[Obsolete("GetHoverTargets has been deprecated. Use interactablesHovered instead.", true)]
		public void GetHoverTargets(List<XRBaseInteractable> targets)
		{
			Debug.LogError("GetHoverTargets has been deprecated. Use interactablesHovered instead.", this);
			throw new NotSupportedException("GetHoverTargets has been deprecated. Use interactablesHovered instead.");
		}

		[Obsolete("GetValidTargets(List<XRBaseInteractable>) has been deprecated. Override GetValidTargets(List<IXRInteractable>) instead.", true)]
		public virtual void GetValidTargets(List<XRBaseInteractable> targets)
		{
			Debug.LogError("GetValidTargets(List<XRBaseInteractable>) has been deprecated. Override GetValidTargets(List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("GetValidTargets(List<XRBaseInteractable>) has been deprecated. Override GetValidTargets(List<IXRInteractable>) instead.");
		}

		[Obsolete("CanHover(XRBaseInteractable) has been deprecated. Use CanHover(IXRHoverInteractable) instead.", true)]
		public virtual bool CanHover(XRBaseInteractable interactable)
		{
			Debug.LogError("CanHover(XRBaseInteractable) has been deprecated. Use CanHover(IXRHoverInteractable) instead.", this);
			throw new NotSupportedException("CanHover(XRBaseInteractable) has been deprecated. Use CanHover(IXRHoverInteractable) instead.");
		}

		[Obsolete("CanSelect(XRBaseInteractable) has been deprecated. Use CanSelect(IXRSelectInteractable) instead.", true)]
		public virtual bool CanSelect(XRBaseInteractable interactable)
		{
			Debug.LogError("CanSelect(XRBaseInteractable) has been deprecated. Use CanSelect(IXRSelectInteractable) instead.", this);
			throw new NotSupportedException("CanSelect(XRBaseInteractable) has been deprecated. Use CanSelect(IXRSelectInteractable) instead.");
		}

		[Obsolete("requireSelectExclusive has been deprecated. Put logic in CanSelect instead.", true)]
		public virtual bool requireSelectExclusive
		{
			get
			{
				Debug.LogError("requireSelectExclusive has been deprecated. Put logic in CanSelect instead.", this);
				throw new NotSupportedException("requireSelectExclusive has been deprecated. Put logic in CanSelect instead.");
			}
		}

		[Obsolete("StartManualInteraction(XRBaseInteractable) has been deprecated. Use StartManualInteraction(IXRSelectInteractable) instead.", true)]
		public virtual void StartManualInteraction(XRBaseInteractable interactable)
		{
			Debug.LogError("StartManualInteraction(XRBaseInteractable) has been deprecated. Use StartManualInteraction(IXRSelectInteractable) instead.", this);
			throw new NotSupportedException("StartManualInteraction(XRBaseInteractable) has been deprecated. Use StartManualInteraction(IXRSelectInteractable) instead.");
		}

		Transform IXRInteractor.get_transform()
		{
			return base.transform;
		}

		private const float k_InteractionStrengthHover = 0f;

		private const float k_InteractionStrengthSelect = 1f;

		[SerializeField]
		private XRInteractionManager m_InteractionManager;

		[SerializeField]
		private InteractionLayerMask m_InteractionLayers = -1;

		[SerializeField]
		private InteractorHandedness m_Handedness;

		[SerializeField]
		private Transform m_AttachTransform;

		[SerializeField]
		private bool m_KeepSelectedTargetValid = true;

		[SerializeField]
		private bool m_DisableVisualsWhenBlockedInGroup = true;

		[SerializeField]
		private XRBaseInteractable m_StartingSelectedInteractable;

		[SerializeField]
		private XRBaseTargetFilter m_StartingTargetFilter;

		[SerializeField]
		private HoverEnterEvent m_HoverEntered = new HoverEnterEvent();

		[SerializeField]
		private HoverExitEvent m_HoverExited = new HoverExitEvent();

		[SerializeField]
		private SelectEnterEvent m_SelectEntered = new SelectEnterEvent();

		[SerializeField]
		private SelectExitEvent m_SelectExited = new SelectExitEvent();

		private IXRTargetFilter m_TargetFilter;

		private bool m_AllowHover = true;

		private bool m_AllowSelect = true;

		private bool m_IsPerformingManualInteraction;

		private readonly HashSetList<IXRHoverInteractable> m_InteractablesHovered = new HashSetList<IXRHoverInteractable>(0);

		private readonly HashSetList<IXRSelectInteractable> m_InteractablesSelected = new HashSetList<IXRSelectInteractable>(0);

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

		private readonly BindableVariable<float> m_LargestInteractionStrength = new BindableVariable<float>(0f, true, null, false);

		private bool m_ClearedLargestInteractionStrength;

		private readonly Dictionary<IXRSelectInteractable, Pose> m_AttachPoseOnSelect = new Dictionary<IXRSelectInteractable, Pose>();

		private readonly Dictionary<IXRSelectInteractable, Pose> m_LocalAttachPoseOnSelect = new Dictionary<IXRSelectInteractable, Pose>();

		private readonly HashSetList<IXRInteractionStrengthInteractable> m_InteractionStrengthInteractables = new HashSetList<IXRInteractionStrengthInteractable>(0);

		private readonly Dictionary<IXRInteractable, float> m_InteractionStrengths = new Dictionary<IXRInteractable, float>();

		private IXRSelectInteractable m_ManualInteractionInteractable;

		private XRInteractionManager m_RegisteredInteractionManager;

		private static readonly ProfilerMarker s_ProcessInteractionStrengthMarker = new ProfilerMarker("XRI.ProcessInteractionStrength.Interactors");

		private static readonly ProfilerMarker s_ProcessInteractionStrengthEventMarker = new ProfilerMarker("XRI.ProcessInteractionStrength.InteractorsEvent");

		private Transform m_XROriginTransform;

		private bool m_HasXROrigin;

		private bool m_FailedToFindXROrigin;

		private const string k_InteractionLayerMaskDeprecated = "interactionLayerMask has been deprecated. Use interactionLayers instead.";

		private const string k_EnableInteractionsDeprecated = "enableInteractions has been deprecated. Use allowHover and allowSelect instead.";

		private const string k_OnHoverEnteringDeprecated = "OnHoverEntering(XRBaseInteractable) has been deprecated. Use OnHoverEntering(HoverEnterEventArgs) instead.";

		private const string k_OnHoverEnteredDeprecated = "OnHoverEntered(XRBaseInteractable) has been deprecated. Use OnHoverEntered(HoverEnterEventArgs) instead.";

		private const string k_OnHoverExitingDeprecated = "OnHoverExiting(XRBaseInteractable) has been deprecated. Use OnHoverExiting(HoverExitEventArgs) instead.";

		private const string k_OnHoverExitedDeprecated = "OnHoverExited(XRBaseInteractable) has been deprecated. Use OnHoverExited(HoverExitEventArgs) instead.";

		private const string k_OnSelectEnteringDeprecated = "OnSelectEntering(XRBaseInteractable) has been deprecated. Use OnSelectEntering(SelectEnterEventArgs) instead.";

		private const string k_OnSelectEnteredDeprecated = "OnSelectEntered(XRBaseInteractable) has been deprecated. Use OnSelectEntered(SelectEnterEventArgs) instead.";

		private const string k_OnSelectExitingDeprecated = "OnSelectExiting(XRBaseInteractable) has been deprecated. Use OnSelectExiting(SelectExitEventArgs) instead.";

		private const string k_OnSelectExitedDeprecated = "OnSelectExited(XRBaseInteractable) has been deprecated. Use OnSelectExited(SelectExitEventArgs) instead.";

		private const string k_SelectTargetDeprecated = "selectTarget has been deprecated. Use interactablesSelected, GetOldestInteractableSelected, hasSelection, or IsSelecting for similar functionality.";

		private const string k_HoverTargetsDeprecated = "hoverTargets has been deprecated. Use interactablesHovered instead.";

		private const string k_GetHoverTargetsDeprecated = "GetHoverTargets has been deprecated. Use interactablesHovered instead.";

		private const string k_GetValidTargetsDeprecated = "GetValidTargets(List<XRBaseInteractable>) has been deprecated. Override GetValidTargets(List<IXRInteractable>) instead.";

		private const string k_CanHoverDeprecated = "CanHover(XRBaseInteractable) has been deprecated. Use CanHover(IXRHoverInteractable) instead.";

		private const string k_CanSelectDeprecated = "CanSelect(XRBaseInteractable) has been deprecated. Use CanSelect(IXRSelectInteractable) instead.";

		private const string k_RequireSelectExclusiveDeprecated = "requireSelectExclusive has been deprecated. Put logic in CanSelect instead.";

		private const string k_StartManualInteractionDeprecated = "StartManualInteraction(XRBaseInteractable) has been deprecated. Use StartManualInteraction(IXRSelectInteractable) instead.";
	}
}
