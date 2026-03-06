using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRBlockQueue
	{
		internal CVRBlockQueue(IntPtr pInterface)
		{
			this.FnTable = (IVRBlockQueue)Marshal.PtrToStructure(pInterface, typeof(IVRBlockQueue));
		}

		public EBlockQueueError Create(ref ulong pulQueueHandle, string pchPath, uint unBlockDataSize, uint unBlockHeaderSize, uint unBlockCount)
		{
			pulQueueHandle = 0UL;
			IntPtr intPtr = Utils.ToUtf8(pchPath);
			EBlockQueueError result = this.FnTable.Create(ref pulQueueHandle, intPtr, unBlockDataSize, unBlockHeaderSize, unBlockCount);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EBlockQueueError Connect(ref ulong pulQueueHandle, string pchPath)
		{
			pulQueueHandle = 0UL;
			IntPtr intPtr = Utils.ToUtf8(pchPath);
			EBlockQueueError result = this.FnTable.Connect(ref pulQueueHandle, intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public EBlockQueueError Destroy(ulong ulQueueHandle)
		{
			return this.FnTable.Destroy(ulQueueHandle);
		}

		public EBlockQueueError AcquireWriteOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer)
		{
			pulBlockHandle = 0UL;
			return this.FnTable.AcquireWriteOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer);
		}

		public EBlockQueueError ReleaseWriteOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle)
		{
			return this.FnTable.ReleaseWriteOnlyBlock(ulQueueHandle, ulBlockHandle);
		}

		public EBlockQueueError WaitAndAcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType, uint unTimeoutMs)
		{
			pulBlockHandle = 0UL;
			return this.FnTable.WaitAndAcquireReadOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer, eReadType, unTimeoutMs);
		}

		public EBlockQueueError AcquireReadOnlyBlock(ulong ulQueueHandle, ref ulong pulBlockHandle, ref IntPtr ppvBuffer, EBlockQueueReadType eReadType)
		{
			pulBlockHandle = 0UL;
			return this.FnTable.AcquireReadOnlyBlock(ulQueueHandle, ref pulBlockHandle, ref ppvBuffer, eReadType);
		}

		public EBlockQueueError ReleaseReadOnlyBlock(ulong ulQueueHandle, ulong ulBlockHandle)
		{
			return this.FnTable.ReleaseReadOnlyBlock(ulQueueHandle, ulBlockHandle);
		}

		public EBlockQueueError QueueHasReader(ulong ulQueueHandle, ref bool pbHasReaders)
		{
			pbHasReaders = false;
			return this.FnTable.QueueHasReader(ulQueueHandle, ref pbHasReaders);
		}

		private IVRBlockQueue FnTable;
	}
}
