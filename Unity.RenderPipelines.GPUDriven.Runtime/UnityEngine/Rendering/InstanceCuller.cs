using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal struct InstanceCuller : IDisposable
	{
		internal void Init(GPUResidentDrawerResources resources, DebugRendererBatcherStats debugStats = null)
		{
			this.m_IndirectStorage.Init();
			this.m_OcclusionTestShader.Init(resources.instanceOcclusionCullingKernels);
			this.m_ResetDrawArgsKernel = this.m_OcclusionTestShader.cs.FindKernel("ResetDrawArgs");
			this.m_CopyInstancesKernel = this.m_OcclusionTestShader.cs.FindKernel("CopyInstances");
			this.m_CullInstancesKernel = this.m_OcclusionTestShader.cs.FindKernel("CullInstances");
			this.m_DebugStats = debugStats;
			this.m_SplitDebugArray = default(InstanceCullerSplitDebugArray);
			this.m_SplitDebugArray.Init();
			this.m_OcclusionEventDebugArray = default(InstanceOcclusionEventDebugArray);
			this.m_OcclusionEventDebugArray.Init();
			this.m_ProfilingSampleInstanceOcclusionTest = new ProfilingSampler("InstanceOcclusionTest");
			this.m_ShaderVariables = new NativeArray<InstanceOcclusionCullerShaderVariables>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_ConstantBuffer = new ComputeBuffer(1, UnsafeUtility.SizeOf<InstanceOcclusionCullerShaderVariables>(), ComputeBufferType.Constant);
			this.m_CommandBuffer = new CommandBuffer();
			this.m_CommandBuffer.name = "EnsureValidOcclusionTestResults";
			this.m_LODParamsToCameraID = new NativeParallelHashMap<int, InstanceCuller.AnimatedFadeData>(16, Allocator.Persistent);
		}

		private JobHandle AnimateCrossFades(CPUPerCameraInstanceData perCameraInstanceData, BatchCullingContext cc, out CPUPerCameraInstanceData.PerCameraInstanceDataArrays cameraInstanceData, out bool hasAnimatedCrossfade)
		{
			int hashCode = cc.lodParameters.GetHashCode();
			InstanceCuller.AnimatedFadeData animatedFadeData;
			hasAnimatedCrossfade = this.m_LODParamsToCameraID.TryGetValue(hashCode, out animatedFadeData);
			if (hasAnimatedCrossfade)
			{
				cameraInstanceData = perCameraInstanceData.perCameraData[animatedFadeData.cameraID];
				return animatedFadeData.jobHandle;
			}
			if (cc.viewType != BatchCullingViewType.Camera && !hasAnimatedCrossfade)
			{
				cameraInstanceData = default(CPUPerCameraInstanceData.PerCameraInstanceDataArrays);
				return default(JobHandle);
			}
			int instanceID = cc.viewID.GetInstanceID();
			CPUPerCameraInstanceData.PerCameraInstanceDataArrays perCameraInstanceDataArrays;
			hasAnimatedCrossfade = perCameraInstanceData.perCameraData.TryGetValue(instanceID, out perCameraInstanceDataArrays);
			if (!hasAnimatedCrossfade)
			{
				cameraInstanceData = default(CPUPerCameraInstanceData.PerCameraInstanceDataArrays);
				return default(JobHandle);
			}
			cameraInstanceData = perCameraInstanceDataArrays;
			JobHandle jobHandle = new AnimateCrossFadeJob
			{
				deltaTime = Time.deltaTime,
				crossFadeArray = cameraInstanceData.crossFades
			}.Schedule(perCameraInstanceData.instancesLength, 512, default(JobHandle));
			this.m_LODParamsToCameraID.TryAdd(hashCode, new InstanceCuller.AnimatedFadeData
			{
				cameraID = instanceID,
				jobHandle = jobHandle
			});
			return jobHandle;
		}

		private unsafe JobHandle CreateFrustumCullingJob(in BatchCullingContext cc, in CPUInstanceData.ReadOnly instanceData, in CPUSharedInstanceData.ReadOnly sharedInstanceData, in CPUPerCameraInstanceData perCameraInstanceData, NativeList<LODGroupCullingData> lodGroupCullingData, in BinningConfig binningConfig, float smallMeshScreenPercentage, OcclusionCullingCommon occlusionCullingCommon, NativeArray<byte> rendererVisibilityMasks, NativeArray<byte> rendererMeshLodSettings, NativeArray<byte> rendererCrossFadeValues)
		{
			ReceiverPlanes receiverPlanes;
			ReceiverSphereCuller receiverSphereCuller;
			FrustumPlaneCuller frustumPlaneCuller;
			float num;
			float num2;
			fixed (BatchCullingContext* ptr = &cc)
			{
				BatchCullingContext* context = ptr;
				InstanceCullerBurst.SetupCullingJobInput(QualitySettings.lodBias, QualitySettings.meshLodThreshold, context, &receiverPlanes, &receiverSphereCuller, &frustumPlaneCuller, &num, &num2);
			}
			if (occlusionCullingCommon != null)
			{
				occlusionCullingCommon.UpdateSilhouettePlanes(cc.viewID.GetInstanceID(), receiverPlanes.SilhouettePlaneSubArray());
			}
			CPUPerCameraInstanceData.PerCameraInstanceDataArrays cameraInstanceData;
			bool animateCrossFades;
			JobHandle dependsOn = this.AnimateCrossFades(perCameraInstanceData, cc, out cameraInstanceData, out animateCrossFades);
			JobHandle jobHandle = new CullingJob
			{
				binningConfig = binningConfig,
				viewType = cc.viewType,
				frustumPlanePackets = frustumPlaneCuller.planePackets.AsArray(),
				frustumSplitInfos = frustumPlaneCuller.splitInfos.AsArray(),
				lightFacingFrustumPlanes = receiverPlanes.LightFacingFrustumPlaneSubArray(),
				receiverSplitInfos = receiverSphereCuller.splitInfos.AsArray(),
				worldToLightSpaceRotation = receiverSphereCuller.worldToLightSpaceRotation,
				cullLightmappedShadowCasters = ((cc.cullingFlags & BatchCullingFlags.CullLightmappedShadowCasters) > BatchCullingFlags.None),
				cameraPosition = cc.lodParameters.cameraPosition,
				sqrMeshLodSelectionConstant = num2 * num2,
				sqrScreenRelativeMetric = num * num,
				minScreenRelativeHeight = smallMeshScreenPercentage * 0.01f,
				isOrtho = cc.lodParameters.isOrthographic,
				animateCrossFades = animateCrossFades,
				instanceData = instanceData,
				sharedInstanceData = sharedInstanceData,
				cameraInstanceData = cameraInstanceData,
				lodGroupCullingData = lodGroupCullingData,
				occlusionBuffer = cc.occlusionBuffer,
				rendererVisibilityMasks = rendererVisibilityMasks,
				rendererMeshLodSettings = rendererMeshLodSettings,
				rendererCrossFadeValues = rendererCrossFadeValues,
				maxLOD = QualitySettings.maximumLODLevel,
				cullingLayerMask = cc.cullingLayerMask,
				sceneCullingMask = cc.sceneCullingMask
			}.Schedule(instanceData.instancesLength, 32, dependsOn);
			receiverPlanes.Dispose(jobHandle);
			frustumPlaneCuller.Dispose(jobHandle);
			receiverSphereCuller.Dispose(jobHandle);
			return jobHandle;
		}

		private int ComputeWorstCaseDrawCommandCount(in BatchCullingContext cc, BinningConfig binningConfig, CPUDrawInstanceData drawInstanceData)
		{
			int length = drawInstanceData.drawInstances.Length;
			int num = drawInstanceData.drawBatches.Length;
			if (binningConfig.supportsCrossFade)
			{
				num *= 2;
			}
			num *= 2;
			if (binningConfig.supportsMotionCheck)
			{
				num *= 2;
			}
			if (cc.cullingSplits.Length > 1)
			{
				num <<= cc.cullingSplits.Length - 1;
			}
			return math.min(num, length);
		}

		public unsafe JobHandle CreateCullJobTree(in BatchCullingContext cc, BatchCullingOutput cullingOutput, in CPUInstanceData.ReadOnly instanceData, in CPUSharedInstanceData.ReadOnly sharedInstanceData, in CPUPerCameraInstanceData perCameraInstanceData, in GPUInstanceDataBuffer.ReadOnly instanceDataBuffer, NativeList<LODGroupCullingData> lodGroupCullingData, CPUDrawInstanceData drawInstanceData, NativeParallelHashMap<uint, BatchID> batchIDs, float smallMeshScreenPercentage, OcclusionCullingCommon occlusionCullingCommon)
		{
			BatchCullingOutputDrawCommands batchCullingOutputDrawCommands = default(BatchCullingOutputDrawCommands);
			batchCullingOutputDrawCommands.drawRangeCount = drawInstanceData.drawRanges.Length;
			batchCullingOutputDrawCommands.drawRanges = MemoryUtilities.Malloc<BatchDrawRange>(batchCullingOutputDrawCommands.drawRangeCount, Allocator.TempJob);
			for (int i = 0; i < batchCullingOutputDrawCommands.drawRangeCount; i++)
			{
				batchCullingOutputDrawCommands.drawRanges[i].drawCommandsCount = 0U;
			}
			cullingOutput.drawCommands[0] = batchCullingOutputDrawCommands;
			cullingOutput.customCullingResult[0] = IntPtr.Zero;
			BinningConfig binningConfig = new BinningConfig
			{
				viewCount = cc.cullingSplits.Length,
				supportsCrossFade = QualitySettings.enableLODCrossFade,
				supportsMotionCheck = (cc.viewType == BatchCullingViewType.Camera)
			};
			int handlesLength = instanceData.handlesLength;
			NativeArray<byte> rendererVisibilityMasks = new NativeArray<byte>(handlesLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<byte> rendererCrossFadeValues = new NativeArray<byte>(handlesLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			NativeArray<byte> rendererMeshLodSettings = new NativeArray<byte>(handlesLength, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			JobHandle jobHandle = this.CreateFrustumCullingJob(cc, instanceData, sharedInstanceData, perCameraInstanceData, lodGroupCullingData, binningConfig, smallMeshScreenPercentage, occlusionCullingCommon, rendererVisibilityMasks, rendererMeshLodSettings, rendererCrossFadeValues);
			if (cc.viewType == BatchCullingViewType.Camera || cc.viewType == BatchCullingViewType.Light || cc.viewType == BatchCullingViewType.SelectionOutline)
			{
				jobHandle = this.CreateCompactedVisibilityMaskJob(instanceData, rendererVisibilityMasks, jobHandle);
				int num = -1;
				DebugRendererBatcherStats debugStats = this.m_DebugStats;
				if (debugStats != null && debugStats.enabled)
				{
					num = this.m_SplitDebugArray.TryAddSplits(cc.viewType, cc.viewID.GetInstanceID(), cc.cullingSplits.Length);
				}
				int length = drawInstanceData.drawBatches.Length;
				int length2 = this.ComputeWorstCaseDrawCommandCount(cc, binningConfig, drawInstanceData);
				NativeArray<int> batchBinAllocOffsets = new NativeArray<int>(length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> batchBinCounts = new NativeArray<int>(length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> batchDrawCommandOffsets = new NativeArray<int>(length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> binAllocCounter = new NativeArray<int>(16, Allocator.TempJob, NativeArrayOptions.ClearMemory);
				NativeArray<short> binConfigIndices = new NativeArray<short>(length2, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> binVisibleInstanceCounts = new NativeArray<int>(length2, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				NativeArray<int> binVisibleInstanceOffsets = new NativeArray<int>(length2, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
				int contextIndex = -1;
				bool flag = occlusionCullingCommon != null && occlusionCullingCommon.HasOccluderContext(cc.viewID.GetInstanceID());
				if (flag)
				{
					int instanceID = cc.viewID.GetInstanceID();
					contextIndex = this.m_IndirectStorage.TryAllocateContext(instanceID);
					cullingOutput.customCullingResult[0] = (IntPtr)instanceID;
				}
				IndirectBufferLimits limits = this.m_IndirectStorage.GetLimits(contextIndex);
				NativeArray<IndirectBufferAllocInfo> allocInfoSubArray = this.m_IndirectStorage.GetAllocInfoSubArray(contextIndex);
				JobHandle jobHandle2 = new AllocateBinsPerBatch
				{
					binningConfig = binningConfig,
					drawBatches = drawInstanceData.drawBatches,
					drawInstanceIndices = drawInstanceData.drawInstanceIndices,
					instanceData = instanceData,
					rendererVisibilityMasks = rendererVisibilityMasks,
					rendererMeshLodSettings = rendererMeshLodSettings,
					batchBinAllocOffsets = batchBinAllocOffsets,
					batchBinCounts = batchBinCounts,
					binAllocCounter = binAllocCounter,
					binConfigIndices = binConfigIndices,
					binVisibleInstanceCounts = binVisibleInstanceCounts,
					splitDebugCounters = this.m_SplitDebugArray.Counters,
					debugCounterIndexBase = num
				}.Schedule(length, 1, jobHandle);
				this.m_SplitDebugArray.AddSync(num, jobHandle2);
				JobHandle dependsOn = new PrefixSumDrawsAndInstances
				{
					drawRanges = drawInstanceData.drawRanges,
					drawBatchIndices = drawInstanceData.drawBatchIndices,
					batchBinAllocOffsets = batchBinAllocOffsets,
					batchBinCounts = batchBinCounts,
					binVisibleInstanceCounts = binVisibleInstanceCounts,
					batchDrawCommandOffsets = batchDrawCommandOffsets,
					binVisibleInstanceOffsets = binVisibleInstanceOffsets,
					cullingOutput = cullingOutput.drawCommands,
					indirectBufferLimits = limits,
					indirectBufferAllocInfo = allocInfoSubArray,
					indirectAllocationCounters = this.m_IndirectStorage.allocationCounters
				}.Schedule(jobHandle2);
				JobHandle jobHandle3 = new DrawCommandOutputPerBatch
				{
					binningConfig = binningConfig,
					batchIDs = batchIDs,
					instanceDataBuffer = instanceDataBuffer,
					drawBatches = drawInstanceData.drawBatches,
					drawInstanceIndices = drawInstanceData.drawInstanceIndices,
					instanceData = instanceData,
					rendererVisibilityMasks = rendererVisibilityMasks,
					rendererMeshLodSettings = rendererMeshLodSettings,
					rendererCrossFadeValues = rendererCrossFadeValues,
					batchBinAllocOffsets = batchBinAllocOffsets,
					batchBinCounts = batchBinCounts,
					batchDrawCommandOffsets = batchDrawCommandOffsets,
					binConfigIndices = binConfigIndices,
					binVisibleInstanceOffsets = binVisibleInstanceOffsets,
					binVisibleInstanceCounts = binVisibleInstanceCounts,
					cullingOutput = cullingOutput.drawCommands,
					indirectBufferLimits = limits,
					visibleInstancesBufferHandle = this.m_IndirectStorage.visibleInstanceBufferHandle,
					indirectArgsBufferHandle = this.m_IndirectStorage.indirectArgsBufferHandle,
					indirectBufferAllocInfo = allocInfoSubArray,
					indirectInstanceInfoGlobalArray = this.m_IndirectStorage.instanceInfoGlobalArray,
					indirectDrawInfoGlobalArray = this.m_IndirectStorage.drawInfoGlobalArray
				}.Schedule(length, 1, dependsOn);
				if (flag)
				{
					this.m_IndirectStorage.SetBufferContext(contextIndex, new IndirectBufferContext(jobHandle3));
				}
				jobHandle = jobHandle3;
			}
			jobHandle = rendererVisibilityMasks.Dispose(jobHandle);
			jobHandle = rendererCrossFadeValues.Dispose(jobHandle);
			return rendererMeshLodSettings.Dispose(jobHandle);
		}

		private JobHandle CreateCompactedVisibilityMaskJob(in CPUInstanceData.ReadOnly instanceData, NativeArray<byte> rendererVisibilityMasks, JobHandle cullingJobHandle)
		{
			if (!this.m_CompactedVisibilityMasks.IsCreated)
			{
				this.m_CompactedVisibilityMasks = new ParallelBitArray(instanceData.handlesLength, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			}
			JobHandle jobHandle = new CompactVisibilityMasksJob
			{
				rendererVisibilityMasks = rendererVisibilityMasks,
				compactedVisibilityMasks = this.m_CompactedVisibilityMasks
			}.ScheduleBatch(rendererVisibilityMasks.Length, 64, cullingJobHandle);
			this.m_CompactedVisibilityMasksJobsHandle = JobHandle.CombineDependencies(this.m_CompactedVisibilityMasksJobsHandle, jobHandle);
			return jobHandle;
		}

		public void InstanceOccludersUpdated(int viewInstanceID, int subviewMask, RenderersBatchersContext batchersContext)
		{
			DebugRendererBatcherStats debugStats = this.m_DebugStats;
			OccluderContext occluderContext;
			if (debugStats != null && debugStats.enabled && batchersContext.occlusionCullingCommon.GetOccluderContext(viewInstanceID, out occluderContext))
			{
				this.m_OcclusionEventDebugArray.TryAdd(viewInstanceID, InstanceOcclusionEventType.OccluderUpdate, occluderContext.version, subviewMask, OcclusionTest.None);
			}
		}

		private void DisposeCompactVisibilityMasks()
		{
			if (this.m_CompactedVisibilityMasks.IsCreated)
			{
				this.m_CompactedVisibilityMasks.Dispose();
			}
		}

		private void DisposeSceneViewHiddenBits()
		{
		}

		public ParallelBitArray GetCompactedVisibilityMasks(bool syncCullingJobs)
		{
			if (syncCullingJobs)
			{
				this.m_CompactedVisibilityMasksJobsHandle.Complete();
			}
			return this.m_CompactedVisibilityMasks;
		}

		public void InstanceOcclusionTest(RenderGraph renderGraph, in OcclusionCullingSettings settings, ReadOnlySpan<SubviewOcclusionTest> subviewOcclusionTests, RenderersBatchersContext batchersContext)
		{
			OccluderContext occluderContext;
			if (!batchersContext.occlusionCullingCommon.GetOccluderContext(settings.viewInstanceID, out occluderContext))
			{
				return;
			}
			OccluderHandles occluderHandles = occluderContext.Import(renderGraph);
			if (!occluderHandles.IsValid())
			{
				return;
			}
			InstanceCuller.InstanceOcclusionTestPassData instanceOcclusionTestPassData;
			using (IComputeRenderGraphBuilder computeRenderGraphBuilder = renderGraph.AddComputePass<InstanceCuller.InstanceOcclusionTestPassData>("Instance Occlusion Test", out instanceOcclusionTestPassData, this.m_ProfilingSampleInstanceOcclusionTest, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@04755ad51d99\\Runtime\\GPUDriven\\InstanceCuller.cs", 2326))
			{
				computeRenderGraphBuilder.AllowGlobalStateModification(true);
				instanceOcclusionTestPassData.settings = settings;
				instanceOcclusionTestPassData.subviewSettings = InstanceOcclusionTestSubviewSettings.FromSpan(subviewOcclusionTests);
				instanceOcclusionTestPassData.bufferHandles = this.m_IndirectStorage.ImportBuffers(renderGraph);
				instanceOcclusionTestPassData.occluderHandles = occluderHandles;
				instanceOcclusionTestPassData.bufferHandles.UseForOcclusionTest(computeRenderGraphBuilder);
				instanceOcclusionTestPassData.occluderHandles.UseForOcclusionTest(computeRenderGraphBuilder);
				computeRenderGraphBuilder.SetRenderFunc<InstanceCuller.InstanceOcclusionTestPassData>(delegate(InstanceCuller.InstanceOcclusionTestPassData data, ComputeGraphContext context)
				{
					GPUResidentBatcher batcher = GPUResidentDrawer.instance.batcher;
					batcher.instanceCullingBatcher.culler.AddOcclusionCullingDispatch(context.cmd, data.settings, data.subviewSettings, data.bufferHandles, data.occluderHandles, batcher.batchersContext);
				});
			}
		}

		internal void EnsureValidOcclusionTestResults(int viewInstanceID)
		{
			int num = this.m_IndirectStorage.TryGetContextIndex(viewInstanceID);
			if (num >= 0)
			{
				IndirectBufferContext bufferContext = this.m_IndirectStorage.GetBufferContext(num);
				if (bufferContext.bufferState == IndirectBufferContext.BufferState.Pending)
				{
					bufferContext.cullingJobHandle.Complete();
				}
				IndirectBufferAllocInfo allocInfo = this.m_IndirectStorage.GetAllocInfo(num);
				if (!allocInfo.IsEmpty())
				{
					CommandBuffer commandBuffer = this.m_CommandBuffer;
					commandBuffer.Clear();
					this.m_IndirectStorage.CopyFromStaging(commandBuffer, allocInfo);
					ComputeShader cs = this.m_OcclusionTestShader.cs;
					this.m_ShaderVariables[0] = new InstanceOcclusionCullerShaderVariables
					{
						_DrawInfoAllocIndex = (uint)allocInfo.drawAllocIndex,
						_DrawInfoCount = (uint)allocInfo.drawCount,
						_InstanceInfoAllocIndex = (uint)(2 * allocInfo.instanceAllocIndex),
						_InstanceInfoCount = (uint)allocInfo.instanceCount,
						_BoundingSphereInstanceDataAddress = 0,
						_DebugCounterIndex = -1,
						_InstanceMultiplierShift = 0
					};
					commandBuffer.SetBufferData<InstanceOcclusionCullerShaderVariables>(this.m_ConstantBuffer, this.m_ShaderVariables);
					commandBuffer.SetComputeConstantBufferParam(cs, InstanceCuller.ShaderIDs.InstanceOcclusionCullerShaderVariables, this.m_ConstantBuffer, 0, this.m_ConstantBuffer.stride);
					int copyInstancesKernel = this.m_CopyInstancesKernel;
					commandBuffer.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._DrawInfo, this.m_IndirectStorage.drawInfoBuffer);
					commandBuffer.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._InstanceInfo, this.m_IndirectStorage.instanceInfoBuffer);
					commandBuffer.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._DrawArgs, this.m_IndirectStorage.argsBuffer);
					commandBuffer.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._InstanceIndices, this.m_IndirectStorage.instanceBuffer);
					commandBuffer.DispatchCompute(cs, copyInstancesKernel, (allocInfo.instanceCount + 63) / 64, 1, 1);
					Graphics.ExecuteCommandBuffer(commandBuffer);
					commandBuffer.Clear();
				}
			}
		}

		private void AddOcclusionCullingDispatch(ComputeCommandBuffer cmd, in OcclusionCullingSettings settings, in InstanceOcclusionTestSubviewSettings subviewSettings, in IndirectBufferContextHandles bufferHandles, in OccluderHandles occluderHandles, RenderersBatchersContext batchersContext)
		{
			OcclusionCullingCommon occlusionCullingCommon = batchersContext.occlusionCullingCommon;
			int num = this.m_IndirectStorage.TryGetContextIndex(settings.viewInstanceID);
			if (num >= 0)
			{
				IndirectBufferContext bufferContext = this.m_IndirectStorage.GetBufferContext(num);
				OccluderContext occluderContext;
				bool flag = occlusionCullingCommon.GetOccluderContext(settings.viewInstanceID, out occluderContext);
				flag = (flag && (subviewSettings.occluderSubviewMask & occluderContext.subviewValidMask) == subviewSettings.occluderSubviewMask);
				IndirectBufferContext.BufferState bufferState = IndirectBufferContext.BufferState.Zeroed;
				int occluderVersion = 0;
				int subviewMask = 0;
				switch (settings.occlusionTest)
				{
				case OcclusionTest.None:
					bufferState = IndirectBufferContext.BufferState.NoOcclusionTest;
					break;
				case OcclusionTest.TestAll:
					if (flag)
					{
						bufferState = IndirectBufferContext.BufferState.AllInstancesOcclusionTested;
						occluderVersion = occluderContext.version;
						subviewMask = subviewSettings.occluderSubviewMask;
					}
					else
					{
						bufferState = IndirectBufferContext.BufferState.NoOcclusionTest;
					}
					break;
				case OcclusionTest.TestCulled:
					if (flag)
					{
						bool flag2 = true;
						IndirectBufferContext.BufferState bufferState2 = bufferContext.bufferState;
						if (bufferState2 - IndirectBufferContext.BufferState.Zeroed > 1)
						{
							if (bufferState2 - IndirectBufferContext.BufferState.AllInstancesOcclusionTested <= 1)
							{
								if (bufferContext.subviewMask != subviewSettings.occluderSubviewMask)
								{
									Debug.Log("Expected an occlusion test of TestCulled to use the same subview mask as the previous occlusion test");
									flag2 = false;
								}
							}
							else
							{
								flag2 = false;
								Debug.Log("Expected the previous occlusion test to be TestAll before using TestCulled");
							}
						}
						else
						{
							flag2 = false;
						}
						if (flag2)
						{
							bufferState = IndirectBufferContext.BufferState.OccludedInstancesReTested;
							occluderVersion = occluderContext.version;
							subviewMask = subviewSettings.occluderSubviewMask;
						}
					}
					break;
				}
				if (!bufferContext.Matches(bufferState, occluderVersion, subviewMask))
				{
					bool flag3 = bufferState == IndirectBufferContext.BufferState.AllInstancesOcclusionTested;
					bool flag4 = bufferState == IndirectBufferContext.BufferState.OccludedInstancesReTested;
					bool flag5 = bufferContext.bufferState == IndirectBufferContext.BufferState.Pending;
					bool flag6 = bufferState == IndirectBufferContext.BufferState.NoOcclusionTest;
					bool flag7 = bufferContext.bufferState != IndirectBufferContext.BufferState.Zeroed && !flag6;
					bool flag8 = bufferState != IndirectBufferContext.BufferState.Zeroed && !flag6;
					if (flag5)
					{
						bufferContext.cullingJobHandle.Complete();
					}
					IndirectBufferAllocInfo allocInfo = this.m_IndirectStorage.GetAllocInfo(num);
					bufferContext.bufferState = bufferState;
					bufferContext.occluderVersion = occluderVersion;
					bufferContext.subviewMask = subviewMask;
					if (!allocInfo.IsEmpty())
					{
						int debugCounterIndex = -1;
						DebugRendererBatcherStats debugStats = this.m_DebugStats;
						if (debugStats != null && debugStats.enabled)
						{
							debugCounterIndex = this.m_OcclusionEventDebugArray.TryAdd(settings.viewInstanceID, InstanceOcclusionEventType.OcclusionTest, occluderVersion, subviewMask, flag3 ? OcclusionTest.TestAll : (flag4 ? OcclusionTest.TestCulled : OcclusionTest.None));
						}
						bool flag9 = false;
						if (flag3 || flag4)
						{
							bool flag10;
							if (OcclusionCullingCommon.UseOcclusionDebug(occluderContext))
							{
								BufferHandle occlusionDebugOverlay = occluderHandles.occlusionDebugOverlay;
								flag10 = occlusionDebugOverlay.IsValid();
							}
							else
							{
								flag10 = false;
							}
							flag9 = flag10;
						}
						ComputeShader cs = this.m_OcclusionTestShader.cs;
						LocalKeyword localKeyword = new LocalKeyword(cs, "OCCLUSION_FIRST_PASS");
						LocalKeyword localKeyword2 = new LocalKeyword(cs, "OCCLUSION_SECOND_PASS");
						OccluderContext.SetKeyword(cmd, cs, localKeyword, flag3);
						OccluderContext.SetKeyword(cmd, cs, localKeyword2, flag4);
						this.m_ShaderVariables[0] = new InstanceOcclusionCullerShaderVariables
						{
							_DrawInfoAllocIndex = (uint)allocInfo.drawAllocIndex,
							_DrawInfoCount = (uint)allocInfo.drawCount,
							_InstanceInfoAllocIndex = (uint)(2 * allocInfo.instanceAllocIndex),
							_InstanceInfoCount = (uint)allocInfo.instanceCount,
							_BoundingSphereInstanceDataAddress = batchersContext.renderersParameters.boundingSphere.gpuAddress,
							_DebugCounterIndex = debugCounterIndex,
							_InstanceMultiplierShift = ((settings.instanceMultiplier == 2) ? 1 : 0)
						};
						cmd.SetBufferData<InstanceOcclusionCullerShaderVariables>(this.m_ConstantBuffer, this.m_ShaderVariables);
						cmd.SetComputeConstantBufferParam(cs, InstanceCuller.ShaderIDs.InstanceOcclusionCullerShaderVariables, this.m_ConstantBuffer, 0, this.m_ConstantBuffer.stride);
						occlusionCullingCommon.PrepareCulling(cmd, occluderContext, settings, subviewSettings, this.m_OcclusionTestShader, flag9);
						if (flag6)
						{
							int copyInstancesKernel = this.m_CopyInstancesKernel;
							cmd.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._DrawInfo, this.m_IndirectStorage.drawInfoBuffer);
							cmd.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._InstanceInfo, this.m_IndirectStorage.instanceInfoBuffer);
							cmd.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._DrawArgs, this.m_IndirectStorage.argsBuffer);
							cmd.SetComputeBufferParam(cs, copyInstancesKernel, InstanceCuller.ShaderIDs._InstanceIndices, this.m_IndirectStorage.instanceBuffer);
							cmd.DispatchCompute(cs, copyInstancesKernel, (allocInfo.instanceCount + 63) / 64, 1, 1);
						}
						if (flag7)
						{
							int resetDrawArgsKernel = this.m_ResetDrawArgsKernel;
							cmd.SetComputeBufferParam(cs, resetDrawArgsKernel, InstanceCuller.ShaderIDs._DrawInfo, bufferHandles.drawInfoBuffer);
							cmd.SetComputeBufferParam(cs, resetDrawArgsKernel, InstanceCuller.ShaderIDs._DrawArgs, bufferHandles.argsBuffer);
							cmd.DispatchCompute(cs, resetDrawArgsKernel, (allocInfo.drawCount + 63) / 64, 1, 1);
						}
						if (flag8)
						{
							int cullInstancesKernel = this.m_CullInstancesKernel;
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._DrawInfo, bufferHandles.drawInfoBuffer);
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._InstanceInfo, bufferHandles.instanceInfoBuffer);
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._DrawArgs, bufferHandles.argsBuffer);
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._InstanceIndices, bufferHandles.instanceBuffer);
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._InstanceDataBuffer, batchersContext.gpuInstanceDataBuffer);
							cmd.SetComputeBufferParam(cs, cullInstancesKernel, InstanceCuller.ShaderIDs._OcclusionDebugCounters, this.m_OcclusionEventDebugArray.CounterBuffer);
							if (flag3 || flag4)
							{
								OcclusionCullingCommon.SetDepthPyramid(cmd, this.m_OcclusionTestShader, cullInstancesKernel, occluderHandles);
							}
							if (flag9)
							{
								OcclusionCullingCommon.SetDebugPyramid(cmd, this.m_OcclusionTestShader, cullInstancesKernel, occluderHandles);
							}
							if (flag4)
							{
								cmd.DispatchCompute(cs, cullInstancesKernel, bufferHandles.argsBuffer, (uint)(20 * allocInfo.GetExtraDrawInfoSlotIndex()));
							}
							else
							{
								cmd.DispatchCompute(cs, cullInstancesKernel, (allocInfo.instanceCount + 63) / 64, 1, 1);
							}
						}
					}
				}
				this.m_IndirectStorage.SetBufferContext(num, bufferContext);
			}
		}

		private void FlushDebugCounters()
		{
			DebugRendererBatcherStats debugStats = this.m_DebugStats;
			if (debugStats != null && debugStats.enabled)
			{
				this.m_SplitDebugArray.MoveToDebugStatsAndClear(this.m_DebugStats);
				this.m_OcclusionEventDebugArray.MoveToDebugStatsAndClear(this.m_DebugStats);
				this.m_DebugStats.FinalizeInstanceCullerViewStats();
			}
		}

		private void OnBeginSceneViewCameraRendering()
		{
		}

		private void OnEndSceneViewCameraRendering()
		{
		}

		public void UpdateFrame(int cameraCount)
		{
			this.DisposeCompactVisibilityMasks();
			if (cameraCount > this.m_LODParamsToCameraID.Capacity)
			{
				this.m_LODParamsToCameraID.Capacity = cameraCount;
			}
			this.m_LODParamsToCameraID.Clear();
			this.FlushDebugCounters();
			this.m_IndirectStorage.ClearContextsAndGrowBuffers();
		}

		public void OnBeginCameraRendering(Camera camera)
		{
			if (camera.cameraType == CameraType.SceneView)
			{
				this.OnBeginSceneViewCameraRendering();
			}
		}

		public void OnEndCameraRendering(Camera camera)
		{
			if (camera.cameraType == CameraType.SceneView)
			{
				this.OnEndSceneViewCameraRendering();
			}
		}

		public void Dispose()
		{
			this.DisposeSceneViewHiddenBits();
			this.DisposeCompactVisibilityMasks();
			this.m_IndirectStorage.Dispose();
			this.m_DebugStats = null;
			this.m_OcclusionEventDebugArray.Dispose();
			this.m_SplitDebugArray.Dispose();
			this.m_ShaderVariables.Dispose();
			this.m_ConstantBuffer.Release();
			this.m_CommandBuffer.Dispose();
			this.m_LODParamsToCameraID.Dispose();
		}

		private NativeParallelHashMap<int, InstanceCuller.AnimatedFadeData> m_LODParamsToCameraID;

		private ParallelBitArray m_CompactedVisibilityMasks;

		private JobHandle m_CompactedVisibilityMasksJobsHandle;

		private IndirectBufferContextStorage m_IndirectStorage;

		private OcclusionTestComputeShader m_OcclusionTestShader;

		private int m_ResetDrawArgsKernel;

		private int m_CopyInstancesKernel;

		private int m_CullInstancesKernel;

		private DebugRendererBatcherStats m_DebugStats;

		private InstanceCullerSplitDebugArray m_SplitDebugArray;

		private InstanceOcclusionEventDebugArray m_OcclusionEventDebugArray;

		private ProfilingSampler m_ProfilingSampleInstanceOcclusionTest;

		private NativeArray<InstanceOcclusionCullerShaderVariables> m_ShaderVariables;

		private ComputeBuffer m_ConstantBuffer;

		private CommandBuffer m_CommandBuffer;

		private struct AnimatedFadeData
		{
			public int cameraID;

			public JobHandle jobHandle;
		}

		private static class ShaderIDs
		{
			public static readonly int InstanceOcclusionCullerShaderVariables = Shader.PropertyToID("InstanceOcclusionCullerShaderVariables");

			public static readonly int _DrawInfo = Shader.PropertyToID("_DrawInfo");

			public static readonly int _InstanceInfo = Shader.PropertyToID("_InstanceInfo");

			public static readonly int _DrawArgs = Shader.PropertyToID("_DrawArgs");

			public static readonly int _InstanceIndices = Shader.PropertyToID("_InstanceIndices");

			public static readonly int _InstanceDataBuffer = Shader.PropertyToID("_InstanceDataBuffer");

			public static readonly int _OccluderDepthPyramid = Shader.PropertyToID("_OccluderDepthPyramid");

			public static readonly int _OcclusionDebugCounters = Shader.PropertyToID("_OcclusionDebugCounters");
		}

		private class InstanceOcclusionTestPassData
		{
			public OcclusionCullingSettings settings;

			public InstanceOcclusionTestSubviewSettings subviewSettings;

			public OccluderHandles occluderHandles;

			public IndirectBufferContextHandles bufferHandles;
		}
	}
}
