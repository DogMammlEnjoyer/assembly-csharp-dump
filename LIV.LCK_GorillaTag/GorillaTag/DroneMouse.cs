using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Liv.Lck.GorillaTag
{
	public class DroneMouse
	{
		public event DroneMouse.DroneMouseMoveEvent OnMouseMoveLeft;

		public event DroneMouse.DroneMouseMoveEvent OnMouseMoveRight;

		public event DroneMouse.DroneMouseEvent OnReset;

		public event DroneMouse.DroneMouseMiddleScrollEvent OnMouseMiddleScroll;

		public event DroneMouse.DroneMouseEvent OnMouseScrollUp;

		public event DroneMouse.DroneMouseEvent OnMouseScrollDown;

		public void Run()
		{
			Mouse current = Mouse.current;
			if (current == null)
			{
				return;
			}
			bool isPressed = current.leftButton.isPressed;
			bool isPressed2 = current.rightButton.isPressed;
			if (isPressed && !isPressed2)
			{
				Vector2 delta = current.delta.ReadValue();
				DroneMouse.DroneMouseMoveEvent onMouseMoveLeft = this.OnMouseMoveLeft;
				if (onMouseMoveLeft != null)
				{
					onMouseMoveLeft(delta);
				}
			}
			if (isPressed2 && !isPressed)
			{
				Vector2 delta2 = current.delta.ReadValue();
				DroneMouse.DroneMouseMoveEvent onMouseMoveRight = this.OnMouseMoveRight;
				if (onMouseMoveRight != null)
				{
					onMouseMoveRight(delta2);
				}
			}
			if (current.middleButton.wasPressedThisFrame)
			{
				DroneMouse.DroneMouseEvent onReset = this.OnReset;
				if (onReset != null)
				{
					onReset();
				}
			}
			if (current.middleButton.isPressed)
			{
				float y = current.scroll.ReadValue().y;
				DroneMouse.DroneMouseMiddleScrollEvent onMouseMiddleScroll = this.OnMouseMiddleScroll;
				if (onMouseMiddleScroll != null)
				{
					onMouseMiddleScroll(y);
				}
				if (Mathf.Approximately(y, 0f))
				{
					return;
				}
				if (y > 0f)
				{
					DroneMouse.DroneMouseEvent onMouseScrollUp = this.OnMouseScrollUp;
					if (onMouseScrollUp == null)
					{
						return;
					}
					onMouseScrollUp();
					return;
				}
				else if (y < 0f)
				{
					DroneMouse.DroneMouseEvent onMouseScrollDown = this.OnMouseScrollDown;
					if (onMouseScrollDown == null)
					{
						return;
					}
					onMouseScrollDown();
				}
			}
		}

		public delegate void DroneMouseEvent();

		public delegate void DroneMouseMoveEvent(Vector2 delta);

		public delegate void DroneMouseMiddleScrollEvent(float delta);
	}
}
