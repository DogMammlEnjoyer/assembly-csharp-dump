using System;
using Fusion;
using Meta.XR.MultiplayerBlocks.Shared;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Fusion
{
	public class PlayerNameTagSpawnerFusion : MonoBehaviour, INameTagSpawner
	{
		private void OnEnable()
		{
			FusionBBEvents.OnSceneLoadDone += this.OnLoaded;
		}

		private void OnDisable()
		{
			FusionBBEvents.OnSceneLoadDone -= this.OnLoaded;
		}

		private void OnLoaded(NetworkRunner networkRunner)
		{
			this._sceneLoaded = true;
			this._networkRunner = networkRunner;
		}

		public bool IsConnected
		{
			get
			{
				return this._networkRunner != null && this._sceneLoaded;
			}
		}

		public void Spawn(string playerName)
		{
			this._networkRunner.Spawn(this.playerNameTagPrefab, new Vector3?(Vector3.zero), new Quaternion?(Quaternion.identity), new PlayerRef?(this._networkRunner.LocalPlayer), null, (NetworkSpawnFlags)0).GetComponent<PlayerNameTagFusion>().OculusName = playerName;
		}

		[SerializeField]
		private GameObject playerNameTagPrefab;

		private NetworkRunner _networkRunner;

		private bool _sceneLoaded;
	}
}
