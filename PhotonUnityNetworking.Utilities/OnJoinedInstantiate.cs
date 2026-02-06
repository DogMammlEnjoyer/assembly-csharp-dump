using System;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Pun.UtilityScripts
{
	public class OnJoinedInstantiate : MonoBehaviour, IMatchmakingCallbacks
	{
		public virtual void OnEnable()
		{
			PhotonNetwork.AddCallbackTarget(this);
		}

		public virtual void OnDisable()
		{
			PhotonNetwork.RemoveCallbackTarget(this);
		}

		public virtual void OnJoinedRoom()
		{
			if (this.AutoSpawnObjects && !PhotonNetwork.LocalPlayer.HasRejoined)
			{
				this.SpawnObjects();
			}
		}

		public virtual void SpawnObjects()
		{
			if (this.PrefabsToInstantiate != null)
			{
				foreach (GameObject gameObject in this.PrefabsToInstantiate)
				{
					if (!(gameObject == null))
					{
						Vector3 position;
						Quaternion rotation;
						this.GetSpawnPoint(out position, out rotation);
						GameObject item = PhotonNetwork.Instantiate(gameObject.name, position, rotation, 0, null);
						this.SpawnedObjects.Push(item);
					}
				}
			}
		}

		public virtual void DespawnObjects(bool localOnly)
		{
			while (this.SpawnedObjects.Count > 0)
			{
				GameObject gameObject = this.SpawnedObjects.Pop();
				if (gameObject)
				{
					if (localOnly)
					{
						Object.Destroy(gameObject);
					}
					else
					{
						PhotonNetwork.Destroy(gameObject);
					}
				}
			}
		}

		public virtual void OnFriendListUpdate(List<FriendInfo> friendList)
		{
		}

		public virtual void OnCreatedRoom()
		{
		}

		public virtual void OnCreateRoomFailed(short returnCode, string message)
		{
		}

		public virtual void OnJoinRoomFailed(short returnCode, string message)
		{
		}

		public virtual void OnJoinRandomFailed(short returnCode, string message)
		{
		}

		public virtual void OnLeftRoom()
		{
		}

		public virtual void OnPreLeavingRoom()
		{
		}

		public virtual void GetSpawnPoint(out Vector3 spawnPos, out Quaternion spawnRot)
		{
			Transform spawnPoint = this.GetSpawnPoint();
			if (spawnPoint != null)
			{
				spawnPos = spawnPoint.position;
				spawnRot = spawnPoint.rotation;
			}
			else
			{
				spawnPos = new Vector3(0f, 0f, 0f);
				spawnRot = new Quaternion(0f, 0f, 0f, 1f);
			}
			if (this.UseRandomOffset)
			{
				Random.InitState((int)(Time.time * 10000f));
				spawnPos += this.GetRandomOffset();
			}
		}

		protected virtual Transform GetSpawnPoint()
		{
			if (this.SpawnPoints == null || this.SpawnPoints.Count == 0)
			{
				return null;
			}
			switch (this.Sequence)
			{
			case OnJoinedInstantiate.SpawnSequence.Connection:
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
				return this.SpawnPoints[(actorNumber == -1) ? 0 : (actorNumber % this.SpawnPoints.Count)];
			}
			case OnJoinedInstantiate.SpawnSequence.Random:
				return this.SpawnPoints[Random.Range(0, this.SpawnPoints.Count)];
			case OnJoinedInstantiate.SpawnSequence.RoundRobin:
				this.lastUsedSpawnPointIndex++;
				if (this.lastUsedSpawnPointIndex >= this.SpawnPoints.Count)
				{
					this.lastUsedSpawnPointIndex = 0;
				}
				if (this.SpawnPoints != null && this.SpawnPoints.Count != 0)
				{
					return this.SpawnPoints[this.lastUsedSpawnPointIndex];
				}
				return null;
			default:
				return null;
			}
		}

		protected virtual Vector3 GetRandomOffset()
		{
			Vector3 insideUnitSphere = Random.insideUnitSphere;
			if (this.ClampY)
			{
				insideUnitSphere.y = 0f;
			}
			return this.RandomOffset * insideUnitSphere.normalized;
		}

		[HideInInspector]
		private Transform SpawnPosition;

		[HideInInspector]
		public OnJoinedInstantiate.SpawnSequence Sequence;

		[HideInInspector]
		public List<Transform> SpawnPoints = new List<Transform>(1)
		{
			null
		};

		[Tooltip("Add a random variance to a spawn point position. GetRandomOffset() can be overridden with your own method for producing offsets.")]
		[HideInInspector]
		public bool UseRandomOffset = true;

		[Tooltip("Radius of the RandomOffset.")]
		[FormerlySerializedAs("PositionOffset")]
		[HideInInspector]
		public float RandomOffset = 2f;

		[Tooltip("Disables the Y axis of RandomOffset. The Y value of the spawn point will be used.")]
		[HideInInspector]
		public bool ClampY = true;

		[HideInInspector]
		public List<GameObject> PrefabsToInstantiate = new List<GameObject>(1)
		{
			null
		};

		[FormerlySerializedAs("autoSpawnObjects")]
		[HideInInspector]
		public bool AutoSpawnObjects = true;

		public Stack<GameObject> SpawnedObjects = new Stack<GameObject>();

		protected int spawnedAsActorId;

		protected int lastUsedSpawnPointIndex = -1;

		public enum SpawnSequence
		{
			Connection,
			Random,
			RoundRobin
		}
	}
}
