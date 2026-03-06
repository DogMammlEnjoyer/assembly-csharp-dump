using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public struct IVRProperties
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRProperties._ReadPropertyBatch ReadPropertyBatch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRProperties._WritePropertyBatch WritePropertyBatch;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRProperties._GetPropErrorNameFromEnum GetPropErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRProperties._TrackedDeviceToPropertyContainer TrackedDeviceToPropertyContainer;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _ReadPropertyBatch(ulong ulContainerHandle, ref PropertyRead_t pBatch, uint unBatchEntryCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ETrackedPropertyError _WritePropertyBatch(ulong ulContainerHandle, ref PropertyWrite_t pBatch, uint unBatchEntryCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetPropErrorNameFromEnum(ETrackedPropertyError error);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate ulong _TrackedDeviceToPropertyContainer(uint nDevice);
	}
}
