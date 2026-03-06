using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct AgreementVersionObject
	{
		[JsonConstructor]
		public AgreementVersionObject(long id, bool is_active, bool is_latest, long type, UserObject user, long date_added, long date_updated, long date_live, string name, string changelog, string description, JObject adjacent_versions)
		{
			this.Id = id;
			this.IsActive = is_active;
			this.IsLatest = is_latest;
			this.Type = type;
			this.User = user;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.DateLive = date_live;
			this.Name = name;
			this.Changelog = changelog;
			this.Description = description;
			this.AdjacentVersions = adjacent_versions;
		}

		internal readonly long Id;

		internal readonly bool IsActive;

		internal readonly bool IsLatest;

		internal readonly long Type;

		internal readonly UserObject User;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly long DateLive;

		internal readonly string Name;

		internal readonly string Changelog;

		internal readonly string Description;

		internal readonly JObject AdjacentVersions;
	}
}
