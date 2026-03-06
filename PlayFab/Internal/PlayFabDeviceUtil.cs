using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.SharedModels;
using UnityEngine;

namespace PlayFab.Internal
{
	public static class PlayFabDeviceUtil
	{
		private static void DoAttributeInstall(PlayFabApiSettings settings, IPlayFabInstanceApi instanceApi)
		{
			if (!PlayFabDeviceUtil._needsAttribution || settings.DisableAdvertising)
			{
				return;
			}
			AttributeInstallRequest attributeInstallRequest = new AttributeInstallRequest();
			string advertisingIdType = settings.AdvertisingIdType;
			if (!(advertisingIdType == "Adid"))
			{
				if (advertisingIdType == "Idfa")
				{
					attributeInstallRequest.Idfa = settings.AdvertisingIdValue;
				}
			}
			else
			{
				attributeInstallRequest.Adid = settings.AdvertisingIdValue;
			}
			PlayFabClientInstanceAPI playFabClientInstanceAPI = instanceApi as PlayFabClientInstanceAPI;
			if (playFabClientInstanceAPI != null)
			{
				playFabClientInstanceAPI.AttributeInstall(attributeInstallRequest, new Action<AttributeInstallResult>(PlayFabDeviceUtil.OnAttributeInstall), null, settings, null);
				return;
			}
			PlayFabClientAPI.AttributeInstall(attributeInstallRequest, new Action<AttributeInstallResult>(PlayFabDeviceUtil.OnAttributeInstall), null, settings, null);
		}

		private static void OnAttributeInstall(AttributeInstallResult result)
		{
			PlayFabApiSettings playFabApiSettings = (PlayFabApiSettings)result.CustomData;
			playFabApiSettings.AdvertisingIdType += "_Successful";
		}

		private static void SendDeviceInfoToPlayFab(PlayFabApiSettings settings, IPlayFabInstanceApi instanceApi)
		{
			if (settings.DisableDeviceInfo || !PlayFabDeviceUtil._gatherDeviceInfo)
			{
				return;
			}
			ISerializerPlugin plugin = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
			DeviceInfoRequest request = new DeviceInfoRequest
			{
				Info = plugin.DeserializeObject<Dictionary<string, object>>(plugin.SerializeObject(new PlayFabDataGatherer()))
			};
			PlayFabClientInstanceAPI playFabClientInstanceAPI = instanceApi as PlayFabClientInstanceAPI;
			if (playFabClientInstanceAPI != null)
			{
				playFabClientInstanceAPI.ReportDeviceInfo(request, null, new Action<PlayFabError>(PlayFabDeviceUtil.OnGatherFail), settings, null);
				return;
			}
			PlayFabClientAPI.ReportDeviceInfo(request, null, new Action<PlayFabError>(PlayFabDeviceUtil.OnGatherFail), settings, null);
		}

		private static void OnGatherFail(PlayFabError error)
		{
			Debug.Log("OnGatherFail: " + error.GenerateErrorReport());
		}

		public static void OnPlayFabLogin(PlayFabResultCommon result, PlayFabApiSettings settings, IPlayFabInstanceApi instanceApi)
		{
			LoginResult loginResult = result as LoginResult;
			RegisterPlayFabUserResult registerPlayFabUserResult = result as RegisterPlayFabUserResult;
			if (loginResult == null && registerPlayFabUserResult == null)
			{
				return;
			}
			UserSettings settingsForUser = null;
			string playFabId = null;
			string entityId = null;
			string entityType = null;
			if (loginResult != null)
			{
				settingsForUser = loginResult.SettingsForUser;
				playFabId = loginResult.PlayFabId;
				if (loginResult.EntityToken != null)
				{
					entityId = loginResult.EntityToken.Entity.Id;
					entityType = loginResult.EntityToken.Entity.Type;
				}
			}
			else if (registerPlayFabUserResult != null)
			{
				settingsForUser = registerPlayFabUserResult.SettingsForUser;
				playFabId = registerPlayFabUserResult.PlayFabId;
				if (registerPlayFabUserResult.EntityToken != null)
				{
					entityId = registerPlayFabUserResult.EntityToken.Entity.Id;
					entityType = registerPlayFabUserResult.EntityToken.Entity.Type;
				}
			}
			PlayFabDeviceUtil._OnPlayFabLogin(settingsForUser, playFabId, entityId, entityType, settings, instanceApi);
		}

		private static void _OnPlayFabLogin(UserSettings settingsForUser, string playFabId, string entityId, string entityType, PlayFabApiSettings settings, IPlayFabInstanceApi instanceApi)
		{
			PlayFabDeviceUtil._needsAttribution = (PlayFabDeviceUtil._gatherDeviceInfo = (PlayFabDeviceUtil._gatherScreenTime = false));
			if (settingsForUser != null)
			{
				PlayFabDeviceUtil._needsAttribution = settingsForUser.NeedsAttribution;
				PlayFabDeviceUtil._gatherDeviceInfo = settingsForUser.GatherDeviceInfo;
				PlayFabDeviceUtil._gatherScreenTime = settingsForUser.GatherFocusInfo;
			}
			if (settings.AdvertisingIdType != null && settings.AdvertisingIdValue != null)
			{
				PlayFabDeviceUtil.DoAttributeInstall(settings, instanceApi);
			}
			else
			{
				PlayFabDeviceUtil.GetAdvertIdFromUnity(settings, instanceApi);
			}
			PlayFabDeviceUtil.SendDeviceInfoToPlayFab(settings, instanceApi);
			if (!string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(entityType) && PlayFabDeviceUtil._gatherScreenTime)
			{
				PlayFabHttp.InitializeScreenTimeTracker(entityId, entityType, playFabId);
				return;
			}
			settings.DisableFocusTimeCollection = true;
		}

		private static void GetAdvertIdFromUnity(PlayFabApiSettings settings, IPlayFabInstanceApi instanceApi)
		{
		}

		private static bool _needsAttribution;

		private static bool _gatherDeviceInfo;

		private static bool _gatherScreenTime;
	}
}
