using System;
using UnityEngine;

namespace Liv.Lck.Tablet
{
	public abstract class LckBaseNotification : MonoBehaviour
	{
		public bool RemainOnScreen { get; private set; }

		public float ShowDuration { get; private set; } = 3f;

		public GameObject SpawnedGameObject { get; private set; }

		public virtual void ShowNotification()
		{
			if (this.SpawnedGameObject != null)
			{
				this.SpawnedGameObject.SetActive(true);
			}
		}

		public virtual void HideNotification()
		{
			if (this.SpawnedGameObject != null)
			{
				this.SpawnedGameObject.SetActive(false);
			}
		}

		public void SetSpawnedGameObject(GameObject go)
		{
			this.SpawnedGameObject = go;
		}
	}
}
