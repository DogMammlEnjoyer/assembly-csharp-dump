using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field)]
	public class HideArrayElementLabelAttribute : DecoratingPropertyAttribute
	{
		public HideArrayElementLabelAttribute() : base(-11000)
		{
		}

		private new const int DefaultOrder = -11000;
	}
}
