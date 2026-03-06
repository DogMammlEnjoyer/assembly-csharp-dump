using System;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[RequiredByNativeCode]
	[Preserve]
	public enum AnalyticsSessionState
	{
		kSessionStopped,
		kSessionStarted,
		kSessionPaused,
		kSessionResumed
	}
}
