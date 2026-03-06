using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	internal class ProbeVolumeScratchBufferPool
	{
		public int chunkSize { get; private set; }

		public int maxChunkCount { get; private set; }

		public int allocatedMemory
		{
			get
			{
				return this.chunkSize * this.m_CurrentlyAllocatedChunkCount;
			}
		}

		public ProbeVolumeScratchBufferPool(ProbeVolumeBakingSet bakingSet, ProbeVolumeSHBands shBands)
		{
			this.chunkSize = bakingSet.GetChunkGPUMemory(shBands);
			this.maxChunkCount = bakingSet.maxSHChunkCount;
			this.m_L0Size = bakingSet.L0ChunkSize;
			this.m_L1Size = bakingSet.L1ChunkSize;
			this.m_ValiditySize = bakingSet.sharedValidityMaskChunkSize;
			this.m_ValidityLayerCount = bakingSet.bakedMaskCount;
			this.m_SkyOcclusionSize = bakingSet.sharedSkyOcclusionL0L1ChunkSize;
			this.m_SkyShadingDirectionSize = bakingSet.sharedSkyShadingDirectionIndicesChunkSize;
			this.m_L2Size = bakingSet.L2TextureChunkSize;
			this.m_ProbeOcclusionSize = bakingSet.ProbeOcclusionChunkSize;
		}

		private ProbeReferenceVolume.CellStreamingScratchBufferLayout GetOrCreateScratchBufferLayout(int chunkCount)
		{
			ProbeReferenceVolume.CellStreamingScratchBufferLayout result;
			if (this.m_Layouts.TryGetValue(chunkCount, out result))
			{
				return result;
			}
			ProbeReferenceVolume.CellStreamingScratchBufferLayout cellStreamingScratchBufferLayout = default(ProbeReferenceVolume.CellStreamingScratchBufferLayout);
			cellStreamingScratchBufferLayout._L0Size = this.m_L0Size;
			cellStreamingScratchBufferLayout._L1Size = this.m_L1Size;
			cellStreamingScratchBufferLayout._ValiditySize = this.m_ValiditySize;
			cellStreamingScratchBufferLayout._ValidityProbeSize = this.m_ValidityLayerCount;
			if (this.m_SkyOcclusionSize != 0)
			{
				cellStreamingScratchBufferLayout._SkyOcclusionSize = this.m_SkyOcclusionSize;
				cellStreamingScratchBufferLayout._SkyOcclusionProbeSize = 8;
				if (this.m_SkyShadingDirectionSize != 0)
				{
					cellStreamingScratchBufferLayout._SkyShadingDirectionSize = this.m_SkyShadingDirectionSize;
					cellStreamingScratchBufferLayout._SkyShadingDirectionProbeSize = 1;
				}
				else
				{
					cellStreamingScratchBufferLayout._SkyShadingDirectionSize = 0;
					cellStreamingScratchBufferLayout._SkyShadingDirectionProbeSize = 0;
				}
			}
			else
			{
				cellStreamingScratchBufferLayout._SkyOcclusionSize = 0;
				cellStreamingScratchBufferLayout._SkyOcclusionProbeSize = 0;
				cellStreamingScratchBufferLayout._SkyShadingDirectionSize = 0;
				cellStreamingScratchBufferLayout._SkyShadingDirectionProbeSize = 0;
			}
			cellStreamingScratchBufferLayout._L2Size = this.m_L2Size;
			if (this.m_ProbeOcclusionSize != 0)
			{
				cellStreamingScratchBufferLayout._ProbeOcclusionSize = this.m_ProbeOcclusionSize;
				cellStreamingScratchBufferLayout._ProbeOcclusionProbeSize = 4;
			}
			else
			{
				cellStreamingScratchBufferLayout._ProbeOcclusionSize = 0;
				cellStreamingScratchBufferLayout._ProbeOcclusionProbeSize = 0;
			}
			cellStreamingScratchBufferLayout._L0ProbeSize = 8;
			cellStreamingScratchBufferLayout._L1ProbeSize = 4;
			cellStreamingScratchBufferLayout._L2ProbeSize = 4;
			int num = chunkCount * 4 * 4;
			cellStreamingScratchBufferLayout._SharedDestChunksOffset = num;
			cellStreamingScratchBufferLayout._L0L1rxOffset = cellStreamingScratchBufferLayout._SharedDestChunksOffset + num;
			cellStreamingScratchBufferLayout._L1GryOffset = cellStreamingScratchBufferLayout._L0L1rxOffset + this.m_L0Size * chunkCount;
			cellStreamingScratchBufferLayout._L1BrzOffset = cellStreamingScratchBufferLayout._L1GryOffset + this.m_L1Size * chunkCount;
			cellStreamingScratchBufferLayout._ValidityOffset = cellStreamingScratchBufferLayout._L1BrzOffset + this.m_L1Size * chunkCount;
			cellStreamingScratchBufferLayout._ProbeOcclusionOffset = cellStreamingScratchBufferLayout._ValidityOffset + this.m_ValiditySize * chunkCount;
			cellStreamingScratchBufferLayout._SkyOcclusionOffset = cellStreamingScratchBufferLayout._ProbeOcclusionOffset + this.m_ProbeOcclusionSize * chunkCount;
			cellStreamingScratchBufferLayout._SkyShadingDirectionOffset = cellStreamingScratchBufferLayout._SkyOcclusionOffset + this.m_SkyOcclusionSize * chunkCount;
			cellStreamingScratchBufferLayout._L2_0Offset = cellStreamingScratchBufferLayout._SkyShadingDirectionOffset + this.m_SkyShadingDirectionSize * chunkCount;
			cellStreamingScratchBufferLayout._L2_1Offset = cellStreamingScratchBufferLayout._L2_0Offset + this.m_L2Size * chunkCount;
			cellStreamingScratchBufferLayout._L2_2Offset = cellStreamingScratchBufferLayout._L2_1Offset + this.m_L2Size * chunkCount;
			cellStreamingScratchBufferLayout._L2_3Offset = cellStreamingScratchBufferLayout._L2_2Offset + this.m_L2Size * chunkCount;
			cellStreamingScratchBufferLayout._ProbeCountInChunkLine = 512;
			cellStreamingScratchBufferLayout._ProbeCountInChunkSlice = 2048;
			this.m_Layouts.Add(chunkCount, cellStreamingScratchBufferLayout);
			return cellStreamingScratchBufferLayout;
		}

		private ProbeReferenceVolume.CellStreamingScratchBuffer CreateScratchBuffer(int chunkCount, bool allocateGraphicsBuffers)
		{
			ProbeReferenceVolume.CellStreamingScratchBuffer result = new ProbeReferenceVolume.CellStreamingScratchBuffer(chunkCount, this.chunkSize, allocateGraphicsBuffers);
			this.m_CurrentlyAllocatedChunkCount += chunkCount;
			return result;
		}

		public bool AllocateScratchBuffer(int chunkCount, out ProbeReferenceVolume.CellStreamingScratchBuffer scratchBuffer, out ProbeReferenceVolume.CellStreamingScratchBufferLayout layout, bool allocateGraphicsBuffers)
		{
			ProbeVolumeScratchBufferPool.s_ChunkCount = chunkCount;
			int num = this.m_Pools.FindIndex(0, (ProbeVolumeScratchBufferPool.ScratchBufferPool o) => o.chunkCount == ProbeVolumeScratchBufferPool.s_ChunkCount);
			layout = this.GetOrCreateScratchBufferLayout(chunkCount);
			if (num == -1)
			{
				ProbeVolumeScratchBufferPool.ScratchBufferPool item = new ProbeVolumeScratchBufferPool.ScratchBufferPool(chunkCount);
				this.m_Pools.Add(item);
				this.m_Pools.Sort();
				scratchBuffer = this.CreateScratchBuffer(chunkCount, allocateGraphicsBuffers);
				return true;
			}
			Stack<ProbeReferenceVolume.CellStreamingScratchBuffer> pool = this.m_Pools[num].pool;
			if (pool.Count > 0)
			{
				scratchBuffer = pool.Pop();
				scratchBuffer.Swap();
				return true;
			}
			for (int i = num; i < this.m_Pools.Count; i++)
			{
				ProbeVolumeScratchBufferPool.ScratchBufferPool scratchBufferPool = this.m_Pools[i];
				if (scratchBufferPool.chunkCount >= chunkCount * 2)
				{
					break;
				}
				if (scratchBufferPool.pool.Count > 0)
				{
					scratchBuffer = scratchBufferPool.pool.Pop();
					scratchBuffer.Swap();
					return true;
				}
			}
			if (this.m_CurrentlyAllocatedChunkCount + chunkCount < this.maxChunkCount)
			{
				scratchBuffer = this.CreateScratchBuffer(chunkCount, allocateGraphicsBuffers);
				return true;
			}
			scratchBuffer = null;
			return false;
		}

		public void ReleaseScratchBuffer(ProbeReferenceVolume.CellStreamingScratchBuffer scratchBuffer)
		{
			if (scratchBuffer.chunkSize != this.chunkSize)
			{
				scratchBuffer.Dispose();
				return;
			}
			ProbeVolumeScratchBufferPool.s_ChunkCount = scratchBuffer.chunkCount;
			this.m_Pools.Find((ProbeVolumeScratchBufferPool.ScratchBufferPool o) => o.chunkCount == ProbeVolumeScratchBufferPool.s_ChunkCount).pool.Push(scratchBuffer);
		}

		public void Cleanup()
		{
			foreach (ProbeVolumeScratchBufferPool.ScratchBufferPool scratchBufferPool in this.m_Pools)
			{
				while (scratchBufferPool.pool.Count > 0)
				{
					scratchBufferPool.pool.Pop().Dispose();
				}
			}
			this.m_Pools.Clear();
			this.m_CurrentlyAllocatedChunkCount = 0;
			this.chunkSize = 0;
			this.maxChunkCount = 0;
		}

		private int m_L0Size;

		private int m_L1Size;

		private int m_ValiditySize;

		private int m_ValidityLayerCount;

		private int m_L2Size;

		private int m_ProbeOcclusionSize;

		private int m_SkyOcclusionSize;

		private int m_SkyShadingDirectionSize;

		private int m_CurrentlyAllocatedChunkCount;

		private List<ProbeVolumeScratchBufferPool.ScratchBufferPool> m_Pools = new List<ProbeVolumeScratchBufferPool.ScratchBufferPool>();

		private Dictionary<int, ProbeReferenceVolume.CellStreamingScratchBufferLayout> m_Layouts = new Dictionary<int, ProbeReferenceVolume.CellStreamingScratchBufferLayout>();

		private static int s_ChunkCount;

		[DebuggerDisplay("ChunkCount = {chunkCount} ElementCount = {pool.Count}")]
		private class ScratchBufferPool : IComparable<ProbeVolumeScratchBufferPool.ScratchBufferPool>
		{
			public ScratchBufferPool(int chunkCount)
			{
				this.chunkCount = chunkCount;
			}

			private ScratchBufferPool()
			{
			}

			public int CompareTo(ProbeVolumeScratchBufferPool.ScratchBufferPool other)
			{
				if (this.chunkCount < other.chunkCount)
				{
					return -1;
				}
				if (this.chunkCount > other.chunkCount)
				{
					return 1;
				}
				return 0;
			}

			public int chunkCount = -1;

			public Stack<ProbeReferenceVolume.CellStreamingScratchBuffer> pool = new Stack<ProbeReferenceVolume.CellStreamingScratchBuffer>();
		}
	}
}
