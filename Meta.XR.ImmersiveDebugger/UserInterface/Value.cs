using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class Value : Controller
	{
		internal Background Background
		{
			get
			{
				return this._background;
			}
		}

		internal Label Label
		{
			get
			{
				return this._label;
			}
		}

		internal ImageStyle BackgroundStyle
		{
			set
			{
				this._backgroundStyle = value;
				this._background.Sprite = value.sprite;
				this._background.PixelDensityMultiplier = value.pixelDensityMultiplier;
				this.RefreshStyle();
			}
		}

		internal TextStyle TextStyle
		{
			get
			{
				return this._label.TextStyle;
			}
			set
			{
				this._label.TextStyle = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._background = base.Append<Background>("background");
			this._background.LayoutStyle = Style.Instantiate<LayoutStyle>("Fill");
			this._label = base.Append<Label>("label");
			this._label.LayoutStyle = Style.Instantiate<LayoutStyle>("Fill");
		}

		protected void RefreshStyle()
		{
			this.UpdateBackground();
		}

		protected virtual void UpdateBackground()
		{
			if (this._backgroundStyle != null && this._backgroundStyle.enabled)
			{
				this._background.Show();
				this._background.Color = (base.Transparent ? this._backgroundStyle.colorOff : this._backgroundStyle.color);
				return;
			}
			this._background.Hide();
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this.RefreshStyle();
		}

		internal virtual string Content
		{
			get
			{
				return this._label.Content;
			}
			set
			{
				this._label.Content = value;
			}
		}

		protected Label _label;

		protected Background _background;

		protected ImageStyle _backgroundStyle;
	}
}
