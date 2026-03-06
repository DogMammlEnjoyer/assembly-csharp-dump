using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct TeamMemberObject
	{
		[JsonConstructor]
		public TeamMemberObject(long id, UserObject user, long level, long date_added, string position, long invite_pending)
		{
			this.Id = id;
			this.User = user;
			this.Level = level;
			this.DateAdded = date_added;
			this.Position = position;
			this.InvitePending = invite_pending;
		}

		internal readonly long Id;

		internal readonly UserObject User;

		internal readonly long Level;

		internal readonly long DateAdded;

		internal readonly string Position;

		internal readonly long InvitePending;
	}
}
