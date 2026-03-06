using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MaxStringByteCountAttribute : DrawerPropertyAttribute
	{
		public MaxStringByteCountAttribute(int count, string encoding)
		{
			this.ByteCount = count;
			this.Encoding = encoding;
		}

		public int ByteCount { get; }

		public string Encoding { get; }
	}
}
