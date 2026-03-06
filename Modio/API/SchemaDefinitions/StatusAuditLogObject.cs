using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct StatusAuditLogObject
	{
		[JsonConstructor]
		public StatusAuditLogObject(long status_new, long status_old, UserObject user, long date_added, string reason)
		{
			this.StatusNew = status_new;
			this.StatusOld = status_old;
			this.User = user;
			this.DateAdded = date_added;
			this.Reason = reason;
		}

		internal readonly long StatusNew;

		internal readonly long StatusOld;

		internal readonly UserObject User;

		internal readonly long DateAdded;

		internal readonly string Reason;
	}
}
