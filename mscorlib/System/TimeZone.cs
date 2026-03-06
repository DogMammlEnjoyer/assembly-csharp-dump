using System;
using System.Globalization;
using System.Threading;

namespace System
{
	/// <summary>Represents a time zone.</summary>
	[Obsolete("System.TimeZone has been deprecated.  Please investigate the use of System.TimeZoneInfo instead.")]
	[Serializable]
	public abstract class TimeZone
	{
		private static object InternalSyncObject
		{
			get
			{
				if (TimeZone.s_InternalSyncObject == null)
				{
					object value = new object();
					Interlocked.CompareExchange<object>(ref TimeZone.s_InternalSyncObject, value, null);
				}
				return TimeZone.s_InternalSyncObject;
			}
		}

		/// <summary>Gets the time zone of the current computer.</summary>
		/// <returns>A <see cref="T:System.TimeZone" /> object that represents the current local time zone.</returns>
		public static TimeZone CurrentTimeZone
		{
			get
			{
				TimeZone timeZone = TimeZone.currentTimeZone;
				if (timeZone == null)
				{
					object internalSyncObject = TimeZone.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (TimeZone.currentTimeZone == null)
						{
							TimeZone.currentTimeZone = new CurrentSystemTimeZone();
						}
						timeZone = TimeZone.currentTimeZone;
					}
				}
				return timeZone;
			}
		}

		internal static void ResetTimeZone()
		{
			if (TimeZone.currentTimeZone != null)
			{
				object internalSyncObject = TimeZone.InternalSyncObject;
				lock (internalSyncObject)
				{
					TimeZone.currentTimeZone = null;
				}
			}
		}

		/// <summary>Gets the standard time zone name.</summary>
		/// <returns>The standard time zone name.</returns>
		/// <exception cref="T:System.ArgumentNullException">An attempt was made to set this property to <see langword="null" />.</exception>
		public abstract string StandardName { get; }

		/// <summary>Gets the daylight saving time zone name.</summary>
		/// <returns>The daylight saving time zone name.</returns>
		public abstract string DaylightName { get; }

		/// <summary>Returns the Coordinated Universal Time (UTC) offset for the specified local time.</summary>
		/// <param name="time">A date and time value.</param>
		/// <returns>The Coordinated Universal Time (UTC) offset from <paramref name="time" />.</returns>
		public abstract TimeSpan GetUtcOffset(DateTime time);

		/// <summary>Returns the Coordinated Universal Time (UTC) that corresponds to a specified time.</summary>
		/// <param name="time">A date and time.</param>
		/// <returns>A <see cref="T:System.DateTime" /> object whose value is the Coordinated Universal Time (UTC) that corresponds to <paramref name="time" />.</returns>
		public virtual DateTime ToUniversalTime(DateTime time)
		{
			if (time.Kind == DateTimeKind.Utc)
			{
				return time;
			}
			long num = time.Ticks - this.GetUtcOffset(time).Ticks;
			if (num > 3155378975999999999L)
			{
				return new DateTime(3155378975999999999L, DateTimeKind.Utc);
			}
			if (num < 0L)
			{
				return new DateTime(0L, DateTimeKind.Utc);
			}
			return new DateTime(num, DateTimeKind.Utc);
		}

		/// <summary>Returns the local time that corresponds to a specified date and time value.</summary>
		/// <param name="time">A Coordinated Universal Time (UTC) time.</param>
		/// <returns>A <see cref="T:System.DateTime" /> object whose value is the local time that corresponds to <paramref name="time" />.</returns>
		public virtual DateTime ToLocalTime(DateTime time)
		{
			if (time.Kind == DateTimeKind.Local)
			{
				return time;
			}
			bool isAmbiguousDst = false;
			long utcOffsetFromUniversalTime = ((CurrentSystemTimeZone)TimeZone.CurrentTimeZone).GetUtcOffsetFromUniversalTime(time, ref isAmbiguousDst);
			return new DateTime(time.Ticks + utcOffsetFromUniversalTime, DateTimeKind.Local, isAmbiguousDst);
		}

		/// <summary>Returns the daylight saving time period for a particular year.</summary>
		/// <param name="year">The year that the daylight saving time period applies to.</param>
		/// <returns>A <see cref="T:System.Globalization.DaylightTime" /> object that contains the start and end date for daylight saving time in <paramref name="year" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" /> is less than 1 or greater than 9999.</exception>
		public abstract DaylightTime GetDaylightChanges(int year);

		/// <summary>Returns a value indicating whether the specified date and time is within a daylight saving time period.</summary>
		/// <param name="time">A date and time.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="time" /> is in a daylight saving time period; otherwise, <see langword="false" />.</returns>
		public virtual bool IsDaylightSavingTime(DateTime time)
		{
			return TimeZone.IsDaylightSavingTime(time, this.GetDaylightChanges(time.Year));
		}

		/// <summary>Returns a value indicating whether the specified date and time is within the specified daylight saving time period.</summary>
		/// <param name="time">A date and time.</param>
		/// <param name="daylightTimes">A daylight saving time period.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="time" /> is in <paramref name="daylightTimes" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="daylightTimes" /> is <see langword="null" />.</exception>
		public static bool IsDaylightSavingTime(DateTime time, DaylightTime daylightTimes)
		{
			return TimeZone.CalculateUtcOffset(time, daylightTimes) != TimeSpan.Zero;
		}

		internal static TimeSpan CalculateUtcOffset(DateTime time, DaylightTime daylightTimes)
		{
			if (daylightTimes == null)
			{
				return TimeSpan.Zero;
			}
			if (time.Kind == DateTimeKind.Utc)
			{
				return TimeSpan.Zero;
			}
			DateTime dateTime = daylightTimes.Start + daylightTimes.Delta;
			DateTime end = daylightTimes.End;
			DateTime t;
			DateTime t2;
			if (daylightTimes.Delta.Ticks > 0L)
			{
				t = end - daylightTimes.Delta;
				t2 = end;
			}
			else
			{
				t = dateTime;
				t2 = dateTime - daylightTimes.Delta;
			}
			bool flag = false;
			if (dateTime > end)
			{
				if (time >= dateTime || time < end)
				{
					flag = true;
				}
			}
			else if (time >= dateTime && time < end)
			{
				flag = true;
			}
			if (flag && time >= t && time < t2)
			{
				flag = time.IsAmbiguousDaylightSavingTime();
			}
			if (flag)
			{
				return daylightTimes.Delta;
			}
			return TimeSpan.Zero;
		}

		internal static void ClearCachedData()
		{
			TimeZone.currentTimeZone = null;
		}

		private static volatile TimeZone currentTimeZone;

		private static object s_InternalSyncObject;
	}
}
