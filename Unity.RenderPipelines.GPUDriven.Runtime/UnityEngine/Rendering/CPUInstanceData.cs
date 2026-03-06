using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct CPUInstanceData : IDisposable
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
			this.instances = new NativeArray<InstanceHandle>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ref this.instances.FillArray(InstanceHandle.Invalid, 0, -1);
			this.sharedInstances = new NativeArray<SharedInstanceHandle>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			ref this.sharedInstances.FillArray(SharedInstanceHandle.Invalid, 0, -1);
			this.localToWorldIsFlippedBits = new ParallelBitArray(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.worldAABBs = new NativeArray<AABB>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.tetrahedronCacheIndices = new NativeArray<int>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
			int num = -1;
			ref this.tetrahedronCacheIndices.FillArray(num, 0, -1);
			this.movedInCurrentFrameBits = new ParallelBitArray(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.movedInPreviousFrameBits = new ParallelBitArray(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.visibleInPreviousFrameBits = new ParallelBitArray(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.editorData.Initialize(initCapacity);
			this.meshLodData = new NativeArray<GPUDrivenRendererMeshLodData>(this.instancesCapacity, Allocator.Persistent, NativeArrayOptions.ClearMemory);
		}

		public void Dispose()
		{
			this.m_StructData.Dispose();
			this.m_InstanceIndices.Dispose();
			this.instances.Dispose();
			this.sharedInstances.Dispose();
			this.localToWorldIsFlippedBits.Dispose();
			this.worldAABBs.Dispose();
			this.tetrahedronCacheIndices.Dispose();
			this.movedInCurrentFrameBits.Dispose();
			this.movedInPreviousFrameBits.Dispose();
			this.visibleInPreviousFrameBits.Dispose();
			this.editorData.Dispose();
			this.meshLodData.Dispose();
		}

		private void Grow(int newCapacity)
		{
			ref this.instances.ResizeArray(newCapacity);
			ref this.instances.FillArray(InstanceHandle.Invalid, this.instancesCapacity, -1);
			ref this.sharedInstances.ResizeArray(newCapacity);
			ref this.sharedInstances.FillArray(SharedInstanceHandle.Invalid, this.instancesCapacity, -1);
			this.localToWorldIsFlippedBits.Resize(newCapacity);
			ref this.worldAABBs.ResizeArray(newCapacity);
			ref this.tetrahedronCacheIndices.ResizeArray(newCapacity);
			int num = -1;
			ref this.tetrahedronCacheIndices.FillArray(num, this.instancesCapacity, -1);
			this.movedInCurrentFrameBits.Resize(newCapacity);
			this.movedInPreviousFrameBits.Resize(newCapacity);
			this.visibleInPreviousFrameBits.Resize(newCapacity);
			this.editorData.Grow(newCapacity);
			ref this.meshLodData.ResizeArray(newCapacity);
			this.instancesCapacity = newCapacity;
		}

		private void AddUnsafe(InstanceHandle instance)
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

		public int InstanceToIndex(InstanceHandle instance)
		{
			return this.m_InstanceIndices[instance.index];
		}

		public InstanceHandle IndexToInstance(int index)
		{
			return this.instances[index];
		}

		public bool IsValidInstance(InstanceHandle instance)
		{
			if (instance.valid && instance.index < this.m_InstanceIndices.Length)
			{
				int num = this.m_InstanceIndices[instance.index];
				return num >= 0 && num < this.instancesLength && this.instances[num].Equals(instance);
			}
			return false;
		}

		public bool IsFreeInstanceHandle(InstanceHandle instance)
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

		public void AddNoGrow(InstanceHandle instance)
		{
			this.AddUnsafe(instance);
			this.SetDefault(instance);
		}

		public void Add(InstanceHandle instance)
		{
			this.EnsureFreeInstances(1);
			this.AddNoGrow(instance);
		}

		public void Remove(InstanceHandle instance)
		{
			int num = this.InstanceToIndex(instance);
			int num2 = this.instancesLength - 1;
			this.instances[num] = this.instances[num2];
			this.sharedInstances[num] = this.sharedInstances[num2];
			this.localToWorldIsFlippedBits.Set(num, this.localToWorldIsFlippedBits.Get(num2));
			this.worldAABBs[num] = this.worldAABBs[num2];
			this.tetrahedronCacheIndices[num] = this.tetrahedronCacheIndices[num2];
			this.movedInCurrentFrameBits.Set(num, this.movedInCurrentFrameBits.Get(num2));
			this.movedInPreviousFrameBits.Set(num, this.movedInPreviousFrameBits.Get(num2));
			this.visibleInPreviousFrameBits.Set(num, this.visibleInPreviousFrameBits.Get(num2));
			this.editorData.Remove(num, num2);
			this.meshLodData[num] = this.meshLodData[num2];
			this.m_InstanceIndices[this.instances[num2].index] = num;
			this.m_InstanceIndices[instance.index] = -1;
			this.instancesLength--;
		}

		public void Set(InstanceHandle instance, SharedInstanceHandle sharedInstance, bool localToWorldIsFlipped, in AABB worldAABB, int tetrahedronCacheIndex, bool movedInCurrentFrame, bool movedInPreviousFrame, bool visibleInPreviousFrame, in GPUDrivenRendererMeshLodData meshLod)
		{
			int num = this.InstanceToIndex(instance);
			this.sharedInstances[num] = sharedInstance;
			this.localToWorldIsFlippedBits.Set(num, localToWorldIsFlipped);
			this.worldAABBs[num] = worldAABB;
			this.tetrahedronCacheIndices[num] = tetrahedronCacheIndex;
			this.movedInCurrentFrameBits.Set(num, movedInCurrentFrame);
			this.movedInPreviousFrameBits.Set(num, movedInPreviousFrame);
			this.visibleInPreviousFrameBits.Set(num, visibleInPreviousFrame);
			this.editorData.SetDefault(num);
			this.meshLodData[num] = meshLod;
		}

		public void SetDefault(InstanceHandle instance)
		{
			SharedInstanceHandle invalid = SharedInstanceHandle.Invalid;
			bool localToWorldIsFlipped = false;
			AABB aabb = default(AABB);
			int tetrahedronCacheIndex = -1;
			bool movedInCurrentFrame = false;
			bool movedInPreviousFrame = false;
			bool visibleInPreviousFrame = false;
			GPUDrivenRendererMeshLodData gpudrivenRendererMeshLodData = default(GPUDrivenRendererMeshLodData);
			this.Set(instance, invalid, localToWorldIsFlipped, aabb, tetrahedronCacheIndex, movedInCurrentFrame, movedInPreviousFrame, visibleInPreviousFrame, gpudrivenRendererMeshLodData);
		}

		public SharedInstanceHandle Get_SharedInstance(InstanceHandle instance)
		{
			return this.sharedInstances[this.InstanceToIndex(instance)];
		}

		public bool Get_LocalToWorldIsFlipped(InstanceHandle instance)
		{
			return this.localToWorldIsFlippedBits.Get(this.InstanceToIndex(instance));
		}

		public AABB Get_WorldAABB(InstanceHandle instance)
		{
			return this.worldAABBs[this.InstanceToIndex(instance)];
		}

		public int Get_TetrahedronCacheIndex(InstanceHandle instance)
		{
			return this.tetrahedronCacheIndices[this.InstanceToIndex(instance)];
		}

		public ref AABB Get_WorldBounds(InstanceHandle instance)
		{
			return UnsafeUtility.ArrayElementAsRef<AABB>(this.worldAABBs.GetUnsafePtr<AABB>(), this.InstanceToIndex(instance));
		}

		public bool Get_MovedInCurrentFrame(InstanceHandle instance)
		{
			return this.movedInCurrentFrameBits.Get(this.InstanceToIndex(instance));
		}

		public bool Get_MovedInPreviousFrame(InstanceHandle instance)
		{
			return this.movedInPreviousFrameBits.Get(this.InstanceToIndex(instance));
		}

		public bool Get_VisibleInPreviousFrame(InstanceHandle instance)
		{
			return this.visibleInPreviousFrameBits.Get(this.InstanceToIndex(instance));
		}

		public GPUDrivenRendererMeshLodData Get_MeshLodData(InstanceHandle instance)
		{
			return this.meshLodData[this.InstanceToIndex(instance)];
		}

		public void Set_SharedInstance(InstanceHandle instance, SharedInstanceHandle sharedInstance)
		{
			this.sharedInstances[this.InstanceToIndex(instance)] = sharedInstance;
		}

		public void Set_LocalToWorldIsFlipped(InstanceHandle instance, bool isFlipped)
		{
			this.localToWorldIsFlippedBits.Set(this.InstanceToIndex(instance), isFlipped);
		}

		public void Set_WorldAABB(InstanceHandle instance, in AABB worldBounds)
		{
			this.worldAABBs[this.InstanceToIndex(instance)] = worldBounds;
		}

		public void Set_TetrahedronCacheIndex(InstanceHandle instance, int tetrahedronCacheIndex)
		{
			this.tetrahedronCacheIndices[this.InstanceToIndex(instance)] = tetrahedronCacheIndex;
		}

		public void Set_MovedInCurrentFrame(InstanceHandle instance, bool movedInCurrentFrame)
		{
			this.movedInCurrentFrameBits.Set(this.InstanceToIndex(instance), movedInCurrentFrame);
		}

		public void Set_MovedInPreviousFrame(InstanceHandle instance, bool movedInPreviousFrame)
		{
			this.movedInPreviousFrameBits.Set(this.InstanceToIndex(instance), movedInPreviousFrame);
		}

		public void Set_VisibleInPreviousFrame(InstanceHandle instance, bool visibleInPreviousFrame)
		{
			this.visibleInPreviousFrameBits.Set(this.InstanceToIndex(instance), visibleInPreviousFrame);
		}

		public void Set_MeshLodData(InstanceHandle instance, GPUDrivenRendererMeshLodData meshLod)
		{
			this.meshLodData[this.InstanceToIndex(instance)] = meshLod;
		}

		public CPUInstanceData.ReadOnly AsReadOnly()
		{
			return new CPUInstanceData.ReadOnly(ref this);
		}

		private const int k_InvalidIndex = -1;

		private NativeArray<int> m_StructData;

		private NativeList<int> m_InstanceIndices;

		public NativeArray<InstanceHandle> instances;

		public NativeArray<SharedInstanceHandle> sharedInstances;

		public ParallelBitArray localToWorldIsFlippedBits;

		public NativeArray<AABB> worldAABBs;

		public NativeArray<int> tetrahedronCacheIndices;

		public ParallelBitArray movedInCurrentFrameBits;

		public ParallelBitArray movedInPreviousFrameBits;

		public ParallelBitArray visibleInPreviousFrameBits;

		public EditorInstanceDataArrays editorData;

		public NativeArray<GPUDrivenRendererMeshLodData> meshLodData;

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

			public ReadOnly(in CPUInstanceData instanceData)
			{
				NativeList<int> nativeList = instanceData.m_InstanceIndices;
				this.instanceIndices = nativeList.AsArray().AsReadOnly();
				NativeArray<InstanceHandle> nativeArray = instanceData.instances;
				int start = 0;
				CPUInstanceData cpuinstanceData = instanceData;
				this.instances = nativeArray.GetSubArray(start, cpuinstanceData.instancesLength).AsReadOnly();
				NativeArray<SharedInstanceHandle> nativeArray2 = instanceData.sharedInstances;
				int start2 = 0;
				cpuinstanceData = instanceData;
				this.sharedInstances = nativeArray2.GetSubArray(start2, cpuinstanceData.instancesLength).AsReadOnly();
				ParallelBitArray parallelBitArray = instanceData.localToWorldIsFlippedBits;
				cpuinstanceData = instanceData;
				this.localToWorldIsFlippedBits = parallelBitArray.GetSubArray(cpuinstanceData.instancesLength);
				NativeArray<AABB> nativeArray3 = instanceData.worldAABBs;
				int start3 = 0;
				cpuinstanceData = instanceData;
				this.worldAABBs = nativeArray3.GetSubArray(start3, cpuinstanceData.instancesLength).AsReadOnly();
				NativeArray<int> nativeArray4 = instanceData.tetrahedronCacheIndices;
				int start4 = 0;
				cpuinstanceData = instanceData;
				this.tetrahedronCacheIndices = nativeArray4.GetSubArray(start4, cpuinstanceData.instancesLength).AsReadOnly();
				parallelBitArray = instanceData.movedInCurrentFrameBits;
				cpuinstanceData = instanceData;
				this.movedInCurrentFrameBits = parallelBitArray.GetSubArray(cpuinstanceData.instancesLength);
				parallelBitArray = instanceData.movedInPreviousFrameBits;
				cpuinstanceData = instanceData;
				this.movedInPreviousFrameBits = parallelBitArray.GetSubArray(cpuinstanceData.instancesLength);
				parallelBitArray = instanceData.visibleInPreviousFrameBits;
				cpuinstanceData = instanceData;
				this.visibleInPreviousFrameBits = parallelBitArray.GetSubArray(cpuinstanceData.instancesLength);
				this.editorData = new EditorInstanceDataArrays.ReadOnly(ref instanceData);
				NativeArray<GPUDrivenRendererMeshLodData> nativeArray5 = instanceData.meshLodData;
				int start5 = 0;
				cpuinstanceData = instanceData;
				this.meshLodData = nativeArray5.GetSubArray(start5, cpuinstanceData.instancesLength).AsReadOnly();
			}

			public int InstanceToIndex(InstanceHandle instance)
			{
				return this.instanceIndices[instance.index];
			}

			public InstanceHandle IndexToInstance(int index)
			{
				return this.instances[index];
			}

			public bool IsValidInstance(InstanceHandle instance)
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
					InstanceHandle instanceHandle = this.instances[index];
					return index == this.instanceIndices[instanceHandle.index];
				}
				return false;
			}

			public readonly NativeArray<int>.ReadOnly instanceIndices;

			public readonly NativeArray<InstanceHandle>.ReadOnly instances;

			public readonly NativeArray<SharedInstanceHandle>.ReadOnly sharedInstances;

			public readonly ParallelBitArray localToWorldIsFlippedBits;

			public readonly NativeArray<AABB>.ReadOnly worldAABBs;

			public readonly NativeArray<int>.ReadOnly tetrahedronCacheIndices;

			public readonly ParallelBitArray movedInCurrentFrameBits;

			public readonly ParallelBitArray movedInPreviousFrameBits;

			public readonly ParallelBitArray visibleInPreviousFrameBits;

			public readonly EditorInstanceDataArrays.ReadOnly editorData;

			public readonly NativeArray<GPUDrivenRendererMeshLodData>.ReadOnly meshLodData;
		}
	}
}
