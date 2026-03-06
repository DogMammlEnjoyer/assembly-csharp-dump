using System;

namespace Valve.VR
{
	public enum EVRApplicationProperty
	{
		Name_String,
		LaunchType_String = 11,
		WorkingDirectory_String,
		BinaryPath_String,
		Arguments_String,
		URL_String,
		Description_String = 50,
		NewsURL_String,
		ImagePath_String,
		Source_String,
		ActionManifestURL_String,
		IsDashboardOverlay_Bool = 60,
		IsTemplate_Bool,
		IsInstanced_Bool,
		IsInternal_Bool,
		WantsCompositorPauseInStandby_Bool,
		IsHidden_Bool,
		LastLaunchTime_Uint64 = 70
	}
}
