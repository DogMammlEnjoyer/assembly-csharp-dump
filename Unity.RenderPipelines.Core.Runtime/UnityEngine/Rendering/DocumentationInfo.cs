using System;

namespace UnityEngine.Rendering
{
	public class DocumentationInfo
	{
		public static string version
		{
			get
			{
				return "13.1";
			}
		}

		public static string GetPackageLink(string packageName, string packageVersion, string pageName)
		{
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html{3}", new object[]
			{
				packageName,
				packageVersion,
				pageName,
				""
			});
		}

		public static string GetPackageLink(string packageName, string packageVersion, string pageName, string pageHash)
		{
			if (!string.IsNullOrEmpty(pageHash) && !pageHash.StartsWith("#"))
			{
				pageHash = "#" + pageHash;
			}
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html{3}", new object[]
			{
				packageName,
				packageVersion,
				pageName,
				pageHash
			});
		}

		public static string GetPageLink(string packageName, string pageName)
		{
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html{3}", new object[]
			{
				packageName,
				DocumentationInfo.version,
				pageName,
				""
			});
		}

		public static string GetPageLink(string packageName, string pageName, string pageHash)
		{
			if (!string.IsNullOrEmpty(pageHash) && !pageHash.StartsWith("#"))
			{
				pageHash = "#" + pageHash;
			}
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html{3}", new object[]
			{
				packageName,
				DocumentationInfo.version,
				pageName,
				pageHash
			});
		}

		public static string GetDefaultPackageLink(string packageName, string packageVersion)
		{
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/", packageName, packageVersion);
		}

		public static string GetDefaultPackageLink(string packageName)
		{
			return string.Format("https://docs.unity3d.com/Packages/{0}@{1}/manual/", packageName, DocumentationInfo.version);
		}

		private const string fallbackVersion = "13.1";

		private const string packageDocumentationUrl = "https://docs.unity3d.com/Packages/{0}@{1}/manual/";

		private const string url = "https://docs.unity3d.com/Packages/{0}@{1}/manual/{2}.html{3}";
	}
}
