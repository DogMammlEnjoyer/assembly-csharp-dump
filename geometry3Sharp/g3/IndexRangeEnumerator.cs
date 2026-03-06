using System;
using System.Collections;
using System.Collections.Generic;

namespace g3
{
	public class IndexRangeEnumerator : IEnumerable<int>, IEnumerable
	{
		public IndexRangeEnumerator(int count)
		{
			this.Count = count;
		}

		public IndexRangeEnumerator(int start, int count)
		{
			this.Start = start;
			this.Count = count;
		}

		public IEnumerator<int> GetEnumerator()
		{
			int num;
			for (int i = 0; i < this.Count; i = num)
			{
				yield return this.Start + i;
				num = i + 1;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private int Start;

		private int Count;
	}
}
