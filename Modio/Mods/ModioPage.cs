using System;

namespace Modio.Mods
{
	public class ModioPage<T>
	{
		internal ModioPage(T[] data, int pageSize, long pageIndex, long totalSearchResults)
		{
			this.Data = data;
			this.PageSize = pageSize;
			this.PageIndex = pageIndex;
			this.TotalSearchResults = totalSearchResults;
		}

		public bool HasMoreResults()
		{
			return (long)this.PageSize * this.PageIndex < this.TotalSearchResults;
		}

		public readonly T[] Data;

		public readonly int PageSize;

		public readonly long PageIndex;

		public readonly long TotalSearchResults;
	}
}
