using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct UpdatePackedMaterialDataCacheJob : IJob
	{
		private void ProcessMaterial(int i)
		{
			int num = this.materialIDs[i];
			GPUDrivenPackedMaterialData value = this.packedMaterialDatas[i];
			if (num == 0)
			{
				return;
			}
			this.packedMaterialHash[num] = value;
		}

		public void Execute()
		{
			for (int i = 0; i < this.materialIDs.Length; i++)
			{
				this.ProcessMaterial(i);
			}
		}

		[ReadOnly]
		public NativeArray<int>.ReadOnly materialIDs;

		[ReadOnly]
		public NativeArray<GPUDrivenPackedMaterialData>.ReadOnly packedMaterialDatas;

		public NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialHash;
	}
}
