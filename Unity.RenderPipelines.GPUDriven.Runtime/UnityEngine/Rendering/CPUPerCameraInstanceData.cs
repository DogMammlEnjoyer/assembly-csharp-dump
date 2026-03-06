using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Rendering
{
	internal struct CPUPerCameraInstanceData : IDisposable
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

		public int cameraCount
		{
			get
			{
				return this.perCameraData.Count();
			}
		}

		public void Initialize(int initCapacity)
		{
			this.perCameraData = new NativeParallelHashMap<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays>(1, Allocator.Persistent);
			this.m_StructData = new NativeArray<int>(2, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.instancesCapacity = initCapacity;
			this.instancesLength = 0;
		}

		public void DeallocateCameras(NativeArray<int> cameraIDs)
		{
			foreach (int key in cameraIDs)
			{
				CPUPerCameraInstanceData.PerCameraInstanceDataArrays perCameraInstanceDataArrays;
				if (this.perCameraData.TryGetValue(key, out perCameraInstanceDataArrays))
				{
					perCameraInstanceDataArrays.Dispose();
					this.perCameraData.Remove(key);
				}
			}
		}

		public void AllocateCameras(NativeArray<int> cameraIDs)
		{
			foreach (int key in cameraIDs)
			{
				CPUPerCameraInstanceData.PerCameraInstanceDataArrays item;
				if (!this.perCameraData.TryGetValue(key, out item))
				{
					item = new CPUPerCameraInstanceData.PerCameraInstanceDataArrays(this.instancesCapacity);
					this.perCameraData.Add(key, item);
				}
			}
		}

		public void Remove(int index)
		{
			int lastIndex = this.instancesLength - 1;
			foreach (KeyValue<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays> keyValue in this.perCameraData)
			{
				keyValue.Value.Remove(index, lastIndex);
			}
			this.instancesLength--;
		}

		public void IncreaseInstanceCount()
		{
			int instancesLength = this.instancesLength;
			this.instancesLength = instancesLength + 1;
		}

		public void Dispose()
		{
			foreach (KeyValue<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays> keyValue in this.perCameraData)
			{
				keyValue.Value.Dispose();
			}
			this.m_StructData.Dispose();
			this.perCameraData.Dispose();
		}

		internal void Grow(int newCapacity)
		{
			if (newCapacity < this.instancesCapacity)
			{
				return;
			}
			int instancesCapacity = this.instancesCapacity;
			this.instancesCapacity = newCapacity;
			foreach (KeyValue<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays> keyValue in this.perCameraData)
			{
				keyValue.Value.Grow(instancesCapacity, this.instancesCapacity);
			}
		}

		public void SetDefault(int index)
		{
			foreach (KeyValue<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays> keyValue in this.perCameraData)
			{
				keyValue.Value.SetDefault(index);
			}
		}

		public const byte k_InvalidByteData = 255;

		public NativeParallelHashMap<int, CPUPerCameraInstanceData.PerCameraInstanceDataArrays> perCameraData;

		private NativeArray<int> m_StructData;

		internal struct PerCameraInstanceDataArrays : IDisposable
		{
			public bool IsCreated
			{
				get
				{
					return this.meshLods.IsCreated && this.crossFades.IsCreated;
				}
			}

			public PerCameraInstanceDataArrays(int initCapacity)
			{
				this.meshLods = new UnsafeList<byte>(initCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.meshLods.Length = initCapacity;
				this.crossFades = new UnsafeList<byte>(initCapacity, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
				this.crossFades.Length = initCapacity;
			}

			public void Dispose()
			{
				this.meshLods.Dispose();
				this.crossFades.Dispose();
			}

			internal void Remove(int index, int lastIndex)
			{
				this.meshLods[index] = this.meshLods[lastIndex];
				this.crossFades[index] = this.crossFades[lastIndex];
			}

			internal void Grow(int previousCapacity, int newCapacity)
			{
				this.meshLods.Length = newCapacity;
				this.crossFades.Length = newCapacity;
			}

			internal void SetDefault(int index)
			{
				this.meshLods[index] = byte.MaxValue;
				this.crossFades[index] = byte.MaxValue;
			}

			internal UnsafeList<byte> meshLods;

			internal UnsafeList<byte> crossFades;
		}
	}
}
