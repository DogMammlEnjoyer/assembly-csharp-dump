using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class ConstantItr<T> : IEnumerable<T>, IEnumerable
	{
		public ConstantItr(int count, T constant)
		{
			this.N = count;
			this.ConstantValue = constant;
		}

		public IEnumerator<T> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this.N; i = num)
			{
				yield return this.ConstantValue;
				num = i + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public T ConstantValue;

		public int N;
	}
}
