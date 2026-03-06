using System;

namespace System.Security.Permissions
{
	/// <summary>Allows a permission to expose an unrestricted state.</summary>
	public interface IUnrestrictedPermission
	{
		/// <summary>Returns a value indicating whether unrestricted access to the resource protected by the permission is allowed.</summary>
		/// <returns>
		///   <see langword="true" /> if unrestricted use of the resource protected by the permission is allowed; otherwise, <see langword="false" />.</returns>
		bool IsUnrestricted();
	}
}
