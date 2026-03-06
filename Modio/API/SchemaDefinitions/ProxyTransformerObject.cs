using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ProxyTransformerObject
	{
		[JsonConstructor]
		public ProxyTransformerObject(bool success)
		{
			this.Success = success;
		}

		internal readonly bool Success;
	}
}
