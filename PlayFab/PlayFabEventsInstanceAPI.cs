using System;
using System.Collections.Generic;
using PlayFab.EventsModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabEventsInstanceAPI : IPlayFabInstanceApi
	{
		public PlayFabEventsInstanceAPI(PlayFabAuthenticationContext context)
		{
			if (context == null)
			{
				throw new PlayFabException(PlayFabExceptionCode.AuthContextRequired, "Context cannot be null, create a PlayFabAuthenticationContext for each player in advance, or call <PlayFabClientInstanceAPI>.GetAuthenticationContext()");
			}
			this.authenticationContext = context;
		}

		public PlayFabEventsInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
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

		public void WriteEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<WriteEventsResponse>("/Event/WriteEvents", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void WriteTelemetryEvents(WriteEventsRequest request, Action<WriteEventsResponse> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<WriteEventsResponse>("/Event/WriteTelemetryEvents", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;
	}
}
