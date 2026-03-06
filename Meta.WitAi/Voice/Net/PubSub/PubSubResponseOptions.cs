using System;
using UnityEngine;

namespace Meta.Voice.Net.PubSub
{
	[Serializable]
	public struct PubSubResponseOptions
	{
		public bool Equals(PubSubResponseOptions other)
		{
			return this.transcriptionResponses == other.transcriptionResponses && this.composerResponses == other.composerResponses;
		}

		[Header("Responses returned from audio interactions.")]
		public bool transcriptionResponses;

		[Header("Responses returned from composer results.")]
		public bool composerResponses;
	}
}
