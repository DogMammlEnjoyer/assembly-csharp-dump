using System;
using System.Linq;
using System.Text;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	public static class TimeSpanUtility
	{
		public static string ToTimeString(this TimeSpan FromTime, TimeSpanFormatOptions options, TimeTextInfo timeTextInfo)
		{
			options = options.Merge(TimeSpanUtility.DefaultFormatOptions).Merge(TimeSpanUtility.AbsoluteDefaults);
			TimeSpanFormatOptions timeSpanFormatOptions = options.Mask(TimeSpanFormatOptions.RangeMilliSeconds | TimeSpanFormatOptions.RangeSeconds | TimeSpanFormatOptions.RangeMinutes | TimeSpanFormatOptions.RangeHours | TimeSpanFormatOptions.RangeDays | TimeSpanFormatOptions.RangeWeeks).AllFlags().Last<TimeSpanFormatOptions>();
			TimeSpanFormatOptions timeSpanFormatOptions2 = options.Mask(TimeSpanFormatOptions.RangeMilliSeconds | TimeSpanFormatOptions.RangeSeconds | TimeSpanFormatOptions.RangeMinutes | TimeSpanFormatOptions.RangeHours | TimeSpanFormatOptions.RangeDays | TimeSpanFormatOptions.RangeWeeks).AllFlags().First<TimeSpanFormatOptions>();
			TimeSpanFormatOptions timeSpanFormatOptions3 = options.Mask(TimeSpanFormatOptions.TruncateShortest | TimeSpanFormatOptions.TruncateAuto | TimeSpanFormatOptions.TruncateFill | TimeSpanFormatOptions.TruncateFull).AllFlags().First<TimeSpanFormatOptions>();
			bool flag = options.Mask(TimeSpanFormatOptions.LessThan | TimeSpanFormatOptions.LessThanOff) != TimeSpanFormatOptions.LessThanOff;
			bool abbr = options.Mask(TimeSpanFormatOptions.Abbreviate | TimeSpanFormatOptions.AbbreviateOff) != TimeSpanFormatOptions.AbbreviateOff;
			Func<double, double> func = flag ? new Func<double, double>(Math.Floor) : new Func<double, double>(Math.Ceiling);
			if (timeSpanFormatOptions2 <= TimeSpanFormatOptions.RangeMinutes)
			{
				if (timeSpanFormatOptions2 != TimeSpanFormatOptions.RangeMilliSeconds)
				{
					if (timeSpanFormatOptions2 != TimeSpanFormatOptions.RangeSeconds)
					{
						if (timeSpanFormatOptions2 == TimeSpanFormatOptions.RangeMinutes)
						{
							FromTime = TimeSpan.FromMinutes(func(FromTime.TotalMinutes));
						}
					}
					else
					{
						FromTime = TimeSpan.FromSeconds(func(FromTime.TotalSeconds));
					}
				}
				else
				{
					FromTime = TimeSpan.FromMilliseconds(func(FromTime.TotalMilliseconds));
				}
			}
			else if (timeSpanFormatOptions2 != TimeSpanFormatOptions.RangeHours)
			{
				if (timeSpanFormatOptions2 != TimeSpanFormatOptions.RangeDays)
				{
					if (timeSpanFormatOptions2 == TimeSpanFormatOptions.RangeWeeks)
					{
						FromTime = TimeSpan.FromDays(func(FromTime.TotalDays / 7.0) * 7.0);
					}
				}
				else
				{
					FromTime = TimeSpan.FromDays(func(FromTime.TotalDays));
				}
			}
			else
			{
				FromTime = TimeSpan.FromHours(func(FromTime.TotalHours));
			}
			bool flag2 = false;
			StringBuilder stringBuilder = StringBuilderPool.Get();
			TimeSpanFormatOptions timeSpanFormatOptions4 = timeSpanFormatOptions;
			while (timeSpanFormatOptions4 >= timeSpanFormatOptions2)
			{
				int num;
				if (timeSpanFormatOptions4 <= TimeSpanFormatOptions.RangeMinutes)
				{
					if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeMilliSeconds)
					{
						if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeSeconds)
						{
							if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeMinutes)
							{
								goto IL_2B2;
							}
							num = (int)Math.Floor(FromTime.TotalMinutes);
							FromTime -= TimeSpan.FromMinutes((double)num);
						}
						else
						{
							num = (int)Math.Floor(FromTime.TotalSeconds);
							FromTime -= TimeSpan.FromSeconds((double)num);
						}
					}
					else
					{
						num = (int)Math.Floor(FromTime.TotalMilliseconds);
						FromTime -= TimeSpan.FromMilliseconds((double)num);
					}
				}
				else if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeHours)
				{
					if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeDays)
					{
						if (timeSpanFormatOptions4 != TimeSpanFormatOptions.RangeWeeks)
						{
							goto IL_2B2;
						}
						num = (int)Math.Floor(FromTime.TotalDays / 7.0);
						FromTime -= TimeSpan.FromDays((double)(num * 7));
					}
					else
					{
						num = (int)Math.Floor(FromTime.TotalDays);
						FromTime -= TimeSpan.FromDays((double)num);
					}
				}
				else
				{
					num = (int)Math.Floor(FromTime.TotalHours);
					FromTime -= TimeSpan.FromHours((double)num);
				}
				bool flag3 = false;
				bool flag4 = false;
				if (timeSpanFormatOptions3 <= TimeSpanFormatOptions.TruncateAuto)
				{
					if (timeSpanFormatOptions3 != TimeSpanFormatOptions.TruncateShortest)
					{
						if (timeSpanFormatOptions3 == TimeSpanFormatOptions.TruncateAuto)
						{
							if (num > 0)
							{
								flag3 = true;
							}
						}
					}
					else if (flag2)
					{
						flag4 = true;
					}
					else if (num > 0)
					{
						flag3 = true;
					}
				}
				else if (timeSpanFormatOptions3 != TimeSpanFormatOptions.TruncateFill)
				{
					if (timeSpanFormatOptions3 == TimeSpanFormatOptions.TruncateFull)
					{
						flag3 = true;
					}
				}
				else if (flag2 || num > 0)
				{
					flag3 = true;
				}
				if (!flag4)
				{
					if (timeSpanFormatOptions4 == timeSpanFormatOptions2 && !flag2)
					{
						flag3 = true;
						if (flag && num < 1)
						{
							string unitText = timeTextInfo.GetUnitText(timeSpanFormatOptions2, 1, abbr);
							stringBuilder.Append(timeTextInfo.GetLessThanText(unitText));
							flag3 = false;
						}
					}
					if (flag3)
					{
						if (flag2)
						{
							stringBuilder.Append(" ");
						}
						string unitText2 = timeTextInfo.GetUnitText(timeSpanFormatOptions4, num, abbr);
						stringBuilder.Append(unitText2);
						flag2 = true;
					}
					timeSpanFormatOptions4 >>= 1;
					continue;
				}
				break;
				IL_2B2:
				throw new ArgumentException("TimeSpanUtility");
			}
			string result = stringBuilder.ToString();
			StringBuilderPool.Release(stringBuilder);
			return result;
		}

		public static TimeSpanFormatOptions DefaultFormatOptions { get; set; } = TimeSpanFormatOptions.AbbreviateOff | TimeSpanFormatOptions.LessThan | TimeSpanFormatOptions.TruncateAuto | TimeSpanFormatOptions.RangeSeconds | TimeSpanFormatOptions.RangeDays;

		public static TimeSpanFormatOptions AbsoluteDefaults { get; } = TimeSpanUtility.DefaultFormatOptions;

		public static TimeSpan Round(this TimeSpan fromTime, long intervalTicks)
		{
			long num = fromTime.Ticks % intervalTicks;
			if (num >= intervalTicks >> 1)
			{
				num -= intervalTicks;
			}
			return TimeSpan.FromTicks(fromTime.Ticks - num);
		}

		internal const TimeSpanFormatOptions AbbreviateAll = TimeSpanFormatOptions.Abbreviate | TimeSpanFormatOptions.AbbreviateOff;

		internal const TimeSpanFormatOptions LessThanAll = TimeSpanFormatOptions.LessThan | TimeSpanFormatOptions.LessThanOff;

		internal const TimeSpanFormatOptions RangeAll = TimeSpanFormatOptions.RangeMilliSeconds | TimeSpanFormatOptions.RangeSeconds | TimeSpanFormatOptions.RangeMinutes | TimeSpanFormatOptions.RangeHours | TimeSpanFormatOptions.RangeDays | TimeSpanFormatOptions.RangeWeeks;

		internal const TimeSpanFormatOptions TruncateAll = TimeSpanFormatOptions.TruncateShortest | TimeSpanFormatOptions.TruncateAuto | TimeSpanFormatOptions.TruncateFill | TimeSpanFormatOptions.TruncateFull;
	}
}
