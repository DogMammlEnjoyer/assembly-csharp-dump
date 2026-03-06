using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Represents a combination of a user's identity and an access mask.</summary>
	/// <typeparam name="T">The type of the audit rule.</typeparam>
	public class AuditRule<T> : AuditRule where T : struct
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule`1" /> class by using the specified values.</summary>
		/// <param name="identity">The identity to which the audit rule applies.</param>
		/// <param name="rights">The rights of the audit rule.</param>
		/// <param name="flags">The properties of the audit rule.</param>
		public AuditRule(string identity, T rights, AuditFlags flags) : this(new NTAccount(identity), rights, flags)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule`1" /> class by using the specified values.</summary>
		/// <param name="identity">The identity to which this audit rule applies.</param>
		/// <param name="rights">The rights of the audit rule.</param>
		/// <param name="flags">The conditions for which the rule is audited.</param>
		public AuditRule(IdentityReference identity, T rights, AuditFlags flags) : this(identity, rights, InheritanceFlags.None, PropagationFlags.None, flags)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule`1" /> class by using the specified values.</summary>
		/// <param name="identity">The identity to which the audit rule applies.</param>
		/// <param name="rights">The rights of the audit rule.</param>
		/// <param name="inheritanceFlags">The inheritance properties of the audit rule.</param>
		/// <param name="propagationFlags">Whether inherited audit rules are automatically propagated.</param>
		/// <param name="flags">The conditions for which the rule is audited.</param>
		public AuditRule(string identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags) : this(new NTAccount(identity), rights, inheritanceFlags, propagationFlags, flags)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule`1" /> class by using the specified values.</summary>
		/// <param name="identity">The identity to which the audit rule applies.</param>
		/// <param name="rights">The rights of the audit rule.</param>
		/// <param name="inheritanceFlags">The inheritance properties of the audit rule.</param>
		/// <param name="propagationFlags">Whether inherited audit rules are automatically propagated.</param>
		/// <param name="flags">The conditions for which the rule is audited.</param>
		public AuditRule(IdentityReference identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags) : this(identity, (int)((object)rights), false, inheritanceFlags, propagationFlags, flags)
		{
		}

		internal AuditRule(IdentityReference identity, int rights, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags) : base(identity, rights, isInherited, inheritanceFlags, propagationFlags, flags)
		{
		}

		/// <summary>Gets the rights of the audit rule.</summary>
		/// <returns>The rights of the audit rule.</returns>
		public T Rights
		{
			get
			{
				return (T)((object)base.AccessMask);
			}
		}
	}
}
