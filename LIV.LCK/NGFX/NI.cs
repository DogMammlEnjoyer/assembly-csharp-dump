using System;
using System.Runtime.InteropServices;

namespace Liv.NGFX
{
	public class NI
	{
		[DllImport("ngfx")]
		public static extern IntPtr GetPluginEventFunction();

		[DllImport("ngfx")]
		public static extern uint AllocResource(IntPtr resource_ctx);

		[DllImport("ngfx")]
		public static extern void SetGlobalLogLevel(LogLevel level, bool enableGLMessages);
	}
}
