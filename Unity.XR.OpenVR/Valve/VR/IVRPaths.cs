using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRPaths
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRPaths._ReadPathBatch ReadPathBatch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRPaths._WritePathBatch WritePathBatch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRPaths._StringToHandle StringToHandle;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRPaths._HandleToString HandleToString;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _ReadPathBatch(ulong ulRootHandle, ref PathRead_t pBatch, uint unBatchEntryCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _WritePathBatch(ulong ulRootHandle, ref PathWrite_t pBatch, uint unBatchEntryCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _StringToHandle(ref ulong pHandle, IntPtr pchPath);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _HandleToString(ulong pHandle, string pchBuffer, uint unBufferSize, ref uint punBufferSizeUsed);
	}
}
