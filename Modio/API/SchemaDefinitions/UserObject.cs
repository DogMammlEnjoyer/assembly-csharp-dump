using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct UserObject
	{
		[JsonConstructor]
		public UserObject(long id, string name_id, string username, string display_name_portal, long date_online, long date_joined, AvatarObject avatar, string timezone, string language, string profile_url)
		{
			this.Id = id;
			this.NameId = name_id;
			this.Username = username;
			this.DisplayNamePortal = display_name_portal;
			this.DateOnline = date_online;
			this.DateJoined = date_joined;
			this.Avatar = avatar;
			this.Timezone = timezone;
			this.Language = language;
			this.ProfileUrl = profile_url;
		}

		internal readonly long Id;

		internal readonly string NameId;

		internal readonly string Username;

		internal readonly string DisplayNamePortal;

		internal readonly long DateOnline;

		internal readonly long DateJoined;

		internal readonly AvatarObject Avatar;

		internal readonly string Timezone;

		internal readonly string Language;

		internal readonly string ProfileUrl;
	}
}
