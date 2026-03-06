using System;
using Liv.Lck.Collections;
using UnityEngine;

namespace Liv.Lck
{
	internal class LckAudioCapture : MonoBehaviour, ILckAudioSource
	{
		public void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback)
		{
			object audioThreadLock = this._audioThreadLock;
			lock (audioThreadLock)
			{
				callback(this._audioBuffer);
				this._audioBuffer.Clear();
			}
		}

		public void EnableCapture()
		{
			this._audioBuffer.Clear();
			this._captureAudio = true;
		}

		public void DisableCapture()
		{
			this._audioBuffer.Clear();
			this._captureAudio = false;
		}

		public bool IsCapturing()
		{
			return this._captureAudio;
		}

		protected virtual void OnAudioFilterRead(float[] data, int channels)
		{
			if (this._captureAudio)
			{
				object audioThreadLock = this._audioThreadLock;
				lock (audioThreadLock)
				{
					if (!this._audioBuffer.TryExtendFrom(data))
					{
						LckLog.LogWarning("LCK Audio Capture losing data. Expecting this to be a lag spike.");
					}
				}
			}
		}

		private bool _captureAudio;

		private AudioBuffer _audioBuffer = new AudioBuffer(96000);

		private readonly object _audioThreadLock = new object();
	}
}
