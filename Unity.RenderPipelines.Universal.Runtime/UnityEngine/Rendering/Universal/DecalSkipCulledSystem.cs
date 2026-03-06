using System;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalSkipCulledSystem
	{
		public DecalSkipCulledSystem(DecalEntityManager entityManager)
		{
			this.m_EntityManager = entityManager;
			this.m_Sampler = new ProfilingSampler("DecalSkipCulledSystem.Execute");
		}

		public void Execute(Camera camera)
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				this.m_Camera = camera;
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
			for (int i = 0; i < count; i++)
			{
				culledChunk.visibleDecalIndices[i] = i;
			}
			culledChunk.visibleDecalCount = count;
			culledChunk.cameraPosition = this.m_Camera.transform.position;
			culledChunk.cullingMask = this.m_Camera.cullingMask;
		}

		internal static ulong GetSceneCullingMaskFromCamera(Camera camera)
		{
			return 0UL;
		}

		private DecalEntityManager m_EntityManager;

		private ProfilingSampler m_Sampler;

		private Camera m_Camera;
	}
}
