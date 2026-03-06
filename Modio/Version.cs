using System;
using System.Collections.Generic;

namespace Modio
{
	public static class Version
	{
		public static void AddEnvironmentDetails(string details)
		{
			Version.EnvironmentDetails.Add(details);
		}

		public static string GetCurrent()
		{
			if (Version.EnvironmentDetails.Count != 0)
			{
				return string.Format("modio.cs/{0} ({1})", Version.Current, string.Join("; ", Version.EnvironmentDetails));
			}
			return string.Format("modio.cs/{0}", Version.Current);
		}

		private static readonly Version Current = new Version(2025, 6);

		private static readonly List<string> EnvironmentDetails = new List<string>();
	}
}
