using System;

namespace Backtrace.Unity.Types
{
	public enum MiniDumpType : uint
	{
		None = 524286U,
		Normal = 0U,
		WithDataSegs,
		WithFullMemory,
		WithHandleData = 4U,
		FilterMemory = 8U,
		ScanMemory = 16U,
		WithUnloadedModules = 32U,
		WithIndirectlyReferencedMemory = 64U,
		FilterModulePaths = 128U,
		WithProcessThreadData = 256U,
		WithPrivateReadWriteMemory = 512U,
		WithoutOptionalData = 1024U,
		WithFullMemoryInfo = 2048U,
		WithThreadInfo = 4096U,
		WithCodeSegs = 8192U,
		WithoutAuxiliaryState = 16384U,
		WithFullAuxiliaryState = 32768U,
		WithPrivateWriteCopyMemory = 65536U,
		IgnoreInaccessibleMemory = 131072U,
		ValidTypeFlags = 262143U
	}
}
