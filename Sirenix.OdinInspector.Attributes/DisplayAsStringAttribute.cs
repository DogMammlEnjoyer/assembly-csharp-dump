using System;
using System.Diagnostics;
using UnityEngine;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class DisplayAsStringAttribute : Attribute
	{
		public DisplayAsStringAttribute()
		{
			this.Overflow = true;
		}

		public DisplayAsStringAttribute(bool overflow)
		{
			this.Overflow = overflow;
		}

		public DisplayAsStringAttribute(TextAlignment alignment)
		{
			this.Alignment = alignment;
		}

		public DisplayAsStringAttribute(int fontSize)
		{
			this.FontSize = fontSize;
		}

		public DisplayAsStringAttribute(bool overflow, TextAlignment alignment)
		{
			this.Overflow = overflow;
			this.Alignment = alignment;
		}

		public DisplayAsStringAttribute(bool overflow, int fontSize)
		{
			this.Overflow = overflow;
			this.FontSize = fontSize;
		}

		public DisplayAsStringAttribute(int fontSize, TextAlignment alignment)
		{
			this.FontSize = fontSize;
			this.Alignment = alignment;
		}

		public DisplayAsStringAttribute(bool overflow, int fontSize, TextAlignment alignment)
		{
			this.Overflow = overflow;
			this.FontSize = fontSize;
			this.Alignment = alignment;
		}

		public DisplayAsStringAttribute(TextAlignment alignment, bool enableRichText)
		{
			this.Alignment = alignment;
			this.EnableRichText = enableRichText;
		}

		public DisplayAsStringAttribute(int fontSize, bool enableRichText)
		{
			this.FontSize = fontSize;
			this.EnableRichText = enableRichText;
		}

		public DisplayAsStringAttribute(bool overflow, TextAlignment alignment, bool enableRichText)
		{
			this.Overflow = overflow;
			this.Alignment = alignment;
			this.EnableRichText = enableRichText;
		}

		public DisplayAsStringAttribute(bool overflow, int fontSize, bool enableRichText)
		{
			this.Overflow = overflow;
			this.FontSize = fontSize;
			this.EnableRichText = enableRichText;
		}

		public DisplayAsStringAttribute(int fontSize, TextAlignment alignment, bool enableRichText)
		{
			this.FontSize = fontSize;
			this.Alignment = alignment;
			this.EnableRichText = enableRichText;
		}

		public DisplayAsStringAttribute(bool overflow, int fontSize, TextAlignment alignment, bool enableRichText)
		{
			this.Overflow = overflow;
			this.FontSize = fontSize;
			this.Alignment = alignment;
			this.EnableRichText = enableRichText;
		}

		public bool Overflow;

		public TextAlignment Alignment;

		public int FontSize;

		public bool EnableRichText;

		public string Format;
	}
}
