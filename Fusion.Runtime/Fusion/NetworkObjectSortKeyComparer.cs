using System;
using System.Collections.Generic;

namespace Fusion
{
	public class NetworkObjectSortKeyComparer : IComparer<NetworkObject>
	{
		public int Compare(NetworkObject x, NetworkObject y)
		{
			return x.SortKey.CompareTo(y.SortKey);
		}

		public static readonly NetworkObjectSortKeyComparer Instance = new NetworkObjectSortKeyComparer();
	}
}
