using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Liv.Lck.GorillaTag
{
	public class DroneGamepad
	{
		public event DroneGamepad.Gamepad2DMove OnMove;

		public event DroneGamepad.Gamepad2DMove OnTiltAndRotate;

		public event DroneGamepad.Gamepad1DMove OnMoveUpAndDown;

		public void Run()
		{
			Gamepad current = Gamepad.current;
			if (current == null)
			{
				return;
			}
			Vector2 move = current.leftStick.ReadValue();
			Vector2 move2 = current.rightStick.ReadValue();
			DroneGamepad.Gamepad2DMove onMove = this.OnMove;
			if (onMove != null)
			{
				onMove(move);
			}
			DroneGamepad.Gamepad2DMove onTiltAndRotate = this.OnTiltAndRotate;
			if (onTiltAndRotate != null)
			{
				onTiltAndRotate(move2);
			}
			float num = current.leftTrigger.ReadValue();
			float num2 = current.rightTrigger.ReadValue() - num;
			if (Mathf.Abs(num2) > 0.05f)
			{
				DroneGamepad.Gamepad1DMove onMoveUpAndDown = this.OnMoveUpAndDown;
				if (onMoveUpAndDown == null)
				{
					return;
				}
				onMoveUpAndDown(num2);
			}
		}

		public delegate void Gamepad2DMove(Vector2 move);

		public delegate void Gamepad1DMove(float move);
	}
}
