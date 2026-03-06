using System;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a safe handle that represents a secret agreement value (NCRYPT_SECRET_HANDLE).</summary>
	public sealed class SafeNCryptSecretHandle : SafeNCryptHandle
	{
		protected override bool ReleaseNativeHandle()
		{
			return false;
		}
	}
}
