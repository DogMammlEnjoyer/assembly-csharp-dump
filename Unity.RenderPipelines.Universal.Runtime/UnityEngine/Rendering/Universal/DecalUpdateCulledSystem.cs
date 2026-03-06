using System;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalUpdateCulledSystem
	{
		public DecalUpdateCulledSystem(DecalEntityManager entityManager)
		{
			this.m_EntityManager = entityManager;
			this.m_Sampler = new ProfilingSampler("DecalUpdateCulledSystem.Execute");
		}

		public void Execute()
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(this.m_EntityManager.culledChunks[i], this.m_EntityManager.culledChunks[i].count);
				}
			}
		}

		private void Execute(DecalCulledChunk culledChunk, int count)
		{
			if (count == 0)
			{
				return;
			}
			culledChunk.currentJobHandle.Complete();
			CullingGroup cullingGroups = culledChunk.cullingGroups;
			culledChunk.visibleDecalCount = cullingGroups.QueryIndices(true, culledChunk.visibleDecalIndexArray, 0);
			culledChunk.visibleDecalIndices.CopyFrom(culledChunk.visibleDecalIndexArray);
		}

		private DecalEntityManager m_EntityManager;

		private ProfilingSampler m_Sampler;
	}
}
