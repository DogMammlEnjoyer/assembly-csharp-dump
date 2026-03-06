using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Profiling;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public sealed class InputActionMap : ICloneable, ISerializationCallbackReceiver, IInputActionCollection2, IInputActionCollection, IEnumerable<InputAction>, IEnumerable, IDisposable
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public InputActionAsset asset
		{
			get
			{
				return this.m_Asset;
			}
		}

		public Guid id
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_Id))
				{
					this.GenerateId();
				}
				return new Guid(this.m_Id);
			}
		}

		internal Guid idDontGenerate
		{
			get
			{
				if (string.IsNullOrEmpty(this.m_Id))
				{
					return default(Guid);
				}
				return new Guid(this.m_Id);
			}
		}

		public bool enabled
		{
			get
			{
				return this.m_EnabledActionsCount > 0;
			}
		}

		public ReadOnlyArray<InputAction> actions
		{
			get
			{
				return new ReadOnlyArray<InputAction>(this.m_Actions);
			}
		}

		public ReadOnlyArray<InputBinding> bindings
		{
			get
			{
				return new ReadOnlyArray<InputBinding>(this.m_Bindings);
			}
		}

		IEnumerable<InputBinding> IInputActionCollection2.bindings
		{
			get
			{
				return this.bindings;
			}
		}

		public ReadOnlyArray<InputControlScheme> controlSchemes
		{
			get
			{
				if (this.m_Asset == null)
				{
					return default(ReadOnlyArray<InputControlScheme>);
				}
				return this.m_Asset.controlSchemes;
			}
		}

		public InputBinding? bindingMask
		{
			get
			{
				return this.m_BindingMask;
			}
			set
			{
				if (this.m_BindingMask == value)
				{
					return;
				}
				this.m_BindingMask = value;
				this.LazyResolveBindings(true);
			}
		}

		public ReadOnlyArray<InputDevice>? devices
		{
			get
			{
				ReadOnlyArray<InputDevice>? result = this.m_Devices.Get();
				if (result != null)
				{
					return result;
				}
				InputActionAsset asset = this.m_Asset;
				if (asset == null)
				{
					return null;
				}
				return asset.devices;
			}
			set
			{
				if (this.m_Devices.Set(value))
				{
					this.LazyResolveBindings(false);
				}
			}
		}

		public InputAction this[string actionNameOrId]
		{
			get
			{
				if (actionNameOrId == null)
				{
					throw new ArgumentNullException("actionNameOrId");
				}
				InputAction inputAction = this.FindAction(actionNameOrId, false);
				if (inputAction == null)
				{
					throw new KeyNotFoundException("Cannot find action '" + actionNameOrId + "'");
				}
				return inputAction;
			}
		}

		public event Action<InputAction.CallbackContext> actionTriggered
		{
			add
			{
				this.m_ActionCallbacks.AddCallback(value);
			}
			remove
			{
				this.m_ActionCallbacks.RemoveCallback(value);
			}
		}

		public InputActionMap()
		{
			InputActionMap.s_NeedToResolveBindings = true;
		}

		public InputActionMap(string name) : this()
		{
			this.m_Name = name;
		}

		public void Dispose()
		{
			InputActionState state = this.m_State;
			if (state == null)
			{
				return;
			}
			state.Dispose();
		}

		internal int FindActionIndex(string nameOrId)
		{
			if (string.IsNullOrEmpty(nameOrId))
			{
				return -1;
			}
			if (this.m_Actions == null)
			{
				return -1;
			}
			this.SetUpActionLookupTable();
			int num = this.m_Actions.Length;
			if (nameOrId.StartsWith("{") && nameOrId.EndsWith("}"))
			{
				int length = nameOrId.Length - 2;
				for (int i = 0; i < num; i++)
				{
					if (string.Compare(this.m_Actions[i].m_Id, 0, nameOrId, 1, length) == 0)
					{
						return i;
					}
				}
			}
			int result;
			if (this.m_ActionIndexByNameOrId.TryGetValue(nameOrId, out result))
			{
				return result;
			}
			for (int j = 0; j < num; j++)
			{
				if (this.m_Actions[j].m_Id == nameOrId || string.Compare(this.m_Actions[j].m_Name, nameOrId, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return j;
				}
			}
			return -1;
		}

		private void SetUpActionLookupTable()
		{
			if (this.m_ActionIndexByNameOrId != null || this.m_Actions == null)
			{
				return;
			}
			this.m_ActionIndexByNameOrId = new Dictionary<string, int>();
			int num = this.m_Actions.Length;
			for (int i = 0; i < num; i++)
			{
				InputAction inputAction = this.m_Actions[i];
				inputAction.MakeSureIdIsInPlace();
				this.m_ActionIndexByNameOrId[inputAction.name] = i;
				this.m_ActionIndexByNameOrId[inputAction.m_Id] = i;
			}
		}

		internal void ClearActionLookupTable()
		{
			Dictionary<string, int> actionIndexByNameOrId = this.m_ActionIndexByNameOrId;
			if (actionIndexByNameOrId == null)
			{
				return;
			}
			actionIndexByNameOrId.Clear();
		}

		private int FindActionIndex(Guid id)
		{
			if (this.m_Actions == null)
			{
				return -1;
			}
			int num = this.m_Actions.Length;
			for (int i = 0; i < num; i++)
			{
				if (this.m_Actions[i].idDontGenerate == id)
				{
					return i;
				}
			}
			return -1;
		}

		public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
		{
			if (actionNameOrId == null)
			{
				throw new ArgumentNullException("actionNameOrId");
			}
			int num = this.FindActionIndex(actionNameOrId);
			if (num != -1)
			{
				return this.m_Actions[num];
			}
			if (throwIfNotFound)
			{
				throw new ArgumentException(string.Format("No action '{0}' in '{1}'", actionNameOrId, this), "actionNameOrId");
			}
			return null;
		}

		public InputAction FindAction(Guid id)
		{
			int num = this.FindActionIndex(id);
			if (num == -1)
			{
				return null;
			}
			return this.m_Actions[num];
		}

		public bool IsUsableWithDevice(InputDevice device)
		{
			if (device == null)
			{
				throw new ArgumentNullException("device");
			}
			if (this.m_Bindings == null)
			{
				return false;
			}
			foreach (InputBinding inputBinding in this.m_Bindings)
			{
				string effectivePath = inputBinding.effectivePath;
				if (!string.IsNullOrEmpty(effectivePath) && InputControlPath.Matches(effectivePath, device))
				{
					return true;
				}
			}
			return false;
		}

		public void Enable()
		{
			if (this.m_Actions == null || this.m_EnabledActionsCount == this.m_Actions.Length)
			{
				return;
			}
			this.ResolveBindingsIfNecessary();
			this.m_State.EnableAllActions(this);
		}

		public void Disable()
		{
			if (!this.enabled)
			{
				return;
			}
			this.m_State.DisableAllActions(this);
		}

		public InputActionMap Clone()
		{
			InputActionMap inputActionMap = new InputActionMap
			{
				m_Name = this.m_Name
			};
			if (this.m_Actions != null)
			{
				int num = this.m_Actions.Length;
				InputAction[] array = new InputAction[num];
				for (int i = 0; i < num; i++)
				{
					InputAction inputAction = this.m_Actions[i];
					array[i] = new InputAction
					{
						m_Name = inputAction.m_Name,
						m_ActionMap = inputActionMap,
						m_Type = inputAction.m_Type,
						m_Interactions = inputAction.m_Interactions,
						m_Processors = inputAction.m_Processors,
						m_ExpectedControlType = inputAction.m_ExpectedControlType,
						m_Flags = inputAction.m_Flags
					};
				}
				inputActionMap.m_Actions = array;
			}
			if (this.m_Bindings != null)
			{
				int num2 = this.m_Bindings.Length;
				InputBinding[] array2 = new InputBinding[num2];
				Array.Copy(this.m_Bindings, 0, array2, 0, num2);
				for (int j = 0; j < num2; j++)
				{
					array2[j].m_Id = null;
				}
				inputActionMap.m_Bindings = array2;
			}
			return inputActionMap;
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public bool Contains(InputAction action)
		{
			return action != null && action.actionMap == this;
		}

		public override string ToString()
		{
			if (this.m_Asset != null)
			{
				return string.Format("{0}:{1}", this.m_Asset, this.m_Name);
			}
			if (!string.IsNullOrEmpty(this.m_Name))
			{
				return this.m_Name;
			}
			return "<Unnamed Action Map>";
		}

		public IEnumerator<InputAction> GetEnumerator()
		{
			return this.actions.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private bool needToResolveBindings
		{
			get
			{
				return (this.m_Flags & InputActionMap.Flags.NeedToResolveBindings) > (InputActionMap.Flags)0;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputActionMap.Flags.NeedToResolveBindings;
					return;
				}
				this.m_Flags &= ~InputActionMap.Flags.NeedToResolveBindings;
			}
		}

		private bool bindingResolutionNeedsFullReResolve
		{
			get
			{
				return (this.m_Flags & InputActionMap.Flags.BindingResolutionNeedsFullReResolve) > (InputActionMap.Flags)0;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputActionMap.Flags.BindingResolutionNeedsFullReResolve;
					return;
				}
				this.m_Flags &= ~InputActionMap.Flags.BindingResolutionNeedsFullReResolve;
			}
		}

		private bool controlsForEachActionInitialized
		{
			get
			{
				return (this.m_Flags & InputActionMap.Flags.ControlsForEachActionInitialized) > (InputActionMap.Flags)0;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputActionMap.Flags.ControlsForEachActionInitialized;
					return;
				}
				this.m_Flags &= ~InputActionMap.Flags.ControlsForEachActionInitialized;
			}
		}

		private bool bindingsForEachActionInitialized
		{
			get
			{
				return (this.m_Flags & InputActionMap.Flags.BindingsForEachActionInitialized) > (InputActionMap.Flags)0;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputActionMap.Flags.BindingsForEachActionInitialized;
					return;
				}
				this.m_Flags &= ~InputActionMap.Flags.BindingsForEachActionInitialized;
			}
		}

		internal ReadOnlyArray<InputBinding> GetBindingsForSingleAction(InputAction action)
		{
			if (!this.bindingsForEachActionInitialized)
			{
				this.SetUpPerActionControlAndBindingArrays();
			}
			return new ReadOnlyArray<InputBinding>(this.m_BindingsForEachAction, action.m_BindingsStartIndex, action.m_BindingsCount);
		}

		internal ReadOnlyArray<InputControl> GetControlsForSingleAction(InputAction action)
		{
			if (!this.controlsForEachActionInitialized)
			{
				this.SetUpPerActionControlAndBindingArrays();
			}
			return new ReadOnlyArray<InputControl>(this.m_ControlsForEachAction, action.m_ControlStartIndex, action.m_ControlCount);
		}

		private unsafe void SetUpPerActionControlAndBindingArrays()
		{
			if (this.m_Bindings == null)
			{
				this.m_ControlsForEachAction = null;
				this.m_BindingsForEachAction = null;
				this.controlsForEachActionInitialized = true;
				this.bindingsForEachActionInitialized = true;
				return;
			}
			if (this.m_SingletonAction != null)
			{
				this.m_BindingsForEachAction = this.m_Bindings;
				InputActionState state = this.m_State;
				this.m_ControlsForEachAction = ((state != null) ? state.controls : null);
				this.m_SingletonAction.m_BindingsStartIndex = 0;
				this.m_SingletonAction.m_BindingsCount = this.m_Bindings.Length;
				this.m_SingletonAction.m_ControlStartIndex = 0;
				InputAction singletonAction = this.m_SingletonAction;
				InputActionState state2 = this.m_State;
				singletonAction.m_ControlCount = ((state2 != null) ? state2.totalControlCount : 0);
				if (this.m_ControlsForEachAction.HaveDuplicateReferences(0, this.m_SingletonAction.m_ControlCount))
				{
					int num = 0;
					InputControl[] array = new InputControl[this.m_SingletonAction.m_ControlCount];
					for (int i = 0; i < this.m_SingletonAction.m_ControlCount; i++)
					{
						if (!array.ContainsReference(this.m_ControlsForEachAction[i]))
						{
							array[num] = this.m_ControlsForEachAction[i];
							num++;
						}
					}
					this.m_ControlsForEachAction = array;
					this.m_SingletonAction.m_ControlCount = num;
				}
			}
			else
			{
				InputActionState state3 = this.m_State;
				InputActionState.ActionMapIndices actionMapIndices = (state3 != null) ? state3.FetchMapIndices(this) : default(InputActionState.ActionMapIndices);
				for (int j = 0; j < this.m_Actions.Length; j++)
				{
					InputAction inputAction = this.m_Actions[j];
					inputAction.m_BindingsCount = 0;
					inputAction.m_BindingsStartIndex = -1;
					inputAction.m_ControlCount = 0;
					inputAction.m_ControlStartIndex = -1;
				}
				int num2 = this.m_Bindings.Length;
				for (int k = 0; k < num2; k++)
				{
					InputAction inputAction2 = this.FindAction(this.m_Bindings[k].action, false);
					if (inputAction2 != null)
					{
						inputAction2.m_BindingsCount++;
					}
				}
				int num3 = 0;
				if (this.m_State != null && (this.m_ControlsForEachAction == null || this.m_ControlsForEachAction.Length != actionMapIndices.controlCount))
				{
					if (actionMapIndices.controlCount == 0)
					{
						this.m_ControlsForEachAction = null;
					}
					else
					{
						this.m_ControlsForEachAction = new InputControl[actionMapIndices.controlCount];
					}
				}
				InputBinding[] array2 = null;
				int num4 = 0;
				int l = 0;
				while (l < this.m_Bindings.Length)
				{
					InputAction inputAction3 = this.FindAction(this.m_Bindings[l].action, false);
					if (inputAction3 == null || inputAction3.m_BindingsStartIndex != -1)
					{
						l++;
					}
					else
					{
						inputAction3.m_BindingsStartIndex = ((array2 != null) ? num3 : l);
						inputAction3.m_ControlStartIndex = num4;
						int bindingsCount = inputAction3.m_BindingsCount;
						int num5 = l;
						for (int m = 0; m < bindingsCount; m++)
						{
							if (this.FindAction(this.m_Bindings[num5].action, false) != inputAction3)
							{
								if (array2 == null)
								{
									array2 = new InputBinding[this.m_Bindings.Length];
									num3 = num5;
									Array.Copy(this.m_Bindings, 0, array2, 0, num5);
								}
								do
								{
									num5++;
								}
								while (this.FindAction(this.m_Bindings[num5].action, false) != inputAction3);
							}
							else if (l == num5)
							{
								l++;
							}
							if (array2 != null)
							{
								array2[num3++] = this.m_Bindings[num5];
							}
							if (this.m_State != null && !this.m_Bindings[num5].isComposite)
							{
								ref InputActionState.BindingState ptr = ref this.m_State.bindingStates[actionMapIndices.bindingStartIndex + num5];
								int controlCount = ptr.controlCount;
								if (controlCount > 0)
								{
									int controlStartIndex = ptr.controlStartIndex;
									for (int n = 0; n < controlCount; n++)
									{
										InputControl inputControl = this.m_State.controls[controlStartIndex + n];
										if (!this.m_ControlsForEachAction.ContainsReference(inputAction3.m_ControlStartIndex, inputAction3.m_ControlCount, inputControl))
										{
											this.m_ControlsForEachAction[num4] = inputControl;
											num4++;
											inputAction3.m_ControlCount++;
										}
									}
								}
							}
							num5++;
						}
					}
				}
				if (array2 == null)
				{
					this.m_BindingsForEachAction = this.m_Bindings;
				}
				else
				{
					this.m_BindingsForEachAction = array2;
				}
			}
			this.controlsForEachActionInitialized = true;
			this.bindingsForEachActionInitialized = true;
		}

		internal void OnWantToChangeSetup()
		{
			if (this.asset != null)
			{
				using (ReadOnlyArray<InputActionMap>.Enumerator enumerator = this.asset.actionMaps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.enabled)
						{
							throw new InvalidOperationException(string.Format("Cannot add, remove, or change elements of InputActionAsset {0} while one or more of its actions are enabled", this.asset));
						}
					}
					return;
				}
			}
			if (this.enabled)
			{
				throw new InvalidOperationException(string.Format("Cannot add, remove, or change elements of InputActionMap {0} while one or more of its actions are enabled", this));
			}
		}

		internal void OnSetupChanged()
		{
			if (this.m_Asset != null)
			{
				this.m_Asset.MarkAsDirty();
				using (ReadOnlyArray<InputActionMap>.Enumerator enumerator = this.m_Asset.actionMaps.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						InputActionMap inputActionMap = enumerator.Current;
						inputActionMap.m_State = null;
					}
					goto IL_5C;
				}
			}
			this.m_State = null;
			IL_5C:
			this.ClearCachedActionData(false);
			this.LazyResolveBindings(true);
		}

		internal void OnBindingModified()
		{
			this.ClearCachedActionData(false);
			this.LazyResolveBindings(true);
		}

		internal void ClearCachedActionData(bool onlyControls = false)
		{
			if (!onlyControls)
			{
				this.bindingsForEachActionInitialized = false;
				this.m_BindingsForEachAction = null;
				this.m_ActionIndexByNameOrId = null;
			}
			this.controlsForEachActionInitialized = false;
			this.m_ControlsForEachAction = null;
		}

		internal void GenerateId()
		{
			this.m_Id = Guid.NewGuid().ToString();
		}

		internal bool LazyResolveBindings(bool fullResolve)
		{
			this.m_ControlsForEachAction = null;
			this.controlsForEachActionInitialized = false;
			InputActionMap.s_NeedToResolveBindings = true;
			if (this.m_State == null)
			{
				return false;
			}
			this.needToResolveBindings = true;
			this.bindingResolutionNeedsFullReResolve = (this.bindingResolutionNeedsFullReResolve || fullResolve);
			if (InputActionMap.s_DeferBindingResolution > 0)
			{
				return false;
			}
			this.ResolveBindings();
			return true;
		}

		internal bool ResolveBindingsIfNecessary()
		{
			if (this.m_State != null && !this.needToResolveBindings)
			{
				return false;
			}
			if (this.m_State != null && this.m_State.isProcessingControlStateChange)
			{
				return false;
			}
			this.ResolveBindings();
			return true;
		}

		internal void ResolveBindings()
		{
			using (InputActionMap.k_ResolveBindingsProfilerMarker.Auto())
			{
				using (InputActionRebindingExtensions.DeferBindingResolution())
				{
					InputActionState.UnmanagedMemory oldMemory = default(InputActionState.UnmanagedMemory);
					try
					{
						InputBindingResolver resolver = default(InputBindingResolver);
						bool flag = this.m_State == null;
						OneOrMore<InputActionMap, ReadOnlyArray<InputActionMap>> oneOrMore;
						if (this.m_Asset != null)
						{
							oneOrMore = this.m_Asset.actionMaps;
							resolver.bindingMask = this.m_Asset.m_BindingMask;
							using (IEnumerator<InputActionMap> enumerator = oneOrMore.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									InputActionMap inputActionMap = enumerator.Current;
									flag |= inputActionMap.bindingResolutionNeedsFullReResolve;
									inputActionMap.needToResolveBindings = false;
									inputActionMap.bindingResolutionNeedsFullReResolve = false;
									inputActionMap.controlsForEachActionInitialized = false;
								}
								goto IL_D8;
							}
						}
						oneOrMore = this;
						flag |= this.bindingResolutionNeedsFullReResolve;
						this.needToResolveBindings = false;
						this.bindingResolutionNeedsFullReResolve = false;
						this.controlsForEachActionInitialized = false;
						IL_D8:
						bool hasEnabledActions = false;
						InputControlList<InputControl> activeControls = default(InputControlList<InputControl>);
						if (this.m_State != null)
						{
							oldMemory = this.m_State.memory.Clone();
							this.m_State.PrepareForBindingReResolution(flag, ref activeControls, ref hasEnabledActions);
							resolver.StartWithPreviousResolve(this.m_State, flag);
							this.m_State.memory.Dispose();
						}
						foreach (InputActionMap actionMap in oneOrMore)
						{
							resolver.AddActionMap(actionMap);
						}
						if (this.m_State == null)
						{
							this.m_State = new InputActionState();
							this.m_State.Initialize(resolver);
						}
						else
						{
							this.m_State.ClaimDataFrom(resolver);
						}
						if (this.m_Asset != null)
						{
							foreach (InputActionMap inputActionMap2 in oneOrMore)
							{
								inputActionMap2.m_State = this.m_State;
							}
							this.m_Asset.m_SharedStateForAllMaps = this.m_State;
						}
						this.m_State.FinishBindingResolution(hasEnabledActions, oldMemory, activeControls, flag);
					}
					finally
					{
						oldMemory.Dispose();
					}
				}
			}
		}

		public int FindBinding(InputBinding mask, out InputAction action)
		{
			int num = this.FindBindingRelativeToMap(mask);
			if (num == -1)
			{
				action = null;
				return -1;
			}
			action = (this.m_SingletonAction ?? this.FindAction(this.bindings[num].action, false));
			return action.BindingIndexOnMapToBindingIndexOnAction(num);
		}

		internal int FindBindingRelativeToMap(InputBinding mask)
		{
			InputBinding[] bindings = this.m_Bindings;
			int num = bindings.LengthSafe<InputBinding>();
			for (int i = 0; i < num; i++)
			{
				ref InputBinding binding = ref bindings[i];
				if (mask.Matches(ref binding, (InputBinding.MatchOptions)0))
				{
					return i;
				}
			}
			return -1;
		}

		public static InputActionMap[] FromJson(string json)
		{
			if (json == null)
			{
				throw new ArgumentNullException("json");
			}
			return JsonUtility.FromJson<InputActionMap.ReadFileJson>(json).ToMaps();
		}

		public static string ToJson(IEnumerable<InputActionMap> maps)
		{
			if (maps == null)
			{
				throw new ArgumentNullException("maps");
			}
			return JsonUtility.ToJson(InputActionMap.WriteFileJson.FromMaps(maps), true);
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(InputActionMap.WriteFileJson.FromMap(this), true);
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			InputActionMap.s_NeedToResolveBindings = true;
			this.m_State = null;
			this.m_MapIndexInState = -1;
			this.m_EnabledActionsCount = 0;
			if (this.m_Actions != null)
			{
				int num = this.m_Actions.Length;
				for (int i = 0; i < num; i++)
				{
					this.m_Actions[i].m_ActionMap = this;
				}
			}
			this.ClearCachedActionData(false);
			this.ClearActionLookupTable();
		}

		private static readonly ProfilerMarker k_ResolveBindingsProfilerMarker = new ProfilerMarker("InputActionMap.ResolveBindings");

		[SerializeField]
		internal string m_Name;

		[SerializeField]
		internal string m_Id;

		[SerializeField]
		internal InputActionAsset m_Asset;

		[SerializeField]
		internal InputAction[] m_Actions;

		[SerializeField]
		internal InputBinding[] m_Bindings;

		[NonSerialized]
		private InputBinding[] m_BindingsForEachAction;

		[NonSerialized]
		private InputControl[] m_ControlsForEachAction;

		[NonSerialized]
		internal int m_EnabledActionsCount;

		[NonSerialized]
		internal InputAction m_SingletonAction;

		[NonSerialized]
		internal int m_MapIndexInState = -1;

		[NonSerialized]
		internal InputActionState m_State;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		private InputActionMap.Flags m_Flags;

		[NonSerialized]
		internal int m_ParameterOverridesCount;

		[NonSerialized]
		internal InputActionRebindingExtensions.ParameterOverride[] m_ParameterOverrides;

		[NonSerialized]
		internal InputActionMap.DeviceArray m_Devices;

		[NonSerialized]
		internal CallbackArray<Action<InputAction.CallbackContext>> m_ActionCallbacks;

		[NonSerialized]
		internal Dictionary<string, int> m_ActionIndexByNameOrId;

		internal static int s_DeferBindingResolution;

		internal static bool s_NeedToResolveBindings;

		[Flags]
		private enum Flags
		{
			NeedToResolveBindings = 1,
			BindingResolutionNeedsFullReResolve = 2,
			ControlsForEachActionInitialized = 4,
			BindingsForEachActionInitialized = 8
		}

		internal struct DeviceArray
		{
			public int IndexOf(InputDevice device)
			{
				return this.m_DeviceArray.IndexOfReference(device, this.m_DeviceCount);
			}

			public bool Remove(InputDevice device)
			{
				int num = this.IndexOf(device);
				if (num < 0)
				{
					return false;
				}
				this.m_DeviceArray.EraseAtWithCapacity(ref this.m_DeviceCount, num);
				return true;
			}

			public ReadOnlyArray<InputDevice>? Get()
			{
				if (!this.m_HaveValue)
				{
					return null;
				}
				return new ReadOnlyArray<InputDevice>?(new ReadOnlyArray<InputDevice>(this.m_DeviceArray, 0, this.m_DeviceCount));
			}

			public bool Set(ReadOnlyArray<InputDevice>? devices)
			{
				if (devices == null)
				{
					if (!this.m_HaveValue)
					{
						return false;
					}
					if (this.m_DeviceCount > 0)
					{
						Array.Clear(this.m_DeviceArray, 0, this.m_DeviceCount);
					}
					this.m_DeviceCount = 0;
					this.m_HaveValue = false;
				}
				else
				{
					ReadOnlyArray<InputDevice> value = devices.Value;
					if (this.m_HaveValue && value.Count == this.m_DeviceCount && value.HaveEqualReferences(this.m_DeviceArray, this.m_DeviceCount))
					{
						return false;
					}
					if (this.m_DeviceCount > 0)
					{
						this.m_DeviceArray.Clear(ref this.m_DeviceCount);
					}
					this.m_HaveValue = true;
					this.m_DeviceCount = 0;
					ArrayHelpers.AppendListWithCapacity<InputDevice, ReadOnlyArray<InputDevice>>(ref this.m_DeviceArray, ref this.m_DeviceCount, value, 10);
				}
				return true;
			}

			private bool m_HaveValue;

			private int m_DeviceCount;

			private InputDevice[] m_DeviceArray;
		}

		[Serializable]
		internal struct BindingOverrideListJson
		{
			public List<InputActionMap.BindingOverrideJson> bindings;
		}

		[Serializable]
		internal struct BindingOverrideJson
		{
			public static InputActionMap.BindingOverrideJson FromBinding(InputBinding binding, string actionName)
			{
				return new InputActionMap.BindingOverrideJson
				{
					action = actionName,
					id = binding.id.ToString(),
					path = (binding.overridePath ?? "null"),
					interactions = (binding.overrideInteractions ?? "null"),
					processors = (binding.overrideProcessors ?? "null")
				};
			}

			public static InputActionMap.BindingOverrideJson FromBinding(InputBinding binding)
			{
				return InputActionMap.BindingOverrideJson.FromBinding(binding, binding.action);
			}

			public static InputBinding ToBinding(InputActionMap.BindingOverrideJson bindingOverride)
			{
				return new InputBinding
				{
					overridePath = ((bindingOverride.path != "null") ? bindingOverride.path : null),
					overrideInteractions = ((bindingOverride.interactions != "null") ? bindingOverride.interactions : null),
					overrideProcessors = ((bindingOverride.processors != "null") ? bindingOverride.processors : null)
				};
			}

			public string action;

			public string id;

			public string path;

			public string interactions;

			public string processors;
		}

		[Serializable]
		internal struct BindingJson
		{
			public InputBinding ToBinding()
			{
				return new InputBinding
				{
					name = (string.IsNullOrEmpty(this.name) ? null : this.name),
					m_Id = (string.IsNullOrEmpty(this.id) ? null : this.id),
					path = this.path,
					action = (string.IsNullOrEmpty(this.action) ? null : this.action),
					interactions = (string.IsNullOrEmpty(this.interactions) ? null : this.interactions),
					processors = (string.IsNullOrEmpty(this.processors) ? null : this.processors),
					groups = (string.IsNullOrEmpty(this.groups) ? null : this.groups),
					isComposite = this.isComposite,
					isPartOfComposite = this.isPartOfComposite
				};
			}

			public static InputActionMap.BindingJson FromBinding(ref InputBinding binding)
			{
				return new InputActionMap.BindingJson
				{
					name = binding.name,
					id = binding.m_Id,
					path = binding.path,
					action = binding.action,
					interactions = binding.interactions,
					processors = binding.processors,
					groups = binding.groups,
					isComposite = binding.isComposite,
					isPartOfComposite = binding.isPartOfComposite
				};
			}

			public string name;

			public string id;

			public string path;

			public string interactions;

			public string processors;

			public string groups;

			public string action;

			public bool isComposite;

			public bool isPartOfComposite;
		}

		[Serializable]
		internal struct ReadActionJson
		{
			public InputAction ToAction(string actionName = null)
			{
				if (!string.IsNullOrEmpty(this.expectedControlLayout))
				{
					this.expectedControlType = this.expectedControlLayout;
				}
				InputActionType inputActionType = InputActionType.Value;
				if (!string.IsNullOrEmpty(this.type))
				{
					inputActionType = (InputActionType)Enum.Parse(typeof(InputActionType), this.type, true);
				}
				else if (this.passThrough)
				{
					inputActionType = InputActionType.PassThrough;
				}
				else if (this.initialStateCheck)
				{
					inputActionType = InputActionType.Value;
				}
				else if (!string.IsNullOrEmpty(this.expectedControlType) && (this.expectedControlType == "Button" || this.expectedControlType == "Key"))
				{
					inputActionType = InputActionType.Button;
				}
				return new InputAction(actionName ?? this.name, inputActionType, null, null, null, null)
				{
					m_Id = (string.IsNullOrEmpty(this.id) ? null : this.id),
					m_ExpectedControlType = ((!string.IsNullOrEmpty(this.expectedControlType)) ? this.expectedControlType : null),
					m_Processors = this.processors,
					m_Interactions = this.interactions,
					wantsInitialStateCheck = this.initialStateCheck
				};
			}

			public string name;

			public string type;

			public string id;

			public string expectedControlType;

			public string expectedControlLayout;

			public string processors;

			public string interactions;

			public bool passThrough;

			public bool initialStateCheck;

			public InputActionMap.BindingJson[] bindings;
		}

		[Serializable]
		internal struct WriteActionJson
		{
			public static InputActionMap.WriteActionJson FromAction(InputAction action)
			{
				return new InputActionMap.WriteActionJson
				{
					name = action.m_Name,
					type = action.m_Type.ToString(),
					id = action.m_Id,
					expectedControlType = action.m_ExpectedControlType,
					processors = action.processors,
					interactions = action.interactions,
					initialStateCheck = action.wantsInitialStateCheck
				};
			}

			public string name;

			public string type;

			public string id;

			public string expectedControlType;

			public string processors;

			public string interactions;

			public bool initialStateCheck;
		}

		[Serializable]
		internal struct ReadMapJson
		{
			public string name;

			public string id;

			public InputActionMap.ReadActionJson[] actions;

			public InputActionMap.BindingJson[] bindings;
		}

		[Serializable]
		internal struct WriteMapJson
		{
			public static InputActionMap.WriteMapJson FromMap(InputActionMap map)
			{
				InputActionMap.WriteActionJson[] array = null;
				InputActionMap.BindingJson[] array2 = null;
				InputAction[] array3 = map.m_Actions;
				if (array3 != null)
				{
					int num = array3.Length;
					array = new InputActionMap.WriteActionJson[num];
					for (int i = 0; i < num; i++)
					{
						array[i] = InputActionMap.WriteActionJson.FromAction(array3[i]);
					}
				}
				InputBinding[] array4 = map.m_Bindings;
				if (array4 != null)
				{
					int num2 = array4.Length;
					array2 = new InputActionMap.BindingJson[num2];
					for (int j = 0; j < num2; j++)
					{
						array2[j] = InputActionMap.BindingJson.FromBinding(ref array4[j]);
					}
				}
				return new InputActionMap.WriteMapJson
				{
					name = map.name,
					id = map.id.ToString(),
					actions = array,
					bindings = array2
				};
			}

			public string name;

			public string id;

			public InputActionMap.WriteActionJson[] actions;

			public InputActionMap.BindingJson[] bindings;
		}

		[Serializable]
		internal struct WriteFileJson
		{
			public static InputActionMap.WriteFileJson FromMap(InputActionMap map)
			{
				return new InputActionMap.WriteFileJson
				{
					maps = new InputActionMap.WriteMapJson[]
					{
						InputActionMap.WriteMapJson.FromMap(map)
					}
				};
			}

			public static InputActionMap.WriteFileJson FromMaps(IEnumerable<InputActionMap> maps)
			{
				int num = maps.Count<InputActionMap>();
				if (num == 0)
				{
					return default(InputActionMap.WriteFileJson);
				}
				InputActionMap.WriteMapJson[] array = new InputActionMap.WriteMapJson[num];
				int num2 = 0;
				foreach (InputActionMap map in maps)
				{
					array[num2++] = InputActionMap.WriteMapJson.FromMap(map);
				}
				return new InputActionMap.WriteFileJson
				{
					maps = array
				};
			}

			public InputActionMap.WriteMapJson[] maps;
		}

		[Serializable]
		internal struct ReadFileJson
		{
			public InputActionMap[] ToMaps()
			{
				List<InputActionMap> list = new List<InputActionMap>();
				List<List<InputAction>> list2 = new List<List<InputAction>>();
				List<List<InputBinding>> list3 = new List<List<InputBinding>>();
				InputActionMap.ReadActionJson[] array = this.actions;
				int num = (array != null) ? array.Length : 0;
				for (int i = 0; i < num; i++)
				{
					InputActionMap.ReadActionJson readActionJson = this.actions[i];
					if (string.IsNullOrEmpty(readActionJson.name))
					{
						throw new InvalidOperationException(string.Format("Action number {0} has no name", i + 1));
					}
					string text = null;
					string text2 = readActionJson.name;
					int num2 = text2.IndexOf('/');
					if (num2 != -1)
					{
						text = text2.Substring(0, num2);
						text2 = text2.Substring(num2 + 1);
						if (string.IsNullOrEmpty(text2))
						{
							throw new InvalidOperationException("Invalid action name '" + readActionJson.name + "' (missing action name after '/')");
						}
					}
					InputActionMap inputActionMap = null;
					int j;
					for (j = 0; j < list.Count; j++)
					{
						if (string.Compare(list[j].name, text, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							inputActionMap = list[j];
							break;
						}
					}
					if (inputActionMap == null)
					{
						inputActionMap = new InputActionMap(text);
						j = list.Count;
						list.Add(inputActionMap);
						list2.Add(new List<InputAction>());
						list3.Add(new List<InputBinding>());
					}
					InputAction inputAction = readActionJson.ToAction(text2);
					list2[j].Add(inputAction);
					if (readActionJson.bindings != null)
					{
						List<InputBinding> list4 = list3[j];
						for (int k = 0; k < readActionJson.bindings.Length; k++)
						{
							InputActionMap.BindingJson bindingJson = readActionJson.bindings[k];
							InputBinding item = bindingJson.ToBinding();
							item.action = inputAction.m_Name;
							list4.Add(item);
						}
					}
				}
				InputActionMap.ReadMapJson[] array2 = this.maps;
				int num3 = (array2 != null) ? array2.Length : 0;
				for (int l = 0; l < num3; l++)
				{
					InputActionMap.ReadMapJson readMapJson = this.maps[l];
					string name = readMapJson.name;
					if (string.IsNullOrEmpty(name))
					{
						throw new InvalidOperationException(string.Format("Map number {0} has no name", l + 1));
					}
					InputActionMap inputActionMap2 = null;
					int m;
					for (m = 0; m < list.Count; m++)
					{
						if (string.Compare(list[m].name, name, StringComparison.InvariantCultureIgnoreCase) == 0)
						{
							inputActionMap2 = list[m];
							break;
						}
					}
					if (inputActionMap2 == null)
					{
						inputActionMap2 = new InputActionMap(name)
						{
							m_Id = (string.IsNullOrEmpty(readMapJson.id) ? null : readMapJson.id)
						};
						m = list.Count;
						list.Add(inputActionMap2);
						list2.Add(new List<InputAction>());
						list3.Add(new List<InputBinding>());
					}
					InputActionMap.ReadActionJson[] array3 = readMapJson.actions;
					int num4 = (array3 != null) ? array3.Length : 0;
					for (int n = 0; n < num4; n++)
					{
						InputActionMap.ReadActionJson readActionJson2 = readMapJson.actions[n];
						if (string.IsNullOrEmpty(readActionJson2.name))
						{
							throw new InvalidOperationException(string.Format("Action number {0} in map '{1}' has no name", l + 1, name));
						}
						InputAction inputAction2 = readActionJson2.ToAction(null);
						list2[m].Add(inputAction2);
						if (readActionJson2.bindings != null)
						{
							List<InputBinding> list5 = list3[m];
							for (int num5 = 0; num5 < readActionJson2.bindings.Length; num5++)
							{
								InputActionMap.BindingJson bindingJson2 = readActionJson2.bindings[num5];
								InputBinding item2 = bindingJson2.ToBinding();
								item2.action = inputAction2.m_Name;
								list5.Add(item2);
							}
						}
					}
					InputActionMap.BindingJson[] bindings = readMapJson.bindings;
					int num6 = (bindings != null) ? bindings.Length : 0;
					List<InputBinding> list6 = list3[m];
					for (int num7 = 0; num7 < num6; num7++)
					{
						InputActionMap.BindingJson bindingJson3 = readMapJson.bindings[num7];
						InputBinding item3 = bindingJson3.ToBinding();
						list6.Add(item3);
					}
				}
				for (int num8 = 0; num8 < list.Count; num8++)
				{
					InputActionMap inputActionMap3 = list[num8];
					InputAction[] array4 = list2[num8].ToArray();
					InputBinding[] bindings2 = list3[num8].ToArray();
					inputActionMap3.m_Actions = array4;
					inputActionMap3.m_Bindings = bindings2;
					for (int num9 = 0; num9 < array4.Length; num9++)
					{
						array4[num9].m_ActionMap = inputActionMap3;
					}
				}
				return list.ToArray();
			}

			public InputActionMap.ReadActionJson[] actions;

			public InputActionMap.ReadMapJson[] maps;
		}
	}
}
