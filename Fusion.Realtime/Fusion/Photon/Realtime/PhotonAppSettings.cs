using System;
using UnityEngine;

namespace Fusion.Photon.Realtime
{
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/getting-started/initial-setup")]
	[CreateAssetMenu(menuName = "Fusion/Photon Application Settings", fileName = "PhotonAppSettings")]
	[FusionGlobalScriptableObject("Assets/Photon/Fusion/Resources/PhotonAppSettings.asset")]
	[Serializable]
	public class PhotonAppSettings : FusionGlobalScriptableObject<PhotonAppSettings>
	{
		public static PhotonAppSettings Global
		{
			get
			{
				return FusionGlobalScriptableObject<PhotonAppSettings>.GlobalInternal;
			}
		}

		public static bool TryGetGlobal(out PhotonAppSettings settings)
		{
			return FusionGlobalScriptableObject<PhotonAppSettings>.TryGetGlobalInternal(out settings);
		}

		public static bool IsGlobalLoaded
		{
			get
			{
				return FusionGlobalScriptableObject<PhotonAppSettings>.IsGlobalLoadedInternal;
			}
		}

		[InlineHelp]
		public FusionAppSettings AppSettings;
	}
}
