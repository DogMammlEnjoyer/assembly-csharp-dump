using System;

namespace Fusion.Statistics
{
	[Flags]
	public enum RenderSimStats
	{
		InPackets = 1,
		OutPackets = 2,
		RTT = 4,
		InBandwidth = 8,
		OutBandwidth = 16,
		Resimulations = 32,
		ForwardTicks = 64,
		InputReceiveDelta = 128,
		TimeResets = 256,
		StateReceiveDelta = 512,
		SimulationTimeOffset = 1024,
		SimulationSpeed = 2048,
		InterpolationOffset = 4096,
		InterpolationSpeed = 8192,
		InputInBandwidth = 16384,
		InputOutBandwidth = 32768,
		AverageInPacketSize = 65536,
		AverageOutPacketSize = 131072,
		InObjectUpdates = 262144,
		OutObjectUpdates = 524288,
		ObjectsAllocatedMemoryInUse = 1048576,
		GeneralAllocatedMemoryInUse = 2097152,
		ObjectsAllocatedMemoryFree = 4194304,
		GeneralAllocatedMemoryFree = 8388608,
		WordsWrittenCount = 16777216,
		WordsWrittenSize = 33554432,
		WordsReadCount = 67108864,
		WordsReadSize = 134217728
	}
}
