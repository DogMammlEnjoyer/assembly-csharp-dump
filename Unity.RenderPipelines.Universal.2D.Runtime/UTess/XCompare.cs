using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct XCompare : IComparer<double>
	{
		public int Compare(double a, double b)
		{
			if (a >= b)
			{
				return 1;
			}
			return -1;
		}
	}
}
