using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct MonetizationTeamAccountsObject
	{
		[JsonConstructor]
		public MonetizationTeamAccountsObject(long id, string name_id, string username, long monetization_status, long monetization_options, long split)
		{
			this.Id = id;
			this.NameId = name_id;
			this.Username = username;
			this.MonetizationStatus = monetization_status;
			this.MonetizationOptions = monetization_options;
			this.Split = split;
		}

		internal readonly long Id;

		internal readonly string NameId;

		internal readonly string Username;

		internal readonly long MonetizationStatus;

		internal readonly long MonetizationOptions;

		internal readonly long Split;
	}
}
