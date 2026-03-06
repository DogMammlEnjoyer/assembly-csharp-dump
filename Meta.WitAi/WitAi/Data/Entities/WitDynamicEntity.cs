using System;
using System.Collections.Generic;
using Meta.WitAi.Data.Info;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;

namespace Meta.WitAi.Data.Entities
{
	[Serializable]
	public class WitDynamicEntity : IDynamicEntitiesProvider
	{
		public WitDynamicEntity()
		{
		}

		public WitDynamicEntity(string entity, WitEntityKeywordInfo keyword)
		{
			this.entity = entity;
			this.keywords.Add(keyword);
		}

		public WitDynamicEntity(string entity, params string[] keywords)
		{
			this.entity = entity;
			foreach (string text in keywords)
			{
				this.keywords.Add(new WitEntityKeywordInfo
				{
					keyword = text,
					synonyms = new List<string>(new string[]
					{
						text
					})
				});
			}
		}

		public WitDynamicEntity(string entity, Dictionary<string, List<string>> keywordsToSynonyms)
		{
			this.entity = entity;
			foreach (KeyValuePair<string, List<string>> keyValuePair in keywordsToSynonyms)
			{
				this.keywords.Add(new WitEntityKeywordInfo
				{
					keyword = keyValuePair.Key,
					synonyms = keyValuePair.Value
				});
			}
		}

		public WitResponseArray AsJson
		{
			get
			{
				return JsonConvert.SerializeToken<List<WitEntityKeywordInfo>>(this.keywords, null, false).AsArray;
			}
		}

		public WitDynamicEntities GetDynamicEntities()
		{
			return new WitDynamicEntities
			{
				entities = new List<WitDynamicEntity>
				{
					this
				}
			};
		}

		public string entity;

		public List<WitEntityKeywordInfo> keywords = new List<WitEntityKeywordInfo>();
	}
}
