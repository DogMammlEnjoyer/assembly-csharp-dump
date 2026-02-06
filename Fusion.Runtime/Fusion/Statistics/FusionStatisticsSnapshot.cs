using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class FusionStatisticsSnapshot
	{
		[Conditional("DEBUG")]
		internal void ClearSnapshot()
		{
			this.Resimulations = 0;
			this.ForwardTicks = 0;
			this.InPackets = 0;
			this.OutPackets = 0;
			this.InBandwidth = 0f;
			this.OutBandwidth = 0f;
			this.RoundTripTime = 0f;
			this.InputInBandwidth = 0f;
			this.InputOutBandwidth = 0f;
			this.InObjectUpdates = 0;
			this.OutObjectUpdates = 0;
			this.ObjectsAllocMemoryUsedInBytes = 0;
			this.GeneralAllocMemoryUsedInBytes = 0;
			this.GeneralAllocMemoryFreeInBytes = 0;
			this.ObjectsAllocMemoryFreeInBytes = 0;
			this.WordsWrittenCount = 0;
			this.WordsReadCount = 0;
			this.InputReceiveDelta = 0f;
			this.TimeResets = 0;
			this.StateReceiveDelta = 0f;
			this.SimulationTimeOffset = 0f;
			this.SimulationSpeed = 0f;
			this.InterpolationOffset = 0f;
			this.InterpolationSpeed = 0f;
		}

		[Conditional("DEBUG")]
		internal void CopyFrom(FusionStatisticsSnapshot snapshot)
		{
			this.Resimulations = snapshot.Resimulations;
			this.ForwardTicks = snapshot.ForwardTicks;
			this.InPackets = snapshot.InPackets;
			this.OutPackets = snapshot.OutPackets;
			this.InBandwidth = snapshot.InBandwidth;
			this.OutBandwidth = snapshot.OutBandwidth;
			this.RoundTripTime = snapshot.RoundTripTime;
			this.InputInBandwidth = snapshot.InputInBandwidth;
			this.InputOutBandwidth = snapshot.InputOutBandwidth;
			this.InObjectUpdates = snapshot.InObjectUpdates;
			this.OutObjectUpdates = snapshot.OutObjectUpdates;
			this.ObjectsAllocMemoryUsedInBytes = snapshot.ObjectsAllocMemoryUsedInBytes;
			this.GeneralAllocMemoryUsedInBytes = snapshot.GeneralAllocMemoryUsedInBytes;
			this.ObjectsAllocMemoryFreeInBytes = snapshot.ObjectsAllocMemoryFreeInBytes;
			this.GeneralAllocMemoryFreeInBytes = snapshot.GeneralAllocMemoryFreeInBytes;
			this.WordsWrittenCount = snapshot.WordsWrittenCount;
			this.WordsReadCount = snapshot.WordsReadCount;
			this.InputReceiveDelta = snapshot.InputReceiveDelta;
			this.TimeResets = snapshot.TimeResets;
			this.StateReceiveDelta = snapshot.StateReceiveDelta;
			this.SimulationTimeOffset = snapshot.SimulationTimeOffset;
			this.SimulationSpeed = snapshot.SimulationSpeed;
			this.InterpolationOffset = snapshot.InterpolationOffset;
			this.InterpolationSpeed = snapshot.InterpolationSpeed;
		}

		public int Resimulations { get; private set; }

		public int ForwardTicks { get; private set; }

		public int InPackets { get; private set; }

		public int OutPackets { get; private set; }

		public float InBandwidth { get; private set; }

		public float OutBandwidth { get; private set; }

		public float RoundTripTime { get; private set; }

		public float InputInBandwidth { get; private set; }

		public float InputOutBandwidth { get; private set; }

		public int InObjectUpdates { get; private set; }

		public int OutObjectUpdates { get; private set; }

		public int ObjectsAllocMemoryUsedInBytes { get; private set; }

		public int GeneralAllocMemoryUsedInBytes { get; private set; }

		public int ObjectsAllocMemoryFreeInBytes { get; private set; }

		public int GeneralAllocMemoryFreeInBytes { get; private set; }

		public int WordsWrittenCount { get; private set; }

		public int WordsReadCount { get; private set; }

		public int WordsWrittenSize
		{
			get
			{
				return this.WordsWrittenCount * 4;
			}
		}

		public int WordsReadSize
		{
			get
			{
				return this.WordsReadCount * 4;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToResimulationStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.Resimulations = value;
			}
			else
			{
				this.Resimulations += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToForwardTicksStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.ForwardTicks = value;
			}
			else
			{
				this.ForwardTicks += value;
			}
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

		[Conditional("DEBUG")]
		internal void AddToRoundTripTimeStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.RoundTripTime = value;
			}
			else
			{
				this.RoundTripTime += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInputInBandwidthStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InputInBandwidth = value;
			}
			else
			{
				this.InputInBandwidth += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInputOutBandwidthStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InputOutBandwidth = value;
			}
			else
			{
				this.InputOutBandwidth += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInObjectUpdatesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InObjectUpdates = value;
			}
			else
			{
				this.InObjectUpdates += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToOutObjectUpdatesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.OutObjectUpdates = value;
			}
			else
			{
				this.OutObjectUpdates += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToObjectsAllocMemoryUsedInBytesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.ObjectsAllocMemoryUsedInBytes = value;
			}
			else
			{
				this.ObjectsAllocMemoryUsedInBytes += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToGeneralAllocMemoryUsedInBytesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.GeneralAllocMemoryUsedInBytes = value;
			}
			else
			{
				this.GeneralAllocMemoryUsedInBytes += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToObjectsAllocMemoryFreeInBytesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.ObjectsAllocMemoryFreeInBytes = value;
			}
			else
			{
				this.ObjectsAllocMemoryFreeInBytes += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToGeneralAllocMemoryFreeInBytesStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.GeneralAllocMemoryFreeInBytes = value;
			}
			else
			{
				this.GeneralAllocMemoryFreeInBytes += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToWordsWrittenCountStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.WordsWrittenCount = value;
			}
			else
			{
				this.WordsWrittenCount += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToWordsReadCountStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.WordsReadCount = value;
			}
			else
			{
				this.WordsReadCount += value;
			}
		}

		public float InputReceiveDelta { get; private set; }

		public int TimeResets { get; private set; }

		public float StateReceiveDelta { get; private set; }

		public float SimulationTimeOffset { get; private set; }

		public float SimulationSpeed { get; private set; }

		public float InterpolationOffset { get; private set; }

		public float InterpolationSpeed { get; private set; }

		[Conditional("DEBUG")]
		internal void AddToInputReceiveDeltaStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InputReceiveDelta = value;
			}
			else
			{
				this.InputReceiveDelta += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToTimeResetsStat(int value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.TimeResets = value;
			}
			else
			{
				this.TimeResets += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToStateReceiveDeltaStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.StateReceiveDelta = value;
			}
			else
			{
				this.StateReceiveDelta += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToSimulationTimeOffsetStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.SimulationTimeOffset = value;
			}
			else
			{
				this.SimulationTimeOffset += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToSimulationSpeedStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.SimulationSpeed = value;
			}
			else
			{
				this.SimulationSpeed += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInterpolationOffsetStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InterpolationOffset = value;
			}
			else
			{
				this.InterpolationOffset += value;
			}
		}

		[Conditional("DEBUG")]
		internal void AddToInterpolationSpeedStat(float value, bool overrideValue = false)
		{
			if (overrideValue)
			{
				this.InterpolationSpeed = value;
			}
			else
			{
				this.InterpolationSpeed += value;
			}
		}
	}
}
