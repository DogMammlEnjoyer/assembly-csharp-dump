using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct UpdateLODGroupDataJob : IJobParallelFor
	{
		public unsafe void Execute(int index)
		{
			GPUInstanceIndex gpuinstanceIndex = this.lodGroupInstances[index];
			LODFadeMode lodfadeMode = this.inputData.fadeMode[index];
			int num = this.inputData.lodOffset[index];
			int num2 = this.inputData.lodCount[index];
			short num3 = this.inputData.renderersCount[index];
			Vector3 v = this.inputData.worldSpaceReferencePoint[index];
			float num4 = this.inputData.worldSpaceSize[index];
			bool flag = this.inputData.lastLODIsBillboard[index];
			byte forceLODMask = this.inputData.forceLODMask[index];
			bool flag2 = lodfadeMode != LODFadeMode.None && this.supportDitheringCrossFade;
			bool flag3 = lodfadeMode == LODFadeMode.SpeedTree;
			LODGroupData* ptr = (LODGroupData*)((byte*)this.lodGroupsData.GetUnsafePtr<LODGroupData>() + (IntPtr)gpuinstanceIndex.index * (IntPtr)sizeof(LODGroupData));
			LODGroupCullingData* ptr2 = (LODGroupCullingData*)((byte*)this.lodGroupsCullingData.GetUnsafePtr<LODGroupCullingData>() + (IntPtr)gpuinstanceIndex.index * (IntPtr)sizeof(LODGroupCullingData));
			ptr->valid = true;
			ptr->lodCount = num2;
			ptr->rendererCount = (int)(flag2 ? num3 : 0);
			ptr2->worldSpaceSize = num4;
			ptr2->worldSpaceReferencePoint = v;
			ptr2->forceLODMask = forceLODMask;
			ptr2->lodCount = num2;
			this.rendererCount.Add(ptr->rendererCount);
			int num5 = 0;
			if (flag3)
			{
				int index2 = num + (num2 - 1);
				bool flag4 = num2 > 0 && this.inputData.lodRenderersCount[index2] == 1 && flag;
				if (num2 == 0)
				{
					num5 = 0;
				}
				else if (flag4)
				{
					num5 = Math.Max(num2, 2) - 2;
				}
				else
				{
					num5 = num2 - 1;
				}
			}
			for (int i = 0; i < num2; i++)
			{
				int num6 = num + i;
				float num7 = this.inputData.lodScreenRelativeTransitionHeight[num6];
				float num8 = LODRenderingUtils.CalculateLODDistance(num7, num4);
				*(ref ptr->screenRelativeTransitionHeights.FixedElementField + (IntPtr)i * 4) = num7;
				*(ref ptr->fadeTransitionWidth.FixedElementField + (IntPtr)i * 4) = 0f;
				*(ref ptr2->sqrDistances.FixedElementField + (IntPtr)i * 4) = num8 * num8;
				*(ref ptr2->percentageFlags.FixedElementField + i) = false;
				*(ref ptr2->transitionDistances.FixedElementField + (IntPtr)i * 4) = 0f;
				if (flag3 && i < num5)
				{
					*(ref ptr2->percentageFlags.FixedElementField + i) = true;
				}
				else if (flag2 && i >= num5)
				{
					float num9 = this.inputData.lodFadeTransitionWidth[num6];
					float num10 = (i != 0) ? this.inputData.lodScreenRelativeTransitionHeight[num6 - 1] : 1f;
					float relativeScreenHeight = num7 + num9 * (num10 - num7);
					float num11 = num8 - LODRenderingUtils.CalculateLODDistance(relativeScreenHeight, num4);
					num11 = Mathf.Max(0f, num11);
					*(ref ptr->fadeTransitionWidth.FixedElementField + (IntPtr)i * 4) = num9;
					*(ref ptr2->transitionDistances.FixedElementField + (IntPtr)i * 4) = num11;
				}
			}
		}

		public const int k_BatchSize = 256;

		[ReadOnly]
		public NativeArray<GPUInstanceIndex> lodGroupInstances;

		[ReadOnly]
		public GPUDrivenLODGroupData inputData;

		[ReadOnly]
		public bool supportDitheringCrossFade;

		public NativeArray<LODGroupData> lodGroupsData;

		public NativeArray<LODGroupCullingData> lodGroupsCullingData;

		[NativeDisableUnsafePtrRestriction]
		public UnsafeAtomicCounter32 rendererCount;
	}
}
