using System;
using UnityEngine.InputSystem;

namespace Liv.Lck.GorillaTag
{
	public class DroneGeneralKeyboard
	{
		public event DroneGeneralKeyboard.DroneKeyboardEvent OnShowUI;

		public event DroneGeneralKeyboard.DroneKeyboardEvent OnShiftPressed;

		public event DroneGeneralKeyboard.DroneKeyboardEvent OnShiftReleased;

		public void Run()
		{
			Keyboard current = Keyboard.current;
			if (current == null)
			{
				return;
			}
			if (current.tabKey.wasPressedThisFrame)
			{
				DroneGeneralKeyboard.DroneKeyboardEvent onShowUI = this.OnShowUI;
				if (onShowUI != null)
				{
					onShowUI();
				}
			}
			if (current.shiftKey.wasPressedThisFrame)
			{
				DroneGeneralKeyboard.DroneKeyboardEvent onShiftPressed = this.OnShiftPressed;
				if (onShiftPressed != null)
				{
					onShiftPressed();
				}
			}
			if (current.shiftKey.wasReleasedThisFrame)
			{
				DroneGeneralKeyboard.DroneKeyboardEvent onShiftReleased = this.OnShiftReleased;
				if (onShiftReleased == null)
				{
					return;
				}
				onShiftReleased();
			}
		}

		public delegate void DroneKeyboardEvent();
	}
}
