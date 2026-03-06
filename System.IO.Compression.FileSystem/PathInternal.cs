using System;

namespace System.IO
{
	internal static class PathInternal
	{
		internal static StringComparison StringComparison
		{
			get
			{
				if (!PathInternal.s_isCaseSensitive)
				{
					return StringComparison.OrdinalIgnoreCase;
				}
				return StringComparison.Ordinal;
			}
		}

		internal static bool IsCaseSensitive
		{
			get
			{
				return PathInternal.s_isCaseSensitive;
			}
		}

		private static bool GetIsCaseSensitive()
		{
			bool result;
			try
			{
				string text = Path.Combine(Path.GetTempPath(), "CASESENSITIVETEST" + Guid.NewGuid().ToString("N"));
				using (new FileStream(text, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, 4096, FileOptions.DeleteOnClose))
				{
					result = !File.Exists(text.ToLowerInvariant());
				}
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private static readonly bool s_isCaseSensitive = PathInternal.GetIsCaseSensitive();
	}
}
