using System;
using System.Runtime.InteropServices;

public class OVRNativeBuffer : IDisposable
{
	public OVRNativeBuffer(int numBytes)
	{
		this.Reallocate(numBytes);
	}

	~OVRNativeBuffer()
	{
		this.Dispose(false);
	}

	public void Reset(int numBytes)
	{
		this.Reallocate(numBytes);
	}

	public int GetCapacity()
	{
		return this.m_numBytes;
	}

	public IntPtr GetPointer(int byteOffset = 0)
	{
		if (byteOffset < 0 || byteOffset >= this.m_numBytes)
		{
			return IntPtr.Zero;
		}
		if (byteOffset != 0)
		{
			return new IntPtr(this.m_ptr.ToInt64() + (long)byteOffset);
		}
		return this.m_ptr;
	}

	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (this.disposed)
		{
			return;
		}
		this.Release();
		this.disposed = true;
	}

	protected void Reallocate(int numBytes)
	{
		this.Release();
		if (numBytes > 0)
		{
			this.m_ptr = Marshal.AllocHGlobal(numBytes);
			this.m_numBytes = numBytes;
			return;
		}
		this.m_ptr = IntPtr.Zero;
		this.m_numBytes = 0;
	}

	protected void Release()
	{
		if (this.m_ptr != IntPtr.Zero)
		{
			Marshal.FreeHGlobal(this.m_ptr);
			this.m_ptr = IntPtr.Zero;
			this.m_numBytes = 0;
		}
	}

	protected bool disposed;

	protected int m_numBytes;

	protected IntPtr m_ptr = IntPtr.Zero;
}
