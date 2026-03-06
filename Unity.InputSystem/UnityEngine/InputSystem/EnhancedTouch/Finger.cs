using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.EnhancedTouch
{
	public class Finger
	{
		public Touchscreen screen { get; }

		public int index { get; }

		public bool isActive
		{
			get
			{
				return this.currentTouch.valid;
			}
		}

		public Vector2 screenPosition
		{
			get
			{
				Touch lastTouch = this.lastTouch;
				if (!lastTouch.valid)
				{
					return default(Vector2);
				}
				return lastTouch.screenPosition;
			}
		}

		public Touch lastTouch
		{
			get
			{
				int count = this.m_StateHistory.Count;
				if (count == 0)
				{
					return default(Touch);
				}
				return new Touch(this, this.m_StateHistory[count - 1]);
			}
		}

		public Touch currentTouch
		{
			get
			{
				Touch lastTouch = this.lastTouch;
				if (!lastTouch.valid)
				{
					return default(Touch);
				}
				if (lastTouch.isInProgress)
				{
					return lastTouch;
				}
				if (lastTouch.updateStepCount == InputUpdate.s_UpdateStepCount)
				{
					return lastTouch;
				}
				return default(Touch);
			}
		}

		public TouchHistory touchHistory
		{
			get
			{
				return new TouchHistory(this, this.m_StateHistory, -1, -1);
			}
		}

		internal unsafe Finger(Touchscreen screen, int index, InputUpdateType updateMask)
		{
			this.screen = screen;
			this.index = index;
			this.m_StateHistory = new InputStateHistory<TouchState>(screen.touches[index])
			{
				historyDepth = Touch.maxHistoryLengthPerFinger,
				extraMemoryPerRecord = UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>(),
				onRecordAdded = new Action<InputStateHistory.Record>(this.OnTouchRecorded),
				onShouldRecordStateChange = new Func<InputControl, double, InputEventPtr, bool>(Finger.ShouldRecordTouch),
				updateMask = updateMask
			};
			this.m_StateHistory.StartRecording();
			if (screen.touches[index].isInProgress)
			{
				this.m_StateHistory.RecordStateChange(screen.touches[index], *screen.touches[index].value, -1.0);
			}
		}

		private unsafe static bool ShouldRecordTouch(InputControl control, double time, InputEventPtr eventPtr)
		{
			if (!eventPtr.valid)
			{
				return false;
			}
			FourCC type = eventPtr.type;
			if (type != 1398030676 && type != 1145852993)
			{
				return false;
			}
			TouchState* ptr = (TouchState*)((byte*)control.currentStatePtr + control.stateBlock.byteOffset);
			return !ptr->isTapRelease;
		}

		private unsafe void OnTouchRecorded(InputStateHistory.Record record)
		{
			int recordIndex = record.recordIndex;
			InputStateHistory.RecordHeader* recordUnchecked = this.m_StateHistory.GetRecordUnchecked(recordIndex);
			TouchState* statePtrWithoutControlIndex = (TouchState*)recordUnchecked->statePtrWithoutControlIndex;
			statePtrWithoutControlIndex->updateStepCount = InputUpdate.s_UpdateStepCount;
			Touch.s_GlobalState.playerState.haveBuiltActiveTouches = false;
			Touch.ExtraDataPerTouchState* ptr = (Touch.ExtraDataPerTouchState*)(recordUnchecked + this.m_StateHistory.bytesPerRecord / sizeof(InputStateHistory.RecordHeader) - UnsafeUtility.SizeOf<Touch.ExtraDataPerTouchState>() / sizeof(InputStateHistory.RecordHeader));
			ref Touch.ExtraDataPerTouchState ptr2 = ref *ptr;
			uint num = Touch.s_GlobalState.playerState.lastId + 1U;
			Touch.s_GlobalState.playerState.lastId = num;
			ptr2.uniqueId = num;
			ptr->accumulatedDelta = statePtrWithoutControlIndex->delta;
			if (statePtrWithoutControlIndex->phase != TouchPhase.Began)
			{
				if (recordIndex != this.m_StateHistory.m_HeadIndex)
				{
					int index = (recordIndex == 0) ? (this.m_StateHistory.historyDepth - 1) : (recordIndex - 1);
					TouchState* statePtrWithoutControlIndex2 = (TouchState*)this.m_StateHistory.GetRecordUnchecked(index)->statePtrWithoutControlIndex;
					statePtrWithoutControlIndex->delta -= statePtrWithoutControlIndex2->delta;
					statePtrWithoutControlIndex->beganInSameFrame = (statePtrWithoutControlIndex2->beganInSameFrame && statePtrWithoutControlIndex2->updateStepCount == statePtrWithoutControlIndex->updateStepCount);
				}
			}
			else
			{
				statePtrWithoutControlIndex->beganInSameFrame = true;
			}
			switch (statePtrWithoutControlIndex->phase)
			{
			case TouchPhase.Began:
				DelegateHelpers.InvokeCallbacksSafe<Finger>(ref Touch.s_GlobalState.onFingerDown, this, "Touch.onFingerDown", null);
				return;
			case TouchPhase.Moved:
				DelegateHelpers.InvokeCallbacksSafe<Finger>(ref Touch.s_GlobalState.onFingerMove, this, "Touch.onFingerMove", null);
				return;
			case TouchPhase.Ended:
			case TouchPhase.Canceled:
				DelegateHelpers.InvokeCallbacksSafe<Finger>(ref Touch.s_GlobalState.onFingerUp, this, "Touch.onFingerUp", null);
				return;
			default:
				return;
			}
		}

		private unsafe Touch FindTouch(uint uniqueId)
		{
			foreach (InputStateHistory<TouchState>.Record touchRecord in this.m_StateHistory)
			{
				if (((Touch.ExtraDataPerTouchState*)touchRecord.GetUnsafeExtraMemoryPtrUnchecked())->uniqueId == uniqueId)
				{
					return new Touch(this, touchRecord);
				}
			}
			return default(Touch);
		}

		internal unsafe TouchHistory GetTouchHistory(Touch touch)
		{
			InputStateHistory<TouchState>.Record touchRecord = touch.m_TouchRecord;
			if (touchRecord.owner != this.m_StateHistory)
			{
				touch = this.FindTouch(touch.uniqueId);
				if (!touch.valid)
				{
					return default(TouchHistory);
				}
			}
			int touchId = touch.touchId;
			int num = touch.m_TouchRecord.index;
			int num2 = 0;
			if (touch.phase != TouchPhase.Began)
			{
				InputStateHistory<TouchState>.Record previous = touch.m_TouchRecord.previous;
				while (previous.valid)
				{
					TouchState* unsafeMemoryPtr = (TouchState*)previous.GetUnsafeMemoryPtr();
					if (unsafeMemoryPtr->touchId != touchId)
					{
						break;
					}
					num2++;
					if (unsafeMemoryPtr->phase == TouchPhase.Began)
					{
						break;
					}
					previous = previous.previous;
				}
			}
			if (num2 == 0)
			{
				return default(TouchHistory);
			}
			num--;
			return new TouchHistory(this, this.m_StateHistory, num, num2);
		}

		internal readonly InputStateHistory<TouchState> m_StateHistory;
	}
}
