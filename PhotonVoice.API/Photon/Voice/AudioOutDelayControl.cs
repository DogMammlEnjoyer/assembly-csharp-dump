using System;

namespace Photon.Voice
{
	public class AudioOutDelayControl
	{
		public class PlayDelayConfig
		{
			public PlayDelayConfig()
			{
				this.Low = 200;
				this.High = 400;
				this.Max = 1000;
				this.SpeedUpPerc = 5;
			}

			public int Low { get; set; }

			public int High { get; set; }

			public int Max { get; set; }

			public int SpeedUpPerc { get; set; }

			public AudioOutDelayControl.PlayDelayConfig Clone()
			{
				return new AudioOutDelayControl.PlayDelayConfig
				{
					Low = this.Low,
					High = this.High,
					Max = this.Max,
					SpeedUpPerc = this.SpeedUpPerc
				};
			}
		}
	}
}
