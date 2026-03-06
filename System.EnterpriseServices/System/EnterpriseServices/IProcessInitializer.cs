using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	/// <summary>Supports methods that can be called when a COM component starts up or shuts down.</summary>
	[Guid("1113f52d-dc7f-4943-aed6-88d04027e32a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	public interface IProcessInitializer
	{
		/// <summary>Performs shutdown actions. Called when Dllhost.exe is shut down.</summary>
		void Shutdown();

		/// <summary>Performs initialization at startup. Called when Dllhost.exe is started.</summary>
		/// <param name="punkProcessControl">In Microsoft Windows XP, a pointer to the <see langword="IUnknown" /> interface of the COM component starting up. In Microsoft Windows 2000, this argument is always <see langword="null" />.</param>
		void Startup([MarshalAs(UnmanagedType.IUnknown)] [In] object punkProcessControl);
	}
}
