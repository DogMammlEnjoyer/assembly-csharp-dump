using System;

namespace Oculus.Voice.Core.Utilities
{
	public class DateTimeUtility
	{
		public static DateTime UtcNow
		{
			get
			{
				return DateTime.UtcNow;
			}
		}

		public static long ElapsedMilliseconds
		{
			get
			{
				return DateTimeUtility.UtcNow.Ticks / 10000L;
			}
		}
	}
}
