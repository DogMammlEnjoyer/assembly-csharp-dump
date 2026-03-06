using System;
using Newtonsoft.Json;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(MemberSerialization.Fields)]
	internal readonly struct ModObject
	{
		[JsonConstructor]
		public ModObject(long id, long game_id, long status, long visible, UserObject submitted_by, long date_added, long date_updated, long date_live, long maturity_option, long community_options, long monetization_options, long credit_options, long stock, long price, long tax, LogoObject logo, string homepage_url, string name, string name_id, string summary, string description, string description_plaintext, string metadata_blob, string profile_url, ModMediaObject media, ModfileObject modfile, bool dependencies, ModPlatformsObject[] platforms, MetadataKvpObject[] metadata_kvp, ModTagObject[] tags, ModStatsObject stats)
		{
			this.Id = id;
			this.GameId = game_id;
			this.Status = status;
			this.Visible = visible;
			this.SubmittedBy = submitted_by;
			this.DateAdded = date_added;
			this.DateUpdated = date_updated;
			this.DateLive = date_live;
			this.MaturityOption = maturity_option;
			this.CommunityOptions = community_options;
			this.MonetizationOptions = monetization_options;
			this.CreditOptions = credit_options;
			this.Stock = stock;
			this.Price = price;
			this.Tax = tax;
			this.Logo = logo;
			this.HomepageUrl = homepage_url;
			this.Name = name;
			this.NameId = name_id;
			this.Summary = summary;
			this.Description = description;
			this.DescriptionPlaintext = description_plaintext;
			this.MetadataBlob = metadata_blob;
			this.ProfileUrl = profile_url;
			this.Media = media;
			this.Modfile = modfile;
			this.Dependencies = dependencies;
			this.Platforms = platforms;
			this.MetadataKvp = metadata_kvp;
			this.Tags = tags;
			this.Stats = stats;
		}

		public static ModObject GetHiddenModObject(long modId, long modFileId)
		{
			return new ModObject(modId, ModioServices.Resolve<ModioSettings>().GameId, 1L, 0L, new UserObject(-1L, "", "", "", -1L, -1L, new AvatarObject("", "", "", ""), "", "", ""), -1L, -1L, -1L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, new LogoObject("", "", "", "", ""), "", "HIDDEN", "", "You don't currently have access to this hidden mod.\nCheck that you're logged in to the correct mod.io account.", "<p>You don't currently have access to this hidden mod.\nCheck that you're logged in to the correct mod.io account.</p>", "You don't currently have access to this hidden mod.\nCheck that you're logged in to the correct mod.io account.\n", "", "", new ModMediaObject(new string[0], new string[0], new ImageObject[0]), new ModfileObject(modFileId, modId, -1L, -1L, -1L, 1L, 0L, null, 0L, 0L, new FilehashObject(""), "", null, null, null, new DownloadObject("", -1L), new ModfilePlatformObject[0]), false, new ModPlatformsObject[0], new MetadataKvpObject[0], new ModTagObject[0], new ModStatsObject(-1L, -1L, -1L, 0L, 0L, 0L, 0L, 0L, 0L, 0L, 0f, "", -1L));
		}

		internal readonly long Id;

		internal readonly long GameId;

		internal readonly long Status;

		internal readonly long Visible;

		internal readonly UserObject SubmittedBy;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly long DateLive;

		internal readonly long MaturityOption;

		internal readonly long CommunityOptions;

		internal readonly long MonetizationOptions;

		internal readonly long CreditOptions;

		internal readonly long Stock;

		internal readonly long Price;

		internal readonly long Tax;

		internal readonly LogoObject Logo;

		internal readonly string HomepageUrl;

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly string Summary;

		internal readonly string Description;

		internal readonly string DescriptionPlaintext;

		internal readonly string MetadataBlob;

		internal readonly string ProfileUrl;

		internal readonly ModMediaObject Media;

		internal readonly ModfileObject Modfile;

		internal readonly bool Dependencies;

		internal readonly ModPlatformsObject[] Platforms;

		internal readonly MetadataKvpObject[] MetadataKvp;

		internal readonly ModTagObject[] Tags;

		internal readonly ModStatsObject Stats;
	}
}
