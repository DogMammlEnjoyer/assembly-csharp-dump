using System;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace System.IO.MemoryMappedFiles
{
	internal class MemoryMappedView : IDisposable
	{
		[SecurityCritical]
		private MemoryMappedView(SafeMemoryMappedViewHandle viewHandle, long pointerOffset, long size, MemoryMappedFileAccess access)
		{
			this.m_viewHandle = viewHandle;
			this.m_pointerOffset = pointerOffset;
			this.m_size = size;
			this.m_access = access;
		}

		internal SafeMemoryMappedViewHandle ViewHandle
		{
			[SecurityCritical]
			get
			{
				return this.m_viewHandle;
			}
		}

		internal long PointerOffset
		{
			get
			{
				return this.m_pointerOffset;
			}
		}

		internal long Size
		{
			get
			{
				return this.m_size;
			}
		}

		internal MemoryMappedFileAccess Access
		{
			get
			{
				return this.m_access;
			}
		}

		internal static MemoryMappedView Create(IntPtr handle, long offset, long size, MemoryMappedFileAccess access)
		{
			IntPtr mmap_handle;
			IntPtr base_address;
			MemoryMapImpl.Map(handle, offset, ref size, access, out mmap_handle, out base_address);
			return new MemoryMappedView(new SafeMemoryMappedViewHandle(mmap_handle, base_address, size), 0L, size, access);
		}

		public void Flush(IntPtr capacity)
		{
			this.m_viewHandle.Flush();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.m_viewHandle != null && !this.m_viewHandle.IsClosed)
			{
				this.m_viewHandle.Dispose();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal bool IsClosed
		{
			get
			{
				return this.m_viewHandle == null || this.m_viewHandle.IsClosed;
			}
		}

		private SafeMemoryMappedViewHandle m_viewHandle;

		private long m_pointerOffset;

		private long m_size;

		private MemoryMappedFileAccess m_access;
	}
}
