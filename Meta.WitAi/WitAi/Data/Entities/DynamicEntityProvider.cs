using System;
using Meta.WitAi.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class DynamicEntityProvider : MonoBehaviour, IDynamicEntitiesProvider
	{
		public WitDynamicEntities GetDynamicEntities()
		{
			return this.entities;
		}

		[SerializeField]
		protected WitDynamicEntities entities;
	}
}
