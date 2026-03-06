using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[IncludeMyAttributes]
	[ShowInInspector]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public class ResponsiveButtonGroupAttribute : PropertyGroupAttribute
	{
		public ResponsiveButtonGroupAttribute(string group = "_DefaultResponsiveButtonGroup") : base(group)
		{
		}

		protected override void CombineValuesWith(PropertyGroupAttribute other)
		{
			ResponsiveButtonGroupAttribute responsiveButtonGroupAttribute = other as ResponsiveButtonGroupAttribute;
			if (other == null)
			{
				return;
			}
			if (responsiveButtonGroupAttribute.DefaultButtonSize != ButtonSizes.Medium)
			{
				this.DefaultButtonSize = responsiveButtonGroupAttribute.DefaultButtonSize;
			}
			else if (this.DefaultButtonSize != ButtonSizes.Medium)
			{
				responsiveButtonGroupAttribute.DefaultButtonSize = this.DefaultButtonSize;
			}
			this.UniformLayout = (this.UniformLayout || responsiveButtonGroupAttribute.UniformLayout);
		}

		public ButtonSizes DefaultButtonSize = ButtonSizes.Medium;

		public bool UniformLayout;
	}
}
