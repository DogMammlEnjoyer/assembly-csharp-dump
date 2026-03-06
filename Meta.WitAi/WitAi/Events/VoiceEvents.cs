using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Meta.WitAi.Events
{
	[Serializable]
	public class VoiceEvents : SpeechEvents
	{
		public WitByteDataEvent OnByteDataReady
		{
			get
			{
				return this._onByteDataReady;
			}
		}

		public WitByteDataEvent OnByteDataSent
		{
			get
			{
				return this._onByteDataSent;
			}
		}

		public WitValidationEvent OnValidatePartialResponse
		{
			get
			{
				return this._onValidatePartialResponse;
			}
		}

		private const string EVENT_CATEGORY_DATA_EVENTS = "Data Events";

		[EventCategory("Data Events")]
		[FormerlySerializedAs("OnByteDataReady")]
		[SerializeField]
		[HideInInspector]
		private WitByteDataEvent _onByteDataReady = new WitByteDataEvent();

		[EventCategory("Data Events")]
		[FormerlySerializedAs("OnByteDataSent")]
		[SerializeField]
		[HideInInspector]
		private WitByteDataEvent _onByteDataSent = new WitByteDataEvent();

		[EventCategory("Activation Response Events")]
		[Tooltip("Called after an on partial response to validate data.  If data.validResponse is true, service will deactivate & use the partial data as final")]
		[FormerlySerializedAs("OnValidatePartialResponse")]
		[SerializeField]
		private WitValidationEvent _onValidatePartialResponse = new WitValidationEvent();
	}
}
