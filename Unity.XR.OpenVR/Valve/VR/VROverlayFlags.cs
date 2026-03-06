using System;

namespace Valve.VR
{
	public enum VROverlayFlags
	{
		NoDashboardTab = 8,
		SendVRDiscreteScrollEvents = 64,
		SendVRTouchpadEvents = 128,
		ShowTouchPadScrollWheel = 256,
		TransferOwnershipToInternalProcess = 512,
		SideBySide_Parallel = 1024,
		SideBySide_Crossed = 2048,
		Panorama = 4096,
		StereoPanorama = 8192,
		SortWithNonSceneOverlays = 16384,
		VisibleInDashboard = 32768,
		MakeOverlaysInteractiveIfVisible = 65536,
		SendVRSmoothScrollEvents = 131072,
		ProtectedContent = 262144,
		HideLaserIntersection = 524288,
		WantsModalBehavior = 1048576,
		IsPremultiplied = 2097152
	}
}
