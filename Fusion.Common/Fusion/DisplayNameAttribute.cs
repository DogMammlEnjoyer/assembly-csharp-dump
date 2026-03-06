using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public class DisplayNameAttribute : DecoratingPropertyAttribute
	{
		public DisplayNameAttribute(string name)
		{
			this.Name = name;
		}

		public readonly string Name;
	}
}
