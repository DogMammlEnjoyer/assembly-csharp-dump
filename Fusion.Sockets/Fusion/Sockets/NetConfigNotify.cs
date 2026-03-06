using System;

namespace Fusion.Sockets
{
	public struct NetConfigNotify
	{
		public int SequenceBounds
		{
			get
			{
				return this.WindowSize * 16;
			}
		}

		public int AckMaskBits
		{
			get
			{
				return this.AckMaskBytes * 8;
			}
		}

		public static NetConfigNotify Defaults
		{
			get
			{
				NetConfigNotify result;
				result.AckMaskBytes = 8;
				result.AckForceCount = 8;
				result.AckForceTimeout = 0.1;
				result.WindowSize = 128;
				result.SequenceBytes = 2;
				return result;
			}
		}

		public int AckMaskBytes;

		public int AckForceCount;

		public double AckForceTimeout;

		public int WindowSize;

		public int SequenceBytes;
	}
}
