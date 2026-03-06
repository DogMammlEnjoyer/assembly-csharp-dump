using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.Images;
using UnityEngine;

namespace Modio.Unity
{
	public static class ModioImageTexture2DExtensions
	{
		[return: TupleElementNames(new string[]
		{
			"error",
			"texture"
		})]
		public static Task<ValueTuple<Error, Texture2D>> DownloadAsTexture2D(this ImageReference imageReference)
		{
			return ImageCacheTexture2D.Instance.DownloadImage(imageReference);
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"texture"
		})]
		public static Task<ValueTuple<Error, Texture2D>> DownloadAsTexture2D<TResolution>(this ModioImageSource<TResolution> imageSource, TResolution resolution) where TResolution : Enum
		{
			return ImageCacheTexture2D.Instance.DownloadImage(imageSource.GetUri(resolution));
		}
	}
}
