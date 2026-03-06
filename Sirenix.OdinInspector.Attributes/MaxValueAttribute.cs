using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class MaxValueAttribute : Attribute
	{
		public MaxValueAttribute(double maxValue)
		{
			this.MaxValue = maxValue;
		}

		public MaxValueAttribute(string expression)
		{
			this.Expression = expression;
		}

		public double MaxValue;

		public string Expression;
	}
}
