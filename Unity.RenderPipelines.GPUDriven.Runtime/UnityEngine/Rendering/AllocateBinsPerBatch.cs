using System;
using System.Threading;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering
{
	[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
	internal struct AllocateBinsPerBatch : IJobParallelFor
	{
		private bool IsInstanceFlipped(int rendererIndex)
		{
			InstanceHandle instance = InstanceHandle.FromInt(rendererIndex);
			int index = this.instanceData.InstanceToIndex(instance);
			return this.instanceData.localToWorldIsFlippedBits.Get(index);
		}

		private bool IsMeshLodVisible(int batchLodLevel, int rendererIndex, bool supportsCrossFade)
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
			return (long)batchLodLevel == (long)((ulong)num + (ulong)((long)num3));
		}

		private static int GetPrimitiveCount(int indexCount, MeshTopology topology, bool nativeQuads)
		{
			switch (topology)
			{
			case MeshTopology.Triangles:
				return indexCount / 3;
			case MeshTopology.Quads:
				if (!nativeQuads)
				{
					return indexCount / 4 * 2;
				}
				return indexCount / 4;
			case MeshTopology.Lines:
				return indexCount / 2;
			case MeshTopology.LineStrip:
				if (indexCount < 1)
				{
					return 0;
				}
				return indexCount - 1;
			case MeshTopology.Points:
				return indexCount;
			}
			return 0;
		}

		public unsafe void Execute(int batchIndex)
		{
			int visibilityConfigCount = this.binningConfig.visibilityConfigCount;
			int* ptr = stackalloc int[checked(unchecked((UIntPtr)visibilityConfigCount) * 4)];
			for (int i = 0; i < visibilityConfigCount; i++)
			{
				ptr[i] = 0;
			}
			int num = (visibilityConfigCount + 63) / 64;
			ulong* ptr2 = stackalloc ulong[checked(unchecked((UIntPtr)num) * 8)];
			for (int j = 0; j < num; j++)
			{
				ptr2[j] = 0UL;
			}
			DrawBatch drawBatch = this.drawBatches[batchIndex];
			int instanceCount = drawBatch.instanceCount;
			int instanceOffset = drawBatch.instanceOffset;
			bool supportsCrossFade = (drawBatch.key.flags & BatchDrawCommandFlags.LODCrossFadeKeyword) > BatchDrawCommandFlags.None;
			for (int k = 0; k < instanceCount; k++)
			{
				int num2 = this.drawInstanceIndices[instanceOffset + k];
				bool flag = this.IsInstanceFlipped(num2);
				int num3 = (int)this.rendererVisibilityMasks[num2];
				if (num3 != 0 && this.IsMeshLodVisible(drawBatch.key.activeMeshLod, num2, supportsCrossFade))
				{
					int num4 = num3 << 1 | (flag ? 1 : 0);
					ptr[num4]++;
					ptr2[num4 >> 6] |= 1UL << num4;
				}
			}
			int num5 = 0;
			for (int l = 0; l < num; l++)
			{
				num5 += math.countbits(ptr2[l]);
			}
			int num6 = 0;
			if (num5 > 0)
			{
				int* ptr3;
				int* ptr4;
				checked
				{
					ptr3 = stackalloc int[unchecked((UIntPtr)this.binningConfig.viewCount) * 4];
					ptr4 = stackalloc int[unchecked((UIntPtr)this.binningConfig.viewCount) * 4];
				}
				for (int m = 0; m < this.binningConfig.viewCount; m++)
				{
					ptr3[m] = 0;
					ptr4[m] = 0;
				}
				bool flag2 = this.debugCounterIndexBase >= 0;
				int num7 = 1 + (this.binningConfig.supportsMotionCheck ? 1 : 0) + (this.binningConfig.supportsCrossFade ? 1 : 0);
				int* unsafePtr = (int*)this.binAllocCounter.GetUnsafePtr<int>();
				num6 = Interlocked.Add(UnsafeUtility.AsRef<int>((void*)unsafePtr), num5) - num5;
				int num8 = num6;
				for (int n = 0; n < num; n++)
				{
					ulong num9 = ptr2[n];
					while (num9 != 0UL)
					{
						int num10 = math.tzcnt(num9);
						num9 ^= 1UL << num10;
						int num11 = 64 * n + num10;
						int num12 = ptr[num11];
						this.binConfigIndices[num8] = (short)num11;
						this.binVisibleInstanceCounts[num8] = num12;
						num8++;
						int num13 = flag2 ? (num11 >> num7) : 0;
						while (num13 != 0)
						{
							int num14 = math.tzcnt(num13);
							num13 ^= 1 << num14;
							ptr3[num14]++;
							ptr4[num14] += num12;
						}
					}
				}
				if (flag2)
				{
					for (int num15 = 0; num15 < this.binningConfig.viewCount; num15++)
					{
						int* ptr5 = (int*)((byte*)this.splitDebugCounters.GetUnsafePtr<int>() + (IntPtr)((this.debugCounterIndexBase + num15) * 3) * 4);
						int num16 = ptr3[num15];
						if (num16 > 0)
						{
							Interlocked.Add(UnsafeUtility.AsRef<int>((void*)(ptr5 + 2)), num16);
						}
						int num17 = ptr4[num15];
						if (num17 > 0)
						{
							int primitiveCount = AllocateBinsPerBatch.GetPrimitiveCount((int)drawBatch.procInfo.indexCount, drawBatch.procInfo.topology, false);
							Interlocked.Add(UnsafeUtility.AsRef<int>((void*)ptr5), num17);
							Interlocked.Add(UnsafeUtility.AsRef<int>((void*)(ptr5 + 1)), num17 * primitiveCount);
						}
					}
				}
			}
			this.batchBinAllocOffsets[batchIndex] = num6;
			this.batchBinCounts[batchIndex] = num5;
		}

		[ReadOnly]
		public BinningConfig binningConfig;

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

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> batchBinAllocOffsets;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> batchBinCounts;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[DeallocateOnJobCompletion]
		public NativeArray<int> binAllocCounter;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<short> binConfigIndices;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		[WriteOnly]
		public NativeArray<int> binVisibleInstanceCounts;

		[ReadOnly]
		public int debugCounterIndexBase;

		[NativeDisableContainerSafetyRestriction]
		[NoAlias]
		public NativeArray<int> splitDebugCounters;
	}
}
