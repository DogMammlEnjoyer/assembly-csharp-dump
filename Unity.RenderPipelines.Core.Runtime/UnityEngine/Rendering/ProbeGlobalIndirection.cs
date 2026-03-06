using System;

namespace UnityEngine.Rendering
{
	internal class ProbeGlobalIndirection
	{
		internal int estimatedVMemCost { get; private set; }

		internal void GetMinMaxEntry(out Vector3Int minEntry, out Vector3Int maxEntry)
		{
			minEntry = this.m_EntryMin;
			maxEntry = this.m_EntryMax;
		}

		internal Vector3Int GetGlobalIndirectionDimension()
		{
			return this.m_EntriesCount;
		}

		internal Vector3Int GetGlobalIndirectionMinEntry()
		{
			return this.m_EntryMin;
		}

		private int entrySizeInBricks
		{
			get
			{
				return Mathf.Min((int)Mathf.Pow(3f, 3f), this.m_CellSizeInMinBricks);
			}
		}

		internal int entriesPerCellDimension
		{
			get
			{
				return this.m_CellSizeInMinBricks / Mathf.Max(1, this.entrySizeInBricks);
			}
		}

		private int GetFlatIndex(Vector3Int normalizedPos)
		{
			return normalizedPos.z * (this.m_EntriesCount.x * this.m_EntriesCount.y) + normalizedPos.y * this.m_EntriesCount.x + normalizedPos.x;
		}

		internal ProbeGlobalIndirection(Vector3Int cellMin, Vector3Int cellMax, int cellSizeInMinBricks)
		{
			this.m_CellSizeInMinBricks = cellSizeInMinBricks;
			Vector3Int a = cellMax + Vector3Int.one - cellMin;
			this.m_EntriesCount = a * this.entriesPerCellDimension;
			this.m_EntryMin = cellMin * this.entriesPerCellDimension;
			this.m_EntryMax = (cellMax + Vector3Int.one) * this.entriesPerCellDimension - Vector3Int.one;
			int num = this.m_EntriesCount.x * this.m_EntriesCount.y * this.m_EntriesCount.z;
			int num2 = 3 * num;
			this.m_IndexOfIndicesBuffer = new ComputeBuffer(num, 12);
			this.m_IndexOfIndicesData = new uint[num2];
			this.m_NeedUpdateComputeBuffer = false;
			this.estimatedVMemCost = num * 3 * 4;
		}

		internal int GetFlatIdxForEntry(Vector3Int entryPosition)
		{
			Vector3Int normalizedPos = entryPosition - this.m_EntryMin;
			return this.GetFlatIndex(normalizedPos);
		}

		internal int[] GetFlatIndicesForCell(Vector3Int cellPosition)
		{
			Vector3Int a = cellPosition * this.entriesPerCellDimension;
			int num = this.m_CellSizeInMinBricks / this.entrySizeInBricks;
			int[] array = new int[this.entriesPerCellDimension * this.entriesPerCellDimension * this.entriesPerCellDimension];
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				for (int j = 0; j < num; j++)
				{
					for (int k = 0; k < num; k++)
					{
						array[num2++] = this.GetFlatIdxForEntry(a + new Vector3Int(i, j, k));
					}
				}
			}
			return array;
		}

		internal void UpdateCell(ProbeReferenceVolume.CellIndexInfo cellInfo)
		{
			for (int i = 0; i < cellInfo.flatIndicesInGlobalIndirection.Length; i++)
			{
				int num = cellInfo.flatIndicesInGlobalIndirection[i];
				ProbeBrickIndex.IndirectionEntryUpdateInfo indirectionEntryUpdateInfo = cellInfo.updateInfo.entriesInfo[i];
				int b = ProbeReferenceVolume.CellSize(indirectionEntryUpdateInfo.minSubdivInCell);
				ProbeGlobalIndirection.IndexMetaData indexMetaData = default(ProbeGlobalIndirection.IndexMetaData);
				indexMetaData.minSubdiv = indirectionEntryUpdateInfo.minSubdivInCell;
				indexMetaData.minLocalIdx = (indirectionEntryUpdateInfo.hasOnlyBiggerBricks ? Vector3Int.zero : (indirectionEntryUpdateInfo.minValidBrickIndexForCellAtMaxRes / b));
				indexMetaData.maxLocalIdxPlusOne = (indirectionEntryUpdateInfo.hasOnlyBiggerBricks ? Vector3Int.one : (indirectionEntryUpdateInfo.maxValidBrickIndexForCellAtMaxResPlusOne / b));
				indexMetaData.firstChunkIndex = indirectionEntryUpdateInfo.firstChunkIndex;
				uint[] array;
				indexMetaData.Pack(out array);
				for (int j = 0; j < 3; j++)
				{
					this.m_IndexOfIndicesData[num * 3 + j] = array[j];
				}
			}
			this.m_NeedUpdateComputeBuffer = true;
		}

		internal void MarkEntriesAsUnloaded(int[] entriesFlatIndices)
		{
			for (int i = 0; i < entriesFlatIndices.Length; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					this.m_IndexOfIndicesData[entriesFlatIndices[i] * 3 + j] = uint.MaxValue;
				}
			}
			this.m_NeedUpdateComputeBuffer = true;
		}

		internal void PushComputeData()
		{
			this.m_IndexOfIndicesBuffer.SetData(this.m_IndexOfIndicesData);
			this.m_NeedUpdateComputeBuffer = false;
		}

		internal void GetRuntimeResources(ref ProbeReferenceVolume.RuntimeResources rr)
		{
			if (this.m_NeedUpdateComputeBuffer)
			{
				this.PushComputeData();
			}
			rr.cellIndices = this.m_IndexOfIndicesBuffer;
		}

		internal void Cleanup()
		{
			CoreUtils.SafeRelease(this.m_IndexOfIndicesBuffer);
			this.m_IndexOfIndicesBuffer = null;
		}

		private const int kUintPerEntry = 3;

		internal const int kEntryMaxSubdivLevel = 3;

		private ComputeBuffer m_IndexOfIndicesBuffer;

		private uint[] m_IndexOfIndicesData;

		private int m_CellSizeInMinBricks;

		private Vector3Int m_EntriesCount;

		private Vector3Int m_EntryMin;

		private Vector3Int m_EntryMax;

		private bool m_NeedUpdateComputeBuffer;

		internal struct IndexMetaData
		{
			internal void Pack(out uint[] vals)
			{
				vals = ProbeGlobalIndirection.IndexMetaData.s_PackedValues;
				for (int i = 0; i < 3; i++)
				{
					vals[i] = 0U;
				}
				Vector3Int vector3Int = this.maxLocalIdxPlusOne - this.minLocalIdx;
				vals[0] = (uint)(this.firstChunkIndex & 536870911);
				vals[0] |= (uint)((uint)(this.minSubdiv & 7) << 29);
				vals[1] = (uint)(this.minLocalIdx.x & 1023);
				vals[1] |= (uint)((uint)(this.minLocalIdx.y & 1023) << 10);
				vals[1] |= (uint)((uint)(this.minLocalIdx.z & 1023) << 20);
				vals[2] = (uint)(vector3Int.x & 1023);
				vals[2] |= (uint)((uint)(vector3Int.y & 1023) << 10);
				vals[2] |= (uint)((uint)(vector3Int.z & 1023) << 20);
			}

			private static uint[] s_PackedValues = new uint[3];

			internal Vector3Int minLocalIdx;

			internal Vector3Int maxLocalIdxPlusOne;

			internal int firstChunkIndex;

			internal int minSubdiv;
		}
	}
}
