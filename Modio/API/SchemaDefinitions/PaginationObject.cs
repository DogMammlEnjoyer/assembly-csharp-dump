using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PaginationObject
	{
		[JsonConstructor]
		public PaginationObject(long per_page, string current_page, long next_page_url, long prev_page_url)
		{
			this.PerPage = per_page;
			this.CurrentPage = current_page;
			this.NextPageUrl = next_page_url;
			this.PrevPageUrl = prev_page_url;
		}

		internal readonly long PerPage;

		internal readonly string CurrentPage;

		internal readonly long NextPageUrl;

		internal readonly long PrevPageUrl;
	}
}
