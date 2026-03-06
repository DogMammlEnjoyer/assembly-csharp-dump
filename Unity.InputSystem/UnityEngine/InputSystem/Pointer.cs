using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(PointerState), isGenericTypeOfDevice = true)]
	public class Pointer : InputDevice, IInputStateCallbackReceiver
	{
		public Vector2Control position { get; protected set; }

		public DeltaControl delta { get; protected set; }

		public Vector2Control radius { get; protected set; }

		public AxisControl pressure { get; protected set; }

		public ButtonControl press { get; protected set; }

		public IntegerControl displayIndex { get; protected set; }

		public static Pointer current { get; internal set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Pointer.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Pointer.current == this)
			{
				Pointer.current = null;
			}
		}

		protected override void FinishSetup()
		{
			this.position = base.GetChildControl<Vector2Control>("position");
			this.delta = base.GetChildControl<DeltaControl>("delta");
			this.radius = base.GetChildControl<Vector2Control>("radius");
			this.pressure = base.GetChildControl<AxisControl>("pressure");
			this.press = base.GetChildControl<ButtonControl>("press");
			this.displayIndex = base.GetChildControl<IntegerControl>("displayIndex");
			base.FinishSetup();
		}

		protected void OnNextUpdate()
		{
			InputState.Change<Vector2>(this.delta, Vector2.zero, InputUpdateType.None, default(InputEventPtr));
		}

		protected void OnStateEvent(InputEventPtr eventPtr)
		{
			this.delta.AccumulateValueInEvent(base.currentStatePtr, eventPtr);
			InputState.Change(this, eventPtr, InputUpdateType.None);
		}

		void IInputStateCallbackReceiver.OnNextUpdate()
		{
			this.OnNextUpdate();
		}

		void IInputStateCallbackReceiver.OnStateEvent(InputEventPtr eventPtr)
		{
			this.OnStateEvent(eventPtr);
		}

		bool IInputStateCallbackReceiver.GetStateOffsetForEvent(InputControl control, InputEventPtr eventPtr, ref uint offset)
		{
			return false;
		}
	}
}
