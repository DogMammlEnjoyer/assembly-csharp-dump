using System;

namespace UnityEngine.Rendering.Universal
{
	internal class DecalUpdateCullingGroupSystem
	{
		public float boundingDistance
		{
			get
			{
				return this.m_BoundingDistance[0];
			}
			set
			{
				this.m_BoundingDistance[0] = value;
			}
		}

		public DecalUpdateCullingGroupSystem(DecalEntityManager entityManager, float drawDistance)
		{
			this.m_EntityManager = entityManager;
			this.m_BoundingDistance[0] = drawDistance;
			this.m_Sampler = new ProfilingSampler("DecalUpdateCullingGroupsSystem.Execute");
		}

		public void Execute(Camera camera)
		{
			using (new ProfilingScope(this.m_Sampler))
			{
				this.m_Camera = camera;
				for (int i = 0; i < this.m_EntityManager.chunkCount; i++)
				{
					this.Execute(this.m_EntityManager.cachedChunks[i], this.m_EntityManager.culledChunks[i], this.m_EntityManager.culledChunks[i].count);
				}
			}
		}

		public void Execute(DecalCachedChunk cachedChunk, DecalCulledChunk culledChunk, int count)
		{
			cachedChunk.currentJobHandle.Complete();
			CullingGroup cullingGroups = culledChunk.cullingGroups;
			cullingGroups.targetCamera = this.m_Camera;
			cullingGroups.SetDistanceReferencePoint(this.m_Camera.transform.position);
			cullingGroups.SetBoundingDistances(this.m_BoundingDistance);
			cachedChunk.boundingSpheres.CopyTo(cachedChunk.boundingSphereArray);
			cullingGroups.SetBoundingSpheres(cachedChunk.boundingSphereArray);
			cullingGroups.SetBoundingSphereCount(count);
			culledChunk.cameraPosition = this.m_Camera.transform.position;
			culledChunk.cullingMask = this.m_Camera.cullingMask;
		}

		internal static ulong GetSceneCullingMaskFromCamera(Camera camera)
		{
			return 0UL;
		}

		private float[] m_BoundingDistance = new float[1];

		private Camera m_Camera;

		private DecalEntityManager m_EntityManager;

		private ProfilingSampler m_Sampler;
	}
}
