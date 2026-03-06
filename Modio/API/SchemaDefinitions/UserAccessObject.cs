using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UserAccessObject
	{
		[JsonConstructor]
		public UserAccessObject(string resource_type, long resource_id, long resource_name, string resource_name_id, string resource_url)
		{
			this.ResourceType = resource_type;
			this.ResourceId = resource_id;
			this.ResourceName = resource_name;
			this.ResourceNameId = resource_name_id;
			this.ResourceUrl = resource_url;
		}

		internal readonly string ResourceType;

		internal readonly long ResourceId;

		internal readonly long ResourceName;

		internal readonly string ResourceNameId;

		internal readonly string ResourceUrl;
	}
}
