using System;
using System.Runtime.InteropServices;
using Liv.Lck.Collections;
using UnityEngine;

namespace Liv.Lck
{
	internal class LckAudioCaptureFMOD : MonoBehaviour, ILckAudioSource
	{
		public bool IsCapturing()
		{
			return this._isCapturing;
		}

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

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
			this._isCapturing = true;
			this._audioBuffer.Clear();
		}

		public void DisableCapture()
		{
			this._isCapturing = false;
			this._audioBuffer.Clear();
		}

		private GCHandle mObjHandle;

		private AudioBuffer _tmpDownmixBuffer = new AudioBuffer(98000);

		private AudioBuffer _tmpAudio = new AudioBuffer(98000);

		private AudioBuffer _audioBuffer = new AudioBuffer(98000);

		private const int channels = 2;

		private bool _isCapturing;

		private readonly object _audioThreadLock = new object();
	}
}
