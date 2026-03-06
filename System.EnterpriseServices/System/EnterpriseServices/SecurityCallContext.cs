using System;

namespace System.EnterpriseServices
{
	/// <summary>Describes the chain of callers leading up to the current method call.</summary>
	public sealed class SecurityCallContext
	{
		internal SecurityCallContext()
		{
		}

		internal SecurityCallContext(ISecurityCallContext context)
		{
		}

		/// <summary>Gets a <see cref="T:System.EnterpriseServices.SecurityCallers" /> object that describes the caller.</summary>
		/// <returns>The <see cref="T:System.EnterpriseServices.SecurityCallers" /> object that describes the caller.</returns>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">There is no security context.</exception>
		public SecurityCallers Callers
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.EnterpriseServices.SecurityCallContext" /> object that describes the security call context.</summary>
		/// <returns>The <see cref="T:System.EnterpriseServices.SecurityCallContext" /> object that describes the security call context.</returns>
		public static SecurityCallContext CurrentCall
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.EnterpriseServices.SecurityIdentity" /> object that describes the direct caller of this method.</summary>
		/// <returns>A <see cref="T:System.EnterpriseServices.SecurityIdentity" /> value.</returns>
		public SecurityIdentity DirectCaller
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Determines whether security checks are enabled in the current context.</summary>
		/// <returns>
		///   <see langword="true" /> if security checks are enabled in the current context; otherwise, <see langword="false" />.</returns>
		public bool IsSecurityEnabled
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the <see langword="MinAuthenticationLevel" /> value from the <see langword="ISecurityCallContext" /> collection in COM+.</summary>
		/// <returns>The <see langword="MinAuthenticationLevel" /> value from the <see langword="ISecurityCallContext" /> collection in COM+.</returns>
		public int MinAuthenticationLevel
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets the <see langword="NumCallers" /> value from the <see langword="ISecurityCallContext" /> collection in COM+.</summary>
		/// <returns>The <see langword="NumCallers" /> value from the <see langword="ISecurityCallContext" /> collection in COM+.</returns>
		public int NumCallers
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Gets a <see cref="T:System.EnterpriseServices.SecurityIdentity" /> that describes the original caller.</summary>
		/// <returns>One of the <see cref="T:System.EnterpriseServices.SecurityIdentity" /> values.</returns>
		public SecurityIdentity OriginalCaller
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Verifies that the direct caller is a member of the specified role.</summary>
		/// <param name="role">The specified role.</param>
		/// <returns>
		///   <see langword="true" /> if the direct caller is a member of the specified role; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsCallerInRole(string role)
		{
			throw new NotImplementedException();
		}

		/// <summary>Verifies that the specified user is in the specified role.</summary>
		/// <param name="user">The specified user.</param>
		/// <param name="role">The specified role.</param>
		/// <returns>
		///   <see langword="true" /> if the specified user is a member of the specified role; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public bool IsUserInRole(string user, string role)
		{
			throw new NotImplementedException();
		}
	}
}
