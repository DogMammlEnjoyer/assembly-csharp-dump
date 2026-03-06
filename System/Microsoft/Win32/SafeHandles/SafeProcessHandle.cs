using System;
using System.Security;
using System.Security.Permissions;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a managed wrapper for a process handle.</summary>
	[SuppressUnmanagedCodeSecurity]
	public sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeProcessHandle() : base(true)
		{
		}

		internal SafeProcessHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		/// <summary>Initializes a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeProcessHandle" /> class from the specified handle, indicating whether to release the handle during the finalization phase.</summary>
		/// <param name="existingHandle">The handle to be wrapped.</param>
		/// <param name="ownsHandle">
		///   <see langword="true" /> to reliably let <see cref="T:Microsoft.Win32.SafeHandles.SafeProcessHandle" /> release the handle during the finalization phase; otherwise, <see langword="false" />.</param>
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		public SafeProcessHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(existingHandle);
		}

		internal void InitialSetHandle(IntPtr h)
		{
			this.handle = h;
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.CloseProcess(this.handle);
		}

		internal static SafeProcessHandle InvalidHandle = new SafeProcessHandle(IntPtr.Zero);
	}
}
