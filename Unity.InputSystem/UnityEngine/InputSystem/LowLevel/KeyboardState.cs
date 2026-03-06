using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem.LowLevel
{
	public struct KeyboardState : IInputStateTypeInfo
	{
		public static FourCC Format
		{
			get
			{
				return new FourCC('K', 'E', 'Y', 'S');
			}
		}

		public KeyboardState(params Key[] pressedKeys)
		{
			this = new KeyboardState(false, pressedKeys);
		}

		public unsafe KeyboardState(bool IMESelected, params Key[] pressedKeys)
		{
			if (pressedKeys == null)
			{
				throw new ArgumentNullException("pressedKeys");
			}
			fixed (byte* ptr = &this.keys.FixedElementField)
			{
				byte* ptr2 = ptr;
				UnsafeUtility.MemClear((void*)ptr2, 16L);
				if (IMESelected)
				{
					MemoryHelpers.WriteSingleBit((void*)ptr2, 111U, true);
				}
				for (int i = 0; i < pressedKeys.Length; i++)
				{
					MemoryHelpers.WriteSingleBit((void*)ptr2, (uint)pressedKeys[i], true);
				}
			}
		}

		public unsafe void Set(Key key, bool state)
		{
			fixed (byte* ptr = &this.keys.FixedElementField)
			{
				MemoryHelpers.WriteSingleBit((void*)ptr, (uint)key, state);
			}
		}

		internal unsafe bool Get(Key key)
		{
			fixed (byte* ptr = &this.keys.FixedElementField)
			{
				return MemoryHelpers.ReadSingleBit((void*)ptr, (uint)key);
			}
		}

		public void Press(Key key)
		{
			this.Set(key, true);
		}

		public void Release(Key key)
		{
			this.Set(key, false);
		}

		public FourCC format
		{
			get
			{
				return KeyboardState.Format;
			}
		}

		private const int kSizeInBits = 123;

		internal const int kSizeInBytes = 16;

		[FixedBuffer(typeof(byte), 16)]
		[InputControl(name = "anyKey", displayName = "Any Key", layout = "AnyKey", bit = 1U, sizeInBits = 123U, synthetic = true)]
		[InputControl(name = "escape", displayName = "Escape", layout = "Key", usages = new string[]
		{
			"Back",
			"Cancel"
		}, bit = 60U)]
		[InputControl(name = "space", displayName = "Space", layout = "Key", bit = 1U)]
		[InputControl(name = "enter", displayName = "Enter", layout = "Key", usage = "Submit", bit = 2U)]
		[InputControl(name = "tab", displayName = "Tab", layout = "Key", bit = 3U)]
		[InputControl(name = "backquote", displayName = "`", layout = "Key", bit = 4U)]
		[InputControl(name = "quote", displayName = "'", layout = "Key", bit = 5U)]
		[InputControl(name = "semicolon", displayName = ";", layout = "Key", bit = 6U)]
		[InputControl(name = "comma", displayName = ",", layout = "Key", bit = 7U)]
		[InputControl(name = "period", displayName = ".", layout = "Key", bit = 8U)]
		[InputControl(name = "slash", displayName = "/", layout = "Key", bit = 9U)]
		[InputControl(name = "backslash", displayName = "\\", layout = "Key", bit = 10U)]
		[InputControl(name = "leftBracket", displayName = "[", layout = "Key", bit = 11U)]
		[InputControl(name = "rightBracket", displayName = "]", layout = "Key", bit = 12U)]
		[InputControl(name = "minus", displayName = "-", layout = "Key", bit = 13U)]
		[InputControl(name = "equals", displayName = "=", layout = "Key", bit = 14U)]
		[InputControl(name = "upArrow", displayName = "Up Arrow", layout = "Key", bit = 63U)]
		[InputControl(name = "downArrow", displayName = "Down Arrow", layout = "Key", bit = 64U)]
		[InputControl(name = "leftArrow", displayName = "Left Arrow", layout = "Key", bit = 61U)]
		[InputControl(name = "rightArrow", displayName = "Right Arrow", layout = "Key", bit = 62U)]
		[InputControl(name = "a", displayName = "A", layout = "Key", bit = 15U)]
		[InputControl(name = "b", displayName = "B", layout = "Key", bit = 16U)]
		[InputControl(name = "c", displayName = "C", layout = "Key", bit = 17U)]
		[InputControl(name = "d", displayName = "D", layout = "Key", bit = 18U)]
		[InputControl(name = "e", displayName = "E", layout = "Key", bit = 19U)]
		[InputControl(name = "f", displayName = "F", layout = "Key", bit = 20U)]
		[InputControl(name = "g", displayName = "G", layout = "Key", bit = 21U)]
		[InputControl(name = "h", displayName = "H", layout = "Key", bit = 22U)]
		[InputControl(name = "i", displayName = "I", layout = "Key", bit = 23U)]
		[InputControl(name = "j", displayName = "J", layout = "Key", bit = 24U)]
		[InputControl(name = "k", displayName = "K", layout = "Key", bit = 25U)]
		[InputControl(name = "l", displayName = "L", layout = "Key", bit = 26U)]
		[InputControl(name = "m", displayName = "M", layout = "Key", bit = 27U)]
		[InputControl(name = "n", displayName = "N", layout = "Key", bit = 28U)]
		[InputControl(name = "o", displayName = "O", layout = "Key", bit = 29U)]
		[InputControl(name = "p", displayName = "P", layout = "Key", bit = 30U)]
		[InputControl(name = "q", displayName = "Q", layout = "Key", bit = 31U)]
		[InputControl(name = "r", displayName = "R", layout = "Key", bit = 32U)]
		[InputControl(name = "s", displayName = "S", layout = "Key", bit = 33U)]
		[InputControl(name = "t", displayName = "T", layout = "Key", bit = 34U)]
		[InputControl(name = "u", displayName = "U", layout = "Key", bit = 35U)]
		[InputControl(name = "v", displayName = "V", layout = "Key", bit = 36U)]
		[InputControl(name = "w", displayName = "W", layout = "Key", bit = 37U)]
		[InputControl(name = "x", displayName = "X", layout = "Key", bit = 38U)]
		[InputControl(name = "y", displayName = "Y", layout = "Key", bit = 39U)]
		[InputControl(name = "z", displayName = "Z", layout = "Key", bit = 40U)]
		[InputControl(name = "1", displayName = "1", layout = "Key", bit = 41U)]
		[InputControl(name = "2", displayName = "2", layout = "Key", bit = 42U)]
		[InputControl(name = "3", displayName = "3", layout = "Key", bit = 43U)]
		[InputControl(name = "4", displayName = "4", layout = "Key", bit = 44U)]
		[InputControl(name = "5", displayName = "5", layout = "Key", bit = 45U)]
		[InputControl(name = "6", displayName = "6", layout = "Key", bit = 46U)]
		[InputControl(name = "7", displayName = "7", layout = "Key", bit = 47U)]
		[InputControl(name = "8", displayName = "8", layout = "Key", bit = 48U)]
		[InputControl(name = "9", displayName = "9", layout = "Key", bit = 49U)]
		[InputControl(name = "0", displayName = "0", layout = "Key", bit = 50U)]
		[InputControl(name = "leftShift", displayName = "Left Shift", layout = "Key", usage = "Modifier", bit = 51U)]
		[InputControl(name = "rightShift", displayName = "Right Shift", layout = "Key", usage = "Modifier", bit = 52U)]
		[InputControl(name = "shift", displayName = "Shift", layout = "DiscreteButton", usage = "Modifier", bit = 51U, sizeInBits = 2U, synthetic = true, parameters = "minValue=1,maxValue=3,writeMode=1")]
		[InputControl(name = "leftAlt", displayName = "Left Alt", layout = "Key", usage = "Modifier", bit = 53U)]
		[InputControl(name = "rightAlt", displayName = "Right Alt", layout = "Key", usage = "Modifier", bit = 54U, alias = "AltGr")]
		[InputControl(name = "alt", displayName = "Alt", layout = "DiscreteButton", usage = "Modifier", bit = 53U, sizeInBits = 2U, synthetic = true, parameters = "minValue=1,maxValue=3,writeMode=1")]
		[InputControl(name = "leftCtrl", displayName = "Left Control", layout = "Key", usage = "Modifier", bit = 55U)]
		[InputControl(name = "rightCtrl", displayName = "Right Control", layout = "Key", usage = "Modifier", bit = 56U)]
		[InputControl(name = "ctrl", displayName = "Control", layout = "DiscreteButton", usage = "Modifier", bit = 55U, sizeInBits = 2U, synthetic = true, parameters = "minValue=1,maxValue=3,writeMode=1")]
		[InputControl(name = "leftMeta", displayName = "Left System", layout = "Key", usage = "Modifier", bit = 57U, aliases = new string[]
		{
			"LeftWindows",
			"LeftApple",
			"LeftCommand"
		})]
		[InputControl(name = "rightMeta", displayName = "Right System", layout = "Key", usage = "Modifier", bit = 58U, aliases = new string[]
		{
			"RightWindows",
			"RightApple",
			"RightCommand"
		})]
		[InputControl(name = "contextMenu", displayName = "Context Menu", layout = "Key", usage = "Modifier", bit = 59U)]
		[InputControl(name = "backspace", displayName = "Backspace", layout = "Key", bit = 65U)]
		[InputControl(name = "pageDown", displayName = "Page Down", layout = "Key", bit = 66U)]
		[InputControl(name = "pageUp", displayName = "Page Up", layout = "Key", bit = 67U)]
		[InputControl(name = "home", displayName = "Home", layout = "Key", bit = 68U)]
		[InputControl(name = "end", displayName = "End", layout = "Key", bit = 69U)]
		[InputControl(name = "insert", displayName = "Insert", layout = "Key", bit = 70U)]
		[InputControl(name = "delete", displayName = "Delete", layout = "Key", bit = 71U)]
		[InputControl(name = "capsLock", displayName = "Caps Lock", layout = "Key", bit = 72U)]
		[InputControl(name = "numLock", displayName = "Num Lock", layout = "Key", bit = 73U)]
		[InputControl(name = "printScreen", displayName = "Print Screen", layout = "Key", bit = 74U)]
		[InputControl(name = "scrollLock", displayName = "Scroll Lock", layout = "Key", bit = 75U)]
		[InputControl(name = "pause", displayName = "Pause/Break", layout = "Key", bit = 76U)]
		[InputControl(name = "numpadEnter", displayName = "Numpad Enter", layout = "Key", bit = 77U)]
		[InputControl(name = "numpadDivide", displayName = "Numpad /", layout = "Key", bit = 78U)]
		[InputControl(name = "numpadMultiply", displayName = "Numpad *", layout = "Key", bit = 79U)]
		[InputControl(name = "numpadPlus", displayName = "Numpad +", layout = "Key", bit = 80U)]
		[InputControl(name = "numpadMinus", displayName = "Numpad -", layout = "Key", bit = 81U)]
		[InputControl(name = "numpadPeriod", displayName = "Numpad .", layout = "Key", bit = 82U)]
		[InputControl(name = "numpadEquals", displayName = "Numpad =", layout = "Key", bit = 83U)]
		[InputControl(name = "numpad1", displayName = "Numpad 1", layout = "Key", bit = 85U)]
		[InputControl(name = "numpad2", displayName = "Numpad 2", layout = "Key", bit = 86U)]
		[InputControl(name = "numpad3", displayName = "Numpad 3", layout = "Key", bit = 87U)]
		[InputControl(name = "numpad4", displayName = "Numpad 4", layout = "Key", bit = 88U)]
		[InputControl(name = "numpad5", displayName = "Numpad 5", layout = "Key", bit = 89U)]
		[InputControl(name = "numpad6", displayName = "Numpad 6", layout = "Key", bit = 90U)]
		[InputControl(name = "numpad7", displayName = "Numpad 7", layout = "Key", bit = 91U)]
		[InputControl(name = "numpad8", displayName = "Numpad 8", layout = "Key", bit = 92U)]
		[InputControl(name = "numpad9", displayName = "Numpad 9", layout = "Key", bit = 93U)]
		[InputControl(name = "numpad0", displayName = "Numpad 0", layout = "Key", bit = 84U)]
		[InputControl(name = "f1", displayName = "F1", layout = "Key", bit = 94U)]
		[InputControl(name = "f2", displayName = "F2", layout = "Key", bit = 95U)]
		[InputControl(name = "f3", displayName = "F3", layout = "Key", bit = 96U)]
		[InputControl(name = "f4", displayName = "F4", layout = "Key", bit = 97U)]
		[InputControl(name = "f5", displayName = "F5", layout = "Key", bit = 98U)]
		[InputControl(name = "f6", displayName = "F6", layout = "Key", bit = 99U)]
		[InputControl(name = "f7", displayName = "F7", layout = "Key", bit = 100U)]
		[InputControl(name = "f8", displayName = "F8", layout = "Key", bit = 101U)]
		[InputControl(name = "f9", displayName = "F9", layout = "Key", bit = 102U)]
		[InputControl(name = "f10", displayName = "F10", layout = "Key", bit = 103U)]
		[InputControl(name = "f11", displayName = "F11", layout = "Key", bit = 104U)]
		[InputControl(name = "f12", displayName = "F12", layout = "Key", bit = 105U)]
		[InputControl(name = "OEM1", layout = "Key", bit = 106U)]
		[InputControl(name = "OEM2", layout = "Key", bit = 107U)]
		[InputControl(name = "OEM3", layout = "Key", bit = 108U)]
		[InputControl(name = "OEM4", layout = "Key", bit = 109U)]
		[InputControl(name = "OEM5", layout = "Key", bit = 110U)]
		[InputControl(name = "f13", displayName = "F13", layout = "Key", bit = 112U)]
		[InputControl(name = "f14", displayName = "F14", layout = "Key", bit = 113U)]
		[InputControl(name = "f15", displayName = "F15", layout = "Key", bit = 114U)]
		[InputControl(name = "f16", displayName = "F16", layout = "Key", bit = 115U)]
		[InputControl(name = "f17", displayName = "F17", layout = "Key", bit = 116U)]
		[InputControl(name = "f18", displayName = "F18", layout = "Key", bit = 117U)]
		[InputControl(name = "f19", displayName = "F19", layout = "Key", bit = 118U)]
		[InputControl(name = "f20", displayName = "F20", layout = "Key", bit = 119U)]
		[InputControl(name = "f21", displayName = "F21", layout = "Key", bit = 120U)]
		[InputControl(name = "f22", displayName = "F22", layout = "Key", bit = 121U)]
		[InputControl(name = "f23", displayName = "F23", layout = "Key", bit = 122U)]
		[InputControl(name = "f24", displayName = "F24", layout = "Key", bit = 123U)]
		[InputControl(name = "IMESelected", layout = "Button", bit = 127U, synthetic = true)]
		public KeyboardState.<keys>e__FixedBuffer keys;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 16)]
		public struct <keys>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
