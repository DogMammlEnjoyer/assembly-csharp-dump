using System;
using System.Diagnostics;

namespace Fusion.Analyzer
{
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("false")]
	public class StaticFieldAttribute : Attribute
	{
		public StaticFieldAttribute(StaticFieldResetMode resetMode)
		{
			this.Reset = resetMode;
		}

		public StaticFieldAttribute() : this(StaticFieldResetMode.ResetMethod)
		{
		}

		public StaticFieldResetMode Reset { get; }
	}
}
