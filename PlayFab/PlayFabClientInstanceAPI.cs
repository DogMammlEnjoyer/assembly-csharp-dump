using System;
using System.Collections.Generic;
using PlayFab.ClientModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabClientInstanceAPI : IPlayFabInstanceApi
	{
		public PlayFabClientInstanceAPI()
		{
			this.authenticationContext = new PlayFabAuthenticationContext();
		}

		public PlayFabClientInstanceAPI(PlayFabApiSettings settings)
		{
			this.apiSettings = settings;
			this.authenticationContext = new PlayFabAuthenticationContext();
		}

		public PlayFabClientInstanceAPI(PlayFabAuthenticationContext context)
		{
			this.authenticationContext = (context ?? new PlayFabAuthenticationContext());
		}

		public PlayFabClientInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
		{
			this.apiSettings = settings;
			this.authenticationContext = (context ?? new PlayFabAuthenticationContext());
		}

		public bool IsClientLoggedIn()
		{
			return this.authenticationContext != null && this.authenticationContext.IsClientLoggedIn();
		}

		public void ForgetAllCredentials()
		{
			if (this.authenticationContext != null)
			{
				this.authenticationContext.ForgetAllCredentials();
			}
		}

		public void AcceptTrade(AcceptTradeRequest request, Action<AcceptTradeResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AcceptTradeResponse>("/Client/AcceptTrade", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddFriend(AddFriendRequest request, Action<AddFriendResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AddFriendResult>("/Client/AddFriend", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddGenericID(AddGenericIDRequest request, Action<AddGenericIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AddGenericIDResult>("/Client/AddGenericID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddOrUpdateContactEmail(AddOrUpdateContactEmailRequest request, Action<AddOrUpdateContactEmailResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AddOrUpdateContactEmailResult>("/Client/AddOrUpdateContactEmail", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddSharedGroupMembers(AddSharedGroupMembersRequest request, Action<AddSharedGroupMembersResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AddSharedGroupMembersResult>("/Client/AddSharedGroupMembers", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddUsernamePassword(AddUsernamePasswordRequest request, Action<AddUsernamePasswordResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AddUsernamePasswordResult>("/Client/AddUsernamePassword", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AddUserVirtualCurrency(AddUserVirtualCurrencyRequest request, Action<ModifyUserVirtualCurrencyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ModifyUserVirtualCurrencyResult>("/Client/AddUserVirtualCurrency", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AndroidDevicePushNotificationRegistration(AndroidDevicePushNotificationRegistrationRequest request, Action<AndroidDevicePushNotificationRegistrationResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AndroidDevicePushNotificationRegistrationResult>("/Client/AndroidDevicePushNotificationRegistration", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void AttributeInstall(AttributeInstallRequest request, Action<AttributeInstallResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AttributeInstallResult>("/Client/AttributeInstall", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void CancelTrade(CancelTradeRequest request, Action<CancelTradeResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CancelTradeResponse>("/Client/CancelTrade", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ConfirmPurchase(ConfirmPurchaseRequest request, Action<ConfirmPurchaseResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ConfirmPurchaseResult>("/Client/ConfirmPurchase", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ConsumeItem(ConsumeItemRequest request, Action<ConsumeItemResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ConsumeItemResult>("/Client/ConsumeItem", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ConsumePSNEntitlements(ConsumePSNEntitlementsRequest request, Action<ConsumePSNEntitlementsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ConsumePSNEntitlementsResult>("/Client/ConsumePSNEntitlements", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ConsumeXboxEntitlements(ConsumeXboxEntitlementsRequest request, Action<ConsumeXboxEntitlementsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ConsumeXboxEntitlementsResult>("/Client/ConsumeXboxEntitlements", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void CreateSharedGroup(CreateSharedGroupRequest request, Action<CreateSharedGroupResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateSharedGroupResult>("/Client/CreateSharedGroup", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ExecuteCloudScript(ExecuteCloudScriptRequest request, Action<ExecuteCloudScriptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ExecuteCloudScriptResult>("/Client/ExecuteCloudScript", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ExecuteCloudScript<TOut>(ExecuteCloudScriptRequest request, Action<ExecuteCloudScriptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			Action<ExecuteCloudScriptResult> resultCallback2 = delegate(ExecuteCloudScriptResult wrappedResult)
			{
				ISerializerPlugin plugin = PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer, "");
				string text = plugin.SerializeObject(wrappedResult.FunctionResult);
				try
				{
					wrappedResult.FunctionResult = plugin.DeserializeObject<TOut>(text);
				}
				catch (Exception)
				{
					wrappedResult.FunctionResult = text;
					wrappedResult.Logs.Add(new LogStatement
					{
						Level = "Warning",
						Data = text,
						Message = "Sdk Message: Could not deserialize result as: " + typeof(TOut).Name
					});
				}
				resultCallback(wrappedResult);
			};
			PlayFabHttp.MakeApiCall<ExecuteCloudScriptResult>("/Client/ExecuteCloudScript", request, AuthType.LoginSession, resultCallback2, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetAccountInfo(GetAccountInfoRequest request, Action<GetAccountInfoResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetAccountInfoResult>("/Client/GetAccountInfo", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetAdPlacements(GetAdPlacementsRequest request, Action<GetAdPlacementsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetAdPlacementsResult>("/Client/GetAdPlacements", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetAllUsersCharacters(ListUsersCharactersRequest request, Action<ListUsersCharactersResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListUsersCharactersResult>("/Client/GetAllUsersCharacters", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCatalogItems(GetCatalogItemsRequest request, Action<GetCatalogItemsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCatalogItemsResult>("/Client/GetCatalogItems", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCharacterData(GetCharacterDataRequest request, Action<GetCharacterDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCharacterDataResult>("/Client/GetCharacterData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCharacterInventory(GetCharacterInventoryRequest request, Action<GetCharacterInventoryResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCharacterInventoryResult>("/Client/GetCharacterInventory", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCharacterLeaderboard(GetCharacterLeaderboardRequest request, Action<GetCharacterLeaderboardResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCharacterLeaderboardResult>("/Client/GetCharacterLeaderboard", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCharacterReadOnlyData(GetCharacterDataRequest request, Action<GetCharacterDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCharacterDataResult>("/Client/GetCharacterReadOnlyData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCharacterStatistics(GetCharacterStatisticsRequest request, Action<GetCharacterStatisticsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetCharacterStatisticsResult>("/Client/GetCharacterStatistics", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetContentDownloadUrl(GetContentDownloadUrlRequest request, Action<GetContentDownloadUrlResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetContentDownloadUrlResult>("/Client/GetContentDownloadUrl", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetCurrentGames(CurrentGamesRequest request, Action<CurrentGamesResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CurrentGamesResult>("/Client/GetCurrentGames", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetFriendLeaderboard(GetFriendLeaderboardRequest request, Action<GetLeaderboardResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetLeaderboardResult>("/Client/GetFriendLeaderboard", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetFriendLeaderboardAroundPlayer(GetFriendLeaderboardAroundPlayerRequest request, Action<GetFriendLeaderboardAroundPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetFriendLeaderboardAroundPlayerResult>("/Client/GetFriendLeaderboardAroundPlayer", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetFriendsList(GetFriendsListRequest request, Action<GetFriendsListResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetFriendsListResult>("/Client/GetFriendsList", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetGameServerRegions(GameServerRegionsRequest request, Action<GameServerRegionsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GameServerRegionsResult>("/Client/GetGameServerRegions", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetLeaderboard(GetLeaderboardRequest request, Action<GetLeaderboardResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetLeaderboardResult>("/Client/GetLeaderboard", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetLeaderboardAroundCharacter(GetLeaderboardAroundCharacterRequest request, Action<GetLeaderboardAroundCharacterResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetLeaderboardAroundCharacterResult>("/Client/GetLeaderboardAroundCharacter", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetLeaderboardAroundPlayer(GetLeaderboardAroundPlayerRequest request, Action<GetLeaderboardAroundPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetLeaderboardAroundPlayerResult>("/Client/GetLeaderboardAroundPlayer", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetLeaderboardForUserCharacters(GetLeaderboardForUsersCharactersRequest request, Action<GetLeaderboardForUsersCharactersResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetLeaderboardForUsersCharactersResult>("/Client/GetLeaderboardForUserCharacters", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPaymentToken(GetPaymentTokenRequest request, Action<GetPaymentTokenResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPaymentTokenResult>("/Client/GetPaymentToken", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPhotonAuthenticationToken(GetPhotonAuthenticationTokenRequest request, Action<GetPhotonAuthenticationTokenResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPhotonAuthenticationTokenResult>("/Client/GetPhotonAuthenticationToken", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerCombinedInfo(GetPlayerCombinedInfoRequest request, Action<GetPlayerCombinedInfoResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerCombinedInfoResult>("/Client/GetPlayerCombinedInfo", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerProfile(GetPlayerProfileRequest request, Action<GetPlayerProfileResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerProfileResult>("/Client/GetPlayerProfile", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerSegments(GetPlayerSegmentsRequest request, Action<GetPlayerSegmentsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerSegmentsResult>("/Client/GetPlayerSegments", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerStatistics(GetPlayerStatisticsRequest request, Action<GetPlayerStatisticsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerStatisticsResult>("/Client/GetPlayerStatistics", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerStatisticVersions(GetPlayerStatisticVersionsRequest request, Action<GetPlayerStatisticVersionsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerStatisticVersionsResult>("/Client/GetPlayerStatisticVersions", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerTags(GetPlayerTagsRequest request, Action<GetPlayerTagsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerTagsResult>("/Client/GetPlayerTags", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayerTrades(GetPlayerTradesRequest request, Action<GetPlayerTradesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayerTradesResponse>("/Client/GetPlayerTrades", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromFacebookIDs(GetPlayFabIDsFromFacebookIDsRequest request, Action<GetPlayFabIDsFromFacebookIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromFacebookIDsResult>("/Client/GetPlayFabIDsFromFacebookIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromFacebookInstantGamesIds(GetPlayFabIDsFromFacebookInstantGamesIdsRequest request, Action<GetPlayFabIDsFromFacebookInstantGamesIdsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromFacebookInstantGamesIdsResult>("/Client/GetPlayFabIDsFromFacebookInstantGamesIds", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromGameCenterIDs(GetPlayFabIDsFromGameCenterIDsRequest request, Action<GetPlayFabIDsFromGameCenterIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromGameCenterIDsResult>("/Client/GetPlayFabIDsFromGameCenterIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromGenericIDs(GetPlayFabIDsFromGenericIDsRequest request, Action<GetPlayFabIDsFromGenericIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromGenericIDsResult>("/Client/GetPlayFabIDsFromGenericIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromGoogleIDs(GetPlayFabIDsFromGoogleIDsRequest request, Action<GetPlayFabIDsFromGoogleIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromGoogleIDsResult>("/Client/GetPlayFabIDsFromGoogleIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromKongregateIDs(GetPlayFabIDsFromKongregateIDsRequest request, Action<GetPlayFabIDsFromKongregateIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromKongregateIDsResult>("/Client/GetPlayFabIDsFromKongregateIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromNintendoSwitchDeviceIds(GetPlayFabIDsFromNintendoSwitchDeviceIdsRequest request, Action<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromNintendoSwitchDeviceIdsResult>("/Client/GetPlayFabIDsFromNintendoSwitchDeviceIds", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromPSNAccountIDs(GetPlayFabIDsFromPSNAccountIDsRequest request, Action<GetPlayFabIDsFromPSNAccountIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromPSNAccountIDsResult>("/Client/GetPlayFabIDsFromPSNAccountIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromSteamIDs(GetPlayFabIDsFromSteamIDsRequest request, Action<GetPlayFabIDsFromSteamIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromSteamIDsResult>("/Client/GetPlayFabIDsFromSteamIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromTwitchIDs(GetPlayFabIDsFromTwitchIDsRequest request, Action<GetPlayFabIDsFromTwitchIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromTwitchIDsResult>("/Client/GetPlayFabIDsFromTwitchIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPlayFabIDsFromXboxLiveIDs(GetPlayFabIDsFromXboxLiveIDsRequest request, Action<GetPlayFabIDsFromXboxLiveIDsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPlayFabIDsFromXboxLiveIDsResult>("/Client/GetPlayFabIDsFromXboxLiveIDs", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPublisherData(GetPublisherDataRequest request, Action<GetPublisherDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPublisherDataResult>("/Client/GetPublisherData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetPurchase(GetPurchaseRequest request, Action<GetPurchaseResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetPurchaseResult>("/Client/GetPurchase", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetSharedGroupData(GetSharedGroupDataRequest request, Action<GetSharedGroupDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetSharedGroupDataResult>("/Client/GetSharedGroupData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetStoreItems(GetStoreItemsRequest request, Action<GetStoreItemsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetStoreItemsResult>("/Client/GetStoreItems", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetTime(GetTimeRequest request, Action<GetTimeResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTimeResult>("/Client/GetTime", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetTitleData(GetTitleDataRequest request, Action<GetTitleDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTitleDataResult>("/Client/GetTitleData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetTitleNews(GetTitleNewsRequest request, Action<GetTitleNewsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTitleNewsResult>("/Client/GetTitleNews", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetTitlePublicKey(GetTitlePublicKeyRequest request, Action<GetTitlePublicKeyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			PlayFabHttp.MakeApiCall<GetTitlePublicKeyResult>("/Client/GetTitlePublicKey", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetTradeStatus(GetTradeStatusRequest request, Action<GetTradeStatusResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTradeStatusResponse>("/Client/GetTradeStatus", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetUserData(GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetUserDataResult>("/Client/GetUserData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetUserInventory(GetUserInventoryRequest request, Action<GetUserInventoryResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetUserInventoryResult>("/Client/GetUserInventory", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetUserPublisherData(GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetUserDataResult>("/Client/GetUserPublisherData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetUserPublisherReadOnlyData(GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetUserDataResult>("/Client/GetUserPublisherReadOnlyData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetUserReadOnlyData(GetUserDataRequest request, Action<GetUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetUserDataResult>("/Client/GetUserReadOnlyData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetWindowsHelloChallenge(GetWindowsHelloChallengeRequest request, Action<GetWindowsHelloChallengeResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			PlayFabHttp.MakeApiCall<GetWindowsHelloChallengeResponse>("/Client/GetWindowsHelloChallenge", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GrantCharacterToUser(GrantCharacterToUserRequest request, Action<GrantCharacterToUserResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GrantCharacterToUserResult>("/Client/GrantCharacterToUser", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkAndroidDeviceID(LinkAndroidDeviceIDRequest request, Action<LinkAndroidDeviceIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkAndroidDeviceIDResult>("/Client/LinkAndroidDeviceID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkApple(LinkAppleRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/Client/LinkApple", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkCustomID(LinkCustomIDRequest request, Action<LinkCustomIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkCustomIDResult>("/Client/LinkCustomID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkFacebookAccount(LinkFacebookAccountRequest request, Action<LinkFacebookAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkFacebookAccountResult>("/Client/LinkFacebookAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkFacebookInstantGamesId(LinkFacebookInstantGamesIdRequest request, Action<LinkFacebookInstantGamesIdResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkFacebookInstantGamesIdResult>("/Client/LinkFacebookInstantGamesId", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkGameCenterAccount(LinkGameCenterAccountRequest request, Action<LinkGameCenterAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkGameCenterAccountResult>("/Client/LinkGameCenterAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkGoogleAccount(LinkGoogleAccountRequest request, Action<LinkGoogleAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkGoogleAccountResult>("/Client/LinkGoogleAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkIOSDeviceID(LinkIOSDeviceIDRequest request, Action<LinkIOSDeviceIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkIOSDeviceIDResult>("/Client/LinkIOSDeviceID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkKongregate(LinkKongregateAccountRequest request, Action<LinkKongregateAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkKongregateAccountResult>("/Client/LinkKongregate", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkNintendoAccount(LinkNintendoAccountRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/Client/LinkNintendoAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkNintendoSwitchDeviceId(LinkNintendoSwitchDeviceIdRequest request, Action<LinkNintendoSwitchDeviceIdResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkNintendoSwitchDeviceIdResult>("/Client/LinkNintendoSwitchDeviceId", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkOpenIdConnect(LinkOpenIdConnectRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/Client/LinkOpenIdConnect", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkPSNAccount(LinkPSNAccountRequest request, Action<LinkPSNAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkPSNAccountResult>("/Client/LinkPSNAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkSteamAccount(LinkSteamAccountRequest request, Action<LinkSteamAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkSteamAccountResult>("/Client/LinkSteamAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkTwitch(LinkTwitchAccountRequest request, Action<LinkTwitchAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkTwitchAccountResult>("/Client/LinkTwitch", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkWindowsHello(LinkWindowsHelloAccountRequest request, Action<LinkWindowsHelloAccountResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkWindowsHelloAccountResponse>("/Client/LinkWindowsHello", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LinkXboxAccount(LinkXboxAccountRequest request, Action<LinkXboxAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<LinkXboxAccountResult>("/Client/LinkXboxAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithAndroidDeviceID(LoginWithAndroidDeviceIDRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithAndroidDeviceID", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithApple(LoginWithAppleRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithApple", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithCustomID(LoginWithCustomIDRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithCustomID", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithEmailAddress(LoginWithEmailAddressRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithEmailAddress", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithFacebook(LoginWithFacebookRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithFacebook", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithFacebookInstantGamesId(LoginWithFacebookInstantGamesIdRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithFacebookInstantGamesId", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithGameCenter(LoginWithGameCenterRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithGameCenter", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithGoogleAccount(LoginWithGoogleAccountRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithGoogleAccount", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithIOSDeviceID(LoginWithIOSDeviceIDRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithIOSDeviceID", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithKongregate(LoginWithKongregateRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithKongregate", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithNintendoAccount(LoginWithNintendoAccountRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithNintendoAccount", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithNintendoSwitchDeviceId(LoginWithNintendoSwitchDeviceIdRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithNintendoSwitchDeviceId", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithOpenIdConnect(LoginWithOpenIdConnectRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithOpenIdConnect", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithPlayFab(LoginWithPlayFabRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithPlayFab", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithPSN(LoginWithPSNRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithPSN", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithSteam(LoginWithSteamRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithSteam", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithTwitch(LoginWithTwitchRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithTwitch", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithWindowsHello(LoginWithWindowsHelloRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithWindowsHello", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void LoginWithXbox(LoginWithXboxRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/LoginWithXbox", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void Matchmake(MatchmakeRequest request, Action<MatchmakeResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<MatchmakeResult>("/Client/Matchmake", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void OpenTrade(OpenTradeRequest request, Action<OpenTradeResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<OpenTradeResponse>("/Client/OpenTrade", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PayForPurchase(PayForPurchaseRequest request, Action<PayForPurchaseResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<PayForPurchaseResult>("/Client/PayForPurchase", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PurchaseItem(PurchaseItemRequest request, Action<PurchaseItemResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<PurchaseItemResult>("/Client/PurchaseItem", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RedeemCoupon(RedeemCouponRequest request, Action<RedeemCouponResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RedeemCouponResult>("/Client/RedeemCoupon", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RefreshPSNAuthToken(RefreshPSNAuthTokenRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/RefreshPSNAuthToken", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RegisterForIOSPushNotification(RegisterForIOSPushNotificationRequest request, Action<RegisterForIOSPushNotificationResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RegisterForIOSPushNotificationResult>("/Client/RegisterForIOSPushNotification", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RegisterPlayFabUser(RegisterPlayFabUserRequest request, Action<RegisterPlayFabUserResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<RegisterPlayFabUserResult>("/Client/RegisterPlayFabUser", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RegisterWithWindowsHello(RegisterWithWindowsHelloRequest request, Action<LoginResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			request.TitleId = (request.TitleId ?? playFabApiSettings.TitleId);
			PlayFabHttp.MakeApiCall<LoginResult>("/Client/RegisterWithWindowsHello", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RemoveContactEmail(RemoveContactEmailRequest request, Action<RemoveContactEmailResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RemoveContactEmailResult>("/Client/RemoveContactEmail", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RemoveFriend(RemoveFriendRequest request, Action<RemoveFriendResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RemoveFriendResult>("/Client/RemoveFriend", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RemoveGenericID(RemoveGenericIDRequest request, Action<RemoveGenericIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RemoveGenericIDResult>("/Client/RemoveGenericID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RemoveSharedGroupMembers(RemoveSharedGroupMembersRequest request, Action<RemoveSharedGroupMembersResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RemoveSharedGroupMembersResult>("/Client/RemoveSharedGroupMembers", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ReportAdActivity(ReportAdActivityRequest request, Action<ReportAdActivityResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ReportAdActivityResult>("/Client/ReportAdActivity", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ReportDeviceInfo(DeviceInfoRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/ReportDeviceInfo", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ReportPlayer(ReportPlayerClientRequest request, Action<ReportPlayerClientResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ReportPlayerClientResult>("/Client/ReportPlayer", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RestoreIOSPurchases(RestoreIOSPurchasesRequest request, Action<RestoreIOSPurchasesResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RestoreIOSPurchasesResult>("/Client/RestoreIOSPurchases", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RewardAdActivity(RewardAdActivityRequest request, Action<RewardAdActivityResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RewardAdActivityResult>("/Client/RewardAdActivity", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void SendAccountRecoveryEmail(SendAccountRecoveryEmailRequest request, Action<SendAccountRecoveryEmailResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			PlayFabHttp.MakeApiCall<SendAccountRecoveryEmailResult>("/Client/SendAccountRecoveryEmail", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void SetFriendTags(SetFriendTagsRequest request, Action<SetFriendTagsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<SetFriendTagsResult>("/Client/SetFriendTags", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void SetPlayerSecret(SetPlayerSecretRequest request, Action<SetPlayerSecretResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<SetPlayerSecretResult>("/Client/SetPlayerSecret", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void StartGame(StartGameRequest request, Action<StartGameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<StartGameResult>("/Client/StartGame", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void StartPurchase(StartPurchaseRequest request, Action<StartPurchaseResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<StartPurchaseResult>("/Client/StartPurchase", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void SubtractUserVirtualCurrency(SubtractUserVirtualCurrencyRequest request, Action<ModifyUserVirtualCurrencyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ModifyUserVirtualCurrencyResult>("/Client/SubtractUserVirtualCurrency", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkAndroidDeviceID(UnlinkAndroidDeviceIDRequest request, Action<UnlinkAndroidDeviceIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkAndroidDeviceIDResult>("/Client/UnlinkAndroidDeviceID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkApple(UnlinkAppleRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/UnlinkApple", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkCustomID(UnlinkCustomIDRequest request, Action<UnlinkCustomIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkCustomIDResult>("/Client/UnlinkCustomID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkFacebookAccount(UnlinkFacebookAccountRequest request, Action<UnlinkFacebookAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkFacebookAccountResult>("/Client/UnlinkFacebookAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkFacebookInstantGamesId(UnlinkFacebookInstantGamesIdRequest request, Action<UnlinkFacebookInstantGamesIdResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkFacebookInstantGamesIdResult>("/Client/UnlinkFacebookInstantGamesId", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkGameCenterAccount(UnlinkGameCenterAccountRequest request, Action<UnlinkGameCenterAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkGameCenterAccountResult>("/Client/UnlinkGameCenterAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkGoogleAccount(UnlinkGoogleAccountRequest request, Action<UnlinkGoogleAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkGoogleAccountResult>("/Client/UnlinkGoogleAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkIOSDeviceID(UnlinkIOSDeviceIDRequest request, Action<UnlinkIOSDeviceIDResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkIOSDeviceIDResult>("/Client/UnlinkIOSDeviceID", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkKongregate(UnlinkKongregateAccountRequest request, Action<UnlinkKongregateAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkKongregateAccountResult>("/Client/UnlinkKongregate", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkNintendoAccount(UnlinkNintendoAccountRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/UnlinkNintendoAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkNintendoSwitchDeviceId(UnlinkNintendoSwitchDeviceIdRequest request, Action<UnlinkNintendoSwitchDeviceIdResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkNintendoSwitchDeviceIdResult>("/Client/UnlinkNintendoSwitchDeviceId", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkOpenIdConnect(UnlinkOpenIdConnectRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/UnlinkOpenIdConnect", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkPSNAccount(UnlinkPSNAccountRequest request, Action<UnlinkPSNAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkPSNAccountResult>("/Client/UnlinkPSNAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkSteamAccount(UnlinkSteamAccountRequest request, Action<UnlinkSteamAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkSteamAccountResult>("/Client/UnlinkSteamAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkTwitch(UnlinkTwitchAccountRequest request, Action<UnlinkTwitchAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkTwitchAccountResult>("/Client/UnlinkTwitch", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkWindowsHello(UnlinkWindowsHelloAccountRequest request, Action<UnlinkWindowsHelloAccountResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkWindowsHelloAccountResponse>("/Client/UnlinkWindowsHello", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlinkXboxAccount(UnlinkXboxAccountRequest request, Action<UnlinkXboxAccountResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlinkXboxAccountResult>("/Client/UnlinkXboxAccount", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlockContainerInstance(UnlockContainerInstanceRequest request, Action<UnlockContainerItemResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlockContainerItemResult>("/Client/UnlockContainerInstance", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnlockContainerItem(UnlockContainerItemRequest request, Action<UnlockContainerItemResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UnlockContainerItemResult>("/Client/UnlockContainerItem", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateAvatarUrl(UpdateAvatarUrlRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/Client/UpdateAvatarUrl", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateCharacterData(UpdateCharacterDataRequest request, Action<UpdateCharacterDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateCharacterDataResult>("/Client/UpdateCharacterData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateCharacterStatistics(UpdateCharacterStatisticsRequest request, Action<UpdateCharacterStatisticsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateCharacterStatisticsResult>("/Client/UpdateCharacterStatistics", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdatePlayerStatistics(UpdatePlayerStatisticsRequest request, Action<UpdatePlayerStatisticsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdatePlayerStatisticsResult>("/Client/UpdatePlayerStatistics", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateSharedGroupData(UpdateSharedGroupDataRequest request, Action<UpdateSharedGroupDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateSharedGroupDataResult>("/Client/UpdateSharedGroupData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateUserData(UpdateUserDataRequest request, Action<UpdateUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateUserDataResult>("/Client/UpdateUserData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateUserPublisherData(UpdateUserDataRequest request, Action<UpdateUserDataResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateUserDataResult>("/Client/UpdateUserPublisherData", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UpdateUserTitleDisplayName(UpdateUserTitleDisplayNameRequest request, Action<UpdateUserTitleDisplayNameResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<UpdateUserTitleDisplayNameResult>("/Client/UpdateUserTitleDisplayName", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ValidateAmazonIAPReceipt(ValidateAmazonReceiptRequest request, Action<ValidateAmazonReceiptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ValidateAmazonReceiptResult>("/Client/ValidateAmazonIAPReceipt", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ValidateGooglePlayPurchase(ValidateGooglePlayPurchaseRequest request, Action<ValidateGooglePlayPurchaseResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ValidateGooglePlayPurchaseResult>("/Client/ValidateGooglePlayPurchase", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ValidateIOSReceipt(ValidateIOSReceiptRequest request, Action<ValidateIOSReceiptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ValidateIOSReceiptResult>("/Client/ValidateIOSReceipt", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ValidateWindowsStoreReceipt(ValidateWindowsReceiptRequest request, Action<ValidateWindowsReceiptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ValidateWindowsReceiptResult>("/Client/ValidateWindowsStoreReceipt", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void WriteCharacterEvent(WriteClientCharacterEventRequest request, Action<WriteEventResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<WriteEventResponse>("/Client/WriteCharacterEvent", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void WritePlayerEvent(WriteClientPlayerEventRequest request, Action<WriteEventResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<WriteEventResponse>("/Client/WritePlayerEvent", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void WriteTitleEvent(WriteTitleEventRequest request, Action<WriteEventResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsClientLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<WriteEventResponse>("/Client/WriteTitleEvent", request, AuthType.LoginSession, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;
	}
}
