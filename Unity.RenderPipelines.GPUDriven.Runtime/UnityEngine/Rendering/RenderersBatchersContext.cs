using System;
using Unity.Collections;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal class RenderersBatchersContext : IDisposable
	{
		public RenderersParameters renderersParameters
		{
			get
			{
				return this.m_RenderersParameters;
			}
		}

		public GraphicsBuffer gpuInstanceDataBuffer
		{
			get
			{
				return this.m_InstanceDataBuffer.gpuBuffer;
			}
		}

		public int activeLodGroupCount
		{
			get
			{
				return this.m_LODGroupDataPool.activeLodGroupCount;
			}
		}

		public NativeArray<GPUInstanceComponentDesc>.ReadOnly defaultDescriptions
		{
			get
			{
				return this.m_InstanceDataBuffer.descriptions.AsReadOnly();
			}
		}

		public NativeArray<MetadataValue> defaultMetadata
		{
			get
			{
				return this.m_InstanceDataBuffer.defaultMetadata;
			}
		}

		public NativeList<LODGroupCullingData> lodGroupCullingData
		{
			get
			{
				return this.m_LODGroupDataPool.lodGroupCullingData;
			}
		}

		public int instanceDataBufferVersion
		{
			get
			{
				return this.m_InstanceDataBuffer.version;
			}
		}

		public int instanceDataBufferLayoutVersion
		{
			get
			{
				return this.m_InstanceDataBuffer.layoutVersion;
			}
		}

		public SphericalHarmonicsL2 cachedAmbientProbe
		{
			get
			{
				return this.m_CachedAmbientProbe;
			}
		}

		public bool hasBoundingSpheres
		{
			get
			{
				return this.m_InstanceDataSystem.hasBoundingSpheres;
			}
		}

		public int cameraCount
		{
			get
			{
				return this.m_InstanceDataSystem.cameraCount;
			}
		}

		public CPUInstanceData.ReadOnly instanceData
		{
			get
			{
				return this.m_InstanceDataSystem.instanceData;
			}
		}

		public CPUSharedInstanceData.ReadOnly sharedInstanceData
		{
			get
			{
				return this.m_InstanceDataSystem.sharedInstanceData;
			}
		}

		public CPUPerCameraInstanceData perCameraInstanceData
		{
			get
			{
				return this.m_InstanceDataSystem.perCameraInstanceData;
			}
		}

		public GPUInstanceDataBuffer.ReadOnly instanceDataBuffer
		{
			get
			{
				return this.m_InstanceDataBuffer.AsReadOnly();
			}
		}

		public NativeArray<InstanceHandle> aliveInstances
		{
			get
			{
				return this.m_InstanceDataSystem.aliveInstances;
			}
		}

		public float smallMeshScreenPercentage
		{
			get
			{
				return this.m_SmallMeshScreenPercentage;
			}
		}

		public GPUResidentDrawerResources resources
		{
			get
			{
				return this.m_Resources;
			}
		}

		internal OcclusionCullingCommon occlusionCullingCommon
		{
			get
			{
				return this.m_OcclusionCullingCommon;
			}
		}

		internal DebugRendererBatcherStats debugStats
		{
			get
			{
				return this.m_DebugStats;
			}
		}

		public RenderersBatchersContext(in RenderersBatchersContextDesc desc, GPUDrivenProcessor gpuDrivenProcessor, GPUResidentDrawerResources resources)
		{
			this.m_Resources = resources;
			this.m_GPUDrivenProcessor = gpuDrivenProcessor;
			RenderersParameters.Flags flags = RenderersParameters.Flags.None;
			if (desc.enableBoundingSpheresInstanceData)
			{
				flags |= RenderersParameters.Flags.UseBoundingSphereParameter;
			}
			this.m_InstanceDataBuffer = RenderersParameters.CreateInstanceDataBuffer(flags, desc.instanceNumInfo);
			this.m_RenderersParameters = new RenderersParameters(ref this.m_InstanceDataBuffer);
			InstanceNumInfo instanceNumInfo = desc.instanceNumInfo;
			this.m_LODGroupDataPool = new LODGroupDataPool(resources, instanceNumInfo.GetInstanceNum(InstanceType.MeshRenderer), desc.supportDitheringCrossFade);
			this.m_UploadResources = default(GPUInstanceDataBufferUploader.GPUResources);
			this.m_UploadResources.LoadShaders(resources);
			this.m_GrowerResources = default(GPUInstanceDataBufferGrower.GPUResources);
			this.m_GrowerResources.LoadShaders(resources);
			this.m_CmdBuffer = new CommandBuffer();
			this.m_CmdBuffer.name = "GPUCullingCommands";
			this.m_CachedAmbientProbe = RenderSettings.ambientProbe;
			instanceNumInfo = desc.instanceNumInfo;
			this.m_InstanceDataSystem = new InstanceDataSystem(instanceNumInfo.GetTotalInstanceNum(), desc.enableBoundingSpheresInstanceData, resources);
			this.m_SmallMeshScreenPercentage = desc.smallMeshScreenPercentage;
			this.m_UpdateLODGroupCallback = new GPUDrivenLODGroupDataCallback(this.UpdateLODGroupData);
			this.m_TransformLODGroupCallback = new GPUDrivenLODGroupDataCallback(this.TransformLODGroupData);
			this.m_OcclusionCullingCommon = new OcclusionCullingCommon();
			this.m_OcclusionCullingCommon.Init(resources);
			this.m_DebugStats = (desc.enableCullerDebugStats ? new DebugRendererBatcherStats() : null);
		}

		public void Dispose()
		{
			NativeArray<int>.ReadOnly rendererGroupIDs = this.m_InstanceDataSystem.sharedInstanceData.rendererGroupIDs;
			if (rendererGroupIDs.Length > 0)
			{
				this.m_GPUDrivenProcessor.DisableGPUDrivenRendering(rendererGroupIDs);
			}
			this.m_InstanceDataSystem.Dispose();
			this.m_CmdBuffer.Release();
			this.m_GrowerResources.Dispose();
			this.m_UploadResources.Dispose();
			this.m_LODGroupDataPool.Dispose();
			this.m_InstanceDataBuffer.Dispose();
			this.m_UpdateLODGroupCallback = null;
			this.m_TransformLODGroupCallback = null;
			DebugRendererBatcherStats debugStats = this.m_DebugStats;
			if (debugStats != null)
			{
				debugStats.Dispose();
			}
			this.m_DebugStats = null;
			OcclusionCullingCommon occlusionCullingCommon = this.m_OcclusionCullingCommon;
			if (occlusionCullingCommon != null)
			{
				occlusionCullingCommon.Dispose();
			}
			this.m_OcclusionCullingCommon = null;
		}

		public int GetMaxInstancesOfType(InstanceType instanceType)
		{
			return this.m_InstanceDataSystem.GetMaxInstancesOfType(instanceType);
		}

		public int GetAliveInstancesOfType(InstanceType instanceType)
		{
			return this.m_InstanceDataSystem.GetAliveInstancesOfType(instanceType);
		}

		public void GrowInstanceBuffer(in InstanceNumInfo instanceNumInfo)
		{
			using (GPUInstanceDataBufferGrower gpuinstanceDataBufferGrower = new GPUInstanceDataBufferGrower(this.m_InstanceDataBuffer, ref instanceNumInfo))
			{
				GPUInstanceDataBuffer gpuinstanceDataBuffer = gpuinstanceDataBufferGrower.SubmitToGpu(ref this.m_GrowerResources);
				if (gpuinstanceDataBuffer != this.m_InstanceDataBuffer)
				{
					if (this.m_InstanceDataBuffer != null)
					{
						this.m_InstanceDataBuffer.Dispose();
					}
					this.m_InstanceDataBuffer = gpuinstanceDataBuffer;
				}
			}
			this.m_RenderersParameters = new RenderersParameters(ref this.m_InstanceDataBuffer);
		}

		private void EnsureInstanceBufferCapacity()
		{
			int maxInstancesOfType = this.m_InstanceDataSystem.GetMaxInstancesOfType(InstanceType.MeshRenderer);
			int maxInstancesOfType2 = this.m_InstanceDataSystem.GetMaxInstancesOfType(InstanceType.SpeedTree);
			int num = this.m_InstanceDataBuffer.instanceNumInfo.GetInstanceNum(InstanceType.MeshRenderer);
			int num2 = this.m_InstanceDataBuffer.instanceNumInfo.GetInstanceNum(InstanceType.SpeedTree);
			bool flag = false;
			if (maxInstancesOfType > num)
			{
				flag = true;
				num = maxInstancesOfType + 1024;
			}
			if (maxInstancesOfType2 > num2)
			{
				flag = true;
				num2 = maxInstancesOfType2 + 256;
			}
			if (flag)
			{
				InstanceNumInfo instanceNumInfo = new InstanceNumInfo(num, num2);
				this.GrowInstanceBuffer(instanceNumInfo);
			}
		}

		private void UpdateLODGroupData(in GPUDrivenLODGroupData lodGroupData)
		{
			this.m_LODGroupDataPool.UpdateLODGroupData(lodGroupData);
		}

		private void TransformLODGroupData(in GPUDrivenLODGroupData lodGroupData)
		{
			this.m_LODGroupDataPool.UpdateLODGroupTransformData(lodGroupData);
		}

		public void DestroyLODGroups(NativeArray<int> destroyed)
		{
			if (destroyed.Length == 0)
			{
				return;
			}
			this.m_LODGroupDataPool.FreeLODGroupData(destroyed);
		}

		public void UpdateLODGroups(NativeArray<int> changedID)
		{
			if (changedID.Length == 0)
			{
				return;
			}
			this.m_GPUDrivenProcessor.DispatchLODGroupData(changedID, this.m_UpdateLODGroupCallback);
		}

		public void ReallocateAndGetInstances(in GPUDrivenRendererGroupData rendererData, NativeArray<InstanceHandle> instances)
		{
			this.m_InstanceDataSystem.ReallocateAndGetInstances(rendererData, instances);
			this.EnsureInstanceBufferCapacity();
		}

		public JobHandle ScheduleUpdateInstanceDataJob(NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData)
		{
			return this.m_InstanceDataSystem.ScheduleUpdateInstanceDataJob(instances, rendererData, this.m_LODGroupDataPool.lodGroupDataHash);
		}

		public void FreeRendererGroupInstances(NativeArray<int> rendererGroupsID)
		{
			this.m_InstanceDataSystem.FreeRendererGroupInstances(rendererGroupsID);
		}

		public void FreeInstances(NativeArray<InstanceHandle> instances)
		{
			this.m_InstanceDataSystem.FreeInstances(instances);
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<InstanceHandle> instances)
		{
			return this.m_InstanceDataSystem.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances);
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeList<InstanceHandle> instances)
		{
			return this.m_InstanceDataSystem.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances);
		}

		public JobHandle ScheduleQueryRendererGroupInstancesJob(NativeArray<int> rendererGroupIDs, NativeArray<int> instancesOffset, NativeArray<int> instancesCount, NativeList<InstanceHandle> instances)
		{
			return this.m_InstanceDataSystem.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instancesOffset, instancesCount, instances);
		}

		public JobHandle ScheduleQueryMeshInstancesJob(NativeArray<int> sortedMeshIDs, NativeList<InstanceHandle> instances)
		{
			return this.m_InstanceDataSystem.ScheduleQuerySortedMeshInstancesJob(sortedMeshIDs, instances);
		}

		public void ChangeInstanceBufferVersion()
		{
			this.m_InstanceDataBuffer.version++;
		}

		public GPUInstanceDataBufferUploader CreateDataBufferUploader(int capacity, InstanceType instanceType)
		{
			return new GPUInstanceDataBufferUploader(ref this.m_InstanceDataBuffer.descriptions, capacity, instanceType);
		}

		public void SubmitToGpu(NativeArray<InstanceHandle> instances, ref GPUInstanceDataBufferUploader uploader, bool submitOnlyWrittenParams)
		{
			uploader.SubmitToGpu(this.m_InstanceDataBuffer, instances, ref this.m_UploadResources, submitOnlyWrittenParams);
		}

		public void SubmitToGpu(NativeArray<GPUInstanceIndex> gpuInstanceIndices, ref GPUInstanceDataBufferUploader uploader, bool submitOnlyWrittenParams)
		{
			uploader.SubmitToGpu(this.m_InstanceDataBuffer, gpuInstanceIndices, ref this.m_UploadResources, submitOnlyWrittenParams);
		}

		public void InitializeInstanceTransforms(NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices, NativeArray<Matrix4x4> prevLocalToWorldMatrices)
		{
			if (instances.Length == 0)
			{
				return;
			}
			this.m_InstanceDataSystem.InitializeInstanceTransforms(instances, localToWorldMatrices, prevLocalToWorldMatrices, this.m_RenderersParameters, this.m_InstanceDataBuffer);
			this.ChangeInstanceBufferVersion();
		}

		public void UpdateInstanceTransforms(NativeArray<InstanceHandle> instances, NativeArray<Matrix4x4> localToWorldMatrices)
		{
			if (instances.Length == 0)
			{
				return;
			}
			this.m_InstanceDataSystem.UpdateInstanceTransforms(instances, localToWorldMatrices, this.m_RenderersParameters, this.m_InstanceDataBuffer);
			this.ChangeInstanceBufferVersion();
		}

		public void UpdateAmbientProbeAndGpuBuffer(bool forceUpdate)
		{
			if (forceUpdate || this.m_CachedAmbientProbe != RenderSettings.ambientProbe)
			{
				this.m_CachedAmbientProbe = RenderSettings.ambientProbe;
				this.m_InstanceDataSystem.UpdateAllInstanceProbes(this.m_RenderersParameters, this.m_InstanceDataBuffer);
				this.ChangeInstanceBufferVersion();
			}
		}

		public void UpdateInstanceWindDataHistory(NativeArray<GPUInstanceIndex> gpuInstanceIndices)
		{
			if (gpuInstanceIndices.Length == 0)
			{
				return;
			}
			this.m_InstanceDataSystem.UpdateInstanceWindDataHistory(gpuInstanceIndices, this.m_RenderersParameters, this.m_InstanceDataBuffer);
			this.ChangeInstanceBufferVersion();
		}

		public void UpdateInstanceMotions()
		{
			this.m_InstanceDataSystem.UpdateInstanceMotions(this.m_RenderersParameters, this.m_InstanceDataBuffer);
			this.ChangeInstanceBufferVersion();
		}

		public void TransformLODGroups(NativeArray<int> lodGroupsID)
		{
			if (lodGroupsID.Length == 0)
			{
				return;
			}
			this.m_GPUDrivenProcessor.DispatchLODGroupData(lodGroupsID, this.m_TransformLODGroupCallback);
		}

		public void UpdatePerFrameInstanceVisibility(in ParallelBitArray compactedVisibilityMasks)
		{
			this.m_InstanceDataSystem.UpdatePerFrameInstanceVisibility(compactedVisibilityMasks);
		}

		public JobHandle ScheduleCollectInstancesLODGroupAndMasksJob(NativeArray<InstanceHandle> instances, NativeArray<uint> lodGroupAndMasks)
		{
			return this.m_InstanceDataSystem.ScheduleCollectInstancesLODGroupAndMasksJob(instances, lodGroupAndMasks);
		}

		public InstanceHandle GetRendererInstanceHandle(int rendererID)
		{
			NativeArray<int> rendererGroupIDs = new NativeArray<int>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			NativeArray<InstanceHandle> instances = new NativeArray<InstanceHandle>(1, Allocator.TempJob, NativeArrayOptions.ClearMemory);
			rendererGroupIDs[0] = rendererID;
			this.m_InstanceDataSystem.ScheduleQueryRendererGroupInstancesJob(rendererGroupIDs, instances).Complete();
			InstanceHandle result = instances[0];
			rendererGroupIDs.Dispose();
			instances.Dispose();
			return result;
		}

		public void GetVisibleTreeInstances(in ParallelBitArray compactedVisibilityMasks, in ParallelBitArray processedBits, NativeList<int> visibeTreeRendererIDs, NativeList<InstanceHandle> visibeTreeInstances, bool becomeVisibleOnly, out int becomeVisibeTreeInstancesCount)
		{
			this.m_InstanceDataSystem.GetVisibleTreeInstances(compactedVisibilityMasks, processedBits, visibeTreeRendererIDs, visibeTreeInstances, becomeVisibleOnly, out becomeVisibeTreeInstancesCount);
		}

		public GPUInstanceDataBuffer GetInstanceDataBuffer()
		{
			return this.m_InstanceDataBuffer;
		}

		public void UpdateFrame()
		{
			this.m_OcclusionCullingCommon.UpdateFrame();
			if (this.m_DebugStats != null)
			{
				this.m_OcclusionCullingCommon.UpdateOccluderStats(this.m_DebugStats);
			}
		}

		public void FreePerCameraInstanceData(NativeArray<int> cameraIDs)
		{
			this.m_InstanceDataSystem.DeallocatePerCameraInstanceData(cameraIDs);
		}

		public void UpdateCameras(NativeArray<int> cameraIDs)
		{
			this.m_InstanceDataSystem.AllocatePerCameraInstanceData(cameraIDs);
		}

		private InstanceDataSystem m_InstanceDataSystem;

		private GPUResidentDrawerResources m_Resources;

		private GPUDrivenProcessor m_GPUDrivenProcessor;

		private LODGroupDataPool m_LODGroupDataPool;

		internal GPUInstanceDataBuffer m_InstanceDataBuffer;

		private RenderersParameters m_RenderersParameters;

		private GPUInstanceDataBufferUploader.GPUResources m_UploadResources;

		private GPUInstanceDataBufferGrower.GPUResources m_GrowerResources;

		internal CommandBuffer m_CmdBuffer;

		private SphericalHarmonicsL2 m_CachedAmbientProbe;

		private float m_SmallMeshScreenPercentage;

		private GPUDrivenLODGroupDataCallback m_UpdateLODGroupCallback;

		private GPUDrivenLODGroupDataCallback m_TransformLODGroupCallback;

		private OcclusionCullingCommon m_OcclusionCullingCommon;

		private DebugRendererBatcherStats m_DebugStats;
	}
}
