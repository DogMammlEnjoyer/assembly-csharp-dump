using System;
using UnityEngine;

namespace Meta.XR.MRUtilityKit.SceneDecorator
{
	public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
	{
		public static T Instance
		{
			get
			{
				if (SingletonMonoBehaviour<T>._instance == null && Application.isPlaying)
				{
					T[] array = Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
					if (array.Length != 0)
					{
						SingletonMonoBehaviour<T>._instance = array[0];
					}
				}
				return SingletonMonoBehaviour<T>._instance;
			}
		}

		private static void InitializeSingleton()
		{
			SingletonMonoBehaviour.InstantiationSettings instantiationSettings = Attribute.GetCustomAttribute(typeof(T), typeof(SingletonMonoBehaviour.InstantiationSettings)) as SingletonMonoBehaviour.InstantiationSettings;
			if (instantiationSettings != null && instantiationSettings.dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(SingletonMonoBehaviour<T>._instance.transform);
			}
		}

		protected virtual void Awake()
		{
			if (SingletonMonoBehaviour<T>._instance == null)
			{
				SingletonMonoBehaviour<T>._instance = (this as T);
				SingletonMonoBehaviour<T>.InitializeSingleton();
				return;
			}
			if (SingletonMonoBehaviour<T>._instance != this)
			{
				Debug.LogWarning(string.Format("An instance of {0} already exists, destroying this instance.", typeof(T)));
				Object.Destroy(this);
			}
		}

		protected virtual void OnDestroy()
		{
			SingletonMonoBehaviour<T>._instance = default(T);
		}

		private static T _instance;
	}
}
