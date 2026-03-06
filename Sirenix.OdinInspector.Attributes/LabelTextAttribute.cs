using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class LabelTextAttribute : Attribute
	{
		public LabelTextAttribute(string text)
		{
			this.Text = text;
		}

		public LabelTextAttribute(SdfIconType icon)
		{
			this.Icon = icon;
		}

		public LabelTextAttribute(string text, bool nicifyText)
		{
			this.Text = text;
			this.NicifyText = nicifyText;
		}

		public LabelTextAttribute(string text, SdfIconType icon)
		{
			this.Text = text;
			this.Icon = icon;
		}

		public LabelTextAttribute(string text, bool nicifyText, SdfIconType icon)
		{
			this.Text = text;
			this.NicifyText = nicifyText;
			this.Icon = icon;
		}

		public string Text;

		public bool NicifyText;

		public SdfIconType Icon;

		public string IconColor;
	}
}
