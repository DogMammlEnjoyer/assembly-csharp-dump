using System;
using System.Runtime.InteropServices;

namespace Photon.Voice
{
	public class ImageBufferNativeAlloc : ImageBufferNative, IDisposable
	{
		public ImageBufferNativeAlloc(ImageBufferNativePool<ImageBufferNativeAlloc> pool, ImageBufferInfo info) : base(info)
		{
			this.pool = pool;
			for (int i = 0; i < info.Stride.Length; i++)
			{
				this.Planes[i] = Marshal.AllocHGlobal(info.Stride[i] * info.Height);
			}
		}

		public override void Release()
		{
			if (this.pool != null)
			{
				this.pool.Release(this);
			}
		}

		public override void Dispose()
		{
			for (int i = 0; i < this.Info.Stride.Length; i++)
			{
				Marshal.FreeHGlobal(this.Planes[i]);
			}
		}

		private ImageBufferNativePool<ImageBufferNativeAlloc> pool;
	}
}
