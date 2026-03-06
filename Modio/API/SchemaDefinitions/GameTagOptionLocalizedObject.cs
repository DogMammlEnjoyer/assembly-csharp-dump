using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.SchemaDefinitions
{
	[JsonObject]
	internal readonly struct GameTagOptionLocalizedObject
	{
		[JsonConstructor]
		public GameTagOptionLocalizedObject(string name, string name_localized, string type, string[] tags, JObject tags_localized, JObject tag_count_map, bool hidden, bool locked)
		{
			this.Name = name;
			this.NameLocalized = name_localized;
			this.Type = type;
			this.Tags = tags;
			this.TagsLocalized = tags_localized;
			this.TagCountMap = tag_count_map;
			this.Hidden = hidden;
			this.Locked = locked;
		}

		internal readonly string Name;

		internal readonly string NameLocalized;

		internal readonly string Type;

		internal readonly string[] Tags;

		internal readonly JObject TagsLocalized;

		internal readonly JObject TagCountMap;

		internal readonly bool Hidden;

		internal readonly bool Locked;
	}
}
