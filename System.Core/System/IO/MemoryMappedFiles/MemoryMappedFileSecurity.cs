using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using Microsoft.Win32.SafeHandles;

namespace System.IO.MemoryMappedFiles
{
	/// <summary>Represents the permissions that can be granted for file access and operations on memory-mapped files. </summary>
	public class MemoryMappedFileSecurity : ObjectSecurity<MemoryMappedFileRights>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.MemoryMappedFiles.MemoryMappedFileSecurity" /> class. </summary>
		public MemoryMappedFileSecurity() : base(false, ResourceType.KernelObject)
		{
		}

		[SecuritySafeCritical]
		internal MemoryMappedFileSecurity(SafeMemoryMappedFileHandle safeHandle, AccessControlSections includeSections) : base(false, ResourceType.KernelObject, safeHandle, includeSections)
		{
		}

		[SecuritySafeCritical]
		internal void PersistHandle(SafeHandle handle)
		{
			base.Persist(handle);
		}
	}
}
