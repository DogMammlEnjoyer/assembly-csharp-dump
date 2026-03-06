using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class ConsoleLine : InteractableController
	{
		internal LogEntry Entry
		{
			get
			{
				return this._entry;
			}
			set
			{
				if (this._entry == value)
				{
					return;
				}
				this._entry = value;
				this.Label = Utils.ClampText(value.Label, 116);
				this.PillStyle = value.Severity.PillStyle;
				this.RefreshLogCounter();
			}
		}

		internal string Label
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

		internal ImageStyle BackgroundStyle
		{
			set
			{
				this._background.Sprite = value.sprite;
				this._background.Color = value.color;
				this._background.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		internal ImageStyle PillStyle
		{
			set
			{
				this._pill.Sprite = value.sprite;
				this._pill.Color = value.color;
				this._pill.PixelDensityMultiplier = value.pixelDensityMultiplier;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._background = base.Append<Background>("background");
			this._background.LayoutStyle = Style.Load<LayoutStyle>("Fill");
			this._backgroundImageStyle = Style.Load<ImageStyle>("ConsoleLineBackground");
			this.BackgroundStyle = this._backgroundImageStyle;
			this._background.RaycastTarget = true;
			this._flex = base.Append<Flex>("line");
			this._flex.LayoutStyle = Style.Load<LayoutStyle>("ConsoleLineFlex");
			this._pill = this._flex.Append<Background>("pill");
			this._pill.LayoutStyle = Style.Load<LayoutStyle>("PillVertical");
			this._label = this._flex.Append<Label>("log");
			this._label.LayoutStyle = Style.Load<LayoutStyle>("ConsoleLineLabel");
			this._label.TextStyle = Style.Load<TextStyle>("ConsoleLineLabel");
			this._label.Text.verticalOverflow = VerticalWrapMode.Truncate;
			this._counterBackground = this._flex.Append<Background>("counterbackground");
			this._counterBackground.LayoutStyle = Object.Instantiate<LayoutStyle>(Style.Load<LayoutStyle>("MiniCounter"));
			ImageStyle imageStyle = Style.Load<ImageStyle>("MiniCounter");
			this._counterBackground.Sprite = imageStyle.sprite;
			this._counterBackground.Color = imageStyle.color;
			this._counterBackground.PixelDensityMultiplier = imageStyle.pixelDensityMultiplier;
			this._counterLabel = this._counterBackground.Append<Label>("counter");
			this._counterLabel.LayoutStyle = Object.Instantiate<LayoutStyle>(Style.Load<LayoutStyle>("MiniCounterValue"));
			this._counterLabel.TextStyle = Style.Load<TextStyle>("ConsoleLogCounter");
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this._backgroundImageStyle.colorHover.a = (base.Transparent ? 0.6f : 1f);
			this._background.Color = (base.Transparent ? this._backgroundImageStyle.colorOff : this._backgroundImageStyle.color);
		}

		private void RefreshLogCounter()
		{
			if (this._counterBackground == null || this._counterLabel == null)
			{
				return;
			}
			bool logCollapseMode = this.Entry.Severity.Owner.LogCollapseMode;
			bool flag = this.Entry.Count > 1 && logCollapseMode;
			this.ShowCounter(flag);
			if (flag)
			{
				this._counterLabel.Content = this.Entry.Count.ToString();
				this._counterBackground.LayoutStyle.size.x = Mathf.Clamp(this._counterLabel.Text.preferredWidth + 8f, 16f, 64f);
				this._counterBackground.RefreshLayout();
			}
			this._label.RefreshLayout();
		}

		private void ShowCounter(bool show = true)
		{
			if (show)
			{
				this._counterBackground.Show();
				this._counterLabel.Show();
				return;
			}
			this._counterBackground.Hide();
			this._counterLabel.Hide();
		}

		public override void OnPointerClick()
		{
			LogEntry entry = this.Entry;
			if (entry == null)
			{
				return;
			}
			entry.DisplayDetails();
		}

		protected override void OnHoverChanged()
		{
			base.OnHoverChanged();
			this._background.Color = (base.Hover ? this._backgroundImageStyle.colorHover : (base.Transparent ? this._backgroundImageStyle.colorOff : this._backgroundImageStyle.color));
		}

		private Label _label;

		private Flex _flex;

		private Background _background;

		private Background _pill;

		private LogEntry _entry;

		internal UnityEvent<LogEntry> OnClick = new UnityEvent<LogEntry>();

		private Label _counterLabel;

		private Background _counterBackground;

		private ImageStyle _backgroundImageStyle;

		private const int MaxLabelCharacterSize = 116;

		private const int DefaultCounterBackgroundWidth = 16;

		private const int MaxCounterBackgroundWidth = 64;
	}
}
