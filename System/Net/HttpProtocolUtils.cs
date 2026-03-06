using System;
using System.Globalization;

namespace System.Net
{
	internal class HttpProtocolUtils
	{
		private HttpProtocolUtils()
		{
		}

		internal static DateTime string2date(string S)
		{
			DateTime result;
			if (HttpDateParse.ParseHttpDate(S, out result))
			{
				return result;
			}
			throw new ProtocolViolationException(SR.GetString("The value of the date string in the header is invalid."));
		}

		internal static string date2string(DateTime D)
		{
			DateTimeFormatInfo provider = new DateTimeFormatInfo();
			return D.ToUniversalTime().ToString("R", provider);
		}
	}
}
