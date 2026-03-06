using System;
using System.IO;
using UnityEngine;

namespace Backtrace.Unity.Common
{
	internal static class ClientPathHelper
	{
		internal static string GetFullPath(string path)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			return path.ParseInterpolatedString().GenerateFullPath();
		}

		private static string ParseInterpolatedString(this string path)
		{
			int num = path.IndexOf("${");
			if (num == -1)
			{
				return path;
			}
			int num2 = path.IndexOf('}', num);
			if (num2 == -1)
			{
				return path;
			}
			string text = path.Substring(num, num2 - num + 1);
			if (string.IsNullOrEmpty(text))
			{
				return path;
			}
			string a = text.ToLower();
			if (a == "${application.persistentdatapath}")
			{
				return path.Replace(text, Application.persistentDataPath);
			}
			if (!(a == "${application.datapath}"))
			{
				return path;
			}
			return path.Replace(text, Application.dataPath);
		}

		private static string GenerateFullPath(this string path)
		{
			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(Application.persistentDataPath, path);
			}
			string result;
			try
			{
				result = Path.GetFullPath(path);
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			return result;
		}

		internal static bool IsFileInDatabaseDirectory(string databasePath, string filePath)
		{
			if (!databasePath.EndsWith("/"))
			{
				return new DirectoryInfo(databasePath).FullName == new DirectoryInfo(Path.GetDirectoryName(filePath)).FullName;
			}
			return Path.GetDirectoryName(databasePath) == Path.GetDirectoryName(filePath);
		}
	}
}
