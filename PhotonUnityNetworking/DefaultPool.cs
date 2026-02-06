using System;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun
{
	public class DefaultPool : IPunPrefabPool
	{
		GameObject IPunPrefabPool.Instantiate(string prefabId, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = null;
			if (!this.ResourceCache.TryGetValue(prefabId, out gameObject))
			{
				gameObject = Resources.Load<GameObject>(prefabId);
				if (gameObject == null)
				{
					Debug.LogError("DefaultPool failed to load \"" + prefabId + "\". Make sure it's in a \"Resources\" folder. Or use a custom IPunPrefabPool.");
				}
				else
				{
					this.ResourceCache.Add(prefabId, gameObject);
				}
			}
			bool activeSelf = gameObject.activeSelf;
			if (activeSelf)
			{
				gameObject.SetActive(false);
			}
			GameObject result = Object.Instantiate<GameObject>(gameObject, position, rotation);
			if (activeSelf)
			{
				gameObject.SetActive(true);
			}
			return result;
		}

		public void Destroy(GameObject gameObject)
		{
			Object.Destroy(gameObject);
		}

		public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
	}
}
