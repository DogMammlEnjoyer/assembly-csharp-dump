using System;

namespace Photon.Voice.Unity.UtilityScripts
{
	public class MicAmplifierShort : IProcessor<short>, IDisposable
	{
		public short AmplificationFactor { get; set; }

		public short BoostValue { get; set; }

		public short MaxBefore { get; private set; }

		public short MaxAfter { get; private set; }

		public bool Disabled { get; set; }

		public MicAmplifierShort(short amplificationFactor, short boostValue)
		{
			this.AmplificationFactor = amplificationFactor;
			this.BoostValue = boostValue;
		}

		public short[] Process(short[] buf)
		{
			if (this.Disabled)
			{
				return buf;
			}
			for (int i = 0; i < buf.Length; i++)
			{
				short num = buf[i];
				int num2 = i;
				buf[num2] *= this.AmplificationFactor;
				int num3 = i;
				buf[num3] += this.BoostValue;
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
