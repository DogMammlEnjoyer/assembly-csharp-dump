using System;

namespace Valve.VR
{
	public class SteamVR_Action_Skeleton_Source_Map : SteamVR_Action_Pose_Source_Map<SteamVR_Action_Skeleton_Source>
	{
		protected override SteamVR_Action_Skeleton_Source GetSourceElementForIndexer(SteamVR_Input_Sources inputSource)
		{
			return this.sources[0];
		}
	}
}
