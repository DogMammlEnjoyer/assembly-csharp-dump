using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering
{
	public class ProbeReferenceVolume
	{
		public void BindAPVRuntimeResources(CommandBuffer cmdBuffer, bool isProbeVolumeEnabled)
		{
			bool flag = true;
			ProbeReferenceVolume instance = ProbeReferenceVolume.instance;
			if (isProbeVolumeEnabled && this.m_ProbeReferenceVolumeInit)
			{
				ProbeReferenceVolume.RuntimeResources runtimeResources = instance.GetRuntimeResources();
				if ((runtimeResources.index != null && runtimeResources.L0_L1rx != null && runtimeResources.L1_G_ry != null && runtimeResources.L1_B_rz != null) & ((instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2 && runtimeResources.L2_0 != null) || instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL1))
				{
					cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._APVResIndex, runtimeResources.index);
					cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._APVResCellIndices, runtimeResources.cellIndices);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL0_L1Rx, runtimeResources.L0_L1rx);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL1G_L1Ry, runtimeResources.L1_G_ry);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL1B_L1Rz, runtimeResources.L1_B_rz);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResValidity, runtimeResources.Validity);
					int skyOcclusionTexL0L = ProbeReferenceVolume.ShaderIDs._SkyOcclusionTexL0L1;
					RenderTexture renderTexture = runtimeResources.SkyOcclusionL0L1;
					cmdBuffer.SetGlobalTexture(skyOcclusionTexL0L, (renderTexture != null) ? renderTexture : CoreUtils.blackVolumeTexture);
					int skyShadingDirectionIndicesTex = ProbeReferenceVolume.ShaderIDs._SkyShadingDirectionIndicesTex;
					renderTexture = runtimeResources.SkyShadingDirectionIndices;
					cmdBuffer.SetGlobalTexture(skyShadingDirectionIndicesTex, (renderTexture != null) ? renderTexture : CoreUtils.blackVolumeTexture);
					cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._SkyPrecomputedDirections, runtimeResources.SkyPrecomputedDirections);
					cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._AntiLeakData, runtimeResources.QualityLeakReductionData);
					if (instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
					{
						cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_0, runtimeResources.L2_0);
						cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_1, runtimeResources.L2_1);
						cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_2, runtimeResources.L2_2);
						cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_3, runtimeResources.L2_3);
					}
					int apvprobeOcclusion = ProbeReferenceVolume.ShaderIDs._APVProbeOcclusion;
					renderTexture = runtimeResources.ProbeOcclusion;
					cmdBuffer.SetGlobalTexture(apvprobeOcclusion, (renderTexture != null) ? renderTexture : CoreUtils.whiteVolumeTexture);
					flag = false;
				}
			}
			if (flag)
			{
				if (this.m_EmptyIndexBuffer == null)
				{
					this.m_EmptyIndexBuffer = new ComputeBuffer(1, 12, ComputeBufferType.Structured);
				}
				cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._APVResIndex, this.m_EmptyIndexBuffer);
				cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._APVResCellIndices, this.m_EmptyIndexBuffer);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL0_L1Rx, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL1G_L1Ry, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL1B_L1Rz, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResValidity, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._SkyOcclusionTexL0L1, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._SkyShadingDirectionIndicesTex, CoreUtils.blackVolumeTexture);
				cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._SkyPrecomputedDirections, this.m_EmptyIndexBuffer);
				cmdBuffer.SetGlobalBuffer(ProbeReferenceVolume.ShaderIDs._AntiLeakData, this.m_EmptyIndexBuffer);
				if (instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
				{
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_0, CoreUtils.blackVolumeTexture);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_1, CoreUtils.blackVolumeTexture);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_2, CoreUtils.blackVolumeTexture);
					cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVResL2_3, CoreUtils.blackVolumeTexture);
				}
				cmdBuffer.SetGlobalTexture(ProbeReferenceVolume.ShaderIDs._APVProbeOcclusion, CoreUtils.whiteVolumeTexture);
			}
		}

		public bool UpdateShaderVariablesProbeVolumes(CommandBuffer cmd, ProbeVolumesOptions probeVolumeOptions, int taaFrameIndex, bool supportRenderingLayers = false)
		{
			bool flag = this.DataHasBeenLoaded();
			if (flag)
			{
				ProbeVolumeShadingParameters parameters;
				parameters.normalBias = probeVolumeOptions.normalBias.value;
				parameters.viewBias = probeVolumeOptions.viewBias.value;
				parameters.scaleBiasByMinDistanceBetweenProbes = probeVolumeOptions.scaleBiasWithMinProbeDistance.value;
				parameters.samplingNoise = probeVolumeOptions.samplingNoise.value;
				parameters.weight = probeVolumeOptions.intensityMultiplier.value;
				parameters.leakReductionMode = probeVolumeOptions.leakReductionMode.value;
				parameters.frameIndexForNoise = taaFrameIndex * (probeVolumeOptions.animateSamplingNoise.value ? 1 : 0);
				parameters.reflNormalizationLowerClamp = 0.005f;
				parameters.reflNormalizationUpperClamp = (probeVolumeOptions.occlusionOnlyReflectionNormalization.value ? 1f : 7f);
				parameters.skyOcclusionIntensity = (this.skyOcclusion ? probeVolumeOptions.skyOcclusionIntensityMultiplier.value : 0f);
				parameters.skyOcclusionShadingDirection = (this.skyOcclusion && this.skyOcclusionShadingDirection);
				parameters.regionCount = ((this.m_CurrentBakingSet != null) ? this.m_CurrentBakingSet.bakedMaskCount : 0);
				parameters.regionLayerMasks = ((supportRenderingLayers && this.m_CurrentBakingSet != null) ? this.m_CurrentBakingSet.bakedLayerMasks : uint.MaxValue);
				parameters.worldOffset = probeVolumeOptions.worldOffset.value;
				this.UpdateConstantBuffer(cmd, parameters);
			}
			return flag;
		}

		internal Bounds globalBounds
		{
			get
			{
				return this.m_CurrGlobalBounds;
			}
			set
			{
				this.m_CurrGlobalBounds = value;
			}
		}

		private ProbeVolumeBakingSet m_CurrentBakingSet
		{
			get
			{
				return this.m_CurrentBakingSetReference.Get();
			}
			set
			{
				this.m_CurrentBakingSetReference.Set(value);
			}
		}

		private ProbeVolumeBakingSet m_LazyBakingSet
		{
			get
			{
				return this.m_LazyBakingSetReference.Get();
			}
			set
			{
				this.m_LazyBakingSetReference.Set(value);
			}
		}

		public bool isInitialized
		{
			get
			{
				return this.m_IsInitialized;
			}
		}

		internal bool enabledBySRP
		{
			get
			{
				return this.m_EnabledBySRP;
			}
		}

		internal bool vertexSampling
		{
			get
			{
				return this.m_VertexSampling;
			}
		}

		internal bool hasUnloadedCells
		{
			get
			{
				return this.m_ToBeLoadedCells.size != 0;
			}
		}

		internal bool supportLightingScenarios
		{
			get
			{
				return this.m_SupportScenarios;
			}
		}

		internal bool supportScenarioBlending
		{
			get
			{
				return this.m_SupportScenarioBlending;
			}
		}

		internal bool gpuStreamingEnabled
		{
			get
			{
				return this.m_SupportGPUStreaming;
			}
		}

		internal bool diskStreamingEnabled
		{
			get
			{
				return this.m_SupportDiskStreaming && !this.m_ForceNoDiskStreaming;
			}
		}

		public bool probeOcclusion
		{
			get
			{
				return this.m_CurrentBakingSet && this.m_CurrentBakingSet.bakedProbeOcclusion;
			}
		}

		public bool skyOcclusion
		{
			get
			{
				return this.m_CurrentBakingSet && this.m_CurrentBakingSet.bakedSkyOcclusion;
			}
		}

		public bool skyOcclusionShadingDirection
		{
			get
			{
				return this.m_CurrentBakingSet && this.m_CurrentBakingSet.bakedSkyShadingDirection;
			}
		}

		private bool useRenderingLayers
		{
			get
			{
				return this.m_CurrentBakingSet.bakedMaskCount != 1;
			}
		}

		public ProbeVolumeSHBands shBands
		{
			get
			{
				return this.m_SHBands;
			}
		}

		public ProbeVolumeBakingSet currentBakingSet
		{
			get
			{
				return this.m_CurrentBakingSet;
			}
		}

		public string lightingScenario
		{
			get
			{
				if (!this.m_CurrentBakingSet)
				{
					return null;
				}
				return this.m_CurrentBakingSet.lightingScenario;
			}
			set
			{
				this.SetActiveScenario(value, true);
			}
		}

		public string otherScenario
		{
			get
			{
				if (!this.m_CurrentBakingSet)
				{
					return null;
				}
				return this.m_CurrentBakingSet.otherScenario;
			}
		}

		public float scenarioBlendingFactor
		{
			get
			{
				if (!this.m_CurrentBakingSet)
				{
					return 0f;
				}
				return this.m_CurrentBakingSet.scenarioBlendingFactor;
			}
			set
			{
				if (this.m_CurrentBakingSet != null)
				{
					this.m_CurrentBakingSet.BlendLightingScenario(this.m_CurrentBakingSet.otherScenario, value);
				}
			}
		}

		internal static string GetSceneGUID(Scene scene)
		{
			return scene.GetGUID();
		}

		internal void SetActiveScenario(string scenario, bool verbose = true)
		{
			if (this.m_CurrentBakingSet != null)
			{
				this.m_CurrentBakingSet.SetActiveScenario(scenario, verbose);
			}
		}

		public void BlendLightingScenario(string otherScenario, float blendingFactor)
		{
			if (this.m_CurrentBakingSet != null)
			{
				this.m_CurrentBakingSet.BlendLightingScenario(otherScenario, blendingFactor);
			}
		}

		public ProbeVolumeTextureMemoryBudget memoryBudget
		{
			get
			{
				return this.m_MemoryBudget;
			}
		}

		internal List<ProbeVolumePerSceneData> perSceneDataList { get; private set; } = new List<ProbeVolumePerSceneData>();

		internal void RegisterPerSceneData(ProbeVolumePerSceneData data)
		{
			if (!this.perSceneDataList.Contains(data))
			{
				this.perSceneDataList.Add(data);
				if (this.m_IsInitialized)
				{
					data.Initialize();
				}
			}
		}

		internal bool ScheduleBakingSet(ProbeVolumeBakingSet bakingSet)
		{
			if (this.m_IsInitialized)
			{
				return false;
			}
			this.m_LazyBakingSet = bakingSet;
			return true;
		}

		internal bool ProcessScheduledBakingSet()
		{
			if (this.m_LazyBakingSet == null)
			{
				return false;
			}
			this.SetActiveBakingSet(this.m_LazyBakingSet);
			this.m_LazyBakingSet = null;
			return true;
		}

		public void SetActiveScene(Scene scene)
		{
			ProbeVolumePerSceneData probeVolumePerSceneData;
			if (this.TryGetPerSceneData(ProbeReferenceVolume.GetSceneGUID(scene), out probeVolumePerSceneData))
			{
				this.SetActiveBakingSet(probeVolumePerSceneData.serializedBakingSet);
			}
		}

		public void SetActiveBakingSet(ProbeVolumeBakingSet bakingSet)
		{
			if (this.m_CurrentBakingSet == bakingSet)
			{
				return;
			}
			if (this.ScheduleBakingSet(bakingSet))
			{
				return;
			}
			foreach (ProbeVolumePerSceneData probeVolumePerSceneData in this.perSceneDataList)
			{
				probeVolumePerSceneData.QueueSceneRemoval();
			}
			this.UnloadBakingSet();
			this.SetBakingSetAsCurrent(bakingSet);
			if (this.m_CurrentBakingSet != null)
			{
				foreach (ProbeVolumePerSceneData probeVolumePerSceneData2 in this.perSceneDataList)
				{
					probeVolumePerSceneData2.QueueSceneLoading();
				}
			}
		}

		private void SetBakingSetAsCurrent(ProbeVolumeBakingSet bakingSet)
		{
			this.m_CurrentBakingSet = bakingSet;
			if (this.m_CurrentBakingSet != null)
			{
				this.InitProbeReferenceVolume();
				this.m_CurrentBakingSet.Initialize(this.m_UseStreamingAssets);
				this.m_CurrGlobalBounds = this.m_CurrentBakingSet.globalBounds;
				this.SetSubdivisionDimensions(bakingSet.minBrickSize, bakingSet.maxSubdivision, bakingSet.bakedProbeOffset);
				this.m_NeedsIndexRebuild = true;
			}
		}

		internal void RegisterBakingSet(ProbeVolumePerSceneData data)
		{
			if (this.m_CurrentBakingSet == null)
			{
				this.SetBakingSetAsCurrent(data.serializedBakingSet);
			}
		}

		internal void UnloadBakingSet()
		{
			this.PerformPendingOperations();
			if (this.m_CurrentBakingSet != null)
			{
				this.m_CurrentBakingSet.Cleanup();
			}
			this.m_CurrentBakingSet = null;
			this.m_CurrGlobalBounds = default(Bounds);
			if (this.m_ScratchBufferPool != null)
			{
				this.m_ScratchBufferPool.Cleanup();
				this.m_ScratchBufferPool = null;
			}
		}

		internal void UnregisterPerSceneData(ProbeVolumePerSceneData data)
		{
			this.perSceneDataList.Remove(data);
			if (this.perSceneDataList.Count == 0)
			{
				this.UnloadBakingSet();
			}
		}

		internal bool TryGetPerSceneData(string sceneGUID, out ProbeVolumePerSceneData perSceneData)
		{
			foreach (ProbeVolumePerSceneData probeVolumePerSceneData in this.perSceneDataList)
			{
				if (ProbeReferenceVolume.GetSceneGUID(probeVolumePerSceneData.gameObject.scene) == sceneGUID)
				{
					perSceneData = probeVolumePerSceneData;
					return true;
				}
			}
			perSceneData = null;
			return false;
		}

		internal float indexFragmentationRate
		{
			get
			{
				if (!this.m_ProbeReferenceVolumeInit)
				{
					return 0f;
				}
				return this.m_Index.fragmentationRate;
			}
		}

		public static ProbeReferenceVolume instance
		{
			get
			{
				return ProbeReferenceVolume._instance;
			}
		}

		public void Initialize(in ProbeVolumeSystemParameters parameters)
		{
			if (this.m_IsInitialized)
			{
				Debug.LogError("Probe Volume System has already been initialized.");
				return;
			}
			ProbeVolumeGlobalSettings renderPipelineSettings = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeGlobalSettings>();
			this.m_MemoryBudget = parameters.memoryBudget;
			this.m_BlendingMemoryBudget = parameters.blendingMemoryBudget;
			this.m_SupportScenarios = parameters.supportScenarios;
			this.m_SupportScenarioBlending = (parameters.supportScenarios && parameters.supportScenarioBlending && SystemInfo.supportsComputeShaders && this.m_BlendingMemoryBudget > (ProbeVolumeBlendingTextureMemoryBudget)0);
			this.m_SHBands = parameters.shBands;
			this.m_UseStreamingAssets = !renderPipelineSettings.probeVolumeDisableStreamingAssets;
			this.m_SupportGPUStreaming = parameters.supportGPUStreaming;
			ProbeVolumeRuntimeResources renderPipelineSettings2 = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeRuntimeResources>();
			ComputeShader x = (renderPipelineSettings2 != null) ? renderPipelineSettings2.probeVolumeUploadDataCS : null;
			ProbeVolumeRuntimeResources renderPipelineSettings3 = GraphicsSettings.GetRenderPipelineSettings<ProbeVolumeRuntimeResources>();
			ComputeShader x2 = (renderPipelineSettings3 != null) ? renderPipelineSettings3.probeVolumeUploadDataL2CS : null;
			this.m_SupportDiskStreaming = (parameters.supportDiskStreaming && SystemInfo.supportsComputeShaders && this.m_SupportGPUStreaming && this.m_UseStreamingAssets && x != null && x2 != null);
			this.m_DiskStreamingUseCompute = (SystemInfo.supportsComputeShaders && x != null && x2 != null);
			this.InitializeDebug();
			ProbeVolumeConstantRuntimeResources.Initialize();
			ProbeBrickPool.Initialize();
			ProbeBrickBlendingPool.Initialize();
			this.InitStreaming();
			this.m_IsInitialized = true;
			this.m_NeedsIndexRebuild = true;
			this.sceneData = parameters.sceneData;
			this.m_EnabledBySRP = true;
			foreach (ProbeVolumePerSceneData probeVolumePerSceneData in this.perSceneDataList)
			{
				probeVolumePerSceneData.Initialize();
			}
			this.ProcessScheduledBakingSet();
		}

		public void SetEnableStateFromSRP(bool srpEnablesPV)
		{
			this.m_EnabledBySRP = srpEnablesPV;
		}

		public void SetVertexSamplingEnabled(bool value)
		{
			this.m_VertexSampling = value;
		}

		internal void ForceMemoryBudget(ProbeVolumeTextureMemoryBudget budget)
		{
			this.m_MemoryBudget = budget;
		}

		internal void ForceSHBand(ProbeVolumeSHBands shBands)
		{
			this.m_SHBands = shBands;
			this.DeinitProbeReferenceVolume();
			foreach (ProbeVolumePerSceneData probeVolumePerSceneData in this.perSceneDataList)
			{
				probeVolumePerSceneData.Initialize();
			}
			this.PerformPendingOperations();
		}

		internal void ForceNoDiskStreaming(bool state)
		{
			this.m_ForceNoDiskStreaming = state;
		}

		public void Cleanup()
		{
			CoreUtils.SafeRelease(this.m_EmptyIndexBuffer);
			this.m_EmptyIndexBuffer = null;
			ProbeVolumeConstantRuntimeResources.Cleanup();
			if (!this.m_IsInitialized)
			{
				Debug.LogError("Adaptive Probe Volumes have not been initialized before calling Cleanup.");
				return;
			}
			this.CleanupLoadedData();
			this.CleanupDebug();
			this.CleanupStreaming();
			this.DeinitProbeReferenceVolume();
			this.m_IsInitialized = false;
		}

		public int GetVideoMemoryCost()
		{
			if (!this.m_ProbeReferenceVolumeInit)
			{
				return 0;
			}
			return this.m_Pool.estimatedVMemCost + this.m_Index.estimatedVMemCost + this.m_CellIndices.estimatedVMemCost + this.m_BlendingPool.estimatedVMemCost + this.m_TemporaryDataLocationMemCost;
		}

		private void RemoveCell(int cellIndex)
		{
			ProbeReferenceVolume.Cell cell;
			if (this.cells.TryGetValue(cellIndex, out cell))
			{
				cell.referenceCount--;
				if (cell.referenceCount <= 0)
				{
					this.cells.Remove(cellIndex);
					if (cell.loaded)
					{
						this.m_LoadedCells.Remove(cell);
						this.UnloadCell(cell);
					}
					else
					{
						this.m_ToBeLoadedCells.Remove(cell);
					}
					this.m_CurrentBakingSet.ReleaseCell(cellIndex);
					this.m_CellPool.Release(cell);
				}
			}
		}

		internal void UnloadCell(ProbeReferenceVolume.Cell cell)
		{
			if (cell.loaded)
			{
				if (cell.blendingInfo.blending)
				{
					this.m_LoadedBlendingCells.Remove(cell);
					this.UnloadBlendingCell(cell);
				}
				else
				{
					this.m_ToBeLoadedBlendingCells.Remove(cell);
				}
				if (cell.indexInfo.flatIndicesInGlobalIndirection != null)
				{
					this.m_CellIndices.MarkEntriesAsUnloaded(cell.indexInfo.flatIndicesInGlobalIndirection);
				}
				if (this.diskStreamingEnabled)
				{
					if (cell.streamingInfo.IsStreaming())
					{
						this.CancelStreamingRequest(cell);
					}
					else
					{
						this.ReleaseBricks(cell);
						cell.data.Cleanup(!this.diskStreamingEnabled);
					}
				}
				else
				{
					this.ReleaseBricks(cell);
				}
				cell.loaded = false;
				cell.debugProbes = null;
				this.ClearDebugData();
			}
		}

		internal void UnloadBlendingCell(ProbeReferenceVolume.Cell cell)
		{
			if (this.diskStreamingEnabled && cell.streamingInfo.IsBlendingStreaming())
			{
				this.CancelBlendingStreamingRequest(cell);
			}
			if (cell.blendingInfo.blending)
			{
				this.m_BlendingPool.Deallocate(cell.blendingInfo.chunkList);
				cell.blendingInfo.chunkList.Clear();
				cell.blendingInfo.blending = false;
			}
		}

		internal unsafe void UnloadAllCells()
		{
			for (int i = 0; i < this.m_LoadedCells.size; i++)
			{
				this.UnloadCell(*this.m_LoadedCells[i]);
			}
			this.m_ToBeLoadedCells.AddRange(this.m_LoadedCells);
			this.m_LoadedCells.Clear();
		}

		internal unsafe void UnloadAllBlendingCells()
		{
			for (int i = 0; i < this.m_LoadedBlendingCells.size; i++)
			{
				this.UnloadBlendingCell(*this.m_LoadedBlendingCells[i]);
			}
			this.m_ToBeLoadedBlendingCells.AddRange(this.m_LoadedBlendingCells);
			this.m_LoadedBlendingCells.Clear();
		}

		private void AddCell(int cellIndex)
		{
			ProbeReferenceVolume.Cell cell;
			if (!this.cells.TryGetValue(cellIndex, out cell))
			{
				ProbeReferenceVolume.CellDesc cellDesc = this.m_CurrentBakingSet.GetCellDesc(cellIndex);
				if (cellDesc != null)
				{
					cell = this.m_CellPool.Get();
					cell.desc = cellDesc;
					cell.data = this.m_CurrentBakingSet.GetCellData(cellIndex);
					cell.poolInfo.shChunkCount = cell.desc.shChunkCount;
					cell.indexInfo.flatIndicesInGlobalIndirection = this.m_CellIndices.GetFlatIndicesForCell(cellDesc.position);
					cell.indexInfo.indexChunkCount = cell.desc.indexChunkCount;
					cell.indexInfo.indirectionEntryInfo = cell.desc.indirectionEntryInfo;
					cell.indexInfo.updateInfo.entriesInfo = new ProbeBrickIndex.IndirectionEntryUpdateInfo[cellDesc.indirectionEntryInfo.Length];
					cell.referenceCount = 1;
					this.cells[cellIndex] = cell;
					this.m_ToBeLoadedCells.Add(cell);
					return;
				}
			}
			else
			{
				cell.referenceCount++;
			}
		}

		internal bool LoadCell(ProbeReferenceVolume.Cell cell, bool ignoreErrorLog = false)
		{
			if (!this.ReservePoolChunks(cell.desc.bricksCount, cell.poolInfo.chunkList, ignoreErrorLog))
			{
				return false;
			}
			int num = cell.indexInfo.indirectionEntryInfo.Length;
			ProbeReferenceVolume.CellIndexInfo indexInfo = cell.indexInfo;
			for (int i = 0; i < num; i++)
			{
				if (!cell.indexInfo.indirectionEntryInfo[i].hasMinMax)
				{
					NativeArray<ProbeBrickIndex.Brick> bricks = cell.data.bricks;
					if (bricks.IsCreated)
					{
						ProbeReferenceVolume.IndirectionEntryInfo[] indirectionEntryInfo = cell.indexInfo.indirectionEntryInfo;
						int num2 = i;
						bricks = cell.data.bricks;
						this.ComputeEntryMinMax(ref indirectionEntryInfo[num2], bricks);
					}
					else
					{
						int num3 = ProbeReferenceVolume.CellSize(this.GetEntrySubdivLevel());
						cell.indexInfo.indirectionEntryInfo[i].minBrickPos = Vector3Int.zero;
						cell.indexInfo.indirectionEntryInfo[i].maxBrickPosPlusOne = new Vector3Int(num3 + 1, num3 + 1, num3 + 1);
						cell.indexInfo.indirectionEntryInfo[i].hasMinMax = true;
					}
				}
				int numberOfBricksAtSubdiv = ProbeReferenceVolume.GetNumberOfBricksAtSubdiv(cell.indexInfo.indirectionEntryInfo[i]);
				indexInfo.updateInfo.entriesInfo[i].numberOfChunks = this.m_Index.GetNumberOfChunks(numberOfBricksAtSubdiv);
			}
			if (this.m_Index.FindSlotsForEntries(ref indexInfo.updateInfo.entriesInfo))
			{
				bool flag = cell.UpdateCellScenarioData(this.lightingScenario, this.otherScenario);
				this.m_Index.ReserveChunks(indexInfo.updateInfo.entriesInfo, ignoreErrorLog);
				for (int j = 0; j < num; j++)
				{
					indexInfo.updateInfo.entriesInfo[j].minValidBrickIndexForCellAtMaxRes = indexInfo.indirectionEntryInfo[j].minBrickPos;
					indexInfo.updateInfo.entriesInfo[j].maxValidBrickIndexForCellAtMaxResPlusOne = indexInfo.indirectionEntryInfo[j].maxBrickPosPlusOne;
					indexInfo.updateInfo.entriesInfo[j].entryPositionInBricksAtMaxRes = indexInfo.indirectionEntryInfo[j].positionInBricks;
					indexInfo.updateInfo.entriesInfo[j].minSubdivInCell = indexInfo.indirectionEntryInfo[j].minSubdiv;
					indexInfo.updateInfo.entriesInfo[j].hasOnlyBiggerBricks = indexInfo.indirectionEntryInfo[j].hasOnlyBiggerBricks;
				}
				cell.loaded = true;
				if (flag)
				{
					this.AddBricks(cell);
				}
				this.minLoadedCellPos = Vector3Int.Min(this.minLoadedCellPos, cell.desc.position);
				this.maxLoadedCellPos = Vector3Int.Max(this.maxLoadedCellPos, cell.desc.position);
				this.ClearDebugData();
				return true;
			}
			this.ReleasePoolChunks(cell.poolInfo.chunkList);
			this.StartIndexDefragmentation();
			return false;
		}

		internal unsafe void LoadAllCells()
		{
			int size = this.m_LoadedCells.size;
			for (int i = 0; i < this.m_ToBeLoadedCells.size; i++)
			{
				ProbeReferenceVolume.Cell cell = *this.m_ToBeLoadedCells[i];
				if (this.LoadCell(cell, true))
				{
					this.m_LoadedCells.Add(cell);
				}
			}
			for (int j = size; j < this.m_LoadedCells.size; j++)
			{
				this.m_ToBeLoadedCells.Remove(*this.m_LoadedCells[j]);
			}
		}

		private void ComputeCellGlobalInfo()
		{
			this.minLoadedCellPos = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
			this.maxLoadedCellPos = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
			foreach (ProbeReferenceVolume.Cell cell in this.cells.Values)
			{
				if (cell.loaded)
				{
					this.minLoadedCellPos = Vector3Int.Min(cell.desc.position, this.minLoadedCellPos);
					this.maxLoadedCellPos = Vector3Int.Max(cell.desc.position, this.maxLoadedCellPos);
				}
			}
		}

		internal void AddPendingSceneLoading(string sceneGUID, ProbeVolumeBakingSet bakingSet)
		{
			if (this.m_PendingScenesToBeLoaded.ContainsKey(sceneGUID))
			{
				this.m_PendingScenesToBeLoaded.Remove(sceneGUID);
			}
			if (bakingSet == null && this.m_CurrentBakingSet != null && this.m_CurrentBakingSet.singleSceneMode)
			{
				return;
			}
			if (bakingSet.chunkSizeInBricks != ProbeBrickPool.GetChunkSizeInBrickCount())
			{
				Debug.LogError("Trying to load Adaptive Probe Volumes data (" + bakingSet.name + ") baked with an older incompatible version of APV. Please rebake your data.");
				return;
			}
			if (this.m_CurrentBakingSet != null && !this.m_CurrentBakingSet.HasSameSceneGUIDs(bakingSet))
			{
				return;
			}
			if (this.m_PendingScenesToBeLoaded.Count != 0)
			{
				using (Dictionary<string, ValueTuple<ProbeVolumeBakingSet, List<int>>>.ValueCollection.Enumerator enumerator = this.m_PendingScenesToBeLoaded.Values.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ValueTuple<ProbeVolumeBakingSet, List<int>> valueTuple = enumerator.Current;
						if (bakingSet != valueTuple.Item1)
						{
							Debug.LogError("Trying to load Adaptive Probe Volumes data for a scene from a different baking set from other scenes that are being loaded. Please make sure all loaded scenes are in the same baking set.");
							return;
						}
					}
				}
			}
			this.m_PendingScenesToBeLoaded.Add(sceneGUID, new ValueTuple<ProbeVolumeBakingSet, List<int>>(bakingSet, this.m_CurrentBakingSet.GetSceneCellIndexList(sceneGUID)));
			this.m_NeedLoadAsset = true;
		}

		internal void AddPendingSceneRemoval(string sceneGUID)
		{
			if (this.m_PendingScenesToBeLoaded.ContainsKey(sceneGUID))
			{
				this.m_PendingScenesToBeLoaded.Remove(sceneGUID);
			}
			if (this.m_ActiveScenes.Contains(sceneGUID) && this.m_CurrentBakingSet != null)
			{
				this.m_PendingScenesToBeUnloaded.TryAdd(sceneGUID, this.m_CurrentBakingSet.GetSceneCellIndexList(sceneGUID));
			}
		}

		internal void RemovePendingScene(string sceneGUID, List<int> cellList)
		{
			if (this.m_ActiveScenes.Contains(sceneGUID))
			{
				this.m_ActiveScenes.Remove(sceneGUID);
			}
			foreach (int cellIndex in cellList)
			{
				this.RemoveCell(cellIndex);
			}
			this.ClearDebugData();
			this.ComputeCellGlobalInfo();
		}

		private void PerformPendingIndexChangeAndInit()
		{
			if (this.m_NeedsIndexRebuild)
			{
				this.CleanupLoadedData();
				this.InitializeGlobalIndirection();
				this.m_HasChangedIndex = true;
				this.m_NeedsIndexRebuild = false;
				return;
			}
			this.m_HasChangedIndex = false;
		}

		internal void SetSubdivisionDimensions(float minBrickSize, int maxSubdiv, Vector3 offset)
		{
			this.m_MinBrickSize = minBrickSize;
			this.SetMaxSubdivision(maxSubdiv);
			this.m_ProbeOffset = offset;
		}

		private bool LoadCells(List<int> cellIndices)
		{
			if (this.m_CurrentBakingSet.ResolveCellData(cellIndices))
			{
				this.ClearDebugData();
				for (int i = 0; i < cellIndices.Count; i++)
				{
					this.AddCell(cellIndices[i]);
				}
				return true;
			}
			return false;
		}

		private void PerformPendingLoading()
		{
			if ((this.m_PendingScenesToBeLoaded.Count == 0 && this.m_ActiveScenes.Count == 0) || !this.m_NeedLoadAsset || !this.m_ProbeReferenceVolumeInit)
			{
				return;
			}
			this.m_Pool.EnsureTextureValidity();
			this.m_BlendingPool.EnsureTextureValidity();
			if (this.m_HasChangedIndex)
			{
				foreach (string sceneGUID in this.m_ActiveScenes)
				{
					this.LoadCells(this.m_CurrentBakingSet.GetSceneCellIndexList(sceneGUID));
				}
			}
			foreach (KeyValuePair<string, ValueTuple<ProbeVolumeBakingSet, List<int>>> keyValuePair in this.m_PendingScenesToBeLoaded)
			{
				string key = keyValuePair.Key;
				if (this.LoadCells(keyValuePair.Value.Item2) && !this.m_ActiveScenes.Contains(key))
				{
					this.m_ActiveScenes.Add(key);
				}
			}
			this.m_PendingScenesToBeLoaded.Clear();
			this.m_NeedLoadAsset = false;
		}

		private void PerformPendingDeletion()
		{
			foreach (KeyValuePair<string, List<int>> keyValuePair in this.m_PendingScenesToBeUnloaded)
			{
				this.RemovePendingScene(keyValuePair.Key, keyValuePair.Value);
			}
			this.m_PendingScenesToBeUnloaded.Clear();
		}

		internal void ComputeEntryMinMax(ref ProbeReferenceVolume.IndirectionEntryInfo entryInfo, ReadOnlySpan<ProbeBrickIndex.Brick> bricks)
		{
			int num = ProbeReferenceVolume.CellSize(this.GetEntrySubdivLevel());
			Vector3Int positionInBricks = entryInfo.positionInBricks;
			Vector3Int vector3Int = entryInfo.positionInBricks + new Vector3Int(num, num, num);
			if (entryInfo.hasOnlyBiggerBricks)
			{
				entryInfo.minBrickPos = positionInBricks;
				entryInfo.maxBrickPosPlusOne = vector3Int;
			}
			else
			{
				entryInfo.minBrickPos = (entryInfo.maxBrickPosPlusOne = Vector3Int.zero);
				bool flag = false;
				for (int i = 0; i < bricks.Length; i++)
				{
					int num2 = ProbeReferenceVolume.CellSize(bricks[i].subdivisionLevel);
					Vector3Int vector3Int2 = bricks[i].position;
					Vector3Int vector3Int3 = bricks[i].position + new Vector3Int(num2, num2, num2);
					if (ProbeBrickIndex.BrickOverlapEntry(vector3Int2, vector3Int3, positionInBricks, vector3Int))
					{
						vector3Int2 = Vector3Int.Max(vector3Int2, positionInBricks);
						vector3Int3 = Vector3Int.Min(vector3Int3, vector3Int);
						if (flag)
						{
							entryInfo.minBrickPos = Vector3Int.Min(vector3Int2, entryInfo.minBrickPos);
							entryInfo.maxBrickPosPlusOne = Vector3Int.Max(vector3Int3, entryInfo.maxBrickPosPlusOne);
						}
						else
						{
							entryInfo.minBrickPos = vector3Int2;
							entryInfo.maxBrickPosPlusOne = vector3Int3;
							flag = true;
						}
					}
				}
			}
			entryInfo.minBrickPos -= positionInBricks;
			entryInfo.maxBrickPosPlusOne = Vector3Int.one + entryInfo.maxBrickPosPlusOne - positionInBricks;
			entryInfo.hasMinMax = true;
		}

		internal static int GetNumberOfBricksAtSubdiv(ProbeReferenceVolume.IndirectionEntryInfo entryInfo)
		{
			if (entryInfo.hasOnlyBiggerBricks)
			{
				return 1;
			}
			Vector3Int vector3Int = (entryInfo.maxBrickPosPlusOne - entryInfo.minBrickPos) / ProbeReferenceVolume.CellSize(entryInfo.minSubdiv);
			return vector3Int.x * vector3Int.y * vector3Int.z;
		}

		public void PerformPendingOperations()
		{
			this.PerformPendingDeletion();
			this.PerformPendingIndexChangeAndInit();
			this.PerformPendingLoading();
		}

		internal void InitializeGlobalIndirection()
		{
			Vector3Int cellMin = this.m_CurrentBakingSet ? this.m_CurrentBakingSet.minCellPosition : Vector3Int.zero;
			Vector3Int cellMax = this.m_CurrentBakingSet ? this.m_CurrentBakingSet.maxCellPosition : Vector3Int.zero;
			if (this.m_CellIndices != null)
			{
				this.m_CellIndices.Cleanup();
			}
			this.m_CellIndices = new ProbeGlobalIndirection(cellMin, cellMax, Mathf.Max(1, (int)Mathf.Pow(3f, (float)(this.m_MaxSubdivision - 1))));
			if (this.m_SupportGPUStreaming)
			{
				if (this.m_DefragCellIndices != null)
				{
					this.m_DefragCellIndices.Cleanup();
				}
				this.m_DefragCellIndices = new ProbeGlobalIndirection(cellMin, cellMax, Mathf.Max(1, (int)Mathf.Pow(3f, (float)(this.m_MaxSubdivision - 1))));
			}
		}

		private void InitProbeReferenceVolume()
		{
			if (this.m_ProbeReferenceVolumeInit && !this.m_Pool.EnsureTextureValidity(this.useRenderingLayers, this.skyOcclusion, this.skyOcclusionShadingDirection, this.probeOcclusion))
			{
				this.m_TemporaryDataLocation.Cleanup();
				this.m_TemporaryDataLocation = ProbeBrickPool.CreateDataLocation(ProbeBrickPool.GetChunkSizeInProbeCount(), false, this.m_SHBands, "APV_Intermediate", false, true, this.useRenderingLayers, this.skyOcclusion, this.skyOcclusionShadingDirection, this.probeOcclusion, out this.m_TemporaryDataLocationMemCost);
			}
			if (!this.m_ProbeReferenceVolumeInit)
			{
				this.m_Pool = new ProbeBrickPool(this.m_MemoryBudget, this.m_SHBands, true, this.useRenderingLayers, this.skyOcclusion, this.skyOcclusionShadingDirection, this.probeOcclusion);
				this.m_BlendingPool = new ProbeBrickBlendingPool(this.m_BlendingMemoryBudget, this.m_SHBands, this.probeOcclusion);
				this.m_Index = new ProbeBrickIndex(this.m_MemoryBudget);
				if (this.m_SupportGPUStreaming)
				{
					this.m_DefragIndex = new ProbeBrickIndex(this.m_MemoryBudget);
				}
				this.InitializeGlobalIndirection();
				this.m_TemporaryDataLocation = ProbeBrickPool.CreateDataLocation(ProbeBrickPool.GetChunkSizeInProbeCount(), false, this.m_SHBands, "APV_Intermediate", false, true, this.useRenderingLayers, this.skyOcclusion, this.skyOcclusionShadingDirection, this.probeOcclusion, out this.m_TemporaryDataLocationMemCost);
				this.m_PositionOffsets[0] = 0f;
				float num = 0.33333334f;
				for (int i = 1; i < 3; i++)
				{
					this.m_PositionOffsets[i] = (float)i * num;
				}
				this.m_PositionOffsets[this.m_PositionOffsets.Length - 1] = 1f;
				this.m_ProbeReferenceVolumeInit = true;
				this.ClearDebugData();
				this.m_NeedLoadAsset = true;
			}
			if (DebugManager.instance.GetPanel(ProbeReferenceVolume.k_DebugPanelName, false, 0, false) != null)
			{
				ProbeReferenceVolume.instance.UnregisterDebug(false);
				ProbeReferenceVolume.instance.RegisterDebug();
			}
		}

		private ProbeReferenceVolume()
		{
			this.m_MinBrickSize = 1f;
		}

		public ProbeReferenceVolume.RuntimeResources GetRuntimeResources()
		{
			if (!this.m_ProbeReferenceVolumeInit)
			{
				return default(ProbeReferenceVolume.RuntimeResources);
			}
			ProbeReferenceVolume.RuntimeResources result = default(ProbeReferenceVolume.RuntimeResources);
			this.m_Index.GetRuntimeResources(ref result);
			this.m_CellIndices.GetRuntimeResources(ref result);
			this.m_Pool.GetRuntimeResources(ref result);
			ProbeVolumeConstantRuntimeResources.GetRuntimeResources(ref result);
			return result;
		}

		internal void SetMaxSubdivision(int maxSubdivision)
		{
			if (Math.Min(maxSubdivision, 7) != this.m_MaxSubdivision)
			{
				this.m_MaxSubdivision = Math.Min(maxSubdivision, 7);
				if (this.m_CellIndices != null)
				{
					this.m_CellIndices.Cleanup();
				}
				if (this.m_SupportGPUStreaming && this.m_DefragCellIndices != null)
				{
					this.m_DefragCellIndices.Cleanup();
				}
				this.InitializeGlobalIndirection();
			}
		}

		internal static int CellSize(int subdivisionLevel)
		{
			return (int)Mathf.Pow(3f, (float)subdivisionLevel);
		}

		internal float BrickSize(int subdivisionLevel)
		{
			return this.m_MinBrickSize * (float)ProbeReferenceVolume.CellSize(subdivisionLevel);
		}

		internal float MinBrickSize()
		{
			return this.m_MinBrickSize;
		}

		internal float MaxBrickSize()
		{
			return this.BrickSize(this.m_MaxSubdivision - 1);
		}

		internal Vector3 ProbeOffset()
		{
			return this.m_ProbeOffset;
		}

		internal int GetMaxSubdivision()
		{
			return this.m_MaxSubdivision;
		}

		internal int GetMaxSubdivision(float multiplier)
		{
			return Mathf.CeilToInt((float)this.m_MaxSubdivision * multiplier);
		}

		internal float GetDistanceBetweenProbes(int subdivisionLevel)
		{
			return this.BrickSize(subdivisionLevel) / 3f;
		}

		internal float MinDistanceBetweenProbes()
		{
			return this.GetDistanceBetweenProbes(0);
		}

		internal int GetGlobalIndirectionEntryMaxSubdiv()
		{
			return 3;
		}

		internal int GetEntrySubdivLevel()
		{
			return Mathf.Min(3, this.m_MaxSubdivision - 1);
		}

		internal float GetEntrySize()
		{
			return this.BrickSize(this.GetEntrySubdivLevel());
		}

		public bool DataHasBeenLoaded()
		{
			return this.m_LoadedCells.size != 0;
		}

		internal void Clear()
		{
			if (this.m_ProbeReferenceVolumeInit)
			{
				try
				{
					this.PerformPendingOperations();
				}
				finally
				{
					this.UnloadAllCells();
					this.m_ToBeLoadedCells.Clear();
					this.m_Pool.Clear();
					this.m_BlendingPool.Clear();
					this.m_Index.Clear();
					this.cells.Clear();
				}
			}
			if (this.clearAssetsOnVolumeClear)
			{
				this.m_PendingScenesToBeLoaded.Clear();
				this.m_ActiveScenes.Clear();
			}
		}

		private List<ProbeBrickPool.BrickChunkAlloc> GetSourceLocations(int count, int chunkSize, ProbeBrickPool.DataLocation dataLoc)
		{
			ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = default(ProbeBrickPool.BrickChunkAlloc);
			this.m_TmpSrcChunks.Clear();
			this.m_TmpSrcChunks.Add(brickChunkAlloc);
			for (int i = 1; i < count; i++)
			{
				brickChunkAlloc.x += chunkSize * 4;
				if (brickChunkAlloc.x >= dataLoc.width)
				{
					brickChunkAlloc.x = 0;
					brickChunkAlloc.y += 4;
					if (brickChunkAlloc.y >= dataLoc.height)
					{
						brickChunkAlloc.y = 0;
						brickChunkAlloc.z += 4;
					}
				}
				this.m_TmpSrcChunks.Add(brickChunkAlloc);
			}
			return this.m_TmpSrcChunks;
		}

		private void UpdateDataLocationTexture<T>(Texture output, NativeArray<T> input) where T : struct
		{
			(output as Texture3D).GetPixelData<T>(0).GetSubArray(0, input.Length).CopyFrom(input);
			(output as Texture3D).Apply();
		}

		private void UpdateValidityTextureWithoutMask(Texture output, NativeArray<byte> input)
		{
			if (GraphicsFormatUtility.GetComponentCount(output.graphicsFormat) == 1U)
			{
				this.UpdateDataLocationTexture<byte>(output, input);
				return;
			}
			NativeArray<ValueTuple<byte, byte, byte, byte>> pixelData = (output as Texture3D).GetPixelData<ValueTuple<byte, byte, byte, byte>>(0);
			for (int i = 0; i < input.Length; i++)
			{
				pixelData[i] = new ValueTuple<byte, byte, byte, byte>(input[i], input[i], input[i], input[i]);
			}
			(output as Texture3D).Apply();
		}

		private void UpdatePool(List<ProbeBrickPool.BrickChunkAlloc> chunkList, ProbeReferenceVolume.CellData.PerScenarioData data, NativeArray<byte> validityNeighMaskData, NativeArray<ushort> skyOcclusionL0L1Data, NativeArray<byte> skyShadingDirectionIndices, int chunkIndex, int poolIndex)
		{
			int chunkSizeInProbeCount = ProbeBrickPool.GetChunkSizeInProbeCount();
			this.UpdateDataLocationTexture<ushort>(this.m_TemporaryDataLocation.TexL0_L1rx, data.shL0L1RxData.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
			this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL1_G_ry, data.shL1GL1RyData.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
			this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL1_B_rz, data.shL1BL1RzData.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
			if (this.m_SHBands == ProbeVolumeSHBands.SphericalHarmonicsL2 && data.shL2Data_0.Length > 0)
			{
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL2_0, data.shL2Data_0.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL2_1, data.shL2Data_1.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL2_2, data.shL2Data_2.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexL2_3, data.shL2Data_3.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
			}
			if (this.probeOcclusion && data.probeOcclusion.Length > 0)
			{
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexProbeOcclusion, data.probeOcclusion.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
			}
			if (poolIndex == -1)
			{
				if (validityNeighMaskData.Length > 0)
				{
					if (this.m_CurrentBakingSet.bakedMaskCount == 1)
					{
						this.UpdateValidityTextureWithoutMask(this.m_TemporaryDataLocation.TexValidity, validityNeighMaskData.GetSubArray(chunkIndex * chunkSizeInProbeCount, chunkSizeInProbeCount));
					}
					else
					{
						this.UpdateDataLocationTexture<uint>(this.m_TemporaryDataLocation.TexValidity, validityNeighMaskData.Reinterpret<uint>(1).GetSubArray(chunkIndex * chunkSizeInProbeCount, chunkSizeInProbeCount));
					}
				}
				if (this.skyOcclusion && skyOcclusionL0L1Data.Length > 0)
				{
					this.UpdateDataLocationTexture<ushort>(this.m_TemporaryDataLocation.TexSkyOcclusion, skyOcclusionL0L1Data.GetSubArray(chunkIndex * chunkSizeInProbeCount * 4, chunkSizeInProbeCount * 4));
				}
				if (this.skyOcclusionShadingDirection && skyShadingDirectionIndices.Length > 0)
				{
					this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexSkyShadingDirectionIndices, skyShadingDirectionIndices.GetSubArray(chunkIndex * chunkSizeInProbeCount, chunkSizeInProbeCount));
				}
			}
			List<ProbeBrickPool.BrickChunkAlloc> sourceLocations = this.GetSourceLocations(1, ProbeBrickPool.GetChunkSizeInBrickCount(), this.m_TemporaryDataLocation);
			if (poolIndex == -1)
			{
				this.m_Pool.Update(this.m_TemporaryDataLocation, sourceLocations, chunkList, chunkIndex, this.m_SHBands);
				return;
			}
			this.m_BlendingPool.Update(this.m_TemporaryDataLocation, sourceLocations, chunkList, chunkIndex, this.m_SHBands, poolIndex);
		}

		private void UpdatePool(CommandBuffer cmd, List<ProbeBrickPool.BrickChunkAlloc> chunkList, ProbeReferenceVolume.CellStreamingScratchBuffer dataBuffer, ProbeReferenceVolume.CellStreamingScratchBufferLayout layout, int poolIndex)
		{
			if (poolIndex == -1)
			{
				this.m_Pool.Update(cmd, dataBuffer, layout, chunkList, true, this.m_Pool.GetValidityTexture(), this.m_SHBands, this.skyOcclusion, this.m_Pool.GetSkyOcclusionTexture(), this.skyOcclusionShadingDirection, this.m_Pool.GetSkyShadingDirectionIndicesTexture(), this.probeOcclusion);
				return;
			}
			this.m_BlendingPool.Update(cmd, dataBuffer, layout, chunkList, this.m_SHBands, poolIndex, this.m_Pool.GetValidityTexture(), this.skyOcclusion, this.m_Pool.GetSkyOcclusionTexture(), this.skyOcclusionShadingDirection, this.m_Pool.GetSkyShadingDirectionIndicesTexture(), this.probeOcclusion);
		}

		private void UpdateSharedData(List<ProbeBrickPool.BrickChunkAlloc> chunkList, NativeArray<byte> validityNeighMaskData, NativeArray<ushort> skyOcclusionData, NativeArray<byte> skyShadingDirectionIndices, int chunkIndex)
		{
			int num = ProbeBrickPool.GetChunkSizeInBrickCount() * 64;
			if (this.m_CurrentBakingSet.bakedMaskCount == 1)
			{
				this.UpdateValidityTextureWithoutMask(this.m_TemporaryDataLocation.TexValidity, validityNeighMaskData.GetSubArray(chunkIndex * num, num));
			}
			else
			{
				this.UpdateDataLocationTexture<uint>(this.m_TemporaryDataLocation.TexValidity, validityNeighMaskData.Reinterpret<uint>(1).GetSubArray(chunkIndex * num, num));
			}
			if (this.skyOcclusion && skyOcclusionData.Length > 0)
			{
				this.UpdateDataLocationTexture<ushort>(this.m_TemporaryDataLocation.TexSkyOcclusion, skyOcclusionData.GetSubArray(chunkIndex * num * 4, num * 4));
			}
			if (this.skyOcclusion && this.skyOcclusionShadingDirection && skyShadingDirectionIndices.Length > 0)
			{
				this.UpdateDataLocationTexture<byte>(this.m_TemporaryDataLocation.TexSkyShadingDirectionIndices, skyShadingDirectionIndices.GetSubArray(chunkIndex * num, num));
			}
			List<ProbeBrickPool.BrickChunkAlloc> sourceLocations = this.GetSourceLocations(1, ProbeBrickPool.GetChunkSizeInBrickCount(), this.m_TemporaryDataLocation);
			this.m_Pool.UpdateValidity(this.m_TemporaryDataLocation, sourceLocations, chunkList, chunkIndex);
		}

		private bool AddBlendingBricks(ProbeReferenceVolume.Cell cell)
		{
			bool result;
			using (new ProfilerMarker("AddBlendingBricks").Auto())
			{
				bool flag = this.m_CurrentBakingSet.otherScenario == null || !cell.hasTwoScenarios;
				if (!flag && !this.m_BlendingPool.Allocate(cell.poolInfo.shChunkCount, cell.blendingInfo.chunkList))
				{
					result = false;
				}
				else
				{
					if (this.diskStreamingEnabled)
					{
						if (flag)
						{
							if (cell.blendingInfo.blendingFactor != this.scenarioBlendingFactor)
							{
								this.PushDiskStreamingRequest(cell, this.lightingScenario, -1, this.m_OnStreamingComplete);
							}
							cell.blendingInfo.MarkUpToDate();
						}
						else
						{
							this.PushDiskStreamingRequest(cell, this.lightingScenario, 0, this.m_OnBlendingStreamingComplete);
							this.PushDiskStreamingRequest(cell, this.otherScenario, 1, this.m_OnBlendingStreamingComplete);
						}
					}
					else
					{
						if (!cell.indexInfo.indexUpdated)
						{
							this.UpdateCellIndex(cell);
							List<ProbeBrickPool.BrickChunkAlloc> chunkList = cell.poolInfo.chunkList;
							for (int i = 0; i < chunkList.Count; i++)
							{
								this.UpdateSharedData(chunkList, cell.data.validityNeighMaskData, cell.data.skyOcclusionDataL0L1, cell.data.skyShadingDirectionIndices, i);
							}
						}
						if (flag)
						{
							if (cell.blendingInfo.blendingFactor != this.scenarioBlendingFactor)
							{
								List<ProbeBrickPool.BrickChunkAlloc> chunkList2 = cell.poolInfo.chunkList;
								for (int j = 0; j < chunkList2.Count; j++)
								{
									this.UpdatePool(chunkList2, cell.scenario0, cell.data.validityNeighMaskData, cell.data.skyOcclusionDataL0L1, cell.data.skyShadingDirectionIndices, j, -1);
								}
							}
							cell.blendingInfo.MarkUpToDate();
						}
						else
						{
							List<ProbeBrickPool.BrickChunkAlloc> chunkList3 = cell.blendingInfo.chunkList;
							for (int k = 0; k < chunkList3.Count; k++)
							{
								this.UpdatePool(chunkList3, cell.scenario0, cell.data.validityNeighMaskData, cell.data.skyOcclusionDataL0L1, cell.data.skyShadingDirectionIndices, k, 0);
								this.UpdatePool(chunkList3, cell.scenario1, cell.data.validityNeighMaskData, cell.data.skyOcclusionDataL0L1, cell.data.skyShadingDirectionIndices, k, 1);
							}
						}
					}
					cell.blendingInfo.blending = true;
					result = true;
				}
			}
			return result;
		}

		private bool ReservePoolChunks(int brickCount, List<ProbeBrickPool.BrickChunkAlloc> chunkList, bool ignoreErrorLog)
		{
			int chunkCount = ProbeBrickPool.GetChunkCount(brickCount);
			chunkList.Clear();
			return this.m_Pool.Allocate(chunkCount, chunkList, ignoreErrorLog);
		}

		private void ReleasePoolChunks(List<ProbeBrickPool.BrickChunkAlloc> chunkList)
		{
			this.m_Pool.Deallocate(chunkList);
			chunkList.Clear();
		}

		private void UpdatePoolAndIndex(ProbeReferenceVolume.Cell cell, ProbeReferenceVolume.CellStreamingScratchBuffer dataBuffer, ProbeReferenceVolume.CellStreamingScratchBufferLayout layout, int poolIndex, CommandBuffer cmd)
		{
			if (this.diskStreamingEnabled)
			{
				if (this.m_DiskStreamingUseCompute)
				{
					this.UpdatePool(cmd, cell.poolInfo.chunkList, dataBuffer, layout, poolIndex);
				}
				else
				{
					int count = cell.poolInfo.chunkList.Count;
					int num = -2 * (count * 4 * 4);
					ProbeReferenceVolume.CellData.PerScenarioData data = default(ProbeReferenceVolume.CellData.PerScenarioData);
					data.shL0L1RxData = dataBuffer.stagingBuffer.GetSubArray(layout._L0L1rxOffset + num, count * layout._L0Size).Reinterpret<ushort>(1);
					data.shL1GL1RyData = dataBuffer.stagingBuffer.GetSubArray(layout._L1GryOffset + num, count * layout._L1Size);
					data.shL1BL1RzData = dataBuffer.stagingBuffer.GetSubArray(layout._L1BrzOffset + num, count * layout._L1Size);
					NativeArray<byte> subArray = dataBuffer.stagingBuffer.GetSubArray(layout._ValidityOffset + num, count * layout._ValiditySize);
					if (this.m_SHBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
					{
						data.shL2Data_0 = dataBuffer.stagingBuffer.GetSubArray(layout._L2_0Offset + num, count * layout._L2Size);
						data.shL2Data_1 = dataBuffer.stagingBuffer.GetSubArray(layout._L2_1Offset + num, count * layout._L2Size);
						data.shL2Data_2 = dataBuffer.stagingBuffer.GetSubArray(layout._L2_2Offset + num, count * layout._L2Size);
						data.shL2Data_3 = dataBuffer.stagingBuffer.GetSubArray(layout._L2_3Offset + num, count * layout._L2Size);
					}
					if (this.probeOcclusion && layout._ProbeOcclusionSize > 0)
					{
						data.probeOcclusion = dataBuffer.stagingBuffer.GetSubArray(layout._ProbeOcclusionOffset + num, count * layout._ProbeOcclusionSize);
					}
					NativeArray<ushort> skyOcclusionL0L1Data = default(NativeArray<ushort>);
					if (this.skyOcclusion && layout._SkyOcclusionSize > 0)
					{
						skyOcclusionL0L1Data = dataBuffer.stagingBuffer.GetSubArray(layout._SkyOcclusionOffset + num, count * layout._SkyOcclusionSize).Reinterpret<ushort>(1);
					}
					NativeArray<byte> skyShadingDirectionIndices = default(NativeArray<byte>);
					if (this.skyOcclusion && this.skyOcclusionShadingDirection && layout._SkyShadingDirectionSize > 0)
					{
						skyShadingDirectionIndices = dataBuffer.stagingBuffer.GetSubArray(layout._SkyShadingDirectionOffset + num, count * layout._SkyShadingDirectionSize);
					}
					for (int i = 0; i < count; i++)
					{
						this.UpdatePool(cell.poolInfo.chunkList, data, subArray, skyOcclusionL0L1Data, skyShadingDirectionIndices, i, poolIndex);
					}
				}
			}
			else
			{
				for (int j = 0; j < cell.poolInfo.chunkList.Count; j++)
				{
					this.UpdatePool(cell.poolInfo.chunkList, cell.scenario0, cell.data.validityNeighMaskData, cell.data.skyOcclusionDataL0L1, cell.data.skyShadingDirectionIndices, j, poolIndex);
				}
			}
			if (!cell.indexInfo.indexUpdated)
			{
				this.UpdateCellIndex(cell);
			}
		}

		private bool AddBricks(ProbeReferenceVolume.Cell cell)
		{
			bool result;
			using (new ProfilerMarker("AddBricks").Auto())
			{
				if (this.supportScenarioBlending)
				{
					this.m_ToBeLoadedBlendingCells.Add(cell);
				}
				if (!this.supportScenarioBlending || this.scenarioBlendingFactor == 0f || !cell.hasTwoScenarios)
				{
					if (this.diskStreamingEnabled)
					{
						this.PushDiskStreamingRequest(cell, this.m_CurrentBakingSet.lightingScenario, -1, this.m_OnStreamingComplete);
					}
					else
					{
						this.UpdatePoolAndIndex(cell, null, default(ProbeReferenceVolume.CellStreamingScratchBufferLayout), -1, null);
					}
					cell.blendingInfo.blendingFactor = 0f;
				}
				else if (this.supportScenarioBlending)
				{
					cell.blendingInfo.Prioritize();
					cell.indexInfo.indexUpdated = false;
				}
				cell.loaded = true;
				this.ClearDebugData();
				result = true;
			}
			return result;
		}

		private void UpdateCellIndex(ProbeReferenceVolume.Cell cell)
		{
			cell.indexInfo.indexUpdated = true;
			NativeArray<ProbeBrickIndex.Brick> bricks = cell.data.bricks;
			this.m_Index.AddBricks(cell.indexInfo, bricks, cell.poolInfo.chunkList, ProbeBrickPool.GetChunkSizeInBrickCount(), this.m_Pool.GetPoolWidth(), this.m_Pool.GetPoolHeight());
			this.m_CellIndices.UpdateCell(cell.indexInfo);
		}

		private void ReleaseBricks(ProbeReferenceVolume.Cell cell)
		{
			if (cell.poolInfo.chunkList.Count == 0)
			{
				Debug.Log("Tried to release bricks from an empty Cell.");
				return;
			}
			this.m_Index.RemoveBricks(cell.indexInfo);
			cell.indexInfo.indexUpdated = false;
			this.m_Pool.Deallocate(cell.poolInfo.chunkList);
			cell.poolInfo.chunkList.Clear();
		}

		internal void UpdateConstantBuffer(CommandBuffer cmd, ProbeVolumeShadingParameters parameters)
		{
			float num = parameters.normalBias;
			float num2 = parameters.viewBias;
			APVLeakReductionMode leakReductionMode = parameters.leakReductionMode;
			if (parameters.scaleBiasByMinDistanceBetweenProbes)
			{
				num *= this.MinDistanceBetweenProbes();
				num2 *= this.MinDistanceBetweenProbes();
			}
			Vector3Int globalIndirectionDimension = this.m_CellIndices.GetGlobalIndirectionDimension();
			Vector3Int poolDimensions = this.m_Pool.GetPoolDimensions();
			Vector3Int vector3Int;
			Vector3Int vector3Int2;
			this.m_CellIndices.GetMinMaxEntry(out vector3Int, out vector3Int2);
			int entriesPerCellDimension = this.m_CellIndices.entriesPerCellDimension;
			float w = parameters.skyOcclusionShadingDirection ? 1f : 0f;
			Vector3 vector = this.ProbeOffset() + parameters.worldOffset;
			ShaderVariablesProbeVolumes shaderVariablesProbeVolumes;
			shaderVariablesProbeVolumes._Offset_LayerCount = new Vector4(vector.x, vector.y, vector.z, (float)parameters.regionCount);
			shaderVariablesProbeVolumes._MinLoadedCellInEntries_IndirectionEntryDim = new Vector4((float)(this.minLoadedCellPos.x * entriesPerCellDimension), (float)(this.minLoadedCellPos.y * entriesPerCellDimension), (float)(this.minLoadedCellPos.z * entriesPerCellDimension), this.GetEntrySize());
			shaderVariablesProbeVolumes._MaxLoadedCellInEntries_RcpIndirectionEntryDim = new Vector4((float)((this.maxLoadedCellPos.x + 1) * entriesPerCellDimension - 1), (float)((this.maxLoadedCellPos.y + 1) * entriesPerCellDimension - 1), (float)((this.maxLoadedCellPos.z + 1) * entriesPerCellDimension - 1), 1f / this.GetEntrySize());
			shaderVariablesProbeVolumes._PoolDim_MinBrickSize = new Vector4((float)poolDimensions.x, (float)poolDimensions.y, (float)poolDimensions.z, this.MinBrickSize());
			shaderVariablesProbeVolumes._RcpPoolDim_XY = new Vector4(1f / (float)poolDimensions.x, 1f / (float)poolDimensions.y, 1f / (float)poolDimensions.z, 1f / (float)(poolDimensions.x * poolDimensions.y));
			shaderVariablesProbeVolumes._MinEntryPos_Noise = new Vector4((float)vector3Int.x, (float)vector3Int.y, (float)vector3Int.z, parameters.samplingNoise);
			shaderVariablesProbeVolumes._EntryCount_X_XY_LeakReduction = new uint4((uint)globalIndirectionDimension.x, (uint)(globalIndirectionDimension.x * globalIndirectionDimension.y), (uint)leakReductionMode, 0U);
			shaderVariablesProbeVolumes._Biases_NormalizationClamp = new Vector4(num, num2, parameters.reflNormalizationLowerClamp, parameters.reflNormalizationUpperClamp);
			shaderVariablesProbeVolumes._FrameIndex_Weights = new Vector4((float)parameters.frameIndexForNoise, parameters.weight, parameters.skyOcclusionIntensity, w);
			shaderVariablesProbeVolumes._ProbeVolumeLayerMask = parameters.regionLayerMasks;
			ConstantBuffer.PushGlobal<ShaderVariablesProbeVolumes>(cmd, shaderVariablesProbeVolumes, this.m_CBShaderID);
		}

		private void DeinitProbeReferenceVolume()
		{
			if (this.m_ProbeReferenceVolumeInit)
			{
				foreach (ProbeVolumePerSceneData probeVolumePerSceneData in this.perSceneDataList)
				{
					this.AddPendingSceneRemoval(probeVolumePerSceneData.sceneGUID);
				}
				this.PerformPendingDeletion();
				this.m_Index.Cleanup();
				this.m_CellIndices.Cleanup();
				if (this.m_SupportGPUStreaming)
				{
					this.m_DefragIndex.Cleanup();
					this.m_DefragCellIndices.Cleanup();
				}
				if (this.m_Pool != null)
				{
					this.m_Pool.Cleanup();
					this.m_BlendingPool.Cleanup();
				}
				this.m_TemporaryDataLocation.Cleanup();
				this.m_ProbeReferenceVolumeInit = false;
				if (this.m_CurrentBakingSet != null)
				{
					this.m_CurrentBakingSet.Cleanup();
				}
				this.m_CurrentBakingSet = null;
			}
			else
			{
				ProbeGlobalIndirection cellIndices = this.m_CellIndices;
				if (cellIndices != null)
				{
					cellIndices.Cleanup();
				}
				ProbeGlobalIndirection defragCellIndices = this.m_DefragCellIndices;
				if (defragCellIndices != null)
				{
					defragCellIndices.Cleanup();
				}
			}
			this.ClearDebugData();
		}

		private void CleanupLoadedData()
		{
			this.UnloadAllCells();
		}

		internal ProbeVolumeDebug probeVolumeDebug { get; } = new ProbeVolumeDebug();

		public Color[] subdivisionDebugColors { get; } = new Color[7];

		private Mesh debugMesh
		{
			get
			{
				if (this.m_DebugMesh == null)
				{
					this.m_DebugMesh = DebugShapes.instance.BuildCustomSphereMesh(0.5f, 9U, 8U);
					this.m_DebugMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000000f);
				}
				return this.m_DebugMesh;
			}
		}

		[Obsolete("Use the other override to support sampling offset in debug modes.")]
		public void RenderDebug(Camera camera, Texture exposureTexture)
		{
			this.RenderDebug(camera, null, exposureTexture);
		}

		public void RenderDebug(Camera camera, ProbeVolumesOptions options, Texture exposureTexture)
		{
			if (camera.cameraType != CameraType.Reflection && camera.cameraType != CameraType.Preview)
			{
				if (options != null)
				{
					ProbeVolumeDebug.currentOffset = options.worldOffset.value;
				}
				this.DrawProbeDebug(camera, exposureTexture);
			}
		}

		public bool IsProbeSamplingDebugEnabled()
		{
			return ProbeReferenceVolume.probeSamplingDebugData.update > ProbeSamplingDebugUpdate.Never;
		}

		public bool GetProbeSamplingDebugResources(Camera camera, out GraphicsBuffer resultBuffer, out Vector2 coords)
		{
			resultBuffer = ProbeReferenceVolume.probeSamplingDebugData.positionNormalBuffer;
			coords = ProbeReferenceVolume.probeSamplingDebugData.coordinates;
			if (!this.probeVolumeDebug.drawProbeSamplingDebug)
			{
				return false;
			}
			if (ProbeReferenceVolume.probeSamplingDebugData.update == ProbeSamplingDebugUpdate.Never)
			{
				return false;
			}
			if (ProbeReferenceVolume.probeSamplingDebugData.update == ProbeSamplingDebugUpdate.Once)
			{
				ProbeReferenceVolume.probeSamplingDebugData.update = ProbeSamplingDebugUpdate.Never;
				ProbeReferenceVolume.probeSamplingDebugData.forceScreenCenterCoordinates = false;
			}
			return true;
		}

		private bool TryCreateDebugRenderData()
		{
			ProbeVolumeDebugResources probeVolumeDebugResources;
			if (!GraphicsSettings.TryGetRenderPipelineSettings<ProbeVolumeDebugResources>(out probeVolumeDebugResources))
			{
				return false;
			}
			ShaderStrippingSetting shaderStrippingSetting;
			if (GraphicsSettings.TryGetRenderPipelineSettings<ShaderStrippingSetting>(out shaderStrippingSetting) && shaderStrippingSetting.stripRuntimeDebugShaders)
			{
				return false;
			}
			this.m_DebugMaterial = CoreUtils.CreateEngineMaterial(probeVolumeDebugResources.probeVolumeDebugShader);
			this.m_DebugMaterial.enableInstancing = true;
			this.m_DebugProbeSamplingMesh = probeVolumeDebugResources.probeSamplingDebugMesh;
			this.m_DebugProbeSamplingMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000000f);
			this.m_ProbeSamplingDebugMaterial = CoreUtils.CreateEngineMaterial(probeVolumeDebugResources.probeVolumeSamplingDebugShader);
			this.m_ProbeSamplingDebugMaterial02 = CoreUtils.CreateEngineMaterial(probeVolumeDebugResources.probeVolumeDebugShader);
			this.m_ProbeSamplingDebugMaterial02.enableInstancing = true;
			ProbeReferenceVolume.probeSamplingDebugData.positionNormalBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 2, Marshal.SizeOf(typeof(Vector4)));
			this.m_DisplayNumbersTexture = probeVolumeDebugResources.numbersDisplayTex;
			this.m_DebugOffsetMesh = Resources.GetBuiltinResource<Mesh>("pyramid.fbx");
			this.m_DebugOffsetMesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000000f);
			this.m_DebugOffsetMaterial = CoreUtils.CreateEngineMaterial(probeVolumeDebugResources.probeVolumeOffsetDebugShader);
			this.m_DebugOffsetMaterial.enableInstancing = true;
			this.m_DebugFragmentationMaterial = CoreUtils.CreateEngineMaterial(probeVolumeDebugResources.probeVolumeFragmentationDebugShader);
			this.subdivisionDebugColors[0] = ProbeVolumeDebugColorPreferences.s_DetailSubdivision;
			this.subdivisionDebugColors[1] = ProbeVolumeDebugColorPreferences.s_MediumSubdivision;
			this.subdivisionDebugColors[2] = ProbeVolumeDebugColorPreferences.s_LowSubdivision;
			this.subdivisionDebugColors[3] = ProbeVolumeDebugColorPreferences.s_VeryLowSubdivision;
			this.subdivisionDebugColors[4] = ProbeVolumeDebugColorPreferences.s_SparseSubdivision;
			this.subdivisionDebugColors[5] = ProbeVolumeDebugColorPreferences.s_SparsestSubdivision;
			this.subdivisionDebugColors[6] = ProbeVolumeDebugColorPreferences.s_DetailSubdivision;
			return true;
		}

		private void InitializeDebug()
		{
			if (this.TryCreateDebugRenderData())
			{
				this.RegisterDebug();
			}
		}

		private void CleanupDebug()
		{
			this.UnregisterDebug(true);
			CoreUtils.Destroy(this.m_DebugMaterial);
			CoreUtils.Destroy(this.m_ProbeSamplingDebugMaterial);
			CoreUtils.Destroy(this.m_ProbeSamplingDebugMaterial02);
			CoreUtils.Destroy(this.m_DebugOffsetMaterial);
			CoreUtils.Destroy(this.m_DebugFragmentationMaterial);
			ProbeSamplingDebugData probeSamplingDebugData = ProbeReferenceVolume.probeSamplingDebugData;
			CoreUtils.SafeRelease((probeSamplingDebugData != null) ? probeSamplingDebugData.positionNormalBuffer : null);
		}

		private void DebugCellIndexChanged<T>(DebugUI.Field<T> field, T value)
		{
			this.ClearDebugData();
		}

		private void RegisterDebug()
		{
			List<DebugUI.Widget> list = new List<DebugUI.Widget>();
			list.Add(new DebugUI.RuntimeDebugShadersMessageBox());
			DebugUI.Container container = new DebugUI.Container();
			container.displayName = "Subdivision Visualization";
			container.isHiddenCallback = (() => false);
			DebugUI.Container container2 = container;
			container2.children.Add(new DebugUI.BoolField
			{
				displayName = "Display Cells",
				tooltip = "Draw Cells used for loading and streaming.",
				getter = (() => this.probeVolumeDebug.drawCells),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.drawCells = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			container2.children.Add(new DebugUI.BoolField
			{
				displayName = "Display Bricks",
				tooltip = "Display Subdivision bricks.",
				getter = (() => this.probeVolumeDebug.drawBricks),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.drawBricks = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			ObservableList<DebugUI.Widget> children = container2.children;
			DebugUI.FloatField floatField = new DebugUI.FloatField();
			floatField.displayName = "Debug Draw Distance";
			floatField.tooltip = "How far from the Scene Camera to draw debug visualization for Cells and Bricks. Large distances can impact Editor performance.";
			floatField.getter = (() => this.probeVolumeDebug.subdivisionViewCullingDistance);
			floatField.setter = delegate(float value)
			{
				this.probeVolumeDebug.subdivisionViewCullingDistance = value;
			};
			floatField.min = (() => 0f);
			children.Add(floatField);
			list.Add(container2);
			list.Add(new DebugUI.RuntimeDebugShadersMessageBox());
			DebugUI.Container container3 = new DebugUI.Container
			{
				displayName = "Probe Visualization"
			};
			container3.children.Add(new DebugUI.BoolField
			{
				displayName = "Display Probes",
				tooltip = "Render the debug view showing probe positions. Use the shading mode to determine which type of lighting data to visualize.",
				getter = (() => this.probeVolumeDebug.drawProbes),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.drawProbes = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			DebugUI.Container container4 = new DebugUI.Container
			{
				isHiddenCallback = (() => !this.probeVolumeDebug.drawProbes)
			};
			container4.children.Add(new DebugUI.EnumField
			{
				displayName = "Probe Shading Mode",
				tooltip = "Choose which lighting data to show in the probe debug visualization.",
				getter = (() => (int)this.probeVolumeDebug.probeShading),
				setter = delegate(int value)
				{
					this.probeVolumeDebug.probeShading = (DebugProbeShadingMode)value;
				},
				autoEnum = typeof(DebugProbeShadingMode),
				getIndex = (() => (int)this.probeVolumeDebug.probeShading),
				setIndex = delegate(int value)
				{
					this.probeVolumeDebug.probeShading = (DebugProbeShadingMode)value;
				}
			});
			ObservableList<DebugUI.Widget> children2 = container4.children;
			DebugUI.FloatField floatField2 = new DebugUI.FloatField();
			floatField2.displayName = "Debug Size";
			floatField2.tooltip = "The size of probes shown in the debug view.";
			floatField2.getter = (() => this.probeVolumeDebug.probeSize);
			floatField2.setter = delegate(float value)
			{
				this.probeVolumeDebug.probeSize = value;
			};
			floatField2.min = (() => 0.05f);
			floatField2.max = (() => 10f);
			children2.Add(floatField2);
			DebugUI.FloatField item = new DebugUI.FloatField
			{
				displayName = "Exposure Compensation",
				tooltip = "Modify the brightness of probe visualizations. Decrease this number to make very bright probes more visible.",
				getter = (() => this.probeVolumeDebug.exposureCompensation),
				setter = delegate(float value)
				{
					this.probeVolumeDebug.exposureCompensation = value;
				},
				isHiddenCallback = delegate()
				{
					switch (this.probeVolumeDebug.probeShading)
					{
					case DebugProbeShadingMode.SH:
						return false;
					case DebugProbeShadingMode.SHL0:
						return false;
					case DebugProbeShadingMode.SHL0L1:
						return false;
					case DebugProbeShadingMode.SkyOcclusionSH:
						return false;
					case DebugProbeShadingMode.SkyDirection:
						return false;
					case DebugProbeShadingMode.ProbeOcclusion:
						return false;
					}
					return true;
				}
			};
			container4.children.Add(item);
			ObservableList<DebugUI.Widget> children3 = container4.children;
			DebugUI.IntField intField = new DebugUI.IntField();
			intField.displayName = "Max Subdivisions Displayed";
			intField.tooltip = "The highest (most dense) probe subdivision level displayed in the debug view.";
			intField.getter = (() => this.probeVolumeDebug.maxSubdivToVisualize);
			intField.setter = delegate(int v)
			{
				this.probeVolumeDebug.maxSubdivToVisualize = ((this.GetMaxSubdivision() == 0) ? 7 : Mathf.Max(0, Mathf.Min(v, this.GetMaxSubdivision() - 1)));
			};
			intField.min = (() => 0);
			intField.max = (() => Mathf.Max(0, this.GetMaxSubdivision() - 1));
			children3.Add(intField);
			ObservableList<DebugUI.Widget> children4 = container4.children;
			DebugUI.IntField intField2 = new DebugUI.IntField();
			intField2.displayName = "Min Subdivisions Displayed";
			intField2.tooltip = "The lowest (least dense) probe subdivision level displayed in the debug view.";
			intField2.getter = (() => this.probeVolumeDebug.minSubdivToVisualize);
			intField2.setter = delegate(int v)
			{
				this.probeVolumeDebug.minSubdivToVisualize = Mathf.Max(v, 0);
			};
			intField2.min = (() => 0);
			intField2.max = (() => Mathf.Max(0, this.GetMaxSubdivision() - 1));
			children4.Add(intField2);
			container3.children.Add(container4);
			container3.children.Add(new DebugUI.BoolField
			{
				displayName = "Debug Probe Sampling",
				tooltip = "Render the debug view displaying how probes are sampled for a selected pixel. Use the viewport overlay 'SelectPixel' button or Ctrl+Click on the viewport to select the debugged pixel",
				getter = (() => this.probeVolumeDebug.drawProbeSamplingDebug),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.drawProbeSamplingDebug = value;
					ProbeReferenceVolume.probeSamplingDebugData.update = ProbeSamplingDebugUpdate.Once;
					ProbeReferenceVolume.probeSamplingDebugData.forceScreenCenterCoordinates = true;
				}
			});
			DebugUI.Container container5 = new DebugUI.Container
			{
				isHiddenCallback = (() => !this.probeVolumeDebug.drawProbeSamplingDebug)
			};
			ObservableList<DebugUI.Widget> children5 = container5.children;
			DebugUI.FloatField floatField3 = new DebugUI.FloatField();
			floatField3.displayName = "Debug Size";
			floatField3.tooltip = "The size of gizmos shown in the debug view.";
			floatField3.getter = (() => this.probeVolumeDebug.probeSamplingDebugSize);
			floatField3.setter = delegate(float value)
			{
				this.probeVolumeDebug.probeSamplingDebugSize = value;
			};
			floatField3.min = (() => 0.05f);
			floatField3.max = (() => 10f);
			children5.Add(floatField3);
			container5.children.Add(new DebugUI.BoolField
			{
				displayName = "Debug With Sampling Noise",
				tooltip = "Enable Sampling Noise for this debug view. It should be enabled for accuracy but it can make results more difficult to read",
				getter = (() => this.probeVolumeDebug.debugWithSamplingNoise),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.debugWithSamplingNoise = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			container3.children.Add(container5);
			container3.children.Add(new DebugUI.BoolField
			{
				displayName = "Virtual Offset Debug",
				tooltip = "Enable Virtual Offset debug visualization. Indicates the offsets applied to probe positions. These are used to capture lighting when probes are considered invalid.",
				getter = (() => this.probeVolumeDebug.drawVirtualOffsetPush),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.drawVirtualOffsetPush = value;
					if (this.probeVolumeDebug.drawVirtualOffsetPush && this.probeVolumeDebug.drawProbes && this.m_CurrentBakingSet != null)
					{
						float value3 = (float)ProbeReferenceVolume.CellSize(0) * this.MinBrickSize() / 3f * this.m_CurrentBakingSet.settings.virtualOffsetSettings.searchMultiplier + this.m_CurrentBakingSet.settings.virtualOffsetSettings.outOfGeoOffset;
						this.probeVolumeDebug.probeSize = Mathf.Min(this.probeVolumeDebug.probeSize, Mathf.Clamp(value3, 0.05f, 10f));
					}
				}
			});
			DebugUI.Container container6 = new DebugUI.Container
			{
				isHiddenCallback = (() => !this.probeVolumeDebug.drawVirtualOffsetPush)
			};
			DebugUI.FloatField floatField4 = new DebugUI.FloatField();
			floatField4.displayName = "Debug Size";
			floatField4.tooltip = "Modify the size of the arrows used in the virtual offset debug visualization.";
			floatField4.getter = (() => this.probeVolumeDebug.offsetSize);
			floatField4.setter = delegate(float value)
			{
				this.probeVolumeDebug.offsetSize = value;
			};
			floatField4.min = (() => 0.001f);
			floatField4.max = (() => 0.1f);
			floatField4.isHiddenCallback = (() => !this.probeVolumeDebug.drawVirtualOffsetPush);
			DebugUI.FloatField item2 = floatField4;
			container6.children.Add(item2);
			container3.children.Add(container6);
			ObservableList<DebugUI.Widget> children6 = container3.children;
			DebugUI.FloatField floatField5 = new DebugUI.FloatField();
			floatField5.displayName = "Debug Draw Distance";
			floatField5.tooltip = "How far from the Scene Camera to draw probe debug visualizations. Large distances can impact Editor performance.";
			floatField5.getter = (() => this.probeVolumeDebug.probeCullingDistance);
			floatField5.setter = delegate(float value)
			{
				this.probeVolumeDebug.probeCullingDistance = value;
			};
			floatField5.min = (() => 0f);
			children6.Add(floatField5);
			list.Add(container3);
			DebugUI.Container container7 = new DebugUI.Container
			{
				displayName = "Probe Adjustment Volumes"
			};
			container7.children.Add(new DebugUI.BoolField
			{
				displayName = "Auto Display Probes",
				tooltip = "When enabled and a Probe Adjustment Volumes is selected, automatically display the probes.",
				getter = (() => this.probeVolumeDebug.autoDrawProbes),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.autoDrawProbes = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			container7.children.Add(new DebugUI.BoolField
			{
				displayName = "Isolate Affected",
				tooltip = "When enabled, only displayed probes in the influence of the currently selected Probe Adjustment Volumes.",
				getter = (() => this.probeVolumeDebug.isolationProbeDebug),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.isolationProbeDebug = value;
				},
				onValueChanged = new Action<DebugUI.Field<bool>, bool>(this.<RegisterDebug>g__RefreshDebug|229_0<bool>)
			});
			list.Add(container7);
			DebugUI.Container container8 = new DebugUI.Container
			{
				displayName = "Streaming",
				isHiddenCallback = (() => !this.gpuStreamingEnabled && !this.diskStreamingEnabled)
			};
			container8.children.Add(new DebugUI.BoolField
			{
				displayName = "Freeze Streaming",
				tooltip = "Stop Unity from streaming probe data in or out of GPU memory.",
				getter = (() => this.probeVolumeDebug.freezeStreaming),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.freezeStreaming = value;
				}
			});
			container8.children.Add(new DebugUI.BoolField
			{
				displayName = "Display Streaming Score",
				getter = (() => this.probeVolumeDebug.displayCellStreamingScore),
				setter = delegate(bool value)
				{
					this.probeVolumeDebug.displayCellStreamingScore = value;
				}
			});
			ObservableList<DebugUI.Widget> children7 = container8.children;
			DebugUI.BoolField boolField = new DebugUI.BoolField();
			boolField.displayName = "Maximum cell streaming";
			boolField.tooltip = "Enable streaming as many cells as possible every frame.";
			boolField.getter = (() => ProbeReferenceVolume.instance.loadMaxCellsPerFrame);
			boolField.setter = delegate(bool value)
			{
				ProbeReferenceVolume.instance.loadMaxCellsPerFrame = value;
			};
			children7.Add(boolField);
			DebugUI.Container container9 = new DebugUI.Container();
			container9.isHiddenCallback = (() => ProbeReferenceVolume.instance.loadMaxCellsPerFrame);
			DebugUI.Container container10 = container9;
			ObservableList<DebugUI.Widget> children8 = container10.children;
			DebugUI.IntField intField3 = new DebugUI.IntField();
			intField3.displayName = "Loaded Cells Per Frame";
			intField3.tooltip = "Determines the maximum number of Cells Unity streams per frame. Loading more Cells per frame can impact performance.";
			intField3.getter = (() => ProbeReferenceVolume.instance.numberOfCellsLoadedPerFrame);
			intField3.setter = delegate(int value)
			{
				ProbeReferenceVolume.instance.SetNumberOfCellsLoadedPerFrame(value);
			};
			intField3.min = (() => 1);
			intField3.max = (() => 10);
			children8.Add(intField3);
			container8.children.Add(container10);
			if (Debug.isDebugBuild)
			{
				container8.children.Add(new DebugUI.BoolField
				{
					displayName = "Display Index Fragmentation",
					getter = (() => this.probeVolumeDebug.displayIndexFragmentation),
					setter = delegate(bool value)
					{
						this.probeVolumeDebug.displayIndexFragmentation = value;
					}
				});
				DebugUI.Container container11 = new DebugUI.Container
				{
					isHiddenCallback = (() => !this.probeVolumeDebug.displayIndexFragmentation)
				};
				ObservableList<DebugUI.Widget> children9 = container11.children;
				DebugUI.Value value2 = new DebugUI.Value();
				value2.displayName = "Index Fragmentation Rate";
				value2.getter = (() => ProbeReferenceVolume.instance.indexFragmentationRate);
				children9.Add(value2);
				container8.children.Add(container11);
				container8.children.Add(new DebugUI.BoolField
				{
					displayName = "Verbose Log",
					getter = (() => this.probeVolumeDebug.verboseStreamingLog),
					setter = delegate(bool value)
					{
						this.probeVolumeDebug.verboseStreamingLog = value;
					}
				});
				container8.children.Add(new DebugUI.BoolField
				{
					displayName = "Debug Streaming",
					getter = (() => this.probeVolumeDebug.debugStreaming),
					setter = delegate(bool value)
					{
						this.probeVolumeDebug.debugStreaming = value;
					}
				});
			}
			list.Add(container8);
			if (this.supportScenarioBlending && this.m_CurrentBakingSet != null)
			{
				DebugUI.Container container12 = new DebugUI.Container
				{
					displayName = "Scenario Blending"
				};
				ObservableList<DebugUI.Widget> children10 = container12.children;
				DebugUI.IntField intField4 = new DebugUI.IntField();
				intField4.displayName = "Number Of Cells Blended Per Frame";
				intField4.getter = (() => ProbeReferenceVolume.instance.numberOfCellsBlendedPerFrame);
				intField4.setter = delegate(int value)
				{
					ProbeReferenceVolume.instance.numberOfCellsBlendedPerFrame = value;
				};
				intField4.min = (() => 0);
				children10.Add(intField4);
				ObservableList<DebugUI.Widget> children11 = container12.children;
				DebugUI.FloatField floatField6 = new DebugUI.FloatField();
				floatField6.displayName = "Turnover Rate";
				floatField6.getter = (() => ProbeReferenceVolume.instance.turnoverRate);
				floatField6.setter = delegate(float value)
				{
					ProbeReferenceVolume.instance.turnoverRate = value;
				};
				floatField6.min = (() => 0f);
				floatField6.max = (() => 1f);
				children11.Add(floatField6);
				this.m_DebugScenarioField = new DebugUI.EnumField
				{
					displayName = "Scenario Blend Target",
					tooltip = "Select another lighting scenario to blend with the active lighting scenario.",
					enumNames = this.m_DebugScenarioNames,
					enumValues = this.m_DebugScenarioValues,
					getIndex = delegate
					{
						if (this.m_CurrentBakingSet == null)
						{
							return 0;
						}
						this.<RegisterDebug>g__RefreshScenarioNames|229_75(ProbeReferenceVolume.GetSceneGUID(SceneManager.GetActiveScene()));
						this.probeVolumeDebug.otherStateIndex = 0;
						if (!string.IsNullOrEmpty(this.m_CurrentBakingSet.otherScenario))
						{
							for (int i = 1; i < this.m_DebugScenarioNames.Length; i++)
							{
								if (this.m_DebugScenarioNames[i].text == this.m_CurrentBakingSet.otherScenario)
								{
									this.probeVolumeDebug.otherStateIndex = i;
									break;
								}
							}
						}
						return this.probeVolumeDebug.otherStateIndex;
					},
					setIndex = delegate(int value)
					{
						string otherScenario = (value == 0) ? null : this.m_DebugScenarioNames[value].text;
						this.m_CurrentBakingSet.BlendLightingScenario(otherScenario, this.m_CurrentBakingSet.scenarioBlendingFactor);
						this.probeVolumeDebug.otherStateIndex = value;
					},
					getter = (() => this.probeVolumeDebug.otherStateIndex),
					setter = delegate(int value)
					{
						this.probeVolumeDebug.otherStateIndex = value;
					}
				};
				container12.children.Add(this.m_DebugScenarioField);
				ObservableList<DebugUI.Widget> children12 = container12.children;
				DebugUI.FloatField floatField7 = new DebugUI.FloatField();
				floatField7.displayName = "Scenario Blending Factor";
				floatField7.tooltip = "Blend between lighting scenarios by adjusting this slider.";
				floatField7.getter = (() => ProbeReferenceVolume.instance.scenarioBlendingFactor);
				floatField7.setter = delegate(float value)
				{
					ProbeReferenceVolume.instance.scenarioBlendingFactor = value;
				};
				floatField7.min = (() => 0f);
				floatField7.max = (() => 1f);
				children12.Add(floatField7);
				list.Add(container12);
			}
			if (list.Count > 0)
			{
				this.m_DebugItems = list.ToArray();
				DebugManager.instance.GetPanel(ProbeReferenceVolume.k_DebugPanelName, true, 0, false).children.Add(this.m_DebugItems);
			}
			DebugManager.instance.RegisterData(this.probeVolumeDebug);
		}

		private void UnregisterDebug(bool destroyPanel)
		{
			if (destroyPanel)
			{
				DebugManager.instance.RemovePanel(ProbeReferenceVolume.k_DebugPanelName);
				return;
			}
			DebugManager.instance.GetPanel(ProbeReferenceVolume.k_DebugPanelName, false, 0, false).children.Remove(this.m_DebugItems);
		}

		public void RenderFragmentationOverlay(RenderGraph renderGraph, TextureHandle colorBuffer, TextureHandle depthBuffer, DebugOverlay debugOverlay)
		{
			if (!this.m_ProbeReferenceVolumeInit || !this.probeVolumeDebug.displayIndexFragmentation)
			{
				return;
			}
			ProbeReferenceVolume.RenderFragmentationOverlayPassData renderFragmentationOverlayPassData;
			using (RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<ProbeReferenceVolume.RenderFragmentationOverlayPassData>("APVFragmentationOverlay", out renderFragmentationOverlayPassData, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Lighting\\ProbeVolume\\ProbeReferenceVolume.Debug.cs", 830))
			{
				renderFragmentationOverlayPassData.debugOverlay = debugOverlay;
				renderFragmentationOverlayPassData.debugFragmentationMaterial = this.m_DebugFragmentationMaterial;
				renderFragmentationOverlayPassData.colorBuffer = renderGraphBuilder.UseColorBuffer(colorBuffer, 0);
				renderFragmentationOverlayPassData.depthBuffer = renderGraphBuilder.UseDepthBuffer(depthBuffer, DepthAccess.ReadWrite);
				renderFragmentationOverlayPassData.debugFragmentationData = this.m_Index.GetDebugFragmentationBuffer();
				renderFragmentationOverlayPassData.chunkCount = renderFragmentationOverlayPassData.debugFragmentationData.count;
				renderGraphBuilder.SetRenderFunc<ProbeReferenceVolume.RenderFragmentationOverlayPassData>(delegate(ProbeReferenceVolume.RenderFragmentationOverlayPassData data, RenderGraphContext ctx)
				{
					MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
					data.debugOverlay.SetViewport(ctx.cmd);
					tempMaterialPropertyBlock.SetInt("_ChunkCount", data.chunkCount);
					tempMaterialPropertyBlock.SetBuffer("_DebugFragmentation", data.debugFragmentationData);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.debugFragmentationMaterial, 0, MeshTopology.Triangles, 3, 1, tempMaterialPropertyBlock);
					data.debugOverlay.Next(1f);
				});
			}
		}

		private bool ShouldCullCell(Vector3 cellPosition, Transform cameraTransform, Plane[] frustumPlanes)
		{
			Bounds cellBounds = this.GetCellBounds(cellPosition);
			float num = this.MaxBrickSize();
			float num2 = (float)Mathf.CeilToInt(this.probeVolumeDebug.probeCullingDistance / num) * num;
			return Vector3.Distance(cameraTransform.position, cellBounds.center) > num2 || !GeometryUtility.TestPlanesAABB(frustumPlanes, cellBounds);
		}

		private static void UpdateDebugFromSelection(ref Vector4[] _AdjustmentVolumeBounds, ref int _AdjustmentVolumeCount)
		{
			int s_ActiveAdjustmentVolumes = ProbeVolumeDebug.s_ActiveAdjustmentVolumes;
		}

		private Bounds GetCellBounds(Vector3 cellPosition)
		{
			float num = this.MaxBrickSize();
			return new Bounds(this.ProbeOffset() + ProbeVolumeDebug.currentOffset + cellPosition * num + Vector3.one * (num / 2f), num * Vector3.one);
		}

		private bool ShouldCullCell(Vector3 cellPosition, Vector4[] adjustmentVolumeBounds, int adjustmentVolumeCount)
		{
			Bounds cellBounds = this.GetCellBounds(cellPosition);
			for (int i = 0; i < adjustmentVolumeCount; i++)
			{
				Vector3 vector = adjustmentVolumeBounds[i * 3];
				if (adjustmentVolumeBounds[i * 3].w == 3.4028235E+38f)
				{
					float num = adjustmentVolumeBounds[i * 3 + 1].x * 2f;
					Bounds bounds = new Bounds(vector, new Vector3(num, num, num));
					if (bounds.Intersects(cellBounds))
					{
						return false;
					}
				}
				else
				{
					ProbeReferenceVolume.Volume volume = default(ProbeReferenceVolume.Volume);
					volume.X = adjustmentVolumeBounds[i * 3 + 1];
					volume.Y = adjustmentVolumeBounds[i * 3 + 2];
					volume.Z = new Vector3(adjustmentVolumeBounds[i * 3].w, adjustmentVolumeBounds[i * 3 + 1].w, adjustmentVolumeBounds[i * 3 + 2].w);
					volume.corner = vector - volume.X - volume.Y - volume.Z;
					volume.X *= 2f;
					volume.Y *= 2.05f;
					volume.Z *= 2f;
					Bounds bounds2 = volume.CalculateAABB();
					if (ProbeVolumePositioning.OBBAABBIntersect(volume, cellBounds, bounds2))
					{
						return false;
					}
				}
			}
			return true;
		}

		private void DrawProbeDebug(Camera camera, Texture exposureTexture)
		{
			if (!this.enabledBySRP || !this.isInitialized)
			{
				return;
			}
			bool flag = this.probeVolumeDebug.drawProbes;
			object obj = flag || this.probeVolumeDebug.drawVirtualOffsetPush || this.probeVolumeDebug.drawProbeSamplingDebug;
			int num = 0;
			Vector4[] array = ProbeReferenceVolume.s_BoundsArray;
			object obj2 = obj;
			if (obj2 == null && this.probeVolumeDebug.autoDrawProbes)
			{
				ProbeReferenceVolume.UpdateDebugFromSelection(ref array, ref num);
				flag |= (num != 0);
			}
			if (obj2 == null && !flag)
			{
				return;
			}
			GeometryUtility.CalculateFrustumPlanes(camera, this.m_DebugFrustumPlanes);
			this.m_DebugMaterial.shaderKeywords = null;
			if (this.m_SHBands == ProbeVolumeSHBands.SphericalHarmonicsL1)
			{
				this.m_DebugMaterial.EnableKeyword("PROBE_VOLUMES_L1");
			}
			else if (this.m_SHBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				this.m_DebugMaterial.EnableKeyword("PROBE_VOLUMES_L2");
			}
			this.m_DebugMaterial.renderQueue = 3000;
			this.m_DebugOffsetMaterial.renderQueue = 3000;
			this.m_ProbeSamplingDebugMaterial.renderQueue = 3000;
			this.m_ProbeSamplingDebugMaterial02.renderQueue = 3000;
			this.m_DebugMaterial.SetVector("_DebugEmptyProbeData", APVDefinitions.debugEmptyColor);
			if (this.probeVolumeDebug.drawProbeSamplingDebug)
			{
				this.m_ProbeSamplingDebugMaterial.SetInt("_ShadingMode", (int)this.probeVolumeDebug.probeShading);
				this.m_ProbeSamplingDebugMaterial.SetInt("_RenderingLayerMask", (int)this.probeVolumeDebug.samplingRenderingLayer);
				this.m_ProbeSamplingDebugMaterial.SetVector("_DebugArrowColor", new Vector4(1f, 1f, 1f, 1f));
				this.m_ProbeSamplingDebugMaterial.SetVector("_DebugLocator01Color", new Vector4(1f, 1f, 1f, 1f));
				this.m_ProbeSamplingDebugMaterial.SetVector("_DebugLocator02Color", new Vector4(0.3f, 0.3f, 0.3f, 1f));
				this.m_ProbeSamplingDebugMaterial.SetFloat("_ProbeSize", this.probeVolumeDebug.probeSamplingDebugSize);
				this.m_ProbeSamplingDebugMaterial.SetTexture("_NumbersTex", this.m_DisplayNumbersTexture);
				this.m_ProbeSamplingDebugMaterial.SetInt("_DebugSamplingNoise", Convert.ToInt32(this.probeVolumeDebug.debugWithSamplingNoise));
				this.m_ProbeSamplingDebugMaterial.SetInt("_ForceDebugNormalViewBias", 0);
				this.m_ProbeSamplingDebugMaterial.SetBuffer("_positionNormalBuffer", ProbeReferenceVolume.probeSamplingDebugData.positionNormalBuffer);
				Graphics.DrawMesh(this.m_DebugProbeSamplingMesh, new Vector4(0f, 0f, 0f, 1f), Quaternion.identity, this.m_ProbeSamplingDebugMaterial, 0, camera);
				Graphics.ClearRandomWriteTargets();
			}
			int num2 = (this.cells.Count > 0) ? (this.GetMaxSubdivision() - 1) : 0;
			foreach (ProbeReferenceVolume.Cell cell in this.cells.Values)
			{
				num2 = Mathf.Min(num2, cell.desc.minSubdiv);
			}
			int num3 = Mathf.Max(0, Mathf.Min(this.probeVolumeDebug.maxSubdivToVisualize, this.GetMaxSubdivision() - 1));
			int value = Mathf.Clamp(this.probeVolumeDebug.minSubdivToVisualize, num2, num3);
			this.m_MaxSubdivVisualizedIsMaxAvailable = (num3 == this.GetMaxSubdivision() - 1);
			bool flag2 = flag && !this.probeVolumeDebug.drawProbes && this.probeVolumeDebug.isolationProbeDebug;
			foreach (ProbeReferenceVolume.Cell cell2 in this.cells.Values)
			{
				if (!this.ShouldCullCell(cell2.desc.position, camera.transform, this.m_DebugFrustumPlanes) && (!flag2 || !this.ShouldCullCell(cell2.desc.position, array, num)))
				{
					ProbeReferenceVolume.CellInstancedDebugProbes cellInstancedDebugProbes = this.CreateInstancedProbes(cell2);
					if (cellInstancedDebugProbes != null)
					{
						for (int i = 0; i < cellInstancedDebugProbes.probeBuffers.Count; i++)
						{
							MaterialPropertyBlock materialPropertyBlock = cellInstancedDebugProbes.props[i];
							materialPropertyBlock.SetInt("_ShadingMode", (int)this.probeVolumeDebug.probeShading);
							materialPropertyBlock.SetFloat("_ExposureCompensation", this.probeVolumeDebug.exposureCompensation);
							materialPropertyBlock.SetFloat("_ProbeSize", this.probeVolumeDebug.probeSize);
							materialPropertyBlock.SetFloat("_CullDistance", this.probeVolumeDebug.probeCullingDistance);
							materialPropertyBlock.SetInt("_MaxAllowedSubdiv", num3);
							materialPropertyBlock.SetInt("_MinAllowedSubdiv", value);
							materialPropertyBlock.SetFloat("_ValidityThreshold", this.m_CurrentBakingSet.settings.dilationSettings.dilationValidityThreshold);
							materialPropertyBlock.SetInt("_RenderingLayerMask", (int)this.probeVolumeDebug.visibleLayers);
							materialPropertyBlock.SetFloat("_OffsetSize", this.probeVolumeDebug.offsetSize);
							materialPropertyBlock.SetTexture("_ExposureTexture", exposureTexture);
							if (flag)
							{
								this.m_DebugMaterial.SetVectorArray("_TouchupVolumeBounds", array);
								this.m_DebugMaterial.SetInt("_AdjustmentVolumeCount", this.probeVolumeDebug.isolationProbeDebug ? num : 0);
								this.m_DebugMaterial.SetVector("_ScreenSize", new Vector4((float)camera.pixelWidth, (float)camera.pixelHeight, 1f / (float)camera.pixelWidth, 1f / (float)camera.pixelHeight));
								Matrix4x4[] array2 = cellInstancedDebugProbes.probeBuffers[i];
								this.m_DebugMaterial.SetInt("_DebugProbeVolumeSampling", 0);
								this.m_DebugMaterial.SetBuffer("_positionNormalBuffer", ProbeReferenceVolume.probeSamplingDebugData.positionNormalBuffer);
								Graphics.DrawMeshInstanced(this.debugMesh, 0, this.m_DebugMaterial, array2, array2.Length, materialPropertyBlock, ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off, null);
							}
							if (this.probeVolumeDebug.drawProbeSamplingDebug)
							{
								Matrix4x4[] array3 = cellInstancedDebugProbes.probeBuffers[i];
								this.m_ProbeSamplingDebugMaterial02.SetInt("_DebugProbeVolumeSampling", 1);
								materialPropertyBlock.SetInt("_ShadingMode", 0);
								materialPropertyBlock.SetFloat("_ProbeSize", this.probeVolumeDebug.probeSamplingDebugSize);
								materialPropertyBlock.SetInt("_DebugSamplingNoise", Convert.ToInt32(this.probeVolumeDebug.debugWithSamplingNoise));
								materialPropertyBlock.SetInt("_RenderingLayerMask", (int)this.probeVolumeDebug.samplingRenderingLayer);
								this.m_ProbeSamplingDebugMaterial02.SetBuffer("_positionNormalBuffer", ProbeReferenceVolume.probeSamplingDebugData.positionNormalBuffer);
								Graphics.DrawMeshInstanced(this.debugMesh, 0, this.m_ProbeSamplingDebugMaterial02, array3, array3.Length, materialPropertyBlock, ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off, null);
							}
							if (this.probeVolumeDebug.drawVirtualOffsetPush)
							{
								this.m_DebugOffsetMaterial.SetVectorArray("_TouchupVolumeBounds", array);
								this.m_DebugOffsetMaterial.SetInt("_AdjustmentVolumeCount", this.probeVolumeDebug.isolationProbeDebug ? num : 0);
								Matrix4x4[] array4 = cellInstancedDebugProbes.offsetBuffers[i];
								Graphics.DrawMeshInstanced(this.m_DebugOffsetMesh, 0, this.m_DebugOffsetMaterial, array4, array4.Length, materialPropertyBlock, ShadowCastingMode.Off, false, 0, camera, LightProbeUsage.Off, null);
							}
						}
					}
				}
			}
		}

		internal void ResetDebugViewToMaxSubdiv()
		{
			if (this.m_MaxSubdivVisualizedIsMaxAvailable)
			{
				this.probeVolumeDebug.maxSubdivToVisualize = this.GetMaxSubdivision() - 1;
			}
		}

		private void ClearDebugData()
		{
			this.realtimeSubdivisionInfo.Clear();
		}

		private static void DecompressSH(ref SphericalHarmonicsL2 shv)
		{
			for (int i = 0; i < 3; i++)
			{
				float num = shv[i, 0];
				float num2 = 2f;
				float num3 = 3.5777087f;
				shv[i, 1] = (shv[i, 1] - 0.5f) * (num * num2 * 2f);
				shv[i, 2] = (shv[i, 2] - 0.5f) * (num * num2 * 2f);
				shv[i, 3] = (shv[i, 3] - 0.5f) * (num * num2 * 2f);
				shv[i, 4] = (shv[i, 4] - 0.5f) * (num * num3 * 2f);
				shv[i, 5] = (shv[i, 5] - 0.5f) * (num * num3 * 2f);
				shv[i, 6] = (shv[i, 6] - 0.5f) * (num * num3 * 2f);
				shv[i, 7] = (shv[i, 7] - 0.5f) * (num * num3 * 2f);
				shv[i, 8] = (shv[i, 8] - 0.5f) * (num * num3 * 2f);
			}
		}

		internal static Vector3 DecodeSkyShadingDirection(uint directionIndex)
		{
			Vector3[] skySamplingDirections = ProbeVolumeConstantRuntimeResources.GetSkySamplingDirections();
			if ((ulong)directionIndex != (ulong)((long)skySamplingDirections.Length))
			{
				return skySamplingDirections[(int)directionIndex];
			}
			return new Vector3(0f, 0f, 0f);
		}

		internal bool GetFlattenedProbeData(string scenario, out Vector3[] positions, out SphericalHarmonicsL2[] irradiance, out float[] validity, out Vector4[] occlusion, out Vector4[] skyOcclusion, out Vector3[] skyOcclusionDirections, out Vector3[] virtualOffset)
		{
			positions = null;
			irradiance = null;
			validity = null;
			occlusion = null;
			skyOcclusion = null;
			skyOcclusionDirections = null;
			virtualOffset = null;
			List<Vector3> list = new List<Vector3>();
			List<SphericalHarmonicsL2> list2 = new List<SphericalHarmonicsL2>();
			List<float> list3 = new List<float>();
			List<Vector4> list4 = new List<Vector4>();
			List<Vector4> list5 = new List<Vector4>();
			List<Vector3> list6 = new List<Vector3>();
			List<Vector3> list7 = new List<Vector3>();
			foreach (ProbeReferenceVolume.Cell cell in this.cells.Values)
			{
				if (this.HasActiveStreamingRequest(cell))
				{
					return false;
				}
				if (!cell.data.bricks.IsCreated || cell.data.bricks.Length == 0 || !cell.data.probePositions.IsCreated || !cell.loaded)
				{
					return false;
				}
				ProbeReferenceVolume.CellData.PerScenarioData perScenarioData;
				if (!cell.data.scenarios.TryGetValue(scenario, out perScenarioData))
				{
					return false;
				}
				List<ProbeBrickPool.BrickChunkAlloc> chunkList = cell.poolInfo.chunkList;
				int chunkSizeInProbeCount = ProbeBrickPool.GetChunkSizeInProbeCount();
				Vector3Int vector3Int = ProbeBrickPool.ProbeCountToDataLocSize(chunkSizeInProbeCount);
				int num = cell.desc.probeCount / 64;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				for (int i = 0; i < num; i++)
				{
					int num5 = i / ProbeBrickPool.GetChunkSizeInBrickCount();
					ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = chunkList[num5];
					Vector3Int vector3Int2 = new Vector3Int(brickChunkAlloc.x + num2, brickChunkAlloc.y + num3, brickChunkAlloc.z + num4);
					for (int j = 0; j < 4; j++)
					{
						for (int k = 0; k < 4; k++)
						{
							for (int l = 0; l < 4; l++)
							{
								new Vector3Int(vector3Int2.x + l, vector3Int2.y + k, vector3Int2.z + j);
								int num6 = num5 * chunkSizeInProbeCount + (num2 + l) + vector3Int.x * (num3 + k + vector3Int.y * (num4 + j));
								Vector3 item = cell.data.probePositions[num6] - this.ProbeOffset();
								list.Add(item);
								list3.Add(cell.data.validity[num6]);
								int num7 = num6 * 4;
								float x = (float)perScenarioData.probeOcclusion[num7] / 255f;
								float y = (float)perScenarioData.probeOcclusion[num7 + 1] / 255f;
								float z = (float)perScenarioData.probeOcclusion[num7 + 2] / 255f;
								float w = (float)perScenarioData.probeOcclusion[num7 + 3] / 255f;
								list4.Add(new Vector4(x, y, z, w));
								if (cell.data.skyOcclusionDataL0L1.Length > 0)
								{
									float x2 = Mathf.HalfToFloat(cell.data.skyOcclusionDataL0L1[num6 * 4]);
									float y2 = Mathf.HalfToFloat(cell.data.skyOcclusionDataL0L1[num6 * 4 + 1]);
									float z2 = Mathf.HalfToFloat(cell.data.skyOcclusionDataL0L1[num6 * 4 + 2]);
									float w2 = Mathf.HalfToFloat(cell.data.skyOcclusionDataL0L1[num6 * 4 + 3]);
									list5.Add(new Vector4(x2, y2, z2, w2));
								}
								if (cell.data.skyShadingDirectionIndices.Length > 0)
								{
									Vector3 item2 = ProbeReferenceVolume.DecodeSkyShadingDirection((uint)cell.data.skyShadingDirectionIndices[num6]);
									list6.Add(item2);
								}
								if (cell.data.offsetVectors.Length > 0)
								{
									Vector3 item3 = cell.data.offsetVectors[num6];
									list7.Add(item3);
								}
								Vector4 zero = Vector4.zero;
								Vector4 zero2 = Vector4.zero;
								Vector4 zero3 = Vector4.zero;
								Vector4 zero4 = Vector4.zero;
								Vector4 zero5 = Vector4.zero;
								Vector4 zero6 = Vector4.zero;
								Vector4 zero7 = Vector4.zero;
								for (int m = 0; m < 4; m++)
								{
									zero[m] = Mathf.HalfToFloat(perScenarioData.shL0L1RxData[num6 * 4 + m]);
									zero2[m] = (float)perScenarioData.shL1GL1RyData[num6 * 4 + m] / 255f;
									zero3[m] = (float)perScenarioData.shL1BL1RzData[num6 * 4 + m] / 255f;
									if (ProbeReferenceVolume.instance.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
									{
										zero4[m] = (float)perScenarioData.shL2Data_0[num6 * 4 + m] / 255f;
										zero5[m] = (float)perScenarioData.shL2Data_1[num6 * 4 + m] / 255f;
										zero6[m] = (float)perScenarioData.shL2Data_2[num6 * 4 + m] / 255f;
										zero7[m] = (float)perScenarioData.shL2Data_3[num6 * 4 + m] / 255f;
									}
								}
								Vector3 vector = new Vector3(zero.x, zero.y, zero.z);
								Vector3 vector2 = new Vector3(zero2.w, zero3.w, zero.w);
								Vector3 vector3 = new Vector3(zero2.y, zero2.z, zero2.x);
								Vector3 vector4 = new Vector3(zero3.y, zero3.z, zero3.x);
								SphericalHarmonicsL2 item4 = default(SphericalHarmonicsL2);
								for (int n = 0; n < 3; n++)
								{
									item4[n, 0] = vector[n];
									item4[0, n + 1] = vector2[n];
									item4[1, n + 1] = vector3[n];
									item4[2, n + 1] = vector4[n];
								}
								item4[0, 4] = zero4.x;
								item4[0, 5] = zero4.y;
								item4[0, 6] = zero4.z;
								item4[0, 7] = zero4.w;
								item4[0, 8] = zero7.x;
								item4[1, 4] = zero5.x;
								item4[1, 5] = zero5.y;
								item4[1, 6] = zero5.z;
								item4[1, 7] = zero5.w;
								item4[1, 8] = zero7.y;
								item4[2, 4] = zero6.x;
								item4[2, 5] = zero6.y;
								item4[2, 6] = zero6.z;
								item4[2, 7] = zero6.w;
								item4[2, 8] = zero7.z;
								ProbeReferenceVolume.DecompressSH(ref item4);
								if (ProbeReferenceVolume.instance.shBands != ProbeVolumeSHBands.SphericalHarmonicsL2)
								{
									for (int num8 = 0; num8 < 5; num8++)
									{
										item4[0, num8 + 4] = 0f;
										item4[1, num8 + 4] = 0f;
										item4[2, num8 + 4] = 0f;
									}
								}
								list2.Add(item4);
							}
						}
					}
					num2 += 4;
					if (num2 >= vector3Int.x)
					{
						num2 = 0;
						num3 += 4;
						if (num3 >= vector3Int.y)
						{
							num3 = 0;
							num4 += 4;
							if (num4 >= vector3Int.z)
							{
								num2 = 0;
								num3 = 0;
								num4 = 0;
							}
						}
					}
				}
			}
			positions = list.ToArray();
			irradiance = list2.ToArray();
			validity = list3.ToArray();
			occlusion = list4.ToArray();
			skyOcclusion = list5.ToArray();
			skyOcclusionDirections = list6.ToArray();
			virtualOffset = list7.ToArray();
			return true;
		}

		private ProbeReferenceVolume.CellInstancedDebugProbes CreateInstancedProbes(ProbeReferenceVolume.Cell cell)
		{
			if (cell.debugProbes != null)
			{
				return cell.debugProbes;
			}
			if (this.HasActiveStreamingRequest(cell))
			{
				return null;
			}
			int num = this.GetMaxSubdivision() - 1;
			if (!cell.data.bricks.IsCreated || cell.data.bricks.Length == 0 || !cell.data.probePositions.IsCreated || !cell.loaded)
			{
				return null;
			}
			List<Matrix4x4[]> list = new List<Matrix4x4[]>();
			List<Matrix4x4[]> list2 = new List<Matrix4x4[]>();
			List<MaterialPropertyBlock> list3 = new List<MaterialPropertyBlock>();
			List<ProbeBrickPool.BrickChunkAlloc> chunkList = cell.poolInfo.chunkList;
			Vector4[] array = new Vector4[511];
			float[] array2 = new float[511];
			float[] array3 = new float[511];
			float[] array4 = new float[511];
			float[] array5 = new float[511];
			float[] array6 = (cell.data.touchupVolumeInteraction.Length > 0) ? new float[511] : null;
			Vector4[] array7 = (cell.data.offsetVectors.Length > 0) ? new Vector4[511] : null;
			List<Matrix4x4> list4 = new List<Matrix4x4>();
			List<Matrix4x4> list5 = new List<Matrix4x4>();
			ProbeReferenceVolume.CellInstancedDebugProbes cellInstancedDebugProbes = new ProbeReferenceVolume.CellInstancedDebugProbes();
			cellInstancedDebugProbes.probeBuffers = list;
			cellInstancedDebugProbes.offsetBuffers = list2;
			cellInstancedDebugProbes.props = list3;
			int chunkSizeInProbeCount = ProbeBrickPool.GetChunkSizeInProbeCount();
			Vector3Int vector3Int = ProbeBrickPool.ProbeCountToDataLocSize(chunkSizeInProbeCount);
			float dilationValidityThreshold = this.m_CurrentBakingSet.settings.dilationSettings.dilationValidityThreshold;
			int num2 = 0;
			int num3 = 0;
			int num4 = cell.desc.probeCount / 64;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			for (int i = 0; i < num4; i++)
			{
				int subdivisionLevel = cell.data.bricks[i].subdivisionLevel;
				int num8 = i / ProbeBrickPool.GetChunkSizeInBrickCount();
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = chunkList[num8];
				Vector3Int vector3Int2 = new Vector3Int(brickChunkAlloc.x + num5, brickChunkAlloc.y + num6, brickChunkAlloc.z + num7);
				for (int j = 0; j < 4; j++)
				{
					for (int k = 0; k < 4; k++)
					{
						for (int l = 0; l < 4; l++)
						{
							Vector3Int vector3Int3 = new Vector3Int(vector3Int2.x + l, vector3Int2.y + k, vector3Int2.z + j);
							int index = num8 * chunkSizeInProbeCount + (num5 + l) + vector3Int.x * (num6 + k + vector3Int.y * (num7 + j));
							Vector3 vector = cell.data.probePositions[index] - this.ProbeOffset();
							list4.Add(Matrix4x4.TRS(vector, Quaternion.identity, Vector3.one * (0.3f * (float)(subdivisionLevel + 1))));
							array3[num2] = cell.data.validity[index];
							array4[num2] = dilationValidityThreshold;
							array[num2] = new Vector4((float)vector3Int3.x, (float)vector3Int3.y, (float)vector3Int3.z, (float)subdivisionLevel);
							array5[num2] = (float)subdivisionLevel / (float)num;
							array2[num2] = math.asfloat((cell.data.layer.Length > 0) ? ((uint)cell.data.layer[index]) : uint.MaxValue);
							if (array6 != null)
							{
								array6[num2] = cell.data.touchupVolumeInteraction[index];
								array4[num2] = ((array6[num2] > 1f) ? (array6[num2] - 1f) : dilationValidityThreshold);
							}
							if (array7 != null)
							{
								Vector3 vector2 = cell.data.offsetVectors[index];
								array7[num2] = vector2;
								if (vector2.sqrMagnitude < 1E-06f)
								{
									list5.Add(Matrix4x4.identity);
								}
								else
								{
									Quaternion q = Quaternion.LookRotation(-vector2);
									Vector3 s = new Vector3(0.5f, 0.5f, vector2.magnitude);
									list5.Add(Matrix4x4.TRS(vector + vector2, q, s));
								}
							}
							num2++;
							if (list4.Count >= 511 || num3 == cell.desc.probeCount - 1)
							{
								num2 = 0;
								MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
								materialPropertyBlock.SetFloatArray("_Validity", array3);
								materialPropertyBlock.SetFloatArray("_RenderingLayer", array2);
								materialPropertyBlock.SetFloatArray("_DilationThreshold", array4);
								materialPropertyBlock.SetFloatArray("_TouchupedByVolume", array6);
								materialPropertyBlock.SetFloatArray("_RelativeSize", array5);
								materialPropertyBlock.SetVectorArray("_IndexInAtlas", array);
								if (array7 != null)
								{
									materialPropertyBlock.SetVectorArray("_Offset", array7);
								}
								list3.Add(materialPropertyBlock);
								list.Add(list4.ToArray());
								list4.Clear();
								list2.Add(list5.ToArray());
								list5.Clear();
							}
							num3++;
						}
					}
				}
				num5 += 4;
				if (num5 >= vector3Int.x)
				{
					num5 = 0;
					num6 += 4;
					if (num6 >= vector3Int.y)
					{
						num6 = 0;
						num7 += 4;
						if (num7 >= vector3Int.z)
						{
							num5 = 0;
							num6 = 0;
							num7 = 0;
						}
					}
				}
			}
			cell.debugProbes = cellInstancedDebugProbes;
			return cellInstancedDebugProbes;
		}

		private void OnClearLightingdata()
		{
			this.ClearDebugData();
		}

		public void EnableMaxCellStreaming(bool value)
		{
			this.m_LoadMaxCellsPerFrame = value;
		}

		public void SetNumberOfCellsLoadedPerFrame(int numberOfCells)
		{
			this.m_NumberOfCellsLoadedPerFrame = Mathf.Min(10, Mathf.Max(1, numberOfCells));
		}

		public bool loadMaxCellsPerFrame
		{
			get
			{
				return this.m_LoadMaxCellsPerFrame;
			}
			set
			{
				this.m_LoadMaxCellsPerFrame = value;
			}
		}

		private int numberOfCellsLoadedPerFrame
		{
			get
			{
				if (!this.m_LoadMaxCellsPerFrame)
				{
					return this.m_NumberOfCellsLoadedPerFrame;
				}
				return this.cells.Count;
			}
		}

		public int numberOfCellsBlendedPerFrame
		{
			get
			{
				return this.m_NumberOfCellsBlendedPerFrame;
			}
			set
			{
				this.m_NumberOfCellsBlendedPerFrame = Mathf.Max(1, value);
			}
		}

		public float turnoverRate
		{
			get
			{
				return this.m_TurnoverRate;
			}
			set
			{
				this.m_TurnoverRate = Mathf.Clamp01(value);
			}
		}

		private void InitStreaming()
		{
			this.m_OnStreamingComplete = new ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate(this.OnStreamingComplete);
			this.m_OnBlendingStreamingComplete = new ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate(this.OnBlendingStreamingComplete);
		}

		private void CleanupStreaming()
		{
			this.ProcessNewRequests();
			this.UpdateActiveRequests(null);
			for (int i = 0; i < this.m_StreamingRequestsPool.countAll; i++)
			{
				this.m_StreamingRequestsPool.Get().Dispose();
			}
			if (this.m_ScratchBufferPool != null)
			{
				this.m_ScratchBufferPool.Cleanup();
				this.m_ScratchBufferPool = null;
			}
			this.m_StreamingRequestsPool = new ObjectPool<ProbeReferenceVolume.CellStreamingRequest>(delegate(ProbeReferenceVolume.CellStreamingRequest val)
			{
				val.Clear();
			}, null, true);
			this.m_ActiveStreamingRequests.Clear();
			this.m_StreamingQueue.Clear();
			this.m_OnStreamingComplete = null;
			this.m_OnBlendingStreamingComplete = null;
		}

		internal unsafe void ScenarioBlendingChanged(bool scenarioChanged)
		{
			if (scenarioChanged)
			{
				this.UnloadAllBlendingCells();
				for (int i = 0; i < this.m_ToBeLoadedBlendingCells.size; i++)
				{
					this.m_ToBeLoadedBlendingCells[i]->blendingInfo.ForceReupload();
				}
			}
		}

		private static void ComputeCellStreamingScore(ProbeReferenceVolume.Cell cell, Vector3 cameraPosition, Vector3 cameraDirection)
		{
			Vector3 normalized = (cell.desc.position - cameraPosition).normalized;
			cell.streamingInfo.streamingScore = Vector3.Distance(cameraPosition, cell.desc.position);
			cell.streamingInfo.streamingScore *= 2f - Vector3.Dot(cameraDirection, normalized);
		}

		private unsafe void ComputeStreamingScore(Vector3 cameraPosition, Vector3 cameraDirection, DynamicArray<ProbeReferenceVolume.Cell> cells)
		{
			for (int i = 0; i < cells.size; i++)
			{
				ProbeReferenceVolume.ComputeCellStreamingScore(*cells[i], cameraPosition, cameraDirection);
			}
		}

		private unsafe void ComputeBestToBeLoadedCells(Vector3 cameraPosition, Vector3 cameraDirection)
		{
			this.m_BestToBeLoadedCells.Clear();
			this.m_BestToBeLoadedCells.Reserve(this.m_ToBeLoadedCells.size, false);
			foreach (ProbeReferenceVolume.Cell ptr in this.m_ToBeLoadedCells)
			{
				ProbeReferenceVolume.Cell cell = ptr;
				ProbeReferenceVolume.ComputeCellStreamingScore(cell, cameraPosition, cameraDirection);
				this.minStreamingScore = Mathf.Min(this.minStreamingScore, cell.streamingInfo.streamingScore);
				this.maxStreamingScore = Mathf.Max(this.maxStreamingScore, cell.streamingInfo.streamingScore);
				int num = Math.Min(this.m_BestToBeLoadedCells.size, this.numberOfCellsLoadedPerFrame);
				int num2 = 0;
				while (num2 < num && cell.streamingInfo.streamingScore >= this.m_BestToBeLoadedCells[num2]->streamingInfo.streamingScore)
				{
					num2++;
				}
				if (num2 < this.numberOfCellsLoadedPerFrame)
				{
					this.m_BestToBeLoadedCells.Insert(num2, cell);
				}
				if (this.m_BestToBeLoadedCells.size > this.numberOfCellsLoadedPerFrame)
				{
					this.m_BestToBeLoadedCells.Resize(this.numberOfCellsLoadedPerFrame, false);
				}
			}
		}

		private unsafe void ComputeStreamingScoreAndWorseLoadedCells(Vector3 cameraPosition, Vector3 cameraDirection)
		{
			this.m_WorseLoadedCells.Clear();
			this.m_WorseLoadedCells.Reserve(this.m_LoadedCells.size, false);
			int num = 0;
			int num2 = 0;
			foreach (ProbeReferenceVolume.Cell ptr in this.m_BestToBeLoadedCells)
			{
				ProbeReferenceVolume.Cell cell = ptr;
				num += cell.desc.shChunkCount;
				num2 += cell.desc.indexChunkCount;
			}
			foreach (ProbeReferenceVolume.Cell ptr2 in this.m_LoadedCells)
			{
				ProbeReferenceVolume.Cell cell2 = ptr2;
				ProbeReferenceVolume.ComputeCellStreamingScore(cell2, cameraPosition, cameraDirection);
				this.minStreamingScore = Mathf.Min(this.minStreamingScore, cell2.streamingInfo.streamingScore);
				this.maxStreamingScore = Mathf.Max(this.maxStreamingScore, cell2.streamingInfo.streamingScore);
				int size = this.m_WorseLoadedCells.size;
				int num3 = 0;
				while (num3 < size && cell2.streamingInfo.streamingScore <= this.m_WorseLoadedCells[num3]->streamingInfo.streamingScore)
				{
					num3++;
				}
				this.m_WorseLoadedCells.Insert(num3, cell2);
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				for (int i = 0; i < this.m_WorseLoadedCells.size; i++)
				{
					ProbeReferenceVolume.Cell cell3 = *this.m_WorseLoadedCells[i];
					num4 += cell3.desc.shChunkCount;
					num5 += cell3.desc.indexChunkCount;
					if (num4 >= num && num5 >= num2)
					{
						num6 = i + 1;
						break;
					}
				}
				if (num6 != 0)
				{
					this.m_WorseLoadedCells.Resize(num6, false);
				}
			}
		}

		private unsafe void ComputeBlendingScore(DynamicArray<ProbeReferenceVolume.Cell> cells, float worstScore)
		{
			float scenarioBlendingFactor = this.scenarioBlendingFactor;
			for (int i = 0; i < cells.size; i++)
			{
				ProbeReferenceVolume.Cell cell = *cells[i];
				ProbeReferenceVolume.CellBlendingInfo blendingInfo = cell.blendingInfo;
				if (scenarioBlendingFactor != blendingInfo.blendingFactor)
				{
					blendingInfo.blendingScore = cell.streamingInfo.streamingScore;
					if (blendingInfo.ShouldPrioritize())
					{
						blendingInfo.blendingScore -= worstScore;
					}
				}
			}
		}

		private bool TryLoadCell(ProbeReferenceVolume.Cell cell, ref int shBudget, ref int indexBudget, DynamicArray<ProbeReferenceVolume.Cell> loadedCells)
		{
			if (cell.poolInfo.shChunkCount <= shBudget && cell.indexInfo.indexChunkCount <= indexBudget && this.LoadCell(cell, true))
			{
				loadedCells.Add(cell);
				shBudget -= cell.poolInfo.shChunkCount;
				indexBudget -= cell.indexInfo.indexChunkCount;
				return true;
			}
			return false;
		}

		private void UnloadBlendingCell(ProbeReferenceVolume.Cell cell, DynamicArray<ProbeReferenceVolume.Cell> unloadedCells)
		{
			this.UnloadBlendingCell(cell);
			unloadedCells.Add(cell);
		}

		private bool TryLoadBlendingCell(ProbeReferenceVolume.Cell cell, DynamicArray<ProbeReferenceVolume.Cell> loadedCells)
		{
			if (!cell.UpdateCellScenarioData(this.lightingScenario, this.m_CurrentBakingSet.otherScenario))
			{
				return false;
			}
			if (!this.AddBlendingBricks(cell))
			{
				return false;
			}
			loadedCells.Add(cell);
			return true;
		}

		private unsafe void ComputeMinMaxStreamingScore()
		{
			this.minStreamingScore = float.MaxValue;
			this.maxStreamingScore = float.MinValue;
			if (this.m_ToBeLoadedCells.size != 0)
			{
				this.minStreamingScore = Mathf.Min(this.minStreamingScore, this.m_ToBeLoadedCells[0]->streamingInfo.streamingScore);
				this.maxStreamingScore = Mathf.Max(this.maxStreamingScore, this.m_ToBeLoadedCells[this.m_ToBeLoadedCells.size - 1]->streamingInfo.streamingScore);
			}
			if (this.m_LoadedCells.size != 0)
			{
				this.minStreamingScore = Mathf.Min(this.minStreamingScore, this.m_LoadedCells[0]->streamingInfo.streamingScore);
				this.maxStreamingScore = Mathf.Max(this.maxStreamingScore, this.m_LoadedCells[this.m_LoadedCells.size - 1]->streamingInfo.streamingScore);
			}
		}

		public void UpdateCellStreaming(CommandBuffer cmd, Camera camera)
		{
			this.UpdateCellStreaming(cmd, camera, null);
		}

		public unsafe void UpdateCellStreaming(CommandBuffer cmd, Camera camera, ProbeVolumesOptions options)
		{
			if (!this.isInitialized || this.m_CurrentBakingSet == null)
			{
				return;
			}
			using (new ProfilingScope(ProfilingSampler.Get<CoreProfileId>(CoreProfileId.APVCellStreamingUpdate)))
			{
				Vector3 position = camera.transform.position;
				if (!this.probeVolumeDebug.freezeStreaming)
				{
					this.m_FrozenCameraPosition = position;
					this.m_FrozenCameraDirection = camera.transform.forward;
				}
				Vector3 b = this.ProbeOffset() + ((options != null) ? options.worldOffset.value : Vector3.zero);
				Vector3 cameraPosition = (this.m_FrozenCameraPosition - b) / this.MaxBrickSize() - Vector3.one * 0.5f;
				DynamicArray<ProbeReferenceVolume.Cell> dynamicArray;
				if (this.m_LoadMaxCellsPerFrame)
				{
					this.ComputeStreamingScore(cameraPosition, this.m_FrozenCameraDirection, this.m_ToBeLoadedCells);
					this.m_ToBeLoadedCells.QuickSort<ProbeReferenceVolume.Cell>();
					dynamicArray = this.m_ToBeLoadedCells;
				}
				else
				{
					this.minStreamingScore = float.MaxValue;
					this.maxStreamingScore = float.MinValue;
					this.ComputeBestToBeLoadedCells(cameraPosition, this.m_FrozenCameraDirection);
					dynamicArray = this.m_BestToBeLoadedCells;
				}
				int num = this.m_Index.GetRemainingChunkCount();
				int num2 = this.m_Pool.GetRemainingChunkCount();
				int num3 = Mathf.Min(this.numberOfCellsLoadedPerFrame, dynamicArray.size);
				bool flag = false;
				if (this.m_SupportGPUStreaming)
				{
					if (this.m_IndexDefragmentationInProgress)
					{
						this.UpdateIndexDefragmentation();
					}
					else
					{
						bool flag2 = false;
						while (this.m_TempCellToLoadList.size < num3)
						{
							ProbeReferenceVolume.Cell cell = *dynamicArray[this.m_TempCellToLoadList.size];
							if (!this.TryLoadCell(cell, ref num2, ref num, this.m_TempCellToLoadList))
							{
								break;
							}
						}
						if (this.m_TempCellToLoadList.size != num3 && !this.m_IndexDefragmentationInProgress)
						{
							DynamicArray<ProbeReferenceVolume.Cell> dynamicArray2;
							if (this.m_LoadMaxCellsPerFrame)
							{
								this.ComputeStreamingScore(cameraPosition, this.m_FrozenCameraDirection, this.m_LoadedCells);
								this.m_LoadedCells.QuickSort<ProbeReferenceVolume.Cell>();
								dynamicArray2 = this.m_LoadedCells;
							}
							else
							{
								this.ComputeStreamingScoreAndWorseLoadedCells(cameraPosition, this.m_FrozenCameraDirection);
								dynamicArray2 = this.m_WorseLoadedCells;
							}
							flag = true;
							int num4 = 0;
							while (this.m_TempCellToLoadList.size < num3 && dynamicArray2.size - num4 != 0)
							{
								int index = this.m_LoadMaxCellsPerFrame ? (dynamicArray2.size - num4 - 1) : num4;
								ProbeReferenceVolume.Cell cell2 = *dynamicArray2[index];
								ProbeReferenceVolume.Cell cell3 = *dynamicArray[this.m_TempCellToLoadList.size];
								if (cell2.streamingInfo.streamingScore <= cell3.streamingInfo.streamingScore)
								{
									break;
								}
								while (num4 < dynamicArray2.size && cell2.streamingInfo.streamingScore > cell3.streamingInfo.streamingScore && (num2 < cell3.desc.shChunkCount || num < cell3.desc.indexChunkCount))
								{
									bool verboseStreamingLog = this.probeVolumeDebug.verboseStreamingLog;
									num4++;
									this.UnloadCell(cell2);
									num2 += cell2.desc.shChunkCount;
									num += cell2.desc.indexChunkCount;
									this.m_TempCellToUnloadList.Add(cell2);
									index = (this.m_LoadMaxCellsPerFrame ? (dynamicArray2.size - num4 - 1) : num4);
									if (num4 < dynamicArray2.size)
									{
										cell2 = *dynamicArray2[index];
									}
								}
								if (num2 >= cell3.desc.shChunkCount && num >= cell3.desc.indexChunkCount && !this.TryLoadCell(cell3, ref num2, ref num, this.m_TempCellToLoadList))
								{
									flag2 = true;
									break;
								}
							}
						}
						if (flag2)
						{
							this.m_Index.ComputeFragmentationRate();
						}
						if (this.m_Index.fragmentationRate >= 0.2f)
						{
							this.StartIndexDefragmentation();
						}
					}
				}
				else
				{
					int i = 0;
					while (i < num3)
					{
						ProbeReferenceVolume.Cell cell4 = *this.m_ToBeLoadedCells[this.m_TempCellToLoadList.size];
						if (!this.TryLoadCell(cell4, ref num2, ref num, this.m_TempCellToLoadList))
						{
							if (i > 0)
							{
								Debug.LogWarning("Max Memory Budget for Adaptive Probe Volumes has been reached, but there is still more data to load. Consider either increasing the Memory Budget, enabling GPU Streaming, or reducing the probe count.");
								break;
							}
							break;
						}
						else
						{
							i++;
						}
					}
				}
				if (!flag && this.supportScenarioBlending)
				{
					this.ComputeStreamingScore(cameraPosition, this.m_FrozenCameraDirection, this.m_LoadedCells);
				}
				if (this.m_LoadMaxCellsPerFrame)
				{
					this.ComputeMinMaxStreamingScore();
				}
				foreach (ProbeReferenceVolume.Cell ptr in this.m_TempCellToLoadList)
				{
					ProbeReferenceVolume.Cell item = ptr;
					this.m_ToBeLoadedCells.Remove(item);
				}
				this.m_LoadedCells.AddRange(this.m_TempCellToLoadList);
				if (this.m_TempCellToUnloadList.size > 0)
				{
					foreach (ProbeReferenceVolume.Cell ptr2 in this.m_TempCellToUnloadList)
					{
						ProbeReferenceVolume.Cell item2 = ptr2;
						this.m_LoadedCells.Remove(item2);
					}
					this.ComputeCellGlobalInfo();
				}
				this.m_ToBeLoadedCells.AddRange(this.m_TempCellToUnloadList);
				this.m_TempCellToLoadList.Clear();
				this.m_TempCellToUnloadList.Clear();
				this.UpdateDiskStreaming(cmd);
			}
			if (this.supportScenarioBlending)
			{
				using (new ProfilingScope(cmd, ProfilingSampler.Get<CoreProfileId>(CoreProfileId.APVScenarioBlendingUpdate)))
				{
					this.UpdateBlendingCellStreaming(cmd);
				}
			}
		}

		private unsafe int FindWorstBlendingCellToBeLoaded()
		{
			int result = -1;
			float num = -1f;
			float scenarioBlendingFactor = this.scenarioBlendingFactor;
			for (int i = this.m_TempBlendingCellToLoadList.size; i < this.m_ToBeLoadedBlendingCells.size; i++)
			{
				float num2 = Mathf.Abs(this.m_ToBeLoadedBlendingCells[i]->blendingInfo.blendingFactor - scenarioBlendingFactor);
				if (num2 > num)
				{
					result = i;
					if (this.m_ToBeLoadedBlendingCells[i]->blendingInfo.ShouldReupload())
					{
						break;
					}
					num = num2;
				}
			}
			return result;
		}

		private static int BlendingComparer(ProbeReferenceVolume.Cell a, ProbeReferenceVolume.Cell b)
		{
			if (a.blendingInfo.blendingScore < b.blendingInfo.blendingScore)
			{
				return -1;
			}
			if (a.blendingInfo.blendingScore > b.blendingInfo.blendingScore)
			{
				return 1;
			}
			return 0;
		}

		private unsafe void UpdateBlendingCellStreaming(CommandBuffer cmd)
		{
			float a = (this.m_LoadedCells.size != 0) ? this.m_LoadedCells[this.m_LoadedCells.size - 1]->streamingInfo.streamingScore : 0f;
			float b = (this.m_ToBeLoadedCells.size != 0) ? this.m_ToBeLoadedCells[this.m_ToBeLoadedCells.size - 1]->streamingInfo.streamingScore : 0f;
			float worstScore = Mathf.Max(a, b);
			this.ComputeBlendingScore(this.m_ToBeLoadedBlendingCells, worstScore);
			this.ComputeBlendingScore(this.m_LoadedBlendingCells, worstScore);
			this.m_ToBeLoadedBlendingCells.QuickSort(ProbeReferenceVolume.s_BlendingComparer);
			this.m_LoadedBlendingCells.QuickSort(ProbeReferenceVolume.s_BlendingComparer);
			int num = Mathf.Min(this.numberOfCellsLoadedPerFrame, this.m_ToBeLoadedBlendingCells.size);
			while (this.m_TempBlendingCellToLoadList.size < num)
			{
				ProbeReferenceVolume.Cell cell = *this.m_ToBeLoadedBlendingCells[this.m_TempBlendingCellToLoadList.size];
				if (!this.TryLoadBlendingCell(cell, this.m_TempBlendingCellToLoadList))
				{
					break;
				}
			}
			if (this.m_TempBlendingCellToLoadList.size != num)
			{
				int num2 = -1;
				int num3 = (int)((float)this.m_LoadedBlendingCells.size * (1f - this.turnoverRate));
				ProbeReferenceVolume.Cell cell2 = (num3 < this.m_LoadedBlendingCells.size) ? (*this.m_LoadedBlendingCells[num3]) : null;
				while (this.m_TempBlendingCellToLoadList.size < num && this.m_LoadedBlendingCells.size - this.m_TempBlendingCellToUnloadList.size != 0)
				{
					ProbeReferenceVolume.Cell cell3 = *this.m_LoadedBlendingCells[this.m_LoadedBlendingCells.size - this.m_TempBlendingCellToUnloadList.size - 1];
					ProbeReferenceVolume.Cell cell4 = *this.m_ToBeLoadedBlendingCells[this.m_TempBlendingCellToLoadList.size];
					if (cell4.blendingInfo.blendingScore >= (cell2 ?? cell3).blendingInfo.blendingScore)
					{
						if (cell2 == null)
						{
							break;
						}
						if (num2 == -1)
						{
							num2 = this.FindWorstBlendingCellToBeLoaded();
						}
						cell4 = *this.m_ToBeLoadedBlendingCells[num2];
						if (cell4.blendingInfo.IsUpToDate())
						{
							break;
						}
					}
					if (cell3.streamingInfo.IsBlendingStreaming())
					{
						break;
					}
					this.UnloadBlendingCell(cell3, this.m_TempBlendingCellToUnloadList);
					bool verboseStreamingLog = this.probeVolumeDebug.verboseStreamingLog;
					if (this.TryLoadBlendingCell(cell4, this.m_TempBlendingCellToLoadList) && num2 != -1)
					{
						*this.m_ToBeLoadedBlendingCells[num2] = *this.m_ToBeLoadedBlendingCells[this.m_TempBlendingCellToLoadList.size - 1];
						*this.m_ToBeLoadedBlendingCells[this.m_TempBlendingCellToLoadList.size - 1] = cell4;
						if (++num2 >= this.m_ToBeLoadedBlendingCells.size)
						{
							num2 = this.m_TempBlendingCellToLoadList.size;
						}
					}
				}
				this.m_LoadedBlendingCells.RemoveRange(this.m_LoadedBlendingCells.size - this.m_TempBlendingCellToUnloadList.size, this.m_TempBlendingCellToUnloadList.size);
			}
			this.m_ToBeLoadedBlendingCells.RemoveRange(0, this.m_TempBlendingCellToLoadList.size);
			this.m_LoadedBlendingCells.AddRange(this.m_TempBlendingCellToLoadList);
			this.m_TempBlendingCellToLoadList.Clear();
			this.m_ToBeLoadedBlendingCells.AddRange(this.m_TempBlendingCellToUnloadList);
			this.m_TempBlendingCellToUnloadList.Clear();
			if (this.m_LoadedBlendingCells.size != 0)
			{
				float scenarioBlendingFactor = this.scenarioBlendingFactor;
				int num4 = 0;
				int num5 = 0;
				while (num5 < this.numberOfCellsBlendedPerFrame && num4 < this.m_LoadedBlendingCells.size)
				{
					ProbeReferenceVolume.Cell cell5 = *this.m_LoadedBlendingCells[num4++];
					if (!cell5.streamingInfo.IsBlendingStreaming() && !cell5.blendingInfo.IsUpToDate())
					{
						bool verboseStreamingLog2 = this.probeVolumeDebug.verboseStreamingLog;
						cell5.blendingInfo.blendingFactor = scenarioBlendingFactor;
						cell5.blendingInfo.MarkUpToDate();
						this.m_BlendingPool.BlendChunks(cell5, this.m_Pool);
						num5++;
					}
				}
				this.m_BlendingPool.PerformBlending(cmd, scenarioBlendingFactor, this.m_Pool);
			}
		}

		private static int DefragComparer(ProbeReferenceVolume.Cell a, ProbeReferenceVolume.Cell b)
		{
			if (a.indexInfo.updateInfo.GetNumberOfChunks() > b.indexInfo.updateInfo.GetNumberOfChunks())
			{
				return 1;
			}
			if (a.indexInfo.updateInfo.GetNumberOfChunks() < b.indexInfo.updateInfo.GetNumberOfChunks())
			{
				return -1;
			}
			return 0;
		}

		private void StartIndexDefragmentation()
		{
			if (!this.m_SupportGPUStreaming)
			{
				return;
			}
			this.m_IndexDefragmentationInProgress = true;
			this.m_IndexDefragCells.Clear();
			this.m_IndexDefragCells.AddRange(this.m_LoadedCells);
			this.m_IndexDefragCells.QuickSort(ProbeReferenceVolume.s_DefragComparer);
			this.m_DefragIndex.Clear();
		}

		private unsafe void UpdateIndexDefragmentation()
		{
			using (new ProfilingScope(ProfilingSampler.Get<CoreProfileId>(CoreProfileId.APVIndexDefragUpdate)))
			{
				this.m_TempIndexDefragCells.Clear();
				int num = Mathf.Min(this.m_IndexDefragCells.size, this.numberOfCellsLoadedPerFrame);
				int num2 = 0;
				int num3 = 0;
				while (num2 < this.m_IndexDefragCells.size && num3 < num)
				{
					ProbeReferenceVolume.Cell cell = *this.m_IndexDefragCells[this.m_IndexDefragCells.size - num2 - 1];
					this.m_DefragIndex.FindSlotsForEntries(ref cell.indexInfo.updateInfo.entriesInfo);
					this.m_DefragIndex.ReserveChunks(cell.indexInfo.updateInfo.entriesInfo, false);
					if (!cell.streamingInfo.IsStreaming() && !cell.streamingInfo.IsBlendingStreaming())
					{
						this.m_DefragIndex.AddBricks(cell.indexInfo, cell.data.bricks, cell.poolInfo.chunkList, ProbeBrickPool.GetChunkSizeInBrickCount(), this.m_Pool.GetPoolWidth(), this.m_Pool.GetPoolHeight());
						this.m_DefragCellIndices.UpdateCell(cell.indexInfo);
						num3++;
					}
					else
					{
						this.m_TempIndexDefragCells.Add(cell);
					}
					num2++;
				}
				this.m_IndexDefragCells.Resize(this.m_IndexDefragCells.size - num2, false);
				this.m_IndexDefragCells.AddRange(this.m_TempIndexDefragCells);
				if (this.m_IndexDefragCells.size == 0)
				{
					ProbeBrickIndex defragIndex = this.m_DefragIndex;
					this.m_DefragIndex = this.m_Index;
					this.m_Index = defragIndex;
					ProbeGlobalIndirection defragCellIndices = this.m_DefragCellIndices;
					this.m_DefragCellIndices = this.m_CellIndices;
					this.m_CellIndices = defragCellIndices;
					this.m_IndexDefragmentationInProgress = false;
				}
			}
		}

		private void OnStreamingComplete(ProbeReferenceVolume.CellStreamingRequest request, CommandBuffer cmd)
		{
			request.cell.streamingInfo.request = null;
			this.UpdatePoolAndIndex(request.cell, request.scratchBuffer, request.scratchBufferLayout, request.poolIndex, cmd);
		}

		private void OnBlendingStreamingComplete(ProbeReferenceVolume.CellStreamingRequest request, CommandBuffer cmd)
		{
			this.UpdatePool(cmd, request.cell.blendingInfo.chunkList, request.scratchBuffer, request.scratchBufferLayout, request.poolIndex);
			if (request.poolIndex == 0)
			{
				request.cell.streamingInfo.blendingRequest0 = null;
			}
			else
			{
				request.cell.streamingInfo.blendingRequest1 = null;
			}
			if (request.cell.streamingInfo.blendingRequest0 == null && request.cell.streamingInfo.blendingRequest1 == null && !request.cell.indexInfo.indexUpdated)
			{
				this.UpdateCellIndex(request.cell);
			}
		}

		private void PushDiskStreamingRequest(ProbeReferenceVolume.Cell cell, string scenario, int poolIndex, ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate onStreamingComplete)
		{
			ProbeReferenceVolume.CellStreamingRequest cellStreamingRequest = this.m_StreamingRequestsPool.Get();
			cellStreamingRequest.cell = cell;
			cellStreamingRequest.state = ProbeReferenceVolume.CellStreamingRequest.State.Pending;
			cellStreamingRequest.scenarioData = this.m_CurrentBakingSet.scenarios[scenario];
			cellStreamingRequest.poolIndex = poolIndex;
			cellStreamingRequest.onStreamingComplete = onStreamingComplete;
			if (poolIndex == -1 || poolIndex == 0)
			{
				cellStreamingRequest.streamSharedData = true;
			}
			if (this.probeVolumeDebug.verboseStreamingLog)
			{
			}
			switch (poolIndex)
			{
			case -1:
				cell.streamingInfo.request = cellStreamingRequest;
				break;
			case 0:
				cell.streamingInfo.blendingRequest0 = cellStreamingRequest;
				break;
			case 1:
				cell.streamingInfo.blendingRequest1 = cellStreamingRequest;
				break;
			}
			this.m_StreamingQueue.Enqueue(cellStreamingRequest);
		}

		private void CancelStreamingRequest(ProbeReferenceVolume.Cell cell)
		{
			this.m_Index.RemoveBricks(cell.indexInfo);
			this.m_Pool.Deallocate(cell.poolInfo.chunkList);
			if (cell.streamingInfo.request != null)
			{
				cell.streamingInfo.request.Cancel();
			}
		}

		private void CancelBlendingStreamingRequest(ProbeReferenceVolume.Cell cell)
		{
			if (cell.streamingInfo.blendingRequest0 != null)
			{
				cell.streamingInfo.blendingRequest0.Cancel();
			}
			if (cell.streamingInfo.blendingRequest1 != null)
			{
				cell.streamingInfo.blendingRequest1.Cancel();
			}
		}

		private unsafe bool ProcessDiskStreamingRequest(ProbeReferenceVolume.CellStreamingRequest request)
		{
			int index = request.cell.desc.index;
			ProbeReferenceVolume.Cell cell = this.cells[index];
			ProbeReferenceVolume.CellDesc desc = cell.desc;
			ProbeReferenceVolume.CellData data = cell.data;
			ProbeReferenceVolume.CellStreamingScratchBuffer scratchBuffer;
			ProbeReferenceVolume.CellStreamingScratchBufferLayout scratchBufferLayout;
			if (!this.m_ScratchBufferPool.AllocateScratchBuffer(desc.shChunkCount, out scratchBuffer, out scratchBufferLayout, this.m_DiskStreamingUseCompute))
			{
				return false;
			}
			if (!this.m_CurrentBakingSet.HasValidSharedData())
			{
				Debug.LogError("One or more data file missing for baking set " + this.m_CurrentBakingSet.name + ". Cannot load shared data.");
				return false;
			}
			if (!request.scenarioData.HasValidData(this.m_SHBands))
			{
				Debug.LogError(string.Concat(new string[]
				{
					"One or more data file missing for baking set ",
					this.m_CurrentBakingSet.name,
					" scenario ",
					this.lightingScenario,
					". Cannot load scenario data."
				}));
				return false;
			}
			if (this.probeVolumeDebug.verboseStreamingLog)
			{
				int poolIndex = request.poolIndex;
			}
			request.scratchBuffer = scratchBuffer;
			request.scratchBufferLayout = scratchBufferLayout;
			request.bytesWritten = 0;
			byte* unsafePtr = (byte*)request.scratchBuffer.stagingBuffer.GetUnsafePtr<byte>();
			byte* ptr = unsafePtr;
			uint* ptr2 = (uint*)ptr;
			List<ProbeBrickPool.BrickChunkAlloc> list = (request.poolIndex == -1) ? request.cell.poolInfo.chunkList : request.cell.blendingInfo.chunkList;
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc = list[i];
				ptr2[i * 4] = (uint)brickChunkAlloc.x;
				ptr2[i * 4 + 1] = (uint)brickChunkAlloc.y;
				ptr2[i * 4 + 2] = (uint)brickChunkAlloc.z;
				ptr2[i * 4 + 3] = 0U;
			}
			ptr += count * 4 * 4;
			ptr2 = (uint*)ptr;
			list = request.cell.poolInfo.chunkList;
			for (int j = 0; j < count; j++)
			{
				ProbeBrickPool.BrickChunkAlloc brickChunkAlloc2 = list[j];
				ptr2[j * 4] = (uint)brickChunkAlloc2.x;
				ptr2[j * 4 + 1] = (uint)brickChunkAlloc2.y;
				ptr2[j * 4 + 2] = (uint)brickChunkAlloc2.z;
				ptr2[j * 4 + 3] = 0U;
			}
			ptr += count * 4 * 4;
			ProbeVolumeStreamableAsset cellDataAsset = request.scenarioData.cellDataAsset;
			ProbeVolumeStreamableAsset.StreamableCellDesc streamableCellDesc = cellDataAsset.streamableCellDescs[index];
			int shChunkCount = desc.shChunkCount;
			int num = this.m_CurrentBakingSet.L0ChunkSize * shChunkCount;
			int num2 = this.m_CurrentBakingSet.L1ChunkSize * shChunkCount;
			int num3 = num + 2 * num2;
			request.cellDataStreamingRequest.AddReadCommand(streamableCellDesc.offset, num3, ptr);
			ptr += num3;
			request.bytesWritten += request.cellDataStreamingRequest.RunCommands(cellDataAsset.OpenFile());
			if (request.streamSharedData)
			{
				ProbeVolumeStreamableAsset cellSharedDataAsset = this.m_CurrentBakingSet.cellSharedDataAsset;
				streamableCellDesc = cellSharedDataAsset.streamableCellDescs[index];
				int sharedDataChunkSize = this.m_CurrentBakingSet.sharedDataChunkSize;
				request.cellSharedDataStreamingRequest.AddReadCommand(streamableCellDesc.offset, sharedDataChunkSize * shChunkCount, ptr);
				ptr += sharedDataChunkSize * shChunkCount;
				request.bytesWritten += request.cellSharedDataStreamingRequest.RunCommands(cellSharedDataAsset.OpenFile());
			}
			if (this.m_SHBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
			{
				ProbeVolumeStreamableAsset cellOptionalDataAsset = request.scenarioData.cellOptionalDataAsset;
				streamableCellDesc = cellOptionalDataAsset.streamableCellDescs[index];
				int num4 = this.m_CurrentBakingSet.L2TextureChunkSize * shChunkCount * 4;
				request.cellOptionalDataStreamingRequest.AddReadCommand(streamableCellDesc.offset, num4, ptr);
				ptr += num4;
				request.bytesWritten += request.cellOptionalDataStreamingRequest.RunCommands(cellOptionalDataAsset.OpenFile());
			}
			if (this.m_CurrentBakingSet.bakedProbeOcclusion)
			{
				ProbeVolumeStreamableAsset cellProbeOcclusionDataAsset = request.scenarioData.cellProbeOcclusionDataAsset;
				streamableCellDesc = cellProbeOcclusionDataAsset.streamableCellDescs[index];
				int num5 = this.m_CurrentBakingSet.ProbeOcclusionChunkSize * shChunkCount;
				request.cellProbeOcclusionDataStreamingRequest.AddReadCommand(streamableCellDesc.offset, num5, ptr);
				ptr += num5;
				request.bytesWritten += request.cellProbeOcclusionDataStreamingRequest.RunCommands(cellProbeOcclusionDataAsset.OpenFile());
			}
			data.bricks = new NativeArray<ProbeBrickIndex.Brick>(desc.bricksCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ProbeVolumeStreamableAsset cellBricksDataAsset = this.m_CurrentBakingSet.cellBricksDataAsset;
			streamableCellDesc = cellBricksDataAsset.streamableCellDescs[index];
			request.brickStreamingRequest.AddReadCommand(streamableCellDesc.offset, cellBricksDataAsset.elementSize * Mathf.Min(streamableCellDesc.elementCount, desc.bricksCount), (byte*)data.bricks.GetUnsafePtr<ProbeBrickIndex.Brick>());
			request.brickStreamingRequest.RunCommands(cellBricksDataAsset.OpenFile());
			if (this.m_CurrentBakingSet.HasSupportData())
			{
				ProbeVolumeStreamableAsset cellSupportDataAsset = this.m_CurrentBakingSet.cellSupportDataAsset;
				streamableCellDesc = cellSupportDataAsset.streamableCellDescs[index];
				int num6 = streamableCellDesc.offset;
				int num7 = streamableCellDesc.elementCount * this.m_CurrentBakingSet.supportPositionChunkSize;
				int num8 = streamableCellDesc.elementCount * this.m_CurrentBakingSet.supportTouchupChunkSize;
				int num9 = streamableCellDesc.elementCount * this.m_CurrentBakingSet.supportOffsetsChunkSize;
				int num10 = streamableCellDesc.elementCount * this.m_CurrentBakingSet.supportLayerMaskChunkSize;
				int num11 = streamableCellDesc.elementCount * this.m_CurrentBakingSet.supportValidityChunkSize;
				data.probePositions = new NativeArray<byte>(num7, Allocator.Persistent, NativeArrayOptions.UninitializedMemory).Reinterpret<Vector3>(1);
				data.validity = new NativeArray<byte>(num11, Allocator.Persistent, NativeArrayOptions.UninitializedMemory).Reinterpret<float>(1);
				data.layer = new NativeArray<byte>(num10, Allocator.Persistent, NativeArrayOptions.UninitializedMemory).Reinterpret<byte>(1);
				data.touchupVolumeInteraction = new NativeArray<byte>(num8, Allocator.Persistent, NativeArrayOptions.UninitializedMemory).Reinterpret<float>(1);
				data.offsetVectors = new NativeArray<byte>(num9, Allocator.Persistent, NativeArrayOptions.UninitializedMemory).Reinterpret<Vector3>(1);
				request.supportStreamingRequest.AddReadCommand(num6, num7, (byte*)data.probePositions.GetUnsafePtr<Vector3>());
				num6 += num7;
				request.supportStreamingRequest.AddReadCommand(num6, num11, (byte*)data.validity.GetUnsafePtr<float>());
				num6 += num11;
				request.supportStreamingRequest.AddReadCommand(num6, num8, (byte*)data.touchupVolumeInteraction.GetUnsafePtr<float>());
				num6 += num8;
				request.supportStreamingRequest.AddReadCommand(num6, num10, (byte*)data.layer.GetUnsafePtr<byte>());
				num6 += num10;
				request.supportStreamingRequest.AddReadCommand(num6, num9, (byte*)data.offsetVectors.GetUnsafePtr<Vector3>());
				request.supportStreamingRequest.RunCommands(cellSupportDataAsset.OpenFile());
			}
			request.state = ProbeReferenceVolume.CellStreamingRequest.State.Active;
			this.m_ActiveStreamingRequests.Add(request);
			return true;
		}

		private void AllocateScratchBufferPoolIfNeeded()
		{
			if (this.m_SupportDiskStreaming)
			{
				int chunkGPUMemory = this.m_CurrentBakingSet.GetChunkGPUMemory(this.m_SHBands);
				int maxSHChunkCount = this.m_CurrentBakingSet.maxSHChunkCount;
				if (this.m_ScratchBufferPool == null || this.m_ScratchBufferPool.chunkSize != chunkGPUMemory || this.m_ScratchBufferPool.maxChunkCount != maxSHChunkCount)
				{
					bool verboseStreamingLog = this.probeVolumeDebug.verboseStreamingLog;
					if (this.m_ScratchBufferPool != null)
					{
						this.m_ScratchBufferPool.Cleanup();
					}
					this.m_ScratchBufferPool = new ProbeVolumeScratchBufferPool(this.m_CurrentBakingSet, this.m_SHBands);
				}
			}
		}

		private void UpdateActiveRequests(CommandBuffer cmd)
		{
			if (this.m_ActiveStreamingRequests.Count > 0)
			{
				for (int i = this.m_ActiveStreamingRequests.Count - 1; i >= 0; i--)
				{
					ProbeReferenceVolume.CellStreamingRequest cellStreamingRequest = this.m_ActiveStreamingRequests[i];
					bool flag = false;
					if (cellStreamingRequest.state == ProbeReferenceVolume.CellStreamingRequest.State.Canceled)
					{
						bool verboseStreamingLog = this.probeVolumeDebug.verboseStreamingLog;
						this.m_ScratchBufferPool.ReleaseScratchBuffer(cellStreamingRequest.scratchBuffer);
						flag = true;
					}
					else
					{
						cellStreamingRequest.UpdateState();
						if (cellStreamingRequest.state == ProbeReferenceVolume.CellStreamingRequest.State.Complete)
						{
							if (this.probeVolumeDebug.verboseStreamingLog)
							{
								int poolIndex = cellStreamingRequest.poolIndex;
							}
							if (cellStreamingRequest.scratchBuffer.buffer != null)
							{
								cellStreamingRequest.scratchBuffer.buffer.LockBufferForWrite<byte>(0, cellStreamingRequest.scratchBuffer.stagingBuffer.Length).CopyFrom(cellStreamingRequest.scratchBuffer.stagingBuffer);
								cellStreamingRequest.scratchBuffer.buffer.UnlockBufferAfterWrite<byte>(cellStreamingRequest.scratchBuffer.stagingBuffer.Length);
							}
							cellStreamingRequest.onStreamingComplete(cellStreamingRequest, cmd);
							this.m_ScratchBufferPool.ReleaseScratchBuffer(cellStreamingRequest.scratchBuffer);
							flag = true;
						}
						else if (cellStreamingRequest.state == ProbeReferenceVolume.CellStreamingRequest.State.Invalid)
						{
							bool verboseStreamingLog2 = this.probeVolumeDebug.verboseStreamingLog;
							this.m_ScratchBufferPool.ReleaseScratchBuffer(cellStreamingRequest.scratchBuffer);
							cellStreamingRequest.Reset();
							this.m_ActiveStreamingRequests.RemoveAt(i);
							this.m_StreamingQueue.Enqueue(cellStreamingRequest);
						}
					}
					if (flag)
					{
						this.m_ActiveStreamingRequests.RemoveAt(i);
						this.m_StreamingRequestsPool.Release(cellStreamingRequest);
					}
				}
			}
		}

		private void ProcessNewRequests()
		{
			ProbeReferenceVolume.CellStreamingRequest cellStreamingRequest;
			while (this.m_StreamingQueue.TryPeek(out cellStreamingRequest))
			{
				if (cellStreamingRequest.state == ProbeReferenceVolume.CellStreamingRequest.State.Canceled)
				{
					if (this.probeVolumeDebug.verboseStreamingLog)
					{
						int poolIndex = cellStreamingRequest.poolIndex;
					}
					this.m_StreamingRequestsPool.Release(cellStreamingRequest);
					this.m_StreamingQueue.Dequeue();
				}
				else
				{
					if (!this.ProcessDiskStreamingRequest(cellStreamingRequest))
					{
						break;
					}
					this.m_StreamingQueue.Dequeue();
				}
			}
		}

		private void UpdateDiskStreaming(CommandBuffer cmd)
		{
			if (!this.diskStreamingEnabled)
			{
				return;
			}
			using (new ProfilingScope(ProfilingSampler.Get<CoreProfileId>(CoreProfileId.APVDiskStreamingUpdate)))
			{
				this.AllocateScratchBufferPoolIfNeeded();
				this.ProcessNewRequests();
				this.UpdateActiveRequests(cmd);
				if (this.m_ActiveStreamingRequests.Count == 0 && this.m_StreamingQueue.Count == 0 && this.m_CurrentBakingSet.cellBricksDataAsset != null && this.m_CurrentBakingSet.cellBricksDataAsset.IsOpen())
				{
					bool verboseStreamingLog = this.probeVolumeDebug.verboseStreamingLog;
					this.m_CurrentBakingSet.cellBricksDataAsset.CloseFile();
					this.m_CurrentBakingSet.cellSupportDataAsset.CloseFile();
					this.m_CurrentBakingSet.cellSharedDataAsset.CloseFile();
					ProbeVolumeBakingSet.PerScenarioDataInfo perScenarioDataInfo;
					if (this.m_CurrentBakingSet.scenarios.TryGetValue(this.lightingScenario, out perScenarioDataInfo))
					{
						perScenarioDataInfo.cellDataAsset.CloseFile();
						perScenarioDataInfo.cellOptionalDataAsset.CloseFile();
						perScenarioDataInfo.cellProbeOcclusionDataAsset.CloseFile();
					}
					ProbeVolumeBakingSet.PerScenarioDataInfo perScenarioDataInfo2;
					if (!string.IsNullOrEmpty(this.otherScenario) && this.m_CurrentBakingSet.scenarios.TryGetValue(this.lightingScenario, out perScenarioDataInfo2))
					{
						perScenarioDataInfo2.cellDataAsset.CloseFile();
						perScenarioDataInfo2.cellOptionalDataAsset.CloseFile();
						perScenarioDataInfo2.cellProbeOcclusionDataAsset.CloseFile();
					}
				}
			}
			if (this.probeVolumeDebug.debugStreaming && this.m_ToBeLoadedCells.size == 0 && this.m_ActiveStreamingRequests.Count == 0)
			{
				this.UnloadAllCells();
			}
		}

		private bool HasActiveStreamingRequest(ProbeReferenceVolume.Cell cell)
		{
			return this.diskStreamingEnabled && this.m_ActiveStreamingRequests.Exists((ProbeReferenceVolume.CellStreamingRequest x) => x.cell == cell);
		}

		[Conditional("UNITY_EDITOR")]
		[Conditional("DEVELOPMENT_BUILD")]
		private void LogStreaming(string log)
		{
			Debug.Log(log);
		}

		[CompilerGenerated]
		private void <RegisterDebug>g__RefreshDebug|229_0<T>(DebugUI.Field<T> field, T value)
		{
			this.UnregisterDebug(false);
			this.RegisterDebug();
		}

		[CompilerGenerated]
		private void <RegisterDebug>g__RefreshScenarioNames|229_75(string guid)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (ProbeVolumeBakingSet probeVolumeBakingSet in Resources.FindObjectsOfTypeAll<ProbeVolumeBakingSet>())
			{
				if (probeVolumeBakingSet.sceneGUIDs.Contains(guid))
				{
					foreach (string item in probeVolumeBakingSet.lightingScenarios)
					{
						hashSet.Add(item);
					}
				}
			}
			hashSet.Remove(this.m_CurrentBakingSet.lightingScenario);
			if (this.m_DebugActiveSceneGUID == guid && hashSet.Count + 1 == this.m_DebugScenarioNames.Length && this.m_DebugActiveScenario == this.m_CurrentBakingSet.lightingScenario)
			{
				return;
			}
			int num = 0;
			ArrayExtensions.ResizeArray<GUIContent>(ref this.m_DebugScenarioNames, hashSet.Count + 1);
			ArrayExtensions.ResizeArray<int>(ref this.m_DebugScenarioValues, hashSet.Count + 1);
			this.m_DebugScenarioNames[0] = new GUIContent("None");
			this.m_DebugScenarioValues[0] = 0;
			foreach (string text in hashSet)
			{
				num++;
				this.m_DebugScenarioNames[num] = new GUIContent(text);
				this.m_DebugScenarioValues[num] = num;
			}
			this.m_DebugActiveSceneGUID = guid;
			this.m_DebugActiveScenario = this.m_CurrentBakingSet.lightingScenario;
			this.m_DebugScenarioField.enumNames = this.m_DebugScenarioNames;
			this.m_DebugScenarioField.enumValues = this.m_DebugScenarioValues;
			if (this.probeVolumeDebug.otherStateIndex >= this.m_DebugScenarioNames.Length)
			{
				this.probeVolumeDebug.otherStateIndex = 0;
			}
		}

		private ComputeBuffer m_EmptyIndexBuffer;

		private bool m_IsInitialized;

		private bool m_SupportScenarios;

		private bool m_SupportScenarioBlending;

		private bool m_ForceNoDiskStreaming;

		private bool m_SupportDiskStreaming;

		private bool m_SupportGPUStreaming;

		private bool m_UseStreamingAssets = true;

		private float m_MinBrickSize;

		private int m_MaxSubdivision;

		private Vector3 m_ProbeOffset;

		private ProbeBrickPool m_Pool;

		private ProbeBrickIndex m_Index;

		private ProbeGlobalIndirection m_CellIndices;

		private ProbeBrickBlendingPool m_BlendingPool;

		private List<ProbeBrickPool.BrickChunkAlloc> m_TmpSrcChunks = new List<ProbeBrickPool.BrickChunkAlloc>();

		private float[] m_PositionOffsets = new float[4];

		private Bounds m_CurrGlobalBounds;

		internal Dictionary<int, ProbeReferenceVolume.Cell> cells = new Dictionary<int, ProbeReferenceVolume.Cell>();

		private ObjectPool<ProbeReferenceVolume.Cell> m_CellPool = new ObjectPool<ProbeReferenceVolume.Cell>(delegate(ProbeReferenceVolume.Cell x)
		{
			x.Clear();
		}, null, false);

		private ProbeBrickPool.DataLocation m_TemporaryDataLocation;

		private int m_TemporaryDataLocationMemCost;

		[Obsolete("This field is only kept for migration purpose.")]
		internal ProbeVolumeSceneData sceneData;

		private Vector3Int minLoadedCellPos = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

		private Vector3Int maxLoadedCellPos = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

		public Action<ProbeReferenceVolume.ExtraDataActionInput> retrieveExtraDataAction;

		public Action checksDuringBakeAction;

		private Dictionary<string, ValueTuple<ProbeVolumeBakingSet, List<int>>> m_PendingScenesToBeLoaded = new Dictionary<string, ValueTuple<ProbeVolumeBakingSet, List<int>>>();

		private Dictionary<string, List<int>> m_PendingScenesToBeUnloaded = new Dictionary<string, List<int>>();

		private List<string> m_ActiveScenes = new List<string>();

		private ProbeVolumeBakingSetWeakReference m_CurrentBakingSetReference = new ProbeVolumeBakingSetWeakReference();

		private ProbeVolumeBakingSetWeakReference m_LazyBakingSetReference = new ProbeVolumeBakingSetWeakReference();

		private bool m_NeedLoadAsset;

		private bool m_ProbeReferenceVolumeInit;

		private bool m_EnabledBySRP;

		private bool m_VertexSampling;

		private bool m_NeedsIndexRebuild;

		private bool m_HasChangedIndex;

		private int m_CBShaderID = Shader.PropertyToID("ShaderVariablesProbeVolumes");

		private ProbeVolumeTextureMemoryBudget m_MemoryBudget;

		private ProbeVolumeBlendingTextureMemoryBudget m_BlendingMemoryBudget;

		private ProbeVolumeSHBands m_SHBands;

		internal bool clearAssetsOnVolumeClear;

		internal static string defaultLightingScenario = "Default";

		private static ProbeReferenceVolume _instance = new ProbeReferenceVolume();

		private const int kProbesPerBatch = 511;

		public static readonly string k_DebugPanelName = "Probe Volumes";

		private Mesh m_DebugMesh;

		private DebugUI.Widget[] m_DebugItems;

		private Material m_DebugMaterial;

		private Mesh m_DebugProbeSamplingMesh;

		private Material m_ProbeSamplingDebugMaterial;

		private Material m_ProbeSamplingDebugMaterial02;

		private Texture m_DisplayNumbersTexture;

		internal static ProbeSamplingDebugData probeSamplingDebugData = new ProbeSamplingDebugData();

		private Mesh m_DebugOffsetMesh;

		private Material m_DebugOffsetMaterial;

		private Material m_DebugFragmentationMaterial;

		private Plane[] m_DebugFrustumPlanes = new Plane[6];

		private GUIContent[] m_DebugScenarioNames = new GUIContent[0];

		private int[] m_DebugScenarioValues = new int[0];

		private string m_DebugActiveSceneGUID;

		private string m_DebugActiveScenario;

		private DebugUI.EnumField m_DebugScenarioField;

		internal Dictionary<Bounds, ProbeBrickIndex.Brick[]> realtimeSubdivisionInfo = new Dictionary<Bounds, ProbeBrickIndex.Brick[]>();

		private bool m_MaxSubdivVisualizedIsMaxAvailable;

		private static Vector4[] s_BoundsArray = new Vector4[48];

		private bool m_LoadMaxCellsPerFrame;

		private const int kMaxCellLoadedPerFrame = 10;

		private int m_NumberOfCellsLoadedPerFrame = 1;

		private int m_NumberOfCellsBlendedPerFrame = 10000;

		private float m_TurnoverRate = 0.1f;

		private DynamicArray<ProbeReferenceVolume.Cell> m_LoadedCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_ToBeLoadedCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_WorseLoadedCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_BestToBeLoadedCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_TempCellToLoadList = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_TempCellToUnloadList = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_LoadedBlendingCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_ToBeLoadedBlendingCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_TempBlendingCellToLoadList = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_TempBlendingCellToUnloadList = new DynamicArray<ProbeReferenceVolume.Cell>();

		private Vector3 m_FrozenCameraPosition;

		private Vector3 m_FrozenCameraDirection;

		private const float kIndexFragmentationThreshold = 0.2f;

		private bool m_IndexDefragmentationInProgress;

		private ProbeBrickIndex m_DefragIndex;

		private ProbeGlobalIndirection m_DefragCellIndices;

		private DynamicArray<ProbeReferenceVolume.Cell> m_IndexDefragCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		private DynamicArray<ProbeReferenceVolume.Cell> m_TempIndexDefragCells = new DynamicArray<ProbeReferenceVolume.Cell>();

		internal float minStreamingScore;

		internal float maxStreamingScore;

		private Queue<ProbeReferenceVolume.CellStreamingRequest> m_StreamingQueue = new Queue<ProbeReferenceVolume.CellStreamingRequest>();

		private List<ProbeReferenceVolume.CellStreamingRequest> m_ActiveStreamingRequests = new List<ProbeReferenceVolume.CellStreamingRequest>();

		private ObjectPool<ProbeReferenceVolume.CellStreamingRequest> m_StreamingRequestsPool = new ObjectPool<ProbeReferenceVolume.CellStreamingRequest>(null, delegate(ProbeReferenceVolume.CellStreamingRequest val)
		{
			val.Clear();
		}, true);

		private bool m_DiskStreamingUseCompute;

		private ProbeVolumeScratchBufferPool m_ScratchBufferPool;

		private ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate m_OnStreamingComplete;

		private ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate m_OnBlendingStreamingComplete;

		private static DynamicArray<ProbeReferenceVolume.Cell>.SortComparer s_BlendingComparer = new DynamicArray<ProbeReferenceVolume.Cell>.SortComparer(ProbeReferenceVolume.BlendingComparer);

		private static DynamicArray<ProbeReferenceVolume.Cell>.SortComparer s_DefragComparer = new DynamicArray<ProbeReferenceVolume.Cell>.SortComparer(ProbeReferenceVolume.DefragComparer);

		internal static class ShaderIDs
		{
			public static readonly int _APVResIndex = Shader.PropertyToID("_APVResIndex");

			public static readonly int _APVResCellIndices = Shader.PropertyToID("_APVResCellIndices");

			public static readonly int _APVResL0_L1Rx = Shader.PropertyToID("_APVResL0_L1Rx");

			public static readonly int _APVResL1G_L1Ry = Shader.PropertyToID("_APVResL1G_L1Ry");

			public static readonly int _APVResL1B_L1Rz = Shader.PropertyToID("_APVResL1B_L1Rz");

			public static readonly int _APVResL2_0 = Shader.PropertyToID("_APVResL2_0");

			public static readonly int _APVResL2_1 = Shader.PropertyToID("_APVResL2_1");

			public static readonly int _APVResL2_2 = Shader.PropertyToID("_APVResL2_2");

			public static readonly int _APVResL2_3 = Shader.PropertyToID("_APVResL2_3");

			public static readonly int _APVProbeOcclusion = Shader.PropertyToID("_APVProbeOcclusion");

			public static readonly int _APVResValidity = Shader.PropertyToID("_APVResValidity");

			public static readonly int _SkyOcclusionTexL0L1 = Shader.PropertyToID("_SkyOcclusionTexL0L1");

			public static readonly int _SkyShadingDirectionIndicesTex = Shader.PropertyToID("_SkyShadingDirectionIndicesTex");

			public static readonly int _SkyPrecomputedDirections = Shader.PropertyToID("_SkyPrecomputedDirections");

			public static readonly int _AntiLeakData = Shader.PropertyToID("_AntiLeakData");
		}

		[Serializable]
		internal struct IndirectionEntryInfo
		{
			public Vector3Int positionInBricks;

			public int minSubdiv;

			public Vector3Int minBrickPos;

			public Vector3Int maxBrickPosPlusOne;

			public bool hasMinMax;

			public bool hasOnlyBiggerBricks;
		}

		[Serializable]
		internal class CellDesc
		{
			public override string ToString()
			{
				return string.Format("Index = {0} position = {1}", this.index, this.position);
			}

			public Vector3Int position;

			public int index;

			public int probeCount;

			public int minSubdiv;

			public int indexChunkCount;

			public int shChunkCount;

			public int bricksCount;

			public ProbeReferenceVolume.IndirectionEntryInfo[] indirectionEntryInfo;
		}

		internal class CellData
		{
			public NativeArray<ushort> skyOcclusionDataL0L1 { get; internal set; }

			public NativeArray<byte> skyShadingDirectionIndices { get; internal set; }

			public NativeArray<ProbeBrickIndex.Brick> bricks { get; internal set; }

			public NativeArray<Vector3> probePositions { get; internal set; }

			public NativeArray<float> touchupVolumeInteraction { get; internal set; }

			public NativeArray<Vector3> offsetVectors { get; internal set; }

			public NativeArray<float> validity { get; internal set; }

			public NativeArray<byte> layer { get; internal set; }

			public void CleanupPerScenarioData(in ProbeReferenceVolume.CellData.PerScenarioData data)
			{
				NativeArray<ushort> shL0L1RxData = data.shL0L1RxData;
				NativeArray<byte> nativeArray;
				if (shL0L1RxData.IsCreated)
				{
					shL0L1RxData = data.shL0L1RxData;
					shL0L1RxData.Dispose();
					nativeArray = data.shL1GL1RyData;
					nativeArray.Dispose();
					nativeArray = data.shL1BL1RzData;
					nativeArray.Dispose();
				}
				nativeArray = data.shL2Data_0;
				if (nativeArray.IsCreated)
				{
					nativeArray = data.shL2Data_0;
					nativeArray.Dispose();
					nativeArray = data.shL2Data_1;
					nativeArray.Dispose();
					nativeArray = data.shL2Data_2;
					nativeArray.Dispose();
					nativeArray = data.shL2Data_3;
					nativeArray.Dispose();
				}
				nativeArray = data.probeOcclusion;
				if (nativeArray.IsCreated)
				{
					nativeArray = data.probeOcclusion;
					nativeArray.Dispose();
				}
			}

			public void Cleanup(bool cleanScenarioList)
			{
				if (this.validityNeighMaskData.IsCreated)
				{
					this.validityNeighMaskData.Dispose();
					this.validityNeighMaskData = default(NativeArray<byte>);
					foreach (ProbeReferenceVolume.CellData.PerScenarioData perScenarioData in this.scenarios.Values)
					{
						this.CleanupPerScenarioData(perScenarioData);
					}
				}
				if (cleanScenarioList)
				{
					this.scenarios.Clear();
				}
				if (this.bricks.IsCreated)
				{
					this.bricks.Dispose();
					this.bricks = default(NativeArray<ProbeBrickIndex.Brick>);
				}
				if (this.skyOcclusionDataL0L1.IsCreated)
				{
					this.skyOcclusionDataL0L1.Dispose();
					this.skyOcclusionDataL0L1 = default(NativeArray<ushort>);
				}
				if (this.skyShadingDirectionIndices.IsCreated)
				{
					this.skyShadingDirectionIndices.Dispose();
					this.skyShadingDirectionIndices = default(NativeArray<byte>);
				}
				if (this.probePositions.IsCreated)
				{
					this.probePositions.Dispose();
					this.probePositions = default(NativeArray<Vector3>);
				}
				if (this.touchupVolumeInteraction.IsCreated)
				{
					this.touchupVolumeInteraction.Dispose();
					this.touchupVolumeInteraction = default(NativeArray<float>);
				}
				if (this.validity.IsCreated)
				{
					this.validity.Dispose();
					this.validity = default(NativeArray<float>);
				}
				if (this.layer.IsCreated)
				{
					this.layer.Dispose();
					this.layer = default(NativeArray<byte>);
				}
				if (this.offsetVectors.IsCreated)
				{
					this.offsetVectors.Dispose();
					this.offsetVectors = default(NativeArray<Vector3>);
				}
			}

			public NativeArray<byte> validityNeighMaskData;

			public Dictionary<string, ProbeReferenceVolume.CellData.PerScenarioData> scenarios = new Dictionary<string, ProbeReferenceVolume.CellData.PerScenarioData>();

			public struct PerScenarioData
			{
				public NativeArray<ushort> shL0L1RxData;

				public NativeArray<byte> shL1GL1RyData;

				public NativeArray<byte> shL1BL1RzData;

				public NativeArray<byte> shL2Data_0;

				public NativeArray<byte> shL2Data_1;

				public NativeArray<byte> shL2Data_2;

				public NativeArray<byte> shL2Data_3;

				public NativeArray<byte> probeOcclusion;
			}
		}

		internal class CellPoolInfo
		{
			public void Clear()
			{
				this.chunkList.Clear();
			}

			public List<ProbeBrickPool.BrickChunkAlloc> chunkList = new List<ProbeBrickPool.BrickChunkAlloc>();

			public int shChunkCount;
		}

		internal class CellIndexInfo
		{
			public void Clear()
			{
				this.flatIndicesInGlobalIndirection = null;
				this.updateInfo = default(ProbeBrickIndex.CellIndexUpdateInfo);
				this.indexUpdated = false;
				this.indirectionEntryInfo = null;
			}

			public int[] flatIndicesInGlobalIndirection;

			public ProbeBrickIndex.CellIndexUpdateInfo updateInfo;

			public bool indexUpdated;

			public ProbeReferenceVolume.IndirectionEntryInfo[] indirectionEntryInfo;

			public int indexChunkCount;
		}

		internal class CellBlendingInfo
		{
			public void MarkUpToDate()
			{
				this.blendingScore = float.MaxValue;
			}

			public bool IsUpToDate()
			{
				return this.blendingScore == float.MaxValue;
			}

			public void ForceReupload()
			{
				this.blendingFactor = -1f;
			}

			public bool ShouldReupload()
			{
				return this.blendingFactor == -1f;
			}

			public void Prioritize()
			{
				this.blendingFactor = -2f;
			}

			public bool ShouldPrioritize()
			{
				return this.blendingFactor == -2f;
			}

			public void Clear()
			{
				this.chunkList.Clear();
				this.blendingScore = 0f;
				this.blendingFactor = 0f;
				this.blending = false;
			}

			public List<ProbeBrickPool.BrickChunkAlloc> chunkList = new List<ProbeBrickPool.BrickChunkAlloc>();

			public float blendingScore;

			public float blendingFactor;

			public bool blending;
		}

		internal class CellStreamingInfo
		{
			public bool IsStreaming()
			{
				return this.request != null && this.request.IsStreaming();
			}

			public bool IsBlendingStreaming()
			{
				return (this.blendingRequest0 != null && this.blendingRequest0.IsStreaming()) || (this.blendingRequest1 != null && this.blendingRequest1.IsStreaming());
			}

			public void Clear()
			{
				this.request = null;
				this.blendingRequest0 = null;
				this.blendingRequest1 = null;
				this.streamingScore = 0f;
			}

			public ProbeReferenceVolume.CellStreamingRequest request;

			public ProbeReferenceVolume.CellStreamingRequest blendingRequest0;

			public ProbeReferenceVolume.CellStreamingRequest blendingRequest1;

			public float streamingScore;
		}

		[DebuggerDisplay("Index = {desc.index} Loaded = {loaded}")]
		internal class Cell : IComparable<ProbeReferenceVolume.Cell>
		{
			public int CompareTo(ProbeReferenceVolume.Cell other)
			{
				if (this.streamingInfo.streamingScore < other.streamingInfo.streamingScore)
				{
					return -1;
				}
				if (this.streamingInfo.streamingScore > other.streamingInfo.streamingScore)
				{
					return 1;
				}
				return 0;
			}

			public bool UpdateCellScenarioData(string scenario0, string scenario1)
			{
				if (!this.data.scenarios.TryGetValue(scenario0, out this.scenario0))
				{
					return false;
				}
				this.hasTwoScenarios = false;
				if (!string.IsNullOrEmpty(scenario1) && this.data.scenarios.TryGetValue(scenario1, out this.scenario1))
				{
					this.hasTwoScenarios = true;
				}
				return true;
			}

			public void Clear()
			{
				this.desc = null;
				this.data = null;
				this.poolInfo.Clear();
				this.indexInfo.Clear();
				this.blendingInfo.Clear();
				this.streamingInfo.Clear();
				this.referenceCount = 0;
				this.loaded = false;
				this.scenario0 = default(ProbeReferenceVolume.CellData.PerScenarioData);
				this.scenario1 = default(ProbeReferenceVolume.CellData.PerScenarioData);
				this.hasTwoScenarios = false;
				this.debugProbes = null;
			}

			public ProbeReferenceVolume.CellDesc desc;

			public ProbeReferenceVolume.CellData data;

			public ProbeReferenceVolume.CellPoolInfo poolInfo = new ProbeReferenceVolume.CellPoolInfo();

			public ProbeReferenceVolume.CellIndexInfo indexInfo = new ProbeReferenceVolume.CellIndexInfo();

			public ProbeReferenceVolume.CellBlendingInfo blendingInfo = new ProbeReferenceVolume.CellBlendingInfo();

			public ProbeReferenceVolume.CellStreamingInfo streamingInfo = new ProbeReferenceVolume.CellStreamingInfo();

			public int referenceCount;

			public bool loaded;

			public ProbeReferenceVolume.CellData.PerScenarioData scenario0;

			public ProbeReferenceVolume.CellData.PerScenarioData scenario1;

			public bool hasTwoScenarios;

			public ProbeReferenceVolume.CellInstancedDebugProbes debugProbes;
		}

		internal struct Volume : IEquatable<ProbeReferenceVolume.Volume>
		{
			public Volume(Matrix4x4 trs, float maxSubdivision, float minSubdivision)
			{
				this.X = trs.GetColumn(0);
				this.Y = trs.GetColumn(1);
				this.Z = trs.GetColumn(2);
				this.corner = trs.GetColumn(3) - this.X * 0.5f - this.Y * 0.5f - this.Z * 0.5f;
				this.maxSubdivisionMultiplier = maxSubdivision;
				this.minSubdivisionMultiplier = minSubdivision;
			}

			public Volume(Vector3 corner, Vector3 X, Vector3 Y, Vector3 Z, float maxSubdivision = 1f, float minSubdivision = 0f)
			{
				this.corner = corner;
				this.X = X;
				this.Y = Y;
				this.Z = Z;
				this.maxSubdivisionMultiplier = maxSubdivision;
				this.minSubdivisionMultiplier = minSubdivision;
			}

			public Volume(ProbeReferenceVolume.Volume copy)
			{
				this.X = copy.X;
				this.Y = copy.Y;
				this.Z = copy.Z;
				this.corner = copy.corner;
				this.maxSubdivisionMultiplier = copy.maxSubdivisionMultiplier;
				this.minSubdivisionMultiplier = copy.minSubdivisionMultiplier;
			}

			public Volume(Bounds bounds)
			{
				Vector3 size = bounds.size;
				this.corner = bounds.center - size * 0.5f;
				this.X = new Vector3(size.x, 0f, 0f);
				this.Y = new Vector3(0f, size.y, 0f);
				this.Z = new Vector3(0f, 0f, size.z);
				this.maxSubdivisionMultiplier = (this.minSubdivisionMultiplier = 0f);
			}

			public Bounds CalculateAABB()
			{
				Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
				Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
				for (int i = 0; i < 2; i++)
				{
					for (int j = 0; j < 2; j++)
					{
						for (int k = 0; k < 2; k++)
						{
							Vector3 vector3 = new Vector3((float)i, (float)j, (float)k);
							Vector3 rhs = this.corner + this.X * vector3.x + this.Y * vector3.y + this.Z * vector3.z;
							vector = Vector3.Min(vector, rhs);
							vector2 = Vector3.Max(vector2, rhs);
						}
					}
				}
				return new Bounds((vector + vector2) / 2f, vector2 - vector);
			}

			public void CalculateCenterAndSize(out Vector3 center, out Vector3 size)
			{
				size = new Vector3(this.X.magnitude, this.Y.magnitude, this.Z.magnitude);
				center = this.corner + this.X * 0.5f + this.Y * 0.5f + this.Z * 0.5f;
			}

			public void Transform(Matrix4x4 trs)
			{
				this.corner = trs.MultiplyPoint(this.corner);
				this.X = trs.MultiplyVector(this.X);
				this.Y = trs.MultiplyVector(this.Y);
				this.Z = trs.MultiplyVector(this.Z);
			}

			public override string ToString()
			{
				return string.Format("Corner: {0}, X: {1}, Y: {2}, Z: {3}, MaxSubdiv: {4}", new object[]
				{
					this.corner,
					this.X,
					this.Y,
					this.Z,
					this.maxSubdivisionMultiplier
				});
			}

			public bool Equals(ProbeReferenceVolume.Volume other)
			{
				return this.corner == other.corner && this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.minSubdivisionMultiplier == other.minSubdivisionMultiplier && this.maxSubdivisionMultiplier == other.maxSubdivisionMultiplier;
			}

			internal Vector3 corner;

			internal Vector3 X;

			internal Vector3 Y;

			internal Vector3 Z;

			internal float maxSubdivisionMultiplier;

			internal float minSubdivisionMultiplier;
		}

		internal struct RefVolTransform
		{
			public Vector3 posWS;

			public Quaternion rot;

			public float scale;
		}

		public struct RuntimeResources
		{
			public ComputeBuffer index;

			public ComputeBuffer cellIndices;

			public RenderTexture L0_L1rx;

			public RenderTexture L1_G_ry;

			public RenderTexture L1_B_rz;

			public RenderTexture L2_0;

			public RenderTexture L2_1;

			public RenderTexture L2_2;

			public RenderTexture L2_3;

			public RenderTexture ProbeOcclusion;

			public RenderTexture Validity;

			public RenderTexture SkyOcclusionL0L1;

			public RenderTexture SkyShadingDirectionIndices;

			public ComputeBuffer SkyPrecomputedDirections;

			public ComputeBuffer QualityLeakReductionData;
		}

		public struct ExtraDataActionInput
		{
		}

		internal class CellInstancedDebugProbes
		{
			public List<Matrix4x4[]> probeBuffers;

			public List<Matrix4x4[]> offsetBuffers;

			public List<MaterialPropertyBlock> props;
		}

		private class RenderFragmentationOverlayPassData
		{
			public Material debugFragmentationMaterial;

			public DebugOverlay debugOverlay;

			public int chunkCount;

			public ComputeBuffer debugFragmentationData;

			public TextureHandle colorBuffer;

			public TextureHandle depthBuffer;
		}

		internal class DiskStreamingRequest
		{
			public DiskStreamingRequest(int maxRequestCount)
			{
				this.m_ReadCommandBuffer = new NativeArray<ReadCommand>(maxRequestCount, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}

			public unsafe void AddReadCommand(int offset, int size, byte* dest)
			{
				int commandCount = this.m_ReadCommandArray.CommandCount;
				this.m_ReadCommandArray.CommandCount = commandCount + 1;
				this.m_ReadCommandBuffer[commandCount] = new ReadCommand
				{
					Buffer = (void*)dest,
					Offset = (long)offset,
					Size = (long)size
				};
				this.m_BytesWritten += size;
			}

			public unsafe int RunCommands(FileHandle file)
			{
				this.m_ReadCommandArray.ReadCommands = (ReadCommand*)this.m_ReadCommandBuffer.GetUnsafePtr<ReadCommand>();
				this.m_ReadHandle = AsyncReadManager.Read(file, this.m_ReadCommandArray);
				return this.m_BytesWritten;
			}

			public void Clear()
			{
				if (this.m_ReadHandle.IsValid())
				{
					this.m_ReadHandle.JobHandle.Complete();
				}
				this.m_ReadHandle = default(ReadHandle);
				this.m_ReadCommandArray.CommandCount = 0;
				this.m_BytesWritten = 0;
			}

			public void Cancel()
			{
				if (this.m_ReadHandle.IsValid())
				{
					this.m_ReadHandle.Cancel();
				}
			}

			public void Wait()
			{
				if (this.m_ReadHandle.IsValid())
				{
					this.m_ReadHandle.JobHandle.Complete();
				}
			}

			public void Dispose()
			{
				this.m_ReadCommandBuffer.Dispose();
			}

			public ReadStatus GetStatus()
			{
				if (!this.m_ReadHandle.IsValid())
				{
					return ReadStatus.Complete;
				}
				return this.m_ReadHandle.Status;
			}

			private ReadHandle m_ReadHandle;

			private ReadCommandArray m_ReadCommandArray;

			private NativeArray<ReadCommand> m_ReadCommandBuffer;

			private int m_BytesWritten;
		}

		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\Lighting\\ProbeVolume\\ProbeReferenceVolume.Streaming.cs", needAccessors = false, generateCBuffer = true)]
		internal struct CellStreamingScratchBufferLayout
		{
			public int _SharedDestChunksOffset;

			public int _L0L1rxOffset;

			public int _L1GryOffset;

			public int _L1BrzOffset;

			public int _ValidityOffset;

			public int _ProbeOcclusionOffset;

			public int _SkyOcclusionOffset;

			public int _SkyShadingDirectionOffset;

			public int _L2_0Offset;

			public int _L2_1Offset;

			public int _L2_2Offset;

			public int _L2_3Offset;

			public int _L0Size;

			public int _L0ProbeSize;

			public int _L1Size;

			public int _L1ProbeSize;

			public int _ValiditySize;

			public int _ValidityProbeSize;

			public int _ProbeOcclusionSize;

			public int _ProbeOcclusionProbeSize;

			public int _SkyOcclusionSize;

			public int _SkyOcclusionProbeSize;

			public int _SkyShadingDirectionSize;

			public int _SkyShadingDirectionProbeSize;

			public int _L2Size;

			public int _L2ProbeSize;

			public int _ProbeCountInChunkLine;

			public int _ProbeCountInChunkSlice;
		}

		internal class CellStreamingScratchBuffer
		{
			public CellStreamingScratchBuffer(int chunkCount, int chunkSize, bool allocateGraphicsBuffers)
			{
				this.chunkCount = chunkCount;
				this.chunkSize = chunkSize;
				int num = chunkCount * chunkSize / 4 + chunkCount * 4;
				num += 2 * chunkCount * 4;
				if (allocateGraphicsBuffers)
				{
					for (int i = 0; i < 2; i++)
					{
						this.m_GraphicsBuffers[i] = new GraphicsBuffer(GraphicsBuffer.Target.Raw, GraphicsBuffer.UsageFlags.LockBufferForWrite, num, 4);
					}
				}
				this.m_CurrentBuffer = 0;
				this.stagingBuffer = new NativeArray<byte>(num * 4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			}

			public void Swap()
			{
				this.m_CurrentBuffer = (this.m_CurrentBuffer + 1) % 2;
			}

			public void Dispose()
			{
				for (int i = 0; i < 2; i++)
				{
					GraphicsBuffer graphicsBuffer = this.m_GraphicsBuffers[i];
					if (graphicsBuffer != null)
					{
						graphicsBuffer.Dispose();
					}
				}
				this.stagingBuffer.Dispose();
			}

			public GraphicsBuffer buffer
			{
				get
				{
					return this.m_GraphicsBuffers[this.m_CurrentBuffer];
				}
			}

			public int chunkCount { get; }

			public int chunkSize { get; }

			public NativeArray<byte> stagingBuffer;

			private int m_CurrentBuffer;

			private GraphicsBuffer[] m_GraphicsBuffers = new GraphicsBuffer[2];
		}

		[DebuggerDisplay("Index = {cell.desc.index} State = {state}")]
		internal class CellStreamingRequest
		{
			public ProbeReferenceVolume.Cell cell { get; set; }

			public ProbeReferenceVolume.CellStreamingRequest.State state { get; set; }

			public ProbeReferenceVolume.CellStreamingScratchBuffer scratchBuffer { get; set; }

			public ProbeReferenceVolume.CellStreamingScratchBufferLayout scratchBufferLayout { get; set; }

			public ProbeVolumeBakingSet.PerScenarioDataInfo scenarioData { get; set; }

			public int poolIndex { get; set; }

			public bool streamSharedData { get; set; }

			public bool IsStreaming()
			{
				return this.state == ProbeReferenceVolume.CellStreamingRequest.State.Pending || this.state == ProbeReferenceVolume.CellStreamingRequest.State.Active;
			}

			public void Cancel()
			{
				if (this.state == ProbeReferenceVolume.CellStreamingRequest.State.Active)
				{
					this.brickStreamingRequest.Cancel();
					this.supportStreamingRequest.Cancel();
					this.cellDataStreamingRequest.Cancel();
					this.cellOptionalDataStreamingRequest.Cancel();
					this.cellSharedDataStreamingRequest.Cancel();
					this.cellProbeOcclusionDataStreamingRequest.Cancel();
				}
				this.state = ProbeReferenceVolume.CellStreamingRequest.State.Canceled;
			}

			public void WaitAll()
			{
				if (this.state == ProbeReferenceVolume.CellStreamingRequest.State.Active)
				{
					this.brickStreamingRequest.Wait();
					this.supportStreamingRequest.Wait();
					this.cellDataStreamingRequest.Wait();
					this.cellOptionalDataStreamingRequest.Wait();
					this.cellSharedDataStreamingRequest.Wait();
					this.cellProbeOcclusionDataStreamingRequest.Wait();
				}
			}

			public bool UpdateRequestState(ProbeReferenceVolume.DiskStreamingRequest request, ref bool isComplete)
			{
				ReadStatus status = request.GetStatus();
				if (status == ReadStatus.Failed)
				{
					return false;
				}
				isComplete &= (status == ReadStatus.Complete);
				return true;
			}

			public void UpdateState()
			{
				if (this.state == ProbeReferenceVolume.CellStreamingRequest.State.Active)
				{
					bool flag = true;
					if (!(this.UpdateRequestState(this.brickStreamingRequest, ref flag) & this.UpdateRequestState(this.supportStreamingRequest, ref flag) & this.UpdateRequestState(this.cellDataStreamingRequest, ref flag) & this.UpdateRequestState(this.cellOptionalDataStreamingRequest, ref flag) & this.UpdateRequestState(this.cellSharedDataStreamingRequest, ref flag) & this.UpdateRequestState(this.cellProbeOcclusionDataStreamingRequest, ref flag)))
					{
						this.Cancel();
						this.state = ProbeReferenceVolume.CellStreamingRequest.State.Invalid;
						return;
					}
					if (flag)
					{
						this.state = ProbeReferenceVolume.CellStreamingRequest.State.Complete;
					}
				}
			}

			public void Clear()
			{
				this.cell = null;
				this.Reset();
			}

			public void Reset()
			{
				this.state = ProbeReferenceVolume.CellStreamingRequest.State.Pending;
				this.scratchBuffer = null;
				this.brickStreamingRequest.Clear();
				this.supportStreamingRequest.Clear();
				this.cellDataStreamingRequest.Clear();
				this.cellOptionalDataStreamingRequest.Clear();
				this.cellSharedDataStreamingRequest.Clear();
				this.cellProbeOcclusionDataStreamingRequest.Clear();
				this.bytesWritten = 0;
			}

			public void Dispose()
			{
				this.brickStreamingRequest.Dispose();
				this.supportStreamingRequest.Dispose();
				this.cellDataStreamingRequest.Dispose();
				this.cellOptionalDataStreamingRequest.Dispose();
				this.cellSharedDataStreamingRequest.Dispose();
				this.cellProbeOcclusionDataStreamingRequest.Dispose();
			}

			public ProbeReferenceVolume.CellStreamingRequest.OnStreamingCompleteDelegate onStreamingComplete;

			public ProbeReferenceVolume.DiskStreamingRequest cellDataStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(1);

			public ProbeReferenceVolume.DiskStreamingRequest cellOptionalDataStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(1);

			public ProbeReferenceVolume.DiskStreamingRequest cellSharedDataStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(1);

			public ProbeReferenceVolume.DiskStreamingRequest cellProbeOcclusionDataStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(1);

			public ProbeReferenceVolume.DiskStreamingRequest brickStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(1);

			public ProbeReferenceVolume.DiskStreamingRequest supportStreamingRequest = new ProbeReferenceVolume.DiskStreamingRequest(5);

			public int bytesWritten;

			public enum State
			{
				Pending,
				Active,
				Canceled,
				Invalid,
				Complete
			}

			public delegate void OnStreamingCompleteDelegate(ProbeReferenceVolume.CellStreamingRequest request, CommandBuffer cmd);
		}
	}
}
