using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Modio.Images
{
	public abstract class BaseImageCache<T> : BaseImageCache where T : class
	{
		protected BaseImageCache()
		{
			BaseImageCache.ImageCacheInstances.Add(this);
		}

		public T GetCachedImage(ImageReference uri)
		{
			ValueTuple<Error, T> valueTuple;
			this._cache.TryGetValue(uri, out valueTuple);
			return valueTuple.Item2;
		}

		[return: TupleElementNames(new string[]
		{
			"errror",
			"image"
		})]
		public Task<ValueTuple<Error, T>> DownloadImage(ImageReference uri)
		{
			ValueTuple<Error, T> result;
			if (this._cache.TryGetValue(uri, out result))
			{
				return Task.FromResult<ValueTuple<Error, T>>(result);
			}
			Task<ValueTuple<Error, T>> result2;
			if (this._ongoingDownloads.TryGetValue(uri, out result2))
			{
				return result2;
			}
			Task<ValueTuple<Error, T>> task = this.DownloadImageInternal(uri);
			if (!task.IsCompleted)
			{
				this._ongoingDownloads[uri] = task;
			}
			return task;
		}

		private Task<ValueTuple<Error, T>> DownloadImageInternal(ImageReference uri)
		{
			BaseImageCache<T>.<DownloadImageInternal>d__5 <DownloadImageInternal>d__;
			<DownloadImageInternal>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, T>>.Create();
			<DownloadImageInternal>d__.<>4__this = this;
			<DownloadImageInternal>d__.uri = uri;
			<DownloadImageInternal>d__.<>1__state = -1;
			<DownloadImageInternal>d__.<>t__builder.Start<BaseImageCache<T>.<DownloadImageInternal>d__5>(ref <DownloadImageInternal>d__);
			return <DownloadImageInternal>d__.<>t__builder.Task;
		}

		private Task<T> LoadFromDiskCache(ImageReference imageReference)
		{
			BaseImageCache<T>.<LoadFromDiskCache>d__6 <LoadFromDiskCache>d__;
			<LoadFromDiskCache>d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
			<LoadFromDiskCache>d__.<>4__this = this;
			<LoadFromDiskCache>d__.imageReference = imageReference;
			<LoadFromDiskCache>d__.<>1__state = -1;
			<LoadFromDiskCache>d__.<>t__builder.Start<BaseImageCache<T>.<LoadFromDiskCache>d__6>(ref <LoadFromDiskCache>d__);
			return <LoadFromDiskCache>d__.<>t__builder.Task;
		}

		public T GetFirstCachedImage(IEnumerable<ImageReference> imageReferences)
		{
			foreach (ImageReference uri in imageReferences)
			{
				T cachedImage = this.GetCachedImage(uri);
				if (cachedImage != null)
				{
					return cachedImage;
				}
			}
			return default(T);
		}

		protected override bool CacheToDiskInternal(ImageReference imageReference)
		{
			Uri serverPath = new Uri(imageReference.Url);
			T cachedImage = this.GetCachedImage(imageReference);
			if (cachedImage == null)
			{
				return false;
			}
			byte[] data = this.ConvertToBytes(cachedImage);
			ModioClient.DataStorage.WriteCachedImage(serverPath, data);
			return true;
		}

		protected abstract T Convert(byte[] rawBytes);

		protected abstract byte[] ConvertToBytes(T image);

		private readonly Dictionary<ImageReference, ValueTuple<Error, T>> _cache = new Dictionary<ImageReference, ValueTuple<Error, T>>();

		private readonly Dictionary<ImageReference, Task<ValueTuple<Error, T>>> _ongoingDownloads = new Dictionary<ImageReference, Task<ValueTuple<Error, T>>>();
	}
}
