using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Lib.Wit.Runtime.Utilities.Logging;
using Meta.Voice.Logging;
using Meta.Voice.Net.Encoding.Wit;
using Meta.Voice.Net.PubSub;
using Meta.WitAi.Attributes;
using Meta.WitAi.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.Voice.Net.WebSockets
{
	[LogCategory(LogCategory.Network, LogCategory.WebSockets)]
	public class WitWebSocketAdapter : MonoBehaviour, IPubSubAdapter, ILogSource, IWitInspectorTools
	{
		public IVLogger Logger { get; } = LoggerRegistry.Instance.GetLogger(LogCategory.WebSockets, null);

		public IWitWebSocketClientProvider WebSocketProvider
		{
			get
			{
				return this._webSocketProvider as IWitWebSocketClientProvider;
			}
		}

		public IWitWebSocketClient WebSocketClient { get; private set; }

		public PubSubSettings Settings
		{
			get
			{
				return this._settings;
			}
			set
			{
				this.SetSettings(value);
			}
		}

		public PubSubSubscriptionState SubscriptionState { get; private set; }

		public event Action<PubSubSubscriptionState> OnTopicSubscriptionStateChange;

		public UnityEvent OnSubscribed { get; } = new UnityEvent();

		public UnityEvent OnUnsubscribed { get; } = new UnityEvent();

		public event WitWebSocketResponseProcessor OnProcessForwardedResponse;

		public event Action<IWitWebSocketRequest> OnRequestGenerated;

		protected virtual void OnEnable()
		{
			this._active = true;
			this.SetClientProvider(this.WebSocketProvider);
			this.Connect();
		}

		protected virtual bool RaiseProcessForwardedResponse(string topicId, string requestId, string clientUserId, WitChunk responseChunk)
		{
			if (!this.Settings.IsSubscribedTopicId(topicId))
			{
				return false;
			}
			WitWebSocketResponseProcessor onProcessForwardedResponse = this.OnProcessForwardedResponse;
			return onProcessForwardedResponse != null && onProcessForwardedResponse(topicId, requestId, clientUserId, responseChunk);
		}

		protected virtual void HandleRequestGenerated(string topicId, IWitWebSocketRequest request)
		{
			if (!this.Settings.IsSubscribedTopicId(topicId))
			{
				return;
			}
			Action<IWitWebSocketRequest> onRequestGenerated = this.OnRequestGenerated;
			if (onRequestGenerated == null)
			{
				return;
			}
			onRequestGenerated(request);
		}

		protected virtual void OnDisable()
		{
			this._active = false;
			this.Disconnect();
		}

		protected virtual void OnDestroy()
		{
			this.WebSocketClient = null;
			this.Disconnect();
		}

		public void SetClientProvider(IWitWebSocketClientProvider clientProvider)
		{
			IWitWebSocketClient witWebSocketClient = (clientProvider != null) ? clientProvider.WebSocketClient : null;
			if (this.WebSocketClient != null && this.WebSocketClient.Equals(witWebSocketClient))
			{
				return;
			}
			if (this._active)
			{
				this.Disconnect();
			}
			this._webSocketProvider = (clientProvider as Object);
			this.WebSocketClient = witWebSocketClient;
			if (clientProvider != null && this._webSocketProvider == null)
			{
				this.Logger.Warning("SetClientProvider failed\nReason: {0} does not inherit from UnityEngine.Object", new object[]
				{
					clientProvider.GetType()
				});
			}
			if (this._active)
			{
				this.Connect();
			}
		}

		private void Connect()
		{
			if (this.WebSocketClient == null || this._connected)
			{
				return;
			}
			this._connected = true;
			this.WebSocketClient.OnTopicSubscriptionStateChange += this.ApplySubscriptionPerTopic;
			this.WebSocketClient.OnProcessForwardedResponse += this.RaiseProcessForwardedResponse;
			this.WebSocketClient.OnTopicRequestTracked += this.HandleRequestGenerated;
			this.WebSocketClient.Connect();
			this.Subscribe();
		}

		private void Disconnect()
		{
			if (this.WebSocketClient == null || !this._connected)
			{
				return;
			}
			this.Unsubscribe();
			this._connected = false;
			this.WebSocketClient.Disconnect();
			this.WebSocketClient.OnTopicSubscriptionStateChange -= this.ApplySubscriptionPerTopic;
			this.WebSocketClient.OnProcessForwardedResponse -= this.RaiseProcessForwardedResponse;
			this.WebSocketClient.OnTopicRequestTracked -= this.HandleRequestGenerated;
		}

		public void SendRequest(IWitWebSocketRequest request)
		{
			request.TopicId = this.Settings.PubSubTopicId;
			request.PublishOptions = this.Settings.PublishOptions;
			this.WebSocketClient.SendRequest(request);
		}

		public void SetSettings(PubSubSettings settings)
		{
			if (this.Settings.Equals(settings))
			{
				return;
			}
			this.Unsubscribe();
			this.Logger.Verbose("Topic set to {0}\nFrom: {1}", settings.PubSubTopicId ?? "Null", this.Settings.PubSubTopicId ?? "Null", null, null, "SetSettings", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketAdapter.cs", 241);
			this._settings = settings;
			this.Subscribe();
		}

		private void Unsubscribe()
		{
			string pubSubTopicId = this.Settings.PubSubTopicId;
			if (string.IsNullOrEmpty(pubSubTopicId) || !this._connected)
			{
				return;
			}
			this.Logger.Verbose("Unsubscribe from topic: {0}", pubSubTopicId, null, null, null, "Unsubscribe", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketAdapter.cs", 261);
			Dictionary<string, string> subscribeTopics = this.Settings.GetSubscribeTopics();
			foreach (string topicId in subscribeTopics.Values)
			{
				this.ApplySubscriptionPerTopic(topicId, PubSubSubscriptionState.Unsubscribing);
			}
			foreach (string topicId2 in subscribeTopics.Values)
			{
				this.WebSocketClient.Unsubscribe(topicId2);
				this.ApplySubscriptionPerTopic(topicId2, PubSubSubscriptionState.NotSubscribed);
			}
			this._subscriptionsPerTopic.Clear();
		}

		private void Subscribe()
		{
			string pubSubTopicId = this.Settings.PubSubTopicId;
			if (string.IsNullOrEmpty(pubSubTopicId) || !this._connected)
			{
				return;
			}
			this.Logger.Verbose("Subscribe to topic: {0}", pubSubTopicId, null, null, null, "Subscribe", ".\\Library\\PackageCache\\com.meta.xr.sdk.voice@d3f6f37b8e1c\\Lib\\Wit.ai\\Scripts\\Runtime\\Net\\WebSockets\\WitWebSocketAdapter.cs", 291);
			foreach (string topicId in this.Settings.GetSubscribeTopics().Values)
			{
				this.ApplySubscriptionPerTopic(topicId, PubSubSubscriptionState.Subscribing);
				this.WebSocketClient.Subscribe(topicId);
			}
		}

		protected virtual void ApplySubscriptionPerTopic(string topicId, PubSubSubscriptionState subscriptionState)
		{
			if (!this.Settings.IsSubscribedTopicId(topicId))
			{
				return;
			}
			if (this._subscriptionsPerTopic.ContainsKey(topicId) && this._subscriptionsPerTopic[topicId] == subscriptionState)
			{
				return;
			}
			this._subscriptionsPerTopic[topicId] = subscriptionState;
			this.RefreshSubscriptionState();
		}

		private void RefreshSubscriptionState()
		{
			this.SetSubscriptionState(this.DetermineSubscriptionState());
		}

		protected PubSubSubscriptionState DetermineSubscriptionState()
		{
			PubSubSubscriptionState result = PubSubSubscriptionState.NotSubscribed;
			bool flag = this._subscriptionsPerTopic.Keys.Count > 0;
			foreach (string key in this._subscriptionsPerTopic.Keys)
			{
				PubSubSubscriptionState pubSubSubscriptionState;
				if (this._subscriptionsPerTopic.TryGetValue(key, out pubSubSubscriptionState))
				{
					if (pubSubSubscriptionState == PubSubSubscriptionState.SubscribeError || pubSubSubscriptionState == PubSubSubscriptionState.UnsubscribeError)
					{
						return pubSubSubscriptionState;
					}
					if (flag && pubSubSubscriptionState != PubSubSubscriptionState.Subscribed)
					{
						flag = false;
					}
					if (pubSubSubscriptionState == PubSubSubscriptionState.Subscribing || pubSubSubscriptionState == PubSubSubscriptionState.Unsubscribing)
					{
						result = pubSubSubscriptionState;
					}
				}
			}
			if (flag)
			{
				return PubSubSubscriptionState.Subscribed;
			}
			return result;
		}

		private void SetSubscriptionState(PubSubSubscriptionState newSubState)
		{
			if (this.SubscriptionState == newSubState)
			{
				return;
			}
			this.SubscriptionState = newSubState;
			Action<PubSubSubscriptionState> onTopicSubscriptionStateChange = this.OnTopicSubscriptionStateChange;
			if (onTopicSubscriptionStateChange != null)
			{
				onTopicSubscriptionStateChange(this.SubscriptionState);
			}
			if (this.SubscriptionState != PubSubSubscriptionState.Subscribed)
			{
				if (this.SubscriptionState == PubSubSubscriptionState.NotSubscribed)
				{
					UnityEvent onUnsubscribed = this.OnUnsubscribed;
					if (onUnsubscribed == null)
					{
						return;
					}
					onUnsubscribed.Invoke();
				}
				return;
			}
			UnityEvent onSubscribed = this.OnSubscribed;
			if (onSubscribed == null)
			{
				return;
			}
			onSubscribed.Invoke();
		}

		[ObjectType(typeof(IWitWebSocketClientProvider), new Type[]
		{

		})]
		[SerializeField]
		private Object _webSocketProvider;

		private PubSubSettings _settings;

		private bool _connected;

		private bool _active;

		private ConcurrentDictionary<string, PubSubSubscriptionState> _subscriptionsPerTopic = new ConcurrentDictionary<string, PubSubSubscriptionState>();
	}
}
