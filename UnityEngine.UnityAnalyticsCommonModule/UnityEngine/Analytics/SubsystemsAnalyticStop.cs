using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[ExcludeFromDocs]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SubsystemsAnalyticStop : SubsystemsAnalyticBase
	{
		public SubsystemsAnalyticStop() : base("SubsystemStop")
		{
		}

		[RequiredByNativeCode]
		internal static SubsystemsAnalyticStop CreateSubsystemsAnalyticStop()
		{
			return new SubsystemsAnalyticStop();
		}
	}
}
