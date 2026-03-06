using System;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace System.Security.Principal
{
	/// <summary>Represents a generic principal.</summary>
	[ComVisible(true)]
	[Serializable]
	public class GenericPrincipal : ClaimsPrincipal
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.Principal.GenericPrincipal" /> class from a user identity and an array of role names to which the user represented by that identity belongs.</summary>
		/// <param name="identity">A basic implementation of <see cref="T:System.Security.Principal.IIdentity" /> that represents any user.</param>
		/// <param name="roles">An array of role names to which the user represented by the <paramref name="identity" /> parameter belongs.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="identity" /> parameter is <see langword="null" />.</exception>
		public GenericPrincipal(IIdentity identity, string[] roles)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.m_identity = identity;
			if (roles != null)
			{
				this.m_roles = new string[roles.Length];
				for (int i = 0; i < roles.Length; i++)
				{
					this.m_roles[i] = roles[i];
				}
			}
		}

		internal string[] Roles
		{
			get
			{
				return this.m_roles;
			}
		}

		/// <summary>Gets the <see cref="T:System.Security.Principal.GenericIdentity" /> of the user represented by the current <see cref="T:System.Security.Principal.GenericPrincipal" />.</summary>
		/// <returns>The <see cref="T:System.Security.Principal.GenericIdentity" /> of the user represented by the <see cref="T:System.Security.Principal.GenericPrincipal" />.</returns>
		public override IIdentity Identity
		{
			get
			{
				return this.m_identity;
			}
		}

		/// <summary>Determines whether the current <see cref="T:System.Security.Principal.GenericPrincipal" /> belongs to the specified role.</summary>
		/// <param name="role">The name of the role for which to check membership.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Security.Principal.GenericPrincipal" /> is a member of the specified role; otherwise, <see langword="false" />.</returns>
		public override bool IsInRole(string role)
		{
			if (this.m_roles == null)
			{
				return false;
			}
			int length = role.Length;
			foreach (string text in this.m_roles)
			{
				if (text != null && length == text.Length && string.Compare(role, 0, text, 0, length, true) == 0)
				{
					return true;
				}
			}
			return false;
		}

		private IIdentity m_identity;

		private string[] m_roles;
	}
}
