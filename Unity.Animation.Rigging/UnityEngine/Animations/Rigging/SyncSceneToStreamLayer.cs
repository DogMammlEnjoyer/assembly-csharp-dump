using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
	internal class SyncSceneToStreamLayer
	{
		public bool Initialize(Animator animator, IList<IRigLayer> layers)
		{
			if (this.isInitialized)
			{
				return true;
			}
			this.m_RigIndices = new List<int>(layers.Count);
			for (int i = 0; i < layers.Count; i++)
			{
				if (layers[i].IsValid())
				{
					this.m_RigIndices.Add(i);
				}
			}
			this.m_Data = RigUtils.CreateSyncSceneToStreamData(animator, layers);
			if (!this.m_Data.IsValid())
			{
				return false;
			}
			this.job = RigUtils.syncSceneToStreamBinder.Create(animator, this.m_Data, null);
			return this.isInitialized = true;
		}

		public void Update(IList<IRigLayer> layers)
		{
			if (!this.isInitialized || !this.m_Data.IsValid())
			{
				return;
			}
			IRigSyncSceneToStreamData rigSyncSceneToStreamData = (IRigSyncSceneToStreamData)this.m_Data;
			int i = 0;
			int count = this.m_RigIndices.Count;
			while (i < count)
			{
				rigSyncSceneToStreamData.rigStates[i] = layers[this.m_RigIndices[i]].active;
				i++;
			}
			RigUtils.syncSceneToStreamBinder.Update(this.job, this.m_Data);
		}

		public void Reset()
		{
			if (!this.isInitialized)
			{
				return;
			}
			if (this.m_Data != null && this.m_Data.IsValid())
			{
				RigUtils.syncSceneToStreamBinder.Destroy(this.job);
				this.m_Data = null;
			}
			this.isInitialized = false;
		}

		public bool IsValid()
		{
			return this.job != null && this.m_Data != null;
		}

		public bool isInitialized { get; private set; }

		public IAnimationJob job;

		private IAnimationJobData m_Data;

		private List<int> m_RigIndices;
	}
}
