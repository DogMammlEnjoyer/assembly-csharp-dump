using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.Controls
{
	[InputControlLayout(stateType = typeof(TouchState))]
	public class TouchControl : InputControl<TouchState>
	{
		public TouchPressControl press { get; set; }

		public IntegerControl displayIndex { get; set; }

		public IntegerControl touchId { get; set; }

		public Vector2Control position { get; set; }

		public DeltaControl delta { get; set; }

		public AxisControl pressure { get; set; }

		public Vector2Control radius { get; set; }

		public TouchPhaseControl phase { get; set; }

		public ButtonControl indirectTouch { get; set; }

		public ButtonControl tap { get; set; }

		public IntegerControl tapCount { get; set; }

		public DoubleControl startTime { get; set; }

		public Vector2Control startPosition { get; set; }

		public unsafe bool isInProgress
		{
			get
			{
				TouchPhase touchPhase = (TouchPhase)(*this.phase.value);
				return touchPhase - TouchPhase.Began <= 1 || touchPhase == TouchPhase.Stationary;
			}
		}

		public TouchControl()
		{
			this.m_StateBlock.format = new FourCC('T', 'O', 'U', 'C');
		}

		protected override void FinishSetup()
		{
			this.press = base.GetChildControl<TouchPressControl>("press");
			this.displayIndex = base.GetChildControl<IntegerControl>("displayIndex");
			this.touchId = base.GetChildControl<IntegerControl>("touchId");
			this.position = base.GetChildControl<Vector2Control>("position");
			this.delta = base.GetChildControl<DeltaControl>("delta");
			this.pressure = base.GetChildControl<AxisControl>("pressure");
			this.radius = base.GetChildControl<Vector2Control>("radius");
			this.phase = base.GetChildControl<TouchPhaseControl>("phase");
			this.indirectTouch = base.GetChildControl<ButtonControl>("indirectTouch");
			this.tap = base.GetChildControl<ButtonControl>("tap");
			this.tapCount = base.GetChildControl<IntegerControl>("tapCount");
			this.startTime = base.GetChildControl<DoubleControl>("startTime");
			this.startPosition = base.GetChildControl<Vector2Control>("startPosition");
			base.FinishSetup();
		}

		public unsafe override TouchState ReadUnprocessedValueFromState(void* statePtr)
		{
			TouchState* ptr = (TouchState*)((byte*)statePtr + this.m_StateBlock.byteOffset);
			return *ptr;
		}

		public unsafe override void WriteValueIntoState(TouchState value, void* statePtr)
		{
			TouchState* destination = (TouchState*)((byte*)statePtr + this.m_StateBlock.byteOffset);
			UnsafeUtility.MemCpy((void*)destination, UnsafeUtility.AddressOf<TouchState>(ref value), (long)UnsafeUtility.SizeOf<TouchState>());
		}
	}
}
