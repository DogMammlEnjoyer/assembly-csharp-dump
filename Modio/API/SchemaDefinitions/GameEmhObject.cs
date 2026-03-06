using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
	internal readonly struct GameEmhObject
	{
		[JsonConstructor]
		public GameEmhObject(long id, long status, long communityOptions, string ugcName, string name, string nameId, GameTagOptionLocalizedObject[] tagOptions, ThemeObject theme, GamePlatformsObject[] platforms)
		{
			this.Id = id;
			this.Status = status;
			this.CommunityOptions = communityOptions;
			this.UgcName = ugcName;
			this.Name = name;
			this.NameId = nameId;
			this.TagOptions = tagOptions;
			this.Theme = theme;
			this.Platforms = platforms;
		}

		internal readonly long Id;

		internal readonly long Status;

		internal readonly long CommunityOptions;

		internal readonly string UgcName;

		internal readonly string Name;

		internal readonly string NameId;

		internal readonly GameTagOptionLocalizedObject[] TagOptions;

		internal readonly ThemeObject Theme;

		internal readonly GamePlatformsObject[] Platforms;
	}
}
