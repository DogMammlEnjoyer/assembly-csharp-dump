using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit.Utilities;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Internal;

namespace UnityEngine.XR.Interaction.Toolkit.Interactors
{
	[MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
	[DisallowMultipleComponent]
	[AddComponentMenu("XR/XR Interaction Group", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Interactors.XRInteractionGroup.html")]
	[DefaultExecutionOrder(-100)]
	public class XRInteractionGroup : MonoBehaviour, IXRInteractionOverrideGroup, IXRInteractionGroup, IXRGroupMember
	{
		public event Action<InteractionGroupRegisteredEventArgs> registered;

		public event Action<InteractionGroupUnregisteredEventArgs> unregistered;

		public string groupName
		{
			get
			{
				return this.m_GroupName;
			}
		}

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

		public List<Object> startingGroupMembers
		{
			get
			{
				return this.m_StartingGroupMembers;
			}
			set
			{
				this.m_StartingGroupMembers = value;
				this.RemoveMissingMembersFromStartingOverridesMap();
			}
		}

		public IXRInteractor activeInteractor { get; private set; }

		public IXRInteractor focusInteractor { get; private set; }

		public IXRFocusInteractable focusInteractable { get; private set; }

		internal bool isRegisteredWithInteractionManager
		{
			get
			{
				return this.m_RegisteredInteractionManager != null;
			}
		}

		internal bool hasRegisteredStartingMembers { get; private set; }

		[Conditional("UNITY_EDITOR")]
		protected virtual void Reset()
		{
		}

		protected virtual void Awake()
		{
			this.FindCreateInteractionManager();
			this.RegisterWithInteractionManager();
			if (this.m_GroupMembers.flushedCount > 0)
			{
				int num = 0;
				using (List<Object>.Enumerator enumerator = this.m_StartingGroupMembers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Object @object = enumerator.Current;
						if (@object != null)
						{
							IXRGroupMember ixrgroupMember = @object as IXRGroupMember;
							if (ixrgroupMember != null)
							{
								this.MoveGroupMemberTo(ixrgroupMember, num++);
							}
						}
					}
					goto IL_B9;
				}
			}
			foreach (Object object2 in this.m_StartingGroupMembers)
			{
				if (object2 != null)
				{
					IXRGroupMember ixrgroupMember2 = object2 as IXRGroupMember;
					if (ixrgroupMember2 != null)
					{
						this.AddGroupMember(ixrgroupMember2);
					}
				}
			}
			IL_B9:
			if (string.IsNullOrWhiteSpace(this.m_GroupName))
			{
				this.m_GroupName = base.gameObject.name;
			}
			this.RemoveMissingMembersFromStartingOverridesMap();
			foreach (XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverridesPair in this.m_StartingInteractionOverridesMap)
			{
				Object groupMember = groupMemberAndOverridesPair.groupMember;
				if (!(groupMember == null))
				{
					IXRGroupMember ixrgroupMember3 = groupMember as IXRGroupMember;
					if (ixrgroupMember3 != null)
					{
						foreach (Object object3 in groupMemberAndOverridesPair.overrideGroupMembers)
						{
							if (object3 != null)
							{
								IXRGroupMember ixrgroupMember4 = object3 as IXRGroupMember;
								if (ixrgroupMember4 != null)
								{
									this.AddInteractionOverrideForGroupMember(ixrgroupMember3, ixrgroupMember4);
								}
							}
						}
					}
				}
			}
			this.hasRegisteredStartingMembers = true;
		}

		internal void RemoveMissingMembersFromStartingOverridesMap()
		{
			for (int i = this.m_StartingInteractionOverridesMap.Count - 1; i >= 0; i--)
			{
				XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverridesPair = this.m_StartingInteractionOverridesMap[i];
				if (!this.m_StartingGroupMembers.Contains(groupMemberAndOverridesPair.groupMember))
				{
					this.m_StartingInteractionOverridesMap.RemoveAt(i);
				}
				else
				{
					List<Object> overrideGroupMembers = groupMemberAndOverridesPair.overrideGroupMembers;
					for (int j = overrideGroupMembers.Count - 1; j >= 0; j--)
					{
						if (!this.m_StartingGroupMembers.Contains(overrideGroupMembers[j]))
						{
							overrideGroupMembers.RemoveAt(j);
						}
					}
				}
			}
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
			this.hasRegisteredStartingMembers = false;
			this.m_InteractionOverridesMap.Clear();
			this.ClearGroupMembers();
		}

		public void AddStartingInteractionOverride(Object sourceGroupMember, Object overrideGroupMember)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return;
			}
			if (overrideGroupMember == null)
			{
				Debug.LogError("overrideGroupMember cannot be null.");
				return;
			}
			if (!this.m_StartingGroupMembers.Contains(sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot add starting override group member for source member {0} ", sourceGroupMember) + string.Format("because {0} is not included in the starting group members.", sourceGroupMember), this);
				return;
			}
			if (!this.m_StartingGroupMembers.Contains(overrideGroupMember))
			{
				Debug.LogError(string.Format("Cannot add override group member {0} for source member ", overrideGroupMember) + string.Format("because {0} is not included in the starting group members.", overrideGroupMember), this);
				return;
			}
			XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverridesPair;
			if (this.TryGetStartingGroupMemberAndOverridesPair(sourceGroupMember, out groupMemberAndOverridesPair))
			{
				groupMemberAndOverridesPair.overrideGroupMembers.Add(overrideGroupMember);
				return;
			}
			this.m_StartingInteractionOverridesMap.Add(new XRInteractionGroup.GroupMemberAndOverridesPair
			{
				groupMember = sourceGroupMember,
				overrideGroupMembers = new List<Object>
				{
					overrideGroupMember
				}
			});
		}

		public bool RemoveStartingInteractionOverride(Object sourceGroupMember, Object overrideGroupMember)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return false;
			}
			XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverridesPair;
			return this.TryGetStartingGroupMemberAndOverridesPair(sourceGroupMember, out groupMemberAndOverridesPair) && groupMemberAndOverridesPair.overrideGroupMembers.Remove(overrideGroupMember);
		}

		private bool TryGetStartingGroupMemberAndOverridesPair(Object sourceGroupMember, out XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverrides)
		{
			if (sourceGroupMember == null)
			{
				groupMemberAndOverrides = null;
				return false;
			}
			foreach (XRInteractionGroup.GroupMemberAndOverridesPair groupMemberAndOverridesPair in this.m_StartingInteractionOverridesMap)
			{
				if (!(groupMemberAndOverridesPair.groupMember != sourceGroupMember))
				{
					groupMemberAndOverrides = groupMemberAndOverridesPair;
					return true;
				}
			}
			groupMemberAndOverrides = null;
			return false;
		}

		void IXRInteractionGroup.OnRegistered(InteractionGroupRegisteredEventArgs args)
		{
			if (args.manager != this.m_InteractionManager)
			{
				Debug.LogWarning("An Interaction Group was registered with an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was registered with \"{2}\".", this, this.m_InteractionManager, args.manager), this);
			}
			this.m_RegisteredInteractionManager = args.manager;
			this.m_GroupMembers.Flush();
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember) && ixrgroupMember.containingGroup == null)
				{
					this.RegisterAsGroupMember(ixrgroupMember);
				}
			}
			this.m_IsProcessingGroupMembers = false;
			Action<InteractionGroupRegisteredEventArgs> action = this.registered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		void IXRInteractionGroup.OnBeforeUnregistered()
		{
			this.m_GroupMembers.Flush();
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember))
				{
					this.RegisterAsNonGroupMember(ixrgroupMember);
				}
			}
			this.m_IsProcessingGroupMembers = false;
		}

		void IXRInteractionGroup.OnUnregistered(InteractionGroupUnregisteredEventArgs args)
		{
			if (args.manager != this.m_RegisteredInteractionManager)
			{
				Debug.LogWarning("An Interaction Group was unregistered from an unexpected XRInteractionManager." + string.Format(" {0} was expecting to communicate with \"{1}\" but was unregistered from \"{2}\".", this, this.m_RegisteredInteractionManager, args.manager), this);
			}
			this.m_RegisteredInteractionManager = null;
			Action<InteractionGroupUnregisteredEventArgs> action = this.unregistered;
			if (action == null)
			{
				return;
			}
			action(args);
		}

		public void AddGroupMember(IXRGroupMember groupMember)
		{
			if (groupMember == null)
			{
				throw new ArgumentNullException("groupMember");
			}
			if (!this.ValidateAddGroupMember(groupMember))
			{
				return;
			}
			if (this.m_IsProcessingGroupMembers)
			{
				Debug.LogWarning(string.Format("{0} added while {1} is processing Group members. It won't be processed until the next process.", groupMember, base.name), this);
			}
			if (this.m_GroupMembers.Register(groupMember))
			{
				this.RegisterAsGroupMember(groupMember);
			}
		}

		public void MoveGroupMemberTo(IXRGroupMember groupMember, int newIndex)
		{
			if (groupMember == null)
			{
				throw new ArgumentNullException("groupMember");
			}
			if (!this.ValidateAddGroupMember(groupMember))
			{
				return;
			}
			if (this.m_IsProcessingGroupMembers)
			{
				Debug.LogError(string.Format("Cannot move {0} while {1} is processing Group members.", groupMember, base.name), this);
				return;
			}
			this.m_GroupMembers.Flush();
			if (this.m_GroupMembers.MoveItemImmediately(groupMember, newIndex) && groupMember.containingGroup == null)
			{
				this.RegisterAsGroupMember(groupMember);
			}
		}

		private bool ValidateAddGroupMember(IXRGroupMember groupMember)
		{
			if (!(groupMember is IXRInteractor) && !(groupMember is IXRInteractionGroup))
			{
				Debug.LogError(string.Format("Group member {0} must be either an Interactor or an Interaction Group.", groupMember), this);
				return false;
			}
			if (groupMember.containingGroup != null && groupMember.containingGroup != this)
			{
				Debug.LogError(string.Format("Cannot add/move {0} because it is already part of a Group. Remove the member from the Group first.", groupMember), this);
				return false;
			}
			IXRInteractionGroup ixrinteractionGroup = groupMember as IXRInteractionGroup;
			if (ixrinteractionGroup != null && ixrinteractionGroup.HasDependencyOnGroup(this))
			{
				Debug.LogError(string.Format("Cannot add/move {0} because this would create a circular dependency of groups.", groupMember), this);
				return false;
			}
			return true;
		}

		public bool RemoveGroupMember(IXRGroupMember groupMember)
		{
			if (this.m_GroupMembers.Unregister(groupMember))
			{
				if (this.activeInteractor != null && this.GroupMemberIsOrContainsInteractor(groupMember, this.activeInteractor))
				{
					this.activeInteractor = null;
				}
				this.m_InteractionOverridesMap.Remove(groupMember);
				this.RegisterAsNonGroupMember(groupMember);
				return true;
			}
			return false;
		}

		private bool GroupMemberIsOrContainsInteractor(IXRGroupMember groupMember, IXRInteractor interactor)
		{
			if (groupMember == interactor)
			{
				return true;
			}
			IXRInteractionGroup ixrinteractionGroup = groupMember as IXRInteractionGroup;
			if (ixrinteractionGroup == null)
			{
				return false;
			}
			ixrinteractionGroup.GetGroupMembers(this.m_TempGroupMembers);
			foreach (IXRGroupMember groupMember2 in this.m_TempGroupMembers)
			{
				if (this.GroupMemberIsOrContainsInteractor(groupMember2, interactor))
				{
					return true;
				}
			}
			return false;
		}

		public void ClearGroupMembers()
		{
			this.m_GroupMembers.Flush();
			for (int i = this.m_GroupMembers.flushedCount - 1; i >= 0; i--)
			{
				IXRGroupMember registeredItemAt = this.m_GroupMembers.GetRegisteredItemAt(i);
				this.RemoveGroupMember(registeredItemAt);
			}
		}

		public bool ContainsGroupMember(IXRGroupMember groupMember)
		{
			return this.m_GroupMembers.IsRegistered(groupMember);
		}

		public void GetGroupMembers(List<IXRGroupMember> results)
		{
			if (results == null)
			{
				throw new ArgumentNullException("results");
			}
			this.m_GroupMembers.GetRegisteredItems(results);
		}

		public bool HasDependencyOnGroup(IXRInteractionGroup group)
		{
			if (group == this)
			{
				return true;
			}
			this.GetGroupMembers(this.m_TempGroupMembers);
			foreach (IXRGroupMember ixrgroupMember in this.m_TempGroupMembers)
			{
				IXRInteractionGroup ixrinteractionGroup = ixrgroupMember as IXRInteractionGroup;
				if (ixrinteractionGroup != null && ixrinteractionGroup.HasDependencyOnGroup(group))
				{
					return true;
				}
			}
			return false;
		}

		public void AddInteractionOverrideForGroupMember(IXRGroupMember sourceGroupMember, IXRGroupMember overrideGroupMember)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return;
			}
			if (overrideGroupMember == null)
			{
				Debug.LogError("overrideGroupMember cannot be null.");
				return;
			}
			if (!(overrideGroupMember is IXRSelectInteractor) && !(overrideGroupMember is IXRInteractionOverrideGroup))
			{
				Debug.LogError(string.Format("Override group member {0} must implement either ", overrideGroupMember) + "IXRSelectInteractor or IXRInteractionOverrideGroup.", this);
				return;
			}
			if (!this.ContainsGroupMember(sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot add override group member for source member {0} because {1} ", sourceGroupMember, sourceGroupMember) + "is not registered with the Group. Call AddGroupMember first.", this);
				return;
			}
			if (!this.ContainsGroupMember(overrideGroupMember))
			{
				Debug.LogError(string.Format("Cannot add override group member {0} for source member because {1} ", overrideGroupMember, overrideGroupMember) + "is not registered with the Group. Call AddGroupMember first.", this);
				return;
			}
			if (this.GroupMemberIsPartOfOverrideChain(overrideGroupMember, sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot add {0} as an override group member for {1} ", overrideGroupMember, sourceGroupMember) + "because this would create a loop of group member overrides.", this);
				return;
			}
			HashSet<IXRGroupMember> hashSet;
			if (this.m_InteractionOverridesMap.TryGetValue(sourceGroupMember, out hashSet))
			{
				hashSet.Add(overrideGroupMember);
				return;
			}
			this.m_InteractionOverridesMap[sourceGroupMember] = new HashSet<IXRGroupMember>
			{
				overrideGroupMember
			};
		}

		public bool GroupMemberIsPartOfOverrideChain(IXRGroupMember sourceGroupMember, IXRGroupMember potentialOverrideGroupMember)
		{
			if (potentialOverrideGroupMember == sourceGroupMember)
			{
				return true;
			}
			HashSet<IXRGroupMember> hashSet;
			if (!this.m_InteractionOverridesMap.TryGetValue(sourceGroupMember, out hashSet))
			{
				return false;
			}
			foreach (IXRGroupMember sourceGroupMember2 in hashSet)
			{
				if (this.GroupMemberIsPartOfOverrideChain(sourceGroupMember2, potentialOverrideGroupMember))
				{
					return true;
				}
			}
			return false;
		}

		public bool RemoveInteractionOverrideForGroupMember(IXRGroupMember sourceGroupMember, IXRGroupMember overrideGroupMember)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return false;
			}
			if (!this.ContainsGroupMember(sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot remove override group member for source member {0} because {1} ", sourceGroupMember, sourceGroupMember) + "is not registered with the Group.", this);
				return false;
			}
			HashSet<IXRGroupMember> hashSet;
			return this.m_InteractionOverridesMap.TryGetValue(sourceGroupMember, out hashSet) && hashSet.Remove(overrideGroupMember);
		}

		public bool ClearInteractionOverridesForGroupMember(IXRGroupMember sourceGroupMember)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return false;
			}
			if (!this.ContainsGroupMember(sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot clear override group members for source member {0} because {1} ", sourceGroupMember, sourceGroupMember) + "is not registered with the Group.", this);
				return false;
			}
			HashSet<IXRGroupMember> hashSet;
			if (!this.m_InteractionOverridesMap.TryGetValue(sourceGroupMember, out hashSet))
			{
				return false;
			}
			hashSet.Clear();
			return true;
		}

		public void GetInteractionOverridesForGroupMember(IXRGroupMember sourceGroupMember, HashSet<IXRGroupMember> results)
		{
			if (sourceGroupMember == null)
			{
				Debug.LogError("sourceGroupMember cannot be null.");
				return;
			}
			if (results == null)
			{
				Debug.LogError("results cannot be null.");
				return;
			}
			if (!this.ContainsGroupMember(sourceGroupMember))
			{
				Debug.LogError(string.Format("Cannot get override group members for source member {0} because {1} ", sourceGroupMember, sourceGroupMember) + "is not registered with the Group.", this);
				return;
			}
			results.Clear();
			HashSet<IXRGroupMember> other;
			if (this.m_InteractionOverridesMap.TryGetValue(sourceGroupMember, out other))
			{
				results.UnionWith(other);
			}
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
				this.m_InteractionManager.RegisterInteractionGroup(this);
			}
		}

		private void UnregisterWithInteractionManager()
		{
			if (this.m_RegisteredInteractionManager == null)
			{
				return;
			}
			this.m_RegisteredInteractionManager.UnregisterInteractionGroup(this);
		}

		private void RegisterAsGroupMember(IXRGroupMember groupMember)
		{
			if (this.m_RegisteredInteractionManager == null)
			{
				return;
			}
			groupMember.OnRegisteringAsGroupMember(this);
			this.ReRegisterGroupMemberWithInteractionManager(groupMember);
		}

		private void RegisterAsNonGroupMember(IXRGroupMember groupMember)
		{
			if (this.m_RegisteredInteractionManager == null)
			{
				return;
			}
			groupMember.OnRegisteringAsNonGroupMember();
			this.ReRegisterGroupMemberWithInteractionManager(groupMember);
		}

		private void ReRegisterGroupMemberWithInteractionManager(IXRGroupMember groupMember)
		{
			if (this.m_RegisteredInteractionManager == null)
			{
				return;
			}
			IXRInteractor ixrinteractor = groupMember as IXRInteractor;
			if (ixrinteractor == null)
			{
				IXRInteractionGroup ixrinteractionGroup = groupMember as IXRInteractionGroup;
				if (ixrinteractionGroup == null)
				{
					Debug.LogError(string.Format("Group member {0} must be either an Interactor or an Interaction Group.", groupMember), this);
				}
				else if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractionGroup))
				{
					this.m_RegisteredInteractionManager.UnregisterInteractionGroup(ixrinteractionGroup);
					this.m_RegisteredInteractionManager.RegisterInteractionGroup(ixrinteractionGroup);
					return;
				}
			}
			else if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractor))
			{
				this.m_RegisteredInteractionManager.UnregisterInteractor(ixrinteractor);
				this.m_RegisteredInteractionManager.RegisterInteractor(ixrinteractor);
				return;
			}
		}

		void IXRInteractionGroup.PreprocessGroupMembers(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			this.m_GroupMembers.Flush();
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember))
				{
					IXRInteractor ixrinteractor = ixrgroupMember as IXRInteractor;
					if (ixrinteractor == null)
					{
						IXRInteractionGroup ixrinteractionGroup = ixrgroupMember as IXRInteractionGroup;
						if (ixrinteractionGroup != null)
						{
							if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractionGroup))
							{
								ixrinteractionGroup.PreprocessGroupMembers(updatePhase);
							}
						}
					}
					else if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractor))
					{
						ixrinteractor.PreprocessInteractor(updatePhase);
					}
				}
			}
			this.m_IsProcessingGroupMembers = false;
		}

		void IXRInteractionGroup.ProcessGroupMembers(XRInteractionUpdateOrder.UpdatePhase updatePhase)
		{
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember))
				{
					IXRInteractor ixrinteractor = ixrgroupMember as IXRInteractor;
					if (ixrinteractor == null)
					{
						IXRInteractionGroup ixrinteractionGroup = ixrgroupMember as IXRInteractionGroup;
						if (ixrinteractionGroup != null)
						{
							if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractionGroup))
							{
								ixrinteractionGroup.ProcessGroupMembers(updatePhase);
							}
						}
					}
					else if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractor))
					{
						ixrinteractor.ProcessInteractor(updatePhase);
					}
				}
			}
			this.m_IsProcessingGroupMembers = false;
		}

		void IXRInteractionGroup.UpdateGroupMemberInteractions()
		{
			IXRInteractor prePrioritizedInteractor = null;
			if (this.activeInteractor != null && this.m_RegisteredInteractionManager.IsRegistered(this.activeInteractor))
			{
				IXRSelectInteractor ixrselectInteractor = this.activeInteractor as IXRSelectInteractor;
				if (ixrselectInteractor != null && this.CanStartOrContinueAnySelect(ixrselectInteractor))
				{
					prePrioritizedInteractor = this.activeInteractor;
				}
			}
			IXRInteractor activeInteractor;
			((IXRInteractionGroup)this).UpdateGroupMemberInteractions(prePrioritizedInteractor, out activeInteractor);
			this.activeInteractor = activeInteractor;
		}

		private bool CanStartOrContinueAnySelect(IXRSelectInteractor selectInteractor)
		{
			if (selectInteractor.keepSelectedTargetValid)
			{
				foreach (IXRSelectInteractable interactable in selectInteractor.interactablesSelected)
				{
					if (this.m_RegisteredInteractionManager.CanSelect(selectInteractor, interactable))
					{
						return true;
					}
				}
			}
			this.m_RegisteredInteractionManager.GetValidTargets(selectInteractor, this.m_ValidTargets);
			foreach (IXRInteractable ixrinteractable in this.m_ValidTargets)
			{
				IXRSelectInteractable ixrselectInteractable = ixrinteractable as IXRSelectInteractable;
				if (ixrselectInteractable != null && this.m_RegisteredInteractionManager.CanSelect(selectInteractor, ixrselectInteractable))
				{
					return true;
				}
			}
			return false;
		}

		void IXRInteractionGroup.UpdateGroupMemberInteractions(IXRInteractor prePrioritizedInteractor, out IXRInteractor interactorThatPerformedInteraction)
		{
			IXRSelectInteractor ixrselectInteractor;
			if (((IXRInteractionOverrideGroup)this).ShouldOverrideActiveInteraction(out ixrselectInteractor))
			{
				prePrioritizedInteractor = ixrselectInteractor;
			}
			interactorThatPerformedInteraction = null;
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember))
				{
					IXRInteractor ixrinteractor = ixrgroupMember as IXRInteractor;
					if (ixrinteractor == null)
					{
						IXRInteractionGroup ixrinteractionGroup = ixrgroupMember as IXRInteractionGroup;
						if (ixrinteractionGroup != null)
						{
							if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractionGroup))
							{
								IXRInteractor ixrinteractor2;
								ixrinteractionGroup.UpdateGroupMemberInteractions(prePrioritizedInteractor, out ixrinteractor2);
								if (ixrinteractor2 != null)
								{
									interactorThatPerformedInteraction = ixrinteractor2;
									prePrioritizedInteractor = ixrinteractor2;
								}
							}
						}
					}
					else if (this.m_RegisteredInteractionManager.IsRegistered(ixrinteractor))
					{
						bool preventInteraction = prePrioritizedInteractor != null && ixrinteractor != prePrioritizedInteractor;
						bool flag;
						this.UpdateInteractorInteractions(ixrinteractor, preventInteraction, out flag);
						if (flag)
						{
							interactorThatPerformedInteraction = ixrinteractor;
							prePrioritizedInteractor = ixrinteractor;
						}
					}
				}
			}
			this.m_IsProcessingGroupMembers = false;
			this.activeInteractor = interactorThatPerformedInteraction;
		}

		bool IXRInteractionOverrideGroup.ShouldOverrideActiveInteraction(out IXRSelectInteractor overridingInteractor)
		{
			overridingInteractor = null;
			HashSet<IXRGroupMember> hashSet;
			if (this.activeInteractor == null || !this.TryGetOverridesForContainedInteractor(this.activeInteractor, out hashSet))
			{
				return false;
			}
			bool result = false;
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember) && hashSet.Contains(ixrgroupMember) && this.ShouldGroupMemberOverrideInteraction(this.activeInteractor, ixrgroupMember, out overridingInteractor))
				{
					result = true;
					break;
				}
			}
			this.m_IsProcessingGroupMembers = false;
			return result;
		}

		private bool TryGetOverridesForContainedInteractor(IXRInteractor interactor, out HashSet<IXRGroupMember> overrideGroupMembers)
		{
			overrideGroupMembers = null;
			IXRGroupMember ixrgroupMember = interactor as IXRGroupMember;
			if (ixrgroupMember == null)
			{
				Debug.LogError(string.Format("Interactor {0} must be a {1}.", interactor, "IXRGroupMember"), this);
				return false;
			}
			IXRInteractionGroup ixrinteractionGroup = ixrgroupMember.containingGroup;
			IXRGroupMember key = ixrgroupMember;
			while (ixrinteractionGroup != null && ixrinteractionGroup != this)
			{
				IXRGroupMember ixrgroupMember2 = ixrinteractionGroup as IXRGroupMember;
				if (ixrgroupMember2 != null)
				{
					ixrinteractionGroup = ixrgroupMember2.containingGroup;
					key = ixrgroupMember2;
				}
				else
				{
					ixrinteractionGroup = null;
				}
			}
			if (ixrinteractionGroup == null)
			{
				Debug.LogError(string.Format("Interactor {0} must be contained by this group or one of its sub-groups.", interactor), this);
				return false;
			}
			return this.m_InteractionOverridesMap.TryGetValue(key, out overrideGroupMembers);
		}

		bool IXRInteractionOverrideGroup.ShouldAnyMemberOverrideInteraction(IXRInteractor interactingInteractor, out IXRSelectInteractor overridingInteractor)
		{
			overridingInteractor = null;
			bool result = false;
			this.m_IsProcessingGroupMembers = true;
			foreach (IXRGroupMember ixrgroupMember in this.m_GroupMembers.registeredSnapshot)
			{
				if (this.m_GroupMembers.IsStillRegistered(ixrgroupMember) && this.ShouldGroupMemberOverrideInteraction(interactingInteractor, ixrgroupMember, out overridingInteractor))
				{
					result = true;
					break;
				}
			}
			this.m_IsProcessingGroupMembers = false;
			return result;
		}

		private bool ShouldGroupMemberOverrideInteraction(IXRInteractor interactingInteractor, IXRGroupMember overrideGroupMember, out IXRSelectInteractor overridingInteractor)
		{
			overridingInteractor = null;
			IXRSelectInteractor ixrselectInteractor = overrideGroupMember as IXRSelectInteractor;
			if (ixrselectInteractor == null)
			{
				IXRInteractionOverrideGroup ixrinteractionOverrideGroup = overrideGroupMember as IXRInteractionOverrideGroup;
				if (ixrinteractionOverrideGroup != null)
				{
					if (!this.m_RegisteredInteractionManager.IsRegistered(ixrinteractionOverrideGroup))
					{
						return false;
					}
					if (ixrinteractionOverrideGroup.ShouldAnyMemberOverrideInteraction(interactingInteractor, out overridingInteractor))
					{
						return true;
					}
				}
			}
			else
			{
				if (!this.m_RegisteredInteractionManager.IsRegistered(ixrselectInteractor))
				{
					return false;
				}
				if (this.ShouldInteractorOverrideInteraction(interactingInteractor, ixrselectInteractor))
				{
					overridingInteractor = ixrselectInteractor;
					return true;
				}
			}
			return false;
		}

		private bool ShouldInteractorOverrideInteraction(IXRInteractor interactingInteractor, IXRSelectInteractor overridingInteractor)
		{
			IXRSelectInteractor ixrselectInteractor = interactingInteractor as IXRSelectInteractor;
			IXRHoverInteractor ixrhoverInteractor = interactingInteractor as IXRHoverInteractor;
			this.m_RegisteredInteractionManager.GetValidTargets(overridingInteractor, this.m_ValidTargets);
			foreach (IXRInteractable ixrinteractable in this.m_ValidTargets)
			{
				IXRSelectInteractable ixrselectInteractable = ixrinteractable as IXRSelectInteractable;
				if (ixrselectInteractable != null && this.m_RegisteredInteractionManager.CanSelect(overridingInteractor, ixrselectInteractable))
				{
					if (ixrselectInteractor != null && ixrselectInteractor.IsSelecting(ixrselectInteractable))
					{
						return true;
					}
					if (ixrhoverInteractor != null)
					{
						IXRHoverInteractable ixrhoverInteractable = ixrinteractable as IXRHoverInteractable;
						if (ixrhoverInteractable != null && ixrhoverInteractor.IsHovering(ixrhoverInteractable))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private void UpdateInteractorInteractions(IXRInteractor interactor, bool preventInteraction, out bool performedInteraction)
		{
			performedInteraction = false;
			using (XRInteractionManager.s_GetValidTargetsMarker.Auto())
			{
				this.m_RegisteredInteractionManager.GetValidTargets(interactor, this.m_ValidTargets);
			}
			IXRSelectInteractor ixrselectInteractor = interactor as IXRSelectInteractor;
			IXRHoverInteractor ixrhoverInteractor = interactor as IXRHoverInteractor;
			if (ixrselectInteractor != null)
			{
				using (XRInteractionManager.s_EvaluateInvalidSelectionsMarker.Auto())
				{
					if (preventInteraction)
					{
						this.ClearAllInteractorSelections(ixrselectInteractor);
					}
					else
					{
						this.m_RegisteredInteractionManager.ClearInteractorSelection(ixrselectInteractor, this.m_ValidTargets);
					}
				}
			}
			if (ixrhoverInteractor != null)
			{
				using (XRInteractionManager.s_EvaluateInvalidHoversMarker.Auto())
				{
					if (preventInteraction)
					{
						this.ClearAllInteractorHovers(ixrhoverInteractor);
					}
					else
					{
						this.m_RegisteredInteractionManager.ClearInteractorHover(ixrhoverInteractor, this.m_ValidTargets);
					}
				}
			}
			if (preventInteraction)
			{
				return;
			}
			if (ixrselectInteractor != null)
			{
				using (XRInteractionManager.s_EvaluateValidSelectionsMarker.Auto())
				{
					this.m_RegisteredInteractionManager.InteractorSelectValidTargets(ixrselectInteractor, this.m_ValidTargets);
				}
				if (!ixrselectInteractor.hasSelection)
				{
					IUIInteractor iuiinteractor = interactor as IUIInteractor;
					if (iuiinteractor == null || !TrackedDeviceGraphicRaycaster.IsPokeInteractingWithUI(iuiinteractor))
					{
						goto IL_116;
					}
				}
				performedInteraction = true;
			}
			IL_116:
			if (ixrhoverInteractor != null)
			{
				using (XRInteractionManager.s_EvaluateValidHoversMarker.Auto())
				{
					this.m_RegisteredInteractionManager.InteractorHoverValidTargets(ixrhoverInteractor, this.m_ValidTargets);
				}
				if (ixrhoverInteractor.hasHover)
				{
					performedInteraction = true;
				}
			}
		}

		private void ClearAllInteractorSelections(IXRSelectInteractor selectInteractor)
		{
			if (selectInteractor.interactablesSelected.Count == 0)
			{
				return;
			}
			XRInteractionGroup.s_InteractablesSelected.Clear();
			XRInteractionGroup.s_InteractablesSelected.AddRange(selectInteractor.interactablesSelected);
			for (int i = XRInteractionGroup.s_InteractablesSelected.Count - 1; i >= 0; i--)
			{
				IXRSelectInteractable interactable = XRInteractionGroup.s_InteractablesSelected[i];
				this.m_RegisteredInteractionManager.SelectExit(selectInteractor, interactable);
			}
		}

		private void ClearAllInteractorHovers(IXRHoverInteractor hoverInteractor)
		{
			if (hoverInteractor.interactablesHovered.Count == 0)
			{
				return;
			}
			XRInteractionGroup.s_InteractablesHovered.Clear();
			XRInteractionGroup.s_InteractablesHovered.AddRange(hoverInteractor.interactablesHovered);
			for (int i = XRInteractionGroup.s_InteractablesHovered.Count - 1; i >= 0; i--)
			{
				IXRHoverInteractable interactable = XRInteractionGroup.s_InteractablesHovered[i];
				this.m_RegisteredInteractionManager.HoverExit(hoverInteractor, interactable);
			}
		}

		public void OnFocusEntering(FocusEnterEventArgs args)
		{
			this.focusInteractable = args.interactableObject;
			this.focusInteractor = args.interactorObject;
		}

		public void OnFocusExiting(FocusExitEventArgs args)
		{
			if (this.focusInteractable == args.interactableObject)
			{
				this.focusInteractable = null;
				this.focusInteractor = null;
			}
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

		[SerializeField]
		[Tooltip("The name of the interaction group, which can be used to retrieve it from the Interaction Manager.")]
		private string m_GroupName;

		[SerializeField]
		[Tooltip("The XR Interaction Manager that this Interaction Group will communicate with (will find one if not set manually).")]
		private XRInteractionManager m_InteractionManager;

		private XRInteractionManager m_RegisteredInteractionManager;

		[SerializeField]
		[Tooltip("Ordered list of Interactors or Interaction Groups that are registered with the Group on Awake.")]
		[RequireInterface(typeof(IXRGroupMember))]
		private List<Object> m_StartingGroupMembers = new List<Object>();

		[SerializeField]
		[Tooltip("Configuration for each Group Member of which other Members are able to override its interaction when they attempt to select, despite the difference in priority order.")]
		private List<XRInteractionGroup.GroupMemberAndOverridesPair> m_StartingInteractionOverridesMap = new List<XRInteractionGroup.GroupMemberAndOverridesPair>();

		private readonly RegistrationList<IXRGroupMember> m_GroupMembers = new RegistrationList<IXRGroupMember>();

		private readonly List<IXRGroupMember> m_TempGroupMembers = new List<IXRGroupMember>();

		private bool m_IsProcessingGroupMembers;

		private readonly Dictionary<IXRGroupMember, HashSet<IXRGroupMember>> m_InteractionOverridesMap = new Dictionary<IXRGroupMember, HashSet<IXRGroupMember>>();

		private readonly List<IXRInteractable> m_ValidTargets = new List<IXRInteractable>();

		private static readonly List<IXRSelectInteractable> s_InteractablesSelected = new List<IXRSelectInteractable>();

		private static readonly List<IXRHoverInteractable> s_InteractablesHovered = new List<IXRHoverInteractable>();

		public static class GroupNames
		{
			public static readonly string k_Left = "Left";

			public static readonly string k_Right = "Right";

			public static readonly string k_Center = "Center";
		}

		[Serializable]
		internal class GroupMemberAndOverridesPair
		{
			[RequireInterface(typeof(IXRGroupMember))]
			public Object groupMember;

			[RequireInterface(typeof(IXRGroupMember))]
			public List<Object> overrideGroupMembers = new List<Object>();
		}
	}
}
