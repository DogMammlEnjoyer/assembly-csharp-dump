using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(JoystickState), isGenericTypeOfDevice = true)]
	public class Joystick : InputDevice
	{
		public ButtonControl trigger { get; protected set; }

		public StickControl stick { get; protected set; }

		public AxisControl twist { get; protected set; }

		public Vector2Control hatswitch { get; protected set; }

		public static Joystick current { get; private set; }

		public new static ReadOnlyArray<Joystick> all
		{
			get
			{
				return new ReadOnlyArray<Joystick>(Joystick.s_Joysticks, 0, Joystick.s_JoystickCount);
			}
		}

		protected override void FinishSetup()
		{
			this.trigger = base.GetChildControl<ButtonControl>("{PrimaryTrigger}");
			this.stick = base.GetChildControl<StickControl>("{Primary2DMotion}");
			this.twist = base.TryGetChildControl<AxisControl>("{Twist}");
			this.hatswitch = base.TryGetChildControl<Vector2Control>("{Hatswitch}");
			base.FinishSetup();
		}

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Joystick.current = this;
		}

		protected override void OnAdded()
		{
			ArrayHelpers.AppendWithCapacity<Joystick>(ref Joystick.s_Joysticks, ref Joystick.s_JoystickCount, this, 10);
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Joystick.current == this)
			{
				Joystick.current = null;
			}
			int num = Joystick.s_Joysticks.IndexOfReference(this, Joystick.s_JoystickCount);
			if (num != -1)
			{
				Joystick.s_Joysticks.EraseAtWithCapacity(ref Joystick.s_JoystickCount, num);
			}
		}

		private static int s_JoystickCount;

		private static Joystick[] s_Joysticks;
	}
}
