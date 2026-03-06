using System;
using Meta.WitAi.Data;

namespace Meta.WitAi
{
	public class AudioDurationTracker
	{
		public AudioDurationTracker(string requestId, AudioEncoding audioEncoding)
		{
			this._requestId = requestId;
			this._audioEncoding = audioEncoding;
			this._bytesPerSample = this._audioEncoding.bits / 8;
		}

		public void AddBytes(long bytes)
		{
			this._bytesCaptured += (double)bytes;
		}

		public void FinalizeAudio()
		{
			this._finalizeTimeStamp = DateTime.UtcNow.Ticks / 10000L;
			this._audioDurationMs = this._bytesCaptured / (double)(this._audioEncoding.samplerate * this._audioEncoding.numChannels * this._bytesPerSample) * 1000.0;
		}

		public long GetFinalizeTimeStamp()
		{
			return this._finalizeTimeStamp;
		}

		public double GetAudioDuration()
		{
			return this._audioDurationMs;
		}

		public string GetRequestId()
		{
			return this._requestId;
		}

		private readonly string _requestId;

		private double _bytesCaptured;

		private readonly int _bytesPerSample;

		private readonly AudioEncoding _audioEncoding;

		private long _finalizeTimeStamp;

		private double _audioDurationMs;
	}
}
