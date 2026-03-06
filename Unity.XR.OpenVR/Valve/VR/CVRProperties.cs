using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRProperties
	{
		internal CVRProperties(IntPtr pInterface)
		{
			this.FnTable = (IVRProperties)Marshal.PtrToStructure(pInterface, typeof(IVRProperties));
		}

		public ETrackedPropertyError ReadPropertyBatch(ulong ulContainerHandle, ref PropertyRead_t pBatch, uint unBatchEntryCount)
		{
			return this.FnTable.ReadPropertyBatch(ulContainerHandle, ref pBatch, unBatchEntryCount);
		}

		public ETrackedPropertyError WritePropertyBatch(ulong ulContainerHandle, ref PropertyWrite_t pBatch, uint unBatchEntryCount)
		{
			return this.FnTable.WritePropertyBatch(ulContainerHandle, ref pBatch, unBatchEntryCount);
		}

		public string GetPropErrorNameFromEnum(ETrackedPropertyError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetPropErrorNameFromEnum(error));
		}

		public ulong TrackedDeviceToPropertyContainer(uint nDevice)
		{
			return this.FnTable.TrackedDeviceToPropertyContainer(nDevice);
		}

		private IVRProperties FnTable;
	}
}
