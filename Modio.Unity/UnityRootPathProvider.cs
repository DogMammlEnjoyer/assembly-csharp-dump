using System;
using System.IO;
using Modio.FileIO;
using UnityEngine;

namespace Modio.Unity
{
	public class UnityRootPathProvider : IModioRootPathProvider
	{
		public string Path
		{
			get
			{
				return Application.persistentDataPath;
			}
		}

		public string UserPath
		{
			get
			{
				return System.IO.Path.Combine(Application.persistentDataPath, "UserData");
			}
		}
	}
}
