using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Modio
{
	public static class ModioCommandLine
	{
		public static bool TryGet(string argument, out string value)
		{
			if (ModioCommandLine._argumentCache == null)
			{
				ModioCommandLine.GetArguments();
			}
			value = null;
			return ModioCommandLine._argumentCache != null && ModioCommandLine._argumentCache.TryGetValue(argument, out value);
		}

		private static void GetArguments()
		{
			if (ModioCommandLine._argumentCache != null)
			{
				return;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			for (int i = 0; i < commandLineArgs.Length; i++)
			{
				string text = commandLineArgs[i];
				if (text.StartsWith("-modio-"))
				{
					string[] array = text.Split('=', StringSplitOptions.None);
					string key;
					string value;
					if (array.Length == 2)
					{
						key = array[0].Substring("-modio-".Length);
						value = array[1];
					}
					else
					{
						if (i + 1 >= commandLineArgs.Length)
						{
							goto IL_83;
						}
						key = text.Substring("-modio-".Length);
						value = commandLineArgs[i + 1];
					}
					dictionary[key] = value;
				}
				IL_83:;
			}
			ModioCommandLine._argumentCache = new ReadOnlyDictionary<string, string>(dictionary);
		}

		private const string PREFIX = "-modio-";

		private static ReadOnlyDictionary<string, string> _argumentCache;
	}
}
