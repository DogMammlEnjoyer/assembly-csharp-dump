using System;
using System.Text.RegularExpressions;

namespace Fusion
{
	internal class JsonUtils
	{
		public static string RemoveExtraReferences(string baseJson)
		{
			Match match = JsonUtils.ReferencesRegex.Match(baseJson);
			bool success = match.Success;
			if (success)
			{
				int num = match.Index + match.Length;
				int num2 = num;
				int num3 = 0;
				bool flag = false;
				do
				{
					bool flag2 = baseJson[num2] == '{';
					if (flag2)
					{
						num3++;
						flag = true;
					}
					bool flag3 = baseJson[num2] == '}';
					if (flag3)
					{
						num3--;
					}
					num2++;
				}
				while (num2 < baseJson.Length && num3 > 0);
				bool flag4 = num3 == 0 && flag;
				if (flag4)
				{
					baseJson = baseJson.Remove(match.Index, num2 - match.Index);
				}
			}
			return baseJson;
		}

		private static Regex ReferencesRegex = new Regex(",\"references\":", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
