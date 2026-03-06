using System;
using Meta.XR.ImmersiveDebugger.Manager;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	internal class ButtonForAction : ButtonWithLabel
	{
		internal ActionHook Action
		{
			get
			{
				return this._hook;
			}
			set
			{
				this._hook = value;
				base.Callback = value.Delegate;
			}
		}

		private ActionHook _hook;
	}
}
