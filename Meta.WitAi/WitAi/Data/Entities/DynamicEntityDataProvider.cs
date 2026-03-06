using System;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class DynamicEntityDataProvider : MonoBehaviour, IDynamicEntitiesProvider
	{
		public WitDynamicEntities GetDynamicEntities()
		{
			WitDynamicEntities witDynamicEntities = new WitDynamicEntities();
			foreach (WitDynamicEntitiesData provider in this.entitiesDefinition)
			{
				witDynamicEntities.Merge(provider);
			}
			return witDynamicEntities;
		}

		[SerializeField]
		internal WitDynamicEntitiesData[] entitiesDefinition;
	}
}
