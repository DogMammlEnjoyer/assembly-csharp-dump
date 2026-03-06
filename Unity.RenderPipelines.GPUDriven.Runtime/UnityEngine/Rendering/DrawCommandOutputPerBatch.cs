using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct DrawCommandOutputPerBatch : IJobParallelFor
	{
		private int EncodeGPUInstanceIndexAndCrossFade(int rendererIndex, bool negateCrossFade)
		{
			GPUInstanceIndex gpuinstanceIndex = this.instanceDataBuffer.CPUInstanceToGPUInstance(InstanceHandle.FromInt(rendererIndex));
			int num = (int)this.rendererCrossFadeValues[rendererIndex];
			if ((long)num == 255L)
			{
				return gpuinstanceIndex.index;
			}
			num -= 127;
			if (negateCrossFade)
			{
				num = -num;
			}
			gpuinstanceIndex.index |= num << 24;
			return gpuinstanceIndex.index;
		}

		private bool IsInstanceFlipped(int rendererIndex)
		{
			InstanceHandle instance = InstanceHandle.FromInt(rendererIndex);
			int index = this.instanceData.InstanceToIndex(instance);
			return this.instanceData.localToWorldIsFlippedBits.Get(index);
		}

		private bool IsMeshLodVisible(int batchLodLevel, int rendererIndex, bool supportsCrossFade, ref bool negateCrossfade)
		{
			if (batchLodLevel < 0)
			{
				return true;
			}
			byte b = this.rendererMeshLodSettings[rendererIndex];
			uint num = (uint)b & 4294967103U;
			if ((long)batchLodLevel == (long)((ulong)num))
			{
				return true;
			}
			if (!supportsCrossFade)
			{
				return false;
			}
			uint num2 = (uint)(b & 192);
			if (num2 == 0U)
			{
				return false;
			}
			int num3 = (int)(num2 - 128U) >> 6;
			negateCrossfade = true;
			return (long)batchLodLevel == (long)((ulong)num + (ulong)((long)num3));
		}

		public unsafe void Execute(int batchIndex)
		{
			DrawBatch drawBatch = this.drawBatches[batchIndex];
			int num = this.batchBinCounts[batchIndex];
			if (num == 0)
			{
				return;
			}
			BatchCullingOutputDrawCommands batchCullingOutputDrawCommands = this.cullingOutput[0];
			IndirectBufferAllocInfo indirectBufferAllocInfo = default(IndirectBufferAllocInfo);
			if (this.indirectBufferLimits.maxDrawCount > 0)
			{
				indirectBufferAllocInfo = this.indirectBufferAllocInfo[0];
			}
			bool flag = !indirectBufferAllocInfo.IsEmpty() && drawBatch.key.range.supportsIndirect;
			int visibilityConfigCount = this.binningConfig.visibilityConfigCount;
			int* ptr = stackalloc int[checked(unchecked((UIntPtr)visibilityConfigCount) * 4)];
			for (int i = 0; i < visibilityConfigCount; i++)
			{
				ptr[i] = 0;
			}
			int* ptr2 = stackalloc int[checked(unchecked((UIntPtr)visibilityConfigCount) * 4)];
			int num2 = this.batchBinAllocOffsets[batchIndex];
			int num3 = this.batchDrawCommandOffsets[batchIndex];
			int num4 = 0;
			bool flag2 = drawBatch.key.range.motionMode == MotionVectorGenerationMode.Object || drawBatch.key.range.motionMode == MotionVectorGenerationMode.ForceNoMotion;
			for (int j = 0; j < num; j++)
			{
				int index = num2 + j;
				int num5 = this.binVisibleInstanceOffsets[index];
				int num6 = this.binVisibleInstanceCounts[index];
				num4 = num5;
				short num7 = this.binConfigIndices[index];
				ptr[num7] = num5;
				int num8 = num3 + j;
				ptr2[num7] = num8;
				BatchDrawCommandFlags batchDrawCommandFlags = drawBatch.key.flags;
				if ((num7 & 1) != 0)
				{
					batchDrawCommandFlags |= BatchDrawCommandFlags.FlipWinding;
				}
				int num9 = num7 >> 1;
				if (this.binningConfig.supportsCrossFade)
				{
					if ((num9 & 1) == 0)
					{
						batchDrawCommandFlags &= ~BatchDrawCommandFlags.LODCrossFadeKeyword;
					}
					else
					{
						batchDrawCommandFlags |= BatchDrawCommandFlags.LODCrossFadeKeyword;
					}
					num9 >>= 1;
				}
				else
				{
					batchDrawCommandFlags &= ~BatchDrawCommandFlags.LODCrossFadeKeyword;
				}
				if (this.binningConfig.supportsMotionCheck)
				{
					if ((num9 & 1) != 0 && flag2)
					{
						batchDrawCommandFlags |= BatchDrawCommandFlags.HasMotion;
					}
					num9 >>= 1;
				}
				int sortingPosition = 0;
				if ((batchDrawCommandFlags & BatchDrawCommandFlags.HasSortingPosition) != BatchDrawCommandFlags.None)
				{
					int num10 = num8;
					if (flag)
					{
						num10 += batchCullingOutputDrawCommands.drawCommandCount;
					}
					sortingPosition = 3 * num10;
				}
				if (flag)
				{
					int num11 = indirectBufferAllocInfo.instanceAllocIndex + num5;
					int num12 = indirectBufferAllocInfo.drawAllocIndex + num8;
					this.indirectDrawInfoGlobalArray[num12] = new IndirectDrawInfo
					{
						indexCount = drawBatch.procInfo.indexCount,
						firstIndex = drawBatch.procInfo.firstIndex,
						baseVertex = drawBatch.procInfo.baseVertex,
						firstInstanceGlobalIndex = (uint)num11,
						maxInstanceCountAndTopology = (uint)(num6 << 3 | (int)drawBatch.procInfo.topology)
					};
					batchCullingOutputDrawCommands.indirectDrawCommands[num8] = new BatchDrawCommandIndirect
					{
						flags = batchDrawCommandFlags,
						visibleOffset = (uint)num11,
						batchID = this.batchIDs[drawBatch.key.overridenComponents],
						materialID = drawBatch.key.materialID,
						splitVisibilityMask = (ushort)num9,
						lightmapIndex = (ushort)drawBatch.key.lightmapIndex,
						sortingPosition = sortingPosition,
						meshID = drawBatch.key.meshID,
						topology = drawBatch.procInfo.topology,
						visibleInstancesBufferHandle = this.visibleInstancesBufferHandle,
						indirectArgsBufferHandle = this.indirectArgsBufferHandle,
						indirectArgsBufferOffset = (uint)(num12 * 20)
					};
				}
				else
				{
					batchCullingOutputDrawCommands.drawCommands[num8] = new BatchDrawCommand
					{
						flags = batchDrawCommandFlags,
						visibleOffset = (uint)num5,
						visibleCount = (uint)num6,
						batchID = this.batchIDs[drawBatch.key.overridenComponents],
						materialID = drawBatch.key.materialID,
						splitVisibilityMask = (ushort)num9,
						lightmapIndex = (ushort)drawBatch.key.lightmapIndex,
						sortingPosition = sortingPosition,
						meshID = drawBatch.key.meshID,
						submeshIndex = (ushort)drawBatch.key.submeshIndex,
						activeMeshLod = (ushort)drawBatch.key.activeMeshLod
					};
				}
			}
			int instanceOffset = drawBatch.instanceOffset;
			int instanceCount = drawBatch.instanceCount;
			bool supportsCrossFade = (drawBatch.key.flags & BatchDrawCommandFlags.LODCrossFadeKeyword) > BatchDrawCommandFlags.None;
			int num13 = 0;
			if (num > 1)
			{
				for (int k = 0; k < instanceCount; k++)
				{
					int num14 = this.drawInstanceIndices[instanceOffset + k];
					bool flag3 = this.IsInstanceFlipped(num14);
					int num15 = (int)this.rendererVisibilityMasks[num14];
					if (num15 != 0)
					{
						bool negateCrossFade = false;
						if (this.IsMeshLodVisible(drawBatch.key.activeMeshLod, num14, supportsCrossFade, ref negateCrossFade))
						{
							num13 = num14;
							int num16 = num15 << 1 | (flag3 ? 1 : 0);
							int num17 = ptr[num16];
							ptr[num16]++;
							int num18 = this.EncodeGPUInstanceIndexAndCrossFade(num14, negateCrossFade);
							if (flag)
							{
								if (this.binningConfig.supportsCrossFade)
								{
									num15 >>= 1;
								}
								if (this.binningConfig.supportsMotionCheck)
								{
									num15 >>= 1;
								}
								this.indirectInstanceInfoGlobalArray[indirectBufferAllocInfo.instanceAllocIndex + num17] = new IndirectInstanceInfo
								{
									drawOffsetAndSplitMask = (ptr2[num16] << 8 | num15),
									instanceIndexAndCrossFade = num18
								};
							}
							else
							{
								batchCullingOutputDrawCommands.visibleInstances[num17] = num18;
							}
						}
					}
				}
			}
			else
			{
				int num19 = num4;
				for (int l = 0; l < instanceCount; l++)
				{
					int num20 = this.drawInstanceIndices[instanceOffset + l];
					int num21 = (int)this.rendererVisibilityMasks[num20];
					if (num21 != 0)
					{
						bool negateCrossFade2 = false;
						if (this.IsMeshLodVisible(drawBatch.key.activeMeshLod, num20, supportsCrossFade, ref negateCrossFade2))
						{
							num13 = num20;
							int num22 = this.EncodeGPUInstanceIndexAndCrossFade(num20, negateCrossFade2);
							if (flag)
							{
								if (this.binningConfig.supportsCrossFade)
								{
									num21 >>= 1;
								}
								if (this.binningConfig.supportsMotionCheck)
								{
									num21 >>= 1;
								}
								this.indirectInstanceInfoGlobalArray[indirectBufferAllocInfo.instanceAllocIndex + num19] = new IndirectInstanceInfo
								{
									drawOffsetAndSplitMask = (num3 << 8 | num21),
									instanceIndexAndCrossFade = num22
								};
							}
							else
							{
								batchCullingOutputDrawCommands.visibleInstances[num19] = num22;
							}
							num19++;
						}
					}
				}
			}
			if ((drawBatch.key.flags & BatchDrawCommandFlags.HasSortingPosition) != BatchDrawCommandFlags.None)
			{
				InstanceHandle instance = InstanceHandle.FromInt(num13 & 16777215);
				int index2 = this.instanceData.InstanceToIndex(instance);
				float3 center = this.instanceData.worldAABBs.UnsafeElementAt(index2).center;
				int num23 = num3;
				if (flag)
				{
					num23 += batchCullingOutputDrawCommands.drawCommandCount;
				}
				int num24 = 3 * num23;
				batchCullingOutputDrawCommands.instanceSortingPositions[num24] = center.x;
				batchCullingOutputDrawCommands.instanceSortingPositions[num24 + 1] = center.y;
				batchCullingOutputDrawCommands.instanceSortingPositions[num24 + 2] = center.z;
			}
		}

		[ReadOnly]
		public BinningConfig binningConfig;

		[ReadOnly]
		public NativeParallelHashMap<uint, BatchID> batchIDs;

		[ReadOnly]
		public GPUInstanceDataBuffer.ReadOnly instanceDataBuffer;

		[ReadOnly]
		public NativeList<DrawBatch> drawBatches;

		[ReadOnly]
		public NativeArray<int> drawInstanceIndices;

		[ReadOnly]
		public CPUInstanceData.ReadOnly instanceData;

		[ReadOnly]
		public NativeArray<byte> rendererVisibilityMasks;

		[ReadOnly]
		public NativeArray<byte> rendererMeshLodSettings;

		[ReadOnly]
		public NativeArray<byte> rendererCrossFadeValues;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> batchBinAllocOffsets;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> batchBinCounts;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> batchDrawCommandOffsets;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<short> binConfigIndices;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> binVisibleInstanceOffsets;

		[ReadOnly]
		[DeallocateOnJobCompletion]
		public NativeArray<int> binVisibleInstanceCounts;

		[ReadOnly]
		public NativeArray<BatchCullingOutputDrawCommands> cullingOutput;

		[ReadOnly]
		public IndirectBufferLimits indirectBufferLimits;

		[ReadOnly]
		public GraphicsBufferHandle visibleInstancesBufferHandle;

		[ReadOnly]
		public GraphicsBufferHandle indirectArgsBufferHandle;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<IndirectBufferAllocInfo> indirectBufferAllocInfo;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<IndirectDrawInfo> indirectDrawInfoGlobalArray;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<IndirectInstanceInfo> indirectInstanceInfoGlobalArray;
	}
}
