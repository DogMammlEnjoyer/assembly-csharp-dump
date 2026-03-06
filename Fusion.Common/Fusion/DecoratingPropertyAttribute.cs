using System;

namespace Fusion
{
	public abstract class DecoratingPropertyAttribute : PropertyAttribute
	{
		protected DecoratingPropertyAttribute()
		{
			base.order = -10000;
		}

		protected DecoratingPropertyAttribute(int order)
		{
			base.order = order;
		}

		public const int DefaultOrder = -10000;
	}
}
