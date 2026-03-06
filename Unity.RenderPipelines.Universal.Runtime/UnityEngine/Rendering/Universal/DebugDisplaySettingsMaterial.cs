using System;

namespace UnityEngine.Rendering.Universal
{
	public class DebugDisplaySettingsMaterial : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		public DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset albedoValidationPreset
		{
			get
			{
				return this.m_AlbedoValidationPreset;
			}
			set
			{
				this.m_AlbedoValidationPreset = value;
				DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData albedoDebugValidationPresetData = this.m_AlbedoDebugValidationPresetData[(int)value];
				this.albedoMinLuminance = albedoDebugValidationPresetData.minLuminance;
				this.albedoMaxLuminance = albedoDebugValidationPresetData.maxLuminance;
				this.albedoCompareColor = albedoDebugValidationPresetData.color;
			}
		}

		public float albedoMinLuminance { get; set; } = 0.01f;

		public float albedoMaxLuminance { get; set; } = 0.9f;

		public float albedoHueTolerance
		{
			get
			{
				if (this.m_AlbedoValidationPreset != DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset.DefaultLuminance)
				{
					return this.m_AlbedoHueTolerance;
				}
				return 1f;
			}
			set
			{
				this.m_AlbedoHueTolerance = value;
			}
		}

		public float albedoSaturationTolerance
		{
			get
			{
				if (this.m_AlbedoValidationPreset != DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset.DefaultLuminance)
				{
					return this.m_AlbedoSaturationTolerance;
				}
				return 1f;
			}
			set
			{
				this.m_AlbedoSaturationTolerance = value;
			}
		}

		public Color albedoCompareColor { get; set; } = new Color(0.49803922f, 0.49803922f, 0.49803922f, 1f);

		public float metallicMinValue { get; set; }

		public float metallicMaxValue { get; set; } = 0.9f;

		public bool renderingLayersSelectedLight { get; set; }

		public bool selectedLightShadowLayerMask { get; set; }

		public uint renderingLayerMask { get; set; }

		public uint GetDebugLightLayersMask()
		{
			return 65535U;
		}

		public DebugMaterialValidationMode materialValidationMode { get; set; }

		public DebugMaterialMode materialDebugMode { get; set; }

		public DebugVertexAttributeMode vertexAttributeDebugMode { get; set; }

		public bool AreAnySettingsActive
		{
			get
			{
				return this.materialDebugMode != DebugMaterialMode.None || this.vertexAttributeDebugMode != DebugVertexAttributeMode.None || this.materialValidationMode > DebugMaterialValidationMode.None;
			}
		}

		public bool IsPostProcessingAllowed
		{
			get
			{
				return !this.AreAnySettingsActive;
			}
		}

		public bool IsLightingActive
		{
			get
			{
				return !this.AreAnySettingsActive;
			}
		}

		IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
		{
			return new DebugDisplaySettingsMaterial.SettingsPanel(this);
		}

		private DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData[] m_AlbedoDebugValidationPresetData = new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData[]
		{
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Default Luminance",
				color = new Color(0.49803922f, 0.49803922f, 0.49803922f),
				minLuminance = 0.01f,
				maxLuminance = 0.9f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Black Acrylic Paint",
				color = new Color(0.21960784f, 0.21960784f, 0.21960784f),
				minLuminance = 0.03f,
				maxLuminance = 0.07f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Dark Soil",
				color = new Color(0.33333334f, 0.23921569f, 0.19215687f),
				minLuminance = 0.05f,
				maxLuminance = 0.14f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Worn Asphalt",
				color = new Color(0.35686275f, 0.35686275f, 0.35686275f),
				minLuminance = 0.1f,
				maxLuminance = 0.15f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Dry Clay Soil",
				color = new Color(0.5372549f, 0.47058824f, 0.4f),
				minLuminance = 0.15f,
				maxLuminance = 0.35f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Green Grass",
				color = new Color(0.48235294f, 0.5137255f, 0.2901961f),
				minLuminance = 0.16f,
				maxLuminance = 0.26f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Old Concrete",
				color = new Color(0.5294118f, 0.53333336f, 0.5137255f),
				minLuminance = 0.17f,
				maxLuminance = 0.3f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Red Clay Tile",
				color = new Color(0.77254903f, 0.49019608f, 0.39215687f),
				minLuminance = 0.23f,
				maxLuminance = 0.33f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Dry Sand",
				color = new Color(0.69411767f, 0.654902f, 0.5176471f),
				minLuminance = 0.2f,
				maxLuminance = 0.45f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "New Concrete",
				color = new Color(0.7254902f, 0.7137255f, 0.6862745f),
				minLuminance = 0.32f,
				maxLuminance = 0.55f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "White Acrylic Paint",
				color = new Color(0.8901961f, 0.8901961f, 0.8901961f),
				minLuminance = 0.75f,
				maxLuminance = 0.85f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Fresh Snow",
				color = new Color(0.9529412f, 0.9529412f, 0.9529412f),
				minLuminance = 0.85f,
				maxLuminance = 0.95f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Blue Sky",
				color = new Color(0.3647059f, 0.48235294f, 0.6156863f),
				minLuminance = new Color(0.3647059f, 0.48235294f, 0.6156863f).linear.maxColorComponent - 0.05f,
				maxLuminance = new Color(0.3647059f, 0.48235294f, 0.6156863f).linear.maxColorComponent + 0.05f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Foliage",
				color = new Color(0.35686275f, 0.42352942f, 0.25490198f),
				minLuminance = new Color(0.35686275f, 0.42352942f, 0.25490198f).linear.maxColorComponent - 0.05f,
				maxLuminance = new Color(0.35686275f, 0.42352942f, 0.25490198f).linear.maxColorComponent + 0.05f
			},
			new DebugDisplaySettingsMaterial.AlbedoDebugValidationPresetData
			{
				name = "Custom",
				color = new Color(0.49803922f, 0.49803922f, 0.49803922f),
				minLuminance = 0.01f,
				maxLuminance = 0.9f
			}
		};

		private DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset m_AlbedoValidationPreset;

		private float m_AlbedoHueTolerance = 0.104f;

		private float m_AlbedoSaturationTolerance = 0.214f;

		public Vector4[] debugRenderingLayersColors = new Vector4[]
		{
			new Vector4(230f, 159f, 0f) / 255f,
			new Vector4(86f, 180f, 233f) / 255f,
			new Vector4(255f, 182f, 291f) / 255f,
			new Vector4(0f, 158f, 115f) / 255f,
			new Vector4(240f, 228f, 66f) / 255f,
			new Vector4(0f, 114f, 178f) / 255f,
			new Vector4(213f, 94f, 0f) / 255f,
			new Vector4(170f, 68f, 170f) / 255f,
			new Vector4(1f, 0.5f, 0.5f),
			new Vector4(0.5f, 1f, 0.5f),
			new Vector4(0.5f, 0.5f, 1f),
			new Vector4(0.5f, 1f, 1f),
			new Vector4(0.75f, 0.25f, 1f),
			new Vector4(0.25f, 1f, 0.75f),
			new Vector4(0.25f, 0.25f, 0.75f),
			new Vector4(0.75f, 0.25f, 0.25f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f),
			new Vector4(0f, 0f, 0f)
		};

		public enum AlbedoDebugValidationPreset
		{
			DefaultLuminance,
			BlackAcrylicPaint,
			DarkSoil,
			WornAsphalt,
			DryClaySoil,
			GreenGrass,
			OldConcrete,
			RedClayTile,
			DrySand,
			NewConcrete,
			WhiteAcrylicPaint,
			FreshSnow,
			BlueSky,
			Foliage,
			Custom
		}

		private struct AlbedoDebugValidationPresetData
		{
			public string name;

			public Color color;

			public float minLuminance;

			public float maxLuminance;
		}

		private static class Strings
		{
			public const string AlbedoSettingsContainerName = "Albedo Settings";

			public const string MetallicSettingsContainerName = "Metallic Settings";

			public const string RenderingLayerMasksSettingsContainerName = "Rendering Layer Masks Settings";

			public static readonly DebugUI.Widget.NameAndTooltip MaterialOverride = new DebugUI.Widget.NameAndTooltip
			{
				name = "Material Override",
				tooltip = "Use the drop-down to select a Material property to visualize on every GameObject on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip VertexAttribute = new DebugUI.Widget.NameAndTooltip
			{
				name = "Vertex Attribute",
				tooltip = "Use the drop-down to select a 3D GameObject attribute, like Texture Coordinates or Vertex Color, to visualize on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MaterialValidationMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Material Validation Mode",
				tooltip = "Debug and validate material properties."
			};

			public static readonly DebugUI.Widget.NameAndTooltip RenderingLayersSelectedLight = new DebugUI.Widget.NameAndTooltip
			{
				name = "Filter Rendering Layers by Light",
				tooltip = "Highlight Renderers affected by Selected Light"
			};

			public static readonly DebugUI.Widget.NameAndTooltip SelectedLightShadowLayerMask = new DebugUI.Widget.NameAndTooltip
			{
				name = "Use Light's Shadow Layer Mask",
				tooltip = "Highlight Renderers that cast shadows for the Selected Light"
			};

			public static readonly DebugUI.Widget.NameAndTooltip FilterRenderingLayerMask = new DebugUI.Widget.NameAndTooltip
			{
				name = "Filter Layers",
				tooltip = "Use the dropdown to filter Rendering Layers that you want to visualize"
			};

			public static readonly DebugUI.Widget.NameAndTooltip ValidationPreset = new DebugUI.Widget.NameAndTooltip
			{
				name = "Validation Preset",
				tooltip = "Validate using a list of preset surfaces and inputs based on real-world surfaces."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AlbedoCustomColor = new DebugUI.Widget.NameAndTooltip
			{
				name = "Target Color",
				tooltip = "Custom target color for albedo validation."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AlbedoMinLuminance = new DebugUI.Widget.NameAndTooltip
			{
				name = "Min Luminance",
				tooltip = "Any values set below this field are invalid and appear red on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AlbedoMaxLuminance = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Luminance",
				tooltip = "Any values set above this field are invalid and appear blue on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AlbedoHueTolerance = new DebugUI.Widget.NameAndTooltip
			{
				name = "Hue Tolerance",
				tooltip = "Validate a material based on a specific hue."
			};

			public static readonly DebugUI.Widget.NameAndTooltip AlbedoSaturationTolerance = new DebugUI.Widget.NameAndTooltip
			{
				name = "Saturation Tolerance",
				tooltip = "Validate a material based on a specific Saturation."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MetallicMinValue = new DebugUI.Widget.NameAndTooltip
			{
				name = "Min Value",
				tooltip = "Any values set below this field are invalid and appear red on screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip MetallicMaxValue = new DebugUI.Widget.NameAndTooltip
			{
				name = "Max Value",
				tooltip = "Any values set above this field are invalid and appear blue on screen."
			};
		}

		internal static class WidgetFactory
		{
			internal static DebugUI.Widget CreateMaterialOverride(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.MaterialOverride,
					autoEnum = typeof(DebugMaterialMode),
					getter = (() => (int)panel.data.materialDebugMode),
					setter = delegate(int value)
					{
						panel.data.materialDebugMode = (DebugMaterialMode)value;
					},
					getIndex = (() => (int)panel.data.materialDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.materialDebugMode = (DebugMaterialMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateVertexAttribute(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.VertexAttribute,
					autoEnum = typeof(DebugVertexAttributeMode),
					getter = (() => (int)panel.data.vertexAttributeDebugMode),
					setter = delegate(int value)
					{
						panel.data.vertexAttributeDebugMode = (DebugVertexAttributeMode)value;
					},
					getIndex = (() => (int)panel.data.vertexAttributeDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.vertexAttributeDebugMode = (DebugVertexAttributeMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateMaterialValidationMode(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				DebugUI.EnumField enumField = new DebugUI.EnumField();
				enumField.nameAndTooltip = DebugDisplaySettingsMaterial.Strings.MaterialValidationMode;
				enumField.autoEnum = typeof(DebugMaterialValidationMode);
				enumField.getter = (() => (int)panel.data.materialValidationMode);
				enumField.setter = delegate(int value)
				{
					panel.data.materialValidationMode = (DebugMaterialValidationMode)value;
				};
				enumField.getIndex = (() => (int)panel.data.materialValidationMode);
				enumField.setIndex = delegate(int value)
				{
					panel.data.materialValidationMode = (DebugMaterialValidationMode)value;
				};
				enumField.onValueChanged = delegate(DebugUI.Field<int> _, int _)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				};
				return enumField;
			}

			internal static DebugUI.Widget CreateRenderingLayersSelectedLight(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.RenderingLayersSelectedLight,
					getter = (() => panel.data.renderingLayersSelectedLight),
					setter = delegate(bool value)
					{
						panel.data.renderingLayersSelectedLight = value;
					},
					flags = DebugUI.Flags.EditorOnly
				};
			}

			internal static DebugUI.Widget CreateSelectedLightShadowLayerMask(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.BoolField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.SelectedLightShadowLayerMask,
					getter = (() => panel.data.selectedLightShadowLayerMask),
					setter = delegate(bool value)
					{
						panel.data.selectedLightShadowLayerMask = value;
					},
					flags = DebugUI.Flags.EditorOnly,
					isHiddenCallback = (() => !panel.data.renderingLayersSelectedLight)
				};
			}

			internal static DebugUI.RenderingLayerField CreateFilterRenderingLayerMasks(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.RenderingLayerField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.FilterRenderingLayerMask,
					getter = (() => panel.data.renderingLayerMask),
					setter = delegate(RenderingLayerMask value)
					{
						panel.data.renderingLayerMask = value;
					},
					getRenderingLayerColor = ((int index) => panel.data.debugRenderingLayersColors[index]),
					setRenderingLayerColor = delegate(Vector4 value, int index)
					{
						panel.data.debugRenderingLayersColors[index] = value;
					},
					isHiddenCallback = (() => panel.data.renderingLayersSelectedLight)
				};
			}

			internal static DebugUI.Widget CreateAlbedoPreset(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				DebugUI.EnumField enumField = new DebugUI.EnumField();
				enumField.nameAndTooltip = DebugDisplaySettingsMaterial.Strings.ValidationPreset;
				enumField.autoEnum = typeof(DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset);
				enumField.getter = (() => (int)panel.data.albedoValidationPreset);
				enumField.setter = delegate(int value)
				{
					panel.data.albedoValidationPreset = (DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset)value;
				};
				enumField.getIndex = (() => (int)panel.data.albedoValidationPreset);
				enumField.setIndex = delegate(int value)
				{
					panel.data.albedoValidationPreset = (DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset)value;
				};
				enumField.onValueChanged = delegate(DebugUI.Field<int> _, int _)
				{
					DebugManager.instance.ReDrawOnScreenDebug();
				};
				return enumField;
			}

			internal static DebugUI.Widget CreateAlbedoCustomColor(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.ColorField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.AlbedoCustomColor,
					getter = (() => panel.data.albedoCompareColor),
					setter = delegate(Color value)
					{
						panel.data.albedoCompareColor = value;
					},
					isHiddenCallback = (() => panel.data.albedoValidationPreset != DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset.Custom)
				};
			}

			internal static DebugUI.Widget CreateAlbedoMinLuminance(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.AlbedoMinLuminance,
					getter = (() => panel.data.albedoMinLuminance),
					setter = delegate(float value)
					{
						panel.data.albedoMinLuminance = value;
					},
					incStep = 0.01f
				};
			}

			internal static DebugUI.Widget CreateAlbedoMaxLuminance(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.AlbedoMaxLuminance,
					getter = (() => panel.data.albedoMaxLuminance),
					setter = delegate(float value)
					{
						panel.data.albedoMaxLuminance = value;
					},
					incStep = 0.01f
				};
			}

			internal static DebugUI.Widget CreateAlbedoHueTolerance(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.AlbedoHueTolerance,
					getter = (() => panel.data.albedoHueTolerance),
					setter = delegate(float value)
					{
						panel.data.albedoHueTolerance = value;
					},
					incStep = 0.01f,
					isHiddenCallback = (() => panel.data.albedoValidationPreset == DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset.DefaultLuminance)
				};
			}

			internal static DebugUI.Widget CreateAlbedoSaturationTolerance(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.AlbedoSaturationTolerance,
					getter = (() => panel.data.albedoSaturationTolerance),
					setter = delegate(float value)
					{
						panel.data.albedoSaturationTolerance = value;
					},
					incStep = 0.01f,
					isHiddenCallback = (() => panel.data.albedoValidationPreset == DebugDisplaySettingsMaterial.AlbedoDebugValidationPreset.DefaultLuminance)
				};
			}

			internal static DebugUI.Widget CreateMetallicMinValue(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.MetallicMinValue,
					getter = (() => panel.data.metallicMinValue),
					setter = delegate(float value)
					{
						panel.data.metallicMinValue = value;
					},
					incStep = 0.01f
				};
			}

			internal static DebugUI.Widget CreateMetallicMaxValue(DebugDisplaySettingsMaterial.SettingsPanel panel)
			{
				return new DebugUI.FloatField
				{
					nameAndTooltip = DebugDisplaySettingsMaterial.Strings.MetallicMaxValue,
					getter = (() => panel.data.metallicMaxValue),
					setter = delegate(float value)
					{
						panel.data.metallicMaxValue = value;
					},
					incStep = 0.01f
				};
			}
		}

		[DisplayInfo(name = "Material", order = 2)]
		internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsMaterial>
		{
			public SettingsPanel(DebugDisplaySettingsMaterial data) : base(data)
			{
				base.AddWidget(new DebugUI.RuntimeDebugShadersMessageBox());
				base.AddWidget(new DebugUI.Foldout
				{
					displayName = "Material Filters",
					flags = DebugUI.Flags.FrequentlyUsed,
					opened = true,
					children = 
					{
						DebugDisplaySettingsMaterial.WidgetFactory.CreateMaterialOverride(this),
						new DebugUI.Container
						{
							displayName = "Rendering Layer Masks Settings",
							isHiddenCallback = (() => data.materialDebugMode != DebugMaterialMode.RenderingLayerMasks),
							children = 
							{
								DebugDisplaySettingsMaterial.WidgetFactory.CreateRenderingLayersSelectedLight(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateSelectedLightShadowLayerMask(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateFilterRenderingLayerMasks(this)
							}
						},
						DebugDisplaySettingsMaterial.WidgetFactory.CreateVertexAttribute(this)
					}
				});
				base.AddWidget(new DebugUI.Foldout
				{
					displayName = "Material Validation",
					opened = true,
					children = 
					{
						DebugDisplaySettingsMaterial.WidgetFactory.CreateMaterialValidationMode(this),
						new DebugUI.Container
						{
							displayName = "Albedo Settings",
							isHiddenCallback = (() => data.materialValidationMode != DebugMaterialValidationMode.Albedo),
							children = 
							{
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoPreset(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoCustomColor(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoMinLuminance(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoMaxLuminance(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoHueTolerance(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateAlbedoSaturationTolerance(this)
							}
						},
						new DebugUI.Container
						{
							displayName = "Metallic Settings",
							isHiddenCallback = (() => data.materialValidationMode != DebugMaterialValidationMode.Metallic),
							children = 
							{
								DebugDisplaySettingsMaterial.WidgetFactory.CreateMetallicMinValue(this),
								DebugDisplaySettingsMaterial.WidgetFactory.CreateMetallicMaxValue(this)
							}
						}
					}
				});
			}
		}
	}
}
