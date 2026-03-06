using System;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class WitDynamicEntitiesData : ScriptableObject, IDynamicEntitiesProvider
	{
		public WitDynamicEntities GetDynamicEntities()
		{
			return this.entities;
		}

		public WitDynamicEntities entities;
	}
}
