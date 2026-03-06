using System;

namespace Unity.XR.CoreUtils.Capabilities
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class CustomCapabilityKeyAttribute : Attribute
	{
		public CustomCapabilityKeyAttribute(int order = 1000)
		{
			this.Order = order;
		}

		public readonly int Order;
	}
}
