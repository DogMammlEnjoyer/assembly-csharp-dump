using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public class NetworkRpcWeavedInvokerAttribute : Attribute
	{
		public int Key { get; }

		public int Sources { get; }

		public int Targets { get; }

		public NetworkRpcWeavedInvokerAttribute(int key, int sources, int targets)
		{
			this.Key = key;
			this.Sources = sources;
			this.Targets = targets;
		}
	}
}
