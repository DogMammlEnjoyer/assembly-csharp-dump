using System;
using System.Collections.Generic;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal class InputEventPartialProvider : IEventProviderImpl
	{
		public void Initialize()
		{
			this._operatingSystemFamily = SystemInfo.operatingSystemFamily;
			this._keyboardButtonsState.Reset();
			this._eventModifiers.Reset();
		}

		public void Shutdown()
		{
		}

		public void Update()
		{
			int eventCount = Event.GetEventCount();
			for (int i = 0; i < eventCount; i++)
			{
				Event.GetEventAtIndex(i, this._ev);
				this.UpdateEventModifiers(this._ev);
				EventType type = this._ev.type;
				EventType eventType = type;
				if (eventType - EventType.KeyDown > 1)
				{
					if (eventType - EventType.ValidateCommand <= 1)
					{
						Event @event = Event.From(this.ToCommandEvent(this._ev));
						EventProvider.Dispatch(@event);
					}
				}
				else
				{
					bool flag = this._ev.keyCode > KeyCode.None;
					if (flag)
					{
						Event @event = Event.From(this.ToKeyEvent(this._ev));
						EventProvider.Dispatch(@event);
						bool sendNavigationEventOnTabKey = this._sendNavigationEventOnTabKey;
						if (sendNavigationEventOnTabKey)
						{
							this.SendNextOrPreviousNavigationEventOnTabKeyDownEvent(this._ev);
						}
					}
					bool flag2 = this._ev.character != '\0' && this._ev.type == EventType.KeyDown;
					if (flag2)
					{
						Event @event = Event.From(this.ToTextInputEvent(this._ev));
						EventProvider.Dispatch(@event);
					}
				}
			}
		}

		public void OnFocusChanged(bool focus)
		{
			bool flag = !focus;
			if (flag)
			{
				this._eventModifiers.Reset();
				this._keyboardButtonsState.Reset();
			}
		}

		public bool RequestCurrentState(Event.Type type)
		{
			bool result;
			if (type != Event.Type.KeyEvent)
			{
				result = false;
			}
			else
			{
				Event @event = Event.From(new KeyEvent
				{
					type = KeyEvent.Type.State,
					keyCode = KeyCode.None,
					buttonsState = this._keyboardButtonsState,
					timestamp = (DiscreteTime)Time.timeAsRational,
					eventSource = EventSource.Keyboard,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
				result = true;
			}
			return result;
		}

		public uint playerCount
		{
			get
			{
				return 0U;
			}
		}

		private DiscreteTime GetTimestamp(in Event ev)
		{
			return (DiscreteTime)Time.timeAsRational;
		}

		private void UpdateEventModifiers(in Event ev)
		{
			this._eventModifiers.SetPressed(EventModifiers.Modifiers.CapsLock, ev.capsLock);
			this._eventModifiers.SetPressed(EventModifiers.Modifiers.FunctionKey, ev.functionKey);
			this._eventModifiers.SetPressed(EventModifiers.Modifiers.Numeric, ev.numeric);
			bool flag = ev.isKey && ev.keyCode > KeyCode.None;
			if (flag)
			{
				bool pressed = ev.type == EventType.KeyDown;
				switch (ev.keyCode)
				{
				case KeyCode.Numlock:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.Numlock, pressed);
					break;
				case KeyCode.RightShift:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.RightShift, pressed);
					break;
				case KeyCode.LeftShift:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.LeftShift, pressed);
					break;
				case KeyCode.RightControl:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.RightCtrl, pressed);
					break;
				case KeyCode.LeftControl:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.LeftCtrl, pressed);
					break;
				case KeyCode.RightAlt:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.RightAlt, pressed);
					break;
				case KeyCode.LeftAlt:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.LeftAlt, pressed);
					break;
				case KeyCode.RightMeta:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.RightMeta, pressed);
					break;
				case KeyCode.LeftMeta:
					this._eventModifiers.SetPressed(EventModifiers.Modifiers.LeftMeta, pressed);
					break;
				}
			}
			bool flag2 = ev.shift != this._eventModifiers.IsPressed(EventModifiers.Modifiers.Shift);
			if (flag2)
			{
				this._eventModifiers.SetPressed(EventModifiers.Modifiers.Shift, ev.shift);
			}
			bool flag3 = ev.control != this._eventModifiers.IsPressed(EventModifiers.Modifiers.Ctrl);
			if (flag3)
			{
				this._eventModifiers.SetPressed(EventModifiers.Modifiers.Ctrl, ev.control);
			}
			bool flag4 = ev.alt != this._eventModifiers.IsPressed(EventModifiers.Modifiers.Alt);
			if (flag4)
			{
				this._eventModifiers.SetPressed(EventModifiers.Modifiers.Alt, ev.alt);
			}
			bool flag5 = ev.command != this._eventModifiers.IsPressed(EventModifiers.Modifiers.Meta);
			if (flag5)
			{
				this._eventModifiers.SetPressed(EventModifiers.Modifiers.Meta, ev.command);
			}
		}

		private KeyEvent ToKeyEvent(in Event ev)
		{
			bool flag = this._keyboardButtonsState.IsPressed(ev.keyCode);
			bool flag2 = ev.type == EventType.KeyDown;
			this._keyboardButtonsState.SetPressed(ev.keyCode, flag2);
			return new KeyEvent
			{
				type = (flag2 ? (flag ? KeyEvent.Type.KeyRepeated : KeyEvent.Type.KeyPressed) : KeyEvent.Type.KeyReleased),
				keyCode = ev.keyCode,
				buttonsState = this._keyboardButtonsState,
				timestamp = this.GetTimestamp(ev),
				eventSource = EventSource.Keyboard,
				playerId = 0U,
				eventModifiers = this._eventModifiers
			};
		}

		private TextInputEvent ToTextInputEvent(in Event ev)
		{
			return new TextInputEvent
			{
				character = ev.character,
				timestamp = this.GetTimestamp(ev),
				eventSource = EventSource.Keyboard,
				playerId = 0U,
				eventModifiers = this._eventModifiers
			};
		}

		private void SendNextOrPreviousNavigationEventOnTabKeyDownEvent(in Event ev)
		{
			bool flag = this._ev.type == EventType.KeyDown && this._ev.keyCode == KeyCode.Tab;
			if (flag)
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Move,
					direction = (this._ev.shift ? NavigationEvent.Direction.Previous : NavigationEvent.Direction.Next),
					timestamp = this.GetTimestamp(this._ev),
					eventSource = EventSource.Keyboard,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
			}
		}

		private CommandEvent ToCommandEvent(in Event ev)
		{
			CommandEvent.Command command;
			bool flag = !this._IMGUICommandToInputForUICommandType.TryGetValue(ev.commandName, out command);
			if (flag)
			{
				Debug.LogWarning("Unsupported command name '" + ev.commandName + "'");
			}
			return new CommandEvent
			{
				type = ((ev.type == EventType.ValidateCommand) ? CommandEvent.Type.Validate : CommandEvent.Type.Execute),
				command = command,
				timestamp = this.GetTimestamp(ev),
				eventSource = EventSource.Unspecified,
				playerId = 0U,
				eventModifiers = this._eventModifiers
			};
		}

		private const int kDefaultPlayerId = 0;

		private Event _ev = new Event();

		private OperatingSystemFamily _operatingSystemFamily;

		private KeyEvent.ButtonsState _keyboardButtonsState;

		internal EventModifiers _eventModifiers;

		internal bool _sendNavigationEventOnTabKey;

		private IDictionary<string, CommandEvent.Command> _IMGUICommandToInputForUICommandType = new Dictionary<string, CommandEvent.Command>
		{
			{
				"Cut",
				CommandEvent.Command.Cut
			},
			{
				"Copy",
				CommandEvent.Command.Copy
			},
			{
				"Paste",
				CommandEvent.Command.Paste
			},
			{
				"SelectAll",
				CommandEvent.Command.SelectAll
			},
			{
				"DeselectAll",
				CommandEvent.Command.DeselectAll
			},
			{
				"InvertSelection",
				CommandEvent.Command.InvertSelection
			},
			{
				"Duplicate",
				CommandEvent.Command.Duplicate
			},
			{
				"Rename",
				CommandEvent.Command.Rename
			},
			{
				"Delete",
				CommandEvent.Command.Delete
			},
			{
				"SoftDelete",
				CommandEvent.Command.SoftDelete
			},
			{
				"Find",
				CommandEvent.Command.Find
			},
			{
				"SelectChildren",
				CommandEvent.Command.SelectChildren
			},
			{
				"SelectPrefabRoot",
				CommandEvent.Command.SelectPrefabRoot
			},
			{
				"UndoRedoPerformed",
				CommandEvent.Command.UndoRedoPerformed
			},
			{
				"OnLostFocus",
				CommandEvent.Command.OnLostFocus
			},
			{
				"NewKeyboardFocus",
				CommandEvent.Command.NewKeyboardFocus
			},
			{
				"ModifierKeysChanged",
				CommandEvent.Command.ModifierKeysChanged
			},
			{
				"EyeDropperUpdate",
				CommandEvent.Command.EyeDropperUpdate
			},
			{
				"EyeDropperClicked",
				CommandEvent.Command.EyeDropperClicked
			},
			{
				"EyeDropperCancelled",
				CommandEvent.Command.EyeDropperCancelled
			},
			{
				"ColorPickerChanged",
				CommandEvent.Command.ColorPickerChanged
			},
			{
				"FrameSelected",
				CommandEvent.Command.FrameSelected
			},
			{
				"FrameSelectedWithLock",
				CommandEvent.Command.FrameSelectedWithLock
			}
		};
	}
}
