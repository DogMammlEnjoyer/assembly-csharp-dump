using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace Valve.VR
{
	public class SteamVR_Behaviour : MonoBehaviour
	{
		public static SteamVR_Behaviour instance
		{
			get
			{
				if (SteamVR_Behaviour._instance == null)
				{
					SteamVR_Behaviour.Initialize(false);
				}
				return SteamVR_Behaviour._instance;
			}
		}

		public static void Initialize(bool forceUnityVRToOpenVR = false)
		{
			if (SteamVR_Behaviour._instance == null && !SteamVR_Behaviour.initializing)
			{
				SteamVR_Behaviour.initializing = true;
				GameObject gameObject = null;
				if (forceUnityVRToOpenVR)
				{
					SteamVR_Behaviour.forcingInitialization = true;
				}
				SteamVR_Render steamVR_Render = Object.FindObjectOfType<SteamVR_Render>();
				if (steamVR_Render != null)
				{
					gameObject = steamVR_Render.gameObject;
				}
				SteamVR_Behaviour steamVR_Behaviour = Object.FindObjectOfType<SteamVR_Behaviour>();
				if (steamVR_Behaviour != null)
				{
					gameObject = steamVR_Behaviour.gameObject;
				}
				if (gameObject == null)
				{
					GameObject gameObject2 = new GameObject("[SteamVR]");
					SteamVR_Behaviour._instance = gameObject2.AddComponent<SteamVR_Behaviour>();
					SteamVR_Behaviour._instance.steamvr_render = gameObject2.AddComponent<SteamVR_Render>();
				}
				else
				{
					steamVR_Behaviour = gameObject.GetComponent<SteamVR_Behaviour>();
					if (steamVR_Behaviour == null)
					{
						steamVR_Behaviour = gameObject.AddComponent<SteamVR_Behaviour>();
					}
					if (steamVR_Render != null)
					{
						steamVR_Behaviour.steamvr_render = steamVR_Render;
					}
					else
					{
						steamVR_Behaviour.steamvr_render = gameObject.GetComponent<SteamVR_Render>();
						if (steamVR_Behaviour.steamvr_render == null)
						{
							steamVR_Behaviour.steamvr_render = gameObject.AddComponent<SteamVR_Render>();
						}
					}
					SteamVR_Behaviour._instance = steamVR_Behaviour;
				}
				if (SteamVR_Behaviour._instance != null && SteamVR_Behaviour._instance.doNotDestroy)
				{
					Object.DontDestroyOnLoad(SteamVR_Behaviour._instance.transform.root.gameObject);
				}
				SteamVR_Behaviour.initializing = false;
			}
		}

		protected void Awake()
		{
			SteamVR_Behaviour.isPlaying = true;
			if (this.initializeSteamVROnAwake && !SteamVR_Behaviour.forcingInitialization)
			{
				this.InitializeSteamVR(false);
			}
		}

		public void InitializeSteamVR(bool forceUnityVRToOpenVR = false)
		{
			if (!forceUnityVRToOpenVR)
			{
				SteamVR.Initialize(false);
				return;
			}
			SteamVR_Behaviour.forcingInitialization = true;
			if (this.initializeCoroutine != null)
			{
				base.StopCoroutine(this.initializeCoroutine);
			}
			if (XRSettings.loadedDeviceName == "OpenVR")
			{
				this.EnableOpenVR();
				return;
			}
			this.initializeCoroutine = base.StartCoroutine(this.DoInitializeSteamVR(forceUnityVRToOpenVR));
		}

		private IEnumerator DoInitializeSteamVR(bool forceUnityVRToOpenVR = false)
		{
			XRDevice.deviceLoaded += this.XRDevice_deviceLoaded;
			XRSettings.LoadDeviceByName("OpenVR");
			while (!this.loadedOpenVRDeviceSuccess)
			{
				yield return null;
			}
			XRDevice.deviceLoaded -= this.XRDevice_deviceLoaded;
			this.EnableOpenVR();
			yield break;
		}

		private void XRDevice_deviceLoaded(string deviceName)
		{
			if (deviceName == "OpenVR")
			{
				this.loadedOpenVRDeviceSuccess = true;
				return;
			}
			Debug.LogError("<b>[SteamVR]</b> Tried to async load: OpenVR. Loaded: " + deviceName, this);
			this.loadedOpenVRDeviceSuccess = true;
		}

		private void EnableOpenVR()
		{
			XRSettings.enabled = true;
			SteamVR.Initialize(false);
			this.initializeCoroutine = null;
			SteamVR_Behaviour.forcingInitialization = false;
		}

		protected void OnEnable()
		{
			Application.onBeforeRender += this.OnBeforeRender;
			SteamVR_Events.System(EVREventType.VREvent_Quit).Listen(new UnityAction<VREvent_t>(this.OnQuit));
		}

		protected void OnDisable()
		{
			Application.onBeforeRender -= this.OnBeforeRender;
			SteamVR_Events.System(EVREventType.VREvent_Quit).Remove(new UnityAction<VREvent_t>(this.OnQuit));
		}

		protected void OnBeforeRender()
		{
			this.PreCull();
		}

		protected void PreCull()
		{
			if (OpenVR.Input != null && Time.frameCount != SteamVR_Behaviour.lastFrameCount)
			{
				SteamVR_Behaviour.lastFrameCount = Time.frameCount;
				SteamVR_Input.OnPreCull();
			}
		}

		protected void FixedUpdate()
		{
			if (OpenVR.Input != null)
			{
				SteamVR_Input.FixedUpdate();
			}
		}

		protected void LateUpdate()
		{
			if (OpenVR.Input != null)
			{
				SteamVR_Input.LateUpdate();
			}
		}

		protected void Update()
		{
			if (OpenVR.Input != null)
			{
				SteamVR_Input.Update();
			}
		}

		protected void OnQuit(VREvent_t vrEvent)
		{
			Application.Quit();
		}

		private const string openVRDeviceName = "OpenVR";

		public static bool forcingInitialization = false;

		private static SteamVR_Behaviour _instance;

		public bool initializeSteamVROnAwake = true;

		public bool doNotDestroy = true;

		[HideInInspector]
		public SteamVR_Render steamvr_render;

		internal static bool isPlaying = false;

		private static bool initializing = false;

		private Coroutine initializeCoroutine;

		private bool loadedOpenVRDeviceSuccess;

		protected static int lastFrameCount = -1;
	}
}
