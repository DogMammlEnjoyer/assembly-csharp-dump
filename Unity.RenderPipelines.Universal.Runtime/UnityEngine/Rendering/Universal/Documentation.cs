using System;

namespace UnityEngine.Rendering.Universal
{
	internal class Documentation : DocumentationInfo
	{
		public static string GetPageLink(string pageName)
		{
			return DocumentationInfo.GetPageLink("com.unity.render-pipelines.universal", pageName);
		}

		public const string packageName = "com.unity.render-pipelines.universal";
	}
}
