using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Profiling;

namespace UnityEngine.UIElements.Layout
{
	internal class LayoutManager : IDisposable
	{
		public static bool IsSharedManagerCreated
		{
			get
			{
				return LayoutManager.s_Initialized == LayoutManager.SharedManagerState.Initialized;
			}
		}

		public static LayoutManager SharedManager
		{
			get
			{
				LayoutManager.Initialize();
				return LayoutManager.s_SharedInstance;
			}
		}

		private static void Initialize()
		{
			bool flag = LayoutManager.s_Initialized > LayoutManager.SharedManagerState.Uninitialized;
			if (!flag)
			{
				LayoutManager.s_Initialized = LayoutManager.SharedManagerState.Initialized;
				bool flag2 = !LayoutManager.s_AppDomainUnloadRegistered;
				if (flag2)
				{
					AppDomain.CurrentDomain.DomainUnload += delegate(object _, EventArgs __)
					{
						LayoutManager.Shutdown();
					};
					LayoutManager.s_AppDomainUnloadRegistered = true;
				}
				LayoutManager.s_SharedInstance = new LayoutManager(Allocator.Persistent);
			}
		}

		private static void Shutdown()
		{
			bool flag = LayoutManager.s_Initialized != LayoutManager.SharedManagerState.Initialized;
			if (!flag)
			{
				LayoutManager.s_Initialized = LayoutManager.SharedManagerState.Shutdown;
				LayoutManager.s_SharedInstance.Dispose();
			}
		}

		private static int DefaultCapacity
		{
			get
			{
				return 16;
			}
		}

		public int NodeCapacity
		{
			get
			{
				return this.m_Nodes.Capacity;
			}
		}

		internal static LayoutManager GetManager(int index)
		{
			return ((ulong)index < (ulong)((long)LayoutManager.s_Managers.Count)) ? LayoutManager.s_Managers[index] : null;
		}

		public LayoutManager(Allocator allocator) : this(allocator, LayoutManager.DefaultCapacity)
		{
		}

		public LayoutManager(Allocator allocator, int initialNodeCapacity)
		{
			this.m_Index = LayoutManager.s_Managers.Count;
			LayoutManager.s_Managers.Add(this);
			ComponentType[] components = new ComponentType[]
			{
				ComponentType.Create<LayoutNodeData>(),
				ComponentType.Create<LayoutStyleData>(),
				ComponentType.Create<LayoutComputedData>(),
				ComponentType.Create<LayoutCacheData>()
			};
			ComponentType[] components2 = new ComponentType[]
			{
				ComponentType.Create<LayoutConfigData>()
			};
			this.m_Nodes = new LayoutDataStore(components, initialNodeCapacity, allocator);
			this.m_Configs = new LayoutDataStore(components2, 32, allocator);
			this.m_DefaultConfig = this.CreateConfig().Handle;
		}

		public unsafe void Dispose()
		{
			LayoutManager.s_Managers[this.m_Index] = null;
			for (int i = 0; i <= this.m_HighMark; i++)
			{
				LayoutNodeData* componentDataPtr = (LayoutNodeData*)this.m_Nodes.GetComponentDataPtr(i, 0);
				bool flag = !componentDataPtr->Children.IsCreated;
				if (!flag)
				{
					componentDataPtr->Children.Dispose();
					componentDataPtr->Children = new LayoutList<LayoutHandle>();
					GCHandle value = this.m_ManagedOwners.GetValue(componentDataPtr->ManagedOwnerIndex);
					bool isAllocated = value.IsAllocated;
					if (isAllocated)
					{
						value.Free();
					}
				}
			}
			this.m_Nodes.Dispose();
			this.m_Configs.Dispose();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private LayoutDataAccess GetAccess()
		{
			return new LayoutDataAccess(this.m_Index, this.m_Nodes, this.m_Configs);
		}

		public LayoutConfig GetDefaultConfig()
		{
			return new LayoutConfig(this.GetAccess(), this.m_DefaultConfig);
		}

		public LayoutConfig CreateConfig()
		{
			LayoutDataAccess access = this.GetAccess();
			LayoutConfigData @default = LayoutConfigData.Default;
			return new LayoutConfig(access, this.m_Configs.Allocate<LayoutConfigData>(@default));
		}

		public void DestroyConfig(ref LayoutConfig config)
		{
			LayoutHandle handle = config.Handle;
			this.m_Configs.Free(handle);
			config = LayoutConfig.Undefined;
		}

		public LayoutNode CreateNode()
		{
			return this.CreateNodeInternal(this.m_DefaultConfig);
		}

		public LayoutNode CreateNode(LayoutConfig config)
		{
			return this.CreateNodeInternal(config.Handle);
		}

		public LayoutNode CreateNode(LayoutNode source)
		{
			LayoutNode result = this.CreateNodeInternal(source.Config.Handle);
			result.CopyStyle(source);
			return result;
		}

		private LayoutNode CreateNodeInternal(LayoutHandle configHandle)
		{
			this.TryRecycleSingleNode();
			LayoutNodeData layoutNodeData = default(LayoutNodeData);
			layoutNodeData.Config = configHandle;
			layoutNodeData.Children = new LayoutList<LayoutHandle>();
			LayoutComputedData @default = LayoutComputedData.Default;
			LayoutHandle layoutHandle = this.m_Nodes.Allocate<LayoutNodeData, LayoutStyleData, LayoutComputedData, LayoutCacheData>(layoutNodeData, LayoutStyleData.Default, @default, LayoutCacheData.Default);
			bool flag = layoutHandle.Index > this.m_HighMark;
			if (flag)
			{
				this.m_HighMark = layoutHandle.Index;
			}
			LayoutNode result = new LayoutNode(this.GetAccess(), layoutHandle);
			Debug.Assert(!this.GetAccess().GetNodeData(layoutHandle).Children.IsCreated, "memory is not initialized");
			return result;
		}

		private void TryRecycleSingleNode()
		{
			LayoutHandle handle;
			bool flag = this.m_NodesToFree.TryDequeue(out handle);
			if (flag)
			{
				this.FreeNode(handle);
			}
		}

		private void TryRecycleNodes()
		{
			int num = 0;
			for (;;)
			{
				LayoutHandle handle;
				bool flag = num < 100 && this.m_NodesToFree.TryDequeue(out handle);
				if (!flag)
				{
					break;
				}
				this.FreeNode(handle);
				num++;
			}
		}

		public void EnqueueNodeForRecycling(ref LayoutNode node)
		{
			bool isUndefined = node.IsUndefined;
			if (!isUndefined)
			{
				this.m_NodesToFree.Enqueue(node.Handle);
				node = LayoutNode.Undefined;
			}
		}

		private void FreeNode(LayoutHandle handle)
		{
			ref LayoutNodeData nodeData = ref this.GetAccess().GetNodeData(handle);
			bool isCreated = nodeData.Children.IsCreated;
			if (isCreated)
			{
				nodeData.Children.Dispose();
				nodeData.Children = new LayoutList<LayoutHandle>();
			}
			nodeData.UsesMeasure = false;
			nodeData.UsesBaseline = false;
			GCHandle value = this.m_ManagedOwners.GetValue(nodeData.ManagedOwnerIndex);
			bool isAllocated = value.IsAllocated;
			if (isAllocated)
			{
				value.Free();
			}
			this.m_ManagedOwners.UpdateValue(ref nodeData.ManagedOwnerIndex, default(GCHandle));
			this.m_Nodes.Free(handle);
		}

		public void Collect()
		{
			using (this.m_CollectMarker.Auto())
			{
				this.TryRecycleNodes();
			}
		}

		public VisualElement GetOwner(LayoutHandle handle)
		{
			bool flag = this.GetAccess().GetNodeData(handle).ManagedOwnerIndex == 0;
			VisualElement result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (this.m_ManagedOwners.GetValue(this.GetAccess().GetNodeData(handle).ManagedOwnerIndex).Target as VisualElement);
			}
			return result;
		}

		public void SetOwner(LayoutHandle handle, VisualElement value)
		{
			ref int ptr = ref this.GetAccess().GetNodeData(handle).ManagedOwnerIndex;
			GCHandle value2 = this.m_ManagedOwners.GetValue(ptr);
			bool isAllocated = value2.IsAllocated;
			if (isAllocated)
			{
				value2.Free();
			}
			bool flag = value == null;
			if (flag)
			{
				value2 = default(GCHandle);
			}
			else
			{
				value2 = GCHandle.Alloc(value, GCHandleType.Weak);
			}
			this.m_ManagedOwners.UpdateValue(ref ptr, value2);
		}

		public LayoutMeasureFunction GetMeasureFunction(LayoutHandle handle)
		{
			int managedMeasureFunctionIndex = this.GetAccess().GetConfigData(handle).ManagedMeasureFunctionIndex;
			return this.m_ManagedMeasureFunctions.GetValue(managedMeasureFunctionIndex);
		}

		public void SetMeasureFunction(LayoutHandle handle, LayoutMeasureFunction value)
		{
			ref int index = ref this.GetAccess().GetConfigData(handle).ManagedMeasureFunctionIndex;
			this.m_ManagedMeasureFunctions.UpdateValue(ref index, value);
		}

		public LayoutBaselineFunction GetBaselineFunction(LayoutHandle handle)
		{
			int managedBaselineFunctionIndex = this.GetAccess().GetConfigData(handle).ManagedBaselineFunctionIndex;
			return this.m_ManagedBaselineFunctions.GetValue(managedBaselineFunctionIndex);
		}

		public void SetBaselineFunction(LayoutHandle handle, LayoutBaselineFunction value)
		{
			ref int index = ref this.GetAccess().GetConfigData(handle).ManagedBaselineFunctionIndex;
			this.m_ManagedBaselineFunctions.UpdateValue(ref index, value);
		}

		private static LayoutManager.SharedManagerState s_Initialized;

		private static bool s_AppDomainUnloadRegistered;

		private static LayoutManager s_SharedInstance;

		private static readonly List<LayoutManager> s_Managers = new List<LayoutManager>();

		public const int k_CapacityBig = 65536;

		public const int k_CapacitySmall = 16;

		private const int k_InitialConfigCapacity = 32;

		private readonly int m_Index;

		private LayoutDataStore m_Nodes;

		private LayoutDataStore m_Configs;

		private readonly ConcurrentQueue<LayoutHandle> m_NodesToFree = new ConcurrentQueue<LayoutHandle>();

		private readonly LayoutHandle m_DefaultConfig;

		private readonly ManagedObjectStore<LayoutMeasureFunction> m_ManagedMeasureFunctions = new ManagedObjectStore<LayoutMeasureFunction>(16);

		private readonly ManagedObjectStore<LayoutBaselineFunction> m_ManagedBaselineFunctions = new ManagedObjectStore<LayoutBaselineFunction>(16);

		private readonly ManagedObjectStore<GCHandle> m_ManagedOwners = new ManagedObjectStore<GCHandle>(2048);

		private readonly ProfilerMarker m_CollectMarker = new ProfilerMarker("UIElements.CollectLayoutNodes");

		private int m_HighMark = -1;

		private enum SharedManagerState
		{
			Uninitialized,
			Initialized,
			Shutdown
		}
	}
}
