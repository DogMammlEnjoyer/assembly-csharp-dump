using System;
using Liv.Lck.DependencyInjection;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtMicVolume : MonoBehaviour
	{
		private void Update()
		{
			this._incomingVolume = Mathf.Clamp01(this._lckService.GetMicrophoneOutputLevel().Result * 10f);
			this._audioButton.SetProgress(this._incomingVolume);
		}

		[InjectLck]
		private ILckService _lckService;

		[SerializeField]
		private float _incomingVolume;

		[SerializeField]
		private GtAudioButton _audioButton;
	}
}
