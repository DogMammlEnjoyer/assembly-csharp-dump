using System;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.UserInterface
{
	public class DebugPanel : OverlayCanvasPanel
	{
		public Texture2D Icon { get; set; }

		public string Title
		{
			get
			{
				return this._title.Content;
			}
			set
			{
				this._title.Content = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this._title = base.Append<Label>("title");
			this._title.LayoutStyle = Style.Load<LayoutStyle>("PanelTitle");
			this._title.TextStyle = Style.Load<TextStyle>("PanelTitle");
			this._closeIcon = base.Append<ButtonWithIcon>("CloseButton");
			this._closeIcon.LayoutStyle = Style.Load<LayoutStyle>("CloseButton");
			this._closeIcon.BackgroundStyle = Style.Load<ImageStyle>("CloseButtonBackground");
			this._closeIcon.Icon = Resources.Load<Texture2D>("Textures/minimize_icon");
			this._closeIcon.IconStyle = Style.Load<ImageStyle>("CloseButtonIcon");
			this._closeIcon.Callback = new Action(base.Hide);
			base.SetExpectedPixelsPerUnit(1000f, 10f, 2.24f);
		}

		private Label _title;

		private ButtonWithIcon _closeIcon;

		private const float DynamicPixelsPerUnit = 10f;
	}
}
