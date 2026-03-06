using System;

namespace UnityEngine.ResourceManagement.Util
{
	[ExecuteInEditMode]
	public abstract class ComponentSingleton<T> : MonoBehaviour where T : ComponentSingleton<T>
	{
		public static bool Exists
		{
			get
			{
				return ComponentSingleton<T>.s_Instance != null;
			}
		}

		public static T Instance
		{
			get
			{
				if (ComponentSingleton<T>.s_Instance == null)
				{
					T t;
					if ((t = ComponentSingleton<T>.FindInstance()) == null)
					{
						t = ComponentSingleton<T>.CreateNewSingleton();
					}
					ComponentSingleton<T>.s_Instance = t;
				}
				return ComponentSingleton<T>.s_Instance;
			}
		}

		private static T FindInstance()
		{
			return Object.FindObjectOfType<T>();
		}

		protected virtual string GetGameObjectName()
		{
			return typeof(T).Name;
		}

		private static T CreateNewSingleton()
		{
			GameObject gameObject = new GameObject();
			if (Application.isPlaying)
			{
				Object.DontDestroyOnLoad(gameObject);
				gameObject.hideFlags = HideFlags.DontSave;
			}
			else
			{
				gameObject.hideFlags = HideFlags.HideAndDontSave;
			}
			T t = gameObject.AddComponent<T>();
			gameObject.name = t.GetGameObjectName();
			return t;
		}

		private void Awake()
		{
			if (ComponentSingleton<T>.s_Instance != null && ComponentSingleton<T>.s_Instance != this)
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
			ComponentSingleton<T>.s_Instance = (this as T);
		}

		public static void DestroySingleton()
		{
			if (ComponentSingleton<T>.Exists)
			{
				Object.DestroyImmediate(ComponentSingleton<T>.Instance.gameObject);
				ComponentSingleton<T>.s_Instance = default(T);
			}
		}

		private static T s_Instance;
	}
}
