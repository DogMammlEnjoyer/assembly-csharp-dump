using System;
using System.Diagnostics;
using UnityEngine.InputSystem.Controls;

namespace UnityEngine.InputSystem
{
	[DebuggerDisplay("Value = {Get()}")]
	public class InputValue
	{
		public object Get()
		{
			return this.m_Context.Value.ReadValueAsObject();
		}

		public TValue Get<TValue>() where TValue : struct
		{
			if (this.m_Context == null)
			{
				throw new InvalidOperationException("Values can only be retrieved while in message callbacks");
			}
			return this.m_Context.Value.ReadValue<TValue>();
		}

		public bool isPressed
		{
			get
			{
				return this.Get<float>() >= ButtonControl.s_GlobalDefaultButtonPressPoint;
			}
		}

		internal InputAction.CallbackContext? m_Context;
	}
}
