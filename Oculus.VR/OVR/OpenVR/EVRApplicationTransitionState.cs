using System;

namespace OVR.OpenVR
{
	public enum EVRApplicationTransitionState
	{
		VRApplicationTransition_None,
		VRApplicationTransition_OldAppQuitSent = 10,
		VRApplicationTransition_WaitingForExternalLaunch,
		VRApplicationTransition_NewAppLaunched = 20
	}
}
