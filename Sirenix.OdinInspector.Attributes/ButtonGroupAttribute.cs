using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[IncludeMyAttributes]
	[ShowInInspector]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class ButtonGroupAttribute : PropertyGroupAttribute
	{
		public IconAlignment IconAlignment
		{
			get
			{
				return this.buttonIconAlignment;
			}
			set
			{
				this.buttonIconAlignment = value;
				this.HasDefinedButtonIconAlignment = true;
			}
		}

		public int ButtonAlignment
		{
			get
			{
				return this.buttonAlignment;
			}
			set
			{
				this.buttonAlignment = value;
				this.HasDefinedButtonAlignment = true;
			}
		}

		public bool Stretch
		{
			get
			{
				return this.stretch;
			}
			set
			{
				this.stretch = value;
				this.HasDefinedStretch = true;
			}
		}

		public bool HasDefinedButtonIconAlignment { get; private set; }

		public bool HasDefinedButtonAlignment { get; private set; }

		public bool HasDefinedStretch { get; private set; }

		public ButtonGroupAttribute(string group = "_DefaultGroup", float order = 0f) : base(group, order)
		{
		}

		public int ButtonHeight;

		private IconAlignment buttonIconAlignment;

		private int buttonAlignment;

		private bool stretch;
	}
}
