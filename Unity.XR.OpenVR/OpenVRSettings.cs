using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR.Management;

namespace Unity.XR.OpenVR
{
	[XRConfigurationData("OpenVR", "Unity.XR.OpenVR.Settings")]
	[Serializable]
	public class OpenVRSettings : ScriptableObject
	{
		public static string GetStreamingSteamVRPath(bool create = true)
		{
			string text = Path.Combine(Application.streamingAssetsPath, "SteamVR");
			if (create)
			{
				OpenVRSettings.CreateDirectory(new DirectoryInfo(text));
			}
			return text;
		}

		private static void CreateDirectory(DirectoryInfo directory)
		{
			if (!directory.Parent.Exists)
			{
				OpenVRSettings.CreateDirectory(directory.Parent);
			}
			if (!directory.Exists)
			{
				directory.Create();
			}
		}

		public ushort GetStereoRenderingMode()
		{
			return (ushort)this.StereoRenderingMode;
		}

		public ushort GetInitializationType()
		{
			return (ushort)this.InitializationType;
		}

		public OpenVRSettings.MirrorViewModes GetMirrorViewMode()
		{
			return this.MirrorView;
		}

		public void SetMirrorViewMode(OpenVRSettings.MirrorViewModes newMode)
		{
			this.MirrorView = newMode;
			OpenVRSettings.SetMirrorViewMode((ushort)newMode);
		}

		public string GenerateEditorAppKey()
		{
			return string.Format("application.generated.unity.{0}.{1}.exe", OpenVRSettings.CleanProductName(), ((int)(Random.value * 2.1474836E+09f)).ToString());
		}

		private static string CleanProductName()
		{
			string text = Application.productName;
			if (string.IsNullOrEmpty(text))
			{
				text = "unnamed_product";
			}
			else
			{
				text = Regex.Replace(Application.productName, "[^\\w\\._]", "");
				text = text.ToLower();
			}
			return text;
		}

		public static OpenVRSettings GetSettings(bool create = true)
		{
			OpenVRSettings openVRSettings = OpenVRSettings.s_Settings;
			if (openVRSettings == null && create)
			{
				openVRSettings = ScriptableObject.CreateInstance<OpenVRSettings>();
			}
			return openVRSettings;
		}

		[DllImport("XRSDKOpenVR", CharSet = CharSet.Auto)]
		public static extern void SetMirrorViewMode(ushort mirrorViewMode);

		public bool InitializeActionManifestFileRelativeFilePath()
		{
			string actionManifestFileRelativeFilePath = this.ActionManifestFileRelativeFilePath;
			if (OpenVRHelpers.IsUsingSteamVRInput())
			{
				string text = Path.Combine(OpenVRSettings.GetStreamingSteamVRPath(false), OpenVRHelpers.GetActionManifestNameFromPlugin());
				string fullPath = Path.GetFullPath(".");
				text = text.Remove(0, fullPath.Length + 1);
				if (text.StartsWith("Assets"))
				{
					text = text.Remove(0, "Assets".Length + 1);
				}
			}
			return false;
		}

		public void Awake()
		{
			OpenVRSettings.s_Settings = this;
		}

		[SerializeField]
		[Tooltip("This will check the package version and the latest on github and prompt you to upgrade once per project load.")]
		public bool PromptToUpgradePackage = true;

		[SerializeField]
		[Tooltip("This will check the package version and the latest on github and prompt you to upgrade once per project load.")]
		public bool PromptToUpgradePreviewPackages = true;

		[SerializeField]
		[Tooltip("This allows developers to skip upgrade prompts for just this version.")]
		public string SkipPromptForVersion;

		[SerializeField]
		[Tooltip("Set the Stereo Rendering Method")]
		public OpenVRSettings.StereoRenderingModes StereoRenderingMode = OpenVRSettings.StereoRenderingModes.SinglePassInstanced;

		[SerializeField]
		[Tooltip("Most applications initialize as type Scene")]
		public OpenVRSettings.InitializationTypes InitializationType = OpenVRSettings.InitializationTypes.Scene;

		[SerializeField]
		[Tooltip("A generated unique identifier for your application while in the editor")]
		public string EditorAppKey;

		[SerializeField]
		[Tooltip("Internal value that tells the system what the relative path is to the manifest")]
		public string ActionManifestFileRelativeFilePath;

		[SerializeField]
		[Tooltip("Which eye to use when rendering the headset view to the main window (none, left, right, or a composite of both + OpenVR overlays)")]
		public OpenVRSettings.MirrorViewModes MirrorView = OpenVRSettings.MirrorViewModes.Right;

		public const string StreamingAssetsFolderName = "SteamVR";

		public const string ActionManifestFileName = "legacy_manifest.json";

		[SerializeField]
		[Tooltip("Internal value that tells the system if we have copied the default binding files yet.")]
		public bool HasCopiedDefaults;

		public static OpenVRSettings s_Settings;

		public enum StereoRenderingModes
		{
			MultiPass,
			SinglePassInstanced
		}

		public enum InitializationTypes
		{
			Scene = 1,
			Overlay
		}

		public enum MirrorViewModes
		{
			None,
			Left,
			Right,
			OpenVR
		}
	}
}
