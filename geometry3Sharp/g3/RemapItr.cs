using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class RemapItr<T, T2> : IEnumerable<T>, IEnumerable
	{
		public RemapItr(IEnumerable<T2> otherIterator, Func<T2, T> valueFunction)
		{
			this.OtherItr = otherIterator;
			this.ValueF = valueFunction;
		}

		public IEnumerator<T> GetEnumerator()
		{
			foreach (T2 arg in this.OtherItr)
			{
				yield return this.ValueF(arg);
			}
			IEnumerator<T2> enumerator = null;
			yield break;
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerable<T2> OtherItr;

		public Func<T2, T> ValueF;
	}
}
