using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle that represents a key (NCRYPT_KEY_HANDLE).</summary>
	public sealed class SafeNCryptKeyHandle : SafeNCryptHandle
	{
		/// <summary>Initializes a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptKeyHandle" /> class.</summary>
		public SafeNCryptKeyHandle()
		{
		}

		/// <summary>Instantiates a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptKeyHandle" /> class. </summary>
		/// <param name="handle">The pre-existing handle to use. Using <see cref="F:System.IntPtr.Zero" /> returns an invalid handle. </param>
		/// <param name="parentHandle">The parent handle of this <see cref="T:Microsoft.Win32.SafeHandles.SafeNCryptKeyHandle" />. </param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="parentHandle" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="parentHandle" /> is closed. -or-<paramref name="parentHandle" /> is invalid. </exception>
		public SafeNCryptKeyHandle(IntPtr handle, SafeHandle parentHandle) : base(handle, parentHandle)
		{
		}

		protected override bool ReleaseNativeHandle()
		{
			return false;
		}
	}
}
