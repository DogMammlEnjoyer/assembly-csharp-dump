using System;

namespace UniLabs.Time
{
	public static class TimeUnitExtensions
	{
		public static string ToShortString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "ms";
				break;
			case TimeUnit.Seconds:
				result = "s";
				break;
			case TimeUnit.Minutes:
				result = "m";
				break;
			case TimeUnit.Hours:
				result = "h";
				break;
			case TimeUnit.Days:
				result = "D";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static string ToSeparatorString(this TimeUnit timeUnit)
		{
			string result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = "";
				break;
			case TimeUnit.Milliseconds:
				result = "";
				break;
			case TimeUnit.Seconds:
				result = ".";
				break;
			case TimeUnit.Minutes:
				result = ":";
				break;
			case TimeUnit.Hours:
				result = ":";
				break;
			case TimeUnit.Days:
				result = ".";
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static double GetUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			int num;
			switch (timeUnit)
			{
			case TimeUnit.None:
				num = 0;
				break;
			case TimeUnit.Milliseconds:
				num = timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				num = timeSpan.Seconds;
				break;
			case TimeUnit.Minutes:
				num = timeSpan.Minutes;
				break;
			case TimeUnit.Hours:
				num = timeSpan.Hours;
				break;
			case TimeUnit.Days:
				num = timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return (double)num;
		}

		public static TimeSpan WithUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.Add(TimeSpan.FromMilliseconds(value - (double)timeSpan.Milliseconds));
				break;
			case TimeUnit.Seconds:
				result = timeSpan.Add(TimeSpan.FromSeconds(value - (double)timeSpan.Seconds));
				break;
			case TimeUnit.Minutes:
				result = timeSpan.Add(TimeSpan.FromMinutes(value - (double)timeSpan.Minutes));
				break;
			case TimeUnit.Hours:
				result = timeSpan.Add(TimeSpan.FromHours(value - (double)timeSpan.Hours));
				break;
			case TimeUnit.Days:
				result = timeSpan.Add(TimeSpan.FromDays(value - (double)timeSpan.Days));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static double GetLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = (double)timeSpan.Milliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static TimeSpan WithLowestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, (int)value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static double GetHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = 0.0;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds).TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0).TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0).TotalHours;
				break;
			case TimeUnit.Days:
				result = (double)timeSpan.Days;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static TimeSpan WithHighestUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(0, 0, 0, 0, timeSpan.Milliseconds).Add(TimeSpan.FromSeconds(value));
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(0, 0, 0, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromMinutes(value));
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(0, 0, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromHours(value));
				break;
			case TimeUnit.Days:
				result = new TimeSpan(0, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds).Add(TimeSpan.FromDays(value));
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static double GetSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			double result;
			switch (timeUnit)
			{
			case TimeUnit.Milliseconds:
				result = timeSpan.TotalMilliseconds;
				break;
			case TimeUnit.Seconds:
				result = timeSpan.TotalSeconds;
				break;
			case TimeUnit.Minutes:
				result = timeSpan.TotalMinutes;
				break;
			case TimeUnit.Hours:
				result = timeSpan.TotalHours;
				break;
			case TimeUnit.Days:
				result = timeSpan.TotalDays;
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static TimeSpan FromSingleUnitValue(this TimeSpan timeSpan, TimeUnit timeUnit, double value)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = TimeSpan.Zero;
				break;
			case TimeUnit.Milliseconds:
				result = TimeSpan.FromMilliseconds(value);
				break;
			case TimeUnit.Seconds:
				result = TimeSpan.FromSeconds(value);
				break;
			case TimeUnit.Minutes:
				result = TimeSpan.FromMinutes(value);
				break;
			case TimeUnit.Hours:
				result = TimeSpan.FromHours(value);
				break;
			case TimeUnit.Days:
				result = TimeSpan.FromDays(value);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public static TimeSpan SnapToUnit(this TimeSpan timeSpan, TimeUnit timeUnit)
		{
			TimeSpan result;
			switch (timeUnit)
			{
			case TimeUnit.None:
				result = timeSpan;
				break;
			case TimeUnit.Milliseconds:
				result = timeSpan;
				break;
			case TimeUnit.Seconds:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
				break;
			case TimeUnit.Minutes:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, 0);
				break;
			case TimeUnit.Hours:
				result = new TimeSpan(timeSpan.Days, timeSpan.Hours, 0, 0);
				break;
			case TimeUnit.Days:
				result = new TimeSpan(timeSpan.Days, 0, 0, 0);
				break;
			default:
				throw new ArgumentOutOfRangeException("timeUnit", timeUnit, null);
			}
			return result;
		}

		public delegate TimeSpan WithUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit, double value);

		public delegate double GetUnitValueDelegate(TimeSpan timeSpan, TimeUnit timeUnit);
	}
}
