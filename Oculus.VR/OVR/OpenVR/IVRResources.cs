using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public struct IVRResources
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRResources._LoadSharedResource LoadSharedResource;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRResources._GetResourceFullPath GetResourceFullPath;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _LoadSharedResource(string pchResourceName, string pchBuffer, uint unBufferLen);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate uint _GetResourceFullPath(string pchResourceName, string pchResourceTypeDirectory, StringBuilder pchPathBuffer, uint unBufferLen);
	}
}
