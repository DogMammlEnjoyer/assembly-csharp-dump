using System;
using Unity.Mathematics;

namespace UnityEngine.Rendering.Universal
{
	internal struct BuddyAllocation
	{
		public BuddyAllocation(int level, int index)
		{
			this.level = level;
			this.index = index;
		}

		public uint2 index2D
		{
			get
			{
				return SpaceFillingCurves.DecodeMorton2D((uint)this.index);
			}
		}

		public int level;

		public int index;
	}
}
