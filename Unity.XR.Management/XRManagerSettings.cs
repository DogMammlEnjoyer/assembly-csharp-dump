using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace UnityEngine.XR.Management
{
	public sealed class XRManagerSettings : ScriptableObject
	{
		public bool automaticLoading
		{
			get
			{
				return this.m_AutomaticLoading;
			}
			set
			{
				this.m_AutomaticLoading = value;
			}
		}

		public bool automaticRunning
		{
			get
			{
				return this.m_AutomaticRunning;
			}
			set
			{
				this.m_AutomaticRunning = value;
			}
		}

		[Obsolete("'XRManagerSettings.loaders' property is obsolete. Use 'XRManagerSettings.activeLoaders' instead to get a list of the current loaders.")]
		public List<XRLoader> loaders
		{
			get
			{
				return this.m_Loaders;
			}
		}

		public IReadOnlyList<XRLoader> activeLoaders
		{
			get
			{
				return this.m_Loaders;
			}
		}

		public bool isInitializationComplete
		{
			get
			{
				return this.m_InitializationComplete;
			}
		}

		[HideInInspector]
		public XRLoader activeLoader { get; private set; }

		public T ActiveLoaderAs<T>() where T : XRLoader
		{
			return this.activeLoader as T;
		}

		public void InitializeLoaderSync()
		{
			if (this.activeLoader != null)
			{
				Debug.LogWarning("XR Management has already initialized an active loader in this scene. Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
				return;
			}
			foreach (XRLoader xrloader in this.currentLoaders)
			{
				if (xrloader != null && this.CheckGraphicsAPICompatibility(xrloader) && xrloader.Initialize())
				{
					this.activeLoader = xrloader;
					this.m_InitializationComplete = true;
					return;
				}
			}
			this.activeLoader = null;
		}

		public IEnumerator InitializeLoader()
		{
			if (this.activeLoader != null)
			{
				Debug.LogWarning("XR Management has already initialized an active loader in this scene. Please make sure to stop all subsystems and deinitialize the active loader before initializing a new one.");
				yield break;
			}
			foreach (XRLoader xrloader in this.currentLoaders)
			{
				if (xrloader != null && this.CheckGraphicsAPICompatibility(xrloader) && xrloader.Initialize())
				{
					this.activeLoader = xrloader;
					this.m_InitializationComplete = true;
					yield break;
				}
				yield return null;
			}
			List<XRLoader>.Enumerator enumerator = default(List<XRLoader>.Enumerator);
			this.activeLoader = null;
			yield break;
			yield break;
		}

		public bool TryAddLoader(XRLoader loader, int index = -1)
		{
			if (loader == null || this.currentLoaders.Contains(loader))
			{
				return false;
			}
			if (!this.m_RegisteredLoaders.Contains(loader))
			{
				return false;
			}
			if (index < 0 || index >= this.currentLoaders.Count)
			{
				this.currentLoaders.Add(loader);
			}
			else
			{
				this.currentLoaders.Insert(index, loader);
			}
			return true;
		}

		public bool TryRemoveLoader(XRLoader loader)
		{
			bool result = true;
			if (this.currentLoaders.Contains(loader))
			{
				result = this.currentLoaders.Remove(loader);
			}
			return result;
		}

		public bool TrySetLoaders(List<XRLoader> reorderedLoaders)
		{
			List<XRLoader> currentLoaders = new List<XRLoader>(this.activeLoaders);
			this.currentLoaders.Clear();
			foreach (XRLoader loader in reorderedLoaders)
			{
				if (!this.TryAddLoader(loader, -1))
				{
					this.currentLoaders = currentLoaders;
					return false;
				}
			}
			return true;
		}

		private void Awake()
		{
			foreach (XRLoader item in this.currentLoaders)
			{
				if (!this.m_RegisteredLoaders.Contains(item))
				{
					this.m_RegisteredLoaders.Add(item);
				}
			}
		}

		private bool CheckGraphicsAPICompatibility(XRLoader loader)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			List<GraphicsDeviceType> supportedGraphicsDeviceTypes = loader.GetSupportedGraphicsDeviceTypes(false);
			if (supportedGraphicsDeviceTypes.Count > 0 && !supportedGraphicsDeviceTypes.Contains(graphicsDeviceType))
			{
				Debug.LogWarning(string.Format("The {0} does not support the initialized graphics device, {1}. Please change the preffered Graphics API in PlayerSettings. Attempting to start the next XR loader.", loader.name, graphicsDeviceType.ToString()));
				return false;
			}
			return true;
		}

		public void StartSubsystems()
		{
			if (!this.m_InitializationComplete)
			{
				Debug.LogWarning("Call to StartSubsystems without an initialized manager.Please make sure wait for initialization to complete before calling this API.");
				return;
			}
			if (this.activeLoader != null)
			{
				this.activeLoader.Start();
			}
		}

		public void StopSubsystems()
		{
			if (!this.m_InitializationComplete)
			{
				Debug.LogWarning("Call to StopSubsystems without an initialized manager.Please make sure wait for initialization to complete before calling this API.");
				return;
			}
			if (this.activeLoader != null)
			{
				this.activeLoader.Stop();
			}
		}

		public void DeinitializeLoader()
		{
			if (!this.m_InitializationComplete)
			{
				Debug.LogWarning("Call to DeinitializeLoader without an initialized manager.Please make sure wait for initialization to complete before calling this API.");
				return;
			}
			this.StopSubsystems();
			if (this.activeLoader != null)
			{
				this.activeLoader.Deinitialize();
				this.activeLoader = null;
			}
			this.m_InitializationComplete = false;
		}

		private void Start()
		{
			if (this.automaticLoading && this.automaticRunning)
			{
				this.StartSubsystems();
			}
		}

		private void OnDisable()
		{
			if (this.automaticLoading && this.automaticRunning)
			{
				this.StopSubsystems();
			}
		}

		private void OnDestroy()
		{
			if (this.automaticLoading)
			{
				this.DeinitializeLoader();
			}
		}

		internal List<XRLoader> currentLoaders
		{
			get
			{
				return this.m_Loaders;
			}
			set
			{
				this.m_Loaders = value;
			}
		}

		internal HashSet<XRLoader> registeredLoaders
		{
			get
			{
				return this.m_RegisteredLoaders;
			}
		}

		[HideInInspector]
		private bool m_InitializationComplete;

		[HideInInspector]
		[SerializeField]
		private bool m_RequiresSettingsUpdate;

		[SerializeField]
		[Tooltip("Determines if the XR Manager instance is responsible for creating and destroying the appropriate loader instance.")]
		[FormerlySerializedAs("AutomaticLoading")]
		private bool m_AutomaticLoading;

		[SerializeField]
		[Tooltip("Determines if the XR Manager instance is responsible for starting and stopping subsystems for the active loader instance.")]
		[FormerlySerializedAs("AutomaticRunning")]
		private bool m_AutomaticRunning;

		[SerializeField]
		[Tooltip("List of XR Loader instances arranged in desired load order.")]
		[FormerlySerializedAs("Loaders")]
		private List<XRLoader> m_Loaders = new List<XRLoader>();

		[SerializeField]
		[HideInInspector]
		private HashSet<XRLoader> m_RegisteredLoaders = new HashSet<XRLoader>();
	}
}
