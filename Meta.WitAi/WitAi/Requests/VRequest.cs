using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.WitAi.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Meta.WitAi.Requests
{
	[LogCategory(LogCategory.Requests)]
	internal class VRequest : IVRequest, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Requests, null);

		private static Task WaitForTurn(VRequest request)
		{
			VRequest.<WaitForTurn>d__5 <WaitForTurn>d__;
			<WaitForTurn>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTurn>d__.request = request;
			<WaitForTurn>d__.<>1__state = -1;
			<WaitForTurn>d__.<>t__builder.Start<VRequest.<WaitForTurn>d__5>(ref <WaitForTurn>d__);
			return <WaitForTurn>d__.<>t__builder.Task;
		}

		public string Url { get; set; }

		public Dictionary<string, string> UrlParameters { get; set; }

		public string ContentType { get; set; }

		public VRequestMethod Method { get; set; }

		public DownloadHandler Downloader { get; set; }

		public UploadHandler Uploader { get; set; }

		public event VRequestProgressDelegate OnUploadProgress;

		public event VRequestProgressDelegate OnDownloadProgress;

		[Obsolete("Use TimeoutMs instead")]
		public int Timeout
		{
			get
			{
				return Mathf.CeilToInt((float)this.TimeoutMs / 1000f);
			}
			set
			{
				this.TimeoutMs = value * 1000;
			}
		}

		public int TimeoutMs { get; set; } = 5000;

		public event VRequestResponseDelegate OnFirstResponse;

		public bool IsQueued { get; private set; }

		public bool IsRunning { get; private set; }

		public bool IsDecoding { get; private set; }

		public bool IsPerforming
		{
			get
			{
				return this.IsQueued || this.IsRunning || this.IsDecoding;
			}
		}

		public bool HasFirstResponse { get; private set; }

		public bool IsComplete { get; private set; }

		public TaskCompletionSource<bool> Completion { get; private set; } = new TaskCompletionSource<bool>();

		public int ResponseCode { get; set; }

		public string ResponseError { get; private set; }

		public float UploadProgress { get; private set; }

		public float DownloadProgress { get; private set; }

		public virtual void Reset()
		{
			this.IsComplete = false;
			this.IsQueued = false;
			this.IsRunning = false;
			this.IsDecoding = false;
			this.HasFirstResponse = false;
			this.UploadProgress = 0f;
			VRequestProgressDelegate onUploadProgress = this.OnUploadProgress;
			if (onUploadProgress != null)
			{
				onUploadProgress(0f);
			}
			this.DownloadProgress = 0f;
			VRequestProgressDelegate onDownloadProgress = this.OnDownloadProgress;
			if (onDownloadProgress != null)
			{
				onDownloadProgress(0f);
			}
			this.ResponseCode = 200;
			this.ResponseError = string.Empty;
		}

		public virtual Task<VRequestResponse<TValue>> Request<TValue>(VRequestDecodeDelegate<TValue> decoder)
		{
			VRequest.<Request>d__92<TValue> <Request>d__;
			<Request>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TValue>>.Create();
			<Request>d__.<>4__this = this;
			<Request>d__.decoder = decoder;
			<Request>d__.<>1__state = -1;
			<Request>d__.<>t__builder.Start<VRequest.<Request>d__92<TValue>>(ref <Request>d__);
			return <Request>d__.<>t__builder.Task;
		}

		protected virtual Uri GetUri()
		{
			string text = this.Url;
			if (!VRequest.HasUriSchema(text))
			{
				text = "file://" + text;
			}
			if (this.UrlParameters != null)
			{
				bool flag = false;
				if (!text.Contains('?'))
				{
					text += "?";
					flag = true;
				}
				else if (text.EndsWith('?'))
				{
					flag = true;
				}
				foreach (string text2 in this.UrlParameters.Keys)
				{
					string text3 = this.UrlParameters[text2];
					if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							text += "&";
						}
						text3 = UnityWebRequest.EscapeURL(text3).Replace("+", "%20");
						text = text + text2 + "=" + text3;
					}
				}
			}
			return new Uri(text);
		}

		protected virtual string GetMethod()
		{
			switch (this.Method)
			{
			case VRequestMethod.HttpGet:
				return "GET";
			case VRequestMethod.HttpPost:
				return "POST";
			case VRequestMethod.HttpPut:
				return "PUT";
			case VRequestMethod.HttpHead:
				return "HEAD";
			default:
				return null;
			}
		}

		protected virtual Dictionary<string, string> GetHeaders()
		{
			return new Dictionary<string, string>();
		}

		private Task WaitForTimeout()
		{
			VRequest.<WaitForTimeout>d__97 <WaitForTimeout>d__;
			<WaitForTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTimeout>d__.<>4__this = this;
			<WaitForTimeout>d__.<>1__state = -1;
			<WaitForTimeout>d__.<>t__builder.Start<VRequest.<WaitForTimeout>d__97>(ref <WaitForTimeout>d__);
			return <WaitForTimeout>d__.<>t__builder.Task;
		}

		private void UpdateLastResponseTime()
		{
			this._lastResponseReceivedTime = DateTime.UtcNow;
		}

		private DateTime GetLastResponseTime()
		{
			return this._lastResponseReceivedTime;
		}

		protected virtual UnityWebRequest CreateRequest(string url, string method, Dictionary<string, string> headers)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(url, method);
			if (headers != null)
			{
				foreach (string text in headers.Keys)
				{
					string value = headers[text];
					if (!string.IsNullOrEmpty(value))
					{
						unityWebRequest.SetRequestHeader(text, value);
					}
					else
					{
						this.Logger.Warning("Failed to set null header value for '{0}'", new object[]
						{
							text
						});
					}
				}
			}
			if (this.Uploader != null)
			{
				unityWebRequest.uploadHandler = this.Uploader;
				unityWebRequest.disposeUploadHandlerOnDispose = true;
			}
			if (this.Downloader != null)
			{
				unityWebRequest.downloadHandler = this.Downloader;
				unityWebRequest.disposeDownloadHandlerOnDispose = true;
				IVRequestDownloadDecoder ivrequestDownloadDecoder = this.Downloader as IVRequestDownloadDecoder;
				if (ivrequestDownloadDecoder != null)
				{
					ivrequestDownloadDecoder.OnFirstResponse += this.RaiseFirstResponse;
					ivrequestDownloadDecoder.OnResponse += this.UpdateLastResponseTime;
					ivrequestDownloadDecoder.OnProgress += this.UpdateDownloadProgress;
				}
			}
			return unityWebRequest;
		}

		private void MarkRequestComplete(AsyncOperation asyncOperation)
		{
			if (this._request != null && !this.IsComplete)
			{
				this.ResponseCode = (int)this._request.responseCode;
				this.ResponseError = this._request.error;
			}
			this._unityRequestComplete.TrySetResult(true);
		}

		protected virtual Task WaitWhileRunning()
		{
			VRequest.<WaitWhileRunning>d__102 <WaitWhileRunning>d__;
			<WaitWhileRunning>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitWhileRunning>d__.<>4__this = this;
			<WaitWhileRunning>d__.<>1__state = -1;
			<WaitWhileRunning>d__.<>t__builder.Start<VRequest.<WaitWhileRunning>d__102>(ref <WaitWhileRunning>d__);
			return <WaitWhileRunning>d__.<>t__builder.Task;
		}

		protected virtual Task<Tuple<int, string>> GetError(UnityWebRequest request)
		{
			VRequest.<GetError>d__103 <GetError>d__;
			<GetError>d__.<>t__builder = AsyncTaskMethodBuilder<Tuple<int, string>>.Create();
			<GetError>d__.<>4__this = this;
			<GetError>d__.request = request;
			<GetError>d__.<>1__state = -1;
			<GetError>d__.<>t__builder.Start<VRequest.<GetError>d__103>(ref <GetError>d__);
			return <GetError>d__.<>t__builder.Task;
		}

		private Task<string> GetDownloadedText(UnityWebRequest request)
		{
			VRequest.<GetDownloadedText>d__104 <GetDownloadedText>d__;
			<GetDownloadedText>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetDownloadedText>d__.<>4__this = this;
			<GetDownloadedText>d__.request = request;
			<GetDownloadedText>d__.<>1__state = -1;
			<GetDownloadedText>d__.<>t__builder.Start<VRequest.<GetDownloadedText>d__104>(ref <GetDownloadedText>d__);
			return <GetDownloadedText>d__.<>t__builder.Task;
		}

		public virtual void Cancel()
		{
			if (!this.IsComplete && string.IsNullOrEmpty(this.ResponseError))
			{
				this.ResponseCode = -6;
				this.ResponseError = "Cancelled";
			}
			if (this._request != null)
			{
				ThreadUtility.CallOnMainThread(this.Logger, delegate()
				{
					UnityWebRequest request = this._request;
					if (request == null)
					{
						return;
					}
					request.Abort();
				});
			}
			this.Dispose();
		}

		protected virtual void Dispose()
		{
			if (this._request != null)
			{
				ThreadUtility.CallOnMainThread(this.Logger, delegate()
				{
					if (this._request != null)
					{
						UploadHandler uploadHandler = this._request.uploadHandler;
						if (uploadHandler != null)
						{
							uploadHandler.Dispose();
						}
						DownloadHandler downloadHandler = this._request.downloadHandler;
						if (downloadHandler != null)
						{
							downloadHandler.Dispose();
						}
						this._request.Dispose();
						this._request = null;
					}
				});
			}
			this.IsComplete = true;
			List<Task> activeRequests = VRequest._activeRequests;
			lock (activeRequests)
			{
				VRequest._activeRequests.Remove(this.Completion.Task);
			}
			if (!this.Completion.Task.IsCompleted)
			{
				this.Completion.SetResult(true);
			}
		}

		protected virtual void RaiseFirstResponse()
		{
			if (this.HasFirstResponse)
			{
				return;
			}
			this.HasFirstResponse = true;
			VRequestResponseDelegate onFirstResponse = this.OnFirstResponse;
			if (onFirstResponse == null)
			{
				return;
			}
			onFirstResponse();
		}

		protected virtual void UpdateDownloadProgress(float progress)
		{
			if (this.DownloadProgress.Equals(progress))
			{
				return;
			}
			this.DownloadProgress = progress;
			VRequestProgressDelegate onDownloadProgress = this.OnDownloadProgress;
			if (onDownloadProgress == null)
			{
				return;
			}
			onDownloadProgress(this.DownloadProgress);
		}

		public Task<VRequestResponse<Dictionary<string, string>>> RequestFileHeaders(string url)
		{
			VRequest.<RequestFileHeaders>d__109 <RequestFileHeaders>d__;
			<RequestFileHeaders>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<Dictionary<string, string>>>.Create();
			<RequestFileHeaders>d__.<>4__this = this;
			<RequestFileHeaders>d__.url = url;
			<RequestFileHeaders>d__.<>1__state = -1;
			<RequestFileHeaders>d__.<>t__builder.Start<VRequest.<RequestFileHeaders>d__109>(ref <RequestFileHeaders>d__);
			return <RequestFileHeaders>d__.<>t__builder.Task;
		}

		private Task<Dictionary<string, string>> DecodeFileHeaders(UnityWebRequest request)
		{
			VRequest.<DecodeFileHeaders>d__110 <DecodeFileHeaders>d__;
			<DecodeFileHeaders>d__.<>t__builder = AsyncTaskMethodBuilder<Dictionary<string, string>>.Create();
			<DecodeFileHeaders>d__.<>4__this = this;
			<DecodeFileHeaders>d__.request = request;
			<DecodeFileHeaders>d__.<>1__state = -1;
			<DecodeFileHeaders>d__.<>t__builder.Start<VRequest.<DecodeFileHeaders>d__110>(ref <DecodeFileHeaders>d__);
			return <DecodeFileHeaders>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<byte[]>> RequestFile(string url)
		{
			VRequest.<RequestFile>d__111 <RequestFile>d__;
			<RequestFile>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<byte[]>>.Create();
			<RequestFile>d__.<>4__this = this;
			<RequestFile>d__.url = url;
			<RequestFile>d__.<>1__state = -1;
			<RequestFile>d__.<>t__builder.Start<VRequest.<RequestFile>d__111>(ref <RequestFile>d__);
			return <RequestFile>d__.<>t__builder.Task;
		}

		private Task<byte[]> DecodeFile(UnityWebRequest request)
		{
			VRequest.<DecodeFile>d__112 <DecodeFile>d__;
			<DecodeFile>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<DecodeFile>d__.<>4__this = this;
			<DecodeFile>d__.request = request;
			<DecodeFile>d__.<>1__state = -1;
			<DecodeFile>d__.<>t__builder.Start<VRequest.<DecodeFile>d__112>(ref <DecodeFile>d__);
			return <DecodeFile>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<bool>> RequestFileDownload(string url, string downloadPath)
		{
			VRequest.<RequestFileDownload>d__113 <RequestFileDownload>d__;
			<RequestFileDownload>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<bool>>.Create();
			<RequestFileDownload>d__.<>4__this = this;
			<RequestFileDownload>d__.url = url;
			<RequestFileDownload>d__.downloadPath = downloadPath;
			<RequestFileDownload>d__.<>1__state = -1;
			<RequestFileDownload>d__.<>t__builder.Start<VRequest.<RequestFileDownload>d__113>(ref <RequestFileDownload>d__);
			return <RequestFileDownload>d__.<>t__builder.Task;
		}

		protected Task<bool> DecodeSuccess(UnityWebRequest request)
		{
			return Task.FromResult<bool>(true);
		}

		public string GetTmpDownloadPath(string downloadPath)
		{
			return downloadPath + ".tmp";
		}

		public Task<VRequestResponse<bool>> RequestFileExists(string url)
		{
			VRequest.<RequestFileExists>d__116 <RequestFileExists>d__;
			<RequestFileExists>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<bool>>.Create();
			<RequestFileExists>d__.<>4__this = this;
			<RequestFileExists>d__.url = url;
			<RequestFileExists>d__.<>1__state = -1;
			<RequestFileExists>d__.<>t__builder.Start<VRequest.<RequestFileExists>d__116>(ref <RequestFileExists>d__);
			return <RequestFileExists>d__.<>t__builder.Task;
		}

		private static bool IsWebUrl(string url)
		{
			return Regex.IsMatch(url, "(http:|https:).*");
		}

		private static bool IsJarPath(string url)
		{
			return Regex.IsMatch(url, "(jar:).*");
		}

		private static bool HasUriSchema(string url)
		{
			return Regex.IsMatch(url, "(http:|https:|jar:|file:).*");
		}

		public Task<VRequestResponse<string>> RequestText(Action<string> onPartial = null)
		{
			VRequest.<RequestText>d__120 <RequestText>d__;
			<RequestText>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<string>>.Create();
			<RequestText>d__.<>4__this = this;
			<RequestText>d__.onPartial = onPartial;
			<RequestText>d__.<>1__state = -1;
			<RequestText>d__.<>t__builder.Start<VRequest.<RequestText>d__120>(ref <RequestText>d__);
			return <RequestText>d__.<>t__builder.Task;
		}

		private Task<string> DecodeText(UnityWebRequest request)
		{
			VRequest.<DecodeText>d__121 <DecodeText>d__;
			<DecodeText>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<DecodeText>d__.<>4__this = this;
			<DecodeText>d__.request = request;
			<DecodeText>d__.<>1__state = -1;
			<DecodeText>d__.<>t__builder.Start<VRequest.<DecodeText>d__121>(ref <DecodeText>d__);
			return <DecodeText>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJson<TData>(Action<TData> onPartial = null)
		{
			VRequest.<RequestJson>d__122<TData> <RequestJson>d__;
			<RequestJson>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJson>d__.<>4__this = this;
			<RequestJson>d__.onPartial = onPartial;
			<RequestJson>d__.<>1__state = -1;
			<RequestJson>d__.<>t__builder.Start<VRequest.<RequestJson>d__122<TData>>(ref <RequestJson>d__);
			return <RequestJson>d__.<>t__builder.Task;
		}

		private TData DecodeJson<TData>(string json)
		{
			if (typeof(TData) == typeof(string))
			{
				return (TData)((object)json);
			}
			return JsonConvert.DeserializeObject<TData>(json, null, true);
		}

		public Task<VRequestResponse<TData>> RequestJsonGet<TData>(Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonGet>d__124<TData> <RequestJsonGet>d__;
			<RequestJsonGet>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonGet>d__.<>4__this = this;
			<RequestJsonGet>d__.onPartial = onPartial;
			<RequestJsonGet>d__.<>1__state = -1;
			<RequestJsonGet>d__.<>t__builder.Start<VRequest.<RequestJsonGet>d__124<TData>>(ref <RequestJsonGet>d__);
			return <RequestJsonGet>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPost<TData>(Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPost>d__125<TData> <RequestJsonPost>d__;
			<RequestJsonPost>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPost>d__.<>4__this = this;
			<RequestJsonPost>d__.onPartial = onPartial;
			<RequestJsonPost>d__.<>1__state = -1;
			<RequestJsonPost>d__.<>t__builder.Start<VRequest.<RequestJsonPost>d__125<TData>>(ref <RequestJsonPost>d__);
			return <RequestJsonPost>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPost<TData>(byte[] postData, Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPost>d__126<TData> <RequestJsonPost>d__;
			<RequestJsonPost>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPost>d__.<>4__this = this;
			<RequestJsonPost>d__.postData = postData;
			<RequestJsonPost>d__.onPartial = onPartial;
			<RequestJsonPost>d__.<>1__state = -1;
			<RequestJsonPost>d__.<>t__builder.Start<VRequest.<RequestJsonPost>d__126<TData>>(ref <RequestJsonPost>d__);
			return <RequestJsonPost>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPost<TData>(string postText, Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPost>d__127<TData> <RequestJsonPost>d__;
			<RequestJsonPost>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPost>d__.<>4__this = this;
			<RequestJsonPost>d__.postText = postText;
			<RequestJsonPost>d__.onPartial = onPartial;
			<RequestJsonPost>d__.<>1__state = -1;
			<RequestJsonPost>d__.<>t__builder.Start<VRequest.<RequestJsonPost>d__127<TData>>(ref <RequestJsonPost>d__);
			return <RequestJsonPost>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPut<TData>(Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPut>d__128<TData> <RequestJsonPut>d__;
			<RequestJsonPut>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPut>d__.<>4__this = this;
			<RequestJsonPut>d__.onPartial = onPartial;
			<RequestJsonPut>d__.<>1__state = -1;
			<RequestJsonPut>d__.<>t__builder.Start<VRequest.<RequestJsonPut>d__128<TData>>(ref <RequestJsonPut>d__);
			return <RequestJsonPut>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPut<TData>(byte[] putData, Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPut>d__129<TData> <RequestJsonPut>d__;
			<RequestJsonPut>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPut>d__.<>4__this = this;
			<RequestJsonPut>d__.putData = putData;
			<RequestJsonPut>d__.onPartial = onPartial;
			<RequestJsonPut>d__.<>1__state = -1;
			<RequestJsonPut>d__.<>t__builder.Start<VRequest.<RequestJsonPut>d__129<TData>>(ref <RequestJsonPut>d__);
			return <RequestJsonPut>d__.<>t__builder.Task;
		}

		public Task<VRequestResponse<TData>> RequestJsonPut<TData>(string putText, Action<TData> onPartial = null)
		{
			VRequest.<RequestJsonPut>d__130<TData> <RequestJsonPut>d__;
			<RequestJsonPut>d__.<>t__builder = AsyncTaskMethodBuilder<VRequestResponse<TData>>.Create();
			<RequestJsonPut>d__.<>4__this = this;
			<RequestJsonPut>d__.putText = putText;
			<RequestJsonPut>d__.onPartial = onPartial;
			<RequestJsonPut>d__.<>1__state = -1;
			<RequestJsonPut>d__.<>t__builder.Start<VRequest.<RequestJsonPut>d__130<TData>>(ref <RequestJsonPut>d__);
			return <RequestJsonPut>d__.<>t__builder.Task;
		}

		private static byte[] EncodeText(string text)
		{
			return Encoding.UTF8.GetBytes(text);
		}

		public static int MaxConcurrentRequests = 3;

		private static List<Task> _activeRequests = new List<Task>();

		private UnityWebRequest _request;

		private TaskCompletionSource<bool> _unityRequestComplete = new TaskCompletionSource<bool>();

		private DateTime _lastResponseReceivedTime;

		protected const string FilePrepend = "file://";
	}
}
