using System;
using System.Collections.Generic;
using Unity.IntegerTime;
using UnityEngine.InputForUI;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem.Plugins.InputForUI
{
	internal class InputSystemProvider : IEventProviderImpl
	{
		static InputSystemProvider()
		{
			EventProvider.SetInputSystemProvider(new InputSystemProvider());
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Bootstrap()
		{
		}

		private EventModifiers m_EventModifiers
		{
			get
			{
				return this.m_InputEventPartialProvider._eventModifiers;
			}
		}

		private DiscreteTime m_CurrentTime
		{
			get
			{
				return (DiscreteTime)Time.timeAsRational;
			}
		}

		public void Initialize()
		{
			if (this.m_InputEventPartialProvider == null)
			{
				this.m_InputEventPartialProvider = new InputEventPartialProvider();
			}
			this.m_InputEventPartialProvider.Initialize();
			this.m_Events.Clear();
			this.m_MouseState.Reset();
			this.m_PenState.Reset();
			this.m_SeenPenEvents = false;
			this.m_TouchState.Reset();
			this.m_SeenTouchEvents = false;
			this.SelectInputActionAsset();
			this.RegisterActions();
			this.RegisterFixedActions();
			InputSystem.onActionsChange += this.OnActionsChange;
		}

		public void Shutdown()
		{
			this.UnregisterActions();
			this.UnregisterFixedActions();
			this.m_InputEventPartialProvider.Shutdown();
			this.m_InputEventPartialProvider = null;
			if (this.m_DefaultInputActions != null)
			{
				this.m_DefaultInputActions.Dispose();
				this.m_DefaultInputActions = null;
			}
			InputSystem.onActionsChange -= this.OnActionsChange;
		}

		public void OnActionsChange()
		{
			this.UnregisterActions();
			this.SelectInputActionAsset();
			this.RegisterActions();
		}

		public void Update()
		{
			this.m_InputEventPartialProvider.Update();
			this.m_Events.Sort((Event a, Event b) => InputSystemProvider.SortEvents(a, b));
			DiscreteTime currentTime = (DiscreteTime)Time.timeAsRational;
			this.DirectionNavigation(currentTime);
			foreach (Event @event in this.m_Events)
			{
				if (this.m_SeenTouchEvents && @event.type == Event.Type.PointerEvent && @event.eventSource == EventSource.Pen)
				{
					this.m_PenState.Reset();
				}
				else if ((this.m_SeenTouchEvents || this.m_SeenPenEvents) && @event.type == Event.Type.PointerEvent && (@event.eventSource == EventSource.Mouse || @event.eventSource == EventSource.Unspecified))
				{
					this.m_MouseState.Reset();
				}
				else
				{
					EventProvider.Dispatch(@event);
				}
			}
			if (this.m_ResetSeenEventsOnUpdate)
			{
				this.ResetSeenEvents();
				this.m_ResetSeenEventsOnUpdate = false;
			}
			this.m_Events.Clear();
		}

		private void ResetSeenEvents()
		{
			this.m_SeenTouchEvents = false;
			this.m_SeenPenEvents = false;
		}

		public bool ActionAssetIsNotNull()
		{
			return this.m_InputActionAsset != null;
		}

		private void DirectionNavigation(DiscreteTime currentTime)
		{
			ValueTuple<Vector2, bool> valueTuple = this.ReadCurrentNavigationMoveVector();
			Vector2 item = valueTuple.Item1;
			bool axisButtonsWherePressedThisFrame = valueTuple.Item2;
			NavigationEvent.Direction direction = NavigationEvent.DetermineMoveDirection(item, 0.6f);
			if (direction == NavigationEvent.Direction.None)
			{
				direction = this.ReadNextPreviousDirection();
				axisButtonsWherePressedThisFrame = this.m_NextPreviousAction.WasPressedThisFrame();
			}
			if (direction == NavigationEvent.Direction.None)
			{
				this.m_RepeatHelper.Reset();
				return;
			}
			if (this.m_RepeatHelper.ShouldSendMoveEvent(currentTime, direction, axisButtonsWherePressedThisFrame))
			{
				Event @event = Event.From(new NavigationEvent
				{
					type = NavigationEvent.Type.Move,
					direction = direction,
					timestamp = currentTime,
					eventSource = this.GetEventSource(this.GetActiveDeviceFromDirection(direction)),
					playerId = 0U,
					eventModifiers = this.m_EventModifiers
				});
				EventProvider.Dispatch(@event);
			}
		}

		private InputDevice GetActiveDeviceFromDirection(NavigationEvent.Direction direction)
		{
			switch (direction)
			{
			case NavigationEvent.Direction.Left:
			case NavigationEvent.Direction.Up:
			case NavigationEvent.Direction.Right:
			case NavigationEvent.Direction.Down:
				if (this.m_MoveAction != null)
				{
					return this.m_MoveAction.action.activeControl.device;
				}
				break;
			case NavigationEvent.Direction.Next:
			case NavigationEvent.Direction.Previous:
				if (this.m_NextPreviousAction != null)
				{
					return this.m_NextPreviousAction.activeControl.device;
				}
				break;
			}
			return Keyboard.current;
		}

		private ValueTuple<Vector2, bool> ReadCurrentNavigationMoveVector()
		{
			if (this.m_MoveAction == null)
			{
				return new ValueTuple<Vector2, bool>(default(Vector2), false);
			}
			Vector2 item = this.m_MoveAction.action.ReadValue<Vector2>();
			bool item2 = this.m_MoveAction.action.WasPressedThisFrame();
			return new ValueTuple<Vector2, bool>(item, item2);
		}

		private NavigationEvent.Direction ReadNextPreviousDirection()
		{
			if (!this.m_NextPreviousAction.IsPressed() || !(this.m_NextPreviousAction.activeControl.device is Keyboard))
			{
				return NavigationEvent.Direction.None;
			}
			if (!(this.m_NextPreviousAction.activeControl.device as Keyboard).shiftKey.isPressed)
			{
				return NavigationEvent.Direction.Next;
			}
			return NavigationEvent.Direction.Previous;
		}

		private static int SortEvents(Event a, Event b)
		{
			return Event.CompareType(a, b);
		}

		public void OnFocusChanged(bool focus)
		{
			this.m_InputEventPartialProvider.OnFocusChanged(focus);
		}

		public bool RequestCurrentState(Event.Type type)
		{
			if (this.m_InputEventPartialProvider.RequestCurrentState(type))
			{
				return true;
			}
			if (type != Event.Type.PointerEvent)
			{
				if (type != Event.Type.IMECompositionEvent)
				{
				}
				return false;
			}
			if (this.m_TouchState.LastPositionValid)
			{
				Event @event = Event.From(this.ToPointerStateEvent(this.m_CurrentTime, this.m_TouchState, EventSource.Touch));
				EventProvider.Dispatch(@event);
			}
			if (this.m_PenState.LastPositionValid)
			{
				Event @event = Event.From(this.ToPointerStateEvent(this.m_CurrentTime, this.m_PenState, EventSource.Pen));
				EventProvider.Dispatch(@event);
			}
			if (this.m_MouseState.LastPositionValid)
			{
				Event @event = Event.From(this.ToPointerStateEvent(this.m_CurrentTime, this.m_MouseState, EventSource.Mouse));
				EventProvider.Dispatch(@event);
			}
			return this.m_TouchState.LastPositionValid || this.m_PenState.LastPositionValid || this.m_MouseState.LastPositionValid;
		}

		public uint playerCount
		{
			get
			{
				return 1U;
			}
		}

		private static Vector2 ScreenBottomLeftToPanelPosition(Vector2 position, int targetDisplay)
		{
			int num = Screen.height;
			if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
			{
				num = Display.displays[targetDisplay].systemHeight;
			}
			position.y = (float)num - position.y;
			return position;
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
			result.button = PointerEvent.Button.None;
			PointerState pointerState = state;
			result.buttonsState = pointerState.ButtonsState;
			result.clickCount = 0;
			result.timestamp = currentTime;
			result.eventSource = eventSource;
			result.playerId = 0U;
			result.eventModifiers = this.m_EventModifiers;
			return result;
		}

		private EventSource GetEventSource(InputAction.CallbackContext ctx)
		{
			InputDevice device = ctx.control.device;
			return this.GetEventSource(device);
		}

		private EventSource GetEventSource(InputDevice device)
		{
			if (device is Touchscreen)
			{
				return EventSource.Touch;
			}
			if (device is Pen)
			{
				return EventSource.Pen;
			}
			if (device is Mouse)
			{
				return EventSource.Mouse;
			}
			if (device is Keyboard)
			{
				return EventSource.Keyboard;
			}
			if (device is Gamepad)
			{
				return EventSource.Gamepad;
			}
			return EventSource.Unspecified;
		}

		private ref PointerState GetPointerStateForSource(EventSource eventSource)
		{
			if (eventSource == EventSource.Pen)
			{
				return ref this.m_PenState;
			}
			if (eventSource == EventSource.Touch)
			{
				return ref this.m_TouchState;
			}
			return ref this.m_MouseState;
		}

		private void DispatchFromCallback(in Event ev)
		{
			this.m_Events.Add(ev);
		}

		private static int FindTouchFingerIndex(Touchscreen touchscreen, InputAction.CallbackContext ctx)
		{
			if (touchscreen == null)
			{
				return 0;
			}
			Vector2Control vector2Control = (ctx.control is Vector2Control) ? ((Vector2Control)ctx.control) : null;
			TouchPressControl touchPressControl = (ctx.control is TouchPressControl) ? ((TouchPressControl)ctx.control) : null;
			TouchControl touchControl = (ctx.control is TouchControl) ? ((TouchControl)ctx.control) : null;
			for (int i = 0; i < touchscreen.touches.Count; i++)
			{
				if (vector2Control != null && vector2Control == touchscreen.touches[i].position)
				{
					return i;
				}
				if (touchPressControl != null && touchPressControl == touchscreen.touches[i].press)
				{
					return i;
				}
				if (touchControl != null && touchControl == touchscreen.touches[i])
				{
					return i;
				}
			}
			return 0;
		}

		private void OnPointerPerformed(InputAction.CallbackContext ctx)
		{
			EventSource eventSource = this.GetEventSource(ctx);
			ref PointerState pointerStateForSource = ref this.GetPointerStateForSource(eventSource);
			Pointer pointer = (ctx.control.device is Pointer) ? ((Pointer)ctx.control.device) : null;
			Pen pen = (ctx.control.device is Pen) ? ((Pen)ctx.control.device) : null;
			Touchscreen touchscreen = (ctx.control.device is Touchscreen) ? ((Touchscreen)ctx.control.device) : null;
			TouchControl touchControl = (ctx.control is TouchControl) ? ((TouchControl)ctx.control) : null;
			int pointerIndex = InputSystemProvider.FindTouchFingerIndex(touchscreen, ctx);
			this.m_ResetSeenEventsOnUpdate = false;
			if (touchControl != null || touchscreen != null)
			{
				this.m_SeenTouchEvents = true;
			}
			else if (pen != null)
			{
				this.m_SeenPenEvents = true;
			}
			Vector2 position = ctx.ReadValue<Vector2>();
			int num = (pointer != null) ? pointer.displayIndex.ReadValue() : ((touchscreen != null) ? touchscreen.displayIndex.ReadValue() : ((pen != null) ? pen.displayIndex.ReadValue() : 0));
			Vector2 vector = InputSystemProvider.ScreenBottomLeftToPanelPosition(position, num);
			Vector2 deltaPosition = pointerStateForSource.LastPositionValid ? (vector - pointerStateForSource.LastPosition) : Vector2.zero;
			Vector2 tilt = (pen != null) ? pen.tilt.ReadValue() : Vector2.zero;
			float twist = (pen != null) ? pen.twist.ReadValue() : 0f;
			float pressure = (pen != null) ? pen.pressure.ReadValue() : ((touchControl != null) ? touchControl.pressure.ReadValue() : 0f);
			bool isInverted = pen != null && pen.eraser.isPressed;
			if (deltaPosition.sqrMagnitude >= 0.01f)
			{
				Event @event = Event.From(new PointerEvent
				{
					type = PointerEvent.Type.PointerMoved,
					pointerIndex = pointerIndex,
					position = vector,
					deltaPosition = deltaPosition,
					scroll = Vector2.zero,
					displayIndex = num,
					tilt = tilt,
					twist = twist,
					pressure = pressure,
					isInverted = isInverted,
					button = PointerEvent.Button.None,
					buttonsState = pointerStateForSource.ButtonsState,
					clickCount = 0,
					timestamp = this.m_CurrentTime,
					eventSource = eventSource,
					playerId = 0U,
					eventModifiers = this.m_EventModifiers
				});
				this.DispatchFromCallback(@event);
				pointerStateForSource.OnMove(this.m_CurrentTime, vector, num);
				return;
			}
			if (!pointerStateForSource.LastPositionValid)
			{
				pointerStateForSource.OnMove(this.m_CurrentTime, vector, num);
			}
		}

		private void OnSubmitPerformed(InputAction.CallbackContext ctx)
		{
			Event @event = Event.From(new NavigationEvent
			{
				type = NavigationEvent.Type.Submit,
				direction = NavigationEvent.Direction.None,
				timestamp = this.m_CurrentTime,
				eventSource = this.GetEventSource(ctx),
				playerId = 0U,
				eventModifiers = this.m_EventModifiers
			});
			this.DispatchFromCallback(@event);
		}

		private void OnCancelPerformed(InputAction.CallbackContext ctx)
		{
			Event @event = Event.From(new NavigationEvent
			{
				type = NavigationEvent.Type.Cancel,
				direction = NavigationEvent.Direction.None,
				timestamp = this.m_CurrentTime,
				eventSource = this.GetEventSource(ctx),
				playerId = 0U,
				eventModifiers = this.m_EventModifiers
			});
			this.DispatchFromCallback(@event);
		}

		private void OnClickPerformed(InputAction.CallbackContext ctx, EventSource eventSource, PointerEvent.Button button)
		{
			ref PointerState pointerStateForSource = ref this.GetPointerStateForSource(eventSource);
			Touchscreen touchscreen = (ctx.control.device is Touchscreen) ? ((Touchscreen)ctx.control.device) : null;
			bool flag = ((ctx.control is TouchControl) ? ((TouchControl)ctx.control) : null) != null;
			int pointerIndex = InputSystemProvider.FindTouchFingerIndex(touchscreen, ctx);
			this.m_ResetSeenEventsOnUpdate = true;
			if (flag || touchscreen != null)
			{
				this.m_SeenTouchEvents = true;
			}
			bool previousState = pointerStateForSource.ButtonsState.Get(button);
			bool flag2 = ctx.ReadValueAsButton();
			pointerStateForSource.OnButtonChange(this.m_CurrentTime, button, previousState, flag2);
			Event @event = Event.From(new PointerEvent
			{
				type = (flag2 ? PointerEvent.Type.ButtonPressed : PointerEvent.Type.ButtonReleased),
				pointerIndex = pointerIndex,
				position = pointerStateForSource.LastPosition,
				deltaPosition = Vector2.zero,
				scroll = Vector2.zero,
				displayIndex = pointerStateForSource.LastDisplayIndex,
				tilt = Vector2.zero,
				twist = 0f,
				pressure = 0f,
				isInverted = false,
				button = button,
				buttonsState = pointerStateForSource.ButtonsState,
				clickCount = pointerStateForSource.ClickCount,
				timestamp = this.m_CurrentTime,
				eventSource = eventSource,
				playerId = 0U,
				eventModifiers = this.m_EventModifiers
			});
			this.DispatchFromCallback(@event);
		}

		private void OnLeftClickPerformed(InputAction.CallbackContext ctx)
		{
			this.OnClickPerformed(ctx, this.GetEventSource(ctx), PointerEvent.Button.Primary);
		}

		private void OnMiddleClickPerformed(InputAction.CallbackContext ctx)
		{
			this.OnClickPerformed(ctx, this.GetEventSource(ctx), PointerEvent.Button.PenBarrelButton);
		}

		private void OnRightClickPerformed(InputAction.CallbackContext ctx)
		{
			this.OnClickPerformed(ctx, this.GetEventSource(ctx), PointerEvent.Button.PenEraserInTouch);
		}

		private void OnScrollWheelPerformed(InputAction.CallbackContext ctx)
		{
			Vector2 vector = ctx.ReadValue<Vector2>() / InputSystem.scrollWheelDeltaPerTick;
			if (vector.sqrMagnitude < 0.01f)
			{
				return;
			}
			EventSource eventSource = this.GetEventSource(ctx);
			ref PointerState pointerStateForSource = ref this.GetPointerStateForSource(eventSource);
			Vector2 position = Vector2.zero;
			int displayIndex = 0;
			if (pointerStateForSource.LastPositionValid)
			{
				position = pointerStateForSource.LastPosition;
				displayIndex = pointerStateForSource.LastDisplayIndex;
			}
			else if (eventSource == EventSource.Mouse && Mouse.current != null)
			{
				position = Mouse.current.position.ReadValue();
				displayIndex = Mouse.current.displayIndex.ReadValue();
			}
			Vector2 scroll = new Vector2
			{
				x = vector.x * 3f,
				y = -vector.y * 3f
			};
			Event @event = Event.From(new PointerEvent
			{
				type = PointerEvent.Type.Scroll,
				pointerIndex = 0,
				position = position,
				deltaPosition = Vector2.zero,
				scroll = scroll,
				displayIndex = displayIndex,
				tilt = Vector2.zero,
				twist = 0f,
				pressure = 0f,
				isInverted = false,
				button = PointerEvent.Button.None,
				buttonsState = pointerStateForSource.ButtonsState,
				clickCount = 0,
				timestamp = this.m_CurrentTime,
				eventSource = EventSource.Mouse,
				playerId = 0U,
				eventModifiers = this.m_EventModifiers
			});
			this.DispatchFromCallback(@event);
		}

		private void RegisterFixedActions()
		{
			this.m_NextPreviousAction = new InputAction("nextPreviousAction", InputActionType.Button, null, null, null, null);
			this.m_NextPreviousAction.AddBinding("<Keyboard>/tab", null, null, null);
			this.m_NextPreviousAction.Enable();
		}

		private void UnregisterFixedActions()
		{
			if (this.m_NextPreviousAction != null)
			{
				this.m_NextPreviousAction.Disable();
				this.m_NextPreviousAction = null;
			}
		}

		private void RegisterActions()
		{
			Action<InputActionAsset> action = InputSystemProvider.s_OnRegisterActions;
			if (action != null)
			{
				action(this.m_InputActionAsset);
			}
			this.m_PointAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.PointAction, false));
			this.m_MoveAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.MoveAction, false));
			this.m_SubmitAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.SubmitAction, false));
			this.m_CancelAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.CancelAction, false));
			this.m_LeftClickAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.LeftClickAction, false));
			this.m_MiddleClickAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.MiddleClickAction, false));
			this.m_RightClickAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.RightClickAction, false));
			this.m_ScrollWheelAction = InputActionReference.Create(this.m_InputActionAsset.FindAction(InputSystemProvider.Actions.ScrollWheelAction, false));
			if (this.m_PointAction != null && this.m_PointAction.action != null)
			{
				this.m_PointAction.action.performed += this.OnPointerPerformed;
			}
			if (this.m_SubmitAction != null && this.m_SubmitAction.action != null)
			{
				this.m_SubmitAction.action.performed += this.OnSubmitPerformed;
			}
			if (this.m_CancelAction != null && this.m_CancelAction.action != null)
			{
				this.m_CancelAction.action.performed += this.OnCancelPerformed;
			}
			if (this.m_LeftClickAction != null && this.m_LeftClickAction.action != null)
			{
				this.m_LeftClickAction.action.performed += this.OnLeftClickPerformed;
			}
			if (this.m_MiddleClickAction != null && this.m_MiddleClickAction.action != null)
			{
				this.m_MiddleClickAction.action.performed += this.OnMiddleClickPerformed;
			}
			if (this.m_RightClickAction != null && this.m_RightClickAction.action != null)
			{
				this.m_RightClickAction.action.performed += this.OnRightClickPerformed;
			}
			if (this.m_ScrollWheelAction != null && this.m_ScrollWheelAction.action != null)
			{
				this.m_ScrollWheelAction.action.performed += this.OnScrollWheelPerformed;
			}
			if (InputSystem.actions == null)
			{
				this.m_InputActionAsset.FindActionMap("UI", true).Enable();
				return;
			}
			this.m_InputActionAsset.Enable();
		}

		private void UnregisterActions()
		{
			if (this.m_PointAction != null && this.m_PointAction.action != null)
			{
				this.m_PointAction.action.performed -= this.OnPointerPerformed;
			}
			if (this.m_SubmitAction != null && this.m_SubmitAction.action != null)
			{
				this.m_SubmitAction.action.performed -= this.OnSubmitPerformed;
			}
			if (this.m_CancelAction != null && this.m_CancelAction.action != null)
			{
				this.m_CancelAction.action.performed -= this.OnCancelPerformed;
			}
			if (this.m_LeftClickAction != null && this.m_LeftClickAction.action != null)
			{
				this.m_LeftClickAction.action.performed -= this.OnLeftClickPerformed;
			}
			if (this.m_MiddleClickAction != null && this.m_MiddleClickAction.action != null)
			{
				this.m_MiddleClickAction.action.performed -= this.OnMiddleClickPerformed;
			}
			if (this.m_RightClickAction != null && this.m_RightClickAction.action != null)
			{
				this.m_RightClickAction.action.performed -= this.OnRightClickPerformed;
			}
			if (this.m_ScrollWheelAction != null && this.m_ScrollWheelAction.action != null)
			{
				this.m_ScrollWheelAction.action.performed -= this.OnScrollWheelPerformed;
			}
			this.m_PointAction = null;
			this.m_MoveAction = null;
			this.m_SubmitAction = null;
			this.m_CancelAction = null;
			this.m_LeftClickAction = null;
			this.m_MiddleClickAction = null;
			this.m_RightClickAction = null;
			this.m_ScrollWheelAction = null;
			if (this.m_InputActionAsset != null)
			{
				this.m_InputActionAsset.Disable();
			}
		}

		private void SelectInputActionAsset()
		{
			InputActionAsset actions = InputSystem.actions;
			if (actions != null && actions.FindActionMap("UI", false) != null)
			{
				this.m_InputActionAsset = InputSystem.actions;
				return;
			}
			if (this.m_DefaultInputActions == null)
			{
				this.m_DefaultInputActions = new DefaultInputActions();
			}
			this.m_InputActionAsset = this.m_DefaultInputActions.asset;
		}

		internal static void SetOnRegisterActions(Action<InputActionAsset> callback)
		{
			InputSystemProvider.s_OnRegisterActions = callback;
		}

		private InputEventPartialProvider m_InputEventPartialProvider;

		private DefaultInputActions m_DefaultInputActions;

		private InputActionAsset m_InputActionAsset;

		private InputActionReference m_PointAction;

		private InputActionReference m_MoveAction;

		private InputActionReference m_SubmitAction;

		private InputActionReference m_CancelAction;

		private InputActionReference m_LeftClickAction;

		private InputActionReference m_MiddleClickAction;

		private InputActionReference m_RightClickAction;

		private InputActionReference m_ScrollWheelAction;

		private InputAction m_NextPreviousAction;

		private List<Event> m_Events = new List<Event>();

		private PointerState m_MouseState;

		private PointerState m_PenState;

		private bool m_SeenPenEvents;

		private PointerState m_TouchState;

		private bool m_SeenTouchEvents;

		private const float k_SmallestReportedMovementSqrDist = 0.01f;

		private NavigationEventRepeatHelper m_RepeatHelper = new NavigationEventRepeatHelper();

		private bool m_ResetSeenEventsOnUpdate;

		private const float kScrollUGUIScaleFactor = 3f;

		private static Action<InputActionAsset> s_OnRegisterActions;

		private const uint k_DefaultPlayerId = 0U;

		public static class Actions
		{
			public static readonly string PointAction = "UI/Point";

			public static readonly string MoveAction = "UI/Navigate";

			public static readonly string SubmitAction = "UI/Submit";

			public static readonly string CancelAction = "UI/Cancel";

			public static readonly string LeftClickAction = "UI/Click";

			public static readonly string MiddleClickAction = "UI/MiddleClick";

			public static readonly string RightClickAction = "UI/RightClick";

			public static readonly string ScrollWheelAction = "UI/ScrollWheel";
		}
	}
}
