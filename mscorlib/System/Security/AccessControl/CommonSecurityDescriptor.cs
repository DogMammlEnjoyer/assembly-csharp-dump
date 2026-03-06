using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	/// <summary>Represents a security descriptor. A security descriptor includes an owner, a primary group, a Discretionary Access Control List (DACL), and a System Access Control List (SACL).</summary>
	public sealed class CommonSecurityDescriptor : GenericSecurityDescriptor
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> class from the specified <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new security descriptor is associated with a container object.</param>
		/// <param name="isDS">
		///   <see langword="true" /> if the new security descriptor is associated with a directory object.</param>
		/// <param name="rawSecurityDescriptor">The <see cref="T:System.Security.AccessControl.RawSecurityDescriptor" /> object from which to create the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		public CommonSecurityDescriptor(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			this.Init(isContainer, isDS, rawSecurityDescriptor);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> class from the specified Security Descriptor Definition Language (SDDL) string.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new security descriptor is associated with a container object.</param>
		/// <param name="isDS">
		///   <see langword="true" /> if the new security descriptor is associated with a directory object.</param>
		/// <param name="sddlForm">The SDDL string from which to create the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		public CommonSecurityDescriptor(bool isContainer, bool isDS, string sddlForm)
		{
			this.Init(isContainer, isDS, new RawSecurityDescriptor(sddlForm));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> class from the specified array of byte values.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new security descriptor is associated with a container object.</param>
		/// <param name="isDS">
		///   <see langword="true" /> if the new security descriptor is associated with a directory object.</param>
		/// <param name="binaryForm">The array of byte values from which to create the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		/// <param name="offset">The offset in the <paramref name="binaryForm" /> array at which to begin copying.</param>
		public CommonSecurityDescriptor(bool isContainer, bool isDS, byte[] binaryForm, int offset)
		{
			this.Init(isContainer, isDS, new RawSecurityDescriptor(binaryForm, offset));
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> class from the specified information.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new security descriptor is associated with a container object.</param>
		/// <param name="isDS">
		///   <see langword="true" /> if the new security descriptor is associated with a directory object.</param>
		/// <param name="flags">Flags that specify behavior of the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		/// <param name="owner">The owner for the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		/// <param name="group">The primary group for the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		/// <param name="systemAcl">The System Access Control List (SACL) for the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		/// <param name="discretionaryAcl">The Discretionary Access Control List (DACL) for the new <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</param>
		public CommonSecurityDescriptor(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.Init(isContainer, isDS, flags, owner, group, systemAcl, discretionaryAcl);
		}

		private void Init(bool isContainer, bool isDS, RawSecurityDescriptor rawSecurityDescriptor)
		{
			if (rawSecurityDescriptor == null)
			{
				throw new ArgumentNullException("rawSecurityDescriptor");
			}
			SystemAcl systemAcl = null;
			if (rawSecurityDescriptor.SystemAcl != null)
			{
				systemAcl = new SystemAcl(isContainer, isDS, rawSecurityDescriptor.SystemAcl);
			}
			DiscretionaryAcl discretionaryAcl = null;
			if (rawSecurityDescriptor.DiscretionaryAcl != null)
			{
				discretionaryAcl = new DiscretionaryAcl(isContainer, isDS, rawSecurityDescriptor.DiscretionaryAcl);
			}
			this.Init(isContainer, isDS, rawSecurityDescriptor.ControlFlags, rawSecurityDescriptor.Owner, rawSecurityDescriptor.Group, systemAcl, discretionaryAcl);
		}

		private void Init(bool isContainer, bool isDS, ControlFlags flags, SecurityIdentifier owner, SecurityIdentifier group, SystemAcl systemAcl, DiscretionaryAcl discretionaryAcl)
		{
			this.flags = (flags & ~ControlFlags.SystemAclPresent);
			this.is_container = isContainer;
			this.is_ds = isDS;
			this.Owner = owner;
			this.Group = group;
			this.SystemAcl = systemAcl;
			this.DiscretionaryAcl = discretionaryAcl;
		}

		/// <summary>Gets values that specify behavior of the <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</summary>
		/// <returns>One or more values of the <see cref="T:System.Security.AccessControl.ControlFlags" /> enumeration combined with a logical OR operation.</returns>
		public override ControlFlags ControlFlags
		{
			get
			{
				ControlFlags controlFlags = this.flags;
				controlFlags |= ControlFlags.DiscretionaryAclPresent;
				controlFlags |= ControlFlags.SelfRelative;
				if (this.SystemAcl != null)
				{
					controlFlags |= ControlFlags.SystemAclPresent;
				}
				return controlFlags;
			}
		}

		/// <summary>Gets or sets the discretionary access control list (DACL) for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object. The DACL contains access rules.</summary>
		/// <returns>The DACL for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</returns>
		public DiscretionaryAcl DiscretionaryAcl
		{
			get
			{
				return this.discretionary_acl;
			}
			set
			{
				if (value == null)
				{
					value = new DiscretionaryAcl(this.IsContainer, this.IsDS, 1);
					value.AddAccess(AccessControlType.Allow, new SecurityIdentifier("WD"), -1, this.IsContainer ? (InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit) : InheritanceFlags.None, PropagationFlags.None);
					value.IsAefa = true;
				}
				this.CheckAclConsistency(value);
				this.discretionary_acl = value;
			}
		}

		internal override GenericAcl InternalDacl
		{
			get
			{
				return this.DiscretionaryAcl;
			}
		}

		/// <summary>Gets or sets the primary group for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</summary>
		/// <returns>The primary group for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</returns>
		public override SecurityIdentifier Group
		{
			get
			{
				return this.group;
			}
			set
			{
				this.group = value;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is a container object.</summary>
		/// <returns>
		///   <see langword="true" /> if the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is a container object; otherwise, <see langword="false" />.</returns>
		public bool IsContainer
		{
			get
			{
				return this.is_container;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is in canonical order.</summary>
		/// <returns>
		///   <see langword="true" /> if the DACL associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is in canonical order; otherwise, <see langword="false" />.</returns>
		public bool IsDiscretionaryAclCanonical
		{
			get
			{
				return this.DiscretionaryAcl.IsCanonical;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is a directory object.</summary>
		/// <returns>
		///   <see langword="true" /> if the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is a directory object; otherwise, <see langword="false" />.</returns>
		public bool IsDS
		{
			get
			{
				return this.is_ds;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is in canonical order.</summary>
		/// <returns>
		///   <see langword="true" /> if the SACL associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object is in canonical order; otherwise, <see langword="false" />.</returns>
		public bool IsSystemAclCanonical
		{
			get
			{
				return this.SystemAcl == null || this.SystemAcl.IsCanonical;
			}
		}

		/// <summary>Gets or sets the owner of the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</summary>
		/// <returns>The owner of the object associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</returns>
		public override SecurityIdentifier Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
			}
		}

		/// <summary>Gets or sets the System Access Control List (SACL) for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object. The SACL contains audit rules.</summary>
		/// <returns>The SACL for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</returns>
		public SystemAcl SystemAcl
		{
			get
			{
				return this.system_acl;
			}
			set
			{
				if (value != null)
				{
					this.CheckAclConsistency(value);
				}
				this.system_acl = value;
			}
		}

		internal override GenericAcl InternalSacl
		{
			get
			{
				return this.SystemAcl;
			}
		}

		/// <summary>Removes all access rules for the specified security identifier from the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</summary>
		/// <param name="sid">The security identifier for which to remove access rules.</param>
		public void PurgeAccessControl(SecurityIdentifier sid)
		{
			this.DiscretionaryAcl.Purge(sid);
		}

		/// <summary>Removes all audit rules for the specified security identifier from the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object.</summary>
		/// <param name="sid">The security identifier for which to remove audit rules.</param>
		public void PurgeAudit(SecurityIdentifier sid)
		{
			if (this.SystemAcl != null)
			{
				this.SystemAcl.Purge(sid);
			}
		}

		/// <summary>Sets the inheritance protection for the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object. DACLs that are protected do not inherit access rules from parent containers.</summary>
		/// <param name="isProtected">
		///   <see langword="true" /> to protect the DACL from inheritance.</param>
		/// <param name="preserveInheritance">
		///   <see langword="true" /> to keep inherited access rules in the DACL; <see langword="false" /> to remove inherited access rules from the DACL.</param>
		public void SetDiscretionaryAclProtection(bool isProtected, bool preserveInheritance)
		{
			this.DiscretionaryAcl.IsAefa = false;
			if (!isProtected)
			{
				this.flags &= ~ControlFlags.DiscretionaryAclProtected;
				return;
			}
			this.flags |= ControlFlags.DiscretionaryAclProtected;
			if (!preserveInheritance)
			{
				this.DiscretionaryAcl.RemoveInheritedAces();
			}
		}

		/// <summary>Sets the inheritance protection for the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> object. SACLs that are protected do not inherit audit rules from parent containers.</summary>
		/// <param name="isProtected">
		///   <see langword="true" /> to protect the SACL from inheritance.</param>
		/// <param name="preserveInheritance">
		///   <see langword="true" /> to keep inherited audit rules in the SACL; <see langword="false" /> to remove inherited audit rules from the SACL.</param>
		public void SetSystemAclProtection(bool isProtected, bool preserveInheritance)
		{
			if (!isProtected)
			{
				this.flags &= ~ControlFlags.SystemAclProtected;
				return;
			}
			this.flags |= ControlFlags.SystemAclProtected;
			if (!preserveInheritance && this.SystemAcl != null)
			{
				this.SystemAcl.RemoveInheritedAces();
			}
		}

		/// <summary>Sets the <see cref="P:System.Security.AccessControl.CommonSecurityDescriptor.DiscretionaryAcl" /> property for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> instance and sets the <see cref="F:System.Security.AccessControl.ControlFlags.DiscretionaryAclPresent" /> flag.</summary>
		/// <param name="revision">The revision level of the new <see cref="T:System.Security.AccessControl.DiscretionaryAcl" /> object.</param>
		/// <param name="trusted">The number of Access Control Entries (ACEs) this <see cref="T:System.Security.AccessControl.DiscretionaryAcl" /> object can contain. This number is to be used only as a hint.</param>
		public void AddDiscretionaryAcl(byte revision, int trusted)
		{
			this.DiscretionaryAcl = new DiscretionaryAcl(this.IsContainer, this.IsDS, revision, trusted);
			this.flags |= ControlFlags.DiscretionaryAclPresent;
		}

		/// <summary>Sets the <see cref="P:System.Security.AccessControl.CommonSecurityDescriptor.SystemAcl" /> property for this <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> instance and sets the <see cref="F:System.Security.AccessControl.ControlFlags.SystemAclPresent" /> flag.</summary>
		/// <param name="revision">The revision level of the new <see cref="T:System.Security.AccessControl.SystemAcl" /> object.</param>
		/// <param name="trusted">The number of Access Control Entries (ACEs) this <see cref="T:System.Security.AccessControl.SystemAcl" /> object can contain. This number should only be used as a hint.</param>
		public void AddSystemAcl(byte revision, int trusted)
		{
			this.SystemAcl = new SystemAcl(this.IsContainer, this.IsDS, revision, trusted);
			this.flags |= ControlFlags.SystemAclPresent;
		}

		private void CheckAclConsistency(CommonAcl acl)
		{
			if (this.IsContainer != acl.IsContainer)
			{
				throw new ArgumentException("IsContainer must match between descriptor and ACL.");
			}
			if (this.IsDS != acl.IsDS)
			{
				throw new ArgumentException("IsDS must match between descriptor and ACL.");
			}
		}

		internal override bool DaclIsUnmodifiedAefa
		{
			get
			{
				return this.DiscretionaryAcl.IsAefa;
			}
		}

		private bool is_container;

		private bool is_ds;

		private ControlFlags flags;

		private SecurityIdentifier owner;

		private SecurityIdentifier group;

		private SystemAcl system_acl;

		private DiscretionaryAcl discretionary_acl;
	}
}
