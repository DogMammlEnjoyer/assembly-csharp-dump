using System;

namespace System.EnterpriseServices
{
	/// <summary>Contains information that regards an identity in a COM+ call chain.</summary>
	public sealed class SecurityIdentity
	{
		[MonoTODO]
		internal SecurityIdentity()
		{
		}

		[MonoTODO]
		internal SecurityIdentity(ISecurityIdentityColl collection)
		{
		}

		/// <summary>Gets the name of the user described by this identity.</summary>
		/// <returns>The name of the user described by this identity.</returns>
		public string AccountName
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the authentication level of the user described by this identity.</summary>
		/// <returns>One of the <see cref="T:System.EnterpriseServices.AuthenticationOption" /> values.</returns>
		public AuthenticationOption AuthenticationLevel
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the authentication service described by this identity.</summary>
		/// <returns>The authentication service described by this identity.</returns>
		public int AuthenticationService
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the impersonation level of the user described by this identity.</summary>
		/// <returns>A <see cref="T:System.EnterpriseServices.ImpersonationLevelOption" /> value.</returns>
		public ImpersonationLevelOption ImpersonationLevel
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
