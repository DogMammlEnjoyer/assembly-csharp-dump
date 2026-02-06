using System;

namespace Fusion
{
	public static class AuthorityMasks
	{
		internal static int Create(bool state, bool input)
		{
			int result;
			if (state)
			{
				result = (1 | (input ? 2 : 0));
			}
			else
			{
				result = (input ? 2 : 4);
			}
			return result;
		}

		public const int STATE = 1;

		public const int INPUT = 2;

		public const int PROXY = 4;

		public const int NONE = 0;

		public const int ALL = 7;
	}
}
