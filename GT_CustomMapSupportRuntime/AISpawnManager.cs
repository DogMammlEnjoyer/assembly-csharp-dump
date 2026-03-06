using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GT_CustomMapSupportRuntime
{
	[NullableContext(1)]
	[Nullable(0)]
	public class AISpawnManager : MonoBehaviour
	{
		public static bool HasInstance
		{
			get
			{
				return AISpawnManager.hasInstance;
			}
		}

		private void Awake()
		{
			if (AISpawnManager.instance != null)
			{
				Object.Destroy(this);
				return;
			}
			AISpawnManager.instance = this;
			AISpawnManager.hasInstance = true;
			this.GetEnemyTypeTemplates();
			this.FindSpawnPoints();
		}

		public void FindSpawnPoints()
		{
			this.spawnPoints.Clear();
			AISpawnPoint[] array = Object.FindObjectsByType<AISpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				this.spawnPoints.Add(array[i].spawnID, array[i]);
			}
		}

		public bool GetSpawnPoint(string spawnPointID, out AISpawnPoint spawnPoint)
		{
			return this.spawnPoints.TryGetValue(spawnPointID, out spawnPoint);
		}

		[NullableContext(2)]
		public bool GetEnemyType(int enemyTypeIndex, out GameObject newEnemy)
		{
			if (!this.enemyTypes.ContainsKey(enemyTypeIndex))
			{
				newEnemy = null;
				return false;
			}
			newEnemy = this.enemyTypes[enemyTypeIndex];
			return true;
		}

		public bool SpawnEnemy(string spawnPointID, int enemyTypeIndex, [Nullable(2)] out AIAgent newEnemy)
		{
			if (!this.enemyTypes.ContainsKey(enemyTypeIndex))
			{
				Debug.Log("AISpawnManager::SpawnEnemy enemy index incorrect");
				newEnemy = null;
				return false;
			}
			AISpawnPoint aispawnPoint;
			if (!this.spawnPoints.TryGetValue(spawnPointID, out aispawnPoint))
			{
				Debug.Log("AISpawnManager::SpawnEnemy Can't find spawn point");
				newEnemy = null;
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.enemyTypes[enemyTypeIndex], aispawnPoint.transform);
			aispawnPoint.spawnCount++;
			newEnemy = gameObject.GetComponent<AIAgent>();
			return true;
		}

		[NullableContext(2)]
		public bool SpawnEnemy(int enemyTypeIndex, out AIAgent newEnemy)
		{
			if (!this.enemyTypes.ContainsKey(enemyTypeIndex))
			{
				Debug.Log("AISpawnManager::SpawnEnemy enemy index incorrect");
				newEnemy = null;
				return false;
			}
			GameObject gameObject = Object.Instantiate<GameObject>(this.enemyTypes[enemyTypeIndex]);
			newEnemy = gameObject.GetComponent<AIAgent>();
			return true;
		}

		private void GetEnemyTypeTemplates()
		{
			for (int i = 0; i < base.transform.childCount; i++)
			{
				Transform child = base.transform.GetChild(i);
				AIAgent component = child.GetComponent<AIAgent>();
				if (!(component == null) && component.isTemplate)
				{
					child.gameObject.SetActive(false);
					this.enemyTypes[(int)component.enemyTypeId] = child.gameObject;
				}
			}
		}

		private Dictionary<int, GameObject> enemyTypes = new Dictionary<int, GameObject>(64);

		[Nullable(2)]
		public static AISpawnManager instance;

		private static bool hasInstance;

		private Dictionary<string, AISpawnPoint> spawnPoints = new Dictionary<string, AISpawnPoint>(128);
	}
}
