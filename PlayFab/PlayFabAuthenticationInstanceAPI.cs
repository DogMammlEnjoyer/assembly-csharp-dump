using System;
using System.Collections.Generic;
using PlayFab.AuthenticationModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabAuthenticationInstanceAPI : IPlayFabInstanceApi
	{
		public PlayFabAuthenticationInstanceAPI()
		{
			this.authenticationContext = new PlayFabAuthenticationContext();
		}

		public PlayFabAuthenticationInstanceAPI(PlayFabApiSettings settings)
		{
			this.apiSettings = settings;
			this.authenticationContext = new PlayFabAuthenticationContext();
		}

		public PlayFabAuthenticationInstanceAPI(PlayFabAuthenticationContext context)
		{
			this.authenticationContext = (context ?? new PlayFabAuthenticationContext());
		}

		public PlayFabAuthenticationInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
		{
			this.apiSettings = settings;
			this.authenticationContext = (context ?? new PlayFabAuthenticationContext());
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

		public void GetEntityToken(GetEntityTokenRequest request, Action<GetEntityTokenResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			AuthType authType = AuthType.None;
			if (playFabAuthenticationContext.IsClientLoggedIn())
			{
				authType = AuthType.LoginSession;
			}
			if (playFabAuthenticationContext.IsEntityLoggedIn())
			{
				authType = AuthType.EntityToken;
			}
			PlayFabHttp.MakeApiCall<GetEntityTokenResponse>("/Authentication/GetEntityToken", request, authType, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ValidateEntityToken(ValidateEntityTokenRequest request, Action<ValidateEntityTokenResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ValidateEntityTokenResponse>("/Authentication/ValidateEntityToken", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;
	}
}
