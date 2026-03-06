using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal abstract class DebugManagerAddon<Type> where Type : DebugManagerAddon<Type>, new()
	{
		public static Type Instance
		{
			get
			{
				if (DebugManagerAddon<Type>._instance == null)
				{
					DebugManagerAddon<Type>._instance = Activator.CreateInstance<Type>();
					DebugManagerAddon<Type>._instance.Setup();
				}
				return DebugManagerAddon<Type>._instance;
			}
		}

		private void Setup()
		{
			if (DebugManager.Instance == null)
			{
				DebugManager.OnReady -= this.OnReady;
				DebugManager.OnReady += this.OnReady;
				return;
			}
			this.OnReady(DebugManager.Instance);
		}

		internal static void Destroy()
		{
			if (DebugManagerAddon<Type>._instance != null)
			{
				DebugManager.OnReady -= DebugManagerAddon<Type>._instance.OnReady;
			}
		}

		private void InitSubManagers()
		{
			foreach (IDebugManager debugManager in DebugManagerAddon<Type>._subManagersToInitialize)
			{
				debugManager.Setup(DebugManagerAddon<Type>._uiPanel, this._instanceCache);
				this._subDebugManagers.Add(debugManager);
			}
		}

		private void OnReady(DebugManager debugManager)
		{
			Telemetry.TelemetryTracker telemetryTracker = Telemetry.TelemetryTracker.Init(this.Method, this._subDebugManagers, this._instanceCache, debugManager);
			DebugManagerAddon<Type>._uiPanel = debugManager.UiPanel;
			this.InitSubManagers();
			this.OnReadyInternal();
			telemetryTracker.OnStart();
		}

		protected abstract Telemetry.Method Method { get; }

		private static List<IDebugManager> _subManagersToInitialize
		{
			get
			{
				return new List<IDebugManager>
				{
					new GizmoManagerForAddon(),
					new WatchManagerForAddon(),
					new ActionManagerForAddon(),
					new TweakManagerForAddon()
				};
			}
		}

		protected virtual void OnReadyInternal()
		{
		}

		private static Type _instance = default(Type);

		protected static IDebugUIPanel _uiPanel = null;

		protected readonly InstanceCache _instanceCache = new InstanceCache();

		protected readonly List<IDebugManager> _subDebugManagers = new List<IDebugManager>();
	}
}
