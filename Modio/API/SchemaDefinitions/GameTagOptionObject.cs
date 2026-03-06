using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameTagOptionObject
	{
		[JsonConstructor]
		public GameTagOptionObject(string name, Dictionary<string, string> name_localization, string type, string[] tags, GameTagOptionObject.EmbeddedTagsLocalization[] tags_localization, Dictionary<string, int> tag_count_map, bool hidden, bool locked)
		{
			this.Name = name;
			this.NameLocalization = name_localization;
			this.Type = type;
			this.Tags = tags;
			this.TagsLocalization = tags_localization;
			this.TagCountMap = tag_count_map;
			this.Hidden = hidden;
			this.Locked = locked;
		}

		internal readonly string Name;

		internal readonly Dictionary<string, string> NameLocalization;

		internal readonly string Type;

		internal readonly string[] Tags;

		internal readonly GameTagOptionObject.EmbeddedTagsLocalization[] TagsLocalization;

		internal readonly Dictionary<string, int> TagCountMap;

		internal readonly bool Hidden;

		internal readonly bool Locked;

		[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
		internal readonly struct EmbeddedTagsLocalization
		{
			[JsonConstructor]
			public EmbeddedTagsLocalization(string tag, Dictionary<string, string> translations)
			{
				this.Tag = tag;
				this.Translations = translations;
			}

			internal readonly string Tag;

			internal readonly Dictionary<string, string> Translations;
		}
	}
}
