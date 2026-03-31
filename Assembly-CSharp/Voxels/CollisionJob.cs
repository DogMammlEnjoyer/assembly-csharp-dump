using System;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;

namespace Voxels
{
	[BurstCompile]
	public struct CollisionJob : IJob
	{
		public void Execute()
		{
			Physics.BakeMesh(this.MeshId, false, MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase);
		}

		public const MeshColliderCookingOptions CookingOptions = MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning | MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.UseFastMidphase;

		public EntityId MeshId;
	}
}
