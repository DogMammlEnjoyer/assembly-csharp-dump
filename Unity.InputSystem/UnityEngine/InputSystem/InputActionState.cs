using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal class InputActionState : IInputStateChangeMonitor, ICloneable, IDisposable
	{
		public int totalCompositeCount
		{
			get
			{
				return this.memory.compositeCount;
			}
		}

		public int totalMapCount
		{
			get
			{
				return this.memory.mapCount;
			}
		}

		public int totalActionCount
		{
			get
			{
				return this.memory.actionCount;
			}
		}

		public int totalBindingCount
		{
			get
			{
				return this.memory.bindingCount;
			}
		}

		public int totalInteractionCount
		{
			get
			{
				return this.memory.interactionCount;
			}
		}

		public int totalControlCount
		{
			get
			{
				return this.memory.controlCount;
			}
		}

		public unsafe InputActionState.ActionMapIndices* mapIndices
		{
			get
			{
				return this.memory.mapIndices;
			}
		}

		public unsafe InputActionState.TriggerState* actionStates
		{
			get
			{
				return this.memory.actionStates;
			}
		}

		public unsafe InputActionState.BindingState* bindingStates
		{
			get
			{
				return this.memory.bindingStates;
			}
		}

		public unsafe InputActionState.InteractionState* interactionStates
		{
			get
			{
				return this.memory.interactionStates;
			}
		}

		public unsafe int* controlIndexToBindingIndex
		{
			get
			{
				return this.memory.controlIndexToBindingIndex;
			}
		}

		public unsafe ushort* controlGroupingAndComplexity
		{
			get
			{
				return this.memory.controlGroupingAndComplexity;
			}
		}

		public unsafe float* controlMagnitudes
		{
			get
			{
				return this.memory.controlMagnitudes;
			}
		}

		public unsafe uint* enabledControls
		{
			get
			{
				return (uint*)this.memory.enabledControls;
			}
		}

		public bool isProcessingControlStateChange
		{
			get
			{
				return this.m_InProcessControlStateChange;
			}
		}

		public void Initialize(InputBindingResolver resolver)
		{
			this.ClaimDataFrom(resolver);
			this.AddToGlobalList();
		}

		private unsafe void ComputeControlGroupingIfNecessary()
		{
			if (this.memory.controlGroupingInitialized)
			{
				return;
			}
			bool flag = !InputSystem.settings.shortcutKeysConsumeInput;
			uint num = 1U;
			for (int i = 0; i < this.totalControlCount; i++)
			{
				InputControl inputControl = this.controls[i];
				int num2 = this.controlIndexToBindingIndex[i];
				ref InputActionState.BindingState ptr = ref this.bindingStates[num2];
				int num3 = 1;
				if (ptr.isPartOfComposite && !flag)
				{
					int compositeOrCompositeBindingIndex = ptr.compositeOrCompositeBindingIndex;
					for (int j = compositeOrCompositeBindingIndex + 1; j < this.totalBindingCount; j++)
					{
						ref InputActionState.BindingState ptr2 = ref this.bindingStates[j];
						if (!ptr2.isPartOfComposite || ptr2.compositeOrCompositeBindingIndex != compositeOrCompositeBindingIndex)
						{
							break;
						}
						num3++;
					}
				}
				this.controlGroupingAndComplexity[i * 2 + 1] = (ushort)num3;
				if (this.controlGroupingAndComplexity[i * 2] == 0)
				{
					if (!flag)
					{
						for (int k = 0; k < this.totalControlCount; k++)
						{
							InputControl inputControl2 = this.controls[k];
							if (inputControl == inputControl2)
							{
								this.controlGroupingAndComplexity[k * 2] = (ushort)num;
							}
						}
					}
					this.controlGroupingAndComplexity[i * 2] = (ushort)num;
					num += 1U;
				}
			}
			this.memory.controlGroupingInitialized = true;
		}

		public void ClaimDataFrom(InputBindingResolver resolver)
		{
			this.totalProcessorCount = resolver.totalProcessorCount;
			this.maps = resolver.maps;
			this.interactions = resolver.interactions;
			this.processors = resolver.processors;
			this.composites = resolver.composites;
			this.controls = resolver.controls;
			this.memory = resolver.memory;
			resolver.memory = default(InputActionState.UnmanagedMemory);
			this.ComputeControlGroupingIfNecessary();
		}

		~InputActionState()
		{
			this.Destroy(true);
		}

		public void Dispose()
		{
			this.Destroy(false);
		}

		private unsafe void Destroy(bool isFinalizing = false)
		{
			if (!isFinalizing)
			{
				for (int i = 0; i < this.totalMapCount; i++)
				{
					InputActionMap inputActionMap = this.maps[i];
					if (inputActionMap.enabled)
					{
						this.DisableControls(i, this.mapIndices[i].controlStartIndex, this.mapIndices[i].controlCount);
					}
					if (inputActionMap.m_Asset != null)
					{
						inputActionMap.m_Asset.m_SharedStateForAllMaps = null;
					}
					inputActionMap.m_State = null;
					inputActionMap.m_MapIndexInState = -1;
					inputActionMap.m_EnabledActionsCount = 0;
					InputAction[] actions = inputActionMap.m_Actions;
					if (actions != null)
					{
						for (int j = 0; j < actions.Length; j++)
						{
							actions[j].m_ActionIndexInState = -1;
						}
					}
				}
				this.RemoveMapFromGlobalList();
			}
			this.memory.Dispose();
		}

		public InputActionState Clone()
		{
			return new InputActionState
			{
				maps = ArrayHelpers.Copy<InputActionMap>(this.maps),
				controls = ArrayHelpers.Copy<InputControl>(this.controls),
				interactions = ArrayHelpers.Copy<IInputInteraction>(this.interactions),
				processors = ArrayHelpers.Copy<InputProcessor>(this.processors),
				composites = ArrayHelpers.Copy<InputBindingComposite>(this.composites),
				totalProcessorCount = this.totalProcessorCount,
				memory = this.memory.Clone()
			};
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		private bool IsUsingDevice(InputDevice device)
		{
			bool flag = false;
			for (int i = 0; i < this.totalMapCount; i++)
			{
				ReadOnlyArray<InputDevice>? devices = this.maps[i].devices;
				if (devices == null)
				{
					flag = true;
				}
				else if (devices.Value.Contains(device))
				{
					return true;
				}
			}
			if (!flag)
			{
				return false;
			}
			for (int j = 0; j < this.totalControlCount; j++)
			{
				if (this.controls[j].device == device)
				{
					return true;
				}
			}
			return false;
		}

		private bool CanUseDevice(InputDevice device)
		{
			bool flag = false;
			for (int i = 0; i < this.totalMapCount; i++)
			{
				ReadOnlyArray<InputDevice>? devices = this.maps[i].devices;
				if (devices == null)
				{
					flag = true;
				}
				else if (devices.Value.Contains(device))
				{
					return true;
				}
			}
			if (!flag)
			{
				return false;
			}
			for (int j = 0; j < this.totalMapCount; j++)
			{
				InputBinding[] bindings = this.maps[j].m_Bindings;
				if (bindings != null)
				{
					int num = bindings.Length;
					for (int k = 0; k < num; k++)
					{
						if (InputControlPath.TryFindControl(device, bindings[k].effectivePath, 0) != null)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool HasEnabledActions()
		{
			for (int i = 0; i < this.totalMapCount; i++)
			{
				if (this.maps[i].enabled)
				{
					return true;
				}
			}
			return false;
		}

		private unsafe void FinishBindingCompositeSetups()
		{
			for (int i = 0; i < this.totalBindingCount; i++)
			{
				ref InputActionState.BindingState ptr = ref this.bindingStates[i];
				if (ptr.isComposite && ptr.compositeOrCompositeBindingIndex != -1)
				{
					InputBindingComposite inputBindingComposite = this.composites[ptr.compositeOrCompositeBindingIndex];
					InputBindingCompositeContext inputBindingCompositeContext = new InputBindingCompositeContext
					{
						m_State = this,
						m_BindingIndex = i
					};
					inputBindingComposite.CallFinishSetup(ref inputBindingCompositeContext);
				}
			}
		}

		internal unsafe void PrepareForBindingReResolution(bool needFullResolve, ref InputControlList<InputControl> activeControls, ref bool hasEnabledActions)
		{
			bool flag = false;
			for (int i = 0; i < this.totalMapCount; i++)
			{
				InputActionMap inputActionMap = this.maps[i];
				if (inputActionMap.enabled)
				{
					hasEnabledActions = true;
					if (needFullResolve)
					{
						this.DisableAllActions(inputActionMap);
					}
					else
					{
						foreach (InputAction inputAction in inputActionMap.actions)
						{
							if (inputAction.phase.IsInProgress())
							{
								if (inputAction.ActiveControlIsValid(inputAction.activeControl))
								{
									if (!flag)
									{
										activeControls = new InputControlList<InputControl>(Allocator.Temp, 0);
										activeControls.Resize(this.totalControlCount);
										flag = true;
									}
									ref InputActionState.TriggerState ptr = ref this.actionStates[inputAction.m_ActionIndexInState];
									int num = ptr.controlIndex;
									activeControls[num] = this.controls[num];
									InputActionState.BindingState bindingState = this.bindingStates[ptr.bindingIndex];
									for (int j = 0; j < bindingState.interactionCount; j++)
									{
										int num2 = bindingState.interactionStartIndex + j;
										if (this.interactionStates[num2].phase.IsInProgress())
										{
											num = this.interactionStates[num2].triggerControlIndex;
											if (inputAction.ActiveControlIsValid(this.controls[num]))
											{
												activeControls[num] = this.controls[num];
											}
											else
											{
												this.ResetInteractionState(num2);
											}
										}
									}
								}
								else
								{
									this.ResetActionState(inputAction.m_ActionIndexInState, InputActionPhase.Waiting, false);
								}
							}
						}
						this.DisableControls(inputActionMap);
					}
				}
				inputActionMap.ClearCachedActionData(!needFullResolve);
			}
			this.NotifyListenersOfActionChange(InputActionChange.BoundControlsAboutToChange);
		}

		public void FinishBindingResolution(bool hasEnabledActions, InputActionState.UnmanagedMemory oldMemory, InputControlList<InputControl> activeControls, bool isFullResolve)
		{
			this.FinishBindingCompositeSetups();
			if (hasEnabledActions)
			{
				this.RestoreActionStatesAfterReResolvingBindings(oldMemory, activeControls, isFullResolve);
				return;
			}
			this.NotifyListenersOfActionChange(InputActionChange.BoundControlsChanged);
		}

		private unsafe void RestoreActionStatesAfterReResolvingBindings(InputActionState.UnmanagedMemory oldState, InputControlList<InputControl> activeControls, bool isFullResolve)
		{
			for (int i = 0; i < this.totalActionCount; i++)
			{
				ref InputActionState.TriggerState ptr = ref oldState.actionStates[i];
				ref InputActionState.TriggerState ptr2 = ref this.actionStates[i];
				ptr2.lastCanceledInUpdate = ptr.lastCanceledInUpdate;
				ptr2.lastPerformedInUpdate = ptr.lastPerformedInUpdate;
				ptr2.lastCompletedInUpdate = ptr.lastCompletedInUpdate;
				ptr2.pressedInUpdate = ptr.pressedInUpdate;
				ptr2.releasedInUpdate = ptr.releasedInUpdate;
				ptr2.startTime = ptr.startTime;
				ptr2.framePerformed = ptr.framePerformed;
				ptr2.frameCompleted = ptr.frameCompleted;
				ptr2.framePressed = ptr.framePressed;
				ptr2.frameReleased = ptr.frameReleased;
				ptr2.bindingIndex = ptr.bindingIndex;
				if (ptr.phase != InputActionPhase.Disabled)
				{
					ptr2.phase = InputActionPhase.Waiting;
					if (isFullResolve)
					{
						this.maps[ptr2.mapIndex].m_EnabledActionsCount++;
					}
				}
			}
			for (int j = 0; j < this.totalBindingCount; j++)
			{
				ref InputActionState.BindingState ptr3 = ref this.memory.bindingStates[j];
				if (!ptr3.isPartOfComposite)
				{
					if (ptr3.isComposite)
					{
						int compositeOrCompositeBindingIndex = ptr3.compositeOrCompositeBindingIndex;
						if (oldState.compositeMagnitudes != null)
						{
							this.memory.compositeMagnitudes[compositeOrCompositeBindingIndex] = oldState.compositeMagnitudes[compositeOrCompositeBindingIndex];
						}
					}
					int actionIndex = ptr3.actionIndex;
					if (actionIndex != -1)
					{
						ref InputActionState.TriggerState ptr4 = ref this.actionStates[actionIndex];
						if (!ptr4.isDisabled)
						{
							ptr3.initialStateCheckPending = ptr3.wantsInitialStateCheck;
							this.EnableControls(ptr3.mapIndex, ptr3.controlStartIndex, ptr3.controlCount);
							if (!isFullResolve)
							{
								ref InputActionState.BindingState ptr5 = ref this.memory.bindingStates[j];
								ptr3.triggerEventIdForComposite = ptr5.triggerEventIdForComposite;
								ref InputActionState.TriggerState ptr6 = ref oldState.actionStates[actionIndex];
								if (j == ptr6.bindingIndex && ptr6.phase.IsInProgress() && activeControls.Count > 0 && activeControls[ptr6.controlIndex] != null)
								{
									InputControl inputControl = activeControls[ptr6.controlIndex];
									int num = this.FindControlIndexOnBinding(j, inputControl);
									if (num != -1)
									{
										ptr4.phase = ptr6.phase;
										ptr4.controlIndex = num;
										ptr4.magnitude = ptr6.magnitude;
										ptr4.interactionIndex = ptr6.interactionIndex;
										this.memory.controlMagnitudes[num] = ptr6.magnitude;
									}
									for (int k = 0; k < ptr3.interactionCount; k++)
									{
										ref InputActionState.InteractionState ptr7 = ref oldState.interactionStates[ptr5.interactionStartIndex + k];
										if (ptr7.phase.IsInProgress())
										{
											inputControl = activeControls[ptr7.triggerControlIndex];
											if (inputControl != null)
											{
												num = this.FindControlIndexOnBinding(j, inputControl);
												ref InputActionState.InteractionState ptr8 = ref this.interactionStates[ptr3.interactionStartIndex + k];
												ptr8.phase = ptr7.phase;
												ptr8.performedTime = ptr7.performedTime;
												ptr8.startTime = ptr7.startTime;
												ptr8.triggerControlIndex = num;
												if (ptr7.isTimerRunning)
												{
													InputActionState.TriggerState triggerState = new InputActionState.TriggerState
													{
														mapIndex = ptr3.mapIndex,
														controlIndex = num,
														bindingIndex = j,
														time = ptr7.timerStartTime,
														interactionIndex = ptr3.interactionStartIndex + k
													};
													this.StartTimeout(ptr7.timerDuration, ref triggerState);
													ptr8.totalTimeoutCompletionDone = ptr7.totalTimeoutCompletionDone;
													ptr8.totalTimeoutCompletionTimeRemaining = ptr7.totalTimeoutCompletionTimeRemaining;
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.HookOnBeforeUpdate();
			this.NotifyListenersOfActionChange(InputActionChange.BoundControlsChanged);
			if (isFullResolve && InputActionState.s_GlobalState.onActionChange.length > 0)
			{
				for (int l = 0; l < this.totalMapCount; l++)
				{
					InputActionMap inputActionMap = this.maps[l];
					if (inputActionMap.m_SingletonAction == null && inputActionMap.m_EnabledActionsCount == inputActionMap.m_Actions.LengthSafe<InputAction>())
					{
						InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionMapEnabled, inputActionMap);
					}
					else
					{
						foreach (InputAction inputAction in inputActionMap.actions)
						{
							if (inputAction.enabled)
							{
								InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionEnabled, inputAction);
							}
						}
					}
				}
			}
		}

		private unsafe bool IsActiveControl(int bindingIndex, int controlIndex)
		{
			ref InputActionState.BindingState ptr = ref this.bindingStates[bindingIndex];
			int actionIndex = ptr.actionIndex;
			if (actionIndex == -1)
			{
				return false;
			}
			if (this.actionStates[actionIndex].controlIndex == controlIndex)
			{
				return true;
			}
			for (int i = 0; i < ptr.interactionCount; i++)
			{
				if (this.interactionStates[this.bindingStates->interactionStartIndex + i].triggerControlIndex == controlIndex)
				{
					return true;
				}
			}
			return false;
		}

		private unsafe int FindControlIndexOnBinding(int bindingIndex, InputControl control)
		{
			int controlStartIndex = this.bindingStates[bindingIndex].controlStartIndex;
			int controlCount = this.bindingStates[bindingIndex].controlCount;
			for (int i = 0; i < controlCount; i++)
			{
				if (control == this.controls[controlStartIndex + i])
				{
					return controlStartIndex + i;
				}
			}
			return -1;
		}

		private unsafe void ResetActionStatesDrivenBy(InputDevice device)
		{
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				for (int i = 0; i < this.totalActionCount; i++)
				{
					InputActionState.TriggerState* ptr = this.actionStates + i;
					if (ptr->phase != InputActionPhase.Waiting && ptr->phase != InputActionPhase.Disabled)
					{
						if (ptr->isPassThrough)
						{
							if (!this.IsActionBoundToControlFromDevice(device, i))
							{
								goto IL_65;
							}
						}
						else
						{
							int controlIndex = ptr->controlIndex;
							if (controlIndex == -1 || this.controls[controlIndex].device != device)
							{
								goto IL_65;
							}
						}
						this.ResetActionState(i, InputActionPhase.Waiting, false);
					}
					IL_65:;
				}
			}
		}

		private unsafe bool IsActionBoundToControlFromDevice(InputDevice device, int actionIndex)
		{
			bool result = false;
			ushort num;
			ushort actionBindingStartIndexAndCount = this.GetActionBindingStartIndexAndCount(actionIndex, out num);
			for (int i = 0; i < (int)num; i++)
			{
				ushort num2 = this.memory.actionBindingIndices[(int)actionBindingStartIndexAndCount + i];
				int controlCount = this.bindingStates[num2].controlCount;
				int controlStartIndex = this.bindingStates[num2].controlStartIndex;
				for (int j = 0; j < controlCount; j++)
				{
					if (this.controls[controlStartIndex + j].device == device)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public unsafe void ResetActionState(int actionIndex, InputActionPhase toPhase = InputActionPhase.Waiting, bool hardReset = false)
		{
			InputActionState.TriggerState* ptr = this.actionStates + actionIndex;
			if (ptr->phase != InputActionPhase.Waiting && ptr->phase != InputActionPhase.Disabled)
			{
				ptr->time = InputState.currentTime;
				if (ptr->interactionIndex != -1)
				{
					int bindingIndex = ptr->bindingIndex;
					if (bindingIndex != -1)
					{
						int mapIndex = ptr->mapIndex;
						int interactionCount = this.bindingStates[bindingIndex].interactionCount;
						int interactionStartIndex = this.bindingStates[bindingIndex].interactionStartIndex;
						for (int i = 0; i < interactionCount; i++)
						{
							int interactionIndex = interactionStartIndex + i;
							this.ResetInteractionStateAndCancelIfNecessary(mapIndex, bindingIndex, interactionIndex, toPhase);
						}
					}
				}
				else if (ptr->phase != InputActionPhase.Canceled)
				{
					this.ChangePhaseOfAction(InputActionPhase.Canceled, ref this.actionStates[actionIndex], toPhase);
				}
			}
			ptr->phase = toPhase;
			ptr->controlIndex = -1;
			ushort num = this.memory.actionBindingIndicesAndCounts[actionIndex];
			ptr->bindingIndex = (int)((this.memory.actionBindingIndices != null) ? this.memory.actionBindingIndices[num] : 0);
			ptr->interactionIndex = -1;
			ptr->startTime = 0.0;
			ptr->time = 0.0;
			ptr->hasMultipleConcurrentActuations = false;
			ptr->inProcessing = false;
			ptr->isPressed = false;
			if (hardReset)
			{
				ptr->lastCanceledInUpdate = 0U;
				ptr->lastPerformedInUpdate = 0U;
				ptr->lastCompletedInUpdate = 0U;
				ptr->pressedInUpdate = 0U;
				ptr->releasedInUpdate = 0U;
				ptr->framePerformed = 0;
				ptr->frameCompleted = 0;
				ptr->framePressed = 0;
				ptr->frameReleased = 0;
			}
		}

		public unsafe ref InputActionState.TriggerState FetchActionState(InputAction action)
		{
			return ref this.actionStates[action.m_ActionIndexInState];
		}

		public unsafe InputActionState.ActionMapIndices FetchMapIndices(InputActionMap map)
		{
			return this.mapIndices[map.m_MapIndexInState];
		}

		public unsafe void EnableAllActions(InputActionMap map)
		{
			this.EnableControls(map);
			int mapIndexInState = map.m_MapIndexInState;
			int actionCount = this.mapIndices[mapIndexInState].actionCount;
			int actionStartIndex = this.mapIndices[mapIndexInState].actionStartIndex;
			for (int i = 0; i < actionCount; i++)
			{
				int num = actionStartIndex + i;
				InputActionState.TriggerState* ptr = this.actionStates + num;
				if (ptr->isDisabled)
				{
					ptr->phase = InputActionPhase.Waiting;
				}
				ptr->inProcessing = false;
			}
			map.m_EnabledActionsCount = actionCount;
			this.HookOnBeforeUpdate();
			if (map.m_SingletonAction != null)
			{
				InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionEnabled, map.m_SingletonAction);
				return;
			}
			InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionMapEnabled, map);
		}

		private unsafe void EnableControls(InputActionMap map)
		{
			int mapIndexInState = map.m_MapIndexInState;
			int controlCount = this.mapIndices[mapIndexInState].controlCount;
			int controlStartIndex = this.mapIndices[mapIndexInState].controlStartIndex;
			if (controlCount > 0)
			{
				this.EnableControls(mapIndexInState, controlStartIndex, controlCount);
			}
		}

		public unsafe void EnableSingleAction(InputAction action)
		{
			this.EnableControls(action);
			int actionIndexInState = action.m_ActionIndexInState;
			this.actionStates[actionIndexInState].phase = InputActionPhase.Waiting;
			action.m_ActionMap.m_EnabledActionsCount++;
			this.HookOnBeforeUpdate();
			InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionEnabled, action);
		}

		private unsafe void EnableControls(InputAction action)
		{
			int actionIndexInState = action.m_ActionIndexInState;
			int mapIndexInState = action.m_ActionMap.m_MapIndexInState;
			int bindingStartIndex = this.mapIndices[mapIndexInState].bindingStartIndex;
			int bindingCount = this.mapIndices[mapIndexInState].bindingCount;
			InputActionState.BindingState* bindingStates = this.memory.bindingStates;
			for (int i = 0; i < bindingCount; i++)
			{
				int num = bindingStartIndex + i;
				InputActionState.BindingState* ptr = bindingStates + num;
				if (ptr->actionIndex == actionIndexInState && !ptr->isPartOfComposite)
				{
					int controlCount = ptr->controlCount;
					if (controlCount != 0)
					{
						this.EnableControls(mapIndexInState, ptr->controlStartIndex, controlCount);
					}
				}
			}
		}

		public unsafe void DisableAllActions(InputActionMap map)
		{
			this.DisableControls(map);
			int mapIndexInState = map.m_MapIndexInState;
			int actionStartIndex = this.mapIndices[mapIndexInState].actionStartIndex;
			int actionCount = this.mapIndices[mapIndexInState].actionCount;
			bool flag = map.m_EnabledActionsCount == actionCount;
			for (int i = 0; i < actionCount; i++)
			{
				int num = actionStartIndex + i;
				if (this.actionStates[num].phase != InputActionPhase.Disabled)
				{
					this.ResetActionState(num, InputActionPhase.Disabled, false);
					if (!flag)
					{
						InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionDisabled, map.m_Actions[i]);
					}
				}
			}
			map.m_EnabledActionsCount = 0;
			if (map.m_SingletonAction != null)
			{
				InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionDisabled, map.m_SingletonAction);
				return;
			}
			if (flag)
			{
				InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionMapDisabled, map);
			}
		}

		public unsafe void DisableControls(InputActionMap map)
		{
			int mapIndexInState = map.m_MapIndexInState;
			int controlCount = this.mapIndices[mapIndexInState].controlCount;
			int controlStartIndex = this.mapIndices[mapIndexInState].controlStartIndex;
			if (controlCount > 0)
			{
				this.DisableControls(mapIndexInState, controlStartIndex, controlCount);
			}
		}

		public void DisableSingleAction(InputAction action)
		{
			this.DisableControls(action);
			this.ResetActionState(action.m_ActionIndexInState, InputActionPhase.Disabled, false);
			action.m_ActionMap.m_EnabledActionsCount--;
			InputActionState.NotifyListenersOfActionChange(InputActionChange.ActionDisabled, action);
		}

		private unsafe void DisableControls(InputAction action)
		{
			int actionIndexInState = action.m_ActionIndexInState;
			int mapIndexInState = action.m_ActionMap.m_MapIndexInState;
			int bindingStartIndex = this.mapIndices[mapIndexInState].bindingStartIndex;
			int bindingCount = this.mapIndices[mapIndexInState].bindingCount;
			InputActionState.BindingState* bindingStates = this.memory.bindingStates;
			for (int i = 0; i < bindingCount; i++)
			{
				int num = bindingStartIndex + i;
				InputActionState.BindingState* ptr = bindingStates + num;
				if (ptr->actionIndex == actionIndexInState && !ptr->isPartOfComposite)
				{
					int controlCount = ptr->controlCount;
					if (controlCount != 0)
					{
						this.DisableControls(mapIndexInState, ptr->controlStartIndex, controlCount);
					}
				}
			}
		}

		private unsafe void EnableControls(int mapIndex, int controlStartIndex, int numControls)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			for (int i = 0; i < numControls; i++)
			{
				int num = controlStartIndex + i;
				if (!this.IsControlEnabled(num))
				{
					int num2 = this.controlIndexToBindingIndex[num];
					long monitorIndex = this.ToCombinedMapAndControlAndBindingIndex(mapIndex, num, num2);
					InputActionState.BindingState* ptr = this.bindingStates + num2;
					if (ptr->wantsInitialStateCheck)
					{
						this.SetInitialStateCheckPending(ptr, true);
					}
					s_Manager.AddStateChangeMonitor(this.controls[num], this, monitorIndex, (uint)this.controlGroupingAndComplexity[num * 2]);
					this.SetControlEnabled(num, true);
				}
			}
		}

		private unsafe void DisableControls(int mapIndex, int controlStartIndex, int numControls)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			for (int i = 0; i < numControls; i++)
			{
				int num = controlStartIndex + i;
				if (this.IsControlEnabled(num))
				{
					int num2 = this.controlIndexToBindingIndex[num];
					long monitorIndex = this.ToCombinedMapAndControlAndBindingIndex(mapIndex, num, num2);
					InputActionState.BindingState* ptr = this.bindingStates + num2;
					if (ptr->wantsInitialStateCheck)
					{
						this.SetInitialStateCheckPending(ptr, false);
					}
					s_Manager.RemoveStateChangeMonitor(this.controls[num], this, monitorIndex);
					ptr->pressTime = 0.0;
					this.SetControlEnabled(num, false);
				}
			}
		}

		public unsafe void SetInitialStateCheckPending(int actionIndex, bool value = true)
		{
			int mapIndex = this.actionStates[actionIndex].mapIndex;
			int bindingStartIndex = this.mapIndices[mapIndex].bindingStartIndex;
			int bindingCount = this.mapIndices[mapIndex].bindingCount;
			for (int i = 0; i < bindingCount; i++)
			{
				ref InputActionState.BindingState ptr = ref this.bindingStates[bindingStartIndex + i];
				if (ptr.actionIndex == actionIndex && !ptr.isPartOfComposite)
				{
					ptr.initialStateCheckPending = value;
				}
			}
		}

		private unsafe void SetInitialStateCheckPending(InputActionState.BindingState* bindingStatePtr, bool value)
		{
			if (bindingStatePtr->isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = bindingStatePtr->compositeOrCompositeBindingIndex;
				this.bindingStates[compositeOrCompositeBindingIndex].initialStateCheckPending = value;
				return;
			}
			bindingStatePtr->initialStateCheckPending = value;
		}

		private unsafe bool IsControlEnabled(int controlIndex)
		{
			int num = controlIndex / 32;
			uint num2 = 1U << controlIndex % 32;
			return (this.enabledControls[num] & num2) > 0U;
		}

		private unsafe void SetControlEnabled(int controlIndex, bool state)
		{
			int num = controlIndex / 32;
			uint num2 = 1U << controlIndex % 32;
			if (state)
			{
				this.enabledControls[num] |= num2;
				return;
			}
			this.enabledControls[num] &= ~num2;
		}

		private void HookOnBeforeUpdate()
		{
			if (this.m_OnBeforeUpdateHooked)
			{
				return;
			}
			if (this.m_OnBeforeUpdateDelegate == null)
			{
				this.m_OnBeforeUpdateDelegate = new Action(this.OnBeforeInitialUpdate);
			}
			InputSystem.s_Manager.onBeforeUpdate += this.m_OnBeforeUpdateDelegate;
			this.m_OnBeforeUpdateHooked = true;
		}

		private void UnhookOnBeforeUpdate()
		{
			if (!this.m_OnBeforeUpdateHooked)
			{
				return;
			}
			InputSystem.s_Manager.onBeforeUpdate -= this.m_OnBeforeUpdateDelegate;
			this.m_OnBeforeUpdateHooked = false;
		}

		private unsafe void OnBeforeInitialUpdate()
		{
			if (InputState.currentUpdateType == InputUpdateType.BeforeRender)
			{
				return;
			}
			this.UnhookOnBeforeUpdate();
			double currentTime = InputState.currentTime;
			InputManager s_Manager = InputSystem.s_Manager;
			for (int i = 0; i < this.totalBindingCount; i++)
			{
				ref InputActionState.BindingState ptr = ref this.bindingStates[i];
				if (ptr.initialStateCheckPending)
				{
					ptr.initialStateCheckPending = false;
					int controlStartIndex = ptr.controlStartIndex;
					int controlCount = ptr.controlCount;
					bool isComposite = ptr.isComposite;
					bool flag = false;
					for (int j = 0; j < controlCount; j++)
					{
						int num = controlStartIndex + j;
						InputControl inputControl = this.controls[num];
						if (!this.IsActiveControl(i, num) && !inputControl.CheckStateIsAtDefault())
						{
							if (inputControl.IsValueConsideredPressed(inputControl.magnitude) && (ptr.pressTime == 0.0 || ptr.pressTime > currentTime))
							{
								ptr.pressTime = currentTime;
							}
							if (!isComposite || !flag)
							{
								s_Manager.SignalStateChangeMonitor(inputControl, this);
								flag = true;
							}
						}
					}
				}
			}
			s_Manager.FireStateChangeNotifications();
		}

		void IInputStateChangeMonitor.NotifyControlStateChanged(InputControl control, double time, InputEventPtr eventPtr, long mapControlAndBindingIndex)
		{
			int mapIndex;
			int controlIndex;
			int bindingIndex;
			this.SplitUpMapAndControlAndBindingIndex(mapControlAndBindingIndex, out mapIndex, out controlIndex, out bindingIndex);
			this.ProcessControlStateChange(mapIndex, controlIndex, bindingIndex, time, eventPtr);
		}

		void IInputStateChangeMonitor.NotifyTimerExpired(InputControl control, double time, long mapControlAndBindingIndex, int interactionIndex)
		{
			int mapIndex;
			int controlIndex;
			int bindingIndex;
			this.SplitUpMapAndControlAndBindingIndex(mapControlAndBindingIndex, out mapIndex, out controlIndex, out bindingIndex);
			this.ProcessTimeout(time, mapIndex, controlIndex, bindingIndex, interactionIndex);
		}

		private unsafe long ToCombinedMapAndControlAndBindingIndex(int mapIndex, int controlIndex, int bindingIndex)
		{
			ushort num = this.controlGroupingAndComplexity[controlIndex * 2 + 1];
			return (long)controlIndex | (long)bindingIndex << 24 | (long)mapIndex << 40 | (long)((long)((ulong)num) << 48);
		}

		private void SplitUpMapAndControlAndBindingIndex(long mapControlAndBindingIndex, out int mapIndex, out int controlIndex, out int bindingIndex)
		{
			controlIndex = (int)(mapControlAndBindingIndex & 16777215L);
			bindingIndex = (int)(mapControlAndBindingIndex >> 24 & 65535L);
			mapIndex = (int)(mapControlAndBindingIndex >> 40 & 255L);
		}

		internal static int GetComplexityFromMonitorIndex(long mapControlAndBindingIndex)
		{
			return (int)(mapControlAndBindingIndex >> 48 & 255L);
		}

		private unsafe void ProcessControlStateChange(int mapIndex, int controlIndex, int bindingIndex, double time, InputEventPtr eventPtr)
		{
			using (InputActionRebindingExtensions.DeferBindingResolution())
			{
				this.m_InProcessControlStateChange = true;
				this.m_CurrentlyProcessingThisEvent = eventPtr;
				try
				{
					InputActionState.BindingState* ptr = this.bindingStates + bindingIndex;
					int actionIndex = ptr->actionIndex;
					InputActionState.TriggerState triggerState = new InputActionState.TriggerState
					{
						mapIndex = mapIndex,
						controlIndex = controlIndex,
						bindingIndex = bindingIndex,
						interactionIndex = -1,
						time = time,
						startTime = time,
						isPassThrough = (actionIndex != -1 && this.actionStates[actionIndex].isPassThrough),
						isButton = (actionIndex != -1 && this.actionStates[actionIndex].isButton)
					};
					if (this.m_OnBeforeUpdateHooked)
					{
						ptr->initialStateCheckPending = false;
					}
					InputControl inputControl = this.controls[controlIndex];
					triggerState.magnitude = (inputControl.CheckStateIsAtDefault() ? 0f : inputControl.magnitude);
					this.controlMagnitudes[controlIndex] = triggerState.magnitude;
					if (inputControl.IsValueConsideredPressed(triggerState.magnitude) && (ptr->pressTime == 0.0 || ptr->pressTime > triggerState.time))
					{
						ptr->pressTime = triggerState.time;
					}
					bool flag = false;
					if (ptr->isPartOfComposite)
					{
						int compositeOrCompositeBindingIndex = ptr->compositeOrCompositeBindingIndex;
						InputActionState.BindingState* ptr2 = this.bindingStates + compositeOrCompositeBindingIndex;
						if (InputActionState.ShouldIgnoreInputOnCompositeBinding(ptr2, eventPtr))
						{
							return;
						}
						int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
						InputBindingCompositeContext inputBindingCompositeContext = new InputBindingCompositeContext
						{
							m_State = this,
							m_BindingIndex = compositeOrCompositeBindingIndex
						};
						triggerState.magnitude = this.composites[compositeOrCompositeBindingIndex2].EvaluateMagnitude(ref inputBindingCompositeContext);
						this.memory.compositeMagnitudes[compositeOrCompositeBindingIndex2] = triggerState.magnitude;
						int interactionCount = ptr2->interactionCount;
						if (interactionCount > 0)
						{
							flag = true;
							this.ProcessInteractions(ref triggerState, ptr2->interactionStartIndex, interactionCount);
						}
					}
					bool flag2 = this.IsConflictingInput(ref triggerState, actionIndex);
					ptr = this.bindingStates + triggerState.bindingIndex;
					if (!flag2)
					{
						this.ProcessButtonState(ref triggerState, actionIndex, ptr);
					}
					int interactionCount2 = ptr->interactionCount;
					if (interactionCount2 > 0 && !ptr->isPartOfComposite)
					{
						this.ProcessInteractions(ref triggerState, ptr->interactionStartIndex, interactionCount2);
					}
					else if (!flag && !flag2)
					{
						this.ProcessDefaultInteraction(ref triggerState, actionIndex);
					}
				}
				finally
				{
					this.m_InProcessControlStateChange = false;
					this.m_CurrentlyProcessingThisEvent = default(InputEventPtr);
				}
			}
		}

		private unsafe void ProcessButtonState(ref InputActionState.TriggerState trigger, int actionIndex, InputActionState.BindingState* bindingStatePtr)
		{
			InputControl inputControl = this.controls[trigger.controlIndex];
			float num = inputControl.isButton ? ((ButtonControl)inputControl).pressPointOrDefault : ButtonControl.s_GlobalDefaultButtonPressPoint;
			if (this.controlMagnitudes[trigger.controlIndex] <= num * ButtonControl.s_GlobalDefaultButtonReleaseThreshold)
			{
				bindingStatePtr->pressTime = 0.0;
			}
			float magnitude = trigger.magnitude;
			InputActionState.TriggerState* ptr = this.actionStates + actionIndex;
			if (!ptr->isPressed && magnitude >= num)
			{
				ptr->framePressed = Time.frameCount;
				ptr->pressedInUpdate = InputUpdate.s_UpdateStepCount;
				ptr->isPressed = true;
				return;
			}
			if (ptr->isPressed)
			{
				float num2 = num * ButtonControl.s_GlobalDefaultButtonReleaseThreshold;
				if (magnitude <= num2)
				{
					ptr->frameReleased = Time.frameCount;
					ptr->releasedInUpdate = InputUpdate.s_UpdateStepCount;
					ptr->isPressed = false;
				}
			}
		}

		private unsafe static bool ShouldIgnoreInputOnCompositeBinding(InputActionState.BindingState* binding, InputEvent* eventPtr)
		{
			if (eventPtr == null)
			{
				return false;
			}
			int eventId = eventPtr->eventId;
			if (eventId != 0 && binding->triggerEventIdForComposite == eventId)
			{
				return true;
			}
			binding->triggerEventIdForComposite = eventId;
			return false;
		}

		private unsafe bool IsConflictingInput(ref InputActionState.TriggerState trigger, int actionIndex)
		{
			InputActionState.TriggerState* ptr = this.actionStates + actionIndex;
			if (!ptr->mayNeedConflictResolution)
			{
				return false;
			}
			int num = trigger.controlIndex;
			if (this.bindingStates[trigger.bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[trigger.bindingIndex].compositeOrCompositeBindingIndex;
				num = this.bindingStates[compositeOrCompositeBindingIndex].controlStartIndex;
			}
			int num2 = ptr->controlIndex;
			if (this.bindingStates[ptr->bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex2 = this.bindingStates[ptr->bindingIndex].compositeOrCompositeBindingIndex;
				num2 = this.bindingStates[compositeOrCompositeBindingIndex2].controlStartIndex;
			}
			if (num2 == -1)
			{
				ptr->magnitude = trigger.magnitude;
				return false;
			}
			bool flag = num == num2 || this.controls[num] == this.controls[num2];
			if (trigger.magnitude > ptr->magnitude)
			{
				if (trigger.magnitude > 0f && !flag && ptr->magnitude > 0f)
				{
					ptr->hasMultipleConcurrentActuations = true;
				}
				ptr->magnitude = trigger.magnitude;
				return false;
			}
			if (trigger.magnitude < ptr->magnitude)
			{
				if (!flag)
				{
					if (trigger.magnitude > 0f)
					{
						ptr->hasMultipleConcurrentActuations = true;
					}
					return true;
				}
				if (!ptr->hasMultipleConcurrentActuations)
				{
					ptr->magnitude = trigger.magnitude;
					return false;
				}
				ushort num3;
				ushort actionBindingStartIndexAndCount = this.GetActionBindingStartIndexAndCount(actionIndex, out num3);
				float num4 = trigger.magnitude;
				int num5 = -1;
				int num6 = -1;
				int num7 = 0;
				for (int i = 0; i < (int)num3; i++)
				{
					ushort num8 = this.memory.actionBindingIndices[(int)actionBindingStartIndexAndCount + i];
					InputActionState.BindingState* ptr2 = this.memory.bindingStates + num8;
					if (ptr2->isComposite)
					{
						int controlStartIndex = ptr2->controlStartIndex;
						int compositeOrCompositeBindingIndex3 = ptr2->compositeOrCompositeBindingIndex;
						float num9 = this.memory.compositeMagnitudes[compositeOrCompositeBindingIndex3];
						if (num9 > 0f)
						{
							num7++;
						}
						if (num9 > num4)
						{
							num5 = controlStartIndex;
							num6 = this.controlIndexToBindingIndex[controlStartIndex];
							num4 = num9;
						}
					}
					else if (!ptr2->isPartOfComposite)
					{
						for (int j = 0; j < ptr2->controlCount; j++)
						{
							int num10 = ptr2->controlStartIndex + j;
							float num11 = this.memory.controlMagnitudes[num10];
							if (num11 > 0f)
							{
								num7++;
							}
							if (num11 > num4)
							{
								num5 = num10;
								num6 = (int)num8;
								num4 = num11;
							}
						}
					}
				}
				if (num7 <= 1)
				{
					ptr->hasMultipleConcurrentActuations = false;
				}
				if (num5 != -1)
				{
					trigger.controlIndex = num5;
					trigger.bindingIndex = num6;
					trigger.magnitude = num4;
					if (ptr->bindingIndex != num6)
					{
						if (ptr->interactionIndex != -1)
						{
							this.ResetInteractionState(ptr->interactionIndex);
						}
						InputActionState.BindingState* ptr3 = this.bindingStates + num6;
						int interactionCount = ptr3->interactionCount;
						int interactionStartIndex = ptr3->interactionStartIndex;
						for (int k = 0; k < interactionCount; k++)
						{
							if (this.interactionStates[interactionStartIndex + k].phase.IsInProgress())
							{
								ptr->interactionIndex = interactionStartIndex + k;
								trigger.interactionIndex = interactionStartIndex + k;
								break;
							}
						}
					}
					ptr->controlIndex = num5;
					ptr->bindingIndex = num6;
					ptr->magnitude = num4;
					return false;
				}
			}
			if (!flag && Mathf.Approximately(trigger.magnitude, ptr->magnitude))
			{
				if (trigger.magnitude > 0f)
				{
					ptr->hasMultipleConcurrentActuations = true;
				}
				return true;
			}
			return false;
		}

		private unsafe ushort GetActionBindingStartIndexAndCount(int actionIndex, out ushort bindingCount)
		{
			bindingCount = this.memory.actionBindingIndicesAndCounts[actionIndex * 2 + 1];
			return this.memory.actionBindingIndicesAndCounts[actionIndex * 2];
		}

		private unsafe void ProcessDefaultInteraction(ref InputActionState.TriggerState trigger, int actionIndex)
		{
			InputActionState.TriggerState* ptr = this.actionStates + actionIndex;
			switch (ptr->phase)
			{
			case InputActionPhase.Waiting:
				if (trigger.isPassThrough)
				{
					this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Waiting);
					return;
				}
				if (trigger.isButton)
				{
					float magnitude = trigger.magnitude;
					if (magnitude > 0f)
					{
						this.ChangePhaseOfAction(InputActionPhase.Started, ref trigger, InputActionPhase.Waiting);
					}
					ButtonControl buttonControl = this.controls[trigger.controlIndex] as ButtonControl;
					float num = (buttonControl != null) ? buttonControl.pressPointOrDefault : ButtonControl.s_GlobalDefaultButtonPressPoint;
					if (magnitude >= num)
					{
						this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Performed);
						return;
					}
				}
				else if (InputActionState.IsActuated(ref trigger, 0f))
				{
					this.ChangePhaseOfAction(InputActionPhase.Started, ref trigger, InputActionPhase.Waiting);
					this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Started);
					return;
				}
				break;
			case InputActionPhase.Started:
				if (ptr->isButton)
				{
					float magnitude2 = trigger.magnitude;
					ButtonControl buttonControl2 = this.controls[trigger.controlIndex] as ButtonControl;
					float num2 = (buttonControl2 != null) ? buttonControl2.pressPointOrDefault : ButtonControl.s_GlobalDefaultButtonPressPoint;
					if (magnitude2 >= num2)
					{
						this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Performed);
						return;
					}
					if (Mathf.Approximately(magnitude2, 0f))
					{
						this.ChangePhaseOfAction(InputActionPhase.Canceled, ref trigger, InputActionPhase.Waiting);
						return;
					}
				}
				else
				{
					if (!InputActionState.IsActuated(ref trigger, 0f))
					{
						this.ChangePhaseOfAction(InputActionPhase.Canceled, ref trigger, InputActionPhase.Waiting);
						return;
					}
					this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Started);
					return;
				}
				break;
			case InputActionPhase.Performed:
				if (ptr->isButton)
				{
					float magnitude3 = trigger.magnitude;
					ButtonControl buttonControl3 = this.controls[trigger.controlIndex] as ButtonControl;
					float num3 = (buttonControl3 != null) ? buttonControl3.pressPointOrDefault : ButtonControl.s_GlobalDefaultButtonPressPoint;
					if (Mathf.Approximately(0f, magnitude3))
					{
						this.ChangePhaseOfAction(InputActionPhase.Canceled, ref trigger, InputActionPhase.Waiting);
						return;
					}
					float num4 = num3 * ButtonControl.s_GlobalDefaultButtonReleaseThreshold;
					if (magnitude3 <= num4)
					{
						this.ChangePhaseOfAction(InputActionPhase.Started, ref trigger, InputActionPhase.Waiting);
						return;
					}
				}
				else if (ptr->isPassThrough)
				{
					this.ChangePhaseOfAction(InputActionPhase.Performed, ref trigger, InputActionPhase.Performed);
				}
				break;
			default:
				return;
			}
		}

		private unsafe void ProcessInteractions(ref InputActionState.TriggerState trigger, int interactionStartIndex, int interactionCount)
		{
			InputInteractionContext inputInteractionContext = new InputInteractionContext
			{
				m_State = this,
				m_TriggerState = trigger
			};
			for (int i = 0; i < interactionCount; i++)
			{
				int num = interactionStartIndex + i;
				InputActionState.InteractionState interactionState = this.interactionStates[num];
				IInputInteraction inputInteraction = this.interactions[num];
				inputInteractionContext.m_TriggerState.phase = interactionState.phase;
				inputInteractionContext.m_TriggerState.startTime = interactionState.startTime;
				inputInteractionContext.m_TriggerState.interactionIndex = num;
				inputInteraction.Process(ref inputInteractionContext);
			}
		}

		private unsafe void ProcessTimeout(double time, int mapIndex, int controlIndex, int bindingIndex, int interactionIndex)
		{
			ref InputActionState.InteractionState ptr = ref this.interactionStates[interactionIndex];
			InputInteractionContext inputInteractionContext = new InputInteractionContext
			{
				m_State = this,
				m_TriggerState = new InputActionState.TriggerState
				{
					phase = ptr.phase,
					time = time,
					mapIndex = mapIndex,
					controlIndex = controlIndex,
					bindingIndex = bindingIndex,
					interactionIndex = interactionIndex,
					startTime = ptr.startTime
				},
				timerHasExpired = true
			};
			ptr.isTimerRunning = false;
			ptr.totalTimeoutCompletionTimeRemaining = Mathf.Max(ptr.totalTimeoutCompletionTimeRemaining - ptr.timerDuration, 0f);
			ptr.timerDuration = 0f;
			this.interactions[interactionIndex].Process(ref inputInteractionContext);
		}

		internal unsafe void SetTotalTimeoutCompletionTime(float seconds, ref InputActionState.TriggerState trigger)
		{
			InputActionState.InteractionState* ptr = this.interactionStates + trigger.interactionIndex;
			ptr->totalTimeoutCompletionDone = 0f;
			ptr->totalTimeoutCompletionTimeRemaining = seconds;
		}

		internal unsafe void StartTimeout(float seconds, ref InputActionState.TriggerState trigger)
		{
			InputManager s_Manager = InputSystem.s_Manager;
			double time = trigger.time;
			InputControl control = this.controls[trigger.controlIndex];
			int interactionIndex = trigger.interactionIndex;
			long num = this.ToCombinedMapAndControlAndBindingIndex(trigger.mapIndex, trigger.controlIndex, trigger.bindingIndex);
			InputActionState.InteractionState* ptr = this.interactionStates + interactionIndex;
			if (ptr->isTimerRunning)
			{
				this.StopTimeout(interactionIndex);
			}
			s_Manager.AddStateChangeMonitorTimeout(control, this, time + (double)seconds, num, interactionIndex);
			ptr->isTimerRunning = true;
			ptr->timerStartTime = time;
			ptr->timerDuration = seconds;
			ptr->timerMonitorIndex = num;
		}

		private unsafe void StopTimeout(int interactionIndex)
		{
			ref InputActionState.InteractionState ptr = ref this.interactionStates[interactionIndex];
			InputSystem.s_Manager.RemoveStateChangeMonitorTimeout(this, ptr.timerMonitorIndex, interactionIndex);
			ptr.isTimerRunning = false;
			ptr.totalTimeoutCompletionDone += ptr.timerDuration;
			ptr.totalTimeoutCompletionTimeRemaining = Mathf.Max(ptr.totalTimeoutCompletionTimeRemaining - ptr.timerDuration, 0f);
			ptr.timerDuration = 0f;
			ptr.timerStartTime = 0.0;
			ptr.timerMonitorIndex = 0L;
		}

		internal unsafe void ChangePhaseOfInteraction(InputActionPhase newPhase, ref InputActionState.TriggerState trigger, InputActionPhase phaseAfterPerformed = InputActionPhase.Waiting, InputActionPhase phaseAfterCanceled = InputActionPhase.Waiting, bool processNextInteractionOnCancel = true)
		{
			int interactionIndex = trigger.interactionIndex;
			int bindingIndex = trigger.bindingIndex;
			InputActionPhase phaseAfterPerformedOrCanceled = InputActionPhase.Waiting;
			if (newPhase == InputActionPhase.Performed)
			{
				phaseAfterPerformedOrCanceled = phaseAfterPerformed;
			}
			else if (newPhase == InputActionPhase.Canceled)
			{
				phaseAfterPerformedOrCanceled = phaseAfterCanceled;
			}
			ref InputActionState.InteractionState ptr = ref this.interactionStates[interactionIndex];
			if (ptr.isTimerRunning)
			{
				this.StopTimeout(trigger.interactionIndex);
			}
			ptr.phase = newPhase;
			ptr.triggerControlIndex = trigger.controlIndex;
			ptr.startTime = trigger.startTime;
			if (newPhase == InputActionPhase.Performed)
			{
				ptr.performedTime = trigger.time;
			}
			int actionIndex = this.bindingStates[bindingIndex].actionIndex;
			if (actionIndex != -1)
			{
				if (this.actionStates[actionIndex].phase == InputActionPhase.Waiting)
				{
					if (!this.ChangePhaseOfAction(newPhase, ref trigger, phaseAfterPerformedOrCanceled))
					{
						return;
					}
				}
				else if (newPhase == InputActionPhase.Canceled && this.actionStates[actionIndex].interactionIndex == trigger.interactionIndex)
				{
					if (!this.ChangePhaseOfAction(newPhase, ref trigger, phaseAfterPerformedOrCanceled))
					{
						return;
					}
					if (!processNextInteractionOnCancel)
					{
						return;
					}
					int interactionStartIndex = this.bindingStates[bindingIndex].interactionStartIndex;
					int interactionCount = this.bindingStates[bindingIndex].interactionCount;
					int i = 0;
					while (i < interactionCount)
					{
						int num = interactionStartIndex + i;
						if (num != trigger.interactionIndex && (this.interactionStates[num].phase == InputActionPhase.Started || this.interactionStates[num].phase == InputActionPhase.Performed))
						{
							double startTime = this.interactionStates[num].startTime;
							InputActionState.TriggerState triggerState = new InputActionState.TriggerState
							{
								phase = InputActionPhase.Started,
								controlIndex = this.interactionStates[num].triggerControlIndex,
								bindingIndex = trigger.bindingIndex,
								interactionIndex = num,
								mapIndex = trigger.mapIndex,
								time = startTime,
								startTime = startTime
							};
							if (!this.ChangePhaseOfAction(InputActionPhase.Started, ref triggerState, phaseAfterPerformedOrCanceled))
							{
								return;
							}
							if (this.interactionStates[num].phase != InputActionPhase.Performed)
							{
								break;
							}
							triggerState = new InputActionState.TriggerState
							{
								phase = InputActionPhase.Performed,
								controlIndex = this.interactionStates[num].triggerControlIndex,
								bindingIndex = trigger.bindingIndex,
								interactionIndex = num,
								mapIndex = trigger.mapIndex,
								time = this.interactionStates[num].performedTime,
								startTime = startTime
							};
							if (!this.ChangePhaseOfAction(InputActionPhase.Performed, ref triggerState, phaseAfterPerformedOrCanceled))
							{
								return;
							}
							while (i < interactionCount)
							{
								num = interactionStartIndex + i;
								this.ResetInteractionState(num);
								i++;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				else if (this.actionStates[actionIndex].interactionIndex == trigger.interactionIndex)
				{
					if (!this.ChangePhaseOfAction(newPhase, ref trigger, phaseAfterPerformedOrCanceled))
					{
						return;
					}
					if (newPhase == InputActionPhase.Performed)
					{
						int interactionStartIndex2 = this.bindingStates[bindingIndex].interactionStartIndex;
						int interactionCount2 = this.bindingStates[bindingIndex].interactionCount;
						for (int j = 0; j < interactionCount2; j++)
						{
							int num2 = interactionStartIndex2 + j;
							if (num2 != trigger.interactionIndex)
							{
								this.ResetInteractionState(num2);
							}
						}
					}
				}
			}
			if (newPhase != InputActionPhase.Performed || actionIndex == -1 || this.actionStates[actionIndex].isPerformed || this.actionStates[actionIndex].interactionIndex == trigger.interactionIndex)
			{
				if (newPhase == InputActionPhase.Performed && phaseAfterPerformed != InputActionPhase.Waiting)
				{
					ptr.phase = phaseAfterPerformed;
					return;
				}
				if (newPhase == InputActionPhase.Performed || newPhase == InputActionPhase.Canceled)
				{
					this.ResetInteractionState(trigger.interactionIndex);
				}
			}
		}

		private unsafe bool ChangePhaseOfAction(InputActionPhase newPhase, ref InputActionState.TriggerState trigger, InputActionPhase phaseAfterPerformedOrCanceled = InputActionPhase.Waiting)
		{
			int actionIndex = this.bindingStates[trigger.bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return true;
			}
			InputActionState.TriggerState* ptr = this.actionStates + actionIndex;
			if (ptr->isDisabled)
			{
				return true;
			}
			ptr->inProcessing = true;
			try
			{
				if (ptr->isPassThrough && trigger.interactionIndex == -1)
				{
					this.ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger, newPhase == InputActionPhase.Canceled && phaseAfterPerformedOrCanceled == InputActionPhase.Disabled);
					if (!ptr->inProcessing)
					{
						return false;
					}
				}
				else if (newPhase == InputActionPhase.Performed && ptr->phase == InputActionPhase.Waiting)
				{
					this.ChangePhaseOfActionInternal(actionIndex, ptr, InputActionPhase.Started, ref trigger, false);
					if (!ptr->inProcessing)
					{
						return false;
					}
					this.ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger, false);
					if (!ptr->inProcessing)
					{
						return false;
					}
					if (phaseAfterPerformedOrCanceled == InputActionPhase.Waiting)
					{
						this.ChangePhaseOfActionInternal(actionIndex, ptr, InputActionPhase.Canceled, ref trigger, false);
					}
					if (!ptr->inProcessing)
					{
						return false;
					}
					ptr->phase = phaseAfterPerformedOrCanceled;
				}
				else if (ptr->phase != newPhase || newPhase == InputActionPhase.Performed)
				{
					this.ChangePhaseOfActionInternal(actionIndex, ptr, newPhase, ref trigger, newPhase == InputActionPhase.Canceled && phaseAfterPerformedOrCanceled == InputActionPhase.Disabled);
					if (!ptr->inProcessing)
					{
						return false;
					}
					if (newPhase == InputActionPhase.Performed || newPhase == InputActionPhase.Canceled)
					{
						ptr->phase = phaseAfterPerformedOrCanceled;
					}
				}
			}
			finally
			{
				ptr->inProcessing = false;
			}
			if (ptr->phase == InputActionPhase.Waiting)
			{
				ptr->controlIndex = -1;
				ptr->flags &= ~InputActionState.TriggerState.Flags.HaveMagnitude;
			}
			return true;
		}

		private unsafe void ChangePhaseOfActionInternal(int actionIndex, InputActionState.TriggerState* actionState, InputActionPhase newPhase, ref InputActionState.TriggerState trigger, bool isDisablingAction = false)
		{
			InputActionState.TriggerState triggerState = trigger;
			triggerState.flags = actionState->flags;
			if (newPhase != InputActionPhase.Canceled)
			{
				triggerState.magnitude = trigger.magnitude;
			}
			else
			{
				triggerState.magnitude = 0f;
			}
			triggerState.phase = newPhase;
			if (newPhase == InputActionPhase.Performed)
			{
				triggerState.framePerformed = Time.frameCount;
				triggerState.lastPerformedInUpdate = InputUpdate.s_UpdateStepCount;
				triggerState.lastCanceledInUpdate = actionState->lastCanceledInUpdate;
				if (this.controlGroupingAndComplexity[trigger.controlIndex * 2 + 1] > 1 && this.m_CurrentlyProcessingThisEvent.valid)
				{
					this.m_CurrentlyProcessingThisEvent.handled = true;
				}
			}
			else if (newPhase == InputActionPhase.Canceled)
			{
				triggerState.lastCanceledInUpdate = InputUpdate.s_UpdateStepCount;
				triggerState.lastPerformedInUpdate = actionState->lastPerformedInUpdate;
				triggerState.framePerformed = actionState->framePerformed;
			}
			else
			{
				triggerState.lastPerformedInUpdate = actionState->lastPerformedInUpdate;
				triggerState.framePerformed = actionState->framePerformed;
				triggerState.lastCanceledInUpdate = actionState->lastCanceledInUpdate;
			}
			if (actionState->phase == InputActionPhase.Performed && newPhase != InputActionPhase.Performed && !isDisablingAction)
			{
				triggerState.frameCompleted = Time.frameCount;
				triggerState.lastCompletedInUpdate = InputUpdate.s_UpdateStepCount;
			}
			else
			{
				triggerState.lastCompletedInUpdate = actionState->lastCompletedInUpdate;
				triggerState.frameCompleted = actionState->frameCompleted;
			}
			triggerState.pressedInUpdate = actionState->pressedInUpdate;
			triggerState.framePressed = actionState->framePressed;
			triggerState.releasedInUpdate = actionState->releasedInUpdate;
			triggerState.frameReleased = actionState->frameReleased;
			if (newPhase == InputActionPhase.Started)
			{
				triggerState.startTime = triggerState.time;
			}
			*actionState = triggerState;
			InputActionMap inputActionMap = this.maps[trigger.mapIndex];
			InputAction inputAction = inputActionMap.m_Actions[actionIndex - this.mapIndices[trigger.mapIndex].actionStartIndex];
			trigger.phase = newPhase;
			switch (newPhase)
			{
			case InputActionPhase.Started:
				this.CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnStarted, "started");
				return;
			case InputActionPhase.Performed:
				this.CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnPerformed, "performed");
				return;
			case InputActionPhase.Canceled:
				this.CallActionListeners(actionIndex, inputActionMap, newPhase, ref inputAction.m_OnCanceled, "canceled");
				return;
			default:
				return;
			}
		}

		private void CallActionListeners(int actionIndex, InputActionMap actionMap, InputActionPhase phase, ref CallbackArray<Action<InputAction.CallbackContext>> listeners, string callbackName)
		{
			CallbackArray<Action<InputAction.CallbackContext>> actionCallbacks = actionMap.m_ActionCallbacks;
			if (listeners.length == 0 && actionCallbacks.length == 0 && InputActionState.s_GlobalState.onActionChange.length == 0)
			{
				return;
			}
			InputAction.CallbackContext argument = new InputAction.CallbackContext
			{
				m_State = this,
				m_ActionIndex = actionIndex
			};
			InputAction action = argument.action;
			if (InputActionState.s_GlobalState.onActionChange.length > 0)
			{
				InputActionChange argument2;
				switch (phase)
				{
				case InputActionPhase.Started:
					argument2 = InputActionChange.ActionStarted;
					break;
				case InputActionPhase.Performed:
					argument2 = InputActionChange.ActionPerformed;
					break;
				case InputActionPhase.Canceled:
					argument2 = InputActionChange.ActionCanceled;
					break;
				default:
					return;
				}
				DelegateHelpers.InvokeCallbacksSafe<object, InputActionChange>(ref InputActionState.s_GlobalState.onActionChange, action, argument2, InputActionState.k_InputOnActionChangeMarker, "InputSystem.onActionChange", null);
			}
			DelegateHelpers.InvokeCallbacksSafe<InputAction.CallbackContext>(ref listeners, argument, callbackName, action);
			DelegateHelpers.InvokeCallbacksSafe<InputAction.CallbackContext>(ref actionCallbacks, argument, callbackName, actionMap);
		}

		private object GetActionOrNoneString(ref InputActionState.TriggerState trigger)
		{
			InputAction actionOrNull = this.GetActionOrNull(ref trigger);
			if (actionOrNull == null)
			{
				return "<none>";
			}
			return actionOrNull;
		}

		internal unsafe InputAction GetActionOrNull(int bindingIndex)
		{
			int actionIndex = this.bindingStates[bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return null;
			}
			int mapIndex = this.bindingStates[bindingIndex].mapIndex;
			int actionStartIndex = this.mapIndices[mapIndex].actionStartIndex;
			return this.maps[mapIndex].m_Actions[actionIndex - actionStartIndex];
		}

		internal unsafe InputAction GetActionOrNull(ref InputActionState.TriggerState trigger)
		{
			int actionIndex = this.bindingStates[trigger.bindingIndex].actionIndex;
			if (actionIndex == -1)
			{
				return null;
			}
			int actionStartIndex = this.mapIndices[trigger.mapIndex].actionStartIndex;
			return this.maps[trigger.mapIndex].m_Actions[actionIndex - actionStartIndex];
		}

		internal InputControl GetControl(ref InputActionState.TriggerState trigger)
		{
			return this.controls[trigger.controlIndex];
		}

		private IInputInteraction GetInteractionOrNull(ref InputActionState.TriggerState trigger)
		{
			if (trigger.interactionIndex == -1)
			{
				return null;
			}
			return this.interactions[trigger.interactionIndex];
		}

		internal unsafe int GetBindingIndexInMap(int bindingIndex)
		{
			int mapIndex = this.bindingStates[bindingIndex].mapIndex;
			int bindingStartIndex = this.mapIndices[mapIndex].bindingStartIndex;
			return bindingIndex - bindingStartIndex;
		}

		internal unsafe int GetBindingIndexInState(int mapIndex, int bindingIndexInMap)
		{
			return this.mapIndices[mapIndex].bindingStartIndex + bindingIndexInMap;
		}

		internal unsafe ref InputActionState.BindingState GetBindingState(int bindingIndex)
		{
			return ref this.bindingStates[bindingIndex];
		}

		internal unsafe ref InputBinding GetBinding(int bindingIndex)
		{
			int mapIndex = this.bindingStates[bindingIndex].mapIndex;
			int bindingStartIndex = this.mapIndices[mapIndex].bindingStartIndex;
			return ref this.maps[mapIndex].m_Bindings[bindingIndex - bindingStartIndex];
		}

		internal unsafe InputActionMap GetActionMap(int bindingIndex)
		{
			int mapIndex = this.bindingStates[bindingIndex].mapIndex;
			return this.maps[mapIndex];
		}

		private unsafe void ResetInteractionStateAndCancelIfNecessary(int mapIndex, int bindingIndex, int interactionIndex, InputActionPhase phaseAfterCanceled)
		{
			int actionIndex = this.bindingStates[bindingIndex].actionIndex;
			if (this.actionStates[actionIndex].interactionIndex == interactionIndex)
			{
				InputActionPhase phase = this.interactionStates[interactionIndex].phase;
				if (phase - InputActionPhase.Started <= 1)
				{
					this.ChangePhaseOfInteraction(InputActionPhase.Canceled, ref this.actionStates[actionIndex], InputActionPhase.Waiting, phaseAfterCanceled, false);
				}
				this.actionStates[actionIndex].interactionIndex = -1;
			}
			this.ResetInteractionState(interactionIndex);
		}

		private unsafe void ResetInteractionState(int interactionIndex)
		{
			this.interactions[interactionIndex].Reset();
			if (this.interactionStates[interactionIndex].isTimerRunning)
			{
				this.StopTimeout(interactionIndex);
			}
			this.interactionStates[interactionIndex] = new InputActionState.InteractionState
			{
				phase = InputActionPhase.Waiting,
				triggerControlIndex = -1
			};
		}

		internal unsafe int GetValueSizeInBytes(int bindingIndex, int controlIndex)
		{
			if (this.bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				return this.composites[compositeOrCompositeBindingIndex2].valueSizeInBytes;
			}
			return this.controls[controlIndex].valueSizeInBytes;
		}

		internal unsafe Type GetValueType(int bindingIndex, int controlIndex)
		{
			if (this.bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				return this.composites[compositeOrCompositeBindingIndex2].valueType;
			}
			return this.controls[controlIndex].valueType;
		}

		internal static bool IsActuated(ref InputActionState.TriggerState trigger, float threshold = 0f)
		{
			float magnitude = trigger.magnitude;
			if (magnitude < 0f)
			{
				return true;
			}
			if (Mathf.Approximately(threshold, 0f))
			{
				return magnitude > 0f;
			}
			return magnitude >= threshold;
		}

		internal unsafe void ReadValue(int bindingIndex, int controlIndex, void* buffer, int bufferSize, bool ignoreComposites = false)
		{
			InputControl control = null;
			if (!ignoreComposites && this.bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite inputBindingComposite = this.composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext inputBindingCompositeContext = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				inputBindingComposite.ReadValue(ref inputBindingCompositeContext, buffer, bufferSize);
				bindingIndex = compositeOrCompositeBindingIndex;
			}
			else
			{
				control = this.controls[controlIndex];
				control.ReadValueIntoBuffer(buffer, bufferSize);
			}
			int processorCount = this.bindingStates[bindingIndex].processorCount;
			if (processorCount > 0)
			{
				int processorStartIndex = this.bindingStates[bindingIndex].processorStartIndex;
				for (int i = 0; i < processorCount; i++)
				{
					this.processors[processorStartIndex + i].Process(buffer, bufferSize, control);
				}
			}
		}

		internal unsafe TValue ReadValue<TValue>(int bindingIndex, int controlIndex, bool ignoreComposites = false) where TValue : struct
		{
			TValue value = default(TValue);
			InputControl<TValue> inputControl = null;
			if (!ignoreComposites && this.bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite inputBindingComposite = this.composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext inputBindingCompositeContext = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				InputBindingComposite<TValue> inputBindingComposite2 = inputBindingComposite as InputBindingComposite<TValue>;
				if (inputBindingComposite2 == null)
				{
					Type valueType = inputBindingComposite.valueType;
					if (!valueType.IsAssignableFrom(typeof(TValue)))
					{
						throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from composite '{1}' bound to action '{2}' (composite is a '{3}' with value type '{4}')", new object[]
						{
							typeof(TValue).Name,
							inputBindingComposite,
							this.GetActionOrNull(bindingIndex),
							compositeOrCompositeBindingIndex2.GetType().Name,
							valueType.GetNiceTypeName()
						}));
					}
					inputBindingComposite.ReadValue(ref inputBindingCompositeContext, UnsafeUtility.AddressOf<TValue>(ref value), UnsafeUtility.SizeOf<TValue>());
				}
				else
				{
					value = inputBindingComposite2.ReadValue(ref inputBindingCompositeContext);
				}
				bindingIndex = compositeOrCompositeBindingIndex;
			}
			else if (controlIndex != -1)
			{
				InputControl inputControl2 = this.controls[controlIndex];
				inputControl = (inputControl2 as InputControl<TValue>);
				if (inputControl == null)
				{
					throw new InvalidOperationException(string.Format("Cannot read value of type '{0}' from control '{1}' bound to action '{2}' (control is a '{3}' with value type '{4}')", new object[]
					{
						typeof(TValue).GetNiceTypeName(),
						inputControl2.path,
						this.GetActionOrNull(bindingIndex),
						inputControl2.GetType().Name,
						inputControl2.valueType.GetNiceTypeName()
					}));
				}
				value = *inputControl.value;
			}
			return this.ApplyProcessors<TValue>(bindingIndex, value, inputControl);
		}

		internal unsafe TValue ApplyProcessors<TValue>(int bindingIndex, TValue value, InputControl<TValue> controlOfType = null) where TValue : struct
		{
			if (this.totalBindingCount == 0)
			{
				return value;
			}
			int processorCount = this.bindingStates[bindingIndex].processorCount;
			if (processorCount > 0)
			{
				int processorStartIndex = this.bindingStates[bindingIndex].processorStartIndex;
				for (int i = 0; i < processorCount; i++)
				{
					InputProcessor<TValue> inputProcessor = this.processors[processorStartIndex + i] as InputProcessor<TValue>;
					if (inputProcessor != null)
					{
						value = inputProcessor.Process(value, controlOfType);
					}
				}
			}
			return value;
		}

		public unsafe float EvaluateCompositePartMagnitude(int bindingIndex, int partNumber)
		{
			int num = bindingIndex + 1;
			float num2 = float.MinValue;
			int num3 = num;
			while (num3 < this.totalBindingCount && this.bindingStates[num3].isPartOfComposite)
			{
				if (this.bindingStates[num3].partIndex == partNumber)
				{
					int controlCount = this.bindingStates[num3].controlCount;
					int controlStartIndex = this.bindingStates[num3].controlStartIndex;
					for (int i = 0; i < controlCount; i++)
					{
						num2 = Mathf.Max(this.controls[controlStartIndex + i].magnitude, num2);
					}
				}
				num3++;
			}
			return num2;
		}

		internal unsafe double GetCompositePartPressTime(int bindingIndex, int partNumber)
		{
			int num = bindingIndex + 1;
			double num2 = double.MaxValue;
			int num3 = num;
			while (num3 < this.totalBindingCount && this.bindingStates[num3].isPartOfComposite)
			{
				ref InputActionState.BindingState ptr = ref this.bindingStates[num3];
				if (ptr.partIndex == partNumber && ptr.pressTime != 0.0 && ptr.pressTime < num2)
				{
					num2 = ptr.pressTime;
				}
				num3++;
			}
			if (num2 == 1.7976931348623157E+308)
			{
				return -1.0;
			}
			return num2;
		}

		internal unsafe TValue ReadCompositePartValue<TValue, TComparer>(int bindingIndex, int partNumber, bool* buttonValuePtr, out int controlIndex, TComparer comparer = default(TComparer)) where TValue : struct where TComparer : IComparer<TValue>
		{
			TValue tvalue = default(TValue);
			int num = bindingIndex + 1;
			bool flag = true;
			controlIndex = -1;
			int num2 = num;
			while (num2 < this.totalBindingCount && this.bindingStates[num2].isPartOfComposite)
			{
				if (this.bindingStates[num2].partIndex == partNumber)
				{
					int controlCount = this.bindingStates[num2].controlCount;
					int controlStartIndex = this.bindingStates[num2].controlStartIndex;
					for (int i = 0; i < controlCount; i++)
					{
						int num3 = controlStartIndex + i;
						TValue tvalue2 = this.ReadValue<TValue>(num2, num3, true);
						if (flag)
						{
							tvalue = tvalue2;
							controlIndex = num3;
							flag = false;
						}
						else if (comparer.Compare(tvalue2, tvalue) > 0)
						{
							tvalue = tvalue2;
							controlIndex = num3;
						}
						if (buttonValuePtr != null && controlIndex == num3)
						{
							InputControl inputControl = this.controls[num3];
							ButtonControl buttonControl = inputControl as ButtonControl;
							if (buttonControl != null)
							{
								*buttonValuePtr = buttonControl.isPressed;
							}
							else if (inputControl is InputControl<float>)
							{
								void* ptr = UnsafeUtility.AddressOf<TValue>(ref tvalue2);
								*buttonValuePtr = (*(float*)ptr >= ButtonControl.s_GlobalDefaultButtonPressPoint);
							}
						}
					}
				}
				num2++;
			}
			return tvalue;
		}

		internal unsafe bool ReadCompositePartValue(int bindingIndex, int partNumber, void* buffer, int bufferSize)
		{
			int num = bindingIndex + 1;
			float num2 = float.MinValue;
			int num3 = num;
			while (num3 < this.totalBindingCount && this.bindingStates[num3].isPartOfComposite)
			{
				if (this.bindingStates[num3].partIndex == partNumber)
				{
					int controlCount = this.bindingStates[num3].controlCount;
					int controlStartIndex = this.bindingStates[num3].controlStartIndex;
					for (int i = 0; i < controlCount; i++)
					{
						int num4 = controlStartIndex + i;
						float magnitude = this.controls[num4].magnitude;
						if (magnitude >= num2)
						{
							this.ReadValue(num3, num4, buffer, bufferSize, true);
							num2 = magnitude;
						}
					}
				}
				num3++;
			}
			return num2 > float.MinValue;
		}

		internal unsafe object ReadCompositePartValueAsObject(int bindingIndex, int partNumber)
		{
			int num = bindingIndex + 1;
			float num2 = float.MinValue;
			object result = null;
			int num3 = num;
			while (num3 < this.totalBindingCount && this.bindingStates[num3].isPartOfComposite)
			{
				if (this.bindingStates[num3].partIndex == partNumber)
				{
					int controlCount = this.bindingStates[num3].controlCount;
					int controlStartIndex = this.bindingStates[num3].controlStartIndex;
					for (int i = 0; i < controlCount; i++)
					{
						int num4 = controlStartIndex + i;
						float magnitude = this.controls[num4].magnitude;
						if (magnitude >= num2)
						{
							result = this.ReadValueAsObject(num3, num4, true);
							num2 = magnitude;
						}
					}
				}
				num3++;
			}
			return result;
		}

		internal unsafe object ReadValueAsObject(int bindingIndex, int controlIndex, bool ignoreComposites = false)
		{
			InputControl control = null;
			object obj = null;
			if (!ignoreComposites && this.bindingStates[bindingIndex].isPartOfComposite)
			{
				int compositeOrCompositeBindingIndex = this.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite inputBindingComposite = this.composites[compositeOrCompositeBindingIndex2];
				InputBindingCompositeContext inputBindingCompositeContext = new InputBindingCompositeContext
				{
					m_State = this,
					m_BindingIndex = compositeOrCompositeBindingIndex
				};
				obj = inputBindingComposite.ReadValueAsObject(ref inputBindingCompositeContext);
				bindingIndex = compositeOrCompositeBindingIndex;
			}
			else if (controlIndex != -1)
			{
				control = this.controls[controlIndex];
				obj = control.ReadValueAsObject();
			}
			if (obj != null)
			{
				int processorCount = this.bindingStates[bindingIndex].processorCount;
				if (processorCount > 0)
				{
					int processorStartIndex = this.bindingStates[bindingIndex].processorStartIndex;
					for (int i = 0; i < processorCount; i++)
					{
						obj = this.processors[processorStartIndex + i].ProcessAsObject(obj, control);
					}
				}
			}
			return obj;
		}

		internal unsafe bool ReadValueAsButton(int bindingIndex, int controlIndex)
		{
			ButtonControl buttonControl = null;
			if (!this.bindingStates[bindingIndex].isPartOfComposite)
			{
				buttonControl = (this.controls[controlIndex] as ButtonControl);
			}
			float num = this.ReadValue<float>(bindingIndex, controlIndex, false);
			if (buttonControl != null)
			{
				return num >= buttonControl.pressPointOrDefault;
			}
			return num >= ButtonControl.s_GlobalDefaultButtonPressPoint;
		}

		internal static ISavedState SaveAndResetState()
		{
			ISavedState result = new SavedStructState<InputActionState.GlobalState>(ref InputActionState.s_GlobalState, delegate(ref InputActionState.GlobalState state)
			{
				InputActionState.s_GlobalState = state;
			}, delegate()
			{
				InputActionState.ResetGlobals();
			});
			InputActionState.s_GlobalState = default(InputActionState.GlobalState);
			return result;
		}

		private void AddToGlobalList()
		{
			InputActionState.CompactGlobalList();
			GCHandle value = GCHandle.Alloc(this, GCHandleType.Weak);
			InputActionState.s_GlobalState.globalList.AppendWithCapacity(value, 10);
		}

		private void RemoveMapFromGlobalList()
		{
			int length = InputActionState.s_GlobalState.globalList.length;
			for (int i = 0; i < length; i++)
			{
				if (InputActionState.s_GlobalState.globalList[i].Target == this)
				{
					InputActionState.s_GlobalState.globalList[i].Free();
					InputActionState.s_GlobalState.globalList.RemoveAtByMovingTailWithCapacity(i);
					return;
				}
			}
		}

		private static void CompactGlobalList()
		{
			int length = InputActionState.s_GlobalState.globalList.length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				GCHandle value = InputActionState.s_GlobalState.globalList[i];
				if (value.IsAllocated && value.Target != null)
				{
					if (num != i)
					{
						InputActionState.s_GlobalState.globalList[num] = value;
					}
					num++;
				}
				else
				{
					if (value.IsAllocated)
					{
						InputActionState.s_GlobalState.globalList[i].Free();
					}
					InputActionState.s_GlobalState.globalList[i] = default(GCHandle);
				}
			}
			InputActionState.s_GlobalState.globalList.length = num;
		}

		internal void NotifyListenersOfActionChange(InputActionChange change)
		{
			for (int i = 0; i < this.totalMapCount; i++)
			{
				InputActionMap inputActionMap = this.maps[i];
				if (inputActionMap.m_SingletonAction != null)
				{
					InputActionState.NotifyListenersOfActionChange(change, inputActionMap.m_SingletonAction);
				}
				else
				{
					if (!(inputActionMap.m_Asset == null))
					{
						InputActionState.NotifyListenersOfActionChange(change, inputActionMap.m_Asset);
						return;
					}
					InputActionState.NotifyListenersOfActionChange(change, inputActionMap);
				}
			}
		}

		internal static void NotifyListenersOfActionChange(InputActionChange change, object actionOrMapOrAsset)
		{
			DelegateHelpers.InvokeCallbacksSafe<object, InputActionChange>(ref InputActionState.s_GlobalState.onActionChange, actionOrMapOrAsset, change, InputActionState.k_InputOnActionChangeMarker, "InputSystem.onActionChange", null);
			if (change == InputActionChange.BoundControlsChanged)
			{
				DelegateHelpers.InvokeCallbacksSafe<object>(ref InputActionState.s_GlobalState.onActionControlsChanged, actionOrMapOrAsset, "onActionControlsChange", null);
			}
		}

		private static void ResetGlobals()
		{
			InputActionState.DestroyAllActionMapStates();
			for (int i = 0; i < InputActionState.s_GlobalState.globalList.length; i++)
			{
				if (InputActionState.s_GlobalState.globalList[i].IsAllocated)
				{
					InputActionState.s_GlobalState.globalList[i].Free();
				}
			}
			InputActionState.s_GlobalState.globalList.length = 0;
			InputActionState.s_GlobalState.onActionChange.Clear();
			InputActionState.s_GlobalState.onActionControlsChanged.Clear();
		}

		internal unsafe static int FindAllEnabledActions(List<InputAction> result)
		{
			int num = 0;
			int length = InputActionState.s_GlobalState.globalList.length;
			for (int i = 0; i < length; i++)
			{
				GCHandle gchandle = InputActionState.s_GlobalState.globalList[i];
				if (gchandle.IsAllocated)
				{
					InputActionState inputActionState = (InputActionState)gchandle.Target;
					if (inputActionState != null)
					{
						int totalMapCount = inputActionState.totalMapCount;
						InputActionMap[] array = inputActionState.maps;
						for (int j = 0; j < totalMapCount; j++)
						{
							InputActionMap inputActionMap = array[j];
							if (inputActionMap.enabled)
							{
								InputAction[] actions = inputActionMap.m_Actions;
								int num2 = actions.Length;
								if (inputActionMap.m_EnabledActionsCount == num2)
								{
									result.AddRange(actions);
									num += num2;
								}
								else
								{
									int actionStartIndex = inputActionState.mapIndices[inputActionMap.m_MapIndexInState].actionStartIndex;
									for (int k = 0; k < num2; k++)
									{
										if (inputActionState.actionStates[actionStartIndex + k].phase != InputActionPhase.Disabled)
										{
											result.Add(actions[k]);
											num++;
										}
									}
								}
							}
						}
					}
				}
			}
			return num;
		}

		internal static void OnDeviceChange(InputDevice device, InputDeviceChange change)
		{
			for (int i = 0; i < InputActionState.s_GlobalState.globalList.length; i++)
			{
				GCHandle gchandle = InputActionState.s_GlobalState.globalList[i];
				if (!gchandle.IsAllocated || gchandle.Target == null)
				{
					if (gchandle.IsAllocated)
					{
						InputActionState.s_GlobalState.globalList[i].Free();
					}
					InputActionState.s_GlobalState.globalList.RemoveAtWithCapacity(i);
					i--;
				}
				else
				{
					InputActionState inputActionState = (InputActionState)gchandle.Target;
					bool fullResolve = true;
					switch (change)
					{
					case InputDeviceChange.Added:
						if (!inputActionState.CanUseDevice(device))
						{
							goto IL_155;
						}
						fullResolve = false;
						break;
					case InputDeviceChange.Removed:
						if (!inputActionState.IsUsingDevice(device))
						{
							goto IL_155;
						}
						for (int j = 0; j < inputActionState.totalMapCount; j++)
						{
							InputActionMap inputActionMap = inputActionState.maps[j];
							inputActionMap.m_Devices.Remove(device);
							InputActionAsset asset = inputActionMap.asset;
							if (asset != null)
							{
								asset.m_Devices.Remove(device);
							}
						}
						fullResolve = false;
						break;
					case InputDeviceChange.UsageChanged:
					case InputDeviceChange.ConfigurationChanged:
						if (!inputActionState.IsUsingDevice(device) && !inputActionState.CanUseDevice(device))
						{
							goto IL_155;
						}
						break;
					case InputDeviceChange.SoftReset:
					case InputDeviceChange.HardReset:
						if (inputActionState.IsUsingDevice(device))
						{
							inputActionState.ResetActionStatesDrivenBy(device);
							goto IL_155;
						}
						goto IL_155;
					}
					int num = 0;
					while (num < inputActionState.totalMapCount && !inputActionState.maps[num].LazyResolveBindings(fullResolve))
					{
						num++;
					}
				}
				IL_155:;
			}
		}

		internal static void DeferredResolutionOfBindings()
		{
			InputActionMap.s_DeferBindingResolution++;
			try
			{
				if (InputActionMap.s_NeedToResolveBindings)
				{
					for (int i = 0; i < InputActionState.s_GlobalState.globalList.length; i++)
					{
						GCHandle gchandle = InputActionState.s_GlobalState.globalList[i];
						InputActionState inputActionState = gchandle.IsAllocated ? ((InputActionState)gchandle.Target) : null;
						if (inputActionState == null)
						{
							if (gchandle.IsAllocated)
							{
								InputActionState.s_GlobalState.globalList[i].Free();
							}
							InputActionState.s_GlobalState.globalList.RemoveAtWithCapacity(i);
							i--;
						}
						else
						{
							for (int j = 0; j < inputActionState.totalMapCount; j++)
							{
								inputActionState.maps[j].ResolveBindingsIfNecessary();
							}
						}
					}
					InputActionMap.s_NeedToResolveBindings = false;
				}
			}
			finally
			{
				InputActionMap.s_DeferBindingResolution--;
			}
		}

		internal static void DisableAllActions()
		{
			for (int i = 0; i < InputActionState.s_GlobalState.globalList.length; i++)
			{
				GCHandle gchandle = InputActionState.s_GlobalState.globalList[i];
				if (gchandle.IsAllocated && gchandle.Target != null)
				{
					InputActionState inputActionState = (InputActionState)gchandle.Target;
					int totalMapCount = inputActionState.totalMapCount;
					InputActionMap[] array = inputActionState.maps;
					for (int j = 0; j < totalMapCount; j++)
					{
						array[j].Disable();
					}
				}
			}
		}

		internal static void DestroyAllActionMapStates()
		{
			while (InputActionState.s_GlobalState.globalList.length > 0)
			{
				int index = InputActionState.s_GlobalState.globalList.length - 1;
				GCHandle gchandle = InputActionState.s_GlobalState.globalList[index];
				if (!gchandle.IsAllocated || gchandle.Target == null)
				{
					if (gchandle.IsAllocated)
					{
						InputActionState.s_GlobalState.globalList[index].Free();
					}
					InputActionState.s_GlobalState.globalList.RemoveAtWithCapacity(index);
				}
				else
				{
					((InputActionState)gchandle.Target).Destroy(false);
				}
			}
		}

		public const int kInvalidIndex = -1;

		public InputActionMap[] maps;

		public InputControl[] controls;

		public IInputInteraction[] interactions;

		public InputProcessor[] processors;

		public InputBindingComposite[] composites;

		public int totalProcessorCount;

		public InputActionState.UnmanagedMemory memory;

		private bool m_OnBeforeUpdateHooked;

		private bool m_OnAfterUpdateHooked;

		private bool m_InProcessControlStateChange;

		private InputEventPtr m_CurrentlyProcessingThisEvent;

		private Action m_OnBeforeUpdateDelegate;

		private Action m_OnAfterUpdateDelegate;

		private static readonly ProfilerMarker k_InputInitialActionStateCheckMarker = new ProfilerMarker("InitialActionStateCheck");

		private static readonly ProfilerMarker k_InputActionResolveConflictMarker = new ProfilerMarker("InputActionResolveConflict");

		private static readonly ProfilerMarker k_InputActionCallbackMarker = new ProfilerMarker("InputActionCallback");

		private static readonly ProfilerMarker k_InputOnActionChangeMarker = new ProfilerMarker("InpustSystem.onActionChange");

		private static readonly ProfilerMarker k_InputOnDeviceChangeMarker = new ProfilerMarker("InpustSystem.onDeviceChange");

		internal static InputActionState.GlobalState s_GlobalState;

		[StructLayout(LayoutKind.Explicit, Size = 48)]
		internal struct InteractionState
		{
			public int triggerControlIndex
			{
				get
				{
					if (this.m_TriggerControlIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_TriggerControlIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_TriggerControlIndex = ushort.MaxValue;
						return;
					}
					if (value < 0 || value >= 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue-1 controls in a single InputActionState");
					}
					this.m_TriggerControlIndex = (ushort)value;
				}
			}

			public double startTime
			{
				get
				{
					return this.m_StartTime;
				}
				set
				{
					this.m_StartTime = value;
				}
			}

			public double performedTime
			{
				get
				{
					return this.m_PerformedTime;
				}
				set
				{
					this.m_PerformedTime = value;
				}
			}

			public double timerStartTime
			{
				get
				{
					return this.m_TimerStartTime;
				}
				set
				{
					this.m_TimerStartTime = value;
				}
			}

			public float timerDuration
			{
				get
				{
					return this.m_TimerDuration;
				}
				set
				{
					this.m_TimerDuration = value;
				}
			}

			public float totalTimeoutCompletionDone
			{
				get
				{
					return this.m_TotalTimeoutCompletionTimeDone;
				}
				set
				{
					this.m_TotalTimeoutCompletionTimeDone = value;
				}
			}

			public float totalTimeoutCompletionTimeRemaining
			{
				get
				{
					return this.m_TotalTimeoutCompletionTimeRemaining;
				}
				set
				{
					this.m_TotalTimeoutCompletionTimeRemaining = value;
				}
			}

			public long timerMonitorIndex
			{
				get
				{
					return this.m_TimerMonitorIndex;
				}
				set
				{
					this.m_TimerMonitorIndex = value;
				}
			}

			public bool isTimerRunning
			{
				get
				{
					return (this.m_Flags & 1) == 1;
				}
				set
				{
					if (value)
					{
						this.m_Flags |= 1;
						return;
					}
					InputActionState.InteractionState.Flags flags = ~InputActionState.InteractionState.Flags.TimerRunning;
					this.m_Flags &= (byte)flags;
				}
			}

			public InputActionPhase phase
			{
				get
				{
					return (InputActionPhase)this.m_Phase;
				}
				set
				{
					this.m_Phase = (byte)value;
				}
			}

			[FieldOffset(0)]
			private ushort m_TriggerControlIndex;

			[FieldOffset(2)]
			private byte m_Phase;

			[FieldOffset(3)]
			private byte m_Flags;

			[FieldOffset(4)]
			private float m_TimerDuration;

			[FieldOffset(8)]
			private double m_StartTime;

			[FieldOffset(16)]
			private double m_TimerStartTime;

			[FieldOffset(24)]
			private double m_PerformedTime;

			[FieldOffset(32)]
			private float m_TotalTimeoutCompletionTimeDone;

			[FieldOffset(36)]
			private float m_TotalTimeoutCompletionTimeRemaining;

			[FieldOffset(40)]
			private long m_TimerMonitorIndex;

			[Flags]
			private enum Flags
			{
				TimerRunning = 1
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 32)]
		internal struct BindingState
		{
			public int controlStartIndex
			{
				get
				{
					return (int)this.m_ControlStartIndex;
				}
				set
				{
					if (value >= 65535)
					{
						throw new NotSupportedException("Total control count in state cannot exceed byte.MaxValue=" + ushort.MaxValue.ToString());
					}
					this.m_ControlStartIndex = (ushort)value;
				}
			}

			public int controlCount
			{
				get
				{
					return (int)this.m_ControlCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Control count per binding cannot exceed byte.MaxValue=" + byte.MaxValue.ToString());
					}
					this.m_ControlCount = (byte)value;
				}
			}

			public int interactionStartIndex
			{
				get
				{
					if (this.m_InteractionStartIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_InteractionStartIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_InteractionStartIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Interaction count cannot exceed ushort.MaxValue=" + ushort.MaxValue.ToString());
					}
					this.m_InteractionStartIndex = (ushort)value;
				}
			}

			public int interactionCount
			{
				get
				{
					return (int)this.m_InteractionCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Interaction count per binding cannot exceed byte.MaxValue=" + byte.MaxValue.ToString());
					}
					this.m_InteractionCount = (byte)value;
				}
			}

			public int processorStartIndex
			{
				get
				{
					if (this.m_ProcessorStartIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_ProcessorStartIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_ProcessorStartIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Processor count cannot exceed ushort.MaxValue=" + ushort.MaxValue.ToString());
					}
					this.m_ProcessorStartIndex = (ushort)value;
				}
			}

			public int processorCount
			{
				get
				{
					return (int)this.m_ProcessorCount;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Processor count per binding cannot exceed byte.MaxValue=" + byte.MaxValue.ToString());
					}
					this.m_ProcessorCount = (byte)value;
				}
			}

			public int actionIndex
			{
				get
				{
					if (this.m_ActionIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_ActionIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_ActionIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Action count cannot exceed ushort.MaxValue=" + ushort.MaxValue.ToString());
					}
					this.m_ActionIndex = (ushort)value;
				}
			}

			public int mapIndex
			{
				get
				{
					return (int)this.m_MapIndex;
				}
				set
				{
					if (value >= 255)
					{
						throw new NotSupportedException("Map count cannot exceed byte.MaxValue=" + byte.MaxValue.ToString());
					}
					this.m_MapIndex = (byte)value;
				}
			}

			public int compositeOrCompositeBindingIndex
			{
				get
				{
					if (this.m_CompositeOrCompositeBindingIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_CompositeOrCompositeBindingIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_CompositeOrCompositeBindingIndex = ushort.MaxValue;
						return;
					}
					if (value >= 65535)
					{
						throw new NotSupportedException("Composite count cannot exceed ushort.MaxValue=" + ushort.MaxValue.ToString());
					}
					this.m_CompositeOrCompositeBindingIndex = (ushort)value;
				}
			}

			public int triggerEventIdForComposite
			{
				get
				{
					return this.m_TriggerEventIdForComposite;
				}
				set
				{
					this.m_TriggerEventIdForComposite = value;
				}
			}

			public double pressTime
			{
				get
				{
					return this.m_PressTime;
				}
				set
				{
					this.m_PressTime = value;
				}
			}

			public InputActionState.BindingState.Flags flags
			{
				get
				{
					return (InputActionState.BindingState.Flags)this.m_Flags;
				}
				set
				{
					this.m_Flags = (byte)value;
				}
			}

			public bool chainsWithNext
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.ChainsWithNext) == InputActionState.BindingState.Flags.ChainsWithNext;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.ChainsWithNext;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.ChainsWithNext;
				}
			}

			public bool isEndOfChain
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.EndOfChain) == InputActionState.BindingState.Flags.EndOfChain;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.EndOfChain;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.EndOfChain;
				}
			}

			public bool isPartOfChain
			{
				get
				{
					return this.chainsWithNext || this.isEndOfChain;
				}
			}

			public bool isComposite
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.Composite) == InputActionState.BindingState.Flags.Composite;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.Composite;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.Composite;
				}
			}

			public bool isPartOfComposite
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.PartOfComposite) == InputActionState.BindingState.Flags.PartOfComposite;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.PartOfComposite;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.PartOfComposite;
				}
			}

			public bool initialStateCheckPending
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.InitialStateCheckPending) > (InputActionState.BindingState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.InitialStateCheckPending;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.InitialStateCheckPending;
				}
			}

			public bool wantsInitialStateCheck
			{
				get
				{
					return (this.flags & InputActionState.BindingState.Flags.WantsInitialStateCheck) > (InputActionState.BindingState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.BindingState.Flags.WantsInitialStateCheck;
						return;
					}
					this.flags &= ~InputActionState.BindingState.Flags.WantsInitialStateCheck;
				}
			}

			public int partIndex
			{
				get
				{
					return (int)this.m_PartIndex;
				}
				set
				{
					if (this.partIndex < 0)
					{
						throw new ArgumentOutOfRangeException("value", "Part index must not be negative");
					}
					if (this.partIndex > 255)
					{
						throw new InvalidOperationException("Part count must not exceed byte.MaxValue=" + byte.MaxValue.ToString());
					}
					this.m_PartIndex = (byte)value;
				}
			}

			[FieldOffset(0)]
			private byte m_ControlCount;

			[FieldOffset(1)]
			private byte m_InteractionCount;

			[FieldOffset(2)]
			private byte m_ProcessorCount;

			[FieldOffset(3)]
			private byte m_MapIndex;

			[FieldOffset(4)]
			private byte m_Flags;

			[FieldOffset(5)]
			private byte m_PartIndex;

			[FieldOffset(6)]
			private ushort m_ActionIndex;

			[FieldOffset(8)]
			private ushort m_CompositeOrCompositeBindingIndex;

			[FieldOffset(10)]
			private ushort m_ProcessorStartIndex;

			[FieldOffset(12)]
			private ushort m_InteractionStartIndex;

			[FieldOffset(14)]
			private ushort m_ControlStartIndex;

			[FieldOffset(16)]
			private double m_PressTime;

			[FieldOffset(24)]
			private int m_TriggerEventIdForComposite;

			[FieldOffset(28)]
			private int __padding;

			[Flags]
			public enum Flags
			{
				ChainsWithNext = 1,
				EndOfChain = 2,
				Composite = 4,
				PartOfComposite = 8,
				InitialStateCheckPending = 16,
				WantsInitialStateCheck = 32
			}
		}

		[StructLayout(LayoutKind.Explicit, Size = 56)]
		public struct TriggerState
		{
			public InputActionPhase phase
			{
				get
				{
					return (InputActionPhase)this.m_Phase;
				}
				set
				{
					this.m_Phase = (byte)value;
				}
			}

			public bool isDisabled
			{
				get
				{
					return this.phase == InputActionPhase.Disabled;
				}
			}

			public bool isWaiting
			{
				get
				{
					return this.phase == InputActionPhase.Waiting;
				}
			}

			public bool isStarted
			{
				get
				{
					return this.phase == InputActionPhase.Started;
				}
			}

			public bool isPerformed
			{
				get
				{
					return this.phase == InputActionPhase.Performed;
				}
			}

			public bool isCanceled
			{
				get
				{
					return this.phase == InputActionPhase.Canceled;
				}
			}

			public double time
			{
				get
				{
					return this.m_Time;
				}
				set
				{
					this.m_Time = value;
				}
			}

			public double startTime
			{
				get
				{
					return this.m_StartTime;
				}
				set
				{
					this.m_StartTime = value;
				}
			}

			public float magnitude
			{
				get
				{
					return this.m_Magnitude;
				}
				set
				{
					this.flags |= InputActionState.TriggerState.Flags.HaveMagnitude;
					this.m_Magnitude = value;
				}
			}

			public bool haveMagnitude
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.HaveMagnitude) > (InputActionState.TriggerState.Flags)0;
				}
			}

			public int mapIndex
			{
				get
				{
					return (int)this.m_MapIndex;
				}
				set
				{
					if (value < 0 || value > 255)
					{
						throw new NotSupportedException("More than byte.MaxValue InputActionMaps in a single InputActionState");
					}
					this.m_MapIndex = (byte)value;
				}
			}

			public int controlIndex
			{
				get
				{
					if (this.m_ControlIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_ControlIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_ControlIndex = ushort.MaxValue;
						return;
					}
					if (value < 0 || value >= 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue-1 controls in a single InputActionState");
					}
					this.m_ControlIndex = (ushort)value;
				}
			}

			public int bindingIndex
			{
				get
				{
					return (int)this.m_BindingIndex;
				}
				set
				{
					if (value < 0 || value > 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue bindings in a single InputActionState");
					}
					this.m_BindingIndex = (ushort)value;
				}
			}

			public int interactionIndex
			{
				get
				{
					if (this.m_InteractionIndex == 65535)
					{
						return -1;
					}
					return (int)this.m_InteractionIndex;
				}
				set
				{
					if (value == -1)
					{
						this.m_InteractionIndex = ushort.MaxValue;
						return;
					}
					if (value < 0 || value >= 65535)
					{
						throw new NotSupportedException("More than ushort.MaxValue-1 interactions in a single InputActionState");
					}
					this.m_InteractionIndex = (ushort)value;
				}
			}

			public uint lastPerformedInUpdate
			{
				get
				{
					return this.m_LastPerformedInUpdate;
				}
				set
				{
					this.m_LastPerformedInUpdate = value;
				}
			}

			public uint lastCompletedInUpdate
			{
				get
				{
					return this.m_LastCompletedInUpdate;
				}
				set
				{
					this.m_LastCompletedInUpdate = value;
				}
			}

			public uint lastCanceledInUpdate
			{
				get
				{
					return this.m_LastCanceledInUpdate;
				}
				set
				{
					this.m_LastCanceledInUpdate = value;
				}
			}

			public uint pressedInUpdate
			{
				get
				{
					return this.m_PressedInUpdate;
				}
				set
				{
					this.m_PressedInUpdate = value;
				}
			}

			public uint releasedInUpdate
			{
				get
				{
					return this.m_ReleasedInUpdate;
				}
				set
				{
					this.m_ReleasedInUpdate = value;
				}
			}

			public bool isPassThrough
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.PassThrough) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.PassThrough;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.PassThrough;
				}
			}

			public bool isButton
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.Button) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.Button;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.Button;
				}
			}

			public bool isPressed
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.Pressed) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.Pressed;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.Pressed;
				}
			}

			public bool mayNeedConflictResolution
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.MayNeedConflictResolution) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.MayNeedConflictResolution;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.MayNeedConflictResolution;
				}
			}

			public bool hasMultipleConcurrentActuations
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.HasMultipleConcurrentActuations) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.HasMultipleConcurrentActuations;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.HasMultipleConcurrentActuations;
				}
			}

			public bool inProcessing
			{
				get
				{
					return (this.flags & InputActionState.TriggerState.Flags.InProcessing) > (InputActionState.TriggerState.Flags)0;
				}
				set
				{
					if (value)
					{
						this.flags |= InputActionState.TriggerState.Flags.InProcessing;
						return;
					}
					this.flags &= ~InputActionState.TriggerState.Flags.InProcessing;
				}
			}

			public InputActionState.TriggerState.Flags flags
			{
				get
				{
					return (InputActionState.TriggerState.Flags)this.m_Flags;
				}
				set
				{
					this.m_Flags = (byte)value;
				}
			}

			public const int kMaxNumMaps = 255;

			public const int kMaxNumControls = 65535;

			public const int kMaxNumBindings = 65535;

			[FieldOffset(0)]
			private byte m_Phase;

			[FieldOffset(1)]
			private byte m_Flags;

			[FieldOffset(2)]
			private byte m_MapIndex;

			[FieldOffset(4)]
			private ushort m_ControlIndex;

			[FieldOffset(8)]
			private double m_Time;

			[FieldOffset(16)]
			private double m_StartTime;

			[FieldOffset(24)]
			private ushort m_BindingIndex;

			[FieldOffset(26)]
			private ushort m_InteractionIndex;

			[FieldOffset(28)]
			private float m_Magnitude;

			[FieldOffset(32)]
			private uint m_LastPerformedInUpdate;

			[FieldOffset(36)]
			private uint m_LastCanceledInUpdate;

			[FieldOffset(40)]
			private uint m_PressedInUpdate;

			[FieldOffset(44)]
			private uint m_ReleasedInUpdate;

			[FieldOffset(48)]
			private uint m_LastCompletedInUpdate;

			[FieldOffset(52)]
			internal int framePerformed;

			[FieldOffset(56)]
			internal int framePressed;

			[FieldOffset(60)]
			internal int frameReleased;

			[FieldOffset(64)]
			internal int frameCompleted;

			[Flags]
			public enum Flags
			{
				HaveMagnitude = 1,
				PassThrough = 2,
				MayNeedConflictResolution = 4,
				HasMultipleConcurrentActuations = 8,
				InProcessing = 16,
				Button = 32,
				Pressed = 64
			}
		}

		public struct ActionMapIndices
		{
			public int actionStartIndex;

			public int actionCount;

			public int controlStartIndex;

			public int controlCount;

			public int bindingStartIndex;

			public int bindingCount;

			public int interactionStartIndex;

			public int interactionCount;

			public int processorStartIndex;

			public int processorCount;

			public int compositeStartIndex;

			public int compositeCount;
		}

		public struct UnmanagedMemory : IDisposable
		{
			public bool isAllocated
			{
				get
				{
					return this.basePtr != null;
				}
			}

			public int sizeInBytes
			{
				get
				{
					return this.mapCount * sizeof(InputActionState.ActionMapIndices) + this.actionCount * sizeof(InputActionState.TriggerState) + this.bindingCount * sizeof(InputActionState.BindingState) + this.interactionCount * sizeof(InputActionState.InteractionState) + this.controlCount * 4 + this.compositeCount * 4 + this.controlCount * 4 + this.controlCount * 2 * 2 + this.actionCount * 2 * 2 + this.bindingCount * 2 + (this.controlCount + 31) / 32 * 4;
				}
			}

			private unsafe static byte* AllocFromBlob(ref byte* top, int size)
			{
				if (size == 0)
				{
					return null;
				}
				byte* result = top;
				top += (IntPtr)size;
				return result;
			}

			public unsafe void Allocate(int mapCount, int actionCount, int bindingCount, int controlCount, int interactionCount, int compositeCount)
			{
				this.mapCount = mapCount;
				this.actionCount = actionCount;
				this.interactionCount = interactionCount;
				this.bindingCount = bindingCount;
				this.controlCount = controlCount;
				this.compositeCount = compositeCount;
				int sizeInBytes = this.sizeInBytes;
				byte* destination = (byte*)UnsafeUtility.Malloc((long)sizeInBytes, 8, Allocator.Persistent);
				UnsafeUtility.MemClear((void*)destination, (long)sizeInBytes);
				this.basePtr = (void*)destination;
				this.actionStates = (InputActionState.TriggerState*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, actionCount * sizeof(InputActionState.TriggerState));
				this.interactionStates = (InputActionState.InteractionState*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, interactionCount * sizeof(InputActionState.InteractionState));
				this.bindingStates = (InputActionState.BindingState*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, bindingCount * sizeof(InputActionState.BindingState));
				this.mapIndices = (InputActionState.ActionMapIndices*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, mapCount * sizeof(InputActionState.ActionMapIndices));
				this.controlMagnitudes = (float*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, controlCount * 4);
				this.compositeMagnitudes = (float*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, compositeCount * 4);
				this.controlIndexToBindingIndex = (int*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, controlCount * 4);
				this.controlGroupingAndComplexity = (ushort*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, controlCount * 2 * 2);
				this.actionBindingIndicesAndCounts = (ushort*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, actionCount * 2 * 2);
				this.actionBindingIndices = (ushort*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, bindingCount * 2);
				this.enabledControls = (int*)InputActionState.UnmanagedMemory.AllocFromBlob(ref destination, (controlCount + 31) / 32 * 4);
			}

			public void Dispose()
			{
				if (this.basePtr == null)
				{
					return;
				}
				UnsafeUtility.Free(this.basePtr, Allocator.Persistent);
				this.basePtr = null;
				this.actionStates = null;
				this.interactionStates = null;
				this.bindingStates = null;
				this.mapIndices = null;
				this.controlMagnitudes = null;
				this.compositeMagnitudes = null;
				this.controlIndexToBindingIndex = null;
				this.controlGroupingAndComplexity = null;
				this.actionBindingIndices = null;
				this.actionBindingIndicesAndCounts = null;
				this.mapCount = 0;
				this.actionCount = 0;
				this.bindingCount = 0;
				this.controlCount = 0;
				this.interactionCount = 0;
				this.compositeCount = 0;
			}

			public unsafe void CopyDataFrom(InputActionState.UnmanagedMemory memory)
			{
				UnsafeUtility.MemCpy((void*)this.mapIndices, (void*)memory.mapIndices, (long)(memory.mapCount * sizeof(InputActionState.ActionMapIndices)));
				UnsafeUtility.MemCpy((void*)this.actionStates, (void*)memory.actionStates, (long)(memory.actionCount * sizeof(InputActionState.TriggerState)));
				UnsafeUtility.MemCpy((void*)this.bindingStates, (void*)memory.bindingStates, (long)(memory.bindingCount * sizeof(InputActionState.BindingState)));
				UnsafeUtility.MemCpy((void*)this.interactionStates, (void*)memory.interactionStates, (long)(memory.interactionCount * sizeof(InputActionState.InteractionState)));
				UnsafeUtility.MemCpy((void*)this.controlMagnitudes, (void*)memory.controlMagnitudes, (long)(memory.controlCount * 4));
				UnsafeUtility.MemCpy((void*)this.compositeMagnitudes, (void*)memory.compositeMagnitudes, (long)(memory.compositeCount * 4));
				UnsafeUtility.MemCpy((void*)this.controlIndexToBindingIndex, (void*)memory.controlIndexToBindingIndex, (long)(memory.controlCount * 4));
				UnsafeUtility.MemCpy((void*)this.controlGroupingAndComplexity, (void*)memory.controlGroupingAndComplexity, (long)(memory.controlCount * 2 * 2));
				UnsafeUtility.MemCpy((void*)this.actionBindingIndicesAndCounts, (void*)memory.actionBindingIndicesAndCounts, (long)(memory.actionCount * 2 * 2));
				UnsafeUtility.MemCpy((void*)this.actionBindingIndices, (void*)memory.actionBindingIndices, (long)(memory.bindingCount * 2));
				UnsafeUtility.MemCpy((void*)this.enabledControls, (void*)memory.enabledControls, (long)((memory.controlCount + 31) / 32 * 4));
			}

			public InputActionState.UnmanagedMemory Clone()
			{
				if (!this.isAllocated)
				{
					return default(InputActionState.UnmanagedMemory);
				}
				InputActionState.UnmanagedMemory result = default(InputActionState.UnmanagedMemory);
				int num = this.mapCount;
				int num2 = this.actionCount;
				int num3 = this.controlCount;
				result.Allocate(num, num2, this.bindingCount, num3, this.interactionCount, this.compositeCount);
				result.CopyDataFrom(this);
				return result;
			}

			public unsafe void* basePtr;

			public int mapCount;

			public int actionCount;

			public int interactionCount;

			public int bindingCount;

			public int controlCount;

			public int compositeCount;

			public unsafe InputActionState.TriggerState* actionStates;

			public unsafe InputActionState.BindingState* bindingStates;

			public unsafe InputActionState.InteractionState* interactionStates;

			public unsafe float* controlMagnitudes;

			public unsafe float* compositeMagnitudes;

			public unsafe int* enabledControls;

			public unsafe ushort* actionBindingIndicesAndCounts;

			public unsafe ushort* actionBindingIndices;

			public unsafe int* controlIndexToBindingIndex;

			public unsafe ushort* controlGroupingAndComplexity;

			public bool controlGroupingInitialized;

			public unsafe InputActionState.ActionMapIndices* mapIndices;
		}

		internal struct GlobalState
		{
			internal InlinedArray<GCHandle> globalList;

			internal CallbackArray<Action<object, InputActionChange>> onActionChange;

			internal CallbackArray<Action<object>> onActionControlsChanged;
		}
	}
}
