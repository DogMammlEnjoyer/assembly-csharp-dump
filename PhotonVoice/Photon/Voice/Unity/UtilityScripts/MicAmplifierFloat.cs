using System;

namespace Photon.Voice.Unity.UtilityScripts
{
	public class MicAmplifierFloat : IProcessor<float>, IDisposable
	{
		public float AmplificationFactor { get; set; }

		public float BoostValue { get; set; }

		public float MaxBefore { get; private set; }

		public float MaxAfter { get; private set; }

		public bool Disabled { get; set; }

		public MicAmplifierFloat(float amplificationFactor, float boostValue)
		{
			this.AmplificationFactor = amplificationFactor;
			this.BoostValue = boostValue;
		}

		public float[] Process(float[] buf)
		{
			if (this.Disabled)
			{
				return buf;
			}
			for (int i = 0; i < buf.Length; i++)
			{
				float num = buf[i];
				buf[i] *= this.AmplificationFactor;
				buf[i] += this.BoostValue;
				if (this.MaxBefore < num)
				{
					this.MaxBefore = num;
					this.MaxAfter = buf[i];
				}
			}
			return buf;
		}

		public void Dispose()
		{
		}
	}
}
