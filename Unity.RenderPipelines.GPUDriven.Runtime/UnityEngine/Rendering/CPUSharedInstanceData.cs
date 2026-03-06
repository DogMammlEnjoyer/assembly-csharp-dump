using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct CPUSharedInstanceData : IDisposable
	{
		public int instancesLength
		{
			get
			{
				return this.m_StructData[0];
			}
			set
			{
				this.m_StructData[0] = value;
			}
		}

		public int instancesCapacity
		{
			get
			{
				return this.m_StructData[1];
			}
			set
			{
				this.m_StructData[1] = value;
			}
		}

		public int handlesLength
		{
			get
			{
				return this.m_InstanceIndices.Length;
			}
		}

		public void Initialize(int initCapacity)
		{
			this.m_StructData = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.instancesCapacity = initCapacity;
			this.m_InstanceIndices = new NativeList<int>(Allocator.Persistent);
			this.instances = new NativeArray<SharedInstanceHandle>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ref this.instances.FillArray(SharedInstanceHandle.Invalid, 0, -1);
			this.rendererGroupIDs = new NativeArray<int>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.materialIDArrays = new NativeArray<SmallIntegerArray>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.meshIDs = new NativeArray<int>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.localAABBs = new NativeArray<AABB>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.flags = new NativeArray<CPUSharedInstanceFlags>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.lodGroupAndMasks = new NativeArray<uint>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			uint maxValue = uint.MaxValue;
			ref this.lodGroupAndMasks.FillArray(maxValue, 0, -1);
			this.meshLodInfos = new NativeArray<GPUDrivenMeshLodInfo>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.gameObjectLayers = new NativeArray<int>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.refCounts = new NativeArray<int>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		public void Dispose()
		{
			this.m_StructData.Dispose();
			this.m_InstanceIndices.Dispose();
			this.instances.Dispose();
			this.rendererGroupIDs.Dispose();
			foreach (SmallIntegerArray smallIntegerArray in this.materialIDArrays)
			{
				smallIntegerArray.Dispose();
			}
			this.materialIDArrays.Dispose();
			this.meshIDs.Dispose();
			this.localAABBs.Dispose();
			this.flags.Dispose();
			this.lodGroupAndMasks.Dispose();
			this.meshLodInfos.Dispose();
			this.gameObjectLayers.Dispose();
			this.refCounts.Dispose();
		}

		private void Grow(int newCapacity)
		{
			ref this.instances.ResizeArray(newCapacity);
			ref this.instances.FillArray(SharedInstanceHandle.Invalid, this.instancesCapacity, -1);
			ref this.rendererGroupIDs.ResizeArray(newCapacity);
			ref this.materialIDArrays.ResizeArray(newCapacity);
			SmallIntegerArray smallIntegerArray = default(SmallIntegerArray);
			ref this.materialIDArrays.FillArray(smallIntegerArray, this.instancesCapacity, -1);
			ref this.meshIDs.ResizeArray(newCapacity);
			ref this.localAABBs.ResizeArray(newCapacity);
			ref this.flags.ResizeArray(newCapacity);
			ref this.lodGroupAndMasks.ResizeArray(newCapacity);
			uint maxValue = uint.MaxValue;
			ref this.lodGroupAndMasks.FillArray(maxValue, this.instancesCapacity, -1);
			ref this.meshLodInfos.ResizeArray(newCapacity);
			ref this.gameObjectLayers.ResizeArray(newCapacity);
			ref this.refCounts.ResizeArray(newCapacity);
			this.instancesCapacity = newCapacity;
		}

		private void AddUnsafe(SharedInstanceHandle instance)
		{
			if (instance.index >= this.m_InstanceIndices.Length)
			{
				int length = this.m_InstanceIndices.Length;
				this.m_InstanceIndices.ResizeUninitialized(instance.index + 1);
				for (int i = length; i < this.m_InstanceIndices.Length - 1; i++)
				{
					this.m_InstanceIndices[i] = -1;
				}
			}
			this.m_InstanceIndices[instance.index] = this.instancesLength;
			this.instances[this.instancesLength] = instance;
			int instancesLength = this.instancesLength + 1;
			this.instancesLength = instancesLength;
		}

		public int SharedInstanceToIndex(SharedInstanceHandle instance)
		{
			return this.m_InstanceIndices[instance.index];
		}

		public SharedInstanceHandle IndexToSharedInstance(int index)
		{
			return this.instances[index];
		}

		public int InstanceToIndex(in CPUInstanceData instanceData, InstanceHandle instance)
		{
			CPUInstanceData cpuinstanceData = instanceData;
			int index = cpuinstanceData.InstanceToIndex(instance);
			NativeArray<SharedInstanceHandle> sharedInstances = instanceData.sharedInstances;
			SharedInstanceHandle instance2 = sharedInstances[index];
			return this.SharedInstanceToIndex(instance2);
		}

		public bool IsValidInstance(SharedInstanceHandle instance)
		{
			if (instance.valid && instance.index < this.m_InstanceIndices.Length)
			{
				int num = this.m_InstanceIndices[instance.index];
				return num >= 0 && num < this.instancesLength && this.instances[num].Equals(instance);
			}
			return false;
		}

		public bool IsFreeInstanceHandle(SharedInstanceHandle instance)
		{
			return instance.valid && (instance.index >= this.m_InstanceIndices.Length || this.m_InstanceIndices[instance.index] == -1);
		}

		public bool IsValidIndex(int index)
		{
			return index >= 0 && index < this.instancesLength && index == this.m_InstanceIndices[this.instances[index].index];
		}

		public int GetFreeInstancesCount()
		{
			return this.instancesCapacity - this.instancesLength;
		}

		public void EnsureFreeInstances(int instancesCount)
		{
			int freeInstancesCount = this.GetFreeInstancesCount();
			int num = instancesCount - freeInstancesCount;
			if (num > 0)
			{
				this.Grow(this.instancesCapacity + num + 256);
			}
		}

		public void AddNoGrow(SharedInstanceHandle instance)
		{
			this.AddUnsafe(instance);
			this.SetDefault(instance);
		}

		public void Add(SharedInstanceHandle instance)
		{
			this.EnsureFreeInstances(1);
			this.AddNoGrow(instance);
		}

		public void Remove(SharedInstanceHandle instance)
		{
			int num = this.SharedInstanceToIndex(instance);
			int index = this.instancesLength - 1;
			this.instances[num] = this.instances[index];
			this.rendererGroupIDs[num] = this.rendererGroupIDs[index];
			this.materialIDArrays[num].Dispose();
			this.materialIDArrays[num] = this.materialIDArrays[index];
			this.materialIDArrays[index] = default(SmallIntegerArray);
			this.meshIDs[num] = this.meshIDs[index];
			this.localAABBs[num] = this.localAABBs[index];
			this.flags[num] = this.flags[index];
			this.lodGroupAndMasks[num] = this.lodGroupAndMasks[index];
			this.meshLodInfos[num] = this.meshLodInfos[index];
			this.gameObjectLayers[num] = this.gameObjectLayers[index];
			this.refCounts[num] = this.refCounts[index];
			this.m_InstanceIndices[this.instances[index].index] = num;
			this.m_InstanceIndices[instance.index] = -1;
			this.instancesLength--;
		}

		public int Get_RendererGroupID(SharedInstanceHandle instance)
		{
			return this.rendererGroupIDs[this.SharedInstanceToIndex(instance)];
		}

		public int Get_MeshID(SharedInstanceHandle instance)
		{
			return this.meshIDs[this.SharedInstanceToIndex(instance)];
		}

		public ref AABB Get_LocalAABB(SharedInstanceHandle instance)
		{
			return UnsafeUtility.ArrayElementAsRef<AABB>(this.localAABBs.GetUnsafePtr<AABB>(), this.SharedInstanceToIndex(instance));
		}

		public CPUSharedInstanceFlags Get_Flags(SharedInstanceHandle instance)
		{
			return this.flags[this.SharedInstanceToIndex(instance)];
		}

		public uint Get_LODGroupAndMask(SharedInstanceHandle instance)
		{
			return this.lodGroupAndMasks[this.SharedInstanceToIndex(instance)];
		}

		public int Get_GameObjectLayer(SharedInstanceHandle instance)
		{
			return this.gameObjectLayers[this.SharedInstanceToIndex(instance)];
		}

		public int Get_RefCount(SharedInstanceHandle instance)
		{
			return this.refCounts[this.SharedInstanceToIndex(instance)];
		}

		public ref SmallIntegerArray Get_MaterialIDs(SharedInstanceHandle instance)
		{
			return UnsafeUtility.ArrayElementAsRef<SmallIntegerArray>(this.materialIDArrays.GetUnsafePtr<SmallIntegerArray>(), this.SharedInstanceToIndex(instance));
		}

		public void Set_RendererGroupID(SharedInstanceHandle instance, int rendererGroupID)
		{
			this.rendererGroupIDs[this.SharedInstanceToIndex(instance)] = rendererGroupID;
		}

		public void Set_MeshID(SharedInstanceHandle instance, int meshID)
		{
			this.meshIDs[this.SharedInstanceToIndex(instance)] = meshID;
		}

		public void Set_LocalAABB(SharedInstanceHandle instance, in AABB localAABB)
		{
			this.localAABBs[this.SharedInstanceToIndex(instance)] = localAABB;
		}

		public void Set_Flags(SharedInstanceHandle instance, CPUSharedInstanceFlags instanceFlags)
		{
			this.flags[this.SharedInstanceToIndex(instance)] = instanceFlags;
		}

		public void Set_LODGroupAndMask(SharedInstanceHandle instance, uint lodGroupAndMask)
		{
			this.lodGroupAndMasks[this.SharedInstanceToIndex(instance)] = lodGroupAndMask;
		}

		public void Set_GameObjectLayer(SharedInstanceHandle instance, int gameObjectLayer)
		{
			this.gameObjectLayers[this.SharedInstanceToIndex(instance)] = gameObjectLayer;
		}

		public void Set_RefCount(SharedInstanceHandle instance, int refCount)
		{
			this.refCounts[this.SharedInstanceToIndex(instance)] = refCount;
		}

		public void Set_MaterialIDs(SharedInstanceHandle instance, in SmallIntegerArray materialIDs)
		{
			int index = this.SharedInstanceToIndex(instance);
			this.materialIDArrays[index].Dispose();
			this.materialIDArrays[index] = materialIDs;
		}

		public void Set(SharedInstanceHandle instance, int rendererGroupID, in SmallIntegerArray materialIDs, int meshID, in AABB localAABB, TransformUpdateFlags transformUpdateFlags, InstanceFlags instanceFlags, uint lodGroupAndMask, GPUDrivenMeshLodInfo meshLodInfo, int gameObjectLayer, int refCount)
		{
			int index = this.SharedInstanceToIndex(instance);
			this.rendererGroupIDs[index] = rendererGroupID;
			this.materialIDArrays[index].Dispose();
			this.materialIDArrays[index] = materialIDs;
			this.meshIDs[index] = meshID;
			this.localAABBs[index] = localAABB;
			this.flags[index] = new CPUSharedInstanceFlags
			{
				transformUpdateFlags = transformUpdateFlags,
				instanceFlags = instanceFlags
			};
			this.lodGroupAndMasks[index] = lodGroupAndMask;
			this.meshLodInfos[index] = meshLodInfo;
			this.gameObjectLayers[index] = gameObjectLayer;
			this.refCounts[index] = refCount;
		}

		public void SetDefault(SharedInstanceHandle instance)
		{
			int rendererGroupID = 0;
			SmallIntegerArray smallIntegerArray = default(SmallIntegerArray);
			int meshID = 0;
			AABB aabb = default(AABB);
			this.Set(instance, rendererGroupID, smallIntegerArray, meshID, aabb, TransformUpdateFlags.None, InstanceFlags.None, uint.MaxValue, default(GPUDrivenMeshLodInfo), 0, 0);
		}

		public CPUSharedInstanceData.ReadOnly AsReadOnly()
		{
			return new CPUSharedInstanceData.ReadOnly(ref this);
		}

		private const int k_InvalidIndex = -1;

		private const uint k_InvalidLODGroupAndMask = 4294967295U;

		private NativeArray<int> m_StructData;

		private NativeList<int> m_InstanceIndices;

		public NativeArray<SharedInstanceHandle> instances;

		public NativeArray<int> rendererGroupIDs;

		public NativeArray<SmallIntegerArray> materialIDArrays;

		public NativeArray<int> meshIDs;

		public NativeArray<AABB> localAABBs;

		public NativeArray<CPUSharedInstanceFlags> flags;

		public NativeArray<uint> lodGroupAndMasks;

		public NativeArray<GPUDrivenMeshLodInfo> meshLodInfos;

		public NativeArray<int> gameObjectLayers;

		public NativeArray<int> refCounts;

		internal readonly struct ReadOnly
		{
			public int handlesLength
			{
				get
				{
					return this.instanceIndices.Length;
				}
			}

			public int instancesLength
			{
				get
				{
					return this.instances.Length;
				}
			}

			public ReadOnly(in CPUSharedInstanceData instanceData)
			{
				NativeList<int> nativeList = instanceData.m_InstanceIndices;
				this.instanceIndices = nativeList.AsArray().AsReadOnly();
				NativeArray<SharedInstanceHandle> nativeArray = instanceData.instances;
				int start = 0;
				CPUSharedInstanceData cpusharedInstanceData = instanceData;
				this.instances = nativeArray.GetSubArray(start, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<int> nativeArray2 = instanceData.rendererGroupIDs;
				int start2 = 0;
				cpusharedInstanceData = instanceData;
				this.rendererGroupIDs = nativeArray2.GetSubArray(start2, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<SmallIntegerArray> nativeArray3 = instanceData.materialIDArrays;
				int start3 = 0;
				cpusharedInstanceData = instanceData;
				this.materialIDArrays = nativeArray3.GetSubArray(start3, cpusharedInstanceData.instancesLength).AsReadOnly();
				nativeArray2 = instanceData.meshIDs;
				int start4 = 0;
				cpusharedInstanceData = instanceData;
				this.meshIDs = nativeArray2.GetSubArray(start4, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<AABB> nativeArray4 = instanceData.localAABBs;
				int start5 = 0;
				cpusharedInstanceData = instanceData;
				this.localAABBs = nativeArray4.GetSubArray(start5, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<CPUSharedInstanceFlags> nativeArray5 = instanceData.flags;
				int start6 = 0;
				cpusharedInstanceData = instanceData;
				this.flags = nativeArray5.GetSubArray(start6, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<uint> nativeArray6 = instanceData.lodGroupAndMasks;
				int start7 = 0;
				cpusharedInstanceData = instanceData;
				this.lodGroupAndMasks = nativeArray6.GetSubArray(start7, cpusharedInstanceData.instancesLength).AsReadOnly();
				NativeArray<GPUDrivenMeshLodInfo> nativeArray7 = instanceData.meshLodInfos;
				int start8 = 0;
				cpusharedInstanceData = instanceData;
				this.meshLodInfos = nativeArray7.GetSubArray(start8, cpusharedInstanceData.instancesLength).AsReadOnly();
				nativeArray2 = instanceData.gameObjectLayers;
				int start9 = 0;
				cpusharedInstanceData = instanceData;
				this.gameObjectLayers = nativeArray2.GetSubArray(start9, cpusharedInstanceData.instancesLength).AsReadOnly();
				nativeArray2 = instanceData.refCounts;
				int start10 = 0;
				cpusharedInstanceData = instanceData;
				this.refCounts = nativeArray2.GetSubArray(start10, cpusharedInstanceData.instancesLength).AsReadOnly();
			}

			public int SharedInstanceToIndex(SharedInstanceHandle instance)
			{
				return this.instanceIndices[instance.index];
			}

			public SharedInstanceHandle IndexToSharedInstance(int index)
			{
				return this.instances[index];
			}

			public bool IsValidSharedInstance(SharedInstanceHandle instance)
			{
				if (instance.valid && instance.index < this.instanceIndices.Length)
				{
					int num = this.instanceIndices[instance.index];
					return num >= 0 && num < this.instances.Length && this.instances[num].Equals(instance);
				}
				return false;
			}

			public bool IsValidIndex(int index)
			{
				if (index >= 0 && index < this.instances.Length)
				{
					SharedInstanceHandle sharedInstanceHandle = this.instances[index];
					return index == this.instanceIndices[sharedInstanceHandle.index];
				}
				return false;
			}

			public int InstanceToIndex(in CPUInstanceData.ReadOnly instanceData, InstanceHandle instance)
			{
				int index = instanceData.InstanceToIndex(instance);
				SharedInstanceHandle instance2 = instanceData.sharedInstances[index];
				return this.SharedInstanceToIndex(instance2);
			}

			public readonly NativeArray<int>.ReadOnly instanceIndices;

			public readonly NativeArray<SharedInstanceHandle>.ReadOnly instances;

			public readonly NativeArray<int>.ReadOnly rendererGroupIDs;

			public readonly NativeArray<SmallIntegerArray>.ReadOnly materialIDArrays;

			public readonly NativeArray<int>.ReadOnly meshIDs;

			public readonly NativeArray<AABB>.ReadOnly localAABBs;

			public readonly NativeArray<CPUSharedInstanceFlags>.ReadOnly flags;

			public readonly NativeArray<uint>.ReadOnly lodGroupAndMasks;

			public readonly NativeArray<GPUDrivenMeshLodInfo>.ReadOnly meshLodInfos;

			public readonly NativeArray<int>.ReadOnly gameObjectLayers;

			public readonly NativeArray<int>.ReadOnly refCounts;
		}
	}
}
