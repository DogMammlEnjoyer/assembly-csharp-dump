using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Windows
{
	public static class CrashReporting
	{
		public static string crashReportFolder
		{
			[NativeHeader("PlatformDependent/WinPlayer/Bindings/CrashReportingBindings.h")]
			[ThreadSafe]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpan;
					CrashReporting.get_crashReportFolder_Injected(out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_crashReportFolder_Injected(out ManagedSpanWrapper ret);
	}
}
