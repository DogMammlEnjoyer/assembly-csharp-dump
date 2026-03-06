using System;
using Meta.WitAi.Data.Info;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class DynamicEntityKeywordRegistry : MonoBehaviour, IDynamicEntitiesProvider
	{
		public static bool HasDynamicEntityRegistry
		{
			get
			{
				return DynamicEntityKeywordRegistry.instance;
			}
		}

		public static DynamicEntityKeywordRegistry Instance
		{
			get
			{
				if (!DynamicEntityKeywordRegistry.instance)
				{
					DynamicEntityKeywordRegistry.instance = Object.FindAnyObjectByType<DynamicEntityKeywordRegistry>();
				}
				return DynamicEntityKeywordRegistry.instance;
			}
		}

		private void OnEnable()
		{
			DynamicEntityKeywordRegistry.instance = this;
		}

		private void OnDisable()
		{
			DynamicEntityKeywordRegistry.instance = null;
		}

		public void RegisterDynamicEntity(string entity, WitEntityKeywordInfo keyword)
		{
			this.entities.AddKeyword(entity, keyword);
		}

		public void UnregisterDynamicEntity(string entity, WitEntityKeywordInfo keyword)
		{
			this.entities.RemoveKeyword(entity, keyword);
		}

		public WitDynamicEntities GetDynamicEntities()
		{
			return this.entities;
		}

		private static DynamicEntityKeywordRegistry instance;

		private WitDynamicEntities entities = new WitDynamicEntities();
	}
}
