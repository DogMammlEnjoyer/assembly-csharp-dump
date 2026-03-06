using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
	internal static class HierarchyTestsHelper
	{
		[NativeMethod(IsThreadSafe = true)]
		internal static int GenerateNodesTree(Hierarchy hierarchy, in HierarchyNode root, int width, int depth, int maxCount = 0)
		{
			return HierarchyTestsHelper.GenerateNodesTree_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), root, width, depth, maxCount);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static void GenerateNodesCount(Hierarchy hierarchy, in HierarchyNode root, int count, int width, int depth)
		{
			HierarchyTestsHelper.GenerateNodesCount_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), root, count, width, depth);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static void GenerateSortIndex(Hierarchy hierarchy, in HierarchyNode root, HierarchyTestsHelper.SortOrder order)
		{
			HierarchyTestsHelper.GenerateSortIndex_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), root, order);
		}

		internal unsafe static void ForEach(Hierarchy hierarchy, in HierarchyNode root, HierarchyTestsHelper.ForEachDelegate func)
		{
			Stack<HierarchyNode> stack = new Stack<HierarchyNode>();
			stack.Push(root);
			using (NativeArray<HierarchyNode> nativeArray = new NativeArray<HierarchyNode>(hierarchy.Count, Allocator.Temp, NativeArrayOptions.ClearMemory))
			{
				while (stack.Count > 0)
				{
					HierarchyNode hierarchyNode = stack.Pop();
					int childrenCount = hierarchy.GetChildrenCount(hierarchyNode);
					Span<HierarchyNode> outChildren = new Span<HierarchyNode>(nativeArray.GetUnsafePtr<HierarchyNode>(), childrenCount);
					int children = hierarchy.GetChildren(hierarchyNode, outChildren);
					bool flag = children != childrenCount;
					if (flag)
					{
						throw new InvalidOperationException(string.Format("Expected GetChildren to return {0}, but was {1}.", childrenCount, children));
					}
					int i = 0;
					int length = outChildren.Length;
					while (i < length)
					{
						HierarchyNode item = *outChildren[i];
						func(item, i);
						stack.Push(item);
						i++;
					}
				}
			}
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static void SetNextHierarchyNodeId(Hierarchy hierarchy, int id)
		{
			HierarchyTestsHelper.SetNextHierarchyNodeId_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), id);
		}

		internal static int GetNodeType<T>() where T : HierarchyNodeTypeHandlerBase
		{
			return HierarchyTestsHelper.GetNodeType(typeof(T));
		}

		[NativeMethod(IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNodeType(Type type);

		[NativeMethod(IsThreadSafe = true)]
		internal static int[] GetRegisteredNodeTypes(Hierarchy hierarchy)
		{
			int[] result;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				HierarchyTestsHelper.GetRegisteredNodeTypes_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				result = array;
			}
			return result;
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static int GetCapacity(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static int GetVersion(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetVersion_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static int GetChildrenCapacity(Hierarchy hierarchy, in HierarchyNode node)
		{
			return HierarchyTestsHelper.GetChildrenCapacity_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy), node);
		}

		internal static bool SearchMatch(HierarchyViewModel model, in HierarchyNode node)
		{
			HierarchyNodeTypeHandlerBase nodeTypeHandlerBase = model.Hierarchy.GetNodeTypeHandlerBase(node);
			return nodeTypeHandlerBase != null && nodeTypeHandlerBase.Internal_SearchMatch(node);
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyScriptingObject(Hierarchy hierarchy)
		{
			return HierarchyTestsHelper.GetHierarchyScriptingObject_Injected((hierarchy == null) ? ((IntPtr)0) : Hierarchy.BindingsMarshaller.ConvertToNative(hierarchy));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyFlattenedScriptingObject(HierarchyFlattened hierarchyFlattened)
		{
			return HierarchyTestsHelper.GetHierarchyFlattenedScriptingObject_Injected((hierarchyFlattened == null) ? ((IntPtr)0) : HierarchyFlattened.BindingsMarshaller.ConvertToNative(hierarchyFlattened));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyViewModelScriptingObject(HierarchyViewModel viewModel)
		{
			return HierarchyTestsHelper.GetHierarchyViewModelScriptingObject_Injected((viewModel == null) ? ((IntPtr)0) : HierarchyViewModel.BindingsMarshaller.ConvertToNative(viewModel));
		}

		[NativeMethod(IsThreadSafe = true)]
		internal static object GetHierarchyCommandListScriptingObject(HierarchyCommandList cmdList)
		{
			return HierarchyTestsHelper.GetHierarchyCommandListScriptingObject_Injected((cmdList == null) ? ((IntPtr)0) : HierarchyCommandList.BindingsMarshaller.ConvertToNative(cmdList));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GenerateNodesTree_Injected(IntPtr hierarchy, in HierarchyNode root, int width, int depth, int maxCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateNodesCount_Injected(IntPtr hierarchy, in HierarchyNode root, int count, int width, int depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GenerateSortIndex_Injected(IntPtr hierarchy, in HierarchyNode root, HierarchyTestsHelper.SortOrder order);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNextHierarchyNodeId_Injected(IntPtr hierarchy, int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetRegisteredNodeTypes_Injected(IntPtr hierarchy, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetCapacity_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetVersion_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetChildrenCapacity_Injected(IntPtr hierarchy, in HierarchyNode node);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyScriptingObject_Injected(IntPtr hierarchy);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyFlattenedScriptingObject_Injected(IntPtr hierarchyFlattened);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyViewModelScriptingObject_Injected(IntPtr viewModel);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetHierarchyCommandListScriptingObject_Injected(IntPtr cmdList);

		[NativeHeader("Modules/HierarchyCore/HierarchyTestsHelper.h")]
		internal enum SortOrder
		{
			Ascending,
			Descending
		}

		internal delegate void ForEachDelegate(in HierarchyNode node, int index);
	}
}
