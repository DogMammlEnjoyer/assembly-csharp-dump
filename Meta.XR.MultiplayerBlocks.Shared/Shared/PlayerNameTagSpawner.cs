using System;
using System.Collections;
using Meta.XR.BuildingBlocks;
using Oculus.Platform.Models;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Shared
{
	public class PlayerNameTagSpawner : MonoBehaviour
	{
		private void Start()
		{
			this._nameTagSpawner = this.GetInterfaceComponent<INameTagSpawner>();
			PlatformInit.GetEntitlementInformation(new Action<PlatformInfo>(this.OnEntitlementFinished));
		}

		private IEnumerator SpawnCoroutine(string playerName)
		{
			if (this._nameTagSpawner == null)
			{
				yield break;
			}
			while (!this._nameTagSpawner.IsConnected)
			{
				yield return null;
			}
			this._nameTagSpawner.Spawn(playerName);
			yield break;
		}

		private void OnEntitlementFinished(PlatformInfo info)
		{
			string format = "Entitlement callback: isEntitled: {0} Name: {1} UserID: {2}";
			object arg = info.IsEntitled;
			User oculusUser = info.OculusUser;
			object arg2 = (oculusUser != null) ? oculusUser.OculusID : null;
			User oculusUser2 = info.OculusUser;
			Debug.Log(string.Format(format, arg, arg2, (oculusUser2 != null) ? new ulong?(oculusUser2.ID) : null));
			string text;
			if (!info.IsEntitled)
			{
				text = this.GetRandomName();
			}
			else
			{
				User oculusUser3 = info.OculusUser;
				text = ((oculusUser3 != null) ? oculusUser3.OculusID : null);
			}
			string playerName = text;
			base.StartCoroutine(this.SpawnCoroutine(playerName));
		}

		private string GetRandomName()
		{
			if (this.namePrefix.Length == 0 || this.namePostfix.Length == 0)
			{
				return null;
			}
			string str = this.namePrefix[Random.Range(0, this.namePrefix.Length - 1)];
			string str2 = this.namePostfix[Random.Range(0, this.namePostfix.Length - 1)];
			return str + " " + str2;
		}

		[Header("Randomized name for non-entitled folks eg. 'HappyHippo'", order = 1)]
		[SerializeField]
		private string[] namePrefix = new string[]
		{
			"Happy",
			"Running",
			"Laughing",
			"Smiling"
		};

		[SerializeField]
		private string[] namePostfix = new string[]
		{
			"Cat",
			"Dog",
			"Hippo",
			"Bird"
		};

		private INameTagSpawner _nameTagSpawner;
	}
}
