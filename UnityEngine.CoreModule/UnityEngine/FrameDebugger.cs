using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Profiler/PerformanceTools/FrameDebugger.h")]
	[StaticAccessor("FrameDebugger", StaticAccessorType.DoubleColon)]
	public static class FrameDebugger
	{
		public static bool enabled
		{
			get
			{
				return FrameDebugger.IsLocalEnabled() || FrameDebugger.IsRemoteEnabled();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsLocalEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsRemoteEnabled();
	}
}
