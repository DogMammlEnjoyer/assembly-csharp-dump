using System;
using System.Collections.Generic;

namespace Valve.VR
{
	public class SteamVR_Action_In_Source_Map<SourceElement> : SteamVR_Action_Source_Map<SourceElement> where SourceElement : SteamVR_Action_In_Source, new()
	{
		public bool IsUpdating(SteamVR_Input_Sources inputSource)
		{
			for (int i = 0; i < this.updatingSources.Count; i++)
			{
				if (inputSource == (SteamVR_Input_Sources)this.updatingSources[i])
				{
					return true;
				}
			}
			return false;
		}

		protected override void OnAccessSource(SteamVR_Input_Sources inputSource)
		{
			if (SteamVR_Action.startUpdatingSourceOnAccess)
			{
				this.ForceAddSourceToUpdateList(inputSource);
			}
		}

		public void ForceAddSourceToUpdateList(SteamVR_Input_Sources inputSource)
		{
			if (this.sources[(int)inputSource] == null)
			{
				this.sources[(int)inputSource] = Activator.CreateInstance<SourceElement>();
			}
			if (!this.sources[(int)inputSource].isUpdating)
			{
				this.updatingSources.Add((int)inputSource);
				this.sources[(int)inputSource].isUpdating = true;
				if (!SteamVR_Input.isStartupFrame)
				{
					this.sources[(int)inputSource].UpdateValue();
				}
			}
		}

		public void UpdateValues()
		{
			for (int i = 0; i < this.updatingSources.Count; i++)
			{
				this.sources[this.updatingSources[i]].UpdateValue();
			}
		}

		protected List<int> updatingSources = new List<int>();
	}
}
