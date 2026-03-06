using System;
using UnityEngine;

namespace Technie.PhysicsCreator
{
	public static class Console
	{
		static Console()
		{
			Console.output.logEnabled = false;
		}

		public const bool IS_DEBUG_OUTPUT_ENABLED = false;

		public const bool SHOW_SHADOW_HIERARCHY = false;

		public const bool ENABLE_JOINT_SUPPORT = false;

		public static string Technie = "Technie.PhysicsCreator";

		public static Logger output = new Logger(Debug.unityLogger.logHandler);
	}
}
