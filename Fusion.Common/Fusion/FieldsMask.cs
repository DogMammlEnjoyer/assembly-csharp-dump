using System;

namespace Fusion
{
	[Serializable]
	public abstract class FieldsMask
	{
		protected FieldsMask(Mask256 mask)
		{
			this.Mask = mask;
		}

		protected FieldsMask(long a, long b, long c, long d)
		{
			this.Mask = default(Mask256);
		}

		protected FieldsMask()
		{
		}

		public static implicit operator Mask256(FieldsMask mask)
		{
			return mask.Mask;
		}

		public Mask256 Mask;
	}
}
