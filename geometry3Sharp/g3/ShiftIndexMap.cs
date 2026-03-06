using System;

namespace g3
{
	public class ShiftIndexMap : IIndexMap
	{
		public ShiftIndexMap(int n)
		{
			this.Shift = n;
		}

		public int this[int index]
		{
			get
			{
				return index + this.Shift;
			}
		}

		public int Shift;
	}
}
