using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct FilehashObject
	{
		[JsonConstructor]
		public FilehashObject(string md5)
		{
			this.Md5 = md5;
		}

		internal readonly string Md5;
	}
}
