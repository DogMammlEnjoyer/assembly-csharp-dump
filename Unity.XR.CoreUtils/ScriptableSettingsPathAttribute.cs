using System;

namespace Unity.XR.CoreUtils
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ScriptableSettingsPathAttribute : Attribute
	{
		public string Path
		{
			get
			{
				return this.m_Path;
			}
		}

		public ScriptableSettingsPathAttribute(string path = "")
		{
			this.m_Path = path;
		}

		private readonly string m_Path;
	}
}
