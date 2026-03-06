using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Multiplayer.Playmode
{
	public static class CurrentPlayer
	{
		public static bool IsMainEditor
		{
			get
			{
				return false;
			}
		}

		public static string[] ReadOnlyTags()
		{
			if (!CurrentPlayer.s_Loaded)
			{
				CurrentPlayer.s_Loaded = true;
				CurrentPlayer.LoadTag();
			}
			return CurrentPlayer.s_Tags.ToArray();
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ReloadLatestTagsOnEnterPlaymode()
		{
			CurrentPlayer.s_Loaded = false;
		}

		private static void LoadTag()
		{
		}

		public static void ReportResult(bool condition, string message = "", [CallerFilePath] string callingFilePath = "", [CallerLineNumber] int lineNumber = 0)
		{
		}

		private static bool s_Loaded;

		private static List<string> s_Tags = new List<string>();
	}
}
