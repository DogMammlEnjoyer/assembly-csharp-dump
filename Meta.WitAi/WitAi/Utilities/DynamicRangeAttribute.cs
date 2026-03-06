using System;
using UnityEngine;

namespace Meta.WitAi.Utilities
{
	public class DynamicRangeAttribute : PropertyAttribute
	{
		public string RangeProperty { get; private set; }

		public float DefaultMin { get; private set; }

		public float DefaultMax { get; private set; }

		public DynamicRangeAttribute(string rangeProperty, float defaultMin = -3.4028235E+38f, float defaultMax = 3.4028235E+38f)
		{
			this.DefaultMin = defaultMin;
			this.DefaultMax = defaultMax;
			this.RangeProperty = rangeProperty;
		}
	}
}
