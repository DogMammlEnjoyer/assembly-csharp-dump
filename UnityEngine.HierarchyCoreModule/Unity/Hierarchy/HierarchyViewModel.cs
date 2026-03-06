using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/Public/HierarchyViewModel.h")]
	[NativeHeader("Modules/HierarchyCore/HierarchyViewModelBindings.h")]
	[RequiredByNativeCode(GenerateProxy = true)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class HierarchyViewModel : IDisposable
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Ptr != IntPtr.Zero;
			}
		}

		public int Count
		{
			get
			{
				return this.m_NodesCount;
			}
		}

		public bool Updating
		{
			[NativeMethod("Updating", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_Updating_Injected(intPtr);
			}
		}

		public bool UpdateNeeded
		{
			[NativeMethod("UpdateNeeded", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_UpdateNeeded_Injected(intPtr);
			}
		}

		public HierarchyFlattened HierarchyFlattened
		{
			get
			{
				return this.m_HierarchyFlattened;
			}
		}

		public Hierarchy Hierarchy
		{
			get
			{
				return this.m_Hierarchy;
			}
		}

		internal unsafe int* NodesPtr
		{
			get
			{
				return (int*)((void*)this.m_NodesPtr);
			}
		}

		internal int Version
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEngine.HierarchyModule"
			})]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				return this.m_Version;
			}
		}

		internal float UpdateProgress
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEngine.HierarchyModule"
			})]
			[NativeMethod("UpdateProgress", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_UpdateProgress_Injected(intPtr);
			}
		}

		internal IHierarchySearchQueryParser QueryParser { [VisibleToOtherModules(new string[]
		{
			"UnityEditor.HierarchyModule"
		})] get; [VisibleToOtherModules(new string[]
		{
			"UnityEditor.HierarchyModule"
		})] set; }

		internal HierarchySearchQueryDescriptor Query
		{
			[NativeMethod(IsThreadSafe = true)]
			[VisibleToOtherModules(new string[]
			{
				"UnityEngine.HierarchyModule"
			})]
			get
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return HierarchyViewModel.get_Query_Injected(intPtr);
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEngine.HierarchyModule"
			})]
			[NativeMethod(IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				HierarchyViewModel.set_Query_Injected(intPtr, value);
			}
		}

		public HierarchyViewModel(HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags = HierarchyNodeFlags.None)
		{
			IntPtr nodesPtr;
			int nodesCount;
			int version;
			this.m_Ptr = HierarchyViewModel.Create(GCHandle.ToIntPtr(GCHandle.Alloc(this)), hierarchyFlattened, defaultFlags, out nodesPtr, out nodesCount, out version);
			this.m_Hierarchy = hierarchyFlattened.Hierarchy;
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_NodesPtr = nodesPtr;
			this.m_NodesCount = nodesCount;
			this.m_Version = version;
			this.m_IsOwner = true;
			this.QueryParser = new DefaultHierarchySearchQueryParser();
		}

		private HierarchyViewModel(IntPtr nativePtr, HierarchyFlattened hierarchyFlattened, IntPtr nodesPtr, int nodesCount, int version)
		{
			this.m_Ptr = nativePtr;
			this.m_Hierarchy = hierarchyFlattened.Hierarchy;
			this.m_HierarchyFlattened = hierarchyFlattened;
			this.m_NodesPtr = nodesPtr;
			this.m_NodesCount = nodesCount;
			this.m_Version = version;
			this.m_IsOwner = false;
			this.QueryParser = new DefaultHierarchySearchQueryParser();
		}

		~HierarchyViewModel()
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
					HierarchyViewModel.Destroy(this.m_Ptr);
				}
				this.m_Ptr = IntPtr.Zero;
			}
		}

		public unsafe HierarchyNode this[int index]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				bool flag = index < 0 || index >= this.m_NodesCount;
				if (flag)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				return HierarchyFlattenedNode.GetNodeByRef(this.m_HierarchyFlattened[*(int*)((byte*)((void*)this.m_NodesPtr) + (IntPtr)index * 4)]);
			}
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int IndexOf(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.IndexOf_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public bool Contains(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.Contains_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetParent(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			HierarchyViewModel.GetParent_Injected(intPtr, node, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNode GetNextSibling(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyNode result;
			HierarchyViewModel.GetNextSibling_Injected(intPtr, node, out result);
			return result;
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCount(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetChildrenCount_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetChildrenCountRecursive(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetChildrenCountRecursive_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int GetDepth(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetDepth_Injected(intPtr, node);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public HierarchyNodeFlags GetFlags(in HierarchyNode node)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.GetFlags_Injected(intPtr, node);
		}

		public void SetFlags(HierarchyNodeFlags flags)
		{
			this.SetFlagsAll(flags);
		}

		public void SetFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			this.SetFlagsNode(node, flags, recurse);
		}

		public int SetFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.SetFlagsNodes(nodes, flags);
		}

		public int SetFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.SetFlagsIndices(indices, flags);
		}

		public bool HasAllFlags(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsAny(flags);
		}

		public bool HasAnyFlags(HierarchyNodeFlags flags)
		{
			return this.HasAnyFlagsAny(flags);
		}

		public bool HasAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsNode(node, flags);
		}

		public bool HasAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAnyFlagsNode(node, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int HasAllFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsCount_Injected(intPtr, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int HasAnyFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsCount_Injected(intPtr, flags);
		}

		public bool DoesNotHaveAllFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsAny(flags);
		}

		public bool DoesNotHaveAnyFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAnyFlagsAny(flags);
		}

		public bool DoesNotHaveAllFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsNode(node, flags);
		}

		public bool DoesNotHaveAnyFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAnyFlagsNode(node, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int DoesNotHaveAllFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsCount_Injected(intPtr, flags);
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		public int DoesNotHaveAnyFlagsCount(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsCount_Injected(intPtr, flags);
		}

		public void ClearFlags(HierarchyNodeFlags flags)
		{
			this.ClearFlagsAll(flags);
		}

		public void ClearFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			this.ClearFlagsNode(node, flags, recurse);
		}

		public int ClearFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.ClearFlagsNodes(nodes, flags);
		}

		public int ClearFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.ClearFlagsIndices(indices, flags);
		}

		public void ToggleFlags(HierarchyNodeFlags flags)
		{
			this.ToggleFlagsAll(flags);
		}

		public void ToggleFlags(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			this.ToggleFlagsNode(node, flags, recurse);
		}

		public int ToggleFlags(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			return this.ToggleFlagsNodes(nodes, flags);
		}

		public int ToggleFlags(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			return this.ToggleFlagsIndices(indices, flags);
		}

		public int GetNodesWithAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAllFlagsSpan(flags, outNodes);
		}

		public int GetNodesWithAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAnyFlagsSpan(flags, outNodes);
		}

		public HierarchyNode[] GetNodesWithAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAllFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] result;
			if (flag)
			{
				result = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array = new HierarchyNode[num];
				this.GetNodesWithAllFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public HierarchyNode[] GetNodesWithAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAnyFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] result;
			if (flag)
			{
				result = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array = new HierarchyNode[num];
				this.GetNodesWithAnyFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public HierarchyViewNodesEnumerable EnumerateNodesWithAllFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewNodesEnumerable(this, flags, new HierarchyViewNodesEnumerable.Predicate(this.HasAllFlags));
		}

		public HierarchyViewNodesEnumerable EnumerateNodesWithAnyFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewNodesEnumerable(this, flags, new HierarchyViewNodesEnumerable.Predicate(this.HasAnyFlags));
		}

		public int GetIndicesWithAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAllFlagsSpan(flags, outIndices);
		}

		public int GetIndicesWithAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAnyFlagsSpan(flags, outIndices);
		}

		public int[] GetIndicesWithAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAllFlagsCount(flags);
			bool flag = num == 0;
			int[] result;
			if (flag)
			{
				result = Array.Empty<int>();
			}
			else
			{
				int[] array = new int[num];
				this.GetIndicesWithAllFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public int[] GetIndicesWithAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.HasAnyFlagsCount(flags);
			bool flag = num == 0;
			int[] result;
			if (flag)
			{
				result = Array.Empty<int>();
			}
			else
			{
				int[] array = new int[num];
				this.GetIndicesWithAnyFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public int GetNodesWithoutAllFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAllFlagsSpan(flags, outNodes);
		}

		public int GetNodesWithoutAnyFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAnyFlagsSpan(flags, outNodes);
		}

		public HierarchyNode[] GetNodesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAllFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] result;
			if (flag)
			{
				result = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array = new HierarchyNode[num];
				this.GetNodesWithoutAllFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public HierarchyNode[] GetNodesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAnyFlagsCount(flags);
			bool flag = num == 0;
			HierarchyNode[] result;
			if (flag)
			{
				result = Array.Empty<HierarchyNode>();
			}
			else
			{
				HierarchyNode[] array = new HierarchyNode[num];
				this.GetNodesWithoutAnyFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public HierarchyViewNodesEnumerable EnumerateNodesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewNodesEnumerable(this, flags, new HierarchyViewNodesEnumerable.Predicate(this.DoesNotHaveAllFlags));
		}

		public HierarchyViewNodesEnumerable EnumerateNodesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			return new HierarchyViewNodesEnumerable(this, flags, new HierarchyViewNodesEnumerable.Predicate(this.DoesNotHaveAnyFlags));
		}

		public int GetIndicesWithoutAllFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAllFlagsSpan(flags, outIndices);
		}

		public int GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAnyFlagsSpan(flags, outIndices);
		}

		public int[] GetIndicesWithoutAllFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAllFlagsCount(flags);
			bool flag = num == 0;
			int[] result;
			if (flag)
			{
				result = Array.Empty<int>();
			}
			else
			{
				int[] array = new int[num];
				this.GetIndicesWithoutAllFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public int[] GetIndicesWithoutAnyFlags(HierarchyNodeFlags flags)
		{
			int num = this.DoesNotHaveAnyFlagsCount(flags);
			bool flag = num == 0;
			int[] result;
			if (flag)
			{
				result = Array.Empty<int>();
			}
			else
			{
				int[] array = new int[num];
				this.GetIndicesWithoutAnyFlagsSpan(flags, array);
				result = array;
			}
			return result;
		}

		public void SetQuery(string query)
		{
			HierarchySearchQueryDescriptor hierarchySearchQueryDescriptor = this.QueryParser.ParseQuery(query);
			bool flag = hierarchySearchQueryDescriptor == this.Query;
			if (!flag)
			{
				this.Query = hierarchySearchQueryDescriptor;
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		public void Update()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.Update_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncremental()
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.UpdateIncremental_Injected(intPtr);
		}

		[NativeMethod(IsThreadSafe = true)]
		public bool UpdateIncrementalTimed(double milliseconds)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.UpdateIncrementalTimed_Injected(intPtr, milliseconds);
		}

		public HierarchyViewModel.Enumerator GetEnumerator()
		{
			return new HierarchyViewModel.Enumerator(this);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static HierarchyViewModel FromIntPtr(IntPtr handlePtr)
		{
			return (handlePtr != IntPtr.Zero) ? ((HierarchyViewModel)GCHandle.FromIntPtr(handlePtr).Target) : null;
		}

		[FreeFunction("HierarchyViewModelBindings::Create", IsThreadSafe = true)]
		private static IntPtr Create(IntPtr handlePtr, HierarchyFlattened hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out int version)
		{
			return HierarchyViewModel.Create_Injected(handlePtr, (hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToNative(hierarchyFlattened), defaultFlags, out nodesPtr, out nodesCount, out version);
		}

		[FreeFunction("HierarchyViewModelBindings::Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr nativePtr);

		[FreeFunction("HierarchyViewModelBindings::SetFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void SetFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void SetFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.SetFlagsNode_Injected(intPtr, node, flags, recurse);
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int SetFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int result;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.SetFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::SetFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int SetFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int result;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.SetFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::HasAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAllFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAnyFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAllFlagsNode_Injected(intPtr, node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::HasAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool HasAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.HasAnyFlagsNode_Injected(intPtr, node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAllFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsAny", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAnyFlagsAny(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsAny_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAllFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAllFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAllFlagsNode_Injected(intPtr, node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::DoesNotHaveAnyFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private bool DoesNotHaveAnyFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return HierarchyViewModel.DoesNotHaveAnyFlagsNode_Injected(intPtr, node, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ClearFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ClearFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ClearFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ClearFlagsNode_Injected(intPtr, node, flags, recurse);
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ClearFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int result;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.ClearFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::ClearFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ClearFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int result;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.ClearFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsAll", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ToggleFlagsAll(HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ToggleFlagsAll_Injected(intPtr, flags);
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNode", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private void ToggleFlagsNode(in HierarchyNode node, HierarchyNodeFlags flags, bool recurse = false)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			HierarchyViewModel.ToggleFlagsNode_Injected(intPtr, node, flags, recurse);
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsNodes", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ToggleFlagsNodes(ReadOnlySpan<HierarchyNode> nodes, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<HierarchyNode> readOnlySpan = nodes;
			int result;
			fixed (HierarchyNode* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.ToggleFlagsNodes_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::ToggleFlagsIndices", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe int ToggleFlagsIndices(ReadOnlySpan<int> indices, HierarchyNodeFlags flags)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<int> readOnlySpan = indices;
			int result;
			fixed (int* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				result = HierarchyViewModel.ToggleFlagsIndices_Injected(intPtr, ref managedSpanWrapper, flags);
			}
			return result;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithAllFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithAllFlagsSpan_Injected = HierarchyViewModel.GetNodesWithAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithAnyFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithAnyFlagsSpan_Injected = HierarchyViewModel.GetNodesWithAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithAllFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithAllFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithAnyFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithAnyFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithoutAllFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithoutAllFlagsSpan_Injected = HierarchyViewModel.GetNodesWithoutAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithoutAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetNodesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetNodesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<HierarchyNode> span = outNodes;
			int nodesWithoutAnyFlagsSpan_Injected;
			fixed (HierarchyNode* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				nodesWithoutAnyFlagsSpan_Injected = HierarchyViewModel.GetNodesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return nodesWithoutAnyFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAllFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithoutAllFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithoutAllFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithoutAllFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithoutAllFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithoutAllFlagsSpan_Injected;
		}

		[FreeFunction("HierarchyViewModelBindings::GetIndicesWithoutAnyFlagsSpan", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
		private unsafe int GetIndicesWithoutAnyFlagsSpan(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			IntPtr intPtr = HierarchyViewModel.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<int> span = outIndices;
			int indicesWithoutAnyFlagsSpan_Injected;
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				indicesWithoutAnyFlagsSpan_Injected = HierarchyViewModel.GetIndicesWithoutAnyFlagsSpan_Injected(intPtr, flags, ref managedSpanWrapper);
			}
			return indicesWithoutAnyFlagsSpan_Injected;
		}

		[RequiredByNativeCode]
		private static IntPtr CreateHierarchyViewModel(IntPtr nativePtr, IntPtr flattenedPtr, IntPtr nodesPtr, int nodesCount, int version)
		{
			return GCHandle.ToIntPtr(GCHandle.Alloc(new HierarchyViewModel(nativePtr, HierarchyFlattened.FromIntPtr(flattenedPtr), nodesPtr, nodesCount, version)));
		}

		[RequiredByNativeCode]
		private static void UpdateHierarchyViewModel(IntPtr handlePtr, IntPtr nodesPtr, int nodesCount, int version)
		{
			HierarchyViewModel hierarchyViewModel = HierarchyViewModel.FromIntPtr(handlePtr);
			hierarchyViewModel.m_NodesPtr = nodesPtr;
			hierarchyViewModel.m_NodesCount = nodesCount;
			hierarchyViewModel.m_Version = version;
		}

		[RequiredByNativeCode]
		private static void SearchBegin(IntPtr handlePtr)
		{
			HierarchyViewModel hierarchyViewModel = HierarchyViewModel.FromIntPtr(handlePtr);
			foreach (HierarchyNodeTypeHandlerBase hierarchyNodeTypeHandlerBase in hierarchyViewModel.m_Hierarchy.EnumerateNodeTypeHandlersBase())
			{
				hierarchyNodeTypeHandlerBase.Internal_SearchBegin(hierarchyViewModel.Query);
			}
		}

		[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasFlags(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsAny(flags);
		}

		[Obsolete("HasFlags is obsolete, please use HasAllFlags or HasAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsNode(node, flags);
		}

		[Obsolete("HasFlagsCount is obsolete, please use HasAllFlagsCount or HasAnyFlagsCount instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int HasFlagsCount(HierarchyNodeFlags flags)
		{
			return this.HasAllFlagsCount(flags);
		}

		[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool DoesNotHaveFlags(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsAny(flags);
		}

		[Obsolete("DoesNotHaveFlags is obsolete, please use DoesNotHaveAllFlags or DoesNotHaveAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool DoesNotHaveFlags(in HierarchyNode node, HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsNode(node, flags);
		}

		[Obsolete("DoesNotHaveFlagsCount is obsolete, please use DoesNotHaveAllFlagsCount or DoesNotHaveAnyFlagsCount instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int DoesNotHaveFlagsCount(HierarchyNodeFlags flags)
		{
			return this.DoesNotHaveAllFlagsCount(flags);
		}

		[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetNodesWithFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithAllFlagsSpan(flags, outNodes);
		}

		[Obsolete("GetNodesWithFlags is obsolete, please use GetNodesWithAllFlags or GetNodesWithAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyNode[] GetNodesWithFlags(HierarchyNodeFlags flags)
		{
			return this.GetNodesWithAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("EnumerateNodesWithFlags is obsolete, please use EnumerateNodesWithAllFlags or EnumerateNodesWithAnyFlags instead", false)]
		public HierarchyViewNodesEnumerable EnumerateNodesWithFlags(HierarchyNodeFlags flags)
		{
			return this.EnumerateNodesWithAllFlags(flags);
		}

		[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetIndicesWithFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithAllFlagsSpan(flags, outIndices);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetIndicesWithFlags is obsolete, please use GetIndicesWithAllFlags or GetIndicesWithAnyFlags instead", false)]
		public int[] GetIndicesWithFlags(HierarchyNodeFlags flags)
		{
			return this.GetIndicesWithAllFlags(flags);
		}

		[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int GetNodesWithoutFlags(HierarchyNodeFlags flags, Span<HierarchyNode> outNodes)
		{
			return this.GetNodesWithoutAllFlagsSpan(flags, outNodes);
		}

		[Obsolete("GetNodesWithoutFlags is obsolete, please use GetNodesWithoutAllFlags or GetNodesWithoutAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyNode[] GetNodesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.GetNodesWithoutAllFlags(flags);
		}

		[Obsolete("EnumerateNodesWithoutFlags is obsolete, please use EnumerateNodesWithoutAllFlags or EnumerateNodesWithoutAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public HierarchyViewNodesEnumerable EnumerateNodesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.EnumerateNodesWithoutAllFlags(flags);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead", false)]
		public int GetIndicesWithoutFlags(HierarchyNodeFlags flags, Span<int> outIndices)
		{
			return this.GetIndicesWithoutAllFlagsSpan(flags, outIndices);
		}

		[Obsolete("GetIndicesWithoutFlags is obsolete, please use GetIndicesWithoutAllFlags or GetIndicesWithoutAnyFlags instead", false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int[] GetIndicesWithoutFlags(HierarchyNodeFlags flags)
		{
			return this.GetIndicesWithoutAllFlags(flags);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_Updating_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_UpdateNeeded_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_UpdateProgress_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HierarchySearchQueryDescriptor get_Query_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_Query_Injected(IntPtr _unity_self, HierarchySearchQueryDescriptor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IndexOf_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Contains_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetParent_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetNextSibling_Injected(IntPtr _unity_self, in HierarchyNode node, out HierarchyNode ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCount_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCountRecursive_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetDepth_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HierarchyNodeFlags GetFlags_Injected(IntPtr _unity_self, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int HasAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int HasAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int DoesNotHaveAllFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int DoesNotHaveAnyFlagsCount_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Update_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncremental_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateIncrementalTimed_Injected(IntPtr _unity_self, double milliseconds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(IntPtr handlePtr, IntPtr hierarchyFlattened, HierarchyNodeFlags defaultFlags, out IntPtr nodesPtr, out int nodesCount, out int version);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, bool recurse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SetFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SetFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAllFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAnyFlagsAny_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAllFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesNotHaveAnyFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, bool recurse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClearFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ClearFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsAll_Injected(IntPtr _unity_self, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToggleFlagsNode_Injected(IntPtr _unity_self, in HierarchyNode node, HierarchyNodeFlags flags, bool recurse);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ToggleFlagsNodes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper nodes, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ToggleFlagsIndices_Injected(IntPtr _unity_self, ref ManagedSpanWrapper indices, HierarchyNodeFlags flags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outNodes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithoutAllFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetIndicesWithoutAnyFlagsSpan_Injected(IntPtr _unity_self, HierarchyNodeFlags flags, ref ManagedSpanWrapper outIndices);

		private IntPtr m_Ptr;

		private readonly Hierarchy m_Hierarchy;

		private readonly HierarchyFlattened m_HierarchyFlattened;

		private IntPtr m_NodesPtr;

		private int m_NodesCount;

		private int m_Version;

		private readonly bool m_IsOwner;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(HierarchyViewModel viewModel)
			{
				return viewModel.m_Ptr;
			}
		}

		public struct Enumerator
		{
			internal unsafe Enumerator(HierarchyViewModel hierarchyViewModel)
			{
				this.m_ViewModel = hierarchyViewModel;
				this.m_HierarchyFlattened = hierarchyViewModel.HierarchyFlattened;
				this.m_NodesPtr = (int*)((void*)hierarchyViewModel.m_NodesPtr);
				this.m_NodesCount = hierarchyViewModel.Count;
				this.m_Version = hierarchyViewModel.Version;
				this.m_Index = -1;
			}

			public unsafe ref readonly HierarchyNode Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					bool flag = this.m_Version != this.m_ViewModel.m_Version;
					if (flag)
					{
						throw new InvalidOperationException("HierarchyViewModel was modified.");
					}
					return HierarchyFlattenedNode.GetNodeByRef(this.m_HierarchyFlattened[this.m_NodesPtr[this.m_Index]]);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				int num = this.m_Index + 1;
				this.m_Index = num;
				return num < this.m_NodesCount;
			}

			private readonly HierarchyViewModel m_ViewModel;

			private readonly HierarchyFlattened m_HierarchyFlattened;

			private unsafe readonly int* m_NodesPtr;

			private readonly int m_NodesCount;

			private readonly int m_Version;

			private int m_Index;
		}
	}
}
