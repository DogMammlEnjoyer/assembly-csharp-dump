using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct KeyValuePairObject
	{
		[JsonConstructor]
		public KeyValuePairObject(string key, string value)
		{
			this.Key = key;
			this.Value = value;
		}

		internal readonly string Key;

		internal readonly string Value;
	}
}
