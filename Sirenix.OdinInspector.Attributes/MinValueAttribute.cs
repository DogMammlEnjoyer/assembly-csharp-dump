using System;
using System.Diagnostics;

namespace Sirenix.OdinInspector
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	[Conditional("UNITY_EDITOR")]
	public sealed class MinValueAttribute : Attribute
	{
		public MinValueAttribute(double minValue)
		{
			this.MinValue = minValue;
		}

		public MinValueAttribute(string expression)
		{
			this.Expression = expression;
		}

		public double MinValue;

		public string Expression;
	}
}
