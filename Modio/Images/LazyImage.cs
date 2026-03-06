using System;
using System.Runtime.CompilerServices;

namespace Modio.Images
{
	public class LazyImage<TImage> where TImage : class
	{
		public event Action<TImage> OnNewImageAvailable;

		public event Action<bool> OnLoadingActive;

		public LazyImage(BaseImageCache<TImage> imageCache, Action<TImage> onImageAvailable = null, Action<bool> onLoadingActive = null)
		{
			this._imageCache = imageCache;
			this.OnNewImageAvailable = onImageAvailable;
			this.OnLoadingActive = onLoadingActive;
		}

		public void SetImage<T>(ModioImageSource<T> source, T resolution) where T : Enum
		{
			LazyImage<TImage>.<SetImage>d__10<T> <SetImage>d__;
			<SetImage>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetImage>d__.<>4__this = this;
			<SetImage>d__.source = source;
			<SetImage>d__.resolution = resolution;
			<SetImage>d__.<>1__state = -1;
			<SetImage>d__.<>t__builder.Start<LazyImage<TImage>.<SetImage>d__10<T>>(ref <SetImage>d__);
		}

		private void ApplyImage(TImage cachedImage)
		{
			Action<TImage> onNewImageAvailable = this.OnNewImageAvailable;
			if (onNewImageAvailable == null)
			{
				return;
			}
			onNewImageAvailable(cachedImage);
		}

		private ImageReference _currentImageReference;

		private BaseImageCache<TImage> _imageCache;

		private bool _failedToLoad;
	}
}
