using System;
using System.Security.AccessControl;
using System.Security.Principal;

namespace System.IO.Pipes
{
	/// <summary>Represents an abstraction of an access control entry (ACE) that defines an access rule for a pipe.</summary>
	public sealed class PipeAccessRule : AccessRule
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Pipes.PipeAccessRule" /> class with the specified identity, pipe access rights, and access control type.</summary>
		/// <param name="identity">The name of the user account.</param>
		/// <param name="rights">One of the <see cref="T:System.IO.Pipes.PipeAccessRights" /> values that specifies the type of operation associated with the access rule.</param>
		/// <param name="type">One of the <see cref="T:System.Security.AccessControl.AccessControlType" /> values that specifies whether to allow or deny the operation.</param>
		public PipeAccessRule(string identity, PipeAccessRights rights, AccessControlType type) : this(new NTAccount(identity), PipeAccessRule.AccessMaskFromRights(rights, type), false, type)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.IO.Pipes.PipeAccessRule" /> class with the specified identity, pipe access rights, and access control type.</summary>
		/// <param name="identity">An <see cref="T:System.Security.Principal.IdentityReference" /> object that encapsulates a reference to a user account.</param>
		/// <param name="rights">One of the <see cref="T:System.IO.Pipes.PipeAccessRights" /> values that specifies the type of operation associated with the access rule.</param>
		/// <param name="type">One of the <see cref="T:System.Security.AccessControl.AccessControlType" /> values that specifies whether to allow or deny the operation.</param>
		public PipeAccessRule(IdentityReference identity, PipeAccessRights rights, AccessControlType type) : this(identity, PipeAccessRule.AccessMaskFromRights(rights, type), false, type)
		{
		}

		internal PipeAccessRule(IdentityReference identity, int accessMask, bool isInherited, AccessControlType type) : base(identity, accessMask, isInherited, InheritanceFlags.None, PropagationFlags.None, type)
		{
		}

		/// <summary>Gets the <see cref="T:System.IO.Pipes.PipeAccessRights" /> flags that are associated with the current <see cref="T:System.IO.Pipes.PipeAccessRule" /> object.</summary>
		/// <returns>A bitwise combination of the <see cref="T:System.IO.Pipes.PipeAccessRights" /> values.</returns>
		public PipeAccessRights PipeAccessRights
		{
			get
			{
				return PipeAccessRule.RightsFromAccessMask(base.AccessMask);
			}
		}

		internal static int AccessMaskFromRights(PipeAccessRights rights, AccessControlType controlType)
		{
			if (rights < (PipeAccessRights)0 || rights > (PipeAccessRights.ReadData | PipeAccessRights.WriteData | PipeAccessRights.ReadAttributes | PipeAccessRights.WriteAttributes | PipeAccessRights.ReadExtendedAttributes | PipeAccessRights.WriteExtendedAttributes | PipeAccessRights.CreateNewInstance | PipeAccessRights.Delete | PipeAccessRights.ReadPermissions | PipeAccessRights.ChangePermissions | PipeAccessRights.TakeOwnership | PipeAccessRights.Synchronize | PipeAccessRights.AccessSystemSecurity))
			{
				throw new ArgumentOutOfRangeException("rights", "Invalid PipeAccessRights value.");
			}
			if (controlType == AccessControlType.Allow)
			{
				rights |= PipeAccessRights.Synchronize;
			}
			else if (controlType == AccessControlType.Deny && rights != PipeAccessRights.FullControl)
			{
				rights &= ~PipeAccessRights.Synchronize;
			}
			return (int)rights;
		}

		internal static PipeAccessRights RightsFromAccessMask(int accessMask)
		{
			return (PipeAccessRights)accessMask;
		}
	}
}
