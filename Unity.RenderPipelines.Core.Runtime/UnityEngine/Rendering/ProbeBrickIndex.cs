using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;

namespace UnityEngine.Rendering
{
	internal class ProbeBrickIndex
	{
		internal int estimatedVMemCost { get; private set; }

		internal ComputeBuffer GetDebugFragmentationBuffer()
		{
			return this.m_DebugFragmentationBuffer;
		}

		internal float fragmentationRate { get; private set; }

		private int SizeOfPhysicalIndexFromBudget(ProbeVolumeTextureMemoryBudget memoryBudget)
		{
			if (memoryBudget == ProbeVolumeTextureMemoryBudget.MemoryBudgetLow)
			{
				return 4000000;
			}
			if (memoryBudget == ProbeVolumeTextureMemoryBudget.MemoryBudgetMedium)
			{
				return 8000000;
			}
			if (memoryBudget != ProbeVolumeTextureMemoryBudget.MemoryBudgetHigh)
			{
				return 32000000;
			}
			return 16000000;
		}

		internal ProbeBrickIndex(ProbeVolumeTextureMemoryBudget memoryBudget)
		{
			this.m_CenterRS = new Vector3Int(0, 0, 0);
			this.m_NeedUpdateIndexComputeBuffer = false;
			this.m_ChunksCount = Mathf.Max(1, Mathf.CeilToInt((float)this.SizeOfPhysicalIndexFromBudget(memoryBudget) / 243f));
			this.m_AvailableChunkCount = this.m_ChunksCount;
			this.m_IndexChunks = new BitArray(this.m_ChunksCount);
			this.m_IndexChunksCopyForChecks = new BitArray(this.m_ChunksCount);
			int num = this.m_ChunksCount * 243;
			this.m_PhysicalIndexBufferData = new NativeArray<int>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			this.m_PhysicalIndexBuffer = new ComputeBuffer(num, 4, ComputeBufferType.Structured);
			this.estimatedVMemCost = num * 4;
			this.Clear();
		}

		public int GetRemainingChunkCount()
		{
			return this.m_AvailableChunkCount;
		}

		internal void UploadIndexData()
		{
			int count = this.m_UpdateMaxIndex - this.m_UpdateMinIndex + 1;
			this.m_PhysicalIndexBuffer.SetData<int>(this.m_PhysicalIndexBufferData, this.m_UpdateMinIndex, this.m_UpdateMinIndex, count);
			this.m_NeedUpdateIndexComputeBuffer = false;
			this.m_UpdateMaxIndex = int.MinValue;
			this.m_UpdateMinIndex = int.MaxValue;
		}

		private void UpdateDebugData()
		{
			if (this.m_DebugFragmentationData == null || this.m_DebugFragmentationData.Length != this.m_IndexChunks.Length)
			{
				this.m_DebugFragmentationData = new int[this.m_IndexChunks.Length];
				CoreUtils.SafeRelease(this.m_DebugFragmentationBuffer);
				this.m_DebugFragmentationBuffer = new ComputeBuffer(this.m_IndexChunks.Length, 4);
			}
			for (int i = 0; i < this.m_IndexChunks.Length; i++)
			{
				this.m_DebugFragmentationData[i] = (this.m_IndexChunks[i] ? 1 : -1);
			}
			this.m_DebugFragmentationBuffer.SetData(this.m_DebugFragmentationData);
		}

		internal unsafe void Clear()
		{
			this.m_IndexChunks.SetAll(false);
			this.m_AvailableChunkCount = this.m_ChunksCount;
			uint* unsafePtr = (uint*)this.m_PhysicalIndexBufferData.GetUnsafePtr<int>();
			UnsafeUtility.MemSet((void*)unsafePtr, byte.MaxValue, (long)(this.m_PhysicalIndexBufferData.Length * 4));
			this.m_NeedUpdateIndexComputeBuffer = true;
			this.m_UpdateMinIndex = 0;
			this.m_UpdateMaxIndex = this.m_PhysicalIndexBufferData.Length - 1;
		}

		internal void GetRuntimeResources(ref ProbeReferenceVolume.RuntimeResources rr)
		{
			bool displayIndexFragmentation = ProbeReferenceVolume.instance.probeVolumeDebug.displayIndexFragmentation;
			if (this.m_NeedUpdateIndexComputeBuffer)
			{
				this.UploadIndexData();
				if (displayIndexFragmentation)
				{
					this.UpdateDebugData();
				}
			}
			if (displayIndexFragmentation && this.m_DebugFragmentationBuffer == null)
			{
				this.UpdateDebugData();
			}
			rr.index = this.m_PhysicalIndexBuffer;
		}

		internal void Cleanup()
		{
			this.m_PhysicalIndexBufferData.Dispose();
			CoreUtils.SafeRelease(this.m_PhysicalIndexBuffer);
			this.m_PhysicalIndexBuffer = null;
			CoreUtils.SafeRelease(this.m_DebugFragmentationBuffer);
			this.m_DebugFragmentationBuffer = null;
		}

		internal void ComputeFragmentationRate()
		{
			int num = 0;
			for (int i = this.m_ChunksCount - 1; i >= 0; i--)
			{
				if (this.m_IndexChunks[i])
				{
					num = i + 1;
					break;
				}
			}
			int num2 = this.m_ChunksCount - num;
			int num3 = this.m_AvailableChunkCount - num2;
			this.fragmentationRate = (float)num3 / (float)num;
		}

		private int MergeIndex(int index, int size)
		{
			return (index & -1879048193) | (size & 7) << 28;
		}

		internal int GetNumberOfChunks(int brickCount)
		{
			return Mathf.CeilToInt((float)brickCount / 243f);
		}

		internal bool FindSlotsForEntries(ref ProbeBrickIndex.IndirectionEntryUpdateInfo[] entriesInfo)
		{
			bool result;
			using (new ProfilerMarker("FindSlotsForEntries").Auto())
			{
				this.m_IndexChunksCopyForChecks.SetAll(false);
				this.m_IndexChunksCopyForChecks.Or(this.m_IndexChunks);
				int num = entriesInfo.Length;
				for (int i = 0; i < num; i++)
				{
					entriesInfo[i].firstChunkIndex = -2;
					int numberOfChunks = entriesInfo[i].numberOfChunks;
					if (numberOfChunks != 0)
					{
						for (int j = 0; j < this.m_ChunksCount - numberOfChunks; j++)
						{
							if (!this.m_IndexChunksCopyForChecks[j])
							{
								int firstChunkIndex = j;
								int num2 = j + numberOfChunks;
								while (j + 1 < num2 && !this.m_IndexChunksCopyForChecks[++j])
								{
								}
								if (!this.m_IndexChunksCopyForChecks[j])
								{
									entriesInfo[i].firstChunkIndex = firstChunkIndex;
									break;
								}
							}
						}
						if (entriesInfo[i].firstChunkIndex < 0)
						{
							for (int k = 0; k < num; k++)
							{
								entriesInfo[k].firstChunkIndex = -1;
							}
							return false;
						}
						for (int l = entriesInfo[i].firstChunkIndex; l < entriesInfo[i].firstChunkIndex + numberOfChunks; l++)
						{
							this.m_IndexChunksCopyForChecks[l] = true;
						}
					}
				}
				result = true;
			}
			return result;
		}

		internal bool ReserveChunks(ProbeBrickIndex.IndirectionEntryUpdateInfo[] entriesInfo, bool ignoreErrorLog)
		{
			int num = entriesInfo.Length;
			for (int i = 0; i < num; i++)
			{
				int firstChunkIndex = entriesInfo[i].firstChunkIndex;
				int numberOfChunks = entriesInfo[i].numberOfChunks;
				if (numberOfChunks != 0)
				{
					if (firstChunkIndex < 0)
					{
						if (!ignoreErrorLog)
						{
							Debug.LogError("APV Index Allocation failed.");
						}
						return false;
					}
					for (int j = firstChunkIndex; j < firstChunkIndex + numberOfChunks; j++)
					{
						this.m_IndexChunks[j] = true;
					}
					this.m_AvailableChunkCount -= numberOfChunks;
				}
			}
			return true;
		}

		internal static bool BrickOverlapEntry(Vector3Int brickMin, Vector3Int brickMax, Vector3Int entryMin, Vector3Int entryMax)
		{
			return brickMax.x > entryMin.x && entryMax.x > brickMin.x && brickMax.y > entryMin.y && entryMax.y > brickMin.y && brickMax.z > entryMin.z && entryMax.z > brickMin.z;
		}

		private static int LocationToIndex(int x, int y, int z, Vector3Int sizeOfValid)
		{
			return z * (sizeOfValid.x * sizeOfValid.y) + x * sizeOfValid.y + y;
		}

		private void MarkBrickInPhysicalBuffer(in ProbeBrickIndex.IndirectionEntryUpdateInfo entry, Vector3Int brickMin, Vector3Int brickMax, int brickSubdivLevel, int entrySubdivLevel, int idx)
		{
			this.m_NeedUpdateIndexComputeBuffer = true;
			if (entry.hasOnlyBiggerBricks)
			{
				int num = entry.firstChunkIndex * 243;
				this.m_UpdateMinIndex = Math.Min(this.m_UpdateMinIndex, num);
				this.m_UpdateMaxIndex = Math.Max(this.m_UpdateMaxIndex, num);
				this.m_PhysicalIndexBufferData[num] = idx;
				return;
			}
			int b = ProbeReferenceVolume.CellSize(entry.minSubdivInCell);
			Vector3Int b2 = entry.minValidBrickIndexForCellAtMaxRes / b;
			Vector3Int vector3Int = entry.maxValidBrickIndexForCellAtMaxResPlusOne / b - b2;
			if (brickSubdivLevel >= entrySubdivLevel)
			{
				brickMin = Vector3Int.zero;
				brickMax = vector3Int;
			}
			else
			{
				brickMin -= entry.entryPositionInBricksAtMaxRes;
				brickMax -= entry.entryPositionInBricksAtMaxRes;
				brickMin /= b;
				brickMax /= b;
				ProbeReferenceVolume.CellSize(entrySubdivLevel - entry.minSubdivInCell);
				brickMin -= b2;
				brickMax -= b2;
			}
			int num2 = entry.firstChunkIndex * 243;
			int val = num2 + ProbeBrickIndex.LocationToIndex(brickMin.x, brickMin.y, brickMin.z, vector3Int);
			int val2 = num2 + ProbeBrickIndex.LocationToIndex(brickMax.x - 1, brickMax.y - 1, brickMax.z - 1, vector3Int);
			this.m_UpdateMinIndex = Math.Min(this.m_UpdateMinIndex, val);
			this.m_UpdateMaxIndex = Math.Max(this.m_UpdateMaxIndex, val2);
			for (int i = brickMin.x; i < brickMax.x; i++)
			{
				for (int j = brickMin.z; j < brickMax.z; j++)
				{
					for (int k = brickMin.y; k < brickMax.y; k++)
					{
						int num3 = ProbeBrickIndex.LocationToIndex(i, k, j, vector3Int);
						this.m_PhysicalIndexBufferData[num2 + num3] = idx;
					}
				}
			}
		}

		public void AddBricks(ProbeReferenceVolume.CellIndexInfo cellInfo, NativeArray<ProbeBrickIndex.Brick> bricks, List<ProbeBrickPool.BrickChunkAlloc> allocations, int allocationSize, int poolWidth, int poolHeight)
		{
			int entrySubdivLevel = ProbeReferenceVolume.instance.GetEntrySubdivLevel();
			int num = 0;
			for (int i = 0; i < allocations.Count; i++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = allocations[i];
				int num2 = num + Mathf.Min(allocationSize, bricks.Length - num);
				while (num != num2)
				{
					ProbeBrickIndex.Brick brick = bricks[num++];
					int idx = this.MergeIndex(brickChunkAlloc.flattenIndex(poolWidth, poolHeight), brick.subdivisionLevel);
					brickChunkAlloc.x += 4;
					int num3 = ProbeReferenceVolume.CellSize(brick.subdivisionLevel);
					Vector3Int position = brick.position;
					Vector3Int brickMax = brick.position + new Vector3Int(num3, num3, num3);
					foreach (ProbeBrickIndex.IndirectionEntryUpdateInfo indirectionEntryUpdateInfo in cellInfo.updateInfo.entriesInfo)
					{
						Vector3Int entryMin = indirectionEntryUpdateInfo.entryPositionInBricksAtMaxRes + indirectionEntryUpdateInfo.minValidBrickIndexForCellAtMaxRes;
						Vector3Int entryMax = indirectionEntryUpdateInfo.entryPositionInBricksAtMaxRes + indirectionEntryUpdateInfo.maxValidBrickIndexForCellAtMaxResPlusOne - Vector3Int.one;
						if (ProbeBrickIndex.BrickOverlapEntry(position, brickMax, entryMin, entryMax))
						{
							this.MarkBrickInPhysicalBuffer(indirectionEntryUpdateInfo, position, brickMax, brick.subdivisionLevel, entrySubdivLevel, idx);
						}
					}
				}
			}
		}

		public void RemoveBricks(ProbeReferenceVolume.CellIndexInfo cellInfo)
		{
			for (int i = 0; i < cellInfo.updateInfo.entriesInfo.Length; i++)
			{
				ref ProbeBrickIndex.IndirectionEntryUpdateInfo ptr = ref cellInfo.updateInfo.entriesInfo[i];
				if (ptr.firstChunkIndex >= 0)
				{
					for (int j = ptr.firstChunkIndex; j < ptr.firstChunkIndex + ptr.numberOfChunks; j++)
					{
						this.m_IndexChunks[j] = false;
					}
					this.m_AvailableChunkCount += ptr.numberOfChunks;
					ptr.numberOfChunks = 0;
				}
			}
		}

		internal const int kMaxSubdivisionLevels = 7;

		internal const int kIndexChunkSize = 243;

		internal const int kFailChunkIndex = -1;

		internal const int kEmptyIndex = -2;

		private BitArray m_IndexChunks;

		private BitArray m_IndexChunksCopyForChecks;

		private int m_ChunksCount;

		private int m_AvailableChunkCount;

		private ComputeBuffer m_PhysicalIndexBuffer;

		private NativeArray<int> m_PhysicalIndexBufferData;

		private ComputeBuffer m_DebugFragmentationBuffer;

		private int[] m_DebugFragmentationData;

		private bool m_NeedUpdateIndexComputeBuffer;

		private int m_UpdateMinIndex = int.MaxValue;

		private int m_UpdateMaxIndex = int.MinValue;

		private Vector3Int m_CenterRS;

		[DebuggerDisplay("Brick [{position}, {subdivisionLevel}]")]
		[Serializable]
		public struct Brick : IEquatable<ProbeBrickIndex.Brick>
		{
			internal Brick(Vector3Int position, int subdivisionLevel)
			{
				this.position = position;
				this.subdivisionLevel = subdivisionLevel;
			}

			public bool Equals(ProbeBrickIndex.Brick other)
			{
				return this.position == other.position && this.subdivisionLevel == other.subdivisionLevel;
			}

			public bool IntersectArea(Bounds boundInBricksToCheck)
			{
				int num = ProbeReferenceVolume.CellSize(this.subdivisionLevel);
				Bounds bounds = default(Bounds);
				bounds.min = this.position;
				bounds.max = this.position + new Vector3Int(num, num, num);
				bounds.extents *= 0.99f;
				return boundInBricksToCheck.Intersects(bounds);
			}

			public Vector3Int position;

			public int subdivisionLevel;
		}

		public struct IndirectionEntryUpdateInfo
		{
			public int firstChunkIndex;

			public int numberOfChunks;

			public int minSubdivInCell;

			public Vector3Int minValidBrickIndexForCellAtMaxRes;

			public Vector3Int maxValidBrickIndexForCellAtMaxResPlusOne;

			public Vector3Int entryPositionInBricksAtMaxRes;

			public bool hasOnlyBiggerBricks;
		}

		public struct CellIndexUpdateInfo
		{
			public int GetNumberOfChunks()
			{
				int num = 0;
				foreach (ProbeBrickIndex.IndirectionEntryUpdateInfo indirectionEntryUpdateInfo in this.entriesInfo)
				{
					num += indirectionEntryUpdateInfo.numberOfChunks;
				}
				return num;
			}

			public ProbeBrickIndex.IndirectionEntryUpdateInfo[] entriesInfo;
		}
	}
}
