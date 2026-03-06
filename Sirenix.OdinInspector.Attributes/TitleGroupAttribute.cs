using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class TitleGroupAttribute : PropertyGroupAttribute
	{
		public TitleGroupAttribute(string title, string subtitle = null, TitleAlignments alignment = TitleAlignments.Left, bool horizontalLine = true, bool boldTitle = true, bool indent = false, float order = 0f) : base(title, order)
		{
			this.Subtitle = subtitle;
			this.Alignment = alignment;
			this.HorizontalLine = horizontalLine;
			this.BoldTitle = boldTitle;
			this.Indent = indent;
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			TitleGroupAttribute titleGroupAttribute = other as TitleGroupAttribute;
			if (this.Subtitle != null)
			{
				titleGroupAttribute.Subtitle = this.Subtitle;
			}
			else
			{
				this.Subtitle = titleGroupAttribute.Subtitle;
			}
			if (this.Alignment != TitleAlignments.Left)
			{
				titleGroupAttribute.Alignment = this.Alignment;
			}
			else
			{
				this.Alignment = titleGroupAttribute.Alignment;
			}
			if (!this.HorizontalLine)
			{
				titleGroupAttribute.HorizontalLine = this.HorizontalLine;
			}
			else
			{
				this.HorizontalLine = titleGroupAttribute.HorizontalLine;
			}
			if (!this.BoldTitle)
			{
				titleGroupAttribute.BoldTitle = this.BoldTitle;
			}
			else
			{
				this.BoldTitle = titleGroupAttribute.BoldTitle;
			}
			if (this.Indent)
			{
				titleGroupAttribute.Indent = this.Indent;
				return;
			}
			this.Indent = titleGroupAttribute.Indent;
		}

		public string Subtitle;

		public TitleAlignments Alignment;

		public bool HorizontalLine;

		public bool BoldTitle;

		public bool Indent;
	}
}
