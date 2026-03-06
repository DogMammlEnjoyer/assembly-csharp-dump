using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ModUserPreviewObject
	{
		[JsonConstructor]
		public ModUserPreviewObject(UserObject user, UserObject user_from, string resource_url, bool subscribed, long date_added)
		{
			this.User = user;
			this.UserFrom = user_from;
			this.ResourceUrl = resource_url;
			this.Subscribed = subscribed;
			this.DateAdded = date_added;
		}

		internal readonly UserObject User;

		internal readonly UserObject UserFrom;

		internal readonly string ResourceUrl;

		internal readonly bool Subscribed;

		internal readonly long DateAdded;
	}
}
