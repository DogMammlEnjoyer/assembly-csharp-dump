using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using Unity;

namespace System
{
	/// <summary>Represents any time zone in the world.</summary>
	[TypeForwardedFrom("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	[Serializable]
	public sealed class TimeZoneInfo : IEquatable<TimeZoneInfo>, ISerializable, IDeserializationCallback
	{
		/// <summary>Retrieves an array of <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> objects that apply to the current <see cref="T:System.TimeZoneInfo" /> object.</summary>
		/// <returns>An array of objects for this time zone.</returns>
		/// <exception cref="T:System.OutOfMemoryException">The system does not have enough memory to make an in-memory copy of the adjustment rules.</exception>
		public TimeZoneInfo.AdjustmentRule[] GetAdjustmentRules()
		{
			if (this._adjustmentRules == null)
			{
				return Array.Empty<TimeZoneInfo.AdjustmentRule>();
			}
			return (TimeZoneInfo.AdjustmentRule[])this._adjustmentRules.Clone();
		}

		private static void PopulateAllSystemTimeZones(TimeZoneInfo.CachedData cachedData)
		{
			if (TimeZoneInfo.HaveRegistry)
			{
				TimeZoneInfo.PopulateAllSystemTimeZonesFromRegistry(cachedData);
				return;
			}
			TimeZoneInfo.GetSystemTimeZonesWinRTFallback(cachedData);
		}

		private static void PopulateAllSystemTimeZonesFromRegistry(TimeZoneInfo.CachedData cachedData)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", false))
			{
				if (registryKey != null)
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					for (int i = 0; i < subKeyNames.Length; i++)
					{
						TimeZoneInfo timeZoneInfo;
						Exception ex;
						TimeZoneInfo.TryGetTimeZone(subKeyNames[i], false, out timeZoneInfo, out ex, cachedData, false);
					}
				}
			}
		}

		private TimeZoneInfo(in Interop.Kernel32.TIME_ZONE_INFORMATION zone, bool dstDisabled)
		{
			Interop.Kernel32.TIME_ZONE_INFORMATION time_ZONE_INFORMATION = zone;
			string standardName = time_ZONE_INFORMATION.GetStandardName();
			if (standardName.Length == 0)
			{
				this._id = "Local";
			}
			else
			{
				this._id = standardName;
			}
			this._baseUtcOffset = new TimeSpan(0, -zone.Bias, 0);
			if (!dstDisabled)
			{
				Interop.Kernel32.REG_TZI_FORMAT reg_TZI_FORMAT = new Interop.Kernel32.REG_TZI_FORMAT(ref zone);
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(reg_TZI_FORMAT, DateTime.MinValue.Date, DateTime.MaxValue.Date, zone.Bias);
				if (adjustmentRule != null)
				{
					this._adjustmentRules = new TimeZoneInfo.AdjustmentRule[]
					{
						adjustmentRule
					};
				}
			}
			TimeZoneInfo.ValidateTimeZoneInfo(this._id, this._baseUtcOffset, this._adjustmentRules, out this._supportsDaylightSavingTime);
			this._displayName = standardName;
			this._standardDisplayName = standardName;
			time_ZONE_INFORMATION = zone;
			this._daylightDisplayName = time_ZONE_INFORMATION.GetDaylightName();
		}

		private static bool CheckDaylightSavingTimeNotSupported(in Interop.Kernel32.TIME_ZONE_INFORMATION timeZone)
		{
			Interop.Kernel32.SYSTEMTIME daylightDate = timeZone.DaylightDate;
			return daylightDate.Equals(timeZone.StandardDate);
		}

		private static TimeZoneInfo.AdjustmentRule CreateAdjustmentRuleFromTimeZoneInformation(in Interop.Kernel32.REG_TZI_FORMAT timeZoneInformation, DateTime startDate, DateTime endDate, int defaultBaseUtcOffset)
		{
			if (timeZoneInformation.StandardDate.Month <= 0)
			{
				if (timeZoneInformation.Bias == defaultBaseUtcOffset)
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, TimeSpan.Zero, TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 1, 1), TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue.AddMilliseconds(1.0), 1, 1), new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0), false);
			}
			else
			{
				TimeZoneInfo.TransitionTime daylightTransitionStart;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out daylightTransitionStart, true))
				{
					return null;
				}
				TimeZoneInfo.TransitionTime transitionTime;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out transitionTime, false))
				{
					return null;
				}
				if (daylightTransitionStart.Equals(transitionTime))
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, new TimeSpan(0, -timeZoneInformation.DaylightBias, 0), daylightTransitionStart, transitionTime, new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.Bias, 0), false);
			}
		}

		private static string FindIdFromTimeZoneInformation(in Interop.Kernel32.TIME_ZONE_INFORMATION timeZone, out bool dstDisabled)
		{
			dstDisabled = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones", false))
			{
				if (registryKey == null)
				{
					return null;
				}
				foreach (string text in registryKey.GetSubKeyNames())
				{
					if (TimeZoneInfo.TryCompareTimeZoneInformationToRegistry(timeZone, text, out dstDisabled))
					{
						return text;
					}
				}
			}
			return null;
		}

		private static TimeZoneInfo GetLocalTimeZone(TimeZoneInfo.CachedData cachedData)
		{
			if (!TimeZoneInfo.HaveRegistry)
			{
				return TimeZoneInfo.GetLocalTimeZoneInfoWinRTFallback();
			}
			Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION time_DYNAMIC_ZONE_INFORMATION = default(Interop.Kernel32.TIME_DYNAMIC_ZONE_INFORMATION);
			if (Interop.Kernel32.GetDynamicTimeZoneInformation(out time_DYNAMIC_ZONE_INFORMATION) == 4294967295U)
			{
				return TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
			}
			string timeZoneKeyName = time_DYNAMIC_ZONE_INFORMATION.GetTimeZoneKeyName();
			TimeZoneInfo result;
			Exception ex;
			if (timeZoneKeyName.Length != 0 && TimeZoneInfo.TryGetTimeZone(timeZoneKeyName, time_DYNAMIC_ZONE_INFORMATION.DynamicDaylightTimeDisabled > 0, out result, out ex, cachedData, false) == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result;
			}
			Interop.Kernel32.TIME_ZONE_INFORMATION time_ZONE_INFORMATION = new Interop.Kernel32.TIME_ZONE_INFORMATION(ref time_DYNAMIC_ZONE_INFORMATION);
			bool dstDisabled;
			string text = TimeZoneInfo.FindIdFromTimeZoneInformation(time_ZONE_INFORMATION, out dstDisabled);
			TimeZoneInfo result2;
			Exception ex2;
			if (text != null && TimeZoneInfo.TryGetTimeZone(text, dstDisabled, out result2, out ex2, cachedData, false) == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result2;
			}
			return TimeZoneInfo.GetLocalTimeZoneFromWin32Data(time_ZONE_INFORMATION, dstDisabled);
		}

		private static TimeZoneInfo GetLocalTimeZoneFromWin32Data(in Interop.Kernel32.TIME_ZONE_INFORMATION timeZoneInformation, bool dstDisabled)
		{
			try
			{
				return new TimeZoneInfo(ref timeZoneInformation, dstDisabled);
			}
			catch (ArgumentException)
			{
			}
			catch (InvalidTimeZoneException)
			{
			}
			if (!dstDisabled)
			{
				try
				{
					return new TimeZoneInfo(ref timeZoneInformation, true);
				}
				catch (ArgumentException)
				{
				}
				catch (InvalidTimeZoneException)
				{
				}
			}
			return TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
		}

		/// <summary>Instantiates a new <see cref="T:System.TimeZoneInfo" /> object based on its identifier.</summary>
		/// <param name="id">The time zone identifier, which corresponds to the <see cref="P:System.TimeZoneInfo.Id" /> property.</param>
		/// <returns>An object whose identifier is the value of the <paramref name="id" /> parameter.</returns>
		/// <exception cref="T:System.OutOfMemoryException">The system does not have enough memory to hold information about the time zone.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="id" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.TimeZoneNotFoundException">The time zone identifier specified by <paramref name="id" /> was not found. This means that a time zone identifier whose name matches <paramref name="id" /> does not exist, or that the identifier exists but does not contain any time zone data.</exception>
		/// <exception cref="T:System.Security.SecurityException">The process does not have the permissions required to read from the registry key that contains the time zone information.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The time zone identifier was found, but the registry data is corrupted.</exception>
		public static TimeZoneInfo FindSystemTimeZoneById(string id)
		{
			if (string.Equals(id, "UTC", StringComparison.OrdinalIgnoreCase))
			{
				return TimeZoneInfo.Utc;
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0 || id.Length > 255 || id.Contains('\0'))
			{
				throw new TimeZoneNotFoundException(SR.Format("The time zone ID '{0}' was not found on the local computer.", id));
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			TimeZoneInfo.CachedData obj = cachedData;
			TimeZoneInfo result;
			Exception ex;
			TimeZoneInfo.TimeZoneInfoResult timeZoneInfoResult;
			lock (obj)
			{
				timeZoneInfoResult = TimeZoneInfo.TryGetTimeZone(id, false, out result, out ex, cachedData, false);
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				return result;
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException)
			{
				throw new InvalidTimeZoneException(SR.Format("The time zone ID '{0}' was found on the local computer, but the registry information was corrupt.", id), ex);
			}
			if (timeZoneInfoResult == TimeZoneInfo.TimeZoneInfoResult.SecurityException)
			{
				throw new SecurityException(SR.Format("The time zone ID '{0}' was found on the local computer, but the application does not have permission to read the registry information.", id), ex);
			}
			throw new TimeZoneNotFoundException(SR.Format("The time zone ID '{0}' was not found on the local computer.", id), ex);
		}

		internal static TimeSpan GetDateTimeNowUtcOffsetFromUtc(DateTime time, out bool isAmbiguousLocalDst)
		{
			isAmbiguousLocalDst = false;
			int year = time.Year;
			TimeZoneInfo.OffsetAndRule oneYearLocalFromUtc = TimeZoneInfo.s_cachedData.GetOneYearLocalFromUtc(year);
			TimeSpan timeSpan = oneYearLocalFromUtc.Offset;
			if (oneYearLocalFromUtc.Rule != null)
			{
				timeSpan += oneYearLocalFromUtc.Rule.BaseUtcOffsetDelta;
				if (oneYearLocalFromUtc.Rule.HasDaylightSaving)
				{
					bool isDaylightSavingsFromUtc = TimeZoneInfo.GetIsDaylightSavingsFromUtc(time, year, oneYearLocalFromUtc.Offset, oneYearLocalFromUtc.Rule, null, out isAmbiguousLocalDst, TimeZoneInfo.Local);
					timeSpan += (isDaylightSavingsFromUtc ? oneYearLocalFromUtc.Rule.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		private static bool TransitionTimeFromTimeZoneInformation(in Interop.Kernel32.REG_TZI_FORMAT timeZoneInformation, out TimeZoneInfo.TransitionTime transitionTime, bool readStartDate)
		{
			if (timeZoneInformation.StandardDate.Month <= 0)
			{
				transitionTime = default(TimeZoneInfo.TransitionTime);
				return false;
			}
			if (readStartDate)
			{
				if (timeZoneInformation.DaylightDate.Year == 0)
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.DaylightDate.Hour, (int)timeZoneInformation.DaylightDate.Minute, (int)timeZoneInformation.DaylightDate.Second, (int)timeZoneInformation.DaylightDate.Milliseconds), (int)timeZoneInformation.DaylightDate.Month, (int)timeZoneInformation.DaylightDate.Day, (DayOfWeek)timeZoneInformation.DaylightDate.DayOfWeek);
				}
				else
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.DaylightDate.Hour, (int)timeZoneInformation.DaylightDate.Minute, (int)timeZoneInformation.DaylightDate.Second, (int)timeZoneInformation.DaylightDate.Milliseconds), (int)timeZoneInformation.DaylightDate.Month, (int)timeZoneInformation.DaylightDate.Day);
				}
			}
			else if (timeZoneInformation.StandardDate.Year == 0)
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.StandardDate.Hour, (int)timeZoneInformation.StandardDate.Minute, (int)timeZoneInformation.StandardDate.Second, (int)timeZoneInformation.StandardDate.Milliseconds), (int)timeZoneInformation.StandardDate.Month, (int)timeZoneInformation.StandardDate.Day, (DayOfWeek)timeZoneInformation.StandardDate.DayOfWeek);
			}
			else
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.StandardDate.Hour, (int)timeZoneInformation.StandardDate.Minute, (int)timeZoneInformation.StandardDate.Second, (int)timeZoneInformation.StandardDate.Milliseconds), (int)timeZoneInformation.StandardDate.Month, (int)timeZoneInformation.StandardDate.Day);
			}
			return true;
		}

		private static bool TryCreateAdjustmentRules(string id, in Interop.Kernel32.REG_TZI_FORMAT defaultTimeZoneInformation, out TimeZoneInfo.AdjustmentRule[] rules, out Exception e, int defaultBaseUtcOffset)
		{
			rules = null;
			e = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + id + "\\Dynamic DST", false))
				{
					if (registryKey == null)
					{
						TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(defaultTimeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
						if (adjustmentRule != null)
						{
							rules = new TimeZoneInfo.AdjustmentRule[]
							{
								adjustmentRule
							};
						}
						return true;
					}
					int num = (int)registryKey.GetValue("FirstEntry", -1, RegistryValueOptions.None);
					int num2 = (int)registryKey.GetValue("LastEntry", -1, RegistryValueOptions.None);
					if (num == -1 || num2 == -1 || num > num2)
					{
						return false;
					}
					Interop.Kernel32.REG_TZI_FORMAT reg_TZI_FORMAT;
					if (!TimeZoneInfo.TryGetTimeZoneEntryFromRegistry(registryKey, num.ToString(CultureInfo.InvariantCulture), out reg_TZI_FORMAT))
					{
						return false;
					}
					if (num == num2)
					{
						TimeZoneInfo.AdjustmentRule adjustmentRule2 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(reg_TZI_FORMAT, DateTime.MinValue.Date, DateTime.MaxValue.Date, defaultBaseUtcOffset);
						if (adjustmentRule2 != null)
						{
							rules = new TimeZoneInfo.AdjustmentRule[]
							{
								adjustmentRule2
							};
						}
						return true;
					}
					List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>(1);
					TimeZoneInfo.AdjustmentRule adjustmentRule3 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(reg_TZI_FORMAT, DateTime.MinValue.Date, new DateTime(num, 12, 31), defaultBaseUtcOffset);
					if (adjustmentRule3 != null)
					{
						list.Add(adjustmentRule3);
					}
					for (int i = num + 1; i < num2; i++)
					{
						if (!TimeZoneInfo.TryGetTimeZoneEntryFromRegistry(registryKey, i.ToString(CultureInfo.InvariantCulture), out reg_TZI_FORMAT))
						{
							return false;
						}
						TimeZoneInfo.AdjustmentRule adjustmentRule4 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(reg_TZI_FORMAT, new DateTime(i, 1, 1), new DateTime(i, 12, 31), defaultBaseUtcOffset);
						if (adjustmentRule4 != null)
						{
							list.Add(adjustmentRule4);
						}
					}
					if (!TimeZoneInfo.TryGetTimeZoneEntryFromRegistry(registryKey, num2.ToString(CultureInfo.InvariantCulture), out reg_TZI_FORMAT))
					{
						return false;
					}
					TimeZoneInfo.AdjustmentRule adjustmentRule5 = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(reg_TZI_FORMAT, new DateTime(num2, 1, 1), DateTime.MaxValue.Date, defaultBaseUtcOffset);
					if (adjustmentRule5 != null)
					{
						list.Add(adjustmentRule5);
					}
					if (list.Count != 0)
					{
						rules = list.ToArray();
					}
				}
			}
			catch (InvalidCastException ex)
			{
				e = ex;
				return false;
			}
			catch (ArgumentOutOfRangeException ex2)
			{
				e = ex2;
				return false;
			}
			catch (ArgumentException ex3)
			{
				e = ex3;
				return false;
			}
			return true;
		}

		private unsafe static bool TryGetTimeZoneEntryFromRegistry(RegistryKey key, string name, out Interop.Kernel32.REG_TZI_FORMAT dtzi)
		{
			byte[] array = key.GetValue(name, null, RegistryValueOptions.None) as byte[];
			if (array == null || array.Length != sizeof(Interop.Kernel32.REG_TZI_FORMAT))
			{
				dtzi = default(Interop.Kernel32.REG_TZI_FORMAT);
				return false;
			}
			fixed (byte* ptr = &array[0])
			{
				byte* ptr2 = ptr;
				dtzi = *(Interop.Kernel32.REG_TZI_FORMAT*)ptr2;
			}
			return true;
		}

		private static bool TryCompareStandardDate(in Interop.Kernel32.TIME_ZONE_INFORMATION timeZone, in Interop.Kernel32.REG_TZI_FORMAT registryTimeZoneInfo)
		{
			if (timeZone.Bias == registryTimeZoneInfo.Bias && timeZone.StandardBias == registryTimeZoneInfo.StandardBias)
			{
				Interop.Kernel32.SYSTEMTIME standardDate = timeZone.StandardDate;
				return standardDate.Equals(registryTimeZoneInfo.StandardDate);
			}
			return false;
		}

		private static bool TryCompareTimeZoneInformationToRegistry(in Interop.Kernel32.TIME_ZONE_INFORMATION timeZone, string id, out bool dstDisabled)
		{
			dstDisabled = false;
			bool result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + id, false))
			{
				Interop.Kernel32.REG_TZI_FORMAT reg_TZI_FORMAT;
				if (registryKey == null)
				{
					result = false;
				}
				else if (!TimeZoneInfo.TryGetTimeZoneEntryFromRegistry(registryKey, "TZI", out reg_TZI_FORMAT))
				{
					result = false;
				}
				else if (!TimeZoneInfo.TryCompareStandardDate(timeZone, reg_TZI_FORMAT))
				{
					result = false;
				}
				else
				{
					bool flag;
					if (!dstDisabled && !TimeZoneInfo.CheckDaylightSavingTimeNotSupported(timeZone))
					{
						if (timeZone.DaylightBias == reg_TZI_FORMAT.DaylightBias)
						{
							Interop.Kernel32.SYSTEMTIME daylightDate = timeZone.DaylightDate;
							flag = daylightDate.Equals(reg_TZI_FORMAT.DaylightDate);
						}
						else
						{
							flag = false;
						}
					}
					else
					{
						flag = true;
					}
					bool flag2 = flag;
					if (flag2)
					{
						string a = registryKey.GetValue("Std", string.Empty, RegistryValueOptions.None) as string;
						Interop.Kernel32.TIME_ZONE_INFORMATION time_ZONE_INFORMATION = timeZone;
						flag2 = string.Equals(a, time_ZONE_INFORMATION.GetStandardName(), StringComparison.Ordinal);
					}
					result = flag2;
				}
			}
			return result;
		}

		private static string TryGetLocalizedNameByMuiNativeResource(string resource)
		{
			if (string.IsNullOrEmpty(resource))
			{
				return string.Empty;
			}
			string[] array = resource.Split(',', StringSplitOptions.None);
			if (array.Length != 2)
			{
				return string.Empty;
			}
			string systemDirectory = Environment.SystemDirectory;
			string path = array[0].TrimStart('@');
			string filePath;
			try
			{
				filePath = Path.Combine(systemDirectory, path);
			}
			catch (ArgumentException)
			{
				return string.Empty;
			}
			int num;
			if (!int.TryParse(array[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				return string.Empty;
			}
			num = -num;
			string result;
			try
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(260);
				stringBuilder.Length = 260;
				int num2 = 260;
				int num3 = 0;
				long num4 = 0L;
				if (!Interop.Kernel32.GetFileMUIPath(16U, filePath, null, ref num3, stringBuilder, ref num2, ref num4))
				{
					StringBuilderCache.Release(stringBuilder);
					result = string.Empty;
				}
				else
				{
					result = TimeZoneInfo.TryGetLocalizedNameByNativeResource(StringBuilderCache.GetStringAndRelease(stringBuilder), num);
				}
			}
			catch (EntryPointNotFoundException)
			{
				result = string.Empty;
			}
			return result;
		}

		private static string TryGetLocalizedNameByNativeResource(string filePath, int resource)
		{
			using (SafeLibraryHandle safeLibraryHandle = Interop.Kernel32.LoadLibraryEx(filePath, IntPtr.Zero, 2))
			{
				if (!safeLibraryHandle.IsInvalid)
				{
					StringBuilder stringBuilder = StringBuilderCache.Acquire(500);
					if (Interop.User32.LoadString(safeLibraryHandle, resource, stringBuilder, 500) != 0)
					{
						return StringBuilderCache.GetStringAndRelease(stringBuilder);
					}
				}
			}
			return string.Empty;
		}

		private static void GetLocalizedNamesByRegistryKey(RegistryKey key, out string displayName, out string standardName, out string daylightName)
		{
			displayName = string.Empty;
			standardName = string.Empty;
			daylightName = string.Empty;
			string text = key.GetValue("MUI_Display", string.Empty, RegistryValueOptions.None) as string;
			string text2 = key.GetValue("MUI_Std", string.Empty, RegistryValueOptions.None) as string;
			string text3 = key.GetValue("MUI_Dlt", string.Empty, RegistryValueOptions.None) as string;
			if (!string.IsNullOrEmpty(text))
			{
				displayName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				standardName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text2);
			}
			if (!string.IsNullOrEmpty(text3))
			{
				daylightName = TimeZoneInfo.TryGetLocalizedNameByMuiNativeResource(text3);
			}
			if (string.IsNullOrEmpty(displayName))
			{
				displayName = (key.GetValue("Display", string.Empty, RegistryValueOptions.None) as string);
			}
			if (string.IsNullOrEmpty(standardName))
			{
				standardName = (key.GetValue("Std", string.Empty, RegistryValueOptions.None) as string);
			}
			if (string.IsNullOrEmpty(daylightName))
			{
				daylightName = (key.GetValue("Dlt", string.Empty, RegistryValueOptions.None) as string);
			}
		}

		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZoneFromLocalMachine(string id, out TimeZoneInfo value, out Exception e)
		{
			if (TimeZoneInfo.HaveRegistry)
			{
				return TimeZoneInfo.TryGetTimeZoneFromLocalRegistry(id, out value, out e);
			}
			e = null;
			value = TimeZoneInfo.FindSystemTimeZoneByIdWinRTFallback(id);
			return TimeZoneInfo.TimeZoneInfoResult.Success;
		}

		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZoneFromLocalRegistry(string id, out TimeZoneInfo value, out Exception e)
		{
			e = null;
			TimeZoneInfo.TimeZoneInfoResult result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones\\" + id, false))
			{
				Interop.Kernel32.REG_TZI_FORMAT reg_TZI_FORMAT;
				TimeZoneInfo.AdjustmentRule[] adjustmentRules;
				if (registryKey == null)
				{
					value = null;
					result = TimeZoneInfo.TimeZoneInfoResult.TimeZoneNotFoundException;
				}
				else if (!TimeZoneInfo.TryGetTimeZoneEntryFromRegistry(registryKey, "TZI", out reg_TZI_FORMAT))
				{
					value = null;
					result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
				}
				else if (!TimeZoneInfo.TryCreateAdjustmentRules(id, reg_TZI_FORMAT, out adjustmentRules, out e, reg_TZI_FORMAT.Bias))
				{
					value = null;
					result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
				}
				else
				{
					string displayName;
					string standardDisplayName;
					string daylightDisplayName;
					TimeZoneInfo.GetLocalizedNamesByRegistryKey(registryKey, out displayName, out standardDisplayName, out daylightDisplayName);
					try
					{
						value = new TimeZoneInfo(id, new TimeSpan(0, -reg_TZI_FORMAT.Bias, 0), displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
						result = TimeZoneInfo.TimeZoneInfoResult.Success;
					}
					catch (ArgumentException ex)
					{
						value = null;
						e = ex;
						result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
					}
					catch (InvalidTimeZoneException ex2)
					{
						value = null;
						e = ex2;
						result = TimeZoneInfo.TimeZoneInfoResult.InvalidTimeZoneException;
					}
				}
			}
			return result;
		}

		private static bool HaveRegistry
		{
			get
			{
				return TimeZoneInfo.lazyHaveRegistry.Value;
			}
		}

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint EnumDynamicTimeZoneInformation(uint dwIndex, out TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION lpTimeZoneInformation);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint GetDynamicTimeZoneInformation(out TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION pTimeZoneInformation);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern uint GetDynamicTimeZoneInformationEffectiveYears(ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION lpTimeZoneInformation, out uint FirstYear, out uint LastYear);

		[DllImport("api-ms-win-core-timezone-l1-1-0.dll")]
		internal static extern bool GetTimeZoneInformationForYear(ushort wYear, ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION pdtzi, out Interop.Kernel32.TIME_ZONE_INFORMATION ptzi);

		internal static TimeZoneInfo.AdjustmentRule CreateAdjustmentRuleFromTimeZoneInformation(ref TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation, DateTime startDate, DateTime endDate, int defaultBaseUtcOffset)
		{
			if (timeZoneInformation.TZI.StandardDate.Month <= 0)
			{
				if (timeZoneInformation.TZI.Bias == defaultBaseUtcOffset)
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, TimeSpan.Zero, TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue, 1, 1), TimeZoneInfo.TransitionTime.CreateFixedDateRule(DateTime.MinValue.AddMilliseconds(1.0), 1, 1), new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.TZI.Bias, 0), false);
			}
			else
			{
				TimeZoneInfo.TransitionTime daylightTransitionStart;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out daylightTransitionStart, true))
				{
					return null;
				}
				TimeZoneInfo.TransitionTime transitionTime;
				if (!TimeZoneInfo.TransitionTimeFromTimeZoneInformation(timeZoneInformation, out transitionTime, false))
				{
					return null;
				}
				if (daylightTransitionStart.Equals(transitionTime))
				{
					return null;
				}
				return TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(startDate, endDate, new TimeSpan(0, -timeZoneInformation.TZI.DaylightBias, 0), daylightTransitionStart, transitionTime, new TimeSpan(0, defaultBaseUtcOffset - timeZoneInformation.TZI.Bias, 0), false);
			}
		}

		private static bool TransitionTimeFromTimeZoneInformation(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation, out TimeZoneInfo.TransitionTime transitionTime, bool readStartDate)
		{
			if (timeZoneInformation.TZI.StandardDate.Month <= 0)
			{
				transitionTime = default(TimeZoneInfo.TransitionTime);
				return false;
			}
			if (readStartDate)
			{
				if (timeZoneInformation.TZI.DaylightDate.Year == 0)
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.DaylightDate.Hour, (int)timeZoneInformation.TZI.DaylightDate.Minute, (int)timeZoneInformation.TZI.DaylightDate.Second, (int)timeZoneInformation.TZI.DaylightDate.Milliseconds), (int)timeZoneInformation.TZI.DaylightDate.Month, (int)timeZoneInformation.TZI.DaylightDate.Day, (DayOfWeek)timeZoneInformation.TZI.DaylightDate.DayOfWeek);
				}
				else
				{
					transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.DaylightDate.Hour, (int)timeZoneInformation.TZI.DaylightDate.Minute, (int)timeZoneInformation.TZI.DaylightDate.Second, (int)timeZoneInformation.TZI.DaylightDate.Milliseconds), (int)timeZoneInformation.TZI.DaylightDate.Month, (int)timeZoneInformation.TZI.DaylightDate.Day);
				}
			}
			else if (timeZoneInformation.TZI.StandardDate.Year == 0)
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.StandardDate.Hour, (int)timeZoneInformation.TZI.StandardDate.Minute, (int)timeZoneInformation.TZI.StandardDate.Second, (int)timeZoneInformation.TZI.StandardDate.Milliseconds), (int)timeZoneInformation.TZI.StandardDate.Month, (int)timeZoneInformation.TZI.StandardDate.Day, (DayOfWeek)timeZoneInformation.TZI.StandardDate.DayOfWeek);
			}
			else
			{
				transitionTime = TimeZoneInfo.TransitionTime.CreateFixedDateRule(new DateTime(1, 1, 1, (int)timeZoneInformation.TZI.StandardDate.Hour, (int)timeZoneInformation.TZI.StandardDate.Minute, (int)timeZoneInformation.TZI.StandardDate.Second, (int)timeZoneInformation.TZI.StandardDate.Milliseconds), (int)timeZoneInformation.TZI.StandardDate.Month, (int)timeZoneInformation.TZI.StandardDate.Day);
			}
			return true;
		}

		internal static TimeZoneInfo TryCreateTimeZone(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation)
		{
			uint num = 0U;
			uint num2 = 0U;
			TimeZoneInfo.AdjustmentRule[] adjustmentRules = null;
			int bias = timeZoneInformation.TZI.Bias;
			if (string.IsNullOrEmpty(timeZoneInformation.TimeZoneKeyName))
			{
				return null;
			}
			try
			{
				if (TimeZoneInfo.GetDynamicTimeZoneInformationEffectiveYears(ref timeZoneInformation, out num, out num2) != 0U)
				{
					num2 = (num = 0U);
				}
			}
			catch
			{
				num2 = (num = 0U);
			}
			if (num == num2)
			{
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref timeZoneInformation, DateTime.MinValue.Date, DateTime.MaxValue.Date, bias);
				if (adjustmentRule != null)
				{
					adjustmentRules = new TimeZoneInfo.AdjustmentRule[]
					{
						adjustmentRule
					};
				}
			}
			else
			{
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION dynamic_TIME_ZONE_INFORMATION = default(TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION);
				List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>();
				if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
				{
					return null;
				}
				TimeZoneInfo.AdjustmentRule adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, DateTime.MinValue.Date, new DateTime((int)num, 12, 31), bias);
				if (adjustmentRule != null)
				{
					list.Add(adjustmentRule);
				}
				for (uint num3 = num + 1U; num3 < num2; num3 += 1U)
				{
					if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num3, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
					{
						return null;
					}
					adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, new DateTime((int)num3, 1, 1), new DateTime((int)num3, 12, 31), bias);
					if (adjustmentRule != null)
					{
						list.Add(adjustmentRule);
					}
				}
				if (!TimeZoneInfo.GetTimeZoneInformationForYear((ushort)num2, ref timeZoneInformation, out dynamic_TIME_ZONE_INFORMATION.TZI))
				{
					return null;
				}
				adjustmentRule = TimeZoneInfo.CreateAdjustmentRuleFromTimeZoneInformation(ref dynamic_TIME_ZONE_INFORMATION, new DateTime((int)num2, 1, 1), DateTime.MaxValue.Date, bias);
				if (adjustmentRule != null)
				{
					list.Add(adjustmentRule);
				}
				if (list.Count > 0)
				{
					adjustmentRules = list.ToArray();
				}
			}
			return new TimeZoneInfo(timeZoneInformation.TimeZoneKeyName, new TimeSpan(0, -timeZoneInformation.TZI.Bias, 0), timeZoneInformation.TZI.GetStandardName(), timeZoneInformation.TZI.GetStandardName(), timeZoneInformation.TZI.GetDaylightName(), adjustmentRules, false);
		}

		internal static TimeZoneInfo GetLocalTimeZoneInfoWinRTFallback()
		{
			TimeZoneInfo result;
			try
			{
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation;
				if (TimeZoneInfo.GetDynamicTimeZoneInformation(out timeZoneInformation) == 4294967295U)
				{
					result = TimeZoneInfo.Utc;
				}
				else
				{
					TimeZoneInfo timeZoneInfo = TimeZoneInfo.TryCreateTimeZone(timeZoneInformation);
					result = ((timeZoneInfo != null) ? timeZoneInfo : TimeZoneInfo.Utc);
				}
			}
			catch
			{
				result = TimeZoneInfo.Utc;
			}
			return result;
		}

		internal static TimeZoneInfo FindSystemTimeZoneByIdWinRTFallback(string id)
		{
			foreach (TimeZoneInfo timeZoneInfo in TimeZoneInfo.GetSystemTimeZones())
			{
				if (string.Compare(id, timeZoneInfo.Id, StringComparison.Ordinal) == 0)
				{
					return timeZoneInfo;
				}
			}
			throw new TimeZoneNotFoundException();
		}

		private static void GetSystemTimeZonesWinRTFallback(TimeZoneInfo.CachedData cachedData)
		{
			List<TimeZoneInfo> list = new List<TimeZoneInfo>();
			try
			{
				uint num = 0U;
				TimeZoneInfo.DYNAMIC_TIME_ZONE_INFORMATION timeZoneInformation;
				while (TimeZoneInfo.EnumDynamicTimeZoneInformation(num++, out timeZoneInformation) != 259U)
				{
					TimeZoneInfo timeZoneInfo = TimeZoneInfo.TryCreateTimeZone(timeZoneInformation);
					if (timeZoneInfo != null)
					{
						list.Add(timeZoneInfo);
					}
				}
			}
			catch
			{
			}
			if (list.Count == 0)
			{
				list.Add(TimeZoneInfo.Local);
				list.Add(TimeZoneInfo.Utc);
			}
			list.Sort(delegate(TimeZoneInfo x, TimeZoneInfo y)
			{
				int num2 = x.BaseUtcOffset.CompareTo(y.BaseUtcOffset);
				if (num2 != 0)
				{
					return num2;
				}
				return string.CompareOrdinal(x.DisplayName, y.DisplayName);
			});
			foreach (TimeZoneInfo timeZoneInfo2 in list)
			{
				if (cachedData._systemTimeZones == null)
				{
					cachedData._systemTimeZones = new Dictionary<string, TimeZoneInfo>(StringComparer.OrdinalIgnoreCase);
				}
				cachedData._systemTimeZones.Add(timeZoneInfo2.Id, timeZoneInfo2);
			}
		}

		/// <summary>Gets the time zone identifier.</summary>
		/// <returns>The time zone identifier.</returns>
		public string Id
		{
			get
			{
				return this._id;
			}
		}

		/// <summary>Gets the general display name that represents the time zone.</summary>
		/// <returns>The time zone's general display name.</returns>
		public string DisplayName
		{
			get
			{
				return this._displayName ?? string.Empty;
			}
		}

		/// <summary>Gets the display name for the time zone's standard time.</summary>
		/// <returns>The display name of the time zone's standard time.</returns>
		public string StandardName
		{
			get
			{
				return this._standardDisplayName ?? string.Empty;
			}
		}

		/// <summary>Gets the display name for the current time zone's daylight saving time.</summary>
		/// <returns>The display name for the time zone's daylight saving time.</returns>
		public string DaylightName
		{
			get
			{
				return this._daylightDisplayName ?? string.Empty;
			}
		}

		/// <summary>Gets the time difference between the current time zone's standard time and Coordinated Universal Time (UTC).</summary>
		/// <returns>An object that indicates the time difference between the current time zone's standard time and Coordinated Universal Time (UTC).</returns>
		public TimeSpan BaseUtcOffset
		{
			get
			{
				return this._baseUtcOffset;
			}
		}

		/// <summary>Gets a value indicating whether the time zone has any daylight saving time rules.</summary>
		/// <returns>
		///   <see langword="true" /> if the time zone supports daylight saving time; otherwise, <see langword="false" />.</returns>
		public bool SupportsDaylightSavingTime
		{
			get
			{
				return this._supportsDaylightSavingTime;
			}
		}

		/// <summary>Returns information about the possible dates and times that an ambiguous date and time can be mapped to.</summary>
		/// <param name="dateTimeOffset">A date and time.</param>
		/// <returns>An array of objects that represents possible Coordinated Universal Time (UTC) offsets that a particular date and time can be mapped to.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="dateTimeOffset" /> is not an ambiguous time.</exception>
		public TimeSpan[] GetAmbiguousTimeOffsets(DateTimeOffset dateTimeOffset)
		{
			if (!this.SupportsDaylightSavingTime)
			{
				throw new ArgumentException("The supplied DateTimeOffset is not in an ambiguous time range.", "dateTimeOffset");
			}
			DateTime dateTime = TimeZoneInfo.ConvertTime(dateTimeOffset, this).DateTime;
			bool flag = false;
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForAmbiguousOffsets = this.GetAdjustmentRuleForAmbiguousOffsets(dateTime, out ruleIndex);
			if (adjustmentRuleForAmbiguousOffsets != null && adjustmentRuleForAmbiguousOffsets.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = this.GetDaylightTime(dateTime.Year, adjustmentRuleForAmbiguousOffsets, ruleIndex);
				flag = TimeZoneInfo.GetIsAmbiguousTime(dateTime, adjustmentRuleForAmbiguousOffsets, daylightTime);
			}
			if (!flag)
			{
				throw new ArgumentException("The supplied DateTimeOffset is not in an ambiguous time range.", "dateTimeOffset");
			}
			TimeSpan[] array = new TimeSpan[2];
			TimeSpan timeSpan = this._baseUtcOffset + adjustmentRuleForAmbiguousOffsets.BaseUtcOffsetDelta;
			if (adjustmentRuleForAmbiguousOffsets.DaylightDelta > TimeSpan.Zero)
			{
				array[0] = timeSpan;
				array[1] = timeSpan + adjustmentRuleForAmbiguousOffsets.DaylightDelta;
			}
			else
			{
				array[0] = timeSpan + adjustmentRuleForAmbiguousOffsets.DaylightDelta;
				array[1] = timeSpan;
			}
			return array;
		}

		/// <summary>Returns information about the possible dates and times that an ambiguous date and time can be mapped to.</summary>
		/// <param name="dateTime">A date and time.</param>
		/// <returns>An array of objects that represents possible Coordinated Universal Time (UTC) offsets that a particular date and time can be mapped to.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="dateTime" /> is not an ambiguous time.</exception>
		public TimeSpan[] GetAmbiguousTimeOffsets(DateTime dateTime)
		{
			if (!this.SupportsDaylightSavingTime)
			{
				throw new ArgumentException("The supplied DateTime is not in an ambiguous time range.", "dateTime");
			}
			DateTime dateTime2;
			if (dateTime.Kind == DateTimeKind.Local)
			{
				TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, TimeZoneInfoOptions.None, cachedData);
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				TimeZoneInfo.CachedData cachedData2 = TimeZoneInfo.s_cachedData;
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.s_utcTimeZone, this, TimeZoneInfoOptions.None, cachedData2);
			}
			else
			{
				dateTime2 = dateTime;
			}
			bool flag = false;
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForAmbiguousOffsets = this.GetAdjustmentRuleForAmbiguousOffsets(dateTime2, out ruleIndex);
			if (adjustmentRuleForAmbiguousOffsets != null && adjustmentRuleForAmbiguousOffsets.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = this.GetDaylightTime(dateTime2.Year, adjustmentRuleForAmbiguousOffsets, ruleIndex);
				flag = TimeZoneInfo.GetIsAmbiguousTime(dateTime2, adjustmentRuleForAmbiguousOffsets, daylightTime);
			}
			if (!flag)
			{
				throw new ArgumentException("The supplied DateTime is not in an ambiguous time range.", "dateTime");
			}
			TimeSpan[] array = new TimeSpan[2];
			TimeSpan timeSpan = this._baseUtcOffset + adjustmentRuleForAmbiguousOffsets.BaseUtcOffsetDelta;
			if (adjustmentRuleForAmbiguousOffsets.DaylightDelta > TimeSpan.Zero)
			{
				array[0] = timeSpan;
				array[1] = timeSpan + adjustmentRuleForAmbiguousOffsets.DaylightDelta;
			}
			else
			{
				array[0] = timeSpan + adjustmentRuleForAmbiguousOffsets.DaylightDelta;
				array[1] = timeSpan;
			}
			return array;
		}

		private TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForAmbiguousOffsets(DateTime adjustedTime, out int? ruleIndex)
		{
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(adjustedTime, out ruleIndex);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.NoDaylightTransitions && !adjustmentRuleForTime.HasDaylightSaving)
			{
				return this.GetPreviousAdjustmentRule(adjustmentRuleForTime, ruleIndex);
			}
			return adjustmentRuleForTime;
		}

		private TimeZoneInfo.AdjustmentRule GetPreviousAdjustmentRule(TimeZoneInfo.AdjustmentRule rule, int? ruleIndex)
		{
			if (ruleIndex != null && 0 < ruleIndex.Value && ruleIndex.Value < this._adjustmentRules.Length)
			{
				return this._adjustmentRules[ruleIndex.Value - 1];
			}
			TimeZoneInfo.AdjustmentRule result = rule;
			for (int i = 1; i < this._adjustmentRules.Length; i++)
			{
				if (rule == this._adjustmentRules[i])
				{
					result = this._adjustmentRules[i - 1];
					break;
				}
			}
			return result;
		}

		/// <summary>Calculates the offset or difference between the time in this time zone and Coordinated Universal Time (UTC) for a particular date and time.</summary>
		/// <param name="dateTimeOffset">The date and time to determine the offset for.</param>
		/// <returns>An object that indicates the time difference between Coordinated Universal Time (UTC) and the current time zone.</returns>
		public TimeSpan GetUtcOffset(DateTimeOffset dateTimeOffset)
		{
			return TimeZoneInfo.GetUtcOffsetFromUtc(dateTimeOffset.UtcDateTime, this);
		}

		/// <summary>Calculates the offset or difference between the time in this time zone and Coordinated Universal Time (UTC) for a particular date and time.</summary>
		/// <param name="dateTime">The date and time to determine the offset for.</param>
		/// <returns>An object that indicates the time difference between the two time zones.</returns>
		public TimeSpan GetUtcOffset(DateTime dateTime)
		{
			return this.GetUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime, TimeZoneInfo.s_cachedData);
		}

		internal static TimeSpan GetLocalUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return cachedData.Local.GetUtcOffset(dateTime, flags, cachedData);
		}

		internal TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return this.GetUtcOffset(dateTime, flags, TimeZoneInfo.s_cachedData);
		}

		private TimeSpan GetUtcOffset(DateTime dateTime, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (dateTime.Kind == DateTimeKind.Local)
			{
				if (cachedData.GetCorrespondingKind(this) != DateTimeKind.Local)
				{
					return TimeZoneInfo.GetUtcOffsetFromUtc(TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, TimeZoneInfo.s_utcTimeZone, flags), this);
				}
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				if (cachedData.GetCorrespondingKind(this) == DateTimeKind.Utc)
				{
					return this._baseUtcOffset;
				}
				return TimeZoneInfo.GetUtcOffsetFromUtc(dateTime, this);
			}
			return TimeZoneInfo.GetUtcOffset(dateTime, this, flags);
		}

		/// <summary>Determines whether a particular date and time in a particular time zone is ambiguous and can be mapped to two or more Coordinated Universal Time (UTC) times.</summary>
		/// <param name="dateTimeOffset">A date and time.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="dateTimeOffset" /> parameter is ambiguous in the current time zone; otherwise, <see langword="false" />.</returns>
		public bool IsAmbiguousTime(DateTimeOffset dateTimeOffset)
		{
			return this._supportsDaylightSavingTime && this.IsAmbiguousTime(TimeZoneInfo.ConvertTime(dateTimeOffset, this).DateTime);
		}

		/// <summary>Determines whether a particular date and time in a particular time zone is ambiguous and can be mapped to two or more Coordinated Universal Time (UTC) times.</summary>
		/// <param name="dateTime">A date and time value.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="dateTime" /> parameter is ambiguous; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateTime" /> value is <see cref="F:System.DateTimeKind.Local" /> and <paramref name="dateTime" /> is an invalid time.</exception>
		public bool IsAmbiguousTime(DateTime dateTime)
		{
			return this.IsAmbiguousTime(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
		}

		internal bool IsAmbiguousTime(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			if (!this._supportsDaylightSavingTime)
			{
				return false;
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			DateTime dateTime2 = (dateTime.Kind == DateTimeKind.Local) ? TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, flags, cachedData) : ((dateTime.Kind == DateTimeKind.Utc) ? TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.s_utcTimeZone, this, flags, cachedData) : dateTime);
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime2, out ruleIndex);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = this.GetDaylightTime(dateTime2.Year, adjustmentRuleForTime, ruleIndex);
				return TimeZoneInfo.GetIsAmbiguousTime(dateTime2, adjustmentRuleForTime, daylightTime);
			}
			return false;
		}

		/// <summary>Indicates whether a specified date and time falls in the range of daylight saving time for the time zone of the current <see cref="T:System.TimeZoneInfo" /> object.</summary>
		/// <param name="dateTimeOffset">A date and time value.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="dateTimeOffset" /> parameter is a daylight saving time; otherwise, <see langword="false" />.</returns>
		public bool IsDaylightSavingTime(DateTimeOffset dateTimeOffset)
		{
			bool result;
			TimeZoneInfo.GetUtcOffsetFromUtc(dateTimeOffset.UtcDateTime, this, out result);
			return result;
		}

		/// <summary>Indicates whether a specified date and time falls in the range of daylight saving time for the time zone of the current <see cref="T:System.TimeZoneInfo" /> object.</summary>
		/// <param name="dateTime">A date and time value.</param>
		/// <returns>
		///   <see langword="true" /> if the <paramref name="dateTime" /> parameter is a daylight saving time; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateTime" /> value is <see cref="F:System.DateTimeKind.Local" /> and <paramref name="dateTime" /> is an invalid time.</exception>
		public bool IsDaylightSavingTime(DateTime dateTime)
		{
			return this.IsDaylightSavingTime(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime, TimeZoneInfo.s_cachedData);
		}

		internal bool IsDaylightSavingTime(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			return this.IsDaylightSavingTime(dateTime, flags, TimeZoneInfo.s_cachedData);
		}

		private bool IsDaylightSavingTime(DateTime dateTime, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (!this._supportsDaylightSavingTime || this._adjustmentRules == null)
			{
				return false;
			}
			DateTime dateTime2;
			if (dateTime.Kind == DateTimeKind.Local)
			{
				dateTime2 = TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, this, flags, cachedData);
			}
			else if (dateTime.Kind == DateTimeKind.Utc)
			{
				if (cachedData.GetCorrespondingKind(this) == DateTimeKind.Utc)
				{
					return false;
				}
				bool result;
				TimeZoneInfo.GetUtcOffsetFromUtc(dateTime, this, out result);
				return result;
			}
			else
			{
				dateTime2 = dateTime;
			}
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime2, out ruleIndex);
			if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
			{
				DaylightTimeStruct daylightTime = this.GetDaylightTime(dateTime2.Year, adjustmentRuleForTime, ruleIndex);
				return TimeZoneInfo.GetIsDaylightSavings(dateTime2, adjustmentRuleForTime, daylightTime, flags);
			}
			return false;
		}

		/// <summary>Indicates whether a particular date and time is invalid.</summary>
		/// <param name="dateTime">A date and time value.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="dateTime" /> is invalid; otherwise, <see langword="false" />.</returns>
		public bool IsInvalidTime(DateTime dateTime)
		{
			bool result = false;
			if (dateTime.Kind == DateTimeKind.Unspecified || (dateTime.Kind == DateTimeKind.Local && TimeZoneInfo.s_cachedData.GetCorrespondingKind(this) == DateTimeKind.Local))
			{
				int? ruleIndex;
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = this.GetAdjustmentRuleForTime(dateTime, out ruleIndex);
				if (adjustmentRuleForTime != null && adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = this.GetDaylightTime(dateTime.Year, adjustmentRuleForTime, ruleIndex);
					result = TimeZoneInfo.GetIsInvalidTime(dateTime, adjustmentRuleForTime, daylightTime);
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		/// <summary>Clears cached time zone data.</summary>
		public static void ClearCachedData()
		{
			TimeZoneInfo.s_cachedData = new TimeZoneInfo.CachedData();
		}

		/// <summary>Converts a time to the time in another time zone based on the time zone's identifier.</summary>
		/// <param name="dateTimeOffset">The date and time to convert.</param>
		/// <param name="destinationTimeZoneId">The identifier of the destination time zone.</param>
		/// <returns>The date and time in the destination time zone.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destinationTimeZoneId" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The time zone identifier was found but the registry data is corrupted.</exception>
		/// <exception cref="T:System.Security.SecurityException">The process does not have the permissions required to read from the registry key that contains the time zone information.</exception>
		/// <exception cref="T:System.TimeZoneNotFoundException">The <paramref name="destinationTimeZoneId" /> identifier was not found on the local system.</exception>
		public static DateTimeOffset ConvertTimeBySystemTimeZoneId(DateTimeOffset dateTimeOffset, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		/// <summary>Converts a time to the time in another time zone based on the time zone's identifier.</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <param name="destinationTimeZoneId">The identifier of the destination time zone.</param>
		/// <returns>The date and time in the destination time zone.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destinationTimeZoneId" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The time zone identifier was found, but the registry data is corrupted.</exception>
		/// <exception cref="T:System.Security.SecurityException">The process does not have the permissions required to read from the registry key that contains the time zone information.</exception>
		/// <exception cref="T:System.TimeZoneNotFoundException">The <paramref name="destinationTimeZoneId" /> identifier was not found on the local system.</exception>
		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string destinationTimeZoneId)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		/// <summary>Converts a time from one time zone to another based on time zone identifiers.</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <param name="sourceTimeZoneId">The identifier of the source time zone.</param>
		/// <param name="destinationTimeZoneId">The identifier of the destination time zone.</param>
		/// <returns>The date and time in the destination time zone that corresponds to the <paramref name="dateTime" /> parameter in the source time zone.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateTime" /> parameter does not correspond to the source time zone.  
		///  -or-  
		///  <paramref name="dateTime" /> is an invalid time in the source time zone.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="sourceTimeZoneId" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="destinationTimeZoneId" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The time zone identifiers were found, but the registry data is corrupted.</exception>
		/// <exception cref="T:System.Security.SecurityException">The user does not have the permissions required to read from the registry keys that hold time zone data.</exception>
		/// <exception cref="T:System.TimeZoneNotFoundException">The <paramref name="sourceTimeZoneId" /> identifier was not found on the local system.  
		///  -or-  
		///  The <paramref name="destinationTimeZoneId" /> identifier was not found on the local system.</exception>
		public static DateTime ConvertTimeBySystemTimeZoneId(DateTime dateTime, string sourceTimeZoneId, string destinationTimeZoneId)
		{
			if (dateTime.Kind == DateTimeKind.Local && string.Equals(sourceTimeZoneId, TimeZoneInfo.Local.Id, StringComparison.OrdinalIgnoreCase))
			{
				TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
				return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId), TimeZoneInfoOptions.None, cachedData);
			}
			if (dateTime.Kind == DateTimeKind.Utc && string.Equals(sourceTimeZoneId, TimeZoneInfo.Utc.Id, StringComparison.OrdinalIgnoreCase))
			{
				return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.s_utcTimeZone, TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId), TimeZoneInfoOptions.None, TimeZoneInfo.s_cachedData);
			}
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.FindSystemTimeZoneById(sourceTimeZoneId), TimeZoneInfo.FindSystemTimeZoneById(destinationTimeZoneId));
		}

		/// <summary>Converts a time to the time in a particular time zone.</summary>
		/// <param name="dateTimeOffset">The date and time to convert.</param>
		/// <param name="destinationTimeZone">The time zone to convert <paramref name="dateTime" /> to.</param>
		/// <returns>The date and time in the destination time zone.</returns>
		/// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="destinationTimeZone" /> parameter is <see langword="null" />.</exception>
		public static DateTimeOffset ConvertTime(DateTimeOffset dateTimeOffset, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			DateTime utcDateTime = dateTimeOffset.UtcDateTime;
			TimeSpan utcOffsetFromUtc = TimeZoneInfo.GetUtcOffsetFromUtc(utcDateTime, destinationTimeZone);
			long num = utcDateTime.Ticks + utcOffsetFromUtc.Ticks;
			if (num > DateTimeOffset.MaxValue.Ticks)
			{
				return DateTimeOffset.MaxValue;
			}
			if (num >= DateTimeOffset.MinValue.Ticks)
			{
				return new DateTimeOffset(num, utcOffsetFromUtc);
			}
			return DateTimeOffset.MinValue;
		}

		/// <summary>Converts a time to the time in a particular time zone.</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <param name="destinationTimeZone">The time zone to convert <paramref name="dateTime" /> to.</param>
		/// <returns>The date and time in the destination time zone.</returns>
		/// <exception cref="T:System.ArgumentException">The value of the <paramref name="dateTime" /> parameter represents an invalid time.</exception>
		/// <exception cref="T:System.ArgumentNullException">The value of the <paramref name="destinationTimeZone" /> parameter is <see langword="null" />.</exception>
		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			if (dateTime.Ticks == 0L)
			{
				TimeZoneInfo.ClearCachedData();
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			TimeZoneInfo sourceTimeZone = (dateTime.Kind == DateTimeKind.Utc) ? TimeZoneInfo.s_utcTimeZone : cachedData.Local;
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone, TimeZoneInfoOptions.None, cachedData);
		}

		/// <summary>Converts a time from one time zone to another.</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <param name="sourceTimeZone">The time zone of <paramref name="dateTime" />.</param>
		/// <param name="destinationTimeZone">The time zone to convert <paramref name="dateTime" /> to.</param>
		/// <returns>The date and time in the destination time zone that corresponds to the <paramref name="dateTime" /> parameter in the source time zone.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateTime" /> parameter is <see cref="F:System.DateTimeKind.Local" />, but the <paramref name="sourceTimeZone" /> parameter does not equal <see cref="F:System.DateTimeKind.Local" />.  
		///  -or-  
		///  The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateTime" /> parameter is <see cref="F:System.DateTimeKind.Utc" />, but the <paramref name="sourceTimeZone" /> parameter does not equal <see cref="P:System.TimeZoneInfo.Utc" />.  
		///  -or-  
		///  The <paramref name="dateTime" /> parameter is an invalid time (that is, it represents a time that does not exist because of a time zone's adjustment rules).</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="sourceTimeZone" /> parameter is <see langword="null" />.  
		///  -or-  
		///  The <paramref name="destinationTimeZone" /> parameter is <see langword="null" />.</exception>
		public static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone, TimeZoneInfoOptions.None, TimeZoneInfo.s_cachedData);
		}

		internal static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone, TimeZoneInfoOptions flags)
		{
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, destinationTimeZone, flags, TimeZoneInfo.s_cachedData);
		}

		private static DateTime ConvertTime(DateTime dateTime, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone, TimeZoneInfoOptions flags, TimeZoneInfo.CachedData cachedData)
		{
			if (sourceTimeZone == null)
			{
				throw new ArgumentNullException("sourceTimeZone");
			}
			if (destinationTimeZone == null)
			{
				throw new ArgumentNullException("destinationTimeZone");
			}
			DateTimeKind correspondingKind = cachedData.GetCorrespondingKind(sourceTimeZone);
			if ((flags & TimeZoneInfoOptions.NoThrowOnInvalidTime) == (TimeZoneInfoOptions)0 && dateTime.Kind != DateTimeKind.Unspecified && dateTime.Kind != correspondingKind)
			{
				throw new ArgumentException("The conversion could not be completed because the supplied DateTime did not have the Kind property set correctly.  For example, when the Kind property is DateTimeKind.Local, the source time zone must be TimeZoneInfo.Local.", "sourceTimeZone");
			}
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = sourceTimeZone.GetAdjustmentRuleForTime(dateTime, out ruleIndex);
			TimeSpan t = sourceTimeZone.BaseUtcOffset;
			if (adjustmentRuleForTime != null)
			{
				t += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = sourceTimeZone.GetDaylightTime(dateTime.Year, adjustmentRuleForTime, ruleIndex);
					if ((flags & TimeZoneInfoOptions.NoThrowOnInvalidTime) == (TimeZoneInfoOptions)0 && TimeZoneInfo.GetIsInvalidTime(dateTime, adjustmentRuleForTime, daylightTime))
					{
						throw new ArgumentException("The supplied DateTime represents an invalid time.  For example, when the clock is adjusted forward, any time in the period that is skipped is invalid.", "dateTime");
					}
					bool isDaylightSavings = TimeZoneInfo.GetIsDaylightSavings(dateTime, adjustmentRuleForTime, daylightTime, flags);
					t += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			DateTimeKind correspondingKind2 = cachedData.GetCorrespondingKind(destinationTimeZone);
			if (dateTime.Kind != DateTimeKind.Unspecified && correspondingKind != DateTimeKind.Unspecified && correspondingKind == correspondingKind2)
			{
				return dateTime;
			}
			bool isAmbiguousDst;
			DateTime dateTime2 = TimeZoneInfo.ConvertUtcToTimeZone(dateTime.Ticks - t.Ticks, destinationTimeZone, out isAmbiguousDst);
			if (correspondingKind2 == DateTimeKind.Local)
			{
				return new DateTime(dateTime2.Ticks, DateTimeKind.Local, isAmbiguousDst);
			}
			return new DateTime(dateTime2.Ticks, correspondingKind2);
		}

		/// <summary>Converts a Coordinated Universal Time (UTC) to the time in a specified time zone.</summary>
		/// <param name="dateTime">The Coordinated Universal Time (UTC).</param>
		/// <param name="destinationTimeZone">The time zone to convert <paramref name="dateTime" /> to.</param>
		/// <returns>The date and time in the destination time zone. Its <see cref="P:System.DateTime.Kind" /> property is <see cref="F:System.DateTimeKind.Utc" /> if <paramref name="destinationTimeZone" /> is <see cref="P:System.TimeZoneInfo.Utc" />; otherwise, its <see cref="P:System.DateTime.Kind" /> property is <see cref="F:System.DateTimeKind.Unspecified" />.</returns>
		/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of <paramref name="dateTime" /> is <see cref="F:System.DateTimeKind.Local" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="destinationTimeZone" /> is <see langword="null" />.</exception>
		public static DateTime ConvertTimeFromUtc(DateTime dateTime, TimeZoneInfo destinationTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.s_utcTimeZone, destinationTimeZone, TimeZoneInfoOptions.None, TimeZoneInfo.s_cachedData);
		}

		/// <summary>Converts the specified date and time to Coordinated Universal Time (UTC).</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <returns>The Coordinated Universal Time (UTC) that corresponds to the <paramref name="dateTime" /> parameter. The <see cref="T:System.DateTime" /> value's <see cref="P:System.DateTime.Kind" /> property is always set to <see cref="F:System.DateTimeKind.Utc" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <see langword="TimeZoneInfo.Local.IsInvalidDateTime(" />
		///   <paramref name="dateTime" />
		///   <see langword=")" /> returns <see langword="true" />.</exception>
		public static DateTime ConvertTimeToUtc(DateTime dateTime)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, TimeZoneInfo.s_utcTimeZone, TimeZoneInfoOptions.None, cachedData);
		}

		internal static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfoOptions flags)
		{
			if (dateTime.Kind == DateTimeKind.Utc)
			{
				return dateTime;
			}
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			return TimeZoneInfo.ConvertTime(dateTime, cachedData.Local, TimeZoneInfo.s_utcTimeZone, flags, cachedData);
		}

		/// <summary>Converts the time in a specified time zone to Coordinated Universal Time (UTC).</summary>
		/// <param name="dateTime">The date and time to convert.</param>
		/// <param name="sourceTimeZone">The time zone of <paramref name="dateTime" />.</param>
		/// <returns>The Coordinated Universal Time (UTC) that corresponds to the <paramref name="dateTime" /> parameter. The <see cref="T:System.DateTime" /> object's <see cref="P:System.DateTime.Kind" /> property is always set to <see cref="F:System.DateTimeKind.Utc" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="dateTime" />.<see langword="Kind" /> is <see cref="F:System.DateTimeKind.Utc" /> and <paramref name="sourceTimeZone" /> does not equal <see cref="P:System.TimeZoneInfo.Utc" />.  
		/// -or-  
		/// <paramref name="dateTime" />.<see langword="Kind" /> is <see cref="F:System.DateTimeKind.Local" /> and <paramref name="sourceTimeZone" /> does not equal <see cref="P:System.TimeZoneInfo.Local" />.  
		/// -or-  
		/// <paramref name="sourceTimeZone" /><see langword=".IsInvalidDateTime(" /><paramref name="dateTime" /><see langword=")" /> returns <see langword="true" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="sourceTimeZone" /> is <see langword="null" />.</exception>
		public static DateTime ConvertTimeToUtc(DateTime dateTime, TimeZoneInfo sourceTimeZone)
		{
			return TimeZoneInfo.ConvertTime(dateTime, sourceTimeZone, TimeZoneInfo.s_utcTimeZone, TimeZoneInfoOptions.None, TimeZoneInfo.s_cachedData);
		}

		/// <summary>Determines whether the current <see cref="T:System.TimeZoneInfo" /> object and another <see cref="T:System.TimeZoneInfo" /> object are equal.</summary>
		/// <param name="other">A second object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if the two <see cref="T:System.TimeZoneInfo" /> objects are equal; otherwise, <see langword="false" />.</returns>
		public bool Equals(TimeZoneInfo other)
		{
			return other != null && string.Equals(this._id, other._id, StringComparison.OrdinalIgnoreCase) && this.HasSameRules(other);
		}

		/// <summary>Determines whether the current <see cref="T:System.TimeZoneInfo" /> object and another object are equal.</summary>
		/// <param name="obj">A second object to compare with the current object.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is a <see cref="T:System.TimeZoneInfo" /> object that is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			return this.Equals(obj as TimeZoneInfo);
		}

		/// <summary>Deserializes a string to re-create an original serialized <see cref="T:System.TimeZoneInfo" /> object.</summary>
		/// <param name="source">The string representation of the serialized <see cref="T:System.TimeZoneInfo" /> object.</param>
		/// <returns>The original serialized object.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="source" /> parameter is <see cref="F:System.String.Empty" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="source" /> parameter is a null string.</exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The source parameter cannot be deserialized back into a <see cref="T:System.TimeZoneInfo" /> object.</exception>
		public static TimeZoneInfo FromSerializedString(string source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.Length == 0)
			{
				throw new ArgumentException(SR.Format("The specified serialized string '{0}' is not supported.", source), "source");
			}
			return TimeZoneInfo.StringSerializer.GetDeserializedTimeZoneInfo(source);
		}

		/// <summary>Serves as a hash function for hashing algorithms and data structures such as hash tables.</summary>
		/// <returns>A 32-bit signed integer that serves as the hash code for this <see cref="T:System.TimeZoneInfo" /> object.</returns>
		public override int GetHashCode()
		{
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this._id);
		}

		/// <summary>Returns a sorted collection of all the time zones about which information is available on the local system.</summary>
		/// <returns>A read-only collection of <see cref="T:System.TimeZoneInfo" /> objects.</returns>
		/// <exception cref="T:System.OutOfMemoryException">There is insufficient memory to store all time zone information.</exception>
		/// <exception cref="T:System.Security.SecurityException">The user does not have permission to read from the registry keys that contain time zone information.</exception>
		public static ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
		{
			TimeZoneInfo.CachedData cachedData = TimeZoneInfo.s_cachedData;
			TimeZoneInfo.CachedData obj = cachedData;
			lock (obj)
			{
				if (cachedData._readOnlySystemTimeZones == null)
				{
					TimeZoneInfo.PopulateAllSystemTimeZones(cachedData);
					cachedData._allSystemTimeZonesRead = true;
					List<TimeZoneInfo> list;
					if (cachedData._systemTimeZones != null)
					{
						list = new List<TimeZoneInfo>(cachedData._systemTimeZones.Values);
					}
					else
					{
						list = new List<TimeZoneInfo>();
					}
					list.Sort(delegate(TimeZoneInfo x, TimeZoneInfo y)
					{
						int num = x.BaseUtcOffset.CompareTo(y.BaseUtcOffset);
						if (num != 0)
						{
							return num;
						}
						return string.CompareOrdinal(x.DisplayName, y.DisplayName);
					});
					cachedData._readOnlySystemTimeZones = new ReadOnlyCollection<TimeZoneInfo>(list);
				}
			}
			return cachedData._readOnlySystemTimeZones;
		}

		/// <summary>Indicates whether the current object and another <see cref="T:System.TimeZoneInfo" /> object have the same adjustment rules.</summary>
		/// <param name="other">A second object to compare with the current <see cref="T:System.TimeZoneInfo" /> object.</param>
		/// <returns>
		///   <see langword="true" /> if the two time zones have identical adjustment rules and an identical base offset; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="other" /> parameter is <see langword="null" />.</exception>
		public bool HasSameRules(TimeZoneInfo other)
		{
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			if (this._baseUtcOffset != other._baseUtcOffset || this._supportsDaylightSavingTime != other._supportsDaylightSavingTime)
			{
				return false;
			}
			TimeZoneInfo.AdjustmentRule[] adjustmentRules = this._adjustmentRules;
			TimeZoneInfo.AdjustmentRule[] adjustmentRules2 = other._adjustmentRules;
			bool flag = (adjustmentRules == null && adjustmentRules2 == null) || (adjustmentRules != null && adjustmentRules2 != null);
			if (!flag)
			{
				return false;
			}
			if (adjustmentRules != null)
			{
				if (adjustmentRules.Length != adjustmentRules2.Length)
				{
					return false;
				}
				for (int i = 0; i < adjustmentRules.Length; i++)
				{
					if (!adjustmentRules[i].Equals(adjustmentRules2[i]))
					{
						return false;
					}
				}
			}
			return flag;
		}

		/// <summary>Gets a <see cref="T:System.TimeZoneInfo" /> object that represents the local time zone.</summary>
		/// <returns>An object that represents the local time zone.</returns>
		public static TimeZoneInfo Local
		{
			get
			{
				return TimeZoneInfo.s_cachedData.Local;
			}
		}

		/// <summary>Converts the current <see cref="T:System.TimeZoneInfo" /> object to a serialized string.</summary>
		/// <returns>A string that represents the current <see cref="T:System.TimeZoneInfo" /> object.</returns>
		public string ToSerializedString()
		{
			return TimeZoneInfo.StringSerializer.GetSerializedString(this);
		}

		/// <summary>Returns the current <see cref="T:System.TimeZoneInfo" /> object's display name.</summary>
		/// <returns>The value of the <see cref="P:System.TimeZoneInfo.DisplayName" /> property of the current <see cref="T:System.TimeZoneInfo" /> object.</returns>
		public override string ToString()
		{
			return this.DisplayName;
		}

		/// <summary>Gets a <see cref="T:System.TimeZoneInfo" /> object that represents the Coordinated Universal Time (UTC) zone.</summary>
		/// <returns>An object that represents the Coordinated Universal Time (UTC) zone.</returns>
		public static TimeZoneInfo Utc
		{
			get
			{
				return TimeZoneInfo.s_utcTimeZone;
			}
		}

		private TimeZoneInfo(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			bool flag;
			TimeZoneInfo.ValidateTimeZoneInfo(id, baseUtcOffset, adjustmentRules, out flag);
			this._id = id;
			this._baseUtcOffset = baseUtcOffset;
			this._displayName = displayName;
			this._standardDisplayName = standardDisplayName;
			this._daylightDisplayName = (disableDaylightSavingTime ? null : daylightDisplayName);
			this._supportsDaylightSavingTime = (flag && !disableDaylightSavingTime);
			this._adjustmentRules = adjustmentRules;
		}

		/// <summary>Creates a custom time zone with a specified identifier, an offset from Coordinated Universal Time (UTC), a display name, and a standard time display name.</summary>
		/// <param name="id">The time zone's identifier.</param>
		/// <param name="baseUtcOffset">An object that represents the time difference between this time zone and Coordinated Universal Time (UTC).</param>
		/// <param name="displayName">The display name of the new time zone.</param>
		/// <param name="standardDisplayName">The name of the new time zone's standard time.</param>
		/// <returns>The new time zone.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="id" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="id" /> parameter is an empty string ("").  
		///  -or-  
		///  The <paramref name="baseUtcOffset" /> parameter does not represent a whole number of minutes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="baseUtcOffset" /> parameter is greater than 14 hours or less than -14 hours.</exception>
		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName)
		{
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, standardDisplayName, null, false);
		}

		/// <summary>Creates a custom time zone with a specified identifier, an offset from Coordinated Universal Time (UTC), a display name, a standard time name, a daylight saving time name, and daylight saving time rules.</summary>
		/// <param name="id">The time zone's identifier.</param>
		/// <param name="baseUtcOffset">An object that represents the time difference between this time zone and Coordinated Universal Time (UTC).</param>
		/// <param name="displayName">The display name of the new time zone.</param>
		/// <param name="standardDisplayName">The new time zone's standard time name.</param>
		/// <param name="daylightDisplayName">The daylight saving time name of the new time zone.</param>
		/// <param name="adjustmentRules">An array that augments the base UTC offset for a particular period.</param>
		/// <returns>A <see cref="T:System.TimeZoneInfo" /> object that represents the new time zone.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="id" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="id" /> parameter is an empty string ("").  
		///  -or-  
		///  The <paramref name="baseUtcOffset" /> parameter does not represent a whole number of minutes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="baseUtcOffset" /> parameter is greater than 14 hours or less than -14 hours.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The adjustment rules specified in the <paramref name="adjustmentRules" /> parameter overlap.  
		///  -or-  
		///  The adjustment rules specified in the <paramref name="adjustmentRules" /> parameter are not in chronological order.  
		///  -or-  
		///  One or more elements in <paramref name="adjustmentRules" /> are <see langword="null" />.  
		///  -or-  
		///  A date can have multiple adjustment rules applied to it.  
		///  -or-  
		///  The sum of the <paramref name="baseUtcOffset" /> parameter and the <see cref="P:System.TimeZoneInfo.AdjustmentRule.DaylightDelta" /> value of one or more objects in the <paramref name="adjustmentRules" /> array is greater than 14 hours or less than -14 hours.</exception>
		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules)
		{
			return TimeZoneInfo.CreateCustomTimeZone(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, false);
		}

		/// <summary>Creates a custom time zone with a specified identifier, an offset from Coordinated Universal Time (UTC), a display name, a standard time name, a daylight saving time name, daylight saving time rules, and a value that indicates whether the returned object reflects daylight saving time information.</summary>
		/// <param name="id">The time zone's identifier.</param>
		/// <param name="baseUtcOffset">A <see cref="T:System.TimeSpan" /> object that represents the time difference between this time zone and Coordinated Universal Time (UTC).</param>
		/// <param name="displayName">The display name of the new time zone.</param>
		/// <param name="standardDisplayName">The standard time name of the new time zone.</param>
		/// <param name="daylightDisplayName">The daylight saving time name of the new time zone.</param>
		/// <param name="adjustmentRules">An array of <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> objects that augment the base UTC offset for a particular period.</param>
		/// <param name="disableDaylightSavingTime">
		///   <see langword="true" /> to discard any daylight saving time-related information present in <paramref name="adjustmentRules" /> with the new object; otherwise, <see langword="false" />.</param>
		/// <returns>The new time zone. If the <paramref name="disableDaylightSavingTime" /> parameter is <see langword="true" />, the returned object has no daylight saving time data.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="id" /> parameter is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The <paramref name="id" /> parameter is an empty string ("").  
		///  -or-  
		///  The <paramref name="baseUtcOffset" /> parameter does not represent a whole number of minutes.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="baseUtcOffset" /> parameter is greater than 14 hours or less than -14 hours.</exception>
		/// <exception cref="T:System.InvalidTimeZoneException">The adjustment rules specified in the <paramref name="adjustmentRules" /> parameter overlap.  
		///  -or-  
		///  The adjustment rules specified in the <paramref name="adjustmentRules" /> parameter are not in chronological order.  
		///  -or-  
		///  One or more elements in <paramref name="adjustmentRules" /> are <see langword="null" />.  
		///  -or-  
		///  A date can have multiple adjustment rules applied to it.  
		///  -or-  
		///  The sum of the <paramref name="baseUtcOffset" /> parameter and the <see cref="P:System.TimeZoneInfo.AdjustmentRule.DaylightDelta" /> value of one or more objects in the <paramref name="adjustmentRules" /> array is greater than 14 hours or less than -14 hours.</exception>
		public static TimeZoneInfo CreateCustomTimeZone(string id, TimeSpan baseUtcOffset, string displayName, string standardDisplayName, string daylightDisplayName, TimeZoneInfo.AdjustmentRule[] adjustmentRules, bool disableDaylightSavingTime)
		{
			if (!disableDaylightSavingTime && adjustmentRules != null && adjustmentRules.Length != 0)
			{
				adjustmentRules = (TimeZoneInfo.AdjustmentRule[])adjustmentRules.Clone();
			}
			return new TimeZoneInfo(id, baseUtcOffset, displayName, standardDisplayName, daylightDisplayName, adjustmentRules, disableDaylightSavingTime);
		}

		/// <summary>Runs when the deserialization of an object has been completed.</summary>
		/// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The <see cref="T:System.TimeZoneInfo" /> object contains invalid or corrupted data.</exception>
		void IDeserializationCallback.OnDeserialization(object sender)
		{
			try
			{
				bool flag;
				TimeZoneInfo.ValidateTimeZoneInfo(this._id, this._baseUtcOffset, this._adjustmentRules, out flag);
				if (flag != this._supportsDaylightSavingTime)
				{
					throw new SerializationException(SR.Format("The value of the field '{0}' is invalid.  The serialized data is corrupt.", "SupportsDaylightSavingTime"));
				}
			}
			catch (ArgumentException innerException)
			{
				throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
			}
			catch (InvalidTimeZoneException innerException2)
			{
				throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException2);
			}
		}

		/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data needed to serialize the current <see cref="T:System.TimeZoneInfo" /> object.</summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object to populate with data.</param>
		/// <param name="context">The destination for this serialization (see <see cref="T:System.Runtime.Serialization.StreamingContext" />).</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info" /> parameter is <see langword="null" />.</exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Id", this._id);
			info.AddValue("DisplayName", this._displayName);
			info.AddValue("StandardName", this._standardDisplayName);
			info.AddValue("DaylightName", this._daylightDisplayName);
			info.AddValue("BaseUtcOffset", this._baseUtcOffset);
			info.AddValue("AdjustmentRules", this._adjustmentRules);
			info.AddValue("SupportsDaylightSavingTime", this._supportsDaylightSavingTime);
		}

		private TimeZoneInfo(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this._id = (string)info.GetValue("Id", typeof(string));
			this._displayName = (string)info.GetValue("DisplayName", typeof(string));
			this._standardDisplayName = (string)info.GetValue("StandardName", typeof(string));
			this._daylightDisplayName = (string)info.GetValue("DaylightName", typeof(string));
			this._baseUtcOffset = (TimeSpan)info.GetValue("BaseUtcOffset", typeof(TimeSpan));
			this._adjustmentRules = (TimeZoneInfo.AdjustmentRule[])info.GetValue("AdjustmentRules", typeof(TimeZoneInfo.AdjustmentRule[]));
			this._supportsDaylightSavingTime = (bool)info.GetValue("SupportsDaylightSavingTime", typeof(bool));
		}

		private TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForTime(DateTime dateTime, out int? ruleIndex)
		{
			return this.GetAdjustmentRuleForTime(dateTime, false, out ruleIndex);
		}

		private TimeZoneInfo.AdjustmentRule GetAdjustmentRuleForTime(DateTime dateTime, bool dateTimeisUtc, out int? ruleIndex)
		{
			if (this._adjustmentRules == null || this._adjustmentRules.Length == 0)
			{
				ruleIndex = null;
				return null;
			}
			DateTime dateOnly = dateTimeisUtc ? (dateTime + this.BaseUtcOffset).Date : dateTime.Date;
			int i = 0;
			int num = this._adjustmentRules.Length - 1;
			while (i <= num)
			{
				int num2 = i + (num - i >> 1);
				TimeZoneInfo.AdjustmentRule adjustmentRule = this._adjustmentRules[num2];
				TimeZoneInfo.AdjustmentRule previousRule = (num2 > 0) ? this._adjustmentRules[num2 - 1] : adjustmentRule;
				int num3 = this.CompareAdjustmentRuleToDateTime(adjustmentRule, previousRule, dateTime, dateOnly, dateTimeisUtc);
				if (num3 == 0)
				{
					ruleIndex = new int?(num2);
					return adjustmentRule;
				}
				if (num3 < 0)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}
			ruleIndex = null;
			return null;
		}

		private int CompareAdjustmentRuleToDateTime(TimeZoneInfo.AdjustmentRule rule, TimeZoneInfo.AdjustmentRule previousRule, DateTime dateTime, DateTime dateOnly, bool dateTimeisUtc)
		{
			bool flag;
			if (rule.DateStart.Kind == DateTimeKind.Utc)
			{
				flag = ((dateTimeisUtc ? dateTime : this.ConvertToUtc(dateTime, previousRule.DaylightDelta, previousRule.BaseUtcOffsetDelta)) >= rule.DateStart);
			}
			else
			{
				flag = (dateOnly >= rule.DateStart);
			}
			if (!flag)
			{
				return 1;
			}
			bool flag2;
			if (rule.DateEnd.Kind == DateTimeKind.Utc)
			{
				flag2 = ((dateTimeisUtc ? dateTime : this.ConvertToUtc(dateTime, rule.DaylightDelta, rule.BaseUtcOffsetDelta)) <= rule.DateEnd);
			}
			else
			{
				flag2 = (dateOnly <= rule.DateEnd);
			}
			if (!flag2)
			{
				return -1;
			}
			return 0;
		}

		private DateTime ConvertToUtc(DateTime dateTime, TimeSpan daylightDelta, TimeSpan baseUtcOffsetDelta)
		{
			return this.ConvertToFromUtc(dateTime, daylightDelta, baseUtcOffsetDelta, true);
		}

		private DateTime ConvertFromUtc(DateTime dateTime, TimeSpan daylightDelta, TimeSpan baseUtcOffsetDelta)
		{
			return this.ConvertToFromUtc(dateTime, daylightDelta, baseUtcOffsetDelta, false);
		}

		private DateTime ConvertToFromUtc(DateTime dateTime, TimeSpan daylightDelta, TimeSpan baseUtcOffsetDelta, bool convertToUtc)
		{
			TimeSpan timeSpan = this.BaseUtcOffset + daylightDelta + baseUtcOffsetDelta;
			if (convertToUtc)
			{
				timeSpan = timeSpan.Negate();
			}
			long num = dateTime.Ticks + timeSpan.Ticks;
			if (num > DateTime.MaxValue.Ticks)
			{
				return DateTime.MaxValue;
			}
			if (num >= DateTime.MinValue.Ticks)
			{
				return new DateTime(num);
			}
			return DateTime.MinValue;
		}

		private static DateTime ConvertUtcToTimeZone(long ticks, TimeZoneInfo destinationTimeZone, out bool isAmbiguousLocalDst)
		{
			ticks += TimeZoneInfo.GetUtcOffsetFromUtc((ticks > DateTime.MaxValue.Ticks) ? DateTime.MaxValue : ((ticks < DateTime.MinValue.Ticks) ? DateTime.MinValue : new DateTime(ticks)), destinationTimeZone, out isAmbiguousLocalDst).Ticks;
			if (ticks > DateTime.MaxValue.Ticks)
			{
				return DateTime.MaxValue;
			}
			if (ticks >= DateTime.MinValue.Ticks)
			{
				return new DateTime(ticks);
			}
			return DateTime.MinValue;
		}

		private DaylightTimeStruct GetDaylightTime(int year, TimeZoneInfo.AdjustmentRule rule, int? ruleIndex)
		{
			TimeSpan daylightDelta = rule.DaylightDelta;
			DateTime start;
			DateTime end;
			if (rule.NoDaylightTransitions)
			{
				TimeZoneInfo.AdjustmentRule previousAdjustmentRule = this.GetPreviousAdjustmentRule(rule, ruleIndex);
				start = this.ConvertFromUtc(rule.DateStart, previousAdjustmentRule.DaylightDelta, previousAdjustmentRule.BaseUtcOffsetDelta);
				end = this.ConvertFromUtc(rule.DateEnd, rule.DaylightDelta, rule.BaseUtcOffsetDelta);
			}
			else
			{
				start = TimeZoneInfo.TransitionTimeToDateTime(year, rule.DaylightTransitionStart);
				end = TimeZoneInfo.TransitionTimeToDateTime(year, rule.DaylightTransitionEnd);
			}
			return new DaylightTimeStruct(start, end, daylightDelta);
		}

		private static bool GetIsDaylightSavings(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime, TimeZoneInfoOptions flags)
		{
			if (rule == null)
			{
				return false;
			}
			DateTime startTime;
			DateTime endTime;
			if (time.Kind == DateTimeKind.Local)
			{
				startTime = (rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : (daylightTime.Start + daylightTime.Delta));
				endTime = (rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) : daylightTime.End);
			}
			else
			{
				bool flag = rule.DaylightDelta > TimeSpan.Zero;
				startTime = (rule.IsStartDateMarkerForBeginningOfYear() ? new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) : (daylightTime.Start + (flag ? rule.DaylightDelta : TimeSpan.Zero)));
				endTime = (rule.IsEndDateMarkerForEndOfYear() ? new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) : (daylightTime.End + (flag ? (-rule.DaylightDelta) : TimeSpan.Zero)));
			}
			bool flag2 = TimeZoneInfo.CheckIsDst(startTime, time, endTime, false, rule);
			if (flag2 && time.Kind == DateTimeKind.Local && TimeZoneInfo.GetIsAmbiguousTime(time, rule, daylightTime))
			{
				flag2 = time.IsAmbiguousDaylightSavingTime();
			}
			return flag2;
		}

		private TimeSpan GetDaylightSavingsStartOffsetFromUtc(TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule rule, int? ruleIndex)
		{
			if (rule.NoDaylightTransitions)
			{
				TimeZoneInfo.AdjustmentRule previousAdjustmentRule = this.GetPreviousAdjustmentRule(rule, ruleIndex);
				return baseUtcOffset + previousAdjustmentRule.BaseUtcOffsetDelta + previousAdjustmentRule.DaylightDelta;
			}
			return baseUtcOffset + rule.BaseUtcOffsetDelta;
		}

		private TimeSpan GetDaylightSavingsEndOffsetFromUtc(TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule rule)
		{
			return baseUtcOffset + rule.BaseUtcOffsetDelta + rule.DaylightDelta;
		}

		private static bool GetIsDaylightSavingsFromUtc(DateTime time, int year, TimeSpan utc, TimeZoneInfo.AdjustmentRule rule, int? ruleIndex, out bool isAmbiguousLocalDst, TimeZoneInfo zone)
		{
			isAmbiguousLocalDst = false;
			if (rule == null)
			{
				return false;
			}
			DaylightTimeStruct daylightTime = zone.GetDaylightTime(year, rule, ruleIndex);
			bool ignoreYearAdjustment = false;
			TimeSpan daylightSavingsStartOffsetFromUtc = zone.GetDaylightSavingsStartOffsetFromUtc(utc, rule, ruleIndex);
			DateTime dateTime;
			if (rule.IsStartDateMarkerForBeginningOfYear() && daylightTime.Start.Year > DateTime.MinValue.Year)
			{
				int? ruleIndex2;
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(new DateTime(daylightTime.Start.Year - 1, 12, 31), out ruleIndex2);
				if (adjustmentRuleForTime != null && adjustmentRuleForTime.IsEndDateMarkerForEndOfYear())
				{
					dateTime = zone.GetDaylightTime(daylightTime.Start.Year - 1, adjustmentRuleForTime, ruleIndex2).Start - utc - adjustmentRuleForTime.BaseUtcOffsetDelta;
					ignoreYearAdjustment = true;
				}
				else
				{
					dateTime = new DateTime(daylightTime.Start.Year, 1, 1, 0, 0, 0) - daylightSavingsStartOffsetFromUtc;
				}
			}
			else
			{
				dateTime = daylightTime.Start - daylightSavingsStartOffsetFromUtc;
			}
			TimeSpan daylightSavingsEndOffsetFromUtc = zone.GetDaylightSavingsEndOffsetFromUtc(utc, rule);
			DateTime dateTime2;
			if (rule.IsEndDateMarkerForEndOfYear() && daylightTime.End.Year < DateTime.MaxValue.Year)
			{
				int? ruleIndex3;
				TimeZoneInfo.AdjustmentRule adjustmentRuleForTime2 = zone.GetAdjustmentRuleForTime(new DateTime(daylightTime.End.Year + 1, 1, 1), out ruleIndex3);
				if (adjustmentRuleForTime2 != null && adjustmentRuleForTime2.IsStartDateMarkerForBeginningOfYear())
				{
					if (adjustmentRuleForTime2.IsEndDateMarkerForEndOfYear())
					{
						dateTime2 = new DateTime(daylightTime.End.Year + 1, 12, 31) - utc - adjustmentRuleForTime2.BaseUtcOffsetDelta - adjustmentRuleForTime2.DaylightDelta;
					}
					else
					{
						dateTime2 = zone.GetDaylightTime(daylightTime.End.Year + 1, adjustmentRuleForTime2, ruleIndex3).End - utc - adjustmentRuleForTime2.BaseUtcOffsetDelta - adjustmentRuleForTime2.DaylightDelta;
					}
					ignoreYearAdjustment = true;
				}
				else
				{
					dateTime2 = new DateTime(daylightTime.End.Year + 1, 1, 1, 0, 0, 0).AddTicks(-1L) - daylightSavingsEndOffsetFromUtc;
				}
			}
			else
			{
				dateTime2 = daylightTime.End - daylightSavingsEndOffsetFromUtc;
			}
			DateTime t;
			DateTime t2;
			if (daylightTime.Delta.Ticks > 0L)
			{
				t = dateTime2 - daylightTime.Delta;
				t2 = dateTime2;
			}
			else
			{
				t = dateTime;
				t2 = dateTime - daylightTime.Delta;
			}
			bool flag = TimeZoneInfo.CheckIsDst(dateTime, time, dateTime2, ignoreYearAdjustment, rule);
			if (flag)
			{
				isAmbiguousLocalDst = (time >= t && time < t2);
				if (!isAmbiguousLocalDst && t.Year != t2.Year)
				{
					try
					{
						t.AddYears(1);
						t2.AddYears(1);
						isAmbiguousLocalDst = (time >= t && time < t2);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
					if (!isAmbiguousLocalDst)
					{
						try
						{
							t.AddYears(-1);
							t2.AddYears(-1);
							isAmbiguousLocalDst = (time >= t && time < t2);
						}
						catch (ArgumentOutOfRangeException)
						{
						}
					}
				}
			}
			return flag;
		}

		private static bool CheckIsDst(DateTime startTime, DateTime time, DateTime endTime, bool ignoreYearAdjustment, TimeZoneInfo.AdjustmentRule rule)
		{
			if (!ignoreYearAdjustment && !rule.NoDaylightTransitions)
			{
				int year = startTime.Year;
				int year2 = endTime.Year;
				if (year != year2)
				{
					endTime = endTime.AddYears(year - year2);
				}
				int year3 = time.Year;
				if (year != year3)
				{
					time = time.AddYears(year - year3);
				}
			}
			if (startTime > endTime)
			{
				return time < endTime || time >= startTime;
			}
			if (rule.NoDaylightTransitions)
			{
				return time >= startTime && time <= endTime;
			}
			return time >= startTime && time < endTime;
		}

		private static bool GetIsAmbiguousTime(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime)
		{
			bool flag = false;
			if (rule == null || rule.DaylightDelta == TimeSpan.Zero)
			{
				return flag;
			}
			DateTime t;
			DateTime t2;
			if (rule.DaylightDelta > TimeSpan.Zero)
			{
				if (rule.IsEndDateMarkerForEndOfYear())
				{
					return false;
				}
				t = daylightTime.End;
				t2 = daylightTime.End - rule.DaylightDelta;
			}
			else
			{
				if (rule.IsStartDateMarkerForBeginningOfYear())
				{
					return false;
				}
				t = daylightTime.Start;
				t2 = daylightTime.Start + rule.DaylightDelta;
			}
			flag = (time >= t2 && time < t);
			if (!flag && t.Year != t2.Year)
			{
				try
				{
					DateTime t3 = t.AddYears(1);
					DateTime t4 = t2.AddYears(1);
					flag = (time >= t4 && time < t3);
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				if (!flag)
				{
					try
					{
						DateTime t3 = t.AddYears(-1);
						DateTime t4 = t2.AddYears(-1);
						flag = (time >= t4 && time < t3);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
				}
			}
			return flag;
		}

		private static bool GetIsInvalidTime(DateTime time, TimeZoneInfo.AdjustmentRule rule, DaylightTimeStruct daylightTime)
		{
			bool flag = false;
			if (rule == null || rule.DaylightDelta == TimeSpan.Zero)
			{
				return flag;
			}
			DateTime t;
			DateTime t2;
			if (rule.DaylightDelta < TimeSpan.Zero)
			{
				if (rule.IsEndDateMarkerForEndOfYear())
				{
					return false;
				}
				t = daylightTime.End;
				t2 = daylightTime.End - rule.DaylightDelta;
			}
			else
			{
				if (rule.IsStartDateMarkerForBeginningOfYear())
				{
					return false;
				}
				t = daylightTime.Start;
				t2 = daylightTime.Start + rule.DaylightDelta;
			}
			flag = (time >= t && time < t2);
			if (!flag && t.Year != t2.Year)
			{
				try
				{
					DateTime t3 = t.AddYears(1);
					DateTime t4 = t2.AddYears(1);
					flag = (time >= t3 && time < t4);
				}
				catch (ArgumentOutOfRangeException)
				{
				}
				if (!flag)
				{
					try
					{
						DateTime t3 = t.AddYears(-1);
						DateTime t4 = t2.AddYears(-1);
						flag = (time >= t3 && time < t4);
					}
					catch (ArgumentOutOfRangeException)
					{
					}
				}
			}
			return flag;
		}

		private static TimeSpan GetUtcOffset(DateTime time, TimeZoneInfo zone, TimeZoneInfoOptions flags)
		{
			TimeSpan timeSpan = zone.BaseUtcOffset;
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(time, out ruleIndex);
			if (adjustmentRuleForTime != null)
			{
				timeSpan += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					DaylightTimeStruct daylightTime = zone.GetDaylightTime(time.Year, adjustmentRuleForTime, ruleIndex);
					bool isDaylightSavings = TimeZoneInfo.GetIsDaylightSavings(time, adjustmentRuleForTime, daylightTime, flags);
					timeSpan += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		private static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone)
		{
			bool flag;
			return TimeZoneInfo.GetUtcOffsetFromUtc(time, zone, out flag);
		}

		private static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone, out bool isDaylightSavings)
		{
			bool flag;
			return TimeZoneInfo.GetUtcOffsetFromUtc(time, zone, out isDaylightSavings, out flag);
		}

		internal static TimeSpan GetUtcOffsetFromUtc(DateTime time, TimeZoneInfo zone, out bool isDaylightSavings, out bool isAmbiguousLocalDst)
		{
			isDaylightSavings = false;
			isAmbiguousLocalDst = false;
			TimeSpan timeSpan = zone.BaseUtcOffset;
			int? ruleIndex;
			TimeZoneInfo.AdjustmentRule adjustmentRuleForTime;
			int year;
			if (time > TimeZoneInfo.s_maxDateOnly)
			{
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(DateTime.MaxValue, out ruleIndex);
				year = 9999;
			}
			else if (time < TimeZoneInfo.s_minDateOnly)
			{
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(DateTime.MinValue, out ruleIndex);
				year = 1;
			}
			else
			{
				adjustmentRuleForTime = zone.GetAdjustmentRuleForTime(time, true, out ruleIndex);
				year = (time + timeSpan).Year;
			}
			if (adjustmentRuleForTime != null)
			{
				timeSpan += adjustmentRuleForTime.BaseUtcOffsetDelta;
				if (adjustmentRuleForTime.HasDaylightSaving)
				{
					isDaylightSavings = TimeZoneInfo.GetIsDaylightSavingsFromUtc(time, year, zone._baseUtcOffset, adjustmentRuleForTime, ruleIndex, out isAmbiguousLocalDst, zone);
					timeSpan += (isDaylightSavings ? adjustmentRuleForTime.DaylightDelta : TimeSpan.Zero);
				}
			}
			return timeSpan;
		}

		internal static DateTime TransitionTimeToDateTime(int year, TimeZoneInfo.TransitionTime transitionTime)
		{
			DateTime timeOfDay = transitionTime.TimeOfDay;
			DateTime result;
			if (transitionTime.IsFixedDateRule)
			{
				int num = DateTime.DaysInMonth(year, transitionTime.Month);
				result = new DateTime(year, transitionTime.Month, (num < transitionTime.Day) ? num : transitionTime.Day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
			}
			else if (transitionTime.Week <= 4)
			{
				result = new DateTime(year, transitionTime.Month, 1, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
				int dayOfWeek = (int)result.DayOfWeek;
				int num2 = transitionTime.DayOfWeek - (DayOfWeek)dayOfWeek;
				if (num2 < 0)
				{
					num2 += 7;
				}
				num2 += 7 * (transitionTime.Week - 1);
				if (num2 > 0)
				{
					result = result.AddDays((double)num2);
				}
			}
			else
			{
				int day = DateTime.DaysInMonth(year, transitionTime.Month);
				result = new DateTime(year, transitionTime.Month, day, timeOfDay.Hour, timeOfDay.Minute, timeOfDay.Second, timeOfDay.Millisecond);
				int num3 = result.DayOfWeek - transitionTime.DayOfWeek;
				if (num3 < 0)
				{
					num3 += 7;
				}
				if (num3 > 0)
				{
					result = result.AddDays((double)(-(double)num3));
				}
			}
			return result;
		}

		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZone(string id, bool dstDisabled, out TimeZoneInfo value, out Exception e, TimeZoneInfo.CachedData cachedData, bool alwaysFallbackToLocalMachine = false)
		{
			TimeZoneInfo.TimeZoneInfoResult result = TimeZoneInfo.TimeZoneInfoResult.Success;
			e = null;
			TimeZoneInfo timeZoneInfo = null;
			if (cachedData._systemTimeZones != null && cachedData._systemTimeZones.TryGetValue(id, out timeZoneInfo))
			{
				if (dstDisabled && timeZoneInfo._supportsDaylightSavingTime)
				{
					value = TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo._id, timeZoneInfo._baseUtcOffset, timeZoneInfo._displayName, timeZoneInfo._standardDisplayName);
				}
				else
				{
					value = new TimeZoneInfo(timeZoneInfo._id, timeZoneInfo._baseUtcOffset, timeZoneInfo._displayName, timeZoneInfo._standardDisplayName, timeZoneInfo._daylightDisplayName, timeZoneInfo._adjustmentRules, false);
				}
				return result;
			}
			if (!cachedData._allSystemTimeZonesRead || alwaysFallbackToLocalMachine)
			{
				result = TimeZoneInfo.TryGetTimeZoneFromLocalMachine(id, dstDisabled, out value, out e, cachedData);
			}
			else
			{
				result = TimeZoneInfo.TimeZoneInfoResult.TimeZoneNotFoundException;
				value = null;
			}
			return result;
		}

		private static TimeZoneInfo.TimeZoneInfoResult TryGetTimeZoneFromLocalMachine(string id, bool dstDisabled, out TimeZoneInfo value, out Exception e, TimeZoneInfo.CachedData cachedData)
		{
			TimeZoneInfo timeZoneInfo;
			TimeZoneInfo.TimeZoneInfoResult timeZoneInfoResult = TimeZoneInfo.TryGetTimeZoneFromLocalMachine(id, out timeZoneInfo, out e);
			if (timeZoneInfoResult != TimeZoneInfo.TimeZoneInfoResult.Success)
			{
				value = null;
				return timeZoneInfoResult;
			}
			if (cachedData._systemTimeZones == null)
			{
				cachedData._systemTimeZones = new Dictionary<string, TimeZoneInfo>(StringComparer.OrdinalIgnoreCase);
			}
			if (!cachedData._systemTimeZones.ContainsKey(id))
			{
				cachedData._systemTimeZones.Add(id, timeZoneInfo);
			}
			if (dstDisabled && timeZoneInfo._supportsDaylightSavingTime)
			{
				value = TimeZoneInfo.CreateCustomTimeZone(timeZoneInfo._id, timeZoneInfo._baseUtcOffset, timeZoneInfo._displayName, timeZoneInfo._standardDisplayName);
				return timeZoneInfoResult;
			}
			value = new TimeZoneInfo(timeZoneInfo._id, timeZoneInfo._baseUtcOffset, timeZoneInfo._displayName, timeZoneInfo._standardDisplayName, timeZoneInfo._daylightDisplayName, timeZoneInfo._adjustmentRules, false);
			return timeZoneInfoResult;
		}

		private static void ValidateTimeZoneInfo(string id, TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule[] adjustmentRules, out bool adjustmentRulesSupportDst)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (id.Length == 0)
			{
				throw new ArgumentException(SR.Format("The specified ID parameter '{0}' is not supported.", id), "id");
			}
			if (TimeZoneInfo.UtcOffsetOutOfRange(baseUtcOffset))
			{
				throw new ArgumentOutOfRangeException("baseUtcOffset", "The TimeSpan parameter must be within plus or minus 14.0 hours.");
			}
			if (baseUtcOffset.Ticks % 600000000L != 0L)
			{
				throw new ArgumentException("The TimeSpan parameter cannot be specified more precisely than whole minutes.", "baseUtcOffset");
			}
			adjustmentRulesSupportDst = false;
			if (adjustmentRules != null && adjustmentRules.Length != 0)
			{
				adjustmentRulesSupportDst = true;
				TimeZoneInfo.AdjustmentRule adjustmentRule = null;
				for (int i = 0; i < adjustmentRules.Length; i++)
				{
					TimeZoneInfo.AdjustmentRule adjustmentRule2 = adjustmentRule;
					adjustmentRule = adjustmentRules[i];
					if (adjustmentRule == null)
					{
						throw new InvalidTimeZoneException("The AdjustmentRule array cannot contain null elements.");
					}
					if (!TimeZoneInfo.IsValidAdjustmentRuleOffest(baseUtcOffset, adjustmentRule))
					{
						throw new InvalidTimeZoneException("The sum of the BaseUtcOffset and DaylightDelta properties must within plus or minus 14.0 hours.");
					}
					if (adjustmentRule2 != null && adjustmentRule.DateStart <= adjustmentRule2.DateEnd)
					{
						throw new InvalidTimeZoneException("The elements of the AdjustmentRule array must be in chronological order and must not overlap.");
					}
				}
			}
		}

		internal static bool UtcOffsetOutOfRange(TimeSpan offset)
		{
			return offset < TimeZoneInfo.MinOffset || offset > TimeZoneInfo.MaxOffset;
		}

		private static TimeSpan GetUtcOffset(TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule adjustmentRule)
		{
			return baseUtcOffset + adjustmentRule.BaseUtcOffsetDelta + (adjustmentRule.HasDaylightSaving ? adjustmentRule.DaylightDelta : TimeSpan.Zero);
		}

		private static bool IsValidAdjustmentRuleOffest(TimeSpan baseUtcOffset, TimeZoneInfo.AdjustmentRule adjustmentRule)
		{
			return !TimeZoneInfo.UtcOffsetOutOfRange(TimeZoneInfo.GetUtcOffset(baseUtcOffset, adjustmentRule));
		}

		private static void NormalizeAdjustmentRuleOffset(TimeSpan baseUtcOffset, ref TimeZoneInfo.AdjustmentRule adjustmentRule)
		{
			TimeSpan utcOffset = TimeZoneInfo.GetUtcOffset(baseUtcOffset, adjustmentRule);
			TimeSpan timeSpan = TimeSpan.Zero;
			if (utcOffset > TimeZoneInfo.MaxOffset)
			{
				timeSpan = TimeZoneInfo.MaxOffset - utcOffset;
			}
			else if (utcOffset < TimeZoneInfo.MinOffset)
			{
				timeSpan = TimeZoneInfo.MinOffset - utcOffset;
			}
			if (timeSpan != TimeSpan.Zero)
			{
				adjustmentRule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(adjustmentRule.DateStart, adjustmentRule.DateEnd, adjustmentRule.DaylightDelta, adjustmentRule.DaylightTransitionStart, adjustmentRule.DaylightTransitionEnd, adjustmentRule.BaseUtcOffsetDelta + timeSpan, adjustmentRule.NoDaylightTransitions);
			}
		}

		internal TimeZoneInfo()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private const string TimeZonesRegistryHive = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones";

		private const string DisplayValue = "Display";

		private const string DaylightValue = "Dlt";

		private const string StandardValue = "Std";

		private const string MuiDisplayValue = "MUI_Display";

		private const string MuiDaylightValue = "MUI_Dlt";

		private const string MuiStandardValue = "MUI_Std";

		private const string TimeZoneInfoValue = "TZI";

		private const string FirstEntryValue = "FirstEntry";

		private const string LastEntryValue = "LastEntry";

		private const int MaxKeyLength = 255;

		private static Lazy<bool> lazyHaveRegistry = new Lazy<bool>(delegate()
		{
			bool result;
			try
			{
				using (Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\TimeZoneInformation", false))
				{
					result = true;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		});

		internal const uint TIME_ZONE_ID_INVALID = 4294967295U;

		internal const uint ERROR_NO_MORE_ITEMS = 259U;

		private readonly string _id;

		private readonly string _displayName;

		private readonly string _standardDisplayName;

		private readonly string _daylightDisplayName;

		private readonly TimeSpan _baseUtcOffset;

		private readonly bool _supportsDaylightSavingTime;

		private readonly TimeZoneInfo.AdjustmentRule[] _adjustmentRules;

		private const string UtcId = "UTC";

		private const string LocalId = "Local";

		private static readonly TimeZoneInfo s_utcTimeZone = TimeZoneInfo.CreateCustomTimeZone("UTC", TimeSpan.Zero, "UTC", "UTC");

		private static TimeZoneInfo.CachedData s_cachedData = new TimeZoneInfo.CachedData();

		private static readonly DateTime s_maxDateOnly = new DateTime(9999, 12, 31);

		private static readonly DateTime s_minDateOnly = new DateTime(1, 1, 2);

		private static readonly TimeSpan MaxOffset = TimeSpan.FromHours(14.0);

		private static readonly TimeSpan MinOffset = -TimeZoneInfo.MaxOffset;

		private sealed class CachedData
		{
			private static TimeZoneInfo GetCurrentOneYearLocal()
			{
				Interop.Kernel32.TIME_ZONE_INFORMATION time_ZONE_INFORMATION;
				if (Interop.Kernel32.GetTimeZoneInformation(out time_ZONE_INFORMATION) != 4294967295U)
				{
					return TimeZoneInfo.GetLocalTimeZoneFromWin32Data(time_ZONE_INFORMATION, false);
				}
				return TimeZoneInfo.CreateCustomTimeZone("Local", TimeSpan.Zero, "Local", "Local");
			}

			public TimeZoneInfo.OffsetAndRule GetOneYearLocalFromUtc(int year)
			{
				TimeZoneInfo.OffsetAndRule offsetAndRule = this._oneYearLocalFromUtc;
				if (offsetAndRule == null || offsetAndRule.Year != year)
				{
					TimeZoneInfo currentOneYearLocal = TimeZoneInfo.CachedData.GetCurrentOneYearLocal();
					TimeZoneInfo.AdjustmentRule rule = (currentOneYearLocal._adjustmentRules == null) ? null : currentOneYearLocal._adjustmentRules[0];
					offsetAndRule = new TimeZoneInfo.OffsetAndRule(year, currentOneYearLocal.BaseUtcOffset, rule);
					this._oneYearLocalFromUtc = offsetAndRule;
				}
				return offsetAndRule;
			}

			private TimeZoneInfo CreateLocal()
			{
				TimeZoneInfo result;
				lock (this)
				{
					TimeZoneInfo timeZoneInfo = this._localTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = TimeZoneInfo.GetLocalTimeZone(this);
						timeZoneInfo = new TimeZoneInfo(timeZoneInfo._id, timeZoneInfo._baseUtcOffset, timeZoneInfo._displayName, timeZoneInfo._standardDisplayName, timeZoneInfo._daylightDisplayName, timeZoneInfo._adjustmentRules, false);
						this._localTimeZone = timeZoneInfo;
					}
					result = timeZoneInfo;
				}
				return result;
			}

			public TimeZoneInfo Local
			{
				get
				{
					TimeZoneInfo timeZoneInfo = this._localTimeZone;
					if (timeZoneInfo == null)
					{
						timeZoneInfo = this.CreateLocal();
					}
					return timeZoneInfo;
				}
			}

			public DateTimeKind GetCorrespondingKind(TimeZoneInfo timeZone)
			{
				if (timeZone == TimeZoneInfo.s_utcTimeZone)
				{
					return DateTimeKind.Utc;
				}
				if (timeZone != this._localTimeZone)
				{
					return DateTimeKind.Unspecified;
				}
				return DateTimeKind.Local;
			}

			private volatile TimeZoneInfo.OffsetAndRule _oneYearLocalFromUtc;

			private volatile TimeZoneInfo _localTimeZone;

			public Dictionary<string, TimeZoneInfo> _systemTimeZones;

			public ReadOnlyCollection<TimeZoneInfo> _readOnlySystemTimeZones;

			public bool _allSystemTimeZonesRead;
		}

		private sealed class OffsetAndRule
		{
			public OffsetAndRule(int year, TimeSpan offset, TimeZoneInfo.AdjustmentRule rule)
			{
				this.Year = year;
				this.Offset = offset;
				this.Rule = rule;
			}

			public readonly int Year;

			public readonly TimeSpan Offset;

			public readonly TimeZoneInfo.AdjustmentRule Rule;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct DYNAMIC_TIME_ZONE_INFORMATION
		{
			internal Interop.Kernel32.TIME_ZONE_INFORMATION TZI;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			internal string TimeZoneKeyName;

			internal byte DynamicDaylightTimeDisabled;
		}

		private enum TimeZoneInfoResult
		{
			Success,
			TimeZoneNotFoundException,
			InvalidTimeZoneException,
			SecurityException
		}

		/// <summary>Provides information about a time zone adjustment, such as the transition to and from daylight saving time.</summary>
		[Serializable]
		public sealed class AdjustmentRule : IEquatable<TimeZoneInfo.AdjustmentRule>, ISerializable, IDeserializationCallback
		{
			/// <summary>Gets the date when the adjustment rule takes effect.</summary>
			/// <returns>A <see cref="T:System.DateTime" /> value that indicates when the adjustment rule takes effect.</returns>
			public DateTime DateStart
			{
				get
				{
					return this._dateStart;
				}
			}

			/// <summary>Gets the date when the adjustment rule ceases to be in effect.</summary>
			/// <returns>A <see cref="T:System.DateTime" /> value that indicates the end date of the adjustment rule.</returns>
			public DateTime DateEnd
			{
				get
				{
					return this._dateEnd;
				}
			}

			/// <summary>Gets the amount of time that is required to form the time zone's daylight saving time. This amount of time is added to the time zone's offset from Coordinated Universal Time (UTC).</summary>
			/// <returns>A <see cref="T:System.TimeSpan" /> object that indicates the amount of time to add to the standard time changes as a result of the adjustment rule.</returns>
			public TimeSpan DaylightDelta
			{
				get
				{
					return this._daylightDelta;
				}
			}

			/// <summary>Gets information about the annual transition from standard time to daylight saving time.</summary>
			/// <returns>A <see cref="T:System.TimeZoneInfo.TransitionTime" /> object that defines the annual transition from a time zone's standard time to daylight saving time.</returns>
			public TimeZoneInfo.TransitionTime DaylightTransitionStart
			{
				get
				{
					return this._daylightTransitionStart;
				}
			}

			/// <summary>Gets information about the annual transition from daylight saving time back to standard time.</summary>
			/// <returns>A <see cref="T:System.TimeZoneInfo.TransitionTime" /> object that defines the annual transition from daylight saving time back to the time zone's standard time.</returns>
			public TimeZoneInfo.TransitionTime DaylightTransitionEnd
			{
				get
				{
					return this._daylightTransitionEnd;
				}
			}

			internal TimeSpan BaseUtcOffsetDelta
			{
				get
				{
					return this._baseUtcOffsetDelta;
				}
			}

			internal bool NoDaylightTransitions
			{
				get
				{
					return this._noDaylightTransitions;
				}
			}

			internal bool HasDaylightSaving
			{
				get
				{
					return this.DaylightDelta != TimeSpan.Zero || (this.DaylightTransitionStart != default(TimeZoneInfo.TransitionTime) && this.DaylightTransitionStart.TimeOfDay != DateTime.MinValue) || (this.DaylightTransitionEnd != default(TimeZoneInfo.TransitionTime) && this.DaylightTransitionEnd.TimeOfDay != DateTime.MinValue.AddMilliseconds(1.0));
				}
			}

			/// <summary>Determines whether the current <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> object is equal to a second <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> object.</summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>
			///   <see langword="true" /> if both <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> objects have equal values; otherwise, <see langword="false" />.</returns>
			public bool Equals(TimeZoneInfo.AdjustmentRule other)
			{
				return other != null && this._dateStart == other._dateStart && this._dateEnd == other._dateEnd && this._daylightDelta == other._daylightDelta && this._baseUtcOffsetDelta == other._baseUtcOffsetDelta && this._daylightTransitionEnd.Equals(other._daylightTransitionEnd) && this._daylightTransitionStart.Equals(other._daylightTransitionStart);
			}

			/// <summary>Serves as a hash function for hashing algorithms and data structures such as hash tables.</summary>
			/// <returns>A 32-bit signed integer that serves as the hash code for the current <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> object.</returns>
			public override int GetHashCode()
			{
				return this._dateStart.GetHashCode();
			}

			private AdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd, TimeSpan baseUtcOffsetDelta, bool noDaylightTransitions)
			{
				TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd, noDaylightTransitions);
				this._dateStart = dateStart;
				this._dateEnd = dateEnd;
				this._daylightDelta = daylightDelta;
				this._daylightTransitionStart = daylightTransitionStart;
				this._daylightTransitionEnd = daylightTransitionEnd;
				this._baseUtcOffsetDelta = baseUtcOffsetDelta;
				this._noDaylightTransitions = noDaylightTransitions;
			}

			/// <summary>Creates a new adjustment rule for a particular time zone.</summary>
			/// <param name="dateStart">The effective date of the adjustment rule. If the value of the <paramref name="dateStart" /> parameter is <see langword="DateTime.MinValue.Date" />, this is the first adjustment rule in effect for a time zone.</param>
			/// <param name="dateEnd">The last date that the adjustment rule is in force. If the value of the <paramref name="dateEnd" /> parameter is <see langword="DateTime.MaxValue.Date" />, the adjustment rule has no end date.</param>
			/// <param name="daylightDelta">The time change that results from the adjustment. This value is added to the time zone's <see cref="P:System.TimeZoneInfo.BaseUtcOffset" /> property to obtain the correct daylight offset from Coordinated Universal Time (UTC). This value can range from -14 to 14.</param>
			/// <param name="daylightTransitionStart">An object that defines the start of daylight saving time.</param>
			/// <param name="daylightTransitionEnd">An object that defines the end of daylight saving time.</param>
			/// <returns>An object that represents the new adjustment rule.</returns>
			/// <exception cref="T:System.ArgumentException">The <see cref="P:System.DateTime.Kind" /> property of the <paramref name="dateStart" /> or <paramref name="dateEnd" /> parameter does not equal <see cref="F:System.DateTimeKind.Unspecified" />.  
			///  -or-  
			///  The <paramref name="daylightTransitionStart" /> parameter is equal to the <paramref name="daylightTransitionEnd" /> parameter.  
			///  -or-  
			///  The <paramref name="dateStart" /> or <paramref name="dateEnd" /> parameter includes a time of day value.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="dateEnd" /> is earlier than <paramref name="dateStart" />.  
			/// -or-  
			/// <paramref name="daylightDelta" /> is less than -14 or greater than 14.  
			/// -or-  
			/// The <see cref="P:System.TimeSpan.Milliseconds" /> property of the <paramref name="daylightDelta" /> parameter is not equal to 0.  
			/// -or-  
			/// The <see cref="P:System.TimeSpan.Ticks" /> property of the <paramref name="daylightDelta" /> parameter does not equal a whole number of seconds.</exception>
			public static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd)
			{
				return new TimeZoneInfo.AdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd, TimeSpan.Zero, false);
			}

			internal static TimeZoneInfo.AdjustmentRule CreateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd, TimeSpan baseUtcOffsetDelta, bool noDaylightTransitions)
			{
				return new TimeZoneInfo.AdjustmentRule(dateStart, dateEnd, daylightDelta, daylightTransitionStart, daylightTransitionEnd, baseUtcOffsetDelta, noDaylightTransitions);
			}

			internal bool IsStartDateMarkerForBeginningOfYear()
			{
				return !this.NoDaylightTransitions && this.DaylightTransitionStart.Month == 1 && this.DaylightTransitionStart.Day == 1 && this.DaylightTransitionStart.TimeOfDay.Hour == 0 && this.DaylightTransitionStart.TimeOfDay.Minute == 0 && this.DaylightTransitionStart.TimeOfDay.Second == 0 && this._dateStart.Year == this._dateEnd.Year;
			}

			internal bool IsEndDateMarkerForEndOfYear()
			{
				return !this.NoDaylightTransitions && this.DaylightTransitionEnd.Month == 1 && this.DaylightTransitionEnd.Day == 1 && this.DaylightTransitionEnd.TimeOfDay.Hour == 0 && this.DaylightTransitionEnd.TimeOfDay.Minute == 0 && this.DaylightTransitionEnd.TimeOfDay.Second == 0 && this._dateStart.Year == this._dateEnd.Year;
			}

			private static void ValidateAdjustmentRule(DateTime dateStart, DateTime dateEnd, TimeSpan daylightDelta, TimeZoneInfo.TransitionTime daylightTransitionStart, TimeZoneInfo.TransitionTime daylightTransitionEnd, bool noDaylightTransitions)
			{
				if (dateStart.Kind != DateTimeKind.Unspecified && dateStart.Kind != DateTimeKind.Utc)
				{
					throw new ArgumentException("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified or DateTimeKind.Utc.", "dateStart");
				}
				if (dateEnd.Kind != DateTimeKind.Unspecified && dateEnd.Kind != DateTimeKind.Utc)
				{
					throw new ArgumentException("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified or DateTimeKind.Utc.", "dateEnd");
				}
				if (daylightTransitionStart.Equals(daylightTransitionEnd) && !noDaylightTransitions)
				{
					throw new ArgumentException("The DaylightTransitionStart property must not equal the DaylightTransitionEnd property.", "daylightTransitionEnd");
				}
				if (dateStart > dateEnd)
				{
					throw new ArgumentException("The DateStart property must come before the DateEnd property.", "dateStart");
				}
				if (daylightDelta.TotalHours < -23.0 || daylightDelta.TotalHours > 14.0)
				{
					throw new ArgumentOutOfRangeException("daylightDelta", daylightDelta, "The TimeSpan parameter must be within plus or minus 14.0 hours.");
				}
				if (daylightDelta.Ticks % 600000000L != 0L)
				{
					throw new ArgumentException("The TimeSpan parameter cannot be specified more precisely than whole minutes.", "daylightDelta");
				}
				if (dateStart != DateTime.MinValue && dateStart.Kind == DateTimeKind.Unspecified && dateStart.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException("The supplied DateTime includes a TimeOfDay setting.   This is not supported.", "dateStart");
				}
				if (dateEnd != DateTime.MaxValue && dateEnd.Kind == DateTimeKind.Unspecified && dateEnd.TimeOfDay != TimeSpan.Zero)
				{
					throw new ArgumentException("The supplied DateTime includes a TimeOfDay setting.   This is not supported.", "dateEnd");
				}
			}

			/// <summary>Runs when the deserialization of a <see cref="T:System.TimeZoneInfo.AdjustmentRule" /> object is completed.</summary>
			/// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.AdjustmentRule.ValidateAdjustmentRule(this._dateStart, this._dateEnd, this._daylightDelta, this._daylightTransitionStart, this._daylightTransitionEnd, this._noDaylightTransitions);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
				}
			}

			/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data that is required to serialize this object.</summary>
			/// <param name="info">The object to populate with data.</param>
			/// <param name="context">The destination for this serialization (see <see cref="T:System.Runtime.Serialization.StreamingContext" />).</param>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("DateStart", this._dateStart);
				info.AddValue("DateEnd", this._dateEnd);
				info.AddValue("DaylightDelta", this._daylightDelta);
				info.AddValue("DaylightTransitionStart", this._daylightTransitionStart);
				info.AddValue("DaylightTransitionEnd", this._daylightTransitionEnd);
				info.AddValue("BaseUtcOffsetDelta", this._baseUtcOffsetDelta);
				info.AddValue("NoDaylightTransitions", this._noDaylightTransitions);
			}

			private AdjustmentRule(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this._dateStart = (DateTime)info.GetValue("DateStart", typeof(DateTime));
				this._dateEnd = (DateTime)info.GetValue("DateEnd", typeof(DateTime));
				this._daylightDelta = (TimeSpan)info.GetValue("DaylightDelta", typeof(TimeSpan));
				this._daylightTransitionStart = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionStart", typeof(TimeZoneInfo.TransitionTime));
				this._daylightTransitionEnd = (TimeZoneInfo.TransitionTime)info.GetValue("DaylightTransitionEnd", typeof(TimeZoneInfo.TransitionTime));
				object valueNoThrow = info.GetValueNoThrow("BaseUtcOffsetDelta", typeof(TimeSpan));
				if (valueNoThrow != null)
				{
					this._baseUtcOffsetDelta = (TimeSpan)valueNoThrow;
				}
				valueNoThrow = info.GetValueNoThrow("NoDaylightTransitions", typeof(bool));
				if (valueNoThrow != null)
				{
					this._noDaylightTransitions = (bool)valueNoThrow;
				}
			}

			internal AdjustmentRule()
			{
				ThrowStub.ThrowNotSupportedException();
			}

			private readonly DateTime _dateStart;

			private readonly DateTime _dateEnd;

			private readonly TimeSpan _daylightDelta;

			private readonly TimeZoneInfo.TransitionTime _daylightTransitionStart;

			private readonly TimeZoneInfo.TransitionTime _daylightTransitionEnd;

			private readonly TimeSpan _baseUtcOffsetDelta;

			private readonly bool _noDaylightTransitions;
		}

		private struct StringSerializer
		{
			public static string GetSerializedString(TimeZoneInfo zone)
			{
				StringBuilder stringBuilder = StringBuilderCache.Acquire(16);
				TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.Id, stringBuilder);
				stringBuilder.Append(';');
				stringBuilder.Append(zone.BaseUtcOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture));
				stringBuilder.Append(';');
				TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DisplayName, stringBuilder);
				stringBuilder.Append(';');
				TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.StandardName, stringBuilder);
				stringBuilder.Append(';');
				TimeZoneInfo.StringSerializer.SerializeSubstitute(zone.DaylightName, stringBuilder);
				stringBuilder.Append(';');
				foreach (TimeZoneInfo.AdjustmentRule adjustmentRule in zone.GetAdjustmentRules())
				{
					stringBuilder.Append('[');
					stringBuilder.Append(adjustmentRule.DateStart.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo));
					stringBuilder.Append(';');
					stringBuilder.Append(adjustmentRule.DateEnd.ToString("MM:dd:yyyy", DateTimeFormatInfo.InvariantInfo));
					stringBuilder.Append(';');
					stringBuilder.Append(adjustmentRule.DaylightDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture));
					stringBuilder.Append(';');
					TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionStart, stringBuilder);
					stringBuilder.Append(';');
					TimeZoneInfo.StringSerializer.SerializeTransitionTime(adjustmentRule.DaylightTransitionEnd, stringBuilder);
					stringBuilder.Append(';');
					if (adjustmentRule.BaseUtcOffsetDelta != TimeSpan.Zero)
					{
						stringBuilder.Append(adjustmentRule.BaseUtcOffsetDelta.TotalMinutes.ToString(CultureInfo.InvariantCulture));
						stringBuilder.Append(';');
					}
					if (adjustmentRule.NoDaylightTransitions)
					{
						stringBuilder.Append('1');
						stringBuilder.Append(';');
					}
					stringBuilder.Append(']');
				}
				stringBuilder.Append(';');
				return StringBuilderCache.GetStringAndRelease(stringBuilder);
			}

			public static TimeZoneInfo GetDeserializedTimeZoneInfo(string source)
			{
				TimeZoneInfo.StringSerializer stringSerializer = new TimeZoneInfo.StringSerializer(source);
				string nextStringValue = stringSerializer.GetNextStringValue();
				TimeSpan nextTimeSpanValue = stringSerializer.GetNextTimeSpanValue();
				string nextStringValue2 = stringSerializer.GetNextStringValue();
				string nextStringValue3 = stringSerializer.GetNextStringValue();
				string nextStringValue4 = stringSerializer.GetNextStringValue();
				TimeZoneInfo.AdjustmentRule[] nextAdjustmentRuleArrayValue = stringSerializer.GetNextAdjustmentRuleArrayValue();
				TimeZoneInfo result;
				try
				{
					result = new TimeZoneInfo(nextStringValue, nextTimeSpanValue, nextStringValue2, nextStringValue3, nextStringValue4, nextAdjustmentRuleArrayValue, false);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
				}
				catch (InvalidTimeZoneException innerException2)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException2);
				}
				return result;
			}

			private StringSerializer(string str)
			{
				this._serializedText = str;
				this._currentTokenStartIndex = 0;
				this._state = TimeZoneInfo.StringSerializer.State.StartOfToken;
			}

			private static void SerializeSubstitute(string text, StringBuilder serializedText)
			{
				foreach (char c in text)
				{
					if (c == '\\' || c == '[' || c == ']' || c == ';')
					{
						serializedText.Append('\\');
					}
					serializedText.Append(c);
				}
			}

			private static void SerializeTransitionTime(TimeZoneInfo.TransitionTime time, StringBuilder serializedText)
			{
				serializedText.Append('[');
				serializedText.Append(time.IsFixedDateRule ? '1' : '0');
				serializedText.Append(';');
				serializedText.Append(time.TimeOfDay.ToString("HH:mm:ss.FFF", DateTimeFormatInfo.InvariantInfo));
				serializedText.Append(';');
				serializedText.Append(time.Month.ToString(CultureInfo.InvariantCulture));
				serializedText.Append(';');
				if (time.IsFixedDateRule)
				{
					serializedText.Append(time.Day.ToString(CultureInfo.InvariantCulture));
					serializedText.Append(';');
				}
				else
				{
					serializedText.Append(time.Week.ToString(CultureInfo.InvariantCulture));
					serializedText.Append(';');
					serializedText.Append(((int)time.DayOfWeek).ToString(CultureInfo.InvariantCulture));
					serializedText.Append(';');
				}
				serializedText.Append(']');
			}

			private static void VerifyIsEscapableCharacter(char c)
			{
				if (c != '\\' && c != ';' && c != '[' && c != ']')
				{
					throw new SerializationException(SR.Format("The serialized data contained an invalid escape sequence '\\\\{0}'.", c));
				}
			}

			private void SkipVersionNextDataFields(int depth)
			{
				if (this._currentTokenStartIndex < 0 || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
				for (int i = this._currentTokenStartIndex; i < this._serializedText.Length; i++)
				{
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this._serializedText[i]);
						state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					}
					else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
					{
						char c = this._serializedText[i];
						if (c == '\0')
						{
							throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
						}
						switch (c)
						{
						case '[':
							depth++;
							break;
						case '\\':
							state = TimeZoneInfo.StringSerializer.State.Escaped;
							break;
						case ']':
							depth--;
							if (depth == 0)
							{
								this._currentTokenStartIndex = i + 1;
								if (this._currentTokenStartIndex >= this._serializedText.Length)
								{
									this._state = TimeZoneInfo.StringSerializer.State.EndOfLine;
									return;
								}
								this._state = TimeZoneInfo.StringSerializer.State.StartOfToken;
								return;
							}
							break;
						}
					}
				}
				throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
			}

			private string GetNextStringValue()
			{
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._currentTokenStartIndex < 0 || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				TimeZoneInfo.StringSerializer.State state = TimeZoneInfo.StringSerializer.State.NotEscaped;
				StringBuilder stringBuilder = StringBuilderCache.Acquire(64);
				for (int i = this._currentTokenStartIndex; i < this._serializedText.Length; i++)
				{
					if (state == TimeZoneInfo.StringSerializer.State.Escaped)
					{
						TimeZoneInfo.StringSerializer.VerifyIsEscapableCharacter(this._serializedText[i]);
						stringBuilder.Append(this._serializedText[i]);
						state = TimeZoneInfo.StringSerializer.State.NotEscaped;
					}
					else if (state == TimeZoneInfo.StringSerializer.State.NotEscaped)
					{
						char c = this._serializedText[i];
						if (c == '\0')
						{
							throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
						}
						if (c == ';')
						{
							this._currentTokenStartIndex = i + 1;
							if (this._currentTokenStartIndex >= this._serializedText.Length)
							{
								this._state = TimeZoneInfo.StringSerializer.State.EndOfLine;
							}
							else
							{
								this._state = TimeZoneInfo.StringSerializer.State.StartOfToken;
							}
							return StringBuilderCache.GetStringAndRelease(stringBuilder);
						}
						switch (c)
						{
						case '[':
							throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
						case '\\':
							state = TimeZoneInfo.StringSerializer.State.Escaped;
							break;
						case ']':
							throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
						default:
							stringBuilder.Append(this._serializedText[i]);
							break;
						}
					}
				}
				if (state == TimeZoneInfo.StringSerializer.State.Escaped)
				{
					throw new SerializationException(SR.Format("The serialized data contained an invalid escape sequence '\\\\{0}'.", string.Empty));
				}
				throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
			}

			private DateTime GetNextDateTimeValue(string format)
			{
				DateTime result;
				if (!DateTime.TryParseExact(this.GetNextStringValue(), format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out result))
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				return result;
			}

			private TimeSpan GetNextTimeSpanValue()
			{
				int nextInt32Value = this.GetNextInt32Value();
				TimeSpan result;
				try
				{
					result = new TimeSpan(0, nextInt32Value, 0);
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
				}
				return result;
			}

			private int GetNextInt32Value()
			{
				int result;
				if (!int.TryParse(this.GetNextStringValue(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result))
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				return result;
			}

			private TimeZoneInfo.AdjustmentRule[] GetNextAdjustmentRuleArrayValue()
			{
				List<TimeZoneInfo.AdjustmentRule> list = new List<TimeZoneInfo.AdjustmentRule>(1);
				int num = 0;
				for (TimeZoneInfo.AdjustmentRule nextAdjustmentRuleValue = this.GetNextAdjustmentRuleValue(); nextAdjustmentRuleValue != null; nextAdjustmentRuleValue = this.GetNextAdjustmentRuleValue())
				{
					list.Add(nextAdjustmentRuleValue);
					num++;
				}
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._currentTokenStartIndex < 0 || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (num == 0)
				{
					return null;
				}
				return list.ToArray();
			}

			private TimeZoneInfo.AdjustmentRule GetNextAdjustmentRuleValue()
			{
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine)
				{
					return null;
				}
				if (this._currentTokenStartIndex < 0 || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._serializedText[this._currentTokenStartIndex] == ';')
				{
					return null;
				}
				if (this._serializedText[this._currentTokenStartIndex] != '[')
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				this._currentTokenStartIndex++;
				DateTime nextDateTimeValue = this.GetNextDateTimeValue("MM:dd:yyyy");
				DateTime nextDateTimeValue2 = this.GetNextDateTimeValue("MM:dd:yyyy");
				TimeSpan nextTimeSpanValue = this.GetNextTimeSpanValue();
				TimeZoneInfo.TransitionTime nextTransitionTimeValue = this.GetNextTransitionTimeValue();
				TimeZoneInfo.TransitionTime nextTransitionTimeValue2 = this.GetNextTransitionTimeValue();
				TimeSpan baseUtcOffsetDelta = TimeSpan.Zero;
				int num = 0;
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if ((this._serializedText[this._currentTokenStartIndex] >= '0' && this._serializedText[this._currentTokenStartIndex] <= '9') || this._serializedText[this._currentTokenStartIndex] == '-' || this._serializedText[this._currentTokenStartIndex] == '+')
				{
					baseUtcOffsetDelta = this.GetNextTimeSpanValue();
				}
				if (this._serializedText[this._currentTokenStartIndex] >= '0' && this._serializedText[this._currentTokenStartIndex] <= '1')
				{
					num = this.GetNextInt32Value();
				}
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._serializedText[this._currentTokenStartIndex] != ']')
				{
					this.SkipVersionNextDataFields(1);
				}
				else
				{
					this._currentTokenStartIndex++;
				}
				TimeZoneInfo.AdjustmentRule result;
				try
				{
					result = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(nextDateTimeValue, nextDateTimeValue2, nextTimeSpanValue, nextTransitionTimeValue, nextTransitionTimeValue2, baseUtcOffsetDelta, num > 0);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
				}
				if (this._currentTokenStartIndex >= this._serializedText.Length)
				{
					this._state = TimeZoneInfo.StringSerializer.State.EndOfLine;
				}
				else
				{
					this._state = TimeZoneInfo.StringSerializer.State.StartOfToken;
				}
				return result;
			}

			private TimeZoneInfo.TransitionTime GetNextTransitionTimeValue()
			{
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine || (this._currentTokenStartIndex < this._serializedText.Length && this._serializedText[this._currentTokenStartIndex] == ']'))
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._currentTokenStartIndex < 0 || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._serializedText[this._currentTokenStartIndex] != '[')
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				this._currentTokenStartIndex++;
				int nextInt32Value = this.GetNextInt32Value();
				if (nextInt32Value != 0 && nextInt32Value != 1)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				DateTime nextDateTimeValue = this.GetNextDateTimeValue("HH:mm:ss.FFF");
				nextDateTimeValue = new DateTime(1, 1, 1, nextDateTimeValue.Hour, nextDateTimeValue.Minute, nextDateTimeValue.Second, nextDateTimeValue.Millisecond);
				int nextInt32Value2 = this.GetNextInt32Value();
				TimeZoneInfo.TransitionTime result;
				if (nextInt32Value == 1)
				{
					int nextInt32Value3 = this.GetNextInt32Value();
					try
					{
						result = TimeZoneInfo.TransitionTime.CreateFixedDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value3);
						goto IL_137;
					}
					catch (ArgumentException innerException)
					{
						throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
					}
				}
				int nextInt32Value4 = this.GetNextInt32Value();
				int nextInt32Value5 = this.GetNextInt32Value();
				try
				{
					result = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(nextDateTimeValue, nextInt32Value2, nextInt32Value4, (DayOfWeek)nextInt32Value5);
				}
				catch (ArgumentException innerException2)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException2);
				}
				IL_137:
				if (this._state == TimeZoneInfo.StringSerializer.State.EndOfLine || this._currentTokenStartIndex >= this._serializedText.Length)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._serializedText[this._currentTokenStartIndex] != ']')
				{
					this.SkipVersionNextDataFields(1);
				}
				else
				{
					this._currentTokenStartIndex++;
				}
				bool flag = false;
				if (this._currentTokenStartIndex < this._serializedText.Length && this._serializedText[this._currentTokenStartIndex] == ';')
				{
					this._currentTokenStartIndex++;
					flag = true;
				}
				if (!flag)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.");
				}
				if (this._currentTokenStartIndex >= this._serializedText.Length)
				{
					this._state = TimeZoneInfo.StringSerializer.State.EndOfLine;
				}
				else
				{
					this._state = TimeZoneInfo.StringSerializer.State.StartOfToken;
				}
				return result;
			}

			private readonly string _serializedText;

			private int _currentTokenStartIndex;

			private TimeZoneInfo.StringSerializer.State _state;

			private const int InitialCapacityForString = 64;

			private const char Esc = '\\';

			private const char Sep = ';';

			private const char Lhs = '[';

			private const char Rhs = ']';

			private const string DateTimeFormat = "MM:dd:yyyy";

			private const string TimeOfDayFormat = "HH:mm:ss.FFF";

			private enum State
			{
				Escaped,
				NotEscaped,
				StartOfToken,
				EndOfLine
			}
		}

		/// <summary>Provides information about a specific time change, such as the change from daylight saving time to standard time or vice versa, in a particular time zone.</summary>
		[Serializable]
		public readonly struct TransitionTime : IEquatable<TimeZoneInfo.TransitionTime>, ISerializable, IDeserializationCallback
		{
			/// <summary>Gets the hour, minute, and second at which the time change occurs.</summary>
			/// <returns>The time of day at which the time change occurs.</returns>
			public DateTime TimeOfDay
			{
				get
				{
					return this._timeOfDay;
				}
			}

			/// <summary>Gets the month in which the time change occurs.</summary>
			/// <returns>The month in which the time change occurs.</returns>
			public int Month
			{
				get
				{
					return (int)this._month;
				}
			}

			/// <summary>Gets the week of the month in which a time change occurs.</summary>
			/// <returns>The week of the month in which the time change occurs.</returns>
			public int Week
			{
				get
				{
					return (int)this._week;
				}
			}

			/// <summary>Gets the day on which the time change occurs.</summary>
			/// <returns>The day on which the time change occurs.</returns>
			public int Day
			{
				get
				{
					return (int)this._day;
				}
			}

			/// <summary>Gets the day of the week on which the time change occurs.</summary>
			/// <returns>The day of the week on which the time change occurs.</returns>
			public DayOfWeek DayOfWeek
			{
				get
				{
					return this._dayOfWeek;
				}
			}

			/// <summary>Gets a value indicating whether the time change occurs at a fixed date and time (such as November 1) or a floating date and time (such as the last Sunday of October).</summary>
			/// <returns>
			///   <see langword="true" /> if the time change rule is fixed-date; <see langword="false" /> if the time change rule is floating-date.</returns>
			public bool IsFixedDateRule
			{
				get
				{
					return this._isFixedDateRule;
				}
			}

			/// <summary>Determines whether an object has identical values to the current <see cref="T:System.TimeZoneInfo.TransitionTime" /> object.</summary>
			/// <param name="obj">An object to compare with the current <see cref="T:System.TimeZoneInfo.TransitionTime" /> object.</param>
			/// <returns>
			///   <see langword="true" /> if the two objects are equal; otherwise, <see langword="false" />.</returns>
			public override bool Equals(object obj)
			{
				return obj is TimeZoneInfo.TransitionTime && this.Equals((TimeZoneInfo.TransitionTime)obj);
			}

			/// <summary>Determines whether two specified <see cref="T:System.TimeZoneInfo.TransitionTime" /> objects are equal.</summary>
			/// <param name="t1">The first object to compare.</param>
			/// <param name="t2">The second object to compare.</param>
			/// <returns>
			///   <see langword="true" /> if <paramref name="t1" /> and <paramref name="t2" /> have identical values; otherwise, <see langword="false" />.</returns>
			public static bool operator ==(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return t1.Equals(t2);
			}

			/// <summary>Determines whether two specified <see cref="T:System.TimeZoneInfo.TransitionTime" /> objects are not equal.</summary>
			/// <param name="t1">The first object to compare.</param>
			/// <param name="t2">The second object to compare.</param>
			/// <returns>
			///   <see langword="true" /> if <paramref name="t1" /> and <paramref name="t2" /> have any different member values; otherwise, <see langword="false" />.</returns>
			public static bool operator !=(TimeZoneInfo.TransitionTime t1, TimeZoneInfo.TransitionTime t2)
			{
				return !t1.Equals(t2);
			}

			/// <summary>Determines whether the current <see cref="T:System.TimeZoneInfo.TransitionTime" /> object has identical values to a second <see cref="T:System.TimeZoneInfo.TransitionTime" /> object.</summary>
			/// <param name="other">An object to compare to the current instance.</param>
			/// <returns>
			///   <see langword="true" /> if the two objects have identical property values; otherwise, <see langword="false" />.</returns>
			public bool Equals(TimeZoneInfo.TransitionTime other)
			{
				if (this._isFixedDateRule != other._isFixedDateRule || !(this._timeOfDay == other._timeOfDay) || this._month != other._month)
				{
					return false;
				}
				if (!other._isFixedDateRule)
				{
					return this._week == other._week && this._dayOfWeek == other._dayOfWeek;
				}
				return this._day == other._day;
			}

			/// <summary>Serves as a hash function for hashing algorithms and data structures such as hash tables.</summary>
			/// <returns>A 32-bit signed integer that serves as the hash code for this <see cref="T:System.TimeZoneInfo.TransitionTime" /> object.</returns>
			public override int GetHashCode()
			{
				return (int)this._month ^ (int)this._week << 8;
			}

			private TransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek, bool isFixedDateRule)
			{
				TimeZoneInfo.TransitionTime.ValidateTransitionTime(timeOfDay, month, week, day, dayOfWeek);
				this._timeOfDay = timeOfDay;
				this._month = (byte)month;
				this._week = (byte)week;
				this._day = (byte)day;
				this._dayOfWeek = dayOfWeek;
				this._isFixedDateRule = isFixedDateRule;
			}

			/// <summary>Defines a time change that uses a fixed-date rule (that is, a time change that occurs on a specific day of a specific month).</summary>
			/// <param name="timeOfDay">The time at which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.TimeOfDay" /> property.</param>
			/// <param name="month">The month in which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.Month" /> property.</param>
			/// <param name="day">The day of the month on which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.Day" /> property.</param>
			/// <returns>Data about the time change.</returns>
			/// <exception cref="T:System.ArgumentException">The <paramref name="timeOfDay" /> parameter has a non-default date component.  
			///  -or-  
			///  The <paramref name="timeOfDay" /> parameter's <see cref="P:System.DateTime.Kind" /> property is not <see cref="F:System.DateTimeKind.Unspecified" />.  
			///  -or-  
			///  The <paramref name="timeOfDay" /> parameter does not represent a whole number of milliseconds.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="month" /> parameter is less than 1 or greater than 12.  
			///  -or-  
			///  The <paramref name="day" /> parameter is less than 1 or greater than 31.</exception>
			public static TimeZoneInfo.TransitionTime CreateFixedDateRule(DateTime timeOfDay, int month, int day)
			{
				return new TimeZoneInfo.TransitionTime(timeOfDay, month, 1, day, DayOfWeek.Sunday, true);
			}

			/// <summary>Defines a time change that uses a floating-date rule (that is, a time change that occurs on a specific day of a specific week of a specific month).</summary>
			/// <param name="timeOfDay">The time at which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.TimeOfDay" /> property.</param>
			/// <param name="month">The month in which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.Month" /> property.</param>
			/// <param name="week">The week of the month in which the time change occurs. Its value can range from 1 to 5, with 5 representing the last week of the month. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.Week" /> property.</param>
			/// <param name="dayOfWeek">The day of the week on which the time change occurs. This parameter corresponds to the <see cref="P:System.TimeZoneInfo.TransitionTime.DayOfWeek" /> property.</param>
			/// <returns>Data about the time change.</returns>
			/// <exception cref="T:System.ArgumentException">The <paramref name="timeOfDay" /> parameter has a non-default date component.  
			///  -or-  
			///  The <paramref name="timeOfDay" /> parameter does not represent a whole number of milliseconds.  
			///  -or-  
			///  The <paramref name="timeOfDay" /> parameter's <see cref="P:System.DateTime.Kind" /> property is not <see cref="F:System.DateTimeKind.Unspecified" />.</exception>
			/// <exception cref="T:System.ArgumentOutOfRangeException">
			///   <paramref name="month" /> is less than 1 or greater than 12.  
			/// -or-  
			/// <paramref name="week" /> is less than 1 or greater than 5.  
			/// -or-  
			/// The <paramref name="dayOfWeek" /> parameter is not a member of the <see cref="T:System.DayOfWeek" /> enumeration.</exception>
			public static TimeZoneInfo.TransitionTime CreateFloatingDateRule(DateTime timeOfDay, int month, int week, DayOfWeek dayOfWeek)
			{
				return new TimeZoneInfo.TransitionTime(timeOfDay, month, week, 1, dayOfWeek, false);
			}

			private static void ValidateTransitionTime(DateTime timeOfDay, int month, int week, int day, DayOfWeek dayOfWeek)
			{
				if (timeOfDay.Kind != DateTimeKind.Unspecified)
				{
					throw new ArgumentException("The supplied DateTime must have the Kind property set to DateTimeKind.Unspecified.", "timeOfDay");
				}
				if (month < 1 || month > 12)
				{
					throw new ArgumentOutOfRangeException("month", "The Month parameter must be in the range 1 through 12.");
				}
				if (day < 1 || day > 31)
				{
					throw new ArgumentOutOfRangeException("day", "The Day parameter must be in the range 1 through 31.");
				}
				if (week < 1 || week > 5)
				{
					throw new ArgumentOutOfRangeException("week", "The Week parameter must be in the range 1 through 5.");
				}
				if (dayOfWeek < DayOfWeek.Sunday || dayOfWeek > DayOfWeek.Saturday)
				{
					throw new ArgumentOutOfRangeException("dayOfWeek", "The DayOfWeek enumeration must be in the range 0 through 6.");
				}
				int num;
				int num2;
				int num3;
				timeOfDay.GetDatePart(out num, out num2, out num3);
				if (num != 1 || num2 != 1 || num3 != 1 || timeOfDay.Ticks % 10000L != 0L)
				{
					throw new ArgumentException("The supplied DateTime must have the Year, Month, and Day properties set to 1.  The time cannot be specified more precisely than whole milliseconds.", "timeOfDay");
				}
			}

			/// <summary>Runs when the deserialization of an object has been completed.</summary>
			/// <param name="sender">The object that initiated the callback. The functionality for this parameter is not currently implemented.</param>
			void IDeserializationCallback.OnDeserialization(object sender)
			{
				try
				{
					TimeZoneInfo.TransitionTime.ValidateTransitionTime(this._timeOfDay, (int)this._month, (int)this._week, (int)this._day, this._dayOfWeek);
				}
				catch (ArgumentException innerException)
				{
					throw new SerializationException("An error occurred while deserializing the object.  The serialized data is corrupt.", innerException);
				}
			}

			/// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> object with the data that is required to serialize this object.</summary>
			/// <param name="info">The object to populate with data.</param>
			/// <param name="context">The destination for this serialization (see <see cref="T:System.Runtime.Serialization.StreamingContext" />).</param>
			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("TimeOfDay", this._timeOfDay);
				info.AddValue("Month", this._month);
				info.AddValue("Week", this._week);
				info.AddValue("Day", this._day);
				info.AddValue("DayOfWeek", this._dayOfWeek);
				info.AddValue("IsFixedDateRule", this._isFixedDateRule);
			}

			private TransitionTime(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				this._timeOfDay = (DateTime)info.GetValue("TimeOfDay", typeof(DateTime));
				this._month = (byte)info.GetValue("Month", typeof(byte));
				this._week = (byte)info.GetValue("Week", typeof(byte));
				this._day = (byte)info.GetValue("Day", typeof(byte));
				this._dayOfWeek = (DayOfWeek)info.GetValue("DayOfWeek", typeof(DayOfWeek));
				this._isFixedDateRule = (bool)info.GetValue("IsFixedDateRule", typeof(bool));
			}

			private readonly DateTime _timeOfDay;

			private readonly byte _month;

			private readonly byte _week;

			private readonly byte _day;

			private readonly DayOfWeek _dayOfWeek;

			private readonly bool _isFixedDateRule;
		}
	}
}
