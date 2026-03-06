using System;
using System.Runtime.InteropServices;

namespace OVR.OpenVR
{
	public class CVRIOBuffer
	{
		internal CVRIOBuffer(IntPtr pInterface)
		{
			this.FnTable = (IVRIOBuffer)Marshal.PtrToStructure(pInterface, typeof(IVRIOBuffer));
		}

		public EIOBufferError Open(string pchPath, EIOBufferMode mode, uint unElementSize, uint unElements, ref ulong pulBuffer)
		{
			pulBuffer = 0UL;
			return this.FnTable.Open(pchPath, mode, unElementSize, unElements, ref pulBuffer);
		}

		public EIOBufferError Close(ulong ulBuffer)
		{
			return this.FnTable.Close(ulBuffer);
		}

		public EIOBufferError Read(ulong ulBuffer, IntPtr pDst, uint unBytes, ref uint punRead)
		{
			punRead = 0U;
			return this.FnTable.Read(ulBuffer, pDst, unBytes, ref punRead);
		}

		public EIOBufferError Write(ulong ulBuffer, IntPtr pSrc, uint unBytes)
		{
			return this.FnTable.Write(ulBuffer, pSrc, unBytes);
		}

		public ulong PropertyContainer(ulong ulBuffer)
		{
			return this.FnTable.PropertyContainer(ulBuffer);
		}

		private IVRIOBuffer FnTable;
	}
}
