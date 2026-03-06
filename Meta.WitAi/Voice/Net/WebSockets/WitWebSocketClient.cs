using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.Voice.Net.Encoding.Wit;
using Meta.Voice.Net.PubSub;
using Meta.Voice.Net.WebSockets.Requests;
using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Json;
using Meta.WitAi.Requests;
using UnityEngine;

namespace Meta.Voice.Net.WebSockets
{
	[LogCategory(LogCategory.Network)]
	public sealed class WitWebSocketClient : IWitWebSocketClient, IPubSubSubscriber, ILogSource
	{
		public WitWebSocketSettings Settings { get; }

		public string ConnectionRequestId
		{
			get
			{
				return this.Options.RequestId;
			}
		}

		public WitWebSocketConnectionState ConnectionState { get; private set; }

		public bool IsAuthenticated { get; private set; }

		public bool IsUploading
		{
			get
			{
				return this._uploadCount > 0;
			}
		}

		public bool IsDownloading
		{
			get
			{
				return this._downloadCount > 0;
			}
		}

		public bool IsReferenced
		{
			get
			{
				return this.ReferenceCount > 0;
			}
		}

		public bool IsReconnecting
		{
			get
			{
				return this.IsReferenced && this.ConnectionState == WitWebSocketConnectionState.Disconnected && (this.Settings.ReconnectAttempts < 0 || this.FailedConnectionAttempts <= this.Settings.ReconnectAttempts);
			}
		}

		public int ReferenceCount { get; private set; }

		public int FailedConnectionAttempts { get; private set; }

		public DateTime LastResponseTime { get; private set; }

		public event Action<WitWebSocketConnectionState> OnConnectionStateChanged;

		public event WitWebSocketResponseProcessor OnProcessForwardedResponse;

		public TaskCompletionSource<bool> ConnectionCompletion { get; private set; } = new TaskCompletionSource<bool>();

		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.Network, null);

		public Dictionary<string, IWitWebSocketRequest> Requests
		{
			get
			{
				return new Dictionary<string, IWitWebSocketRequest>(this._requests);
			}
		}

		public WitWebSocketClient(WitWebSocketSettings settings)
		{
			this.Settings = settings;
		}

		public WitWebSocketClient(IWitRequestConfiguration configuration) : this(new WitWebSocketSettings(configuration))
		{
		}

		private void SetConnectionState(WitWebSocketConnectionState newConnectionState)
		{
			if (newConnectionState == this.ConnectionState)
			{
				return;
			}
			this.ConnectionState = newConnectionState;
			this.Logger.Info(this.ConnectionState.ToString(), null, null, null, null, "SetConnectionState", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 174);
			Action<WitWebSocketConnectionState> onConnectionStateChanged = this.OnConnectionStateChanged;
			if (onConnectionStateChanged != null)
			{
				onConnectionStateChanged(this.ConnectionState);
			}
			if (this.ConnectionState == WitWebSocketConnectionState.Connected)
			{
				if (!this.ConnectionCompletion.Task.IsCompleted)
				{
					this.ConnectionCompletion.SetResult(true);
					return;
				}
			}
			else if (this.ConnectionState == WitWebSocketConnectionState.Disconnected)
			{
				TaskCompletionSource<bool> connectionCompletion = this.ConnectionCompletion;
				this.ConnectionCompletion = new TaskCompletionSource<bool>();
				if (!connectionCompletion.Task.IsCompleted)
				{
					connectionCompletion.SetResult(false);
				}
			}
		}

		public void Connect()
		{
			int referenceCount = this.ReferenceCount;
			this.ReferenceCount = referenceCount + 1;
			if (!this.IsReferenced)
			{
				return;
			}
			this.ConnectSafely();
		}

		private void ConnectSafely()
		{
			if (this.ConnectionState == WitWebSocketConnectionState.Connecting || this.ConnectionState == WitWebSocketConnectionState.Connected)
			{
				return;
			}
			this.SetConnectionState(WitWebSocketConnectionState.Connecting);
			this.WaitForConnectionTimeout().WrapErrors();
			ThreadUtility.BackgroundAsync(this.Logger, new Func<Task>(this.ConnectAsync)).WrapErrors();
		}

		private Task ConnectAsync()
		{
			WitWebSocketClient.<ConnectAsync>d__61 <ConnectAsync>d__;
			<ConnectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ConnectAsync>d__.<>4__this = this;
			<ConnectAsync>d__.<>1__state = -1;
			<ConnectAsync>d__.<>t__builder.Start<WitWebSocketClient.<ConnectAsync>d__61>(ref <ConnectAsync>d__);
			return <ConnectAsync>d__.<>t__builder.Task;
		}

		private Task WaitForConnectionTimeout()
		{
			WitWebSocketClient.<WaitForConnectionTimeout>d__62 <WaitForConnectionTimeout>d__;
			<WaitForConnectionTimeout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitForConnectionTimeout>d__.<>4__this = this;
			<WaitForConnectionTimeout>d__.<>1__state = -1;
			<WaitForConnectionTimeout>d__.<>t__builder.Start<WitWebSocketClient.<WaitForConnectionTimeout>d__62>(ref <WaitForConnectionTimeout>d__);
			return <WaitForConnectionTimeout>d__.<>t__builder.Task;
		}

		private IWebSocket GenerateWebSocket(string url, Dictionary<string, string> headers)
		{
			if (this.Settings.WebSocketProvider != null)
			{
				IWebSocket webSocket = this.Settings.WebSocketProvider.GetWebSocket(url, headers);
				if (webSocket != null)
				{
					return webSocket;
				}
			}
			return new NativeWebSocketWrapper(url, headers);
		}

		private void HandleSocketError(string errorMessage)
		{
			if (this.ConnectionState == WitWebSocketConnectionState.Connecting)
			{
				this.HandleSetupFailed(errorMessage);
				return;
			}
			this.Logger.Warning("Socket Error\nMessage: {0}", new object[]
			{
				errorMessage
			});
		}

		private void HandleSocketConnected()
		{
			if (this.ConnectionState != WitWebSocketConnectionState.Connecting)
			{
				this.HandleSetupFailed(string.Format("State changed to {0} during connection.", this.ConnectionState));
				return;
			}
			if (this._socket == null)
			{
				this.HandleSetupFailed("WebSocket client no longer exists.");
				return;
			}
			if (this._socket.State != WitWebSocketConnectionState.Connected)
			{
				this.HandleSetupFailed(string.Format("Socket is {0}", this._socket.State));
				return;
			}
			if (this.ConnectionState == WitWebSocketConnectionState.Connected)
			{
				return;
			}
			ThreadUtility.BackgroundAsync(this.Logger, new Func<Task>(this.SetupAsync)).WrapErrors();
		}

		private Task SetupAsync()
		{
			WitWebSocketClient.<SetupAsync>d__66 <SetupAsync>d__;
			<SetupAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetupAsync>d__.<>4__this = this;
			<SetupAsync>d__.<>1__state = -1;
			<SetupAsync>d__.<>t__builder.Start<WitWebSocketClient.<SetupAsync>d__66>(ref <SetupAsync>d__);
			return <SetupAsync>d__.<>t__builder.Task;
		}

		private void HandleSetupFailed(string error)
		{
			if (this.ConnectionState == WitWebSocketConnectionState.Connecting)
			{
				this.Logger.Error("Connection Failed\nConnection Request Id: {0}\nMessage: {1}", new object[]
				{
					this.Options.RequestId,
					error
				});
				int failedConnectionAttempts = this.FailedConnectionAttempts;
				this.FailedConnectionAttempts = failedConnectionAttempts + 1;
				if (this.Settings.ReconnectAttempts >= 0 && this.FailedConnectionAttempts > this.Settings.ReconnectAttempts)
				{
					this.Logger.Error("Connection Refused\nConnection Request Id: {0}\nMessage: {1}\nFailed Attempts: {2}", new object[]
					{
						this.Options.RequestId,
						error,
						this.FailedConnectionAttempts
					});
				}
				else
				{
					this.Logger.Warning("Connection Refused - Will Retry\nConnection Request Id: {0}\nMessage: {1}\nFailed Attempts: {2}", new object[]
					{
						this.Options.RequestId,
						error,
						this.FailedConnectionAttempts
					});
				}
				this.ForceDisconnect();
				return;
			}
			this.Logger.Warning("Connection Cancelled\nConnection Request Id: {0}\nMessage: {1}", new object[]
			{
				this.Options.RequestId,
				error
			});
		}

		private void HandleSocketDisconnect(WebSocketCloseCode closeCode)
		{
			if (this.ConnectionState == WitWebSocketConnectionState.Connected)
			{
				this.Logger.Warning("Socket Closed\nConnection Request Id: {0}\nReason: {1}", new object[]
				{
					this.Options.RequestId,
					closeCode
				});
				this.ForceDisconnect();
			}
		}

		public void Disconnect()
		{
			int referenceCount = this.ReferenceCount;
			this.ReferenceCount = referenceCount - 1;
			if (this.IsReferenced)
			{
				return;
			}
			this.ReferenceCount = 0;
			this.ForceDisconnect();
		}

		public void ForceDisconnect()
		{
			if (this.ConnectionState == WitWebSocketConnectionState.Disconnecting || this.ConnectionState == WitWebSocketConnectionState.Disconnected)
			{
				return;
			}
			this.DisconnectAsync().WrapErrors();
		}

		private Task DisconnectAsync()
		{
			WitWebSocketClient.<DisconnectAsync>d__71 <DisconnectAsync>d__;
			<DisconnectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DisconnectAsync>d__.<>4__this = this;
			<DisconnectAsync>d__.<>1__state = -1;
			<DisconnectAsync>d__.<>t__builder.Start<WitWebSocketClient.<DisconnectAsync>d__71>(ref <DisconnectAsync>d__);
			return <DisconnectAsync>d__.<>t__builder.Task;
		}

		private Task BreakdownAsync()
		{
			WitWebSocketClient.<BreakdownAsync>d__72 <BreakdownAsync>d__;
			<BreakdownAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<BreakdownAsync>d__.<>4__this = this;
			<BreakdownAsync>d__.<>1__state = -1;
			<BreakdownAsync>d__.<>t__builder.Start<WitWebSocketClient.<BreakdownAsync>d__72>(ref <BreakdownAsync>d__);
			return <BreakdownAsync>d__.<>t__builder.Task;
		}

		private void Reconnect()
		{
			if (!this.IsReferenced || this.ConnectionState != WitWebSocketConnectionState.Disconnected)
			{
				return;
			}
			if (this.Settings.ReconnectAttempts >= 0 && this.FailedConnectionAttempts > this.Settings.ReconnectAttempts)
			{
				this.Logger.Error("Reconnect Failed\nToo many failed reconnect attempts\nFailures: {0}\nAttempts Allowed: {1}", new object[]
				{
					this.FailedConnectionAttempts,
					this.Settings.ReconnectAttempts
				});
				return;
			}
			ThreadUtility.BackgroundAsync(this.Logger, new Func<Task>(this.WaitAndConnect)).WrapErrors();
		}

		private Task WaitAndConnect()
		{
			WitWebSocketClient.<WaitAndConnect>d__74 <WaitAndConnect>d__;
			<WaitAndConnect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitAndConnect>d__.<>4__this = this;
			<WaitAndConnect>d__.<>1__state = -1;
			<WaitAndConnect>d__.<>t__builder.Start<WitWebSocketClient.<WaitAndConnect>d__74>(ref <WaitAndConnect>d__);
			return <WaitAndConnect>d__.<>t__builder.Task;
		}

		public bool SendRequest(IWitWebSocketRequest request)
		{
			if (!this.TrackRequest(request))
			{
				return false;
			}
			request.HandleUpload(new UploadChunkDelegate(this.SendChunk));
			return true;
		}

		public Task<string> SendRequestAsync(IWitWebSocketRequest request)
		{
			WitWebSocketClient.<SendRequestAsync>d__76 <SendRequestAsync>d__;
			<SendRequestAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<SendRequestAsync>d__.<>4__this = this;
			<SendRequestAsync>d__.request = request;
			<SendRequestAsync>d__.<>1__state = -1;
			<SendRequestAsync>d__.<>t__builder.Start<WitWebSocketClient.<SendRequestAsync>d__76>(ref <SendRequestAsync>d__);
			return <SendRequestAsync>d__.<>t__builder.Task;
		}

		private void SendChunk(string requestId, WitResponseNode requestJsonData, byte[] requestBinaryData)
		{
			ThreadUtility.BackgroundAsync(this.Logger, () => this.SendChunkAsync(requestId, requestJsonData, requestBinaryData));
		}

		private Task SendChunkAsync(string requestId, WitResponseNode requestJsonData, byte[] requestBinaryData)
		{
			WitWebSocketClient.<SendChunkAsync>d__78 <SendChunkAsync>d__;
			<SendChunkAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendChunkAsync>d__.<>4__this = this;
			<SendChunkAsync>d__.requestId = requestId;
			<SendChunkAsync>d__.requestJsonData = requestJsonData;
			<SendChunkAsync>d__.requestBinaryData = requestBinaryData;
			<SendChunkAsync>d__.<>1__state = -1;
			<SendChunkAsync>d__.<>t__builder.Start<WitWebSocketClient.<SendChunkAsync>d__78>(ref <SendChunkAsync>d__);
			return <SendChunkAsync>d__.<>t__builder.Task;
		}

		private void TrySimulateError(IWitWebSocketRequest request)
		{
			if (request.SimulatedErrorType == VoiceErrorSimulationType.Disconnect)
			{
				this.Logger.Info("[DEBUG] Simulating Abnormal Disconnect\nState: {0}\nRequest: {1}", this.ConnectionState, request.RequestId, null, null, "TrySimulateError", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 769);
				this.HandleSocketDisconnect(WebSocketCloseCode.Abnormal);
				return;
			}
			if (request.SimulatedErrorType != VoiceErrorSimulationType.Server)
			{
				return;
			}
			this.Logger.Info("[DEBUG] Simulating Server Error\nRequest: {0}", request.RequestId, null, null, null, "TrySimulateError", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 777);
			WitResponseClass witResponseClass = new WitResponseClass();
			witResponseClass["client_request_id"] = new WitResponseData(request.RequestId);
			witResponseClass["client_user_id"] = new WitResponseData(request.ClientUserId);
			witResponseClass["operation_id"] = new WitResponseData(request.OperationId);
			witResponseClass["code"] = new WitResponseData(500);
			witResponseClass["error"] = new WitResponseData("Simulated Server Error");
			request.HandleDownload(witResponseClass.ToString(), witResponseClass, null);
		}

		private byte[] EncodeChunk(WitChunk chunk)
		{
			return WitChunkConverter.Encode(chunk);
		}

		private void HandleSocketResponse(byte[] rawBytes, int offset, int length)
		{
			this._downloadCount++;
			this._decoder.Decode(rawBytes, offset, length, new Action<WitChunk>(this.ApplyDecodedChunk), null);
			this._downloadCount--;
		}

		private void ApplyDecodedChunk(WitChunk chunk)
		{
			if (this.Settings.VerboseJsonLogging)
			{
				this.Logger.Verbose("Downloaded Chunk:\n{0}\n", chunk.jsonString, null, null, null, "ApplyDecodedChunk", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 816);
			}
			WitResponseNode jsonData = chunk.jsonData;
			WitResponseNode witResponseNode = (jsonData != null) ? jsonData["client_request_id"] : null;
			if (string.IsNullOrEmpty(witResponseNode))
			{
				if (string.IsNullOrEmpty(this._lastRequestId))
				{
					this.Logger.Error("Download Chunk Failed\nError: no request id found in chunk\nJson: {0}", new object[]
					{
						chunk.jsonString ?? "Null"
					});
					return;
				}
				witResponseNode = this._lastRequestId;
				if (chunk.jsonData == null)
				{
					chunk.jsonData = new WitResponseClass();
					chunk.jsonData["client_request_id"] = witResponseNode;
				}
			}
			else
			{
				this._lastRequestId = witResponseNode;
			}
			IWitWebSocketRequest witWebSocketRequest;
			if (!this._requests.TryGetValue(witResponseNode, out witWebSocketRequest))
			{
				this.ProcessForwardedResponse(witResponseNode, chunk);
				this._requests.TryGetValue(witResponseNode, out witWebSocketRequest);
			}
			if (witWebSocketRequest == null)
			{
				return;
			}
			if (witWebSocketRequest.SimulatedErrorType != (VoiceErrorSimulationType)(-1))
			{
				return;
			}
			try
			{
				witWebSocketRequest.HandleDownload(chunk.jsonString, chunk.jsonData, chunk.binaryData);
			}
			catch (Exception ex)
			{
				this.Logger.Error("Request HandleDownload method exception caught\n{0}\n\n{1}\n", new object[]
				{
					witWebSocketRequest,
					ex
				});
				this.UntrackRequest(witWebSocketRequest);
			}
		}

		private void ProcessForwardedResponse(string requestId, WitChunk chunk)
		{
			if (this._untrackedRequests.Contains(requestId))
			{
				this.Logger.Verbose("Process Forwarded Response - Ignored\nReason: Request has been cancelled\nRequest Id: {0}\nJson:\n{1}", requestId, chunk.jsonString ?? "Null", null, null, "ProcessForwardedResponse", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 884);
				return;
			}
			string value = chunk.jsonData["topic"].Value;
			if (string.IsNullOrEmpty(value))
			{
				this.Logger.Warning("Process Forwarded Response - Failed\nReason: No topic id provided in response\nRequest Id: {0}\nJson:\n{1}", new object[]
				{
					requestId,
					chunk.jsonString ?? "Null"
				});
				return;
			}
			PubSubSubscriptionState topicSubscriptionState = this.GetTopicSubscriptionState(value);
			if (topicSubscriptionState != PubSubSubscriptionState.Subscribed && topicSubscriptionState != PubSubSubscriptionState.Subscribing)
			{
				this.Logger.Warning("Process Forwarded Response - Failed\nReason: Topic id is not currently subscribed to\nTopic Id: {0}\nRequest Id: {1}\nJson:\n{2}", new object[]
				{
					value,
					requestId,
					chunk.jsonString ?? "Null"
				});
				return;
			}
			string text = chunk.jsonData["client_user_id"].Value;
			if (string.IsNullOrEmpty(text))
			{
				text = "unknown";
			}
			bool flag = false;
			if (this.OnProcessForwardedResponse != null)
			{
				Delegate[] invocationList = this.OnProcessForwardedResponse.GetInvocationList();
				for (int i = 0; i < invocationList.Length; i++)
				{
					WitWebSocketResponseProcessor witWebSocketResponseProcessor = invocationList[i] as WitWebSocketResponseProcessor;
					if (witWebSocketResponseProcessor != null)
					{
						flag |= witWebSocketResponseProcessor(value, requestId, text, chunk);
					}
				}
			}
			if (!flag)
			{
				this.Logger.Warning("Process Forwarded Response - Ignored\nReason: No OnProcessForwardedResponse events handled the response\nTopic Id: {0}\nRequest Id: {1}\nClient User Id: {2}", new object[]
				{
					value,
					requestId,
					text
				});
			}
		}

		public bool TrackRequest(IWitWebSocketRequest request)
		{
			if (request == null)
			{
				return false;
			}
			Dictionary<string, IWitWebSocketRequest> requests = this._requests;
			lock (requests)
			{
				if (this._requests.ContainsValue(request))
				{
					return false;
				}
				this._requests[request.RequestId] = request;
			}
			request.TimeoutMs = ((request.TimeoutMs > 0) ? request.TimeoutMs : this.Settings.RequestTimeoutMs);
			request.OnComplete = (Action<IWitWebSocketRequest>)Delegate.Combine(request.OnComplete, new Action<IWitWebSocketRequest>(this.CompleteRequestTracking));
			this.Logger.Info(string.Format("Track Request\n{0}", request), null, null, null, null, "TrackRequest", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 962);
			string topicId = request.TopicId;
			if (this.GetTopicSubscriptionState(topicId) != PubSubSubscriptionState.NotSubscribed)
			{
				Action<string, IWitWebSocketRequest> onTopicRequestTracked = this.OnTopicRequestTracked;
				if (onTopicRequestTracked != null)
				{
					onTopicRequestTracked(topicId, request);
				}
			}
			return true;
		}

		private void CompleteRequestTracking(IWitWebSocketRequest request)
		{
			this.UntrackRequest(request);
			WitWebSocketSubscriptionRequest witWebSocketSubscriptionRequest = request as WitWebSocketSubscriptionRequest;
			if (witWebSocketSubscriptionRequest != null)
			{
				this.FinalizeSubscription(witWebSocketSubscriptionRequest);
			}
		}

		public bool UntrackRequest(IWitWebSocketRequest request)
		{
			return request != null && this.UntrackRequest(request.RequestId);
		}

		public bool UntrackRequest(string requestId)
		{
			if (string.IsNullOrEmpty(requestId))
			{
				return false;
			}
			Dictionary<string, IWitWebSocketRequest> requests = this._requests;
			IWitWebSocketRequest witWebSocketRequest;
			lock (requests)
			{
				if (!this._requests.ContainsKey(requestId))
				{
					return false;
				}
				witWebSocketRequest = this._requests[requestId];
				this._requests.Remove(requestId);
				this._untrackedRequests.Add(requestId);
			}
			IWitWebSocketRequest witWebSocketRequest2 = witWebSocketRequest;
			witWebSocketRequest2.OnComplete = (Action<IWitWebSocketRequest>)Delegate.Remove(witWebSocketRequest2.OnComplete, new Action<IWitWebSocketRequest>(this.CompleteRequestTracking));
			if (!witWebSocketRequest.IsComplete)
			{
				if (this.ConnectionState == WitWebSocketConnectionState.Disconnecting || this.ConnectionState == WitWebSocketConnectionState.Disconnected)
				{
					WitResponseClass witResponseClass = new WitResponseClass();
					witResponseClass["client_request_id"] = new WitResponseData(witWebSocketRequest.RequestId);
					witResponseClass["code"] = new WitResponseData(499);
					witResponseClass["error"] = new WitResponseData("WebSocket disconnected");
					witWebSocketRequest.HandleDownload(witResponseClass.ToString(), witResponseClass, null);
				}
				else
				{
					witWebSocketRequest.Cancel();
				}
			}
			this.Logger.Info(string.Format("Untrack Request\n{0}", witWebSocketRequest), null, null, null, null, "UntrackRequest", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 1043);
			return true;
		}

		public event PubSubTopicSubscriptionDelegate OnTopicSubscriptionStateChange;

		public event Action<string, IWitWebSocketRequest> OnTopicRequestTracked;

		public PubSubSubscriptionState GetTopicSubscriptionState(string topicId)
		{
			WitWebSocketClient.PubSubSubscription pubSubSubscription;
			if (!string.IsNullOrEmpty(topicId) && this._subscriptions.TryGetValue(topicId, out pubSubSubscription))
			{
				return pubSubSubscription.state;
			}
			return PubSubSubscriptionState.NotSubscribed;
		}

		public void Subscribe(string topicId)
		{
			this.Subscribe(topicId, false);
		}

		private void Subscribe(string topicId, bool ignoreRefCount)
		{
			if (string.IsNullOrEmpty(topicId))
			{
				return;
			}
			WitWebSocketClient.PubSubSubscription pubSubSubscription;
			if (!this._subscriptions.TryGetValue(topicId, out pubSubSubscription))
			{
				pubSubSubscription = new WitWebSocketClient.PubSubSubscription();
			}
			if (!ignoreRefCount)
			{
				pubSubSubscription.referenceCount++;
			}
			if (this.ConnectionState != WitWebSocketConnectionState.Connected && this.ConnectionState != WitWebSocketConnectionState.Connecting)
			{
				this.SetTopicSubscriptionState(pubSubSubscription, topicId, PubSubSubscriptionState.SubscribeError, "Not connected.  Will retry once connected.");
				return;
			}
			if (pubSubSubscription.state == PubSubSubscriptionState.Subscribing || pubSubSubscription.state == PubSubSubscriptionState.Subscribed)
			{
				return;
			}
			this.SetTopicSubscriptionState(pubSubSubscription, topicId, PubSubSubscriptionState.Subscribing, null);
			WitWebSocketSubscriptionRequest request = new WitWebSocketSubscriptionRequest(topicId, WitWebSocketSubscriptionType.Subscribe);
			this.SendRequest(request);
		}

		public void Unsubscribe(string topicId)
		{
			this.Unsubscribe(topicId, false);
		}

		public void Unsubscribe(string topicId, bool ignoreRefCount)
		{
			if (string.IsNullOrEmpty(topicId))
			{
				return;
			}
			WitWebSocketClient.PubSubSubscription pubSubSubscription;
			if (!this._subscriptions.TryGetValue(topicId, out pubSubSubscription))
			{
				return;
			}
			if (!ignoreRefCount)
			{
				pubSubSubscription.referenceCount = Mathf.Max(0, pubSubSubscription.referenceCount - 1);
			}
			if (pubSubSubscription.referenceCount > 0)
			{
				return;
			}
			if (this.ConnectionState != WitWebSocketConnectionState.Connected)
			{
				this.SetTopicSubscriptionState(pubSubSubscription, topicId, PubSubSubscriptionState.Unsubscribing, null);
				this.SetTopicSubscriptionState(pubSubSubscription, topicId, PubSubSubscriptionState.NotSubscribed, null);
				return;
			}
			if (pubSubSubscription.state == PubSubSubscriptionState.Unsubscribing || pubSubSubscription.state == PubSubSubscriptionState.NotSubscribed)
			{
				return;
			}
			this.SetTopicSubscriptionState(pubSubSubscription, topicId, PubSubSubscriptionState.Unsubscribing, null);
			WitWebSocketSubscriptionRequest request = new WitWebSocketSubscriptionRequest(topicId, WitWebSocketSubscriptionType.Unsubscribe);
			this.SendRequest(request);
		}

		private void FinalizeSubscription(WitWebSocketSubscriptionRequest request)
		{
			string topicId = request.TopicId;
			WitWebSocketClient.PubSubSubscription subscription;
			if (!this._subscriptions.TryGetValue(topicId, out subscription))
			{
				return;
			}
			bool flag = request.SubscriptionType == WitWebSocketSubscriptionType.Subscribe;
			if (!string.IsNullOrEmpty(request.Error))
			{
				PubSubSubscriptionState state = flag ? PubSubSubscriptionState.SubscribeError : PubSubSubscriptionState.UnsubscribeError;
				this.SetTopicSubscriptionState(subscription, topicId, state, request.Error);
				this.WaitAndRetry(flag, topicId).WrapErrors();
				return;
			}
			PubSubSubscriptionState state2 = flag ? PubSubSubscriptionState.Subscribed : PubSubSubscriptionState.NotSubscribed;
			this.SetTopicSubscriptionState(subscription, topicId, state2, null);
		}

		private Task WaitAndRetry(bool subscribing, string topicId)
		{
			WitWebSocketClient.<WaitAndRetry>d__102 <WaitAndRetry>d__;
			<WaitAndRetry>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitAndRetry>d__.<>4__this = this;
			<WaitAndRetry>d__.subscribing = subscribing;
			<WaitAndRetry>d__.topicId = topicId;
			<WaitAndRetry>d__.<>1__state = -1;
			<WaitAndRetry>d__.<>t__builder.Start<WitWebSocketClient.<WaitAndRetry>d__102>(ref <WaitAndRetry>d__);
			return <WaitAndRetry>d__.<>t__builder.Task;
		}

		private void SetTopicSubscriptionState(WitWebSocketClient.PubSubSubscription subscription, string topicId, PubSubSubscriptionState state, string error = null)
		{
			if (subscription.state == state)
			{
				return;
			}
			subscription.state = state;
			if (state == PubSubSubscriptionState.NotSubscribed)
			{
				this._subscriptions.Remove(topicId);
			}
			else
			{
				this._subscriptions[topicId] = subscription;
			}
			if (!string.IsNullOrEmpty(error))
			{
				this.Logger.Warning("Set State Failed\nState: {0}\nError: {1}\nTopic Id: {2}", new object[]
				{
					state,
					error,
					topicId
				});
			}
			else
			{
				this.Logger.Info(string.Format("{0}\nTopic Id: {1}", state, topicId), null, null, null, null, "SetTopicSubscriptionState", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketClient.cs", 1278);
			}
			PubSubTopicSubscriptionDelegate onTopicSubscriptionStateChange = this.OnTopicSubscriptionStateChange;
			if (onTopicSubscriptionStateChange == null)
			{
				return;
			}
			onTopicSubscriptionStateChange(topicId, state);
		}

		public WitRequestOptions Options;

		private string _lastRequestId;

		private int _uploadCount;

		private int _downloadCount;

		private Dictionary<string, IWitWebSocketRequest> _requests = new Dictionary<string, IWitWebSocketRequest>();

		private List<string> _untrackedRequests = new List<string>();

		private IWebSocket _socket;

		private readonly WitChunkConverter _decoder = new WitChunkConverter();

		private Dictionary<string, WitWebSocketClient.PubSubSubscription> _subscriptions = new Dictionary<string, WitWebSocketClient.PubSubSubscription>();

		private class PubSubSubscription
		{
			public PubSubSubscriptionState state;

			public int referenceCount;
		}
	}
}
