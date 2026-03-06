using System;
using System.Collections.Generic;

namespace g3
{
	public class IntTagSet<T>
	{
		private void create()
		{
			if (this.tags == null)
			{
				this.tags = new Dictionary<T, int>();
			}
		}

		public void Add(T reference, int tag)
		{
			this.create();
			this.tags.Add(reference, tag);
		}

		public bool Has(T reference)
		{
			int num = 0;
			return this.tags != null && this.tags.TryGetValue(reference, out num);
		}

		public int Get(T reference)
		{
			int result = 0;
			if (this.tags != null && this.tags.TryGetValue(reference, out result))
			{
				return result;
			}
			return int.MaxValue;
		}

		public const int InvalidTag = 2147483647;

		private Dictionary<T, int> tags;
	}
}
