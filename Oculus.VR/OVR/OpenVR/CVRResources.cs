using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRResources
	{
		internal CVRResources(IntPtr pInterface)
		{
			this.FnTable = (IVRResources)Marshal.PtrToStructure(pInterface, typeof(IVRResources));
		}

		public uint LoadSharedResource(string pchResourceName, string pchBuffer, uint unBufferLen)
		{
			return this.FnTable.LoadSharedResource(pchResourceName, pchBuffer, unBufferLen);
		}

		public uint GetResourceFullPath(string pchResourceName, string pchResourceTypeDirectory, StringBuilder pchPathBuffer, uint unBufferLen)
		{
			return this.FnTable.GetResourceFullPath(pchResourceName, pchResourceTypeDirectory, pchPathBuffer, unBufferLen);
		}

		private IVRResources FnTable;
	}
}
