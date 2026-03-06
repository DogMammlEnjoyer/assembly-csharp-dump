using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	public class RuntimeSettings : OVRRuntimeAssetsBase, ISerializationCallbackReceiver
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void Init()
		{
			RuntimeSettings._instance = null;
		}

		internal static RuntimeSettings Instance
		{
			get
			{
				if (RuntimeSettings._instance == null)
				{
					RuntimeSettings instance;
					OVRRuntimeAssetsBase.LoadAsset<RuntimeSettings>(out instance, RuntimeSettings.InstanceAssetName, null);
					RuntimeSettings._instance = instance;
				}
				return RuntimeSettings._instance;
			}
		}

		internal static event Action OnImmersiveDebuggerEnabledChanged;

		internal bool ImmersiveDebuggerEnabled
		{
			get
			{
				return this.immersiveDebuggerEnabled;
			}
			set
			{
				if (this.immersiveDebuggerEnabled != value)
				{
					this.immersiveDebuggerEnabled = value;
					Action onImmersiveDebuggerEnabledChanged = RuntimeSettings.OnImmersiveDebuggerEnabledChanged;
					if (onImmersiveDebuggerEnabledChanged == null)
					{
						return;
					}
					onImmersiveDebuggerEnabledChanged();
				}
			}
		}

		internal bool ImmersiveDebuggerDisplayAtStartup
		{
			get
			{
				return this.immersiveDebuggerDisplayAtStartup;
			}
			set
			{
				this.immersiveDebuggerDisplayAtStartup = value;
			}
		}

		internal bool EnableOnlyInDebugBuild
		{
			get
			{
				return this.enableOnlyInDebugBuild;
			}
			set
			{
				this.enableOnlyInDebugBuild = value;
			}
		}

		internal bool ShowInspectors
		{
			get
			{
				return this.showInspectors;
			}
			set
			{
				this.showInspectors = value;
			}
		}

		internal bool ShowConsole
		{
			get
			{
				return this.showConsole;
			}
			set
			{
				this.showConsole = value;
			}
		}

		internal bool FollowOverride
		{
			get
			{
				return this.followOverride;
			}
			set
			{
				this.followOverride = value;
			}
		}

		internal bool RotateOverride
		{
			get
			{
				return this.rotateOverride;
			}
			set
			{
				this.rotateOverride = value;
			}
		}

		internal bool ShowInfoLog
		{
			get
			{
				return this.showInfoLog;
			}
			set
			{
				this.showInfoLog = value;
			}
		}

		internal bool ShowWarningLog
		{
			get
			{
				return this.showWarningLog;
			}
			set
			{
				this.showWarningLog = value;
			}
		}

		internal bool ShowErrorLog
		{
			get
			{
				return this.showErrorLog;
			}
			set
			{
				this.showErrorLog = value;
			}
		}

		internal bool CollapsedIdenticalLogEntries
		{
			get
			{
				return this.collapsedIdenticalLogEntries;
			}
			set
			{
				this.collapsedIdenticalLogEntries = value;
			}
		}

		internal int MaximumNumberOfLogEntries
		{
			get
			{
				return this.maximumNumberOfLogEntries;
			}
			set
			{
				this.maximumNumberOfLogEntries = value;
			}
		}

		internal RuntimeSettings.DistanceOption PanelDistance
		{
			get
			{
				return this.panelDistance;
			}
			set
			{
				this.panelDistance = value;
			}
		}

		internal bool CreateEventSystem
		{
			get
			{
				return this.createEventSystem;
			}
			set
			{
				this.createEventSystem = value;
			}
		}

		internal bool AutomaticLayerCullingUpdate
		{
			get
			{
				return this.automaticLayerCullingUpdate;
			}
			set
			{
				this.automaticLayerCullingUpdate = value;
			}
		}

		internal int PanelLayer
		{
			get
			{
				return this.panelLayer;
			}
			set
			{
				this.panelLayer = value;
			}
		}

		internal int MeshRendererLayer
		{
			get
			{
				return this.meshRendererLayer;
			}
			set
			{
				this.meshRendererLayer = value;
			}
		}

		internal int OverlayDepth
		{
			get
			{
				return this.overlayDepth;
			}
			set
			{
				this.overlayDepth = value;
			}
		}

		internal bool UseOverlay
		{
			get
			{
				return this.useOverlay;
			}
			set
			{
				this.useOverlay = value;
			}
		}

		internal bool ShouldUseOverlay
		{
			get
			{
				return this.UseOverlay;
			}
		}

		internal List<bool> InspectedDataEnabled
		{
			get
			{
				return this.inspectedDataEnabled;
			}
			set
			{
				this.inspectedDataEnabled = value;
			}
		}

		internal List<InspectedData> InspectedDataAssets
		{
			get
			{
				return this.inspectedDataAssets;
			}
			set
			{
				this.inspectedDataAssets = value;
			}
		}

		internal bool UseCustomIntegrationConfig
		{
			get
			{
				return this.useCustomIntegrationConfig;
			}
			set
			{
				this.useCustomIntegrationConfig = value;
			}
		}

		internal string CustomIntegrationConfigClassName
		{
			get
			{
				return this.customIntegrationConfigClassName;
			}
			set
			{
				this.customIntegrationConfigClassName = value;
			}
		}

		internal bool HierarchyViewShowsPrivateMembers
		{
			get
			{
				return this.hierarchyViewShowsPrivateMembers;
			}
			set
			{
				this.hierarchyViewShowsPrivateMembers = value;
			}
		}

		internal OVRInput.Button ClickButton
		{
			get
			{
				return this.clickButton;
			}
			set
			{
				this.clickButton = value;
			}
		}

		internal OVRInput.Button ToggleFollowTranslationButton
		{
			get
			{
				return this.toggleFollowTranslationButton;
			}
			set
			{
				this.toggleFollowTranslationButton = value;
			}
		}

		internal OVRInput.Button ToggleFollowRotationButton
		{
			get
			{
				return this.toggleFollowRotationButton;
			}
			set
			{
				this.toggleFollowRotationButton = value;
			}
		}

		internal OVRInput.Button ImmersiveDebuggerToggleDisplayButton
		{
			get
			{
				return this.immersiveDebuggerToggleDisplayButton;
			}
			set
			{
				this.immersiveDebuggerToggleDisplayButton = value;
			}
		}

		internal RuntimeSettings()
		{
			this.debugTypes = new List<DebugData>();
			this.debugTypesDict = new Dictionary<string, List<string>>();
		}

		public void OnBeforeSerialize()
		{
			this.debugTypes.Clear();
			foreach (KeyValuePair<string, List<string>> keyValuePair in this.debugTypesDict)
			{
				string text;
				List<string> list;
				keyValuePair.Deconstruct(out text, out list);
				string assemblyName = text;
				List<string> types = list;
				this.debugTypes.Add(new DebugData(assemblyName, types));
			}
		}

		public void OnAfterDeserialize()
		{
			foreach (DebugData debugData in this.debugTypes)
			{
				this.debugTypesDict[debugData.AssemblyName] = debugData.DebugTypes;
			}
		}

		internal static string InstanceAssetName = "ImmersiveDebuggerSettings";

		private static RuntimeSettings _instance;

		[SerializeField]
		private List<DebugData> debugTypes;

		internal Dictionary<string, List<string>> debugTypesDict;

		[SerializeField]
		private bool immersiveDebuggerEnabled;

		[SerializeField]
		private bool immersiveDebuggerDisplayAtStartup;

		[SerializeField]
		private bool enableOnlyInDebugBuild;

		[SerializeField]
		private bool showInspectors;

		[SerializeField]
		private bool showConsole;

		[SerializeField]
		private bool followOverride = true;

		[SerializeField]
		private bool rotateOverride;

		[SerializeField]
		private bool showInfoLog;

		[SerializeField]
		private bool showWarningLog = true;

		[SerializeField]
		private bool showErrorLog = true;

		[SerializeField]
		private bool collapsedIdenticalLogEntries;

		[SerializeField]
		private int maximumNumberOfLogEntries = 1000;

		[SerializeField]
		private RuntimeSettings.DistanceOption panelDistance = RuntimeSettings.DistanceOption.Default;

		[SerializeField]
		private bool createEventSystem = true;

		[SerializeField]
		private bool automaticLayerCullingUpdate = true;

		[SerializeField]
		private int panelLayer = 20;

		[SerializeField]
		private int meshRendererLayer = 21;

		[SerializeField]
		private int overlayDepth = 10;

		[SerializeField]
		private bool useOverlay = true;

		[SerializeField]
		private List<bool> inspectedDataEnabled = new List<bool>();

		[SerializeField]
		private List<InspectedData> inspectedDataAssets = new List<InspectedData>();

		[SerializeField]
		private bool useCustomIntegrationConfig;

		[SerializeField]
		private string customIntegrationConfigClassName;

		[SerializeField]
		private bool hierarchyViewShowsPrivateMembers;

		[SerializeField]
		private OVRInput.Button clickButton = OVRInput.Button.One | OVRInput.Button.PrimaryIndexTrigger;

		[SerializeField]
		private OVRInput.Button toggleFollowTranslationButton;

		[SerializeField]
		private OVRInput.Button toggleFollowRotationButton;

		[SerializeField]
		private OVRInput.Button immersiveDebuggerToggleDisplayButton = OVRInput.Button.Two;

		public enum DistanceOption
		{
			Close,
			Default,
			Far
		}
	}
}
