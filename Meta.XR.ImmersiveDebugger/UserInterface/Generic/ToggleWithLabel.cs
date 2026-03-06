using System;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class ToggleWithLabel : ButtonWithLabel
	{
		public bool State
		{
			get
			{
				return this._state;
			}
			set
			{
				if (this._state == value)
				{
					return;
				}
				this._state = value;
				this.OnStateChanged();
			}
		}

		public Action<bool> StateChanged { get; set; }

		private void OnStateChanged()
		{
			Action<bool> stateChanged = this.StateChanged;
			if (stateChanged != null)
			{
				stateChanged(this.State);
			}
			base.RefreshStyle();
		}

		protected override void UpdateBackground()
		{
			if (this._backgroundStyle != null && this._backgroundStyle.enabled)
			{
				this._background.Show();
				this._background.Color = (base.Hover ? this._backgroundStyle.colorHover : (this.State ? this._backgroundStyle.colorHover : this._backgroundStyle.color));
				this._background.RaycastTarget = true;
				return;
			}
			this._background.Hide();
		}

		private bool _state;
	}
}
