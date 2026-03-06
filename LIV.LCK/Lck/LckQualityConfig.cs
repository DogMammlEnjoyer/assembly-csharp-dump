using System;
using System.Collections.Generic;
using UnityEngine;

namespace Liv.Lck
{
	[CreateAssetMenu(fileName = "LckQualityConfig", menuName = "LIV/LCK/QualityConfig")]
	public class LckQualityConfig : ScriptableObject, ILckQualityConfig
	{
		public List<QualityOption> GetQualityOptionsForSystem()
		{
			RuntimePlatform platform = Application.platform;
			if (platform <= RuntimePlatform.WindowsEditor)
			{
				if (platform > RuntimePlatform.WindowsPlayer && platform != RuntimePlatform.WindowsEditor)
				{
					goto IL_A1;
				}
			}
			else
			{
				if (platform == RuntimePlatform.Android)
				{
					DeviceModel? currentDeviceModel = this.GetCurrentDeviceModel();
					if (currentDeviceModel != null)
					{
						foreach (QualityOptionOverride qualityOptionOverride in this.AndroidOptionsDeviceOverrides)
						{
							DeviceModel deviceModel = qualityOptionOverride.DeviceModel;
							DeviceModel? deviceModel2 = currentDeviceModel;
							if (deviceModel == deviceModel2.GetValueOrDefault() & deviceModel2 != null)
							{
								return qualityOptionOverride.QualityOptions;
							}
						}
					}
					return this.BaseAndroidQualityOptions;
				}
				if (platform != RuntimePlatform.LinuxPlayer && platform != RuntimePlatform.LinuxEditor)
				{
					goto IL_A1;
				}
			}
			return this.DesktopQualityOptions;
			IL_A1:
			throw new NotImplementedException(string.Format("LCK does not support {0} platform", Application.platform));
		}

		private DeviceModel? GetCurrentDeviceModel()
		{
			return null;
		}

		[Header("Android")]
		public List<QualityOption> BaseAndroidQualityOptions = new List<QualityOption>();

		public List<QualityOptionOverride> AndroidOptionsDeviceOverrides = new List<QualityOptionOverride>();

		[Header("Desktop")]
		public List<QualityOption> DesktopQualityOptions = new List<QualityOption>();
	}
}
