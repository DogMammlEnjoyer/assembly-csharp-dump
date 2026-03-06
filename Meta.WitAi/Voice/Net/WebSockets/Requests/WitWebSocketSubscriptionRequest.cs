using System;
using Meta.WitAi.Json;

namespace Meta.Voice.Net.WebSockets.Requests
{
	public class WitWebSocketSubscriptionRequest : WitWebSocketJsonRequest
	{
		public WitWebSocketSubscriptionType SubscriptionType { get; }

		public WitWebSocketSubscriptionRequest(string topicId, WitWebSocketSubscriptionType subscriptionType) : base(WitWebSocketSubscriptionRequest.GetSubscriptionNode(topicId, subscriptionType), null, null, null)
		{
			base.TopicId = topicId;
			this.SubscriptionType = subscriptionType;
		}

		public override string ToString()
		{
			return string.Format("{0}\nSubscription Type: {1}", base.ToString(), this.SubscriptionType);
		}

		private static WitResponseNode GetSubscriptionNode(string topicId, WitWebSocketSubscriptionType subscriptionType)
		{
			WitResponseClass witResponseClass = new WitResponseClass();
			WitResponseClass witResponseClass2 = new WitResponseClass();
			WitResponseClass witResponseClass3 = new WitResponseClass();
			witResponseClass3["topic"] = topicId;
			witResponseClass2[WitWebSocketSubscriptionRequest.GetSubscriptionNodeKey(subscriptionType)] = witResponseClass3;
			witResponseClass["data"] = witResponseClass2;
			return witResponseClass;
		}

		private static string GetSubscriptionNodeKey(WitWebSocketSubscriptionType subscriptionType)
		{
			if (subscriptionType == WitWebSocketSubscriptionType.Subscribe)
			{
				return "subscribe";
			}
			if (subscriptionType != WitWebSocketSubscriptionType.Unsubscribe)
			{
				return string.Empty;
			}
			return "unsubscribe";
		}
	}
}
