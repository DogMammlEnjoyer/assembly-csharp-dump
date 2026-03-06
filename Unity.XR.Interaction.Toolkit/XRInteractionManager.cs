using System;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit
{
	[AddComponentMenu("XR/XR Interaction Manager", 11)]
	[DisallowMultipleComponent]
	[DefaultExecutionOrder(-105)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.XRInteractionManager.html")]
	public class XRInteractionManager : MonoBehaviour
	{
		public event Action<InteractionGroupRegisteredEventArgs> interactionGroupRegistered;

		public event Action<InteractionGroupUnregisteredEventArgs> interactionGroupUnregistered;

		public event Action<InteractorRegisteredEventArgs> interactorRegistered;

		public event Action<InteractorUnregisteredEventArgs> interactorUnregistered;

		public event Action<InteractableRegisteredEventArgs> interactableRegistered;

		public event Action<InteractableUnregisteredEventArgs> interactableUnregistered;

		public event Action<FocusEnterEventArgs> focusGained;

		public event Action<FocusExitEventArgs> focusLost;

		internal static event Action<XRInteractionManager, bool> activeInteractionManagersChanged;

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

		public IXRFocusInteractable lastFocused { get; protected set; }

		internal static List<XRInteractionManager> activeInteractionManagers { get; } = new List<XRInteractionManager>();

		protected virtual void Awake()
		{
			this.m_HoverFilters.RegisterReferences<Object>(this.m_StartingHoverFilters, this);
			this.m_SelectFilters.RegisterReferences<Object>(this.m_StartingSelectFilters, this);
		}

		protected virtual void OnEnable()
		{
			if (XRInteractionManager.activeInteractionManagers.Count > 0)
			{
				Debug.LogWarning("There are multiple active and enabled XR Interaction Manager components in the loaded scenes. This is supported, but may not be intended since interactors and interactables are not able to interact with those registered to a different manager. You can use the <b>Window</b> > <b>Analysis</b> > <b>XR Interaction Debugger</b> window to verify the interactors and interactables registered with each.", this);
			}
			XRInteractionManager.activeInteractionManagers.Add(this);
			Action<XRInteractionManager, bool> action = XRInteractionManager.activeInteractionManagersChanged;
			if (action != null)
			{
				action(this, true);
			}
			Application.onBeforeRender += this.OnBeforeRender;
		}

		protected virtual void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
			XRInteractionManager.activeInteractionManagers.Remove(this);
			Action<XRInteractionManager, bool> action = XRInteractionManager.activeInteractionManagersChanged;
			if (action != null)
			{
				action(this, false);
			}
			this.ClearPriorityForSelectionMap();
		}

		protected virtual void Update()
		{
			this.ClearPriorityForSelectionMap();
			this.FlushRegistration();
			using (XRInteractionManager.s_PreprocessInteractorsMarker.Auto())
			{
				this.PreprocessInteractors(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
			}
			foreach (IXRInteractionGroup ixrinteractionGroup in this.m_InteractionGroups.registeredSnapshot)
			{
				if (this.m_InteractionGroups.IsStillRegistered(ixrinteractionGroup) && !this.m_GroupsInGroup.Contains(ixrinteractionGroup))
				{
					using (XRInteractionManager.s_EvaluateInvalidFocusMarker.Auto())
					{
						this.ClearInteractionGroupFocus(ixrinteractionGroup);
					}
					using (XRInteractionManager.s_UpdateGroupMemberInteractionsMarker.Auto())
					{
						ixrinteractionGroup.UpdateGroupMemberInteractions();
					}
				}
			}
			foreach (IXRInteractor ixrinteractor in this.m_Interactors.registeredSnapshot)
			{
				if (this.m_Interactors.IsStillRegistered(ixrinteractor) && !this.m_InteractorsInGroup.Contains(ixrinteractor))
				{
					using (XRInteractionManager.s_GetValidTargetsMarker.Auto())
					{
						this.GetValidTargets(ixrinteractor, this.m_ValidTargets);
					}
					IXRSelectInteractor ixrselectInteractor = ixrinteractor as IXRSelectInteractor;
					IXRHoverInteractor ixrhoverInteractor = ixrinteractor as IXRHoverInteractor;
					if (ixrselectInteractor != null)
					{
						using (XRInteractionManager.s_EvaluateInvalidSelectionsMarker.Auto())
						{
							this.ClearInteractorSelection(ixrselectInteractor, this.m_ValidTargets);
						}
					}
					if (ixrhoverInteractor != null)
					{
						using (XRInteractionManager.s_EvaluateInvalidHoversMarker.Auto())
						{
							this.ClearInteractorHover(ixrhoverInteractor, this.m_ValidTargets);
						}
					}
					if (ixrselectInteractor != null)
					{
						using (XRInteractionManager.s_EvaluateValidSelectionsMarker.Auto())
						{
							this.InteractorSelectValidTargets(ixrselectInteractor, this.m_ValidTargets);
						}
					}
					if (ixrhoverInteractor != null)
					{
						using (XRInteractionManager.s_EvaluateValidHoversMarker.Auto())
						{
							this.InteractorHoverValidTargets(ixrhoverInteractor, this.m_ValidTargets);
						}
					}
				}
			}
			using (XRInteractionManager.s_ProcessInteractionStrengthMarker.Auto())
			{
				this.ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
			}
			using (XRInteractionManager.s_ProcessInteractorsMarker.Auto())
			{
				this.ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
			}
			using (XRInteractionManager.s_ProcessInteractablesMarker.Auto())
			{
				this.ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Dynamic);
			}
		}

		protected virtual void LateUpdate()
		{
			this.FlushRegistration();
			using (XRInteractionManager.s_ProcessInteractorsMarker.Auto())
			{
				this.ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Late);
			}
			using (XRInteractionManager.s_ProcessInteractablesMarker.Auto())
			{
				this.ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Late);
			}
		}

		protected virtual void FixedUpdate()
		{
			this.FlushRegistration();
			using (XRInteractionManager.s_ProcessInteractorsMarker.Auto())
			{
				this.ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.Fixed);
			}
			using (XRInteractionManager.s_ProcessInteractablesMarker.Auto())
			{
				this.ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.Fixed);
			}
		}

		[BeforeRenderOrder(100)]
		protected virtual void OnBeforeRender()
		{
			this.FlushRegistration();
			using (XRInteractionManager.s_ProcessInteractorsMarker.Auto())
			{
				this.ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
			}
			using (XRInteractionManager.s_ProcessInteractablesMarker.Auto())
			{
				this.ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase.OnBeforeRender);
			}
		}

		protected virtual void PreprocessInteractors(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (IXRInteractionGroup ixrinteractionGroup in this.m_InteractionGroups.registeredSnapshot)
			{
				if (this.m_InteractionGroups.IsStillRegistered(ixrinteractionGroup) && !this.m_GroupsInGroup.Contains(ixrinteractionGroup))
				{
					ixrinteractionGroup.PreprocessGroupMembers(updatePhase);
				}
			}
			foreach (IXRInteractor ixrinteractor in this.m_Interactors.registeredSnapshot)
			{
				if (this.m_Interactors.IsStillRegistered(ixrinteractor) && !this.m_InteractorsInGroup.Contains(ixrinteractor))
				{
					ixrinteractor.PreprocessInteractor(updatePhase);
				}
			}
			XRUIToolkitHandler.UpdateEventSystem();
		}

		protected virtual void ProcessInteractors(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (IXRInteractionGroup ixrinteractionGroup in this.m_InteractionGroups.registeredSnapshot)
			{
				if (this.m_InteractionGroups.IsStillRegistered(ixrinteractionGroup) && !this.m_GroupsInGroup.Contains(ixrinteractionGroup))
				{
					ixrinteractionGroup.ProcessGroupMembers(updatePhase);
				}
			}
			foreach (IXRInteractor ixrinteractor in this.m_Interactors.registeredSnapshot)
			{
				if (this.m_Interactors.IsStillRegistered(ixrinteractor) && !this.m_InteractorsInGroup.Contains(ixrinteractor))
				{
					ixrinteractor.ProcessInteractor(updatePhase);
				}
			}
		}

		protected virtual void ProcessInteractables(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (IXRInteractable ixrinteractable in this.m_Interactables.registeredSnapshot)
			{
				if (this.m_Interactables.IsStillRegistered(ixrinteractable))
				{
					ixrinteractable.ProcessInteractable(updatePhase);
				}
			}
		}

		protected virtual void ProcessInteractionStrength(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			foreach (IXRInteractable ixrinteractable in this.m_Interactables.registeredSnapshot)
			{
				if (this.m_Interactables.IsStillRegistered(ixrinteractable))
				{
					IXRInteractionStrengthInteractable ixrinteractionStrengthInteractable = ixrinteractable as IXRInteractionStrengthInteractable;
					if (ixrinteractionStrengthInteractable != null)
					{
						ixrinteractionStrengthInteractable.ProcessInteractionStrength(updatePhase);
					}
				}
			}
			foreach (IXRInteractor ixrinteractor in this.m_Interactors.registeredSnapshot)
			{
				if (this.m_Interactors.IsStillRegistered(ixrinteractor))
				{
					IXRInteractionStrengthInteractor ixrinteractionStrengthInteractor = ixrinteractor as IXRInteractionStrengthInteractor;
					if (ixrinteractionStrengthInteractor != null)
					{
						ixrinteractionStrengthInteractor.ProcessInteractionStrength(updatePhase);
					}
				}
			}
		}

		public virtual bool CanHover(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			return interactor.isHoverActive && this.IsHoverPossible(interactor, interactable);
		}

		public bool IsHoverPossible(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			return XRInteractionManager.HasInteractionLayerOverlap(interactor, interactable) && this.ProcessHoverFilters(interactor, interactable) && interactor.CanHover(interactable) && interactable.IsHoverableBy(interactor);
		}

		public virtual bool CanSelect(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			return interactor.isSelectActive && this.IsSelectPossible(interactor, interactable);
		}

		public bool IsSelectPossible(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			return XRInteractionManager.HasInteractionLayerOverlap(interactor, interactable) && this.ProcessSelectFilters(interactor, interactable) && interactor.CanSelect(interactable) && interactable.IsSelectableBy(interactor);
		}

		public virtual bool CanFocus(IXRInteractor interactor, IXRFocusInteractable interactable)
		{
			return this.IsFocusPossible(interactor, interactable);
		}

		public bool IsFocusPossible(IXRInteractor interactor, IXRFocusInteractable interactable)
		{
			if (interactable.canFocus && XRInteractionManager.HasInteractionLayerOverlap(interactor, interactable))
			{
				IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
				if (ixrgroupMember != null)
				{
					return ixrgroupMember.containingGroup != null;
				}
			}
			return false;
		}

		public virtual void RegisterInteractionGroup(IXRInteractionGroup interactionGroup)
		{
			IXRInteractionGroup ixrinteractionGroup = null;
			IXRGroupMember ixrgroupMember = interactionGroup as IXRGroupMember;
			if (ixrgroupMember != null)
			{
				ixrinteractionGroup = ixrgroupMember.containingGroup;
			}
			if (ixrinteractionGroup != null && !this.IsRegistered(ixrinteractionGroup))
			{
				Debug.LogError(string.Format("Cannot register {0} with Interaction Manager before its containing ", interactionGroup) + "Interaction Group is registered.", this);
				return;
			}
			if (this.m_InteractionGroups.Register(interactionGroup))
			{
				if (ixrinteractionGroup != null)
				{
					this.m_GroupsInGroup.Add(interactionGroup);
				}
				InteractionGroupRegisteredEventArgs interactionGroupRegisteredEventArgs;
				using (this.m_InteractionGroupRegisteredEventArgs.Get(out interactionGroupRegisteredEventArgs))
				{
					interactionGroupRegisteredEventArgs.manager = this;
					interactionGroupRegisteredEventArgs.interactionGroupObject = interactionGroup;
					interactionGroupRegisteredEventArgs.containingGroupObject = ixrinteractionGroup;
					this.OnRegistered(interactionGroupRegisteredEventArgs);
				}
			}
		}

		protected virtual void OnRegistered(InteractionGroupRegisteredEventArgs args)
		{
			args.interactionGroupObject.OnRegistered(args);
			Action<InteractionGroupRegisteredEventArgs> action = this.interactionGroupRegistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public virtual void UnregisterInteractionGroup(IXRInteractionGroup interactionGroup)
		{
			if (!this.IsRegistered(interactionGroup))
			{
				return;
			}
			interactionGroup.OnBeforeUnregistered();
			if (this.m_InteractionGroups.flushedCount > 0)
			{
				this.m_InteractionGroups.GetRegisteredItems(this.m_ScratchInteractionGroups);
				foreach (IXRInteractionGroup ixrinteractionGroup in this.m_ScratchInteractionGroups)
				{
					IXRGroupMember ixrgroupMember = ixrinteractionGroup as IXRGroupMember;
					if (ixrgroupMember != null && ixrgroupMember.containingGroup == interactionGroup)
					{
						Debug.LogError(string.Format("Cannot unregister {0} with Interaction Manager before its ", interactionGroup) + "Group Members have been unregistered or re-registered as not part of the Group.", this);
						return;
					}
				}
			}
			if (this.m_Interactors.flushedCount > 0)
			{
				this.m_Interactors.GetRegisteredItems(this.m_ScratchInteractors);
				foreach (IXRInteractor ixrinteractor in this.m_ScratchInteractors)
				{
					IXRGroupMember ixrgroupMember2 = ixrinteractor as IXRGroupMember;
					if (ixrgroupMember2 != null && ixrgroupMember2.containingGroup == interactionGroup)
					{
						Debug.LogError(string.Format("Cannot unregister {0} with Interaction Manager before its ", interactionGroup) + "Group Members have been unregistered or re-registered as not part of the Group.", this);
						return;
					}
				}
			}
			if (this.m_InteractionGroups.Unregister(interactionGroup))
			{
				this.m_GroupsInGroup.Remove(interactionGroup);
				InteractionGroupUnregisteredEventArgs interactionGroupUnregisteredEventArgs;
				using (this.m_InteractionGroupUnregisteredEventArgs.Get(out interactionGroupUnregisteredEventArgs))
				{
					interactionGroupUnregisteredEventArgs.manager = this;
					interactionGroupUnregisteredEventArgs.interactionGroupObject = interactionGroup;
					this.OnUnregistered(interactionGroupUnregisteredEventArgs);
				}
			}
		}

		protected virtual void OnUnregistered(InteractionGroupUnregisteredEventArgs args)
		{
			args.interactionGroupObject.OnUnregistered(args);
			Action<InteractionGroupUnregisteredEventArgs> action = this.interactionGroupUnregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public void GetInteractionGroups(List<IXRInteractionGroup> interactionGroups)
		{
			this.m_InteractionGroups.GetRegisteredItems(interactionGroups);
		}

		public IXRInteractionGroup GetInteractionGroup(string groupName)
		{
			foreach (IXRInteractionGroup ixrinteractionGroup in this.m_InteractionGroups.registeredSnapshot)
			{
				if (ixrinteractionGroup.groupName == groupName)
				{
					return ixrinteractionGroup;
				}
			}
			return null;
		}

		public virtual void RegisterInteractor(IXRInteractor interactor)
		{
			IXRInteractionGroup ixrinteractionGroup = null;
			IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
			if (ixrgroupMember != null)
			{
				ixrinteractionGroup = ixrgroupMember.containingGroup;
			}
			if (ixrinteractionGroup != null && !this.IsRegistered(ixrinteractionGroup))
			{
				Debug.LogError(string.Format("Cannot register {0} with Interaction Manager before its containing ", interactor) + "Interaction Group is registered.", this);
				return;
			}
			if (this.m_Interactors.Register(interactor))
			{
				if (ixrinteractionGroup != null)
				{
					this.m_InteractorsInGroup.Add(interactor);
				}
				InteractorRegisteredEventArgs interactorRegisteredEventArgs;
				using (this.m_InteractorRegisteredEventArgs.Get(out interactorRegisteredEventArgs))
				{
					interactorRegisteredEventArgs.manager = this;
					interactorRegisteredEventArgs.interactorObject = interactor;
					interactorRegisteredEventArgs.containingGroupObject = ixrinteractionGroup;
					this.OnRegistered(interactorRegisteredEventArgs);
				}
			}
		}

		protected virtual void OnRegistered(InteractorRegisteredEventArgs args)
		{
			args.interactorObject.OnRegistered(args);
			Action<InteractorRegisteredEventArgs> action = this.interactorRegistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public virtual void UnregisterInteractor(IXRInteractor interactor)
		{
			if (!this.IsRegistered(interactor))
			{
				return;
			}
			Transform transform = interactor.transform;
			if (transform == null || transform.gameObject.activeSelf)
			{
				this.CancelInteractorFocus(interactor);
			}
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			if (ixrselectInteractor != null)
			{
				this.CancelInteractorSelection(ixrselectInteractor);
			}
			IXRHoverInteractor ixrhoverInteractor = interactor as IXRHoverInteractor;
			if (ixrhoverInteractor != null)
			{
				this.CancelInteractorHover(ixrhoverInteractor);
			}
			if (this.m_Interactors.Unregister(interactor))
			{
				this.m_InteractorsInGroup.Remove(interactor);
				InteractorUnregisteredEventArgs interactorUnregisteredEventArgs;
				using (this.m_InteractorUnregisteredEventArgs.Get(out interactorUnregisteredEventArgs))
				{
					interactorUnregisteredEventArgs.manager = this;
					interactorUnregisteredEventArgs.interactorObject = interactor;
					this.OnUnregistered(interactorUnregisteredEventArgs);
				}
			}
		}

		protected virtual void OnUnregistered(InteractorUnregisteredEventArgs args)
		{
			args.interactorObject.OnUnregistered(args);
			Action<InteractorUnregisteredEventArgs> action = this.interactorUnregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public virtual void RegisterInteractable(IXRInteractable interactable)
		{
			if (this.m_Interactables.Register(interactable))
			{
				foreach (Collider collider in interactable.colliders)
				{
					if (!(collider == null))
					{
						IXRInteractable arg;
						if (!this.m_ColliderToInteractableMap.TryGetValue(collider, out arg))
						{
							this.m_ColliderToInteractableMap.Add(collider, interactable);
						}
						else
						{
							Debug.LogWarning("A collider used by an Interactable object is already registered with another Interactable object." + string.Format(" The {0} will remain associated with {1}, which was registered before {2}.", collider, arg, interactable) + " The value returned by XRInteractionManager.TryGetInteractableForCollider will be the first association.", interactable as Object);
						}
					}
				}
				InteractableRegisteredEventArgs interactableRegisteredEventArgs;
				using (this.m_InteractableRegisteredEventArgs.Get(out interactableRegisteredEventArgs))
				{
					interactableRegisteredEventArgs.manager = this;
					interactableRegisteredEventArgs.interactableObject = interactable;
					this.OnRegistered(interactableRegisteredEventArgs);
				}
			}
		}

		protected virtual void OnRegistered(InteractableRegisteredEventArgs args)
		{
			args.interactableObject.OnRegistered(args);
			Action<InteractableRegisteredEventArgs> action = this.interactableRegistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public virtual void UnregisterInteractable(IXRInteractable interactable)
		{
			if (!this.IsRegistered(interactable))
			{
				return;
			}
			IXRFocusInteractable ixrfocusInteractable = interactable as IXRFocusInteractable;
			if (ixrfocusInteractable != null)
			{
				this.CancelInteractableFocus(ixrfocusInteractable);
			}
			IXRSelectInteractable ixrselectInteractable = interactable as IXRSelectInteractable;
			if (ixrselectInteractable != null)
			{
				this.CancelInteractableSelection(ixrselectInteractable);
			}
			IXRHoverInteractable ixrhoverInteractable = interactable as IXRHoverInteractable;
			if (ixrhoverInteractable != null)
			{
				this.CancelInteractableHover(ixrhoverInteractable);
			}
			if (this.m_Interactables.Unregister(interactable))
			{
				foreach (Collider collider in interactable.colliders)
				{
					IXRInteractable ixrinteractable;
					if (!(collider == null) && this.m_ColliderToInteractableMap.TryGetValue(collider, out ixrinteractable) && ixrinteractable == interactable)
					{
						this.m_ColliderToInteractableMap.Remove(collider);
					}
				}
				InteractableUnregisteredEventArgs interactableUnregisteredEventArgs;
				using (this.m_InteractableUnregisteredEventArgs.Get(out interactableUnregisteredEventArgs))
				{
					interactableUnregisteredEventArgs.manager = this;
					interactableUnregisteredEventArgs.interactableObject = interactable;
					this.OnUnregistered(interactableUnregisteredEventArgs);
				}
			}
		}

		protected virtual void OnUnregistered(InteractableUnregisteredEventArgs args)
		{
			args.interactableObject.OnUnregistered(args);
			Action<InteractableUnregisteredEventArgs> action = this.interactableUnregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public void RegisterSnapVolume(XRInteractableSnapVolume snapVolume)
		{
			if (snapVolume == null)
			{
				return;
			}
			Collider snapCollider = snapVolume.snapCollider;
			if (snapCollider == null)
			{
				return;
			}
			XRInteractableSnapVolume arg;
			if (!this.m_ColliderToSnapVolumes.TryGetValue(snapCollider, out arg))
			{
				this.m_ColliderToSnapVolumes.Add(snapCollider, snapVolume);
				return;
			}
			Debug.LogWarning("A collider used by a snap volume component is already registered with another snap volume component." + string.Format(" The {0} will remain associated with {1}, which was registered before {2}.", snapCollider, arg, snapVolume) + " The value returned by XRInteractionManager.TryGetInteractableForCollider will be the first association.", snapVolume);
		}

		public void UnregisterSnapVolume(XRInteractableSnapVolume snapVolume)
		{
			if (snapVolume == null)
			{
				return;
			}
			Collider snapCollider = snapVolume.snapCollider;
			if (snapCollider == null)
			{
				return;
			}
			XRInteractableSnapVolume x;
			if (this.m_ColliderToSnapVolumes.TryGetValue(snapCollider, out x) && x == snapVolume)
			{
				this.m_ColliderToSnapVolumes.Remove(snapCollider);
			}
		}

		public void GetRegisteredInteractionGroups(List<IXRInteractionGroup> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_InteractionGroups.GetRegisteredItems(results);
		}

		public void GetRegisteredInteractors(List<IXRInteractor> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_Interactors.GetRegisteredItems(results);
		}

		public void GetRegisteredInteractables(List<IXRInteractable> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_Interactables.GetRegisteredItems(results);
		}

		public bool IsRegistered(IXRInteractionGroup interactionGroup)
		{
			return this.m_InteractionGroups.IsRegistered(interactionGroup);
		}

		public bool IsRegistered(IXRInteractor interactor)
		{
			return this.m_Interactors.IsRegistered(interactor);
		}

		public bool IsRegistered(IXRInteractable interactable)
		{
			return this.m_Interactables.IsRegistered(interactable);
		}

		public bool TryGetInteractableForCollider(Collider interactableCollider, out IXRInteractable interactable)
		{
			interactable = null;
			if (interactableCollider == null)
			{
				return false;
			}
			XRInteractableSnapVolume xrinteractableSnapVolume;
			if (!this.m_ColliderToInteractableMap.TryGetValue(interactableCollider, out interactable) && this.m_ColliderToSnapVolumes.TryGetValue(interactableCollider, out xrinteractableSnapVolume) && xrinteractableSnapVolume != null)
			{
				interactable = xrinteractableSnapVolume.interactable;
			}
			if (interactable != null)
			{
				Object @object = interactable as Object;
				return @object == null || @object != null;
			}
			return false;
		}

		public bool TryGetInteractableForCollider(Collider interactableCollider, out IXRInteractable interactable, out XRInteractableSnapVolume snapVolume)
		{
			interactable = null;
			snapVolume = null;
			if (interactableCollider == null)
			{
				return false;
			}
			bool flag = this.m_ColliderToInteractableMap.TryGetValue(interactableCollider, out interactable);
			if (this.m_ColliderToSnapVolumes.TryGetValue(interactableCollider, out snapVolume) && snapVolume != null)
			{
				if (flag)
				{
					if (snapVolume.interactable != interactable)
					{
						snapVolume = null;
					}
				}
				else
				{
					interactable = snapVolume.interactable;
				}
			}
			if (interactable != null)
			{
				Object @object = interactable as Object;
				return @object == null || @object != null;
			}
			return false;
		}

		public bool IsColliderRegisteredToInteractable(in Collider colliderToCheck)
		{
			return this.m_ColliderToInteractableMap.ContainsKey(colliderToCheck) || this.m_ColliderToSnapVolumes.ContainsKey(colliderToCheck);
		}

		public bool IsColliderRegisteredSnapVolume(in Collider potentialSnapVolumeCollider)
		{
			return this.m_ColliderToSnapVolumes.ContainsKey(potentialSnapVolumeCollider);
		}

		public bool IsHighestPriorityTarget(IXRSelectInteractable target, List<IXRTargetPriorityInteractor> interactors = null)
		{
			List<IXRTargetPriorityInteractor> collection;
			if (!this.m_HighestPriorityTargetMap.TryGetValue(target, out collection))
			{
				return false;
			}
			if (interactors == null)
			{
				return true;
			}
			interactors.Clear();
			interactors.AddRange(collection);
			return true;
		}

		public bool IsHandSelecting(InteractorHandedness hand)
		{
			foreach (IXRInteractor ixrinteractor in this.m_Interactors.registeredSnapshot)
			{
				if (this.m_Interactors.IsStillRegistered(ixrinteractor) && ixrinteractor.handedness == hand)
				{
					IXRSelectInteractor ixrselectInteractor = ixrinteractor as IXRSelectInteractor;
					if (ixrselectInteractor != null && ixrselectInteractor.hasSelection)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void GetValidTargets(IXRInteractor interactor, List<IXRInteractable> targets)
		{
			targets.Clear();
			interactor.GetValidTargets(targets);
			using (XRInteractionManager.s_FilterRegisteredValidTargetsMarker.Auto())
			{
				XRInteractionManager.RemoveAllUnregistered(this, targets);
			}
		}

		internal static int RemoveAllUnregistered(XRInteractionManager manager, List<IXRInteractable> interactables)
		{
			int num = 0;
			for (int i = interactables.Count - 1; i >= 0; i--)
			{
				if (!manager.m_Interactables.IsRegistered(interactables[i]))
				{
					interactables.RemoveAt(i);
					num++;
				}
			}
			return num;
		}

		protected virtual void ClearInteractionGroupFocus(IXRInteractionGroup interactionGroup)
		{
			IXRInteractor focusInteractor = interactionGroup.focusInteractor;
			IXRFocusInteractable focusInteractable = interactionGroup.focusInteractable;
			if (focusInteractor == null || focusInteractable == null)
			{
				return;
			}
			bool flag = false;
			IXRSelectInteractor ixrselectInteractor = focusInteractor as IXRSelectInteractor;
			if (ixrselectInteractor != null)
			{
				IXRSelectInteractable ixrselectInteractable = focusInteractable as IXRSelectInteractable;
				if (ixrselectInteractable != null)
				{
					flag = (ixrselectInteractor.isSelectActive && !ixrselectInteractor.IsSelecting(ixrselectInteractable));
				}
				else
				{
					flag = ixrselectInteractor.isSelectActive;
				}
			}
			if (flag || !this.CanFocus(focusInteractor, focusInteractable))
			{
				this.FocusExit(interactionGroup, focusInteractable);
			}
		}

		private void CancelInteractorFocus(IXRInteractor interactor)
		{
			IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
			IXRInteractionGroup ixrinteractionGroup = (ixrgroupMember != null) ? ixrgroupMember.containingGroup : null;
			if (ixrinteractionGroup != null && ixrinteractionGroup.focusInteractable != null && ixrinteractionGroup.focusInteractor == interactor)
			{
				this.FocusCancel(ixrinteractionGroup, ixrinteractionGroup.focusInteractable);
			}
		}

		public virtual void CancelInteractableFocus(IXRFocusInteractable interactable)
		{
			for (int i = interactable.interactionGroupsFocusing.Count - 1; i >= 0; i--)
			{
				this.FocusCancel(interactable.interactionGroupsFocusing[i], interactable);
			}
		}

		protected internal virtual void ClearInteractorSelection(IXRSelectInteractor interactor, List<IXRInteractable> validTargets)
		{
			if (!interactor.hasSelection)
			{
				return;
			}
			this.m_CurrentSelected.Clear();
			this.m_CurrentSelected.AddRange(interactor.interactablesSelected);
			this.m_UnorderedValidTargets.Clear();
			if (validTargets.Count > 0)
			{
				foreach (IXRInteractable item in validTargets)
				{
					this.m_UnorderedValidTargets.Add(item);
				}
			}
			for (int i = this.m_CurrentSelected.Count - 1; i >= 0; i--)
			{
				IXRSelectInteractable ixrselectInteractable = this.m_CurrentSelected[i];
				if (!this.CanSelect(interactor, ixrselectInteractable) || (!interactor.keepSelectedTargetValid && !this.m_UnorderedValidTargets.Contains(ixrselectInteractable)))
				{
					this.SelectExit(interactor, ixrselectInteractable);
				}
			}
		}

		public virtual void CancelInteractorSelection(IXRSelectInteractor interactor)
		{
			for (int i = interactor.interactablesSelected.Count - 1; i >= 0; i--)
			{
				this.SelectCancel(interactor, interactor.interactablesSelected[i]);
			}
		}

		public virtual void CancelInteractableSelection(IXRSelectInteractable interactable)
		{
			for (int i = interactable.interactorsSelecting.Count - 1; i >= 0; i--)
			{
				this.SelectCancel(interactable.interactorsSelecting[i], interactable);
			}
		}

		protected internal virtual void ClearInteractorHover(IXRHoverInteractor interactor, List<IXRInteractable> validTargets)
		{
			if (!interactor.hasHover)
			{
				return;
			}
			this.m_CurrentHovered.Clear();
			this.m_CurrentHovered.AddRange(interactor.interactablesHovered);
			this.m_UnorderedValidTargets.Clear();
			if (validTargets.Count > 0)
			{
				foreach (IXRInteractable item in validTargets)
				{
					this.m_UnorderedValidTargets.Add(item);
				}
			}
			for (int i = this.m_CurrentHovered.Count - 1; i >= 0; i--)
			{
				IXRHoverInteractable ixrhoverInteractable = this.m_CurrentHovered[i];
				if (!this.CanHover(interactor, ixrhoverInteractable) || !this.m_UnorderedValidTargets.Contains(ixrhoverInteractable))
				{
					this.HoverExit(interactor, ixrhoverInteractable);
				}
			}
		}

		public virtual void CancelInteractorHover(IXRHoverInteractor interactor)
		{
			for (int i = interactor.interactablesHovered.Count - 1; i >= 0; i--)
			{
				this.HoverCancel(interactor, interactor.interactablesHovered[i]);
			}
		}

		public virtual void CancelInteractableHover(IXRHoverInteractable interactable)
		{
			for (int i = interactable.interactorsHovering.Count - 1; i >= 0; i--)
			{
				this.HoverCancel(interactable.interactorsHovering[i], interactable);
			}
		}

		public virtual void FocusEnter(IXRInteractor interactor, IXRFocusInteractable interactable)
		{
			IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
			IXRInteractionGroup ixrinteractionGroup = (ixrgroupMember != null) ? ixrgroupMember.containingGroup : null;
			if (ixrinteractionGroup == null || !this.CanFocus(interactor, interactable))
			{
				return;
			}
			if (interactable.isFocused && !this.ResolveExistingFocus(ixrinteractionGroup, interactable))
			{
				return;
			}
			FocusEnterEventArgs focusEnterEventArgs;
			using (this.m_FocusEnterEventArgs.Get(out focusEnterEventArgs))
			{
				focusEnterEventArgs.manager = this;
				focusEnterEventArgs.interactorObject = interactor;
				focusEnterEventArgs.interactableObject = interactable;
				focusEnterEventArgs.interactionGroup = ixrinteractionGroup;
				this.FocusEnter(ixrinteractionGroup, interactable, focusEnterEventArgs);
			}
		}

		public virtual void FocusExit(IXRInteractionGroup group, IXRFocusInteractable interactable)
		{
			FocusExitEventArgs focusExitEventArgs;
			using (this.m_FocusExitEventArgs.Get(out focusExitEventArgs))
			{
				focusExitEventArgs.manager = this;
				focusExitEventArgs.interactorObject = group.focusInteractor;
				focusExitEventArgs.interactableObject = interactable;
				focusExitEventArgs.interactionGroup = group;
				focusExitEventArgs.isCanceled = false;
				this.FocusExit(group, interactable, focusExitEventArgs);
			}
		}

		public virtual void FocusCancel(IXRInteractionGroup group, IXRFocusInteractable interactable)
		{
			FocusExitEventArgs focusExitEventArgs;
			using (this.m_FocusExitEventArgs.Get(out focusExitEventArgs))
			{
				focusExitEventArgs.manager = this;
				focusExitEventArgs.interactorObject = group.focusInteractor;
				focusExitEventArgs.interactableObject = interactable;
				focusExitEventArgs.interactionGroup = group;
				focusExitEventArgs.isCanceled = true;
				this.FocusExit(group, interactable, focusExitEventArgs);
			}
		}

		public virtual void SelectEnter(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			if (interactable.isSelected && !this.ResolveExistingSelect(interactor, interactable))
			{
				return;
			}
			SelectEnterEventArgs selectEnterEventArgs;
			using (this.m_SelectEnterEventArgs.Get(out selectEnterEventArgs))
			{
				selectEnterEventArgs.manager = this;
				selectEnterEventArgs.interactorObject = interactor;
				selectEnterEventArgs.interactableObject = interactable;
				this.SelectEnter(interactor, interactable, selectEnterEventArgs);
			}
			IXRFocusInteractable ixrfocusInteractable = interactable as IXRFocusInteractable;
			if (ixrfocusInteractable != null)
			{
				this.FocusEnter(interactor, ixrfocusInteractable);
			}
		}

		public virtual void SelectExit(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			SelectExitEventArgs selectExitEventArgs;
			using (this.m_SelectExitEventArgs.Get(out selectExitEventArgs))
			{
				selectExitEventArgs.manager = this;
				selectExitEventArgs.interactorObject = interactor;
				selectExitEventArgs.interactableObject = interactable;
				selectExitEventArgs.isCanceled = false;
				this.SelectExit(interactor, interactable, selectExitEventArgs);
			}
		}

		public virtual void SelectCancel(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			SelectExitEventArgs selectExitEventArgs;
			using (this.m_SelectExitEventArgs.Get(out selectExitEventArgs))
			{
				selectExitEventArgs.manager = this;
				selectExitEventArgs.interactorObject = interactor;
				selectExitEventArgs.interactableObject = interactable;
				selectExitEventArgs.isCanceled = true;
				this.SelectExit(interactor, interactable, selectExitEventArgs);
			}
		}

		public virtual void HoverEnter(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			HoverEnterEventArgs hoverEnterEventArgs;
			using (this.m_HoverEnterEventArgs.Get(out hoverEnterEventArgs))
			{
				hoverEnterEventArgs.manager = this;
				hoverEnterEventArgs.interactorObject = interactor;
				hoverEnterEventArgs.interactableObject = interactable;
				this.HoverEnter(interactor, interactable, hoverEnterEventArgs);
			}
		}

		public virtual void HoverExit(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			HoverExitEventArgs hoverExitEventArgs;
			using (this.m_HoverExitEventArgs.Get(out hoverExitEventArgs))
			{
				hoverExitEventArgs.manager = this;
				hoverExitEventArgs.interactorObject = interactor;
				hoverExitEventArgs.interactableObject = interactable;
				hoverExitEventArgs.isCanceled = false;
				this.HoverExit(interactor, interactable, hoverExitEventArgs);
			}
		}

		public virtual void HoverCancel(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			HoverExitEventArgs hoverExitEventArgs;
			using (this.m_HoverExitEventArgs.Get(out hoverExitEventArgs))
			{
				hoverExitEventArgs.manager = this;
				hoverExitEventArgs.interactorObject = interactor;
				hoverExitEventArgs.interactableObject = interactable;
				hoverExitEventArgs.isCanceled = true;
				this.HoverExit(interactor, interactable, hoverExitEventArgs);
			}
		}

		protected virtual void FocusEnter(IXRInteractionGroup group, IXRFocusInteractable interactable, FocusEnterEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_FocusEnterMarker.Auto())
			{
				group.OnFocusEntering(args);
				IXRGroupMember ixrgroupMember = group as IXRGroupMember;
				if (ixrgroupMember != null)
				{
					IXRGroupMember ixrgroupMember2;
					for (IXRInteractionGroup ixrinteractionGroup = ixrgroupMember.containingGroup; ixrinteractionGroup != null; ixrinteractionGroup = ((ixrgroupMember2 != null) ? ixrgroupMember2.containingGroup : null))
					{
						ixrinteractionGroup.OnFocusEntering(args);
						ixrgroupMember2 = (ixrinteractionGroup as IXRGroupMember);
					}
				}
				interactable.OnFocusEntering(args);
				interactable.OnFocusEntered(args);
			}
			this.lastFocused = interactable;
			Action<FocusEnterEventArgs> action = this.focusGained;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void FocusExit(IXRInteractionGroup group, IXRFocusInteractable interactable, FocusExitEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_FocusExitMarker.Auto())
			{
				group.OnFocusExiting(args);
				IXRGroupMember ixrgroupMember = group as IXRGroupMember;
				if (ixrgroupMember != null)
				{
					IXRGroupMember ixrgroupMember2;
					for (IXRInteractionGroup ixrinteractionGroup = ixrgroupMember.containingGroup; ixrinteractionGroup != null; ixrinteractionGroup = ((ixrgroupMember2 != null) ? ixrgroupMember2.containingGroup : null))
					{
						ixrinteractionGroup.OnFocusExiting(args);
						ixrgroupMember2 = (ixrinteractionGroup as IXRGroupMember);
					}
				}
				interactable.OnFocusExiting(args);
				interactable.OnFocusExited(args);
			}
			if (interactable == this.lastFocused)
			{
				this.lastFocused = null;
			}
			Action<FocusExitEventArgs> action = this.focusLost;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		protected virtual void SelectEnter(IXRSelectInteractor interactor, IXRSelectInteractable interactable, SelectEnterEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_SelectEnterMarker.Auto())
			{
				interactor.OnSelectEntering(args);
				interactable.OnSelectEntering(args);
				interactor.OnSelectEntered(args);
				interactable.OnSelectEntered(args);
			}
		}

		protected virtual void SelectExit(IXRSelectInteractor interactor, IXRSelectInteractable interactable, SelectExitEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_SelectExitMarker.Auto())
			{
				interactor.OnSelectExiting(args);
				interactable.OnSelectExiting(args);
				interactor.OnSelectExited(args);
				interactable.OnSelectExited(args);
			}
		}

		protected virtual void HoverEnter(IXRHoverInteractor interactor, IXRHoverInteractable interactable, HoverEnterEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_HoverEnterMarker.Auto())
			{
				interactor.OnHoverEntering(args);
				interactable.OnHoverEntering(args);
				interactor.OnHoverEntered(args);
				interactable.OnHoverEntered(args);
			}
		}

		protected virtual void HoverExit(IXRHoverInteractor interactor, IXRHoverInteractable interactable, HoverExitEventArgs args)
		{
			args.manager = this;
			using (XRInteractionManager.s_HoverExitMarker.Auto())
			{
				interactor.OnHoverExiting(args);
				interactable.OnHoverExiting(args);
				interactor.OnHoverExited(args);
				interactable.OnHoverExited(args);
			}
		}

		protected internal virtual void InteractorSelectValidTargets(IXRSelectInteractor interactor, List<IXRInteractable> validTargets)
		{
			if (validTargets.Count == 0)
			{
				return;
			}
			IXRTargetPriorityInteractor ixrtargetPriorityInteractor = interactor as IXRTargetPriorityInteractor;
			TargetPriorityMode targetPriorityMode = TargetPriorityMode.None;
			if (ixrtargetPriorityInteractor != null)
			{
				targetPriorityMode = ixrtargetPriorityInteractor.targetPriorityMode;
			}
			bool flag = false;
			foreach (IXRInteractable ixrinteractable in validTargets)
			{
				IXRSelectInteractable ixrselectInteractable = ixrinteractable as IXRSelectInteractable;
				if (ixrselectInteractable != null)
				{
					if (targetPriorityMode == TargetPriorityMode.None || (targetPriorityMode == TargetPriorityMode.HighestPriorityOnly && flag))
					{
						if (this.CanSelect(interactor, ixrselectInteractable))
						{
							this.SelectEnter(interactor, ixrselectInteractable);
						}
					}
					else if (this.IsSelectPossible(interactor, ixrselectInteractable))
					{
						if (!flag)
						{
							flag = true;
							List<IXRTargetPriorityInteractor> list;
							if (!this.m_HighestPriorityTargetMap.TryGetValue(ixrselectInteractable, out list))
							{
								list = XRInteractionManager.s_TargetPriorityInteractorListPool.Get();
								this.m_HighestPriorityTargetMap[ixrselectInteractable] = list;
							}
							list.Add(ixrtargetPriorityInteractor);
						}
						List<IXRSelectInteractable> targetsForSelection = ixrtargetPriorityInteractor.targetsForSelection;
						if (targetsForSelection != null)
						{
							targetsForSelection.Add(ixrselectInteractable);
						}
						if (interactor.isSelectActive)
						{
							this.SelectEnter(interactor, ixrselectInteractable);
						}
					}
				}
			}
		}

		protected internal virtual void InteractorHoverValidTargets(IXRHoverInteractor interactor, List<IXRInteractable> validTargets)
		{
			if (validTargets.Count == 0)
			{
				return;
			}
			foreach (IXRInteractable ixrinteractable in validTargets)
			{
				IXRHoverInteractable ixrhoverInteractable = ixrinteractable as IXRHoverInteractable;
				if (ixrhoverInteractable != null && this.CanHover(interactor, ixrhoverInteractable) && !interactor.IsHovering(ixrhoverInteractable))
				{
					this.HoverEnter(interactor, ixrhoverInteractable);
				}
			}
		}

		protected virtual bool ResolveExistingFocus(IXRInteractionGroup interactionGroup, IXRFocusInteractable interactable)
		{
			if (interactionGroup.focusInteractable == interactable)
			{
				return false;
			}
			InteractableFocusMode focusMode = interactable.focusMode;
			if (focusMode != InteractableFocusMode.Single)
			{
				if (focusMode != InteractableFocusMode.Multiple)
				{
					return false;
				}
			}
			else
			{
				this.ExitInteractableFocus(interactable);
			}
			return true;
		}

		protected virtual bool ResolveExistingSelect(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			if (interactor.IsSelecting(interactable))
			{
				return false;
			}
			InteractableSelectMode selectMode = interactable.selectMode;
			if (selectMode != InteractableSelectMode.Single)
			{
				if (selectMode != InteractableSelectMode.Multiple)
				{
					return false;
				}
			}
			else
			{
				this.ExitInteractableSelection(interactable);
			}
			return true;
		}

		protected static bool HasInteractionLayerOverlap(IXRInteractor interactor, IXRInteractable interactable)
		{
			return (interactor.interactionLayers & interactable.interactionLayers) != 0;
		}

		protected bool ProcessHoverFilters(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
		{
			return XRFilterUtility.Process(this.m_HoverFilters, interactor, interactable);
		}

		protected bool ProcessSelectFilters(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
		{
			return XRFilterUtility.Process(this.m_SelectFilters, interactor, interactable);
		}

		private void ExitInteractableSelection(IXRSelectInteractable interactable)
		{
			for (int i = interactable.interactorsSelecting.Count - 1; i >= 0; i--)
			{
				this.SelectExit(interactable.interactorsSelecting[i], interactable);
			}
		}

		private void ExitInteractableFocus(IXRFocusInteractable interactable)
		{
			for (int i = interactable.interactionGroupsFocusing.Count - 1; i >= 0; i--)
			{
				this.FocusExit(interactable.interactionGroupsFocusing[i], interactable);
			}
		}

		private void ClearPriorityForSelectionMap()
		{
			if (this.m_HighestPriorityTargetMap.Count == 0)
			{
				return;
			}
			foreach (List<IXRTargetPriorityInteractor> list in this.m_HighestPriorityTargetMap.Values)
			{
				foreach (IXRTargetPriorityInteractor ixrtargetPriorityInteractor in list)
				{
					if (ixrtargetPriorityInteractor != null)
					{
						List<IXRSelectInteractable> targetsForSelection = ixrtargetPriorityInteractor.targetsForSelection;
						if (targetsForSelection != null)
						{
							targetsForSelection.Clear();
						}
					}
				}
				XRInteractionManager.s_TargetPriorityInteractorListPool.Release(list);
			}
			this.m_HighestPriorityTargetMap.Clear();
		}

		private void FlushRegistration()
		{
			this.m_InteractionGroups.Flush();
			this.m_Interactors.Flush();
			this.m_Interactables.Flush();
		}

		[Obsolete("RegisterInteractor(XRBaseInteractor) has been deprecated. Use RegisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractor((IXRInteractor)this)` instead.", true)]
		public virtual void RegisterInteractor(XRBaseInteractor interactor)
		{
			Debug.LogError("RegisterInteractor(XRBaseInteractor) has been deprecated. Use RegisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractor((IXRInteractor)this)` instead.", this);
			throw new NotSupportedException("RegisterInteractor(XRBaseInteractor) has been deprecated. Use RegisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractor((IXRInteractor)this)` instead.");
		}

		[Obsolete("UnregisterInteractor(XRBaseInteractor) has been deprecated. Use UnregisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractor((IXRInteractor)this)` instead.", true)]
		public virtual void UnregisterInteractor(XRBaseInteractor interactor)
		{
			Debug.LogError("UnregisterInteractor(XRBaseInteractor) has been deprecated. Use UnregisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractor((IXRInteractor)this)` instead.", this);
			throw new NotSupportedException("UnregisterInteractor(XRBaseInteractor) has been deprecated. Use UnregisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractor((IXRInteractor)this)` instead.");
		}

		[Obsolete("RegisterInteractable(XRBaseInteractable) has been deprecated. Use RegisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractable((IXRInteractable)this)` instead.", true)]
		public virtual void RegisterInteractable(XRBaseInteractable interactable)
		{
			Debug.LogError("RegisterInteractable(XRBaseInteractable) has been deprecated. Use RegisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractable((IXRInteractable)this)` instead.", this);
			throw new NotSupportedException("RegisterInteractable(XRBaseInteractable) has been deprecated. Use RegisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractable((IXRInteractable)this)` instead.");
		}

		[Obsolete("UnregisterInteractable(XRBaseInteractable) has been deprecated. Use UnregisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractable((IXRInteractable)this)` instead.", true)]
		public virtual void UnregisterInteractable(XRBaseInteractable interactable)
		{
			Debug.LogError("UnregisterInteractable(XRBaseInteractable) has been deprecated. Use UnregisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractable((IXRInteractable)this)` instead.", this);
			throw new NotSupportedException("UnregisterInteractable(XRBaseInteractable) has been deprecated. Use UnregisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractable((IXRInteractable)this)` instead.");
		}

		[Obsolete("GetRegisteredInteractors(List<XRBaseInteractor>) has been deprecated. Use GetRegisteredInteractors(List<IXRInteractor>) instead.", true)]
		public void GetRegisteredInteractors(List<XRBaseInteractor> results)
		{
			Debug.LogError("GetRegisteredInteractors(List<XRBaseInteractor>) has been deprecated. Use GetRegisteredInteractors(List<IXRInteractor>) instead.", this);
			throw new NotSupportedException("GetRegisteredInteractors(List<XRBaseInteractor>) has been deprecated. Use GetRegisteredInteractors(List<IXRInteractor>) instead.");
		}

		[Obsolete("GetRegisteredInteractables(List<XRBaseInteractable>) has been deprecated. Use GetRegisteredInteractables(List<IXRInteractable>) instead.", true)]
		public void GetRegisteredInteractables(List<XRBaseInteractable> results)
		{
			Debug.LogError("GetRegisteredInteractables(List<XRBaseInteractable>) has been deprecated. Use GetRegisteredInteractables(List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("GetRegisteredInteractables(List<XRBaseInteractable>) has been deprecated. Use GetRegisteredInteractables(List<IXRInteractable>) instead.");
		}

		[Obsolete("IsRegistered(XRBaseInteractor) has been deprecated. Use IsRegistered(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractor)this)` instead.", true)]
		public bool IsRegistered(XRBaseInteractor interactor)
		{
			Debug.LogError("IsRegistered(XRBaseInteractor) has been deprecated. Use IsRegistered(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractor)this)` instead.", this);
			throw new NotSupportedException("IsRegistered(XRBaseInteractor) has been deprecated. Use IsRegistered(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractor)this)` instead.");
		}

		[Obsolete("IsRegistered(XRBaseInteractable) has been deprecated. Use IsRegistered(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractable)this)` instead.", true)]
		public bool IsRegistered(XRBaseInteractable interactable)
		{
			Debug.LogError("IsRegistered(XRBaseInteractable) has been deprecated. Use IsRegistered(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractable)this)` instead.", this);
			throw new NotSupportedException("IsRegistered(XRBaseInteractable) has been deprecated. Use IsRegistered(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractable)this)` instead.");
		}

		[Obsolete("TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)", true)]
		public XRBaseInteractable TryGetInteractableForCollider(Collider interactableCollider)
		{
			Debug.LogError("TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)", this);
			throw new NotSupportedException("TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)");
		}

		[Obsolete("GetInteractableForCollider has been deprecated. Use TryGetInteractableForCollider(Collider, out IXRInteractable) instead.", true)]
		public XRBaseInteractable GetInteractableForCollider(Collider interactableCollider)
		{
			Debug.LogError("GetInteractableForCollider has been deprecated. Use TryGetInteractableForCollider(Collider, out IXRInteractable) instead.", this);
			throw new NotSupportedException("GetInteractableForCollider has been deprecated. Use TryGetInteractableForCollider(Collider, out IXRInteractable) instead.");
		}

		[Obsolete("GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.", true)]
		public void GetColliderToInteractableMap(ref Dictionary<Collider, XRBaseInteractable> map)
		{
			Debug.LogError("GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.", this);
			throw new NotSupportedException("GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.");
		}

		[Obsolete("GetValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use GetValidTargets(IXRInteractor, List<IXRInteractable>) instead.", true)]
		public List<XRBaseInteractable> GetValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
		{
			Debug.LogError("GetValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use GetValidTargets(IXRInteractor, List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("GetValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use GetValidTargets(IXRInteractor, List<IXRInteractable>) instead.");
		}

		[Obsolete("ForceSelect(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead.", true)]
		public void ForceSelect(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("ForceSelect(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead.", this);
			throw new NotSupportedException("ForceSelect(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead.");
		}

		[Obsolete("ClearInteractorSelection(XRBaseInteractor) has been deprecated. Use ClearInteractorSelection(IXRSelectInteractor, List<IXRInteractable>) instead.", true)]
		public virtual void ClearInteractorSelection(XRBaseInteractor interactor)
		{
			Debug.LogError("ClearInteractorSelection(XRBaseInteractor) has been deprecated. Use ClearInteractorSelection(IXRSelectInteractor, List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("ClearInteractorSelection(XRBaseInteractor) has been deprecated. Use ClearInteractorSelection(IXRSelectInteractor, List<IXRInteractable>) instead.");
		}

		[Obsolete("CancelInteractorSelection(XRBaseInteractor) has been deprecated. Use CancelInteractorSelection(IXRSelectInteractor) instead.", true)]
		public virtual void CancelInteractorSelection(XRBaseInteractor interactor)
		{
			Debug.LogError("CancelInteractorSelection(XRBaseInteractor) has been deprecated. Use CancelInteractorSelection(IXRSelectInteractor) instead.", this);
			throw new NotSupportedException("CancelInteractorSelection(XRBaseInteractor) has been deprecated. Use CancelInteractorSelection(IXRSelectInteractor) instead.");
		}

		[Obsolete("CancelInteractableSelection(XRBaseInteractable) has been deprecated. Use CancelInteractableSelection(IXRSelectInteractable) instead.", true)]
		public virtual void CancelInteractableSelection(XRBaseInteractable interactable)
		{
			Debug.LogError("CancelInteractableSelection(XRBaseInteractable) has been deprecated. Use CancelInteractableSelection(IXRSelectInteractable) instead.", this);
			throw new NotSupportedException("CancelInteractableSelection(XRBaseInteractable) has been deprecated. Use CancelInteractableSelection(IXRSelectInteractable) instead.");
		}

		[Obsolete("ClearInteractorHover(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use ClearInteractorHover(IXRHoverInteractor, List<IXRInteractable>) instead.", true)]
		public virtual void ClearInteractorHover(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
		{
			Debug.LogError("ClearInteractorHover(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use ClearInteractorHover(IXRHoverInteractor, List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("ClearInteractorHover(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use ClearInteractorHover(IXRHoverInteractor, List<IXRInteractable>) instead.");
		}

		[Obsolete("CancelInteractorHover(XRBaseInteractor) has been deprecated. Use CancelInteractorHover(IXRHoverInteractor) instead.", true)]
		public virtual void CancelInteractorHover(XRBaseInteractor interactor)
		{
			Debug.LogError("CancelInteractorHover(XRBaseInteractor) has been deprecated. Use CancelInteractorHover(IXRHoverInteractor) instead.", this);
			throw new NotSupportedException("CancelInteractorHover(XRBaseInteractor) has been deprecated. Use CancelInteractorHover(IXRHoverInteractor) instead.");
		}

		[Obsolete("CancelInteractableHover(XRBaseInteractable) has been deprecated. Use CancelInteractableHover(IXRHoverInteractable) instead.", true)]
		public virtual void CancelInteractableHover(XRBaseInteractable interactable)
		{
			Debug.LogError("CancelInteractableHover(XRBaseInteractable) has been deprecated. Use CancelInteractableHover(IXRHoverInteractable) instead.", this);
			throw new NotSupportedException("CancelInteractableHover(XRBaseInteractable) has been deprecated. Use CancelInteractableHover(IXRHoverInteractable) instead.");
		}

		[Obsolete("SelectEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", true)]
		public virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("SelectEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", this);
			throw new NotSupportedException("SelectEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.");
		}

		[Obsolete("SelectExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", true)]
		public virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("SelectExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", this);
			throw new NotSupportedException("SelectExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.");
		}

		[Obsolete("SelectCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectCancel(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectCancel((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", true)]
		public virtual void SelectCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("SelectCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectCancel(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectCancel((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.", this);
			throw new NotSupportedException("SelectCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectCancel(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectCancel((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.");
		}

		[Obsolete("HoverEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverEnter((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", true)]
		public virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("HoverEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverEnter((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", this);
			throw new NotSupportedException("HoverEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverEnter((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.");
		}

		[Obsolete("HoverExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverExit((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", true)]
		public virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("HoverExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverExit((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", this);
			throw new NotSupportedException("HoverExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverExit((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.");
		}

		[Obsolete("HoverCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverCancel(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverCancel((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", true)]
		public virtual void HoverCancel(XRBaseInteractor interactor, XRBaseInteractable interactable)
		{
			Debug.LogError("HoverCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverCancel(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverCancel((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.", this);
			throw new NotSupportedException("HoverCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverCancel(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverCancel((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.");
		}

		[Obsolete("SelectEnter(XRBaseInteractor, XRBaseInteractable, SelectEnterEventArgs) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable, SelectEnterEventArgs) instead.", true)]
		protected virtual void SelectEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectEnterEventArgs args)
		{
			Debug.LogError("SelectEnter(XRBaseInteractor, XRBaseInteractable, SelectEnterEventArgs) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable, SelectEnterEventArgs) instead.", this);
			throw new NotSupportedException("SelectEnter(XRBaseInteractor, XRBaseInteractable, SelectEnterEventArgs) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable, SelectEnterEventArgs) instead.");
		}

		[Obsolete("SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs) instead.", true)]
		protected virtual void SelectExit(XRBaseInteractor interactor, XRBaseInteractable interactable, SelectExitEventArgs args)
		{
			Debug.LogError("SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs) instead.", this);
			throw new NotSupportedException("SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs) instead.");
		}

		[Obsolete("HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs) instead.", true)]
		protected virtual void HoverEnter(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverEnterEventArgs args)
		{
			Debug.LogError("HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs) instead.", this);
			throw new NotSupportedException("HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs) instead.");
		}

		[Obsolete("HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs) instead.", true)]
		protected virtual void HoverExit(XRBaseInteractor interactor, XRBaseInteractable interactable, HoverExitEventArgs args)
		{
			Debug.LogError("HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs) instead.", this);
			throw new NotSupportedException("HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs) instead.");
		}

		[Obsolete("InteractorSelectValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorSelectValidTargets(IXRSelectInteractor, List<IXRInteractable>) instead.", true)]
		protected virtual void InteractorSelectValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
		{
			Debug.LogError("InteractorSelectValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorSelectValidTargets(IXRSelectInteractor, List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("InteractorSelectValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorSelectValidTargets(IXRSelectInteractor, List<IXRInteractable>) instead.");
		}

		[Obsolete("InteractorHoverValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorHoverValidTargets(IXRHoverInteractor, List<IXRInteractable>) instead.", true)]
		protected virtual void InteractorHoverValidTargets(XRBaseInteractor interactor, List<XRBaseInteractable> validTargets)
		{
			Debug.LogError("InteractorHoverValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorHoverValidTargets(IXRHoverInteractor, List<IXRInteractable>) instead.", this);
			throw new NotSupportedException("InteractorHoverValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorHoverValidTargets(IXRHoverInteractor, List<IXRInteractable>) instead.");
		}

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

		private readonly Dictionary<Collider, IXRInteractable> m_ColliderToInteractableMap = new Dictionary<Collider, IXRInteractable>();

		private readonly Dictionary<Collider, XRInteractableSnapVolume> m_ColliderToSnapVolumes = new Dictionary<Collider, XRInteractableSnapVolume>();

		private readonly RegistrationList<IXRInteractor> m_Interactors = new RegistrationList<IXRInteractor>();

		private readonly RegistrationList<IXRInteractionGroup> m_InteractionGroups = new RegistrationList<IXRInteractionGroup>();

		private readonly RegistrationList<IXRInteractable> m_Interactables = new RegistrationList<IXRInteractable>();

		private readonly List<IXRHoverInteractable> m_CurrentHovered = new List<IXRHoverInteractable>();

		private readonly List<IXRSelectInteractable> m_CurrentSelected = new List<IXRSelectInteractable>();

		private readonly Dictionary<IXRSelectInteractable, List<IXRTargetPriorityInteractor>> m_HighestPriorityTargetMap = new Dictionary<IXRSelectInteractable, List<IXRTargetPriorityInteractor>>();

		private static readonly LinkedPool<List<IXRTargetPriorityInteractor>> s_TargetPriorityInteractorListPool = new LinkedPool<List<IXRTargetPriorityInteractor>>(() => new List<IXRTargetPriorityInteractor>(), null, delegate(List<IXRTargetPriorityInteractor> list)
		{
			list.Clear();
		}, null, false, 10000);

		private readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();

		private readonly HashSet<IXRInteractable> m_UnorderedValidTargets = new HashSet<IXRInteractable>();

		private readonly HashSet<IXRInteractor> m_InteractorsInGroup = new HashSet<IXRInteractor>();

		private readonly HashSet<IXRInteractionGroup> m_GroupsInGroup = new HashSet<IXRInteractionGroup>();

		private readonly List<IXRInteractionGroup> m_ScratchInteractionGroups = new List<IXRInteractionGroup>();

		private readonly List<IXRInteractor> m_ScratchInteractors = new List<IXRInteractor>();

		private readonly LinkedPool<FocusEnterEventArgs> m_FocusEnterEventArgs = new LinkedPool<FocusEnterEventArgs>(() => new FocusEnterEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<FocusExitEventArgs> m_FocusExitEventArgs = new LinkedPool<FocusExitEventArgs>(() => new FocusExitEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<SelectEnterEventArgs> m_SelectEnterEventArgs = new LinkedPool<SelectEnterEventArgs>(() => new SelectEnterEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<SelectExitEventArgs> m_SelectExitEventArgs = new LinkedPool<SelectExitEventArgs>(() => new SelectExitEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<HoverEnterEventArgs> m_HoverEnterEventArgs = new LinkedPool<HoverEnterEventArgs>(() => new HoverEnterEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<HoverExitEventArgs> m_HoverExitEventArgs = new LinkedPool<HoverExitEventArgs>(() => new HoverExitEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractionGroupRegisteredEventArgs> m_InteractionGroupRegisteredEventArgs = new LinkedPool<InteractionGroupRegisteredEventArgs>(() => new InteractionGroupRegisteredEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractionGroupUnregisteredEventArgs> m_InteractionGroupUnregisteredEventArgs = new LinkedPool<InteractionGroupUnregisteredEventArgs>(() => new InteractionGroupUnregisteredEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractorRegisteredEventArgs> m_InteractorRegisteredEventArgs = new LinkedPool<InteractorRegisteredEventArgs>(() => new InteractorRegisteredEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractorUnregisteredEventArgs> m_InteractorUnregisteredEventArgs = new LinkedPool<InteractorUnregisteredEventArgs>(() => new InteractorUnregisteredEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractableRegisteredEventArgs> m_InteractableRegisteredEventArgs = new LinkedPool<InteractableRegisteredEventArgs>(() => new InteractableRegisteredEventArgs(), null, null, null, false, 10000);

		private readonly LinkedPool<InteractableUnregisteredEventArgs> m_InteractableUnregisteredEventArgs = new LinkedPool<InteractableUnregisteredEventArgs>(() => new InteractableUnregisteredEventArgs(), null, null, null, false, 10000);

		private static readonly ProfilerMarker s_PreprocessInteractorsMarker = new ProfilerMarker("XRI.PreprocessInteractors");

		private static readonly ProfilerMarker s_ProcessInteractionStrengthMarker = new ProfilerMarker("XRI.ProcessInteractionStrength");

		private static readonly ProfilerMarker s_ProcessInteractorsMarker = new ProfilerMarker("XRI.ProcessInteractors");

		private static readonly ProfilerMarker s_ProcessInteractablesMarker = new ProfilerMarker("XRI.ProcessInteractables");

		private static readonly ProfilerMarker s_UpdateGroupMemberInteractionsMarker = new ProfilerMarker("XRI.UpdateGroupMemberInteractions");

		internal static readonly ProfilerMarker s_GetValidTargetsMarker = new ProfilerMarker("XRI.GetValidTargets");

		private static readonly ProfilerMarker s_FilterRegisteredValidTargetsMarker = new ProfilerMarker("XRI.FilterRegisteredValidTargets");

		internal static readonly ProfilerMarker s_EvaluateInvalidFocusMarker = new ProfilerMarker("XRI.EvaluateInvalidFocus");

		internal static readonly ProfilerMarker s_EvaluateInvalidSelectionsMarker = new ProfilerMarker("XRI.EvaluateInvalidSelections");

		internal static readonly ProfilerMarker s_EvaluateInvalidHoversMarker = new ProfilerMarker("XRI.EvaluateInvalidHovers");

		internal static readonly ProfilerMarker s_EvaluateValidSelectionsMarker = new ProfilerMarker("XRI.EvaluateValidSelections");

		internal static readonly ProfilerMarker s_EvaluateValidHoversMarker = new ProfilerMarker("XRI.EvaluateValidHovers");

		private static readonly ProfilerMarker s_FocusEnterMarker = new ProfilerMarker("XRI.FocusEnter");

		private static readonly ProfilerMarker s_FocusExitMarker = new ProfilerMarker("XRI.FocusExit");

		private static readonly ProfilerMarker s_SelectEnterMarker = new ProfilerMarker("XRI.SelectEnter");

		private static readonly ProfilerMarker s_SelectExitMarker = new ProfilerMarker("XRI.SelectExit");

		private static readonly ProfilerMarker s_HoverEnterMarker = new ProfilerMarker("XRI.HoverEnter");

		private static readonly ProfilerMarker s_HoverExitMarker = new ProfilerMarker("XRI.HoverExit");

		private const string k_RegisterInteractorDeprecated = "RegisterInteractor(XRBaseInteractor) has been deprecated. Use RegisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractor((IXRInteractor)this)` instead.";

		private const string k_UnregisterInteractorDeprecated = "UnregisterInteractor(XRBaseInteractor) has been deprecated. Use UnregisterInteractor(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractor((IXRInteractor)this)` instead.";

		private const string k_RegisterInteractableDeprecated = "RegisterInteractable(XRBaseInteractable) has been deprecated. Use RegisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `RegisterInteractable((IXRInteractable)this)` instead.";

		private const string k_UnregisterInteractableDeprecated = "UnregisterInteractable(XRBaseInteractable) has been deprecated. Use UnregisterInteractable(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `UnregisterInteractable((IXRInteractable)this)` instead.";

		private const string k_GetRegisteredInteractorsDeprecated = "GetRegisteredInteractors(List<XRBaseInteractor>) has been deprecated. Use GetRegisteredInteractors(List<IXRInteractor>) instead.";

		private const string k_GetRegisteredInteractablesDeprecated = "GetRegisteredInteractables(List<XRBaseInteractable>) has been deprecated. Use GetRegisteredInteractables(List<IXRInteractable>) instead.";

		private const string k_IsRegisteredInteractorDeprecated = "IsRegistered(XRBaseInteractor) has been deprecated. Use IsRegistered(IXRInteractor) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractor)this)` instead.";

		private const string k_IsRegisteredInteractableDeprecated = "IsRegistered(XRBaseInteractable) has been deprecated. Use IsRegistered(IXRInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `IsRegistered((IXRInteractable)this)` instead.";

		private const string k_TryGetInteractableForColliderDeprecated = "TryGetInteractableForCollider has been deprecated. Use GetInteractableForCollider instead. (UnityUpgradable) -> GetInteractableForCollider(*)";

		private const string k_GetInteractableForColliderDeprecated = "GetInteractableForCollider has been deprecated. Use TryGetInteractableForCollider(Collider, out IXRInteractable) instead.";

		private const string k_GetColliderToInteractableMapDeprecated = "GetColliderToInteractableMap has been deprecated. The signature no longer matches the field used by the XRInteractionManager, so a copy is returned instead of a ref. Changes to the returned Dictionary will not be observed by the XRInteractionManager.";

		private const string k_GetValidTargetsDeprecated = "GetValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use GetValidTargets(IXRInteractor, List<IXRInteractable>) instead.";

		private const string k_ForceSelectDeprecated = "ForceSelect(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead.";

		private const string k_ClearInteractorSelectionDeprecated = "ClearInteractorSelection(XRBaseInteractor) has been deprecated. Use ClearInteractorSelection(IXRSelectInteractor, List<IXRInteractable>) instead.";

		private const string k_CancelInteractorSelectionDeprecated = "CancelInteractorSelection(XRBaseInteractor) has been deprecated. Use CancelInteractorSelection(IXRSelectInteractor) instead.";

		private const string k_CancelInteractableSelectionDeprecated = "CancelInteractableSelection(XRBaseInteractable) has been deprecated. Use CancelInteractableSelection(IXRSelectInteractable) instead.";

		private const string k_ClearInteractorHoverDeprecated = "ClearInteractorHover(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use ClearInteractorHover(IXRHoverInteractor, List<IXRInteractable>) instead.";

		private const string k_CancelInteractorHoverDeprecated = "CancelInteractorHover(XRBaseInteractor) has been deprecated. Use CancelInteractorHover(IXRHoverInteractor) instead.";

		private const string k_CancelInteractableHoverDeprecated = "CancelInteractableHover(XRBaseInteractable) has been deprecated. Use CancelInteractableHover(IXRHoverInteractable) instead.";

		private const string k_SelectEnterDeprecated = "SelectEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectEnter((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";

		private const string k_SelectExitDeprecated = "SelectExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectExit((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";

		private const string k_SelectCancelDeprecated = "SelectCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use SelectCancel(IXRSelectInteractor, IXRSelectInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `SelectCancel((IXRSelectInteractor)interactor, (IXRSelectInteractable)interactable)` instead.";

		private const string k_HoverEnterDeprecated = "HoverEnter(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverEnter((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";

		private const string k_HoverExitDeprecated = "HoverExit(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverExit((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";

		private const string k_HoverCancelDeprecated = "HoverCancel(XRBaseInteractor, XRBaseInteractable) has been deprecated. Use HoverCancel(IXRHoverInteractor, IXRHoverInteractable) instead. You may need to modify your code by casting the argument to call the intended method, such as `HoverCancel((IXRHoverInteractor)interactor, (IXRHoverInteractable)interactable)` instead.";

		private const string k_SelectEnterProtectedDeprecated = "SelectEnter(XRBaseInteractor, XRBaseInteractable, SelectEnterEventArgs) has been deprecated. Use SelectEnter(IXRSelectInteractor, IXRSelectInteractable, SelectEnterEventArgs) instead.";

		private const string k_SelectExitProtectedDeprecated = "SelectExit(XRBaseInteractor, XRBaseInteractable, SelectExitEventArgs) has been deprecated. Use SelectExit(IXRSelectInteractor, IXRSelectInteractable, SelectExitEventArgs) instead.";

		private const string k_HoverEnterProtectedDeprecated = "HoverEnter(XRBaseInteractor, XRBaseInteractable, HoverEnterEventArgs) has been deprecated. Use HoverEnter(IXRHoverInteractor, IXRHoverInteractable, HoverEnterEventArgs) instead.";

		private const string k_HoverExitProtectedDeprecated = "HoverExit(XRBaseInteractor, XRBaseInteractable, HoverExitEventArgs) has been deprecated. Use HoverExit(IXRHoverInteractor, IXRHoverInteractable, HoverExitEventArgs) instead.";

		private const string k_InteractorSelectValidTargetsDeprecated = "InteractorSelectValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorSelectValidTargets(IXRSelectInteractor, List<IXRInteractable>) instead.";

		private const string k_InteractorHoverValidTargetsDeprecated = "InteractorHoverValidTargets(XRBaseInteractor, List<XRBaseInteractable>) has been deprecated. Use InteractorHoverValidTargets(IXRHoverInteractor, List<IXRInteractable>) instead.";
	}
}
