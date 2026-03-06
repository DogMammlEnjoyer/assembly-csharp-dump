using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering
{
	internal class GPUResidentBatcher : IDisposable
	{
		internal RenderersBatchersContext batchersContext
		{
			get
			{
				return this.m_BatchersContext;
			}
		}

		internal OcclusionCullingCommon occlusionCullingCommon
		{
			get
			{
				return this.m_BatchersContext.occlusionCullingCommon;
			}
		}

		internal InstanceCullingBatcher instanceCullingBatcher
		{
			get
			{
				return this.m_InstanceCullingBatcher;
			}
		}

		public GPUResidentBatcher(RenderersBatchersContext batcherContext, InstanceCullingBatcherDesc instanceCullerBatcherDesc, GPUDrivenProcessor gpuDrivenProcessor)
		{
			this.m_BatchersContext = batcherContext;
			this.m_GPUDrivenProcessor = gpuDrivenProcessor;
			this.m_UpdateRendererInstancesAndBatchesCallback = new GPUDrivenRendererDataCallback(this.UpdateRendererInstancesAndBatches);
			this.m_UpdateRendererBatchesCallback = new GPUDrivenRendererDataCallback(this.UpdateRendererBatches);
			this.m_InstanceCullingBatcher = new InstanceCullingBatcher(batcherContext, instanceCullerBatcherDesc, new BatchRendererGroup.OnFinishedCulling(this.OnFinishedCulling));
		}

		public void Dispose()
		{
			this.m_GPUDrivenProcessor.ClearMaterialFilters();
			this.m_InstanceCullingBatcher.Dispose();
			if (this.m_ProcessedThisFrameTreeBits.IsCreated)
			{
				this.m_ProcessedThisFrameTreeBits.Dispose();
			}
		}

		public void OnBeginContextRendering()
		{
			if (this.m_ProcessedThisFrameTreeBits.IsCreated)
			{
				this.m_ProcessedThisFrameTreeBits.Dispose();
			}
		}

		public void OnEndContextRendering()
		{
			InstanceCullingBatcher instanceCullingBatcher = this.m_InstanceCullingBatcher;
			if (instanceCullingBatcher == null)
			{
				return;
			}
			instanceCullingBatcher.OnEndContextRendering();
		}

		public void OnBeginCameraRendering(Camera camera)
		{
			InstanceCullingBatcher instanceCullingBatcher = this.m_InstanceCullingBatcher;
			if (instanceCullingBatcher == null)
			{
				return;
			}
			instanceCullingBatcher.OnBeginCameraRendering(camera);
		}

		public void OnEndCameraRendering(Camera camera)
		{
			InstanceCullingBatcher instanceCullingBatcher = this.m_InstanceCullingBatcher;
			if (instanceCullingBatcher == null)
			{
				return;
			}
			instanceCullingBatcher.OnEndCameraRendering(camera);
		}

		public void UpdateFrame()
		{
			this.m_InstanceCullingBatcher.UpdateFrame();
			this.m_BatchersContext.UpdateFrame();
		}

		public void DestroyMaterials(NativeArray<int> destroyedMaterials)
		{
			this.m_InstanceCullingBatcher.DestroyMaterials(destroyedMaterials);
		}

		public void DestroyDrawInstances(NativeArray<InstanceHandle> instances)
		{
			this.m_InstanceCullingBatcher.DestroyDrawInstances(instances);
		}

		public void DestroyMeshes(NativeArray<int> destroyedMeshes)
		{
			this.m_InstanceCullingBatcher.DestroyMeshes(destroyedMeshes);
		}

		internal void FreeRendererGroupInstances(NativeArray<int> rendererGroupIDs)
		{
			if (rendererGroupIDs.Length == 0)
			{
				return;
			}
			NativeList<InstanceHandle> instances = new NativeList<InstanceHandle>(rendererGroupIDs.Length, Allocator.TempJob);
			this.m_BatchersContext.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances).Complete();
			this.DestroyDrawInstances(instances.AsArray());
			instances.Dispose();
			this.m_BatchersContext.FreeRendererGroupInstances(rendererGroupIDs);
		}

		public void InstanceOcclusionTest(RenderGraph renderGraph, in OcclusionCullingSettings settings, ReadOnlySpan<SubviewOcclusionTest> subviewOcclusionTests)
		{
			if (!this.m_BatchersContext.hasBoundingSpheres)
			{
				return;
			}
			this.m_InstanceCullingBatcher.culler.InstanceOcclusionTest(renderGraph, settings, subviewOcclusionTests, this.m_BatchersContext);
		}

		public void UpdateInstanceOccluders(RenderGraph renderGraph, in OccluderParameters occluderParams, ReadOnlySpan<OccluderSubviewUpdate> occluderSubviewUpdates)
		{
			if (!this.m_BatchersContext.hasBoundingSpheres)
			{
				return;
			}
			this.m_BatchersContext.occlusionCullingCommon.UpdateInstanceOccluders(renderGraph, occluderParams, occluderSubviewUpdates);
		}

		public void UpdateRenderers(NativeArray<int> renderersID, bool materialUpdateOnly = false)
		{
			if (renderersID.Length == 0)
			{
				return;
			}
			this.m_GPUDrivenProcessor.enablePartialRendering = false;
			this.m_GPUDrivenProcessor.EnableGPUDrivenRenderingAndDispatchRendererData(renderersID, materialUpdateOnly ? this.m_UpdateRendererBatchesCallback : this.m_UpdateRendererInstancesAndBatchesCallback, materialUpdateOnly);
			this.m_GPUDrivenProcessor.enablePartialRendering = false;
		}

		public JobHandle SchedulePackedMaterialCacheUpdate(NativeArray<int> materialIDs, NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas)
		{
			return this.m_InstanceCullingBatcher.SchedulePackedMaterialCacheUpdate(materialIDs, packedMaterialDatas);
		}

		public void PostCullBeginCameraRendering(RenderRequestBatcherContext context)
		{
			this.m_InstanceCullingBatcher.PostCullBeginCameraRendering(context);
		}

		public void OnSetupAmbientProbe()
		{
			this.m_BatchersContext.UpdateAmbientProbeAndGpuBuffer(false);
		}

		private void UpdateRendererInstancesAndBatches(in GPUDrivenRendererGroupData rendererData, IList<Mesh> meshes, IList<Material> materials)
		{
			this.FreeRendererGroupInstances(rendererData.invalidRendererGroupID);
			NativeArray<int> rendererGroupID = rendererData.rendererGroupID;
			if (rendererGroupID.Length == 0)
			{
				return;
			}
			NativeArray<Matrix4x4> localToWorldMatrix = rendererData.localToWorldMatrix;
			NativeArray<InstanceHandle> instances = new NativeArray<InstanceHandle>(localToWorldMatrix.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			this.m_BatchersContext.ReallocateAndGetInstances(rendererData, instances);
			JobHandle jobHandle = this.m_BatchersContext.ScheduleUpdateInstanceDataJob(instances, rendererData);
			GPUInstanceDataBufferUploader gpuinstanceDataBufferUploader = this.m_BatchersContext.CreateDataBufferUploader(instances.Length, InstanceType.MeshRenderer);
			gpuinstanceDataBufferUploader.AllocateUploadHandles(instances.Length);
			JobHandle jobHandle2 = default(JobHandle);
			gpuinstanceDataBufferUploader.WriteInstanceDataJob<Vector4>(this.m_BatchersContext.renderersParameters.lightmapScale.index, rendererData.lightmapScaleOffset, rendererData.rendererGroupIndex).Complete();
			this.m_BatchersContext.SubmitToGpu(instances, ref gpuinstanceDataBufferUploader, true);
			this.m_BatchersContext.ChangeInstanceBufferVersion();
			gpuinstanceDataBufferUploader.Dispose();
			jobHandle.Complete();
			this.m_BatchersContext.InitializeInstanceTransforms(instances, rendererData.localToWorldMatrix, rendererData.prevLocalToWorldMatrix);
			this.m_InstanceCullingBatcher.BuildBatch(instances, rendererData, true);
			instances.Dispose();
		}

		private void UpdateRendererBatches(in GPUDrivenRendererGroupData rendererData, IList<Mesh> meshes, IList<Material> materials)
		{
			NativeArray<int> rendererGroupID = rendererData.rendererGroupID;
			if (rendererGroupID.Length == 0)
			{
				return;
			}
			NativeArray<Matrix4x4> localToWorldMatrix = rendererData.localToWorldMatrix;
			NativeList<InstanceHandle> instances = new NativeList<InstanceHandle>(localToWorldMatrix.Length, Allocator.TempJob);
			this.m_BatchersContext.ScheduleQueryRendererGroupInstancesJob(rendererData.rendererGroupID, instances).Complete();
			this.m_InstanceCullingBatcher.BuildBatch(instances.AsArray(), rendererData, false);
			instances.Dispose();
		}

		private void OnFinishedCulling(IntPtr customCullingResult)
		{
			this.ProcessTrees();
			this.m_InstanceCullingBatcher.OnFinishedCulling(customCullingResult);
		}

		private void ProcessTrees()
		{
			if (this.m_BatchersContext.GetAliveInstancesOfType(InstanceType.SpeedTree) == 0)
			{
				return;
			}
			ParallelBitArray compactedVisibilityMasks = this.m_InstanceCullingBatcher.GetCompactedVisibilityMasks(false);
			if (!compactedVisibilityMasks.IsCreated)
			{
				return;
			}
			int length = this.m_BatchersContext.aliveInstances.Length;
			if (!this.m_ProcessedThisFrameTreeBits.IsCreated)
			{
				this.m_ProcessedThisFrameTreeBits = new ParallelBitArray(length, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			}
			else if (this.m_ProcessedThisFrameTreeBits.Length < length)
			{
				this.m_ProcessedThisFrameTreeBits.Resize(length);
			}
			bool becomeVisibleOnly = !Application.isPlaying;
			NativeList<int> visibeTreeRendererIDs = new NativeList<int>(Allocator.TempJob);
			NativeList<InstanceHandle> visibeTreeInstances = new NativeList<InstanceHandle>(Allocator.TempJob);
			int length2;
			this.m_BatchersContext.GetVisibleTreeInstances(compactedVisibilityMasks, this.m_ProcessedThisFrameTreeBits, visibeTreeRendererIDs, visibeTreeInstances, becomeVisibleOnly, out length2);
			if (visibeTreeRendererIDs.Length > 0)
			{
				NativeArray<int> subArray = visibeTreeRendererIDs.AsArray().GetSubArray(0, length2);
				NativeArray<InstanceHandle> subArray2 = visibeTreeInstances.AsArray().GetSubArray(0, length2);
				if (subArray.Length > 0)
				{
					this.UpdateSpeedTreeWindAndUploadWindParamsToGPU(subArray, subArray2, true);
				}
				this.UpdateSpeedTreeWindAndUploadWindParamsToGPU(visibeTreeRendererIDs.AsArray(), visibeTreeInstances.AsArray(), false);
			}
			visibeTreeRendererIDs.Dispose();
			visibeTreeInstances.Dispose();
		}

		private unsafe void UpdateSpeedTreeWindAndUploadWindParamsToGPU(NativeArray<int> treeRendererIDs, NativeArray<InstanceHandle> treeInstances, bool history)
		{
			if (treeRendererIDs.Length == 0)
			{
				return;
			}
			NativeArray<GPUInstanceIndex> gpuInstanceIndices = new NativeArray<GPUInstanceIndex>(treeInstances.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			this.m_BatchersContext.instanceDataBuffer.CPUInstanceArrayToGPUInstanceArray(treeInstances, gpuInstanceIndices);
			if (!history)
			{
				this.m_BatchersContext.UpdateInstanceWindDataHistory(gpuInstanceIndices);
			}
			GPUInstanceDataBufferUploader gpuinstanceDataBufferUploader = this.m_BatchersContext.CreateDataBufferUploader(treeInstances.Length, InstanceType.SpeedTree);
			gpuinstanceDataBufferUploader.AllocateUploadHandles(treeInstances.Length);
			SpeedTreeWindParamsBufferIterator windParams = default(SpeedTreeWindParamsBufferIterator);
			windParams.bufferPtr = gpuinstanceDataBufferUploader.GetUploadBufferPtr();
			for (int i = 0; i < 16; i++)
			{
				*(ref windParams.uintParamOffsets.FixedElementField + (IntPtr)i * 4) = gpuinstanceDataBufferUploader.PrepareParamWrite<Vector4>(this.m_BatchersContext.renderersParameters.windParams[i].index);
			}
			windParams.uintStride = gpuinstanceDataBufferUploader.GetUIntPerInstance();
			windParams.elementOffset = 0;
			windParams.elementsCount = treeInstances.Length;
			SpeedTreeWindManager.UpdateWindAndWriteBufferWindParams(treeRendererIDs, windParams, history);
			this.m_BatchersContext.SubmitToGpu(gpuInstanceIndices, ref gpuinstanceDataBufferUploader, true);
			gpuInstanceIndices.Dispose();
			gpuinstanceDataBufferUploader.Dispose();
		}

		private RenderersBatchersContext m_BatchersContext;

		private GPUDrivenProcessor m_GPUDrivenProcessor;

		private GPUDrivenRendererDataCallback m_UpdateRendererInstancesAndBatchesCallback;

		private GPUDrivenRendererDataCallback m_UpdateRendererBatchesCallback;

		private InstanceCullingBatcher m_InstanceCullingBatcher;

		private ParallelBitArray m_ProcessedThisFrameTreeBits;
	}
}
