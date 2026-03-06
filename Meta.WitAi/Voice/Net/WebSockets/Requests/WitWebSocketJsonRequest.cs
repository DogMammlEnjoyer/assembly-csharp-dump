using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.Voice.Net.PubSub;
using Meta.WitAi;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;

namespace Meta.Voice.Net.WebSockets.Requests
{
	[LogCategory(LogCategory.Requests)]
	public class WitWebSocketJsonRequest : IWitWebSocketRequest, ILogSource
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Requests, null);

		public string RequestId { get; }

		public string ClientUserId { get; }

		public string OperationId { get; }

		public string TopicId { get; set; }

		public PubSubResponseOptions PublishOptions { get; set; }

		public int TimeoutMs { get; set; }

		public bool IsUploading { get; protected set; }

		public bool IsDownloading { get; private set; }

		public bool IsComplete { get; private set; }

		public TaskCompletionSource<bool> Completion { get; } = new TaskCompletionSource<bool>();

		public int Code { get; protected set; }

		public string Error { get; protected set; }

		public VoiceErrorSimulationType SimulatedErrorType { get; internal set; } = (VoiceErrorSimulationType)(-1);

		public WitResponseNode PostData { get; }

		public WitResponseNode ResponseData { get; protected set; }

		public Action<string> OnRawResponse { get; set; }

		public Action<IWitWebSocketRequest> OnFirstResponse { get; set; }

		public Action<IWitWebSocketRequest> OnComplete { get; set; }

		public WitWebSocketJsonRequest(WitResponseNode postData, string requestId = null, string clientUserId = null, string operationId = null)
		{
			this.PostData = postData;
			this.RequestId = (string.IsNullOrEmpty(requestId) ? WitConstants.GetUniqueId() : requestId);
			this.ClientUserId = (string.IsNullOrEmpty(clientUserId) ? WitRequestSettings.LocalClientUserId : clientUserId);
			this.OperationId = (string.IsNullOrEmpty(operationId) ? WitConstants.GetUniqueId() : operationId);
		}

		public virtual void HandleUpload(UploadChunkDelegate uploadChunk)
		{
			if (this.IsUploading)
			{
				return;
			}
			this.IsUploading = true;
			this._uploader = uploadChunk;
			if (!string.IsNullOrEmpty(this.TopicId) && this.PostData != null)
			{
				WitResponseClass witResponseClass = new WitResponseClass();
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				PubSubSettings.GetTopics(dictionary, this.TopicId, this.PublishOptions);
				foreach (KeyValuePair<string, string> keyValuePair in dictionary)
				{
					witResponseClass[keyValuePair.Key] = keyValuePair.Value;
				}
				this.PostData["publish_topics"] = witResponseClass;
			}
			if (!string.IsNullOrEmpty(this.ClientUserId) && this.PostData != null)
			{
				this.PostData["client_user_id"] = this.ClientUserId;
			}
			if (!string.IsNullOrEmpty(this.OperationId) && this.PostData != null)
			{
				this.PostData["operation_id"] = this.OperationId;
			}
			this.UploadChunk(this.PostData, null);
			this.WaitForTimeout().WrapErrors();
		}

		protected void UploadChunk(WitResponseNode uploadJson, byte[] uploadBinary)
		{
			UploadChunkDelegate uploader = this._uploader;
			if (uploader == null)
			{
				return;
			}
			uploader(this.RequestId, uploadJson, uploadBinary);
		}

		protected Task WaitForTimeout()
		{
			WitWebSocketJsonRequest.<WaitForTimeout>d__75 <WaitForTimeout>d__;
			<WaitForTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForTimeout>d__.<>4__this = this;
			<WaitForTimeout>d__.<>1__state = -1;
			<WaitForTimeout>d__.<>t__builder.Start<WitWebSocketJsonRequest.<WaitForTimeout>d__75>(ref <WaitForTimeout>d__);
			return <WaitForTimeout>d__.<>t__builder.Task;
		}

		private DateTime GetTimeoutStart()
		{
			return this._timeoutStart;
		}

		protected void UpdateTimeoutStart()
		{
			this._timeoutStart = DateTime.UtcNow;
		}

		public virtual void Cancel()
		{
			if (this.IsComplete)
			{
				return;
			}
			this.SendAbort("Cancelled");
			this.Code = -6;
			this.Error = "Cancelled";
			this.HandleComplete();
		}

		protected void SendAbort(string reason)
		{
			if (!this.IsUploading || this._uploader == null)
			{
				return;
			}
			WitResponseClass witResponseClass = new WitResponseClass();
			WitResponseClass witResponseClass2 = new WitResponseClass();
			witResponseClass2["abort"] = new WitResponseClass();
			witResponseClass2["abort"]["reason"] = reason;
			witResponseClass["data"] = witResponseClass2;
			UploadChunkDelegate uploader = this._uploader;
			if (uploader == null)
			{
				return;
			}
			uploader(this.RequestId, witResponseClass, null);
		}

		public virtual void HandleDownload(string jsonString, WitResponseNode jsonData, byte[] binaryData)
		{
			if (this.IsDownloading || this.IsComplete)
			{
				return;
			}
			this.HandleDownloadBegin();
			this.ReturnRawResponse(jsonString);
			this.SetResponseData(jsonData);
			this.HandleComplete();
		}

		protected virtual void ReturnRawResponse(string jsonString)
		{
			if (this.OnRawResponse == null)
			{
				return;
			}
			ThreadUtility.CallOnMainThread(delegate()
			{
				this.OnRawResponse(jsonString);
			});
		}

		protected virtual void SetResponseData(WitResponseNode newResponseData)
		{
			this.UpdateTimeoutStart();
			this.ResponseData = newResponseData;
			string value = this.ResponseData["code"].Value;
			if (!string.IsNullOrEmpty(value))
			{
				int code;
				if (int.TryParse(value, out code))
				{
					this.Code = code;
				}
				else
				{
					this.Code = -1;
					this.Logger.Warning("{0} Response Code is not an integer: {1}\n{2}", new object[]
					{
						base.GetType().Name,
						value,
						this
					});
				}
			}
			this.Error = this.ResponseData["error"].Value;
			WitResponseNode witResponseNode = this.ResponseData["topic"];
			string text = (witResponseNode != null) ? witResponseNode.Value : null;
			if (!string.IsNullOrEmpty(text))
			{
				this.TopicId = text;
			}
		}

		protected virtual void HandleDownloadBegin()
		{
			if (this.IsDownloading)
			{
				return;
			}
			this.IsDownloading = true;
			ThreadUtility.CallOnMainThread(new Action(this.RaiseFirstResponse));
		}

		protected virtual void RaiseFirstResponse()
		{
			Action<IWitWebSocketRequest> onFirstResponse = this.OnFirstResponse;
			if (onFirstResponse == null)
			{
				return;
			}
			onFirstResponse(this);
		}

		protected virtual void HandleComplete()
		{
			if (this.IsComplete)
			{
				return;
			}
			this.IsUploading = false;
			this._uploader = null;
			this.IsDownloading = false;
			this.IsComplete = true;
			if (!this.Completion.Task.IsCompleted)
			{
				this.Completion.SetResult(string.IsNullOrEmpty(this.Error));
			}
			ThreadUtility.CallOnMainThread(new Action(this.RaiseComplete));
		}

		protected virtual void RaiseComplete()
		{
			Action<IWitWebSocketRequest> onComplete = this.OnComplete;
			if (onComplete == null)
			{
				return;
			}
			onComplete(this);
		}

		public override string ToString()
		{
			return string.Format("Type: {0}\nRequest Id: {1}\nClient User Id: {2}\nTopic Id: {3}\nError: {4}", new object[]
			{
				base.GetType().Name,
				this.RequestId,
				this.ClientUserId ?? "Null",
				this.TopicId ?? "Null",
				this.Error ?? "Null"
			});
		}

		private UploadChunkDelegate _uploader;

		private DateTime _timeoutStart;
	}
}
