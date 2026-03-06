using System;
using UnityEngine.Events;

namespace Meta.Voice.Net.PubSub
{
	public interface IPubSubAdapter
	{
		PubSubSettings Settings { get; set; }

		PubSubSubscriptionState SubscriptionState { get; }

		event Action<PubSubSubscriptionState> OnTopicSubscriptionStateChange;

		UnityEvent OnSubscribed { get; }

		UnityEvent OnUnsubscribed { get; }
	}
}
