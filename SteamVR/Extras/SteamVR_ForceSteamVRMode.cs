using System;
using System.Collections;
using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_ForceSteamVRMode : MonoBehaviour
	{
		private IEnumerator Start()
		{
			yield return new WaitForSeconds(1f);
			SteamVR.Initialize(true);
			while (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
			{
				yield return null;
			}
			for (int i = 0; i < this.disableObjectsOnLoad.Length; i++)
			{
				GameObject gameObject = this.disableObjectsOnLoad[i];
				if (gameObject != null)
				{
					gameObject.SetActive(false);
				}
			}
			Object.Instantiate<GameObject>(this.vrCameraPrefab);
			yield break;
		}

		public GameObject vrCameraPrefab;

		public GameObject[] disableObjectsOnLoad;
	}
}
