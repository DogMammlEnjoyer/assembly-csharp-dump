using System;
using System.Collections.Generic;

namespace Modio.Images
{
	public class ModioImageSource<TResolution> where TResolution : Enum
	{
		public string FileName { get; private set; }

		internal ModioImageSource(string fileName, params string[] links)
		{
			this.FileName = fileName;
			this._resolutions = new ImageReference[links.Length];
			for (int i = 0; i < this._resolutions.Length; i++)
			{
				this._resolutions[i] = new ImageReference(links[i]);
			}
		}

		public ImageReference GetUri(TResolution resolution)
		{
			int num = (int)((object)resolution);
			num = Math.Min(this._resolutions.Length - 1, num);
			return this._resolutions[num];
		}

		public IEnumerable<ImageReference> GetAllReferences()
		{
			return this._resolutions;
		}

		public void CacheLowestResolutionOnDisk(bool shouldCache)
		{
			if (this._isCachingLowestResolution == shouldCache)
			{
				return;
			}
			this._isCachingLowestResolution = shouldCache;
			if (this._resolutions.Length != 0)
			{
				BaseImageCache.CacheToDisk(this._resolutions[0], shouldCache);
			}
		}

		private readonly ImageReference[] _resolutions;

		private bool _isCachingLowestResolution;
	}
}
