using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 36)]
	public struct PenState : IInputStateTypeInfo
	{
		public static FourCC Format
		{
			get
			{
				return new FourCC('P', 'E', 'N', ' ');
			}
		}

		public PenState WithButton(PenButton button, bool state = true)
		{
			uint num = 1U << (int)button;
			if (state)
			{
				this.buttons |= (ushort)num;
			}
			else
			{
				this.buttons &= (ushort)(~(ushort)num);
			}
			return this;
		}

		public FourCC format
		{
			get
			{
				return PenState.Format;
			}
		}

		[InputControl(usage = "Point", dontReset = true)]
		[FieldOffset(0)]
		public Vector2 position;

		[InputControl(usage = "Secondary2DMotion", layout = "Delta")]
		[FieldOffset(8)]
		public Vector2 delta;

		[InputControl(layout = "Vector2", displayName = "Tilt", usage = "Tilt")]
		[FieldOffset(16)]
		public Vector2 tilt;

		[InputControl(layout = "Analog", usage = "Pressure", defaultState = 0f)]
		[FieldOffset(24)]
		public float pressure;

		[InputControl(layout = "Axis", displayName = "Twist", usage = "Twist")]
		[FieldOffset(28)]
		public float twist;

		[InputControl(name = "tip", displayName = "Tip", layout = "Button", bit = 0U, usage = "PrimaryAction")]
		[InputControl(name = "press", useStateFrom = "tip", synthetic = true, usages = new string[]
		{

		})]
		[InputControl(name = "eraser", displayName = "Eraser", layout = "Button", bit = 1U)]
		[InputControl(name = "inRange", displayName = "In Range?", layout = "Button", bit = 4U, synthetic = true)]
		[InputControl(name = "barrel1", displayName = "Barrel Button #1", layout = "Button", bit = 2U, alias = "barrelFirst", usage = "SecondaryAction")]
		[InputControl(name = "barrel2", displayName = "Barrel Button #2", layout = "Button", bit = 3U, alias = "barrelSecond")]
		[InputControl(name = "barrel3", displayName = "Barrel Button #3", layout = "Button", bit = 5U, alias = "barrelThird")]
		[InputControl(name = "barrel4", displayName = "Barrel Button #4", layout = "Button", bit = 6U, alias = "barrelFourth")]
		[InputControl(name = "radius", layout = "Vector2", format = "VEC2", sizeInBits = 64U, usage = "Radius", offset = 4294967294U)]
		[InputControl(name = "pointerId", layout = "Digital", format = "UINT", sizeInBits = 32U, offset = 4294967294U)]
		[FieldOffset(32)]
		public ushort buttons;

		[InputControl(name = "displayIndex", displayName = "Display Index", layout = "Integer")]
		[FieldOffset(34)]
		private ushort displayIndex;
	}
}
