using System;
using UnityEngine.InputSystem;

namespace Liv.Lck.GorillaTag
{
	public class DroneKeyboard
	{
		public event DroneKeyboard.DroneKeyboardEvent OnMoveForward;

		public event DroneKeyboard.DroneKeyboardEvent OnMoveBackward;

		public event DroneKeyboard.DroneKeyboardEvent OnMoveLeft;

		public event DroneKeyboard.DroneKeyboardEvent OnMoveRight;

		public event DroneKeyboard.DroneKeyboardEvent OnMoveUp;

		public event DroneKeyboard.DroneKeyboardEvent OnMoveDown;

		public event DroneKeyboard.DroneKeyboardEvent OnRotateLeft;

		public event DroneKeyboard.DroneKeyboardEvent OnRotateRight;

		public event DroneKeyboard.DroneKeyboardEvent OnTiltUp;

		public event DroneKeyboard.DroneKeyboardEvent OnTiltDown;

		public event DroneKeyboard.DroneKeyboardEvent OnBurstStarted;

		public event DroneKeyboard.DroneKeyboardEvent OnBurstEnded;

		public void Run()
		{
			Keyboard current = Keyboard.current;
			if (current == null)
			{
				return;
			}
			if (current.wKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveForward = this.OnMoveForward;
				if (onMoveForward != null)
				{
					onMoveForward();
				}
			}
			else if (current.sKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveBackward = this.OnMoveBackward;
				if (onMoveBackward != null)
				{
					onMoveBackward();
				}
			}
			if (current.aKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveLeft = this.OnMoveLeft;
				if (onMoveLeft != null)
				{
					onMoveLeft();
				}
			}
			else if (current.dKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveRight = this.OnMoveRight;
				if (onMoveRight != null)
				{
					onMoveRight();
				}
			}
			if (current.qKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveDown = this.OnMoveDown;
				if (onMoveDown != null)
				{
					onMoveDown();
				}
			}
			else if (current.eKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onMoveUp = this.OnMoveUp;
				if (onMoveUp != null)
				{
					onMoveUp();
				}
			}
			if (current.leftArrowKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onRotateLeft = this.OnRotateLeft;
				if (onRotateLeft != null)
				{
					onRotateLeft();
				}
			}
			else if (current.rightArrowKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onRotateRight = this.OnRotateRight;
				if (onRotateRight != null)
				{
					onRotateRight();
				}
			}
			if (current.upArrowKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onTiltUp = this.OnTiltUp;
				if (onTiltUp != null)
				{
					onTiltUp();
				}
			}
			else if (current.downArrowKey.isPressed)
			{
				DroneKeyboard.DroneKeyboardEvent onTiltDown = this.OnTiltDown;
				if (onTiltDown != null)
				{
					onTiltDown();
				}
			}
			if (current.spaceKey.wasPressedThisFrame)
			{
				DroneKeyboard.DroneKeyboardEvent onBurstStarted = this.OnBurstStarted;
				if (onBurstStarted != null)
				{
					onBurstStarted();
				}
			}
			if (current.spaceKey.wasReleasedThisFrame)
			{
				DroneKeyboard.DroneKeyboardEvent onBurstEnded = this.OnBurstEnded;
				if (onBurstEnded == null)
				{
					return;
				}
				onBurstEnded();
			}
		}

		public delegate void DroneKeyboardEvent();
	}
}
