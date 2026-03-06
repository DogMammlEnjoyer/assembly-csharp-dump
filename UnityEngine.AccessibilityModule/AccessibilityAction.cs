using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityAction.h")]
	[StructLayout(LayoutKind.Sequential)]
	internal sealed class AccessibilityAction : IDisposable
	{
		public AccessibilityAction()
		{
			this.m_Ptr = AccessibilityAction.Internal_Create(this);
		}

		public AccessibilityAction(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		~AccessibilityAction()
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
				AccessibilityAction.Internal_Destroy(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create([Unmarshalled] AccessibilityAction self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		public int id
		{
			get
			{
				IntPtr intPtr = AccessibilityAction.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AccessibilityAction.get_id_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AccessibilityAction.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AccessibilityAction.set_id_Injected(intPtr, value);
			}
		}

		public unsafe string label
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = AccessibilityAction.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpan;
					AccessibilityAction.get_label_Injected(intPtr, out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = AccessibilityAction.BindingsMarshaller.ConvertToNative(this);
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
					AccessibilityAction.set_label_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public Func<bool> activated { get; set; }

		[RequiredByNativeCode]
		private bool Internal_InvokeActivated()
		{
			return this.activated != null && this.activated();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_id_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_id_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_label_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_label_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		private IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(AccessibilityAction obj)
			{
				return obj.m_Ptr;
			}

			public static AccessibilityAction ConvertToManaged(IntPtr ptr)
			{
				return new AccessibilityAction(ptr);
			}
		}
	}
}
