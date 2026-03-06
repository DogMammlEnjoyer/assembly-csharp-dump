using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Represents a combination of a user's identity, an access mask, and an access control type (allow or deny). An AccessRule`1 object also contains information about the how the rule is inherited by child objects and how that inheritance is propagated.</summary>
	/// <typeparam name="T">The access rights type for the access rule.</typeparam>
	public class AccessRule<T> : AccessRule where T : struct
	{
		/// <summary>Initializes a new instance of the AccessRule'1 class by using the specified values.</summary>
		/// <param name="identity">The identity to which the access rule applies.</param>
		/// <param name="rights">The rights of the access rule.</param>
		/// <param name="type">The valid access control type.</param>
		public AccessRule(string identity, T rights, AccessControlType type) : this(new NTAccount(identity), rights, type)
		{
		}

		/// <summary>Initializes a new instance of the AccessRule'1 class by using the specified values.</summary>
		/// <param name="identity">The identity to which the access rule applies.</param>
		/// <param name="rights">The rights of the access rule.</param>
		/// <param name="type">The valid access control type.</param>
		public AccessRule(IdentityReference identity, T rights, AccessControlType type) : this(identity, rights, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		/// <summary>Initializes a new instance of the AccessRule'1 class by using the specified values.</summary>
		/// <param name="identity">The identity to which the access rule applies.</param>
		/// <param name="rights">The rights of the access rule.</param>
		/// <param name="inheritanceFlags">The inheritance properties of the access rule.</param>
		/// <param name="propagationFlags">Whether inherited access rules are automatically propagated. The propagation flags are ignored if <paramref name="inheritanceFlags" /> is set to <see cref="F:System.Security.AccessControl.InheritanceFlags.None" />.</param>
		/// <param name="type">The valid access control type.</param>
		public AccessRule(string identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type) : this(new NTAccount(identity), rights, inheritanceFlags, propagationFlags, type)
		{
		}

		/// <summary>Initializes a new instance of the AccessRule'1 class by using the specified values.</summary>
		/// <param name="identity">The identity to which the access rule applies.</param>
		/// <param name="rights">The rights of the access rule.</param>
		/// <param name="inheritanceFlags">The inheritance properties of the access rule.</param>
		/// <param name="propagationFlags">Whether inherited access rules are automatically propagated. The propagation flags are ignored if <paramref name="inheritanceFlags" /> is set to <see cref="F:System.Security.AccessControl.InheritanceFlags.None" />.</param>
		/// <param name="type">The valid access control type.</param>
		public AccessRule(IdentityReference identity, T rights, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type) : this(identity, (int)((object)rights), false, inheritanceFlags, propagationFlags, type)
		{
		}

		internal AccessRule(IdentityReference identity, int rights, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type) : base(identity, rights, isInherited, inheritanceFlags, propagationFlags, type)
		{
		}

		/// <summary>Gets the rights of the current instance.</summary>
		/// <returns>The rights, cast as type &lt;T&gt;, of the current instance.</returns>
		public T Rights
		{
			get
			{
				return (T)((object)base.AccessMask);
			}
		}
	}
}
