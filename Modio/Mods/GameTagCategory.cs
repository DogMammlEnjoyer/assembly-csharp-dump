using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Modio.API.SchemaDefinitions;
using Newtonsoft.Json;

namespace Modio.Mods
{
	public class GameTagCategory
	{
		[JsonConstructor]
		internal GameTagCategory(string name, bool multiSelect, ModTag[] tags, bool hidden, bool locked)
		{
			this.Name = name;
			this.MultiSelect = multiSelect;
			this.Tags = tags;
			this.Hidden = hidden;
			this.Locked = locked;
		}

		internal GameTagCategory(GameTagOptionObject tagObject)
		{
			this.Name = tagObject.Name;
			this.MultiSelect = (tagObject.Type == "checkboxes");
			this.Hidden = tagObject.Hidden;
			this.Locked = tagObject.Locked;
			this.Tags = tagObject.Tags.Select(new Func<string, ModTag>(ModTag.Get)).ToArray<ModTag>();
			foreach (KeyValuePair<string, int> keyValuePair in tagObject.TagCountMap)
			{
				string text;
				int i;
				keyValuePair.Deconstruct(out text, out i);
				string tagName = text;
				int count = i;
				ModTag.Get(tagName).Count = count;
			}
			if (tagObject.TagsLocalization != null)
			{
				foreach (GameTagOptionObject.EmbeddedTagsLocalization embeddedTagsLocalization in tagObject.TagsLocalization)
				{
					ModTag.Get(embeddedTagsLocalization.Tag).SetLocalizations(embeddedTagsLocalization.Translations);
				}
			}
		}

		static GameTagCategory()
		{
			ModioClient.OnInitialized += delegate()
			{
				GameTagCategory._cachedTags = null;
			};
		}

		public static Task<ValueTuple<Error, GameTagCategory[]>> GetGameTagOptions()
		{
			GameTagCategory.<GetGameTagOptions>d__9 <GetGameTagOptions>d__;
			<GetGameTagOptions>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, GameTagCategory[]>>.Create();
			<GetGameTagOptions>d__.<>1__state = -1;
			<GetGameTagOptions>d__.<>t__builder.Start<GameTagCategory.<GetGameTagOptions>d__9>(ref <GetGameTagOptions>d__);
			return <GetGameTagOptions>d__.<>t__builder.Task;
		}

		private static GameTagCategory[] _cachedTags;

		public readonly string Name;

		public readonly bool MultiSelect;

		public readonly ModTag[] Tags;

		public readonly bool Hidden;

		public readonly bool Locked;
	}
}
