using System;
using UnityEngine;

namespace Meta.WitAi.Attributes
{
	public class LabelAttribute : PropertyAttribute
	{
		public string LabelFieldPropertyOrMethodName { get; }

		public string TooltipFieldPropertyOrMethodName { get; }

		public LabelAttribute(string labelFieldPropertyOrMethodName, string tooltipFieldPropertyOrMethodName = "")
		{
			this.LabelFieldPropertyOrMethodName = labelFieldPropertyOrMethodName;
			this.TooltipFieldPropertyOrMethodName = tooltipFieldPropertyOrMethodName;
		}
	}
}
