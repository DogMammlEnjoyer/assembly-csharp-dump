using System;

namespace UnityEngine.Rendering
{
	internal struct IndirectBufferAllocInfo
	{
		public bool IsEmpty()
		{
			return this.drawCount == 0;
		}

		public bool IsWithinLimits(in IndirectBufferLimits limits)
		{
			return this.drawAllocIndex + this.drawCount <= limits.maxDrawCount && this.instanceAllocIndex + this.instanceCount <= limits.maxInstanceCount;
		}

		public int GetExtraDrawInfoSlotIndex()
		{
			return this.drawAllocIndex + this.drawCount;
		}

		public int drawAllocIndex;

		public int drawCount;

		public int instanceAllocIndex;

		public int instanceCount;
	}
}
