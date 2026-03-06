using System;
using Meta.XR.ImmersiveDebugger.Manager;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	internal class ToggleForGizmo : Toggle
	{
		public GizmoHook Hook
		{
			get
			{
				return this._hook;
			}
			set
			{
				if (this._hook == value)
				{
					return;
				}
				this._hook = value;
				base.StateChanged = null;
				Func<bool> getState = value.GetState;
				base.State = (getState != null && getState());
				base.StateChanged = value.SetState;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			base.Callback = delegate()
			{
				base.State = !base.State;
			};
		}

		private GizmoHook _hook;
	}
}
