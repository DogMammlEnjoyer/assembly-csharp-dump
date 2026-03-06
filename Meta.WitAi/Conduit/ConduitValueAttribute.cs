using System;

namespace Meta.Conduit
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ConduitValueAttribute : Attribute
	{
		public ConduitValueAttribute(params string[] aliases)
		{
			this.Aliases = aliases;
		}

		public string[] Aliases { get; }
	}
}
