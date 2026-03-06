using System;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class WitSimpleDynamicEntity : MonoBehaviour, IDynamicEntitiesProvider
	{
		public WitDynamicEntities GetDynamicEntities()
		{
			WitDynamicEntity witDynamicEntity = new WitDynamicEntity(this.entityName, this.keywords);
			return new WitDynamicEntities(new WitDynamicEntity[]
			{
				witDynamicEntity
			});
		}

		[SerializeField]
		private string entityName;

		[SerializeField]
		private string[] keywords;
	}
}
