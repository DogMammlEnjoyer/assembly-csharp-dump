using System;

namespace Assets.OVR.Scripts
{
	internal class RangedRecord : Record
	{
		public RangedRecord(int order, string cat, string msg, float val, float minVal, float maxVal) : base(order, cat, msg)
		{
			this.value = val;
			this.min = minVal;
			this.max = maxVal;
		}

		public float value;

		public float min;

		public float max;
	}
}
