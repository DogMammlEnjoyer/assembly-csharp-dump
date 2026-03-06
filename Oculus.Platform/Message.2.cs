using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public class Message
	{
		public Message(IntPtr c_message)
		{
			this.type = CAPI.ovr_Message_GetType(c_message);
			bool flag = CAPI.ovr_Message_IsError(c_message);
			this.requestID = CAPI.ovr_Message_GetRequestID(c_message);
			if (!flag)
			{
				IntPtr obj = CAPI.ovr_Message_GetNativeMessage(c_message);
				if (CAPI.ovr_Message_IsError(obj))
				{
					IntPtr obj2 = CAPI.ovr_Message_GetError(obj);
					this.error = new Error(CAPI.ovr_Error_GetCode(obj2), CAPI.ovr_Error_GetMessage(obj2), CAPI.ovr_Error_GetHttpCode(obj2));
				}
			}
			if (flag)
			{
				IntPtr obj3 = CAPI.ovr_Message_GetError(c_message);
				this.error = new Error(CAPI.ovr_Error_GetCode(obj3), CAPI.ovr_Error_GetMessage(obj3), CAPI.ovr_Error_GetHttpCode(obj3));
				return;
			}
			if (Core.LogMessages)
			{
				string text = CAPI.ovr_Message_GetString(c_message);
				if (text != null)
				{
					Debug.Log(text);
					return;
				}
				Debug.Log(string.Format("null message string {0}", c_message));
			}
		}

		~Message()
		{
		}

		public Message.MessageType Type
		{
			get
			{
				return this.type;
			}
		}

		public bool IsError
		{
			get
			{
				return this.error != null;
			}
		}

		public ulong RequestID
		{
			get
			{
				return this.requestID;
			}
		}

		public virtual Error GetError()
		{
			return this.error;
		}

		public virtual HttpTransferUpdate GetHttpTransferUpdate()
		{
			return null;
		}

		public virtual PlatformInitialize GetPlatformInitialize()
		{
			return null;
		}

		public virtual AbuseReportRecording GetAbuseReportRecording()
		{
			return null;
		}

		public virtual AchievementDefinitionList GetAchievementDefinitions()
		{
			return null;
		}

		public virtual AchievementProgressList GetAchievementProgressList()
		{
			return null;
		}

		public virtual AchievementUpdate GetAchievementUpdate()
		{
			return null;
		}

		public virtual AppDownloadProgressResult GetAppDownloadProgressResult()
		{
			return null;
		}

		public virtual AppDownloadResult GetAppDownloadResult()
		{
			return null;
		}

		public virtual ApplicationInviteList GetApplicationInviteList()
		{
			return null;
		}

		public virtual ApplicationVersion GetApplicationVersion()
		{
			return null;
		}

		public virtual AssetDetails GetAssetDetails()
		{
			return null;
		}

		public virtual AssetDetailsList GetAssetDetailsList()
		{
			return null;
		}

		public virtual AssetFileDeleteResult GetAssetFileDeleteResult()
		{
			return null;
		}

		public virtual AssetFileDownloadCancelResult GetAssetFileDownloadCancelResult()
		{
			return null;
		}

		public virtual AssetFileDownloadResult GetAssetFileDownloadResult()
		{
			return null;
		}

		public virtual AssetFileDownloadUpdate GetAssetFileDownloadUpdate()
		{
			return null;
		}

		public virtual AvatarEditorResult GetAvatarEditorResult()
		{
			return null;
		}

		public virtual BlockedUserList GetBlockedUserList()
		{
			return null;
		}

		public virtual Challenge GetChallenge()
		{
			return null;
		}

		public virtual ChallengeEntryList GetChallengeEntryList()
		{
			return null;
		}

		public virtual ChallengeList GetChallengeList()
		{
			return null;
		}

		public virtual CowatchingState GetCowatchingState()
		{
			return null;
		}

		public virtual CowatchViewerList GetCowatchViewerList()
		{
			return null;
		}

		public virtual CowatchViewerUpdate GetCowatchViewerUpdate()
		{
			return null;
		}

		public virtual DestinationList GetDestinationList()
		{
			return null;
		}

		public virtual GroupPresenceJoinIntent GetGroupPresenceJoinIntent()
		{
			return null;
		}

		public virtual GroupPresenceLeaveIntent GetGroupPresenceLeaveIntent()
		{
			return null;
		}

		public virtual InstalledApplicationList GetInstalledApplicationList()
		{
			return null;
		}

		public virtual InvitePanelResultInfo GetInvitePanelResultInfo()
		{
			return null;
		}

		public virtual LaunchBlockFlowResult GetLaunchBlockFlowResult()
		{
			return null;
		}

		public virtual LaunchFriendRequestFlowResult GetLaunchFriendRequestFlowResult()
		{
			return null;
		}

		public virtual LaunchInvitePanelFlowResult GetLaunchInvitePanelFlowResult()
		{
			return null;
		}

		public virtual LaunchReportFlowResult GetLaunchReportFlowResult()
		{
			return null;
		}

		public virtual LaunchUnblockFlowResult GetLaunchUnblockFlowResult()
		{
			return null;
		}

		public virtual bool GetLeaderboardDidUpdate()
		{
			return false;
		}

		public virtual LeaderboardEntryList GetLeaderboardEntryList()
		{
			return null;
		}

		public virtual LeaderboardList GetLeaderboardList()
		{
			return null;
		}

		public virtual LinkedAccountList GetLinkedAccountList()
		{
			return null;
		}

		public virtual LivestreamingApplicationStatus GetLivestreamingApplicationStatus()
		{
			return null;
		}

		public virtual LivestreamingStartResult GetLivestreamingStartResult()
		{
			return null;
		}

		public virtual LivestreamingStatus GetLivestreamingStatus()
		{
			return null;
		}

		public virtual LivestreamingVideoStats GetLivestreamingVideoStats()
		{
			return null;
		}

		public virtual MicrophoneAvailabilityState GetMicrophoneAvailabilityState()
		{
			return null;
		}

		public virtual NetSyncConnection GetNetSyncConnection()
		{
			return null;
		}

		public virtual NetSyncSessionList GetNetSyncSessionList()
		{
			return null;
		}

		public virtual NetSyncSessionsChangedNotification GetNetSyncSessionsChangedNotification()
		{
			return null;
		}

		public virtual NetSyncSetSessionPropertyResult GetNetSyncSetSessionPropertyResult()
		{
			return null;
		}

		public virtual NetSyncVoipAttenuationValueList GetNetSyncVoipAttenuationValueList()
		{
			return null;
		}

		public virtual OrgScopedID GetOrgScopedID()
		{
			return null;
		}

		public virtual Party GetParty()
		{
			return null;
		}

		public virtual PartyID GetPartyID()
		{
			return null;
		}

		public virtual PartyUpdateNotification GetPartyUpdateNotification()
		{
			return null;
		}

		public virtual PidList GetPidList()
		{
			return null;
		}

		public virtual ProductList GetProductList()
		{
			return null;
		}

		public virtual Purchase GetPurchase()
		{
			return null;
		}

		public virtual PurchaseList GetPurchaseList()
		{
			return null;
		}

		public virtual PushNotificationResult GetPushNotificationResult()
		{
			return null;
		}

		public virtual RejoinDialogResult GetRejoinDialogResult()
		{
			return null;
		}

		public virtual SdkAccountList GetSdkAccountList()
		{
			return null;
		}

		public virtual SendInvitesResult GetSendInvitesResult()
		{
			return null;
		}

		public virtual ShareMediaResult GetShareMediaResult()
		{
			return null;
		}

		public virtual string GetString()
		{
			return null;
		}

		public virtual SystemVoipState GetSystemVoipState()
		{
			return null;
		}

		public virtual User GetUser()
		{
			return null;
		}

		public virtual UserAccountAgeCategory GetUserAccountAgeCategory()
		{
			return null;
		}

		public virtual UserCapabilityList GetUserCapabilityList()
		{
			return null;
		}

		public virtual UserList GetUserList()
		{
			return null;
		}

		public virtual UserProof GetUserProof()
		{
			return null;
		}

		public virtual UserReportID GetUserReportID()
		{
			return null;
		}

		internal static Message ParseMessageHandle(IntPtr messageHandle)
		{
			if (messageHandle.ToInt64() == 0L)
			{
				return null;
			}
			Message.MessageType messageType = CAPI.ovr_Message_GetType(messageHandle);
			if (messageType > Message.MessageType.Challenges_GetList)
			{
				if (messageType <= Message.MessageType.Leaderboard_GetEntries)
				{
					if (messageType <= Message.MessageType.Leaderboard_GetNextEntries)
					{
						if (messageType <= Message.MessageType.Leaderboard_GetPreviousEntries)
						{
							if (messageType <= Message.MessageType.Voip_SetSystemVoipSuppressed)
							{
								if (messageType <= Message.MessageType.AssetFile_DownloadCancelByName)
								{
									if (messageType == Message.MessageType.User_GetLoggedInUser)
									{
										goto IL_C47;
									}
									if (messageType != Message.MessageType.AssetFile_DownloadCancelByName)
									{
										goto IL_C8F;
									}
									goto IL_A58;
								}
								else
								{
									if (messageType == Message.MessageType.Application_StartAppDownload)
									{
										goto IL_A10;
									}
									if (messageType != Message.MessageType.Voip_SetSystemVoipSuppressed)
									{
										goto IL_C8F;
									}
									return new MessageWithSystemVoipState(messageHandle);
								}
							}
							else if (messageType <= Message.MessageType.IAP_GetNextPurchaseArrayPage)
							{
								if (messageType == Message.MessageType.Notification_GroupPresence_LeaveIntentReceived)
								{
									return new MessageWithGroupPresenceLeaveIntent(messageHandle);
								}
								if (messageType != Message.MessageType.IAP_GetNextPurchaseArrayPage)
								{
									goto IL_C8F;
								}
								goto IL_BF0;
							}
							else
							{
								if (messageType == Message.MessageType.Party_GetCurrent)
								{
									return new MessageWithPartyUnderCurrentParty(messageHandle);
								}
								if (messageType == Message.MessageType.GroupPresence_SetLobbySession)
								{
									goto IL_AE8;
								}
								if (messageType != Message.MessageType.Leaderboard_GetPreviousEntries)
								{
									goto IL_C8F;
								}
								goto IL_B54;
							}
						}
						else if (messageType <= Message.MessageType.Cowatching_ResignFromPresenting)
						{
							if (messageType <= Message.MessageType.Notification_Cowatching_SessionStopped)
							{
								if (messageType != Message.MessageType.Cowatching_GetPresenterData && messageType != Message.MessageType.Notification_Cowatching_SessionStopped)
								{
									goto IL_C8F;
								}
								goto IL_C35;
							}
							else
							{
								if (messageType == Message.MessageType.AssetFile_GetList)
								{
									return new MessageWithAssetDetailsList(messageHandle);
								}
								if (messageType != Message.MessageType.Cowatching_ResignFromPresenting)
								{
									goto IL_C8F;
								}
								goto IL_AE8;
							}
						}
						else if (messageType <= Message.MessageType.GroupPresence_SetDestination)
						{
							if (messageType != Message.MessageType.AbuseReport_ReportRequestHandled && messageType != Message.MessageType.GroupPresence_SetDestination)
							{
								goto IL_C8F;
							}
							goto IL_AE8;
						}
						else
						{
							if (messageType == Message.MessageType.ApplicationLifecycle_RegisterSessionKey)
							{
								goto IL_AE8;
							}
							if (messageType == Message.MessageType.Notification_Cowatching_PresenterDataChanged)
							{
								goto IL_C35;
							}
							if (messageType != Message.MessageType.Leaderboard_GetNextEntries)
							{
								goto IL_C8F;
							}
							goto IL_B54;
						}
					}
					else if (messageType <= Message.MessageType.RichPresence_Clear)
					{
						if (messageType <= Message.MessageType.GroupPresence_SetDeeplinkMessageOverride)
						{
							if (messageType <= Message.MessageType.AssetFile_DownloadCancelById)
							{
								if (messageType == Message.MessageType.Achievements_GetAllProgress)
								{
									goto IL_9EC;
								}
								if (messageType != Message.MessageType.AssetFile_DownloadCancelById)
								{
									goto IL_C8F;
								}
								goto IL_A58;
							}
							else
							{
								if (messageType == Message.MessageType.Platform_InitializeStandaloneOculus)
								{
									goto IL_C86;
								}
								if (messageType != Message.MessageType.GroupPresence_SetDeeplinkMessageOverride)
								{
									goto IL_C8F;
								}
								goto IL_AE8;
							}
						}
						else if (messageType <= Message.MessageType.Application_CheckAppDownloadProgress)
						{
							if (messageType == Message.MessageType.Application_LaunchOtherApp)
							{
								goto IL_C35;
							}
							if (messageType != Message.MessageType.Application_CheckAppDownloadProgress)
							{
								goto IL_C8F;
							}
							return new MessageWithAppDownloadProgressResult(messageHandle);
						}
						else
						{
							if (messageType == Message.MessageType.Challenges_DeclineInvite)
							{
								goto IL_A94;
							}
							if (messageType == Message.MessageType.User_GetLinkedAccounts)
							{
								return new MessageWithLinkedAccountList(messageHandle);
							}
							if (messageType != Message.MessageType.RichPresence_Clear)
							{
								goto IL_C8F;
							}
							goto IL_AE8;
						}
					}
					else if (messageType <= Message.MessageType.AssetFile_DeleteById)
					{
						if (messageType <= Message.MessageType.User_GetLoggedInUserFriends)
						{
							if (messageType != Message.MessageType.RichPresence_GetDestinations)
							{
								if (messageType != Message.MessageType.User_GetLoggedInUserFriends)
								{
									goto IL_C8F;
								}
								goto IL_C59;
							}
						}
						else
						{
							if (messageType == Message.MessageType.Notification_Voip_SystemVoipState)
							{
								return new MessageWithSystemVoipState(messageHandle);
							}
							if (messageType == Message.MessageType.Achievements_Unlock)
							{
								goto IL_9F8;
							}
							if (messageType != Message.MessageType.AssetFile_DeleteById)
							{
								goto IL_C8F;
							}
							goto IL_A4C;
						}
					}
					else if (messageType <= Message.MessageType.Challenges_GetNextChallenges)
					{
						if (messageType == Message.MessageType.LanguagePack_SetCurrent)
						{
							goto IL_A64;
						}
						if (messageType != Message.MessageType.Challenges_GetNextChallenges)
						{
							goto IL_C8F;
						}
						goto IL_AA0;
					}
					else
					{
						if (messageType == Message.MessageType.Cowatching_GetViewersData)
						{
							goto IL_AB8;
						}
						if (messageType == Message.MessageType.AssetFile_StatusById)
						{
							goto IL_A34;
						}
						if (messageType != Message.MessageType.Leaderboard_GetEntries)
						{
							goto IL_C8F;
						}
						goto IL_B54;
					}
				}
				else if (messageType <= Message.MessageType.Platform_InitializeWindowsAsynchronous)
				{
					if (messageType <= Message.MessageType.User_GetSdkAccounts)
					{
						if (messageType <= Message.MessageType.Cowatching_JoinSession)
						{
							if (messageType <= Message.MessageType.AssetFile_DownloadByName)
							{
								if (messageType == Message.MessageType.Achievements_GetDefinitionsByName)
								{
									goto IL_9E0;
								}
								if (messageType != Message.MessageType.AssetFile_DownloadByName)
								{
									goto IL_C8F;
								}
								goto IL_A64;
							}
							else
							{
								if (messageType == Message.MessageType.IAP_GetViewerPurchasesDurableCache)
								{
									goto IL_BF0;
								}
								if (messageType != Message.MessageType.Cowatching_JoinSession)
								{
									goto IL_C8F;
								}
								goto IL_AE8;
							}
						}
						else if (messageType <= Message.MessageType.Notification_Cowatching_ApiNotReady)
						{
							if (messageType == Message.MessageType.Cowatching_IsInSession)
							{
								goto IL_AD0;
							}
							if (messageType != Message.MessageType.Notification_Cowatching_ApiNotReady)
							{
								goto IL_C8F;
							}
							goto IL_C35;
						}
						else
						{
							if (messageType == Message.MessageType.PushNotification_Register)
							{
								return new MessageWithPushNotificationResult(messageHandle);
							}
							if (messageType != Message.MessageType.RichPresence_GetNextDestinationArrayPage)
							{
								if (messageType != Message.MessageType.User_GetSdkAccounts)
								{
									goto IL_C8F;
								}
								return new MessageWithSdkAccountList(messageHandle);
							}
						}
					}
					else if (messageType <= Message.MessageType.Notification_Cowatching_ViewersDataChanged)
					{
						if (messageType <= Message.MessageType.Notification_GroupPresence_InvitationsSent)
						{
							if (messageType == Message.MessageType.GroupPresence_Set)
							{
								goto IL_AE8;
							}
							if (messageType != Message.MessageType.Notification_GroupPresence_InvitationsSent)
							{
								goto IL_C8F;
							}
							return new MessageWithLaunchInvitePanelFlowResult(messageHandle);
						}
						else
						{
							if (messageType == Message.MessageType.Challenges_Create)
							{
								goto IL_A94;
							}
							if (messageType == Message.MessageType.Application_GetVersion)
							{
								return new MessageWithApplicationVersion(messageHandle);
							}
							if (messageType != Message.MessageType.Notification_Cowatching_ViewersDataChanged)
							{
								goto IL_C8F;
							}
							return new MessageWithCowatchViewerUpdate(messageHandle);
						}
					}
					else if (messageType <= Message.MessageType.User_Get)
					{
						if (messageType == Message.MessageType.Leaderboard_Get)
						{
							goto IL_B48;
						}
						if (messageType != Message.MessageType.User_Get)
						{
							goto IL_C8F;
						}
						goto IL_C47;
					}
					else
					{
						if (messageType == Message.MessageType.Cowatching_SetPresenterData)
						{
							goto IL_AE8;
						}
						if (messageType == Message.MessageType.AssetFile_Delete)
						{
							goto IL_A4C;
						}
						if (messageType != Message.MessageType.Platform_InitializeWindowsAsynchronous)
						{
							goto IL_C8F;
						}
						goto IL_C86;
					}
				}
				else
				{
					if (messageType > Message.MessageType.Notification_Cowatching_Initialized)
					{
						if (messageType <= Message.MessageType.User_GetNextBlockedUserArrayPage)
						{
							if (messageType <= Message.MessageType.Challenges_Get)
							{
								if (messageType == Message.MessageType.Notification_GroupPresence_JoinIntentReceived)
								{
									return new MessageWithGroupPresenceJoinIntent(messageHandle);
								}
								if (messageType != Message.MessageType.Challenges_Get)
								{
									goto IL_C8F;
								}
								goto IL_A94;
							}
							else
							{
								if (messageType == Message.MessageType.Challenges_GetPreviousEntries)
								{
									goto IL_AAC;
								}
								if (messageType == Message.MessageType.Application_CancelAppDownload)
								{
									goto IL_A10;
								}
								if (messageType != Message.MessageType.User_GetNextBlockedUserArrayPage)
								{
									goto IL_C8F;
								}
							}
						}
						else if (messageType <= Message.MessageType.Notification_HTTP_Transfer)
						{
							if (messageType != Message.MessageType.User_GetBlockedUsers)
							{
								if (messageType != Message.MessageType.Notification_HTTP_Transfer)
								{
									goto IL_C8F;
								}
								return new MessageWithHttpTransferUpdate(messageHandle);
							}
						}
						else
						{
							if (messageType == Message.MessageType.IAP_GetProductsBySKU)
							{
								goto IL_BD8;
							}
							if (messageType == Message.MessageType.Challenges_GetNextEntries)
							{
								goto IL_AAC;
							}
							if (messageType != Message.MessageType.Cowatching_RequestToPresent)
							{
								goto IL_C8F;
							}
							goto IL_AE8;
						}
						return new MessageWithBlockedUserList(messageHandle);
					}
					if (messageType <= Message.MessageType.User_GetLoggedInUserManagedInfo)
					{
						if (messageType <= Message.MessageType.Notification_Vrcamera_GetDataChannelMessageUpdate)
						{
							if (messageType == Message.MessageType.GroupPresence_Clear)
							{
								goto IL_AE8;
							}
							if (messageType != Message.MessageType.Notification_Vrcamera_GetDataChannelMessageUpdate)
							{
								goto IL_C8F;
							}
							goto IL_C35;
						}
						else
						{
							if (messageType == Message.MessageType.User_LaunchBlockFlow)
							{
								return new MessageWithLaunchBlockFlowResult(messageHandle);
							}
							if (messageType != Message.MessageType.User_GetLoggedInUserManagedInfo)
							{
								goto IL_C8F;
							}
							goto IL_C47;
						}
					}
					else if (messageType <= Message.MessageType.Leaderboard_WriteEntryWithSupplementaryMetric)
					{
						if (messageType == Message.MessageType.Notification_MarkAsRead)
						{
							goto IL_AE8;
						}
						if (messageType != Message.MessageType.Leaderboard_WriteEntryWithSupplementaryMetric)
						{
							goto IL_C8F;
						}
						goto IL_B60;
					}
					else
					{
						if (messageType == Message.MessageType.Notification_Cowatching_SessionStarted)
						{
							goto IL_C35;
						}
						if (messageType == Message.MessageType.Voip_GetMicrophoneAvailability)
						{
							return new MessageWithMicrophoneAvailabilityState(messageHandle);
						}
						if (messageType != Message.MessageType.Notification_Cowatching_Initialized)
						{
							goto IL_C8F;
						}
						goto IL_C35;
					}
				}
				return new MessageWithDestinationList(messageHandle);
				IL_C47:
				return new MessageWithUser(messageHandle);
			}
			if (messageType <= Message.MessageType.IAP_ConsumePurchase)
			{
				if (messageType <= Message.MessageType.Challenges_GetPreviousChallenges)
				{
					if (messageType <= Message.MessageType.User_GetAccessToken)
					{
						if (messageType <= Message.MessageType.Achievements_AddCount)
						{
							if (messageType <= Message.MessageType.AssetFile_Status)
							{
								if (messageType == Message.MessageType.Media_ShareToFacebook)
								{
									return new MessageWithShareMediaResult(messageHandle);
								}
								if (messageType != Message.MessageType.AssetFile_Status)
								{
									goto IL_C8F;
								}
								goto IL_A34;
							}
							else
							{
								if (messageType == Message.MessageType.Achievements_GetAllDefinitions)
								{
									goto IL_9E0;
								}
								if (messageType != Message.MessageType.Achievements_AddCount)
								{
									goto IL_C8F;
								}
								goto IL_9F8;
							}
						}
						else if (messageType <= Message.MessageType.ApplicationLifecycle_GetRegisteredPIDs)
						{
							if (messageType == Message.MessageType.Notification_ApplicationLifecycle_LaunchIntentChanged)
							{
								goto IL_C35;
							}
							if (messageType != Message.MessageType.ApplicationLifecycle_GetRegisteredPIDs)
							{
								goto IL_C8F;
							}
							return new MessageWithPidList(messageHandle);
						}
						else if (messageType != Message.MessageType.GroupPresence_GetNextApplicationInviteArrayPage)
						{
							if (messageType == Message.MessageType.Avatar_LaunchAvatarEditor)
							{
								return new MessageWithAvatarEditorResult(messageHandle);
							}
							if (messageType != Message.MessageType.User_GetAccessToken)
							{
								goto IL_C8F;
							}
							goto IL_C35;
						}
					}
					else if (messageType <= Message.MessageType.Challenges_GetEntriesAfterRank)
					{
						if (messageType <= Message.MessageType.AssetFile_DownloadCancel)
						{
							if (messageType == Message.MessageType.Notification_NetSync_ConnectionStatusChanged)
							{
								return new MessageWithNetSyncConnection(messageHandle);
							}
							if (messageType != Message.MessageType.AssetFile_DownloadCancel)
							{
								goto IL_C8F;
							}
							goto IL_A58;
						}
						else if (messageType != Message.MessageType.GroupPresence_GetSentInvites)
						{
							if (messageType != Message.MessageType.Challenges_GetEntriesAfterRank)
							{
								goto IL_C8F;
							}
							goto IL_AAC;
						}
					}
					else if (messageType <= Message.MessageType.Notification_Cowatching_ApiReady)
					{
						if (messageType == Message.MessageType.User_LaunchFriendRequestFlow)
						{
							return new MessageWithLaunchFriendRequestFlowResult(messageHandle);
						}
						if (messageType != Message.MessageType.Notification_Cowatching_ApiReady)
						{
							goto IL_C8F;
						}
						goto IL_C35;
					}
					else
					{
						if (messageType == Message.MessageType.GroupPresence_SendInvites)
						{
							return new MessageWithSendInvitesResult(messageHandle);
						}
						if (messageType == Message.MessageType.Notification_Cowatching_InSessionChanged)
						{
							goto IL_AD0;
						}
						if (messageType != Message.MessageType.Challenges_GetPreviousChallenges)
						{
							goto IL_C8F;
						}
						goto IL_AA0;
					}
					return new MessageWithApplicationInviteList(messageHandle);
				}
				if (messageType <= Message.MessageType.Achievements_GetProgressByName)
				{
					if (messageType <= Message.MessageType.Leaderboard_WriteEntry)
					{
						if (messageType <= Message.MessageType.AssetFile_Download)
						{
							if (messageType == Message.MessageType.GroupPresence_LaunchInvitePanel)
							{
								return new MessageWithInvitePanelResultInfo(messageHandle);
							}
							if (messageType != Message.MessageType.AssetFile_Download)
							{
								goto IL_C8F;
							}
							goto IL_A64;
						}
						else
						{
							if (messageType == Message.MessageType.Challenges_UpdateInfo)
							{
								goto IL_A94;
							}
							if (messageType != Message.MessageType.Leaderboard_WriteEntry)
							{
								goto IL_C8F;
							}
							goto IL_B60;
						}
					}
					else if (messageType <= Message.MessageType.Application_InstallAppUpdateAndRelaunch)
					{
						if (messageType == Message.MessageType.Challenges_GetEntries)
						{
							goto IL_AAC;
						}
						if (messageType != Message.MessageType.Application_InstallAppUpdateAndRelaunch)
						{
							goto IL_C8F;
						}
						goto IL_A10;
					}
					else
					{
						if (messageType == Message.MessageType.User_LaunchUnblockFlow)
						{
							return new MessageWithLaunchUnblockFlowResult(messageHandle);
						}
						if (messageType == Message.MessageType.Achievements_AddFields)
						{
							goto IL_9F8;
						}
						if (messageType != Message.MessageType.Achievements_GetProgressByName)
						{
							goto IL_C8F;
						}
						goto IL_9EC;
					}
				}
				else if (messageType <= Message.MessageType.Platform_InitializeAndroidAsynchronous)
				{
					if (messageType <= Message.MessageType.Leaderboard_GetEntriesAfterRank)
					{
						if (messageType == Message.MessageType.GroupPresence_LaunchRejoinDialog)
						{
							return new MessageWithRejoinDialogResult(messageHandle);
						}
						if (messageType != Message.MessageType.Leaderboard_GetEntriesAfterRank)
						{
							goto IL_C8F;
						}
						goto IL_B54;
					}
					else
					{
						if (messageType == Message.MessageType.Entitlement_GetIsViewerEntitled)
						{
							goto IL_AE8;
						}
						if (messageType == Message.MessageType.User_GetOrgScopedID)
						{
							return new MessageWithOrgScopedID(messageHandle);
						}
						if (messageType != Message.MessageType.Platform_InitializeAndroidAsynchronous)
						{
							goto IL_C8F;
						}
						goto IL_C86;
					}
				}
				else if (messageType <= Message.MessageType.Notification_Party_PartyUpdate)
				{
					if (messageType == Message.MessageType.IAP_GetNextProductArrayPage)
					{
						goto IL_BD8;
					}
					if (messageType != Message.MessageType.Notification_Party_PartyUpdate)
					{
						goto IL_C8F;
					}
					return new MessageWithPartyUpdateNotification(messageHandle);
				}
				else
				{
					if (messageType == Message.MessageType.Cowatching_GetNextCowatchViewerArrayPage)
					{
						goto IL_AB8;
					}
					if (messageType == Message.MessageType.LanguagePack_GetCurrent)
					{
						goto IL_A34;
					}
					if (messageType != Message.MessageType.IAP_ConsumePurchase)
					{
						goto IL_C8F;
					}
					goto IL_AE8;
				}
			}
			else if (messageType <= Message.MessageType.Notification_AssetFile_DownloadUpdate)
			{
				if (messageType <= Message.MessageType.Challenges_Delete)
				{
					if (messageType <= Message.MessageType.User_GetUserProof)
					{
						if (messageType <= Message.MessageType.UserAgeCategory_Get)
						{
							if (messageType == Message.MessageType.Challenges_Join)
							{
								goto IL_A94;
							}
							if (messageType != Message.MessageType.UserAgeCategory_Get)
							{
								goto IL_C8F;
							}
							return new MessageWithUserAccountAgeCategory(messageHandle);
						}
						else
						{
							if (messageType == Message.MessageType.Notification_Livestreaming_StatusChange)
							{
								return new MessageWithLivestreamingStatus(messageHandle);
							}
							if (messageType != Message.MessageType.User_GetUserProof)
							{
								goto IL_C8F;
							}
							return new MessageWithUserProof(messageHandle);
						}
					}
					else if (messageType <= Message.MessageType.User_GetNextUserCapabilityArrayPage)
					{
						if (messageType == Message.MessageType.Cowatching_LaunchInviteDialog)
						{
							goto IL_AE8;
						}
						if (messageType != Message.MessageType.User_GetNextUserCapabilityArrayPage)
						{
							goto IL_C8F;
						}
						return new MessageWithUserCapabilityList(messageHandle);
					}
					else
					{
						if (messageType == Message.MessageType.GroupPresence_GetInvitableUsers)
						{
							goto IL_C59;
						}
						if (messageType == Message.MessageType.Notification_AbuseReport_ReportButtonPressed)
						{
							goto IL_C35;
						}
						if (messageType != Message.MessageType.Challenges_Delete)
						{
							goto IL_C8F;
						}
						goto IL_AE8;
					}
				}
				else if (messageType <= Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage)
				{
					if (messageType <= Message.MessageType.GroupPresence_LaunchMultiplayerErrorDialog)
					{
						if (messageType == Message.MessageType.User_GetNextUserArrayPage)
						{
							goto IL_C59;
						}
						if (messageType != Message.MessageType.GroupPresence_LaunchMultiplayerErrorDialog)
						{
							goto IL_C8F;
						}
						goto IL_AE8;
					}
					else
					{
						if (messageType == Message.MessageType.Challenges_Leave)
						{
							goto IL_A94;
						}
						if (messageType != Message.MessageType.Achievements_GetNextAchievementDefinitionArrayPage)
						{
							goto IL_C8F;
						}
					}
				}
				else if (messageType <= Message.MessageType.AssetFile_DownloadById)
				{
					if (messageType == Message.MessageType.GroupPresence_SetIsJoinable)
					{
						goto IL_AE8;
					}
					if (messageType != Message.MessageType.AssetFile_DownloadById)
					{
						goto IL_C8F;
					}
					goto IL_A64;
				}
				else
				{
					if (messageType == Message.MessageType.UserAgeCategory_Report)
					{
						goto IL_AE8;
					}
					if (messageType == Message.MessageType.Achievements_GetNextAchievementProgressArrayPage)
					{
						goto IL_9EC;
					}
					if (messageType != Message.MessageType.Notification_AssetFile_DownloadUpdate)
					{
						goto IL_C8F;
					}
					return new MessageWithAssetFileDownloadUpdate(messageHandle);
				}
			}
			else if (messageType <= Message.MessageType.Leaderboard_GetEntriesByIds)
			{
				if (messageType <= Message.MessageType.Platform_InitializeWithAccessToken)
				{
					if (messageType <= Message.MessageType.Challenges_GetEntriesByIds)
					{
						if (messageType == Message.MessageType.GroupPresence_SetMatchSession)
						{
							goto IL_AE8;
						}
						if (messageType != Message.MessageType.Challenges_GetEntriesByIds)
						{
							goto IL_C8F;
						}
						goto IL_AAC;
					}
					else
					{
						if (messageType == Message.MessageType.DeviceApplicationIntegrity_GetIntegrityToken)
						{
							goto IL_C35;
						}
						if (messageType != Message.MessageType.Platform_InitializeWithAccessToken)
						{
							goto IL_C8F;
						}
						goto IL_C86;
					}
				}
				else if (messageType <= Message.MessageType.Leaderboard_GetNextLeaderboardArrayPage)
				{
					if (messageType == Message.MessageType.GroupPresence_LaunchRosterPanel)
					{
						goto IL_AE8;
					}
					if (messageType != Message.MessageType.Leaderboard_GetNextLeaderboardArrayPage)
					{
						goto IL_C8F;
					}
					goto IL_B48;
				}
				else
				{
					if (messageType == Message.MessageType.Notification_Vrcamera_GetSurfaceUpdate)
					{
						goto IL_C35;
					}
					if (messageType == Message.MessageType.Notification_NetSync_SessionsChanged)
					{
						return new MessageWithNetSyncSessionsChangedNotification(messageHandle);
					}
					if (messageType != Message.MessageType.Leaderboard_GetEntriesByIds)
					{
						goto IL_C8F;
					}
					goto IL_B54;
				}
			}
			else if (messageType <= Message.MessageType.Cowatching_SetViewerData)
			{
				if (messageType <= Message.MessageType.ApplicationLifecycle_GetSessionKey)
				{
					if (messageType == Message.MessageType.IAP_GetViewerPurchases)
					{
						goto IL_BF0;
					}
					if (messageType != Message.MessageType.ApplicationLifecycle_GetSessionKey)
					{
						goto IL_C8F;
					}
					goto IL_C35;
				}
				else
				{
					if (messageType != Message.MessageType.RichPresence_Set && messageType != Message.MessageType.Cowatching_LeaveSession && messageType != Message.MessageType.Cowatching_SetViewerData)
					{
						goto IL_C8F;
					}
					goto IL_AE8;
				}
			}
			else if (messageType <= Message.MessageType.IAP_LaunchCheckoutFlow)
			{
				if (messageType == Message.MessageType.Notification_Voip_MicrophoneAvailabilityStateUpdate)
				{
					goto IL_C35;
				}
				if (messageType != Message.MessageType.IAP_LaunchCheckoutFlow)
				{
					goto IL_C8F;
				}
				return new MessageWithPurchase(messageHandle);
			}
			else
			{
				if (messageType == Message.MessageType.AssetFile_StatusByName)
				{
					goto IL_A34;
				}
				if (messageType == Message.MessageType.AssetFile_DeleteByName)
				{
					goto IL_A4C;
				}
				if (messageType != Message.MessageType.Challenges_GetList)
				{
					goto IL_C8F;
				}
				goto IL_AA0;
			}
			IL_9E0:
			return new MessageWithAchievementDefinitions(messageHandle);
			IL_9EC:
			return new MessageWithAchievementProgressList(messageHandle);
			IL_9F8:
			return new MessageWithAchievementUpdate(messageHandle);
			IL_A10:
			return new MessageWithAppDownloadResult(messageHandle);
			IL_A34:
			return new MessageWithAssetDetails(messageHandle);
			IL_A4C:
			return new MessageWithAssetFileDeleteResult(messageHandle);
			IL_A58:
			return new MessageWithAssetFileDownloadCancelResult(messageHandle);
			IL_A64:
			return new MessageWithAssetFileDownloadResult(messageHandle);
			IL_A94:
			return new MessageWithChallenge(messageHandle);
			IL_AA0:
			return new MessageWithChallengeList(messageHandle);
			IL_AAC:
			return new MessageWithChallengeEntryList(messageHandle);
			IL_AB8:
			return new MessageWithCowatchViewerList(messageHandle);
			IL_AD0:
			return new MessageWithCowatchingState(messageHandle);
			IL_AE8:
			return new Message(messageHandle);
			IL_B48:
			return new MessageWithLeaderboardList(messageHandle);
			IL_B54:
			return new MessageWithLeaderboardEntryList(messageHandle);
			IL_B60:
			return new MessageWithLeaderboardDidUpdate(messageHandle);
			IL_BD8:
			return new MessageWithProductList(messageHandle);
			IL_BF0:
			return new MessageWithPurchaseList(messageHandle);
			IL_C35:
			return new MessageWithString(messageHandle);
			IL_C59:
			return new MessageWithUserList(messageHandle);
			IL_C86:
			return new MessageWithPlatformInitialize(messageHandle);
			IL_C8F:
			Message message = PlatformInternal.ParseMessageHandle(messageHandle, messageType);
			if (message == null)
			{
				Debug.LogError(string.Format("Unrecognized message type {0}\n", messageType));
			}
			return message;
		}

		public static Message PopMessage()
		{
			if (!Core.IsInitialized())
			{
				return null;
			}
			IntPtr intPtr = CAPI.ovr_PopMessage();
			Message result = Message.ParseMessageHandle(intPtr);
			CAPI.ovr_FreeMessage(intPtr);
			return result;
		}

		internal static Message.ExtraMessageTypesHandler HandleExtraMessageTypes { private get; set; }

		private Message.MessageType type;

		private ulong requestID;

		private Error error;

		public delegate void Callback(Message message);

		public enum MessageType : uint
		{
			Unknown,
			AbuseReport_ReportRequestHandled = 1267661958U,
			Achievements_AddCount = 65495601U,
			Achievements_AddFields = 346693929U,
			Achievements_GetAllDefinitions = 64177549U,
			Achievements_GetAllProgress = 1335877149U,
			Achievements_GetDefinitionsByName = 1653670332U,
			Achievements_GetNextAchievementDefinitionArrayPage = 712888917U,
			Achievements_GetNextAchievementProgressArrayPage = 792913703U,
			Achievements_GetProgressByName = 354837425U,
			Achievements_Unlock = 1497156573U,
			ApplicationLifecycle_GetRegisteredPIDs = 82169698U,
			ApplicationLifecycle_GetSessionKey = 984570141U,
			ApplicationLifecycle_RegisterSessionKey = 1303818232U,
			Application_CancelAppDownload = 2082496734U,
			Application_CheckAppDownloadProgress = 1429514532U,
			Application_GetVersion = 1751583246U,
			Application_InstallAppUpdateAndRelaunch = 343960453U,
			Application_LaunchOtherApp = 1424151032U,
			Application_StartAppDownload = 1157365870U,
			AssetFile_Delete = 1834842246U,
			AssetFile_DeleteById = 1525206354U,
			AssetFile_DeleteByName = 1108001231U,
			AssetFile_Download = 289710021U,
			AssetFile_DownloadById = 755009938U,
			AssetFile_DownloadByName = 1664536314U,
			AssetFile_DownloadCancel = 134927303U,
			AssetFile_DownloadCancelById = 1365611796U,
			AssetFile_DownloadCancelByName = 1147858170U,
			AssetFile_GetList = 1258057588U,
			AssetFile_Status = 47394656U,
			AssetFile_StatusById = 1570069816U,
			AssetFile_StatusByName = 1104140880U,
			Avatar_LaunchAvatarEditor = 99737939U,
			Challenges_Create = 1750718017U,
			Challenges_DeclineInvite = 1452177088U,
			Challenges_Delete = 642287050U,
			Challenges_Get = 2002276083U,
			Challenges_GetEntries = 303739999U,
			Challenges_GetEntriesAfterRank = 143202943U,
			Challenges_GetEntriesByIds = 828705244U,
			Challenges_GetList = 1126581078U,
			Challenges_GetNextChallenges = 1534894518U,
			Challenges_GetNextEntries = 2135728326U,
			Challenges_GetPreviousChallenges = 246678541U,
			Challenges_GetPreviousEntries = 2026439792U,
			Challenges_Join = 556040297U,
			Challenges_Leave = 694228709U,
			Challenges_UpdateInfo = 292929120U,
			Cowatching_GetNextCowatchViewerArrayPage = 490748210U,
			Cowatching_GetPresenterData = 1233536821U,
			Cowatching_GetViewersData = 1557635663U,
			Cowatching_IsInSession = 1696286852U,
			Cowatching_JoinSession = 1669899604U,
			Cowatching_LaunchInviteDialog = 580072087U,
			Cowatching_LeaveSession = 1017005773U,
			Cowatching_RequestToPresent = 2138684586U,
			Cowatching_ResignFromPresenting = 1263124994U,
			Cowatching_SetPresenterData = 1830586630U,
			Cowatching_SetViewerData = 1021044774U,
			DeviceApplicationIntegrity_GetIntegrityToken = 846310362U,
			Entitlement_GetIsViewerEntitled = 409688241U,
			GroupPresence_Clear = 1839897795U,
			GroupPresence_GetInvitableUsers = 592167921U,
			GroupPresence_GetNextApplicationInviteArrayPage = 83411186U,
			GroupPresence_GetSentInvites = 136710833U,
			GroupPresence_LaunchInvitePanel = 262066079U,
			GroupPresence_LaunchMultiplayerErrorDialog = 693481252U,
			GroupPresence_LaunchRejoinDialog = 360121199U,
			GroupPresence_LaunchRosterPanel = 896698498U,
			GroupPresence_SendInvites = 231461732U,
			GroupPresence_Set = 1734302756U,
			GroupPresence_SetDeeplinkMessageOverride = 1377492749U,
			GroupPresence_SetDestination = 1281042058U,
			GroupPresence_SetIsJoinable = 714018901U,
			GroupPresence_SetLobbySession = 1224693182U,
			GroupPresence_SetMatchSession = 827098296U,
			IAP_ConsumePurchase = 532378329U,
			IAP_GetNextProductArrayPage = 467225263U,
			IAP_GetNextPurchaseArrayPage = 1196886677U,
			IAP_GetProductsBySKU = 2124073717U,
			IAP_GetViewerPurchases = 974095385U,
			IAP_GetViewerPurchasesDurableCache = 1666817579U,
			IAP_LaunchCheckoutFlow = 1067126029U,
			LanguagePack_GetCurrent = 529592533U,
			LanguagePack_SetCurrent = 1531952096U,
			Leaderboard_Get = 1792298744U,
			Leaderboard_GetEntries = 1572030284U,
			Leaderboard_GetEntriesAfterRank = 406293487U,
			Leaderboard_GetEntriesByIds = 962624508U,
			Leaderboard_GetNextEntries = 1310751961U,
			Leaderboard_GetNextLeaderboardArrayPage = 905344667U,
			Leaderboard_GetPreviousEntries = 1224858304U,
			Leaderboard_WriteEntry = 293587198U,
			Leaderboard_WriteEntryWithSupplementaryMetric = 1925616378U,
			Media_ShareToFacebook = 14912239U,
			Notification_MarkAsRead = 1903319523U,
			Party_GetCurrent = 1200830304U,
			PushNotification_Register = 1715112799U,
			RichPresence_Clear = 1471632051U,
			RichPresence_GetDestinations = 1483681044U,
			RichPresence_GetNextDestinationArrayPage = 1731624773U,
			RichPresence_Set = 1007973641U,
			UserAgeCategory_Get = 567009472U,
			UserAgeCategory_Report = 776853718U,
			User_Get = 1808768583U,
			User_GetAccessToken = 111696574U,
			User_GetBlockedUsers = 2099254614U,
			User_GetLinkedAccounts = 1469314134U,
			User_GetLoggedInUser = 1131361373U,
			User_GetLoggedInUserFriends = 1484532365U,
			User_GetLoggedInUserManagedInfo = 1891252974U,
			User_GetNextBlockedUserArrayPage = 2083192267U,
			User_GetNextUserArrayPage = 645723971U,
			User_GetNextUserCapabilityArrayPage = 587854745U,
			User_GetOrgScopedID = 418426907U,
			User_GetSdkAccounts = 1733454467U,
			User_GetUserProof = 578880643U,
			User_LaunchBlockFlow = 1876305192U,
			User_LaunchFriendRequestFlow = 151303576U,
			User_LaunchUnblockFlow = 346172055U,
			Voip_GetMicrophoneAvailability = 1951195973U,
			Voip_SetSystemVoipSuppressed = 1161808298U,
			Notification_AbuseReport_ReportButtonPressed = 608644972U,
			Notification_ApplicationLifecycle_LaunchIntentChanged = 78859427U,
			Notification_AssetFile_DownloadUpdate = 803015885U,
			Notification_Cowatching_ApiNotReady = 1711880577U,
			Notification_Cowatching_ApiReady = 160786067U,
			Notification_Cowatching_InSessionChanged = 234434835U,
			Notification_Cowatching_Initialized = 1960397043U,
			Notification_Cowatching_PresenterDataChanged = 1309118190U,
			Notification_Cowatching_SessionStarted = 1931580316U,
			Notification_Cowatching_SessionStopped = 1239866362U,
			Notification_Cowatching_ViewersDataChanged = 1760752127U,
			Notification_GroupPresence_InvitationsSent = 1738179766U,
			Notification_GroupPresence_JoinIntentReceived = 2000194038U,
			Notification_GroupPresence_LeaveIntentReceived = 1194846749U,
			Notification_HTTP_Transfer = 2111073839U,
			Notification_Livestreaming_StatusChange = 575101294U,
			Notification_NetSync_ConnectionStatusChanged = 120882378U,
			Notification_NetSync_SessionsChanged = 947814198U,
			Notification_Party_PartyUpdate = 487688882U,
			Notification_Voip_MicrophoneAvailabilityStateUpdate = 1042336599U,
			Notification_Voip_SystemVoipState = 1490179237U,
			Notification_Vrcamera_GetDataChannelMessageUpdate = 1860498236U,
			Notification_Vrcamera_GetSurfaceUpdate = 938610820U,
			Platform_InitializeWithAccessToken = 896085803U,
			Platform_InitializeStandaloneOculus = 1375260172U,
			Platform_InitializeAndroidAsynchronous = 450037684U,
			Platform_InitializeWindowsAsynchronous = 1839708815U
		}

		internal delegate Message ExtraMessageTypesHandler(IntPtr messageHandle, Message.MessageType message_type);
	}
}
