using System;
using System.Reflection;

namespace UnityEngine.Rendering.Universal
{
	public class DebugDisplaySettingsLighting : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
	{
		public DebugLightingMode lightingDebugMode { get; set; }

		public DebugLightingFeatureFlags lightingFeatureFlags { get; set; }

		public HDRDebugMode hdrDebugMode { get; set; }

		public bool AreAnySettingsActive
		{
			get
			{
				return this.lightingDebugMode != DebugLightingMode.None || this.lightingFeatureFlags != DebugLightingFeatureFlags.None || this.hdrDebugMode > HDRDebugMode.None;
			}
		}

		public bool IsPostProcessingAllowed
		{
			get
			{
				return this.lightingDebugMode != DebugLightingMode.Reflections && this.lightingDebugMode != DebugLightingMode.ReflectionsWithSmoothness;
			}
		}

		public bool IsLightingActive
		{
			get
			{
				return true;
			}
		}

		IDebugDisplaySettingsPanelDisposable IDebugDisplaySettingsData.CreatePanel()
		{
			return new DebugDisplaySettingsLighting.SettingsPanel(this);
		}

		internal static class Strings
		{
			public static readonly DebugUI.Widget.NameAndTooltip LightingDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "Lighting Debug Mode",
				tooltip = "Use the drop-down to select which lighting and shadow debug information to overlay on the screen."
			};

			public static readonly DebugUI.Widget.NameAndTooltip LightingFeatures = new DebugUI.Widget.NameAndTooltip
			{
				name = "Lighting Features",
				tooltip = "Filter and debug selected lighting features in the system."
			};

			public static readonly DebugUI.Widget.NameAndTooltip HDRDebugMode = new DebugUI.Widget.NameAndTooltip
			{
				name = "HDR Debug Mode",
				tooltip = "Select which HDR brightness debug information to overlay on the screen."
			};
		}

		internal static class WidgetFactory
		{
			internal static DebugUI.Widget CreateLightingDebugMode(DebugDisplaySettingsLighting.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsLighting.Strings.LightingDebugMode,
					autoEnum = typeof(DebugLightingMode),
					getter = (() => (int)panel.data.lightingDebugMode),
					setter = delegate(int value)
					{
						panel.data.lightingDebugMode = (DebugLightingMode)value;
					},
					getIndex = (() => (int)panel.data.lightingDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.lightingDebugMode = (DebugLightingMode)value;
					}
				};
			}

			internal static DebugUI.Widget CreateLightingFeatures(DebugDisplaySettingsLighting.SettingsPanel panel)
			{
				return new DebugUI.BitField
				{
					nameAndTooltip = DebugDisplaySettingsLighting.Strings.LightingFeatures,
					getter = (() => panel.data.lightingFeatureFlags),
					setter = delegate(Enum value)
					{
						panel.data.lightingFeatureFlags = (DebugLightingFeatureFlags)value;
					},
					enumType = typeof(DebugLightingFeatureFlags)
				};
			}

			internal static DebugUI.Widget CreateHDRDebugMode(DebugDisplaySettingsLighting.SettingsPanel panel)
			{
				return new DebugUI.EnumField
				{
					nameAndTooltip = DebugDisplaySettingsLighting.Strings.HDRDebugMode,
					autoEnum = typeof(HDRDebugMode),
					getter = (() => (int)panel.data.hdrDebugMode),
					setter = delegate(int value)
					{
						panel.data.hdrDebugMode = (HDRDebugMode)value;
					},
					getIndex = (() => (int)panel.data.hdrDebugMode),
					setIndex = delegate(int value)
					{
						panel.data.hdrDebugMode = (HDRDebugMode)value;
					}
				};
			}
		}

		[DisplayInfo(name = "Lighting", order = 3)]
		internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsLighting>
		{
			public SettingsPanel(DebugDisplaySettingsLighting data) : base(data)
			{
				base.AddWidget(new DebugUI.RuntimeDebugShadersMessageBox());
				DebugUI.Foldout foldout = new DebugUI.Foldout();
				foldout.displayName = "Lighting Debug Modes";
				foldout.flags = DebugUI.Flags.FrequentlyUsed;
				foldout.opened = true;
				foldout.children.Add(DebugDisplaySettingsLighting.WidgetFactory.CreateLightingDebugMode(this));
				foldout.children.Add(DebugDisplaySettingsLighting.WidgetFactory.CreateHDRDebugMode(this));
				foldout.children.Add(DebugDisplaySettingsLighting.WidgetFactory.CreateLightingFeatures(this));
				HelpURLAttribute customAttribute = typeof(DebugDisplaySettingsLighting).GetCustomAttribute<HelpURLAttribute>();
				foldout.documentationUrl = ((customAttribute != null) ? customAttribute.URL : null);
				base.AddWidget(foldout);
			}
		}
	}
}
