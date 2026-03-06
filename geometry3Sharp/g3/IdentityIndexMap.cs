using System;

namespace g3
{
	public class IdentityIndexMap : IIndexMap
	{
		public int this[int index]
		{
			get
			{
				return index;
			}
		}
	}
}
