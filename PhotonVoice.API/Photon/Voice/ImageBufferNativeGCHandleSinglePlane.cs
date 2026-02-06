using System;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
	public class ImageBufferNativeGCHandleSinglePlane : ImageBufferNative, IDisposable
	{
		public ImageBufferNativeGCHandleSinglePlane(ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool, ImageBufferInfo info) : base(info)
		{
			if (info.Stride.Length != 1)
			{
				throw new Exception("ImageBufferNativeGCHandleSinglePlane wrong plane count " + info.Stride.Length.ToString());
			}
			this.pool = pool;
		}

		public void PinPlane(byte[] plane)
		{
			this.planeHandle = GCHandle.Alloc(plane, GCHandleType.Pinned);
			this.Planes[0] = this.planeHandle.AddrOfPinnedObject();
		}

		public override void Release()
		{
			this.planeHandle.Free();
			if (this.pool != null)
			{
				this.pool.Release(this);
			}
		}

		public override void Dispose()
		{
		}

		private ImageBufferNativePool<ImageBufferNativeGCHandleSinglePlane> pool;

		private GCHandle planeHandle;
	}
}
