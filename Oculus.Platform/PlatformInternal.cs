using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class PlatformInternal
	{
		public static void CrashApplication()
		{
			CAPI.ovr_CrashApplication();
		}

		internal static Message ParseMessageHandle(IntPtr messageHandle, Message.MessageType messageType)
		{
			Message result = null;
			if (messageType > (Message.MessageType)1317270237U)
			{
				if (messageType <= (Message.MessageType)1675623566U)
				{
					if (messageType <= (Message.MessageType)1477614734U)
					{
						if (messageType <= (Message.MessageType)1376744524U)
						{
							if (messageType == (Message.MessageType)1328734477U)
							{
								goto IL_3D8;
							}
							if (messageType != (Message.MessageType)1343932350U)
							{
								if (messageType != (Message.MessageType)1376744524U)
								{
									return result;
								}
								return new MessageWithInstalledApplicationList(messageHandle);
							}
						}
						else if (messageType <= (Message.MessageType)1449304081U)
						{
							if (messageType == (Message.MessageType)1434844938U)
							{
								goto IL_423;
							}
							if (messageType != (Message.MessageType)1449304081U)
							{
								return result;
							}
							goto IL_462;
						}
						else
						{
							if (messageType == (Message.MessageType)1467721888U)
							{
								goto IL_42C;
							}
							if (messageType != (Message.MessageType)1477614734U)
							{
								return result;
							}
							goto IL_3D8;
						}
					}
					else if (messageType <= (Message.MessageType)1553310963U)
					{
						if (messageType == (Message.MessageType)1480774160U)
						{
							goto IL_450;
						}
						if (messageType == (Message.MessageType)1489764138U)
						{
							return new MessageWithPartyUnderCurrentParty(messageHandle);
						}
						if (messageType != (Message.MessageType)1553310963U)
						{
							return result;
						}
						goto IL_3D8;
					}
					else if (messageType <= (Message.MessageType)1590749746U)
					{
						if (messageType == (Message.MessageType)1586058173U)
						{
							return new MessageWithParty(messageHandle);
						}
						if (messageType != (Message.MessageType)1590749746U)
						{
							return result;
						}
						goto IL_3D8;
					}
					else
					{
						if (messageType == (Message.MessageType)1618513035U)
						{
							goto IL_462;
						}
						if (messageType != (Message.MessageType)1675623566U)
						{
							return result;
						}
						goto IL_3D8;
					}
				}
				else if (messageType <= (Message.MessageType)1819161571U)
				{
					if (messageType <= (Message.MessageType)1742839095U)
					{
						if (messageType == (Message.MessageType)1684899167U)
						{
							goto IL_411;
						}
						if (messageType == (Message.MessageType)1701884605U)
						{
							goto IL_3D8;
						}
						if (messageType != (Message.MessageType)1742839095U)
						{
							return result;
						}
						goto IL_423;
					}
					else if (messageType <= (Message.MessageType)1788128654U)
					{
						if (messageType == (Message.MessageType)1744993395U)
						{
							goto IL_447;
						}
						if (messageType != (Message.MessageType)1788128654U)
						{
							return result;
						}
						goto IL_3D8;
					}
					else
					{
						if (messageType == (Message.MessageType)1798743375U)
						{
							goto IL_450;
						}
						if (messageType != (Message.MessageType)1819161571U)
						{
							return result;
						}
						return new MessageWithAbuseReportRecording(messageHandle);
					}
				}
				else if (messageType <= (Message.MessageType)1921499523U)
				{
					if (messageType <= (Message.MessageType)1874211363U)
					{
						if (messageType == (Message.MessageType)1859521077U)
						{
							return new MessageWithNetSyncSessionList(messageHandle);
						}
						if (messageType != (Message.MessageType)1874211363U)
						{
							return result;
						}
						goto IL_450;
					}
					else
					{
						if (messageType == (Message.MessageType)1895893271U)
						{
							goto IL_3D8;
						}
						if (messageType != (Message.MessageType)1921499523U)
						{
							return result;
						}
						goto IL_447;
					}
				}
				else if (messageType <= (Message.MessageType)2066701532U)
				{
					if (messageType == (Message.MessageType)1990567876U)
					{
						goto IL_450;
					}
					if (messageType != (Message.MessageType)2066701532U)
					{
						return result;
					}
				}
				else
				{
					if (messageType == (Message.MessageType)2077219214U)
					{
						goto IL_450;
					}
					if (messageType != (Message.MessageType)2139314275U)
					{
						return result;
					}
					goto IL_3ED;
				}
				return new MessageWithLivestreamingStartResult(messageHandle);
				IL_423:
				return new MessageWithNetSyncSetSessionPropertyResult(messageHandle);
			}
			if (messageType <= (Message.MessageType)645772532U)
			{
				if (messageType <= (Message.MessageType)359268021U)
				{
					if (messageType <= (Message.MessageType)271557598U)
					{
						if (messageType != (Message.MessageType)65065289U)
						{
							if (messageType == (Message.MessageType)191729014U)
							{
								return new MessageWithLivestreamingApplicationStatus(messageHandle);
							}
							if (messageType != (Message.MessageType)271557598U)
							{
								return result;
							}
							goto IL_450;
						}
					}
					else if (messageType <= (Message.MessageType)292822787U)
					{
						if (messageType == (Message.MessageType)288016919U)
						{
							goto IL_42C;
						}
						if (messageType != (Message.MessageType)292822787U)
						{
							return result;
						}
						goto IL_450;
					}
					else
					{
						if (messageType == (Message.MessageType)303837564U)
						{
							return new MessageWithUserCapabilityList(messageHandle);
						}
						if (messageType != (Message.MessageType)359268021U)
						{
							return result;
						}
						goto IL_411;
					}
				}
				else if (messageType <= (Message.MessageType)432190251U)
				{
					if (messageType != (Message.MessageType)408048078U && messageType != (Message.MessageType)409847005U)
					{
						if (messageType != (Message.MessageType)432190251U)
						{
							return result;
						}
						goto IL_462;
					}
				}
				else if (messageType <= (Message.MessageType)475495815U)
				{
					if (messageType == (Message.MessageType)450042703U)
					{
						goto IL_447;
					}
					if (messageType != (Message.MessageType)475495815U)
					{
						return result;
					}
				}
				else
				{
					if (messageType == (Message.MessageType)517416647U)
					{
						goto IL_450;
					}
					if (messageType != (Message.MessageType)645772532U)
					{
						return result;
					}
				}
			}
			else if (messageType <= (Message.MessageType)878018226U)
			{
				if (messageType <= (Message.MessageType)822018158U)
				{
					if (messageType != (Message.MessageType)661065560U && messageType != (Message.MessageType)766496213U)
					{
						if (messageType != (Message.MessageType)822018158U)
						{
							return result;
						}
						goto IL_450;
					}
				}
				else if (messageType <= (Message.MessageType)848430801U)
				{
					if (messageType != (Message.MessageType)840263277U && messageType != (Message.MessageType)848430801U)
					{
						return result;
					}
				}
				else if (messageType != (Message.MessageType)855832432U)
				{
					if (messageType != (Message.MessageType)878018226U)
					{
						return result;
					}
					goto IL_450;
				}
			}
			else if (messageType <= (Message.MessageType)1050353505U)
			{
				if (messageType <= (Message.MessageType)901104867U)
				{
					if (messageType != (Message.MessageType)882366454U)
					{
						if (messageType != (Message.MessageType)901104867U)
						{
							return result;
						}
						goto IL_447;
					}
				}
				else
				{
					if (messageType == (Message.MessageType)921194380U)
					{
						goto IL_450;
					}
					if (messageType != (Message.MessageType)1050353505U)
					{
						return result;
					}
				}
			}
			else if (messageType <= (Message.MessageType)1286683246U)
			{
				if (messageType == (Message.MessageType)1155796426U)
				{
					return new MessageWithLivestreamingVideoStats(messageHandle);
				}
				if (messageType != (Message.MessageType)1286683246U)
				{
					return result;
				}
				goto IL_3ED;
			}
			else
			{
				if (messageType == (Message.MessageType)1317133401U)
				{
					goto IL_450;
				}
				if (messageType != (Message.MessageType)1317270237U)
				{
					return result;
				}
			}
			IL_3D8:
			return new Message(messageHandle);
			IL_3ED:
			return new MessageWithLaunchReportFlowResult(messageHandle);
			IL_411:
			return new MessageWithNetSyncConnection(messageHandle);
			IL_42C:
			return new MessageWithNetSyncVoipAttenuationValueList(messageHandle);
			IL_447:
			return new MessageWithPartyID(messageHandle);
			IL_450:
			return new MessageWithString(messageHandle);
			IL_462:
			result = new MessageWithUserReportID(messageHandle);
			return result;
		}

		public static Request<PlatformInitialize> InitializeStandaloneAsync(ulong appID, string accessToken)
		{
			Request<PlatformInitialize> request = new StandalonePlatform().AsyncInitialize(appID, accessToken);
			if (request == null)
			{
				throw new UnityException("Oculus Platform failed to initialize.");
			}
			Core.ForceInitialized();
			new GameObject("Oculus.Platform.CallbackRunner").AddComponent<CallbackRunner>();
			return request;
		}

		public enum MessageTypeInternal : uint
		{
			AbuseReport_LaunchAdvancedReportFlow = 1286683246U,
			Application_ExecuteCoordinatedLaunch = 645772532U,
			Application_GetInstalledApplications = 1376744524U,
			Avatar_UpdateMetaData = 2077219214U,
			Cal_FinalizeApplication = 497667029U,
			Cal_GetSuggestedApplications = 1450209301U,
			Cal_ProposeApplication = 1317270237U,
			Colocation_GetCurrentMapUuid = 878018226U,
			Colocation_RequestMap = 840263277U,
			Colocation_ShareMap = 409847005U,
			DeviceApplicationIntegrity_GetAttestationToken = 271557598U,
			GraphAPI_Get = 822018158U,
			GraphAPI_Post = 1990567876U,
			HTTP_Get = 1874211363U,
			HTTP_GetToFile = 1317133401U,
			HTTP_MultiPartPost = 1480774160U,
			HTTP_Post = 1798743375U,
			Livestreaming_IsAllowedForApplication = 191729014U,
			Livestreaming_StartPartyStream = 2066701532U,
			Livestreaming_StartStream = 1343932350U,
			Livestreaming_StopPartyStream = 661065560U,
			Livestreaming_StopStream = 1155796426U,
			Livestreaming_UpdateMicStatus = 475495815U,
			NetSync_Connect = 1684899167U,
			NetSync_Disconnect = 359268021U,
			NetSync_GetSessions = 1859521077U,
			NetSync_GetVoipAttenuation = 288016919U,
			NetSync_GetVoipAttenuationDefault = 1467721888U,
			NetSync_SetVoipAttenuation = 882366454U,
			NetSync_SetVoipAttenuationModel = 1788128654U,
			NetSync_SetVoipChannelCfg = 1553310963U,
			NetSync_SetVoipGroup = 1477614734U,
			NetSync_SetVoipListentoChannels = 1590749746U,
			NetSync_SetVoipMicSource = 855832432U,
			NetSync_SetVoipSessionMuted = 1434844938U,
			NetSync_SetVoipSpeaktoChannels = 766496213U,
			NetSync_SetVoipStreamMode = 1742839095U,
			Party_Create = 450042703U,
			Party_GatherInApplication = 1921499523U,
			Party_Get = 1586058173U,
			Party_GetCurrentForUser = 1489764138U,
			Party_Invite = 901104867U,
			Party_Join = 1744993395U,
			Party_Leave = 848430801U,
			RichPresence_SetDestination = 1328734477U,
			RichPresence_SetIsJoinable = 1050353505U,
			RichPresence_SetLobbySession = 1895893271U,
			RichPresence_SetMatchSession = 1675623566U,
			Room_CreateOrUpdateAndJoinNamed = 2089683601U,
			Room_GetNamedRooms = 125660812U,
			Room_GetSocialRooms = 1636310390U,
			User_CancelRecordingForReportFlow = 65065289U,
			User_GetUserCapabilities = 303837564U,
			User_LaunchReportFlow = 1449304081U,
			User_LaunchReportFlow2 = 2139314275U,
			User_NewEntitledTestUser = 292822787U,
			User_NewTestUser = 921194380U,
			User_NewTestUserFriends = 517416647U,
			User_StartRecordingForReportFlow = 1819161571U,
			User_StopRecordingAndLaunchReportFlow = 1618513035U,
			User_StopRecordingAndLaunchReportFlow2 = 432190251U,
			User_TestUserCreateDeviceManifest = 1701884605U,
			Voip_ReportAppVoipSessions = 408048078U
		}

		public static class HTTP
		{
			public static void SetHttpTransferUpdateCallback(Message<HttpTransferUpdate>.Callback callback)
			{
				Callback.SetNotificationCallback<HttpTransferUpdate>(Message.MessageType.Notification_HTTP_Transfer, callback);
			}
		}

		public static class Users
		{
			public static Request<LinkedAccountList> GetLinkedAccounts(ServiceProvider[] providers)
			{
				if (Core.IsInitialized())
				{
					UserOptions userOptions = new UserOptions();
					foreach (ServiceProvider value in providers)
					{
						userOptions.AddServiceProvider(value);
					}
					return new Request<LinkedAccountList>(CAPI.ovr_User_GetLinkedAccounts((IntPtr)userOptions));
				}
				return null;
			}
		}
	}
}
