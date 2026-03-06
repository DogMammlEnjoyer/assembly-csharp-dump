using System;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class ButtonWithLabel : Button
	{
		public Background Background
		{
			get
			{
				return this._background;
			}
		}

		public ImageStyle BackgroundStyle
		{
			set
			{
				this._backgroundStyle = value;
				this._background.Sprite = value.sprite;
				this._background.PixelDensityMultiplier = value.pixelDensityMultiplier;
				this.RefreshStyle();
			}
		}

		public TextStyle TextStyle
		{
			set
			{
				this._label.TextStyle = value;
			}
		}

		public LayoutStyle LabelLayoutStyle
		{
			set
			{
				this._label.LayoutStyle = value;
			}
		}

		public string Label
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

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._background = base.Append<Background>("background");
			this._background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this._label = base.Append<Label>("label");
			this._label.LayoutStyle = Style.Load<LayoutStyle>("Fill");
		}

		protected override void OnHoverChanged()
		{
			base.OnHoverChanged();
			this.RefreshStyle();
		}

		protected void RefreshStyle()
		{
			this.UpdateBackground();
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._backgroundStyle.colorHover.a = (base.Transparent ? 0.6f : 1f);
			this._background.Color = (base.Transparent ? this._backgroundStyle.colorOff : this._backgroundStyle.color);
		}

		protected virtual void UpdateBackground()
		{
			if (this._backgroundStyle != null && this._backgroundStyle.enabled)
			{
				this._background.Show();
				this._background.Color = (base.Hover ? this._backgroundStyle.colorHover : (base.Transparent ? this._backgroundStyle.colorOff : this._backgroundStyle.color));
				this._background.RaycastTarget = true;
				return;
			}
			this._background.Hide();
		}

		protected Label _label;

		protected Background _background;

		protected ImageStyle _backgroundStyle;
	}
}
