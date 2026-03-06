using System;
using System.Collections.Generic;
using PlayFab.Internal;
using PlayFab.MultiplayerModels;

namespace PlayFab
{
	public static class PlayFabMultiplayerAPI
	{
		public static bool IsEntityLoggedIn()
		{
			return PlayFabSettings.staticPlayer.IsEntityLoggedIn();
		}

		public static void ForgetAllCredentials()
		{
			PlayFabSettings.staticPlayer.ForgetAllCredentials();
		}

		public static void CancelAllMatchmakingTicketsForPlayer(CancelAllMatchmakingTicketsForPlayerRequest request, Action<CancelAllMatchmakingTicketsForPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CancelAllMatchmakingTicketsForPlayerResult>("/Match/CancelAllMatchmakingTicketsForPlayer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CancelAllServerBackfillTicketsForPlayer(CancelAllServerBackfillTicketsForPlayerRequest request, Action<CancelAllServerBackfillTicketsForPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CancelAllServerBackfillTicketsForPlayerResult>("/Match/CancelAllServerBackfillTicketsForPlayer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CancelMatchmakingTicket(CancelMatchmakingTicketRequest request, Action<CancelMatchmakingTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CancelMatchmakingTicketResult>("/Match/CancelMatchmakingTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CancelServerBackfillTicket(CancelServerBackfillTicketRequest request, Action<CancelServerBackfillTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CancelServerBackfillTicketResult>("/Match/CancelServerBackfillTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateBuildAlias(CreateBuildAliasRequest request, Action<BuildAliasDetailsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<BuildAliasDetailsResponse>("/MultiplayerServer/CreateBuildAlias", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateBuildWithCustomContainer(CreateBuildWithCustomContainerRequest request, Action<CreateBuildWithCustomContainerResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateBuildWithCustomContainerResponse>("/MultiplayerServer/CreateBuildWithCustomContainer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateBuildWithManagedContainer(CreateBuildWithManagedContainerRequest request, Action<CreateBuildWithManagedContainerResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateBuildWithManagedContainerResponse>("/MultiplayerServer/CreateBuildWithManagedContainer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateMatchmakingTicket(CreateMatchmakingTicketRequest request, Action<CreateMatchmakingTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateMatchmakingTicketResult>("/Match/CreateMatchmakingTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateRemoteUser(CreateRemoteUserRequest request, Action<CreateRemoteUserResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateRemoteUserResponse>("/MultiplayerServer/CreateRemoteUser", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateServerBackfillTicket(CreateServerBackfillTicketRequest request, Action<CreateServerBackfillTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateServerBackfillTicketResult>("/Match/CreateServerBackfillTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void CreateServerMatchmakingTicket(CreateServerMatchmakingTicketRequest request, Action<CreateMatchmakingTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<CreateMatchmakingTicketResult>("/Match/CreateServerMatchmakingTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteAsset(DeleteAssetRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteAsset", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteBuild(DeleteBuildRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteBuild", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteBuildAlias(DeleteBuildAliasRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteBuildAlias", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteBuildRegion(DeleteBuildRegionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteBuildRegion", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteCertificate(DeleteCertificateRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteCertificate", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteContainerImageRepository(DeleteContainerImageRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteContainerImageRepository", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void DeleteRemoteUser(DeleteRemoteUserRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/DeleteRemoteUser", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void EnableMultiplayerServersForTitle(EnableMultiplayerServersForTitleRequest request, Action<EnableMultiplayerServersForTitleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EnableMultiplayerServersForTitleResponse>("/MultiplayerServer/EnableMultiplayerServersForTitle", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetAssetUploadUrl(GetAssetUploadUrlRequest request, Action<GetAssetUploadUrlResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetAssetUploadUrlResponse>("/MultiplayerServer/GetAssetUploadUrl", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetBuild(GetBuildRequest request, Action<GetBuildResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetBuildResponse>("/MultiplayerServer/GetBuild", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetBuildAlias(GetBuildAliasRequest request, Action<BuildAliasDetailsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<BuildAliasDetailsResponse>("/MultiplayerServer/GetBuildAlias", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetContainerRegistryCredentials(GetContainerRegistryCredentialsRequest request, Action<GetContainerRegistryCredentialsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetContainerRegistryCredentialsResponse>("/MultiplayerServer/GetContainerRegistryCredentials", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMatch(GetMatchRequest request, Action<GetMatchResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMatchResult>("/Match/GetMatch", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMatchmakingQueue(GetMatchmakingQueueRequest request, Action<GetMatchmakingQueueResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMatchmakingQueueResult>("/Match/GetMatchmakingQueue", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMatchmakingTicket(GetMatchmakingTicketRequest request, Action<GetMatchmakingTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMatchmakingTicketResult>("/Match/GetMatchmakingTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMultiplayerServerDetails(GetMultiplayerServerDetailsRequest request, Action<GetMultiplayerServerDetailsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMultiplayerServerDetailsResponse>("/MultiplayerServer/GetMultiplayerServerDetails", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMultiplayerServerLogs(GetMultiplayerServerLogsRequest request, Action<GetMultiplayerServerLogsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMultiplayerServerLogsResponse>("/MultiplayerServer/GetMultiplayerServerLogs", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetMultiplayerSessionLogsBySessionId(GetMultiplayerSessionLogsBySessionIdRequest request, Action<GetMultiplayerServerLogsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetMultiplayerServerLogsResponse>("/MultiplayerServer/GetMultiplayerSessionLogsBySessionId", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetQueueStatistics(GetQueueStatisticsRequest request, Action<GetQueueStatisticsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetQueueStatisticsResult>("/Match/GetQueueStatistics", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetRemoteLoginEndpoint(GetRemoteLoginEndpointRequest request, Action<GetRemoteLoginEndpointResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetRemoteLoginEndpointResponse>("/MultiplayerServer/GetRemoteLoginEndpoint", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetServerBackfillTicket(GetServerBackfillTicketRequest request, Action<GetServerBackfillTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetServerBackfillTicketResult>("/Match/GetServerBackfillTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetTitleEnabledForMultiplayerServersStatus(GetTitleEnabledForMultiplayerServersStatusRequest request, Action<GetTitleEnabledForMultiplayerServersStatusResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTitleEnabledForMultiplayerServersStatusResponse>("/MultiplayerServer/GetTitleEnabledForMultiplayerServersStatus", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void GetTitleMultiplayerServersQuotas(GetTitleMultiplayerServersQuotasRequest request, Action<GetTitleMultiplayerServersQuotasResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetTitleMultiplayerServersQuotasResponse>("/MultiplayerServer/GetTitleMultiplayerServersQuotas", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void JoinMatchmakingTicket(JoinMatchmakingTicketRequest request, Action<JoinMatchmakingTicketResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<JoinMatchmakingTicketResult>("/Match/JoinMatchmakingTicket", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListArchivedMultiplayerServers(ListMultiplayerServersRequest request, Action<ListMultiplayerServersResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListMultiplayerServersResponse>("/MultiplayerServer/ListArchivedMultiplayerServers", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListAssetSummaries(ListAssetSummariesRequest request, Action<ListAssetSummariesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListAssetSummariesResponse>("/MultiplayerServer/ListAssetSummaries", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListBuildAliases(MultiplayerEmptyRequest request, Action<ListBuildAliasesForTitleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListBuildAliasesForTitleResponse>("/MultiplayerServer/ListBuildAliases", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListBuildSummaries(ListBuildSummariesRequest request, Action<ListBuildSummariesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListBuildSummariesResponse>("/MultiplayerServer/ListBuildSummaries", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListCertificateSummaries(ListCertificateSummariesRequest request, Action<ListCertificateSummariesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListCertificateSummariesResponse>("/MultiplayerServer/ListCertificateSummaries", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListContainerImages(ListContainerImagesRequest request, Action<ListContainerImagesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListContainerImagesResponse>("/MultiplayerServer/ListContainerImages", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListContainerImageTags(ListContainerImageTagsRequest request, Action<ListContainerImageTagsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListContainerImageTagsResponse>("/MultiplayerServer/ListContainerImageTags", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListMatchmakingQueues(ListMatchmakingQueuesRequest request, Action<ListMatchmakingQueuesResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListMatchmakingQueuesResult>("/Match/ListMatchmakingQueues", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListMatchmakingTicketsForPlayer(ListMatchmakingTicketsForPlayerRequest request, Action<ListMatchmakingTicketsForPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListMatchmakingTicketsForPlayerResult>("/Match/ListMatchmakingTicketsForPlayer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListMultiplayerServers(ListMultiplayerServersRequest request, Action<ListMultiplayerServersResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListMultiplayerServersResponse>("/MultiplayerServer/ListMultiplayerServers", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListPartyQosServers(ListPartyQosServersRequest request, Action<ListPartyQosServersResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext authenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			PlayFabHttp.MakeApiCall<ListPartyQosServersResponse>("/MultiplayerServer/ListPartyQosServers", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, authenticationContext, staticSettings, null);
		}

		[Obsolete("Use 'ListQosServersForTitle' instead", false)]
		public static void ListQosServers(ListQosServersRequest request, Action<ListQosServersResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext authenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			PlayFabHttp.MakeApiCall<ListQosServersResponse>("/MultiplayerServer/ListQosServers", request, AuthType.None, resultCallback, errorCallback, customData, extraHeaders, authenticationContext, staticSettings, null);
		}

		public static void ListQosServersForTitle(ListQosServersForTitleRequest request, Action<ListQosServersForTitleResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListQosServersForTitleResponse>("/MultiplayerServer/ListQosServersForTitle", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListServerBackfillTicketsForPlayer(ListServerBackfillTicketsForPlayerRequest request, Action<ListServerBackfillTicketsForPlayerResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListServerBackfillTicketsForPlayerResult>("/Match/ListServerBackfillTicketsForPlayer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ListVirtualMachineSummaries(ListVirtualMachineSummariesRequest request, Action<ListVirtualMachineSummariesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListVirtualMachineSummariesResponse>("/MultiplayerServer/ListVirtualMachineSummaries", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void RemoveMatchmakingQueue(RemoveMatchmakingQueueRequest request, Action<RemoveMatchmakingQueueResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RemoveMatchmakingQueueResult>("/Match/RemoveMatchmakingQueue", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void RequestMultiplayerServer(RequestMultiplayerServerRequest request, Action<RequestMultiplayerServerResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RequestMultiplayerServerResponse>("/MultiplayerServer/RequestMultiplayerServer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void RolloverContainerRegistryCredentials(RolloverContainerRegistryCredentialsRequest request, Action<RolloverContainerRegistryCredentialsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<RolloverContainerRegistryCredentialsResponse>("/MultiplayerServer/RolloverContainerRegistryCredentials", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void SetMatchmakingQueue(SetMatchmakingQueueRequest request, Action<SetMatchmakingQueueResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<SetMatchmakingQueueResult>("/Match/SetMatchmakingQueue", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void ShutdownMultiplayerServer(ShutdownMultiplayerServerRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/ShutdownMultiplayerServer", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void UntagContainerImage(UntagContainerImageRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/UntagContainerImage", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void UpdateBuildAlias(UpdateBuildAliasRequest request, Action<BuildAliasDetailsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<BuildAliasDetailsResponse>("/MultiplayerServer/UpdateBuildAlias", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void UpdateBuildRegion(UpdateBuildRegionRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/UpdateBuildRegion", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void UpdateBuildRegions(UpdateBuildRegionsRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/UpdateBuildRegions", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}

		public static void UploadCertificate(UploadCertificateRequest request, Action<EmptyResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? PlayFabSettings.staticPlayer;
			PlayFabApiSettings staticSettings = PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResponse>("/MultiplayerServer/UploadCertificate", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, staticSettings, null);
		}
	}
}
