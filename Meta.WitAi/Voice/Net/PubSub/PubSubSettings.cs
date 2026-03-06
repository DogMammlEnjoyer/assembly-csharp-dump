using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.Voice.Net.PubSub
{
	[Serializable]
	public struct PubSubSettings
	{
		public PubSubSettings(string pubSubTopicId = "")
		{
			this.PubSubTopicId = pubSubTopicId;
			this.PublishOptions = PubSubSettings.GetDefaultOptions();
			this.SubscribeOptions = PubSubSettings.GetDefaultOptions();
		}

		public bool Equals(PubSubSettings other)
		{
			return string.Equals(this.PubSubTopicId, other.PubSubTopicId) && this.PublishOptions.Equals(other.PublishOptions) && this.SubscribeOptions.Equals(other.SubscribeOptions);
		}

		public void GetPublishTopics(Dictionary<string, string> topics)
		{
			PubSubSettings.GetTopics(topics, this.PubSubTopicId, this.PublishOptions);
		}

		public Dictionary<string, string> GetPublishTopics()
		{
			return PubSubSettings.GetTopics(this.PubSubTopicId, this.PublishOptions);
		}

		public void GetSubscribeTopics(Dictionary<string, string> topics)
		{
			PubSubSettings.GetTopics(topics, this.PubSubTopicId, this.SubscribeOptions);
		}

		public Dictionary<string, string> GetSubscribeTopics()
		{
			return PubSubSettings.GetTopics(this.PubSubTopicId, this.SubscribeOptions);
		}

		public static void GetTopics(Dictionary<string, string> topics, string topicId, PubSubResponseOptions options)
		{
			if (string.IsNullOrEmpty(topicId))
			{
				return;
			}
			if (options.transcriptionResponses)
			{
				PubSubSettings.SetTopicKey(topics, topicId, "1", "_ASR");
			}
			if (options.composerResponses)
			{
				PubSubSettings.SetTopicKey(topics, topicId, "2", "_COMP");
			}
		}

		public static Dictionary<string, string> GetTopics(string topicId, PubSubResponseOptions options)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			PubSubSettings.GetTopics(dictionary, topicId, options);
			return dictionary;
		}

		private static void SetTopicKey(Dictionary<string, string> topics, string topicId, string key, string append)
		{
			topics[key] = topicId + append;
		}

		public bool IsSubscribedTopicId(string topicId)
		{
			return !string.IsNullOrEmpty(this.PubSubTopicId) && !string.IsNullOrEmpty(topicId) && ((this.SubscribeOptions.transcriptionResponses && topicId.Equals(this.PubSubTopicId + "_ASR")) || (this.SubscribeOptions.composerResponses && topicId.Equals(this.PubSubTopicId + "_COMP")));
		}

		private static PubSubResponseOptions GetDefaultOptions()
		{
			return new PubSubResponseOptions
			{
				transcriptionResponses = true,
				composerResponses = true
			};
		}

		[Tooltip("The unique pubsub topic id to publish and/or subscribe to")]
		public string PubSubTopicId;

		[Tooltip("Toggles for publishing per response type.")]
		public PubSubResponseOptions PublishOptions;

		[Tooltip("Toggles for subscribing per response type.")]
		public PubSubResponseOptions SubscribeOptions;
	}
}
