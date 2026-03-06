using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	[StructLayout(LayoutKind.Explicit, Size = 560)]
	internal struct TouchscreenState : IInputStateTypeInfo
	{
		public static FourCC Format
		{
			get
			{
				return new FourCC('T', 'S', 'C', 'R');
			}
		}

		public unsafe TouchState* primaryTouch
		{
			get
			{
				fixed (byte* ptr = &this.primaryTouchData.FixedElementField)
				{
					return (TouchState*)ptr;
				}
			}
		}

		public unsafe TouchState* touches
		{
			get
			{
				fixed (byte* ptr = &this.touchData.FixedElementField)
				{
					return (TouchState*)ptr;
				}
			}
		}

		public FourCC format
		{
			get
			{
				return TouchscreenState.Format;
			}
		}

		public const int MaxTouches = 10;

		[FixedBuffer(typeof(byte), 56)]
		[InputControl(name = "primaryTouch", displayName = "Primary Touch", layout = "Touch", synthetic = true)]
		[InputControl(name = "primaryTouch/tap", usage = "PrimaryAction")]
		[InputControl(name = "position", useStateFrom = "primaryTouch/position")]
		[InputControl(name = "delta", useStateFrom = "primaryTouch/delta", layout = "Delta")]
		[InputControl(name = "pressure", useStateFrom = "primaryTouch/pressure")]
		[InputControl(name = "radius", useStateFrom = "primaryTouch/radius")]
		[InputControl(name = "press", useStateFrom = "primaryTouch/phase", layout = "TouchPress", synthetic = true, usages = new string[]
		{

		})]
		[InputControl(name = "displayIndex", useStateFrom = "primaryTouch/displayIndex", format = "BYTE")]
		[FieldOffset(0)]
		public TouchscreenState.<primaryTouchData>e__FixedBuffer primaryTouchData;

		internal const int kTouchDataOffset = 56;

		[FixedBuffer(typeof(byte), 560)]
		[InputControl(layout = "Touch", name = "touch", displayName = "Touch", arraySize = 10)]
		[FieldOffset(56)]
		public TouchscreenState.<touchData>e__FixedBuffer touchData;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 56)]
		public struct <primaryTouchData>e__FixedBuffer
		{
			public byte FixedElementField;
		}

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 560)]
		public struct <touchData>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
