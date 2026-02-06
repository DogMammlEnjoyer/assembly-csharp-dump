using System;

namespace Fusion
{
	public class TimeSyncConfiguration
	{
		internal static TimeSyncConfiguration GetFromTickrate(TickRate.Resolved tickrate)
		{
			TimeSyncConfiguration timeSyncConfiguration = new TimeSyncConfiguration();
			int clientSend = tickrate.ClientSend;
			int num = clientSend;
			if (num <= 32)
			{
				if (num <= 16)
				{
					if (num == 8 || num == 10 || num == 16)
					{
						timeSyncConfiguration.MaxLateInputs = 1.0;
						timeSyncConfiguration.RedundantInputs = 1;
					}
				}
				else if (num <= 24)
				{
					if (num == 20 || num == 24)
					{
						timeSyncConfiguration.MaxLateInputs = 1.0;
						timeSyncConfiguration.RedundantInputs = 1;
					}
				}
				else if (num == 30 || num == 32)
				{
					timeSyncConfiguration.MaxLateInputs = 1.0;
					timeSyncConfiguration.RedundantInputs = 1;
				}
			}
			else
			{
				if (num <= 100)
				{
					if (num <= 60)
					{
						if (num != 50 && num != 60)
						{
							goto IL_120;
						}
					}
					else if (num != 64 && num != 100)
					{
						goto IL_120;
					}
				}
				else if (num <= 128)
				{
					if (num != 120 && num != 128)
					{
						goto IL_120;
					}
				}
				else if (num != 240 && num != 256)
				{
					goto IL_120;
				}
				timeSyncConfiguration.MaxLateInputs = 1.0;
				timeSyncConfiguration.RedundantInputs = 1;
			}
			IL_120:
			int serverSend = tickrate.ServerSend;
			int num2 = serverSend;
			if (num2 <= 32)
			{
				if (num2 <= 16)
				{
					if (num2 == 8 || num2 == 10 || num2 == 16)
					{
						timeSyncConfiguration.MaxLateSnapshots = 5.0;
						timeSyncConfiguration.RedundantSnapshots = 1;
					}
				}
				else if (num2 <= 24)
				{
					if (num2 == 20 || num2 == 24)
					{
						timeSyncConfiguration.MaxLateSnapshots = 5.0;
						timeSyncConfiguration.RedundantSnapshots = 1;
					}
				}
				else if (num2 == 30 || num2 == 32)
				{
					timeSyncConfiguration.MaxLateSnapshots = 5.0;
					timeSyncConfiguration.RedundantSnapshots = 1;
				}
			}
			else
			{
				if (num2 <= 100)
				{
					if (num2 <= 60)
					{
						if (num2 != 50 && num2 != 60)
						{
							return timeSyncConfiguration;
						}
					}
					else if (num2 != 64 && num2 != 100)
					{
						return timeSyncConfiguration;
					}
				}
				else if (num2 <= 128)
				{
					if (num2 != 120 && num2 != 128)
					{
						return timeSyncConfiguration;
					}
				}
				else if (num2 != 240 && num2 != 256)
				{
					return timeSyncConfiguration;
				}
				timeSyncConfiguration.MaxLateSnapshots = 5.0;
				timeSyncConfiguration.RedundantSnapshots = 1;
			}
			return timeSyncConfiguration;
		}

		internal double SampleWindowSecondsNormalized
		{
			get
			{
				return Maths.Clamp(this.SampleWindowSeconds, 1.0, 10.0);
			}
		}

		internal double MaxLateInputsNormalized
		{
			get
			{
				return Maths.Clamp(this.MaxLateInputs, 0.1, 10.0) / 100.0;
			}
		}

		internal double MaxLateSnapshotsNormalized
		{
			get
			{
				return Maths.Clamp(this.MaxLateSnapshots, 0.1, 10.0) / 100.0;
			}
		}

		internal int RedundantInputsNormalized
		{
			get
			{
				return Maths.Clamp(this.RedundantInputs, 0, 8);
			}
		}

		internal int RedundantSnapshotsNormalized
		{
			get
			{
				return Maths.Clamp(this.RedundantSnapshots, 0, 8);
			}
		}

		private double MaxSimSpeedAdjust
		{
			get
			{
				return 5.0;
			}
		}

		internal double MaxSimSpeedAdjustNormalized
		{
			get
			{
				return Maths.Clamp(this.MaxSimSpeedAdjust, 1.0, 10.0) / 100.0;
			}
		}

		private double MaxInterpSpeedAdjust
		{
			get
			{
				return 5.0;
			}
		}

		internal double MaxInterpSpeedAdjustNormalized
		{
			get
			{
				return Maths.Clamp(this.MaxInterpSpeedAdjust, 1.0, 10.0) / 100.0;
			}
		}

		[InlineHelp]
		[Unit(Units.Seconds)]
		[RangeEx(1.0, 10.0)]
		public double SampleWindowSeconds = 1.0;

		[InlineHelp]
		[Unit(Units.Percentage)]
		[RangeEx(0.10000000149011612, 10.0)]
		public double MaxLateInputs = 1.0;

		[InlineHelp]
		[Unit(Units.Percentage)]
		[RangeEx(0.10000000149011612, 10.0)]
		public double MaxLateSnapshots = 5.0;

		[InlineHelp]
		[Unit(Units.Packets)]
		[RangeEx(0.0, 8.0)]
		public int RedundantInputs = 1;

		[InlineHelp]
		[Unit(Units.Packets)]
		[RangeEx(0.0, 8.0)]
		public int RedundantSnapshots = 1;
	}
}
