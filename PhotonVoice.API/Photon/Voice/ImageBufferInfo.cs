using System;

namespace Photon.Voice
{
	public struct ImageBufferInfo
	{
		public readonly int Width { get; }

		public readonly int Height { get; }

		public readonly ImageBufferInfo.StrideSet Stride { get; }

		public readonly ImageFormat Format { get; }

		public Rotation Rotation { readonly get; set; }

		public Flip Flip { readonly get; set; }

		public ImageBufferInfo(int width, int height, ImageBufferInfo.StrideSet stride, ImageFormat format)
		{
			this.Width = width;
			this.Height = height;
			this.Stride = stride;
			this.Format = format;
			this.Rotation = Rotation.Rotate0;
			this.Flip = Flip.None;
		}

		public struct StrideSet
		{
			public StrideSet(int length, int s0 = 0, int s1 = 0, int s2 = 0, int s3 = 0)
			{
				this.Length = length;
				this.stride0 = s0;
				this.stride1 = s1;
				this.stride2 = s2;
				this.stride3 = s3;
			}

			public int this[int key]
			{
				get
				{
					switch (key)
					{
					case 0:
						return this.stride0;
					case 1:
						return this.stride1;
					case 2:
						return this.stride2;
					case 3:
						return this.stride3;
					default:
						return 0;
					}
				}
				set
				{
					switch (key)
					{
					case 0:
						this.stride0 = value;
						return;
					case 1:
						this.stride1 = value;
						return;
					case 2:
						this.stride2 = value;
						return;
					case 3:
						this.stride3 = value;
						return;
					default:
						return;
					}
				}
			}

			public int Length { readonly get; private set; }

			private int stride0;

			private int stride1;

			private int stride2;

			private int stride3;
		}
	}
}
