using System;
using System.Text;

namespace ICSharpCode.SharpZipLib.GZip
{
	public sealed class GZipConstants
	{
		public static Encoding Encoding
		{
			get
			{
				Encoding result;
				try
				{
					result = Encoding.GetEncoding(1252);
				}
				catch
				{
					result = Encoding.ASCII;
				}
				return result;
			}
		}

		public const byte ID1 = 31;

		public const byte ID2 = 139;

		public const byte CompressionMethodDeflate = 8;
	}
}
