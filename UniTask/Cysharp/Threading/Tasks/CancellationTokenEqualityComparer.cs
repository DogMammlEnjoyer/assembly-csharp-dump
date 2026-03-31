using System;
using System.Collections.Generic;
using System.Threading;

namespace Cysharp.Threading.Tasks
{
	public class CancellationTokenEqualityComparer : IEqualityComparer<CancellationToken>
	{
		public bool Equals(CancellationToken x, CancellationToken y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(CancellationToken obj)
		{
			return obj.GetHashCode();
		}

		public static readonly IEqualityComparer<CancellationToken> Default = new CancellationTokenEqualityComparer();
	}
}
