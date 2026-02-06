using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Photon.Voice
{
	public struct FrameBuffer
	{
		public FrameBuffer(byte[] array, int offset, int count, FrameFlags flags, IDisposable disposer)
		{
			this.array = array;
			this.offset = offset;
			this.count = count;
			this.Flags = flags;
			this.disposer = disposer;
			this.disposed = false;
			this.refCnt = 1;
			this.gcHandle = default(GCHandle);
			this.ptr = IntPtr.Zero;
			this.pinned = false;
			if (disposer != null)
			{
				Interlocked.Increment(ref FrameBuffer.statDisposerCreated);
			}
		}

		public FrameBuffer(byte[] array, FrameFlags flags)
		{
			this.array = array;
			this.offset = 0;
			this.count = ((array == null) ? 0 : array.Length);
			this.Flags = flags;
			this.disposer = null;
			this.disposed = false;
			this.refCnt = 1;
			this.gcHandle = default(GCHandle);
			this.ptr = IntPtr.Zero;
			this.pinned = false;
			if (this.disposer != null)
			{
				Interlocked.Increment(ref FrameBuffer.statDisposerCreated);
			}
		}

		public IntPtr Ptr
		{
			get
			{
				if (!this.pinned)
				{
					this.gcHandle = GCHandle.Alloc(this.array, GCHandleType.Pinned);
					this.ptr = IntPtr.Add(this.gcHandle.AddrOfPinnedObject(), this.offset);
					this.pinned = true;
					Interlocked.Increment(ref FrameBuffer.statPinned);
				}
				return this.ptr;
			}
		}

		public void Retain()
		{
			this.refCnt++;
		}

		public void Release()
		{
			this.refCnt--;
			if (this.refCnt <= 0)
			{
				this.Dispose();
			}
		}

		private void Dispose()
		{
			if (this.pinned)
			{
				this.gcHandle.Free();
				this.pinned = false;
				Interlocked.Increment(ref FrameBuffer.statUnpinned);
			}
			if (this.disposer != null && !this.disposed)
			{
				this.disposer.Dispose();
				this.disposed = true;
				Interlocked.Increment(ref FrameBuffer.statDisposerDisposed);
			}
		}

		public byte[] Array
		{
			get
			{
				return this.array;
			}
		}

		public int Length
		{
			get
			{
				return this.count;
			}
		}

		public int Offset
		{
			get
			{
				return this.offset;
			}
		}

		public readonly FrameFlags Flags { get; }

		private readonly byte[] array;

		private readonly int offset;

		private readonly int count;

		private readonly IDisposable disposer;

		private bool disposed;

		private int refCnt;

		private GCHandle gcHandle;

		private IntPtr ptr;

		private bool pinned;

		internal static int statDisposerCreated;

		internal static int statDisposerDisposed;

		internal static int statPinned;

		internal static int statUnpinned;
	}
}
