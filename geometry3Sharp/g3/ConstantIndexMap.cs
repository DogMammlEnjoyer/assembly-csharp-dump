using System;

namespace g3
{
	public class ConstantIndexMap : IIndexMap
	{
		public ConstantIndexMap(int c)
		{
			this.Constant = c;
		}

		public int this[int index]
		{
			get
			{
				return this.Constant;
			}
		}

		public int Constant;
	}
}
