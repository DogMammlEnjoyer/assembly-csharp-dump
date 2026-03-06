using System;

namespace UnityEngine.UIElements
{
	[HelpURL("UIE-get-started-with-runtime-ui")]
	[ExecuteAlways]
	[AddComponentMenu("UI Toolkit/Panel Input Configuration", 1)]
	[DisallowMultipleComponent]
	public sealed class PanelInputConfiguration : MonoBehaviour
	{
		internal static PanelInputConfiguration current { get; set; }

		internal PanelInputConfiguration.Settings settings
		{
			get
			{
				return this.m_Settings;
			}
		}

		public bool processWorldSpaceInput
		{
			get
			{
				return this.m_Settings.m_ProcessWorldSpaceInput;
			}
			set
			{
				bool flag = this.m_Settings.m_ProcessWorldSpaceInput == value;
				if (!flag)
				{
					this.m_Settings.m_ProcessWorldSpaceInput = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public LayerMask interactionLayers
		{
			get
			{
				return this.m_Settings.m_InteractionLayers;
			}
			set
			{
				bool flag = this.m_Settings.m_InteractionLayers == value;
				if (!flag)
				{
					this.m_Settings.m_InteractionLayers = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public float maxInteractionDistance
		{
			get
			{
				return this.m_Settings.m_MaxInteractionDistance;
			}
			set
			{
				bool flag = this.m_Settings.m_MaxInteractionDistance == value;
				if (!flag)
				{
					this.m_Settings.m_MaxInteractionDistance = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public bool defaultEventCameraIsMainCamera
		{
			get
			{
				return this.m_Settings.m_DefaultEventCameraIsMainCamera;
			}
			set
			{
				bool flag = this.m_Settings.m_DefaultEventCameraIsMainCamera == value;
				if (!flag)
				{
					this.m_Settings.m_DefaultEventCameraIsMainCamera = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public Camera[] eventCameras
		{
			get
			{
				return this.m_Settings.m_EventCameras;
			}
			set
			{
				bool flag = this.m_Settings.m_EventCameras == value;
				if (!flag)
				{
					this.m_Settings.m_EventCameras = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public PanelInputConfiguration.PanelInputRedirection panelInputRedirection
		{
			get
			{
				return this.m_Settings.m_PanelInputRedirection;
			}
			set
			{
				bool flag = this.m_Settings.m_PanelInputRedirection == value;
				if (!flag)
				{
					this.m_Settings.m_PanelInputRedirection = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		public bool autoCreatePanelComponents
		{
			get
			{
				return this.m_Settings.m_AutoCreatePanelComponents;
			}
			set
			{
				bool flag = this.m_Settings.m_AutoCreatePanelComponents == value;
				if (!flag)
				{
					this.m_Settings.m_AutoCreatePanelComponents = value;
					PanelInputConfiguration.Apply(this);
				}
			}
		}

		private void OnEnable()
		{
			PanelInputConfiguration.s_ActiveInstances++;
			bool flag = PanelInputConfiguration.current != null;
			if (flag)
			{
				bool isPlaying = Application.isPlaying;
				if (isPlaying)
				{
					string[] array = new string[5];
					array[0] = "Multiple Input Configuration components active. Only one will be considered, the rest will be disabled.\nEnabled: ";
					int num = 1;
					PanelInputConfiguration current = PanelInputConfiguration.current;
					array[num] = ((current != null) ? current.ToString() : null);
					array[2] = ". Disabled: ";
					array[3] = ((this != null) ? this.ToString() : null);
					array[4] = ".";
					Debug.LogWarning(string.Concat(array));
					base.enabled = false;
				}
			}
			else
			{
				PanelInputConfiguration.current = this;
				PanelInputConfiguration.Apply(this);
			}
		}

		private void OnDisable()
		{
			PanelInputConfiguration.s_ActiveInstances--;
			bool flag = PanelInputConfiguration.current != this;
			if (!flag)
			{
				PanelInputConfiguration.current = null;
				PanelInputConfiguration.Apply(null);
			}
		}

		private static void Apply(PanelInputConfiguration input)
		{
			PanelInputConfiguration.Settings settings = (input != null) ? input.settings : PanelInputConfiguration.Settings.Default;
			PanelInputConfiguration.PanelInputRedirection panelInputRedirection = settings.panelInputRedirection;
			if (!true)
			{
			}
			bool? overrideUseDefaultEventSystem;
			if (panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Never)
			{
				if (panelInputRedirection != PanelInputConfiguration.PanelInputRedirection.Always)
				{
					overrideUseDefaultEventSystem = null;
				}
				else
				{
					overrideUseDefaultEventSystem = new bool?(false);
				}
			}
			else
			{
				overrideUseDefaultEventSystem = new bool?(true);
			}
			if (!true)
			{
			}
			UIElementsRuntimeUtility.overrideUseDefaultEventSystem = overrideUseDefaultEventSystem;
			UIElementsRuntimeUtility.defaultEventSystem.worldSpaceLayers = settings.interactionLayers;
			UIElementsRuntimeUtility.defaultEventSystem.worldSpaceMaxDistance = settings.maxInteractionDistance;
			DefaultEventSystem defaultEventSystem = UIElementsRuntimeUtility.defaultEventSystem;
			CameraScreenRaycaster raycaster;
			if (!settings.processWorldSpaceInput)
			{
				(raycaster = new CameraScreenRaycaster()).cameras = Array.Empty<Camera>();
			}
			else if (!settings.defaultEventCameraIsMainCamera)
			{
				(raycaster = new CameraScreenRaycaster()).cameras = (Camera[])settings.eventCameras.Clone();
			}
			else
			{
				raycaster = new MainCameraScreenRaycaster();
			}
			defaultEventSystem.raycaster = raycaster;
			Action<PanelInputConfiguration> action = PanelInputConfiguration.onApply;
			if (action != null)
			{
				action(input);
			}
		}

		internal static int s_ActiveInstances;

		internal static Action<PanelInputConfiguration> onApply;

		[SerializeField]
		private PanelInputConfiguration.Settings m_Settings = PanelInputConfiguration.Settings.Default;

		internal const string SettingsProperty = "m_Settings";

		public enum PanelInputRedirection
		{
			[InspectorName("Auto-switch (redirect from EventSystem if present)")]
			AutoSwitch,
			[InspectorName("No input redirection")]
			Never,
			[InspectorName("Always redirect from EventSystem (wait if unavailable)")]
			Always
		}

		[Serializable]
		internal struct Settings
		{
			public static PanelInputConfiguration.Settings Default
			{
				get
				{
					return PanelInputConfiguration.Settings.s_Default;
				}
			}

			public bool processWorldSpaceInput
			{
				get
				{
					return this.m_ProcessWorldSpaceInput;
				}
			}

			public LayerMask interactionLayers
			{
				get
				{
					return this.m_InteractionLayers;
				}
			}

			public float maxInteractionDistance
			{
				get
				{
					return this.m_MaxInteractionDistance;
				}
			}

			public bool defaultEventCameraIsMainCamera
			{
				get
				{
					return this.m_DefaultEventCameraIsMainCamera;
				}
			}

			public Camera[] eventCameras
			{
				get
				{
					return this.m_EventCameras;
				}
			}

			public PanelInputConfiguration.PanelInputRedirection panelInputRedirection
			{
				get
				{
					return this.m_PanelInputRedirection;
				}
			}

			public bool autoCreatePanelComponents
			{
				get
				{
					return this.m_AutoCreatePanelComponents;
				}
			}

			private static PanelInputConfiguration.Settings s_Default = new PanelInputConfiguration.Settings
			{
				m_ProcessWorldSpaceInput = true,
				m_InteractionLayers = -5,
				m_MaxInteractionDistance = float.PositiveInfinity,
				m_DefaultEventCameraIsMainCamera = true,
				m_EventCameras = Array.Empty<Camera>(),
				m_PanelInputRedirection = PanelInputConfiguration.PanelInputRedirection.AutoSwitch,
				m_AutoCreatePanelComponents = true
			};

			[Tooltip("Determines whether world space panels process input events. Disable this if you need UGUI support but do not require world space input to improve performance.")]
			[SerializeField]
			internal bool m_ProcessWorldSpaceInput;

			[Tooltip("Determines which layers can block input events on world space panels.")]
			[SerializeField]
			internal LayerMask m_InteractionLayers;

			[Tooltip("Sets how far away interactions with world-space UI are possible. Defaults to unlimited (infinity), but you can customize it for XR or performance needs. The distance uses GameObject units, consistent with transform positions and Camera clipping planes.")]
			[SerializeField]
			internal float m_MaxInteractionDistance;

			[SerializeField]
			[Tooltip("Defines whether the Main Camera is used as the Event Camera for world space panels. Disable to specify alternative Event Camera(s) for raycasting input.")]
			internal bool m_DefaultEventCameraIsMainCamera;

			[SerializeField]
			[Tooltip("Defines the Event Camera(s) used for world space raycasting input.")]
			internal Camera[] m_EventCameras;

			[Tooltip("Determines which input event system is used for UI interactions when combining UI Toolkit and UGUI.")]
			[SerializeField]
			internal PanelInputConfiguration.PanelInputRedirection m_PanelInputRedirection;

			[Tooltip("Automatically adds UI Toolkit components under the EventSystem to handle input redirection between UI Toolkit and UGUI panels. Disable to manually assign these components through code.")]
			[SerializeField]
			internal bool m_AutoCreatePanelComponents;
		}
	}
}
