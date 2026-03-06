using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Meta.Net.NativeWebSocket
{
	public class MainThreadUtil : MonoBehaviour
	{
		public static MainThreadUtil Instance { get; private set; }

		public static SynchronizationContext synchronizationContext { get; private set; }

		private void Awake()
		{
			base.gameObject.hideFlags = HideFlags.HideAndDontSave;
			Object.DontDestroyOnLoad(base.gameObject);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Setup()
		{
			MainThreadUtil.Instance = new GameObject("MainThreadUtil").AddComponent<MainThreadUtil>();
			MainThreadUtil.synchronizationContext = SynchronizationContext.Current;
		}

		public static void Run(IEnumerator waitForUpdate)
		{
			if (!MainThreadUtil.Instance)
			{
				Debug.LogWarning("Attempting to run on main thread after shutdown.");
				throw new Exception("Attempting to run on main thread after shutdown.");
			}
			MainThreadUtil.synchronizationContext.Post(delegate(object _)
			{
				MainThreadUtil.Instance.StartCoroutine(waitForUpdate);
			}, null);
		}
	}
}
