using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(TouchscreenState), isGenericTypeOfDevice = true)]
	public class Touchscreen : Pointer, IInputStateCallbackReceiver, IEventMerger, ICustomDeviceReset
	{
		public TouchControl primaryTouch { get; protected set; }

		public ReadOnlyArray<TouchControl> touches { get; protected set; }

		protected TouchControl[] touchControlArray
		{
			get
			{
				return this.touches.m_Array;
			}
			set
			{
				this.touches = new ReadOnlyArray<TouchControl>(value);
			}
		}

		public new static Touchscreen current { get; internal set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Touchscreen.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Touchscreen.current == this)
			{
				Touchscreen.current = null;
			}
		}

		protected override void FinishSetup()
		{
			base.FinishSetup();
			this.primaryTouch = base.GetChildControl<TouchControl>("primaryTouch");
			int num = 0;
			using (ReadOnlyArray<InputControl>.Enumerator enumerator = base.children.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is TouchControl)
					{
						num++;
					}
				}
			}
			if (num >= 1)
			{
				num--;
			}
			TouchControl[] array = new TouchControl[num];
			int num2 = 0;
			foreach (InputControl inputControl in base.children)
			{
				if (inputControl != this.primaryTouch)
				{
					TouchControl touchControl = inputControl as TouchControl;
					if (touchControl != null)
					{
						array[num2++] = touchControl;
					}
				}
			}
			this.touches = new ReadOnlyArray<TouchControl>(array);
		}

		protected new unsafe void OnNextUpdate()
		{
			void* currentStatePtr = base.currentStatePtr;
			TouchState* ptr = (TouchState*)((byte*)((byte*)currentStatePtr + base.stateBlock.byteOffset) + 56);
			int i = 0;
			while (i < this.touches.Count)
			{
				if (ptr->delta != default(Vector2))
				{
					InputState.Change<Vector2>(this.touches[i].delta, Vector2.zero, InputUpdateType.None, default(InputEventPtr));
				}
				if (ptr->tapCount > 0 && InputState.currentTime >= ptr->startTime + (double)Touchscreen.s_TapTime + (double)Touchscreen.s_TapDelayTime)
				{
					InputState.Change<byte>(this.touches[i].tapCount, 0, InputUpdateType.None, default(InputEventPtr));
				}
				i++;
				ptr++;
			}
			TouchState* ptr2 = (TouchState*)((byte*)currentStatePtr + base.stateBlock.byteOffset);
			if (ptr2->delta != default(Vector2))
			{
				InputState.Change<Vector2>(this.primaryTouch.delta, Vector2.zero, InputUpdateType.None, default(InputEventPtr));
			}
			if (ptr2->tapCount > 0 && InputState.currentTime >= ptr2->startTime + (double)Touchscreen.s_TapTime + (double)Touchscreen.s_TapDelayTime)
			{
				InputState.Change<byte>(this.primaryTouch.tapCount, 0, InputUpdateType.None, default(InputEventPtr));
			}
		}

		protected new unsafe void OnStateEvent(InputEventPtr eventPtr)
		{
			if (eventPtr.type == 1145852993)
			{
				return;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
			if (ptr->stateFormat != TouchState.Format)
			{
				InputState.Change(this, eventPtr, InputUpdateType.None);
				return;
			}
			void* currentStatePtr = base.currentStatePtr;
			TouchState* ptr2 = (TouchState*)((byte*)currentStatePtr + this.touches[0].stateBlock.byteOffset);
			TouchState* ptr3 = (TouchState*)((byte*)currentStatePtr + this.primaryTouch.stateBlock.byteOffset);
			int count = this.touches.Count;
			TouchState touchState;
			if (ptr->stateSizeInBytes == 56U)
			{
				touchState = *(TouchState*)ptr->state;
			}
			else
			{
				touchState = default(TouchState);
				UnsafeUtility.MemCpy(UnsafeUtility.AddressOf<TouchState>(ref touchState), ptr->state, (long)((ulong)ptr->stateSizeInBytes));
			}
			touchState.tapCount = 0;
			touchState.isTapPress = false;
			touchState.isTapRelease = false;
			touchState.updateStepCount = InputUpdate.s_UpdateStepCount;
			if (touchState.phase != TouchPhase.Began)
			{
				int touchId = touchState.touchId;
				int i = 0;
				while (i < count)
				{
					if (ptr2[i].touchId == touchId)
					{
						bool isPrimaryTouch = ptr2[i].isPrimaryTouch;
						touchState.isPrimaryTouch = isPrimaryTouch;
						if (touchState.delta == default(Vector2))
						{
							touchState.delta = touchState.position - ptr2[i].position;
						}
						touchState.delta += ptr2[i].delta;
						touchState.startTime = ptr2[i].startTime;
						touchState.startPosition = ptr2[i].startPosition;
						bool flag = touchState.isNoneEndedOrCanceled && eventPtr.time - touchState.startTime <= (double)Touchscreen.s_TapTime && (touchState.position - touchState.startPosition).sqrMagnitude <= Touchscreen.s_TapRadiusSquared;
						if (flag)
						{
							touchState.tapCount = ptr2[i].tapCount + 1;
						}
						else
						{
							touchState.tapCount = ptr2[i].tapCount;
						}
						if (isPrimaryTouch)
						{
							if (touchState.isNoneEndedOrCanceled)
							{
								touchState.isPrimaryTouch = false;
								bool flag2 = false;
								for (int j = 0; j < count; j++)
								{
									if (j != i && ptr2[j].isInProgress)
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									if (flag)
									{
										Touchscreen.TriggerTap(this.primaryTouch, ref touchState, eventPtr);
									}
									else
									{
										InputState.Change<TouchState>(this.primaryTouch, ref touchState, InputUpdateType.None, eventPtr);
									}
								}
								else
								{
									TouchState touchState2 = touchState;
									touchState2.phase = TouchPhase.Moved;
									touchState2.isOrphanedPrimaryTouch = true;
									InputState.Change<TouchState>(this.primaryTouch, ref touchState2, InputUpdateType.None, eventPtr);
								}
							}
							else
							{
								InputState.Change<TouchState>(this.primaryTouch, ref touchState, InputUpdateType.None, eventPtr);
							}
						}
						else if (touchState.isNoneEndedOrCanceled && ptr3->isOrphanedPrimaryTouch)
						{
							bool flag3 = false;
							for (int k = 0; k < count; k++)
							{
								if (k != i && ptr2[k].isInProgress)
								{
									flag3 = true;
									break;
								}
							}
							if (!flag3)
							{
								ptr3->isOrphanedPrimaryTouch = false;
								InputState.Change<byte>(this.primaryTouch.phase, 3, InputUpdateType.None, default(InputEventPtr));
							}
						}
						if (flag)
						{
							Touchscreen.TriggerTap(this.touches[i], ref touchState, eventPtr);
							return;
						}
						InputState.Change<TouchState>(this.touches[i], ref touchState, InputUpdateType.None, eventPtr);
						return;
					}
					else
					{
						i++;
					}
				}
				return;
			}
			int l = 0;
			while (l < count)
			{
				if (ptr2->isNoneEndedOrCanceled)
				{
					touchState.delta = Vector2.zero;
					touchState.startTime = eventPtr.time;
					touchState.startPosition = touchState.position;
					touchState.isPrimaryTouch = false;
					touchState.isOrphanedPrimaryTouch = false;
					touchState.isTap = false;
					touchState.tapCount = ptr2->tapCount;
					if (ptr3->isNoneEndedOrCanceled)
					{
						touchState.isPrimaryTouch = true;
						InputState.Change<TouchState>(this.primaryTouch, ref touchState, InputUpdateType.None, eventPtr);
					}
					InputState.Change<TouchState>(this.touches[l], ref touchState, InputUpdateType.None, eventPtr);
					return;
				}
				l++;
				ptr2++;
			}
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			this.OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			this.OnStateEvent(eventPtr);
		}

		unsafe bool IInputStateCallbackReceiver.GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
		{
			if (!eventPtr.IsA<StateEvent>())
			{
				return false;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(eventPtr);
			if (ptr->stateFormat != TouchState.Format)
			{
				return false;
			}
			if (control == null)
			{
				TouchState* ptr2 = (TouchState*)((byte*)base.currentStatePtr + this.touches[0].stateBlock.byteOffset);
				TouchState* state = (TouchState*)ptr->state;
				int touchId = state->touchId;
				TouchPhase phase = state->phase;
				int count = this.touches.Count;
				for (int i = 0; i < count; i++)
				{
					TouchState* ptr3 = ptr2 + i;
					if (ptr3->touchId == touchId || (!ptr3->isInProgress && phase.IsActive()))
					{
						offset = this.primaryTouch.m_StateBlock.byteOffset + this.primaryTouch.m_StateBlock.alignedSizeInBytes - this.m_StateBlock.byteOffset + (uint)(i * UnsafeUtility.SizeOf<TouchState>());
						return true;
					}
				}
				return false;
			}
			TouchControl touchControl = control.FindInParentChain<TouchControl>();
			if (touchControl == null || touchControl.parent != this)
			{
				return false;
			}
			if (touchControl != this.primaryTouch)
			{
				return false;
			}
			offset = touchControl.stateBlock.byteOffset - this.m_StateBlock.byteOffset;
			return true;
		}

		unsafe void ICustomDeviceReset.Reset()
		{
			void* currentStatePtr = base.currentStatePtr;
			using (NativeArray<byte> nativeArray = new NativeArray<byte>(StateEvent.GetEventSizeWithPayload<TouchState>(), Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				StateEvent* unsafePtr = (StateEvent*)nativeArray.GetUnsafePtr<byte>();
				unsafePtr->baseEvent = new InputEvent(1398030676, nativeArray.Length, base.deviceId, -1.0);
				TouchState* ptr = (TouchState*)((byte*)currentStatePtr + this.primaryTouch.stateBlock.byteOffset);
				if (ptr->phase.IsActive())
				{
					UnsafeUtility.MemCpy(unsafePtr->state, (void*)ptr, (long)UnsafeUtility.SizeOf<TouchState>());
					((TouchState*)unsafePtr->state)->phase = TouchPhase.Canceled;
					InputState.Change<TouchPhase>(this.primaryTouch.phase, TouchPhase.Canceled, InputUpdateType.None, new InputEventPtr((InputEvent*)unsafePtr));
				}
				TouchState* ptr2 = (TouchState*)((byte*)currentStatePtr + this.touches[0].stateBlock.byteOffset);
				int count = this.touches.Count;
				for (int i = 0; i < count; i++)
				{
					if (ptr2[i].phase.IsActive())
					{
						UnsafeUtility.MemCpy(unsafePtr->state, (void*)(ptr2 + i), (long)UnsafeUtility.SizeOf<TouchState>());
						((TouchState*)unsafePtr->state)->phase = TouchPhase.Canceled;
						InputState.Change<TouchPhase>(this.touches[i].phase, TouchPhase.Canceled, InputUpdateType.None, new InputEventPtr((InputEvent*)unsafePtr));
					}
				}
			}
		}

		internal unsafe static bool MergeForward(InputEventPtr currentEventPtr, InputEventPtr nextEventPtr)
		{
			if (currentEventPtr.type != 1398030676 || nextEventPtr.type != 1398030676)
			{
				return false;
			}
			StateEvent* ptr = StateEvent.FromUnchecked(currentEventPtr);
			StateEvent* ptr2 = StateEvent.FromUnchecked(nextEventPtr);
			if (ptr->stateFormat != TouchState.Format || ptr2->stateFormat != TouchState.Format)
			{
				return false;
			}
			TouchState* state = (TouchState*)ptr->state;
			TouchState* state2 = (TouchState*)ptr2->state;
			if (state->touchId != state2->touchId || state->phaseId != state2->phaseId || state->flags != state2->flags)
			{
				return false;
			}
			state2->delta += state->delta;
			return true;
		}

		bool IEventMerger.MergeForward(InputEventPtr currentEventPtr, InputEventPtr nextEventPtr)
		{
			return Touchscreen.MergeForward(currentEventPtr, nextEventPtr);
		}

		private static void TriggerTap(TouchControl control, ref TouchState state, InputEventPtr eventPtr)
		{
			state.isTapPress = true;
			state.isTapRelease = false;
			InputState.Change<TouchState>(control, ref state, InputUpdateType.None, eventPtr);
			state.isTapPress = false;
			state.isTapRelease = true;
			InputState.Change<TouchState>(control, ref state, InputUpdateType.None, eventPtr);
			state.isTapRelease = false;
		}

		private static readonly ProfilerMarker k_TouchscreenUpdateMarker = new ProfilerMarker("Touchscreen.OnNextUpdate");

		private static readonly ProfilerMarker k_TouchAllocateMarker = new ProfilerMarker("TouchAllocate");

		internal static float s_TapTime;

		internal static float s_TapDelayTime;

		internal static float s_TapRadiusSquared;
	}
}
