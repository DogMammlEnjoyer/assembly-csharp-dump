using System;

namespace System.Threading
{
	/// <summary>Represents pre-allocated state for native overlapped I/O operations.</summary>
	public sealed class PreAllocatedOverlapped : IDisposable, IDeferredDisposable
	{
		static PreAllocatedOverlapped()
		{
			if (!Environment.IsRunningOnWindows)
			{
				throw new PlatformNotSupportedException();
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Threading.PreAllocatedOverlapped" /> class and specifies a delegate to invoke when each asynchronous I/O operation is complete, a user-provided object that provides context, and managed objects that serve as buffers.</summary>
		/// <param name="callback">A delegate that represents the callback method to invoke when each asynchronous I/O operation completes.</param>
		/// <param name="state">A user-supplied object that distinguishes the <see cref="T:System.Threading.NativeOverlapped" /> instance produced from this object from other <see cref="T:System.Threading.NativeOverlapped" /> instances. Its value can be <see langword="null" />.</param>
		/// <param name="pinData">An object or array of objects that represent the input or output buffer for the operations. Each object represents a buffer, such as an array of bytes. Its value can be <see langword="null" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="callback" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ObjectDisposedException">This method was called after the <see cref="T:System.Threading.ThreadPoolBoundHandle" /> was disposed.</exception>
		[CLSCompliant(false)]
		public PreAllocatedOverlapped(IOCompletionCallback callback, object state, object pinData)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			this._overlapped = Win32ThreadPoolNativeOverlapped.Allocate(callback, state, pinData, this);
		}

		internal bool AddRef()
		{
			return this._lifetime.AddRef(this);
		}

		internal void Release()
		{
			this._lifetime.Release(this);
		}

		/// <summary>Frees the resources associated with this <see cref="T:System.Threading.PreAllocatedOverlapped" /> instance.</summary>
		public void Dispose()
		{
			this._lifetime.Dispose(this);
			GC.SuppressFinalize(this);
		}

		/// <summary>Frees unmanaged resources before the current instance is reclaimed by garbage collection.</summary>
		~PreAllocatedOverlapped()
		{
			if (!Environment.HasShutdownStarted)
			{
				this.Dispose();
			}
		}

		unsafe void IDeferredDisposable.OnFinalRelease(bool disposed)
		{
			if (this._overlapped != null)
			{
				if (disposed)
				{
					Win32ThreadPoolNativeOverlapped.Free(this._overlapped);
					return;
				}
				*Win32ThreadPoolNativeOverlapped.ToNativeOverlapped(this._overlapped) = default(NativeOverlapped);
			}
		}

		internal unsafe readonly Win32ThreadPoolNativeOverlapped* _overlapped;

		private DeferredDisposableLifetime<PreAllocatedOverlapped> _lifetime;
	}
}
