using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct UpdateLODGroupTransformJob : IJobParallelFor
	{
		public unsafe void Execute(int index)
		{
			int key = this.lodGroupIDs[index];
			GPUInstanceIndex gpuinstanceIndex;
			if (this.lodGroupDataHash.TryGetValue(key, out gpuinstanceIndex))
			{
				float num = this.worldSpaceSizes[index];
				LODGroupData* ptr = this.lodGroupData.GetUnsafePtr<LODGroupData>() + gpuinstanceIndex.index;
				LODGroupCullingData* ptr2 = this.lodGroupCullingData.GetUnsafePtr<LODGroupCullingData>() + gpuinstanceIndex.index;
				ptr2->worldSpaceSize = num;
				ptr2->worldSpaceReferencePoint = this.worldSpaceReferencePoints[index];
				for (int i = 0; i < ptr->lodCount; i++)
				{
					float num2 = *(ref ptr->screenRelativeTransitionHeights.FixedElementField + (IntPtr)i * 4);
					float num3 = LODRenderingUtils.CalculateLODDistance(num2, num);
					*(ref ptr2->sqrDistances.FixedElementField + (IntPtr)i * 4) = num3 * num3;
					if (this.supportDitheringCrossFade && !(*(ref ptr2->percentageFlags.FixedElementField + i)))
					{
						float num4 = (i != 0) ? (*(ref ptr->screenRelativeTransitionHeights.FixedElementField + (IntPtr)(i - 1) * 4)) : 1f;
						float relativeScreenHeight = num2 + *(ref ptr->fadeTransitionWidth.FixedElementField + (IntPtr)i * 4) * (num4 - num2);
						float num5 = num3 - LODRenderingUtils.CalculateLODDistance(relativeScreenHeight, num);
						num5 = Mathf.Max(0f, num5);
						*(ref ptr2->transitionDistances.FixedElementField + (IntPtr)i * 4) = num5;
					}
					else
					{
						*(ref ptr2->transitionDistances.FixedElementField + (IntPtr)i * 4) = 0f;
					}
				}
			}
		}

		public const int k_BatchSize = 256;

		[ReadOnly]
		public NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash;

		[ReadOnly]
		public NativeArray<int> lodGroupIDs;

		[ReadOnly]
		public NativeArray<Vector3> worldSpaceReferencePoints;

		[ReadOnly]
		public NativeArray<float> worldSpaceSizes;

		[ReadOnly]
		public bool requiresGPUUpload;

		[ReadOnly]
		public bool supportDitheringCrossFade;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[ReadOnly]
		public NativeList<LODGroupData> lodGroupData;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeList<LODGroupCullingData> lodGroupCullingData;

		[NativeDisableUnsafePtrRestriction]
		public UnsafeAtomicCounter32 atomicUpdateCount;
	}
}
