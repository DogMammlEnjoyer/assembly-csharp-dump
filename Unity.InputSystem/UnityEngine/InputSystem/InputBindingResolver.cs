using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	internal struct InputBindingResolver : IDisposable
	{
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

		public int totalControlCount
		{
			get
			{
				return this.memory.controlCount;
			}
		}

		public void Dispose()
		{
			this.memory.Dispose();
		}

		public void StartWithPreviousResolve(InputActionState state, bool isFullResolve)
		{
			this.m_IsControlOnlyResolve = !isFullResolve;
			this.maps = state.maps;
			this.interactions = state.interactions;
			this.processors = state.processors;
			this.composites = state.composites;
			this.controls = state.controls;
			if (isFullResolve)
			{
				if (this.maps != null)
				{
					Array.Clear(this.maps, 0, state.totalMapCount);
				}
				if (this.interactions != null)
				{
					Array.Clear(this.interactions, 0, state.totalInteractionCount);
				}
				if (this.processors != null)
				{
					Array.Clear(this.processors, 0, state.totalProcessorCount);
				}
				if (this.composites != null)
				{
					Array.Clear(this.composites, 0, state.totalCompositeCount);
				}
			}
			if (this.controls != null)
			{
				Array.Clear(this.controls, 0, state.totalControlCount);
			}
			state.maps = null;
			state.interactions = null;
			state.processors = null;
			state.composites = null;
			state.controls = null;
		}

		public unsafe void AddActionMap(InputActionMap actionMap)
		{
			InputSystem.EnsureInitialized();
			InputAction[] actions = actionMap.m_Actions;
			InputBinding[] bindings = actionMap.m_Bindings;
			int num = (bindings != null) ? bindings.Length : 0;
			int num2 = (actions != null) ? actions.Length : 0;
			int totalMapCount = this.totalMapCount;
			int totalActionCount = this.totalActionCount;
			int totalBindingCount = this.totalBindingCount;
			int totalControlCount = this.totalControlCount;
			int num3 = this.totalInteractionCount;
			int num4 = this.totalProcessorCount;
			int num5 = this.totalCompositeCount;
			InputActionState.UnmanagedMemory unmanagedMemory = default(InputActionState.UnmanagedMemory);
			int mapCount = this.totalMapCount + 1;
			int actionCount = this.totalActionCount + num2;
			int bindingCount = this.totalBindingCount + num;
			int interactionCount = this.totalInteractionCount;
			int compositeCount = this.totalCompositeCount;
			unmanagedMemory.Allocate(mapCount, actionCount, bindingCount, this.totalControlCount, interactionCount, compositeCount);
			if (this.memory.isAllocated)
			{
				unmanagedMemory.CopyDataFrom(this.memory);
			}
			int num6 = -1;
			int num7 = -1;
			int num8 = 0;
			int num9 = -1;
			InputAction inputAction = null;
			InputBinding? inputBinding = actionMap.m_BindingMask;
			ReadOnlyArray<InputDevice>? devices = actionMap.devices;
			bool flag = actionMap.m_SingletonAction != null;
			InputControlList<InputControl> values = new InputControlList<InputControl>(Allocator.Temp, 0);
			try
			{
				for (int i = 0; i < num; i++)
				{
					InputActionState.BindingState* bindingStates = unmanagedMemory.bindingStates;
					ref InputBinding ptr = ref bindings[i];
					int num10 = totalBindingCount + i;
					bool isComposite = ptr.isComposite;
					bool flag2 = !isComposite && ptr.isPartOfComposite;
					InputActionState.BindingState* ptr2 = bindingStates + num10;
					try
					{
						int controlStartIndex = 0;
						int num11 = -1;
						int num12 = -1;
						int actionIndex = -1;
						int partIndex = -1;
						int num13 = 0;
						int num14 = 0;
						int num15 = 0;
						if (flag2 && num6 == -1)
						{
							throw new InvalidOperationException(string.Format("Binding '{0}' is marked as being part of a composite but the preceding binding is not a composite", ptr));
						}
						int num16 = -1;
						string action = ptr.action;
						InputAction inputAction2 = null;
						if (!flag2)
						{
							if (flag)
							{
								num16 = 0;
							}
							else if (!string.IsNullOrEmpty(action))
							{
								num16 = actionMap.FindActionIndex(action);
							}
							if (num16 != -1)
							{
								inputAction2 = actions[num16];
							}
						}
						else
						{
							num16 = num9;
							inputAction2 = inputAction;
						}
						if (isComposite)
						{
							num6 = num10;
							inputAction = inputAction2;
							num9 = num16;
						}
						string effectivePath = ptr.effectivePath;
						bool flag3 = string.IsNullOrEmpty(effectivePath) || inputAction2 == null || (!isComposite && this.bindingMask != null && !this.bindingMask.Value.Matches(ref ptr, InputBinding.MatchOptions.EmptyGroupMatchesAny)) || (!isComposite && inputBinding != null && !inputBinding.Value.Matches(ref ptr, InputBinding.MatchOptions.EmptyGroupMatchesAny)) || (!isComposite && inputAction2 != null && inputAction2.m_BindingMask != null && !inputAction2.m_BindingMask.Value.Matches(ref ptr, InputBinding.MatchOptions.EmptyGroupMatchesAny));
						if (!flag3 && !isComposite)
						{
							controlStartIndex = this.memory.controlCount + values.Count;
							if (devices != null)
							{
								ReadOnlyArray<InputDevice> value = devices.Value;
								for (int j = 0; j < value.Count; j++)
								{
									InputDevice inputDevice = value[j];
									if (inputDevice.added)
									{
										num13 += InputControlPath.TryFindControls<InputControl>(inputDevice, effectivePath, 0, ref values);
									}
								}
							}
							else
							{
								num13 = InputSystem.FindControls<InputControl>(effectivePath, ref values);
							}
						}
						if (!flag3)
						{
							string effectiveProcessors = ptr.effectiveProcessors;
							if (!string.IsNullOrEmpty(effectiveProcessors))
							{
								num12 = this.InstantiateWithParameters<InputProcessor>(InputProcessor.s_Processors, effectiveProcessors, ref this.processors, ref this.totalProcessorCount, actionMap, ref ptr);
								if (num12 != -1)
								{
									num15 = this.totalProcessorCount - num12;
								}
							}
							if (!string.IsNullOrEmpty(inputAction2.m_Processors))
							{
								int num17 = this.InstantiateWithParameters<InputProcessor>(InputProcessor.s_Processors, inputAction2.m_Processors, ref this.processors, ref this.totalProcessorCount, actionMap, ref ptr);
								if (num17 != -1)
								{
									if (num12 == -1)
									{
										num12 = num17;
									}
									num15 += this.totalProcessorCount - num17;
								}
							}
							if (flag2)
							{
								if (num6 != -1)
								{
									num11 = bindingStates[num6].interactionStartIndex;
									num14 = bindingStates[num6].interactionCount;
								}
							}
							else
							{
								string effectiveInteractions = ptr.effectiveInteractions;
								if (!string.IsNullOrEmpty(effectiveInteractions))
								{
									num11 = this.InstantiateWithParameters<IInputInteraction>(InputInteraction.s_Interactions, effectiveInteractions, ref this.interactions, ref this.totalInteractionCount, actionMap, ref ptr);
									if (num11 != -1)
									{
										num14 = this.totalInteractionCount - num11;
									}
								}
								if (!string.IsNullOrEmpty(inputAction2.m_Interactions))
								{
									int num18 = this.InstantiateWithParameters<IInputInteraction>(InputInteraction.s_Interactions, inputAction2.m_Interactions, ref this.interactions, ref this.totalInteractionCount, actionMap, ref ptr);
									if (num18 != -1)
									{
										if (num11 == -1)
										{
											num11 = num18;
										}
										num14 += this.totalInteractionCount - num18;
									}
								}
							}
							if (isComposite)
							{
								InputBindingComposite value2 = InputBindingResolver.InstantiateBindingComposite(ref ptr, actionMap);
								num7 = ArrayHelpers.AppendWithCapacity<InputBindingComposite>(ref this.composites, ref this.totalCompositeCount, value2, 10);
								controlStartIndex = this.memory.controlCount + values.Count;
							}
							else if (!flag2 && num6 != -1)
							{
								num8 = 0;
								num6 = -1;
								num7 = -1;
								inputAction = null;
								num9 = -1;
							}
						}
						if (flag2 && num6 != -1 && num13 > 0)
						{
							if (string.IsNullOrEmpty(ptr.name))
							{
								throw new InvalidOperationException(string.Format("Binding '{0}' that is part of composite '{1}' is missing a name", ptr, this.composites[num7]));
							}
							partIndex = InputBindingResolver.AssignCompositePartIndex(this.composites[num7], ptr.name, ref num8);
							bindingStates[num6].controlCount += num13;
							actionIndex = bindingStates[num6].actionIndex;
						}
						else if (num16 != -1)
						{
							actionIndex = totalActionCount + num16;
						}
						*ptr2 = new InputActionState.BindingState
						{
							controlStartIndex = controlStartIndex,
							controlCount = num13,
							interactionStartIndex = num11,
							interactionCount = num14,
							processorStartIndex = num12,
							processorCount = num15,
							isComposite = isComposite,
							isPartOfComposite = ptr.isPartOfComposite,
							partIndex = partIndex,
							actionIndex = actionIndex,
							compositeOrCompositeBindingIndex = (isComposite ? num7 : num6),
							mapIndex = this.totalMapCount,
							wantsInitialStateCheck = (inputAction2 != null && inputAction2.wantsInitialStateCheck)
						};
					}
					catch (Exception ex)
					{
						Debug.LogError(string.Format("{0} while resolving binding '{1}' in action map '{2}'", ex.GetType().Name, ptr, actionMap));
						Debug.LogException(ex);
						if (ex.IsExceptionIndicatingBugInCode())
						{
							throw;
						}
					}
				}
				int count = values.Count;
				int num19 = this.memory.controlCount + count;
				if (unmanagedMemory.interactionCount != this.totalInteractionCount || unmanagedMemory.compositeCount != this.totalCompositeCount || unmanagedMemory.controlCount != num19)
				{
					InputActionState.UnmanagedMemory unmanagedMemory2 = default(InputActionState.UnmanagedMemory);
					unmanagedMemory2.Allocate(unmanagedMemory.mapCount, unmanagedMemory.actionCount, unmanagedMemory.bindingCount, num19, this.totalInteractionCount, this.totalCompositeCount);
					unmanagedMemory2.CopyDataFrom(unmanagedMemory);
					unmanagedMemory.Dispose();
					unmanagedMemory = unmanagedMemory2;
				}
				int controlCount = this.memory.controlCount;
				ArrayHelpers.AppendListWithCapacity<InputControl, InputControlList<InputControl>>(ref this.controls, ref controlCount, values, 10);
				for (int k = 0; k < num; k++)
				{
					InputActionState.BindingState* ptr3 = unmanagedMemory.bindingStates + (totalBindingCount + k);
					int controlCount2 = ptr3->controlCount;
					int controlStartIndex2 = ptr3->controlStartIndex;
					for (int l = 0; l < controlCount2; l++)
					{
						unmanagedMemory.controlIndexToBindingIndex[controlStartIndex2 + l] = totalBindingCount + k;
					}
				}
				for (int m = this.memory.interactionCount; m < unmanagedMemory.interactionCount; m++)
				{
					InputActionState.InteractionState* ptr4 = unmanagedMemory.interactionStates + m;
					ptr4->phase = InputActionPhase.Waiting;
					ptr4->triggerControlIndex = -1;
				}
				int num20 = this.memory.bindingCount;
				for (int n = 0; n < num2; n++)
				{
					InputAction inputAction3 = actions[n];
					int num21 = totalActionCount + n;
					inputAction3.m_ActionIndexInState = num21;
					unmanagedMemory.actionBindingIndicesAndCounts[num21 * 2] = (ushort)num20;
					int num22 = -1;
					int num23 = 0;
					int num24 = 0;
					for (int num25 = 0; num25 < num; num25++)
					{
						int num26 = totalBindingCount + num25;
						InputActionState.BindingState* ptr5 = unmanagedMemory.bindingStates + num26;
						if (ptr5->actionIndex == num21 && !ptr5->isPartOfComposite)
						{
							unmanagedMemory.actionBindingIndices[num20] = (ushort)num26;
							num20++;
							num23++;
							if (num22 == -1)
							{
								num22 = num26;
							}
							if (ptr5->isComposite)
							{
								if (ptr5->controlCount > 0)
								{
									num24++;
								}
							}
							else
							{
								num24 += ptr5->controlCount;
							}
						}
					}
					if (num22 == -1)
					{
						num22 = 0;
					}
					unmanagedMemory.actionBindingIndicesAndCounts[num21 * 2 + 1] = (ushort)num23;
					bool flag4 = inputAction3.type == InputActionType.PassThrough;
					bool isButton = inputAction3.type == InputActionType.Button;
					bool mayNeedConflictResolution = !flag4 && num24 > 1;
					unmanagedMemory.actionStates[num21] = new InputActionState.TriggerState
					{
						phase = InputActionPhase.Disabled,
						mapIndex = totalMapCount,
						controlIndex = -1,
						interactionIndex = -1,
						isPassThrough = flag4,
						isButton = isButton,
						mayNeedConflictResolution = mayNeedConflictResolution,
						bindingIndex = num22
					};
				}
				unmanagedMemory.mapIndices[totalMapCount] = new InputActionState.ActionMapIndices
				{
					actionStartIndex = totalActionCount,
					actionCount = num2,
					controlStartIndex = totalControlCount,
					controlCount = count,
					bindingStartIndex = totalBindingCount,
					bindingCount = num,
					interactionStartIndex = num3,
					interactionCount = this.totalInteractionCount - num3,
					processorStartIndex = num4,
					processorCount = this.totalProcessorCount - num4,
					compositeStartIndex = num5,
					compositeCount = this.totalCompositeCount - num5
				};
				actionMap.m_MapIndexInState = totalMapCount;
				int mapCount2 = this.memory.mapCount;
				ArrayHelpers.AppendWithCapacity<InputActionMap>(ref this.maps, ref mapCount2, actionMap, 4);
				this.memory.Dispose();
				this.memory = unmanagedMemory;
			}
			catch (Exception)
			{
				unmanagedMemory.Dispose();
				throw;
			}
			finally
			{
				values.Dispose();
			}
		}

		private int InstantiateWithParameters<TType>(TypeTable registrations, string namesAndParameters, ref TType[] array, ref int count, InputActionMap actionMap, ref InputBinding binding)
		{
			if (!NameAndParameters.ParseMultiple(namesAndParameters, ref this.m_Parameters))
			{
				return -1;
			}
			int result = count;
			for (int i = 0; i < this.m_Parameters.Count; i++)
			{
				string name = this.m_Parameters[i].name;
				Type type = registrations.LookupTypeRegistration(name);
				if (type == null)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"No ",
						typeof(TType).Name,
						" with name '",
						name,
						"' (mentioned in '",
						namesAndParameters,
						"') has been registered"
					}));
				}
				else if (!this.m_IsControlOnlyResolve)
				{
					object obj = Activator.CreateInstance(type);
					if (obj is TType)
					{
						TType ttype = (TType)((object)obj);
						InputBindingResolver.ApplyParameters(this.m_Parameters[i].parameters, ttype, actionMap, ref binding, name, namesAndParameters);
						ArrayHelpers.AppendWithCapacity<TType>(ref array, ref count, ttype, 10);
					}
					else
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Type '",
							type.Name,
							"' registered as '",
							name,
							"' (mentioned in '",
							namesAndParameters,
							"') is not an ",
							typeof(TType).Name
						}));
					}
				}
				else
				{
					count++;
				}
			}
			return result;
		}

		private static InputBindingComposite InstantiateBindingComposite(ref InputBinding binding, InputActionMap actionMap)
		{
			NameAndParameters nameAndParameters = NameAndParameters.Parse(binding.effectivePath);
			Type type = InputBindingComposite.s_Composites.LookupTypeRegistration(nameAndParameters.name);
			if (type == null)
			{
				throw new InvalidOperationException("No binding composite with name '" + nameAndParameters.name + "' has been registered");
			}
			InputBindingComposite inputBindingComposite = Activator.CreateInstance(type) as InputBindingComposite;
			if (inputBindingComposite == null)
			{
				throw new InvalidOperationException(string.Concat(new string[]
				{
					"Registered type '",
					type.Name,
					"' used for '",
					nameAndParameters.name,
					"' is not an InputBindingComposite"
				}));
			}
			InputBindingResolver.ApplyParameters(nameAndParameters.parameters, inputBindingComposite, actionMap, ref binding, nameAndParameters.name, binding.effectivePath);
			return inputBindingComposite;
		}

		private static void ApplyParameters(ReadOnlyArray<NamedValue> parameters, object instance, InputActionMap actionMap, ref InputBinding binding, string objectRegistrationName, string namesAndParameters)
		{
			foreach (NamedValue namedValue in parameters)
			{
				FieldInfo field = instance.GetType().GetField(namedValue.name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					Debug.LogError(string.Concat(new string[]
					{
						"Type '",
						instance.GetType().Name,
						"' registered as '",
						objectRegistrationName,
						"' (mentioned in '",
						namesAndParameters,
						"') has no public field called '",
						namedValue.name,
						"'"
					}));
				}
				else
				{
					TypeCode typeCode = Type.GetTypeCode(field.FieldType);
					InputActionRebindingExtensions.ParameterOverride? parameterOverride = InputActionRebindingExtensions.ParameterOverride.Find(actionMap, ref binding, namedValue.name, objectRegistrationName);
					field.SetValue(instance, ((parameterOverride != null) ? parameterOverride.Value.value : namedValue.value).ConvertTo(typeCode).ToObject());
				}
			}
		}

		private static int AssignCompositePartIndex(object composite, string name, ref int currentCompositePartCount)
		{
			Type type = composite.GetType();
			FieldInfo field = type.GetField(name, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field == null)
			{
				throw new InvalidOperationException(string.Format("Cannot find public field '{0}' used as parameter of binding composite '{1}' of type '{2}'", name, composite, type));
			}
			if (field.FieldType != typeof(int))
			{
				throw new InvalidOperationException(string.Format("Field '{0}' used as a parameter of binding composite '{1}' must be of type 'int' but is of type '{2}' instead", name, composite, type.Name));
			}
			int num = (int)field.GetValue(composite);
			if (num == 0)
			{
				int num2 = currentCompositePartCount + 1;
				currentCompositePartCount = num2;
				num = num2;
				field.SetValue(composite, num);
			}
			return num;
		}

		public int totalProcessorCount;

		public int totalCompositeCount;

		public int totalInteractionCount;

		public InputActionMap[] maps;

		public InputControl[] controls;

		public InputActionState.UnmanagedMemory memory;

		public IInputInteraction[] interactions;

		public InputProcessor[] processors;

		public InputBindingComposite[] composites;

		public InputBinding? bindingMask;

		private bool m_IsControlOnlyResolve;

		private List<NameAndParameters> m_Parameters;
	}
}
