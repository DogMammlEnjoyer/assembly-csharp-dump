using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Backtrace.Unity.Extensions
{
	internal static class StringHelper
	{
		internal static string OnlyLetters(this string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return string.Empty;
			}
			return new string((from n in source
			where char.IsLetter(n)
			select n).ToArray<char>());
		}

		internal static string GetSha(this StringBuilder source)
		{
			if (source == null)
			{
				return string.Empty;
			}
			return source.ToString().GetSha();
		}

		internal static string GetSha(this string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return "0000000000000000000000000000000000000000000000000000000000000000";
			}
			string result;
			using (SHA256 sha = SHA256.Create())
			{
				byte[] array = sha.ComputeHash(Encoding.ASCII.GetBytes(source));
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < array.Length; i++)
				{
					stringBuilder.Append(array[i].ToString("x2"));
				}
				result = stringBuilder.ToString();
			}
			return result;
		}
	}
}
