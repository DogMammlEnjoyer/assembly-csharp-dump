using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[Obsolete("Use RangeAttribute instead")]
	public sealed class RangeSliderAttribute : PropertyAttribute
	{
		public RangeSliderAttribute(float min, float max)
		{
			this.Min = min;
			this.Max = max;
		}

		public float Min;

		public float Max;
	}
}
