using System;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Serialization;

namespace UnityEngine.InputSystem
{
	[Serializable]
	public sealed class InputAction : ICloneable, IDisposable
	{
		public string name
		{
			get
			{
				return this.m_Name;
			}
		}

		public InputActionType type
		{
			get
			{
				return this.m_Type;
			}
		}

		public Guid id
		{
			get
			{
				this.MakeSureIdIsInPlace();
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

		public string expectedControlType
		{
			get
			{
				return this.m_ExpectedControlType;
			}
			set
			{
				this.m_ExpectedControlType = value;
			}
		}

		public string processors
		{
			get
			{
				return this.m_Processors;
			}
		}

		public string interactions
		{
			get
			{
				return this.m_Interactions;
			}
		}

		public InputActionMap actionMap
		{
			get
			{
				if (!this.isSingletonAction)
				{
					return this.m_ActionMap;
				}
				return null;
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
				if (value == this.m_BindingMask)
				{
					return;
				}
				if (value != null)
				{
					InputBinding value2 = value.Value;
					value2.action = this.name;
					value = new InputBinding?(value2);
				}
				this.m_BindingMask = value;
				InputActionMap orCreateActionMap = this.GetOrCreateActionMap();
				if (orCreateActionMap.m_State != null)
				{
					orCreateActionMap.LazyResolveBindings(true);
				}
			}
		}

		public ReadOnlyArray<InputBinding> bindings
		{
			get
			{
				return this.GetOrCreateActionMap().GetBindingsForSingleAction(this);
			}
		}

		public ReadOnlyArray<InputControl> controls
		{
			get
			{
				InputActionMap orCreateActionMap = this.GetOrCreateActionMap();
				orCreateActionMap.ResolveBindingsIfNecessary();
				return orCreateActionMap.GetControlsForSingleAction(this);
			}
		}

		public InputActionPhase phase
		{
			get
			{
				return this.currentState.phase;
			}
		}

		public bool inProgress
		{
			get
			{
				return this.phase.IsInProgress();
			}
		}

		public bool enabled
		{
			get
			{
				return this.phase > InputActionPhase.Disabled;
			}
		}

		public event Action<InputAction.CallbackContext> started
		{
			add
			{
				this.m_OnStarted.AddCallback(value);
			}
			remove
			{
				this.m_OnStarted.RemoveCallback(value);
			}
		}

		public event Action<InputAction.CallbackContext> canceled
		{
			add
			{
				this.m_OnCanceled.AddCallback(value);
			}
			remove
			{
				this.m_OnCanceled.RemoveCallback(value);
			}
		}

		public event Action<InputAction.CallbackContext> performed
		{
			add
			{
				this.m_OnPerformed.AddCallback(value);
			}
			remove
			{
				this.m_OnPerformed.RemoveCallback(value);
			}
		}

		public bool triggered
		{
			get
			{
				return this.WasPerformedThisFrame();
			}
		}

		public unsafe InputControl activeControl
		{
			get
			{
				InputActionState state = this.GetOrCreateActionMap().m_State;
				if (state != null)
				{
					int controlIndex = state.actionStates[this.m_ActionIndexInState].controlIndex;
					if (controlIndex != -1)
					{
						return state.controls[controlIndex];
					}
				}
				return null;
			}
		}

		public unsafe Type activeValueType
		{
			get
			{
				InputActionState state = this.GetOrCreateActionMap().m_State;
				if (state != null)
				{
					InputActionState.TriggerState* ptr = state.actionStates + this.m_ActionIndexInState;
					int controlIndex = ptr->controlIndex;
					if (controlIndex != -1)
					{
						return state.GetValueType(ptr->bindingIndex, controlIndex);
					}
				}
				return null;
			}
		}

		public bool wantsInitialStateCheck
		{
			get
			{
				return this.type == InputActionType.Value || (this.m_Flags & InputAction.ActionFlags.WantsInitialStateCheck) > (InputAction.ActionFlags)0;
			}
			set
			{
				if (value)
				{
					this.m_Flags |= InputAction.ActionFlags.WantsInitialStateCheck;
					return;
				}
				this.m_Flags &= ~InputAction.ActionFlags.WantsInitialStateCheck;
			}
		}

		public InputAction()
		{
			this.m_Id = Guid.NewGuid().ToString();
		}

		public InputAction(string name = null, InputActionType type = InputActionType.Value, string binding = null, string interactions = null, string processors = null, string expectedControlType = null)
		{
			this.m_Name = name;
			this.m_Type = type;
			if (!string.IsNullOrEmpty(binding))
			{
				this.m_SingletonActionBindings = new InputBinding[]
				{
					new InputBinding
					{
						path = binding,
						interactions = interactions,
						processors = processors,
						action = this.m_Name,
						id = Guid.NewGuid()
					}
				};
				this.m_BindingsStartIndex = 0;
				this.m_BindingsCount = 1;
			}
			else
			{
				this.m_Interactions = interactions;
				this.m_Processors = processors;
			}
			this.m_ExpectedControlType = expectedControlType;
			this.m_Id = Guid.NewGuid().ToString();
		}

		public void Dispose()
		{
			InputActionMap actionMap = this.m_ActionMap;
			if (actionMap == null)
			{
				return;
			}
			InputActionState state = actionMap.m_State;
			if (state == null)
			{
				return;
			}
			state.Dispose();
		}

		public override string ToString()
		{
			string text;
			if (this.m_Name == null)
			{
				text = "<Unnamed>";
			}
			else if (this.m_ActionMap != null && !this.isSingletonAction && !string.IsNullOrEmpty(this.m_ActionMap.name))
			{
				text = this.m_ActionMap.name + "/" + this.m_Name;
			}
			else
			{
				text = this.m_Name;
			}
			ReadOnlyArray<InputControl> controls = this.controls;
			if (controls.Count > 0)
			{
				text += "[";
				bool flag = true;
				foreach (InputControl inputControl in controls)
				{
					if (!flag)
					{
						text += ",";
					}
					text += inputControl.path;
					flag = false;
				}
				text += "]";
			}
			return text;
		}

		public void Enable()
		{
			using (InputAction.k_InputActionEnableProfilerMarker.Auto())
			{
				if (!this.enabled)
				{
					InputActionMap orCreateActionMap = this.GetOrCreateActionMap();
					orCreateActionMap.ResolveBindingsIfNecessary();
					orCreateActionMap.m_State.EnableSingleAction(this);
				}
			}
		}

		public void Disable()
		{
			using (InputAction.k_InputActionDisableProfilerMarker.Auto())
			{
				if (this.enabled)
				{
					this.m_ActionMap.m_State.DisableSingleAction(this);
				}
			}
		}

		public InputAction Clone()
		{
			return new InputAction(this.m_Name, this.m_Type, null, null, null, null)
			{
				m_SingletonActionBindings = this.bindings.ToArray(),
				m_BindingsCount = this.m_BindingsCount,
				m_ExpectedControlType = this.m_ExpectedControlType,
				m_Interactions = this.m_Interactions,
				m_Processors = this.m_Processors,
				m_Flags = this.m_Flags
			};
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public unsafe TValue ReadValue<TValue>() where TValue : struct
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state == null)
			{
				return default(TValue);
			}
			InputActionState.TriggerState* ptr = state.actionStates + this.m_ActionIndexInState;
			if (!ptr->phase.IsInProgress())
			{
				return state.ApplyProcessors<TValue>(ptr->bindingIndex, default(TValue), null);
			}
			return state.ReadValue<TValue>(ptr->bindingIndex, ptr->controlIndex, false);
		}

		public unsafe object ReadValueAsObject()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state == null)
			{
				return null;
			}
			InputActionState.TriggerState* ptr = state.actionStates + this.m_ActionIndexInState;
			if (ptr->phase.IsInProgress())
			{
				int controlIndex = ptr->controlIndex;
				if (controlIndex != -1)
				{
					return state.ReadValueAsObject(ptr->bindingIndex, controlIndex, false);
				}
			}
			return null;
		}

		public unsafe float GetControlMagnitude()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state != null)
			{
				InputActionState.TriggerState* ptr = state.actionStates + this.m_ActionIndexInState;
				if (ptr->haveMagnitude)
				{
					return ptr->magnitude;
				}
			}
			return 0f;
		}

		public void Reset()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state == null)
			{
				return;
			}
			state.ResetActionState(this.m_ActionIndexInState, this.enabled ? InputActionPhase.Waiting : InputActionPhase.Disabled, true);
		}

		public unsafe bool IsPressed()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].isPressed;
		}

		public unsafe bool IsInProgress()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].phase.IsInProgress();
		}

		private int ExpectedFrame()
		{
			int num = (InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsManually) ? 1 : 0;
			return Time.frameCount - num;
		}

		public unsafe bool WasPressedThisFrame()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state != null)
			{
				ref InputActionState.TriggerState ptr = ref state.actionStates[this.m_ActionIndexInState];
				uint s_UpdateStepCount = InputUpdate.s_UpdateStepCount;
				return ptr.pressedInUpdate == s_UpdateStepCount && s_UpdateStepCount > 0U;
			}
			return false;
		}

		public unsafe bool WasPressedThisDynamicUpdate()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].framePressed == this.ExpectedFrame();
		}

		public unsafe bool WasReleasedThisFrame()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state != null)
			{
				ref InputActionState.TriggerState ptr = ref state.actionStates[this.m_ActionIndexInState];
				uint s_UpdateStepCount = InputUpdate.s_UpdateStepCount;
				return ptr.releasedInUpdate == s_UpdateStepCount && s_UpdateStepCount > 0U;
			}
			return false;
		}

		public unsafe bool WasReleasedThisDynamicUpdate()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].frameReleased == this.ExpectedFrame();
		}

		public unsafe bool WasPerformedThisFrame()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state != null)
			{
				ref InputActionState.TriggerState ptr = ref state.actionStates[this.m_ActionIndexInState];
				uint s_UpdateStepCount = InputUpdate.s_UpdateStepCount;
				return ptr.lastPerformedInUpdate == s_UpdateStepCount && s_UpdateStepCount > 0U;
			}
			return false;
		}

		public unsafe bool WasPerformedThisDynamicUpdate()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].framePerformed == this.ExpectedFrame();
		}

		public unsafe bool WasCompletedThisFrame()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state != null)
			{
				ref InputActionState.TriggerState ptr = ref state.actionStates[this.m_ActionIndexInState];
				uint s_UpdateStepCount = InputUpdate.s_UpdateStepCount;
				return ptr.lastCompletedInUpdate == s_UpdateStepCount && s_UpdateStepCount > 0U;
			}
			return false;
		}

		public unsafe bool WasCompletedThisDynamicUpdate()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			return state != null && state.actionStates[this.m_ActionIndexInState].frameCompleted == this.ExpectedFrame();
		}

		public unsafe float GetTimeoutCompletionPercentage()
		{
			InputActionState state = this.GetOrCreateActionMap().m_State;
			if (state == null)
			{
				return 0f;
			}
			ref InputActionState.TriggerState ptr = ref state.actionStates[this.m_ActionIndexInState];
			int interactionIndex = ptr.interactionIndex;
			if (interactionIndex == -1)
			{
				return (float)((ptr.phase == InputActionPhase.Performed) ? 1 : 0);
			}
			ref InputActionState.InteractionState ptr2 = ref state.interactionStates[interactionIndex];
			InputActionPhase phase = ptr2.phase;
			if (phase != InputActionPhase.Started)
			{
				if (phase != InputActionPhase.Performed)
				{
					return 0f;
				}
				return 1f;
			}
			else
			{
				float num = 0f;
				if (ptr2.isTimerRunning)
				{
					float timerDuration = ptr2.timerDuration;
					double num2 = ptr2.timerStartTime + (double)timerDuration - InputState.currentTime;
					if (num2 <= 0.0)
					{
						num = 1f;
					}
					else
					{
						num = (float)(((double)timerDuration - num2) / (double)timerDuration);
					}
				}
				if (ptr2.totalTimeoutCompletionTimeRemaining > 0f)
				{
					return (ptr2.totalTimeoutCompletionDone + num * ptr2.timerDuration) / (ptr2.totalTimeoutCompletionDone + ptr2.totalTimeoutCompletionTimeRemaining);
				}
				return num;
			}
		}

		internal bool isSingletonAction
		{
			get
			{
				return this.m_ActionMap == null || this.m_ActionMap.m_SingletonAction == this;
			}
		}

		private unsafe InputActionState.TriggerState currentState
		{
			get
			{
				if (this.m_ActionIndexInState == -1)
				{
					return default(InputActionState.TriggerState);
				}
				return *this.m_ActionMap.m_State.FetchActionState(this);
			}
		}

		internal string MakeSureIdIsInPlace()
		{
			if (string.IsNullOrEmpty(this.m_Id))
			{
				this.GenerateId();
			}
			return this.m_Id;
		}

		internal void GenerateId()
		{
			this.m_Id = Guid.NewGuid().ToString();
		}

		internal InputActionMap GetOrCreateActionMap()
		{
			if (this.m_ActionMap == null)
			{
				this.CreateInternalActionMapForSingletonAction();
			}
			return this.m_ActionMap;
		}

		private void CreateInternalActionMapForSingletonAction()
		{
			this.m_ActionMap = new InputActionMap
			{
				m_Actions = new InputAction[]
				{
					this
				},
				m_SingletonAction = this,
				m_Bindings = this.m_SingletonActionBindings
			};
		}

		internal void RequestInitialStateCheckOnEnabledAction()
		{
			this.GetOrCreateActionMap().m_State.SetInitialStateCheckPending(this.m_ActionIndexInState, true);
		}

		internal bool ActiveControlIsValid(InputControl control)
		{
			if (control == null)
			{
				return false;
			}
			InputDevice device = control.device;
			if (!device.added)
			{
				return false;
			}
			ReadOnlyArray<InputDevice>? devices = this.GetOrCreateActionMap().devices;
			return devices == null || devices.Value.ContainsReference(device);
		}

		internal InputBinding? FindEffectiveBindingMask()
		{
			if (this.m_BindingMask != null)
			{
				return this.m_BindingMask;
			}
			InputActionMap actionMap = this.m_ActionMap;
			if (actionMap != null && actionMap.m_BindingMask != null)
			{
				return this.m_ActionMap.m_BindingMask;
			}
			InputActionMap actionMap2 = this.m_ActionMap;
			if (actionMap2 == null)
			{
				return null;
			}
			InputActionAsset asset = actionMap2.m_Asset;
			if (asset == null)
			{
				return null;
			}
			return asset.m_BindingMask;
		}

		internal int BindingIndexOnActionToBindingIndexOnMap(int indexOfBindingOnAction)
		{
			InputBinding[] bindings = this.GetOrCreateActionMap().m_Bindings;
			int num = bindings.LengthSafe<InputBinding>();
			string name = this.name;
			int num2 = -1;
			for (int i = 0; i < num; i++)
			{
				if (bindings[i].TriggersAction(this))
				{
					num2++;
					if (num2 == indexOfBindingOnAction)
					{
						return i;
					}
				}
			}
			throw new ArgumentOutOfRangeException("indexOfBindingOnAction", string.Format("Binding index {0} is out of range for action '{1}' with {2} bindings", indexOfBindingOnAction, this, num2 + 1));
		}

		internal int BindingIndexOnMapToBindingIndexOnAction(int indexOfBindingOnMap)
		{
			InputBinding[] bindings = this.GetOrCreateActionMap().m_Bindings;
			string name = this.name;
			int num = 0;
			for (int i = indexOfBindingOnMap - 1; i >= 0; i--)
			{
				ref InputBinding ptr = ref bindings[i];
				if (string.Compare(ptr.action, name, StringComparison.InvariantCultureIgnoreCase) == 0 || ptr.action == this.m_Id)
				{
					num++;
				}
			}
			return num;
		}

		private static readonly ProfilerMarker k_InputActionEnableProfilerMarker = new ProfilerMarker("InputAction.Enable");

		private static readonly ProfilerMarker k_InputActionDisableProfilerMarker = new ProfilerMarker("InputAction.Disable");

		[Tooltip("Human readable name of the action. Must be unique within its action map (case is ignored). Can be changed without breaking references to the action.")]
		[SerializeField]
		internal string m_Name;

		[Tooltip("Determines how the action triggers.\n\nA Value action will start and perform when a control moves from its default value and then perform on every value change. It will cancel when controls go back to default value. Also, when enabled, a Value action will respond right away to a control's current value.\n\nA Button action will start when a button is pressed and perform when the press threshold (see 'Default Button Press Point' in settings) is reached. It will cancel when the button is going below the release threshold (see 'Button Release Threshold' in settings). Also, if a button is already pressed when the action is enabled, the button has to be released first.\n\nA Pass-Through action will not explicitly start and will never cancel. Instead, for every value change on any bound control, the action will perform.")]
		[SerializeField]
		internal InputActionType m_Type;

		[FormerlySerializedAs("m_ExpectedControlLayout")]
		[Tooltip("The type of control expected by the action (e.g. \"Button\" or \"Stick\"). This will limit the controls shown when setting up bindings in the UI and will also limit which controls can be bound interactively to the action.")]
		[SerializeField]
		internal string m_ExpectedControlType;

		[Tooltip("Unique ID of the action (GUID). Used to reference the action from bindings such that actions can be renamed without breaking references.")]
		[SerializeField]
		internal string m_Id;

		[SerializeField]
		internal string m_Processors;

		[SerializeField]
		internal string m_Interactions;

		[SerializeField]
		internal InputBinding[] m_SingletonActionBindings;

		[SerializeField]
		internal InputAction.ActionFlags m_Flags;

		[NonSerialized]
		internal InputBinding? m_BindingMask;

		[NonSerialized]
		internal int m_BindingsStartIndex;

		[NonSerialized]
		internal int m_BindingsCount;

		[NonSerialized]
		internal int m_ControlStartIndex;

		[NonSerialized]
		internal int m_ControlCount;

		[NonSerialized]
		internal int m_ActionIndexInState = -1;

		[NonSerialized]
		internal InputActionMap m_ActionMap;

		[NonSerialized]
		internal CallbackArray<Action<InputAction.CallbackContext>> m_OnStarted;

		[NonSerialized]
		internal CallbackArray<Action<InputAction.CallbackContext>> m_OnCanceled;

		[NonSerialized]
		internal CallbackArray<Action<InputAction.CallbackContext>> m_OnPerformed;

		[Flags]
		internal enum ActionFlags
		{
			WantsInitialStateCheck = 1
		}

		public struct CallbackContext
		{
			private int actionIndex
			{
				get
				{
					return this.m_ActionIndex;
				}
			}

			private unsafe int bindingIndex
			{
				get
				{
					return this.m_State.actionStates[this.actionIndex].bindingIndex;
				}
			}

			private unsafe int controlIndex
			{
				get
				{
					return this.m_State.actionStates[this.actionIndex].controlIndex;
				}
			}

			private unsafe int interactionIndex
			{
				get
				{
					return this.m_State.actionStates[this.actionIndex].interactionIndex;
				}
			}

			public unsafe InputActionPhase phase
			{
				get
				{
					if (this.m_State == null)
					{
						return InputActionPhase.Disabled;
					}
					return this.m_State.actionStates[this.actionIndex].phase;
				}
			}

			public bool started
			{
				get
				{
					return this.phase == InputActionPhase.Started;
				}
			}

			public bool performed
			{
				get
				{
					return this.phase == InputActionPhase.Performed;
				}
			}

			public bool canceled
			{
				get
				{
					return this.phase == InputActionPhase.Canceled;
				}
			}

			public InputAction action
			{
				get
				{
					InputActionState state = this.m_State;
					if (state == null)
					{
						return null;
					}
					return state.GetActionOrNull(this.bindingIndex);
				}
			}

			public InputControl control
			{
				get
				{
					InputActionState state = this.m_State;
					if (state == null)
					{
						return null;
					}
					return state.controls[this.controlIndex];
				}
			}

			public IInputInteraction interaction
			{
				get
				{
					if (this.m_State == null)
					{
						return null;
					}
					int interactionIndex = this.interactionIndex;
					if (interactionIndex == -1)
					{
						return null;
					}
					return this.m_State.interactions[interactionIndex];
				}
			}

			public unsafe double time
			{
				get
				{
					if (this.m_State == null)
					{
						return 0.0;
					}
					return this.m_State.actionStates[this.actionIndex].time;
				}
			}

			public unsafe double startTime
			{
				get
				{
					if (this.m_State == null)
					{
						return 0.0;
					}
					return this.m_State.actionStates[this.actionIndex].startTime;
				}
			}

			public double duration
			{
				get
				{
					return this.time - this.startTime;
				}
			}

			public Type valueType
			{
				get
				{
					InputActionState state = this.m_State;
					if (state == null)
					{
						return null;
					}
					return state.GetValueType(this.bindingIndex, this.controlIndex);
				}
			}

			public int valueSizeInBytes
			{
				get
				{
					if (this.m_State == null)
					{
						return 0;
					}
					return this.m_State.GetValueSizeInBytes(this.bindingIndex, this.controlIndex);
				}
			}

			public unsafe void ReadValue(void* buffer, int bufferSize)
			{
				if (buffer == null)
				{
					throw new ArgumentNullException("buffer");
				}
				if (this.m_State != null && this.phase.IsInProgress())
				{
					this.m_State.ReadValue(this.bindingIndex, this.controlIndex, buffer, bufferSize, false);
					return;
				}
				int valueSizeInBytes = this.valueSizeInBytes;
				if (bufferSize < valueSizeInBytes)
				{
					throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer of only {1} bytes", valueSizeInBytes, bufferSize), "bufferSize");
				}
				UnsafeUtility.MemClear(buffer, (long)this.valueSizeInBytes);
			}

			public TValue ReadValue<TValue>() where TValue : struct
			{
				TValue tvalue = default(TValue);
				if (this.m_State != null)
				{
					tvalue = (this.phase.IsInProgress() ? this.m_State.ReadValue<TValue>(this.bindingIndex, this.controlIndex, false) : this.m_State.ApplyProcessors<TValue>(this.bindingIndex, tvalue, null));
				}
				return tvalue;
			}

			public bool ReadValueAsButton()
			{
				bool result = false;
				if (this.m_State != null && this.phase.IsInProgress())
				{
					result = this.m_State.ReadValueAsButton(this.bindingIndex, this.controlIndex);
				}
				return result;
			}

			public object ReadValueAsObject()
			{
				if (this.m_State != null && this.phase.IsInProgress())
				{
					return this.m_State.ReadValueAsObject(this.bindingIndex, this.controlIndex, false);
				}
				return null;
			}

			public override string ToString()
			{
				return string.Format("{{ action={0} phase={1} time={2} control={3} value={4} interaction={5} }}", new object[]
				{
					this.action,
					this.phase,
					this.time,
					this.control,
					this.ReadValueAsObject(),
					this.interaction
				});
			}

			internal InputActionState m_State;

			internal int m_ActionIndex;
		}
	}
}
