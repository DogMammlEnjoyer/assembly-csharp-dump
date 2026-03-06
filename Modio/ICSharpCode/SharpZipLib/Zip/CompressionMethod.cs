using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public enum CompressionMethod
	{
		Stored,
		Deflated = 8,
		Deflate64,
		BZip2 = 12,
		LZMA = 14,
		PPMd = 98,
		WinZipAES
	}
}
