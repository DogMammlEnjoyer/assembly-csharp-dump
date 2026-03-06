using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRDebug
	{
		internal CVRDebug(IntPtr pInterface)
		{
			this.FnTable = (IVRDebug)Marshal.PtrToStructure(pInterface, typeof(IVRDebug));
		}

		public EVRDebugError EmitVrProfilerEvent(string pchMessage)
		{
			IntPtr intPtr = Utils.ToUtf8(pchMessage);
			EVRDebugError result = this.FnTable.EmitVrProfilerEvent(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EVRDebugError BeginVrProfilerEvent(ref ulong pHandleOut)
		{
			pHandleOut = 0UL;
			return this.FnTable.BeginVrProfilerEvent(ref pHandleOut);
		}

		public EVRDebugError FinishVrProfilerEvent(ulong hHandle, string pchMessage)
		{
			IntPtr intPtr = Utils.ToUtf8(pchMessage);
			EVRDebugError result = this.FnTable.FinishVrProfilerEvent(hHandle, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint DriverDebugRequest(uint unDeviceIndex, string pchRequest, StringBuilder pchResponseBuffer, uint unResponseBufferSize)
		{
			IntPtr intPtr = Utils.ToUtf8(pchRequest);
			uint result = this.FnTable.DriverDebugRequest(unDeviceIndex, intPtr, pchResponseBuffer, unResponseBufferSize);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		private IVRDebug FnTable;
	}
}
