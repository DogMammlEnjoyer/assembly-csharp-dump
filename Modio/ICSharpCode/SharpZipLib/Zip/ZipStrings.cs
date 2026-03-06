using System;
using System.Text;
using ICSharpCode.SharpZipLib.Core;

namespace ICSharpCode.SharpZipLib.Zip
{
	public static class ZipStrings
	{
		static ZipStrings()
		{
			try
			{
				int num = Encoding.GetEncoding(0).CodePage;
				ZipStrings.SystemDefaultCodePage = ((num == 1 || num == 2 || num == 3 || num == 42) ? 437 : num);
			}
			catch
			{
				ZipStrings.SystemDefaultCodePage = 437;
			}
		}

		public static int CodePage
		{
			get
			{
				if (ZipStrings.codePage != -1)
				{
					return ZipStrings.codePage;
				}
				return Encoding.UTF8.CodePage;
			}
			set
			{
				if (value < 0 || value > 65535 || value == 1 || value == 2 || value == 3 || value == 42)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				ZipStrings.codePage = value;
			}
		}

		public static int SystemDefaultCodePage { get; }

		public static bool UseUnicode
		{
			get
			{
				return ZipStrings.codePage == Encoding.UTF8.CodePage;
			}
			set
			{
				if (value)
				{
					ZipStrings.codePage = Encoding.UTF8.CodePage;
					return;
				}
				ZipStrings.codePage = ZipStrings.SystemDefaultCodePage;
			}
		}

		public static string ConvertToString(byte[] data, int count)
		{
			if (data != null)
			{
				return Encoding.GetEncoding(ZipStrings.CodePage).GetString(data, 0, count);
			}
			return string.Empty;
		}

		public static string ConvertToString(byte[] data)
		{
			return ZipStrings.ConvertToString(data, data.Length);
		}

		private static Encoding EncodingFromFlag(int flags)
		{
			if ((flags & 2048) == 0)
			{
				return Encoding.GetEncoding((ZipStrings.codePage == -1) ? ZipStrings.SystemDefaultCodePage : ZipStrings.codePage);
			}
			return Encoding.UTF8;
		}

		public static string ConvertToStringExt(int flags, byte[] data, int count)
		{
			if (data != null)
			{
				return ZipStrings.EncodingFromFlag(flags).GetString(data, 0, count);
			}
			return string.Empty;
		}

		public static string ConvertToStringExt(int flags, byte[] data)
		{
			return ZipStrings.ConvertToStringExt(flags, data, data.Length);
		}

		public static byte[] ConvertToArray(string str)
		{
			if (str != null)
			{
				return Encoding.GetEncoding(ZipStrings.CodePage).GetBytes(str);
			}
			return Empty.Array<byte>();
		}

		public static byte[] ConvertToArray(int flags, string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				return ZipStrings.EncodingFromFlag(flags).GetBytes(str);
			}
			return Empty.Array<byte>();
		}

		private static int codePage = -1;

		private const int AutomaticCodePage = -1;

		private const int FallbackCodePage = 437;
	}
}
