using System;
using UnityEngine;

namespace Modio.Unity.UI.Input
{
	public class ModioUIInputListenerLoader : MonoBehaviour
	{
		private void Awake()
		{
			bool flag = true;
			string[] prefabNames = this._prefabNames;
			for (int i = 0; i < prefabNames.Length; i++)
			{
				GameObject gameObject = Resources.Load<GameObject>(prefabNames[i]);
				if (!(gameObject == null))
				{
					Object.Instantiate<GameObject>(gameObject, base.transform);
					flag = false;
				}
			}
			if (flag && !string.IsNullOrEmpty(this._fallbackPrefabName))
			{
				GameObject gameObject2 = Resources.Load<GameObject>(this._fallbackPrefabName);
				if (gameObject2 != null)
				{
					Object.Instantiate<GameObject>(gameObject2, base.transform);
				}
			}
		}

		[SerializeField]
		private string[] _prefabNames;

		[SerializeField]
		private string _fallbackPrefabName;
	}
}
