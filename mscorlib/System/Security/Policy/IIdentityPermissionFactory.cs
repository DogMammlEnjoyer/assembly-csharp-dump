using System;

namespace System.Security.Policy
{
	/// <summary>Defines the method that creates a new identity permission.</summary>
	public interface IIdentityPermissionFactory
	{
		/// <summary>Creates a new identity permission for the specified evidence.</summary>
		/// <param name="evidence">The evidence from which to create the new identity permission.</param>
		/// <returns>The new identity permission.</returns>
		IPermission CreateIdentityPermission(Evidence evidence);
	}
}
