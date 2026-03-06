using System;

namespace UnityEngine.Localization.SmartFormat.Net.Utilities
{
	internal static class SystemTime
	{
		public static void SetDateTime(DateTime dateTimeNow)
		{
			SystemTime.Now = (() => dateTimeNow);
		}

		public static void SetDateTimeOffset(DateTimeOffset dateTimeOffset)
		{
			SystemTime.OffsetNow = (() => dateTimeOffset);
		}

		public static void ResetDateTime()
		{
			SystemTime.Now = (() => DateTime.Now);
			SystemTime.OffsetNow = (() => DateTimeOffset.Now);
		}

		public static Func<DateTime> Now = () => DateTime.Now;

		public static Func<DateTimeOffset> OffsetNow = () => DateTimeOffset.Now;
	}
}
