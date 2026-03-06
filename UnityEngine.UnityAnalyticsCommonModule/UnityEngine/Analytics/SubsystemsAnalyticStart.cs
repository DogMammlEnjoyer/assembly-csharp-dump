using System;
using System.Runtime.InteropServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[ExcludeFromDocs]
	[RequiredByNativeCode(GenerateProxy = true)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public class SubsystemsAnalyticStart : SubsystemsAnalyticBase
	{
		public SubsystemsAnalyticStart() : base("SubsystemStart")
		{
		}

		[RequiredByNativeCode]
		internal static SubsystemsAnalyticStart CreateSubsystemsAnalyticStart()
		{
			return new SubsystemsAnalyticStart();
		}
	}
}
