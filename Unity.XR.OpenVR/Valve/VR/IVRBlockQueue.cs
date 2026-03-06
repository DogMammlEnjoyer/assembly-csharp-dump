using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRBlockQueue
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._Create Create;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._Connect Connect;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._Destroy Destroy;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._AcquireWriteOnlyBlock AcquireWriteOnlyBlock;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._ReleaseWriteOnlyBlock ReleaseWriteOnlyBlock;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._WaitAndAcquireReadOnlyBlock WaitAndAcquireReadOnlyBlock;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._AcquireReadOnlyBlock AcquireReadOnlyBlock;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._ReleaseReadOnlyBlock ReleaseReadOnlyBlock;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRBlockQueue._QueueHasReader QueueHasReader;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _Create(ref ulong pulQueueHandle, IntPtr pchPath, uint unBlockDataSize, uint unBlockHeaderSize, uint unBlockCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _Connect(ref ulong pulQueueHandle, IntPtr pchPath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _Destroy(ulong ulQueueHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _AcquireWriteOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _ReleaseWriteOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _WaitAndAcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType, uint unTimeoutMs);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _AcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _ReleaseReadOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate EBlockQueueError _QueueHasReader(ulong ulQueueHandle, ref bool pbHasReaders);
	}
}
