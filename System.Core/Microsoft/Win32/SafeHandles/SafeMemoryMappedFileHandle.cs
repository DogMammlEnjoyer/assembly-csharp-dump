using System;
using System.IO.MemoryMappedFiles;
using Unity;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle that represents a memory-mapped file for sequential access.</summary>
	public sealed class SafeMemoryMappedFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeMemoryMappedFileHandle(IntPtr preexistingHandle, bool ownsHandle) : base(ownsHandle)
		{
			this.handle = preexistingHandle;
		}

		protected override bool ReleaseHandle()
		{
			MemoryMapImpl.CloseMapping(this.handle);
			this.handle = IntPtr.Zero;
			return true;
		}

		internal SafeMemoryMappedFileHandle()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
