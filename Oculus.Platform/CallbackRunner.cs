using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Oculus.Platform
{
	public class CallbackRunner : MonoBehaviour
	{
		[DllImport("LibOVRPlatformImpl64_1")]
		private static extern void ovr_UnityResetTestPlatform();

		private void Awake()
		{
			if (Object.FindObjectOfType<CallbackRunner>() != this)
			{
				Debug.LogWarning("You only need one instance of CallbackRunner");
			}
			if (this.IsPersistantBetweenSceneLoads)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		private void Update()
		{
			Request.RunCallbacks(0U);
		}

		private void OnDestroy()
		{
		}

		private void OnApplicationQuit()
		{
			Callback.OnApplicationQuit();
		}

		public bool IsPersistantBetweenSceneLoads = true;
	}
}
