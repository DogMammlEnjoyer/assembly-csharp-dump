using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct TessEdgeCompare : IComparer<int2>
	{
		public int Compare(int2 a, int2 b)
		{
			int num = a.x - b.x;
			if (num != 0)
			{
				return num;
			}
			return a.y - b.y;
		}
	}
}
