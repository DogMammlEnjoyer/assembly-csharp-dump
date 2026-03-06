using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(1)]
	[Nullable(0)]
	public class MapSpawnManager : MonoBehaviour
	{
		public static bool HasInstance
		{
			get
			{
				return MapSpawnManager.hasInstance;
			}
		}

		private void Awake()
		{
			if (MapSpawnManager.instance != null)
			{
				Object.Destroy(this);
				return;
			}
			MapSpawnManager.instance = this;
			MapSpawnManager.hasInstance = true;
			this.GetEntityTypeTemplates();
			this.FindSpawnPoints();
		}

		public void FindSpawnPoints()
		{
			this.spawnPoints.Clear();
			MapSpawnPoint[] array = Object.FindObjectsByType<MapSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.spawnPoints.Add(array[i].spawnID, array[i]);
			}
		}

		public bool GetSpawnPoint(string spawnPointID, out MapSpawnPoint spawnPoint)
		{
			return this.spawnPoints.TryGetValue(spawnPointID, out spawnPoint);
		}

		[NullableContext(2)]
		public bool GetEntityType(int enemyTypeIndex, out GameObject newEntity)
		{
			if (!this.entityTypes.ContainsKey(enemyTypeIndex))
			{
				newEntity = null;
				return false;
			}
			newEntity = this.entityTypes[enemyTypeIndex];
			return true;
		}

		public bool SpawnEntity(string spawnPointID, int enemyTypeIndex, [Nullable(2)] out MapEntity newEntity)
		{
			if (!this.entityTypes.ContainsKey(enemyTypeIndex))
			{
				Debug.Log("AISpawnManager::SpawnEnemy enemy index incorrect");
				newEntity = null;
				return false;
			}
			MapSpawnPoint mapSpawnPoint;
			if (!this.spawnPoints.TryGetValue(spawnPointID, out mapSpawnPoint))
			{
				Debug.Log("AISpawnManager::SpawnEnemy Can't find spawn point");
				newEntity = null;
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.entityTypes[enemyTypeIndex], mapSpawnPoint.transform);
			mapSpawnPoint.spawnCount++;
			newEntity = gameObject.GetComponent<MapEntity>();
			return true;
		}

		[NullableContext(2)]
		public bool SpawnEntity(int enemyTypeIndex, out MapEntity newEnemy)
		{
			if (!this.entityTypes.ContainsKey(enemyTypeIndex))
			{
				Debug.Log("AISpawnManager::SpawnEnemy enemy index incorrect");
				newEnemy = null;
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.entityTypes[enemyTypeIndex]);
			newEnemy = gameObject.GetComponent<MapEntity>();
			return true;
		}

		private void GetEntityTypeTemplates()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				MapEntity component = child.GetComponent<MapEntity>();
				if (!(component == null) && component.isTemplate)
				{
					child.gameObject.SetActive(false);
					this.entityTypes[(int)component.entityTypeId] = child.gameObject;
				}
			}
		}

		private Dictionary<int, GameObject> entityTypes = new Dictionary<int, GameObject>(64);

		[Nullable(2)]
		public static MapSpawnManager instance;

		private static bool hasInstance;

		private Dictionary<string, MapSpawnPoint> spawnPoints = new Dictionary<string, MapSpawnPoint>(128);
	}
}
