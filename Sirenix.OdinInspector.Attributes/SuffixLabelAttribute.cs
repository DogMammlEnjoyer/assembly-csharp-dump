using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
	[Conditional("UNITY_EDITOR")]
	public sealed class SuffixLabelAttribute : Attribute
	{
		[ShowInInspector]
		[OdinDesignerBinding(new string[]
		{
			"icon",
			"HasDefinedIcon"
		})]
		public SdfIconType Icon
		{
			get
			{
				return this.icon;
			}
			set
			{
				this.icon = value;
				this.HasDefinedIcon = true;
			}
		}

		public bool HasDefinedIcon { get; private set; }

		public SuffixLabelAttribute(string label, bool overlay = false)
		{
			this.Label = label;
			this.Overlay = overlay;
		}

		public SuffixLabelAttribute(string label, SdfIconType icon, bool overlay = false)
		{
			this.Label = label;
			this.Icon = icon;
			this.Overlay = overlay;
		}

		public SuffixLabelAttribute(SdfIconType icon)
		{
			this.Icon = icon;
		}

		public string Label;

		public bool Overlay;

		[ColorResolver]
		public string IconColor;

		private SdfIconType icon;
	}
}
