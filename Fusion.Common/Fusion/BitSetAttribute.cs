using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class BitSetAttribute : DrawerPropertyAttribute
	{
		public BitSetAttribute(int bitCount)
		{
			this.BitCount = bitCount;
		}

		public int BitCount { get; }
	}
}
