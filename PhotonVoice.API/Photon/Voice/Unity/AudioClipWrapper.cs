using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
	public class AudioClipWrapper : IAudioReader<float>, IDataReader<float>, IDisposable, IAudioDesc
	{
		public bool Loop { get; set; }

		public AudioClipWrapper(AudioClip audioClip)
		{
			this.audioClip = audioClip;
			this.startTime = Time.time;
		}

		public bool Read(float[] buffer)
		{
			if (!this.playing)
			{
				return false;
			}
			int num = (int)((Time.time - this.startTime) * (float)this.audioClip.frequency);
			int num2 = buffer.Length / this.audioClip.channels;
			if (num > this.readPos + num2)
			{
				this.audioClip.GetData(buffer, this.readPos);
				this.readPos += num2;
				if (this.readPos >= this.audioClip.samples)
				{
					if (this.Loop)
					{
						this.readPos = 0;
						this.startTime = Time.time;
					}
					else
					{
						this.playing = false;
					}
				}
				return true;
			}
			return false;
		}

		public int SamplingRate
		{
			get
			{
				return this.audioClip.frequency;
			}
		}

		public int Channels
		{
			get
			{
				return this.audioClip.channels;
			}
		}

		public string Error { get; private set; }

		public void Dispose()
		{
		}

		private AudioClip audioClip;

		private int readPos;

		private float startTime;

		private bool playing = true;
	}
}
