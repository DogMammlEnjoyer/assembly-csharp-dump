using System;
using Unity.Collections;

namespace UnityEngine.Rendering
{
	internal struct InstanceAllocator
	{
		public int length
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

		public bool valid
		{
			get
			{
				return this.m_StructData.IsCreated;
			}
		}

		public void Initialize(int baseInstanceOffset = 0, int instanceStride = 1)
		{
			this.m_StructData = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);
			this.m_FreeInstances = new NativeList<int>(Allocator.Persistent);
			this.m_BaseInstanceOffset = baseInstanceOffset;
			this.m_InstanceStride = instanceStride;
		}

		public void Dispose()
		{
			this.m_StructData.Dispose();
			this.m_FreeInstances.Dispose();
		}

		public int AllocateInstance()
		{
			int result;
			if (this.m_FreeInstances.Length > 0)
			{
				result = this.m_FreeInstances[this.m_FreeInstances.Length - 1];
				this.m_FreeInstances.RemoveAtSwapBack(this.m_FreeInstances.Length - 1);
			}
			else
			{
				result = this.length * this.m_InstanceStride + this.m_BaseInstanceOffset;
				this.length++;
			}
			return result;
		}

		public void FreeInstance(int instance)
		{
			this.m_FreeInstances.Add(instance);
		}

		public int GetNumAllocated()
		{
			return this.length - this.m_FreeInstances.Length;
		}

		private NativeArray<int> m_StructData;

		private NativeList<int> m_FreeInstances;

		private int m_BaseInstanceOffset;

		private int m_InstanceStride;
	}
}
