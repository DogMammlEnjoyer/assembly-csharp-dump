using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal.UTess
{
	internal struct DelaEdgeCompare : IComparer<int4>
	{
		public int Compare(int4 a, int4 b)
		{
			int num = a.x - b.x;
			if (num != 0)
			{
				return num;
			}
			num = a.y - b.y;
			if (num != 0)
			{
				return num;
			}
			num = a.z - b.z;
			if (num != 0)
			{
				return num;
			}
			return a.w - b.w;
		}
	}
}
