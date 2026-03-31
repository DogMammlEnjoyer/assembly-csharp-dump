using System;
using System.Threading;

namespace K4os.Compression.LZ4.Internal
{
	public abstract class UnmanagedResources : IDisposable
	{
		public bool IsDisposed
		{
			get
			{
				return Interlocked.CompareExchange(ref this._disposed, 0, 0) != 0;
			}
		}

		protected void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName + " is already disposed");
			}
		}

		protected virtual void ReleaseUnmanaged()
		{
		}

		protected virtual void ReleaseManaged()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref this._disposed, 1, 0) != 0)
			{
				return;
			}
			this.ReleaseUnmanaged();
			if (disposing)
			{
				this.ReleaseManaged();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~UnmanagedResources()
		{
			this.Dispose(false);
		}

		private int _disposed;
	}
}
