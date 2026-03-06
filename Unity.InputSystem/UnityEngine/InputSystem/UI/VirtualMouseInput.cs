using System;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

namespace UnityEngine.InputSystem.UI
{
	[AddComponentMenu("Input/Virtual Mouse")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/UISupport.html#virtual-mouse-cursor-control")]
	public class VirtualMouseInput : MonoBehaviour
	{
		public RectTransform cursorTransform
		{
			get
			{
				return this.m_CursorTransform;
			}
			set
			{
				this.m_CursorTransform = value;
			}
		}

		public float cursorSpeed
		{
			get
			{
				return this.m_CursorSpeed;
			}
			set
			{
				this.m_CursorSpeed = value;
			}
		}

		public VirtualMouseInput.CursorMode cursorMode
		{
			get
			{
				return this.m_CursorMode;
			}
			set
			{
				if (this.m_CursorMode == value)
				{
					return;
				}
				if (this.m_CursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable && this.m_SystemMouse != null)
				{
					InputSystem.EnableDevice(this.m_SystemMouse);
					this.m_SystemMouse = null;
				}
				this.m_CursorMode = value;
				if (this.m_CursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable)
				{
					this.TryEnableHardwareCursor();
					return;
				}
				if (this.m_CursorGraphic != null)
				{
					this.m_CursorGraphic.enabled = true;
				}
			}
		}

		public Graphic cursorGraphic
		{
			get
			{
				return this.m_CursorGraphic;
			}
			set
			{
				this.m_CursorGraphic = value;
				this.TryFindCanvas();
			}
		}

		public float scrollSpeed
		{
			get
			{
				return this.m_ScrollSpeed;
			}
			set
			{
				this.m_ScrollSpeed = value;
			}
		}

		public Mouse virtualMouse
		{
			get
			{
				return this.m_VirtualMouse;
			}
		}

		public InputActionProperty stickAction
		{
			get
			{
				return this.m_StickAction;
			}
			set
			{
				VirtualMouseInput.SetAction(ref this.m_StickAction, value);
			}
		}

		public InputActionProperty leftButtonAction
		{
			get
			{
				return this.m_LeftButtonAction;
			}
			set
			{
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_LeftButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				}
				VirtualMouseInput.SetAction(ref this.m_LeftButtonAction, value);
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_LeftButtonAction, this.m_ButtonActionTriggeredDelegate, true);
				}
			}
		}

		public InputActionProperty rightButtonAction
		{
			get
			{
				return this.m_RightButtonAction;
			}
			set
			{
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_RightButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				}
				VirtualMouseInput.SetAction(ref this.m_RightButtonAction, value);
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_RightButtonAction, this.m_ButtonActionTriggeredDelegate, true);
				}
			}
		}

		public InputActionProperty middleButtonAction
		{
			get
			{
				return this.m_MiddleButtonAction;
			}
			set
			{
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_MiddleButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				}
				VirtualMouseInput.SetAction(ref this.m_MiddleButtonAction, value);
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_MiddleButtonAction, this.m_ButtonActionTriggeredDelegate, true);
				}
			}
		}

		public InputActionProperty forwardButtonAction
		{
			get
			{
				return this.m_ForwardButtonAction;
			}
			set
			{
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_ForwardButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				}
				VirtualMouseInput.SetAction(ref this.m_ForwardButtonAction, value);
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_ForwardButtonAction, this.m_ButtonActionTriggeredDelegate, true);
				}
			}
		}

		public InputActionProperty backButtonAction
		{
			get
			{
				return this.m_BackButtonAction;
			}
			set
			{
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_BackButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				}
				VirtualMouseInput.SetAction(ref this.m_BackButtonAction, value);
				if (this.m_ButtonActionTriggeredDelegate != null)
				{
					VirtualMouseInput.SetActionCallback(this.m_BackButtonAction, this.m_ButtonActionTriggeredDelegate, true);
				}
			}
		}

		public InputActionProperty scrollWheelAction
		{
			get
			{
				return this.m_ScrollWheelAction;
			}
			set
			{
				VirtualMouseInput.SetAction(ref this.m_ScrollWheelAction, value);
			}
		}

		protected void OnEnable()
		{
			if (this.m_CursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable)
			{
				this.TryEnableHardwareCursor();
			}
			if (this.m_VirtualMouse == null)
			{
				this.m_VirtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse", null, null);
			}
			else if (!this.m_VirtualMouse.added)
			{
				InputSystem.AddDevice(this.m_VirtualMouse);
			}
			if (this.m_CursorTransform != null)
			{
				Vector2 anchoredPosition = this.m_CursorTransform.anchoredPosition;
				InputState.Change<Vector2>(this.m_VirtualMouse.position, anchoredPosition, InputUpdateType.None, default(InputEventPtr));
				Mouse systemMouse = this.m_SystemMouse;
				if (systemMouse != null)
				{
					systemMouse.WarpCursorPosition(anchoredPosition);
				}
			}
			if (this.m_AfterInputUpdateDelegate == null)
			{
				this.m_AfterInputUpdateDelegate = new Action(this.OnAfterInputUpdate);
			}
			InputSystem.onAfterUpdate += this.m_AfterInputUpdateDelegate;
			if (this.m_ButtonActionTriggeredDelegate == null)
			{
				this.m_ButtonActionTriggeredDelegate = new Action<InputAction.CallbackContext>(this.OnButtonActionTriggered);
			}
			VirtualMouseInput.SetActionCallback(this.m_LeftButtonAction, this.m_ButtonActionTriggeredDelegate, true);
			VirtualMouseInput.SetActionCallback(this.m_RightButtonAction, this.m_ButtonActionTriggeredDelegate, true);
			VirtualMouseInput.SetActionCallback(this.m_MiddleButtonAction, this.m_ButtonActionTriggeredDelegate, true);
			VirtualMouseInput.SetActionCallback(this.m_ForwardButtonAction, this.m_ButtonActionTriggeredDelegate, true);
			VirtualMouseInput.SetActionCallback(this.m_BackButtonAction, this.m_ButtonActionTriggeredDelegate, true);
			InputAction action = this.m_StickAction.action;
			if (action != null)
			{
				action.Enable();
			}
			InputAction action2 = this.m_LeftButtonAction.action;
			if (action2 != null)
			{
				action2.Enable();
			}
			InputAction action3 = this.m_RightButtonAction.action;
			if (action3 != null)
			{
				action3.Enable();
			}
			InputAction action4 = this.m_MiddleButtonAction.action;
			if (action4 != null)
			{
				action4.Enable();
			}
			InputAction action5 = this.m_ForwardButtonAction.action;
			if (action5 != null)
			{
				action5.Enable();
			}
			InputAction action6 = this.m_BackButtonAction.action;
			if (action6 != null)
			{
				action6.Enable();
			}
			InputAction action7 = this.m_ScrollWheelAction.action;
			if (action7 == null)
			{
				return;
			}
			action7.Enable();
		}

		protected void OnDisable()
		{
			if (this.m_VirtualMouse != null && this.m_VirtualMouse.added)
			{
				InputSystem.RemoveDevice(this.m_VirtualMouse);
			}
			if (this.m_SystemMouse != null)
			{
				InputSystem.EnableDevice(this.m_SystemMouse);
				this.m_SystemMouse = null;
			}
			if (this.m_AfterInputUpdateDelegate != null)
			{
				InputSystem.onAfterUpdate -= this.m_AfterInputUpdateDelegate;
			}
			InputAction action = this.m_StickAction.action;
			if (action != null)
			{
				action.Disable();
			}
			InputAction action2 = this.m_LeftButtonAction.action;
			if (action2 != null)
			{
				action2.Disable();
			}
			InputAction action3 = this.m_RightButtonAction.action;
			if (action3 != null)
			{
				action3.Disable();
			}
			InputAction action4 = this.m_MiddleButtonAction.action;
			if (action4 != null)
			{
				action4.Disable();
			}
			InputAction action5 = this.m_ForwardButtonAction.action;
			if (action5 != null)
			{
				action5.Disable();
			}
			InputAction action6 = this.m_BackButtonAction.action;
			if (action6 != null)
			{
				action6.Disable();
			}
			InputAction action7 = this.m_ScrollWheelAction.action;
			if (action7 != null)
			{
				action7.Disable();
			}
			if (this.m_ButtonActionTriggeredDelegate != null)
			{
				VirtualMouseInput.SetActionCallback(this.m_LeftButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				VirtualMouseInput.SetActionCallback(this.m_RightButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				VirtualMouseInput.SetActionCallback(this.m_MiddleButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				VirtualMouseInput.SetActionCallback(this.m_ForwardButtonAction, this.m_ButtonActionTriggeredDelegate, false);
				VirtualMouseInput.SetActionCallback(this.m_BackButtonAction, this.m_ButtonActionTriggeredDelegate, false);
			}
			this.m_LastTime = 0.0;
			this.m_LastStickValue = default(Vector2);
		}

		private void TryFindCanvas()
		{
			Graphic cursorGraphic = this.m_CursorGraphic;
			this.m_Canvas = ((cursorGraphic != null) ? cursorGraphic.GetComponentInParent<Canvas>() : null);
		}

		private unsafe void TryEnableHardwareCursor()
		{
			ReadOnlyArray<InputDevice> devices = InputSystem.devices;
			for (int i = 0; i < devices.Count; i++)
			{
				InputDevice inputDevice = devices[i];
				if (inputDevice.native)
				{
					Mouse mouse = inputDevice as Mouse;
					if (mouse != null)
					{
						this.m_SystemMouse = mouse;
						break;
					}
				}
			}
			if (this.m_SystemMouse == null)
			{
				if (this.m_CursorGraphic != null)
				{
					this.m_CursorGraphic.enabled = true;
				}
				return;
			}
			InputSystem.DisableDevice(this.m_SystemMouse, false);
			if (this.m_VirtualMouse != null)
			{
				this.m_SystemMouse.WarpCursorPosition(*this.m_VirtualMouse.position.value);
			}
			if (this.m_CursorGraphic != null)
			{
				this.m_CursorGraphic.enabled = false;
			}
		}

		private unsafe void UpdateMotion()
		{
			if (this.m_VirtualMouse == null)
			{
				return;
			}
			InputAction action = this.m_StickAction.action;
			if (action == null)
			{
				return;
			}
			Vector2 vector = action.ReadValue<Vector2>();
			if (Mathf.Approximately(0f, vector.x) && Mathf.Approximately(0f, vector.y))
			{
				this.m_LastTime = 0.0;
				this.m_LastStickValue = default(Vector2);
			}
			else
			{
				double currentTime = InputState.currentTime;
				if (Mathf.Approximately(0f, this.m_LastStickValue.x) && Mathf.Approximately(0f, this.m_LastStickValue.y))
				{
					this.m_LastTime = currentTime;
				}
				float num = (float)(currentTime - this.m_LastTime);
				Vector2 vector2 = new Vector2(this.m_CursorSpeed * vector.x * num, this.m_CursorSpeed * vector.y * num);
				Vector2 vector3 = *this.m_VirtualMouse.position.value + vector2;
				if (this.m_Canvas != null)
				{
					Rect pixelRect = this.m_Canvas.pixelRect;
					vector3.x = Mathf.Clamp(vector3.x, pixelRect.xMin, pixelRect.xMax);
					vector3.y = Mathf.Clamp(vector3.y, pixelRect.yMin, pixelRect.yMax);
				}
				InputState.Change<Vector2>(this.m_VirtualMouse.position, vector3, InputUpdateType.None, default(InputEventPtr));
				InputState.Change<Vector2>(this.m_VirtualMouse.delta, vector2, InputUpdateType.None, default(InputEventPtr));
				if (this.m_CursorTransform != null && (this.m_CursorMode == VirtualMouseInput.CursorMode.SoftwareCursor || (this.m_CursorMode == VirtualMouseInput.CursorMode.HardwareCursorIfAvailable && this.m_SystemMouse == null)))
				{
					this.m_CursorTransform.anchoredPosition = vector3;
				}
				this.m_LastStickValue = vector;
				this.m_LastTime = currentTime;
				Mouse systemMouse = this.m_SystemMouse;
				if (systemMouse != null)
				{
					systemMouse.WarpCursorPosition(vector3);
				}
			}
			InputAction action2 = this.m_ScrollWheelAction.action;
			if (action2 != null)
			{
				Vector2 state = action2.ReadValue<Vector2>();
				state.x *= this.m_ScrollSpeed;
				state.y *= this.m_ScrollSpeed;
				InputState.Change<Vector2>(this.m_VirtualMouse.scroll, state, InputUpdateType.None, default(InputEventPtr));
			}
		}

		private void OnButtonActionTriggered(InputAction.CallbackContext context)
		{
			if (this.m_VirtualMouse == null)
			{
				return;
			}
			InputAction action = context.action;
			MouseButton? mouseButton = null;
			if (action == this.m_LeftButtonAction.action)
			{
				mouseButton = new MouseButton?(MouseButton.Left);
			}
			else if (action == this.m_RightButtonAction.action)
			{
				mouseButton = new MouseButton?(MouseButton.Right);
			}
			else if (action == this.m_MiddleButtonAction.action)
			{
				mouseButton = new MouseButton?(MouseButton.Middle);
			}
			else if (action == this.m_ForwardButtonAction.action)
			{
				mouseButton = new MouseButton?(MouseButton.Forward);
			}
			else if (action == this.m_BackButtonAction.action)
			{
				mouseButton = new MouseButton?(MouseButton.Back);
			}
			if (mouseButton != null)
			{
				bool state = context.control.IsPressed(0f);
				MouseState state2;
				this.m_VirtualMouse.CopyState(out state2);
				state2.WithButton(mouseButton.Value, state);
				InputState.Change<MouseState>(this.m_VirtualMouse, state2, InputUpdateType.None, default(InputEventPtr));
			}
		}

		private static void SetActionCallback(InputActionProperty field, Action<InputAction.CallbackContext> callback, bool install = true)
		{
			InputAction action = field.action;
			if (action == null)
			{
				return;
			}
			if (install)
			{
				action.started += callback;
				action.canceled += callback;
				return;
			}
			action.started -= callback;
			action.canceled -= callback;
		}

		private static void SetAction(ref InputActionProperty field, InputActionProperty value)
		{
			InputActionProperty inputActionProperty = field;
			field = value;
			if (inputActionProperty.reference == null)
			{
				InputAction action = inputActionProperty.action;
				if (action != null && action.enabled)
				{
					action.Disable();
					if (value.reference == null)
					{
						InputAction action2 = value.action;
						if (action2 == null)
						{
							return;
						}
						action2.Enable();
					}
				}
			}
		}

		private void OnAfterInputUpdate()
		{
			this.UpdateMotion();
		}

		[Header("Cursor")]
		[Tooltip("Whether the component should set the cursor position of the hardware mouse cursor, if one is available. If so, the software cursor pointed (to by 'Cursor Graphic') will be hidden.")]
		[SerializeField]
		private VirtualMouseInput.CursorMode m_CursorMode;

		[Tooltip("The graphic that represents the software cursor. This is hidden if a hardware cursor (see 'Cursor Mode') is used.")]
		[SerializeField]
		private Graphic m_CursorGraphic;

		[Tooltip("The transform for the software cursor. Will only be set if a software cursor is used (see 'Cursor Mode'). Moving the cursor updates the anchored position of the transform.")]
		[SerializeField]
		private RectTransform m_CursorTransform;

		[Header("Motion")]
		[Tooltip("Speed in pixels per second with which to move the cursor. Scaled by the input from 'Stick Action'.")]
		[SerializeField]
		private float m_CursorSpeed = 400f;

		[Tooltip("Scale factor to apply to 'Scroll Wheel Action' when setting the mouse 'scrollWheel' control.")]
		[SerializeField]
		private float m_ScrollSpeed = 45f;

		[Space(10f)]
		[Tooltip("Vector2 action that moves the cursor left/right (X) and up/down (Y) on screen.")]
		[SerializeField]
		private InputActionProperty m_StickAction;

		[Tooltip("Button action that triggers a left-click on the mouse.")]
		[SerializeField]
		private InputActionProperty m_LeftButtonAction;

		[Tooltip("Button action that triggers a middle-click on the mouse.")]
		[SerializeField]
		private InputActionProperty m_MiddleButtonAction;

		[Tooltip("Button action that triggers a right-click on the mouse.")]
		[SerializeField]
		private InputActionProperty m_RightButtonAction;

		[Tooltip("Button action that triggers a forward button (button #4) click on the mouse.")]
		[SerializeField]
		private InputActionProperty m_ForwardButtonAction;

		[Tooltip("Button action that triggers a back button (button #5) click on the mouse.")]
		[SerializeField]
		private InputActionProperty m_BackButtonAction;

		[Tooltip("Vector2 action that feeds into the mouse 'scrollWheel' action (scaled by 'Scroll Speed').")]
		[SerializeField]
		private InputActionProperty m_ScrollWheelAction;

		private Canvas m_Canvas;

		private Mouse m_VirtualMouse;

		private Mouse m_SystemMouse;

		private Action m_AfterInputUpdateDelegate;

		private Action<InputAction.CallbackContext> m_ButtonActionTriggeredDelegate;

		private double m_LastTime;

		private Vector2 m_LastStickValue;

		public enum CursorMode
		{
			SoftwareCursor,
			HardwareCursorIfAvailable
		}
	}
}
