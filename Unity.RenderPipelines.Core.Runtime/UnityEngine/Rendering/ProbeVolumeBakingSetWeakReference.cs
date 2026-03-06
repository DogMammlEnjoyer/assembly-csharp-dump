using System;

namespace UnityEngine.Rendering
{
	internal class ProbeVolumeBakingSetWeakReference
	{
		public ProbeVolumeBakingSetWeakReference(ProbeVolumeBakingSet bakingSet)
		{
			this.Set(bakingSet);
		}

		public ProbeVolumeBakingSetWeakReference()
		{
			this.m_InstanceID = 0;
		}

		public void Set(ProbeVolumeBakingSet bakingSet)
		{
			if (bakingSet == null)
			{
				this.m_InstanceID = 0;
				return;
			}
			this.m_InstanceID = bakingSet.GetInstanceID();
		}

		public ProbeVolumeBakingSet Get()
		{
			return Resources.InstanceIDToObject(this.m_InstanceID) as ProbeVolumeBakingSet;
		}

		public bool IsLoaded()
		{
			return Resources.InstanceIDIsValid(this.m_InstanceID);
		}

		public void Unload()
		{
			if (!this.IsLoaded())
			{
				return;
			}
			Resources.UnloadAsset(this.Get());
		}

		public int m_InstanceID;
	}
}
