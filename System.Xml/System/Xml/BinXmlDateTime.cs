using System;
using System.Globalization;
using System.Text;

namespace System.Xml
{
	internal abstract class BinXmlDateTime
	{
		private static void Write2Dig(StringBuilder sb, int val)
		{
			sb.Append((char)(48 + val / 10));
			sb.Append((char)(48 + val % 10));
		}

		private static void Write4DigNeg(StringBuilder sb, int val)
		{
			if (val < 0)
			{
				val = -val;
				sb.Append('-');
			}
			BinXmlDateTime.Write2Dig(sb, val / 100);
			BinXmlDateTime.Write2Dig(sb, val % 100);
		}

		private static void Write3Dec(StringBuilder sb, int val)
		{
			int num = val % 10;
			val /= 10;
			int num2 = val % 10;
			val /= 10;
			int num3 = val;
			sb.Append('.');
			sb.Append((char)(48 + num3));
			sb.Append((char)(48 + num2));
			sb.Append((char)(48 + num));
		}

		private static void WriteDate(StringBuilder sb, int yr, int mnth, int day)
		{
			BinXmlDateTime.Write4DigNeg(sb, yr);
			sb.Append('-');
			BinXmlDateTime.Write2Dig(sb, mnth);
			sb.Append('-');
			BinXmlDateTime.Write2Dig(sb, day);
		}

		private static void WriteTime(StringBuilder sb, int hr, int min, int sec, int ms)
		{
			BinXmlDateTime.Write2Dig(sb, hr);
			sb.Append(':');
			BinXmlDateTime.Write2Dig(sb, min);
			sb.Append(':');
			BinXmlDateTime.Write2Dig(sb, sec);
			if (ms != 0)
			{
				BinXmlDateTime.Write3Dec(sb, ms);
			}
		}

		private static void WriteTimeFullPrecision(StringBuilder sb, int hr, int min, int sec, int fraction)
		{
			BinXmlDateTime.Write2Dig(sb, hr);
			sb.Append(':');
			BinXmlDateTime.Write2Dig(sb, min);
			sb.Append(':');
			BinXmlDateTime.Write2Dig(sb, sec);
			if (fraction != 0)
			{
				int i = 7;
				while (fraction % 10 == 0)
				{
					i--;
					fraction /= 10;
				}
				char[] array = new char[i];
				while (i > 0)
				{
					i--;
					array[i] = (char)(fraction % 10 + 48);
					fraction /= 10;
				}
				sb.Append('.');
				sb.Append(array);
			}
		}

		private static void WriteTimeZone(StringBuilder sb, TimeSpan zone)
		{
			bool negTimeZone = true;
			if (zone.Ticks < 0L)
			{
				negTimeZone = false;
				zone = zone.Negate();
			}
			BinXmlDateTime.WriteTimeZone(sb, negTimeZone, zone.Hours, zone.Minutes);
		}

		private static void WriteTimeZone(StringBuilder sb, bool negTimeZone, int hr, int min)
		{
			if (hr == 0 && min == 0)
			{
				sb.Append('Z');
				return;
			}
			sb.Append(negTimeZone ? '+' : '-');
			BinXmlDateTime.Write2Dig(sb, hr);
			sb.Append(':');
			BinXmlDateTime.Write2Dig(sb, min);
		}

		private static void BreakDownXsdDateTime(long val, out int yr, out int mnth, out int day, out int hr, out int min, out int sec, out int ms)
		{
			if (val >= 0L)
			{
				long num = val / 4L;
				ms = (int)(num % 1000L);
				num /= 1000L;
				sec = (int)(num % 60L);
				num /= 60L;
				min = (int)(num % 60L);
				num /= 60L;
				hr = (int)(num % 24L);
				num /= 24L;
				day = (int)(num % 31L) + 1;
				num /= 31L;
				mnth = (int)(num % 12L) + 1;
				num /= 12L;
				yr = (int)(num - 9999L);
				if (yr >= -9999 && yr <= 9999)
				{
					return;
				}
			}
			throw new XmlException("Arithmetic Overflow.", null);
		}

		private static void BreakDownXsdDate(long val, out int yr, out int mnth, out int day, out bool negTimeZone, out int hr, out int min)
		{
			if (val >= 0L)
			{
				val /= 4L;
				int num = (int)(val % 1740L) - 840;
				long num2 = val / 1740L;
				if (negTimeZone = (num < 0))
				{
					num = -num;
				}
				min = num % 60;
				hr = num / 60;
				day = (int)(num2 % 31L) + 1;
				num2 /= 31L;
				mnth = (int)(num2 % 12L) + 1;
				yr = (int)(num2 / 12L) - 9999;
				if (yr >= -9999 && yr <= 9999)
				{
					return;
				}
			}
			throw new XmlException("Arithmetic Overflow.", null);
		}

		private static void BreakDownXsdTime(long val, out int hr, out int min, out int sec, out int ms)
		{
			if (val >= 0L)
			{
				val /= 4L;
				ms = (int)(val % 1000L);
				val /= 1000L;
				sec = (int)(val % 60L);
				val /= 60L;
				min = (int)(val % 60L);
				hr = (int)(val / 60L);
				if (0 <= hr && hr <= 23)
				{
					return;
				}
			}
			throw new XmlException("Arithmetic Overflow.", null);
		}

		public static string XsdDateTimeToString(long val)
		{
			int yr;
			int mnth;
			int day;
			int hr;
			int min;
			int sec;
			int ms;
			BinXmlDateTime.BreakDownXsdDateTime(val, out yr, out mnth, out day, out hr, out min, out sec, out ms);
			StringBuilder stringBuilder = new StringBuilder(20);
			BinXmlDateTime.WriteDate(stringBuilder, yr, mnth, day);
			stringBuilder.Append('T');
			BinXmlDateTime.WriteTime(stringBuilder, hr, min, sec, ms);
			stringBuilder.Append('Z');
			return stringBuilder.ToString();
		}

		public static DateTime XsdDateTimeToDateTime(long val)
		{
			int year;
			int month;
			int day;
			int hour;
			int minute;
			int second;
			int millisecond;
			BinXmlDateTime.BreakDownXsdDateTime(val, out year, out month, out day, out hour, out minute, out second, out millisecond);
			return new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);
		}

		public static string XsdDateToString(long val)
		{
			int yr;
			int mnth;
			int day;
			bool negTimeZone;
			int hr;
			int min;
			BinXmlDateTime.BreakDownXsdDate(val, out yr, out mnth, out day, out negTimeZone, out hr, out min);
			StringBuilder stringBuilder = new StringBuilder(20);
			BinXmlDateTime.WriteDate(stringBuilder, yr, mnth, day);
			BinXmlDateTime.WriteTimeZone(stringBuilder, negTimeZone, hr, min);
			return stringBuilder.ToString();
		}

		public static DateTime XsdDateToDateTime(long val)
		{
			int year;
			int month;
			int day;
			bool flag;
			int num;
			int num2;
			BinXmlDateTime.BreakDownXsdDate(val, out year, out month, out day, out flag, out num, out num2);
			DateTime dateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
			int num3 = (flag ? -1 : 1) * (num * 60 + num2);
			return TimeZone.CurrentTimeZone.ToLocalTime(dateTime.AddMinutes((double)num3));
		}

		public static string XsdTimeToString(long val)
		{
			int hr;
			int min;
			int sec;
			int ms;
			BinXmlDateTime.BreakDownXsdTime(val, out hr, out min, out sec, out ms);
			StringBuilder stringBuilder = new StringBuilder(16);
			BinXmlDateTime.WriteTime(stringBuilder, hr, min, sec, ms);
			stringBuilder.Append('Z');
			return stringBuilder.ToString();
		}

		public static DateTime XsdTimeToDateTime(long val)
		{
			int hour;
			int minute;
			int second;
			int millisecond;
			BinXmlDateTime.BreakDownXsdTime(val, out hour, out minute, out second, out millisecond);
			return new DateTime(1, 1, 1, hour, minute, second, millisecond, DateTimeKind.Utc);
		}

		public static string SqlDateTimeToString(int dateticks, uint timeticks)
		{
			DateTime dateTime = BinXmlDateTime.SqlDateTimeToDateTime(dateticks, timeticks);
			string format = (dateTime.Millisecond != 0) ? "yyyy/MM/dd\\THH:mm:ss.ffff" : "yyyy/MM/dd\\THH:mm:ss";
			return dateTime.ToString(format, CultureInfo.InvariantCulture);
		}

		public static DateTime SqlDateTimeToDateTime(int dateticks, uint timeticks)
		{
			DateTime dateTime = new DateTime(1900, 1, 1);
			long num = (long)(timeticks / BinXmlDateTime.SQLTicksPerMillisecond + 0.5);
			return dateTime.Add(new TimeSpan((long)dateticks * 864000000000L + num * 10000L));
		}

		public static string SqlSmallDateTimeToString(short dateticks, ushort timeticks)
		{
			return BinXmlDateTime.SqlSmallDateTimeToDateTime(dateticks, timeticks).ToString("yyyy/MM/dd\\THH:mm:ss", CultureInfo.InvariantCulture);
		}

		public static DateTime SqlSmallDateTimeToDateTime(short dateticks, ushort timeticks)
		{
			return BinXmlDateTime.SqlDateTimeToDateTime((int)dateticks, (uint)((int)timeticks * BinXmlDateTime.SQLTicksPerMinute));
		}

		public static DateTime XsdKatmaiDateToDateTime(byte[] data, int offset)
		{
			long katmaiDateTicks = BinXmlDateTime.GetKatmaiDateTicks(data, ref offset);
			return new DateTime(katmaiDateTicks);
		}

		public static DateTime XsdKatmaiDateTimeToDateTime(byte[] data, int offset)
		{
			long katmaiTimeTicks = BinXmlDateTime.GetKatmaiTimeTicks(data, ref offset);
			long katmaiDateTicks = BinXmlDateTime.GetKatmaiDateTicks(data, ref offset);
			return new DateTime(katmaiDateTicks + katmaiTimeTicks);
		}

		public static DateTime XsdKatmaiTimeToDateTime(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateTimeToDateTime(data, offset);
		}

		public static DateTime XsdKatmaiDateOffsetToDateTime(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateOffsetToDateTimeOffset(data, offset).LocalDateTime;
		}

		public static DateTime XsdKatmaiDateTimeOffsetToDateTime(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset).LocalDateTime;
		}

		public static DateTime XsdKatmaiTimeOffsetToDateTime(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiTimeOffsetToDateTimeOffset(data, offset).LocalDateTime;
		}

		public static DateTimeOffset XsdKatmaiDateToDateTimeOffset(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateToDateTime(data, offset);
		}

		public static DateTimeOffset XsdKatmaiDateTimeToDateTimeOffset(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateTimeToDateTime(data, offset);
		}

		public static DateTimeOffset XsdKatmaiTimeToDateTimeOffset(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiTimeToDateTime(data, offset);
		}

		public static DateTimeOffset XsdKatmaiDateOffsetToDateTimeOffset(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
		}

		public static DateTimeOffset XsdKatmaiDateTimeOffsetToDateTimeOffset(byte[] data, int offset)
		{
			long katmaiTimeTicks = BinXmlDateTime.GetKatmaiTimeTicks(data, ref offset);
			long katmaiDateTicks = BinXmlDateTime.GetKatmaiDateTicks(data, ref offset);
			long katmaiTimeZoneTicks = BinXmlDateTime.GetKatmaiTimeZoneTicks(data, offset);
			return new DateTimeOffset(katmaiDateTicks + katmaiTimeTicks + katmaiTimeZoneTicks, new TimeSpan(katmaiTimeZoneTicks));
		}

		public static DateTimeOffset XsdKatmaiTimeOffsetToDateTimeOffset(byte[] data, int offset)
		{
			return BinXmlDateTime.XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
		}

		public static string XsdKatmaiDateToString(byte[] data, int offset)
		{
			DateTime dateTime = BinXmlDateTime.XsdKatmaiDateToDateTime(data, offset);
			StringBuilder stringBuilder = new StringBuilder(10);
			BinXmlDateTime.WriteDate(stringBuilder, dateTime.Year, dateTime.Month, dateTime.Day);
			return stringBuilder.ToString();
		}

		public static string XsdKatmaiDateTimeToString(byte[] data, int offset)
		{
			DateTime dt = BinXmlDateTime.XsdKatmaiDateTimeToDateTime(data, offset);
			StringBuilder stringBuilder = new StringBuilder(33);
			BinXmlDateTime.WriteDate(stringBuilder, dt.Year, dt.Month, dt.Day);
			stringBuilder.Append('T');
			BinXmlDateTime.WriteTimeFullPrecision(stringBuilder, dt.Hour, dt.Minute, dt.Second, BinXmlDateTime.GetFractions(dt));
			return stringBuilder.ToString();
		}

		public static string XsdKatmaiTimeToString(byte[] data, int offset)
		{
			DateTime dt = BinXmlDateTime.XsdKatmaiTimeToDateTime(data, offset);
			StringBuilder stringBuilder = new StringBuilder(16);
			BinXmlDateTime.WriteTimeFullPrecision(stringBuilder, dt.Hour, dt.Minute, dt.Second, BinXmlDateTime.GetFractions(dt));
			return stringBuilder.ToString();
		}

		public static string XsdKatmaiDateOffsetToString(byte[] data, int offset)
		{
			DateTimeOffset dateTimeOffset = BinXmlDateTime.XsdKatmaiDateOffsetToDateTimeOffset(data, offset);
			StringBuilder stringBuilder = new StringBuilder(16);
			BinXmlDateTime.WriteDate(stringBuilder, dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day);
			BinXmlDateTime.WriteTimeZone(stringBuilder, dateTimeOffset.Offset);
			return stringBuilder.ToString();
		}

		public static string XsdKatmaiDateTimeOffsetToString(byte[] data, int offset)
		{
			DateTimeOffset dt = BinXmlDateTime.XsdKatmaiDateTimeOffsetToDateTimeOffset(data, offset);
			StringBuilder stringBuilder = new StringBuilder(39);
			BinXmlDateTime.WriteDate(stringBuilder, dt.Year, dt.Month, dt.Day);
			stringBuilder.Append('T');
			BinXmlDateTime.WriteTimeFullPrecision(stringBuilder, dt.Hour, dt.Minute, dt.Second, BinXmlDateTime.GetFractions(dt));
			BinXmlDateTime.WriteTimeZone(stringBuilder, dt.Offset);
			return stringBuilder.ToString();
		}

		public static string XsdKatmaiTimeOffsetToString(byte[] data, int offset)
		{
			DateTimeOffset dt = BinXmlDateTime.XsdKatmaiTimeOffsetToDateTimeOffset(data, offset);
			StringBuilder stringBuilder = new StringBuilder(22);
			BinXmlDateTime.WriteTimeFullPrecision(stringBuilder, dt.Hour, dt.Minute, dt.Second, BinXmlDateTime.GetFractions(dt));
			BinXmlDateTime.WriteTimeZone(stringBuilder, dt.Offset);
			return stringBuilder.ToString();
		}

		private static long GetKatmaiDateTicks(byte[] data, ref int pos)
		{
			int num = pos;
			pos = num + 3;
			return (long)((int)data[num] | (int)data[num + 1] << 8 | (int)data[num + 2] << 16) * 864000000000L;
		}

		private static long GetKatmaiTimeTicks(byte[] data, ref int pos)
		{
			int num = pos;
			byte b = data[num];
			num++;
			long num2;
			if (b <= 2)
			{
				num2 = (long)((int)data[num] | (int)data[num + 1] << 8 | (int)data[num + 2] << 16);
				pos = num + 3;
			}
			else if (b <= 4)
			{
				num2 = (long)((int)data[num] | (int)data[num + 1] << 8 | (int)data[num + 2] << 16);
				num2 |= (long)((long)((ulong)data[num + 3]) << 24);
				pos = num + 4;
			}
			else
			{
				if (b > 7)
				{
					throw new XmlException("Arithmetic Overflow.", null);
				}
				num2 = (long)((int)data[num] | (int)data[num + 1] << 8 | (int)data[num + 2] << 16);
				num2 |= (long)((ulong)data[num + 3] << 24 | (ulong)data[num + 4] << 32);
				pos = num + 5;
			}
			return num2 * (long)BinXmlDateTime.KatmaiTimeScaleMultiplicator[(int)b];
		}

		private static long GetKatmaiTimeZoneTicks(byte[] data, int pos)
		{
			return (long)((short)((int)data[pos] | (int)data[pos + 1] << 8)) * 600000000L;
		}

		private static int GetFractions(DateTime dt)
		{
			return (int)(dt.Ticks - new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second).Ticks);
		}

		private static int GetFractions(DateTimeOffset dt)
		{
			return (int)(dt.Ticks - new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second).Ticks);
		}

		private const int MaxFractionDigits = 7;

		internal static int[] KatmaiTimeScaleMultiplicator = new int[]
		{
			10000000,
			1000000,
			100000,
			10000,
			1000,
			100,
			10,
			1
		};

		private static readonly double SQLTicksPerMillisecond = 0.3;

		public static readonly int SQLTicksPerSecond = 300;

		public static readonly int SQLTicksPerMinute = BinXmlDateTime.SQLTicksPerSecond * 60;

		public static readonly int SQLTicksPerHour = BinXmlDateTime.SQLTicksPerMinute * 60;

		private static readonly int SQLTicksPerDay = BinXmlDateTime.SQLTicksPerHour * 24;
	}
}
