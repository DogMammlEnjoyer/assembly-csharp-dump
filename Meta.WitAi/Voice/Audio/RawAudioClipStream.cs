using System;
using UnityEngine;

namespace Meta.Voice.Audio
{
	public class RawAudioClipStream : BaseAudioClipStream
	{
		public float[] SampleBuffer { get; }

		public RawAudioClipStream(float newReadyLength = 1.5f, float newMaxLength = 15f) : this(1, 24000, newReadyLength, newMaxLength)
		{
		}

		public RawAudioClipStream(int newChannels, int newSampleRate, float newReadyLength = 1.5f, float newMaxLength = 15f) : base(newChannels, newSampleRate, newReadyLength)
		{
			this.SampleBuffer = new float[Mathf.CeilToInt((float)(newChannels * newSampleRate) * newMaxLength)];
		}

		public override void AddSamples(float[] buffer, int bufferOffset, int bufferLength)
		{
			int addedSamples = base.AddedSamples;
			int num = Mathf.Min(bufferLength, this.SampleBuffer.Length - addedSamples);
			if (num <= 0)
			{
				return;
			}
			Array.Copy(buffer, bufferOffset, this.SampleBuffer, addedSamples, num);
			base.AddedSamples += num;
			AudioClipStreamSampleDelegate onAddSamples = base.OnAddSamples;
			if (onAddSamples != null)
			{
				onAddSamples(this.SampleBuffer, addedSamples, num);
			}
			this.UpdateState();
		}
	}
}
