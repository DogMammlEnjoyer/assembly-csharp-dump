using System;

namespace Valve.VR
{
	public class SteamVR_Action_Pose_Source_Map<Source> : SteamVR_Action_In_Source_Map<Source> where Source : SteamVR_Action_Pose_Source, new()
	{
		public void SetTrackingUniverseOrigin(ETrackingUniverseOrigin newOrigin)
		{
			for (int i = 0; i < this.sources.Length; i++)
			{
				if (this.sources[i] != null)
				{
					this.sources[i].universeOrigin = newOrigin;
				}
			}
		}

		public virtual void UpdateValues(bool skipStateAndEventUpdates)
		{
			for (int i = 0; i < this.updatingSources.Count; i++)
			{
				this.sources[this.updatingSources[i]].UpdateValue(skipStateAndEventUpdates);
			}
		}
	}
}
