using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct HeaderImageObject
	{
		[JsonConstructor]
		public HeaderImageObject(string filename, string original)
		{
			this.Filename = filename;
			this.Original = original;
		}

		internal readonly string Filename;

		internal readonly string Original;
	}
}
