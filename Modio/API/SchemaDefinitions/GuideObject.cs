using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GuideObject
	{
		[JsonConstructor]
		public GuideObject(long id, long game_id, string game_name, LogoObject logo, UserObject user, long date_added, long date_updated, long date_live, long status, string url, string name, string name_id, string summary, string description, long community_options, GuideTagObject[] tags, GuideStatsObject[] stats)
		{
			this.Id = id;
			this.GameId = game_id;
			this.GameName = game_name;
			this.Logo = logo;
			this.User = user;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.DateLive = date_live;
			this.Status = status;
			this.Url = url;
			this.Name = name;
			this.NameId = name_id;
			this.Summary = summary;
			this.Description = description;
			this.CommunityOptions = community_options;
			this.Tags = tags;
			this.Stats = stats;
		}

		internal readonly long Id;

		internal readonly long GameId;

		internal readonly string GameName;

		internal readonly LogoObject Logo;

		internal readonly UserObject User;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly long DateLive;

		internal readonly long Status;

		internal readonly string Url;

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly string Summary;

		internal readonly string Description;

		internal readonly long CommunityOptions;

		internal readonly GuideTagObject[] Tags;

		internal readonly GuideStatsObject[] Stats;
	}
}
