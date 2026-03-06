using System;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.ServiceReferences
{
	public class VoiceServiceAudioEventReference : AudioInputServiceReference
	{
		public override IAudioInputEvents AudioEvents
		{
			get
			{
				return this._voiceServiceReference.VoiceService.AudioEvents;
			}
		}

		[SerializeField]
		private VoiceServiceReference _voiceServiceReference;
	}
}
