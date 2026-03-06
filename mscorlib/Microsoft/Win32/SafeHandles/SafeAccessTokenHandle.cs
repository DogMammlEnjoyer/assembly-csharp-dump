using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle to a Windows thread or process access token. For more information, see Access Tokens.</summary>
	[SecurityCritical]
	public sealed class SafeAccessTokenHandle : SafeHandle
	{
		private SafeAccessTokenHandle() : base(IntPtr.Zero, true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeAccessTokenHandle" /> class.</summary>
		/// <param name="handle">An <see cref="T:System.IntPtr" /> object that represents the pre-existing handle to use. Using <see cref="F:System.IntPtr.Zero" /> returns an invalid handle.</param>
		public SafeAccessTokenHandle(IntPtr handle) : base(IntPtr.Zero, true)
		{
			base.SetHandle(handle);
		}

		/// <summary>Returns an invalid handle by instantiating a <see cref="T:Microsoft.Win32.SafeHandles.SafeAccessTokenHandle" /> object with <see cref="F:System.IntPtr.Zero" />.</summary>
		/// <returns>Returns a <see cref="T:Microsoft.Win32.SafeHandles.SafeAccessTokenHandle" /> object.</returns>
		public static SafeAccessTokenHandle InvalidHandle
		{
			[SecurityCritical]
			get
			{
				return new SafeAccessTokenHandle(IntPtr.Zero);
			}
		}

		/// <summary>Gets a value that indicates whether the handle is invalid.</summary>
		/// <returns>
		///   <see langword="true" /> if the handle is not valid; otherwise, <see langword="false" />.</returns>
		public override bool IsInvalid
		{
			[SecurityCritical]
			get
			{
				return this.handle == IntPtr.Zero || this.handle == new IntPtr(-1);
			}
		}

		[SecurityCritical]
		protected override bool ReleaseHandle()
		{
			return true;
		}
	}
}
