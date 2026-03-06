using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class Label : Controller
	{
		internal Text Text { get; private set; }

		public string Content
		{
			get
			{
				return this.Text.text;
			}
			set
			{
				this.Text.text = value;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this.Text = base.GameObject.AddComponent<Text>();
			this.Text.horizontalOverflow = HorizontalWrapMode.Overflow;
			this.Text.verticalOverflow = VerticalWrapMode.Overflow;
			this.Text.text = "";
			this.Text.raycastTarget = false;
		}

		public TextStyle TextStyle
		{
			get
			{
				return this._textStyle;
			}
			set
			{
				this._textStyle = value;
				this.Text.font = value.font;
				this.Text.fontSize = value.fontSize;
				this.Text.alignment = value.textAlignement;
				this.Text.color = value.color;
			}
		}

		private TextStyle _textStyle;
	}
}
