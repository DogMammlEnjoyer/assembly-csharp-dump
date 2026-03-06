using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct RegisterNewMeshesJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			this.hashMap.TryAdd(this.instanceIDs[index], this.batchIDs[index]);
		}

		public const int k_BatchSize = 128;

		[ReadOnly]
		public NativeArray<int> instanceIDs;

		[ReadOnly]
		public NativeArray<BatchMeshID> batchIDs;

		[WriteOnly]
		public NativeParallelHashMap<int, BatchMeshID>.ParallelWriter hashMap;
	}
}
