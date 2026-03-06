using System;
using System.Diagnostics;

namespace System.Linq
{
	internal sealed class SystemLinq_GroupingDebugView<TKey, TElement>
	{
		public SystemLinq_GroupingDebugView(Grouping<TKey, TElement> grouping)
		{
			this._grouping = grouping;
		}

		public TKey Key
		{
			get
			{
				return this._grouping.Key;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public TElement[] Values
		{
			get
			{
				TElement[] result;
				if ((result = this._cachedValues) == null)
				{
					result = (this._cachedValues = this._grouping.ToArray<TElement>());
				}
				return result;
			}
		}

		private readonly Grouping<TKey, TElement> _grouping;

		private TElement[] _cachedValues;
	}
}
