using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	public static class FixedStorage
	{
		public static int GetWordCount<[IsUnmanaged] T>() where T : struct, ValueType, IFixedStorage
		{
			return sizeof(T) / 4;
		}
	}
}
