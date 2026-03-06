using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Data.Info;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;

namespace Meta.WitAi.Data.Entities
{
	[Serializable]
	public class WitDynamicEntities : IDynamicEntitiesProvider, IEnumerable<WitDynamicEntity>, IEnumerable
	{
		public WitDynamicEntities()
		{
		}

		public WitDynamicEntities(IEnumerable<WitDynamicEntity> entity)
		{
			this.entities.AddRange(entity);
		}

		public WitDynamicEntities(params WitDynamicEntity[] entity)
		{
			this.entities.AddRange(entity);
		}

		public WitResponseClass AsJson
		{
			get
			{
				WitResponseClass witResponseClass = new WitResponseClass();
				foreach (WitDynamicEntity witDynamicEntity in this.entities)
				{
					witResponseClass.Add(witDynamicEntity.entity, witDynamicEntity.AsJson);
				}
				return witResponseClass;
			}
		}

		public override string ToString()
		{
			return this.AsJson.ToString();
		}

		public IEnumerator<WitDynamicEntity> GetEnumerator()
		{
			return this.entities.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public WitDynamicEntities GetDynamicEntities()
		{
			return this;
		}

		public void Merge(IDynamicEntitiesProvider provider)
		{
			if (provider == null)
			{
				return;
			}
			this.entities.AddRange(provider.GetDynamicEntities());
		}

		public void Merge(IEnumerable<WitDynamicEntity> mergeEntities)
		{
			if (mergeEntities == null)
			{
				return;
			}
			this.entities.AddRange(mergeEntities);
		}

		public void Add(WitDynamicEntity dynamicEntity)
		{
			if (this.entities.FindIndex((WitDynamicEntity e) => e.entity == dynamicEntity.entity) < 0)
			{
				this.entities.Add(dynamicEntity);
				return;
			}
			VLog.W("Cannot add entity, registry already has an entry for " + dynamicEntity.entity, null);
		}

		public void Remove(WitDynamicEntity dynamicEntity)
		{
			this.entities.Remove(dynamicEntity);
		}

		public void AddKeyword(string entityName, WitEntityKeywordInfo keyword)
		{
			WitDynamicEntity witDynamicEntity = this.entities.Find((WitDynamicEntity e) => entityName == e.entity);
			if (witDynamicEntity == null)
			{
				witDynamicEntity = new WitDynamicEntity(entityName, Array.Empty<string>());
				this.entities.Add(witDynamicEntity);
			}
			witDynamicEntity.keywords.Add(keyword);
		}

		public void RemoveKeyword(string entityName, WitEntityKeywordInfo keyword)
		{
			int num = this.entities.FindIndex((WitDynamicEntity e) => e.entity == entityName);
			if (num >= 0)
			{
				this.entities[num].keywords.Remove(keyword);
				if (this.entities[num].keywords.Count == 0)
				{
					this.entities.RemoveAt(num);
				}
			}
		}

		public List<WitDynamicEntity> entities = new List<WitDynamicEntity>();
	}
}
