using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
	internal class ProbeBrickBlendingPool
	{
		internal static void Initialize()
		{
			if (SystemInfo.supportsComputeShaders)
			{
				ProbeVolumeRuntimeResources renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeRuntimeResources>();
				ProbeBrickBlendingPool.stateBlendShader = ((renderPipelineSettings != null) ? renderPipelineSettings.probeVolumeBlendStatesCS : null);
				ProbeBrickBlendingPool.scenarioBlendingKernel = (ProbeBrickBlendingPool.stateBlendShader ? ProbeBrickBlendingPool.stateBlendShader.FindKernel("BlendScenarios") : -1);
			}
		}

		internal bool isAllocated
		{
			get
			{
				return this.m_State0 != null;
			}
		}

		internal int estimatedVMemCost
		{
			get
			{
				if (!ProbeReferenceVolume.instance.supportScenarioBlending)
				{
					return 0;
				}
				if (this.isAllocated)
				{
					return this.m_State0.estimatedVMemCost + this.m_State1.estimatedVMemCost;
				}
				return ProbeBrickPool.EstimateMemoryCostForBlending(this.m_MemoryBudget, false, this.m_ShBands) * 2;
			}
		}

		internal int GetPoolWidth()
		{
			return this.m_State0.m_Pool.width;
		}

		internal int GetPoolHeight()
		{
			return this.m_State0.m_Pool.height;
		}

		internal int GetPoolDepth()
		{
			return this.m_State0.m_Pool.depth;
		}

		internal ProbeBrickBlendingPool(ProbeVolumeBlendingTextureMemoryBudget memoryBudget, ProbeVolumeSHBands shBands, bool probeOcclusion)
		{
			this.m_MemoryBudget = (ProbeVolumeTextureMemoryBudget)memoryBudget;
			this.m_ShBands = shBands;
			this.m_ProbeOcclusion = probeOcclusion;
		}

		internal void AllocateResourcesIfNeeded()
		{
			if (this.isAllocated)
			{
				return;
			}
			this.m_State0 = new ProbeBrickPool(this.m_MemoryBudget, this.m_ShBands, false, false, false, false, this.m_ProbeOcclusion);
			this.m_State1 = new ProbeBrickPool(this.m_MemoryBudget, this.m_ShBands, false, false, false, false, this.m_ProbeOcclusion);
			int num = this.GetPoolWidth() / 512 * (this.GetPoolHeight() / 4) * (this.GetPoolDepth() / 4);
			this.m_ChunkList = new Vector4[num];
			this.m_MappedChunks = 0;
		}

		internal void Update(ProbeBrickPool.DataLocation source, List<ProbeBrickPool.BrickChunkAlloc> srcLocations, List<ProbeBrickPool.BrickChunkAlloc> dstLocations, int destStartIndex, ProbeVolumeSHBands bands, int state)
		{
			((state == 0) ? this.m_State0 : this.m_State1).Update(source, srcLocations, dstLocations, destStartIndex, bands);
		}

		internal void Update(CommandBuffer cmd, ProbeReferenceVolume.CellStreamingScratchBuffer dataBuffer, ProbeReferenceVolume.CellStreamingScratchBufferLayout layout, List<ProbeBrickPool.BrickChunkAlloc> dstLocations, ProbeVolumeSHBands bands, int state, Texture validityTexture, bool skyOcclusion, Texture skyOcclusionTexture, bool skyShadingDirections, Texture skyShadingDirectionsTexture, bool probeOcclusion)
		{
			bool flag = state == 0;
			((state == 0) ? this.m_State0 : this.m_State1).Update(cmd, dataBuffer, layout, dstLocations, flag, validityTexture, bands, flag && skyOcclusion, skyOcclusionTexture, flag && skyShadingDirections, skyShadingDirectionsTexture, probeOcclusion);
		}

		internal void PerformBlending(CommandBuffer cmd, float factor, ProbeBrickPool dstPool)
		{
			if (this.m_MappedChunks == 0)
			{
				return;
			}
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L0_L1Rx, this.m_State0.m_Pool.TexL0_L1rx);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L1G_L1Ry, this.m_State0.m_Pool.TexL1_G_ry);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L1B_L1Rz, this.m_State0.m_Pool.TexL1_B_rz);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L0_L1Rx, this.m_State1.m_Pool.TexL0_L1rx);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L1G_L1Ry, this.m_State1.m_Pool.TexL1_G_ry);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L1B_L1Rz, this.m_State1.m_Pool.TexL1_B_rz);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L0_L1Rx, dstPool.m_Pool.TexL0_L1rx);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L1G_L1Ry, dstPool.m_Pool.TexL1_G_ry);
			cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L1B_L1Rz, dstPool.m_Pool.TexL1_B_rz);
			if (this.m_ShBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				ProbeBrickBlendingPool.stateBlendShader.EnableKeyword("PROBE_VOLUMES_L2");
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L2_0, this.m_State0.m_Pool.TexL2_0);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L2_1, this.m_State0.m_Pool.TexL2_1);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L2_2, this.m_State0.m_Pool.TexL2_2);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_L2_3, this.m_State0.m_Pool.TexL2_3);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L2_0, this.m_State1.m_Pool.TexL2_0);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L2_1, this.m_State1.m_Pool.TexL2_1);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L2_2, this.m_State1.m_Pool.TexL2_2);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_L2_3, this.m_State1.m_Pool.TexL2_3);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L2_0, dstPool.m_Pool.TexL2_0);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L2_1, dstPool.m_Pool.TexL2_1);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L2_2, dstPool.m_Pool.TexL2_2);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_L2_3, dstPool.m_Pool.TexL2_3);
			}
			else
			{
				ProbeBrickBlendingPool.stateBlendShader.DisableKeyword("PROBE_VOLUMES_L2");
			}
			if (this.m_ProbeOcclusion)
			{
				ProbeBrickBlendingPool.stateBlendShader.EnableKeyword("USE_APV_PROBE_OCCLUSION");
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State0_ProbeOcclusion, this.m_State0.m_Pool.TexProbeOcclusion);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickBlendingPool._State1_ProbeOcclusion, this.m_State1.m_Pool.TexProbeOcclusion);
				cmd.SetComputeTextureParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, ProbeBrickPool._Out_ProbeOcclusion, dstPool.m_Pool.TexProbeOcclusion);
			}
			else
			{
				ProbeBrickBlendingPool.stateBlendShader.DisableKeyword("USE_APV_PROBE_OCCLUSION");
			}
			Vector4 val = new Vector4((float)dstPool.GetPoolWidth(), (float)dstPool.GetPoolHeight(), factor, 0f);
			int threadGroupsX = ProbeBrickPool.DivRoundUp(512, 4);
			int threadGroupsY = ProbeBrickPool.DivRoundUp(4, 4);
			int num = ProbeBrickPool.DivRoundUp(4, 4);
			cmd.SetComputeVectorArrayParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool._ChunkList, this.m_ChunkList);
			cmd.SetComputeVectorParam(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool._PoolDim_LerpFactor, val);
			cmd.DispatchCompute(ProbeBrickBlendingPool.stateBlendShader, ProbeBrickBlendingPool.scenarioBlendingKernel, threadGroupsX, threadGroupsY, num * this.m_MappedChunks);
			this.m_MappedChunks = 0;
		}

		internal void BlendChunks(ProbeReferenceVolume.Cell cell, ProbeBrickPool dstPool)
		{
			for (int i = 0; i < cell.blendingInfo.chunkList.Count; i++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = cell.blendingInfo.chunkList[i];
				int num = cell.poolInfo.chunkList[i].flattenIndex(dstPool.GetPoolWidth(), dstPool.GetPoolHeight());
				Vector4[] chunkList = this.m_ChunkList;
				int mappedChunks = this.m_MappedChunks;
				this.m_MappedChunks = mappedChunks + 1;
				chunkList[mappedChunks] = new Vector4((float)brickChunkAlloc.x, (float)brickChunkAlloc.y, (float)brickChunkAlloc.z, (float)num);
			}
		}

		internal void Clear()
		{
			ProbeBrickPool state = this.m_State0;
			if (state == null)
			{
				return;
			}
			state.Clear();
		}

		internal bool Allocate(int numberOfBrickChunks, List<ProbeBrickPool.BrickChunkAlloc> outAllocations)
		{
			this.AllocateResourcesIfNeeded();
			return numberOfBrickChunks <= this.m_State0.GetRemainingChunkCount() && this.m_State0.Allocate(numberOfBrickChunks, outAllocations, false);
		}

		internal void Deallocate(List<ProbeBrickPool.BrickChunkAlloc> allocations)
		{
			if (allocations.Count == 0)
			{
				return;
			}
			this.m_State0.Deallocate(allocations);
		}

		internal void EnsureTextureValidity()
		{
			if (this.isAllocated)
			{
				this.m_State0.EnsureTextureValidity();
				this.m_State1.EnsureTextureValidity();
			}
		}

		internal void Cleanup()
		{
			if (this.isAllocated)
			{
				this.m_State0.Cleanup();
				this.m_State1.Cleanup();
			}
		}

		private static ComputeShader stateBlendShader;

		private static int scenarioBlendingKernel = -1;

		private static readonly int _PoolDim_LerpFactor = Shader.PropertyToID("_PoolDim_LerpFactor");

		private static readonly int _ChunkList = Shader.PropertyToID("_ChunkList");

		private static readonly int _State0_L0_L1Rx = Shader.PropertyToID("_State0_L0_L1Rx");

		private static readonly int _State0_L1G_L1Ry = Shader.PropertyToID("_State0_L1G_L1Ry");

		private static readonly int _State0_L1B_L1Rz = Shader.PropertyToID("_State0_L1B_L1Rz");

		private static readonly int _State0_L2_0 = Shader.PropertyToID("_State0_L2_0");

		private static readonly int _State0_L2_1 = Shader.PropertyToID("_State0_L2_1");

		private static readonly int _State0_L2_2 = Shader.PropertyToID("_State0_L2_2");

		private static readonly int _State0_L2_3 = Shader.PropertyToID("_State0_L2_3");

		private static readonly int _State0_ProbeOcclusion = Shader.PropertyToID("_State0_ProbeOcclusion");

		private static readonly int _State1_L0_L1Rx = Shader.PropertyToID("_State1_L0_L1Rx");

		private static readonly int _State1_L1G_L1Ry = Shader.PropertyToID("_State1_L1G_L1Ry");

		private static readonly int _State1_L1B_L1Rz = Shader.PropertyToID("_State1_L1B_L1Rz");

		private static readonly int _State1_L2_0 = Shader.PropertyToID("_State1_L2_0");

		private static readonly int _State1_L2_1 = Shader.PropertyToID("_State1_L2_1");

		private static readonly int _State1_L2_2 = Shader.PropertyToID("_State1_L2_2");

		private static readonly int _State1_L2_3 = Shader.PropertyToID("_State1_L2_3");

		private static readonly int _State1_ProbeOcclusion = Shader.PropertyToID("_State1_ProbeOcclusion");

		private Vector4[] m_ChunkList;

		private int m_MappedChunks;

		private ProbeBrickPool m_State0;

		private ProbeBrickPool m_State1;

		private ProbeVolumeTextureMemoryBudget m_MemoryBudget;

		private ProbeVolumeSHBands m_ShBands;

		private bool m_ProbeOcclusion;
	}
}
