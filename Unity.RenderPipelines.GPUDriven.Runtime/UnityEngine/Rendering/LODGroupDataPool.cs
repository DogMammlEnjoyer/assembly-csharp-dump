using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace UnityEngine.Rendering
{
	internal class LODGroupDataPool : IDisposable
	{
		public NativeParallelHashMap<int, GPUInstanceIndex> lodGroupDataHash
		{
			get
			{
				return this.m_LODGroupDataHash;
			}
		}

		public NativeList<LODGroupCullingData> lodGroupCullingData
		{
			get
			{
				return this.m_LODGroupCullingData;
			}
		}

		public int crossfadedRendererCount
		{
			get
			{
				return this.m_CrossfadedRendererCount;
			}
		}

		public int activeLodGroupCount
		{
			get
			{
				return this.m_LODGroupData.Length;
			}
		}

		public LODGroupDataPool(GPUResidentDrawerResources resources, int initialInstanceCount, bool supportDitheringCrossFade)
		{
			this.m_LODGroupData = new NativeList<LODGroupData>(Allocator.Persistent);
			this.m_LODGroupDataHash = new NativeParallelHashMap<int, GPUInstanceIndex>(64, Allocator.Persistent);
			this.m_LODGroupCullingData = new NativeList<LODGroupCullingData>(Allocator.Persistent);
			this.m_FreeLODGroupDataHandles = new NativeList<GPUInstanceIndex>(Allocator.Persistent);
			this.m_SupportDitheringCrossFade = supportDitheringCrossFade;
		}

		public void Dispose()
		{
			this.m_LODGroupData.Dispose();
			this.m_LODGroupDataHash.Dispose();
			this.m_LODGroupCullingData.Dispose();
			this.m_FreeLODGroupDataHandles.Dispose();
		}

		public unsafe void UpdateLODGroupTransformData(in GPUDrivenLODGroupData inputData)
		{
			NativeArray<int> lodGroupID = inputData.lodGroupID;
			int length = lodGroupID.Length;
			int num = 0;
			UpdateLODGroupTransformJob jobData = new UpdateLODGroupTransformJob
			{
				lodGroupDataHash = this.m_LODGroupDataHash,
				lodGroupIDs = inputData.lodGroupID,
				worldSpaceReferencePoints = inputData.worldSpaceReferencePoint,
				worldSpaceSizes = inputData.worldSpaceSize,
				lodGroupData = this.m_LODGroupData,
				lodGroupCullingData = this.m_LODGroupCullingData,
				supportDitheringCrossFade = this.m_SupportDitheringCrossFade,
				atomicUpdateCount = new UnsafeAtomicCounter32((void*)(&num))
			};
			if (length >= 256)
			{
				jobData.Schedule(length, 256, default(JobHandle)).Complete();
				return;
			}
			jobData.Run(length);
		}

		public unsafe void UpdateLODGroupData(in GPUDrivenLODGroupData inputData)
		{
			this.FreeLODGroupData(inputData.invalidLODGroupID);
			NativeArray<int> lodGroupID = inputData.lodGroupID;
			NativeArray<GPUInstanceIndex> lodGroupInstances = new NativeArray<GPUInstanceIndex>(lodGroupID.Length, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			int num = LODGroupDataPoolBurst.AllocateOrGetLODGroupDataInstances(inputData.lodGroupID, ref this.m_LODGroupData, ref this.m_LODGroupCullingData, ref this.m_LODGroupDataHash, ref this.m_FreeLODGroupDataHandles, ref lodGroupInstances);
			this.m_CrossfadedRendererCount -= num;
			int num2 = 0;
			UpdateLODGroupDataJob jobData = new UpdateLODGroupDataJob
			{
				lodGroupInstances = lodGroupInstances,
				inputData = inputData,
				supportDitheringCrossFade = this.m_SupportDitheringCrossFade,
				lodGroupsData = this.m_LODGroupData.AsArray(),
				lodGroupsCullingData = this.m_LODGroupCullingData.AsArray(),
				rendererCount = new UnsafeAtomicCounter32((void*)(&num2))
			};
			if (lodGroupInstances.Length >= 256)
			{
				jobData.Schedule(lodGroupInstances.Length, 256, default(JobHandle)).Complete();
			}
			else
			{
				jobData.Run(lodGroupInstances.Length);
			}
			this.m_CrossfadedRendererCount += num2;
			lodGroupInstances.Dispose();
		}

		public void FreeLODGroupData(NativeArray<int> destroyedLODGroupsID)
		{
			if (destroyedLODGroupsID.Length == 0)
			{
				return;
			}
			int num = LODGroupDataPoolBurst.FreeLODGroupData(destroyedLODGroupsID, ref this.m_LODGroupData, ref this.m_LODGroupDataHash, ref this.m_FreeLODGroupDataHandles);
			this.m_CrossfadedRendererCount -= num;
		}

		private NativeList<LODGroupData> m_LODGroupData;

		private NativeParallelHashMap<int, GPUInstanceIndex> m_LODGroupDataHash;

		private NativeList<LODGroupCullingData> m_LODGroupCullingData;

		private NativeList<GPUInstanceIndex> m_FreeLODGroupDataHandles;

		private int m_CrossfadedRendererCount;

		private bool m_SupportDitheringCrossFade;

		private static class LodGroupShaderIDs
		{
			public static readonly int _SupportDitheringCrossFade = Shader.PropertyToID("_SupportDitheringCrossFade");

			public static readonly int _LodGroupCullingDataGPUByteSize = Shader.PropertyToID("_LodGroupCullingDataGPUByteSize");

			public static readonly int _LodGroupCullingDataStartOffset = Shader.PropertyToID("_LodGroupCullingDataStartOffset");

			public static readonly int _LodCullingDataQueueCount = Shader.PropertyToID("_LodCullingDataQueueCount");

			public static readonly int _InputLodCullingDataIndices = Shader.PropertyToID("_InputLodCullingDataIndices");

			public static readonly int _InputLodCullingDataBuffer = Shader.PropertyToID("_InputLodCullingDataBuffer");

			public static readonly int _LodGroupCullingData = Shader.PropertyToID("_LodGroupCullingData");
		}
	}
}
