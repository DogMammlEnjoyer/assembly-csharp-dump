using System;
using System.Collections;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.OpenXR
{
	internal class OpenXRRestarter : MonoBehaviour
	{
		public void ResetCallbacks()
		{
			this.onAfterRestart = null;
			this.onAfterSuccessfulRestart = null;
			this.onAfterShutdown = null;
			this.onAfterCoroutine = null;
			this.onQuit = null;
			OpenXRRestarter.m_pauseAndRestartAttempts = 0;
		}

		public bool isRunning
		{
			get
			{
				return this.m_Coroutine != null;
			}
		}

		public static float TimeBetweenRestartAttempts { get; set; } = 5f;

		public static int PauseAndRestartAttempts
		{
			get
			{
				return OpenXRRestarter.m_pauseAndRestartAttempts;
			}
		}

		internal static int PauseAndRestartCoroutineCount
		{
			get
			{
				return OpenXRRestarter.m_pauseAndRestartCoroutineCount;
			}
		}

		public static OpenXRRestarter Instance
		{
			get
			{
				if (OpenXRRestarter.s_Instance == null)
				{
					GameObject gameObject = GameObject.Find("~oxrestarter");
					if (gameObject == null)
					{
						gameObject = new GameObject("~oxrestarter");
						gameObject.hideFlags = HideFlags.HideAndDontSave;
						gameObject.AddComponent<OpenXRRestarter>();
					}
					OpenXRRestarter.s_Instance = gameObject.GetComponent<OpenXRRestarter>();
				}
				return OpenXRRestarter.s_Instance;
			}
		}

		internal static bool DisableApplicationQuit { get; set; } = false;

		public void Shutdown()
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				return;
			}
			if (this.m_Coroutine != null)
			{
				Debug.LogError("Only one shutdown or restart can be executed at a time");
				return;
			}
			this.m_Coroutine = base.StartCoroutine(this.RestartCoroutine(false, true));
		}

		public void ShutdownAndRestart()
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				return;
			}
			if (this.m_Coroutine != null)
			{
				Debug.LogError("Only one shutdown or restart can be executed at a time");
				return;
			}
			this.m_Coroutine = base.StartCoroutine(this.RestartCoroutine(true, true));
		}

		public void PauseAndShutdownAndRestart()
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				return;
			}
			base.StartCoroutine(this.PauseAndShutdownAndRestartCoroutine(OpenXRRestarter.TimeBetweenRestartAttempts));
		}

		public void PauseAndRetryInitialization()
		{
			if (OpenXRLoaderBase.Instance == null)
			{
				return;
			}
			base.StartCoroutine(this.PauseAndRetryInitializationCoroutine(OpenXRRestarter.TimeBetweenRestartAttempts));
		}

		private void IncrementPauseAndRestartCoroutineCount()
		{
			Object pauseAndRestartCoroutineCountLock = this.m_PauseAndRestartCoroutineCountLock;
			lock (pauseAndRestartCoroutineCountLock)
			{
				OpenXRRestarter.m_pauseAndRestartCoroutineCount++;
			}
		}

		private void DecrementPauseAndRestartCoroutineCount()
		{
			Object pauseAndRestartCoroutineCountLock = this.m_PauseAndRestartCoroutineCountLock;
			lock (pauseAndRestartCoroutineCountLock)
			{
				OpenXRRestarter.m_pauseAndRestartCoroutineCount--;
			}
		}

		private IEnumerator PauseAndShutdownAndRestartCoroutine(float pauseTimeInSeconds)
		{
			this.IncrementPauseAndRestartCoroutineCount();
			try
			{
				yield return new WaitForSeconds(pauseTimeInSeconds);
				yield return new WaitForRestartFinish(5f);
				OpenXRRestarter.m_pauseAndRestartAttempts++;
				this.m_Coroutine = base.StartCoroutine(this.RestartCoroutine(true, true));
			}
			finally
			{
				Action action = this.onAfterCoroutine;
				if (action != null)
				{
					action();
				}
			}
			this.DecrementPauseAndRestartCoroutineCount();
			yield break;
			yield break;
		}

		private IEnumerator PauseAndRetryInitializationCoroutine(float pauseTimeInSeconds)
		{
			this.IncrementPauseAndRestartCoroutineCount();
			try
			{
				yield return new WaitForSeconds(pauseTimeInSeconds);
				yield return new WaitForRestartFinish(5f);
				if (!(XRGeneralSettings.Instance.Manager.activeLoader != null))
				{
					OpenXRRestarter.m_pauseAndRestartAttempts++;
					this.m_Coroutine = base.StartCoroutine(this.RestartCoroutine(true, false));
				}
			}
			finally
			{
				Action action = this.onAfterCoroutine;
				if (action != null)
				{
					action();
				}
			}
			this.DecrementPauseAndRestartCoroutineCount();
			yield break;
			yield break;
		}

		private IEnumerator RestartCoroutine(bool shouldRestart, bool shouldShutdown)
		{
			try
			{
				if (shouldShutdown)
				{
					Debug.Log("Shutting down OpenXR.");
					yield return null;
					XRGeneralSettings.Instance.Manager.DeinitializeLoader();
					yield return null;
					Action action = this.onAfterShutdown;
					if (action != null)
					{
						action();
					}
				}
				if (shouldRestart && OpenXRRuntime.ShouldRestart())
				{
					Debug.Log("Initializing OpenXR.");
					yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
					XRGeneralSettings.Instance.Manager.StartSubsystems();
					if (XRGeneralSettings.Instance.Manager.activeLoader != null)
					{
						OpenXRRestarter.m_pauseAndRestartAttempts = 0;
						Action action2 = this.onAfterSuccessfulRestart;
						if (action2 != null)
						{
							action2();
						}
					}
					Action action3 = this.onAfterRestart;
					if (action3 != null)
					{
						action3();
					}
				}
				else if (OpenXRRuntime.ShouldQuit())
				{
					Action action4 = this.onQuit;
					if (action4 != null)
					{
						action4();
					}
					if (!OpenXRRestarter.DisableApplicationQuit)
					{
						Application.Quit();
					}
				}
			}
			finally
			{
				this.m_Coroutine = null;
				Action action5 = this.onAfterCoroutine;
				if (action5 != null)
				{
					action5();
				}
			}
			yield break;
			yield break;
		}

		internal Action onAfterRestart;

		internal Action onAfterShutdown;

		internal Action onQuit;

		internal Action onAfterCoroutine;

		internal Action onAfterSuccessfulRestart;

		private static OpenXRRestarter s_Instance;

		private Coroutine m_Coroutine;

		private static int m_pauseAndRestartCoroutineCount;

		private Object m_PauseAndRestartCoroutineCountLock = new Object();

		private static int m_pauseAndRestartAttempts;
	}
}
