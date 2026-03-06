using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Modules/HierarchyCore/Public/Hierarchy.h")]
	[NativeHeader("Modules/HierarchyCore/HierarchyBindings.h")]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyNodeTypeHandlerBase.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class Hierarchy : IDisposable
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public unsafe ref readonly HierarchyNode Root
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return (void*)this.m_RootPtr;
			}
		}

		public int Count
		{
			[NativeMethod("Count", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Hierarchy.get_Count_Injected(intPtr);
			}
		}

		public bool Updating
		{
			[NativeMethod("Updating", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Hierarchy.get_Updating_Injected(intPtr);
			}
		}

		public bool UpdateNeeded
		{
			[NativeMethod("UpdateNeeded", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Hierarchy.get_UpdateNeeded_Injected(intPtr);
			}
		}

		internal unsafe int Version
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return *(int*)((void*)this.m_VersionPtr);
			}
		}

		public Hierarchy()
		{
			IntPtr rootPtr;
			IntPtr versionPtr;
			this.m_Ptr = Hierarchy.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), out rootPtr, out versionPtr);
			this.m_RootPtr = rootPtr;
			this.m_VersionPtr = versionPtr;
			this.m_IsOwner = true;
		}

		private Hierarchy(IntPtr nativePtr, IntPtr rootPtr, IntPtr versionPtr)
		{
			this.m_Ptr = nativePtr;
			this.m_RootPtr = rootPtr;
			this.m_VersionPtr = versionPtr;
			this.m_IsOwner = false;
		}

		~Hierarchy()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				bool isOwner = this.m_IsOwner;
				if (isOwner)
				{
					Hierarchy.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public T GetOrCreateNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return (T)((object)HierarchyNodeTypeHandlerBase.FromIntPtr(this.GetOrCreateNodeTypeHandler(typeof(T))));
		}

		public T GetNodeTypeHandlerBase<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return (T)((object)HierarchyNodeTypeHandlerBase.FromIntPtr(this.GetNodeTypeHandlerFromType(typeof(T))));
		}

		public HierarchyNodeTypeHandlerBase GetNodeTypeHandlerBase(in HierarchyNode node)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(this.GetNodeTypeHandlerFromNode(node));
		}

		public HierarchyNodeTypeHandlerBase GetNodeTypeHandlerBase(string nodeTypeName)
		{
			return HierarchyNodeTypeHandlerBase.FromIntPtr(this.GetNodeTypeHandlerFromName(nodeTypeName));
		}

		public HierarchyNodeTypeHandlerBaseEnumerable EnumerateNodeTypeHandlersBase()
		{
			return new HierarchyNodeTypeHandlerBaseEnumerable(this);
		}

		public HierarchyNodeType GetNodeType<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return this.GetNodeTypeFromType(typeof(T));
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNodeType GetNodeType(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNodeType result;
			Hierarchy.GetNodeType_Injected(intPtr, node, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void Reserve(int count)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.Reserve_Injected(intPtr, count);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void ReserveChildren(in HierarchyNode node, int count)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.ReserveChildren_Injected(intPtr, node, count);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Exists(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.Exists_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetNextSibling(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			Hierarchy.GetNextSibling_Injected(intPtr, node, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetDepth(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetDepth_Injected(intPtr, node);
		}

		public HierarchyNode Add(in HierarchyNode parent)
		{
			return this.AddNode(parent);
		}

		public HierarchyNode[] Add(in HierarchyNode parent, int count)
		{
			HierarchyNode[] array = new HierarchyNode[count];
			this.AddNodeSpan(parent, array);
			return array;
		}

		public void Add(in HierarchyNode parent, Span<HierarchyNode> outNodes)
		{
			this.AddNodeSpan(parent, outNodes);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Remove(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.Remove_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void RemoveChildren(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.RemoveChildren_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Clear()
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.Clear_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SetParent(in HierarchyNode node, in HierarchyNode parent)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.SetParent_Injected(intPtr, node, parent);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetParent(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			Hierarchy.GetParent_Injected(intPtr, node, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetChild(in HierarchyNode node, int index)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			Hierarchy.GetChild_Injected(intPtr, node, index, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildIndex(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetChildIndex_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode[] GetChildren(in HierarchyNode node)
		{
			HierarchyNode[] result;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				Hierarchy.GetChildren_Injected(intPtr, node, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				HierarchyNode[] array;
				blittableArrayWrapper.Unmarshal<HierarchyNode>(ref array);
				result = array;
			}
			return result;
		}

		public int GetChildren(in HierarchyNode node, Span<HierarchyNode> outChildren)
		{
			return this.GetNodeChildrenSpan(node, outChildren);
		}

		public HierarchyNodeChildren EnumerateChildren(in HierarchyNode node)
		{
			return new HierarchyNodeChildren(this, this.EnumerateChildrenPtr(node));
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCount(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetChildrenCount_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCountRecursive(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetChildrenCountRecursive_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void SetSortIndex(in HierarchyNode node, int sortIndex)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.SetSortIndex_Injected(intPtr, node, sortIndex);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetSortIndex(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetSortIndex_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void SortChildren(in HierarchyNode node, bool recurse = false)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.SortChildren_Injected(intPtr, node, recurse);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool DoesChildrenNeedsSorting(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.DoesChildrenNeedsSorting_Injected(intPtr, node);
		}

		public HierarchyPropertyUnmanaged<T> GetOrCreatePropertyUnmanaged<[IsUnmanaged] T>(string name, HierarchyPropertyStorageType type = HierarchyPropertyStorageType.Dense) where T : struct, ValueType
		{
			HierarchyPropertyDescriptor hierarchyPropertyDescriptor = default(HierarchyPropertyDescriptor);
			hierarchyPropertyDescriptor.Size = UnsafeUtility.SizeOf<T>();
			hierarchyPropertyDescriptor.Type = type;
			HierarchyPropertyId orCreateProperty = this.GetOrCreateProperty(name, hierarchyPropertyDescriptor);
			return new HierarchyPropertyUnmanaged<T>(this, ref orCreateProperty);
		}

		public HierarchyPropertyString GetOrCreatePropertyString(string name)
		{
			HierarchyPropertyDescriptor hierarchyPropertyDescriptor = default(HierarchyPropertyDescriptor);
			hierarchyPropertyDescriptor.Size = 0;
			hierarchyPropertyDescriptor.Type = HierarchyPropertyStorageType.Blob;
			HierarchyPropertyId orCreateProperty = this.GetOrCreateProperty(name, hierarchyPropertyDescriptor);
			return new HierarchyPropertyString(this, ref orCreateProperty);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public unsafe bool SetName(in HierarchyNode node, string name)
		{
			bool result;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				result = Hierarchy.SetName_Injected(intPtr, node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public string GetName(in HierarchyNode node)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpan;
				Hierarchy.GetName_Injected(intPtr, node, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public string GetPath(in HierarchyNode node)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpan;
				Hierarchy.GetPath_Injected(intPtr, node, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Update()
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.Update_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncremental()
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.UpdateIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.UpdateIncrementalTimed_Injected(intPtr, milliseconds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static Hierarchy FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((Hierarchy)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyBindings::Create", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(IntPtr handlePtr, out IntPtr rootPtr, out IntPtr versionPtr);

		[FreeFunction("HierarchyBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[FreeFunction("HierarchyBindings::GetOrCreateNodeTypeHandler", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private IntPtr GetOrCreateNodeTypeHandler(Type type)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetOrCreateNodeTypeHandler_Injected(intPtr, type);
		}

		[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromType", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private IntPtr GetNodeTypeHandlerFromType(Type type)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetNodeTypeHandlerFromType_Injected(intPtr, type);
		}

		[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private IntPtr GetNodeTypeHandlerFromNode(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetNodeTypeHandlerFromNode_Injected(intPtr, node);
		}

		[FreeFunction("HierarchyBindings::GetNodeTypeHandlerFromName", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe IntPtr GetNodeTypeHandlerFromName(string nodeTypeName)
		{
			IntPtr nodeTypeHandlerFromName_Injected;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(nodeTypeName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = nodeTypeName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				nodeTypeHandlerFromName_Injected = Hierarchy.GetNodeTypeHandlerFromName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return nodeTypeHandlerFromName_Injected;
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.HierarchyModule"
		})]
		[FreeFunction("HierarchyBindings::GetNodeTypeHandlersBaseCount", HasExplicitThis = true, IsThreadSafe = true)]
		internal int GetNodeTypeHandlersBaseCount()
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetNodeTypeHandlersBaseCount_Injected(intPtr);
		}

		[VisibleToOtherModules(new string[]
		{
			"UnityEngine.HierarchyModule"
		})]
		[FreeFunction("HierarchyBindings::GetNodeTypeHandlersBaseSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal unsafe int GetNodeTypeHandlersBaseSpan(Span<IntPtr> outHandlers)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<IntPtr> span = outHandlers;
			int nodeTypeHandlersBaseSpan_Injected;
			fixed (IntPtr* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodeTypeHandlersBaseSpan_Injected = Hierarchy.GetNodeTypeHandlersBaseSpan_Injected(intPtr, ref managedSpanWrapper);
			}
			return nodeTypeHandlersBaseSpan_Injected;
		}

		[FreeFunction("HierarchyBindings::GetNodeTypeFromType", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private HierarchyNodeType GetNodeTypeFromType(Type type)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNodeType result;
			Hierarchy.GetNodeTypeFromType_Injected(intPtr, type, out result);
			return result;
		}

		[FreeFunction("HierarchyBindings::AddNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private HierarchyNode AddNode(in HierarchyNode parent)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			Hierarchy.AddNode_Injected(intPtr, parent, out result);
			return result;
		}

		[FreeFunction("HierarchyBindings::AddNodeSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe void AddNodeSpan(in HierarchyNode parent, Span<HierarchyNode> nodes)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = nodes;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Hierarchy.AddNodeSpan_Injected(intPtr, parent, ref managedSpanWrapper);
			}
		}

		[FreeFunction("HierarchyBindings::GetNodeChildrenSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodeChildrenSpan(in HierarchyNode node, Span<HierarchyNode> outChildren)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outChildren;
			int nodeChildrenSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodeChildrenSpan_Injected = Hierarchy.GetNodeChildrenSpan_Injected(intPtr, node, ref managedSpanWrapper);
			}
			return nodeChildrenSpan_Injected;
		}

		[FreeFunction("HierarchyBindings::EnumerateChildrenPtr", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private IntPtr EnumerateChildrenPtr(in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.EnumerateChildrenPtr_Injected(intPtr, node);
		}

		[FreeFunction("HierarchyBindings::GetOrCreateProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe HierarchyPropertyId GetOrCreateProperty(string name, in HierarchyPropertyDescriptor descriptor)
		{
			HierarchyPropertyId result;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				HierarchyPropertyId hierarchyPropertyId;
				Hierarchy.GetOrCreateProperty_Injected(intPtr, ref managedSpanWrapper, descriptor, out hierarchyPropertyId);
			}
			finally
			{
				char* ptr = null;
				HierarchyPropertyId hierarchyPropertyId;
				result = hierarchyPropertyId;
			}
			return result;
		}

		[FreeFunction("HierarchyBindings::SetPropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal unsafe void SetPropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.SetPropertyRaw_Injected(intPtr, property, node, ptr, size);
		}

		[FreeFunction("HierarchyBindings::GetPropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal unsafe void* GetPropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, out int size)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Hierarchy.GetPropertyRaw_Injected(intPtr, property, node, out size);
		}

		[FreeFunction("HierarchyBindings::SetPropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal unsafe void SetPropertyString(in HierarchyPropertyId property, in HierarchyNode node, string value)
		{
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Hierarchy.SetPropertyString_Injected(intPtr, property, node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction("HierarchyBindings::GetPropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal string GetPropertyString(in HierarchyPropertyId property, in HierarchyNode node)
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpan;
				Hierarchy.GetPropertyString_Injected(intPtr, property, node, out managedSpan);
			}
			finally
			{
				ManagedSpanWrapper managedSpan;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
			}
			return stringAndDispose;
		}

		[FreeFunction("HierarchyBindings::ClearProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		internal void ClearProperty(in HierarchyPropertyId property, in HierarchyNode node)
		{
			IntPtr intPtr = Hierarchy.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Hierarchy.ClearProperty_Injected(intPtr, property, node);
		}

		[RequiredByNativeCode]
		private static IntPtr CreateHierarchy(IntPtr nativePtr, IntPtr rootPtr, IntPtr versionPtr)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new Hierarchy(nativePtr, rootPtr, versionPtr)));
		}

		[Obsolete("RegisterNodeTypeHandler has been renamed GetOrCreateNodeTypeHandler (UnityUpgradable) -> GetOrCreateNodeTypeHandler<T>()")]
		public T RegisterNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return (T)((object)HierarchyNodeTypeHandlerBase.FromIntPtr(this.GetOrCreateNodeTypeHandler(typeof(T))));
		}

		[Obsolete("UnregisterNodeTypeHandler no longer has any effect and will be removed in a future release.")]
		public void UnregisterNodeTypeHandler<T>() where T : HierarchyNodeTypeHandlerBase
		{
		}

		[Obsolete("GetAllNodeTypeHandlersBaseCount is obsolete, please use EnumerateNodeTypeHandlersBase instead.")]
		public int GetAllNodeTypeHandlersBaseCount()
		{
			return this.GetNodeTypeHandlersBaseCount();
		}

		[Obsolete("GetAllNodeTypeHandlersBase is obsolete, please use EnumerateNodeTypeHandlersBase instead.")]
		public void GetAllNodeTypeHandlersBase(List<HierarchyNodeTypeHandlerBase> handlers)
		{
			handlers.Clear();
			foreach (HierarchyNodeTypeHandlerBase item in this.EnumerateNodeTypeHandlersBase())
			{
				handlers.Add(item);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Count_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_Updating_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeType_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNodeType ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Reserve_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReserveChildren_Injected(IntPtr _unity_self, in HierarchyNode node, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Exists_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextSibling_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDepth_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Remove_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetParent_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParent_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChild_Injected(IntPtr _unity_self, in HierarchyNode node, int index, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetChildren_Injected(IntPtr _unity_self, in HierarchyNode node, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCountRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node, int sortIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SortChildren_Injected(IntPtr _unity_self, in HierarchyNode node, bool recurse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesChildrenNeedsSorting_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetName_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetName_Injected(IntPtr _unity_self, in HierarchyNode node, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPath_Injected(IntPtr _unity_self, in HierarchyNode node, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetOrCreateNodeTypeHandler_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNodeTypeHandlerFromType_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNodeTypeHandlerFromNode_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNodeTypeHandlerFromName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodeTypeName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeTypeHandlersBaseCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeTypeHandlersBaseSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper outHandlers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNodeTypeFromType_Injected(IntPtr _unity_self, Type type, out HierarchyNodeType ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddNode_Injected(IntPtr _unity_self, in HierarchyNode parent, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddNodeSpan_Injected(IntPtr _unity_self, in HierarchyNode parent, ref ManagedSpanWrapper nodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeChildrenSpan_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper outChildren);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr EnumerateChildrenPtr_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetOrCreateProperty_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name, in HierarchyPropertyDescriptor descriptor, out HierarchyPropertyId ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetPropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetPropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, out int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearProperty_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node);

		private IntPtr m_Ptr;

		private readonly IntPtr m_RootPtr;

		private readonly IntPtr m_VersionPtr;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(Hierarchy hierarchy)
			{
				return hierarchy.m_Ptr;
			}
		}
	}
}
