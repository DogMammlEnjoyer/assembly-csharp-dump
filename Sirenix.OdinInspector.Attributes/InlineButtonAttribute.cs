using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class InlineButtonAttribute : Attribute
	{
		[Obsolete("Use the Action member instead.", false)]
		public string MemberMethod
		{
			get
			{
				return this.Action;
			}
			set
			{
				this.Action = value;
			}
		}

		public InlineButtonAttribute(string action, string label = null)
		{
			this.Action = action;
			this.Label = label;
		}

		public InlineButtonAttribute(string action, SdfIconType icon, string label = null)
		{
			this.Action = action;
			this.Icon = icon;
			this.Label = label;
		}

		public string Action;

		public string Label;

		public string ShowIf;

		public string ButtonColor;

		public string TextColor;

		public SdfIconType Icon;

		public IconAlignment IconAlignment;
	}
}
