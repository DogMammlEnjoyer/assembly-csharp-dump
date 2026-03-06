using System;
using System.Runtime.InteropServices;

namespace Unity.Media
{
	public class RefHandle<T> : IDisposable where T : class
	{
		public bool IsCreated
		{
			get
			{
				return this.m_Handle.IsAllocated;
			}
		}

		public T Target
		{
			get
			{
				if (!this.IsCreated)
				{
					return default(T);
				}
				return this.m_Handle.Target as T;
			}
			set
			{
				if (this.IsCreated)
				{
					this.m_Handle.Free();
				}
				if (value != null)
				{
					this.m_Handle = GCHandle.Alloc(value, GCHandleType.Normal);
				}
			}
		}

		public RefHandle()
		{
		}

		public RefHandle(T target)
		{
			this.m_Handle = default(GCHandle);
			this.Target = target;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (this.Disposed)
			{
				return;
			}
			if (this.IsCreated)
			{
				this.m_Handle.Free();
			}
			this.Disposed = true;
		}

		~RefHandle()
		{
			this.Dispose(false);
		}

		private GCHandle m_Handle;

		private bool Disposed;
	}
}
