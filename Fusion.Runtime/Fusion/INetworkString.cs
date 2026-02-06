using System;
using System.Runtime.CompilerServices;

namespace Fusion
{
	internal interface INetworkString
	{
		bool Equals<[IsUnmanaged] TOtherSize>(ref NetworkString<TOtherSize> other) where TOtherSize : struct, ValueType, IFixedStorage;
	}
}
