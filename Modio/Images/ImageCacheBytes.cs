using System;

namespace Modio.Images
{
	public class ImageCacheBytes : BaseImageCache<byte[]>
	{
		protected override byte[] Convert(byte[] rawBytes)
		{
			return rawBytes;
		}

		protected override byte[] ConvertToBytes(byte[] image)
		{
			return image;
		}

		public static readonly ImageCacheBytes Instance = new ImageCacheBytes();
	}
}
