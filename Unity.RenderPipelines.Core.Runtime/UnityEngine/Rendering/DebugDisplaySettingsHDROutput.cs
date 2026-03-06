using System;

namespace UnityEngine.Rendering
{
	public class DebugDisplaySettingsHDROutput
	{
		public static DebugUI.Table CreateHDROuputDisplayTable()
		{
			DebugUI.Table table = new DebugUI.Table
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.hdrOutputAPI,
				isReadOnly = true
			};
			DebugUI.Table.Row row = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.hdrActive,
				opened = true
			};
			DebugUI.Table.Row row2 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.hdrAvailable,
				opened = true
			};
			DebugUI.Table.Row row3 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.gamut,
				opened = false
			};
			DebugUI.Table.Row row4 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.format,
				opened = false
			};
			DebugUI.Table.Row row5 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.autoHdrTonemapping,
				opened = false
			};
			DebugUI.Table.Row row6 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.paperWhite,
				opened = false
			};
			DebugUI.Table.Row row7 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.minLuminance,
				opened = false
			};
			DebugUI.Table.Row row8 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.maxLuminance,
				opened = false
			};
			DebugUI.Table.Row row9 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.maxFullFrameLuminance,
				opened = false
			};
			DebugUI.Table.Row row10 = new DebugUI.Table.Row
			{
				displayName = DebugDisplaySettingsHDROutput.Strings.modeChangeRequested,
				opened = false
			};
			HDROutputSettings[] displays = HDROutputSettings.displays;
			for (int i = 0; i < displays.Length; i++)
			{
				HDROutputSettings d = displays[i];
				int num = i + 1;
				string text = DebugDisplaySettingsHDROutput.Strings.displayName + num.ToString();
				if (HDROutputSettings.main == d)
				{
					text += DebugDisplaySettingsHDROutput.Strings.displayMain;
				}
				row.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = (() => d.active)
				});
				row2.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = (() => d.available)
				});
				row3.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.displayColorGamut;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row4.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.graphicsFormat;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row5.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.automaticHDRTonemapping;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row6.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.paperWhiteNits;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row7.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.minToneMapLuminance;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row8.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.maxToneMapLuminance;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row9.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.maxFullFrameToneMapLuminance;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
				row10.children.Add(new DebugUI.Value
				{
					displayName = text,
					getter = delegate
					{
						if (d.available)
						{
							return d.HDRModeChangeRequested;
						}
						return DebugDisplaySettingsHDROutput.Strings.notAvailable;
					}
				});
			}
			table.children.Add(row);
			table.children.Add(row2);
			table.children.Add(row3);
			table.children.Add(row4);
			table.children.Add(row5);
			table.children.Add(row6);
			table.children.Add(row7);
			table.children.Add(row8);
			table.children.Add(row9);
			table.children.Add(row10);
			return table;
		}

		private static class Strings
		{
			public static readonly string hdrOutputAPI = "HDROutputSettings";

			public static readonly string displayName = "Display ";

			public static readonly string displayMain = " (main)";

			public static readonly string hdrActive = "HDR Output Active";

			public static readonly string hdrAvailable = "HDR Output Available";

			public static readonly string gamut = "Display Color Gamut";

			public static readonly string format = "Display Buffer Graphics Format";

			public static readonly string autoHdrTonemapping = "Automatic HDR Tonemapping";

			public static readonly string paperWhite = "Paper White Nits";

			public static readonly string minLuminance = "Min Tone Map Luminance";

			public static readonly string maxLuminance = "Max Tone Map Luminance";

			public static readonly string maxFullFrameLuminance = "Max Full Frame Tone Map Luminance";

			public static readonly string modeChangeRequested = "HDR Mode Change Requested";

			public static readonly string notAvailable = "N/A";
		}
	}
}
