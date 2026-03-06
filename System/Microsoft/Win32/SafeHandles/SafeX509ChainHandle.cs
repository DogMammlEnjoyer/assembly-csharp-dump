using System;
using Unity;

namespace Microsoft.Win32.SafeHandles
{
	/// <summary>Provides a wrapper class that represents the handle of an X.509 chain object. For more information, see <see cref="T:System.Security.Cryptography.X509Certificates.X509Chain" />.</summary>
	public sealed class SafeX509ChainHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeX509ChainHandle(IntPtr handle) : base(true)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		protected override bool ReleaseHandle()
		{
			throw new NotImplementedException();
		}

		internal SafeX509ChainHandle()
		{
			ThrowStub.ThrowNotSupportedException();
		}
	}
}
