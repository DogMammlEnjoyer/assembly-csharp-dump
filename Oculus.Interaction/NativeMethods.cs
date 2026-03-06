using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Oculus.Interaction
{
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		[DllImport("InteractionSdk")]
		public static extern int isdk_NativeComponent_Activate(ulong id);

		public const int IsdkSuccess = 0;
	}
}
