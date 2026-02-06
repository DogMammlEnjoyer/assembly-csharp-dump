using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class NetworkObjectStatisticsSnapshot
	{
		public int InPackets { get; private set; }

		public int OutPackets { get; private set; }

		public float InBandwidth { get; private set; }

		public float OutBandwidth { get; private set; }

		internal void Reset()
		{
			this.InPackets = 0;
			this.OutPackets = 0;
			this.InBandwidth = 0f;
			this.OutBandwidth = 0f;
		}

		[Conditional("DEBUG")]
		internal void AddToInPacketsStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InPackets = value;
			}
			else
			{
				this.InPackets += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToOutPacketsStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.OutPackets = value;
			}
			else
			{
				this.OutPackets += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInBandwidthStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InBandwidth = value;
			}
			else
			{
				this.InBandwidth += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToOutBandwidthStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.OutBandwidth = value;
			}
			else
			{
				this.OutBandwidth += value;
			}
		}
	}
}
