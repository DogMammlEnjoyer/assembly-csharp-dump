using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	public static class ZipEntryExtensions
	{
		public static bool HasFlag(this ZipEntry entry, GeneralBitFlags flag)
		{
			return (entry.Flags & (int)flag) != 0;
		}

		public static void SetFlag(this ZipEntry entry, GeneralBitFlags flag, bool enabled = true)
		{
			entry.Flags = (enabled ? (entry.Flags | (int)flag) : (entry.Flags & (int)(~(int)flag)));
		}
	}
}
