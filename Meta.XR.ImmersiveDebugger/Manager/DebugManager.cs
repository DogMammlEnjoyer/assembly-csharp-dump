using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Hierarchy;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal class DebugManager : MonoBehaviour
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			DebugManager.Instance = null;
		}

		public static DebugManager Instance { get; private set; }

		public static event Action<DebugManager> OnReady;

		public event Action OnFocusLostAction;

		public event Action OnDisableAction;

		public event Action OnUpdateAction;

		internal event DebugManager.ShouldRetrieveInstanceDelegate CustomShouldRetrieveInstanceCondition;

		public IDebugUIPanel UiPanel { get; private set; }

		private void Awake()
		{
			DebugManager.Instance = this;
			this.InstanceCache.OnCacheChangedForTypeEvent += this.ProcessLoadedTypeBySubManagers;
			this.InstanceCache.OnInstanceRemoved += this.UnregisterInspector;
		}

		private void Start()
		{
			Telemetry.TelemetryTracker telemetryTracker = Telemetry.TelemetryTracker.Init(Telemetry.Method.Attributes, this.SubDebugManagers, this.InstanceCache, this);
			this.UiPanel = base.GetComponentInChildren<IDebugUIPanel>(true);
			this.InitSubManagers();
			AssemblyParser.RegisterAssemblyTypes(new Action<List<Type>>(this.InstanceCache.RegisterClassTypes));
			this.RegisterTypesFromInspectedData();
			this.ShouldRetrieveInstances = true;
			this.RetrieveInstancesIfNeeded();
			Action<DebugManager> onReady = DebugManager.OnReady;
			if (onReady != null)
			{
				onReady(this);
			}
			telemetryTracker.OnStart();
			Manager instance = DebugManagerAddon<Manager>.Instance;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				Action onFocusLostAction = this.OnFocusLostAction;
				if (onFocusLostAction == null)
				{
					return;
				}
				onFocusLostAction();
			}
		}

		private void OnDisable()
		{
			Action onDisableAction = this.OnDisableAction;
			if (onDisableAction == null)
			{
				return;
			}
			onDisableAction();
		}

		private void OnDestroy()
		{
			AssemblyParser.Unregister(new Action<List<Type>>(this.InstanceCache.RegisterClassTypes));
		}

		private void Update()
		{
			this.RetrieveInstancesIfNeeded();
			Action onUpdateAction = this.OnUpdateAction;
			if (onUpdateAction != null)
			{
				onUpdateAction();
			}
			this._frameUpdateRecorder.Send();
		}

		private void RetrieveInstancesIfNeeded()
		{
			if (Time.time - this._lastRetrievedTime > 1f)
			{
				this.ShouldRetrieveInstances = true;
			}
			if (this.ShouldRetrieveInstances && this.CustomShouldRetrieveInstanceCondition != null)
			{
				this.ShouldRetrieveInstances = this.CustomShouldRetrieveInstanceCondition();
			}
			if (!this.ShouldRetrieveInstances)
			{
				return;
			}
			this._frameUpdateRecorder.Start();
			this.InstanceCache.RetrieveInstances();
			this._lastRetrievedTime = Time.time;
			this.ShouldRetrieveInstances = false;
		}

		protected virtual void InitSubManagers()
		{
			this.RegisterManager<GizmoManager>();
			this.RegisterManager<WatchManager>();
			this.RegisterManager<ActionManager>();
			this.RegisterManager<TweakManager>();
		}

		private void RegisterManager<TManagerType>() where TManagerType : IDebugManager, new()
		{
			TManagerType tmanagerType = Activator.CreateInstance<TManagerType>();
			tmanagerType.Setup(this.UiPanel, this.InstanceCache);
			this.SubDebugManagers.Add(tmanagerType);
		}

		private void ProcessLoadedTypeBySubManagers(Type type)
		{
			foreach (IDebugManager debugManager in this.SubDebugManagers)
			{
				debugManager.ProcessType(type);
			}
		}

		private void UnregisterInspector(InstanceHandle handle)
		{
			this.UiPanel.UnregisterInspector(handle, Category.Default, true);
		}

		private void RegisterTypesFromInspectedData()
		{
			InspectedDataRegistry.Reset();
			List<InspectedData> inspectedDataAssets = RuntimeSettings.Instance.InspectedDataAssets;
			List<bool> inspectedDataEnabled = RuntimeSettings.Instance.InspectedDataEnabled;
			for (int i = 0; i < inspectedDataAssets.Count; i++)
			{
				if (inspectedDataEnabled[i])
				{
					this.InstanceCache.RegisterClassTypes(inspectedDataAssets[i].ExtractTypesFromInspectedMembers());
				}
			}
		}

		protected readonly InstanceCache InstanceCache = new InstanceCache();

		protected readonly List<IDebugManager> SubDebugManagers = new List<IDebugManager>();

		internal bool ShouldRetrieveInstances;

		private const float RetrievalIntervalInSec = 1f;

		private float _lastRetrievedTime;

		private readonly OVRSampledEventSender _frameUpdateRecorder = new OVRSampledEventSender(163056655, 0.1f, (OVRTelemetryMarker marker) => marker.AddPlayModeOrigin());

		internal delegate bool ShouldRetrieveInstanceDelegate();
	}
}
