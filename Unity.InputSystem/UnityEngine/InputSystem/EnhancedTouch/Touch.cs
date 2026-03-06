using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public struct Touch : IEquatable<Touch>
	{
		public bool valid
		{
			get
			{
				return this.m_TouchRecord.valid;
			}
		}

		public Finger finger
		{
			get
			{
				return this.m_Finger;
			}
		}

		public TouchPhase phase
		{
			get
			{
				return this.state.phase;
			}
		}

		public bool began
		{
			get
			{
				return this.phase == TouchPhase.Began;
			}
		}

		public bool inProgress
		{
			get
			{
				return this.phase == TouchPhase.Moved || this.phase == TouchPhase.Stationary || this.phase == TouchPhase.Began;
			}
		}

		public bool ended
		{
			get
			{
				return this.phase == TouchPhase.Ended || this.phase == TouchPhase.Canceled;
			}
		}

		public int touchId
		{
			get
			{
				return this.state.touchId;
			}
		}

		public float pressure
		{
			get
			{
				return this.state.pressure;
			}
		}

		public Vector2 radius
		{
			get
			{
				return this.state.radius;
			}
		}

		public double startTime
		{
			get
			{
				return this.state.startTime;
			}
		}

		public double time
		{
			get
			{
				return this.m_TouchRecord.time;
			}
		}

		public Touchscreen screen
		{
			get
			{
				return this.finger.screen;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				return this.state.position;
			}
		}

		public Vector2 startScreenPosition
		{
			get
			{
				return this.state.startPosition;
			}
		}

		public Vector2 delta
		{
			get
			{
				return this.state.delta;
			}
		}

		public int tapCount
		{
			get
			{
				return (int)this.state.tapCount;
			}
		}

		public bool isTap
		{
			get
			{
				return this.state.isTap;
			}
		}

		public int displayIndex
		{
			get
			{
				return (int)this.state.displayIndex;
			}
		}

		public bool isInProgress
		{
			get
			{
				TouchPhase phase = this.phase;
				return phase - TouchPhase.Began <= 1 || phase == TouchPhase.Stationary;
			}
		}

		internal uint updateStepCount
		{
			get
			{
				return this.state.updateStepCount;
			}
		}

		internal uint uniqueId
		{
			get
			{
				return this.extraData.uniqueId;
			}
		}

		private unsafe ref TouchState state
		{
			get
			{
				return ref *(TouchState*)this.m_TouchRecord.GetUnsafeMemoryPtr();
			}
		}

		private unsafe ref Touch.ExtraDataPerTouchState extraData
		{
			get
			{
				return ref *(Touch.ExtraDataPerTouchState*)this.m_TouchRecord.GetUnsafeExtraMemoryPtr();
			}
		}

		public TouchHistory history
		{
			get
			{
				if (!this.valid)
				{
					throw new InvalidOperationException("Touch is invalid");
				}
				return this.finger.GetTouchHistory(this);
			}
		}

		public static ReadOnlyArray<Touch> activeTouches
		{
			get
			{
				Touch.s_GlobalState.playerState.UpdateActiveTouches();
				return new ReadOnlyArray<Touch>(Touch.s_GlobalState.playerState.activeTouches, 0, Touch.s_GlobalState.playerState.activeTouchCount);
			}
		}

		public static ReadOnlyArray<Finger> fingers
		{
			get
			{
				return new ReadOnlyArray<Finger>(Touch.s_GlobalState.playerState.fingers, 0, Touch.s_GlobalState.playerState.totalFingerCount);
			}
		}

		public static ReadOnlyArray<Finger> activeFingers
		{
			get
			{
				Touch.s_GlobalState.playerState.UpdateActiveFingers();
				return new ReadOnlyArray<Finger>(Touch.s_GlobalState.playerState.activeFingers, 0, Touch.s_GlobalState.playerState.activeFingerCount);
			}
		}

		public static IEnumerable<Touchscreen> screens
		{
			get
			{
				return Touch.s_GlobalState.touchscreens;
			}
		}

		public static event Action<Finger> onFingerDown
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerDown.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerDown.RemoveCallback(value);
			}
		}

		public static event Action<Finger> onFingerUp
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerUp.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerUp.RemoveCallback(value);
			}
		}

		public static event Action<Finger> onFingerMove
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerMove.AddCallback(value);
			}
			remove
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				Touch.s_GlobalState.onFingerMove.RemoveCallback(value);
			}
		}

		public static int maxHistoryLengthPerFinger
		{
			get
			{
				return Touch.s_GlobalState.historyLengthPerFinger;
			}
		}

		internal Touch(Finger finger, InputStateHistory<TouchState>.Record touchRecord)
		{
			this.m_Finger = finger;
			this.m_TouchRecord = touchRecord;
		}

		public override string ToString()
		{
			if (!this.valid)
			{
				return "<None>";
			}
			return string.Format("{{id={0} finger={1} phase={2} position={3} delta={4} time={5}}}", new object[]
			{
				this.touchId,
				this.finger.index,
				this.phase,
				this.screenPosition,
				this.delta,
				this.time
			});
		}

		public bool Equals(Touch other)
		{
			return object.Equals(this.m_Finger, other.m_Finger) && this.m_TouchRecord.Equals(other.m_TouchRecord);
		}

		public override bool Equals(object obj)
		{
			if (obj is Touch)
			{
				Touch other = (Touch)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return ((this.m_Finger != null) ? this.m_Finger.GetHashCode() : 0) * 397 ^ this.m_TouchRecord.GetHashCode();
		}

		internal static void AddTouchscreen(Touchscreen screen)
		{
			Touch.s_GlobalState.touchscreens.AppendWithCapacity(screen, 5);
			Touch.s_GlobalState.playerState.AddFingers(screen);
		}

		internal static void RemoveTouchscreen(Touchscreen screen)
		{
			int index = Touch.s_GlobalState.touchscreens.IndexOfReference(screen);
			Touch.s_GlobalState.touchscreens.RemoveAtWithCapacity(index);
			Touch.s_GlobalState.playerState.RemoveFingers(screen);
		}

		internal static void BeginUpdate()
		{
			if (Touch.s_GlobalState.playerState.haveActiveTouchesNeedingRefreshNextUpdate)
			{
				Touch.s_GlobalState.playerState.haveBuiltActiveTouches = false;
			}
		}

		private static Touch.GlobalState CreateGlobalState()
		{
			return new Touch.GlobalState
			{
				historyLengthPerFinger = 64
			};
		}

		internal static ISavedState SaveAndResetState()
		{
			ISavedState result = new SavedStructState<Touch.GlobalState>(ref Touch.s_GlobalState, delegate(ref Touch.GlobalState state)
			{
				Touch.s_GlobalState = state;
			}, delegate()
			{
			});
			Touch.s_GlobalState = Touch.CreateGlobalState();
			return result;
		}

		private readonly Finger m_Finger;

		internal InputStateHistory<TouchState>.Record m_TouchRecord;

		internal static Touch.GlobalState s_GlobalState = Touch.CreateGlobalState();

		internal struct GlobalState
		{
			internal InlinedArray<Touchscreen> touchscreens;

			internal int historyLengthPerFinger;

			internal CallbackArray<Action<Finger>> onFingerDown;

			internal CallbackArray<Action<Finger>> onFingerMove;

			internal CallbackArray<Action<Finger>> onFingerUp;

			internal Touch.FingerAndTouchState playerState;
		}

		internal struct FingerAndTouchState
		{
			public void AddFingers(Touchscreen screen)
			{
				int count = screen.touches.Count;
				ArrayHelpers.EnsureCapacity<Finger>(ref this.fingers, this.totalFingerCount, count, 10);
				for (int i = 0; i < count; i++)
				{
					Finger value = new Finger(screen, i, this.updateMask);
					ArrayHelpers.AppendWithCapacity<Finger>(ref this.fingers, ref this.totalFingerCount, value, 10);
				}
			}

			public void RemoveFingers(Touchscreen screen)
			{
				int count = screen.touches.Count;
				for (int i = 0; i < this.fingers.Length; i++)
				{
					if (this.fingers[i].screen == screen)
					{
						for (int j = 0; j < count; j++)
						{
							this.fingers[i + j].m_StateHistory.Dispose();
						}
						ArrayHelpers.EraseSliceWithCapacity<Finger>(ref this.fingers, ref this.totalFingerCount, i, count);
						break;
					}
				}
				this.haveBuiltActiveTouches = false;
			}

			public void Destroy()
			{
				for (int i = 0; i < this.totalFingerCount; i++)
				{
					this.fingers[i].m_StateHistory.Dispose();
				}
				InputStateHistory<TouchState> inputStateHistory = this.activeTouchState;
				if (inputStateHistory != null)
				{
					inputStateHistory.Dispose();
				}
				this.activeTouchState = null;
			}

			public void UpdateActiveFingers()
			{
				this.activeFingerCount = 0;
				for (int i = 0; i < this.totalFingerCount; i++)
				{
					Finger finger = this.fingers[i];
					if (finger.currentTouch.valid)
					{
						ArrayHelpers.AppendWithCapacity<Finger>(ref this.activeFingers, ref this.activeFingerCount, finger, 10);
					}
				}
			}

			public unsafe void UpdateActiveTouches()
			{
				if (this.haveBuiltActiveTouches)
				{
					return;
				}
				if (this.activeTouchState == null)
				{
					this.activeTouchState = new InputStateHistory<TouchState>(null)
					{
						extraMemoryPerRecord = UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>()
					};
				}
				else
				{
					this.activeTouchState.Clear();
					this.activeTouchState.m_ControlCount = 0;
					this.activeTouchState.m_Controls.Clear<InputControl>();
				}
				this.activeTouchCount = 0;
				this.haveActiveTouchesNeedingRefreshNextUpdate = false;
				uint s_UpdateStepCount = InputUpdate.s_UpdateStepCount;
				for (int i = 0; i < this.totalFingerCount; i++)
				{
					ref Finger ptr = ref this.fingers[i];
					InputStateHistory<TouchState> stateHistory = ptr.m_StateHistory;
					int count = stateHistory.Count;
					if (count != 0)
					{
						int index = this.activeTouchCount;
						int num = 0;
						TouchState* ptr2 = default(TouchState*);
						int num2 = stateHistory.UserIndexToRecordIndex(count - 1);
						InputStateHistory.RecordHeader* ptr3 = stateHistory.GetRecordUnchecked(num2);
						int bytesPerRecord = stateHistory.bytesPerRecord;
						int num3 = bytesPerRecord - stateHistory.extraMemoryPerRecord;
						for (int j = 0; j < count; j++)
						{
							if (j != 0)
							{
								num2--;
								if (num2 < 0)
								{
									num2 = stateHistory.historyDepth - 1;
									ptr3 = stateHistory.GetRecordUnchecked(num2);
								}
								else
								{
									ptr3 -= bytesPerRecord / sizeof(InputStateHistory.RecordHeader);
								}
							}
							TouchState* statePtrWithoutControlIndex = (TouchState*)ptr3->statePtrWithoutControlIndex;
							bool flag = statePtrWithoutControlIndex->updateStepCount == s_UpdateStepCount;
							if (statePtrWithoutControlIndex->touchId == num && !statePtrWithoutControlIndex->phase.IsEndedOrCanceled())
							{
								if (flag && statePtrWithoutControlIndex->phase == TouchPhase.Began)
								{
									ptr2->phase = TouchPhase.Began;
									ptr2->position = statePtrWithoutControlIndex->position;
									ptr2->delta = default(Vector2);
									this.haveActiveTouchesNeedingRefreshNextUpdate = true;
								}
							}
							else
							{
								if (statePtrWithoutControlIndex->phase.IsEndedOrCanceled() && (!statePtrWithoutControlIndex->beganInSameFrame || statePtrWithoutControlIndex->updateStepCount != s_UpdateStepCount - 1U) && !flag)
								{
									break;
								}
								Touch.ExtraDataPerTouchState* source = (Touch.ExtraDataPerTouchState*)(ptr3 + num3 / sizeof(InputStateHistory.RecordHeader));
								int index2;
								InputStateHistory.RecordHeader* ptr4 = this.activeTouchState.AllocateRecord(out index2);
								TouchState* statePtrWithControlIndex = (TouchState*)ptr4->statePtrWithControlIndex;
								Touch.ExtraDataPerTouchState* ptr5 = (Touch.ExtraDataPerTouchState*)(ptr4 + this.activeTouchState.bytesPerRecord / sizeof(InputStateHistory.RecordHeader) - UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>() / sizeof(InputStateHistory.RecordHeader));
								ptr4->time = ptr3->time;
								ptr4->controlIndex = ArrayHelpers.AppendWithCapacity<InputControl>(ref this.activeTouchState.m_Controls, ref this.activeTouchState.m_ControlCount, ptr.m_StateHistory.controls[0], 10);
								UnsafeUtility.MemCpy((void*)statePtrWithControlIndex, (void*)statePtrWithoutControlIndex, (long)UnsafeUtility.SizeOf<TouchState>());
								UnsafeUtility.MemCpy((void*)ptr5, (void*)source, (long)UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>());
								TouchPhase phase = statePtrWithoutControlIndex->phase;
								if ((phase == TouchPhase.Moved || phase == TouchPhase.Began) && !flag && (phase != TouchPhase.Moved || !statePtrWithoutControlIndex->beganInSameFrame || statePtrWithoutControlIndex->updateStepCount != s_UpdateStepCount - 1U))
								{
									statePtrWithControlIndex->phase = TouchPhase.Stationary;
									statePtrWithControlIndex->delta = default(Vector2);
								}
								else if (!flag && !statePtrWithoutControlIndex->beganInSameFrame)
								{
									statePtrWithControlIndex->delta = default(Vector2);
								}
								else
								{
									statePtrWithControlIndex->delta = ptr5->accumulatedDelta;
								}
								InputStateHistory<TouchState>.Record touchRecord = new InputStateHistory<TouchState>.Record(this.activeTouchState, index2, ptr4);
								Touch value = new Touch(ptr, touchRecord);
								ArrayHelpers.InsertAtWithCapacity<Touch>(ref this.activeTouches, ref this.activeTouchCount, index, value, 10);
								num = statePtrWithoutControlIndex->touchId;
								ptr2 = statePtrWithControlIndex;
								if (value.phase != TouchPhase.Stationary)
								{
									this.haveActiveTouchesNeedingRefreshNextUpdate = true;
								}
							}
						}
					}
				}
				this.haveBuiltActiveTouches = true;
			}

			public InputUpdateType updateMask;

			public Finger[] fingers;

			public Finger[] activeFingers;

			public Touch[] activeTouches;

			public int activeFingerCount;

			public int activeTouchCount;

			public int totalFingerCount;

			public uint lastId;

			public bool haveBuiltActiveTouches;

			public bool haveActiveTouchesNeedingRefreshNextUpdate;

			public InputStateHistory<TouchState> activeTouchState;
		}

		internal struct ExtraDataPerTouchState
		{
			public Vector2 accumulatedDelta;

			public uint uniqueId;
		}
	}
}
