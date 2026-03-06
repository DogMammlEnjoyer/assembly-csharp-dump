using System;
using System.ComponentModel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Haptics;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(GamepadState), isGenericTypeOfDevice = true)]
	public class Gamepad : InputDevice, IDualMotorRumble, IHaptics
	{
		public ButtonControl buttonWest { get; protected set; }

		public ButtonControl buttonNorth { get; protected set; }

		public ButtonControl buttonSouth { get; protected set; }

		public ButtonControl buttonEast { get; protected set; }

		public ButtonControl leftStickButton { get; protected set; }

		public ButtonControl rightStickButton { get; protected set; }

		public ButtonControl startButton { get; protected set; }

		public ButtonControl selectButton { get; protected set; }

		public DpadControl dpad { get; protected set; }

		public ButtonControl leftShoulder { get; protected set; }

		public ButtonControl rightShoulder { get; protected set; }

		public StickControl leftStick { get; protected set; }

		public StickControl rightStick { get; protected set; }

		public ButtonControl leftTrigger { get; protected set; }

		public ButtonControl rightTrigger { get; protected set; }

		public ButtonControl aButton
		{
			get
			{
				return this.buttonSouth;
			}
		}

		public ButtonControl bButton
		{
			get
			{
				return this.buttonEast;
			}
		}

		public ButtonControl xButton
		{
			get
			{
				return this.buttonWest;
			}
		}

		public ButtonControl yButton
		{
			get
			{
				return this.buttonNorth;
			}
		}

		public ButtonControl triangleButton
		{
			get
			{
				return this.buttonNorth;
			}
		}

		public ButtonControl squareButton
		{
			get
			{
				return this.buttonWest;
			}
		}

		public ButtonControl circleButton
		{
			get
			{
				return this.buttonEast;
			}
		}

		public ButtonControl crossButton
		{
			get
			{
				return this.buttonSouth;
			}
		}

		public ButtonControl this[GamepadButton button]
		{
			get
			{
				switch (button)
				{
				case GamepadButton.DpadUp:
					return this.dpad.up;
				case GamepadButton.DpadDown:
					return this.dpad.down;
				case GamepadButton.DpadLeft:
					return this.dpad.left;
				case GamepadButton.DpadRight:
					return this.dpad.right;
				case GamepadButton.North:
					return this.buttonNorth;
				case GamepadButton.East:
					return this.buttonEast;
				case GamepadButton.South:
					return this.buttonSouth;
				case GamepadButton.West:
					return this.buttonWest;
				case GamepadButton.LeftStick:
					return this.leftStickButton;
				case GamepadButton.RightStick:
					return this.rightStickButton;
				case GamepadButton.LeftShoulder:
					return this.leftShoulder;
				case GamepadButton.RightShoulder:
					return this.rightShoulder;
				case GamepadButton.Start:
					return this.startButton;
				case GamepadButton.Select:
					return this.selectButton;
				default:
					if (button == GamepadButton.LeftTrigger)
					{
						return this.leftTrigger;
					}
					if (button != GamepadButton.RightTrigger)
					{
						throw new InvalidEnumArgumentException("button", (int)button, typeof(GamepadButton));
					}
					return this.rightTrigger;
				}
			}
		}

		public static Gamepad current { get; private set; }

		public new static ReadOnlyArray<Gamepad> all
		{
			get
			{
				return new ReadOnlyArray<Gamepad>(Gamepad.s_Gamepads, 0, Gamepad.s_GamepadCount);
			}
		}

		protected override void FinishSetup()
		{
			this.buttonWest = base.GetChildControl<ButtonControl>("buttonWest");
			this.buttonNorth = base.GetChildControl<ButtonControl>("buttonNorth");
			this.buttonSouth = base.GetChildControl<ButtonControl>("buttonSouth");
			this.buttonEast = base.GetChildControl<ButtonControl>("buttonEast");
			this.startButton = base.GetChildControl<ButtonControl>("start");
			this.selectButton = base.GetChildControl<ButtonControl>("select");
			this.leftStickButton = base.GetChildControl<ButtonControl>("leftStickPress");
			this.rightStickButton = base.GetChildControl<ButtonControl>("rightStickPress");
			this.dpad = base.GetChildControl<DpadControl>("dpad");
			this.leftShoulder = base.GetChildControl<ButtonControl>("leftShoulder");
			this.rightShoulder = base.GetChildControl<ButtonControl>("rightShoulder");
			this.leftStick = base.GetChildControl<StickControl>("leftStick");
			this.rightStick = base.GetChildControl<StickControl>("rightStick");
			this.leftTrigger = base.GetChildControl<ButtonControl>("leftTrigger");
			this.rightTrigger = base.GetChildControl<ButtonControl>("rightTrigger");
			base.FinishSetup();
		}

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Gamepad.current = this;
		}

		protected override void OnAdded()
		{
			ArrayHelpers.AppendWithCapacity<Gamepad>(ref Gamepad.s_Gamepads, ref Gamepad.s_GamepadCount, this, 10);
		}

		protected override void OnRemoved()
		{
			if (Gamepad.current == this)
			{
				Gamepad.current = null;
			}
			int num = Gamepad.s_Gamepads.IndexOfReference(this, Gamepad.s_GamepadCount);
			if (num != -1)
			{
				Gamepad.s_Gamepads.EraseAtWithCapacity(ref Gamepad.s_GamepadCount, num);
			}
		}

		public virtual void PauseHaptics()
		{
			this.m_Rumble.PauseHaptics(this);
		}

		public virtual void ResumeHaptics()
		{
			this.m_Rumble.ResumeHaptics(this);
		}

		public virtual void ResetHaptics()
		{
			this.m_Rumble.ResetHaptics(this);
		}

		public virtual void SetMotorSpeeds(float lowFrequency, float highFrequency)
		{
			this.m_Rumble.SetMotorSpeeds(this, lowFrequency, highFrequency);
		}

		private DualMotorRumble m_Rumble;

		private static int s_GamepadCount;

		private static Gamepad[] s_Gamepads;
	}
}
