using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct MetadataKvpObject
	{
		[JsonConstructor]
		public MetadataKvpObject(string metakey, string metavalue)
		{
			this.Metakey = metakey;
			this.Metavalue = metavalue;
		}

		internal readonly string Metakey;

		internal readonly string Metavalue;
	}
}
