using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct ModEventObject
	{
		[JsonConstructor]
		public ModEventObject(long id, long mod_id, long user_id, long date_added, string event_type)
		{
			this.Id = id;
			this.ModId = mod_id;
			this.UserId = user_id;
			this.DateAdded = date_added;
			this.EventType = event_type;
		}

		internal readonly long Id;

		internal readonly long ModId;

		internal readonly long UserId;

		internal readonly long DateAdded;

		internal readonly string EventType;
	}
}
