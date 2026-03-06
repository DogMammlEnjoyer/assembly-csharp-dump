using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal class InstanceCullingBatcher : IDisposable
	{
		public NativeParallelHashMap<int, BatchMaterialID> batchMaterialHash
		{
			get
			{
				return this.m_BatchMaterialHash;
			}
		}

		public NativeParallelHashMap<int, GPUDrivenPackedMaterialData> packedMaterialHash
		{
			get
			{
				return this.m_PackedMaterialHash;
			}
		}

		public InstanceCullingBatcher(RenderersBatchersContext batcherContext, InstanceCullingBatcherDesc desc, BatchRendererGroup.OnFinishedCulling onFinishedCulling)
		{
			this.m_BatchersContext = batcherContext;
			this.m_DrawInstanceData = new CPUDrawInstanceData();
			this.m_DrawInstanceData.Initialize();
			this.m_BRG = new BatchRendererGroup(new BatchRendererGroupCreateInfo
			{
				cullingCallback = new BatchRendererGroup.OnPerformCulling(this.OnPerformCulling),
				finishedCullingCallback = onFinishedCulling,
				userContext = IntPtr.Zero
			});
			this.m_Culler = default(InstanceCuller);
			this.m_Culler.Init(batcherContext.resources, batcherContext.debugStats);
			this.m_CachedInstanceDataBufferLayoutVersion = -1;
			this.m_OnCompleteCallback = desc.onCompleteCallback;
			this.m_BatchMaterialHash = new NativeParallelHashMap<int, BatchMaterialID>(64, Allocator.Persistent);
			this.m_PackedMaterialHash = new NativeParallelHashMap<int, GPUDrivenPackedMaterialData>(64, Allocator.Persistent);
			this.m_BatchMeshHash = new NativeParallelHashMap<int, BatchMeshID>(64, Allocator.Persistent);
			this.m_GlobalBatchIDs = new NativeParallelHashMap<uint, BatchID>(6, Allocator.Persistent);
			this.m_GlobalBatchIDs.Add(1U, this.GetBatchID(InstanceComponentGroup.Default));
			this.m_GlobalBatchIDs.Add(3U, this.GetBatchID(InstanceComponentGroup.DefaultWind));
			this.m_GlobalBatchIDs.Add(5U, this.GetBatchID(InstanceComponentGroup.DefaultLightProbe));
			this.m_GlobalBatchIDs.Add(9U, this.GetBatchID(InstanceComponentGroup.DefaultLightmap));
			this.m_GlobalBatchIDs.Add(7U, this.GetBatchID(InstanceComponentGroup.DefaultWindLightProbe));
			this.m_GlobalBatchIDs.Add(11U, this.GetBatchID(InstanceComponentGroup.DefaultWindLightmap));
		}

		internal ref InstanceCuller culler
		{
			get
			{
				return ref this.m_Culler;
			}
		}

		public unsafe void Dispose()
		{
			this.m_OnCompleteCallback = null;
			this.m_Culler.Dispose();
			foreach (KeyValue<uint, BatchID> keyValue in this.m_GlobalBatchIDs)
			{
				if (!keyValue.Value.Equals(BatchID.Null))
				{
					this.m_BRG.RemoveBatch(*keyValue.Value);
				}
			}
			this.m_GlobalBatchIDs.Dispose();
			if (this.m_BRG != null)
			{
				this.m_BRG.Dispose();
			}
			this.m_DrawInstanceData.Dispose();
			this.m_DrawInstanceData = null;
			this.m_BatchMaterialHash.Dispose();
			this.m_PackedMaterialHash.Dispose();
			this.m_BatchMeshHash.Dispose();
		}

		private BatchID GetBatchID(InstanceComponentGroup componentsOverriden)
		{
			if (this.m_CachedInstanceDataBufferLayoutVersion != this.m_BatchersContext.instanceDataBufferLayoutVersion)
			{
				return BatchID.Null;
			}
			NativeList<MetadataValue> nativeList = new NativeList<MetadataValue>(this.m_BatchersContext.defaultMetadata.Length, Allocator.Temp);
			for (int i = 0; i < this.m_BatchersContext.defaultDescriptions.Length; i++)
			{
				InstanceComponentGroup componentGroup = this.m_BatchersContext.defaultDescriptions[i].componentGroup;
				MetadataValue metadataValue = this.m_BatchersContext.defaultMetadata[i];
				uint num = metadataValue.Value;
				if ((componentsOverriden & componentGroup) == (InstanceComponentGroup)0U)
				{
					num &= 1342177279U;
				}
				MetadataValue metadataValue2 = default(MetadataValue);
				metadataValue2.NameID = metadataValue.NameID;
				metadataValue2.Value = num;
				nativeList.Add(metadataValue2);
			}
			return this.m_BRG.AddBatch(nativeList.AsArray(), this.m_BatchersContext.gpuInstanceDataBuffer.bufferHandle);
		}

		private unsafe void UpdateInstanceDataBufferLayoutVersion()
		{
			if (this.m_CachedInstanceDataBufferLayoutVersion != this.m_BatchersContext.instanceDataBufferLayoutVersion)
			{
				this.m_CachedInstanceDataBufferLayoutVersion = this.m_BatchersContext.instanceDataBufferLayoutVersion;
				foreach (KeyValue<uint, BatchID> keyValue in this.m_GlobalBatchIDs)
				{
					BatchID batchID = *keyValue.Value;
					if (!batchID.Equals(BatchID.Null))
					{
						this.m_BRG.RemoveBatch(batchID);
					}
					InstanceComponentGroup key = (InstanceComponentGroup)keyValue.Key;
					*keyValue.Value = this.GetBatchID(key);
				}
			}
		}

		public CPUDrawInstanceData GetDrawInstanceData()
		{
			return this.m_DrawInstanceData;
		}

		public JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cc, BatchCullingOutput cullingOutput, IntPtr userContext)
		{
			foreach (KeyValue<uint, BatchID> keyValue in this.m_GlobalBatchIDs)
			{
				if (keyValue.Value.Equals(BatchID.Null))
				{
					return default(JobHandle);
				}
			}
			this.m_DrawInstanceData.RebuildDrawListsIfNeeded();
			bool hasBoundingSpheres = this.m_BatchersContext.hasBoundingSpheres;
			BatchCullingOutput cullingOutput2 = cullingOutput;
			CPUInstanceData.ReadOnly instanceData = this.m_BatchersContext.instanceData;
			CPUSharedInstanceData.ReadOnly sharedInstanceData = this.m_BatchersContext.sharedInstanceData;
			CPUPerCameraInstanceData perCameraInstanceData = this.m_BatchersContext.perCameraInstanceData;
			GPUInstanceDataBuffer.ReadOnly instanceDataBuffer = this.m_BatchersContext.instanceDataBuffer;
			JobHandle jobHandle = this.m_Culler.CreateCullJobTree(cc, cullingOutput2, instanceData, sharedInstanceData, perCameraInstanceData, instanceDataBuffer, this.m_BatchersContext.lodGroupCullingData, this.m_DrawInstanceData, this.m_GlobalBatchIDs, this.m_BatchersContext.smallMeshScreenPercentage, hasBoundingSpheres ? this.m_BatchersContext.occlusionCullingCommon : null);
			if (this.m_OnCompleteCallback != null)
			{
				this.m_OnCompleteCallback(jobHandle, cc, cullingOutput);
			}
			return jobHandle;
		}

		public void OnFinishedCulling(IntPtr customCullingResult)
		{
			int viewInstanceID = (int)customCullingResult;
			this.m_Culler.EnsureValidOcclusionTestResults(viewInstanceID);
		}

		public void DestroyDrawInstances(NativeArray<InstanceHandle> instances)
		{
			if (instances.Length == 0)
			{
				return;
			}
			this.m_DrawInstanceData.DestroyDrawInstances(instances);
		}

		public void DestroyMaterials(NativeArray<int> destroyedMaterials)
		{
			if (destroyedMaterials.Length == 0)
			{
				return;
			}
			NativeList<uint> nativeList = new NativeList<uint>(destroyedMaterials.Length, Allocator.TempJob);
			foreach (int key in destroyedMaterials)
			{
				BatchMaterialID material;
				if (this.m_BatchMaterialHash.TryGetValue(key, out material))
				{
					nativeList.Add(material.value);
					this.m_BatchMaterialHash.Remove(key);
					this.m_PackedMaterialHash.Remove(key);
					this.m_BRG.UnregisterMaterial(material);
				}
			}
			this.m_DrawInstanceData.DestroyMaterialDrawInstances(nativeList.AsArray());
			nativeList.Dispose();
		}

		public void DestroyMeshes(NativeArray<int> destroyedMeshes)
		{
			if (destroyedMeshes.Length == 0)
			{
				return;
			}
			foreach (int key in destroyedMeshes)
			{
				BatchMeshID mesh;
				if (this.m_BatchMeshHash.TryGetValue(key, out mesh))
				{
					this.m_BatchMeshHash.Remove(key);
					this.m_BRG.UnregisterMesh(mesh);
				}
			}
		}

		public void PostCullBeginCameraRendering(RenderRequestBatcherContext context)
		{
		}

		private void RegisterBatchMeshes(NativeArray<int> meshIDs)
		{
			NativeList<int> nativeList = new NativeList<int>(meshIDs.Length, Allocator.TempJob);
			new FindNonRegisteredMeshesJob
			{
				instanceIDs = meshIDs,
				hashMap = this.m_BatchMeshHash,
				outInstancesWriter = nativeList.AsParallelWriter()
			}.ScheduleBatch(meshIDs.Length, 128, default(JobHandle)).Complete();
			NativeArray<BatchMeshID> batchIDs = new NativeArray<BatchMeshID>(nativeList.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BatchRendererGroup brg = this.m_BRG;
			NativeArray<int> nativeArray = nativeList.AsArray();
			brg.RegisterMeshes(nativeArray, batchIDs);
			int num = this.m_BatchMeshHash.Count() + batchIDs.Length;
			this.m_BatchMeshHash.Capacity = Math.Max(this.m_BatchMeshHash.Capacity, Mathf.CeilToInt((float)num / 1023f) * 1024);
			new RegisterNewMeshesJob
			{
				instanceIDs = nativeList.AsArray(),
				batchIDs = batchIDs,
				hashMap = this.m_BatchMeshHash.AsParallelWriter()
			}.Schedule(nativeList.Length, 128, default(JobHandle)).Complete();
			nativeList.Dispose();
			batchIDs.Dispose();
		}

		private void RegisterBatchMaterials(in NativeArray<int> usedMaterialIDs, in NativeArray<GPUDrivenPackedMaterialData> usedPackedMaterialDatas)
		{
			NativeArray<int> nativeArray = usedMaterialIDs;
			NativeList<int> nativeList = new NativeList<int>(nativeArray.Length, Allocator.TempJob);
			nativeArray = usedMaterialIDs;
			NativeList<GPUDrivenPackedMaterialData> nativeList2 = new NativeList<GPUDrivenPackedMaterialData>(nativeArray.Length, Allocator.TempJob);
			FindNonRegisteredMaterialsJob jobData = new FindNonRegisteredMaterialsJob
			{
				instanceIDs = usedMaterialIDs,
				packedMaterialDatas = usedPackedMaterialDatas,
				hashMap = this.m_BatchMaterialHash,
				outInstancesWriter = nativeList.AsParallelWriter(),
				outPackedMaterialDatasWriter = nativeList2.AsParallelWriter()
			};
			nativeArray = usedMaterialIDs;
			jobData.ScheduleBatch(nativeArray.Length, 128, default(JobHandle)).Complete();
			NativeArray<BatchMaterialID> batchIDs = new NativeArray<BatchMaterialID>(nativeList.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			BatchRendererGroup brg = this.m_BRG;
			nativeArray = nativeList.AsArray();
			brg.RegisterMaterials(nativeArray, batchIDs);
			int num = this.m_BatchMaterialHash.Count() + nativeList.Length;
			this.m_BatchMaterialHash.Capacity = Math.Max(this.m_BatchMaterialHash.Capacity, Mathf.CeilToInt((float)num / 1023f) * 1024);
			this.m_PackedMaterialHash.Capacity = this.m_BatchMaterialHash.Capacity;
			new RegisterNewMaterialsJob
			{
				instanceIDs = nativeList.AsArray(),
				packedMaterialDatas = nativeList2.AsArray(),
				batchIDs = batchIDs,
				batchMaterialHashMap = this.m_BatchMaterialHash.AsParallelWriter(),
				packedMaterialHashMap = this.m_PackedMaterialHash.AsParallelWriter()
			}.Schedule(nativeList.Length, 128, default(JobHandle)).Complete();
			nativeList.Dispose();
			nativeList2.Dispose();
			batchIDs.Dispose();
		}

		public JobHandle SchedulePackedMaterialCacheUpdate(NativeArray<int> materialIDs, NativeArray<GPUDrivenPackedMaterialData> packedMaterialDatas)
		{
			return new UpdatePackedMaterialDataCacheJob
			{
				materialIDs = materialIDs.AsReadOnly(),
				packedMaterialDatas = packedMaterialDatas.AsReadOnly(),
				packedMaterialHash = this.m_PackedMaterialHash
			}.Schedule(default(JobHandle));
		}

		public void BuildBatch(NativeArray<InstanceHandle> instances, in GPUDrivenRendererGroupData rendererData, bool registerMaterialsAndMeshes)
		{
			if (registerMaterialsAndMeshes)
			{
				this.RegisterBatchMaterials(rendererData.materialID, rendererData.packedMaterialData);
				this.RegisterBatchMeshes(rendererData.meshID);
			}
			NativeParallelHashMap<RangeKey, int> rangeHash = this.m_DrawInstanceData.rangeHash;
			NativeList<DrawRange> drawRanges = this.m_DrawInstanceData.drawRanges;
			NativeParallelHashMap<DrawKey, int> batchHash = this.m_DrawInstanceData.batchHash;
			NativeList<DrawBatch> drawBatches = this.m_DrawInstanceData.drawBatches;
			NativeList<DrawInstance> drawInstances = this.m_DrawInstanceData.drawInstances;
			NativeArray<int> instancesCount = rendererData.instancesCount;
			InstanceCullingBatcherBurst.CreateDrawBatches(instancesCount.Length == 0, instances, rendererData, this.m_BatchMeshHash, this.m_BatchMaterialHash, this.m_PackedMaterialHash, ref rangeHash, ref drawRanges, ref batchHash, ref drawBatches, ref drawInstances);
			this.m_DrawInstanceData.NeedsRebuild();
			this.UpdateInstanceDataBufferLayoutVersion();
		}

		public void InstanceOccludersUpdated(int viewInstanceID, int subviewMask)
		{
			this.m_Culler.InstanceOccludersUpdated(viewInstanceID, subviewMask, this.m_BatchersContext);
		}

		public void UpdateFrame()
		{
			this.m_Culler.UpdateFrame(this.m_BatchersContext.cameraCount);
		}

		public ParallelBitArray GetCompactedVisibilityMasks(bool syncCullingJobs)
		{
			return this.m_Culler.GetCompactedVisibilityMasks(syncCullingJobs);
		}

		public void OnEndContextRendering()
		{
			ParallelBitArray compactedVisibilityMasks = this.GetCompactedVisibilityMasks(true);
			if (compactedVisibilityMasks.IsCreated)
			{
				this.m_BatchersContext.UpdatePerFrameInstanceVisibility(compactedVisibilityMasks);
			}
		}

		public void OnBeginCameraRendering(Camera camera)
		{
			this.m_Culler.OnBeginCameraRendering(camera);
		}

		public void OnEndCameraRendering(Camera camera)
		{
			this.m_Culler.OnEndCameraRendering(camera);
		}

		private RenderersBatchersContext m_BatchersContext;

		private CPUDrawInstanceData m_DrawInstanceData;

		private BatchRendererGroup m_BRG;

		private NativeParallelHashMap<uint, BatchID> m_GlobalBatchIDs;

		private InstanceCuller m_Culler;

		private NativeParallelHashMap<int, BatchMaterialID> m_BatchMaterialHash;

		private NativeParallelHashMap<int, GPUDrivenPackedMaterialData> m_PackedMaterialHash;

		private NativeParallelHashMap<int, BatchMeshID> m_BatchMeshHash;

		private int m_CachedInstanceDataBufferLayoutVersion;

		private OnCullingCompleteCallback m_OnCompleteCallback;
	}
}
