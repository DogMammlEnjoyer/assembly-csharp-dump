using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public sealed class MinMaxRangeSliderAttribute : PropertyAttribute
	{
		public MinMaxRangeSliderAttribute(float min, float max)
		{
			this.Min = min;
			this.Max = max;
		}

		public float Min;

		public float Max;
	}
}
