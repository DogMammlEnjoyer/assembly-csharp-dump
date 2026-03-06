using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameOtherUrlsObject
	{
		[JsonConstructor]
		public GameOtherUrlsObject(string label, string url)
		{
			this.Label = label;
			this.Url = url;
		}

		internal readonly string Label;

		internal readonly string Url;
	}
}
