using System;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.InputSystem
{
	[InputControlLayout(stateType = typeof(KeyboardState), isGenericTypeOfDevice = true)]
	public class Keyboard : InputDevice, ITextInputReceiver, IEventPreProcessor
	{
		public event Action<char> onTextInput
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!this.m_TextInputListeners.Contains(value))
				{
					this.m_TextInputListeners.Append(value);
				}
			}
			remove
			{
				this.m_TextInputListeners.Remove(value);
			}
		}

		public event Action<IMECompositionString> onIMECompositionChange
		{
			add
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!this.m_ImeCompositionListeners.Contains(value))
				{
					this.m_ImeCompositionListeners.Append(value);
				}
			}
			remove
			{
				this.m_ImeCompositionListeners.Remove(value);
			}
		}

		public void SetIMEEnabled(bool enabled)
		{
			EnableIMECompositionCommand enableIMECompositionCommand = EnableIMECompositionCommand.Create(enabled);
			base.ExecuteCommand<EnableIMECompositionCommand>(ref enableIMECompositionCommand);
		}

		public void SetIMECursorPosition(Vector2 position)
		{
			SetIMECursorPositionCommand setIMECursorPositionCommand = SetIMECursorPositionCommand.Create(position);
			base.ExecuteCommand<SetIMECursorPositionCommand>(ref setIMECursorPositionCommand);
		}

		public string keyboardLayout
		{
			get
			{
				base.RefreshConfigurationIfNeeded();
				return this.m_KeyboardLayoutName;
			}
			protected set
			{
				this.m_KeyboardLayoutName = value;
			}
		}

		public AnyKeyControl anyKey { get; protected set; }

		public KeyControl spaceKey
		{
			get
			{
				return this[Key.Space];
			}
		}

		public KeyControl enterKey
		{
			get
			{
				return this[Key.Enter];
			}
		}

		public KeyControl tabKey
		{
			get
			{
				return this[Key.Tab];
			}
		}

		public KeyControl backquoteKey
		{
			get
			{
				return this[Key.Backquote];
			}
		}

		public KeyControl quoteKey
		{
			get
			{
				return this[Key.Quote];
			}
		}

		public KeyControl semicolonKey
		{
			get
			{
				return this[Key.Semicolon];
			}
		}

		public KeyControl commaKey
		{
			get
			{
				return this[Key.Comma];
			}
		}

		public KeyControl periodKey
		{
			get
			{
				return this[Key.Period];
			}
		}

		public KeyControl slashKey
		{
			get
			{
				return this[Key.Slash];
			}
		}

		public KeyControl backslashKey
		{
			get
			{
				return this[Key.Backslash];
			}
		}

		public KeyControl leftBracketKey
		{
			get
			{
				return this[Key.LeftBracket];
			}
		}

		public KeyControl rightBracketKey
		{
			get
			{
				return this[Key.RightBracket];
			}
		}

		public KeyControl minusKey
		{
			get
			{
				return this[Key.Minus];
			}
		}

		public KeyControl equalsKey
		{
			get
			{
				return this[Key.Equals];
			}
		}

		public KeyControl aKey
		{
			get
			{
				return this[Key.A];
			}
		}

		public KeyControl bKey
		{
			get
			{
				return this[Key.B];
			}
		}

		public KeyControl cKey
		{
			get
			{
				return this[Key.C];
			}
		}

		public KeyControl dKey
		{
			get
			{
				return this[Key.D];
			}
		}

		public KeyControl eKey
		{
			get
			{
				return this[Key.E];
			}
		}

		public KeyControl fKey
		{
			get
			{
				return this[Key.F];
			}
		}

		public KeyControl gKey
		{
			get
			{
				return this[Key.G];
			}
		}

		public KeyControl hKey
		{
			get
			{
				return this[Key.H];
			}
		}

		public KeyControl iKey
		{
			get
			{
				return this[Key.I];
			}
		}

		public KeyControl jKey
		{
			get
			{
				return this[Key.J];
			}
		}

		public KeyControl kKey
		{
			get
			{
				return this[Key.K];
			}
		}

		public KeyControl lKey
		{
			get
			{
				return this[Key.L];
			}
		}

		public KeyControl mKey
		{
			get
			{
				return this[Key.M];
			}
		}

		public KeyControl nKey
		{
			get
			{
				return this[Key.N];
			}
		}

		public KeyControl oKey
		{
			get
			{
				return this[Key.O];
			}
		}

		public KeyControl pKey
		{
			get
			{
				return this[Key.P];
			}
		}

		public KeyControl qKey
		{
			get
			{
				return this[Key.Q];
			}
		}

		public KeyControl rKey
		{
			get
			{
				return this[Key.R];
			}
		}

		public KeyControl sKey
		{
			get
			{
				return this[Key.S];
			}
		}

		public KeyControl tKey
		{
			get
			{
				return this[Key.T];
			}
		}

		public KeyControl uKey
		{
			get
			{
				return this[Key.U];
			}
		}

		public KeyControl vKey
		{
			get
			{
				return this[Key.V];
			}
		}

		public KeyControl wKey
		{
			get
			{
				return this[Key.W];
			}
		}

		public KeyControl xKey
		{
			get
			{
				return this[Key.X];
			}
		}

		public KeyControl yKey
		{
			get
			{
				return this[Key.Y];
			}
		}

		public KeyControl zKey
		{
			get
			{
				return this[Key.Z];
			}
		}

		public KeyControl digit1Key
		{
			get
			{
				return this[Key.Digit1];
			}
		}

		public KeyControl digit2Key
		{
			get
			{
				return this[Key.Digit2];
			}
		}

		public KeyControl digit3Key
		{
			get
			{
				return this[Key.Digit3];
			}
		}

		public KeyControl digit4Key
		{
			get
			{
				return this[Key.Digit4];
			}
		}

		public KeyControl digit5Key
		{
			get
			{
				return this[Key.Digit5];
			}
		}

		public KeyControl digit6Key
		{
			get
			{
				return this[Key.Digit6];
			}
		}

		public KeyControl digit7Key
		{
			get
			{
				return this[Key.Digit7];
			}
		}

		public KeyControl digit8Key
		{
			get
			{
				return this[Key.Digit8];
			}
		}

		public KeyControl digit9Key
		{
			get
			{
				return this[Key.Digit9];
			}
		}

		public KeyControl digit0Key
		{
			get
			{
				return this[Key.Digit0];
			}
		}

		public KeyControl leftShiftKey
		{
			get
			{
				return this[Key.LeftShift];
			}
		}

		public KeyControl rightShiftKey
		{
			get
			{
				return this[Key.RightShift];
			}
		}

		public KeyControl leftAltKey
		{
			get
			{
				return this[Key.LeftAlt];
			}
		}

		public KeyControl rightAltKey
		{
			get
			{
				return this[Key.RightAlt];
			}
		}

		public KeyControl leftCtrlKey
		{
			get
			{
				return this[Key.LeftCtrl];
			}
		}

		public KeyControl rightCtrlKey
		{
			get
			{
				return this[Key.RightCtrl];
			}
		}

		public KeyControl leftMetaKey
		{
			get
			{
				return this[Key.LeftMeta];
			}
		}

		public KeyControl rightMetaKey
		{
			get
			{
				return this[Key.RightMeta];
			}
		}

		public KeyControl leftWindowsKey
		{
			get
			{
				return this[Key.LeftMeta];
			}
		}

		public KeyControl rightWindowsKey
		{
			get
			{
				return this[Key.RightMeta];
			}
		}

		public KeyControl leftAppleKey
		{
			get
			{
				return this[Key.LeftMeta];
			}
		}

		public KeyControl rightAppleKey
		{
			get
			{
				return this[Key.RightMeta];
			}
		}

		public KeyControl leftCommandKey
		{
			get
			{
				return this[Key.LeftMeta];
			}
		}

		public KeyControl rightCommandKey
		{
			get
			{
				return this[Key.RightMeta];
			}
		}

		public KeyControl contextMenuKey
		{
			get
			{
				return this[Key.ContextMenu];
			}
		}

		public KeyControl escapeKey
		{
			get
			{
				return this[Key.Escape];
			}
		}

		public KeyControl leftArrowKey
		{
			get
			{
				return this[Key.LeftArrow];
			}
		}

		public KeyControl rightArrowKey
		{
			get
			{
				return this[Key.RightArrow];
			}
		}

		public KeyControl upArrowKey
		{
			get
			{
				return this[Key.UpArrow];
			}
		}

		public KeyControl downArrowKey
		{
			get
			{
				return this[Key.DownArrow];
			}
		}

		public KeyControl backspaceKey
		{
			get
			{
				return this[Key.Backspace];
			}
		}

		public KeyControl pageDownKey
		{
			get
			{
				return this[Key.PageDown];
			}
		}

		public KeyControl pageUpKey
		{
			get
			{
				return this[Key.PageUp];
			}
		}

		public KeyControl homeKey
		{
			get
			{
				return this[Key.Home];
			}
		}

		public KeyControl endKey
		{
			get
			{
				return this[Key.End];
			}
		}

		public KeyControl insertKey
		{
			get
			{
				return this[Key.Insert];
			}
		}

		public KeyControl deleteKey
		{
			get
			{
				return this[Key.Delete];
			}
		}

		public KeyControl capsLockKey
		{
			get
			{
				return this[Key.CapsLock];
			}
		}

		public KeyControl scrollLockKey
		{
			get
			{
				return this[Key.ScrollLock];
			}
		}

		public KeyControl numLockKey
		{
			get
			{
				return this[Key.NumLock];
			}
		}

		public KeyControl printScreenKey
		{
			get
			{
				return this[Key.PrintScreen];
			}
		}

		public KeyControl pauseKey
		{
			get
			{
				return this[Key.Pause];
			}
		}

		public KeyControl numpadEnterKey
		{
			get
			{
				return this[Key.NumpadEnter];
			}
		}

		public KeyControl numpadDivideKey
		{
			get
			{
				return this[Key.NumpadDivide];
			}
		}

		public KeyControl numpadMultiplyKey
		{
			get
			{
				return this[Key.NumpadMultiply];
			}
		}

		public KeyControl numpadMinusKey
		{
			get
			{
				return this[Key.NumpadMinus];
			}
		}

		public KeyControl numpadPlusKey
		{
			get
			{
				return this[Key.NumpadPlus];
			}
		}

		public KeyControl numpadPeriodKey
		{
			get
			{
				return this[Key.NumpadPeriod];
			}
		}

		public KeyControl numpadEqualsKey
		{
			get
			{
				return this[Key.NumpadEquals];
			}
		}

		public KeyControl numpad0Key
		{
			get
			{
				return this[Key.Numpad0];
			}
		}

		public KeyControl numpad1Key
		{
			get
			{
				return this[Key.Numpad1];
			}
		}

		public KeyControl numpad2Key
		{
			get
			{
				return this[Key.Numpad2];
			}
		}

		public KeyControl numpad3Key
		{
			get
			{
				return this[Key.Numpad3];
			}
		}

		public KeyControl numpad4Key
		{
			get
			{
				return this[Key.Numpad4];
			}
		}

		public KeyControl numpad5Key
		{
			get
			{
				return this[Key.Numpad5];
			}
		}

		public KeyControl numpad6Key
		{
			get
			{
				return this[Key.Numpad6];
			}
		}

		public KeyControl numpad7Key
		{
			get
			{
				return this[Key.Numpad7];
			}
		}

		public KeyControl numpad8Key
		{
			get
			{
				return this[Key.Numpad8];
			}
		}

		public KeyControl numpad9Key
		{
			get
			{
				return this[Key.Numpad9];
			}
		}

		public KeyControl f1Key
		{
			get
			{
				return this[Key.F1];
			}
		}

		public KeyControl f2Key
		{
			get
			{
				return this[Key.F2];
			}
		}

		public KeyControl f3Key
		{
			get
			{
				return this[Key.F3];
			}
		}

		public KeyControl f4Key
		{
			get
			{
				return this[Key.F4];
			}
		}

		public KeyControl f5Key
		{
			get
			{
				return this[Key.F5];
			}
		}

		public KeyControl f6Key
		{
			get
			{
				return this[Key.F6];
			}
		}

		public KeyControl f7Key
		{
			get
			{
				return this[Key.F7];
			}
		}

		public KeyControl f8Key
		{
			get
			{
				return this[Key.F8];
			}
		}

		public KeyControl f9Key
		{
			get
			{
				return this[Key.F9];
			}
		}

		public KeyControl f10Key
		{
			get
			{
				return this[Key.F10];
			}
		}

		public KeyControl f11Key
		{
			get
			{
				return this[Key.F11];
			}
		}

		public KeyControl f12Key
		{
			get
			{
				return this[Key.F12];
			}
		}

		public KeyControl oem1Key
		{
			get
			{
				return this[Key.OEM1];
			}
		}

		public KeyControl oem2Key
		{
			get
			{
				return this[Key.OEM2];
			}
		}

		public KeyControl oem3Key
		{
			get
			{
				return this[Key.OEM3];
			}
		}

		public KeyControl oem4Key
		{
			get
			{
				return this[Key.OEM4];
			}
		}

		public KeyControl oem5Key
		{
			get
			{
				return this[Key.OEM5];
			}
		}

		public KeyControl f13Key
		{
			get
			{
				return this[Key.F13];
			}
		}

		public KeyControl f14Key
		{
			get
			{
				return this[Key.F14];
			}
		}

		public KeyControl f15Key
		{
			get
			{
				return this[Key.F15];
			}
		}

		public KeyControl f16Key
		{
			get
			{
				return this[Key.F16];
			}
		}

		public KeyControl f17Key
		{
			get
			{
				return this[Key.F17];
			}
		}

		public KeyControl f18Key
		{
			get
			{
				return this[Key.F18];
			}
		}

		public KeyControl f19Key
		{
			get
			{
				return this[Key.F19];
			}
		}

		public KeyControl f20Key
		{
			get
			{
				return this[Key.F20];
			}
		}

		public KeyControl f21Key
		{
			get
			{
				return this[Key.F21];
			}
		}

		public KeyControl f22Key
		{
			get
			{
				return this[Key.F22];
			}
		}

		public KeyControl f23Key
		{
			get
			{
				return this[Key.F23];
			}
		}

		public KeyControl f24Key
		{
			get
			{
				return this[Key.F24];
			}
		}

		public ButtonControl shiftKey { get; protected set; }

		public ButtonControl ctrlKey { get; protected set; }

		public ButtonControl altKey { get; protected set; }

		public ButtonControl imeSelected { get; protected set; }

		public KeyControl this[Key key]
		{
			get
			{
				int num = key - Key.Space;
				if (num < 0 || num >= this.m_Keys.Length)
				{
					throw new ArgumentOutOfRangeException("key");
				}
				return this.m_Keys[num];
			}
		}

		public ReadOnlyArray<KeyControl> allKeys
		{
			get
			{
				return new ReadOnlyArray<KeyControl>(this.m_Keys);
			}
		}

		public static Keyboard current { get; private set; }

		public override void MakeCurrent()
		{
			base.MakeCurrent();
			Keyboard.current = this;
		}

		protected override void OnRemoved()
		{
			base.OnRemoved();
			if (Keyboard.current == this)
			{
				Keyboard.current = null;
			}
		}

		protected override void FinishSetup()
		{
			string[] array = new string[]
			{
				"space",
				"enter",
				"tab",
				"backquote",
				"quote",
				"semicolon",
				"comma",
				"period",
				"slash",
				"backslash",
				"leftbracket",
				"rightbracket",
				"minus",
				"equals",
				"a",
				"b",
				"c",
				"d",
				"e",
				"f",
				"g",
				"h",
				"i",
				"j",
				"k",
				"l",
				"m",
				"n",
				"o",
				"p",
				"q",
				"r",
				"s",
				"t",
				"u",
				"v",
				"w",
				"x",
				"y",
				"z",
				"1",
				"2",
				"3",
				"4",
				"5",
				"6",
				"7",
				"8",
				"9",
				"0",
				"leftshift",
				"rightshift",
				"leftalt",
				"rightalt",
				"leftctrl",
				"rightctrl",
				"leftmeta",
				"rightmeta",
				"contextmenu",
				"escape",
				"leftarrow",
				"rightarrow",
				"uparrow",
				"downarrow",
				"backspace",
				"pagedown",
				"pageup",
				"home",
				"end",
				"insert",
				"delete",
				"capslock",
				"numlock",
				"printscreen",
				"scrolllock",
				"pause",
				"numpadenter",
				"numpaddivide",
				"numpadmultiply",
				"numpadplus",
				"numpadminus",
				"numpadperiod",
				"numpadequals",
				"numpad0",
				"numpad1",
				"numpad2",
				"numpad3",
				"numpad4",
				"numpad5",
				"numpad6",
				"numpad7",
				"numpad8",
				"numpad9",
				"f1",
				"f2",
				"f3",
				"f4",
				"f5",
				"f6",
				"f7",
				"f8",
				"f9",
				"f10",
				"f11",
				"f12",
				"oem1",
				"oem2",
				"oem3",
				"oem4",
				"oem5",
				null,
				"f13",
				"f14",
				"f15",
				"f16",
				"f17",
				"f18",
				"f19",
				"f20",
				"f21",
				"f22",
				"f23",
				"f24"
			};
			this.m_Keys = new KeyControl[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrEmpty(array[i]))
				{
					this.m_Keys[i] = base.GetChildControl<KeyControl>(array[i]);
					this.m_Keys[i].keyCode = i + Key.Space;
				}
			}
			this.anyKey = base.GetChildControl<AnyKeyControl>("anyKey");
			this.shiftKey = base.GetChildControl<ButtonControl>("shift");
			this.ctrlKey = base.GetChildControl<ButtonControl>("ctrl");
			this.altKey = base.GetChildControl<ButtonControl>("alt");
			this.imeSelected = base.GetChildControl<ButtonControl>("IMESelected");
			base.FinishSetup();
		}

		protected override void RefreshConfiguration()
		{
			this.keyboardLayout = null;
			QueryKeyboardLayoutCommand queryKeyboardLayoutCommand = QueryKeyboardLayoutCommand.Create();
			if (base.ExecuteCommand<QueryKeyboardLayoutCommand>(ref queryKeyboardLayoutCommand) >= 0L)
			{
				this.keyboardLayout = queryKeyboardLayoutCommand.ReadLayoutName();
			}
		}

		public void OnTextInput(char character)
		{
			for (int i = 0; i < this.m_TextInputListeners.length; i++)
			{
				this.m_TextInputListeners[i](character);
			}
		}

		public KeyControl FindKeyOnCurrentKeyboardLayout(string displayName)
		{
			ReadOnlyArray<KeyControl> allKeys = this.allKeys;
			for (int i = 0; i < allKeys.Count; i++)
			{
				if (string.Equals(allKeys[i].displayName, displayName, StringComparison.CurrentCultureIgnoreCase))
				{
					return allKeys[i];
				}
			}
			return null;
		}

		public void OnIMECompositionChanged(IMECompositionString compositionString)
		{
			if (this.m_ImeCompositionListeners.length > 0)
			{
				for (int i = 0; i < this.m_ImeCompositionListeners.length; i++)
				{
					this.m_ImeCompositionListeners[i](compositionString);
				}
			}
		}

		unsafe bool IEventPreProcessor.PreProcessEvent(InputEventPtr currentEventPtr)
		{
			if (currentEventPtr.type == 1398030676)
			{
				StateEvent* ptr = StateEvent.FromUnchecked(currentEventPtr);
				if (ptr->stateFormat == KeyboardState.Format)
				{
					KeyboardState* ptr2 = (KeyboardState*)(&ptr->stateData.FixedElementField);
					if (ptr2->Get(Key.IMESelected))
					{
						ptr2->Set(Key.IMESelected, false);
						ptr2->Set((Key)127, true);
					}
				}
			}
			return true;
		}

		protected KeyControl[] keys
		{
			get
			{
				return this.m_Keys;
			}
			set
			{
				this.m_Keys = value;
			}
		}

		public const int KeyCount = 110;

		internal const int ExtendedKeyCount = 123;

		private InlinedArray<Action<char>> m_TextInputListeners;

		private string m_KeyboardLayoutName;

		private KeyControl[] m_Keys;

		private InlinedArray<Action<IMECompositionString>> m_ImeCompositionListeners;
	}
}
