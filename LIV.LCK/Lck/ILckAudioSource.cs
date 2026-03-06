using System;
using Liv.Lck.Collections;

namespace Liv.Lck
{
	public interface ILckAudioSource
	{
		void GetAudioData(ILckAudioSource.AudioDataCallbackDelegate callback);

		void EnableCapture();

		void DisableCapture();

		bool IsCapturing();

		public delegate void AudioDataCallbackDelegate(AudioBuffer audioBuffer);
	}
}
