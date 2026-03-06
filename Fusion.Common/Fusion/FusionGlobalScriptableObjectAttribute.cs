using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Class)]
	public class FusionGlobalScriptableObjectAttribute : Attribute
	{
		public FusionGlobalScriptableObjectAttribute(string defaultPath)
		{
			this.DefaultPath = defaultPath;
		}

		public string DefaultPath { get; }

		public string DefaultContents { get; set; }

		public string DefaultContentsGeneratorMethod { get; set; }
	}
}
