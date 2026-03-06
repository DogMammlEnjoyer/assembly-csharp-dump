using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[DontApplyToListElements]
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class PropertyTooltipAttribute : Attribute
	{
		public PropertyTooltipAttribute(string tooltip)
		{
			this.Tooltip = tooltip;
		}

		public string Tooltip;
	}
}
