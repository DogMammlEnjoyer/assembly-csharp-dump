using System;

namespace OVR.OpenVR
{
	public enum ChaperoneCalibrationState
	{
		OK = 1,
		Warning = 100,
		Warning_BaseStationMayHaveMoved,
		Warning_BaseStationRemoved,
		Warning_SeatedBoundsInvalid,
		Error = 200,
		Error_BaseStationUninitialized,
		Error_BaseStationConflict,
		Error_PlayAreaInvalid,
		Error_CollisionBoundsInvalid
	}
}
