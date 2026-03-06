using System;
using System.Collections;
using Liv.Lck.Collections;
using UnityEngine;

namespace Liv.Lck
{
	internal class LckAudioCaptureWwise : MonoBehaviour, ILckAudioSource
	{
		public bool IsCapturing()
		{
			return this._captureAudio;
		}

		private IEnumerator Start()
		{
			yield return null;
			yield break;
		}

		public virtual void EnableCapture()
		{
			LckLog.Log("Wwise: enable capture");
		}

		public virtual void DisableCapture()
		{
			LckLog.Log("Wwise: disable capture");
		}

		private void OnDestroy()
		{
			LckLog.Log("Wwise destroyed");
		}

		public void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback)
		{
		}

		private bool _captureAudio;

		private AudioBuffer _audioBuffer = new AudioBuffer(96000);
	}
}
