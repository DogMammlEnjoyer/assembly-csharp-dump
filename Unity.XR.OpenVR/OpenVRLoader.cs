using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Valve.VR;

namespace Unity.XR.OpenVR
{
	public class OpenVRLoader : XRLoaderHelper
	{
		public XRDisplaySubsystem displaySubsystem
		{
			get
			{
				return this.GetLoadedSubsystem<XRDisplaySubsystem>();
			}
		}

		public XRInputSubsystem inputSubsystem
		{
			get
			{
				return this.GetLoadedSubsystem<XRInputSubsystem>();
			}
		}

		public override bool Initialize()
		{
			base.CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(OpenVRLoader.s_DisplaySubsystemDescriptors, "OpenVR Display");
			EVRInitError initializationResult = OpenVRLoader.GetInitializationResult();
			if (initializationResult != EVRInitError.None)
			{
				base.DestroySubsystem<XRDisplaySubsystem>();
				Debug.LogError("<b>[OpenVR]</b> Could not initialize OpenVR. Error code: " + initializationResult.ToString());
				return false;
			}
			base.CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(OpenVRLoader.s_InputSubsystemDescriptors, "OpenVR Input");
			OpenVREvents.Initialize(false);
			OpenVRLoader.TickCallbackDelegate tickCallbackDelegate = new OpenVRLoader.TickCallbackDelegate(OpenVRLoader.TickCallback);
			OpenVRLoader.RegisterTickCallback(tickCallbackDelegate);
			tickCallbackDelegate(0);
			return this.displaySubsystem != null && this.inputSubsystem != null;
		}

		private string GetEscapedApplicationName()
		{
			if (string.IsNullOrEmpty(Application.productName))
			{
				return "";
			}
			return Application.productName.Replace("\\", "\\\\").Replace("\"", "\\\"");
		}

		private void WatchForReload()
		{
		}

		private void CleanupReloadWatcher()
		{
		}

		public override bool Start()
		{
			this.running = true;
			this.WatchForReload();
			base.StartSubsystem<XRDisplaySubsystem>();
			base.StartSubsystem<XRInputSubsystem>();
			this.SetupFileSystemWatchers();
			return true;
		}

		private void SetupFileSystemWatchers()
		{
			this.SetupFileSystemWatcher();
		}

		private void SetupFileSystemWatcher()
		{
			try
			{
				this.settings = OpenVRSettings.GetSettings(true);
				if (this.watcher == null && this.running)
				{
					this.watcherFile = new FileInfo("openvr_mirrorview.cfg");
					this.watcher = new FileSystemWatcher(this.watcherFile.DirectoryName, this.watcherFile.Name);
					this.watcher.NotifyFilter = NotifyFilters.LastWrite;
					this.watcher.Created += this.OnChanged;
					this.watcher.Changed += this.OnChanged;
					this.watcher.EnableRaisingEvents = true;
					if (this.watcherFile.Exists)
					{
						this.OnChanged(null, null);
					}
				}
			}
			catch
			{
			}
		}

		private void DestroyMirrorModeWatcher()
		{
			if (this.watcher != null)
			{
				this.watcher.Created -= this.OnChanged;
				this.watcher.Changed -= this.OnChanged;
				this.watcher.EnableRaisingEvents = false;
				this.watcher.Dispose();
				this.watcher = null;
			}
		}

		private void OnChanged(object source, FileSystemEventArgs e)
		{
			this.ReadMirrorModeConfig();
		}

		private void ReadMirrorModeConfig()
		{
			try
			{
				string[] array = File.ReadAllLines("openvr_mirrorview.cfg");
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('=', StringSplitOptions.None);
					if (array2.Length == 2 && array2[0] == "MirrorViewMode")
					{
						string text = array2[1];
						OpenVRSettings.MirrorViewModes mirrorViewModes = OpenVRSettings.MirrorViewModes.None;
						if (text.Equals("left", StringComparison.CurrentCultureIgnoreCase))
						{
							mirrorViewModes = OpenVRSettings.MirrorViewModes.Left;
						}
						else if (text.Equals("right", StringComparison.CurrentCultureIgnoreCase))
						{
							mirrorViewModes = OpenVRSettings.MirrorViewModes.Right;
						}
						else if (text.Equals("openvr", StringComparison.CurrentCultureIgnoreCase))
						{
							mirrorViewModes = OpenVRSettings.MirrorViewModes.OpenVR;
						}
						else if (text.Equals("none", StringComparison.CurrentCultureIgnoreCase))
						{
							mirrorViewModes = OpenVRSettings.MirrorViewModes.None;
						}
						else
						{
							Debug.LogError("<b>[OpenVR]</b> Invalid mode specified in openvr_mirrorview.cfg. Options are: Left, Right, None, and OpenVR.");
						}
						Debug.Log("<b>[OpenVR]</b> Mirror View Mode changed via file to: " + mirrorViewModes.ToString());
						OpenVRSettings.SetMirrorViewMode((ushort)mirrorViewModes);
					}
				}
			}
			catch
			{
			}
		}

		public override bool Stop()
		{
			this.running = false;
			OpenVRLoader.CleanupTick();
			this.CleanupReloadWatcher();
			this.DestroyMirrorModeWatcher();
			base.StopSubsystem<XRInputSubsystem>();
			base.StopSubsystem<XRDisplaySubsystem>();
			return true;
		}

		public override bool Deinitialize()
		{
			OpenVRLoader.CleanupTick();
			this.CleanupReloadWatcher();
			this.DestroyMirrorModeWatcher();
			base.DestroySubsystem<XRInputSubsystem>();
			base.DestroySubsystem<XRDisplaySubsystem>();
			return true;
		}

		private static void CleanupTick()
		{
			OpenVRLoader.RegisterTickCallback(null);
		}

		[DllImport("XRSDKOpenVR", CharSet = CharSet.Auto)]
		private static extern void SetUserDefinedSettings(OpenVRLoader.UserDefinedSettings settings);

		[DllImport("XRSDKOpenVR", CharSet = CharSet.Auto)]
		private static extern EVRInitError GetInitializationResult();

		[DllImport("XRSDKOpenVR", CharSet = CharSet.Auto)]
		private static extern void RegisterTickCallback([MarshalAs(UnmanagedType.FunctionPtr)] OpenVRLoader.TickCallbackDelegate callbackPointer);

		[MonoPInvokeCallback(typeof(OpenVRLoader.TickCallbackDelegate))]
		public static void TickCallback(int value)
		{
			OpenVREvents.Update();
		}

		private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();

		private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

		private bool running;

		private FileInfo watcherFile;

		private FileSystemWatcher watcher;

		private const string mirrorViewPath = "openvr_mirrorview.cfg";

		private OpenVRSettings settings;

		private UnityEvent[] events;

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct UserDefinedSettings
		{
			public ushort stereoRenderingMode;

			public ushort initializationType;

			public ushort mirrorViewMode;

			[MarshalAs(UnmanagedType.LPStr)]
			public string editorAppKey;

			[MarshalAs(UnmanagedType.LPStr)]
			public string actionManifestPath;

			[MarshalAs(UnmanagedType.LPStr)]
			public string applicationName;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void TickCallbackDelegate(int value);
	}
}
