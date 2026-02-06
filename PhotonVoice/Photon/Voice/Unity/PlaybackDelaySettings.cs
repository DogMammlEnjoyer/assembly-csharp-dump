using System;

namespace Photon.Voice.Unity
{
	[Serializable]
	public struct PlaybackDelaySettings
	{
		public override string ToString()
		{
			return string.Format("[low={0}ms,high={1}ms,max={2}ms]", this.MinDelaySoft, this.MaxDelaySoft, this.MaxDelayHard);
		}

		public const int DEFAULT_LOW = 200;

		public const int DEFAULT_HIGH = 400;

		public const int DEFAULT_MAX = 1000;

		public int MinDelaySoft;

		public int MaxDelaySoft;

		public int MaxDelayHard;
	}
}
