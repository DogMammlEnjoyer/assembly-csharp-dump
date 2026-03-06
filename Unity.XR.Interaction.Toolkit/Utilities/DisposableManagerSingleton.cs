using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Collections;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	[AddComponentMenu("")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@3.2/api/UnityEngine.XR.Interaction.Toolkit.Utilities.DisposableManagerSingleton.html")]
	internal sealed class DisposableManagerSingleton : MonoBehaviour
	{
		private static DisposableManagerSingleton instance
		{
			get
			{
				return DisposableManagerSingleton.Initialize();
			}
		}

		private static DisposableManagerSingleton Initialize()
		{
			if (DisposableManagerSingleton.s_DisposableManagerSingleton == null)
			{
				GameObject gameObject = new GameObject("[DisposableManagerSingleton]");
				Object.DontDestroyOnLoad(gameObject);
				DisposableManagerSingleton.s_DisposableManagerSingleton = gameObject.AddComponent<DisposableManagerSingleton>();
			}
			return DisposableManagerSingleton.s_DisposableManagerSingleton;
		}

		private void Awake()
		{
			if (DisposableManagerSingleton.s_DisposableManagerSingleton != null && DisposableManagerSingleton.s_DisposableManagerSingleton != this)
			{
				Object.Destroy(this);
				return;
			}
			if (DisposableManagerSingleton.s_DisposableManagerSingleton == null)
			{
				DisposableManagerSingleton.s_DisposableManagerSingleton = this;
			}
		}

		private void OnDestroy()
		{
			this.DisposeAll();
		}

		private void OnApplicationQuit()
		{
			this.DisposeAll();
		}

		private void DisposeAll()
		{
			IReadOnlyList<IDisposable> readOnlyList = this.m_Disposables.AsList();
			for (int i = 0; i < readOnlyList.Count; i++)
			{
				readOnlyList[i].Dispose();
			}
			this.m_Disposables.Clear();
		}

		public static void RegisterDisposable(IDisposable disposableToRegister)
		{
			DisposableManagerSingleton.instance.m_Disposables.Add(disposableToRegister);
		}

		private static DisposableManagerSingleton s_DisposableManagerSingleton;

		private readonly HashSetList<IDisposable> m_Disposables = new HashSetList<IDisposable>(0);
	}
}
