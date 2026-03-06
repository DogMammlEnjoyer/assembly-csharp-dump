using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modio.API.Interfaces;
using Modio.Errors;
using Modio.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Modio.API.HttpClient
{
	public class ModioAPIHttpClient : IModioAPIInterface, IDisposable
	{
		private Error CancelledOrShutDownError(CancellationToken shutdownCancellationToken)
		{
			return new Error(shutdownCancellationToken.IsCancellationRequested ? ErrorCode.SHUTTING_DOWN : ErrorCode.OPERATION_CANCELLED);
		}

		public void SetBasePath(string value)
		{
			this._basePath = value;
		}

		public void AddDefaultPathParameter(string key, string value)
		{
			this._pathParameters.Add(key, value);
		}

		public void RemoveDefaultPathParameter(string key)
		{
			this._pathParameters.Remove(key);
		}

		public void SetDefaultHeader(string name, string value)
		{
			this.RemoveDefaultHeader(name);
			this._client.DefaultRequestHeaders.Add(name, value);
		}

		public void RemoveDefaultHeader(string name)
		{
			this._client.DefaultRequestHeaders.Remove(name);
		}

		public void AddDefaultParameter(string value)
		{
			this._defaultParameters.Add(value);
		}

		public void RemoveDefaultParameter(string value)
		{
			this._defaultParameters.Remove(value);
		}

		public void ResetConfiguration()
		{
			CancellationTokenSource cancellationTokenSource = this._cancellationTokenSource;
			if (cancellationTokenSource != null)
			{
				cancellationTokenSource.Cancel();
			}
			this._cancellationTokenSource = new CancellationTokenSource();
			this._defaultParameters.Clear();
			this._pathParameters.Clear();
			this._basePath = string.Empty;
			this._client.DefaultRequestHeaders.Clear();
			ModioClient.OnShutdown -= this.Shutdown;
			ModioClient.OnShutdown += this.Shutdown;
		}

		private void Shutdown()
		{
			CancellationTokenSource cancellationTokenSource = this._cancellationTokenSource;
			if (cancellationTokenSource == null)
			{
				return;
			}
			cancellationTokenSource.Cancel();
		}

		public Task<ValueTuple<Error, Stream>> DownloadFile(string url, CancellationToken token = default(CancellationToken))
		{
			ModioAPIHttpClient.<DownloadFile>d__15 <DownloadFile>d__;
			<DownloadFile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Stream>>.Create();
			<DownloadFile>d__.<>4__this = this;
			<DownloadFile>d__.url = url;
			<DownloadFile>d__.token = token;
			<DownloadFile>d__.<>1__state = -1;
			<DownloadFile>d__.<>t__builder.Start<ModioAPIHttpClient.<DownloadFile>d__15>(ref <DownloadFile>d__);
			return <DownloadFile>d__.<>t__builder.Task;
		}

		private static Error EnforceAuthentication(ModioAPIRequest downloadRequest, HttpRequestMessage httpRequest)
		{
			if (downloadRequest.Options.RequiresAuthentication && !User.Current.IsAuthenticated)
			{
				return new Error(ErrorCode.USER_NOT_AUTHENTICATED);
			}
			if (User.Current.IsAuthenticated)
			{
				HttpHeaders headers = httpRequest.Headers;
				string name = "Authorization";
				string str = "Bearer ";
				User user = User.Current;
				headers.Add(name, str + ((user != null) ? user.GetAuthToken() : null));
			}
			return Error.None;
		}

		private Task<Error> CheckFakeErrorsForTest(string url)
		{
			ModioAPIHttpClient.<>c__DisplayClass17_0 CS$<>8__locals1 = new ModioAPIHttpClient.<>c__DisplayClass17_0();
			CS$<>8__locals1.testSettings = ModioClient.Settings.GetPlatformSettings<ModioAPITestSettings>();
			if (CS$<>8__locals1.testSettings == null)
			{
				return Task.FromResult<Error>(Error.None);
			}
			if (CS$<>8__locals1.testSettings.ShouldFakeDisconnected(url))
			{
				return CS$<>8__locals1.<CheckFakeErrorsForTest>g__FakeConnectionError|0();
			}
			if (CS$<>8__locals1.testSettings.ShouldFakeRateLimit(url))
			{
				return Task.FromResult<Error>(new RateLimitError(RateLimitErrorCode.RATELIMITED, 42));
			}
			return Task.FromResult<Error>(Error.None);
		}

		private HttpContent MapContent(ModioAPIRequest request)
		{
			if (request.Method == ModioAPIRequestMethod.Get)
			{
				return null;
			}
			switch (request.ContentType)
			{
			case ModioAPIRequestContentType.None:
				return new StringContent(string.Empty)
				{
					Headers = 
					{
						ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
					}
				};
			case ModioAPIRequestContentType.String:
				if (request.ContentTypeHint == "application/json")
				{
					return new ByteArrayContent(request.Options.BodyDataBytes)
					{
						Headers = 
						{
							ContentType = new MediaTypeHeaderValue("application/json")
						}
					};
				}
				break;
			case ModioAPIRequestContentType.FormUrlEncoded:
				return new FormUrlEncodedContent(request.Options.FormParameters);
			case ModioAPIRequestContentType.ByteArray:
				return ModioAPIHttpClient.PrepareByteArray(request.Options);
			case ModioAPIRequestContentType.MultipartFormData:
				return ModioAPIHttpClient.PrepareMultipartFormDataContent(request.Options);
			}
			throw new NotImplementedException();
		}

		private HttpMethod MapMethod(ModioAPIRequestMethod method)
		{
			HttpMethod result;
			switch (method)
			{
			case ModioAPIRequestMethod.Get:
				result = HttpMethod.Get;
				break;
			case ModioAPIRequestMethod.Delete:
				result = HttpMethod.Delete;
				break;
			case ModioAPIRequestMethod.Post:
				result = HttpMethod.Post;
				break;
			case ModioAPIRequestMethod.Put:
				result = HttpMethod.Put;
				break;
			default:
				throw new NotImplementedException();
			}
			return result;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			null
		})]
		private Task<ValueTuple<Error, T>> GetJson<T>(ModioAPIRequest request, Func<JsonTextReader, Task<T>> reader)
		{
			ModioAPIHttpClient.<GetJson>d__20<T> <GetJson>d__;
			<GetJson>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, T>>.Create();
			<GetJson>d__.<>4__this = this;
			<GetJson>d__.request = request;
			<GetJson>d__.reader = reader;
			<GetJson>d__.<>1__state = -1;
			<GetJson>d__.<>t__builder.Start<ModioAPIHttpClient.<GetJson>d__20<T>>(ref <GetJson>d__);
			return <GetJson>d__.<>t__builder.Task;
		}

		private static Task<Error> GetErrorAndLogBadResponse(StreamReader streamReader)
		{
			ModioAPIHttpClient.<GetErrorAndLogBadResponse>d__21 <GetErrorAndLogBadResponse>d__;
			<GetErrorAndLogBadResponse>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<GetErrorAndLogBadResponse>d__.streamReader = streamReader;
			<GetErrorAndLogBadResponse>d__.<>1__state = -1;
			<GetErrorAndLogBadResponse>d__.<>t__builder.Start<ModioAPIHttpClient.<GetErrorAndLogBadResponse>d__21>(ref <GetErrorAndLogBadResponse>d__);
			return <GetErrorAndLogBadResponse>d__.<>t__builder.Task;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			"result"
		})]
		public Task<ValueTuple<Error, T?>> GetJson<T>(ModioAPIRequest request) where T : struct
		{
			return this.GetJson<T?>(request, (JsonTextReader reader) => Task.FromResult<T?>(new T?(new JsonSerializer().Deserialize<T>(reader))));
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			null
		})]
		public Task<ValueTuple<Error, JToken>> GetJson(ModioAPIRequest request)
		{
			return this.GetJson<JToken>(request, (JsonTextReader reader) => JToken.ReadFromAsync(reader, default(CancellationToken)));
		}

		private string BuildPath(ModioAPIRequest request)
		{
			string text = request.GetUri(this._defaultParameters);
			if (!text.StartsWith("https://"))
			{
				text = this._basePath + text;
			}
			StringBuilder stringBuilder = new StringBuilder(text);
			foreach (KeyValuePair<string, string> keyValuePair in this._pathParameters)
			{
				stringBuilder.Replace("{" + keyValuePair.Key + "}", keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private static HttpContent PrepareMultipartFormDataContent(ModioAPIRequestOptions options)
		{
			MultipartFormDataContent multipartFormDataContent = new MultipartFormDataContent();
			foreach (KeyValuePair<string, string> keyValuePair in options.FormParameters)
			{
				multipartFormDataContent.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);
			}
			foreach (KeyValuePair<string, ModioAPIFileParameter> keyValuePair2 in options.FileParameters)
			{
				if (!keyValuePair2.Value.Unused)
				{
					Stream content = keyValuePair2.Value.GetContent();
					if (content != null)
					{
						StreamContent streamContent = new StreamContent(content);
						if (keyValuePair2.Value.ContentType != null)
						{
							streamContent.Headers.ContentType = new MediaTypeHeaderValue(keyValuePair2.Value.ContentType);
						}
						if (keyValuePair2.Value.Name != null)
						{
							multipartFormDataContent.Add(streamContent, keyValuePair2.Key, keyValuePair2.Value.Name);
						}
						else
						{
							multipartFormDataContent.Add(streamContent, keyValuePair2.Key);
						}
					}
				}
			}
			return multipartFormDataContent;
		}

		private static HttpContent PrepareByteArray(ModioAPIRequestOptions options)
		{
			return new ByteArrayContent(options.BodyDataBytes)
			{
				Headers = 
				{
					ContentType = new MediaTypeHeaderValue("multipart/form-data")
				}
			};
		}

		private Task LogRequest(HttpRequestMessage request)
		{
			ModioAPIHttpClient.<LogRequest>d__27 <LogRequest>d__;
			<LogRequest>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LogRequest>d__.<>4__this = this;
			<LogRequest>d__.request = request;
			<LogRequest>d__.<>1__state = -1;
			<LogRequest>d__.<>t__builder.Start<ModioAPIHttpClient.<LogRequest>d__27>(ref <LogRequest>d__);
			return <LogRequest>d__.<>t__builder.Task;
		}

		public void Dispose()
		{
			this._client.Dispose();
		}

		private readonly HttpClient _client = new HttpClient();

		private readonly List<string> _defaultParameters = new List<string>();

		private readonly Dictionary<string, string> _pathParameters = new Dictionary<string, string>();

		private string _basePath = string.Empty;

		private CancellationTokenSource _cancellationTokenSource;
	}
}
