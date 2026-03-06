using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtAudioTrigger : MonoBehaviour
	{
		public void PlayTapStartedSound()
		{
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickDown);
		}

		public void PlayTapEndedSound()
		{
			this._audioController.PlayDiscreetAudioClip(LckDiscreetAudioController.AudioClip.ClickUp);
		}

		[SerializeField]
		private LckDiscreetAudioController _audioController;
	}
}
