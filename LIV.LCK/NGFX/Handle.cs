using System;
using System.Runtime.InteropServices;

namespace Liv.NGFX
{
	public class Handle<T> : IDisposable
	{
		public Handle(T data)
		{
			this.m_data = data;
			this.m_handle = GCHandle.Alloc(this.m_data, GCHandleType.Pinned);
			this.m_valid = true;
		}

		~Handle()
		{
			this.Dispose();
		}

		public IntPtr ptr()
		{
			return this.m_handle.AddrOfPinnedObject();
		}

		public T data()
		{
			return this.m_data;
		}

		public void Dispose()
		{
			if (this.m_valid)
			{
				this.m_handle.Free();
			}
			this.m_valid = false;
		}

		private T m_data;

		private GCHandle m_handle;

		private bool m_valid;
	}
}
