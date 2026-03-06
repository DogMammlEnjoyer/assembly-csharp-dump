using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameUserPreviewObject
	{
		[JsonConstructor]
		public GameUserPreviewObject(UserObject user, UserObject user_from, string resource_url, long date_added)
		{
			this.User = user;
			this.UserFrom = user_from;
			this.ResourceUrl = resource_url;
			this.DateAdded = date_added;
		}

		internal readonly UserObject User;

		internal readonly UserObject UserFrom;

		internal readonly string ResourceUrl;

		internal readonly long DateAdded;
	}
}
