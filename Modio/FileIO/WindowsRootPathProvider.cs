using System;
using UnityEngine;

namespace Modio.FileIO
{
	public class WindowsRootPathProvider : IModioRootPathProvider
	{
		public static bool IsPublicEnvironmentVariableSet()
		{
			return Environment.GetEnvironmentVariable("public") != null;
		}

		public string Path
		{
			get
			{
				return Environment.GetEnvironmentVariable("public") ?? "";
			}
		}

		public string UserPath
		{
			get
			{
				return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) ?? "";
			}
		}

		public string LegacyPath = Application.persistentDataPath ?? "";

		public string LegacyUserPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/mod.io";
	}
}
