using System;
using UnityEngine;

namespace Meta.WitAi.Attributes
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class HideIfAttribute : PropertyAttribute
	{
		public HideIfAttribute(string conditionFieldName)
		{
			this.conditionFieldName = conditionFieldName;
		}

		public string conditionFieldName;
	}
}
