using System;
using UnityEngine;

namespace Oculus.Platform
{
	public sealed class PlatformSettings : ScriptableObject
	{
		public static string AppID
		{
			get
			{
				return PlatformSettings.Instance.ovrAppID;
			}
			set
			{
				PlatformSettings.Instance.ovrAppID = value;
			}
		}

		public static string MobileAppID
		{
			get
			{
				return PlatformSettings.Instance.ovrMobileAppID;
			}
			set
			{
				PlatformSettings.Instance.ovrMobileAppID = value;
			}
		}

		public static bool UseStandalonePlatform
		{
			get
			{
				return PlatformSettings.Instance.ovrUseStandalonePlatform;
			}
			set
			{
				PlatformSettings.Instance.ovrUseStandalonePlatform = value;
			}
		}

		public static bool UseMobileAppIDInEditor
		{
			get
			{
				return PlatformSettings.Instance.ovrUseMobileAppIDInEditor;
			}
			set
			{
				PlatformSettings.Instance.ovrUseMobileAppIDInEditor = value;
			}
		}

		public static PlatformSettings Instance
		{
			get
			{
				if (PlatformSettings.instance == null)
				{
					PlatformSettings.instance = Resources.Load<PlatformSettings>("OculusPlatformSettings");
					if (PlatformSettings.instance == null)
					{
						PlatformSettings.instance = ScriptableObject.CreateInstance<PlatformSettings>();
					}
				}
				return PlatformSettings.instance;
			}
			set
			{
				PlatformSettings.instance = value;
			}
		}

		[SerializeField]
		private string ovrAppID = "";

		[SerializeField]
		private string ovrMobileAppID = "";

		[SerializeField]
		private bool ovrUseMobileAppIDInEditor;

		[SerializeField]
		private bool ovrUseStandalonePlatform = true;

		private static PlatformSettings instance;
	}
}
