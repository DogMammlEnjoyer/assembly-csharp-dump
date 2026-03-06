using System;

namespace Valve.VR
{
	public enum SteamVR_UpdateModes
	{
		Nothing = 1,
		OnUpdate,
		OnFixedUpdate = 4,
		OnPreCull = 8,
		OnLateUpdate = 16
	}
}
