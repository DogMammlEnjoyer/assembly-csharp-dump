using System;

namespace UnityEngine.InputSystem
{
	public static class InputExtensions
	{
		public static bool IsInProgress(this InputActionPhase phase)
		{
			return phase == InputActionPhase.Started || phase == InputActionPhase.Performed;
		}

		public static bool IsEndedOrCanceled(this TouchPhase phase)
		{
			return phase == TouchPhase.Canceled || phase == TouchPhase.Ended;
		}

		public static bool IsActive(this TouchPhase phase)
		{
			return phase - TouchPhase.Began <= 1 || phase == TouchPhase.Stationary;
		}

		public static bool IsModifierKey(this Key key)
		{
			return key - Key.LeftShift <= 7;
		}

		public static bool IsTextInputKey(this Key key)
		{
			return key > Key.Tab && key - Key.LeftShift > 26 && key - Key.F1 > 29;
		}
	}
}
