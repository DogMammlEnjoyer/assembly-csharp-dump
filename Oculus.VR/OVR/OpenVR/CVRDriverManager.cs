using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRDriverManager
	{
		internal CVRDriverManager(IntPtr pInterface)
		{
			this.FnTable = (IVRDriverManager)Marshal.PtrToStructure(pInterface, typeof(IVRDriverManager));
		}

		public uint GetDriverCount()
		{
			return this.FnTable.GetDriverCount();
		}

		public uint GetDriverName(uint nDriver, StringBuilder pchValue, uint unBufferSize)
		{
			return this.FnTable.GetDriverName(nDriver, pchValue, unBufferSize);
		}

		public ulong GetDriverHandle(string pchDriverName)
		{
			return this.FnTable.GetDriverHandle(pchDriverName);
		}

		private IVRDriverManager FnTable;
	}
}
