using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Identifiers
{
	[NativeHeader("Modules/Identifiers/Identifiers.h")]
	public static class Identifiers
	{
		public static string installationId
		{
			get
			{
				return Identifiers.GetInstallationId();
			}
		}

		[FreeFunction("UnityEngine_Identifiers_GetInstallationId")]
		private static string GetInstallationId()
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpan;
				Identifiers.GetInstallationId_Injected(out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInstallationId_Injected(out ManagedSpanWrapper ret);
	}
}
