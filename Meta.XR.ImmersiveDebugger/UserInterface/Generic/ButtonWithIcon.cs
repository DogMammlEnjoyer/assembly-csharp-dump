using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class ButtonWithIcon : Button
	{
		public ImageStyle BackgroundStyle
		{
			get
			{
				return this._backgroundStyle;
			}
			set
			{
				if (this._backgroundStyle == value)
				{
					return;
				}
				this._backgroundStyle = value;
				this._background.Sprite = this._backgroundStyle.sprite;
				this._background.PixelDensityMultiplier = this._backgroundStyle.pixelDensityMultiplier;
				this.RefreshStyle();
			}
		}

		public ImageStyle IconStyle
		{
			set
			{
				this._iconStyle = value;
				this.RefreshStyle();
			}
		}

		public Texture2D Icon
		{
			set
			{
				this._icon.Texture = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._background = base.Append<Background>("background");
			this._background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this._icon = base.Append<Icon>("icon");
			this._icon.LayoutStyle = Style.Load<LayoutStyle>("Fill");
		}

		protected override void OnHoverChanged()
		{
			base.OnHoverChanged();
			this.RefreshStyle();
		}

		protected void RefreshStyle()
		{
			this.UpdateBackground();
			this.UpdateIcon();
		}

		protected virtual void UpdateBackground()
		{
			if (this._backgroundStyle != null && this._backgroundStyle.enabled)
			{
				this._background.Show();
				this._background.Color = (base.Hover ? this._backgroundStyle.colorHover : this._backgroundStyle.color);
				this._background.RaycastTarget = true;
				return;
			}
			this._background.Hide();
		}

		protected virtual void UpdateIcon()
		{
			if (this._iconStyle != null && this._iconStyle.enabled)
			{
				this._icon.Show();
				this._icon.Color = (base.Hover ? this._iconStyle.colorHover : this._iconStyle.color);
				this._icon.RaycastTarget = (this._backgroundStyle == null || !this._backgroundStyle.enabled);
				return;
			}
			this._icon.Hide();
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			if (this.BackgroundStyle == null || !this.BackgroundStyle.enabled)
			{
				return;
			}
			this.BackgroundStyle.color.a = (base.Transparent ? 0.25f : 1f);
			this.BackgroundStyle.colorHover.a = (base.Transparent ? 0.5f : 1f);
			this.RefreshStyle();
		}

		protected Icon _icon;

		protected Background _background;

		protected ImageStyle _backgroundStyle;

		protected ImageStyle _iconStyle;
	}
}
