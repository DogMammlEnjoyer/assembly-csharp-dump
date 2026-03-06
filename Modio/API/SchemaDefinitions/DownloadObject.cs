using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct DownloadObject
	{
		[JsonConstructor]
		internal DownloadObject(string binary_url, long date_expires)
		{
			this.BinaryUrl = binary_url;
			this.DateExpires = date_expires;
		}

		internal readonly string BinaryUrl;

		internal readonly long DateExpires;
	}
}
