using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal
{
	/// <summary>Provides access to internal properties of an <see cref="T:System.ApplicationIdentity" /> object.</summary>
	[ComVisible(false)]
	public static class InternalApplicationIdentityHelper
	{
		/// <summary>Gets an IDefinitionAppId Interface representing the unique identifier of an <see cref="T:System.ApplicationIdentity" /> object.</summary>
		/// <param name="id">The object from which to extract the identifier.</param>
		/// <returns>The unique identifier held by the <see cref="T:System.ApplicationIdentity" /> object.</returns>
		[MonoTODO]
		public static object GetInternalAppId(ApplicationIdentity id)
		{
			throw new NotImplementedException();
		}
	}
}
