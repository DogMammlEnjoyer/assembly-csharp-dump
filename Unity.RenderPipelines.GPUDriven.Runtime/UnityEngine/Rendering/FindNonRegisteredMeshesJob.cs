using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct FindNonRegisteredMeshesJob : IJobParallelForBatch
	{
		public unsafe void Execute(int startIndex, int count)
		{
			int* ptr = stackalloc int[(UIntPtr)512];
			UnsafeList<int> unsafeList = new UnsafeList<int>(ptr, 128);
			unsafeList.Length = 0;
			for (int i = startIndex; i < startIndex + count; i++)
			{
				int num = this.instanceIDs[i];
				if (!this.hashMap.ContainsKey(num))
				{
					unsafeList.AddNoResize(num);
				}
			}
			this.outInstancesWriter.AddRangeNoResize((void*)ptr, unsafeList.Length);
		}

		public const int k_BatchSize = 128;

		[ReadOnly]
		public NativeArray<int> instanceIDs;

		[ReadOnly]
		public NativeParallelHashMap<int, BatchMeshID> hashMap;

		[WriteOnly]
		public NativeList<int>.ParallelWriter outInstancesWriter;
	}
}
