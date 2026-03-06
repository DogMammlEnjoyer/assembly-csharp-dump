using System;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	[Flags]
	public enum TimeSpanFormatOptions
	{
		InheritDefaults = 0,
		Abbreviate = 1,
		AbbreviateOff = 2,
		LessThan = 4,
		LessThanOff = 8,
		TruncateShortest = 16,
		TruncateAuto = 32,
		TruncateFill = 64,
		TruncateFull = 128,
		RangeMilliSeconds = 256,
		RangeSeconds = 512,
		RangeMinutes = 1024,
		RangeHours = 2048,
		RangeDays = 4096,
		RangeWeeks = 8192
	}
}
