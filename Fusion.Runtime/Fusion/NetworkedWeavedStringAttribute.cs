using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Property)]
	public class NetworkedWeavedStringAttribute : Attribute
	{
		public NetworkedWeavedStringAttribute(int capacity, string cacheFieldName)
		{
			this.Capacity = capacity;
			this.CacheFieldName = cacheFieldName;
		}

		public int Capacity { get; }

		public string CacheFieldName { get; }
	}
}
