using System;

namespace UnityEngine.Rendering
{
	internal struct InstanceAllocators
	{
		public void Initialize()
		{
			this.m_InstanceAlloc_MeshRenderer = default(InstanceAllocator);
			this.m_InstanceAlloc_SpeedTree = default(InstanceAllocator);
			this.m_InstanceAlloc_MeshRenderer.Initialize(0, 2);
			this.m_InstanceAlloc_SpeedTree.Initialize(1, 2);
			this.m_SharedInstanceAlloc = default(InstanceAllocator);
			this.m_SharedInstanceAlloc.Initialize(0, 1);
		}

		public void Dispose()
		{
			this.m_InstanceAlloc_MeshRenderer.Dispose();
			this.m_InstanceAlloc_SpeedTree.Dispose();
			this.m_SharedInstanceAlloc.Dispose();
		}

		private InstanceAllocator GetInstanceAllocator(InstanceType type)
		{
			if (type == InstanceType.MeshRenderer)
			{
				return this.m_InstanceAlloc_MeshRenderer;
			}
			if (type != InstanceType.SpeedTree)
			{
				throw new ArgumentException("Allocator for this type is not created.");
			}
			return this.m_InstanceAlloc_SpeedTree;
		}

		public int GetInstanceHandlesLength(InstanceType type)
		{
			return this.GetInstanceAllocator(type).length;
		}

		public int GetInstancesLength(InstanceType type)
		{
			return this.GetInstanceAllocator(type).GetNumAllocated();
		}

		public InstanceHandle AllocateInstance(InstanceType type)
		{
			return InstanceHandle.FromInt(this.GetInstanceAllocator(type).AllocateInstance());
		}

		public void FreeInstance(InstanceHandle instance)
		{
			this.GetInstanceAllocator(instance.type).FreeInstance(instance.index);
		}

		public SharedInstanceHandle AllocateSharedInstance()
		{
			return new SharedInstanceHandle
			{
				index = this.m_SharedInstanceAlloc.AllocateInstance()
			};
		}

		public void FreeSharedInstance(SharedInstanceHandle instance)
		{
			this.m_SharedInstanceAlloc.FreeInstance(instance.index);
		}

		private InstanceAllocator m_InstanceAlloc_MeshRenderer;

		private InstanceAllocator m_InstanceAlloc_SpeedTree;

		private InstanceAllocator m_SharedInstanceAlloc;
	}
}
