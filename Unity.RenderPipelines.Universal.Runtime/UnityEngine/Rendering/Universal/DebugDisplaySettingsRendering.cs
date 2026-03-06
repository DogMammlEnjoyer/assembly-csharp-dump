using System;

namespace UnityEngine.Rendering.Universal
{
	public class DebugDisplaySettingsRendering : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		public DebugWireframeMode wireframeMode
		{
			get
			{
				return this.m_WireframeMode;
			}
			set
			{
				this.m_WireframeMode = value;
				this.UpdateDebugSceneOverrideMode();
			}
		}

		[Obsolete("overdraw has been deprecated. Use overdrawMode instead.", true)]
		public bool overdraw
		{
			get
			{
				return this.m_Overdraw;
			}
			set
			{
				this.m_Overdraw = value;
				this.UpdateDebugSceneOverrideMode();
			}
		}

		public DebugOverdrawMode overdrawMode
		{
			get
			{
				return this.m_OverdrawMode;
			}
			set
			{
				this.m_OverdrawMode = value;
				this.UpdateDebugSceneOverrideMode();
			}
		}

		public int maxOverdrawCount { get; set; } = 10;

		private void UpdateDebugSceneOverrideMode()
		{
			switch (this.wireframeMode)
			{
			case DebugWireframeMode.Wireframe:
				this.sceneOverrideMode = DebugSceneOverrideMode.Wireframe;
				return;
			case DebugWireframeMode.SolidWireframe:
				this.sceneOverrideMode = DebugSceneOverrideMode.SolidWireframe;
				return;
			case DebugWireframeMode.ShadedWireframe:
				this.sceneOverrideMode = DebugSceneOverrideMode.ShadedWireframe;
				return;
			default:
				this.sceneOverrideMode = ((this.overdrawMode != DebugOverdrawMode.None) ? DebugSceneOverrideMode.Overdraw : DebugSceneOverrideMode.None);
				return;
			}
		}

		public DebugFullScreenMode fullScreenDebugMode { get; set; }

		internal int stpDebugViewIndex { get; set; }

		public int fullScreenDebugModeOutputSizeScreenPercent { get; set; } = 50;

		internal DebugSceneOverrideMode sceneOverrideMode { get; set; }

		public DebugMipInfoMode mipInfoMode { get; set; }

		public bool mipDebugStatusShowCode { get; set; }

		public DebugMipMapStatusMode mipDebugStatusMode { get; set; }

		public float mipDebugOpacity { get; set; } = 1f;

		public float mipDebugRecentUpdateCooldown { get; set; } = 3f;

		public int mipDebugMaterialTextureSlot { get; set; }

		public bool showInfoForAllSlots { get; set; } = true;

		internal bool canAggregateData
		{
			get
			{
				return this.mipInfoMode == DebugMipInfoMode.MipStreamingStatus || this.mipInfoMode == DebugMipInfoMode.MipStreamingActivity;
			}
		}

		public DebugMipMapModeTerrainTexture mipDebugTerrainTexture { get; set; }

		public DebugPostProcessingMode postProcessingDebugMode { get; set; } = DebugPostProcessingMode.Auto;

		public bool enableMsaa { get; set; } = true;

		public bool enableHDR { get; set; } = true;

		public DebugDisplaySettingsRendering.TaaDebugMode taaDebugMode { get; set; }

		public DebugValidationMode validationMode { get; set; }

		public PixelValidationChannels validationChannels { get; set; }

		public float validationRangeMin { get; set; }

		public float validationRangeMax { get; set; } = 1f;

		public bool AreAnySettingsActive
		{
			get
			{
				return this.postProcessingDebugMode != DebugPostProcessingMode.Auto || this.fullScreenDebugMode != DebugFullScreenMode.None || this.sceneOverrideMode != DebugSceneOverrideMode.None || this.mipInfoMode != DebugMipInfoMode.None || this.validationMode != DebugValidationMode.None || !this.enableMsaa || !this.enableHDR || this.taaDebugMode > DebugDisplaySettingsRendering.TaaDebugMode.None;
			}
		}

		public bool IsPostProcessingAllowed
		{
			get
			{
				return this.postProcessingDebugMode != DebugPostProcessingMode.Disabled && this.sceneOverrideMode == DebugSceneOverrideMode.None && this.mipInfoMode == DebugMipInfoMode.None;
			}
		}

		public bool IsLightingActive
		{
			get
			{
				return this.sceneOverrideMode == DebugSceneOverrideMode.None && this.mipInfoMode == DebugMipInfoMode.None;
			}
		}

		public bool TryGetScreenClearColor(ref Color color)
		{
			if (this.mipInfoMode != DebugMipInfoMode.None)
			{
				color = Color.black;
				return true;
			}
			switch (this.sceneOverrideMode)
			{
			case DebugSceneOverrideMode.None:
			case DebugSceneOverrideMode.ShadedWireframe:
				return false;
			case DebugSceneOverrideMode.Overdraw:
				color = Color.black;
				return true;
			case DebugSceneOverrideMode.Wireframe:
			case DebugSceneOverrideMode.SolidWireframe:
				color = new Color(0.1f, 0.1f, 0.1f, 1f);
				return true;
			default:
				throw new ArgumentOutOfRangeException("color");
			}
		}

		IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
		{
			return new DebugDisplaySettingsRendering.SettingsPanel(this);
		}

		private DebugWireframeMode m_WireframeMode;

		private bool m_Overdraw;

		private DebugOverdrawMode m_OverdrawMode;

		public enum TaaDebugMode
		{
			None,
			ShowRawFrame,
			ShowRawFrameNoJitter,
			ShowClampedHistory
		}

		private static class Strings
		{
			public const string RangeValidationSettingsContainerName = "Pixel Range Settings";

			public static readonly DebugUI.Widget.NameAndTooltip MapOverlays = new DebugUI.Widget.NameAndTooltip
			{
				name = "Map Overlays",
				tooltip = "Overlays render pipeline textures to validate the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip StpDebugViews = new DebugUI.Widget.NameAndTooltip
			{
				name = "STP Debug Views",
				tooltip = "Debug visualizations provided by STP."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MapSize = new DebugUI.Widget.NameAndTooltip
			{
				name = "Map Size",
				tooltip = "Set the size of the render pipeline texture in the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AdditionalWireframeModes = new DebugUI.Widget.NameAndTooltip
			{
				name = "Additional Wireframe Modes",
				tooltip = "Debug the scene with additional wireframe shader views that are different from those in the scene view."
			};

			public static readonly DebugUI.Widget.NameAndTooltip WireframeNotSupportedWarning = new DebugUI.Widget.NameAndTooltip
			{
				name = "Warning: This platform might not support wireframe rendering.",
				tooltip = "Some platforms, for example, mobile platforms using OpenGL ES and Vulkan, might not support wireframe rendering."
			};

			public static readonly DebugUI.Widget.NameAndTooltip OverdrawMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Overdraw Mode",
				tooltip = "Debug anywhere materials that overdrawn pixels top of each other."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaxOverdrawCount = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Overdraw Count",
				tooltip = "Maximum overdraw count allowed for a single pixel."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapDisableMipCaching = new DebugUI.Widget.NameAndTooltip
			{
				name = "Disable Mip Caching",
				tooltip = "By disabling mip caching, the data on GPU accurately reflects what the TextureStreamer calculates. While this can significantly increase CPU-to-GPU traffic, it can be an invaluable tool to validate that the Streamer behaves as expected."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapDebugView = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug View",
				tooltip = "Use the drop-down to select a mipmap property to debug."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapDebugOpacity = new DebugUI.Widget.NameAndTooltip
			{
				name = "Debug Opacity",
				tooltip = "Opacity of texture mipmap streaming debug colors."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapMaterialTextureSlot = new DebugUI.Widget.NameAndTooltip
			{
				name = "Material Texture Slot",
				tooltip = "Use the drop-down to select the material texture slot to debug (does not affect terrain).\n\nThe slot indices follow the default order by which texture properties appear in the Material Inspector.\nThe default order is itself defined by the order in which (non-hidden) texture properties appear in the shader's \"Properties\" block."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapTerrainTexture = new DebugUI.Widget.NameAndTooltip
			{
				name = "Terrain Texture",
				tooltip = "Use the drop-down to select the terrain Texture to debug the mipmap for."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapDisplayStatusCodes = new DebugUI.Widget.NameAndTooltip
			{
				name = "Display Status Codes",
				tooltip = "Show detailed status codes indicating why textures are not streaming or highlighting points of attention."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapActivityTimespan = new DebugUI.Widget.NameAndTooltip
			{
				name = "Activity Timespan",
				tooltip = "How long a texture should be shown as \"recently updated\"."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MipMapCombinePerMaterial = new DebugUI.Widget.NameAndTooltip
			{
				name = "Combined per Material",
				tooltip = "Combine the information over all slots per material."
			};

			public static readonly DebugUI.Widget.NameAndTooltip PostProcessing = new DebugUI.Widget.NameAndTooltip
			{
				name = "Post-processing",
				tooltip = "Override the controls for Post Processing in the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MSAA = new DebugUI.Widget.NameAndTooltip
			{
				name = "MSAA",
				tooltip = "Use the checkbox to disable MSAA in the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip HDR = new DebugUI.Widget.NameAndTooltip
			{
				name = "HDR",
				tooltip = "Use the checkbox to disable High Dynamic Range in the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip TaaDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "TAA Debug Mode",
				tooltip = "Choose whether to force TAA to output the raw jittered frame or clamped reprojected history."
			};

			public static readonly DebugUI.Widget.NameAndTooltip PixelValidationMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Pixel Validation Mode",
				tooltip = "Choose between modes that validate pixel on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip Channels = new DebugUI.Widget.NameAndTooltip
			{
				name = "Channels",
				tooltip = "Choose the texture channel used to validate the scene."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValueRangeMin = new DebugUI.Widget.NameAndTooltip
			{
				name = "Value Range Min",
				tooltip = "Any values set below this field will be considered invalid and will appear red on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValueRangeMax = new DebugUI.Widget.NameAndTooltip
			{
				name = "Value Range Max",
				tooltip = "Any values set above this field will be considered invalid and will appear blue on screen."
			};
		}

		internal static class WidgetFactory
		{
			internal static DebugUI.Widget CreateMapOverlays(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MapOverlays,
					autoEnum = typeof(DebugFullScreenMode),
					getter = (() => (int)panel.data.fullScreenDebugMode),
					setter = delegate(int value)
					{
						panel.data.fullScreenDebugMode = (DebugFullScreenMode)value;
					},
					getIndex = (() => (int)panel.data.fullScreenDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.fullScreenDebugMode = (DebugFullScreenMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateStpDebugViews(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.StpDebugViews,
					isHiddenCallback = (() => panel.data.fullScreenDebugMode != DebugFullScreenMode.STP),
					enumNames = STP.debugViewDescriptions,
					enumValues = STP.debugViewIndices,
					getter = (() => panel.data.stpDebugViewIndex),
					setter = delegate(int value)
					{
						panel.data.stpDebugViewIndex = value;
					},
					getIndex = (() => panel.data.stpDebugViewIndex),
					setIndex = delegate(int value)
					{
						panel.data.stpDebugViewIndex = value;
					}
				};
			}

			internal static DebugUI.Widget CreateMapOverlaySize(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.Container container = new DebugUI.Container();
				ObservableList<DebugUI.Widget> children = container.children;
				DebugUI.IntField intField = new DebugUI.IntField();
				intField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.MapSize;
				intField.getter = (() => panel.data.fullScreenDebugModeOutputSizeScreenPercent);
				intField.setter = delegate(int value)
				{
					panel.data.fullScreenDebugModeOutputSizeScreenPercent = value;
				};
				intField.incStep = 10;
				intField.min = (() => 0);
				intField.max = (() => 100);
				children.Add(intField);
				return container;
			}

			internal static DebugUI.Widget CreateAdditionalWireframeShaderViews(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.EnumField enumField = new DebugUI.EnumField();
				enumField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.AdditionalWireframeModes;
				enumField.autoEnum = typeof(DebugWireframeMode);
				enumField.getter = (() => (int)panel.data.wireframeMode);
				enumField.setter = delegate(int value)
				{
					panel.data.wireframeMode = (DebugWireframeMode)value;
				};
				enumField.getIndex = (() => (int)panel.data.wireframeMode);
				enumField.setIndex = delegate(int value)
				{
					panel.data.wireframeMode = (DebugWireframeMode)value;
				};
				enumField.onValueChanged = delegate(DebugUI.Field<int> _, int _)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				};
				return enumField;
			}

			internal static DebugUI.Widget CreateWireframeNotSupportedWarning(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.MessageBox
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.WireframeNotSupportedWarning,
					style = DebugUI.MessageBox.Style.Warning,
					isHiddenCallback = delegate()
					{
						GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
						return (graphicsDeviceType != GraphicsDeviceType.OpenGLES3 && graphicsDeviceType != GraphicsDeviceType.Vulkan) || panel.data.wireframeMode == DebugWireframeMode.None;
					}
				};
			}

			internal static DebugUI.Widget CreateOverdrawMode(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.OverdrawMode,
					autoEnum = typeof(DebugOverdrawMode),
					getter = (() => (int)panel.data.overdrawMode),
					setter = delegate(int value)
					{
						panel.data.overdrawMode = (DebugOverdrawMode)value;
					},
					getIndex = (() => (int)panel.data.overdrawMode),
					setIndex = delegate(int value)
					{
						panel.data.overdrawMode = (DebugOverdrawMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateMaxOverdrawCount(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.Container container = new DebugUI.Container();
				container.isHiddenCallback = (() => panel.data.overdrawMode == DebugOverdrawMode.None);
				ObservableList<DebugUI.Widget> children = container.children;
				DebugUI.IntField intField = new DebugUI.IntField();
				intField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.MaxOverdrawCount;
				intField.getter = (() => panel.data.maxOverdrawCount);
				intField.setter = delegate(int value)
				{
					panel.data.maxOverdrawCount = value;
				};
				intField.incStep = 10;
				intField.min = (() => 1);
				intField.max = (() => 500);
				children.Add(intField);
				return container;
			}

			internal static DebugUI.Widget CreateMipMapDebugWidget(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.Container container = new DebugUI.Container();
				container.displayName = "Mipmap Streaming";
				ObservableList<DebugUI.Widget> children = container.children;
				DebugUI.BoolField boolField = new DebugUI.BoolField();
				boolField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapDisableMipCaching;
				boolField.getter = (() => Texture.streamingTextureDiscardUnusedMips);
				boolField.setter = delegate(bool value)
				{
					Texture.streamingTextureDiscardUnusedMips = value;
				};
				children.Add(boolField);
				container.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapMode(panel));
				container.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapDebugSettings(panel));
				return container;
			}

			internal static DebugUI.Widget CreateMipMapMode(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapDebugView,
					autoEnum = typeof(DebugMipInfoMode),
					getter = (() => (int)panel.data.mipInfoMode),
					setter = delegate(int value)
					{
						panel.data.mipInfoMode = (DebugMipInfoMode)value;
					},
					getIndex = (() => (int)panel.data.mipInfoMode),
					setIndex = delegate(int value)
					{
						panel.data.mipInfoMode = (DebugMipInfoMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateMipMapDebugSettings(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				GUIContent[] array = new GUIContent[64];
				int[] array2 = new int[64];
				for (int i = 0; i < 64; i++)
				{
					array[i] = new GUIContent(string.Format("Slot {0}", i));
					array2[i] = i;
				}
				DebugUI.Container container = new DebugUI.Container();
				container.isHiddenCallback = (() => panel.data.mipInfoMode == DebugMipInfoMode.None);
				ObservableList<DebugUI.Widget> children = container.children;
				DebugUI.FloatField floatField = new DebugUI.FloatField();
				floatField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapDebugOpacity;
				floatField.getter = (() => panel.data.mipDebugOpacity);
				floatField.setter = delegate(float value)
				{
					panel.data.mipDebugOpacity = value;
				};
				floatField.min = (() => 0f);
				floatField.max = (() => 1f);
				children.Add(floatField);
				container.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapDebugSlotSelector(panel, () => panel.data.canAggregateData, array, array2));
				container.children.Add(new DebugUI.BoolField
				{
					isHiddenCallback = (() => !panel.data.canAggregateData),
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapCombinePerMaterial,
					getter = (() => panel.data.showInfoForAllSlots),
					setter = delegate(bool value)
					{
						panel.data.showInfoForAllSlots = value;
						panel.data.mipDebugStatusMode = (value ? DebugMipMapStatusMode.Material : DebugMipMapStatusMode.Texture);
					}
				});
				ObservableList<DebugUI.Widget> children2 = container.children;
				DebugUI.Container container2 = new DebugUI.Container();
				container2.isHiddenCallback = (() => !panel.data.canAggregateData || panel.data.showInfoForAllSlots);
				container2.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapDebugSlotSelector(panel, () => false, array, array2));
				container2.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapShowStatusCodeToggle(panel));
				children2.Add(container2);
				container.children.Add(new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapTerrainTexture,
					getter = (() => (int)panel.data.mipDebugTerrainTexture),
					setter = delegate(int value)
					{
						panel.data.mipDebugTerrainTexture = (DebugMipMapModeTerrainTexture)value;
					},
					autoEnum = typeof(DebugMipMapModeTerrainTexture),
					getIndex = (() => (int)panel.data.mipDebugTerrainTexture),
					setIndex = delegate(int value)
					{
						panel.data.mipDebugTerrainTexture = (DebugMipMapModeTerrainTexture)value;
					}
				});
				container.children.Add(DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapDebugCooldownSlider(panel));
				return container;
			}

			internal static DebugUI.Widget CreateMipMapDebugSlotSelector(DebugDisplaySettingsRendering.SettingsPanel panel, Func<bool> hiddenCB, GUIContent[] texSlotStrings, int[] texSlotValues)
			{
				return new DebugUI.EnumField
				{
					isHiddenCallback = hiddenCB,
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapMaterialTextureSlot,
					getter = (() => panel.data.mipDebugMaterialTextureSlot),
					setter = delegate(int value)
					{
						panel.data.mipDebugMaterialTextureSlot = value;
					},
					getIndex = (() => panel.data.mipDebugMaterialTextureSlot),
					setIndex = delegate(int value)
					{
						panel.data.mipDebugMaterialTextureSlot = value;
					},
					enumNames = texSlotStrings,
					enumValues = texSlotValues
				};
			}

			internal static DebugUI.Widget CreateMipMapDebugCooldownSlider(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.FloatField floatField = new DebugUI.FloatField();
				floatField.isHiddenCallback = (() => panel.data.mipInfoMode != DebugMipInfoMode.MipStreamingActivity);
				floatField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapActivityTimespan;
				floatField.getter = (() => panel.data.mipDebugRecentUpdateCooldown);
				floatField.setter = delegate(float value)
				{
					panel.data.mipDebugRecentUpdateCooldown = value;
				};
				floatField.min = (() => 0f);
				floatField.max = (() => 60f);
				return floatField;
			}

			internal static DebugUI.Widget CreateMipMapShowStatusCodeToggle(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.BoolField
				{
					isHiddenCallback = (() => panel.data.mipInfoMode != DebugMipInfoMode.MipStreamingStatus),
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MipMapDisplayStatusCodes,
					getter = (() => panel.data.mipDebugStatusShowCode),
					setter = delegate(bool value)
					{
						panel.data.mipDebugStatusShowCode = value;
					}
				};
			}

			internal static DebugUI.Widget CreatePostProcessing(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.PostProcessing,
					autoEnum = typeof(DebugPostProcessingMode),
					getter = (() => (int)panel.data.postProcessingDebugMode),
					setter = delegate(int value)
					{
						panel.data.postProcessingDebugMode = (DebugPostProcessingMode)value;
					},
					getIndex = (() => (int)panel.data.postProcessingDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.postProcessingDebugMode = (DebugPostProcessingMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateMSAA(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.MSAA,
					getter = (() => panel.data.enableMsaa),
					setter = delegate(bool value)
					{
						panel.data.enableMsaa = value;
					}
				};
			}

			internal static DebugUI.Widget CreateHDR(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.HDR,
					getter = (() => panel.data.enableHDR),
					setter = delegate(bool value)
					{
						panel.data.enableHDR = value;
					}
				};
			}

			internal static DebugUI.Widget CreateTaaDebugMode(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.EnumField enumField = new DebugUI.EnumField();
				enumField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.TaaDebugMode;
				enumField.autoEnum = typeof(DebugDisplaySettingsRendering.TaaDebugMode);
				enumField.getter = (() => (int)panel.data.taaDebugMode);
				enumField.setter = delegate(int value)
				{
					panel.data.taaDebugMode = (DebugDisplaySettingsRendering.TaaDebugMode)value;
				};
				enumField.getIndex = (() => (int)panel.data.taaDebugMode);
				enumField.setIndex = delegate(int value)
				{
					panel.data.taaDebugMode = (DebugDisplaySettingsRendering.TaaDebugMode)value;
				};
				enumField.onValueChanged = delegate(DebugUI.Field<int> _, int _)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				};
				return enumField;
			}

			internal static DebugUI.Widget CreatePixelValidationMode(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				DebugUI.EnumField enumField = new DebugUI.EnumField();
				enumField.nameAndTooltip = DebugDisplaySettingsRendering.Strings.PixelValidationMode;
				enumField.autoEnum = typeof(DebugValidationMode);
				enumField.getter = (() => (int)panel.data.validationMode);
				enumField.setter = delegate(int value)
				{
					panel.data.validationMode = (DebugValidationMode)value;
				};
				enumField.getIndex = (() => (int)panel.data.validationMode);
				enumField.setIndex = delegate(int value)
				{
					panel.data.validationMode = (DebugValidationMode)value;
				};
				enumField.onValueChanged = delegate(DebugUI.Field<int> _, int _)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				};
				return enumField;
			}

			internal static DebugUI.Widget CreatePixelValidationChannels(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.Channels,
					autoEnum = typeof(PixelValidationChannels),
					getter = (() => (int)panel.data.validationChannels),
					setter = delegate(int value)
					{
						panel.data.validationChannels = (PixelValidationChannels)value;
					},
					getIndex = (() => (int)panel.data.validationChannels),
					setIndex = delegate(int value)
					{
						panel.data.validationChannels = (PixelValidationChannels)value;
					}
				};
			}

			internal static DebugUI.Widget CreatePixelValueRangeMin(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.ValueRangeMin,
					getter = (() => panel.data.validationRangeMin),
					setter = delegate(float value)
					{
						panel.data.validationRangeMin = value;
					},
					incStep = 0.01f
				};
			}

			internal static DebugUI.Widget CreatePixelValueRangeMax(DebugDisplaySettingsRendering.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsRendering.Strings.ValueRangeMax,
					getter = (() => panel.data.validationRangeMax),
					setter = delegate(float value)
					{
						panel.data.validationRangeMax = value;
					},
					incStep = 0.01f
				};
			}
		}

		[DisplayInfo(name = "Rendering", order = 1)]
		internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsRendering>
		{
			public SettingsPanel(DebugDisplaySettingsRendering data) : base(data)
			{
				base.AddWidget(new DebugUI.RuntimeDebugShadersMessageBox());
				base.AddWidget(new DebugUI.Foldout
				{
					displayName = "Rendering Debug",
					flags = DebugUI.Flags.FrequentlyUsed,
					opened = true,
					children = 
					{
						DebugDisplaySettingsRendering.WidgetFactory.CreateMapOverlays(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateStpDebugViews(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateMapOverlaySize(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateHDR(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateMSAA(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateTaaDebugMode(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreatePostProcessing(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateAdditionalWireframeShaderViews(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateWireframeNotSupportedWarning(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateOverdrawMode(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateMaxOverdrawCount(this),
						DebugDisplaySettingsRendering.WidgetFactory.CreateMipMapDebugWidget(this)
					}
				});
				base.AddWidget(new DebugUI.Foldout
				{
					displayName = "Pixel Validation",
					opened = true,
					children = 
					{
						DebugDisplaySettingsRendering.WidgetFactory.CreatePixelValidationMode(this),
						new DebugUI.Container
						{
							displayName = "Pixel Range Settings",
							isHiddenCallback = (() => data.validationMode != DebugValidationMode.HighlightOutsideOfRange),
							children = 
							{
								DebugDisplaySettingsRendering.WidgetFactory.CreatePixelValidationChannels(this),
								DebugDisplaySettingsRendering.WidgetFactory.CreatePixelValueRangeMin(this),
								DebugDisplaySettingsRendering.WidgetFactory.CreatePixelValueRangeMax(this)
							}
						}
					}
				});
				base.AddWidget(new DebugUI.Foldout
				{
					displayName = "HDR Output",
					opened = true,
					children = 
					{
						new DebugUI.MessageBox
						{
							displayName = "The values on the Rendering Debugger editor window might not be accurate. Please use the playmode debug UI (Ctrl+Backspace).",
							style = DebugUI.MessageBox.Style.Warning
						},
						DebugDisplaySettingsHDROutput.CreateHDROuputDisplayTable()
					}
				});
			}
		}
	}
}
