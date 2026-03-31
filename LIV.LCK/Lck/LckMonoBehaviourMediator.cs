using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Liv.Lck
{
	[DefaultExecutionOrder(-1000)]
	public class LckMonoBehaviourMediator : MonoBehaviour
	{
		public static event LckMonoBehaviourMediator.LckApplicationLifecycleEventDelegate OnApplicationLifecycleEvent;

		public static LckMonoBehaviourMediator Instance
		{
			get
			{
				if (LckMonoBehaviourMediator._instance == null)
				{
					GameObject gameObject = new GameObject("LckMonoBehaviourMediator");
					LckMonoBehaviourMediator._instance = gameObject.AddComponent<LckMonoBehaviourMediator>();
					Object.DontDestroyOnLoad(gameObject);
				}
				return LckMonoBehaviourMediator._instance;
			}
		}

		private void Awake()
		{
			if (LckMonoBehaviourMediator._instance != null && LckMonoBehaviourMediator._instance != this)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			LckMonoBehaviourMediator._instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			LckMonoBehaviourMediator.LckApplicationLifecycleEventDelegate onApplicationLifecycleEvent = LckMonoBehaviourMediator.OnApplicationLifecycleEvent;
			if (onApplicationLifecycleEvent == null)
			{
				return;
			}
			onApplicationLifecycleEvent(pauseStatus ? LckMonoBehaviourMediator.ApplicationLifecycleEventType.Pause : LckMonoBehaviourMediator.ApplicationLifecycleEventType.Resume);
		}

		private void OnApplicationQuit()
		{
			LckMonoBehaviourMediator.LckApplicationLifecycleEventDelegate onApplicationLifecycleEvent = LckMonoBehaviourMediator.OnApplicationLifecycleEvent;
			if (onApplicationLifecycleEvent == null)
			{
				return;
			}
			onApplicationLifecycleEvent(LckMonoBehaviourMediator.ApplicationLifecycleEventType.Quit);
		}

		private void Update()
		{
			this.HMDMountedOnHeadStateChange();
			LckMonoBehaviourMediator.ProcessExectionQueue();
		}

		private void HMDMountedOnHeadStateChange()
		{
			if (this._hMDFound)
			{
				Vector3 vector;
				this._hmd.TryGetFeatureValue(CommonUsages.deviceVelocity, out vector);
				if (vector.magnitude > 0.01f)
				{
					if (this._hMDIsIdle)
					{
						this._hMDIsIdle = false;
						LckMonoBehaviourMediator.LckApplicationLifecycleEventDelegate onApplicationLifecycleEvent = LckMonoBehaviourMediator.OnApplicationLifecycleEvent;
						if (onApplicationLifecycleEvent != null)
						{
							onApplicationLifecycleEvent(LckMonoBehaviourMediator.ApplicationLifecycleEventType.HMDActive);
						}
					}
					this._hMDWasMoving = true;
					this._hMDIdleTime = 0f;
					return;
				}
				if (this._hMDWasMoving)
				{
					this._hMDIdleTime += Time.deltaTime;
					if (this._hMDIdleTime >= 10f)
					{
						this._hMDIsIdle = true;
						LckMonoBehaviourMediator.LckApplicationLifecycleEventDelegate onApplicationLifecycleEvent2 = LckMonoBehaviourMediator.OnApplicationLifecycleEvent;
						if (onApplicationLifecycleEvent2 != null)
						{
							onApplicationLifecycleEvent2(LckMonoBehaviourMediator.ApplicationLifecycleEventType.HMDIdle);
						}
						this._hMDWasMoving = false;
						return;
					}
				}
			}
			else
			{
				List<InputDevice> list = new List<InputDevice>();
				InputDevices.GetDevices(list);
				foreach (InputDevice hmd in list)
				{
					if (hmd.characteristics.HasFlag(InputDeviceCharacteristics.HeadMounted))
					{
						this._hmd = hmd;
						this._hMDFound = true;
					}
				}
			}
		}

		private static void ProcessExectionQueue()
		{
			Queue<Action> executionQueue = LckMonoBehaviourMediator._executionQueue;
			lock (executionQueue)
			{
				while (LckMonoBehaviourMediator._executionQueue.Count > 0)
				{
					LckMonoBehaviourMediator._executionQueue.Dequeue()();
				}
			}
		}

		public static T[] FindObjectsOfComponentType<T>() where T : Object
		{
			return Object.FindObjectsOfType<T>();
		}

		public static T AddComponentToMediator<T>() where T : Component
		{
			return LckMonoBehaviourMediator.Instance.gameObject.AddComponent<T>();
		}

		public static Coroutine StartCoroutine(string coroutineName, IEnumerator routine)
		{
			return LckMonoBehaviourMediator.Instance.StartCoroutineInternal(coroutineName, routine);
		}

		public static void StopCoroutineByName(string coroutineName)
		{
			if (LckMonoBehaviourMediator._instance != null)
			{
				LckMonoBehaviourMediator.Instance.StopCoroutineInternal(coroutineName);
			}
		}

		public static void StopAllActiveCoroutines()
		{
			if (LckMonoBehaviourMediator._instance != null)
			{
				LckMonoBehaviourMediator.Instance.StopAllCoroutinesInternal();
			}
		}

		public void EnqueueMainThreadAction(Action action)
		{
			Queue<Action> executionQueue = LckMonoBehaviourMediator._executionQueue;
			lock (executionQueue)
			{
				LckMonoBehaviourMediator._executionQueue.Enqueue(action);
			}
		}

		private Coroutine StartCoroutineInternal(string coroutineName, IEnumerator routine)
		{
			if (this._activeCoroutines.ContainsKey(coroutineName))
			{
				this.StopCoroutineInternal(coroutineName);
			}
			Coroutine coroutine = base.StartCoroutine(routine);
			this._activeCoroutines[coroutineName] = coroutine;
			return coroutine;
		}

		private void StopCoroutineInternal(string coroutineName)
		{
			Coroutine routine;
			if (this._activeCoroutines.TryGetValue(coroutineName, out routine))
			{
				try
				{
					base.StopCoroutine(routine);
				}
				catch (Exception)
				{
				}
				this._activeCoroutines.Remove(coroutineName);
			}
		}

		private void StopAllCoroutinesInternal()
		{
			base.StopAllCoroutines();
			this._activeCoroutines.Clear();
		}

		private void OnDestroy()
		{
			this.StopAllCoroutinesInternal();
		}

		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		private static LckMonoBehaviourMediator _instance;

		private const float DurationForHMDToBecomeIdle = 10f;

		private float _hMDIdleTime;

		private Dictionary<string, Coroutine> _activeCoroutines = new Dictionary<string, Coroutine>();

		private bool _hMDFound;

		private bool _hMDWasMoving;

		private bool _hMDIsIdle;

		private InputDevice _hmd;

		public enum ApplicationLifecycleEventType
		{
			Quit,
			Pause,
			Resume,
			HMDIdle,
			HMDActive
		}

		public delegate void LckApplicationLifecycleEventDelegate(LckMonoBehaviourMediator.ApplicationLifecycleEventType applicationLifecycleEventType);
	}
}
