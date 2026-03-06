using System;
using System.Collections;
using System.Collections.Generic;
using Unity.IntegerTime;

namespace UnityEngine.InputForUI
{
	internal class InputManagerProvider : IEventProviderImpl
	{
		private EventModifiers _eventModifiers
		{
			get
			{
				return this._inputEventPartialProvider._eventModifiers;
			}
		}

		public InputManagerProvider()
		{
		}

		internal InputManagerProvider(InputManagerProvider.IInput inputOverride, InputManagerProvider.ITime timeOverride)
		{
			this._input = inputOverride;
			this._time = timeOverride;
		}

		public void Initialize()
		{
			if (this._inputEventPartialProvider == null)
			{
				this._inputEventPartialProvider = new InputEventPartialProvider();
			}
			this._inputEventPartialProvider.Initialize();
			this._inputEventPartialProvider._sendNavigationEventOnTabKey = true;
			this._mouseState.Reset();
			this._isPenPresent = false;
			this._seenAtLeastOnePenPosition = false;
			this._lastSeenPenPositionForDetection = default(Vector2);
			this._penState.Reset();
			this._lastPenData = default(PenData);
			this._touchFingerIdToFingerIndex.Clear();
			this._touchNextFingerIndex = 0;
			this._touchState.Reset();
		}

		public void Shutdown()
		{
		}

		public void Update()
		{
			this._inputEventPartialProvider.Update();
			DiscreteTime discreteTime = (DiscreteTime)this._time.timeAsRational;
			this.DetectPen();
			bool flag = false;
			bool touchSupported = this._input.touchSupported;
			if (touchSupported)
			{
				flag = this.CheckTouchEvents(discreteTime);
			}
			bool flag2 = false;
			bool flag3 = !flag && this._isPenPresent;
			if (flag3)
			{
				DiscreteTime currentTime = discreteTime;
				PenData lastPenContactEvent = this._input.GetLastPenContactEvent();
				flag2 = this.CheckPenEvent(currentTime, lastPenContactEvent);
			}
			else
			{
				this._penState.Reset();
			}
			bool flag4 = !flag2 && !flag && this._input.mousePresent;
			if (flag4)
			{
				this.CheckMouseEvents(discreteTime, false);
			}
			else
			{
				this.CheckMouseEvents(discreteTime, true);
				this._mouseState.LastPositionValid = false;
			}
			bool mousePresent = this._input.mousePresent;
			if (mousePresent)
			{
				this.CheckMouseScroll(discreteTime);
			}
			this.CheckIfIMEChanged(discreteTime);
			this.DirectionNavigation(discreteTime);
			this.SubmitCancelNavigation(discreteTime);
			this.NextPreviousNavigation(discreteTime);
		}

		private bool CheckTouchEvents(DiscreteTime currentTime)
		{
			bool flag = true;
			bool result = false;
			for (int i = 0; i < this._input.touchCount; i++)
			{
				Touch touch = this._input.GetTouch(i);
				bool flag2 = touch.type == TouchType.Indirect || touch.phase == TouchPhase.Stationary;
				if (!flag2)
				{
					int num;
					bool flag3 = !this._touchFingerIdToFingerIndex.TryGetValue(touch.fingerId, out num);
					if (flag3)
					{
						int touchNextFingerIndex = this._touchNextFingerIndex;
						this._touchNextFingerIndex = touchNextFingerIndex + 1;
						num = touchNextFingerIndex;
						this._touchFingerIdToFingerIndex.Add(touch.fingerId, num);
					}
					int displayIndex;
					Vector2 position = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(touch.position, out displayIndex);
					Vector2 deltaPosition = InputManagerProvider.ScreenBottomLeftToPanelDelta(touch.deltaPosition);
					PointerEvent.Type type = PointerEvent.Type.PointerMoved;
					PointerEvent.Button button = PointerEvent.Button.None;
					switch (touch.phase)
					{
					case TouchPhase.Began:
						type = PointerEvent.Type.ButtonPressed;
						button = PointerEvent.Button.Primary;
						flag = false;
						this._touchState.OnButtonDown(currentTime, button);
						break;
					case TouchPhase.Moved:
						flag = false;
						break;
					case TouchPhase.Ended:
						type = PointerEvent.Type.ButtonReleased;
						button = PointerEvent.Button.Primary;
						this._touchState.OnButtonUp(currentTime, button);
						break;
					case TouchPhase.Canceled:
						type = PointerEvent.Type.TouchCanceled;
						button = PointerEvent.Button.Primary;
						this._touchState.OnButtonUp(currentTime, button);
						break;
					}
					Event @event = Event.From(new PointerEvent
					{
						type = type,
						pointerIndex = num,
						position = position,
						deltaPosition = deltaPosition,
						scroll = Vector2.zero,
						displayIndex = displayIndex,
						tilt = InputManagerProvider.AzimuthAndAlitutudeToTilt(touch.altitudeAngle, touch.azimuthAngle),
						twist = 0f,
						pressure = ((Mathf.Abs(touch.maximumPossiblePressure) > Mathf.Epsilon) ? (touch.pressure / touch.maximumPossiblePressure) : 1f),
						isInverted = false,
						button = button,
						buttonsState = this._touchState.ButtonsState,
						clickCount = this._touchState.ClickCount,
						timestamp = currentTime,
						eventSource = EventSource.Touch,
						playerId = 0U,
						eventModifiers = this._eventModifiers
					});
					EventProvider.Dispatch(@event);
					result = true;
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				this._touchNextFingerIndex = 0;
				this._touchFingerIdToFingerIndex.Clear();
			}
			return result;
		}

		private void DetectPen()
		{
			bool isPenPresent = this._isPenPresent;
			if (!isPenPresent)
			{
				Vector2 position = this._input.GetLastPenContactEvent().position;
				bool seenAtLeastOnePenPosition = this._seenAtLeastOnePenPosition;
				if (seenAtLeastOnePenPosition)
				{
					float sqrMagnitude = (position - this._lastSeenPenPositionForDetection).sqrMagnitude;
					this._isPenPresent = (sqrMagnitude >= 0.01f);
				}
				else
				{
					this._lastSeenPenPositionForDetection = position;
					this._seenAtLeastOnePenPosition = true;
				}
			}
		}

		private static PointerEvent.Button PenStatusToButton(PenStatus status)
		{
			bool flag = (status & PenStatus.Eraser) > PenStatus.None;
			PointerEvent.Button result;
			if (flag)
			{
				result = PointerEvent.Button.PenEraserInTouch;
			}
			else
			{
				bool flag2 = (status & PenStatus.Barrel) > PenStatus.None;
				if (flag2)
				{
					result = PointerEvent.Button.PenBarrelButton;
				}
				else
				{
					result = PointerEvent.Button.Primary;
				}
			}
			return result;
		}

		private bool CheckPenEvent(DiscreteTime currentTime, in PenData currentPenData)
		{
			Vector2 position = currentPenData.position;
			int displayIndex = 0;
			Vector2 deltaPosition = this._penState.LastPositionValid ? (position - this._penState.LastPosition) : Vector2.zero;
			PointerEvent.Button button = PointerEvent.Button.None;
			bool flag = currentPenData.contactType != this._lastPenData.contactType;
			PointerEvent.Type type;
			if (flag)
			{
				PenEventType contactType = currentPenData.contactType;
				PenEventType penEventType = contactType;
				if (penEventType != PenEventType.PenDown)
				{
					if (penEventType != PenEventType.PenUp)
					{
						type = PointerEvent.Type.PointerMoved;
					}
					else
					{
						type = PointerEvent.Type.ButtonReleased;
						button = InputManagerProvider.PenStatusToButton(this._lastPenData.penStatus);
						this._penState.OnButtonUp(currentTime, button);
					}
				}
				else
				{
					type = PointerEvent.Type.ButtonPressed;
					button = InputManagerProvider.PenStatusToButton(currentPenData.penStatus);
					this._penState.OnButtonDown(currentTime, button);
				}
			}
			else
			{
				type = PointerEvent.Type.PointerMoved;
			}
			this._lastPenData = currentPenData;
			bool result = false;
			bool flag2 = type != PointerEvent.Type.PointerMoved || !this._penState.LastPositionValid || deltaPosition.sqrMagnitude >= 0.01f;
			if (flag2)
			{
				Event @event = Event.From(new PointerEvent
				{
					type = type,
					pointerIndex = 0,
					position = position,
					deltaPosition = deltaPosition,
					scroll = Vector2.zero,
					displayIndex = displayIndex,
					tilt = currentPenData.tilt,
					twist = currentPenData.twist,
					pressure = currentPenData.pressure,
					isInverted = ((currentPenData.penStatus & PenStatus.Inverted) > PenStatus.None),
					button = button,
					buttonsState = this._penState.ButtonsState,
					clickCount = this._penState.ClickCount,
					timestamp = currentTime,
					eventSource = EventSource.Pen,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
				result = true;
			}
			this._penState.OnMove(currentTime, position, displayIndex);
			return result;
		}

		private void CheckMouseEvents(DiscreteTime currentTime, bool muted = false)
		{
			int displayIndex;
			Vector2 vector = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(this._input.mousePosition, out displayIndex);
			bool lastPositionValid = this._mouseState.LastPositionValid;
			if (lastPositionValid)
			{
				Vector2 deltaPosition = vector - this._mouseState.LastPosition;
				bool flag = deltaPosition.sqrMagnitude >= 0.01f;
				if (flag)
				{
					bool flag2 = !muted;
					if (flag2)
					{
						PointerEvent pointerEvent = new PointerEvent
						{
							type = PointerEvent.Type.PointerMoved,
							pointerIndex = 0,
							position = vector,
							deltaPosition = deltaPosition,
							scroll = Vector2.zero,
							displayIndex = displayIndex,
							tilt = Vector2.zero,
							twist = 0f,
							pressure = 0f,
							isInverted = false,
							button = PointerEvent.Button.None,
							buttonsState = this._mouseState.ButtonsState,
							clickCount = 0,
							timestamp = currentTime,
							eventSource = EventSource.Mouse,
							playerId = 0U,
							eventModifiers = this._eventModifiers
						};
						Event @event = Event.From(pointerEvent);
						EventProvider.Dispatch(@event);
					}
					this._mouseState.OnMove(currentTime, vector, displayIndex);
				}
			}
			else
			{
				this._mouseState.OnMove(currentTime, vector, displayIndex);
			}
			for (int i = 0; i < 5; i++)
			{
				PointerEvent.Button button = PointerEvent.ButtonFromButtonIndex(i);
				bool flag3 = this._mouseState.ButtonsState.Get(button);
				bool mouseButtonDown = this._input.GetMouseButtonDown(i);
				bool mouseButtonUp = this._input.GetMouseButtonUp(i);
				bool mouseButton = this._input.GetMouseButton(i);
				InputManagerProvider.ButtonEventsIterator buttonEventsIterator = InputManagerProvider.ButtonEventsIterator.FromState(flag3, mouseButtonDown, mouseButtonUp, mouseButton);
				bool previousState = flag3;
				while (buttonEventsIterator.MoveNext())
				{
					bool newState = buttonEventsIterator.Current;
					this._mouseState.OnButtonChange(currentTime, button, previousState, newState);
					previousState = buttonEventsIterator.Current;
					bool flag4 = !muted;
					if (flag4)
					{
						PointerEvent pointerEvent = new PointerEvent
						{
							type = (buttonEventsIterator.Current ? PointerEvent.Type.ButtonPressed : PointerEvent.Type.ButtonReleased),
							pointerIndex = 0,
							position = this._mouseState.LastPosition,
							deltaPosition = Vector2.zero,
							scroll = Vector2.zero,
							displayIndex = this._mouseState.LastDisplayIndex,
							tilt = Vector2.zero,
							twist = 0f,
							pressure = 0f,
							isInverted = false,
							button = button,
							buttonsState = this._mouseState.ButtonsState,
							clickCount = this._mouseState.ClickCount,
							timestamp = currentTime,
							eventSource = EventSource.Mouse,
							playerId = 0U,
							eventModifiers = this._eventModifiers
						};
						Event @event = Event.From(pointerEvent);
						EventProvider.Dispatch(@event);
					}
				}
			}
		}

		private void CheckMouseScroll(DiscreteTime currentTime)
		{
			Vector2 mouseScrollDelta = this._input.mouseScrollDelta;
			bool flag = mouseScrollDelta.sqrMagnitude < 0.01f;
			if (!flag)
			{
				int displayIndex = 0;
				bool lastPositionValid = this._mouseState.LastPositionValid;
				Vector2 position;
				if (lastPositionValid)
				{
					position = this._mouseState.LastPosition;
					displayIndex = this._mouseState.LastDisplayIndex;
				}
				else
				{
					position = InputManagerProvider.MultiDisplayBottomLeftToPanelPosition(this._input.mousePosition, out displayIndex);
				}
				mouseScrollDelta.x *= 3f;
				mouseScrollDelta.y *= -3f;
				Event @event = Event.From(new PointerEvent
				{
					type = PointerEvent.Type.Scroll,
					pointerIndex = 0,
					position = position,
					deltaPosition = Vector2.zero,
					scroll = mouseScrollDelta,
					displayIndex = displayIndex,
					tilt = Vector2.zero,
					twist = 0f,
					pressure = 0f,
					isInverted = false,
					button = PointerEvent.Button.None,
					buttonsState = this._mouseState.ButtonsState,
					clickCount = 0,
					timestamp = currentTime,
					eventSource = EventSource.Mouse,
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
			}
		}

		private PointerEvent ToPointerStateEvent(DiscreteTime currentTime, in PointerState state, EventSource eventSource)
		{
			PointerEvent result = default(PointerEvent);
			result.type = PointerEvent.Type.State;
			result.pointerIndex = 0;
			result.position = state.LastPosition;
			result.deltaPosition = Vector2.zero;
			result.scroll = Vector2.zero;
			result.displayIndex = state.LastDisplayIndex;
			result.tilt = ((eventSource == EventSource.Pen) ? this._lastPenData.tilt : Vector2.zero);
			result.twist = ((eventSource == EventSource.Pen) ? this._lastPenData.twist : 0f);
			result.pressure = ((eventSource == EventSource.Pen) ? this._lastPenData.pressure : 0f);
			result.isInverted = (eventSource == EventSource.Pen && (this._lastPenData.penStatus & PenStatus.Inverted) > PenStatus.None);
			result.button = PointerEvent.Button.None;
			PointerState pointerState = state;
			result.buttonsState = pointerState.ButtonsState;
			result.clickCount = 0;
			result.timestamp = currentTime;
			result.eventSource = eventSource;
			result.playerId = 0U;
			result.eventModifiers = this._eventModifiers;
			return result;
		}

		private void NextPreviousNavigation(DiscreteTime currentTime)
		{
			int num = (this.InputManagerGetButtonDownOrDefault(this._configuration.NavigateNextButton) ? 1 : 0) + (this.InputManagerGetButtonDownOrDefault(this._configuration.NavigatePreviousButton) ? -1 : 0);
			bool flag = num != 0;
			if (flag)
			{
				bool isShiftPressed = this._eventModifiers.isShiftPressed;
				if (isShiftPressed)
				{
					num = -num;
				}
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Move,
					direction = ((num >= 0) ? NavigationEvent.Direction.Next : NavigationEvent.Direction.Previous),
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
			}
		}

		private void SubmitCancelNavigation(DiscreteTime currentTime)
		{
			bool flag = this.InputManagerGetButtonDownOrDefault(this._configuration.SubmitButton);
			if (flag)
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Submit,
					direction = NavigationEvent.Direction.None,
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
			}
			bool flag2 = this.InputManagerGetButtonDownOrDefault(this._configuration.CancelButton);
			if (flag2)
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Cancel,
					direction = NavigationEvent.Direction.None,
					timestamp = currentTime,
					eventSource = this.GetEventSourceFromPressedKey(),
					playerId = 0U,
					eventModifiers = this._eventModifiers
				});
				EventProvider.Dispatch(@event);
			}
		}

		private void DirectionNavigation(DiscreteTime currentTime)
		{
			ValueTuple<Vector2, bool> valueTuple = this.ReadCurrentNavigationMoveVector();
			Vector2 item = valueTuple.Item1;
			bool item2 = valueTuple.Item2;
			NavigationEvent.Direction direction = NavigationEvent.DetermineMoveDirection(item, 0.6f);
			bool flag = direction == NavigationEvent.Direction.None;
			if (flag)
			{
				this._navigationEventRepeatHelper.Reset();
			}
			else
			{
				bool flag2 = this._navigationEventRepeatHelper.ShouldSendMoveEvent(currentTime, direction, item2);
				if (flag2)
				{
					EventSource eventSource = this.GetEventSourceFromPressedKey();
					bool flag3 = eventSource == EventSource.Unspecified && !item2;
					if (flag3)
					{
						eventSource = EventSource.Gamepad;
					}
					Event @event = Event.From(new NavigationEvent
					{
						type = NavigationEvent.Type.Move,
						direction = direction,
						timestamp = currentTime,
						eventSource = eventSource,
						playerId = 0U,
						eventModifiers = this._eventModifiers
					});
					EventProvider.Dispatch(@event);
				}
			}
		}

		private void CheckIfIMEChanged(DiscreteTime currentTime)
		{
			string compositionString = this._input.compositionString;
			bool flag = this._compositionString != compositionString;
			if (flag)
			{
				this._compositionString = compositionString;
				Event @event = Event.From(this.ToIMECompositionEvent(currentTime, this._compositionString));
				EventProvider.Dispatch(@event);
			}
		}

		public void OnFocusChanged(bool focus)
		{
			this._inputEventPartialProvider.OnFocusChanged(focus);
		}

		public bool RequestCurrentState(Event.Type type)
		{
			bool flag = this._inputEventPartialProvider.RequestCurrentState(type);
			bool result;
			if (flag)
			{
				result = true;
			}
			else
			{
				DiscreteTime currentTime = (DiscreteTime)this._time.timeAsRational;
				if (type != Event.Type.PointerEvent)
				{
					if (type != Event.Type.IMECompositionEvent)
					{
						result = false;
					}
					else
					{
						Event @event = Event.From(this.ToIMECompositionEvent(currentTime, this._compositionString));
						EventProvider.Dispatch(@event);
						result = true;
					}
				}
				else
				{
					bool lastPositionValid = this._touchState.LastPositionValid;
					if (lastPositionValid)
					{
						Event @event = Event.From(this.ToPointerStateEvent(currentTime, this._touchState, EventSource.Touch));
						EventProvider.Dispatch(@event);
					}
					bool lastPositionValid2 = this._penState.LastPositionValid;
					if (lastPositionValid2)
					{
						Event @event = Event.From(this.ToPointerStateEvent(currentTime, this._penState, EventSource.Pen));
						EventProvider.Dispatch(@event);
					}
					bool lastPositionValid3 = this._mouseState.LastPositionValid;
					if (lastPositionValid3)
					{
						Event @event = Event.From(this.ToPointerStateEvent(currentTime, this._mouseState, EventSource.Mouse));
						EventProvider.Dispatch(@event);
					}
					result = (this._touchState.LastPositionValid || this._penState.LastPositionValid || this._mouseState.LastPositionValid);
				}
			}
			return result;
		}

		public uint playerCount
		{
			get
			{
				return 1U;
			}
		}

		private EventSource GetEventSourceFromPressedKey()
		{
			bool flag = this.InputManagerKeyboardWasPressed();
			EventSource result;
			if (flag)
			{
				result = EventSource.Keyboard;
			}
			else
			{
				bool flag2 = this.InputManagerJoystickWasPressed();
				if (flag2)
				{
					result = EventSource.Gamepad;
				}
				else
				{
					result = EventSource.Unspecified;
				}
			}
			return result;
		}

		private bool InputManagerJoystickWasPressed()
		{
			for (KeyCode keyCode = KeyCode.Joystick1Button0; keyCode <= KeyCode.Joystick8Button19; keyCode++)
			{
				bool key = this._input.GetKey(keyCode);
				if (key)
				{
					return true;
				}
			}
			return false;
		}

		private bool InputManagerKeyboardWasPressed()
		{
			for (KeyCode keyCode = KeyCode.None; keyCode <= KeyCode.Menu; keyCode++)
			{
				bool key = this._input.GetKey(keyCode);
				if (key)
				{
					return true;
				}
			}
			return false;
		}

		private float InputManagerGetAxisRawOrDefault(string axisName)
		{
			float result;
			try
			{
				result = ((!string.IsNullOrEmpty(axisName)) ? this._input.GetAxisRaw(axisName) : 0f);
			}
			catch
			{
				result = 0f;
			}
			return result;
		}

		private bool InputManagerGetButtonDownOrDefault(string axisName)
		{
			bool result;
			try
			{
				result = (!string.IsNullOrEmpty(axisName) && this._input.GetButtonDown(axisName));
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private ValueTuple<Vector2, bool> ReadCurrentNavigationMoveVector()
		{
			Vector2 vector = new Vector2(this.InputManagerGetAxisRawOrDefault(this._configuration.HorizontalAxis), this.InputManagerGetAxisRawOrDefault(this._configuration.VerticalAxis));
			bool item = false;
			bool flag = this.InputManagerGetButtonDownOrDefault(this._configuration.HorizontalAxis);
			if (flag)
			{
				bool flag2 = vector.x < 0f;
				if (flag2)
				{
					vector.x = -1f;
				}
				else
				{
					bool flag3 = vector.x > 0f;
					if (flag3)
					{
						vector.x = 1f;
					}
				}
				item = true;
			}
			bool flag4 = this.InputManagerGetButtonDownOrDefault(this._configuration.VerticalAxis);
			if (flag4)
			{
				bool flag5 = vector.y < 0f;
				if (flag5)
				{
					vector.y = -1f;
				}
				else
				{
					bool flag6 = vector.y > 0f;
					if (flag6)
					{
						vector.y = 1f;
					}
				}
				item = true;
			}
			return new ValueTuple<Vector2, bool>(vector, item);
		}

		private IMECompositionEvent ToIMECompositionEvent(DiscreteTime currentTime, string compositionString)
		{
			return new IMECompositionEvent
			{
				compositionString = compositionString,
				timestamp = currentTime,
				eventSource = EventSource.Unspecified,
				playerId = 0U,
				eventModifiers = this._eventModifiers
			};
		}

		internal static float TiltToAzimuth(Vector2 tilt)
		{
			float num = 0f;
			bool flag = tilt.x != 0f;
			if (flag)
			{
				num = 1.5707964f - Mathf.Atan2(-Mathf.Cos(tilt.x) * Mathf.Sin(tilt.y), Mathf.Cos(tilt.y) * Mathf.Sin(tilt.x));
				bool flag2 = num < 0f;
				if (flag2)
				{
					num += 6.2831855f;
				}
				bool flag3 = num >= 1.5707964f;
				if (flag3)
				{
					num -= 1.5707964f;
				}
				else
				{
					num += 4.712389f;
				}
			}
			return num;
		}

		internal static Vector2 AzimuthAndAlitutudeToTilt(float altitude, float azimuth)
		{
			return new Vector2(0f, 0f)
			{
				x = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Cos(altitude) / Mathf.Sin(azimuth)),
				y = Mathf.Atan(Mathf.Cos(azimuth) * Mathf.Sin(altitude) / Mathf.Sin(azimuth))
			};
		}

		internal static float TiltToAltitude(Vector2 tilt)
		{
			return 1.5707964f - Mathf.Acos(Mathf.Cos(tilt.x) * Mathf.Cos(tilt.y));
		}

		private static Vector2 MultiDisplayBottomLeftToPanelPosition(Vector2 position, out int targetDisplay)
		{
			int? num;
			Vector2 position2 = InputManagerProvider.MultiDisplayToLocalScreenPosition(position, out num);
			targetDisplay = num.GetValueOrDefault();
			return InputManagerProvider.ScreenBottomLeftToPanelPosition(position2, targetDisplay);
		}

		private static Vector2 MultiDisplayToLocalScreenPosition(Vector2 position, out int? targetDisplay)
		{
			Vector3 vector = Display.RelativeMouseAt(position);
			bool flag = vector != Vector3.zero;
			Vector2 result;
			if (flag)
			{
				targetDisplay = new int?((int)vector.z);
				result = vector;
			}
			else
			{
				targetDisplay = null;
				result = position;
			}
			return result;
		}

		private static Vector2 ScreenBottomLeftToPanelPosition(Vector2 position, int targetDisplay)
		{
			int num = Screen.height;
			bool flag = targetDisplay > 0 && targetDisplay < Display.displays.Length;
			if (flag)
			{
				num = Display.displays[targetDisplay].systemHeight;
			}
			position.y = (float)num - position.y;
			return position;
		}

		private static Vector2 ScreenBottomLeftToPanelDelta(Vector2 delta)
		{
			delta.y = -delta.y;
			return delta;
		}

		private InputEventPartialProvider _inputEventPartialProvider;

		private const int kDefaultPlayerId = 0;

		private string _compositionString = string.Empty;

		private InputManagerProvider.Configuration _configuration = InputManagerProvider.Configuration.GetDefaultConfiguration();

		private InputManagerProvider.IInput _input = new InputManagerProvider.Input();

		private InputManagerProvider.ITime _time = new InputManagerProvider.Time();

		private NavigationEventRepeatHelper _navigationEventRepeatHelper = new NavigationEventRepeatHelper();

		private const int kMaxMouseButtons = 5;

		private PointerState _mouseState;

		private bool _isPenPresent;

		private bool _seenAtLeastOnePenPosition;

		private Vector2 _lastSeenPenPositionForDetection;

		private PointerState _penState;

		private PenData _lastPenData;

		private Dictionary<int, int> _touchFingerIdToFingerIndex = new Dictionary<int, int>();

		private int _touchNextFingerIndex;

		private PointerState _touchState;

		private const float kSmallestReportedMovementSqrDist = 0.01f;

		private const float kScrollUGUIScaleFactor = 3f;

		private struct ButtonEventsIterator : IEnumerator
		{
			public bool Current
			{
				get
				{
					return this._bit % 2 == 0;
				}
			}

			public bool MoveNext()
			{
				for (;;)
				{
					this._bit++;
					bool flag = (this._mask & 1U << this._bit) > 0U;
					if (flag)
					{
						break;
					}
					if (this._bit >= 4)
					{
						goto Block_1;
					}
				}
				return true;
				Block_1:
				return false;
			}

			public void Reset()
			{
				this._bit = -1;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			public static InputManagerProvider.ButtonEventsIterator FromState(bool previous, bool down, bool up, bool current)
			{
				uint mask = (!previous && current) ? 1U : ((previous && !current) ? 2U : 0U);
				return new InputManagerProvider.ButtonEventsIterator
				{
					_mask = mask,
					_bit = -1
				};
			}

			private uint _mask;

			private int _bit;

			private const uint kWasPressed = 1U;

			private const uint kWasReleased = 2U;

			private const int kMaxBits = 4;
		}

		public struct Configuration
		{
			public static InputManagerProvider.Configuration GetDefaultConfiguration()
			{
				return new InputManagerProvider.Configuration
				{
					HorizontalAxis = "Horizontal",
					VerticalAxis = "Vertical",
					SubmitButton = "Submit",
					CancelButton = "Cancel",
					NavigateNextButton = "Next",
					NavigatePreviousButton = "Previous",
					InputActionsPerSecond = 10f,
					RepeatDelay = 0.5f
				};
			}

			public string HorizontalAxis;

			public string VerticalAxis;

			public string SubmitButton;

			public string CancelButton;

			public string NavigateNextButton;

			public string NavigatePreviousButton;

			public float InputActionsPerSecond;

			public float RepeatDelay;
		}

		internal interface IInput
		{
			string compositionString { get; }

			bool GetKey(KeyCode keyCode);

			bool GetKeyDown(KeyCode keyCode);

			bool GetButtonDown(string button);

			float GetAxisRaw(string axis);

			PenData GetPenEvent(int index);

			PenData GetLastPenContactEvent();

			bool touchSupported { get; }

			int touchCount { get; }

			Touch GetTouch(int index);

			bool mousePresent { get; }

			bool GetMouseButton(int button);

			bool GetMouseButtonDown(int button);

			bool GetMouseButtonUp(int button);

			Vector3 mousePosition { get; }

			Vector2 mouseScrollDelta { get; }
		}

		private class Input : InputManagerProvider.IInput
		{
			public string compositionString
			{
				get
				{
					return UnityEngine.Input.compositionString;
				}
			}

			public bool GetKey(KeyCode key)
			{
				return UnityEngine.Input.GetKey(key);
			}

			public bool GetKeyDown(KeyCode key)
			{
				return UnityEngine.Input.GetKeyDown(key);
			}

			public bool GetButtonDown(string button)
			{
				return UnityEngine.Input.GetButtonDown(button);
			}

			public float GetAxisRaw(string axis)
			{
				return UnityEngine.Input.GetAxisRaw(axis);
			}

			public PenData GetPenEvent(int index)
			{
				return UnityEngine.Input.GetPenEvent(index);
			}

			public PenData GetLastPenContactEvent()
			{
				return UnityEngine.Input.GetLastPenContactEvent();
			}

			public bool touchSupported
			{
				get
				{
					return UnityEngine.Input.touchSupported;
				}
			}

			public int touchCount
			{
				get
				{
					return UnityEngine.Input.touchCount;
				}
			}

			public Touch GetTouch(int index)
			{
				return UnityEngine.Input.GetTouch(index);
			}

			public bool mousePresent
			{
				get
				{
					return UnityEngine.Input.mousePresent;
				}
			}

			public bool GetMouseButton(int button)
			{
				return UnityEngine.Input.GetMouseButton(button);
			}

			public bool GetMouseButtonDown(int button)
			{
				return UnityEngine.Input.GetMouseButtonDown(button);
			}

			public bool GetMouseButtonUp(int button)
			{
				return UnityEngine.Input.GetMouseButtonUp(button);
			}

			public Vector3 mousePosition
			{
				get
				{
					return UnityEngine.Input.mousePosition;
				}
			}

			public Vector2 mouseScrollDelta
			{
				get
				{
					return UnityEngine.Input.mouseScrollDelta;
				}
			}
		}

		internal interface ITime
		{
			RationalTime timeAsRational { get; }
		}

		private class Time : InputManagerProvider.ITime
		{
			public RationalTime timeAsRational
			{
				get
				{
					return UnityEngine.Time.timeAsRational;
				}
			}
		}
	}
}
