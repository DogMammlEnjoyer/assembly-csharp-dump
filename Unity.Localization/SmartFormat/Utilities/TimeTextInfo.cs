using System;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	public class TimeTextInfo
	{
		public TimeTextInfo(PluralRules.PluralRuleDelegate pluralRule, string[] week, string[] day, string[] hour, string[] minute, string[] second, string[] millisecond, string[] w, string[] d, string[] h, string[] m, string[] s, string[] ms, string lessThan)
		{
			this.PluralRule = pluralRule;
			this.week = week;
			this.day = day;
			this.hour = hour;
			this.minute = minute;
			this.second = second;
			this.millisecond = millisecond;
			this.w = w;
			this.d = d;
			this.h = h;
			this.m = m;
			this.s = s;
			this.ms = ms;
			this.lessThan = lessThan;
		}

		public TimeTextInfo(string week, string day, string hour, string minute, string second, string millisecond, string lessThan)
		{
			this.d = (this.h = (this.m = (this.ms = (this.s = (this.w = new string[0])))));
			this.PluralRule = ((decimal d, int c) => 0);
			this.week = new string[]
			{
				week
			};
			this.day = new string[]
			{
				day
			};
			this.hour = new string[]
			{
				hour
			};
			this.minute = new string[]
			{
				minute
			};
			this.second = new string[]
			{
				second
			};
			this.millisecond = new string[]
			{
				millisecond
			};
			this.lessThan = lessThan;
		}

		private static string GetValue(PluralRules.PluralRuleDelegate pluralRule, int value, string[] units)
		{
			int num = (units.Length == 1) ? 0 : pluralRule(value, units.Length);
			return string.Format(units[num], value);
		}

		public string GetLessThanText(string minimumValue)
		{
			return string.Format(this.lessThan, minimumValue);
		}

		public virtual string GetUnitText(TimeSpanFormatOptions unit, int value, bool abbr)
		{
			if (unit <= TimeSpanFormatOptions.RangeMinutes)
			{
				if (unit == TimeSpanFormatOptions.RangeMilliSeconds)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.ms : this.millisecond);
				}
				if (unit == TimeSpanFormatOptions.RangeSeconds)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.s : this.second);
				}
				if (unit == TimeSpanFormatOptions.RangeMinutes)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.m : this.minute);
				}
			}
			else
			{
				if (unit == TimeSpanFormatOptions.RangeHours)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.h : this.hour);
				}
				if (unit == TimeSpanFormatOptions.RangeDays)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.d : this.day);
				}
				if (unit == TimeSpanFormatOptions.RangeWeeks)
				{
					return TimeTextInfo.GetValue(this.PluralRule, value, abbr ? this.w : this.week);
				}
			}
			return null;
		}

		private readonly string[] d;

		private readonly string[] day;

		private readonly string[] h;

		private readonly string[] hour;

		private readonly string lessThan;

		private readonly string[] m;

		private readonly string[] millisecond;

		private readonly string[] minute;

		private readonly string[] ms;

		private readonly PluralRules.PluralRuleDelegate PluralRule;

		private readonly string[] s;

		private readonly string[] second;

		private readonly string[] w;

		private readonly string[] week;
	}
}
