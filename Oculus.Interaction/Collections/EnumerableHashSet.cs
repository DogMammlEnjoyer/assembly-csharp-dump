using System;
using System.Collections;
using System.Collections.Generic;

namespace Oculus.Interaction.Collections
{
	public class EnumerableHashSet<T> : HashSet<T>, IEnumerableHashSet<T>, IEnumerable<T>, IEnumerable
	{
		public EnumerableHashSet()
		{
		}

		public EnumerableHashSet(IEnumerable<T> values) : base(values)
		{
		}

		HashSet<T>.Enumerator IEnumerableHashSet<!0>.GetEnumerator()
		{
			return base.GetEnumerator();
		}
	}
}
