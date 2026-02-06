using System;

namespace Photon.Voice
{
	public class ImageBufferNativePool<T> : ObjectPool<T, ImageBufferInfo> where T : ImageBufferNative
	{
		public ImageBufferNativePool(int capacity, ImageBufferNativePool<T>.Factory factory, string name) : base(capacity, name)
		{
			this.factory = factory;
		}

		public ImageBufferNativePool(int capacity, ImageBufferNativePool<T>.Factory factory, string name, ImageBufferInfo info) : base(capacity, name, info)
		{
			this.factory = factory;
		}

		protected override T createObject(ImageBufferInfo info)
		{
			return this.factory(this, info);
		}

		protected override void destroyObject(T obj)
		{
			obj.Dispose();
		}

		protected override bool infosMatch(ImageBufferInfo i0, ImageBufferInfo i1)
		{
			if (i0.Height != i1.Height)
			{
				return false;
			}
			ImageBufferInfo.StrideSet stride = i0.Stride;
			ImageBufferInfo.StrideSet stride2 = i1.Stride;
			if (stride.Length != stride2.Length)
			{
				return false;
			}
			switch (i0.Stride.Length)
			{
			case 1:
				return stride[0] == stride2[0];
			case 2:
				return stride[0] == stride2[0] && stride[1] == stride2[1];
			case 3:
				return stride[0] == stride2[0] && stride[1] == stride2[1] && stride[2] == stride2[2];
			default:
				for (int j = 0; j < stride.Length; j++)
				{
					if (stride[j] != stride2[j])
					{
						return false;
					}
				}
				return true;
			}
		}

		private ImageBufferNativePool<T>.Factory factory;

		public delegate T Factory(ImageBufferNativePool<T> pool, ImageBufferInfo info);
	}
}
