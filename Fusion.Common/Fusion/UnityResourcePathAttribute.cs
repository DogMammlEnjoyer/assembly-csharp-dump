using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class UnityResourcePathAttribute : DrawerPropertyAttribute
	{
		public UnityResourcePathAttribute(Type resourceType)
		{
			this.ResourceType = resourceType;
		}

		public Type ResourceType { get; }
	}
}
