using System;

namespace Photon.Voice
{
	public interface IAudioOut<T>
	{
		bool IsPlaying { get; }

		void Start(int frequency, int channels, int frameSamplesPerChannel);

		void Flush();

		void Stop();

		void Push(T[] frame);

		void Service();

		int Lag { get; }

		void ToggleAudioSource(bool toggle);
	}
}
