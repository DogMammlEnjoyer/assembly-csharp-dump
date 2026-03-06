using System;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle that represents a key storage provider (NCRYPT_PROV_HANDLE).</summary>
	public sealed class SafeNCryptProviderHandle : SafeNCryptHandle
	{
		protected override bool ReleaseNativeHandle()
		{
			return false;
		}
	}
}
