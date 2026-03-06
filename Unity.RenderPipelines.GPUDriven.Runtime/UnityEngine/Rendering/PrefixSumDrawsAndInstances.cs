using System;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct PrefixSumDrawsAndInstances : IJob
	{
		public unsafe void Execute()
		{
			BatchCullingOutputDrawCommands batchCullingOutputDrawCommands = this.cullingOutput[0];
			bool flag = this.indirectBufferLimits.maxInstanceCount > 0;
			int num2;
			int num3;
			int num4;
			for (;;)
			{
				int num = 0;
				num2 = 0;
				num3 = 0;
				num4 = 0;
				int num5 = 0;
				for (int i = 0; i < this.drawRanges.Length; i++)
				{
					DrawRange drawRange = this.drawRanges[i];
					bool flag2 = flag && drawRange.key.supportsIndirect;
					int num6 = 0;
					int drawCommandsBegin = flag2 ? num4 : num2;
					for (int j = 0; j < drawRange.drawCount; j++)
					{
						int index = this.drawBatchIndices[drawRange.drawOffset + j];
						int num7 = this.batchBinAllocOffsets[index];
						int num8 = this.batchBinCounts[index];
						if (flag2)
						{
							this.batchDrawCommandOffsets[index] = num4;
							num4 += num8;
						}
						else
						{
							this.batchDrawCommandOffsets[index] = num2;
							num2 += num8;
						}
						num6 += num8;
						for (int k = 0; k < num8; k++)
						{
							int index2 = num7 + k;
							if (flag2)
							{
								this.binVisibleInstanceOffsets[index2] = num5;
								num5 += this.binVisibleInstanceCounts[index2];
							}
							else
							{
								this.binVisibleInstanceOffsets[index2] = num3;
								num3 += this.binVisibleInstanceCounts[index2];
							}
						}
					}
					if (num6 != 0)
					{
						RangeKey key = drawRange.key;
						batchCullingOutputDrawCommands.drawRanges[num] = new BatchDrawRange
						{
							drawCommandsBegin = (uint)drawCommandsBegin,
							drawCommandsCount = (uint)num6,
							drawCommandsType = (flag2 ? BatchDrawCommandType.Indirect : BatchDrawCommandType.Direct),
							filterSettings = new BatchFilterSettings
							{
								renderingLayerMask = key.renderingLayerMask,
								rendererPriority = key.rendererPriority,
								layer = key.layer,
								batchLayer = (flag2 ? 28 : 29),
								motionMode = key.motionMode,
								shadowCastingMode = key.shadowCastingMode,
								receiveShadows = true,
								staticShadowCaster = key.staticShadowCaster,
								allDepthSorted = false
							}
						};
						num++;
					}
				}
				batchCullingOutputDrawCommands.drawRangeCount = num;
				bool flag3 = true;
				if (flag)
				{
					int* unsafePtr = (int*)this.indirectAllocationCounters.GetUnsafePtr<int>();
					IndirectBufferAllocInfo indirectBufferAllocInfo = new IndirectBufferAllocInfo
					{
						drawCount = num4,
						instanceCount = num5
					};
					int num9 = indirectBufferAllocInfo.drawCount + 1;
					int num10 = Interlocked.Add(UnsafeUtility.AsRef<int>((void*)(unsafePtr + 1)), num9);
					indirectBufferAllocInfo.drawAllocIndex = num10 - num9;
					int num11 = Interlocked.Add(UnsafeUtility.AsRef<int>((void*)unsafePtr), indirectBufferAllocInfo.instanceCount);
					indirectBufferAllocInfo.instanceAllocIndex = num11 - indirectBufferAllocInfo.instanceCount;
					if (!indirectBufferAllocInfo.IsWithinLimits(this.indirectBufferLimits))
					{
						indirectBufferAllocInfo = default(IndirectBufferAllocInfo);
						flag3 = false;
					}
					this.indirectBufferAllocInfo[0] = indirectBufferAllocInfo;
				}
				if (flag3)
				{
					break;
				}
				flag = false;
			}
			if (num2 != 0)
			{
				batchCullingOutputDrawCommands.drawCommandCount = num2;
				batchCullingOutputDrawCommands.drawCommands = MemoryUtilities.Malloc<BatchDrawCommand>(num2, Allocator.TempJob);
				batchCullingOutputDrawCommands.visibleInstanceCount = num3;
				batchCullingOutputDrawCommands.visibleInstances = MemoryUtilities.Malloc<int>(num3, Allocator.TempJob);
			}
			if (num4 != 0)
			{
				batchCullingOutputDrawCommands.indirectDrawCommandCount = num4;
				batchCullingOutputDrawCommands.indirectDrawCommands = MemoryUtilities.Malloc<BatchDrawCommandIndirect>(num4, Allocator.TempJob);
			}
			int num12 = num2 + num4;
			batchCullingOutputDrawCommands.instanceSortingPositions = MemoryUtilities.Malloc<float>(3 * num12, Allocator.TempJob);
			this.cullingOutput[0] = batchCullingOutputDrawCommands;
		}

		[ReadOnly]
		public NativeList<DrawRange> drawRanges;

		[ReadOnly]
		public NativeArray<int> drawBatchIndices;

		[ReadOnly]
		public NativeArray<int> batchBinAllocOffsets;

		[ReadOnly]
		public NativeArray<int> batchBinCounts;

		[ReadOnly]
		public NativeArray<int> binVisibleInstanceCounts;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> batchDrawCommandOffsets;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> binVisibleInstanceOffsets;

		[NativeDisableUnsafePtrRestriction]
		public NativeArray<BatchCullingOutputDrawCommands> cullingOutput;

		[ReadOnly]
		public IndirectBufferLimits indirectBufferLimits;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<IndirectBufferAllocInfo> indirectBufferAllocInfo;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<int> indirectAllocationCounters;
	}
}
