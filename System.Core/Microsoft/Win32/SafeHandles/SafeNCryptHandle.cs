using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle that can be used by Cryptography Next Generation (CNG) objects.</summary>
	public abstract class SafeNCryptHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		/// <summary>Initializes a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptHandle" /> class.</summary>
		protected SafeNCryptHandle() : base(true)
		{
		}

		/// <summary>Instantiates a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptHandle" /> class. </summary>
		/// <param name="handle">The pre-existing handle to use. Using <see cref="F:System.IntPtr.Zero" /> returns an invalid handle. </param>
		/// <param name="parentHandle">The parent handle of this <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptHandle" />. </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="parentHandle" /> is <see langword="null" />.  </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="parentHandle" /> is closed. -or-<paramref name="parentHandle" /> is invalid. </exception>
		protected SafeNCryptHandle(IntPtr handle, SafeHandle parentHandle) : base(false)
		{
			throw new NotImplementedException();
		}

		public override bool IsInvalid
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Releases a handle used by a Cryptography Next Generation (CNG) object.</summary>
		/// <returns>
		///     <see langword="true" /> if the handle is released successfully; otherwise, <see langword="false" />.</returns>
		protected override bool ReleaseHandle()
		{
			return false;
		}

		/// <summary>Releases a native handle used by a Cryptography Next Generation (CNG) object.</summary>
		/// <returns>
		///     <see langword="true" /> if the handle is released successfully; otherwise, <see langword="false" />.</returns>
		protected abstract bool ReleaseNativeHandle();
	}
}
