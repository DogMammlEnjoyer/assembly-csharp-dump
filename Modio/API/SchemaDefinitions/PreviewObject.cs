using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct PreviewObject
	{
		[JsonConstructor]
		public PreviewObject(string resource_url, long date_added, long date_updated)
		{
			this.ResourceUrl = resource_url;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
		}

		internal readonly string ResourceUrl;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;
	}
}
