using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class TitleAttribute : Attribute
	{
		public TitleAttribute(string title, string subtitle = null, TitleAlignments titleAlignment = TitleAlignments.Left, bool horizontalLine = true, bool bold = true)
		{
			this.Title = (title ?? "null");
			this.Subtitle = subtitle;
			this.Bold = bold;
			this.TitleAlignment = titleAlignment;
			this.HorizontalLine = horizontalLine;
		}

		public string Title;

		public string Subtitle;

		public bool Bold;

		public bool HorizontalLine;

		public TitleAlignments TitleAlignment;
	}
}
