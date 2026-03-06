using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	internal static class TimeSpanFormatOptionsConverter
	{
		public static TimeSpanFormatOptions Merge(this TimeSpanFormatOptions left, TimeSpanFormatOptions right)
		{
			foreach (TimeSpanFormatOptions timeSpanFormatOptions in new TimeSpanFormatOptions[]
			{
				TimeSpanFormatOptions.Abbreviate | TimeSpanFormatOptions.AbbreviateOff,
				TimeSpanFormatOptions.LessThan | TimeSpanFormatOptions.LessThanOff,
				TimeSpanFormatOptions.RangeMilliSeconds | TimeSpanFormatOptions.RangeSeconds | TimeSpanFormatOptions.RangeMinutes | TimeSpanFormatOptions.RangeHours | TimeSpanFormatOptions.RangeDays | TimeSpanFormatOptions.RangeWeeks,
				TimeSpanFormatOptions.TruncateShortest | TimeSpanFormatOptions.TruncateAuto | TimeSpanFormatOptions.TruncateFill | TimeSpanFormatOptions.TruncateFull
			})
			{
				if ((left & timeSpanFormatOptions) == TimeSpanFormatOptions.InheritDefaults)
				{
					left |= (right & timeSpanFormatOptions);
				}
			}
			return left;
		}

		public static TimeSpanFormatOptions Mask(this TimeSpanFormatOptions timeSpanFormatOptions, TimeSpanFormatOptions mask)
		{
			return timeSpanFormatOptions & mask;
		}

		public static IEnumerable<TimeSpanFormatOptions> AllFlags(this TimeSpanFormatOptions timeSpanFormatOptions)
		{
			for (uint value = 1U; value <= (uint)timeSpanFormatOptions; value <<= 1)
			{
				if ((value & (uint)timeSpanFormatOptions) != 0U)
				{
					yield return (TimeSpanFormatOptions)value;
				}
			}
			yield break;
		}

		public static TimeSpanFormatOptions Parse(string formatOptionsString)
		{
			formatOptionsString = formatOptionsString.ToLower();
			TimeSpanFormatOptions timeSpanFormatOptions = TimeSpanFormatOptions.InheritDefaults;
			foreach (object obj in TimeSpanFormatOptionsConverter.parser.Matches(formatOptionsString))
			{
				string value = ((Match)obj).Value;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(value);
				if (num <= 3047058026U)
				{
					if (num <= 1275514075U)
					{
						if (num <= 560682936U)
						{
							if (num != 50083050U)
							{
								if (num != 50267956U)
								{
									if (num != 560682936U)
									{
										continue;
									}
									if (!(value == "less"))
									{
										continue;
									}
									timeSpanFormatOptions |= TimeSpanFormatOptions.LessThan;
									continue;
								}
								else
								{
									if (!(value == "hours"))
									{
										continue;
									}
									goto IL_3FA;
								}
							}
							else if (!(value == "millisecond"))
							{
								continue;
							}
						}
						else if (num != 954666857U)
						{
							if (num != 1158594403U)
							{
								if (num != 1275514075U)
								{
									continue;
								}
								if (!(value == "milliseconds"))
								{
									continue;
								}
							}
							else
							{
								if (!(value == "noless"))
								{
									continue;
								}
								timeSpanFormatOptions |= TimeSpanFormatOptions.LessThanOff;
								continue;
							}
						}
						else
						{
							if (!(value == "minute"))
							{
								continue;
							}
							goto IL_404;
						}
					}
					else if (num <= 2453644182U)
					{
						if (num != 1445858897U)
						{
							if (num != 1723256298U)
							{
								if (num != 2453644182U)
								{
									continue;
								}
								if (!(value == "auto"))
								{
									continue;
								}
								timeSpanFormatOptions |= TimeSpanFormatOptions.TruncateAuto;
								continue;
							}
							else
							{
								if (!(value == "seconds"))
								{
									continue;
								}
								goto IL_40E;
							}
						}
						else if (!(value == "ms"))
						{
							continue;
						}
					}
					else if (num <= 2914829806U)
					{
						if (num != 2885211357U)
						{
							if (num != 2914829806U)
							{
								continue;
							}
							if (!(value == "minutes"))
							{
								continue;
							}
							goto IL_404;
						}
						else
						{
							if (!(value == "second"))
							{
								continue;
							}
							goto IL_40E;
						}
					}
					else if (num != 2984927816U)
					{
						if (num != 3047058026U)
						{
							continue;
						}
						if (!(value == "weeks"))
						{
							continue;
						}
						goto IL_3E6;
					}
					else
					{
						if (!(value == "fill"))
						{
							continue;
						}
						timeSpanFormatOptions |= TimeSpanFormatOptions.TruncateFill;
						continue;
					}
					timeSpanFormatOptions |= TimeSpanFormatOptions.RangeMilliSeconds;
					continue;
				}
				if (num <= 3775669363U)
				{
					if (num <= 3586561629U)
					{
						if (num != 3053661199U)
						{
							if (num != 3122818005U)
							{
								if (num != 3586561629U)
								{
									continue;
								}
								if (!(value == "week"))
								{
									continue;
								}
								goto IL_3E6;
							}
							else
							{
								if (!(value == "short"))
								{
									continue;
								}
								timeSpanFormatOptions |= TimeSpanFormatOptions.TruncateShortest;
								continue;
							}
						}
						else
						{
							if (!(value == "hour"))
							{
								continue;
							}
							goto IL_3FA;
						}
					}
					else if (num != 3593516222U)
					{
						if (num != 3686686185U)
						{
							if (num != 3775669363U)
							{
								continue;
							}
							if (!(value == "d"))
							{
								continue;
							}
						}
						else
						{
							if (!(value == "noabbr"))
							{
								continue;
							}
							timeSpanFormatOptions |= TimeSpanFormatOptions.AbbreviateOff;
							continue;
						}
					}
					else
					{
						if (!(value == "abbr"))
						{
							continue;
						}
						timeSpanFormatOptions |= TimeSpanFormatOptions.Abbreviate;
						continue;
					}
				}
				else if (num <= 3977000791U)
				{
					if (num != 3830391293U)
					{
						if (num != 3893112696U)
						{
							if (num != 3977000791U)
							{
								continue;
							}
							if (!(value == "h"))
							{
								continue;
							}
							goto IL_3FA;
						}
						else
						{
							if (!(value == "m"))
							{
								continue;
							}
							goto IL_404;
						}
					}
					else if (!(value == "day"))
					{
						continue;
					}
				}
				else if (num <= 4127999362U)
				{
					if (num != 4060888886U)
					{
						if (num != 4127999362U)
						{
							continue;
						}
						if (!(value == "s"))
						{
							continue;
						}
						goto IL_40E;
					}
					else
					{
						if (!(value == "w"))
						{
							continue;
						}
						goto IL_3E6;
					}
				}
				else if (num != 4136751754U)
				{
					if (num != 4286165820U)
					{
						continue;
					}
					if (!(value == "full"))
					{
						continue;
					}
					timeSpanFormatOptions |= TimeSpanFormatOptions.TruncateFull;
					continue;
				}
				else if (!(value == "days"))
				{
					continue;
				}
				timeSpanFormatOptions |= TimeSpanFormatOptions.RangeDays;
				continue;
				IL_3E6:
				timeSpanFormatOptions |= TimeSpanFormatOptions.RangeWeeks;
				continue;
				IL_3FA:
				timeSpanFormatOptions |= TimeSpanFormatOptions.RangeHours;
				continue;
				IL_404:
				timeSpanFormatOptions |= TimeSpanFormatOptions.RangeMinutes;
				continue;
				IL_40E:
				timeSpanFormatOptions |= TimeSpanFormatOptions.RangeSeconds;
			}
			return timeSpanFormatOptions;
		}

		private static readonly Regex parser = new Regex("\\b(w|week|weeks|d|day|days|h|hour|hours|m|minute|minutes|s|second|seconds|ms|millisecond|milliseconds|auto|short|fill|full|abbr|noabbr|less|noless)\\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
