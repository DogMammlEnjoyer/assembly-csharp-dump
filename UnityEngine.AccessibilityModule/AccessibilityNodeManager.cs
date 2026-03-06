using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeManager.h")]
	internal static class AccessibilityNodeManager
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CreateNativeNode(int id);

		internal static bool CreateNativeNodeWithData(AccessibilityNodeData nodeData)
		{
			return AccessibilityNodeManager.CreateNativeNodeWithData_Injected(ref nodeData);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyNativeNode(int id, int parentId);

		internal static void SetNodeData(int id, AccessibilityNodeData nodeData)
		{
			AccessibilityNodeManager.SetNodeData_Injected(id, ref nodeData);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetIsActive(int id, bool isActive);

		internal unsafe static void SetLabel(int id, string label)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(label, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = label.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetLabel_Injected(id, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		internal unsafe static void SetValue(int id, string value)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetValue_Injected(id, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		internal unsafe static void SetHint(int id, string hint)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(hint, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = hint.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				AccessibilityNodeManager.SetHint_Injected(id, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetRole(int id, AccessibilityRole role);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAllowsDirectInteraction(int id, bool allows);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetState(int id, AccessibilityState state);

		internal static void SetFrame(int id, Rect frame)
		{
			AccessibilityNodeManager.SetFrame_Injected(id, ref frame);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetParent(int id, int parentId, int index = -1);

		internal unsafe static void SetChildren(int id, int[] childIds)
		{
			Span<int> span = new Span<int>(childIds);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				AccessibilityNodeManager.SetChildren_Injected(id, ref managedSpanWrapper);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool GetIsFocused(int id);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetActions(int id, AccessibilityAction[] actions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetLanguage(int id, SystemLanguage language);

		[RequiredByNativeCode]
		internal static void Internal_InvokeFocusChanged(int id, bool isNodeFocused)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			if (!flag)
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				if (flag2)
				{
					accessibilityNode.NotifyFocusChanged(isNodeFocused);
				}
			}
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeSelected(int id)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				result = (flag2 && accessibilityNode.InvokeSelected());
			}
			return result;
		}

		[RequiredByNativeCode]
		internal static void Internal_InvokeIncremented(int id)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			if (!flag)
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				if (flag2)
				{
					accessibilityNode.InvokeIncremented();
				}
			}
		}

		[RequiredByNativeCode]
		internal static void Internal_InvokeDecremented(int id)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			if (!flag)
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				if (flag2)
				{
					accessibilityNode.InvokeDecremented();
				}
			}
		}

		[RequiredByNativeCode]
		internal static bool Internal_InvokeDismissed(int id)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				result = (flag2 && accessibilityNode.Dismissed());
			}
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateNativeNodeWithData_Injected([In] ref AccessibilityNodeData nodeData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetNodeData_Injected(int id, [In] ref AccessibilityNodeData nodeData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLabel_Injected(int id, ref ManagedSpanWrapper label);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetValue_Injected(int id, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetHint_Injected(int id, ref ManagedSpanWrapper hint);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetFrame_Injected(int id, [In] ref Rect frame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetChildren_Injected(int id, ref ManagedSpanWrapper childIds);

		internal const int k_InvalidNodeId = -1;
	}
}
