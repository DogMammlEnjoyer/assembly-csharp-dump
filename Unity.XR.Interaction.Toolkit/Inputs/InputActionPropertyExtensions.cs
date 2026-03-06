using System;
using UnityEngine.InputSystem;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs
{
	public static class InputActionPropertyExtensions
	{
		public static void EnableDirectAction(this InputActionProperty property)
		{
			if (property.reference != null)
			{
				return;
			}
			InputAction action = property.action;
			if (action == null)
			{
				return;
			}
			action.Enable();
		}

		public static void DisableDirectAction(this InputActionProperty property)
		{
			if (property.reference != null)
			{
				return;
			}
			InputAction action = property.action;
			if (action == null)
			{
				return;
			}
			action.Disable();
		}
	}
}
