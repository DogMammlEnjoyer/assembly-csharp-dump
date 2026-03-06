using System;

namespace UnityEngine.Rendering
{
	public static class ComponentSingleton<TType> where TType : Component
	{
		public static TType instance
		{
			get
			{
				if (ComponentSingleton<TType>.s_Instance == null)
				{
					GameObject gameObject = new GameObject("Default " + typeof(TType).Name);
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					Object.DontDestroyOnLoad(gameObject);
					gameObject.SetActive(false);
					ComponentSingleton<TType>.s_Instance = gameObject.AddComponent<TType>();
				}
				return ComponentSingleton<TType>.s_Instance;
			}
		}

		public static void Release()
		{
			if (ComponentSingleton<TType>.s_Instance != null)
			{
				CoreUtils.Destroy(ComponentSingleton<TType>.s_Instance.gameObject);
				ComponentSingleton<TType>.s_Instance = default(TType);
			}
		}

		private static TType s_Instance;
	}
}
