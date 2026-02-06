using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class OnChangedRenderAttribute : Attribute
	{
		public string MethodName { get; private set; }

		public OnChangedRenderAttribute(string methodName)
		{
			bool flag = string.IsNullOrEmpty(methodName);
			if (flag)
			{
				throw new ArgumentNullException("methodName", "Method name cannot be null or empty.");
			}
			this.MethodName = methodName;
		}
	}
}
