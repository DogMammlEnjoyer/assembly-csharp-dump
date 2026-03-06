using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem.Utilities
{
	public sealed class InputActionTrace : IEnumerable<InputActionTrace.ActionEventPtr>, IEnumerable, IDisposable
	{
		public InputEventBuffer buffer
		{
			get
			{
				return this.m_EventBuffer;
			}
		}

		public int count
		{
			get
			{
				return this.m_EventBuffer.eventCount;
			}
		}

		public InputActionTrace()
		{
		}

		public InputActionTrace(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			this.SubscribeTo(action);
		}

		public InputActionTrace(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			this.SubscribeTo(actionMap);
		}

		public void SubscribeToAll()
		{
			if (this.m_SubscribedToAll)
			{
				return;
			}
			this.HookOnActionChange();
			this.m_SubscribedToAll = true;
			while (this.m_SubscribedActions.length > 0)
			{
				this.UnsubscribeFrom(this.m_SubscribedActions[this.m_SubscribedActions.length - 1]);
			}
			while (this.m_SubscribedActionMaps.length > 0)
			{
				this.UnsubscribeFrom(this.m_SubscribedActionMaps[this.m_SubscribedActionMaps.length - 1]);
			}
		}

		public void UnsubscribeFromAll()
		{
			if (this.count == 0)
			{
				this.UnhookOnActionChange();
			}
			this.m_SubscribedToAll = false;
			while (this.m_SubscribedActions.length > 0)
			{
				this.UnsubscribeFrom(this.m_SubscribedActions[this.m_SubscribedActions.length - 1]);
			}
			while (this.m_SubscribedActionMaps.length > 0)
			{
				this.UnsubscribeFrom(this.m_SubscribedActionMaps[this.m_SubscribedActionMaps.length - 1]);
			}
		}

		public void SubscribeTo(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (this.m_CallbackDelegate == null)
			{
				this.m_CallbackDelegate = new Action<InputAction.CallbackContext>(this.RecordAction);
			}
			action.performed += this.m_CallbackDelegate;
			action.started += this.m_CallbackDelegate;
			action.canceled += this.m_CallbackDelegate;
			this.m_SubscribedActions.AppendWithCapacity(action, 10);
		}

		public void SubscribeTo(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (this.m_CallbackDelegate == null)
			{
				this.m_CallbackDelegate = new Action<InputAction.CallbackContext>(this.RecordAction);
			}
			actionMap.actionTriggered += this.m_CallbackDelegate;
			this.m_SubscribedActionMaps.AppendWithCapacity(actionMap, 10);
		}

		public void UnsubscribeFrom(InputAction action)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			if (this.m_CallbackDelegate == null)
			{
				return;
			}
			action.performed -= this.m_CallbackDelegate;
			action.started -= this.m_CallbackDelegate;
			action.canceled -= this.m_CallbackDelegate;
			int num = this.m_SubscribedActions.IndexOfReference(action);
			if (num != -1)
			{
				this.m_SubscribedActions.RemoveAtWithCapacity(num);
			}
		}

		public void UnsubscribeFrom(InputActionMap actionMap)
		{
			if (actionMap == null)
			{
				throw new ArgumentNullException("actionMap");
			}
			if (this.m_CallbackDelegate == null)
			{
				return;
			}
			actionMap.actionTriggered -= this.m_CallbackDelegate;
			int num = this.m_SubscribedActionMaps.IndexOfReference(actionMap);
			if (num != -1)
			{
				this.m_SubscribedActionMaps.RemoveAtWithCapacity(num);
			}
		}

		public unsafe void RecordAction(InputAction.CallbackContext context)
		{
			int num = this.m_ActionMapStates.IndexOfReference(context.m_State);
			if (num == -1)
			{
				num = this.m_ActionMapStates.AppendWithCapacity(context.m_State, 10);
			}
			this.HookOnActionChange();
			int valueSizeInBytes = context.valueSizeInBytes;
			ActionEvent* ptr = (ActionEvent*)this.m_EventBuffer.AllocateEvent(ActionEvent.GetEventSizeWithValueSize(valueSizeInBytes), 2048, Allocator.Persistent);
			ref InputActionState.TriggerState ptr2 = ref context.m_State.actionStates[context.m_ActionIndex];
			ptr->baseEvent.type = ActionEvent.Type;
			ptr->baseEvent.time = ptr2.time;
			ptr->stateIndex = num;
			ptr->controlIndex = ptr2.controlIndex;
			ptr->bindingIndex = ptr2.bindingIndex;
			ptr->interactionIndex = ptr2.interactionIndex;
			ptr->startTime = ptr2.startTime;
			ptr->phase = ptr2.phase;
			byte* valueData = ptr->valueData;
			context.ReadValue((void*)valueData, valueSizeInBytes);
		}

		public void Clear()
		{
			this.m_EventBuffer.Reset();
			this.m_ActionMapStates.ClearWithCapacity();
		}

		~InputActionTrace()
		{
			this.DisposeInternal();
		}

		public override string ToString()
		{
			if (this.count == 0)
			{
				return "[]";
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('[');
			bool flag = true;
			foreach (InputActionTrace.ActionEventPtr actionEventPtr in this)
			{
				if (!flag)
				{
					stringBuilder.Append(",\n");
				}
				stringBuilder.Append(actionEventPtr.ToString());
				flag = false;
			}
			stringBuilder.Append(']');
			return stringBuilder.ToString();
		}

		public void Dispose()
		{
			this.UnsubscribeFromAll();
			this.DisposeInternal();
		}

		private void DisposeInternal()
		{
			for (int i = 0; i < this.m_ActionMapStateClones.length; i++)
			{
				this.m_ActionMapStateClones[i].Dispose();
			}
			this.m_EventBuffer.Dispose();
			this.m_ActionMapStates.Clear();
			this.m_ActionMapStateClones.Clear();
			if (this.m_ActionChangeDelegate != null)
			{
				InputSystem.onActionChange -= this.m_ActionChangeDelegate;
				this.m_ActionChangeDelegate = null;
			}
		}

		public IEnumerator<InputActionTrace.ActionEventPtr> GetEnumerator()
		{
			return new InputActionTrace.Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void HookOnActionChange()
		{
			if (this.m_OnActionChangeHooked)
			{
				return;
			}
			if (this.m_ActionChangeDelegate == null)
			{
				this.m_ActionChangeDelegate = new Action<object, InputActionChange>(this.OnActionChange);
			}
			InputSystem.onActionChange += this.m_ActionChangeDelegate;
			this.m_OnActionChangeHooked = true;
		}

		private void UnhookOnActionChange()
		{
			if (!this.m_OnActionChangeHooked)
			{
				return;
			}
			InputSystem.onActionChange -= this.m_ActionChangeDelegate;
			this.m_OnActionChangeHooked = false;
		}

		private void OnActionChange(object actionOrMapOrAsset, InputActionChange change)
		{
			if (this.m_SubscribedToAll && change - InputActionChange.ActionStarted <= 2)
			{
				InputAction inputAction = (InputAction)actionOrMapOrAsset;
				int actionIndexInState = inputAction.m_ActionIndexInState;
				InputActionState state = inputAction.m_ActionMap.m_State;
				InputAction.CallbackContext context = new InputAction.CallbackContext
				{
					m_State = state,
					m_ActionIndex = actionIndexInState
				};
				this.RecordAction(context);
				return;
			}
			if (change != InputActionChange.BoundControlsAboutToChange)
			{
				return;
			}
			InputAction inputAction2 = actionOrMapOrAsset as InputAction;
			if (inputAction2 != null)
			{
				this.CloneActionStateBeforeBindingsChange(inputAction2.m_ActionMap);
				return;
			}
			InputActionMap inputActionMap = actionOrMapOrAsset as InputActionMap;
			if (inputActionMap != null)
			{
				this.CloneActionStateBeforeBindingsChange(inputActionMap);
				return;
			}
			InputActionAsset inputActionAsset = actionOrMapOrAsset as InputActionAsset;
			if (inputActionAsset != null)
			{
				foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
				{
					this.CloneActionStateBeforeBindingsChange(actionMap);
				}
			}
		}

		private void CloneActionStateBeforeBindingsChange(InputActionMap actionMap)
		{
			InputActionState state = actionMap.m_State;
			if (state == null)
			{
				return;
			}
			int num = this.m_ActionMapStates.IndexOfReference(state);
			if (num == -1)
			{
				return;
			}
			InputActionState value = state.Clone();
			this.m_ActionMapStateClones.Append(value);
			this.m_ActionMapStates[num] = value;
		}

		private bool m_SubscribedToAll;

		private bool m_OnActionChangeHooked;

		private InlinedArray<InputAction> m_SubscribedActions;

		private InlinedArray<InputActionMap> m_SubscribedActionMaps;

		private InputEventBuffer m_EventBuffer;

		private InlinedArray<InputActionState> m_ActionMapStates;

		private InlinedArray<InputActionState> m_ActionMapStateClones;

		private Action<InputAction.CallbackContext> m_CallbackDelegate;

		private Action<object, InputActionChange> m_ActionChangeDelegate;

		public struct ActionEventPtr
		{
			public unsafe InputAction action
			{
				get
				{
					return this.m_State.GetActionOrNull(this.m_Ptr->bindingIndex);
				}
			}

			public unsafe InputActionPhase phase
			{
				get
				{
					return this.m_Ptr->phase;
				}
			}

			public unsafe InputControl control
			{
				get
				{
					return this.m_State.controls[this.m_Ptr->controlIndex];
				}
			}

			public unsafe IInputInteraction interaction
			{
				get
				{
					int interactionIndex = this.m_Ptr->interactionIndex;
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
					return this.m_Ptr->baseEvent.time;
				}
			}

			public unsafe double startTime
			{
				get
				{
					return this.m_Ptr->startTime;
				}
			}

			public double duration
			{
				get
				{
					return this.time - this.startTime;
				}
			}

			public unsafe int valueSizeInBytes
			{
				get
				{
					return this.m_Ptr->valueSizeInBytes;
				}
			}

			public unsafe object ReadValueAsObject()
			{
				if (this.m_Ptr == null)
				{
					throw new InvalidOperationException("ActionEventPtr is invalid");
				}
				byte* valueData = this.m_Ptr->valueData;
				int bindingIndex = this.m_Ptr->bindingIndex;
				if (!this.m_State.bindingStates[bindingIndex].isPartOfComposite)
				{
					int valueSizeInBytes = this.m_Ptr->valueSizeInBytes;
					return this.control.ReadValueFromBufferAsObject((void*)valueData, valueSizeInBytes);
				}
				int compositeOrCompositeBindingIndex = this.m_State.bindingStates[bindingIndex].compositeOrCompositeBindingIndex;
				int compositeOrCompositeBindingIndex2 = this.m_State.bindingStates[compositeOrCompositeBindingIndex].compositeOrCompositeBindingIndex;
				InputBindingComposite inputBindingComposite = this.m_State.composites[compositeOrCompositeBindingIndex2];
				Type valueType = inputBindingComposite.valueType;
				if (valueType == null)
				{
					throw new InvalidOperationException(string.Format("Cannot read value from Composite '{0}' which does not have a valueType set", inputBindingComposite));
				}
				return Marshal.PtrToStructure(new IntPtr((void*)valueData), valueType);
			}

			public unsafe void ReadValue(void* buffer, int bufferSize)
			{
				int valueSizeInBytes = this.m_Ptr->valueSizeInBytes;
				if (bufferSize < valueSizeInBytes)
				{
					throw new ArgumentException(string.Format("Expected buffer of at least {0} bytes but got buffer of just {1} bytes instead", valueSizeInBytes, bufferSize), "bufferSize");
				}
				UnsafeUtility.MemCpy(buffer, (void*)this.m_Ptr->valueData, (long)valueSizeInBytes);
			}

			public unsafe TValue ReadValue<TValue>() where TValue : struct
			{
				int valueSizeInBytes = this.m_Ptr->valueSizeInBytes;
				if (UnsafeUtility.SizeOf<TValue>() != valueSizeInBytes)
				{
					throw new InvalidOperationException(string.Format("Cannot read a value of type '{0}' with size {1} from event on action '{2}' with value size {3}", new object[]
					{
						typeof(TValue).Name,
						UnsafeUtility.SizeOf<TValue>(),
						this.action,
						valueSizeInBytes
					}));
				}
				TValue result = Activator.CreateInstance<TValue>();
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TValue>(ref result), (void*)this.m_Ptr->valueData, (long)valueSizeInBytes);
				return result;
			}

			public override string ToString()
			{
				if (this.m_Ptr == null)
				{
					return "<null>";
				}
				string text = (this.action.actionMap != null) ? (this.action.actionMap.name + "/" + this.action.name) : this.action.name;
				return string.Format("{{ action={0} phase={1} time={2} control={3} value={4} interaction={5} duration={6} }}", new object[]
				{
					text,
					this.phase,
					this.time,
					this.control,
					this.ReadValueAsObject(),
					this.interaction,
					this.duration
				});
			}

			internal InputActionState m_State;

			internal unsafe ActionEvent* m_Ptr;
		}

		private struct Enumerator : IEnumerator<InputActionTrace.ActionEventPtr>, IEnumerator, IDisposable
		{
			public unsafe Enumerator(InputActionTrace trace)
			{
				this.m_Trace = trace;
				this.m_Buffer = (ActionEvent*)trace.m_EventBuffer.bufferPtr.data;
				this.m_EventCount = trace.m_EventBuffer.eventCount;
				this.m_CurrentEvent = null;
				this.m_CurrentIndex = 0;
			}

			public unsafe bool MoveNext()
			{
				if (this.m_CurrentIndex == this.m_EventCount)
				{
					return false;
				}
				if (this.m_CurrentEvent == null)
				{
					this.m_CurrentEvent = this.m_Buffer;
					return this.m_CurrentEvent != null;
				}
				this.m_CurrentIndex++;
				if (this.m_CurrentIndex == this.m_EventCount)
				{
					return false;
				}
				this.m_CurrentEvent = (ActionEvent*)InputEvent.GetNextInMemory((InputEvent*)this.m_CurrentEvent);
				return true;
			}

			public void Reset()
			{
				this.m_CurrentEvent = null;
				this.m_CurrentIndex = 0;
			}

			public void Dispose()
			{
			}

			public unsafe InputActionTrace.ActionEventPtr Current
			{
				get
				{
					InputActionState state = this.m_Trace.m_ActionMapStates[this.m_CurrentEvent->stateIndex];
					return new InputActionTrace.ActionEventPtr
					{
						m_State = state,
						m_Ptr = this.m_CurrentEvent
					};
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			private readonly InputActionTrace m_Trace;

			private unsafe readonly ActionEvent* m_Buffer;

			private readonly int m_EventCount;

			private unsafe ActionEvent* m_CurrentEvent;

			private int m_CurrentIndex;
		}
	}
}
