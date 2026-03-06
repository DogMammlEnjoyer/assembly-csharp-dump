using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Linq.Parallel
{
	internal class EmptyEnumerator<T> : QueryOperatorEnumerator<T, int>, IEnumerator<!0>, IDisposable, IEnumerator
	{
		internal override bool MoveNext(ref T currentElement, ref int currentKey)
		{
			return false;
		}

		public T Current
		{
			get
			{
				return default(T);
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return null;
			}
		}

		public bool MoveNext()
		{
			return false;
		}

		void IEnumerator.Reset()
		{
		}
	}
}
