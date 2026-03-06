using System;
using System.Collections.Generic;

namespace Modio.API
{
	public abstract class SearchFilter
	{
		protected SearchFilter(int pageIndex, int pageSize)
		{
			this.Parameters = new Dictionary<string, object>();
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
		}

		internal int PageIndex;

		internal int PageSize;

		internal readonly Dictionary<string, object> Parameters;
	}
}
