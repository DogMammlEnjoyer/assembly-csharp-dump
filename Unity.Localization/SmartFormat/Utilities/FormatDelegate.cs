using System;

namespace UnityEngine.Localization.SmartFormat.Utilities
{
	public class FormatDelegate : IFormattable
	{
		public FormatDelegate(Func<string, string> getFormat)
		{
			this.getFormat1 = getFormat;
		}

		public FormatDelegate(Func<string, IFormatProvider, string> getFormat)
		{
			this.getFormat2 = getFormat;
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (this.getFormat1 == null)
			{
				return this.getFormat2(format, formatProvider);
			}
			return this.getFormat1(format);
		}

		private readonly Func<string, string> getFormat1;

		private readonly Func<string, IFormatProvider, string> getFormat2;
	}
}
