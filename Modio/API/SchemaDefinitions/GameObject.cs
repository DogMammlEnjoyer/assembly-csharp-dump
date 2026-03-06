using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct GameObject
	{
		[JsonConstructor]
		public GameObject(long id, long status, JObject submittedBy, long dateAdded, long dateUpdated, long dateLive, long presentationOption, long submissionOption, long dependencyOption, long curationOption, long communityOptions, long monetizationOptions, GameMonetizationTeamObject monetizationTeam, long revenueOptions, long maxStock, long apiAccessOptions, long maturityOptions, string ugcName, string tokenName, IconObject icon, LogoObject logo, HeaderImageObject header, string name, string nameId, string summary, string instructions, string instructionsUrl, string profileUrl, GameOtherUrlsObject[] otherUrls, GameTagOptionLocalizedObject[] tagOptions, GameStatsObject stats, ThemeObject theme, GamePlatformsObject[] platforms)
		{
			this.Id = id;
			this.Status = status;
			this.SubmittedBy = submittedBy;
			this.DateAdded = dateAdded;
			this.DateUpdated = dateUpdated;
			this.DateLive = dateLive;
			this.PresentationOption = presentationOption;
			this.SubmissionOption = submissionOption;
			this.DependencyOption = dependencyOption;
			this.CurationOption = curationOption;
			this.CommunityOptions = communityOptions;
			this.MonetizationOptions = monetizationOptions;
			this.MonetizationTeam = monetizationTeam;
			this.RevenueOptions = revenueOptions;
			this.MaxStock = maxStock;
			this.ApiAccessOptions = apiAccessOptions;
			this.MaturityOptions = maturityOptions;
			this.UgcName = ugcName;
			this.TokenName = tokenName;
			this.Icon = icon;
			this.Logo = logo;
			this.Header = header;
			this.Name = name;
			this.NameId = nameId;
			this.Summary = summary;
			this.Instructions = instructions;
			this.InstructionsUrl = instructionsUrl;
			this.ProfileUrl = profileUrl;
			this.OtherUrls = otherUrls;
			this.TagOptions = tagOptions;
			this.Stats = stats;
			this.Theme = theme;
			this.Platforms = platforms;
		}

		internal readonly long Id;

		internal readonly long Status;

		internal readonly JObject SubmittedBy;

		internal readonly long DateAdded;

		internal readonly long DateUpdated;

		internal readonly long DateLive;

		internal readonly long PresentationOption;

		internal readonly long SubmissionOption;

		internal readonly long DependencyOption;

		internal readonly long CurationOption;

		internal readonly long CommunityOptions;

		internal readonly long MonetizationOptions;

		internal readonly GameMonetizationTeamObject MonetizationTeam;

		internal readonly long RevenueOptions;

		internal readonly long MaxStock;

		internal readonly long ApiAccessOptions;

		internal readonly long MaturityOptions;

		internal readonly string UgcName;

		internal readonly string TokenName;

		internal readonly IconObject Icon;

		internal readonly LogoObject Logo;

		internal readonly HeaderImageObject Header;

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly string Summary;

		internal readonly string Instructions;

		internal readonly string InstructionsUrl;

		internal readonly string ProfileUrl;

		internal readonly GameOtherUrlsObject[] OtherUrls;

		internal readonly GameTagOptionLocalizedObject[] TagOptions;

		internal readonly GameStatsObject Stats;

		internal readonly ThemeObject Theme;

		internal readonly GamePlatformsObject[] Platforms;
	}
}
