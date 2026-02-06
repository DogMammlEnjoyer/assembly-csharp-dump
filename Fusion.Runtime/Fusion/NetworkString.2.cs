using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class NetworkString
	{
		public static int GetCapacity<[IsUnmanaged] TSize>() where TSize : struct, ValueType, IFixedStorage
		{
			return sizeof(TSize) / 4;
		}
	}
}
