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
	public class VRDeviceAnalyticBase : AnalyticsEventBase
	{
		public VRDeviceAnalyticBase() : base("deviceStatus", 1, SendEventOptions.kAppendNone, "")
		{
		}
	}
}
