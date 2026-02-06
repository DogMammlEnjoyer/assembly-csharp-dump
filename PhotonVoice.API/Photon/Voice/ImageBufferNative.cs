using System;

namespace Photon.Voice
{
	public class ImageBufferNative
	{
		public ImageBufferNative(ImageBufferInfo info)
		{
			this.Info = info;
			this.Planes = new ImageBufferNative.PlaneSet(info.Stride.Length, (IntPtr)0, (IntPtr)0, (IntPtr)0, (IntPtr)0);
		}

		public ImageBufferNative(IntPtr buf, int width, int height, int stride, ImageFormat imageFormat)
		{
			this.Info = new ImageBufferInfo(width, height, new ImageBufferInfo.StrideSet(1, stride, 0, 0, 0), imageFormat);
			this.Planes = new ImageBufferNative.PlaneSet(1, buf, (IntPtr)0, (IntPtr)0, (IntPtr)0);
		}

		public virtual void Release()
		{
		}

		public virtual void Dispose()
		{
		}

		public ImageBufferInfo Info;

		public ImageBufferNative.PlaneSet Planes;

		public struct PlaneSet
		{
			public PlaneSet(int length, IntPtr p0 = default(IntPtr), IntPtr p1 = default(IntPtr), IntPtr p2 = default(IntPtr), IntPtr p3 = default(IntPtr))
			{
				this.Length = length;
				this.plane0 = p0;
				this.plane1 = p1;
				this.plane2 = p2;
				this.plane3 = p3;
			}

			public IntPtr this[int key]
			{
				get
				{
					switch (key)
					{
					case 0:
						return this.plane0;
					case 1:
						return this.plane1;
					case 2:
						return this.plane2;
					case 3:
						return this.plane3;
					default:
						return IntPtr.Zero;
					}
				}
				set
				{
					switch (key)
					{
					case 0:
						this.plane0 = value;
						return;
					case 1:
						this.plane1 = value;
						return;
					case 2:
						this.plane2 = value;
						return;
					case 3:
						this.plane3 = value;
						return;
					default:
						return;
					}
				}
			}

			public int Length { readonly get; private set; }

			private IntPtr plane0;

			private IntPtr plane1;

			private IntPtr plane2;

			private IntPtr plane3;
		}
	}
}
