using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class WrapAttribute : Attribute
	{
		public WrapAttribute(double min, double max)
		{
			this.Min = ((min < max) ? min : max);
			this.Max = ((max > min) ? max : min);
		}

		public double Min;

		public double Max;
	}
}
