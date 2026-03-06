using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace System.Security.AccessControl
{
	/// <summary>Provides the ability to control access to objects without direct manipulation of Access Control Lists (ACLs). This class is the abstract base class for the <see cref="T:System.Security.AccessControl.CommonObjectSecurity" /> and <see cref="T:System.Security.AccessControl.DirectoryObjectSecurity" /> classes.</summary>
	public abstract class ObjectSecurity
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.ObjectSecurity" /> class.</summary>
		protected ObjectSecurity()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.ObjectSecurity" /> class.</summary>
		/// <param name="securityDescriptor">The <see cref="T:System.Security.AccessControl.CommonSecurityDescriptor" /> of the new <see cref="T:System.Security.AccessControl.CommonObjectSecurity" /> instance.</param>
		protected ObjectSecurity(CommonSecurityDescriptor securityDescriptor)
		{
			if (securityDescriptor == null)
			{
				throw new ArgumentNullException("securityDescriptor");
			}
			this.descriptor = securityDescriptor;
			this.rw_lock = new ReaderWriterLock();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.ObjectSecurity" /> class.</summary>
		/// <param name="isContainer">
		///   <see langword="true" /> if the new <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a container object.</param>
		/// <param name="isDS">True if the new <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a directory object.</param>
		protected ObjectSecurity(bool isContainer, bool isDS) : this(new CommonSecurityDescriptor(isContainer, isDS, ControlFlags.None, null, null, null, new DiscretionaryAcl(isContainer, isDS, 0)))
		{
		}

		/// <summary>Gets the <see cref="T:System.Type" /> of the securable object associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <returns>The type of the securable object associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</returns>
		public abstract Type AccessRightType { get; }

		/// <summary>Gets the <see cref="T:System.Type" /> of the object associated with the access rules of this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object. The <see cref="T:System.Type" /> object must be an object that can be cast as a <see cref="T:System.Security.Principal.SecurityIdentifier" /> object.</summary>
		/// <returns>The type of the object associated with the access rules of this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</returns>
		public abstract Type AccessRuleType { get; }

		/// <summary>Gets the <see cref="T:System.Type" /> object associated with the audit rules of this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object. The <see cref="T:System.Type" /> object must be an object that can be cast as a <see cref="T:System.Security.Principal.SecurityIdentifier" /> object.</summary>
		/// <returns>The type of the object associated with the audit rules of this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</returns>
		public abstract Type AuditRuleType { get; }

		/// <summary>Gets a Boolean value that specifies whether the access rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object are in canonical order.</summary>
		/// <returns>
		///   <see langword="true" /> if the access rules are in canonical order; otherwise, <see langword="false" />.</returns>
		public bool AreAccessRulesCanonical
		{
			get
			{
				this.ReadLock();
				bool isDiscretionaryAclCanonical;
				try
				{
					isDiscretionaryAclCanonical = this.descriptor.IsDiscretionaryAclCanonical;
				}
				finally
				{
					this.ReadUnlock();
				}
				return isDiscretionaryAclCanonical;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is protected.</summary>
		/// <returns>
		///   <see langword="true" /> if the DACL is protected; otherwise, <see langword="false" />.</returns>
		public bool AreAccessRulesProtected
		{
			get
			{
				this.ReadLock();
				bool result;
				try
				{
					result = ((this.descriptor.ControlFlags & ControlFlags.DiscretionaryAclProtected) > ControlFlags.None);
				}
				finally
				{
					this.ReadUnlock();
				}
				return result;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the audit rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object are in canonical order.</summary>
		/// <returns>
		///   <see langword="true" /> if the audit rules are in canonical order; otherwise, <see langword="false" />.</returns>
		public bool AreAuditRulesCanonical
		{
			get
			{
				this.ReadLock();
				bool isSystemAclCanonical;
				try
				{
					isSystemAclCanonical = this.descriptor.IsSystemAclCanonical;
				}
				finally
				{
					this.ReadUnlock();
				}
				return isSystemAclCanonical;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is protected.</summary>
		/// <returns>
		///   <see langword="true" /> if the SACL is protected; otherwise, <see langword="false" />.</returns>
		public bool AreAuditRulesProtected
		{
			get
			{
				this.ReadLock();
				bool result;
				try
				{
					result = ((this.descriptor.ControlFlags & ControlFlags.SystemAclProtected) > ControlFlags.None);
				}
				finally
				{
					this.ReadUnlock();
				}
				return result;
			}
		}

		internal AccessControlSections AccessControlSectionsModified
		{
			get
			{
				this.Reading();
				return this.sections_modified;
			}
			set
			{
				this.Writing();
				this.sections_modified = value;
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether the access rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object have been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the access rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object have been modified; otherwise, <see langword="false" />.</returns>
		protected bool AccessRulesModified
		{
			get
			{
				return this.AreAccessControlSectionsModified(AccessControlSections.Access);
			}
			set
			{
				this.SetAccessControlSectionsModified(AccessControlSections.Access, value);
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether the audit rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object have been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the audit rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object have been modified; otherwise, <see langword="false" />.</returns>
		protected bool AuditRulesModified
		{
			get
			{
				return this.AreAccessControlSectionsModified(AccessControlSections.Audit);
			}
			set
			{
				this.SetAccessControlSectionsModified(AccessControlSections.Audit, value);
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether the group associated with the securable object has been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the group associated with the securable object has been modified; otherwise, <see langword="false" />.</returns>
		protected bool GroupModified
		{
			get
			{
				return this.AreAccessControlSectionsModified(AccessControlSections.Group);
			}
			set
			{
				this.SetAccessControlSectionsModified(AccessControlSections.Group, value);
			}
		}

		/// <summary>Gets a Boolean value that specifies whether this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a container object.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a container object; otherwise, <see langword="false" />.</returns>
		protected bool IsContainer
		{
			get
			{
				return this.descriptor.IsContainer;
			}
		}

		/// <summary>Gets a Boolean value that specifies whether this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a directory object.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object is a directory object; otherwise, <see langword="false" />.</returns>
		protected bool IsDS
		{
			get
			{
				return this.descriptor.IsDS;
			}
		}

		/// <summary>Gets or sets a Boolean value that specifies whether the owner of the securable object has been modified.</summary>
		/// <returns>
		///   <see langword="true" /> if the owner of the securable object has been modified; otherwise, <see langword="false" />.</returns>
		protected bool OwnerModified
		{
			get
			{
				return this.AreAccessControlSectionsModified(AccessControlSections.Owner);
			}
			set
			{
				this.SetAccessControlSectionsModified(AccessControlSections.Owner, value);
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AccessRule" /> class with the specified values.</summary>
		/// <param name="identityReference">The identity to which the access rule applies.  It must be an object that can be cast as a <see cref="T:System.Security.Principal.SecurityIdentifier" />.</param>
		/// <param name="accessMask">The access mask of this rule. The access mask is a 32-bit collection of anonymous bits, the meaning of which is defined by the individual integrators.</param>
		/// <param name="isInherited">true if this rule is inherited from a parent container.</param>
		/// <param name="inheritanceFlags">Specifies the inheritance properties of the access rule.</param>
		/// <param name="propagationFlags">Specifies whether inherited access rules are automatically propagated. The propagation flags are ignored if <paramref name="inheritanceFlags" /> is set to <see cref="F:System.Security.AccessControl.InheritanceFlags.None" />.</param>
		/// <param name="type">Specifies the valid access control type.</param>
		/// <returns>The <see cref="T:System.Security.AccessControl.AccessRule" /> object that this method creates.</returns>
		public abstract AccessRule AccessRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AccessControlType type);

		/// <summary>Initializes a new instance of the <see cref="T:System.Security.AccessControl.AuditRule" /> class with the specified values.</summary>
		/// <param name="identityReference">The identity to which the audit rule applies.  It must be an object that can be cast as a <see cref="T:System.Security.Principal.SecurityIdentifier" />.</param>
		/// <param name="accessMask">The access mask of this rule. The access mask is a 32-bit collection of anonymous bits, the meaning of which is defined by the individual integrators.</param>
		/// <param name="isInherited">
		///   <see langword="true" /> if this rule is inherited from a parent container.</param>
		/// <param name="inheritanceFlags">Specifies the inheritance properties of the audit rule.</param>
		/// <param name="propagationFlags">Specifies whether inherited audit rules are automatically propagated. The propagation flags are ignored if <paramref name="inheritanceFlags" /> is set to <see cref="F:System.Security.AccessControl.InheritanceFlags.None" />.</param>
		/// <param name="flags">Specifies the conditions for which the rule is audited.</param>
		/// <returns>The <see cref="T:System.Security.AccessControl.AuditRule" /> object that this method creates.</returns>
		public abstract AuditRule AuditRuleFactory(IdentityReference identityReference, int accessMask, bool isInherited, InheritanceFlags inheritanceFlags, PropagationFlags propagationFlags, AuditFlags flags);

		/// <summary>Gets the primary group associated with the specified owner.</summary>
		/// <param name="targetType">The owner for which to get the primary group.</param>
		/// <returns>The primary group associated with the specified owner.</returns>
		public IdentityReference GetGroup(Type targetType)
		{
			this.ReadLock();
			IdentityReference result;
			try
			{
				if (this.descriptor.Group == null)
				{
					result = null;
				}
				else
				{
					result = this.descriptor.Group.Translate(targetType);
				}
			}
			finally
			{
				this.ReadUnlock();
			}
			return result;
		}

		/// <summary>Gets the owner associated with the specified primary group.</summary>
		/// <param name="targetType">The primary group for which to get the owner.</param>
		/// <returns>The owner associated with the specified group.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="targetType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="targetType" /> is not an <see cref="T:System.Security.Principal.IdentityReference" /> type.</exception>
		/// <exception cref="T:System.Security.Principal.IdentityNotMappedException">Some or all identity references could not be translated.</exception>
		/// <exception cref="T:System.SystemException">A Win32 error code was returned.</exception>
		public IdentityReference GetOwner(Type targetType)
		{
			this.ReadLock();
			IdentityReference result;
			try
			{
				if (this.descriptor.Owner == null)
				{
					result = null;
				}
				else
				{
					result = this.descriptor.Owner.Translate(targetType);
				}
			}
			finally
			{
				this.ReadUnlock();
			}
			return result;
		}

		/// <summary>Returns an array of byte values that represents the security descriptor information for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <returns>An array of byte values that represents the security descriptor for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object. This method returns <see langword="null" /> if there is no security information in this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</returns>
		public byte[] GetSecurityDescriptorBinaryForm()
		{
			this.ReadLock();
			byte[] result;
			try
			{
				byte[] array = new byte[this.descriptor.BinaryLength];
				this.descriptor.GetBinaryForm(array, 0);
				result = array;
			}
			finally
			{
				this.ReadUnlock();
			}
			return result;
		}

		/// <summary>Returns the Security Descriptor Definition Language (SDDL) representation of the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="includeSections">Specifies which sections (access rules, audit rules, primary group, owner) of the security descriptor to get.</param>
		/// <returns>The SDDL representation of the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</returns>
		public string GetSecurityDescriptorSddlForm(AccessControlSections includeSections)
		{
			this.ReadLock();
			string sddlForm;
			try
			{
				sddlForm = this.descriptor.GetSddlForm(includeSections);
			}
			finally
			{
				this.ReadUnlock();
			}
			return sddlForm;
		}

		/// <summary>Returns a Boolean value that specifies whether the security descriptor associated with this  <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object can be converted to the Security Descriptor Definition Language (SDDL) format.</summary>
		/// <returns>
		///   <see langword="true" /> if the security descriptor associated with this  <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object can be converted to the Security Descriptor Definition Language (SDDL) format; otherwise, <see langword="false" />.</returns>
		public static bool IsSddlConversionSupported()
		{
			return GenericSecurityDescriptor.IsSddlConversionSupported();
		}

		/// <summary>Applies the specified modification to the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="modification">The modification to apply to the DACL.</param>
		/// <param name="rule">The access rule to modify.</param>
		/// <param name="modified">
		///   <see langword="true" /> if the DACL is successfully modified; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the DACL is successfully modified; otherwise, <see langword="false" />.</returns>
		public virtual bool ModifyAccessRule(AccessControlModification modification, AccessRule rule, out bool modified)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (!this.AccessRuleType.IsAssignableFrom(rule.GetType()))
			{
				throw new ArgumentException("rule");
			}
			return this.ModifyAccess(modification, rule, out modified);
		}

		/// <summary>Applies the specified modification to the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="modification">The modification to apply to the SACL.</param>
		/// <param name="rule">The audit rule to modify.</param>
		/// <param name="modified">
		///   <see langword="true" /> if the SACL is successfully modified; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the SACL is successfully modified; otherwise, <see langword="false" />.</returns>
		public virtual bool ModifyAuditRule(AccessControlModification modification, AuditRule rule, out bool modified)
		{
			if (rule == null)
			{
				throw new ArgumentNullException("rule");
			}
			if (!this.AuditRuleType.IsAssignableFrom(rule.GetType()))
			{
				throw new ArgumentException("rule");
			}
			return this.ModifyAudit(modification, rule, out modified);
		}

		/// <summary>Removes all access rules associated with the specified <see cref="T:System.Security.Principal.IdentityReference" />.</summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IdentityReference" /> for which to remove all access rules.</param>
		/// <exception cref="T:System.InvalidOperationException">All access rules are not in canonical order.</exception>
		public virtual void PurgeAccessRules(IdentityReference identity)
		{
			if (null == identity)
			{
				throw new ArgumentNullException("identity");
			}
			this.WriteLock();
			try
			{
				this.descriptor.PurgeAccessControl(ObjectSecurity.SidFromIR(identity));
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Removes all audit rules associated with the specified <see cref="T:System.Security.Principal.IdentityReference" />.</summary>
		/// <param name="identity">The <see cref="T:System.Security.Principal.IdentityReference" /> for which to remove all audit rules.</param>
		/// <exception cref="T:System.InvalidOperationException">All audit rules are not in canonical order.</exception>
		public virtual void PurgeAuditRules(IdentityReference identity)
		{
			if (null == identity)
			{
				throw new ArgumentNullException("identity");
			}
			this.WriteLock();
			try
			{
				this.descriptor.PurgeAudit(ObjectSecurity.SidFromIR(identity));
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Sets or removes protection of the access rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object. Protected access rules cannot be modified by parent objects through inheritance.</summary>
		/// <param name="isProtected">
		///   <see langword="true" /> to protect the access rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from inheritance; <see langword="false" /> to allow inheritance.</param>
		/// <param name="preserveInheritance">
		///   <see langword="true" /> to preserve inherited access rules; <see langword="false" /> to remove inherited access rules. This parameter is ignored if <paramref name="isProtected" /> is <see langword="false" />.</param>
		/// <exception cref="T:System.InvalidOperationException">This method attempts to remove inherited rules from a non-canonical Discretionary Access Control List (DACL).</exception>
		public void SetAccessRuleProtection(bool isProtected, bool preserveInheritance)
		{
			this.WriteLock();
			try
			{
				this.descriptor.SetDiscretionaryAclProtection(isProtected, preserveInheritance);
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Sets or removes protection of the audit rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object. Protected audit rules cannot be modified by parent objects through inheritance.</summary>
		/// <param name="isProtected">
		///   <see langword="true" /> to protect the audit rules associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from inheritance; <see langword="false" /> to allow inheritance.</param>
		/// <param name="preserveInheritance">
		///   <see langword="true" /> to preserve inherited audit rules; <see langword="false" /> to remove inherited audit rules. This parameter is ignored if <paramref name="isProtected" /> is <see langword="false" />.</param>
		/// <exception cref="T:System.InvalidOperationException">This method attempts to remove inherited rules from a non-canonical System Access Control List (SACL).</exception>
		public void SetAuditRuleProtection(bool isProtected, bool preserveInheritance)
		{
			this.WriteLock();
			try
			{
				this.descriptor.SetSystemAclProtection(isProtected, preserveInheritance);
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Sets the primary group for the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="identity">The primary group to set.</param>
		public void SetGroup(IdentityReference identity)
		{
			this.WriteLock();
			try
			{
				this.descriptor.Group = ObjectSecurity.SidFromIR(identity);
				this.GroupModified = true;
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Sets the owner for the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="identity">The owner to set.</param>
		public void SetOwner(IdentityReference identity)
		{
			this.WriteLock();
			try
			{
				this.descriptor.Owner = ObjectSecurity.SidFromIR(identity);
				this.OwnerModified = true;
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Sets the security descriptor for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from the specified array of byte values.</summary>
		/// <param name="binaryForm">The array of bytes from which to set the security descriptor.</param>
		public void SetSecurityDescriptorBinaryForm(byte[] binaryForm)
		{
			this.SetSecurityDescriptorBinaryForm(binaryForm, AccessControlSections.All);
		}

		/// <summary>Sets the specified sections of the security descriptor for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from the specified array of byte values.</summary>
		/// <param name="binaryForm">The array of bytes from which to set the security descriptor.</param>
		/// <param name="includeSections">The sections (access rules, audit rules, owner, primary group) of the security descriptor to set.</param>
		public void SetSecurityDescriptorBinaryForm(byte[] binaryForm, AccessControlSections includeSections)
		{
			this.CopySddlForm(new CommonSecurityDescriptor(this.IsContainer, this.IsDS, binaryForm, 0), includeSections);
		}

		/// <summary>Sets the security descriptor for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from the specified Security Descriptor Definition Language (SDDL) string.</summary>
		/// <param name="sddlForm">The SDDL string from which to set the security descriptor.</param>
		public void SetSecurityDescriptorSddlForm(string sddlForm)
		{
			this.SetSecurityDescriptorSddlForm(sddlForm, AccessControlSections.All);
		}

		/// <summary>Sets the specified sections of the security descriptor for this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object from the specified Security Descriptor Definition Language (SDDL) string.</summary>
		/// <param name="sddlForm">The SDDL string from which to set the security descriptor.</param>
		/// <param name="includeSections">The sections (access rules, audit rules, owner, primary group) of the security descriptor to set.</param>
		public void SetSecurityDescriptorSddlForm(string sddlForm, AccessControlSections includeSections)
		{
			this.CopySddlForm(new CommonSecurityDescriptor(this.IsContainer, this.IsDS, sddlForm), includeSections);
		}

		private void CopySddlForm(CommonSecurityDescriptor sourceDescriptor, AccessControlSections includeSections)
		{
			this.WriteLock();
			try
			{
				this.AccessControlSectionsModified |= includeSections;
				if ((includeSections & AccessControlSections.Audit) != AccessControlSections.None)
				{
					this.descriptor.SystemAcl = sourceDescriptor.SystemAcl;
				}
				if ((includeSections & AccessControlSections.Access) != AccessControlSections.None)
				{
					this.descriptor.DiscretionaryAcl = sourceDescriptor.DiscretionaryAcl;
				}
				if ((includeSections & AccessControlSections.Owner) != AccessControlSections.None)
				{
					this.descriptor.Owner = sourceDescriptor.Owner;
				}
				if ((includeSections & AccessControlSections.Group) != AccessControlSections.None)
				{
					this.descriptor.Group = sourceDescriptor.Group;
				}
			}
			finally
			{
				this.WriteUnlock();
			}
		}

		/// <summary>Applies the specified modification to the Discretionary Access Control List (DACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="modification">The modification to apply to the DACL.</param>
		/// <param name="rule">The access rule to modify.</param>
		/// <param name="modified">
		///   <see langword="true" /> if the DACL is successfully modified; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the DACL is successfully modified; otherwise, <see langword="false" />.</returns>
		protected abstract bool ModifyAccess(AccessControlModification modification, AccessRule rule, out bool modified);

		/// <summary>Applies the specified modification to the System Access Control List (SACL) associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object.</summary>
		/// <param name="modification">The modification to apply to the SACL.</param>
		/// <param name="rule">The audit rule to modify.</param>
		/// <param name="modified">
		///   <see langword="true" /> if the SACL is successfully modified; otherwise, <see langword="false" />.</param>
		/// <returns>
		///   <see langword="true" /> if the SACL is successfully modified; otherwise, <see langword="false" />.</returns>
		protected abstract bool ModifyAudit(AccessControlModification modification, AuditRule rule, out bool modified);

		private Exception GetNotImplementedException()
		{
			return new NotImplementedException();
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="handle">The handle used to retrieve the persisted information.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		protected virtual void Persist(SafeHandle handle, AccessControlSections includeSections)
		{
			throw this.GetNotImplementedException();
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="name">The name used to retrieve the persisted information.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		protected virtual void Persist(string name, AccessControlSections includeSections)
		{
			throw this.GetNotImplementedException();
		}

		/// <summary>Saves the specified sections of the security descriptor associated with this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object to permanent storage. We recommend that the values of the <paramref name="includeSections" /> parameters passed to the constructor and persist methods be identical.</summary>
		/// <param name="enableOwnershipPrivilege">
		///   <see langword="true" /> to enable the privilege that allows the caller to take ownership of the object.</param>
		/// <param name="name">The name used to retrieve the persisted information.</param>
		/// <param name="includeSections">One of the <see cref="T:System.Security.AccessControl.AccessControlSections" /> enumeration values that specifies the sections of the security descriptor (access rules, audit rules, owner, primary group) of the securable object to save.</param>
		[MonoTODO]
		[HandleProcessCorruptedStateExceptions]
		protected virtual void Persist(bool enableOwnershipPrivilege, string name, AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		private void Reading()
		{
			if (!this.rw_lock.IsReaderLockHeld && !this.rw_lock.IsWriterLockHeld)
			{
				throw new InvalidOperationException("Either a read or a write lock must be held.");
			}
		}

		/// <summary>Locks this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object for read access.</summary>
		protected void ReadLock()
		{
			this.rw_lock.AcquireReaderLock(-1);
		}

		/// <summary>Unlocks this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object for read access.</summary>
		protected void ReadUnlock()
		{
			this.rw_lock.ReleaseReaderLock();
		}

		private void Writing()
		{
			if (!this.rw_lock.IsWriterLockHeld)
			{
				throw new InvalidOperationException("Write lock must be held.");
			}
		}

		/// <summary>Locks this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object for write access.</summary>
		protected void WriteLock()
		{
			this.rw_lock.AcquireWriterLock(-1);
		}

		/// <summary>Unlocks this <see cref="T:System.Security.AccessControl.ObjectSecurity" /> object for write access.</summary>
		protected void WriteUnlock()
		{
			this.rw_lock.ReleaseWriterLock();
		}

		internal AuthorizationRuleCollection InternalGetAccessRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			List<AuthorizationRule> list = new List<AuthorizationRule>();
			this.ReadLock();
			try
			{
				foreach (GenericAce genericAce in this.descriptor.DiscretionaryAcl)
				{
					QualifiedAce qualifiedAce = genericAce as QualifiedAce;
					if (!(null == qualifiedAce) && (!qualifiedAce.IsInherited || includeInherited) && (qualifiedAce.IsInherited || includeExplicit))
					{
						AccessControlType type;
						if (qualifiedAce.AceQualifier == AceQualifier.AccessAllowed)
						{
							type = AccessControlType.Allow;
						}
						else
						{
							if (AceQualifier.AccessDenied != qualifiedAce.AceQualifier)
							{
								continue;
							}
							type = AccessControlType.Deny;
						}
						AccessRule item = this.InternalAccessRuleFactory(qualifiedAce, targetType, type);
						list.Add(item);
					}
				}
			}
			finally
			{
				this.ReadUnlock();
			}
			return new AuthorizationRuleCollection(list.ToArray());
		}

		internal virtual AccessRule InternalAccessRuleFactory(QualifiedAce ace, Type targetType, AccessControlType type)
		{
			return this.AccessRuleFactory(ace.SecurityIdentifier.Translate(targetType), ace.AccessMask, ace.IsInherited, ace.InheritanceFlags, ace.PropagationFlags, type);
		}

		internal AuthorizationRuleCollection InternalGetAuditRules(bool includeExplicit, bool includeInherited, Type targetType)
		{
			List<AuthorizationRule> list = new List<AuthorizationRule>();
			this.ReadLock();
			try
			{
				if (this.descriptor.SystemAcl != null)
				{
					foreach (GenericAce genericAce in this.descriptor.SystemAcl)
					{
						QualifiedAce qualifiedAce = genericAce as QualifiedAce;
						if (!(null == qualifiedAce) && (!qualifiedAce.IsInherited || includeInherited) && (qualifiedAce.IsInherited || includeExplicit) && AceQualifier.SystemAudit == qualifiedAce.AceQualifier)
						{
							AuditRule item = this.InternalAuditRuleFactory(qualifiedAce, targetType);
							list.Add(item);
						}
					}
				}
			}
			finally
			{
				this.ReadUnlock();
			}
			return new AuthorizationRuleCollection(list.ToArray());
		}

		internal virtual AuditRule InternalAuditRuleFactory(QualifiedAce ace, Type targetType)
		{
			return this.AuditRuleFactory(ace.SecurityIdentifier.Translate(targetType), ace.AccessMask, ace.IsInherited, ace.InheritanceFlags, ace.PropagationFlags, ace.AuditFlags);
		}

		internal static SecurityIdentifier SidFromIR(IdentityReference identity)
		{
			if (null == identity)
			{
				throw new ArgumentNullException("identity");
			}
			return (SecurityIdentifier)identity.Translate(typeof(SecurityIdentifier));
		}

		private bool AreAccessControlSectionsModified(AccessControlSections mask)
		{
			return (this.AccessControlSectionsModified & mask) > AccessControlSections.None;
		}

		private void SetAccessControlSectionsModified(AccessControlSections mask, bool modified)
		{
			if (modified)
			{
				this.AccessControlSectionsModified |= mask;
				return;
			}
			this.AccessControlSectionsModified &= ~mask;
		}

		internal CommonSecurityDescriptor descriptor;

		private AccessControlSections sections_modified;

		private ReaderWriterLock rw_lock;
	}
}
