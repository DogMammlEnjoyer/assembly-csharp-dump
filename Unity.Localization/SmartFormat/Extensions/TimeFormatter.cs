using System;
using System.Globalization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;
using UnityEngine.Localization.SmartFormat.Net.Utilities;
using UnityEngine.Localization.SmartFormat.Utilities;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Serializable]
	public class TimeFormatter : FormatterBase
	{
		public TimeFormatter()
		{
			base.Names = this.DefaultNames;
		}

		public override string[] DefaultNames
		{
			get
			{
				return new string[]
				{
					"timespan",
					"time",
					"t",
					""
				};
			}
		}

		public TimeSpanFormatOptions DefaultFormatOptions
		{
			get
			{
				return this.m_DefaultFormatOptions;
			}
			set
			{
				this.m_DefaultFormatOptions = value;
			}
		}

		public string DefaultTwoLetterISOLanguageName
		{
			get
			{
				return this.m_DefaultTwoLetterIsoLanguageName;
			}
			set
			{
				if (CommonLanguagesTimeTextInfo.GetTimeTextInfo(value) == null)
				{
					throw new ArgumentException("Language '" + value + "' for value is not implemented.");
				}
				this.m_DefaultTwoLetterIsoLanguageName = value;
			}
		}

		public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
		{
			Format format = formattingInfo.Format;
			object currentValue = formattingInfo.CurrentValue;
			if (format != null && format.HasNested)
			{
				return false;
			}
			string formatOptionsString;
			if (!string.IsNullOrEmpty(formattingInfo.FormatterOptions))
			{
				formatOptionsString = formattingInfo.FormatterOptions;
			}
			else if (format != null)
			{
				formatOptionsString = format.GetLiteralText();
			}
			else
			{
				formatOptionsString = string.Empty;
			}
			TimeSpan fromTime;
			if (currentValue is TimeSpan)
			{
				TimeSpan timeSpan = (TimeSpan)currentValue;
				fromTime = timeSpan;
			}
			else if (currentValue is DateTime)
			{
				DateTime dateTime = (DateTime)currentValue;
				if (!(formattingInfo.FormatterOptions != string.Empty))
				{
					return false;
				}
				fromTime = SystemTime.Now().ToUniversalTime().Subtract(dateTime.ToUniversalTime());
			}
			else
			{
				if (!(currentValue is DateTimeOffset))
				{
					return false;
				}
				DateTimeOffset dateTimeOffset = (DateTimeOffset)currentValue;
				if (!(formattingInfo.FormatterOptions != string.Empty))
				{
					return false;
				}
				fromTime = SystemTime.OffsetNow().UtcDateTime.Subtract(dateTimeOffset.UtcDateTime);
			}
			TimeTextInfo timeTextInfo = this.GetTimeTextInfo(formattingInfo.FormatDetails.Provider);
			if (timeTextInfo == null)
			{
				return false;
			}
			TimeSpanFormatOptions options = TimeSpanFormatOptionsConverter.Parse(formatOptionsString);
			string text = fromTime.ToTimeString(options, timeTextInfo);
			formattingInfo.Write(text);
			return true;
		}

		private TimeTextInfo GetTimeTextInfo(IFormatProvider provider)
		{
			if (provider == null)
			{
				return CommonLanguagesTimeTextInfo.GetTimeTextInfo(this.DefaultTwoLetterISOLanguageName);
			}
			TimeTextInfo timeTextInfo = provider.GetFormat(typeof(TimeTextInfo)) as TimeTextInfo;
			if (timeTextInfo != null)
			{
				return timeTextInfo;
			}
			CultureInfo cultureInfo = provider as CultureInfo;
			if (cultureInfo == null)
			{
				return CommonLanguagesTimeTextInfo.GetTimeTextInfo(this.DefaultTwoLetterISOLanguageName);
			}
			return CommonLanguagesTimeTextInfo.GetTimeTextInfo(cultureInfo.TwoLetterISOLanguageName);
		}

		[SerializeField]
		private TimeSpanFormatOptions m_DefaultFormatOptions = TimeSpanUtility.DefaultFormatOptions;

		private string m_DefaultTwoLetterIsoLanguageName = "en";
	}
}
