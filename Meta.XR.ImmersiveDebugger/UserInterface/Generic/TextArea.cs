using System;
using UnityEngine;
using UnityEngine.UI;

namespace Meta.XR.ImmersiveDebugger.UserInterface.Generic
{
	public class TextArea : Value
	{
		private Text Text
		{
			get
			{
				return base.Label.Text;
			}
		}

		protected override void Setup(Controller owner)
		{
			base.Setup(owner);
			this.Text.horizontalOverflow = HorizontalWrapMode.Wrap;
			this.Text.verticalOverflow = VerticalWrapMode.Overflow;
			this.Text.text = "";
		}

		internal override string Content
		{
			get
			{
				return this.Text.text;
			}
			set
			{
				string text = value.Replace("\\n", Environment.NewLine);
				this.Text.text = text;
				this.UpdateLayoutSize();
			}
		}

		protected override void OnTransparencyChanged()
		{
			base.OnTransparencyChanged();
			this.Text.color = (base.Transparent ? Color.white : base.TextStyle.color);
		}

		internal void UpdateLayoutSize()
		{
			base.LayoutStyle.size.y = this.TextAreaHeight + base.Owner.LayoutStyle.spacing + base.Label.LayoutStyle.margin.y * 2f;
			base.RefreshLayout();
		}

		internal float TextAreaHeight
		{
			get
			{
				return this.CalculateHeight(base.LayoutStyle.size.x);
			}
		}

		private float CalculateHeight(float textWidth)
		{
			TextGenerationSettings settings = default(TextGenerationSettings);
			settings.generationExtents = new Vector2(textWidth, 0f);
			settings.fontSize = this.Text.fontSize;
			settings.textAnchor = this.Text.alignment;
			settings.alignByGeometry = this.Text.alignByGeometry;
			settings.scaleFactor = this.Text.pixelsPerUnit;
			settings.color = this.Text.color;
			settings.font = this.Text.font;
			settings.pivot = base.RectTransform.pivot;
			settings.richText = false;
			settings.lineSpacing = this.Text.lineSpacing;
			settings.fontStyle = this.Text.fontStyle;
			settings.resizeTextForBestFit = false;
			settings.updateBounds = true;
			settings.horizontalOverflow = this.Text.horizontalOverflow;
			settings.verticalOverflow = this.Text.verticalOverflow;
			TextGenerator textGenerator = new TextGenerator();
			textGenerator.Populate(this.Text.text, settings);
			return textGenerator.rectExtents.height / this.Text.pixelsPerUnit;
		}
	}
}
