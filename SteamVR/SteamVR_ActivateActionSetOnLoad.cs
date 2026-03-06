using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_ActivateActionSetOnLoad : MonoBehaviour
	{
		private void Start()
		{
			if (this.actionSet != null && this.activateOnStart)
			{
				this.actionSet.Activate(this.forSources, this.initialPriority, this.disableAllOtherActionSets);
			}
		}

		private void OnDestroy()
		{
			if (this.actionSet != null && this.deactivateOnDestroy)
			{
				this.actionSet.Deactivate(this.forSources);
			}
		}

		public SteamVR_ActionSet actionSet = SteamVR_Input.GetActionSet("default", false, false);

		public SteamVR_Input_Sources forSources;

		public bool disableAllOtherActionSets;

		public bool activateOnStart = true;

		public bool deactivateOnDestroy = true;

		public int initialPriority;
	}
}
