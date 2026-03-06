using System;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Represents a wrapper class for a pipe handle. </summary>
	public sealed class SafePipeHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		protected override bool ReleaseHandle()
		{
			return Interop.Kernel32.CloseHandle(this.handle);
		}

		internal SafePipeHandle() : this(new IntPtr(0), true)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:Microsoft.Win32.SafeHandles.SafePipeHandle" /> class.</summary>
		/// <param name="preexistingHandle">An <see cref="T:System.IntPtr" /> object that represents the pre-existing handle to use.</param>
		/// <param name="ownsHandle">
		///       <see langword="true" /> to reliably release the handle during the finalization phase; <see langword="false" /> to prevent reliable release (not recommended).</param>
		public SafePipeHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		internal void SetHandle(int descriptor)
		{
			base.SetHandle((IntPtr)descriptor);
		}

		private const int DefaultInvalidHandle = 0;
	}
}
