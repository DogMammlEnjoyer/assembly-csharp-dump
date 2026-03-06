using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Modio.API;
using Modio.API.HttpClient;
using Modio.API.Interfaces;
using Modio.API.SchemaDefinitions;
using Modio.Errors;
using Modio.Users;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Modio.Unity
{
	public class ModioAPIUnityClient : IModioAPIInterface, IDisposable
	{
		public void SetBasePath(string value)
		{
			this._basePath = value;
		}

		public void AddDefaultPathParameter(string key, string value)
		{
			this._pathParameters[key] = value;
		}

		public void RemoveDefaultPathParameter(string key)
		{
			this._pathParameters.Remove(key);
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
			this._defaultHeaders.Clear();
			this._basePath = string.Empty;
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
			ModioAPIUnityClient.<DownloadFile>d__13 <DownloadFile>d__;
			<DownloadFile>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, Stream>>.Create();
			<DownloadFile>d__.<>4__this = this;
			<DownloadFile>d__.url = url;
			<DownloadFile>d__.token = token;
			<DownloadFile>d__.<>1__state = -1;
			<DownloadFile>d__.<>t__builder.Start<ModioAPIUnityClient.<DownloadFile>d__13>(ref <DownloadFile>d__);
			return <DownloadFile>d__.<>t__builder.Task;
		}

		private Task<Error> CheckFakeErrorsForTest(string url)
		{
			ModioAPIUnityClient.<>c__DisplayClass14_0 CS$<>8__locals1 = new ModioAPIUnityClient.<>c__DisplayClass14_0();
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

		public void SetDefaultHeader(string name, string value)
		{
			this._defaultHeaders[name] = value;
		}

		public void RemoveDefaultHeader(string name)
		{
			this._defaultHeaders.Remove(name);
		}

		private UnityWebRequest CreateWebRequest(ModioAPIRequest request, string target, DownloadHandler downloadHandler = null)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(target, this.MapMethod(request.Method))
			{
				downloadHandler = (downloadHandler ?? new DownloadHandlerBuffer())
			};
			foreach (KeyValuePair<string, string> keyValuePair in this._defaultHeaders)
			{
				unityWebRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
			}
			unityWebRequest.SetRequestHeader("User-Agent", Version.GetCurrent());
			unityWebRequest.uploadHandler = this.MapUploadHandler(request);
			if (unityWebRequest.uploadHandler == null)
			{
				unityWebRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
			}
			foreach (KeyValuePair<string, string> keyValuePair2 in request.Options.HeaderParameters)
			{
				unityWebRequest.SetRequestHeader(keyValuePair2.Key, keyValuePair2.Value);
			}
			foreach (KeyValuePair<string, string> keyValuePair3 in this._defaultHeaders)
			{
				request.Options.HeaderParameters[keyValuePair3.Key] = keyValuePair3.Value;
			}
			return unityWebRequest;
		}

		private static Error EnforceAuthentication(ModioAPIRequest downloadRequest, UnityWebRequest webRequest)
		{
			if (downloadRequest.Options.RequiresAuthentication && !User.Current.IsAuthenticated)
			{
				return new Error(ErrorCode.USER_NOT_AUTHENTICATED);
			}
			if (User.Current.IsAuthenticated)
			{
				string name = "Authorization";
				string str = "Bearer ";
				User user = User.Current;
				webRequest.SetRequestHeader(name, str + ((user != null) ? user.GetAuthToken() : null));
			}
			return Error.None;
		}

		[return: TupleElementNames(new string[]
		{
			"error",
			null
		})]
		private Task<ValueTuple<Error, T>> GetJson<T>(ModioAPIRequest request, Func<JsonTextReader, Task<T>> reader)
		{
			ModioAPIUnityClient.<GetJson>d__19<T> <GetJson>d__;
			<GetJson>d__.<>t__builder = AsyncTaskMethodBuilder<ValueTuple<Error, T>>.Create();
			<GetJson>d__.<>4__this = this;
			<GetJson>d__.request = request;
			<GetJson>d__.reader = reader;
			<GetJson>d__.<>1__state = -1;
			<GetJson>d__.<>t__builder.Start<ModioAPIUnityClient.<GetJson>d__19<T>>(ref <GetJson>d__);
			return <GetJson>d__.<>t__builder.Task;
		}

		private static Error GetErrorAndLogBadResponse(string jsonResponse)
		{
			if (!string.IsNullOrEmpty(jsonResponse) && jsonResponse[0] != '{')
			{
				int num = jsonResponse.IndexOf('{');
				if (num > 0)
				{
					string str = jsonResponse.Substring(0, num);
					ModioLog verbose = ModioLog.Verbose;
					if (verbose != null)
					{
						verbose.Log("Unexpected error from server before JSON: " + str);
					}
					jsonResponse = jsonResponse.Substring(num);
				}
			}
			ErrorObject errorObject;
			try
			{
				using (StringReader stringReader = new StringReader(jsonResponse))
				{
					using (JsonTextReader jsonTextReader = new JsonTextReader(stringReader))
					{
						errorObject = new JsonSerializer().Deserialize<ErrorObject>(jsonTextReader);
					}
				}
			}
			catch (JsonException)
			{
				ModioLog error = ModioLog.Error;
				if (error != null)
				{
					error.Log("There is an error with the json response.");
				}
				return new Error(ErrorCode.INVALID_JSON);
			}
			if (errorObject.Error.ErrorRef == 0L)
			{
				ModioLog error2 = ModioLog.Error;
				if (error2 != null)
				{
					error2.Log("Invalid error returned from API, please contact mod.io support.\n" + string.Format("{0}: {1}", errorObject.Error.Code, errorObject.Error.Message));
				}
				return new Error(ErrorCode.UNKNOWN);
			}
			return new Error((ErrorCode)errorObject.Error.ErrorRef);
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

		private UploadHandler MapUploadHandler(ModioAPIRequest request)
		{
			switch (request.ContentType)
			{
			case ModioAPIRequestContentType.None:
				return null;
			case ModioAPIRequestContentType.String:
				if (request.ContentTypeHint == "application/json")
				{
					return new UploadHandlerRaw(request.Options.BodyDataBytes)
					{
						contentType = request.ContentTypeHint
					};
				}
				break;
			case ModioAPIRequestContentType.FormUrlEncoded:
			{
				string text = this.CreateFormUrlEncodedContent(request.Options.FormParameters);
				if (!string.IsNullOrEmpty(text))
				{
					return new UploadHandlerRaw(Encoding.UTF8.GetBytes(text))
					{
						contentType = "application/x-www-form-urlencoded"
					};
				}
				return null;
			}
			case ModioAPIRequestContentType.ByteArray:
				return ModioAPIUnityClient.PrepareByteArray(request.Options);
			case ModioAPIRequestContentType.MultipartFormData:
				return this.CreateMultipartFormDataUploadHandler(request.Options);
			}
			throw new NotImplementedException();
		}

		private UploadHandler CreateMultipartFormDataUploadHandler(ModioAPIRequestOptions options)
		{
			string str = Guid.NewGuid().ToString().ToUpperInvariant();
			UploadHandler result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, 1024, true))
				{
					streamWriter.WriteLine("--" + str);
					foreach (KeyValuePair<string, string> keyValuePair in options.FormParameters)
					{
						streamWriter.WriteLine("--" + str);
						streamWriter.WriteLine("Content-Disposition: form-data; name=" + keyValuePair.Key);
						streamWriter.WriteLine("Content-Type: text/plain; charset=utf-8");
						streamWriter.WriteLine();
						streamWriter.WriteLine(keyValuePair.Value);
					}
					foreach (KeyValuePair<string, ModioAPIFileParameter> keyValuePair2 in options.FileParameters)
					{
						if (!keyValuePair2.Value.Unused)
						{
							using (Stream content = keyValuePair2.Value.GetContent())
							{
								if (content != null)
								{
									streamWriter.WriteLine("--" + str);
									streamWriter.WriteLine(string.Concat(new string[]
									{
										"Content-Disposition: form-data; name=\"",
										keyValuePair2.Key,
										"\"; filename=\"",
										keyValuePair2.Value.Name,
										"\"; filename*=utf-8''",
										keyValuePair2.Value.Name
									}));
									streamWriter.WriteLine("Content-Type: " + keyValuePair2.Value.ContentType);
									streamWriter.WriteLine();
									streamWriter.Flush();
									content.CopyTo(memoryStream);
									streamWriter.WriteLine();
								}
							}
						}
					}
					streamWriter.WriteLine("--" + str + "--");
					streamWriter.Flush();
				}
				result = new UploadHandlerRaw(memoryStream.ToArray())
				{
					contentType = "multipart/form-data; boundary=" + str
				};
			}
			return result;
		}

		private static UploadHandler PrepareByteArray(ModioAPIRequestOptions options)
		{
			string str = Guid.NewGuid().ToString().ToUpperInvariant();
			return new UploadHandlerRaw(options.BodyDataBytes)
			{
				contentType = "multipart/form-data; boundary=" + str
			};
		}

		private string CreateFormUrlEncodedContent(Dictionary<string, string> formParameters)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, string> keyValuePair in formParameters)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("&");
				}
				stringBuilder.Append(UnityWebRequest.EscapeURL(keyValuePair.Key) + "=" + UnityWebRequest.EscapeURL(keyValuePair.Value));
			}
			return stringBuilder.ToString();
		}

		private string MapMethod(ModioAPIRequestMethod method)
		{
			string result;
			switch (method)
			{
			case ModioAPIRequestMethod.Get:
				result = "GET";
				break;
			case ModioAPIRequestMethod.Delete:
				result = "DELETE";
				break;
			case ModioAPIRequestMethod.Post:
				result = "POST";
				break;
			case ModioAPIRequestMethod.Put:
				result = "PUT";
				break;
			default:
				throw new NotImplementedException();
			}
			return result;
		}

		private string BuildPath(ModioAPIRequest request)
		{
			StringBuilder stringBuilder = new StringBuilder(this._basePath + request.GetUri(this._defaultParameters));
			foreach (KeyValuePair<string, string> keyValuePair in this._pathParameters)
			{
				stringBuilder.Replace("{" + keyValuePair.Key + "}", keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		private Task<Error> SendRequest(UnityWebRequest webRequest, CancellationToken shutdownToken = default(CancellationToken), CancellationToken token = default(CancellationToken))
		{
			ModioAPIUnityClient.<SendRequest>d__29 <SendRequest>d__;
			<SendRequest>d__.<>t__builder = AsyncTaskMethodBuilder<Error>.Create();
			<SendRequest>d__.webRequest = webRequest;
			<SendRequest>d__.shutdownToken = shutdownToken;
			<SendRequest>d__.token = token;
			<SendRequest>d__.<>1__state = -1;
			<SendRequest>d__.<>t__builder.Start<ModioAPIUnityClient.<SendRequest>d__29>(ref <SendRequest>d__);
			return <SendRequest>d__.<>t__builder.Task;
		}

		public void Dispose()
		{
			foreach (UnityWebRequest unityWebRequest in this._webRequests)
			{
				if (unityWebRequest != null)
				{
					unityWebRequest.Dispose();
				}
			}
		}

		private Task LogRequest(UnityWebRequest request, ModioAPIRequest modioRequest = null)
		{
			if (ModioLog.Verbose == null)
			{
				return Task.CompletedTask;
			}
			if (request == null)
			{
				return Task.CompletedTask;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(request.method + " " + request.uri.PathAndQuery + " HTTP/1.1");
			if (modioRequest != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in modioRequest.Options.HeaderParameters)
				{
					string text;
					string text2;
					keyValuePair.Deconstruct(out text, out text2);
					string text3 = text;
					string text4 = text2;
					stringBuilder.AppendLine(string.Equals(text3, "Authorization") ? (text3 + ": Bearer (omitted)") : (text3 + ": " + string.Join(", ", new string[]
					{
						text4
					})));
				}
			}
			foreach (string value in this._defaultParameters)
			{
				stringBuilder.AppendLine(value);
			}
			if (request.uploadHandler != null && request.uploadHandler.data.Length != 0)
			{
				stringBuilder.AppendLine("Content-Type: " + request.uploadHandler.contentType);
				stringBuilder.AppendLine();
				stringBuilder.Append(Encoding.UTF8.GetString(request.uploadHandler.data));
			}
			ModioLog verbose = ModioLog.Verbose;
			if (verbose != null)
			{
				verbose.Log(stringBuilder.ToString());
			}
			return Task.CompletedTask;
		}

		private static bool IsResponseConnectionFailure(long responseCode)
		{
			return responseCode == 0L || responseCode == 408L || responseCode == 503L;
		}

		[ModioDebugMenu(ShowInSettingsMenu = true, ShowInBrowserMenu = false)]
		public static bool UseUnityClient
		{
			get
			{
				return ModioServices.Resolve<IModioAPIInterface>() is ModioAPIUnityClient;
			}
			set
			{
				if (value == ModioAPIUnityClient.UseUnityClient)
				{
					return;
				}
				if (value)
				{
					ModioServices.Bind<IModioAPIInterface>().FromNew<ModioAPIUnityClient>(ModioServicePriority.DeveloperOverride, null);
				}
				else
				{
					ModioServices.Bind<IModioAPIInterface>().FromNew<ModioAPIHttpClient>(ModioServicePriority.DeveloperOverride, null);
				}
				User.LogOut();
			}
		}

		private string _basePath = string.Empty;

		private readonly List<string> _defaultParameters = new List<string>();

		private readonly Dictionary<string, string> _pathParameters = new Dictionary<string, string>();

		private readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>();

		private readonly List<UnityWebRequest> _webRequests = new List<UnityWebRequest>();

		private CancellationTokenSource _cancellationTokenSource;
	}
}
