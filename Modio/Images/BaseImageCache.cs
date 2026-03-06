using System;
using System.Collections.Generic;

namespace Modio.Images
{
	public abstract class BaseImageCache
	{
		public static void CacheToDisk(ImageReference image, bool shouldCache)
		{
			if (!shouldCache)
			{
				ModioClient.DataStorage.DeleteCachedImage(new Uri(image.Url));
				BaseImageCache.PendingDiskSaves.Remove(image);
				return;
			}
			bool flag = false;
			foreach (BaseImageCache baseImageCache in BaseImageCache.ImageCacheInstances)
			{
				flag |= baseImageCache.CacheToDiskInternal(image);
			}
			if (!flag)
			{
				BaseImageCache.PendingDiskSaves.Add(image);
			}
		}

		protected abstract bool CacheToDiskInternal(ImageReference imageReference);

		protected static readonly List<BaseImageCache> ImageCacheInstances = new List<BaseImageCache>();

		protected static readonly HashSet<ImageReference> PendingDiskSaves = new HashSet<ImageReference>();
	}
}
