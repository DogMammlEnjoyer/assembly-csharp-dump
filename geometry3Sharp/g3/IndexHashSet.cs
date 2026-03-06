using System;
using System.Collections.Generic;

namespace g3
{
	public class IndexHashSet : HashSet<int>
	{
		public bool this[int key]
		{
			get
			{
				return base.Contains(key);
			}
			set
			{
				if (value)
				{
					base.Add(key);
					return;
				}
				if (!value && base.Contains(key))
				{
					base.Remove(key);
				}
			}
		}
	}
}
