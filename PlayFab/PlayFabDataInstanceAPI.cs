using System;
using System.Collections.Generic;
using PlayFab.DataModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabDataInstanceAPI : IPlayFabInstanceApi
	{
		public PlayFabDataInstanceAPI(PlayFabAuthenticationContext context)
		{
			if (context == null)
			{
				throw new PlayFabException(PlayFabExceptionCode.AuthContextRequired, "Context cannot be null, create a PlayFabAuthenticationContext for each player in advance, or call <PlayFabClientInstanceAPI>.GetAuthenticationContext()");
			}
			this.authenticationContext = context;
		}

		public PlayFabDataInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
		{
			if (context == null)
			{
				throw new PlayFabException(PlayFabExceptionCode.AuthContextRequired, "Context cannot be null, create a PlayFabAuthenticationContext for each player in advance, or call <PlayFabClientInstanceAPI>.GetAuthenticationContext()");
			}
			this.apiSettings = settings;
			this.authenticationContext = context;
		}

		public bool IsEntityLoggedIn()
		{
			return this.authenticationContext != null && this.authenticationContext.IsEntityLoggedIn();
		}

		public void ForgetAllCredentials()
		{
			if (this.authenticationContext != null)
			{
				this.authenticationContext.ForgetAllCredentials();
			}
		}

		public void AbortFileUploads(AbortFileUploadsRequest request, Action<AbortFileUploadsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<AbortFileUploadsResponse>("/File/AbortFileUploads", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void DeleteFiles(DeleteFilesRequest request, Action<DeleteFilesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<DeleteFilesResponse>("/File/DeleteFiles", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void FinalizeFileUploads(FinalizeFileUploadsRequest request, Action<FinalizeFileUploadsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<FinalizeFileUploadsResponse>("/File/FinalizeFileUploads", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetFiles(GetFilesRequest request, Action<GetFilesResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetFilesResponse>("/File/GetFiles", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void GetObjects(GetObjectsRequest request, Action<GetObjectsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<GetObjectsResponse>("/Object/GetObjects", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void InitiateFileUploads(InitiateFileUploadsRequest request, Action<InitiateFileUploadsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<InitiateFileUploadsResponse>("/File/InitiateFileUploads", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void SetObjects(SetObjectsRequest request, Action<SetObjectsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<SetObjectsResponse>("/Object/SetObjects", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;
	}
}
