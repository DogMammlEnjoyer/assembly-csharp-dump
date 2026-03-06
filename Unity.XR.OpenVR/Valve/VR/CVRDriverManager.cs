using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
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
			IntPtr intPtr = Utils.ToUtf8(pchDriverName);
			ulong result = this.FnTable.GetDriverHandle(intPtr);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public bool IsEnabled(uint nDriver)
		{
			return this.FnTable.IsEnabled(nDriver);
		}

		private IVRDriverManager FnTable;
	}
}
