using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
	public sealed class DefaultForPropertyAttribute : PropertyAttribute
	{
		public string PropertyName { get; }

		public int WordOffset { get; }

		public int WordCount { get; }

		public DefaultForPropertyAttribute(string propertyName, int wordOffset, int wordCount)
		{
			this.PropertyName = propertyName;
			this.WordOffset = wordOffset;
			this.WordCount = wordCount;
		}
	}
}
