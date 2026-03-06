using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Pooling;

namespace UnityEngine.XR.Interaction.Toolkit.UI
{
	[AddComponentMenu("Event/XR UI Input Module", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule.html")]
	public class XRUIInputModule : UIInputModule
	{
		[Obsolete("activeInputMode has been deprecated in version 3.1.0. Input System Package (New) will be the default input handling mode used when active input handling is set to Both.")]
		public XRUIInputModule.ActiveInputMode activeInputMode
		{
			get
			{
				return this.m_ActiveInputMode;
			}
			set
			{
				this.m_ActiveInputMode = value;
			}
		}

		public bool enableXRInput
		{
			get
			{
				return this.m_EnableXRInput;
			}
			set
			{
				this.m_EnableXRInput = value;
			}
		}

		public bool enableMouseInput
		{
			get
			{
				return this.m_EnableMouseInput;
			}
			set
			{
				this.m_EnableMouseInput = value;
			}
		}

		public bool enableTouchInput
		{
			get
			{
				return this.m_EnableTouchInput;
			}
			set
			{
				this.m_EnableTouchInput = value;
			}
		}

		public bool enableGamepadInput
		{
			get
			{
				return this.m_EnableGamepadInput;
			}
			set
			{
				this.m_EnableGamepadInput = value;
			}
		}

		public bool enableJoystickInput
		{
			get
			{
				return this.m_EnableJoystickInput;
			}
			set
			{
				this.m_EnableJoystickInput = value;
			}
		}

		public InputActionReference pointAction
		{
			get
			{
				return this.m_PointAction;
			}
			set
			{
				this.SetInputAction(ref this.m_PointAction, value);
			}
		}

		public InputActionReference leftClickAction
		{
			get
			{
				return this.m_LeftClickAction;
			}
			set
			{
				this.SetInputAction(ref this.m_LeftClickAction, value);
			}
		}

		public InputActionReference middleClickAction
		{
			get
			{
				return this.m_MiddleClickAction;
			}
			set
			{
				this.SetInputAction(ref this.m_MiddleClickAction, value);
			}
		}

		public InputActionReference rightClickAction
		{
			get
			{
				return this.m_RightClickAction;
			}
			set
			{
				this.SetInputAction(ref this.m_RightClickAction, value);
			}
		}

		public InputActionReference scrollWheelAction
		{
			get
			{
				return this.m_ScrollWheelAction;
			}
			set
			{
				this.SetInputAction(ref this.m_ScrollWheelAction, value);
			}
		}

		public InputActionReference navigateAction
		{
			get
			{
				return this.m_NavigateAction;
			}
			set
			{
				this.SetInputAction(ref this.m_NavigateAction, value);
			}
		}

		public InputActionReference submitAction
		{
			get
			{
				return this.m_SubmitAction;
			}
			set
			{
				this.SetInputAction(ref this.m_SubmitAction, value);
			}
		}

		public InputActionReference cancelAction
		{
			get
			{
				return this.m_CancelAction;
			}
			set
			{
				this.SetInputAction(ref this.m_CancelAction, value);
			}
		}

		public bool enableBuiltinActionsAsFallback
		{
			get
			{
				return this.m_EnableBuiltinActionsAsFallback;
			}
			set
			{
				this.m_EnableBuiltinActionsAsFallback = value;
				this.m_UseBuiltInInputSystemActions = (this.m_EnableBuiltinActionsAsFallback && !this.InputActionReferencesAreSet());
			}
		}

		public string horizontalAxis
		{
			get
			{
				return this.m_HorizontalAxis;
			}
			set
			{
				this.m_HorizontalAxis = value;
			}
		}

		public string verticalAxis
		{
			get
			{
				return this.m_VerticalAxis;
			}
			set
			{
				this.m_VerticalAxis = value;
			}
		}

		public string submitButton
		{
			get
			{
				return this.m_SubmitButton;
			}
			set
			{
				this.m_SubmitButton = value;
			}
		}

		public string cancelButton
		{
			get
			{
				return this.m_CancelButton;
			}
			set
			{
				this.m_CancelButton = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_ActiveInputMode = XRUIInputModule.ActiveInputMode.InputSystemActions;
			this.m_PointerState = new PointerModel(0);
			this.m_NavigationState = default(NavigationModel);
			this.m_UseBuiltInInputSystemActions = (this.m_EnableBuiltinActionsAsFallback && !this.InputActionReferencesAreSet());
			if (this.m_ActiveInputMode != XRUIInputModule.ActiveInputMode.InputManagerBindings)
			{
				this.EnableAllActions();
			}
		}

		protected override void OnDisable()
		{
			base.RemovePointerEventData(this.m_PointerState.pointerId);
			if (this.m_ActiveInputMode != XRUIInputModule.ActiveInputMode.InputManagerBindings)
			{
				this.DisableAllActions();
			}
			base.OnDisable();
		}

		public void RegisterInteractor(IUIInteractor interactor)
		{
			if (interactor == null)
			{
				return;
			}
			for (int i = 0; i < this.m_RegisteredInteractors.Count; i++)
			{
				XRUIInputModule.RegisteredInteractor registeredInteractor = this.m_RegisteredInteractors[i];
				if (registeredInteractor.interactor == interactor)
				{
					if (!registeredInteractor.active)
					{
						registeredInteractor.active = true;
						registeredInteractor.deactivating = false;
						registeredInteractor.model.Reset(true);
						this.m_RegisteredInteractors[i] = registeredInteractor;
					}
					return;
				}
			}
			int deviceIndex;
			if (!this.m_DeletedPointerIds.TryPop(out deviceIndex))
			{
				int rollingPointerId = this.m_RollingPointerId;
				this.m_RollingPointerId = rollingPointerId + 1;
				deviceIndex = rollingPointerId;
			}
			this.m_RegisteredInteractors.Add(new XRUIInputModule.RegisteredInteractor(interactor, deviceIndex));
		}

		public void UnregisterInteractor(IUIInteractor interactor)
		{
			if (interactor == null)
			{
				return;
			}
			for (int i = 0; i < this.m_RegisteredInteractors.Count; i++)
			{
				XRUIInputModule.RegisteredInteractor registeredInteractor = this.m_RegisteredInteractors[i];
				if (registeredInteractor.interactor == interactor)
				{
					if (registeredInteractor.active)
					{
						registeredInteractor.deactivating = true;
						registeredInteractor.active = false;
						this.m_RegisteredInteractors[i] = registeredInteractor;
					}
					return;
				}
			}
		}

		public IUIInteractor GetInteractor(int pointerId)
		{
			for (int i = 0; i < this.m_RegisteredInteractors.Count; i++)
			{
				if (this.m_RegisteredInteractors[i].model.pointerId == pointerId && this.m_RegisteredInteractors[i].active)
				{
					return this.m_RegisteredInteractors[i].interactor;
				}
			}
			return null;
		}

		public bool GetTrackedDeviceModel(IUIInteractor interactor, out TrackedDeviceModel model)
		{
			for (int i = 0; i < this.m_RegisteredInteractors.Count; i++)
			{
				if (this.m_RegisteredInteractors[i].interactor == interactor)
				{
					model = this.m_RegisteredInteractors[i].model;
					return true;
				}
			}
			model = new TrackedDeviceModel(-1);
			return false;
		}

		protected override void DoProcess()
		{
			if (this.m_EnableXRInput)
			{
				int i = 0;
				while (i < this.m_RegisteredInteractors.Count)
				{
					XRUIInputModule.RegisteredInteractor registeredInteractor = this.m_RegisteredInteractors[i];
					GameObject pointerTarget = registeredInteractor.model.implementationData.pointerTarget;
					Object @object = registeredInteractor.interactor as Object;
					bool flag = @object != null && @object == null;
					if (flag || registeredInteractor.deactivating)
					{
						registeredInteractor.model.Reset(false);
						base.ProcessTrackedDevice(ref registeredInteractor.model, true);
						base.RemovePointerEventData(registeredInteractor.model.pointerId);
						if (!flag)
						{
							registeredInteractor.deactivating = false;
							registeredInteractor.model.Reset(true);
							this.m_RegisteredInteractors[i] = registeredInteractor;
							goto IL_124;
						}
						this.m_DeletedPointerIds.Push(registeredInteractor.model.pointerId);
						this.m_RegisteredInteractors.RemoveAt(i--);
					}
					else if (registeredInteractor.active)
					{
						registeredInteractor.interactor.UpdateUIModel(ref registeredInteractor.model);
						base.ProcessTrackedDevice(ref registeredInteractor.model, false);
						registeredInteractor.model.UpdatePokeSelectState();
						this.m_RegisteredInteractors[i] = registeredInteractor;
						goto IL_124;
					}
					IL_281:
					i++;
					continue;
					IL_124:
					GameObject pointerTarget2 = registeredInteractor.model.implementationData.pointerTarget;
					if (pointerTarget != pointerTarget2)
					{
						UIHoverEventArgs uihoverEventArgs;
						using (this.m_UIHoverEventArgs.Get(out uihoverEventArgs))
						{
							uihoverEventArgs.interactorObject = registeredInteractor.interactor;
							uihoverEventArgs.deviceModel = registeredInteractor.model;
							if (uihoverEventArgs.interactorObject != null)
							{
								IUIHoverInteractor iuihoverInteractor = uihoverEventArgs.interactorObject as IUIHoverInteractor;
								if (iuihoverInteractor != null)
								{
									if (pointerTarget != null)
									{
										uihoverEventArgs.uiObject = pointerTarget;
										iuihoverInteractor.OnUIHoverExited(uihoverEventArgs);
									}
									if (pointerTarget2 != null && pointerTarget2.activeInHierarchy)
									{
										uihoverEventArgs.uiObject = pointerTarget2;
										iuihoverInteractor.OnUIHoverEntered(uihoverEventArgs);
									}
								}
							}
						}
					}
					if ((pointerTarget != null && !pointerTarget.activeInHierarchy) || (pointerTarget != null && pointerTarget == null))
					{
						UIHoverEventArgs uihoverEventArgs2;
						using (this.m_UIHoverEventArgs.Get(out uihoverEventArgs2))
						{
							if (pointerTarget == pointerTarget2)
							{
								registeredInteractor.model.Reset(true);
								this.m_RegisteredInteractors[i] = registeredInteractor;
							}
							IUIInteractor interactor = registeredInteractor.interactor;
							if (interactor != null)
							{
								IUIHoverInteractor iuihoverInteractor2 = interactor as IUIHoverInteractor;
								if (iuihoverInteractor2 != null)
								{
									uihoverEventArgs2.interactorObject = interactor;
									uihoverEventArgs2.uiObject = pointerTarget;
									uihoverEventArgs2.deviceModel = registeredInteractor.model;
									iuihoverInteractor2.OnUIHoverExited(uihoverEventArgs2);
								}
							}
						}
						goto IL_281;
					}
					goto IL_281;
				}
			}
			if (this.m_ActiveInputMode != XRUIInputModule.ActiveInputMode.InputManagerBindings)
			{
				this.GetPointerStates();
			}
			base.ProcessPointerState(ref this.m_PointerState);
			base.ProcessNavigationState(ref this.m_NavigationState);
		}

		private void GetPointerStates()
		{
			if (this.m_UseBuiltInInputSystemActions)
			{
				if (this.m_EnableTouchInput && Touchscreen.current != null)
				{
					this.m_PointerState.position = Touchscreen.current.position.ReadValue();
					this.m_PointerState.displayIndex = Touchscreen.current.displayIndex.ReadValue();
				}
				if (this.m_EnableMouseInput && Mouse.current != null)
				{
					this.m_PointerState.position = Mouse.current.position.ReadValue();
					this.m_PointerState.displayIndex = Mouse.current.displayIndex.ReadValue();
					this.m_PointerState.scrollDelta = Mouse.current.scroll.ReadValue() * 0.05f;
					this.m_PointerState.leftButtonPressed = Mouse.current.leftButton.isPressed;
					this.m_PointerState.rightButtonPressed = Mouse.current.rightButton.isPressed;
					this.m_PointerState.middleButtonPressed = Mouse.current.middleButton.isPressed;
				}
				if (this.m_EnableGamepadInput && Gamepad.current != null)
				{
					this.m_NavigationState.move = Gamepad.current.leftStick.ReadValue() + Gamepad.current.dpad.ReadValue();
					this.m_NavigationState.submitButtonDown = Gamepad.current.buttonSouth.isPressed;
					this.m_NavigationState.cancelButtonDown = Gamepad.current.buttonEast.isPressed;
				}
				if (this.m_EnableJoystickInput && Joystick.current != null)
				{
					this.m_NavigationState.move = Joystick.current.stick.ReadValue() + ((Joystick.current.hatswitch != null) ? Joystick.current.hatswitch.ReadValue() : Vector2.zero);
					this.m_NavigationState.submitButtonDown = Joystick.current.trigger.isPressed;
					this.m_NavigationState.cancelButtonDown = false;
					return;
				}
			}
			else
			{
				if (XRUIInputModule.IsActionEnabled(this.m_PointAction))
				{
					this.m_PointerState.position = this.m_PointAction.action.ReadValue<Vector2>();
					this.m_PointerState.displayIndex = this.GetDisplayIndexFor(this.m_PointAction.action.activeControl);
				}
				if (XRUIInputModule.IsActionEnabled(this.m_ScrollWheelAction))
				{
					this.m_PointerState.scrollDelta = this.m_ScrollWheelAction.action.ReadValue<Vector2>() * 0.05f;
				}
				if (XRUIInputModule.IsActionEnabled(this.m_LeftClickAction))
				{
					this.m_PointerState.leftButtonPressed = this.m_LeftClickAction.action.IsPressed();
				}
				if (XRUIInputModule.IsActionEnabled(this.m_RightClickAction))
				{
					this.m_PointerState.rightButtonPressed = this.m_RightClickAction.action.IsPressed();
				}
				if (XRUIInputModule.IsActionEnabled(this.m_MiddleClickAction))
				{
					this.m_PointerState.middleButtonPressed = this.m_MiddleClickAction.action.IsPressed();
				}
				if (XRUIInputModule.IsActionEnabled(this.m_NavigateAction))
				{
					this.m_NavigationState.move = this.m_NavigateAction.action.ReadValue<Vector2>();
				}
				if (XRUIInputModule.IsActionEnabled(this.m_SubmitAction))
				{
					this.m_NavigationState.submitButtonDown = this.m_SubmitAction.action.WasPerformedThisFrame();
				}
				if (XRUIInputModule.IsActionEnabled(this.m_CancelAction))
				{
					this.m_NavigationState.cancelButtonDown = this.m_CancelAction.action.WasPerformedThisFrame();
				}
			}
		}

		private bool InputActionReferencesAreSet()
		{
			return this.m_PointAction != null || this.m_LeftClickAction != null || this.m_RightClickAction != null || this.m_MiddleClickAction != null || this.m_NavigateAction != null || this.m_SubmitAction != null || this.m_CancelAction != null || this.m_ScrollWheelAction != null;
		}

		private void EnableAllActions()
		{
			XRUIInputModule.EnableInputAction(this.m_PointAction);
			XRUIInputModule.EnableInputAction(this.m_LeftClickAction);
			XRUIInputModule.EnableInputAction(this.m_RightClickAction);
			XRUIInputModule.EnableInputAction(this.m_MiddleClickAction);
			XRUIInputModule.EnableInputAction(this.m_NavigateAction);
			XRUIInputModule.EnableInputAction(this.m_SubmitAction);
			XRUIInputModule.EnableInputAction(this.m_CancelAction);
			XRUIInputModule.EnableInputAction(this.m_ScrollWheelAction);
		}

		private void DisableAllActions()
		{
			XRUIInputModule.DisableInputAction(this.m_PointAction);
			XRUIInputModule.DisableInputAction(this.m_LeftClickAction);
			XRUIInputModule.DisableInputAction(this.m_RightClickAction);
			XRUIInputModule.DisableInputAction(this.m_MiddleClickAction);
			XRUIInputModule.DisableInputAction(this.m_NavigateAction);
			XRUIInputModule.DisableInputAction(this.m_SubmitAction);
			XRUIInputModule.DisableInputAction(this.m_CancelAction);
			XRUIInputModule.DisableInputAction(this.m_ScrollWheelAction);
		}

		private static bool IsActionEnabled(InputActionReference inputAction)
		{
			return inputAction != null && inputAction.action != null && inputAction.action.enabled;
		}

		private static void EnableInputAction(InputActionReference inputAction)
		{
			if (inputAction == null || inputAction.action == null)
			{
				return;
			}
			inputAction.action.Enable();
		}

		private static void DisableInputAction(InputActionReference inputAction)
		{
			if (inputAction == null || inputAction.action == null)
			{
				return;
			}
			inputAction.action.Disable();
		}

		private void SetInputAction(ref InputActionReference inputAction, InputActionReference value)
		{
			if (Application.isPlaying && inputAction != null)
			{
				InputAction action = inputAction.action;
				if (action != null)
				{
					action.Disable();
				}
			}
			inputAction = value;
			if (Application.isPlaying && base.isActiveAndEnabled && inputAction != null)
			{
				InputAction action2 = inputAction.action;
				if (action2 == null)
				{
					return;
				}
				action2.Enable();
			}
		}

		private int GetDisplayIndexFor(InputControl control)
		{
			int result = 0;
			if (control != null)
			{
				Pointer pointer = control.device as Pointer;
				if (pointer != null && pointer != null)
				{
					result = pointer.displayIndex.ReadValue();
				}
			}
			return result;
		}

		[Obsolete("maxRaycastDistance has been deprecated. Its value was unused, calling this property is unnecessary and should be removed.", true)]
		public float maxRaycastDistance
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		[HideInInspector]
		[SerializeField]
		private XRUIInputModule.ActiveInputMode m_ActiveInputMode;

		[Header("Input Devices")]
		[SerializeField]
		[Tooltip("If true, will forward 3D tracked device data to UI elements.")]
		private bool m_EnableXRInput = true;

		[SerializeField]
		[Tooltip("If true, will forward 2D mouse data to UI elements. Ignored when any Input System UI Actions are used.")]
		private bool m_EnableMouseInput = true;

		[SerializeField]
		[Tooltip("If true, will forward 2D touch data to UI elements. Ignored when any Input System UI Actions are used.")]
		private bool m_EnableTouchInput = true;

		[SerializeField]
		[Tooltip("If true, will forward gamepad data to UI elements. Ignored when any Input System UI Actions are used.")]
		private bool m_EnableGamepadInput = true;

		[SerializeField]
		[Tooltip("If true, will forward joystick data to UI elements. Ignored when any Input System UI Actions are used.")]
		private bool m_EnableJoystickInput = true;

		[Header("Input System UI Actions")]
		[SerializeField]
		[Tooltip("Pointer input action reference, such as a mouse or single-finger touch device.")]
		private InputActionReference m_PointAction;

		[SerializeField]
		[Tooltip("Left-click input action reference, typically the left button on a mouse.")]
		private InputActionReference m_LeftClickAction;

		[SerializeField]
		[Tooltip("Middle-click input action reference, typically the middle button on a mouse.")]
		private InputActionReference m_MiddleClickAction;

		[SerializeField]
		[Tooltip("Right-click input action reference, typically the right button on a mouse.")]
		private InputActionReference m_RightClickAction;

		[SerializeField]
		[Tooltip("Scroll wheel input action reference, typically the scroll wheel on a mouse.")]
		private InputActionReference m_ScrollWheelAction;

		[SerializeField]
		[Tooltip("Navigation input action reference will change which UI element is currently selected to the one up, down, left of or right of the currently selected one.")]
		private InputActionReference m_NavigateAction;

		[SerializeField]
		[Tooltip("Submit input action reference will trigger a submission of the currently selected UI in the Event System.")]
		private InputActionReference m_SubmitAction;

		[SerializeField]
		[Tooltip("Cancel input action reference will trigger canceling out of the currently selected UI in the Event System.")]
		private InputActionReference m_CancelAction;

		[SerializeField]
		[Tooltip("When enabled, built-in Input System actions will be used if no Input System UI Actions are assigned.")]
		private bool m_EnableBuiltinActionsAsFallback = true;

		[HideInInspector]
		[SerializeField]
		[Tooltip("Name of the horizontal axis for gamepad/joystick UI navigation when using the old Input Manager.")]
		private string m_HorizontalAxis = "Horizontal";

		[HideInInspector]
		[SerializeField]
		[Tooltip("Name of the vertical axis for gamepad/joystick UI navigation when using the old Input Manager.")]
		private string m_VerticalAxis = "Vertical";

		[HideInInspector]
		[SerializeField]
		[Tooltip("Name of the gamepad/joystick button to use for UI selection or submission when using the old Input Manager.")]
		private string m_SubmitButton = "Submit";

		[HideInInspector]
		[SerializeField]
		[Tooltip("Name of the gamepad/joystick button to use for UI cancel or back commands when using the old Input Manager.")]
		private string m_CancelButton = "Cancel";

		private int m_RollingPointerId = 1;

		private Stack<int> m_DeletedPointerIds = new Stack<int>();

		private bool m_UseBuiltInInputSystemActions;

		private PointerModel m_PointerState;

		private NavigationModel m_NavigationState;

		internal const float kPixelPerLine = 20f;

		private readonly List<XRUIInputModule.RegisteredTouch> m_RegisteredTouches = new List<XRUIInputModule.RegisteredTouch>();

		private readonly List<XRUIInputModule.RegisteredInteractor> m_RegisteredInteractors = new List<XRUIInputModule.RegisteredInteractor>();

		private readonly LinkedPool<UIHoverEventArgs> m_UIHoverEventArgs = new LinkedPool<UIHoverEventArgs>(() => new UIHoverEventArgs(), null, null, null, false, 10000);

		private struct RegisteredInteractor
		{
			public RegisteredInteractor(IUIInteractor interactor, int deviceIndex)
			{
				this.interactor = interactor;
				this.model = new TrackedDeviceModel(deviceIndex)
				{
					interactor = interactor
				};
				this.active = true;
				this.deactivating = false;
			}

			public IUIInteractor interactor;

			public TrackedDeviceModel model;

			internal bool deactivating;

			internal bool active;
		}

		private struct RegisteredTouch
		{
			public RegisteredTouch(Touch touch, int deviceIndex)
			{
				this.touchId = touch.fingerId;
				this.model = new TouchModel(deviceIndex);
				this.isValid = true;
			}

			public bool isValid;

			public int touchId;

			public TouchModel model;
		}

		public enum ActiveInputMode
		{
			InputManagerBindings,
			InputSystemActions,
			Both
		}
	}
}
