using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Meta.Voice;
using Meta.Voice.Logging;
using Meta.WitAi.Configuration;
using Meta.WitAi.Data;
using Meta.WitAi.Data.Configuration;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;

namespace Meta.WitAi
{
	[LogCategory(LogCategory.Requests)]
	public class WitRequest : VoiceServiceRequest, IAudioUploadHandler, IDataUploadHandler
	{
		public WitConfiguration Configuration { get; private set; }

		public int TimeoutMs
		{
			get
			{
				return base.Options.TimeoutMs;
			}
		}

		public AudioEncoding AudioEncoding { get; set; }

		[Obsolete("Deprecated for AudioEncoding")]
		public AudioEncoding audioEncoding
		{
			get
			{
				return this.AudioEncoding;
			}
			set
			{
				this.AudioEncoding = value;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				if (this._canSetPath)
				{
					this._path = value;
					return;
				}
				this.Logger.Warning("Cannot set WitRequest.Path while after transmission.", Array.Empty<object>());
			}
		}

		public string Command { get; private set; }

		public bool IsPost { get; private set; }

		[Obsolete("Deprecated for Options.QueryParams")]
		public VoiceServiceRequestOptions.QueryParam[] queryParams
		{
			get
			{
				List<VoiceServiceRequestOptions.QueryParam> list = new List<VoiceServiceRequestOptions.QueryParam>();
				WitRequestOptions options = base.Options;
				Dictionary<string, string>.KeyCollection keyCollection;
				if (options == null)
				{
					keyCollection = null;
				}
				else
				{
					Dictionary<string, string> queryParams = options.QueryParams;
					keyCollection = ((queryParams != null) ? queryParams.Keys : null);
				}
				foreach (string key in keyCollection)
				{
					VoiceServiceRequestOptions.QueryParam queryParam = new VoiceServiceRequestOptions.QueryParam();
					queryParam.key = key;
					WitRequestOptions options2 = base.Options;
					queryParam.value = ((options2 != null) ? options2.QueryParams[key] : null);
					VoiceServiceRequestOptions.QueryParam item = queryParam;
					list.Add(item);
				}
				return list.ToArray();
			}
		}

		protected override bool DecodeRawResponses
		{
			get
			{
				return true;
			}
		}

		public bool IsRequestStreamActive
		{
			get
			{
				return base.IsActive || this.IsInputStreamReady;
			}
		}

		public bool HasResponseStarted { get; private set; }

		public bool IsInputStreamReady { get; private set; }

		public override string ToString()
		{
			return this.Path;
		}

		public event WitRequest.OnProvideCustomHeadersEvent onProvideCustomHeaders;

		[Obsolete("Use OnInputStreamReady instead")]
		public event Action<WitRequest> onInputStreamReady;

		public Action OnInputStreamReady { get; set; }

		[Obsolete("Deprecated for Events.OnPartialTranscription")]
		public event Action<string> onPartialTranscription;

		[Obsolete("Deprecated for Events.OnFullTranscription")]
		public event Action<string> onFullTranscription;

		[Obsolete("Deprecated for Events.OnPartialResponse")]
		public event Action<WitRequest> onPartialResponse;

		[Obsolete("Deprecated for Events.OnComplete")]
		public event Action<WitRequest> onResponse;

		public WitRequest(WitConfiguration newConfiguration, string newPath, WitRequestOptions newOptions, VoiceServiceRequestEvents newEvents) : base(NLPRequestInputType.Audio, newOptions, newEvents)
		{
			this.Configuration = newConfiguration;
			this.Path = newPath;
			this._initialized = true;
			this.SetState(VoiceRequestState.Initialized);
		}

		protected override void SetState(VoiceRequestState newState)
		{
			if (this._initialized)
			{
				base.SetState(newState);
			}
		}

		protected override void OnInit()
		{
			this.Command = this.Path.Split('/', StringSplitOptions.None).First<string>();
			this.IsPost = (WitEndpointConfig.GetEndpointConfig(this.Configuration).Speech == this.Command || WitEndpointConfig.GetEndpointConfig(this.Configuration).Dictation == this.Command);
			base.OnInit();
		}

		protected override void HandleAudioActivation()
		{
			this.SetAudioInputState(VoiceAudioInputState.On);
		}

		protected override void HandleAudioDeactivation()
		{
			if (base.State == VoiceRequestState.Transmitting)
			{
				this.CloseRequestStream();
			}
			this.SetAudioInputState(VoiceAudioInputState.Off);
		}

		protected override string GetSendError()
		{
			if (this.Configuration == null)
			{
				return "Configuration is not set. Cannot start request.";
			}
			if (string.IsNullOrEmpty(this.Configuration.GetClientAccessToken()))
			{
				return "Client access token is not defined. Cannot start request.";
			}
			if (this.OnInputStreamReady == null)
			{
				return "No input stream delegate found";
			}
			return base.GetSendError();
		}

		private Uri GetUri()
		{
			Dictionary<string, string> queryParams = new Dictionary<string, string>(base.Options.QueryParams);
			Uri uri = WitRequestSettings.GetUri(this.Configuration, this.Path, queryParams);
			if (this.onCustomizeUri != null)
			{
				uri = this.onCustomizeUri(new UriBuilder(uri));
			}
			return uri;
		}

		private Dictionary<string, string> GetHeaders()
		{
			Dictionary<string, string> headers = WitRequestSettings.GetHeaders(this.Configuration, base.Options, false);
			if (this.onProvideCustomHeaders != null)
			{
				Delegate[] invocationList = this.onProvideCustomHeaders.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					Dictionary<string, string> dictionary = ((WitRequest.OnProvideCustomHeadersEvent)invocationList[i])();
					if (dictionary != null)
					{
						foreach (string key in dictionary.Keys)
						{
							headers[key] = dictionary[key];
						}
					}
				}
			}
			return headers;
		}

		protected override void HandleSend()
		{
			this.HasResponseStarted = false;
			this._bytesWritten = 0;
			this._requestStartTime = DateTime.UtcNow;
			this._requestThread = new Thread(delegate()
			{
				WitRequest.<<HandleSend>b__89_0>d <<HandleSend>b__89_0>d;
				<<HandleSend>b__89_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<HandleSend>b__89_0>d.<>4__this = this;
				<<HandleSend>b__89_0>d.<>1__state = -1;
				<<HandleSend>b__89_0>d.<>t__builder.Start<WitRequest.<<HandleSend>b__89_0>d>(ref <<HandleSend>b__89_0>d);
			});
			this._requestThread.Start();
		}

		private void SetupSend(out Uri uri, out Dictionary<string, string> headers, CorrelationID correlationID)
		{
			this.Logger.CorrelationID = correlationID;
			uri = this.GetUri();
			this._canSetPath = false;
			this.Logger.Verbose(correlationID, "Setup request with URL: {0}", new object[]
			{
				uri
			});
			headers = this.GetHeaders();
			WitRequest.PreSendRequestDelegate preSendRequestDelegate = WitRequest.onPreSendRequest;
			if (preSendRequestDelegate == null)
			{
				return;
			}
			preSendRequestDelegate(ref uri, out headers);
		}

		private Task StartThreadedRequest(CorrelationID correlationID)
		{
			WitRequest.<StartThreadedRequest>d__91 <StartThreadedRequest>d__;
			<StartThreadedRequest>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartThreadedRequest>d__.<>4__this = this;
			<StartThreadedRequest>d__.correlationID = correlationID;
			<StartThreadedRequest>d__.<>1__state = -1;
			<StartThreadedRequest>d__.<>t__builder.Start<WitRequest.<StartThreadedRequest>d__91>(ref <StartThreadedRequest>d__);
			return <StartThreadedRequest>d__.<>t__builder.Task;
		}

		private DateTime GetLastUpdate()
		{
			return this._timeoutLastUpdate;
		}

		private Task WaitForTimeout()
		{
			WitRequest.<WaitForTimeout>d__94 <WaitForTimeout>d__;
			<WaitForTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTimeout>d__.<>4__this = this;
			<WaitForTimeout>d__.<>1__state = -1;
			<WaitForTimeout>d__.<>t__builder.Start<WitRequest.<WaitForTimeout>d__94>(ref <WaitForTimeout>d__);
			return <WaitForTimeout>d__.<>t__builder.Task;
		}

		private void HandleWriteStream(IAsyncResult ar)
		{
			try
			{
				Stream stream = this._request.EndGetRequestStream(ar);
				this._bytesWritten = 0;
				if (this.postData != null && this.postData.Length != 0)
				{
					this._bytesWritten += this.postData.Length;
					stream.Write(this.postData, 0, this.postData.Length);
					stream.Close();
				}
				else
				{
					this.IsInputStreamReady = true;
					this._writeStream = stream;
					if (this.OnInputStreamReady != null)
					{
						base.MainThreadCallback(delegate
						{
							Action<WitRequest> action = this.onInputStreamReady;
							if (action != null)
							{
								action(this);
							}
							this.OnInputStreamReady();
						});
					}
				}
			}
			catch (WebException ex)
			{
				WebException e3 = ex;
				WebException e = e3;
				if (e.Status != WebExceptionStatus.RequestCanceled && e.Status != WebExceptionStatus.Timeout && base.StatusCode == 0)
				{
					base.MainThreadCallback(delegate
					{
						this.HandleFailure((int)e.Status, e.ToString());
					});
				}
			}
			catch (Exception ex2)
			{
				Exception e2 = ex2;
				Exception e = e2;
				if (base.StatusCode == 0)
				{
					base.MainThreadCallback(delegate
					{
						this.HandleFailure(-1, e.ToString());
					});
				}
			}
		}

		public void Write(byte[] data, int offset, int length)
		{
			if (!this.IsInputStreamReady || data == null || length == 0)
			{
				return;
			}
			try
			{
				this._writeStream.Write(data, offset, length);
				this._bytesWritten += length;
				if (this.audioDurationTracker != null)
				{
					this.audioDurationTracker.AddBytes((long)length);
				}
			}
			catch (ObjectDisposedException)
			{
				this._writeStream = null;
			}
			catch (Exception)
			{
				return;
			}
			if (this.WaitingForPost())
			{
				base.MainThreadCallback(delegate
				{
					this.Cancel("Stream was closed with no data written.");
				});
			}
		}

		private void HandleResponse(IAsyncResult asyncResult)
		{
			this.HasResponseStarted = true;
			string text = "";
			int statusCode = 200;
			string error = null;
			try
			{
				using (WebResponse webResponse = this._request.EndGetResponse(asyncResult))
				{
					HttpWebResponse httpWebResponse = webResponse as HttpWebResponse;
					int statusCode2 = (int)httpWebResponse.StatusCode;
					if (statusCode != statusCode2)
					{
						statusCode = statusCode2;
						error = httpWebResponse.StatusDescription;
					}
					else
					{
						using (Stream responseStream = httpWebResponse.GetResponseStream())
						{
							text = this.ProcessStreamResponses(responseStream);
						}
					}
				}
			}
			catch (JSONParseException arg)
			{
				statusCode = -5;
				error = string.Format("Server returned invalid data.\n\n{0}", arg);
			}
			catch (WebException ex)
			{
				if (ex.Status != WebExceptionStatus.RequestCanceled && ex.Status != WebExceptionStatus.Timeout)
				{
					statusCode = (int)ex.Status;
					error = ex.ToString();
					HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
					if (httpWebResponse2 != null)
					{
						statusCode = (int)httpWebResponse2.StatusCode;
						try
						{
							using (Stream responseStream2 = httpWebResponse2.GetResponseStream())
							{
								if (responseStream2 != null)
								{
									using (StreamReader streamReader = new StreamReader(responseStream2))
									{
										text = streamReader.ReadToEnd();
										if (!string.IsNullOrEmpty(text))
										{
											this.ProcessStringResponses(text);
										}
									}
								}
							}
						}
						catch (JSONParseException)
						{
						}
						catch (Exception)
						{
						}
					}
				}
			}
			catch (Exception ex2)
			{
				statusCode = -1;
				error = ex2.ToString();
			}
			this.CloseRequestStream();
			this.HasResponseStarted = false;
			if (!base.IsActive)
			{
				return;
			}
			if (statusCode != 200 && !string.IsNullOrEmpty(text))
			{
				WitResponseNode witResponseNode = JsonConvert.DeserializeToken(text);
				if (witResponseNode != null && witResponseNode.AsObject.HasChild("error"))
				{
					error = witResponseNode["error"].Value;
				}
			}
			base.MainThreadCallback(delegate
			{
				if (statusCode != 200)
				{
					this.HandleFailure(statusCode, error);
					return;
				}
				if (this.ResponseData == null && !this.IsDecoding)
				{
					error = "Server did not return a valid json response.";
					this.HandleFailure(error);
					return;
				}
				this.MakeLastResponseFinal();
			});
		}

		private string ProcessStreamResponses(Stream stream)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(stream))
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (!streamReader.EndOfStream)
				{
					string text = streamReader.ReadLine();
					if (!string.IsNullOrEmpty(text))
					{
						stringBuilder.Append(text);
						if (string.Equals(text, "}"))
						{
							this.ProcessStringResponse(stringBuilder.ToString());
							stringBuilder.Clear();
						}
					}
				}
				if (stringBuilder.Length > 0)
				{
					this.ProcessStringResponse(stringBuilder.ToString());
					result = stringBuilder.ToString();
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		private void ProcessStringResponses(string stringResponse)
		{
			foreach (string stringResponse2 in stringResponse.Split(new string[]
			{
				"\r\n"
			}, StringSplitOptions.RemoveEmptyEntries))
			{
				this.ProcessStringResponse(stringResponse2);
			}
		}

		private void ProcessStringResponse(string stringResponse)
		{
			this._timeoutLastUpdate = DateTime.UtcNow;
			this.HandleRawResponse(stringResponse, false);
		}

		protected override void OnRawResponse(string rawResponse)
		{
			base.MainThreadCallback(delegate
			{
				this.<>n__0(rawResponse);
				Action<string> action = this.onRawResponse;
				if (action == null)
				{
					return;
				}
				action(rawResponse);
			});
		}

		protected override void OnPartialTranscription()
		{
			base.OnPartialTranscription();
			Action<string> action = this.onPartialTranscription;
			if (action == null)
			{
				return;
			}
			action(base.Transcription);
		}

		protected override void OnFullTranscription()
		{
			base.OnFullTranscription();
			Action<string> action = this.onFullTranscription;
			if (action == null)
			{
				return;
			}
			action(base.Transcription);
		}

		protected override void OnPartialResponse(WitResponseNode responseNode)
		{
			base.OnPartialResponse(responseNode);
			Action<WitRequest> action = this.onPartialResponse;
			if (action == null)
			{
				return;
			}
			action(this);
		}

		private bool WaitingForPost()
		{
			return this.IsPost && this._bytesWritten == 0 && base.StatusCode == 0;
		}

		protected override bool HasSentAudio()
		{
			return this.IsPost && this._bytesWritten > 0;
		}

		private void CloseRequestStream()
		{
			if (this.WaitingForPost())
			{
				this.Cancel("Request was closed with no audio captured.");
				return;
			}
			this.CloseActiveStream();
		}

		private void CloseActiveStream()
		{
			this.IsInputStreamReady = false;
			object streamLock = this._streamLock;
			lock (streamLock)
			{
				if (this._writeStream != null)
				{
					try
					{
						this._writeStream.Close();
					}
					catch (Exception ex)
					{
						this.Logger.Warning("Write Stream - Close Failed\n{0}", new object[]
						{
							ex
						});
					}
					this._writeStream = null;
				}
			}
			if (this._requestThread != null)
			{
				this._requestThread.Abort();
				this._requestThread = null;
			}
		}

		protected override void HandleCancel()
		{
			this.CloseActiveStream();
			if (this._request != null)
			{
				this._request.Abort();
				this._request = null;
			}
		}

		protected override void OnComplete()
		{
			base.OnComplete();
			if (this._writeStream != null)
			{
				this.CloseActiveStream();
			}
			if (this._request != null)
			{
				this._request.Abort();
				this._request = null;
			}
			Action<WitRequest> action = this.onResponse;
			if (action != null)
			{
				action(this);
			}
			this.onResponse = null;
		}

		private string _path;

		private bool _canSetPath = true;

		public byte[] postData;

		public string postContentType;

		public string forcedHttpMethodType;

		public AudioDurationTracker audioDurationTracker;

		private HttpWebRequest _request;

		private Stream _writeStream;

		private object _streamLock = new object();

		private int _bytesWritten;

		private DateTime _requestStartTime;

		private ConcurrentQueue<byte[]> _writeBuffer = new ConcurrentQueue<byte[]>();

		[Obsolete("Deprecated for Events.OnRawResponse")]
		public Action<string> onRawResponse;

		[Obsolete("Deprecated for WitVRequest.OnProvideCustomUri")]
		public WitRequest.OnCustomizeUriEvent onCustomizeUri;

		public static WitRequest.PreSendRequestDelegate onPreSendRequest;

		private bool _initialized;

		private Thread _requestThread;

		private DateTime _timeoutLastUpdate;

		public delegate Dictionary<string, string> OnProvideCustomHeadersEvent();

		public delegate Uri OnCustomizeUriEvent(UriBuilder uriBuilder);

		public delegate void PreSendRequestDelegate(ref Uri src_uri, out Dictionary<string, string> headers);
	}
}
