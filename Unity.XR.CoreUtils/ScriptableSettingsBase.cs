using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace Unity.XR.CoreUtils
{
	public abstract class ScriptableSettingsBase : ScriptableObject
	{
		public static ScriptableSettingsBase GetInstanceByType(Type settingsType)
		{
			return (ScriptableSettingsBase)settingsType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.GetProperty).GetValue(null, null);
		}

		private void Awake()
		{
		}

		private void OnEnable()
		{
			this.OnLoaded();
		}

		protected virtual void OnLoaded()
		{
		}

		internal static bool ValidatePath(string path, out string cleanedPath)
		{
			cleanedPath = path;
			if (cleanedPath == null)
			{
				Debug.LogWarning("Path cannot be null");
				return false;
			}
			foreach (char c in ScriptableSettingsBase.k_InvalidCharacters)
			{
				if (cleanedPath.Contains(c.ToString()))
				{
					Debug.LogWarning("Paths on Windows cannot contain the following characters: ':', '*', '?', '\"', '<', '>', '|'");
					return false;
				}
			}
			foreach (string value in ScriptableSettingsBase.k_InvalidStrings)
			{
				if (cleanedPath.Contains(value))
				{
					Debug.LogWarning("Path cannot contain the character '.' before or after a directory separator");
					return false;
				}
			}
			try
			{
				if (Path.IsPathRooted(cleanedPath))
				{
					Debug.LogWarning("Path cannot be absolute");
					return false;
				}
			}
			catch (Exception arg)
			{
				Debug.LogWarning(string.Format("{0}\n{1}", "Exception caught trying to create path.", arg));
				return false;
			}
			cleanedPath = cleanedPath.Trim(ScriptableSettingsBase.k_PathTrimChars);
			int num = 0;
			for (int j = cleanedPath.Length - 1; j >= 0; j--)
			{
				if (cleanedPath[j] == '\\' || cleanedPath[j] == '/')
				{
					num++;
				}
				else if (num > 0)
				{
					cleanedPath = cleanedPath.Remove(j + 1, num - 1);
					num = 0;
				}
			}
			if (cleanedPath != "")
			{
				cleanedPath += "/";
			}
			return true;
		}

		private const string k_AbsolutePathMessage = "Path cannot be absolute";

		protected const string PathExceptionMessage = "Exception caught trying to create path.";

		internal const string NullPathMessage = "Path cannot be null";

		internal const string PathWithPeriodMessage = "Path cannot contain the character '.' before or after a directory separator";

		internal const string PathWithInvalidCharacterMessage = "Paths on Windows cannot contain the following characters: ':', '*', '?', '\"', '<', '>', '|'";

		private static readonly char[] k_PathTrimChars = new char[]
		{
			Path.DirectorySeparatorChar,
			Path.AltDirectorySeparatorChar,
			' '
		};

		private static readonly char[] k_InvalidCharacters = new char[]
		{
			':',
			'*',
			'?',
			'"',
			'<',
			'>',
			'|',
			'\\'
		};

		private static readonly string[] k_InvalidStrings = new string[]
		{
			"\\.",
			"/.",
			".\\",
			"./"
		};
	}
}
