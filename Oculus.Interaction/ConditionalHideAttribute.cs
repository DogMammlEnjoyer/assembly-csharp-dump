using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class ConditionalHideAttribute : PropertyAttribute
	{
		public string ConditionalFieldPath { get; private set; }

		public object Value { get; private set; }

		public ConditionalHideAttribute.DisplayMode Display { get; private set; } = ConditionalHideAttribute.DisplayMode.ShowIfTrue;

		public ConditionalHideAttribute(string fieldName, object value)
		{
			this.ConditionalFieldPath = fieldName;
			this.Value = value;
			this.Display = ConditionalHideAttribute.DisplayMode.ShowIfTrue;
		}

		public ConditionalHideAttribute(string fieldName, object value, ConditionalHideAttribute.DisplayMode displayMode)
		{
			this.ConditionalFieldPath = fieldName;
			this.Value = value;
			this.Display = displayMode;
		}

		public enum DisplayMode
		{
			Always,
			Never,
			ShowIfTrue,
			HideIfTrue
		}
	}
}
