using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class MainThreadUtil : MonoBehaviour
{
	public static MainThreadUtil Instance { get; private set; }

	public static SynchronizationContext synchronizationContext { get; private set; }

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Setup()
	{
		MainThreadUtil.Instance = new GameObject("MainThreadUtil").AddComponent<MainThreadUtil>();
		MainThreadUtil.synchronizationContext = SynchronizationContext.Current;
	}

	public static void Run(IEnumerator waitForUpdate)
	{
		MainThreadUtil.synchronizationContext.Post(delegate(object _)
		{
			MainThreadUtil.Instance.StartCoroutine(waitForUpdate);
		}, null);
	}

	private void Awake()
	{
		base.gameObject.hideFlags = HideFlags.HideAndDontSave;
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
