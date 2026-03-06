using System;

namespace Fusion
{
	[Serializable]
	public class FieldsMask<T> : FieldsMask
	{
		public FieldsMask(Mask256 mask) : base(mask)
		{
		}

		public FieldsMask(long maskA, long maskB = 0L, long maskC = 0L, long maskD = 0L) : base(maskA, maskB, maskC, maskD)
		{
		}

		public FieldsMask()
		{
		}

		public FieldsMask(Func<Mask256> getDefaultsDelegate)
		{
			this.Mask = ((getDefaultsDelegate != null) ? getDefaultsDelegate() : default(Mask256));
		}
	}
}
