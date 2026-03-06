using System;

namespace Modio.API
{
	public abstract class SearchFilter<T> : SearchFilter where T : SearchFilter<T>
	{
		protected SearchFilter(int pageIndex, int pageSize) : base(pageIndex, pageSize)
		{
		}

		public T SetPagination(int pageIndex, int pageSize = 100)
		{
			this.PageIndex = pageIndex;
			this.PageSize = pageSize;
			return this as T;
		}
	}
}
