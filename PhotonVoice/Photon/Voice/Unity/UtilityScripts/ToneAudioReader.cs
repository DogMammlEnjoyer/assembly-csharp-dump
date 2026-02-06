using System;
using UnityEngine;

namespace Photon.Voice.Unity.UtilityScripts
{
	internal class ToneAudioReader : IAudioReader<float>, IDataReader<float>, IDisposable, IAudioDesc
	{
		public ToneAudioReader()
		{
			this.k = 2764.601535159018 / (double)this.SamplingRate;
		}

		public int Channels
		{
			get
			{
				return 2;
			}
		}

		public int SamplingRate
		{
			get
			{
				return 24000;
			}
		}

		public string Error
		{
			get
			{
				return null;
			}
		}

		public void Dispose()
		{
		}

		public bool Read(float[] buf)
		{
			int num = buf.Length / this.Channels;
			long num2 = (long)(AudioSettings.dspTime * (double)this.SamplingRate);
			long num3 = num2 - this.timeSamples;
			if (Math.Abs(num3) > (long)(this.SamplingRate / 4))
			{
				Debug.LogWarningFormat("ToneAudioReader sample time is out: {0} / {1}", new object[]
				{
					this.timeSamples,
					num2
				});
				num3 = (long)num;
				this.timeSamples = num2 - (long)num;
			}
			if (num3 < (long)num)
			{
				return false;
			}
			int num4 = 0;
			for (int i = 0; i < num; i++)
			{
				long num5 = this.timeSamples;
				this.timeSamples = num5 + 1L;
				float num6 = (float)Math.Sin((double)num5 * this.k) * 0.2f;
				for (int j = 0; j < this.Channels; j++)
				{
					buf[num4++] = num6;
				}
			}
			return true;
		}

		private double k;

		private long timeSamples;
	}
}
