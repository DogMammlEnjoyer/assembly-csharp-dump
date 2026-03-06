using System;
using Meta.WitAi.Data.Info;
using UnityEngine;

namespace Meta.WitAi.Data.Entities
{
	public class RegisteredDynamicEntityKeyword : MonoBehaviour
	{
		private void OnEnable()
		{
			if (string.IsNullOrEmpty(this.keyword.keyword))
			{
				return;
			}
			if (string.IsNullOrEmpty(this.entity))
			{
				return;
			}
			if (DynamicEntityKeywordRegistry.HasDynamicEntityRegistry)
			{
				DynamicEntityKeywordRegistry.Instance.RegisterDynamicEntity(this.entity, this.keyword);
				return;
			}
			VLog.W("Cannot register " + base.name + ": No dynamic entity registry present in the scene.Please add one and try again.", null);
		}

		private void OnDisable()
		{
			if (string.IsNullOrEmpty(this.keyword.keyword))
			{
				return;
			}
			if (string.IsNullOrEmpty(this.entity))
			{
				return;
			}
			if (DynamicEntityKeywordRegistry.HasDynamicEntityRegistry)
			{
				DynamicEntityKeywordRegistry.Instance.UnregisterDynamicEntity(this.entity, this.keyword);
			}
		}

		[SerializeField]
		private string entity;

		[SerializeField]
		private WitEntityKeywordInfo keyword;
	}
}
