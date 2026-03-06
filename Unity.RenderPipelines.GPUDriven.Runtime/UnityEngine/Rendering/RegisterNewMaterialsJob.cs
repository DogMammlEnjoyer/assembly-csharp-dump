using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct RegisterNewMaterialsJob : IJobParallelFor
	{
		public void Execute(int index)
		{
			int key = this.instanceIDs[index];
			this.batchMaterialHashMap.TryAdd(key, this.batchIDs[index]);
			this.packedMaterialHashMap.TryAdd(key, this.packedMaterialDatas[index]);
		}

		public const int k_BatchSize = 128;

		[ReadOnly]
		public NativeArray<int> instanceIDs;

		[ReadOnly]
		public NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas;

		[ReadOnly]
		public NativeArray<BatchMaterialID> batchIDs;

		[WriteOnly]
		public NativeParallelHashMap<int, BatchMaterialID>.ParallelWriter batchMaterialHashMap;

		[WriteOnly]
		public NativeParallelHashMap<int, GPUDrivenPackedMaterialData>.ParallelWriter packedMaterialHashMap;
	}
}
