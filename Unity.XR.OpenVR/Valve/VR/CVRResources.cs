using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRResources
	{
		internal CVRResources(IntPtr pInterface)
		{
			this.FnTable = (IVRResources)Marshal.PtrToStructure(pInterface, typeof(IVRResources));
		}

		public uint LoadSharedResource(string pchResourceName, string pchBuffer, uint unBufferLen)
		{
			IntPtr intPtr = Utils.ToUtf8(pchResourceName);
			uint result = this.FnTable.LoadSharedResource(intPtr, pchBuffer, unBufferLen);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public uint GetResourceFullPath(string pchResourceName, string pchResourceTypeDirectory, StringBuilder pchPathBuffer, uint unBufferLen)
		{
			IntPtr intPtr = Utils.ToUtf8(pchResourceName);
			IntPtr intPtr2 = Utils.ToUtf8(pchResourceTypeDirectory);
			uint result = this.FnTable.GetResourceFullPath(intPtr, intPtr2, pchPathBuffer, unBufferLen);
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
			return result;
		}

		private IVRResources FnTable;
	}
}
