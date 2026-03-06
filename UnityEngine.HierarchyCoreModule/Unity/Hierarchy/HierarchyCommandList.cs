using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Modules/HierarchyCore/HierarchyCommandListBindings.h")]
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyCommandList.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchyCommandList : IDisposable
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public int Size
		{
			[NativeMethod("Size", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_Size_Injected(intPtr);
			}
		}

		public int Capacity
		{
			[NativeMethod("Capacity", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_Capacity_Injected(intPtr);
			}
		}

		public bool IsEmpty
		{
			[NativeMethod("IsEmpty", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_IsEmpty_Injected(intPtr);
			}
		}

		public bool IsExecuting
		{
			[NativeMethod("IsExecuting", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyCommandList.get_IsExecuting_Injected(intPtr);
			}
		}

		public unsafe HierarchyCommandList(Hierarchy hierarchy, int initialCapacity = 65536) : this(hierarchy, *HierarchyNodeType.Null, initialCapacity)
		{
		}

		internal HierarchyCommandList(Hierarchy hierarchy, HierarchyNodeType nodeType, int initialCapacity = 65536)
		{
			this.m_Ptr = HierarchyCommandList.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchy, nodeType, initialCapacity);
			this.m_IsOwner = true;
		}

		private HierarchyCommandList(IntPtr nativePtr)
		{
			this.m_Ptr = nativePtr;
			this.m_IsOwner = false;
		}

		~HierarchyCommandList()
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
					HierarchyCommandList.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Clear()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyCommandList.Clear_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Reserve(int count)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.Reserve_Injected(intPtr, count);
		}

		public bool Add(in HierarchyNode parent, out HierarchyNode node)
		{
			return this.AddNode(parent, out node);
		}

		public bool Add(in HierarchyNode parent, int count, out HierarchyNode[] nodes)
		{
			nodes = new HierarchyNode[count];
			return this.AddNodeSpan(parent, nodes);
		}

		public bool Add(in HierarchyNode parent, Span<HierarchyNode> outNodes)
		{
			return this.AddNodeSpan(parent, outNodes);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Remove(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.Remove_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool RemoveChildren(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.RemoveChildren_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SetParent(in HierarchyNode node, in HierarchyNode parent)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetParent_Injected(intPtr, node, parent);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SetSortIndex(in HierarchyNode node, int sortIndex)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetSortIndex_Injected(intPtr, node, sortIndex);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool SortChildren(in HierarchyNode node, bool recurse = false)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SortChildren_Injected(intPtr, node, recurse);
		}

		public unsafe bool SetProperty<[IsUnmanaged] T>(in HierarchyPropertyUnmanaged<T> property, in HierarchyNode node, T value) where T : struct, ValueType
		{
			return this.SetNodePropertyRaw(property.m_Property, node, (void*)(&value), sizeof(T));
		}

		public bool SetProperty(in HierarchyPropertyString property, in HierarchyNode node, string value)
		{
			return this.SetNodePropertyString(property.m_Property, node, value);
		}

		public bool ClearProperty<[IsUnmanaged] T>(in HierarchyPropertyUnmanaged<T> property, in HierarchyNode node) where T : struct, ValueType
		{
			return this.ClearNodeProperty(property.m_Property, node);
		}

		public bool ClearProperty(in HierarchyPropertyString property, in HierarchyNode node)
		{
			return this.ClearNodeProperty(property.m_Property, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public unsafe bool SetName(in HierarchyNode node, string name)
		{
			bool result;
			try
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
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
				result = HierarchyCommandList.SetName_Injected(intPtr, node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public void Execute()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyCommandList.Execute_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool ExecuteIncremental()
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ExecuteIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool ExecuteIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ExecuteIncrementalTimed_Injected(intPtr, milliseconds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyCommandList FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyCommandList)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyCommandListBindings::Create", IsThreadSafe = true)]
		private static IntPtr Create(IntPtr handlePtr, Hierarchy hierarchy, HierarchyNodeType nodeType, int initialCapacity)
		{
			return HierarchyCommandList.Create_Injected(handlePtr, (hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), ref nodeType, initialCapacity);
		}

		[FreeFunction("HierarchyCommandListBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[FreeFunction("HierarchyCommandListBindings::AddNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool AddNode(in HierarchyNode parent, out HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.AddNode_Injected(intPtr, parent, out node);
		}

		[FreeFunction("HierarchyCommandListBindings::AddNodeSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool AddNodeSpan(in HierarchyNode parent, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			bool result;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				result = HierarchyCommandList.AddNodeSpan_Injected(intPtr, parent, ref managedSpanWrapper);
			}
			return result;
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodePropertyRaw", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool SetNodePropertyRaw(in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.SetNodePropertyRaw_Injected(intPtr, property, node, ptr, size);
		}

		[FreeFunction("HierarchyCommandListBindings::SetNodePropertyString", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe bool SetNodePropertyString(in HierarchyPropertyId property, in HierarchyNode node, string value)
		{
			bool result;
			try
			{
				IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
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
				result = HierarchyCommandList.SetNodePropertyString_Injected(intPtr, property, node, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return result;
		}

		[FreeFunction("HierarchyCommandListBindings::ClearNodeProperty", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool ClearNodeProperty(in HierarchyPropertyId property, in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyCommandList.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyCommandList.ClearNodeProperty_Injected(intPtr, property, node);
		}

		[RequiredByNativeCode]
		private static IntPtr CreateCommandList(IntPtr nativePtr)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyCommandList(nativePtr)));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Size_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_Capacity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_IsEmpty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_IsExecuting_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Clear_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Reserve_Injected(IntPtr _unity_self, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Remove_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RemoveChildren_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetParent_Injected(IntPtr _unity_self, in HierarchyNode node, in HierarchyNode parent);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetSortIndex_Injected(IntPtr _unity_self, in HierarchyNode node, int sortIndex);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SortChildren_Injected(IntPtr _unity_self, in HierarchyNode node, bool recurse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetName_Injected(IntPtr _unity_self, in HierarchyNode node, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Execute_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ExecuteIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ExecuteIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchy, [In] ref HierarchyNodeType nodeType, int initialCapacity);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddNode_Injected(IntPtr _unity_self, in HierarchyNode parent, out HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AddNodeSpan_Injected(IntPtr _unity_self, in HierarchyNode parent, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool SetNodePropertyRaw_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, void* ptr, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetNodePropertyString_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ClearNodeProperty_Injected(IntPtr _unity_self, in HierarchyPropertyId property, in HierarchyNode node);

		private IntPtr m_Ptr;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(HierarchyCommandList cmdList)
			{
				return cmdList.m_Ptr;
			}
		}
	}
}
