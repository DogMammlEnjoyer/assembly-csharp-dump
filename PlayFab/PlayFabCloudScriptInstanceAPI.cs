using System;
using System.Collections.Generic;
using PlayFab.CloudScriptModels;
using PlayFab.Internal;
using PlayFab.SharedModels;

namespace PlayFab
{
	public class PlayFabCloudScriptInstanceAPI : IPlayFabInstanceApi
	{
		public PlayFabCloudScriptInstanceAPI(PlayFabAuthenticationContext context)
		{
			if (context == null)
			{
				throw new PlayFabException(PlayFabExceptionCode.AuthContextRequired, "Context cannot be null, create a PlayFabAuthenticationContext for each player in advance, or call <PlayFabClientInstanceAPI>.GetAuthenticationContext()");
			}
			this.authenticationContext = context;
		}

		public PlayFabCloudScriptInstanceAPI(PlayFabApiSettings settings, PlayFabAuthenticationContext context)
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

		public void ExecuteEntityCloudScript(ExecuteEntityCloudScriptRequest request, Action<ExecuteCloudScriptResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ExecuteCloudScriptResult>("/CloudScript/ExecuteEntityCloudScript", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ExecuteFunction(ExecuteFunctionRequest request, Action<ExecuteFunctionResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ExecuteFunctionResult>("/CloudScript/ExecuteFunction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ListFunctions(ListFunctionsRequest request, Action<ListFunctionsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListFunctionsResult>("/CloudScript/ListFunctions", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ListHttpFunctions(ListFunctionsRequest request, Action<ListHttpFunctionsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListHttpFunctionsResult>("/CloudScript/ListHttpFunctions", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void ListQueuedFunctions(ListFunctionsRequest request, Action<ListQueuedFunctionsResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<ListQueuedFunctionsResult>("/CloudScript/ListQueuedFunctions", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PostFunctionResultForEntityTriggeredAction(PostFunctionResultForEntityTriggeredActionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/PostFunctionResultForEntityTriggeredAction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PostFunctionResultForFunctionExecution(PostFunctionResultForFunctionExecutionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/PostFunctionResultForFunctionExecution", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PostFunctionResultForPlayerTriggeredAction(PostFunctionResultForPlayerTriggeredActionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/PostFunctionResultForPlayerTriggeredAction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void PostFunctionResultForScheduledTask(PostFunctionResultForScheduledTaskRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/PostFunctionResultForScheduledTask", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RegisterHttpFunction(RegisterHttpFunctionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/RegisterHttpFunction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void RegisterQueuedFunction(RegisterQueuedFunctionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/RegisterQueuedFunction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public void UnregisterFunction(UnregisterFunctionRequest request, Action<EmptyResult> resultCallback, Action<PlayFabError> errorCallback, object customData = null, Dictionary<string, string> extraHeaders = null)
		{
			PlayFabAuthenticationContext playFabAuthenticationContext = ((request == null) ? null : request.AuthenticationContext) ?? this.authenticationContext;
			PlayFabApiSettings playFabApiSettings = this.apiSettings ?? PlayFabSettings.staticSettings;
			if (!playFabAuthenticationContext.IsEntityLoggedIn())
			{
				throw new PlayFabException(PlayFabExceptionCode.NotLoggedIn, "Must be logged in to call this method");
			}
			PlayFabHttp.MakeApiCall<EmptyResult>("/CloudScript/UnregisterFunction", request, AuthType.EntityToken, resultCallback, errorCallback, customData, extraHeaders, playFabAuthenticationContext, playFabApiSettings, this);
		}

		public readonly PlayFabApiSettings apiSettings;

		public readonly PlayFabAuthenticationContext authenticationContext;
	}
}
