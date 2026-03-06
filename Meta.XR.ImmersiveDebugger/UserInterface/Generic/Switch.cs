using System;
using Meta.XR.ImmersiveDebugger.Manager;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Switch : ButtonWithIcon
	{
		internal Tweak Tweak { get; set; }

		public bool State
		{
			get
			{
				return this.Tweak != null && Math.Abs(this.Tweak.Tween - 1f) < Mathf.Epsilon;
			}
			set
			{
				this.Tweak.Tween = (value ? 1f : 0f);
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

		private void Start()
		{
			this.State = (this.Tweak.Tween > 0f);
			this.UpdateIcon();
		}

		internal void SetToggleIcons(Texture2D onState, Texture2D offState)
		{
			this._toggleIconOn = onState;
			this._toggleIconOff = offState;
		}

		protected override void UpdateIcon()
		{
			base.Icon = (this.State ? this._toggleIconOn : this._toggleIconOff);
			this._icon.Color = (base.Hover ? this._iconStyle.colorHover : (this.State ? this._iconStyle.color : this._iconStyle.colorOff));
			this._icon.RaycastTarget = (this._backgroundStyle == null || !this._backgroundStyle.enabled);
		}

		private Texture2D _toggleIconOn;

		private Texture2D _toggleIconOff;
	}
}
