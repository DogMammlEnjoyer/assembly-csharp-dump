using System;
using UnityEngine;

public class MothershipWebSocketDispatcher : MonoBehaviour
{
	public static MothershipWebSocketDispatcher instance
	{
		get
		{
			if (MothershipWebSocketDispatcher._instance == null)
			{
				MothershipWebSocketDispatcher._instance = Object.FindFirstObjectByType<MothershipWebSocketDispatcher>();
				if (MothershipWebSocketDispatcher._instance == null)
				{
					MothershipWebSocketDispatcher._instance = new GameObject("MothershipWebSocketDispatcher").AddComponent<MothershipWebSocketDispatcher>();
				}
			}
			return MothershipWebSocketDispatcher._instance;
		}
	}

	public void Awake()
	{
		if (MothershipWebSocketDispatcher._instance != null && MothershipWebSocketDispatcher._instance != this)
		{
			Debug.LogWarning("WebSocket: Duplicate MothershipWebSocketDispatcher found. Destroying.");
			Object.DestroyImmediate(base.gameObject);
			return;
		}
		MothershipWebSocketDispatcher._instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Update()
	{
		if (MothershipWebSocketDispatcher._isApplicationQuitting)
		{
			return;
		}
		MothershipClientApiUnity.TickWebSockets(Time.deltaTime);
	}

	private void OnApplicationQuit()
	{
		MothershipWebSocketDispatcher._isApplicationQuitting = true;
		MothershipClientApiUnity.CloseWebSockets();
	}

	private void OnDestroy()
	{
		if (MothershipWebSocketDispatcher._instance == this)
		{
			MothershipClientApiUnity.CloseWebSockets();
			MothershipWebSocketDispatcher._instance = null;
		}
	}

	private static MothershipWebSocketDispatcher _instance;

	private static bool _isApplicationQuitting;
}
