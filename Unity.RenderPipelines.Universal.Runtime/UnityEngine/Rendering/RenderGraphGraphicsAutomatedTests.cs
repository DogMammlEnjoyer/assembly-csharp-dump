using System;

namespace UnityEngine.Rendering
{
	public static class RenderGraphGraphicsAutomatedTests
	{
		private static bool activatedFromCommandLine
		{
			get
			{
				return Array.Exists<string>(Environment.GetCommandLineArgs(), (string arg) => arg == "-render-graph-reuse-tests");
			}
		}

		public static bool enabled { get; set; } = RenderGraphGraphicsAutomatedTests.activatedFromCommandLine;
	}
}
