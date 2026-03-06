using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRPaths
	{
		internal CVRPaths(IntPtr pInterface)
		{
			this.FnTable = (IVRPaths)Marshal.PtrToStructure(pInterface, typeof(IVRPaths));
		}

		public ETrackedPropertyError ReadPathBatch(ulong ulRootHandle, ref PathRead_t pBatch, uint unBatchEntryCount)
		{
			return this.FnTable.ReadPathBatch(ulRootHandle, ref pBatch, unBatchEntryCount);
		}

		public ETrackedPropertyError WritePathBatch(ulong ulRootHandle, ref PathWrite_t pBatch, uint unBatchEntryCount)
		{
			return this.FnTable.WritePathBatch(ulRootHandle, ref pBatch, unBatchEntryCount);
		}

		public ETrackedPropertyError StringToHandle(ref ulong pHandle, string pchPath)
		{
			pHandle = 0UL;
			IntPtr intPtr = Utils.ToUtf8(pchPath);
			ETrackedPropertyError result = this.FnTable.StringToHandle(ref pHandle, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public ETrackedPropertyError HandleToString(ulong pHandle, string pchBuffer, uint unBufferSize, ref uint punBufferSizeUsed)
		{
			punBufferSizeUsed = 0U;
			return this.FnTable.HandleToString(pHandle, pchBuffer, unBufferSize, ref punBufferSizeUsed);
		}

		private IVRPaths FnTable;
	}
}
