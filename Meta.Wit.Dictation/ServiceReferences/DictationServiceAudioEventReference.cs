using System;
using Meta.WitAi.Interfaces;
using Meta.WitAi.Utilities;
using UnityEngine;

namespace Meta.WitAi.ServiceReferences
{
	public class DictationServiceAudioEventReference : AudioInputServiceReference
	{
		public override IAudioInputEvents AudioEvents
		{
			get
			{
				return this._dictationServiceReference.DictationService.AudioEvents;
			}
		}

		[SerializeField]
		private DictationServiceReference _dictationServiceReference;
	}
}
