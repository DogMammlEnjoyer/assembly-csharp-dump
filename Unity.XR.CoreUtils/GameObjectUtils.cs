using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.XR.CoreUtils
{
	public static class GameObjectUtils
	{
		public static event Action<GameObject> GameObjectInstantiated;

		public static GameObject Create()
		{
			GameObject gameObject = new GameObject();
			Action<GameObject> gameObjectInstantiated = GameObjectUtils.GameObjectInstantiated;
			if (gameObjectInstantiated != null)
			{
				gameObjectInstantiated(gameObject);
			}
			return gameObject;
		}

		public static GameObject Create(string name)
		{
			GameObject gameObject = new GameObject(name);
			Action<GameObject> gameObjectInstantiated = GameObjectUtils.GameObjectInstantiated;
			if (gameObjectInstantiated != null)
			{
				gameObjectInstantiated(gameObject);
			}
			return gameObject;
		}

		public static GameObject Instantiate(GameObject original, Transform parent = null, bool worldPositionStays = true)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(original, parent, worldPositionStays);
			if (gameObject != null && GameObjectUtils.GameObjectInstantiated != null)
			{
				GameObjectUtils.GameObjectInstantiated(gameObject);
			}
			return gameObject;
		}

		public static GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation)
		{
			return GameObjectUtils.Instantiate(original, null, position, rotation);
		}

		public static GameObject Instantiate(GameObject original, Transform parent, Vector3 position, Quaternion rotation)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(original, position, rotation, parent);
			if (gameObject != null && GameObjectUtils.GameObjectInstantiated != null)
			{
				GameObjectUtils.GameObjectInstantiated(gameObject);
			}
			return gameObject;
		}

		public static GameObject CloneWithHideFlags(GameObject original, Transform parent = null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(original, parent);
			GameObjectUtils.CopyHideFlagsRecursively(original, gameObject);
			return gameObject;
		}

		private static void CopyHideFlagsRecursively(GameObject copyFrom, GameObject copyTo)
		{
			copyTo.hideFlags = copyFrom.hideFlags;
			Transform transform = copyFrom.transform;
			Transform transform2 = copyTo.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				GameObjectUtils.CopyHideFlagsRecursively(transform.GetChild(i).gameObject, transform2.GetChild(i).gameObject);
			}
		}

		public static T ExhaustiveComponentSearch<T>(GameObject desiredSource) where T : Component
		{
			T t = default(T);
			if (desiredSource != null)
			{
				t = desiredSource.GetComponentInChildren<T>(true);
			}
			if (t == null)
			{
				t = Object.FindAnyObjectByType<T>();
			}
			t != null;
			return t;
		}

		public static T ExhaustiveTaggedComponentSearch<T>(GameObject desiredSource, string tag) where T : Component
		{
			T t = default(T);
			if (desiredSource != null)
			{
				foreach (T t2 in desiredSource.GetComponentsInChildren<T>(true))
				{
					if (t2.gameObject.CompareTag(tag))
					{
						t = t2;
						break;
					}
				}
			}
			if (t == null)
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(tag);
				for (int i = 0; i < array.Length; i++)
				{
					t = array[i].GetComponent<T>();
					if (t != null)
					{
						break;
					}
				}
			}
			if (t == null)
			{
				t = Object.FindAnyObjectByType<T>();
			}
			return t;
		}

		public static T GetComponentInScene<T>(Scene scene) where T : Component
		{
			scene.GetRootGameObjects(GameObjectUtils.k_GameObjects);
			foreach (GameObject gameObject in GameObjectUtils.k_GameObjects)
			{
				T componentInChildren = gameObject.GetComponentInChildren<T>();
				if (componentInChildren)
				{
					return componentInChildren;
				}
			}
			return default(T);
		}

		public static void GetComponentsInScene<T>(Scene scene, List<T> components, bool includeInactive = false) where T : Component
		{
			scene.GetRootGameObjects(GameObjectUtils.k_GameObjects);
			foreach (GameObject gameObject in GameObjectUtils.k_GameObjects)
			{
				if (includeInactive || gameObject.activeInHierarchy)
				{
					components.AddRange(gameObject.GetComponentsInChildren<T>(includeInactive));
				}
			}
		}

		public static T GetComponentInActiveScene<T>() where T : Component
		{
			return GameObjectUtils.GetComponentInScene<T>(SceneManager.GetActiveScene());
		}

		public static void GetComponentsInActiveScene<T>(List<T> components, bool includeInactive = false) where T : Component
		{
			GameObjectUtils.GetComponentsInScene<T>(SceneManager.GetActiveScene(), components, includeInactive);
		}

		public static void GetComponentsInAllScenes<T>(List<T> components, bool includeInactive = false) where T : Component
		{
			int sceneCount = SceneManager.sceneCount;
			for (int i = 0; i < sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.isLoaded)
				{
					GameObjectUtils.GetComponentsInScene<T>(sceneAt, components, includeInactive);
				}
			}
		}

		public static void GetChildGameObjects(this GameObject go, List<GameObject> childGameObjects)
		{
			Transform transform = go.transform;
			int childCount = transform.childCount;
			if (childCount == 0)
			{
				return;
			}
			childGameObjects.EnsureCapacity(childCount);
			for (int i = 0; i < childCount; i++)
			{
				childGameObjects.Add(transform.GetChild(i).gameObject);
			}
		}

		public static GameObject GetNamedChild(this GameObject go, string name)
		{
			GameObjectUtils.k_Transforms.Clear();
			go.GetComponentsInChildren<Transform>(GameObjectUtils.k_Transforms);
			Transform transform = GameObjectUtils.k_Transforms.Find((Transform currentTransform) => currentTransform.name == name);
			GameObjectUtils.k_Transforms.Clear();
			if (transform != null)
			{
				return transform.gameObject;
			}
			return null;
		}

		private static readonly List<GameObject> k_GameObjects = new List<GameObject>();

		private static readonly List<Transform> k_Transforms = new List<Transform>();
	}
}
