using System;
using UnityEngine;

namespace Fusion.Statistics
{
	internal static class FusionStatisticsHelper
	{
		internal static void GetStatGraphDefaultSettings(RenderSimStats stat, out string valueTextFormat, out float valueTextMultiplier, out bool ignoreZeroOnAverage, out bool ignoreZeroOnBuffer, out int accumulateTimeMs)
		{
			valueTextFormat = "{0:0}";
			valueTextMultiplier = 1f;
			ignoreZeroOnAverage = false;
			ignoreZeroOnBuffer = false;
			accumulateTimeMs = 0;
			if (stat > RenderSimStats.InterpolationSpeed)
			{
				if (stat <= RenderSimStats.ObjectsAllocatedMemoryInUse)
				{
					if (stat <= RenderSimStats.AverageInPacketSize)
					{
						if (stat == RenderSimStats.InputInBandwidth || stat == RenderSimStats.InputOutBandwidth)
						{
							goto IL_1D5;
						}
						if (stat != RenderSimStats.AverageInPacketSize)
						{
							goto IL_24B;
						}
					}
					else if (stat <= RenderSimStats.InObjectUpdates)
					{
						if (stat != RenderSimStats.AverageOutPacketSize)
						{
							if (stat != RenderSimStats.InObjectUpdates)
							{
								goto IL_24B;
							}
							goto IL_1AF;
						}
					}
					else
					{
						if (stat == RenderSimStats.OutObjectUpdates)
						{
							goto IL_1AF;
						}
						if (stat != RenderSimStats.ObjectsAllocatedMemoryInUse)
						{
							goto IL_24B;
						}
						goto IL_21B;
					}
					valueTextFormat = "{0:0} B";
					ignoreZeroOnBuffer = true;
					ignoreZeroOnAverage = true;
					return;
				}
				if (stat > RenderSimStats.GeneralAllocatedMemoryFree)
				{
					if (stat <= RenderSimStats.WordsWrittenSize)
					{
						if (stat != RenderSimStats.WordsWrittenCount)
						{
							if (stat != RenderSimStats.WordsWrittenSize)
							{
								goto IL_24B;
							}
							goto IL_237;
						}
					}
					else if (stat != RenderSimStats.WordsReadCount)
					{
						if (stat != RenderSimStats.WordsReadSize)
						{
							goto IL_24B;
						}
						goto IL_237;
					}
					valueTextFormat = "{0:0}";
					ignoreZeroOnBuffer = true;
					accumulateTimeMs = 1000;
					return;
					IL_237:
					valueTextFormat = "{0:0} B";
					ignoreZeroOnBuffer = true;
					accumulateTimeMs = 1000;
					return;
				}
				if (stat != RenderSimStats.GeneralAllocatedMemoryInUse && stat != RenderSimStats.ObjectsAllocatedMemoryFree && stat != RenderSimStats.GeneralAllocatedMemoryFree)
				{
					goto IL_24B;
				}
				IL_21B:
				valueTextFormat = "{0:0} B";
				return;
			}
			if (stat > RenderSimStats.ForwardTicks)
			{
				if (stat <= RenderSimStats.StateReceiveDelta)
				{
					if (stat == RenderSimStats.InputReceiveDelta)
					{
						goto IL_20C;
					}
					if (stat != RenderSimStats.TimeResets)
					{
						if (stat != RenderSimStats.StateReceiveDelta)
						{
							goto IL_24B;
						}
						goto IL_20C;
					}
				}
				else if (stat <= RenderSimStats.SimulationSpeed)
				{
					if (stat == RenderSimStats.SimulationTimeOffset)
					{
						goto IL_20C;
					}
					if (stat != RenderSimStats.SimulationSpeed)
					{
						goto IL_24B;
					}
				}
				else
				{
					if (stat == RenderSimStats.InterpolationOffset)
					{
						goto IL_20C;
					}
					if (stat != RenderSimStats.InterpolationSpeed)
					{
						goto IL_24B;
					}
				}
				valueTextFormat = "{0:0}";
				return;
				IL_20C:
				valueTextMultiplier = 1000f;
				valueTextFormat = "{0:0} ms";
				return;
			}
			if (stat <= RenderSimStats.InBandwidth)
			{
				if (stat - RenderSimStats.InPackets > 1)
				{
					if (stat == RenderSimStats.RTT)
					{
						valueTextFormat = "{0:0} ms";
						valueTextMultiplier = 1000f;
						ignoreZeroOnAverage = true;
						ignoreZeroOnBuffer = true;
						return;
					}
					if (stat != RenderSimStats.InBandwidth)
					{
						goto IL_24B;
					}
					goto IL_1D5;
				}
			}
			else
			{
				if (stat == RenderSimStats.OutBandwidth)
				{
					goto IL_1D5;
				}
				if (stat == RenderSimStats.Resimulations)
				{
					valueTextFormat = "{0:0}";
					return;
				}
				if (stat != RenderSimStats.ForwardTicks)
				{
					goto IL_24B;
				}
				valueTextFormat = "{0:0}";
				return;
			}
			IL_1AF:
			valueTextFormat = "{0:0}";
			accumulateTimeMs = 1000;
			return;
			IL_1D5:
			valueTextFormat = "{0:0} B";
			accumulateTimeMs = 1000;
			return;
			IL_24B:
			valueTextFormat = "{0:0}";
		}

		internal static float GetStatDataFromSnapshot(RenderSimStats stat, FusionStatisticsSnapshot simulationStatsSnapshot)
		{
			if (stat <= RenderSimStats.InputInBandwidth)
			{
				if (stat <= RenderSimStats.InputReceiveDelta)
				{
					if (stat <= RenderSimStats.OutBandwidth)
					{
						switch (stat)
						{
						case RenderSimStats.InPackets:
							return (float)simulationStatsSnapshot.InPackets;
						case RenderSimStats.OutPackets:
							return (float)simulationStatsSnapshot.OutPackets;
						case RenderSimStats.InPackets | RenderSimStats.OutPackets:
							break;
						case RenderSimStats.RTT:
							return simulationStatsSnapshot.RoundTripTime;
						default:
							if (stat == RenderSimStats.InBandwidth)
							{
								return simulationStatsSnapshot.InBandwidth;
							}
							if (stat == RenderSimStats.OutBandwidth)
							{
								return simulationStatsSnapshot.OutBandwidth;
							}
							break;
						}
					}
					else
					{
						if (stat == RenderSimStats.Resimulations)
						{
							return (float)simulationStatsSnapshot.Resimulations;
						}
						if (stat == RenderSimStats.ForwardTicks)
						{
							return (float)simulationStatsSnapshot.ForwardTicks;
						}
						if (stat == RenderSimStats.InputReceiveDelta)
						{
							return simulationStatsSnapshot.InputReceiveDelta;
						}
					}
				}
				else if (stat <= RenderSimStats.SimulationTimeOffset)
				{
					if (stat == RenderSimStats.TimeResets)
					{
						return (float)simulationStatsSnapshot.TimeResets;
					}
					if (stat == RenderSimStats.StateReceiveDelta)
					{
						return simulationStatsSnapshot.StateReceiveDelta;
					}
					if (stat == RenderSimStats.SimulationTimeOffset)
					{
						return simulationStatsSnapshot.SimulationTimeOffset;
					}
				}
				else if (stat <= RenderSimStats.InterpolationOffset)
				{
					if (stat == RenderSimStats.SimulationSpeed)
					{
						return simulationStatsSnapshot.SimulationSpeed;
					}
					if (stat == RenderSimStats.InterpolationOffset)
					{
						return simulationStatsSnapshot.InterpolationOffset;
					}
				}
				else
				{
					if (stat == RenderSimStats.InterpolationSpeed)
					{
						return simulationStatsSnapshot.InterpolationSpeed;
					}
					if (stat == RenderSimStats.InputInBandwidth)
					{
						return simulationStatsSnapshot.InputInBandwidth;
					}
				}
			}
			else if (stat <= RenderSimStats.ObjectsAllocatedMemoryInUse)
			{
				if (stat <= RenderSimStats.AverageOutPacketSize)
				{
					if (stat == RenderSimStats.InputOutBandwidth)
					{
						return simulationStatsSnapshot.InputOutBandwidth;
					}
					if (stat == RenderSimStats.AverageInPacketSize)
					{
						return simulationStatsSnapshot.InBandwidth / (float)Mathf.Max(simulationStatsSnapshot.InPackets, 1);
					}
					if (stat == RenderSimStats.AverageOutPacketSize)
					{
						return simulationStatsSnapshot.OutBandwidth / (float)Mathf.Max(simulationStatsSnapshot.OutPackets, 1);
					}
				}
				else
				{
					if (stat == RenderSimStats.InObjectUpdates)
					{
						return (float)simulationStatsSnapshot.InObjectUpdates;
					}
					if (stat == RenderSimStats.OutObjectUpdates)
					{
						return (float)simulationStatsSnapshot.OutObjectUpdates;
					}
					if (stat == RenderSimStats.ObjectsAllocatedMemoryInUse)
					{
						return (float)simulationStatsSnapshot.ObjectsAllocMemoryUsedInBytes;
					}
				}
			}
			else if (stat <= RenderSimStats.GeneralAllocatedMemoryFree)
			{
				if (stat == RenderSimStats.GeneralAllocatedMemoryInUse)
				{
					return (float)simulationStatsSnapshot.GeneralAllocMemoryUsedInBytes;
				}
				if (stat == RenderSimStats.ObjectsAllocatedMemoryFree)
				{
					return (float)simulationStatsSnapshot.ObjectsAllocMemoryFreeInBytes;
				}
				if (stat == RenderSimStats.GeneralAllocatedMemoryFree)
				{
					return (float)simulationStatsSnapshot.GeneralAllocMemoryFreeInBytes;
				}
			}
			else if (stat <= RenderSimStats.WordsWrittenSize)
			{
				if (stat == RenderSimStats.WordsWrittenCount)
				{
					return (float)simulationStatsSnapshot.WordsWrittenCount;
				}
				if (stat == RenderSimStats.WordsWrittenSize)
				{
					return (float)simulationStatsSnapshot.WordsWrittenSize;
				}
			}
			else
			{
				if (stat == RenderSimStats.WordsReadCount)
				{
					return (float)simulationStatsSnapshot.WordsReadCount;
				}
				if (stat == RenderSimStats.WordsReadSize)
				{
					return (float)simulationStatsSnapshot.WordsReadSize;
				}
			}
			return 0f;
		}

		public const float DEFAULT_GRAPH_HEIGHT = 150f;

		public const float DEFAULT_HEADER_HEIGHT = 50f;
	}
}
