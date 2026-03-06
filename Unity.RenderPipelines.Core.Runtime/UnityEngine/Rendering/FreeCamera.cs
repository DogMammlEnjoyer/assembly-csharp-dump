using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.Rendering
{
	public class FreeCamera : MonoBehaviour
	{
		private void OnEnable()
		{
			this.RegisterInputs();
		}

		private void RegisterInputs()
		{
			InputActionMap map = new InputActionMap("Free Camera");
			this.lookAction = map.AddAction("look", InputActionType.Value, "<Mouse>/delta", null, null, null, null);
			this.moveAction = map.AddAction("move", InputActionType.Value, "<Gamepad>/leftStick", null, null, null, null);
			this.speedAction = map.AddAction("speed", InputActionType.Value, "<Gamepad>/dpad", null, null, null, null);
			this.yMoveAction = map.AddAction("yMove", InputActionType.Value, null, null, null, null, null);
			this.lookAction.AddBinding("<Gamepad>/rightStick", null, null, null).WithProcessor("scaleVector2(x=15, y=15)");
			this.moveAction.AddCompositeBinding("Dpad", null, null).With("Up", "<Keyboard>/w", null, null).With("Up", "<Keyboard>/upArrow", null, null).With("Down", "<Keyboard>/s", null, null).With("Down", "<Keyboard>/downArrow", null, null).With("Left", "<Keyboard>/a", null, null).With("Left", "<Keyboard>/leftArrow", null, null).With("Right", "<Keyboard>/d", null, null).With("Right", "<Keyboard>/rightArrow", null, null);
			this.speedAction.AddCompositeBinding("Dpad", null, null).With("Up", "<Keyboard>/home", null, null).With("Down", "<Keyboard>/end", null, null);
			this.yMoveAction.AddCompositeBinding("Dpad", null, null).With("Up", "<Keyboard>/pageUp", null, null).With("Down", "<Keyboard>/pageDown", null, null).With("Up", "<Keyboard>/e", null, null).With("Down", "<Keyboard>/q", null, null).With("Up", "<Gamepad>/rightshoulder", null, null).With("Down", "<Gamepad>/leftshoulder", null, null);
			this.moveAction.Enable();
			this.lookAction.Enable();
			this.speedAction.Enable();
			this.yMoveAction.Enable();
		}

		private void UpdateInputs()
		{
			this.inputRotateAxisX = 0f;
			this.inputRotateAxisY = 0f;
			this.leftShiftBoost = false;
			this.fire1 = false;
			Vector2 vector = this.lookAction.ReadValue<Vector2>();
			this.inputRotateAxisX = vector.x * this.m_LookSpeedMouse * 0.01f;
			this.inputRotateAxisY = vector.y * this.m_LookSpeedMouse * 0.01f;
			Keyboard current = Keyboard.current;
			bool? flag;
			if (current == null)
			{
				flag = null;
			}
			else
			{
				KeyControl leftShiftKey = current.leftShiftKey;
				flag = ((leftShiftKey != null) ? new bool?(leftShiftKey.isPressed) : null);
			}
			bool? flag2 = flag;
			this.leftShift = flag2.GetValueOrDefault();
			Mouse current2 = Mouse.current;
			bool flag3;
			if (current2 == null)
			{
				flag3 = false;
			}
			else
			{
				ButtonControl leftButton = current2.leftButton;
				flag2 = ((leftButton != null) ? new bool?(leftButton.isPressed) : null);
				bool flag4 = true;
				flag3 = (flag2.GetValueOrDefault() == flag4 & flag2 != null);
			}
			bool flag5;
			if (!flag3)
			{
				Gamepad current3 = Gamepad.current;
				if (current3 == null)
				{
					flag5 = false;
				}
				else
				{
					ButtonControl xButton = current3.xButton;
					flag2 = ((xButton != null) ? new bool?(xButton.isPressed) : null);
					bool flag4 = true;
					flag5 = (flag2.GetValueOrDefault() == flag4 & flag2 != null);
				}
			}
			else
			{
				flag5 = true;
			}
			this.fire1 = flag5;
			this.inputChangeSpeed = this.speedAction.ReadValue<Vector2>().y;
			Vector2 vector2 = this.moveAction.ReadValue<Vector2>();
			this.inputVertical = vector2.y;
			this.inputHorizontal = vector2.x;
			this.inputYAxis = this.yMoveAction.ReadValue<Vector2>().y;
		}

		private void Update()
		{
			if (DebugManager.instance.displayRuntimeUI)
			{
				return;
			}
			this.UpdateInputs();
			if (this.inputChangeSpeed != 0f)
			{
				this.m_MoveSpeed += this.inputChangeSpeed * this.m_MoveSpeedIncrement;
				if (this.m_MoveSpeed < this.m_MoveSpeedIncrement)
				{
					this.m_MoveSpeed = this.m_MoveSpeedIncrement;
				}
			}
			if (this.inputRotateAxisX != 0f || this.inputRotateAxisY != 0f || this.inputVertical != 0f || this.inputHorizontal != 0f || this.inputYAxis != 0f)
			{
				float x = base.transform.localEulerAngles.x;
				float y = base.transform.localEulerAngles.y + this.inputRotateAxisX;
				float num = x - this.inputRotateAxisY;
				if (x <= 90f && num >= 0f)
				{
					num = Mathf.Clamp(num, 0f, 90f);
				}
				if (x >= 270f)
				{
					num = Mathf.Clamp(num, 270f, 360f);
				}
				base.transform.localRotation = Quaternion.Euler(num, y, base.transform.localEulerAngles.z);
				float num2 = Time.deltaTime * this.m_MoveSpeed;
				if (this.fire1 || (this.leftShiftBoost && this.leftShift))
				{
					num2 *= this.m_Turbo;
				}
				base.transform.position += base.transform.forward * (num2 * this.inputVertical) + base.transform.right * (num2 * this.inputHorizontal) + Vector3.up * (num2 * this.inputYAxis);
			}
		}

		private const float k_MouseSensitivityMultiplier = 0.01f;

		public float m_LookSpeedController = 120f;

		public float m_LookSpeedMouse = 4f;

		public float m_MoveSpeed = 10f;

		public float m_MoveSpeedIncrement = 2.5f;

		public float m_Turbo = 10f;

		private InputAction lookAction;

		private InputAction moveAction;

		private InputAction speedAction;

		private InputAction yMoveAction;

		private float inputRotateAxisX;

		private float inputRotateAxisY;

		private float inputChangeSpeed;

		private float inputVertical;

		private float inputHorizontal;

		private float inputYAxis;

		private bool leftShiftBoost;

		private bool leftShift;

		private bool fire1;
	}
}
