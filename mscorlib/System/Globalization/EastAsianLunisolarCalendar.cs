using System;
using System.Runtime.InteropServices;

namespace System.Globalization
{
	/// <summary>Represents a calendar that divides time into months, days, years, and eras, and has dates that are based on cycles of the sun and the moon.</summary>
	[ComVisible(true)]
	[Serializable]
	public abstract class EastAsianLunisolarCalendar : Calendar
	{
		/// <summary>Gets a value indicating whether the current calendar is solar-based, lunar-based, or a combination of both.</summary>
		/// <returns>Always returns <see cref="F:System.Globalization.CalendarAlgorithmType.LunisolarCalendar" />.</returns>
		public override CalendarAlgorithmType AlgorithmType
		{
			get
			{
				return CalendarAlgorithmType.LunisolarCalendar;
			}
		}

		/// <summary>Calculates the year in the sexagenary (60-year) cycle that corresponds to the specified date.</summary>
		/// <param name="time">A <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>A number from 1 through 60 in the sexagenary cycle that corresponds to the <paramref name="date" /> parameter.</returns>
		public virtual int GetSexagenaryYear(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.TimeToLunar(time, ref num, ref num2, ref num3);
			return (num - 4) % 60 + 1;
		}

		/// <summary>Calculates the celestial stem of the specified year in the sexagenary (60-year) cycle.</summary>
		/// <param name="sexagenaryYear">An integer from 1 through 60 that represents a year in the sexagenary cycle.</param>
		/// <returns>A number from 1 through 10.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="sexagenaryYear" /> is less than 1 or greater than 60.</exception>
		public int GetCelestialStem(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || sexagenaryYear > 60)
			{
				throw new ArgumentOutOfRangeException("sexagenaryYear", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					1,
					60
				}));
			}
			return (sexagenaryYear - 1) % 10 + 1;
		}

		/// <summary>Calculates the terrestrial branch of the specified year in the sexagenary (60-year) cycle.</summary>
		/// <param name="sexagenaryYear">An integer from 1 through 60 that represents a year in the sexagenary cycle.</param>
		/// <returns>An integer from 1 through 12.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="sexagenaryYear" /> is less than 1 or greater than 60.</exception>
		public int GetTerrestrialBranch(int sexagenaryYear)
		{
			if (sexagenaryYear < 1 || sexagenaryYear > 60)
			{
				throw new ArgumentOutOfRangeException("sexagenaryYear", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					1,
					60
				}));
			}
			return (sexagenaryYear - 1) % 12 + 1;
		}

		internal abstract int GetYearInfo(int LunarYear, int Index);

		internal abstract int GetYear(int year, DateTime time);

		internal abstract int GetGregorianYear(int year, int era);

		internal abstract int MinCalendarYear { get; }

		internal abstract int MaxCalendarYear { get; }

		internal abstract EraInfo[] CalEraInfo { get; }

		internal abstract DateTime MinDate { get; }

		internal abstract DateTime MaxDate { get; }

		internal int MinEraCalendarYear(int era)
		{
			EraInfo[] calEraInfo = this.CalEraInfo;
			if (calEraInfo == null)
			{
				return this.MinCalendarYear;
			}
			if (era == 0)
			{
				era = this.CurrentEraValue;
			}
			if (era == this.GetEra(this.MinDate))
			{
				return this.GetYear(this.MinCalendarYear, this.MinDate);
			}
			for (int i = 0; i < calEraInfo.Length; i++)
			{
				if (era == calEraInfo[i].era)
				{
					return calEraInfo[i].minEraYear;
				}
			}
			throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
		}

		internal int MaxEraCalendarYear(int era)
		{
			EraInfo[] calEraInfo = this.CalEraInfo;
			if (calEraInfo == null)
			{
				return this.MaxCalendarYear;
			}
			if (era == 0)
			{
				era = this.CurrentEraValue;
			}
			if (era == this.GetEra(this.MaxDate))
			{
				return this.GetYear(this.MaxCalendarYear, this.MaxDate);
			}
			for (int i = 0; i < calEraInfo.Length; i++)
			{
				if (era == calEraInfo[i].era)
				{
					return calEraInfo[i].maxEraYear;
				}
			}
			throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
		}

		internal EastAsianLunisolarCalendar()
		{
		}

		internal void CheckTicksRange(long ticks)
		{
			if (ticks < this.MinSupportedDateTime.Ticks || ticks > this.MaxSupportedDateTime.Ticks)
			{
				throw new ArgumentOutOfRangeException("time", string.Format(CultureInfo.InvariantCulture, Environment.GetResourceString("Specified time is not supported in this calendar. It should be between {0} (Gregorian date) and {1} (Gregorian date), inclusive."), this.MinSupportedDateTime, this.MaxSupportedDateTime));
			}
		}

		internal void CheckEraRange(int era)
		{
			if (era == 0)
			{
				era = this.CurrentEraValue;
			}
			if (era < this.GetEra(this.MinDate) || era > this.GetEra(this.MaxDate))
			{
				throw new ArgumentOutOfRangeException("era", Environment.GetResourceString("Era value was not valid."));
			}
		}

		internal int CheckYearRange(int year, int era)
		{
			this.CheckEraRange(era);
			year = this.GetGregorianYear(year, era);
			if (year < this.MinCalendarYear || year > this.MaxCalendarYear)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					this.MinEraCalendarYear(era),
					this.MaxEraCalendarYear(era)
				}));
			}
			return year;
		}

		internal int CheckYearMonthRange(int year, int month, int era)
		{
			year = this.CheckYearRange(year, era);
			if (month == 13 && this.GetYearInfo(year, 0) == 0)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("Month must be between one and twelve."));
			}
			if (month < 1 || month > 13)
			{
				throw new ArgumentOutOfRangeException("month", Environment.GetResourceString("Month must be between one and twelve."));
			}
			return year;
		}

		internal int InternalGetDaysInMonth(int year, int month)
		{
			int num = 32768;
			num >>= month - 1;
			int result;
			if ((this.GetYearInfo(year, 3) & num) == 0)
			{
				result = 29;
			}
			else
			{
				result = 30;
			}
			return result;
		}

		/// <summary>Calculates the number of days in the specified month of the specified year and era.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="month">An integer from 1 through 12 in a common year, or 1 through 13 in a leap year, that represents the month.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>The number of days in the specified month of the specified year and era.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" />, <paramref name="month" />, or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override int GetDaysInMonth(int year, int month, int era)
		{
			year = this.CheckYearMonthRange(year, month, era);
			return this.InternalGetDaysInMonth(year, month);
		}

		private static int GregorianIsLeapYear(int y)
		{
			if (y % 4 != 0)
			{
				return 0;
			}
			if (y % 100 != 0)
			{
				return 1;
			}
			if (y % 400 == 0)
			{
				return 1;
			}
			return 0;
		}

		/// <summary>Returns a <see cref="T:System.DateTime" /> that is set to the specified date, time, and era.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="month">An integer from 1 through 13 that represents the month.</param>
		/// <param name="day">An integer from 1 through 31 that represents the day.</param>
		/// <param name="hour">An integer from 0 through 23 that represents the hour.</param>
		/// <param name="minute">An integer from 0 through 59 that represents the minute.</param>
		/// <param name="second">An integer from 0 through 59 that represents the second.</param>
		/// <param name="millisecond">An integer from 0 through 999 that represents the millisecond.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>A <see cref="T:System.DateTime" /> that is set to the specified date, time, and era.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, <paramref name="hour" />, <paramref name="minute" />, <paramref name="second" />, <paramref name="millisecond" />, or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
		{
			year = this.CheckYearMonthRange(year, month, era);
			int num = this.InternalGetDaysInMonth(year, month);
			if (day < 1 || day > num)
			{
				throw new ArgumentOutOfRangeException("day", Environment.GetResourceString("Day must be between 1 and {0} for month {1}.", new object[]
				{
					num,
					month
				}));
			}
			int year2 = 0;
			int month2 = 0;
			int day2 = 0;
			if (this.LunarToGregorian(year, month, day, ref year2, ref month2, ref day2))
			{
				return new DateTime(year2, month2, day2, hour, minute, second, millisecond);
			}
			throw new ArgumentOutOfRangeException(null, Environment.GetResourceString("Year, Month, and Day parameters describe an un-representable DateTime."));
		}

		internal void GregorianToLunar(int nSYear, int nSMonth, int nSDate, ref int nLYear, ref int nLMonth, ref int nLDate)
		{
			int i = ((EastAsianLunisolarCalendar.GregorianIsLeapYear(nSYear) == 1) ? EastAsianLunisolarCalendar.DaysToMonth366[nSMonth - 1] : EastAsianLunisolarCalendar.DaysToMonth365[nSMonth - 1]) + nSDate;
			nLYear = nSYear;
			int yearInfo;
			int yearInfo2;
			if (nLYear == this.MaxCalendarYear + 1)
			{
				nLYear--;
				i += ((EastAsianLunisolarCalendar.GregorianIsLeapYear(nLYear) == 1) ? 366 : 365);
				yearInfo = this.GetYearInfo(nLYear, 1);
				yearInfo2 = this.GetYearInfo(nLYear, 2);
			}
			else
			{
				yearInfo = this.GetYearInfo(nLYear, 1);
				yearInfo2 = this.GetYearInfo(nLYear, 2);
				if (nSMonth < yearInfo || (nSMonth == yearInfo && nSDate < yearInfo2))
				{
					nLYear--;
					i += ((EastAsianLunisolarCalendar.GregorianIsLeapYear(nLYear) == 1) ? 366 : 365);
					yearInfo = this.GetYearInfo(nLYear, 1);
					yearInfo2 = this.GetYearInfo(nLYear, 2);
				}
			}
			i -= EastAsianLunisolarCalendar.DaysToMonth365[yearInfo - 1];
			i -= yearInfo2 - 1;
			int num = 32768;
			int yearInfo3 = this.GetYearInfo(nLYear, 3);
			int num2 = ((yearInfo3 & num) != 0) ? 30 : 29;
			nLMonth = 1;
			while (i > num2)
			{
				i -= num2;
				nLMonth++;
				num >>= 1;
				num2 = (((yearInfo3 & num) != 0) ? 30 : 29);
			}
			nLDate = i;
		}

		internal bool LunarToGregorian(int nLYear, int nLMonth, int nLDate, ref int nSolarYear, ref int nSolarMonth, ref int nSolarDay)
		{
			if (nLDate < 1 || nLDate > 30)
			{
				return false;
			}
			int num = nLDate - 1;
			for (int i = 1; i < nLMonth; i++)
			{
				num += this.InternalGetDaysInMonth(nLYear, i);
			}
			int yearInfo = this.GetYearInfo(nLYear, 1);
			int yearInfo2 = this.GetYearInfo(nLYear, 2);
			int num2 = EastAsianLunisolarCalendar.GregorianIsLeapYear(nLYear);
			int[] array = (num2 == 1) ? EastAsianLunisolarCalendar.DaysToMonth366 : EastAsianLunisolarCalendar.DaysToMonth365;
			nSolarDay = yearInfo2;
			if (yearInfo > 1)
			{
				nSolarDay += array[yearInfo - 1];
			}
			nSolarDay += num;
			if (nSolarDay > num2 + 365)
			{
				nSolarYear = nLYear + 1;
				nSolarDay -= num2 + 365;
			}
			else
			{
				nSolarYear = nLYear;
			}
			nSolarMonth = 1;
			while (nSolarMonth < 12 && array[nSolarMonth] < nSolarDay)
			{
				nSolarMonth++;
			}
			nSolarDay -= array[nSolarMonth - 1];
			return true;
		}

		internal DateTime LunarToTime(DateTime time, int year, int month, int day)
		{
			int year2 = 0;
			int month2 = 0;
			int day2 = 0;
			this.LunarToGregorian(year, month, day, ref year2, ref month2, ref day2);
			return GregorianCalendar.GetDefaultInstance().ToDateTime(year2, month2, day2, time.Hour, time.Minute, time.Second, time.Millisecond);
		}

		internal void TimeToLunar(DateTime time, ref int year, ref int month, ref int day)
		{
			Calendar defaultInstance = GregorianCalendar.GetDefaultInstance();
			int year2 = defaultInstance.GetYear(time);
			int month2 = defaultInstance.GetMonth(time);
			int dayOfMonth = defaultInstance.GetDayOfMonth(time);
			this.GregorianToLunar(year2, month2, dayOfMonth, ref year, ref month, ref day);
		}

		/// <summary>Calculates the date that is the specified number of months away from the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to which to add <paramref name="months" />.</param>
		/// <param name="months">The number of months to add.</param>
		/// <returns>A new <see cref="T:System.DateTime" /> that results from adding the specified number of months to the <paramref name="time" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">The result is outside the supported range of a <see cref="T:System.DateTime" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="months" /> is less than -120000 or greater than 120000.  
		/// -or-  
		/// <paramref name="time" /> is less than <see cref="P:System.Globalization.Calendar.MinSupportedDateTime" /> or greater than <see cref="P:System.Globalization.Calendar.MaxSupportedDateTime" />.</exception>
		public override DateTime AddMonths(DateTime time, int months)
		{
			if (months < -120000 || months > 120000)
			{
				throw new ArgumentOutOfRangeException("months", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
				{
					-120000,
					120000
				}));
			}
			this.CheckTicksRange(time.Ticks);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.TimeToLunar(time, ref num, ref num2, ref num3);
			int i = num2 + months;
			if (i > 0)
			{
				int num4 = this.InternalIsLeapYear(num) ? 13 : 12;
				while (i - num4 > 0)
				{
					i -= num4;
					num++;
					num4 = (this.InternalIsLeapYear(num) ? 13 : 12);
				}
				num2 = i;
			}
			else
			{
				while (i <= 0)
				{
					int num5 = this.InternalIsLeapYear(num - 1) ? 13 : 12;
					i += num5;
					num--;
				}
				num2 = i;
			}
			int num6 = this.InternalGetDaysInMonth(num, num2);
			if (num3 > num6)
			{
				num3 = num6;
			}
			DateTime result = this.LunarToTime(time, num, num2, num3);
			Calendar.CheckAddResult(result.Ticks, this.MinSupportedDateTime, this.MaxSupportedDateTime);
			return result;
		}

		/// <summary>Calculates the date that is the specified number of years away from the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to which to add <paramref name="years" />.</param>
		/// <param name="years">The number of years to add.</param>
		/// <returns>A new <see cref="T:System.DateTime" /> that results from adding the specified number of years to the <paramref name="time" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentException">The result is outside the supported range of a <see cref="T:System.DateTime" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="time" /> is less than <see cref="P:System.Globalization.Calendar.MinSupportedDateTime" /> or greater than <see cref="P:System.Globalization.Calendar.MaxSupportedDateTime" />.</exception>
		public override DateTime AddYears(DateTime time, int years)
		{
			this.CheckTicksRange(time.Ticks);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			this.TimeToLunar(time, ref num, ref num2, ref num3);
			num += years;
			if (num2 == 13 && !this.InternalIsLeapYear(num))
			{
				num2 = 12;
				num3 = this.InternalGetDaysInMonth(num, num2);
			}
			int num4 = this.InternalGetDaysInMonth(num, num2);
			if (num3 > num4)
			{
				num3 = num4;
			}
			DateTime result = this.LunarToTime(time, num, num2, num3);
			Calendar.CheckAddResult(result.Ticks, this.MinSupportedDateTime, this.MaxSupportedDateTime);
			return result;
		}

		/// <summary>Calculates the day of the year in the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>An integer from 1 through 354 in a common year, or 1 through 384 in a leap year, that represents the day of the year specified in the <paramref name="time" /> parameter.</returns>
		public override int GetDayOfYear(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			int year = 0;
			int num = 0;
			int num2 = 0;
			this.TimeToLunar(time, ref year, ref num, ref num2);
			for (int i = 1; i < num; i++)
			{
				num2 += this.InternalGetDaysInMonth(year, i);
			}
			return num2;
		}

		/// <summary>Calculates the day of the month in the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>An integer from 1 through 31 that represents the day of the month specified in the <paramref name="time" /> parameter.</returns>
		public override int GetDayOfMonth(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			int num = 0;
			int num2 = 0;
			int result = 0;
			this.TimeToLunar(time, ref num, ref num2, ref result);
			return result;
		}

		/// <summary>Calculates the number of days in the specified year and era.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>The number of days in the specified year and era.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" /> or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override int GetDaysInYear(int year, int era)
		{
			year = this.CheckYearRange(year, era);
			int num = 0;
			int num2 = this.InternalIsLeapYear(year) ? 13 : 12;
			while (num2 != 0)
			{
				num += this.InternalGetDaysInMonth(year, num2--);
			}
			return num;
		}

		/// <summary>Returns the month in the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>An integer from 1 to 13 that represents the month specified in the <paramref name="time" /> parameter.</returns>
		public override int GetMonth(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			int num = 0;
			int result = 0;
			int num2 = 0;
			this.TimeToLunar(time, ref num, ref result, ref num2);
			return result;
		}

		/// <summary>Returns the year in the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>An integer that represents the year in the specified <see cref="T:System.DateTime" />.</returns>
		public override int GetYear(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			int year = 0;
			int num = 0;
			int num2 = 0;
			this.TimeToLunar(time, ref year, ref num, ref num2);
			return this.GetYear(year, time);
		}

		/// <summary>Calculates the day of the week in the specified date.</summary>
		/// <param name="time">The <see cref="T:System.DateTime" /> to read.</param>
		/// <returns>One of the <see cref="T:System.DayOfWeek" /> values that represents the day of the week specified in the <paramref name="time" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="time" /> is less than <see cref="P:System.Globalization.Calendar.MinSupportedDateTime" /> or greater than <see cref="P:System.Globalization.Calendar.MaxSupportedDateTime" />.</exception>
		public override DayOfWeek GetDayOfWeek(DateTime time)
		{
			this.CheckTicksRange(time.Ticks);
			return (DayOfWeek)(time.Ticks / 864000000000L + 1L) % (DayOfWeek)7;
		}

		/// <summary>Calculates the number of months in the specified year and era.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>The number of months in the specified year in the specified era. The return value is 12 months in a common year or 13 months in a leap year.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" /> or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override int GetMonthsInYear(int year, int era)
		{
			year = this.CheckYearRange(year, era);
			if (!this.InternalIsLeapYear(year))
			{
				return 12;
			}
			return 13;
		}

		/// <summary>Determines whether the specified date in the specified era is a leap day.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="month">An integer from 1 through 13 that represents the month.</param>
		/// <param name="day">An integer from 1 through 31 that represents the day.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>
		///   <see langword="true" /> if the specified day is a leap day; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" />, <paramref name="month" />, <paramref name="day" />, or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override bool IsLeapDay(int year, int month, int day, int era)
		{
			year = this.CheckYearMonthRange(year, month, era);
			int num = this.InternalGetDaysInMonth(year, month);
			if (day < 1 || day > num)
			{
				throw new ArgumentOutOfRangeException("day", Environment.GetResourceString("Day must be between 1 and {0} for month {1}.", new object[]
				{
					num,
					month
				}));
			}
			int yearInfo = this.GetYearInfo(year, 0);
			return yearInfo != 0 && month == yearInfo + 1;
		}

		/// <summary>Determines whether the specified month in the specified year and era is a leap month.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="month">An integer from 1 through 13 that represents the month.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="month" /> parameter is a leap month; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" />, <paramref name="month" />, or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override bool IsLeapMonth(int year, int month, int era)
		{
			year = this.CheckYearMonthRange(year, month, era);
			int yearInfo = this.GetYearInfo(year, 0);
			return yearInfo != 0 && month == yearInfo + 1;
		}

		/// <summary>Calculates the leap month for the specified year and era.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>A positive integer from 1 through 13 that indicates the leap month in the specified year and era.  
		///  -or-  
		///  Zero if this calendar does not support a leap month, or if the <paramref name="year" /> and <paramref name="era" /> parameters do not specify a leap year.</returns>
		public override int GetLeapMonth(int year, int era)
		{
			year = this.CheckYearRange(year, era);
			int yearInfo = this.GetYearInfo(year, 0);
			if (yearInfo > 0)
			{
				return yearInfo + 1;
			}
			return 0;
		}

		internal bool InternalIsLeapYear(int year)
		{
			return this.GetYearInfo(year, 0) != 0;
		}

		/// <summary>Determines whether the specified year in the specified era is a leap year.</summary>
		/// <param name="year">An integer that represents the year.</param>
		/// <param name="era">An integer that represents the era.</param>
		/// <returns>
		///   <see langword="true" /> if the specified year is a leap year; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" /> or <paramref name="era" /> is outside the range supported by this calendar.</exception>
		public override bool IsLeapYear(int year, int era)
		{
			year = this.CheckYearRange(year, era);
			return this.InternalIsLeapYear(year);
		}

		/// <summary>Gets or sets the last year of a 100-year range that can be represented by a 2-digit year.</summary>
		/// <returns>The last year of a 100-year range that can be represented by a 2-digit year.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Globalization.EastAsianLunisolarCalendar" /> is read-only.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The value in a set operation is less than 99 or greater than the maximum supported year in the current calendar.</exception>
		public override int TwoDigitYearMax
		{
			get
			{
				if (this.twoDigitYearMax == -1)
				{
					this.twoDigitYearMax = Calendar.GetSystemTwoDigitYearSetting(this.BaseCalendarID, this.GetYear(new DateTime(2029, 1, 1)));
				}
				return this.twoDigitYearMax;
			}
			set
			{
				base.VerifyWritable();
				if (value < 99 || value > this.MaxCalendarYear)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("Valid values are between {0} and {1}, inclusive.", new object[]
					{
						99,
						this.MaxCalendarYear
					}));
				}
				this.twoDigitYearMax = value;
			}
		}

		/// <summary>Converts the specified year to a four-digit year.</summary>
		/// <param name="year">A two-digit or four-digit integer that represents the year to convert.</param>
		/// <returns>An integer that contains the four-digit representation of the <paramref name="year" /> parameter.</returns>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="year" /> is outside the range supported by this calendar.</exception>
		public override int ToFourDigitYear(int year)
		{
			if (year < 0)
			{
				throw new ArgumentOutOfRangeException("year", Environment.GetResourceString("Non-negative number required."));
			}
			year = base.ToFourDigitYear(year);
			this.CheckYearRange(year, 0);
			return year;
		}

		internal const int LeapMonth = 0;

		internal const int Jan1Month = 1;

		internal const int Jan1Date = 2;

		internal const int nDaysPerMonth = 3;

		internal static readonly int[] DaysToMonth365 = new int[]
		{
			0,
			31,
			59,
			90,
			120,
			151,
			181,
			212,
			243,
			273,
			304,
			334
		};

		internal static readonly int[] DaysToMonth366 = new int[]
		{
			0,
			31,
			60,
			91,
			121,
			152,
			182,
			213,
			244,
			274,
			305,
			335
		};

		internal const int DatePartYear = 0;

		internal const int DatePartDayOfYear = 1;

		internal const int DatePartMonth = 2;

		internal const int DatePartDay = 3;

		internal const int MaxCalendarMonth = 13;

		internal const int MaxCalendarDay = 30;

		private const int DEFAULT_GREGORIAN_TWO_DIGIT_YEAR_MAX = 2029;
	}
}
