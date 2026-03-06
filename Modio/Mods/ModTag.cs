using System;
using System.Collections.Generic;
using Modio.API;
using Modio.API.SchemaDefinitions;
using Newtonsoft.Json;

namespace Modio.Mods
{
	public class ModTag
	{
		public string NameLocalized { get; private set; }

		public bool IsVisible { get; private set; }

		public int Count { get; internal set; }

		private ModTag(string apiName)
		{
			this.ApiName = apiName;
		}

		[JsonConstructor]
		public ModTag(string apiName, Dictionary<string, string> translations, string nameLocalized, bool isVisible, int count)
		{
			this.ApiName = apiName;
			this._translations = translations;
			this.NameLocalized = nameLocalized;
			this.IsVisible = isVisible;
			this.Count = count;
		}

		internal static ModTag Get(ModTagObject modTag)
		{
			ModTag modTag2;
			if (ModTag.Tags.TryGetValue(modTag.Name, out modTag2))
			{
				modTag2.NameLocalized = modTag.NameLocalized;
				return modTag2;
			}
			modTag2 = new ModTag(modTag.Name)
			{
				NameLocalized = modTag.NameLocalized
			};
			ModTag.Tags.Add(modTag.Name, modTag2);
			return modTag2;
		}

		internal static ModTag Get(string tagName)
		{
			ModTag modTag;
			if (ModTag.Tags.TryGetValue(tagName, out modTag))
			{
				return modTag;
			}
			modTag = new ModTag(tagName)
			{
				NameLocalized = tagName
			};
			ModTag.Tags.Add(tagName, modTag);
			return modTag;
		}

		public void SetLocalizations(Dictionary<string, string> translations)
		{
			this._translations = translations;
			string nameLocalized;
			if (translations.TryGetValue(ModioAPI.LanguageCodeResponse, out nameLocalized))
			{
				this.NameLocalized = nameLocalized;
			}
		}

		private static readonly Dictionary<string, ModTag> Tags = new Dictionary<string, ModTag>();

		public readonly string ApiName;

		private Dictionary<string, string> _translations;
	}
}
