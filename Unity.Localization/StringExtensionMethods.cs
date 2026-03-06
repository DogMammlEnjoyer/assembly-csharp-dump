using System;
using System.Text.RegularExpressions;

namespace UnityEngine.Localization
{
	internal static class StringExtensionMethods
	{
		public static string ReplaceWhiteSpaces(this string str, string replacement = "")
		{
			return StringExtensionMethods.s_WhitespaceRegex.Replace(str, replacement);
		}

		private static readonly Regex s_WhitespaceRegex = new Regex("\\s+");
	}
}
